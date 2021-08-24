$dateSinceLastCommitToMaster = git log master -1 --format=%ct.
$output = git log --after=$dateSinceLastCommitToMaster --grep="feat!"

if (!$output) { return "test2" }
else { return "test1" }