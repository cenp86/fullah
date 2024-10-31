import os
import json
import urllib3
from datetime import datetime
import boto3

#Retrieve environment variables
MAMBU_URL_BASE = os.environ.get('MAMBU_URL_BASE')
SECRETS_ARN = os.environ.get('SECRETS_ARN')

# Retrieve the secrets from Secrets Manager
secrets_client = boto3.client('secretsmanager')
secrets = json.loads(secrets_client.get_secret_value(SecretId=SECRETS_ARN).get('SecretString'))

# HTTP headers used in the requests
HEADERS = {
    "Accept": "application/vnd.mambu.v2+json",
    "Content-Type": "application/json",
    "apikey": secrets['mambu_apikey']
}

# Account states valid for this process
ACCOUNTSTATES = ["ACTIVE","ACTIVE_IN_ARREARS"]

# HTTP connection pool manager
http = urllib3.PoolManager()

def lambda_handler(event, context):
     
    # Obtain the HTTP method
    http_method = event.get('httpMethod', '')

    # Handle POST method
    if http_method == 'POST':
        try:
            # Check if event['body'] is a string or dict
            if isinstance(event['body'], str):
                request_body = json.loads(event['body'])
            else:
                request_body = event['body']
        except (json.JSONDecodeError, KeyError):
            return response(400, {'message': 'Invalid or missing JSON body'})

        #Check if environment variables were passed correctly
        if not MAMBU_URL_BASE and not SECRETS_ARN:
            return response(400, {'message': 'Variables MAMBU_URL_BASE and SECRETS_ARN missing.'})
        
        # Extract body variables
        loanAccountId = request_body.get("accountId")
        dateTime = request_body.get("dateTime")
        branchKey = request_body.get("branchAddress")

        if not all([loanAccountId, dateTime, branchKey]):
            return response(400, {'message': 'Missing required fields accountId, dateTime and branchAddress'})
        
        #Get Account details and parse relevant fields
        data, status_code = get_account_details(loanAccountId)
        dataJson = json.loads(data);
        accountState = dataJson.get("accountState")
        accruedInterest = dataJson.get("accruedInterest")
        currentStage = dataJson.get("assignedBranchKey") 
        
        if 200 <= status_code <= 299 and accountState in ACCOUNTSTATES and accruedInterest > 0 and currentStage != branchKey:
            # Apply interest
            data, status_code = apply_interest(loanAccountId, dateTime)
    
            if 200 <= status_code <= 299:
                # Update fields if interest application was successful
                data, status_code = update_fields(loanAccountId, dateTime, branchKey)    
        
        if 200 <= status_code <= 299:
            return response(200, {'message': 'Process completed successfully'})
        else:
            return response(status_code, data)

    # Unsupported method
    return response(405, {'message': 'Method Not Allowed'})

def get_account_details(loanAccountId):
    """Function to get account status and interest accrued"""
    api_url = f"{MAMBU_URL_BASE}/api/loans/{loanAccountId}"
    data = ""
    
    return send_request('GET', api_url, data)

def apply_interest(loanAccountId, dateTime):
    """Function to apply interest to the account."""
    api_url = f"{MAMBU_URL_BASE}/api/loans/{loanAccountId}:applyInterest"
    data = json.dumps({"interestApplicationDate": dateTime})

    return send_request('POST', api_url, data)

def update_fields(loanAccountId, dateTime, branchKey):
    """Function to update fields of the account."""
    api_url = f"{MAMBU_URL_BASE}/api/loans/{loanAccountId}"
    datepart = dateTime[:10]
    data = json.dumps([
        {
            "op": "ADD",
            "path": "_ACCOUNTING",
            "value": {
                "_ACTUAL_STAGE": "3",
                "_LAST_STAGE_CHANGE": datepart
            }
        },
        {
            "op": "ADD",
            "path": "/assignedBranchKey",
            "value": branchKey
        }
    ])

    return send_request('PATCH', api_url, data)

def send_request(method, url, data):
    """Helper function to send HTTP requests and handle errors."""
    try:
        response = http.request(method, url, body=data.encode('utf-8'), headers=HEADERS)
        return response.data.decode('utf-8'), response.status
    except Exception as e:
        return json.dumps({'error': str(e)}), 500

def response(status_code, data):
    """Helper function to return consistent responses."""
    return {
        "statusCode": status_code,
        "headers": {
            "Content-Type": "application/json"
        },
        "body": json.dumps(data)
    }