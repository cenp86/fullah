###################################################################################
# main.tf
###################################################################################

###################################################################################
# AWS as Provider
###################################################################################

# provider "aws" {
#   region = var.region
#   shared_credentials_files = ["~/.aws/credentials"]
# }

###################################################################################
# Terraform backend
###################################################################################

terraform {
  backend "s3" {
    bucket = "carlosn-bucket"
    key    = "my-terraform-project"
    region = "us-east-1"
    # shared_credentials_file = "~/.aws/credentials"
  }
}

##########################################################################################
# RDS Database Instance
##########################################################################################

#create DB subnet group
resource "aws_db_subnet_group" "db_subnets" {
  name       = "accounting-hub-db-subnet-${var.environment}"
  subnet_ids = var.private_subnets

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}

#create a random password
resource "random_password" "master" {
  length           = 32
  special          = true
  override_special = "_!%^"
}

#create a RDS Database Instance
resource "aws_db_instance" "rds_instance" {
  engine                 = "mysql"
  identifier             = "accounting-hub-rds-${var.environment}"
  allocated_storage      = 50
  engine_version         = "8.0.35"
  instance_class         = "db.t4g.medium"
  db_name                = "conta"
  username               = var.db_username
  password               = random_password.master.result
  vpc_security_group_ids = [var.sg_rds]
  skip_final_snapshot    = true
  publicly_accessible    = false
  db_subnet_group_name   = aws_db_subnet_group.db_subnets.name

  # Enable enhanced monitoring
  monitoring_interval = 60 # Interval in seconds (minimum 60 seconds)
  monitoring_role_arn = var.rds_monitor_role_arn

  # Enable performance insights
  performance_insights_enabled = true

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}


##########################################################################################
# Secrets manager
##########################################################################################

resource "aws_secretsmanager_secret" "credentials" {
  name                    = "accounting-hub-credentials-${var.environment}"
  description             = "Accounting Hub secrets"
  recovery_window_in_days = 0

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}

resource "aws_secretsmanager_secret_version" "credentials" {
  secret_id = aws_secretsmanager_secret.credentials.id
  secret_string = jsonencode(
    {
      "db_password" : "${random_password.master.result}",
      "connection_string" : "server=${aws_db_instance.rds_instance.address};database=conta;user=${var.db_username};password=${random_password.master.result}",
      "mambu_apikey" : var.mambu_apikey
    }
  )
}

resource "aws_secretsmanager_secret_policy" "credentials" {
  secret_arn = aws_secretsmanager_secret.credentials.arn

  policy = jsonencode({
    "Version" : "2012-10-17",
    "Statement" : [{
      "Sid" : "AllowAccessToSecret",
      "Effect" : "Allow",
      "Principal" : {
        "AWS" : [var.lambda_role_arn, var.task_role_arn]
      },
      "Action" : ["secretsmanager:GetSecretValue", "secretsmanager:ListSecrets"],
      "Resource" : "*"
    }]
  })
}


##########################################################################################
# Cloudwatch logs
##########################################################################################
resource "aws_cloudwatch_log_group" "ecs" {
  name = "/ecs/accounting-hub-${var.environment}"

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}


#################################################################################################
# Load Balancer resources: LB, LB target group, LB listener
#################################################################################################
resource "aws_lb" "load_balancer" {
  name               = "accounting-hub-lb-${var.environment}"
  internal           = true
  load_balancer_type = "network"
  subnets            = var.private_subnets
  security_groups    = [var.sg_lb]
  idle_timeout       = 1200

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}

#Defining the target group and a health check on the application
resource "aws_lb_target_group" "target_group" {
  name        = "accounting-hub-tg-${var.environment}"
  port        = var.container_port
  protocol    = "TCP"
  target_type = "ip"
  vpc_id      = var.vpc_id
  health_check {
    # path                = "/health"
    protocol = "TCP"
    # matcher             = "200"
    # port                = "traffic-port"
    healthy_threshold   = 2
    unhealthy_threshold = 2
    timeout             = 10
    interval            = 30
  }
  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}

#Defines an HTTP Listener for the LB
resource "aws_lb_listener" "listener_http" {
  load_balancer_arn = aws_lb.load_balancer.arn
  port              = "80"
  protocol          = "TCP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.arn
  }
}

#Defines an HTTPS Listener for the LB
resource "aws_lb_listener" "listener_https" {
  load_balancer_arn = aws_lb.load_balancer.arn
  port              = "443"
  protocol          = "TLS"
  ssl_policy        = "ELBSecurityPolicy-TLS13-1-2-2021-06"
  certificate_arn   = var.acm_certificate_arn

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.target_group.arn
  }
}

#Route 53 record for https access to LB
resource "aws_route53_record" "node" {
  zone_id = var.hosted_zone_id
  name    = var.domain_name
  type    = "A"
  alias {
    name                   = aws_lb.load_balancer.dns_name
    zone_id                = aws_lb.load_balancer.zone_id
    evaluate_target_health = true
  }
}


##########################################################################################
# ECS resources: ECS cluster, ECS task definition, ECS service
##########################################################################################
#ECS cluster
resource "aws_ecs_cluster" "ecs_cluster" {
  name = "accounting-hub-ecs-cluster-${var.environment}"
  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}
#The Task Definition used in conjunction with the ECS service
resource "aws_ecs_task_definition" "task_definition" {
  family = "accounting-hub-td-${var.environment}"
  container_definitions = jsonencode(
    [
      {
        "name" : "accounting-hub-container-${var.environment}",
        "image" : var.image_url,
        "cpu" : 2048,
        "memory" : 8192,
        "entryPoint" : []
        "essential" : true,
        "networkMode" : "awsvpc",
        "portMappings" : [
          {
            "containerPort" : var.container_port
            "hostPort" : var.container_port
          }
        ]
        "linuxParameters" : {
          "initProcessEnabled" : true
        }
        "healthCheck" : {
          "command" : ["CMD-SHELL", "curl -f http://localhost:8080/health || exit 1"],
          "interval" : 30,
          "timeout" : 5,
          "startPeriod" : 10,
          "retries" : 3
        }
        secrets : [
          { "name" : "ACHUB", "valueFrom" : "${aws_secretsmanager_secret_version.credentials.arn}:connection_string::" },
          { "name" : "MAMBU_APIKEY", "valueFrom" : "${aws_secretsmanager_secret_version.credentials.arn}:mambu_apikey::" }
        ],
        environment = [
          {
            name  = "AWS_S3_BUCKET"
            value = "${aws_s3_bucket.s3_bucket.id}"
          },
          {
            name  = "AWS_S3_FOLDER"
            value = "backups/"
          }
        ]
        "logConfiguration" : {
          "logDriver" : "awslogs",
          "options" : {
            "awslogs-group" : aws_cloudwatch_log_group.ecs.name,
            "awslogs-region" : var.region,
            "awslogs-stream-prefix" : "ecs"
          }
        }
      }
    ]
  )
  #Fargate is used as opposed to EC2, so we do not need to manage the EC2 instances. Fargate is serveless
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = "2048"
  memory                   = "8192"
  execution_role_arn       = var.task_role_arn
  task_role_arn            = var.task_role_arn
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "ARM64"
  }
  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}
#The ECS service described. This resources allows you to manage tasks
resource "aws_ecs_service" "ecs_service" {
  name                = "accounting-hub-ecs-service-${var.environment}"
  cluster             = aws_ecs_cluster.ecs_cluster.arn
  task_definition     = aws_ecs_task_definition.task_definition.arn
  launch_type         = "FARGATE"
  scheduling_strategy = "REPLICA"
  desired_count       = 1 # the number of tasks you wish to run
  network_configuration {
    subnets          = var.private_subnets
    assign_public_ip = false
    security_groups  = [var.sg_ecs_service]
  }
  # This block registers the tasks to a target group of the loadbalancer.
  load_balancer {
    target_group_arn = aws_lb_target_group.target_group.arn
    container_name   = "accounting-hub-container-${var.environment}"
    container_port   = var.container_port
  }
  depends_on = [aws_lb_listener.listener_http, aws_lb_listener.listener_https]
  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}


##########################################################################################
# S3 bucket
##########################################################################################

resource "aws_s3_bucket" "s3_bucket" {
  bucket = "accounting-hub-s3-bucket2-${var.environment}"

  tags = {
    Application = var.appname
    Stage       = var.environment
  }
}

resource "aws_s3_object" "s3_folder" {
  bucket       = aws_s3_bucket.s3_bucket.id
  key          = "backups/"
  content_type = "application/x-directory"
}

resource "aws_s3_bucket_policy" "bucket_policy" {
  bucket = aws_s3_bucket.s3_bucket.id

  policy = jsonencode({
    "Version" : "2012-10-17",
    "Statement" : [
      {
        "Effect" : "Allow",
        "Principal" : {
          "AWS" : var.task_role_arn
        },
        "Action" : ["s3:*"],
        "Resource" : [
          "${aws_s3_bucket.s3_bucket.arn}",
          "${aws_s3_bucket.s3_bucket.arn}/*"
        ]
      }
    ]
  })
}

##########################################################################################
# Locals
##########################################################################################

locals {
  function_name               = "uala_stage3_steps"
  function_handler            = "lambda_function.lambda_handler"
  function_runtime            = "python3.12"
  function_timeout_in_seconds = 120

  function_source_dir = "${path.module}/aws_lambda_functions/${local.function_name}"
}


##########################################################################################
# Lambda
##########################################################################################


resource "aws_lambda_function" "state_change_lambda" {
  function_name = "${local.function_name}-${var.environment}"
  handler       = local.function_handler
  runtime       = local.function_runtime
  timeout       = local.function_timeout_in_seconds

  filename         = "${local.function_source_dir}.zip"
  source_code_hash = data.archive_file.function_zip.output_base64sha256

  role = var.lambda_role_arn

  environment {
    variables = {
      SECRETS_ARN = aws_secretsmanager_secret.credentials.arn
    }
  }
}

data "archive_file" "function_zip" {
  source_dir  = local.function_source_dir
  type        = "zip"
  output_path = "${local.function_source_dir}.zip"
}


##########################################################################################
# API Gateway
##########################################################################################

resource "aws_api_gateway_rest_api" "ah_apigw" {
  name = "Uala Accounting Hub API GW"

  endpoint_configuration {
    types = ["REGIONAL"]
  }

  depends_on = [aws_lb_listener.listener_http, aws_lb_listener.listener_https]

  tags = {
    Application = "Accounting Hub"
    Stage       = "dev"
  }
}
resource "aws_api_gateway_deployment" "api_gateway_deploy" {
  rest_api_id = aws_api_gateway_rest_api.ah_apigw.id
  stage_name  = var.environment

  variables = {
    "vpc_link_id" = aws_api_gateway_vpc_link.vpc_link.id
  }

  depends_on = [
    aws_api_gateway_vpc_link.vpc_link,
    aws_api_gateway_integration.stage_change_lambda_integration,
    aws_api_gateway_integration.orchestrate_ah_integration,
    aws_api_gateway_integration.status_orchestrate_ah_integration
  ]

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_vpc_link" "vpc_link" {
  name = "accounting-hub-vpc-link-${var.environment}"

  target_arns = [aws_lb.load_balancer.arn]

  depends_on = [aws_lb_listener.listener_http, aws_lb_listener.listener_https]

  tags = {
    Application = "Accounting Hub"
    Stage       = "dev"
  }
}

resource "aws_api_gateway_resource" "stage_change_lambda_resource" {
  rest_api_id = aws_api_gateway_rest_api.ah_apigw.id
  parent_id   = aws_api_gateway_rest_api.ah_apigw.root_resource_id
  path_part   = "stagechange"
}

resource "aws_api_gateway_method" "state_change_lambda_method" {
  rest_api_id   = aws_api_gateway_rest_api.ah_apigw.id
  resource_id   = aws_api_gateway_resource.stage_change_lambda_resource.id
  http_method   = "POST"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "stage_change_lambda_integration" {
  rest_api_id             = aws_api_gateway_rest_api.ah_apigw.id
  resource_id             = aws_api_gateway_resource.stage_change_lambda_resource.id
  http_method             = aws_api_gateway_method.state_change_lambda_method.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = aws_lambda_function.state_change_lambda.invoke_arn
}

resource "aws_api_gateway_resource" "orchestrate_ah_resource" {
  rest_api_id = aws_api_gateway_rest_api.ah_apigw.id
  parent_id   = aws_api_gateway_rest_api.ah_apigw.root_resource_id
  path_part   = "orchestrate"
}

resource "aws_api_gateway_method" "orchestrate_ah_method" {
  rest_api_id   = aws_api_gateway_rest_api.ah_apigw.id
  resource_id   = aws_api_gateway_resource.orchestrate_ah_resource.id
  http_method   = "POST"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "orchestrate_ah_integration" {
  rest_api_id             = aws_api_gateway_rest_api.ah_apigw.id
  resource_id             = aws_api_gateway_resource.orchestrate_ah_resource.id
  http_method             = aws_api_gateway_method.orchestrate_ah_method.http_method
  integration_http_method = "POST"
  type                    = "HTTP_PROXY"
  uri                     = "http://${aws_lb.load_balancer.dns_name}/api/Execute/orchestrate"

  connection_type = "VPC_LINK"
  connection_id   = aws_api_gateway_vpc_link.vpc_link.id
}

resource "aws_api_gateway_resource" "status_orchestrate_ah_resource" {
  rest_api_id = aws_api_gateway_rest_api.ah_apigw.id
  parent_id   = aws_api_gateway_rest_api.ah_apigw.root_resource_id
  path_part   = "statusorchestrate"
}

resource "aws_api_gateway_method" "status_orchestrate_ah_method" {
  rest_api_id   = aws_api_gateway_rest_api.ah_apigw.id
  resource_id   = aws_api_gateway_resource.status_orchestrate_ah_resource.id
  http_method   = "POST"
  authorization = "NONE"
}

resource "aws_api_gateway_integration" "status_orchestrate_ah_integration" {
  rest_api_id             = aws_api_gateway_rest_api.ah_apigw.id
  resource_id             = aws_api_gateway_resource.status_orchestrate_ah_resource.id
  http_method             = aws_api_gateway_method.status_orchestrate_ah_method.http_method
  integration_http_method = "POST"
  type                    = "HTTP_PROXY"
  uri                     = "http://${aws_lb.load_balancer.dns_name}/api/Execute/statusorchestrate"

  connection_type = "VPC_LINK"
  connection_id   = aws_api_gateway_vpc_link.vpc_link.id
}

resource "aws_lambda_permission" "allow_api_gateway" {
  statement_id  = "AllowAPIGateway"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.state_change_lambda.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.ah_apigw.execution_arn}/*/*"
}
