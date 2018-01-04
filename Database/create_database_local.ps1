function Log
{
  param ($isError = $False, [string]$text = [string]::Empty)

  If ($isError)
  {
    Write-Host "Error: " + $text -ForegroundColor Red
  }
  else
  {
    Write-Host $text -ForegroundColor Green
  }
}

$Server = ".";
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
$CreateDbPath = Join-Path $BasePath "Create\create_db_user.sql"

Invoke-Sqlcmd -ServerInstance . -Database master -InputFile $CreateDbPath
If (!($?))
{
  Log -isError $True -text "Creating database failed."
  [System.Data.SqlClient.SqlConnection]::ClearAllPools()
  Exit $LASTEXITCODE
}
Else
{
	Log $false "Database creation successful."
}

Invoke-Sqlcmd -ServerInstance . -Database testDB -InputFile $CreatePath
If (!($?))
{
  Log $True, "Creating tables failed."
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
	Invoke-Sqlcmd -ServerInstance . -Database testDB -InputFile $file
	If (!($?))
	{
	  Log $True, "Script" + $files[$i].Name + "failed."
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