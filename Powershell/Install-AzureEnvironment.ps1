<#
.SYNOPSIS
- You need to be already signed in with AZ CLI
- Creates a Resource Group, with Storage Account, Function App with Application Insights. Defaults to the UKSouth location. This does not check if the name already exists.

.EXAMPLE
.\Install-AzureEnvironment.ps1 -Environment "cfcodedev" -Name:"AuditWebHook"

.EXAMPLE
.\Install-AzureEnvironment.ps1 -Environment "cfcodedev" -Name:"AuditWebHook" -Location:"westus"

#>


param(
	[Parameter(Mandatory)]
	[string]
	$Environment,
	[Parameter(Mandatory)]
	[string]
	$Name,
	[string]
	$Location = "uksouth",
)

$ErrorActionPreference = 'Stop'
$InformationPreference = 'Continue'

[string]$Identifier = "$Environment-$Name"


Import-Module -Name:"$PSScriptRoot\O365AuditWebhooks" -Force -ArgumentList:@(
    $ErrorActionPreference,
    $InformationPreference,
    $VerbosePreference
)

Write-Information "Setting the Azure CLI defaults..."
Invoke-AzCommand -command:"az configure --defaults location=$Location"

Write-Information -Message:"Creating the $Identifier resource group..."
Invoke-AzCommand -command:"az group create --name $Identifier"

Write-Information "Setting the Azure CLI defaults..."
Invoke-AzCommand -command"az configure --defaults location=$Location group=$Identifier"

[string]$StorageAccountName = ConvertTo-StorageAccountName -Name:$Identifier

Invoke-AzCommand -Command:"az storage account create --name ""$StorageAccountName"" --access-tier ""Hot"" --sku ""Standard_LRS"" --kind ""StorageV2"" --https-only $true"

$appInsights = Invoke-AzCommand -Command:"az resource create --resource-type 'Microsoft.Insights/components' --name '$Identifier' --properties '{\""Application_Type\"":\""web\""}'" | ForEach-Object { $PSItem -join '' } | ConvertFrom-Json

Invoke-AzCommand -Command:"az functionapp create --name ""$Identifier"" --storage-account ""$StorageAccountName"" --consumption-plan-location ""$Location"" --runtime ""dotnet"" --app-insights '$Identifier' --app-insights-key '$($appInsights.properties.InstrumentationKey)'"