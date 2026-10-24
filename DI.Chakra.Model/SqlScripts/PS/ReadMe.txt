How to run PowerShell script?

1.In _Var.ps1 following details needs to be modified.

$database = "IACM_V51"    - Give the Database(DB) name in which we need to run the script
$serverName = "."         - If the DB is in local server, we can have  "." as value for $servername or else we can give the server IP (Eg.192.168.15.76)
$username ="sa"		  - DB Credentials , Username
$password ="di@123"       - DB Credentials , Password


2. Tools –> Library Package Manager –> Package Manager Console

Copy full Path of Power Shell script location. 
Eg.. Consider we need to execute the script "DeepIdentity.Model\SqlScripts\PS\Core\Setup-All.ps1", copy full path of the script
Paste the full path in Package Manager Console and press enter. the script will get executed.

 File Setup-All.ps1 execute all scripts in Core Folder. If there is need to exceute only particular script that is also possible.
Copy full path of the script which need to execute
Paste the full path in Package Manager Console and press enter. the script will get executed.

Endpoint :: Select the endpoint which you would like to display and execute the respective  script under the folder "EndPoint".

NOTE: If you are having Command Not Found errors, this could be due to the path having folders with space in the name. 
While running the script, first do a ChangeDirectory to your required folder with the path inside quotes.
Then run the script with a .\ in front of the script name.
i.e run the following commands in the Package Manager Console:
1) cd "Path to your directory where the script is" 
2) .\scriptname


  


