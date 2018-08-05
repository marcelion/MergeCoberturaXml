Write-Host "#######################"
Write-Host "### Executing tests ###"
Write-Host "#######################"

$TestDirectories = Get-ChildItem -Directory tests\*.Test
$CoverageOutputFiles = New-Object 'System.Collections.Generic.List[string]'
ForEach ($TestDirectory in $TestDirectories) {
	$TestOutputFile = $TestDirectory.Name + ".test.trx"
	$CoverageOutputFile = $TestDirectory.Name + ".cobertura.xml"
	$CoverageOutputFiles.Add($CoverageOutputFile)
	
	Push-Location $TestDirectory
	dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput="..\..\${CoverageOutputFile}" --logger "trx;LogFileName=..\..\..\${TestOutputFile}" -c Release /p:CollectCoverage=true
	Pop-Location
}

Write-Host "DONE"
Write-Host