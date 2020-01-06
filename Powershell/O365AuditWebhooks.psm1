param (
    [Parameter(Position = 0)]
    [string]
    $ErrorActionOverride = $(throw "You must supply an error action preference"),

    [Parameter(Position = 1)]
    [string]
    $InformationOverride = $(throw "You must supply an information preference"),

    [Parameter(Position = 2)]
    [string]
    $VerboseOverride = $(throw "You must supply a verbose preference")
)

$ErrorActionPreference = $ErrorActionOverride
$InformationPreference = $InformationOverride
$VerbosePreference = $VerboseOverride


function Invoke-AzCommand {
    param(
        # The command to execute
        [Parameter(Mandatory)]
        [string]
        $Command,

        # Output that overrides displaying the command, e.g. when it contains a plain text password
        [string]
        $Message = $Command
    )

    Write-Information -MessageData:$Message

    # Az can output WARNINGS on STD_ERR which PowerShell interprets as Errors
    $CurrentErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = "Continue" 

    Invoke-Expression -Command:$Command
    $ExitCode = $LastExitCode
    Write-Information -MessageData:"Exit Code: $ExitCode"
    $ErrorActionPreference = $CurrentErrorActionPreference

    switch ($ExitCode) {
        0 {
            Write-Debug -Message:"Last exit code: $ExitCode"
        }
        default {
            throw $ExitCode
        }
    }
}

function ConvertTo-StorageAccountName {
    param(
        # The generic name to use for all related assets
        [Parameter(Mandatory)]
        [string]
        $Name
    )

    $StorageAccountName = $($Name -replace '-', '').ToLowerInvariant()
    if ($StorageAccountName.Length -gt 24) {
        $StorageAccountName = $StorageAccountName.Substring(0, 24);
    }
    

    Write-Output $StorageAccountName
}