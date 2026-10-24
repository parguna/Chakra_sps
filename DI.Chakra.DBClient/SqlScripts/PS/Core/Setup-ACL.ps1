. $PSScriptRoot\..\__Var.ps1

$folders = get-childitem $core\ACL -file

foreach ($folder in $folders)
{
	  echo $folder.fullname
	#  Invoke-Sqlcmd -InputFile $folder.fullname -ServerInstance $serverName -Database $database -verbose   # Windows Authentication
	  Invoke-Sqlcmd -InputFile $folder.fullname -ServerInstance $serverName -Database $database -Username $username -Password $password  -verbose
} 

