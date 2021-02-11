#Function for checking installed and imported modules
function CheckModules {
    param (
        [Parameter(Mandatory=$true)]
        [string]
        $ModuleName
    )
    try {
    Write-Output "Importing module $ModuleName"
    Import-Module $ModuleName -ErrorAction Stop
    }
    catch {
        try {
        Install-Module $ModuleName -ErrorAction Stop
        }
        catch {
            $_.Exception.Message
            Write-Error "Problems importing module: $ModuleName"
        }
    }
}

# Creating a new App Service Plan
function New-AppServicePlan {
    param (
        [Parameter(Mandatory=$true)]
        [string]
        $AppServiceName,
        [Parameter(Mandatory=$true)]
        [string]
        $ResourceGroup,
        [Parameter()]
        [string]
        $Location = "WestEurope",
        [Parameter(Mandatory=$true)]
        [string]
        $SkuName
    )
    CheckModules -ModuleName Az.Websites
    try {
        $asp = Get-AzAppServicePlan -Name $AppServiceName -ResourceGroupName $ResourceGroup
        if (!$asp) {
            Write-Output "Creating new App Service Plan"
            New-AzResource -Name $AppServiceName -ResourceGroupName $ResourceGroup -Location $Location -ResourceType "Microsoft.Web/serverFarms" -kind Linux -sku @{name="$SkuName"} -Force
            Start-Sleep 10
        }
        else {
            Write-Output "App Service Plan $AppServiceName Alraedy Exsists"
        }
    }
    catch {
        $_.Exception.Message
    }
}

# Create new function app
function New-FunctionApp {
    param (
        [Parameter(Mandatory=$True)]
        [string]
        $FunctionName,
        [Parameter(Mandatory=$True)]
        [string]
        $AppServicePlan,
        [Parameter(Mandatory=$True)]
        [string]
        $RgName,
        [Parameter(Mandatory=$True)]
        [string]
        $StorageAccountName,
        [Parameter()]
        [string]
        $Location = "WestEurope",
        [Parameter(Mandatory=$True)]
        [string]
        $Runtime
    )
    try {
        $storageAcoount = Get-AzStorageAccount -Name $StorageAccountName -ResourceGroupName $RgName -ErrorAction SilentlyContinue
        if (!$storageAcoount) {
            Write-Output "Creating New storage account"
            New-AzStorageAccount -Name $StorageAccountName -ResourceGroupName $RgName -Location $Location -Kind StorageV2 -SkuName Standard_LRS -AccessTier Hot
        }
        else {
            Write-Output "Using existing storage account $StorageAccountName"
        }
    }
    catch {
        
    }
    $funcApp = Get-AzFunctionApp -Name $FunctionName -ResourceGroupName $RgName
    try {
        if (!$funcApp) {
            New-AzFunctionApp `
                -ResourceGroupName $rgName `
                -Name $functionName `
                -PlanName $AppServicePlan `
                -StorageAccountName $StorageAccountName `
                -Runtime $Runtime
        }
    else {
        Write-Output "Function App $functionName already exist"
    }
    }
    catch {
        
    }
}

# Set Function Application settings from a file. The file must be located in the same context as the powershell script, or use set with the AppSettings paramter
function Set-FunctionAppSettings {
    param (
        [Parameter(Mandatory=$True)]
        [string]
        $FunctionName,
        [Parameter(Mandatory=$True)]
        [string]
        $RgName,
        [Parameter()]
        [string]
        $StorageAccountName,
        [Parameter()]
        [string]
        $AppSettings = "./appsettings.txt"
    )

    $settings = Get-Content "$AppSettings"
    Write-Output "Updating application settings for $FunctionName"
    az functionapp config appsettings set -n $FunctionName -g $RgName --settings $settings
}

function Update-FunctionAppContainerSettings {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]
        $FunctionName,
        [Parameter(Mandatory=$true)]
        [string]
        $RgName,
        [Parameter(Mandatory=$true)]
        [string]
        $ImageName,
        [Parameter(Mandatory=$true)]
        [string]
        $RegistryPassord,
        [Parameter(Mandatory=$true)]
        [string]
        $RegistryUrl,
        [Parameter(Mandatory=$true)]
        [string]
        $RegistryUsername
    )
    az functionapp config container set `
    --docker-custom-image-name $ImageName `
    --docker-registry-server-password $RegistryPassord `
    --docker-registry-server-url $RegistryUrl `
    --docker-registry-server-user $RegistryUsername `
    --name $FunctionName `
    --resource-group $RgName
}