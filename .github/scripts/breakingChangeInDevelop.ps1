$dateSinceLastCommitToMaster = git log master -1 --format=%ct.
git checkout build/add_badge_for_breaking_change
$output = git log --after=$dateSinceLastCommitToMaster --grep="feat!" --grep="fix!" --grep="refactor!"

if (!$output) { return "No breaking change from backend" }
else { return "Backend change to be merged to prod" }