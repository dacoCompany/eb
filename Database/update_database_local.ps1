function Log
{
  param ($isError = $False, $printDefault = $False, [string]$text = [string]::Empty)

  If ($isError)
  {
    Write-Host "Error: " + $text -ForegroundColor Red
  }
  elseif ($printDefault)
  {
  Write-Host $text
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

$UpdatePath = Join-Path $BasePath "Update"

Log $False $True "Reading applied scripts..."
$AppliedScripts = New-Object System.Collections.ArrayList
# Select all script names from database table SCRIPT_AUDIT
$OutputVariable = Invoke-Sqlcmd -ServerInstance $Server -Database testDB -Query "select ScriptName from ScriptAudit order by ScriptName" | Out-String
If (!($?))
{
  Log -isError $True -printDefault $False -text "Reading applied scripts failed."
  [System.Data.SqlClient.SqlConnection]::ClearAllPools()
  Exit $LASTEXITCODE
}

Foreach ($line in $OutputVariable.Split("`n"))
{
  $line = $line.Trim()

  # Skip empty entries
  If ($line -eq [string]::Empty)
  {
    Continue
  }

  if ($line.Contains(".sql"))
  {
	$AppliedScripts.Add($line) | Out-Null
  }
}
Log $False $True "Found $($AppliedScripts.Count) entries."

# Call update scripts
$SourceScripts = Get-ChildItem $UpdatePath -Filter *.sql

for($i=0; $i -lt $SourceScripts.Count; $i++)
{
	$file = $SourceScripts[$i].FullName;
	$fileName = $SourceScripts[$i].Name;

	If ($AppliedScripts -contains $fileName)
	{
		Log $False $True "Script $($fileName) is already applied."
	}
	Else
	{
		# Execution of update script.
		Log $False $True "`r`nApplying script $($fileName).`r`n"
		Invoke-Sqlcmd -ServerInstance $Server -Database testDB -InputFile $file -Verbose
		If (!($?))
		{
		  Log $True $False "Script" + $files[$i].Name + "was not applied successfully."

		  # Updating script status in database.
		  Log $False $True "Marking script $($fileName) as 'FAILED'."
		  Invoke-Sqlcmd -ServerInstance $Server -Database testDB -Query "insert into ScriptAudit (ScriptName, ScriptStatus) values ('$filename', 'FAILED')"
		  [System.Data.SqlClient.SqlConnection]::ClearAllPools()
		  Exit $LASTEXITCODE
		}
		Else
		{
			Log $false $False "Script $($fileName) was applied successfully."
			Invoke-Sqlcmd -ServerInstance $Server -Database testDB -Query "insert into ScriptAudit (ScriptName, ScriptStatus) values ('$fileName', 'SUCCESS')"
		}
	}
	
}

[System.Data.SqlClient.SqlConnection]::ClearAllPools()

Log $false $False "Script finished successfully."