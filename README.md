# Introduction

This is the main README.md file for the O365AuditWebHook project. 
This README.md file will show you how to set up your environment read to use this code. 

The purpose of this project is to show how you can set up a webhook connected to the Office 365 Audit logs.

Related blog post URL: <TO BE WRITTEN>

## Disclaimer

This code is to be used as an example, and should never be used directly within Production environoment.

## Pre-requsites knowledge required
- PowerShell
- Azure Functions V1
- Azure Storage
- SharePoint Tenant
- Creating a App Token

## Getting Started

### Create Environment Settings file

Create an local.settings.json file that holds all the settings needed for the environment in the [AuditWebHook](./AuditWebHook) folder.

```json
{
 "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "<Location to your Azure Function Storage>",
    "AzureWebJobsDashboard": "<Location to your Azure Fucntion Storage>",
    "Tenant": "<TenantName>",
    "ClientId": "<ClientID>",
    "Secret": "<AppSecret>" ,
    "FUNCTIONS_EXTENSION_VERSION": "~1",
    "AzureServicesAuthConnectionString": "RunAs=Developer;DeveloperTool=AzureCli"
    }
}
```

### Steps to Set up

- Create Azure Resource Group
    - Add Azure Functions V1
    - Add Storage
        - Take copy of the **Access Keys Connection String for Key 1**
    - Add Application Insights
- Create a App Registration
    - Get Copy of **Application (Client) ID**
    - Create a Secret and take a copy of the **Application secret**
    - Take a copy of the **Directory (tenant) ID** 
    - Add API Permssion - **Office 365 Managemnet APIs** -> **Application permissions** -> **ActivityFeed.Read**
- Put the values you copied from previous values into the local.settings.json file
    - Keep the **Directory (tenant) ID** for PowerShell later
    - The **Tenant** name is just the name before .onmicrosoft.com _(no need to include .onmicrosoft.com)_
- Publish the Solution to the Azure Function
    - Ensure the settings are in the Configuration settings
    - Take a copy of your **Azure Function URL**

### Add an O365 Audit log to your Webhook

There are 5 different logs that can connect to the webhook.
- Audit.AzureActiveDirectory
- Audit.Exchange
- Audit.SharePoint
- Audit.General
- DLP.All

Call the following PowerShell to register the Audit.SharePoint to the webhook.

```ps
.\Set-AuditLogs.ps1 -ClientID:<CLIENTID> -ClientSecret:<APPSECRET> -TenantDomain:<TENANT>.onmicrosoft.com -TenantGUID:<TENANTGUID> -WebHookUrl:https://<AzurefunctionURL>/API/AuditWebHook -ContentType:Audit.SharePoint
```

### Remove an O365 Audit log from your Webhook

Call the following PowerShell to de-register the Audit.SharePoint from the webhook.

```ps
.\Remove-AuditLogs.ps1 -ClientID:<CLIENTID> -ClientSecret:<APPSECRET> -TenantDomain:<TENANT>.onmicrosoft.com -TenantGUID:<TENANTGUID> -WebHookUrl:https://<AzurefunctionURL>/API/AuditWebHook -ContentType:Audit.SharePoint
```