Param(
  [Parameter(Mandatory=$True)]
  [string]$Server,
  
  [Parameter(Mandatory=$True)]
  [string]$UserName,

  [Parameter(Mandatory=$True)]
  [string]$Password
)

function Log
{
  param ($isError = $False, [string]$text = [string]::Empty)

  If ($isError)
  {
    Write-Host "Error: $($text)" -ForegroundColor Red
  }
  else
  {
    Write-Host $text -ForegroundColor Green
  }
}

if ([String]::IsNullOrWhiteSpace($Server))
{
	Log $True "Server name is mandatory."
	Exit 0
}

if ([String]::IsNullOrWhiteSpace($UserName))
{
	Log $True "User name is mandatory."
	Exit 0
}

if ([String]::IsNullOrWhiteSpace($Password))
{
	Log $True "Password is mandatory."
	Exit 0
}

$BasePath = Get-Location;
Import-Module sqlps -DisableNameChecking

## Check connection to database
#sqlplus -L $Username/$Password@$Sid $Role "@TestConnection.sql"
#If (!($?))
#{
#  Log $True "User credentials are invalid or not able to connect to database."
  
#  Exit $LASTEXITCODE
#}

$CreatePath = Join-Path $BasePath "Create\create_script.sql"

Invoke-Sqlcmd -ServerInstance $Server -Database testDB -UserName $Username -Password $Password -InputFile $CreatePath
If (!($?))
{
  Log $True "Creating tables failed."
  [System.Data.SqlClient.SqlConnection]::ClearAllPools()
  Exit $LASTEXITCODE
}
Else
{
	Log $false "Tables creation successful."
}

# Call insert scripts
$InsertPath = Join-Path $BasePath "Insert"
$files = Get-ChildItem $InsertPath -Filter *.sql

for($i=0; $i -lt $files.Count; $i++)
{
	$file = $files[$i].FullName;
	$fileName = $files[$i].Name;
	Invoke-Sqlcmd -ServerInstance $Server -Database testDB -UserName $Username -Password $Password -InputFile $file
	If (!($?))
	{
	  Log $True, "Script" + $file + "failed."
	  [System.Data.SqlClient.SqlConnection]::ClearAllPools()
	  Exit $LASTEXITCODE
	}
	Else
	{
		Log $false "Insert of data from file $($fileName) successful."
	}
}

[System.Data.SqlClient.SqlConnection]::ClearAllPools()

Log $false "Script finished successfully."