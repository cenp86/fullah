###################################################################################
# output.tf
###################################################################################

#ARNs
output "aws_secretsmanager_secret_arn" {
  value = aws_secretsmanager_secret.credentials.arn
}
output "aws_s3_bucket_arn" {
  value = aws_s3_bucket.s3_bucket.arn
}
output "aws_lb_arn" {
  value = aws_lb.load_balancer.arn
}
output "aws_rds_instance_arn" {
  value = aws_db_instance.rds_instance.id
}
output "aws_ecs_cluster_arn" {
  value = aws_ecs_cluster.ecs_cluster.arn
}
output "aws_ecs_service_id" {
  value = aws_ecs_service.ecs_service.id
}
output "aws_ecs_task_definition_arn" {
  value = aws_ecs_task_definition.task_definition.arn
}


#ENDPOINTS
output "aws_lb_dns_name" {
  value = aws_lb.load_balancer.dns_name
}
output "aws_rds_endpoint" {
  value = aws_db_instance.rds_instance.endpoint
}
output "api_gateway_accountinghub_url" {
  value       = "https://${aws_api_gateway_rest_api.ah_apigw.id}.execute-api.${var.region}.amazonaws.com/${var.environment}/accountinghub"
  description = "The base URL of the API Gateway for the accounting hub resource"
}
output "api_gateway_statechange_url" {
  value       = "https://${aws_api_gateway_rest_api.ah_apigw.id}.execute-api.${var.region}.amazonaws.com/${var.environment}/statechange"
  description = "The base URL of the API Gateway for the state change lambda resource"
}