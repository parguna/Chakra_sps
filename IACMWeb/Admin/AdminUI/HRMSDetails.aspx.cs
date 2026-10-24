using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.AdminUI
{
    public partial class HRMSDetails : DI.Chakra.Web.UIBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                ddlApplicationTypes.Items.Add(new ListItem("None", Convert.ToString((long)EndpointType.None)));
                ddlApplicationTypes.Items.Add(new ListItem("SAP", Convert.ToString((long)EndpointType.SAP)));
                ddlApplicationTypes.Items.Add(new ListItem("SuccessFactors", Convert.ToString((long)EndpointType.SuccessFactor)));

                var events = new string[] { "Select",
            "On Board",
            "Off Board",
            "Transfer",
            "Re-Org", "ReHire", "Propagation", "Promotion/Demotion" };

                foreach (var eve in events)
                    ddlEvents.Items.Add(eve); 

                ddlEvents.SelectedIndex = 0;
                ddlApplicationTypes.SelectedIndex = 0;
                btn.Enabled = false;
            }
        }





        protected void GetData_Click(object sender, EventArgs e)
        {
            var pernr = txtPERNR.Text;

            if (string.IsNullOrWhiteSpace(pernr))
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Enter the PERNR");
                return;
            }

            if (ddlTableNames.SelectedValue == "Select")
            {
                MasterPage.ShowAlert(Infrastructure.AlertType.Error, "Select the Table Name.");
                return;
            }

            GetHRData(pernr);
            //HRMSTableDataGrid.DataBind();
        }

        private void GetHRData(string pernr)
        {
            string epTypeId = "0";

            if (ddlApplicationTypes.SelectedItem != null)
            {
                epTypeId = ddlApplicationTypes.SelectedItem.Value;
            }
            //pernr, string eventType, string tableName
            var result =
                DomainServiceClient.GetByAction<DataTable>
                   ("EndPointService", "GetHRMSDetail", new Dictionary<string, string> {
                       { "pernr",pernr},
                       { "tableName",ddlTableNames.SelectedItem.ToString() },
                       { "eventType", string.Empty},
                      {"endpointSystemId", epTypeId}
                   });

             

            HRMSTableDataGrid.DataSource = result;
            HRMSTableDataGrid.DataBind();
        }

        protected void sapuserselectionchanged(object sender, EventArgs e)
        {
            string epTypeId = "0";

            if (ddlApplicationTypes.SelectedItem != null)
            {
                epTypeId = ddlApplicationTypes.SelectedItem.Value;
            }

            if (ddlEvents.SelectedValue == "Select" )
            { 
                DataTable dt = new DataTable();
                DataColumn dc = null;

                dc = new DataColumn();
                dc.ColumnName = "EventType";
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);

                dc = new DataColumn();
                dc.ColumnName = "EventReason";
                dc.DataType = typeof(string);
                dt.Columns.Add(dc);
                HRMSEventDataGrid.DataSource = null;
                HRMSEventDataGrid.DataSource = dt;
                return;
            }
            var result =
              DomainServiceClient.GetByAction<DataTable>
                 ("EndPointService", "GetHRMSDetail", new Dictionary<string, string> {
                      { "pernr",string.Empty},
                      { "tableName",string.Empty},
                      { "eventType", ddlEvents.SelectedItem.ToString() },
                      {"endpointSystemId", epTypeId} 
                 });
            //RadGrid2.DataSource = null;
             
            HRMSEventDataGrid.DataSource = result;
            HRMSEventDataGrid.DataBind();
        }

        protected void HRMSEventDataGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.IsPostBack)
            {
                string epTypeId = "0";

                if (ddlApplicationTypes.SelectedIndex>0 && ddlApplicationTypes.SelectedItem != null)
                {
                    epTypeId = ddlApplicationTypes.SelectedItem.Value;
                }

                if (ddlEvents.SelectedValue == "Select")
                {
                    DataTable dt = new DataTable();
                    DataColumn dc = null;

                    dc = new DataColumn();
                    dc.ColumnName = "EventType";
                    dc.DataType = typeof(string);
                    dt.Columns.Add(dc);

                    dc = new DataColumn();
                    dc.ColumnName = "EventReason";
                    dc.DataType = typeof(string);
                    dt.Columns.Add(dc);
                    HRMSEventDataGrid.DataSource = null;
                    HRMSEventDataGrid.DataSource = dt;
                    return;
                }

                var result =
                 DomainServiceClient.GetByAction<DataTable>
                    ("EndPointService", "GetHRMSDetail", new Dictionary<string, string> {
                      { "pernr",string.Empty},
                      { "tableName",string.Empty},
                      { "eventType", ddlEvents.SelectedItem.ToString()},
                      {"endpointSystemId", epTypeId}
                    });
                //RadGrid2.DataSource = null;
                HRMSEventDataGrid.DataSource = result;
            }

        }

        protected void HRMSTableDataGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.IsPostBack)
            {
                var pernr = txtPERNR.Text;
                if (!string.IsNullOrWhiteSpace(pernr) && ddlTableNames.SelectedValue != "Select")
                {
                    GetHRData(pernr);
                }
            }
        }

        protected void ddlApplicationTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlApplicationTypes.SelectedIndex > 0 && ddlApplicationTypes.SelectedItem!=null)
            {
                string[] tableNames = new string[] { };
                var ep = DomainApiClient<Dto.EndPoint>.GetByAction<Dto.EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByEndpointTypeAndTrustedSource,
                    new Dictionary<string, string> { { "endpointTypeID", ddlApplicationTypes.SelectedItem.Value.ToString() },{"isTrustedSource", Boolean.TrueString } });

                if (ep == null)
                {
                    ddlTableNames.Items.Clear();
                    ddlTableNames.SelectedIndex = -1;
                    return;
                }

                if (ep.EndpointSystemTypeId.HasValue && ep.EndpointSystemTypeId.Value == (long)Infrastructure.Enums.EndpointType.SAP)
                {
                    tableNames = new string[] {
            "Select",
            "PA0000",
            "PA0001",
            "PA0002",
            "PA0302"};
                }

                if (ep.EndpointSystemTypeId.HasValue && ep.EndpointSystemTypeId.Value == (long)Infrastructure.Enums.EndpointType.SuccessFactor)
                {
                    tableNames = new string[] {
            "Select",
           "Address Details",
            "Contact Details",
            "Email Details",
            "Personal Details",
            "Employeement Details",
                "Event Details"};
                }

                foreach (var tablename in tableNames)
                    ddlTableNames.Items.Add(tablename);

                ddlTableNames.SelectedIndex = 0;
                btn.Enabled = true;
            }
            else
            {
                ddlTableNames.Items.Clear();
                ddlTableNames.SelectedIndex = -1;
                btn.Enabled = false;
            }

        }
    }
}