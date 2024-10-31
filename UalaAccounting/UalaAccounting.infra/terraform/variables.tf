###################################################################################
# variables.tf
###################################################################################

variable "appname" {
  description = "Application name for tagging resources"
  type        = string
  default     = "accounting hub"
}
variable "environment" {
  description = "Stage name for tagging resources"
  type        = string
  default     = "dev"
}
variable "region" {
  description = "AWS region"
  type        = string
  default     = "us-east-1"
}
variable "container_port" {
  description = "Container port"
  type        = number
  default     = 8080
}
variable "image_url" {
  description = "Docker image url hosted on ECR"
  type        = string
  default     = ""
}
variable "vpc_id" {
  description = "VPC ID"
  type        = string
  default     = "vpc-00465b6029aa62142"
}
variable "public_subnets" {
  description = "Public subnets"
  type        = list(string)
  default     = ["subnet-0f6b374232907413f", "subnet-041ca93a6bc2db135"]
}
variable "private_subnets" {
  description = "Private subnets"
  type        = list(string)
  default     = ["subnet-004c23dd3f21601b2", "subnet-05668d3946b1d4b33"]
}
variable "task_role_arn" {
  description = "ECS role"
  type        = string
  default     = "arn:aws:iam::996158444070:role/ecsTaskExecutionRole"
}
variable "rds_monitor_role_arn" {
  description = "RDS monitoring role"
  type        = string
  default     = "arn:aws:iam::996158444070:role/rdsMonitoringRole"
}
variable "lambda_role_arn" {
  description = "Lambda role"
  type        = string
  default     = "arn:aws:iam::996158444070:role/service-role/myLambdaRole"
}
variable "sg_ecs_service" {
  description = "ECS security group"
  type        = string
  default     = "sg-0d7857c711fc0fca7"
}
variable "sg_lb" {
  description = "LB security group"
  type        = string
  default     = "sg-0b9f385632dc5c6bd"
}
variable "sg_rds" {
  description = "RDS security group"
  type        = string
  default     = "sg-00a3c2da669a382f5"
}
variable "db_username" {
  description = "MySQL RDS username"
  type        = string
  default     = "root"
}
variable "mambu_url_base" {
  description = "Mambu URL base"
  type        = string
  default     = "https://ualamxdev.sandbox.mambu.com"
}
variable "mambu_apikey" {
  description = "Mambu apikey"
  type        = string
  default     = ""
}
variable "acm_certificate_arn" {
  description = "ACM certificate ARN"
  type        = string
  default     = "arn:aws:acm:us-east-1:996158444070:certificate/2c5188c5-e741-4705-8ae9-ac50089e49e1"
}
variable "hosted_zone_id" {
  description = "ACM hosted zone id"
  type        = string
  default     = "Z02785561U7BN9ZZRI601"
}
variable "domain_name" {
  description = "Route53 Domain name for public access to LB"
  type        = string
  default     = "ah.cenponline.net"
}