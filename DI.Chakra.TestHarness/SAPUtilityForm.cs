using DI.Chakra.Connector;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Connector.SAPV3;
using System.Data;
using DI.Chakra.Services.ProvisioningProcessor;
using DI.Chakra.Services.ProvisioningProcessor.FormExecutors;
using DI.Chakra.DomainModel;

namespace TestHarness
{
    public partial class SAPUtilityForm : Form
    {
        const string MasterConfigHelperServiceUser = "System";
        List<DI.Chakra.DomainModel.EndPoint> LstEndpoints = new List<DI.Chakra.DomainModel.EndPoint>();
        bool ConnectionStatus { get; set; }
        public SAPUtilityForm()
        {
            InitializeComponent();
        }

        private ConnectorBase _connector;

        ConnectorBase Connector
        {
            get
            {
                var endpoint = DI.Chakra.DomainModel.EndPoint.GetById(Convert.ToInt64(cmbEndpoint.SelectedValue));
                _connector = ConnectorFactory.GetConnector(endpoint, null);
                return _connector;
            }
            set { _connector = value; }
        }

        EndPoint GetSelectedEndpoint()
        {
            var endpoint = DI.Chakra.DomainModel.EndPoint.GetById(Convert.ToInt64(cmbEndpoint.SelectedValue));
            return endpoint;
        }

        private void SAPUtilityForm_Load(object sender, EventArgs e)
        {
            try
            {
                InitializeControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void InitializeControls()
        {
            //btnGetData.Enabled = false;

            LstEndpoints = DI.Chakra.DomainModel.EndPoint.GetAll();
            cmbEndpoint.DataSource = LstEndpoints;
            cmbEndpoint.DisplayMember = "DisplayName";
            cmbEndpoint.ValueMember = "Id";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ConnectionStatus = false;
                //MessageBox.Show("Trying to connect");
                var connected = Connector.Connect();
                if (connected)
                {
                    ConnectionStatus = true;
                    MessageBox.Show("Connection successful");
                    lblStatus.Text = $"{cmbEndpoint.Text} has been connected successfully.";
                }
                else
                    MessageBox.Show("Connection NOT successful");

                //btnGetData.Enabled = ConnectionStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void btnGetData_Click(object sender, EventArgs e)
        {
            try
            {
                gvResult.DataSource = null;
                switch (cmbObject.Text)
                {
                    case "Profile":
                        GetProfiles();
                        break;
                    case "Group":
                        GetGroups();
                        break;
                    case "Role":
                        GetRoles();
                        break;
                    case "User":
                        GetUsers();
                        break;
                    case "Parameter":
                        break;
                    case "TCode":
                        GetTCodes();
                        break;
                    case "Field":
                        break;
                    case "AuthObjects":
                        break;
                    case "Position":
                        break;
                    case "Section":
                        GetSection();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void GetTCodes()
        {
            dynamic processConfig = Connector.GetProcessorConfig();
            var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
            var _idmsLogger = new DiscoveryLogger(0, epId, EndpointType.SAP, cmbEndpoint.SelectedText
                );
            var obj = new DI.Chakra.Connector.SAPV3.TCode(processConfig);
            gvResult.DataSource = obj.GetTCodes();
        }

        private void GetGroups()
        {
            dynamic processConfig = Connector.GetProcessorConfig();
            var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
            var _idmsLogger = new DiscoveryLogger(0, epId, EndpointType.SAP, cmbEndpoint.SelectedText);
            var obj = new DI.Chakra.Connector.SAPV3.Group(processConfig);
            gvResult.DataSource = obj.GetGroups();
        }

        private void GetProfiles()
        {
            dynamic processConfig = Connector.GetProcessorConfig();
            var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
            var _idmsLogger = new DiscoveryLogger(0, epId, EndpointType.SAP, cmbEndpoint.SelectedText);
            var obj = new DI.Chakra.Connector.SAPV3.Profile(processConfig);
            gvResult.DataSource = obj.GetProfiles();
        }

        private void GetUsers()
        {
            var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
            var endpoint = LstEndpoints.Where(p => p.Id == epId).FirstOrDefault();
            if (endpoint.EndpointSystemTypeId == Convert.ToInt64(DI.Chakra.Infrastructure.Enums.EndpointType.OneCard))
            {
                var obj = new DI.Chakra.Connector.LenelConnector(endpoint, null, null);
                gvResult.DataSource = obj.GetUsers(txtObjectValue.Text);
            }
            else if (endpoint.EndpointSystemTypeId == Convert.ToInt64(DI.Chakra.Infrastructure.Enums.EndpointType.SAP))
            {
                var obj = new DI.Chakra.Connector.SAPV3.User(MasterConfigHelperServiceUser);
                dynamic processConfig = Connector.GetProcessorConfig();
                obj.ProcessorConfig = processConfig;
                string filter = null;
                if (!string.IsNullOrWhiteSpace(txtObjectValue.Text))
                    filter = $"BNAME EQ '{txtObjectValue.Text}'";
                gvResult.DataSource = obj.GetUsers(filter);
            }
            else
            {
                MessageBox.Show("Endpoint NOT supported as of now.");
            }
        }

        private void GetRoles()
        {
            var obj = new DI.Chakra.Connector.SAPV3.Role(MasterConfigHelperServiceUser);
            dynamic processConfig = Connector.GetProcessorConfig();
            obj.ProcessorConfig = processConfig;
            string filter = null;
            if (!string.IsNullOrWhiteSpace(txtObjectValue.Text))
                filter = $"AGR_NAME EQ '{txtObjectValue.Text}'";
            gvResult.DataSource = obj.GetRoles(filter);
        }

        private void GetSection()
        {
            dynamic processConfig = Connector.GetProcessorConfig();
            var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
            var _idmsLogger = new DiscoveryLogger(0, epId, EndpointType.SAP, cmbEndpoint.SelectedText);
            var obj = new DI.Chakra.Connector.SAPV3.Section(processConfig, _idmsLogger);
            gvResult.DataSource = obj.GetSections(txtObjectValue.Text);
        }

        private void btnCheckUsername_Click(object sender, EventArgs e)
        {
            try
            {
                var epId = Convert.ToInt64(cmbEndpoint.SelectedValue);
                var endpoint = LstEndpoints.Where(p => p.Id == epId).FirstOrDefault();
                if (endpoint.EndpointSystemTypeId == Convert.ToInt64(DI.Chakra.Infrastructure.Enums.EndpointType.SAP))
                {
                    var obj = new DI.Chakra.Connector.SAPV3.User(MasterConfigHelperServiceUser);
                    dynamic processConfig = Connector.GetProcessorConfig();
                    obj.ProcessorConfig = processConfig;
                    var result = obj.CheckUserExists(txtUsername.Text);
                    MessageBox.Show(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UI:{ex.Message }");
            }
        }
        private void btnCheckPosition_Click(object sender, EventArgs e)
        {
            try
            {
                SAPV3Connector sapConnector = (SAPV3Connector)Connector;
                var result = sapConnector.CheckPostionExistingByUser(txtPositionCode.Text, txtUsername.Text);
                MessageBox.Show($"Result:{result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter the password");
                    return;
                }
                SAPV3Connector sapConnector = (SAPV3Connector)Connector;
                var result = sapConnector.ChangePassword(txtUsername.Text, txtPassword.Text, 0, true);
                MessageBox.Show($"Result from system:{result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBAPIGetAll_Click(object sender, EventArgs e)
        {
            try
            {
                var obj = new DI.Chakra.Connector.SAPV3.User(MasterConfigHelperServiceUser);
                dynamic processConfig = Connector.GetProcessorConfig();
                obj.ProcessorConfig = processConfig;
                List<string> fields = new List<string>();
                //List<string> values = new List<string>();
                if (!string.IsNullOrWhiteSpace(txtFields.Text))
                    fields = txtFields.Text.Split(',').ToList();

                //if(fields.Count > 0)
                //{
                //    if (fields.Contains("SOBID"))
                //    {
                //        values.Add(txtUsername.Text);
                //    }
                //    if (fields.Contains("OBJID"))
                //    {
                //        values.Add(txtPositionCode.Text);
                //    }
                //}

                var dataTable = obj.ReadSAPData(txtTableName.Text, fields, txtFilter.Text);

                //var dataTable = obj.GetBAPI_USER_GETLIST(txtTableName.Text, cmbSAPTables.Text, Convert.ToInt32(txtObjectValue.Text));
                //foreach (DataTable dt in dataSet.Tables)
                //{
                //    MessageBox.Show($"Name: {dt.TableName}, Count: {dt.Rows.Count}");
                //}
                gvResult.DataSource = dataTable;

                MessageBox.Show(dataTable.Rows.Count.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnExtension_Click(object sender, EventArgs e)
        {
            try
            {
                var user = DI.Chakra.DomainModel.User.GetByEmployeeId(txtEmployeeId.Text);
                var ep = GetSelectedEndpoint();
                IdmsLogger idmsLogger = null;
                if (ep == null) {
                    idmsLogger = new ProvisioningLogger(0, null, null,string.Empty);
                }
                else
                {
                   idmsLogger = new ProvisioningLogger(0, ep.Id, ep.EndpointSystemTypeId,ep.DisplayName);
                }
                var accessRequest = new AccessRequest { AccountType = AccountType.User };
                FormExecutorBase formExecutor = FormExecutorFactory.Get(ep, accessRequest, idmsLogger);
                var fields = NProvisioningField.GetProvisioningDefaults(ep.Id, AccountType.User);
                bool success = formExecutor.Extension(user, fields, accessRequest.AccountType);
                MessageBox.Show($"Extended: {success}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter the password");
                    return;
                }
                SAPV3Connector sapConnector = (SAPV3Connector)Connector;
                var result = sapConnector.ResetPassword(txtUsername.Text, txtPassword.Text, string.Empty);
                MessageBox.Show($"Result from system:{result}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSRMReverse_Click(object sender, EventArgs e)
        {
            try
            {
                SRMConnector connector = new SRMConnector(GetSelectedEndpoint(), new AccessRequest(), new DiscoveryLogger(1, 1, EndpointType.SAP, "SRM"));
                connector.DoReverseEngineeringForUserToRoleMapping();
                MessageBox.Show("Completed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
