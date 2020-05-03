[CmdletBinding()]
param
(
	[string] $repositoryPath="",
	[string] $destinationPath="",
	[string] $sourceFileName=""
)
Set-Location $PSScriptRoot
$gitFolder = Split-Path -Path $repositoryPath -Leaf
echo "Step 1: Cloning GitRepository ($repositoryPath)"
git clone https://oauth:$env:SYSTEM_ACCESSTOKEN@$repositoryPath
echo "Step 2: Remove old Documentation"
New-Item -Path $gitFolder/$destinationPath -ItemType directory -Force
Get-ChildItem $gitFolder/$destinationPath -Filter *.md -Recurse | Remove-Item
echo "Step 3: Creating documentation for $sourceFileName"
./docpal.exe ./$sourceFileName -out $gitFolder/$destinationPath
echo "Step 4: Switching to GitFolder ($gitFolder)"
cd ./$gitFolder
echo "Step 5: Setting Git Config"
git config user.email "cd@valkyrEngine.com"
git config user.name "Continous delivery publish Task"
git config --unset-all http.Leginiel.extraheader
git config http.Leginiel.extraheader "AUTHORIZATION: bearer <System_AccessToken>"
echo "Step 6: Changes commited"
git add *.md
git commit -m "Updated Documentation"
echo "Step 7: Changes published"
git push
git status
git config --unset-all http.Leginiel.extraheader