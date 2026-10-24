using DI.Chakra.DomainModel;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestHarness
{
    public partial class VPNTokenUpdate : Form
    {
        private delegate void SafeCallDelegate(string text);
        private DataTable dtXMLData;
        public VPNTokenUpdate()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.opnFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.txtFileName.Text = opnFileDialog.FileName;
                    dtXMLData = ReadCSV(this.txtFileName.Text);
                    dgVPNData.DataSource = dtXMLData;
                }
            }
            catch (Exception ex)
            {
                AppendTextBox(ex.Message);
            }
        }
        public DataTable ReadCSV(string filename)
        {
            try
            {
                DataTable csvData = new DataTable("xmlData");

                using (TextFieldParser parser = new TextFieldParser(filename))
                {
                    parser.SetDelimiters(",");
                    parser.TrimWhiteSpace = true;
                    parser.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = parser.ReadFields();

                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!parser.EndOfData)
                    {
                        string[] fieldData = parser.ReadFields();
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
                csvData.Columns.Add(new DataColumn("Log", typeof(string)));
                csvData.Columns.Add(new DataColumn("Old VPNKey", typeof(string)));
                csvData.Columns.Add(new DataColumn("New VPNKey", typeof(string)));
                csvData.Columns.Add(new DataColumn("AccountReconId", typeof(string)));
                csvData.Columns.Add(new DataColumn("UserId", typeof(string)));
                csvData.Columns.Add(new DataColumn("EndpointUserId", typeof(string)));
                return csvData;
            }
            catch (System.Exception ex)
            {
                AppendTextBox( "exception : " + ex.Message);
                return null;
            }
        }
        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), value + Environment.NewLine + Environment.NewLine);
                return;
            }
            txtLog.Text += value + Environment.NewLine + Environment.NewLine;
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtFileName.Text = "";
            txtLog.Text = "";
            dgVPNData.DataSource = null;
        }

        private void btnUpdateToken_Click(object sender, EventArgs e)
        {
            try
            {
                if(dtXMLData != null)
                {
                    var users = User.GetActiveEmployees();
                    users = users.Where(i => !string.IsNullOrWhiteSpace(i.OneKeyTokenId)).ToList();
                    var ep = EndPoint.GetByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.VPN);
                    var accountRecons = AccountReconcilation.GetAcountsByEndpointSystemId(ep.Id,DI.Chakra.Infrastructure.Enums.AccountType.User).Where(i=>i.IsDeleted==false);
                    var vpnUsers = VPNUserInfo.GetAllByEndpointSystemId(ep.Id);
                    for (int i = 0; i < dtXMLData.Rows.Count; i++)
                    {
                        AppendTextBox($"Updating row:{i+1}");
                        var adid = string.Empty;
                        var vpnKey = string.Empty;
                        try
                        {
                            var row = dtXMLData.Rows[i];
                            adid = row[0].ToString();
                            vpnKey = row[1].ToString();
                            var updatedDate = DateTime.Now;
                            var user = users.FirstOrDefault(j => j.UserName.Equals(adid,StringComparison.InvariantCultureIgnoreCase));



                            if (!string.IsNullOrWhiteSpace(adid) && !string.IsNullOrWhiteSpace(vpnKey))
                            {
                                if (user != null)
                                {
                                    if(!string.IsNullOrWhiteSpace(user.OneKeyTokenId) && user.OneKeyTokenId.Equals(vpnKey, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        row[2] = $"user:{user.DisplayName}({user.OneKeyTokenId}) adid:{adid} same. No update";
                                        continue;
                                    }
                                    var accRec = accountRecons.FirstOrDefault(j=> j.UserId == user.Id);
                                    if (accRec != null)
                                    {
                                        var vpnUser = vpnUsers.FirstOrDefault(k=> k.OneKeyTokenId == user.OneKeyTokenId && k.UserId == user.Id);
                                        if(vpnUser != null)
                                        {
                                            row[7] = $"{vpnUser?.Id}";
                                            if (accRec.EndpointUserId == vpnUser.Id)
                                            {
                                                row[5] = $"{accRec.Id}";
                                                var s = $"accRec:{accRec.AccountName} new Actual token:{vpnKey}";
                                                vpnUser.OneKeyTokenId = vpnKey;
                                                vpnUser.Save("System");
                                                accRec.AccountName = vpnKey;
                                                accRec.Save("System");
                                                user = User.GetById(user.Id);
                                                user.OneKeyTokenId = vpnKey;
                                                user.Save("System");
                                                s = $"Updated new Actual token:{vpnKey}";
                                                row[2] = s;
                                            }
                                            else
                                            {
                                                
                                                row[2] = $"accRecId:{accRec.Id} AccountName:{accRec.AccountName} Key:{accRec.EndpointUserId} VPNID:{vpnUser.Id}";
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        row[2] = $"User:{user.DisplayName}/OneKeyToken:{user.OneKeyTokenId} account recon does not exists";
                                    }

                                    row[3] = $"{user.OneKeyTokenId}";
                                    row[4] = $"{vpnKey}";
                                    row[5] = $"{accRec?.Id}";
                                    row[6] = $"{user?.Id}";
                                    
                                }
                                else
                                {
                                    row[2] = $"user might be Inactive for adid:{adid} not found";
                                }
                            }
                            else
                            {
                                row[2] = $"adid:{adid} vpnKey:{vpnKey} empty";
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendTextBox($"Row{i+1} vpnKey:{vpnKey} adid:{adid}");
                        }
                    }
                    dgVPNData.DataSource = dtXMLData;
                }
                MessageBox.Show($"Process completed");
            }
            catch (Exception ex)
            {
                AppendTextBox(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(saveFileDialog.FileName))
                    {
                        var columns = string.Empty;
                        for (int col = 0; col < dtXMLData.Columns.Count; col++)
                        {
                            columns= columns +","+$"{dtXMLData.Columns[col].ColumnName}";
                        }
                        file.WriteLine(columns);
                        for (int row = 0; row < dtXMLData.Rows.Count; row++)
                        {
                            columns = string.Empty;
                            for (int col = 0; col < dtXMLData.Columns.Count; col++)
                            {
                                columns = columns + "," + $"{dtXMLData.Rows[row][col]}";
                            }
                            file.WriteLine(columns);
                        }
                    }
                    MessageBox.Show($"File Exported to :{saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void BackupData(string fileName, DataTable data)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
                {
                    var columns = string.Empty;
                    for (int col = 0; col < data.Columns.Count; col++)
                    {
                        columns = columns + "," + $"{data.Columns[col].ColumnName}";
                    }
                    file.WriteLine(columns);
                    for (int row = 0; row < data.Rows.Count; row++)
                    {
                        columns = string.Empty;
                        for (int col = 0; col < data.Columns.Count; col++)
                        {
                            columns = columns + "," + $"{data.Rows[row][col]}";
                        }
                        file.WriteLine(columns);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
