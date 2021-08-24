$dateSinceLastCommitToMaster = git log master -1 --format=%ct.
$output = git log --after=$dateSinceLastCommitToMaster --grep="feat!" --grep="fix!" --grep="refactor!"

if (!$output) { return "No breaking change from backend" }
else { return "Backend change to be merged to prod" }