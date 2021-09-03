$relevantLine = Get-ChildItem ../../src/CodeCoverage/Combined/Summary.txt | Select -Index 6;


return $relevantLine -replace '\D+(\d+)','$1'
