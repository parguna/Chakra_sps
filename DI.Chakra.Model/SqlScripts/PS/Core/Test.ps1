$d= $args[0]

. $PSScriptRoot\..\C_Var.ps1
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
$folders = get-childitem $core\Test -file
foreach ($folder in $folders)
{
	Invoke-Sqlcmd -InputFile $folder.fullname -ServerInstance . -Database $d -Username sa -Password di@123  -verbose
}
	 
