[CmdletBinding()]
Param(
    [Parameter(Mandatory=$true)]
    [string]$productVersion
)

$buildNumber = $env:BUILD_BUILDNUMBER 
$SrcPath = $env:BUILD_SOURCESDIRECTORY
Write-Verbose "Executing Update-AssemblyFileVersion in path $SrcPath for product version $productVersion"
     
$AllVersionFiles = Get-ChildItem $SrcPath AssemblyInfo.cs -recurse

# calculate version number
$versions = $productVersion.Split('.')
$build = $buildNumber.Split('.')
$major = $versions[0]
$minor = $versions[1]
$build = $build[0]
$revision = $build[1]

$assemblyFileVersion = "$major.$minor.$build.$revision"
     
Write-Verbose "Transformed Assembly File Version is $assemblyFileVersion"
 
foreach ($file in $AllVersionFiles) 
{ 
	#version replacements
    (Get-Content $file.FullName) |
    %{$_ -replace 'AssemblyDescription\(""\)', "AssemblyDescription(""Assembly built by TFS Build $buildNumber"")" } |
    %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', "AssemblyFileVersion(""$assemblyFileVersion"")" } |
	%{$_ -replace 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', "AssemblyInformationalVersion(""$productVersion"")" } |
	Set-Content $file.FullName -Force
}