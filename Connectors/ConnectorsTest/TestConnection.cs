using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using DI.Chakra.DomainModel;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConnectorsTest
{
    public partial class TestConnection : Form
    {
        public TestConnection()
        {
            InitializeComponent();
        }

        private void TestConnection_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
                treeView1.CheckBoxes = true;
               DataTable dt = this.GetData("SELECT Id, DisplayName FROM DI_M_EndpointSystemType where IsActive=1 and EndpointSystemTypeName!=9");
            this.PopulateTreeView(dt, 0, null);
        }

        private void PopulateTreeView(DataTable dtParent, int parentId, TreeNode treeNode)
        {
            
            foreach (DataRow row in dtParent.Rows)
            {
                TreeNode child = new TreeNode
                {
                    Text = row["DisplayName"].ToString(),
                    Tag = row["Id"]
                };
                if (parentId == 0)
                {
                    treeView1.Nodes.Add(child);
                    DataTable dtChild = this.GetData("SELECT Id, DisplayName FROM DI_M_EndpointSystem WHERE IsActive=1 and EndpointSystemTypeId!=9 and  EndpointSystemTypeId = " + child.Tag);
                    PopulateTreeView(dtChild, Convert.ToInt32(child.Tag), child);
                }
                else
                {
                    treeNode.Nodes.Add(child);
                }
            }
        }

        private DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            string ConnString = ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
            using (SqlConnection con = new SqlConnection(ConnString))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
                return dt;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Visible = true;

                progressBar1.Value = 10;
                var log = new StringBuilder();
            var list = new List<TreeNode>();
            LookupChecks(treeView1.Nodes, list);
            int i = 0;
            foreach (var item in list)
            {
                i++;
                    var endpointID = Convert.ToInt32(item.Tag);
               
                    var endpoint = EndPoint.GetByIdIncludingEndPointSystemType(endpointID);
                    if (endpoint.EndPointSystemType.EndpointSystemTypeName != DI.Chakra.Infrastructure.Enums.EndpointType.InhouseSystems)
                    {
                        log.Append("\r\n");
                        var getConnector = ConFactory.GetConnector(endpoint, new AccessRequest { Id = i });
                    
                        log.Append(string.Format("Endpoint Name : {0} \r\n", endpoint.DisplayName));
                        log.Append(string.Format("Connector Type : {0} \r\n", endpoint.EndPointSystemType.EndpointSystemTypeName.ToString()));
                        log.Append(string.Format("{0}\r\n", GetConfigurationDetails(endpointID)));
               

                        try
                        {
                            var connectorResult = getConnector.Connect();
                            log.Append(string.Format("===Result : {0}=== \r\n", connectorResult));
                            getConnector.EnableAccount(null);
                        }
                        catch (Exception conex)
                        {

                            log.Append(string.Format("Exception while connecting {0} \r\n Exception :{1}\r\n Stack trace :{2} \r\n", endpoint.DisplayName, conex.Message,conex.StackTrace));
                        }
                        log.Append("\r\n");
                        richTextBox1.Text = log.ToString();
                    }
                    
                    progressBar1.Value = 10+i+2>100?99 : 10 + i + 2;
                }


              

            }
            catch (Exception ex)
            {

                richTextBox1.Text = richTextBox1.Text + "-------------------------------------------Exception-------------------------\r\n" + ex.Message.ToString()+"\r\n--------------------------------------------Stack trace---------------------\r\n"+ex.StackTrace.ToString();
            }
            finally

            {
                progressBar1.Value = 100;

            }
        }

        private   string GetConfigurationDetails(int endpointID)
        {
            try
            {

            
            var configDetails = new StringBuilder();

            var config = EndpointConfiguration.GetByEndpointSystemId(endpointID);
            var endpointConfigurationKeys = EndpointConfigurationKey.GetAll();
            foreach (var item in config?.ConfigurationDetails)
            {
                var key = endpointConfigurationKeys.FirstOrDefault(mv => mv.Id == item.KeyId);
                if(key!=null && !key.KeyName.Contains("Password"))
                {
                    configDetails.Append(string.Format("{0} : {1}\r\n", key.KeyName,item.Value));
                }
            }
                return configDetails.ToString();
            }
            catch (Exception e) 
            {
                     
             
            }

            return string.Empty;
        }

        
        void LookupChecks(TreeNodeCollection nodes, List<TreeNode> list)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Checked & node.Parent!=null)
                    list.Add(node);

                LookupChecks(node.Nodes, list);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
             
            foreach (TreeNode node in treeView1.Nodes)
            {
                node.Checked = checkBox1.Checked;

                if (node.Nodes != null)
                {
                    foreach (TreeNode nodec in node.Nodes)
                    {
                        nodec.Checked = checkBox1.Checked;
                    }
                }
                
            }
        }
    }
}
