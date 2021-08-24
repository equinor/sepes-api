$dateSinceLastCommitToMaster = git log master -1 --format=%ct.
$output = git log --after=$dateSinceLastCommitToMaster --grep="feat!"

if (!$output) { Write-Host "test2" }
else { Write-Host "test1" }