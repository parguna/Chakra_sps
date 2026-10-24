using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
namespace TestHarness
{
    public partial class Office365Form : Form
    {
        Office365Connector connector = null;
        public Office365Form()
        {
            InitializeComponent();
            if (chkFromDB.Checked)
            {
                var ep = EndPoint.GetByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.Office365);
                connector = (Office365Connector)ConnectorFactory.GetConnector(ep, null, null);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtMessage.Text = "";
                MessageBox.Show($"Connected: {connector.Connect()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                var rPolicy = connector.GetUsers(new List<AccountReconcilation>(),new List<Office365UserInfo>());
                Showtext($"Total GetUsers: {rPolicy.Count()}");
                foreach (var item in rPolicy)
                {
                    Showtext($"{item.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var rPolicy = connector.GetSharingPolicy();
                Showtext($"Total SharingPolicy: {rPolicy.Count()}");
                foreach (var item in rPolicy)
                {
                    Showtext($"{item.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var rPolicy = connector.GetRetentionPolicy();
                Showtext($"Total RetentionPolicy: {rPolicy.Count()}");
                foreach (var item in rPolicy)
                {
                    Showtext($"{item.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show($"Connected: {this.connector.DownloadUser(txtUPN.Text)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtMessage.Text = "";
                foreach (var item in connector.GetLicenses())
                {
                    txtMessage.Text= txtMessage.Text+ ($"{item.AccountSkuId}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtMessage.Text = "";
                MessageBox.Show($"IsMailBoxExists: {this.connector.IsMailBoxExists(txtUPN.Text)}");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtMessage.Text = "";
                var rPolicy = connector.GetRolePolicy();
                Showtext($"Total RolePolicy: {rPolicy.Count()}");
                foreach (var item in rPolicy)
                {
                    Showtext($"{item.Name}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Showtext(string message)
        {
            txtMessage.Text = $"{txtMessage.Text}{Environment.NewLine}{message}";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                connector.GetData();
                Showtext("Discovery completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGroup_Click(object sender, EventArgs e)
        {
            try
            {
                this.txtMessage.Text = "";
                connector.GetOffice365Groups();
                MessageBox.Show("Group discovery done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string cmdImportScriptV1 = @"CreateExoPSSession.ps1";
        private void button9_Click(object sender, EventArgs e)
        {
            bool isConnected = false;
            try
            {
                System.Management.Automation.PSCredential credentials = GetPSCredential();
                InitialSessionState iss = InitialSessionState.CreateDefault();
                iss.ImportPSModule(new[] { @"C:\Program Files\WindowsPowerShell\Modules\ExchangeOnlineManagementV1\CreateExoPSSession.ps1" });
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(iss))
                {
                    psRunSpace.Open();
                    using (System.Management.Automation.PowerShell powershell = System.Management.Automation.PowerShell.Create())
                    {
                        try
                        {
                            powershell.Runspace = psRunSpace;
                            Command connect = new Command("Connect-EXOPSSession");
                            connect.Parameters.Add("Credential", credentials);
                            powershell.Commands.AddCommand(connect);
                            var results = powershell.Invoke();

                            var err = powershell.Streams.Error.ReadAll();
                            if (err.Count() > 0)
                            {
                                string errMsg = string.Empty;
                                foreach (var msg in err)
                                {
                                    errMsg += msg;
                                }
                                Showtext(errMsg);
                                return ;
                            }
                            powershell.Commands.Clear();
                            connect = new Command("Connect-MsolService");
                            connect.Parameters.Add("Credential", credentials);
                            powershell.Commands.AddCommand(connect);
                            results = powershell.Invoke();
                            err = powershell.Streams.Error.ReadAll();
                            if (err.Count() > 0)
                            {
                                string errMsg = string.Empty;
                                foreach (var msg in err)
                                {
                                    errMsg += msg;
                                }
                                Showtext(errMsg);
                                return;
                            }
                            powershell.Commands.Clear();
                            isConnected = true;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            powershell.Commands.AddScript("Get-PSSession | Remove-PSSession");
                            powershell.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isConnected = false;
                MessageBox.Show($"Error: {ex.Message}");
            }
            MessageBox.Show($"Connected: {isConnected}");
        }

        private PSCredential GetPSCredential()
        {
            string userName = txtUserName.Text;
            string password = txtPassword.Text;
            System.Security.SecureString secureString = new System.Security.SecureString();
            string myPassword = password;
            foreach (char c in myPassword)
                secureString.AppendChar(c);
            return new PSCredential(userName, secureString);
        }

        private void chkFromDB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var ep = EndPoint.GetByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.Office365);
                connector = (Office365Connector)ConnectorFactory.GetConnector(ep, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                List<SkypeForBusinessScriptBlock> executionSteps = new List<SkypeForBusinessScriptBlock>();
                executionSteps.Add(new SkypeForBusinessScriptBlock() { Script = @"$DistributionGroups= Get-DistributionGroup | where { (Get-DistributionGroupMember $_.Name | foreach {$_.Name}) -contains " + txtUPN.Text + "} \n $DistributionGroups" });
                for (int i = 0; i < 30; i++)
                {
                    IsUserPartOfDistributiongroup(executionSteps);
                }
                
            }
            catch (Exception ex)
            {
            }
        }
        public bool IsUserPartOfDistributiongroup(List<SkypeForBusinessScriptBlock> executionSteps)
        {
            bool isSuccess = false;
            InitialSessionState iss = InitialSessionState.CreateDefault();
            iss.ImportPSModule(new[] { @"C:\\Program Files\\WindowsPowerShell\\Modules\\ExchangeOnlineManagement\\2.0.5\\ExchangeOnlineManagement.psd1" });
            using (Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create())
                {

                    powershell.Runspace = runspace;
                        try
                        {
                            var connect = new Command("Connect-ExchangeOnline");
                            connect.Parameters.Add("Credential", GetPSCredential());
                            connect.Parameters.Add("CommandName", new[] { "Get-DistributionGroup", "Get-DistributionGroupMember" });
                            powershell.Commands.AddCommand(connect);
                            var moduelImport = powershell.Invoke();
                            //if (powershell.HadErrors == false)
                            {
                                powershell.Commands.Clear();
                                foreach (var executionStep in executionSteps)
                                {
                                    var script = executionStep.Script;
                                    powershell.Commands.Clear();
                                    powershell.Runspace = runspace;
                                    powershell.Commands.AddScript(script);
                                    var executionResult = powershell.Invoke();
                                   // if (powershell.HadErrors == false)
                                    {
                                        foreach (PSObject item in executionResult)
                                        {

                                        }
                                        return executionResult.Count > 0;
                                    }
                                    isSuccess = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        finally
                        {

                        }
                    runspace.Close();
                }
            }
            return isSuccess;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    InitialSessionState iss = InitialSessionState.CreateDefault();
                    iss.ImportPSModule(new[] { @"C:\\Program Files\\WindowsPowerShell\\Modules\\ExchangeOnlineManagement\\2.0.5\\ExchangeOnlineManagement.psd1" });
                    using (Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(iss))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            //powershell.Dispose();
                            //runspace.Close();
                            //runspace.Dispose();
                            iss = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    InitialSessionState iss = InitialSessionState.CreateDefault();
                    iss.ImportPSModule(new[] { @"C:\\Program Files\\WindowsPowerShell\\Modules\\ExchangeOnlineManagement\\2.0.5\\ExchangeOnlineManagement.psd1" });
                    using (Runspace runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(iss))
                    {
                        runspace.Open();
                        using (PowerShell powershell = PowerShell.Create())
                        {
                            powershell.Runspace = runspace;
                            powershell.Dispose();
                            runspace.Close();
                            runspace.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
