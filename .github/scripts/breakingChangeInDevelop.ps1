$dateSinceLastCommitToMaster = git log master -1 --format=%ct.
$output = git log --after=$dateSinceLastCommitToMaster --grep="feat!"

if (!$output) { Write-Host "No breaking change from backend" }
else { Write-Host "Backend change to be merged to prod" }