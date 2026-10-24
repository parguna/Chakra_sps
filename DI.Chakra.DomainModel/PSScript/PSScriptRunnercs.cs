using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class PSScriptRunner
    {
        public static bool Run(string file,string Error)
        {
            Error = string.Empty;
            bool ScriptExecuted = false;
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;

                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                string dbName = builder.InitialCatalog;
                string dbDataSource = builder.DataSource;
                string userId = builder.UserID;
                string pass = builder.Password;

                // var scriptfile = @"D:\DI\V5\Source\SOURCE\DI.Chakra.DomainModel\SqlScripts\PS\Core\Test.ps1";

                StringBuilder sb = new StringBuilder();
                Collection<PSObject> results;
                Collection<ErrorRecord> errors;


                //////========Using Pipeline

                //RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

                //Runspace prunspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                //prunspace.Open();

                //RunspaceInvoke scriptInvoker = new RunspaceInvoke(prunspace);

                //Pipeline pipeline = prunspace.CreatePipeline();

                //Command myCommand = new Command(file);
                //CommandParameter testParam = new CommandParameter("database", dbName);
                //myCommand.Parameters.Add(testParam);

                //pipeline.Commands.Add(myCommand);

                //results = pipeline.Invoke();
                //var error = pipeline.Error.Read() as Collection<ErrorRecord>;
                //if (error.Count > 0)
                //{
                //    Error = error.ToString();
                //}
                //else
                //{
                //    foreach (PSObject result in results)
                //    {
                //        sb.AppendLine(result.ToString());
                //    }
                //    ScriptExecuted = true;
                //}


                //////========Using Powershell
                var runspace = RunspaceFactory.CreateRunspace();
                PowerShell psExec = PowerShell.Create();
                runspace.Open();

                psExec.Runspace = runspace;
                psExec.AddScript(file);

                //Passing variable to PS Script - Type 1
                psExec.Runspace.SessionStateProxy.SetVariable("database", dbName);


                //Passing variable to PS Script - Type 2
                //psExec.AddParameter("database", dbName);
                psExec.AddParameter("serverName", dbDataSource);
                psExec.AddParameter("username", userId);
                psExec.AddParameter("password", pass);


                results = psExec.Invoke();
                errors = psExec.Streams.Error.ReadAll();

                if (errors.Count > 0)
                {
                    foreach (ErrorRecord error in errors)
                    {
                        sb.AppendLine(error.ToString());
                    }
                    Error = sb.ToString();
                }
                else
                {
                    foreach (PSObject result in results)
                    {
                        sb.AppendLine(result.ToString());
                    }
                    ScriptExecuted = true;
                }
            }
            catch (Exception ex)
            {
                Error = ex.ToString();
            }

            return ScriptExecuted;
        }
    }
}
