$regex = "(?<oldVersion>(?<newVersion>\d{1,}\.\d{1,}\.\d{1,})[^<]*)"

$path = "./*.nupkg"
$zipfileName = Get-Childitem –Path $path

echo "Reading packages from $zipfileName"
Add-Type -assembly  System.IO.Compression.FileSystem
$zip =  [System.IO.Compression.ZipFile]::Open($zipfileName, 'update')

echo "Replacing version information in nuspec file"
$zipEntry = ($zip.Entries | Where-Object Name -like *.nuspec)
$file = New-Object System.IO.StreamReader($zipEntry.Open())
$filecontent = $file.ReadToEnd()
$oldVersion =  ([regex]::matches($filecontent,$regex))[0].Groups['oldVersion']
$newVersion =  ([regex]::matches($filecontent,$regex))[0].Groups['newVersion']

echo "Replacing $oldVersion with $newVersion"
$filecontent = $filecontent.Replace($oldVersion, $newVersion)

$file.Dispose()
$file = New-Object System.IO.StreamWriter($zipEntry.Open())
$file.Write($filecontent)


$file.Dispose()
$zip.Dispose()

echo "Replacing nupkg filename"
$fileName = $zipfileName.BaseName
$extension = $zipfileName.Extension 
echo $fileName
$oldVersion =  ([regex]::matches($fileName,$regex))[0].Groups['oldVersion']
$newVersion =  ([regex]::matches($fileName,$regex))[0].Groups['newVersion']

$newName = $fileName.Replace($oldVersion, $newVersion)
$newName = "$newName$extension"
Rename-Item -Path $zipfileName -NewName $newName