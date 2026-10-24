. $PSScriptRoot\..\__Var.ps1

Invoke-Sqlcmd -InputFile $endpoint\SAPEP.sql -ServerInstance $serverName -Database $database -verbose