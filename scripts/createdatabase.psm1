function New-SqlDatabase {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string]
        $Name,
        [Parameter(Mandatory=$true)]
        [string]
        $ResourceGroup,
        [Parameter(Mandatory=$true)]
        [string]
        $Server,
        [Parameter()]
        [string]
        $ElasticPool
    )
    try {
        az sql db create `
            --name $Name `
            --resource-group $ResourceGroup `
            --server $Server `
            --elastic-pool $ElasticPool
    }
    catch {
        $_.Exception
    }
}