<#
.SYNOPSIS
Will start an Audit Webhook.

.EXAMPLE
 .\Set-AuditLogs.ps1 -ClientID:<CLIENTID> -ClientSecret:<APPSECRET> -TenantDomain:<TENANT>.onmicrosoft.com -TenantGUID:<TENANTGUID> -WebHookUrl:https://<AzurefunctionURL>/API/AuditWebHook -ContentType:Audit.SharePoint
#>

param(
	[Parameter(Mandatory)]
	[string]
	$ClientID,
	[Parameter(Mandatory)]
	[string]
	$ClientSecret,
	[Parameter(Mandatory)]
	[string]
	$TenantDomain,
	[Parameter(Mandatory)]
	[string]
	$TenantGUID,
	[Parameter(Mandatory)]
	[string]
	$WebhookUrl,
	[Parameter(Mandatory)]
	[string]
	[ValidateSet('Audit.AzureActiveDirectory','Audit.Exchange','Audit.SharePoint','Audit.General','DLP.All')]
	$ContentType
)

$ErrorActionPreference = 'Stop'
$InformationPreference = 'Continue'

Write-Information "Preparing the call to WebHook"
$loginURL = "https://login.microsoftonline.com/"
$resource = "https://manage.office.com"
# auth
$body = @{grant_type="client_credentials";resource=$resource;client_id=$ClientID;client_secret=$ClientSecret}
$oauth = Invoke-RestMethod -Method Post -Uri $loginURL/$tenantdomain.onmicrosoft.com/oauth2/token?api-version=1.0 -Body $body
$headerParams = @{'Authorization'="$($oauth.token_type) $($oauth.access_token)"}

$webhook = @{'address' = $webhookUrl; 'authId' = '365notificationaad_' + $contentType; 'expiration' = ''}
$hook  = @{ 'webhook' = $webhook}


$body = $hook | ConvertTo-Json

Write-Information "Start a new Subscription $ContentType"

Invoke-WebRequest -Method Post -Body:$body -ContentType "application/json" -Headers $headerParams -Uri "https://manage.office.com/api/v1.0/$TenantGUID/activity/feed/subscriptions/start?contentType=$contentType"