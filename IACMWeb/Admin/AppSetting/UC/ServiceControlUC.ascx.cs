using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.AppSetting.UC
{
    public partial class ServiceControlUC : System.Web.UI.UserControl
    {
        private const string SERVICE_NAME = "V_SVCNAME";
        private const string SERVICECONTROLLER_NAME = "V_SERVICECONTROLLER_NAME";
        private const string SERVICE_PRODUCT = "V_SERVICE_PRODUCT";
        private const string SERVICE_DISPLAYNAME = "V_SVCDISPLAYNAME";
        public string ServiceControllerName
        {
            get
            {
                if (ViewState[SERVICECONTROLLER_NAME].ToString() == null)
                    return string.Empty;

                return ViewState[SERVICECONTROLLER_NAME].ToString();
            }
            set
            {
                ViewState[SERVICECONTROLLER_NAME] = value;
            }
        }
        public string ServiceName
        {
            get
            {
                if (ViewState[SERVICE_NAME].ToString() == null)
                    return string.Empty;

                return ViewState[SERVICE_NAME].ToString();
            }
            set
            {
                ViewState[SERVICE_NAME] = value;
            }
        }
        public string Product
        {
            get
            {
                if (ViewState[SERVICE_PRODUCT].ToString() == null)
                    return string.Empty;

                return ViewState[SERVICE_PRODUCT].ToString();
            }
            set
            {
                ViewState[SERVICE_PRODUCT] = value;
            }
        }
        public string ServiceDisplayName
        {
            get
            {
                if (ViewState[SERVICE_DISPLAYNAME].ToString() == null)
                    return string.Empty;

                return ViewState[SERVICE_DISPLAYNAME].ToString();
            }
            set
            {
                ViewState[SERVICE_DISPLAYNAME] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblTitle.Text = ServiceDisplayName;

                DI.Chakra.Infrastructure.Enums.Product prd = (Infrastructure.Enums.Product)Enum.Parse(typeof(Infrastructure.Enums.Product), Product);
                var config = DBConfigHelper.MasterConfigHelper.GetConfigValue(this.ServiceName, prd);

                if(config != null)
                {
                    ServiceControllerName = config.ConfigValue;

                    GetServiceStatus();
                }
            }
        }

        protected void lnkSvcStart_Click(object sender, EventArgs e)
        {

            string status = string.Empty;
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("servicename", this.ServiceControllerName);
                ServiceControllerStatus srvcontProcess = DomainServiceClient.GetByAction<ServiceControllerStatus>
                    (DomainServiceAction.AppWinService.RootName, DomainServiceAction.AppWinService.GetServicesStatus, parameter);

                if (srvcontProcess == ServiceControllerStatus.Running)
                {
                    parameter.Clear();
                    parameter.Add("toStartStop", "Stop");
                    parameter.Add("servicename", this.ServiceControllerName);
                    parameter.Add("timeoutMilliseconds", 20.ToString());
                    status = DomainServiceClient.GetByAction<string>
                         (DomainServiceAction.AppWinService.RootName, DomainServiceAction.AppWinService.StartStopWinService, parameter);


                }
                else if (srvcontProcess == ServiceControllerStatus.Stopped)
                {
                    parameter.Clear();
                    parameter.Add("toStartStop", "Start");
                    parameter.Add("serviceName", ServiceControllerName);
                    parameter.Add("timeoutMilliseconds", 20.ToString());
                    status = DomainServiceClient.GetByAction<string>
                        (DomainServiceAction.AppWinService.RootName, DomainServiceAction.AppWinService.StartStopWinService, parameter);

                }

                System.Threading.Thread.Sleep(1200);
                GetServiceStatus();


            }
            catch (System.Exception ex)
            {

                throw;
            }
            
        }
        private void GetServiceStatus()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("servicename", ServiceControllerName);
            ServiceControllerStatus srvcontProcess = DomainServiceClient.GetByAction<ServiceControllerStatus>
                (DomainServiceAction.AppWinService.RootName, DomainServiceAction.AppWinService.GetServicesStatus, parameter);

            string serviceStatus = srvcontProcess.ToString();
            if (srvcontProcess == ServiceControllerStatus.Running)
            {
                lnkSvcStart.Text = "Stop";
                txtSvc.ForeColor = System.Drawing.Color.Green;
                txtSvc.Text = serviceStatus;
            }
            else if (srvcontProcess == ServiceControllerStatus.Stopped)
            {
                lnkSvcStart.Text = "Start";
                txtSvc.ForeColor = System.Drawing.Color.Red;
                txtSvc.Text = serviceStatus;
            }
        }
        protected void lnkSvcRestart_Click(object sender, EventArgs e)
        {


            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("toStartStop", "Restart");
            parameter.Add("serviceName", ServiceControllerName);
            parameter.Add("timeoutMilliseconds", 20000.ToString());
            DomainServiceClient.GetByAction<string>
                (DomainServiceAction.AppWinService.RootName, DomainServiceAction.AppWinService.StartStopWinService, parameter);

            System.Threading.Thread.Sleep(1200);
            GetServiceStatus();

        }
    }
}