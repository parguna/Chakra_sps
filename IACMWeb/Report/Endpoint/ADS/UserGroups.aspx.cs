using System;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System.Web.UI.WebControls;
using System.Web;

namespace DI.Chakra.Web.Report.Endpoint.ADS
{
    public partial class UserGroups : ReportBasePage
    {
        #region Members
        private static IList<PresentationADSUserToGroups> list = new List<PresentationADSUserToGroups>();
        private string V_ENDPOINTSYSTEMID = "V_ENDPOINTSYSTEMID";
        #endregion

        #region Property
        private long EndPointSystemID
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEMID] != null)
                {
                    return (long)ViewState[V_ENDPOINTSYSTEMID];
                }
                return 0;
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEMID] = value;
            }
        }
        #endregion

        #region Events
        protected new void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            reportHistoryId = Request["reportHistoryId"];
            if (!Page.IsPostBack)
            {
                if (reportHistoryId != null)
                {
                    try
                    {
                        base.Page_Load(sender, e);
                        string endpointsystemtypeid = string.Empty;
                        parameter.TryGetValue("EndPointSystemId", out endpointsystemtypeid);
                        parameter.Clear();
                        parameter.Add("endpointsystemid", endpointsystemtypeid);
                        list = DomainApiClient<PresentationADSUserToGroups>.GetByAction<List<PresentationADSUserToGroups>>
                    (DomainApiAction.Endpoint.Presentation.AD.GetADSUserToGroupsReportData, parameter);

                        grdUser.DataSource = list;
                        ServiceExport(grdUser, fileformat);
                    }
                    catch (Exception ex)
                    { }
                }


                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =
                    new EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.ActiveDirectory };

                Dictionary<string, string> repparameter = new Dictionary<string, string>();
                repparameter.Add("id", EndpointUC.SelectedApplication.Id.ToString());
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, repparameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    MasterPage.ShowAlert(AlertType.Error, "First Create AD Endpoint");
                }
               
            }
        }

        protected void ExportGrid(ACLAction action)
        {
            try
            {
                HttpContext.Current.Session["ExportStatus"] = "false";
                var RadGrid1 = new RadGrid();

                //remove the below code to dump all the columns, for now added the needed columns.
                RadGrid1.AutoGenerateColumns = false;

                GridBoundColumn boundColumn = new GridBoundColumn();
                boundColumn.DataField = "UserName";
                boundColumn.HeaderText = "User Name";
                RadGrid1.MasterTableView.Columns.Add(boundColumn);
                boundColumn = new GridBoundColumn();
                boundColumn.DataField = "DisplayName";
                boundColumn.HeaderText = "Display Name";
                RadGrid1.MasterTableView.Columns.Add(boundColumn);
                boundColumn = new GridBoundColumn();
                boundColumn.DataField = "GroupName";
                boundColumn.HeaderText = "Group Name";
                RadGrid1.MasterTableView.Columns.Add(boundColumn);
                boundColumn = new GridBoundColumn();
                boundColumn.DataField = "GroupDescription";
                boundColumn.HeaderText = "Group Description";
                RadGrid1.MasterTableView.Columns.Add(boundColumn);
                //remove the above code to dump all the columns, for now added the needed columns.

                RadGrid1.ID = string.Format("{0}_{1}", "Report-ADS-UserGroups", DateTime.Now.Date.ToString());
                RadGrid1.DataSource = list;
                RadGrid1.Rebind();
                Page.Controls.Add(RadGrid1); // It will not add control in your page

                RadGrid1.ExportSettings.FileName = "Report-ADS-UserGroups";
                RadGrid1.ExportSettings.IgnorePaging = true;
                RadGrid1.ExportSettings.ExportOnlyData = true;
                RadGrid1.ExportSettings.OpenInNewWindow = true;
                RadGrid1.ExportSettings.HideStructureColumns = true;
                RadGrid1.ExportSettings.SuppressColumnDataFormatStrings = true;

                RadGrid1.GridLines = GridLines.Both;

                if (action == ACLAction.Excel)
                    RadGrid1.MasterTableView.ExportToExcel();
                else if (action == ACLAction.CSV)
                    RadGrid1.MasterTableView.ExportToCSV();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }

        protected void DropDownSelectectionChanged(EndPoint EndPoint)
        {
            try
            {
                EndPointSystemID = EndPoint == null ? 0 : EndPoint.Id;
                grdUser.Rebind();
            }
            catch (Exception ex)
            {
            }
        }

        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (EndPointSystemID > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointsystemid", EndPointSystemID.ToString());
                list = DomainApiClient<PresentationADSUserToGroups>.GetByAction<List<PresentationADSUserToGroups>>
                    (DomainApiAction.Endpoint.Presentation.AD.GetADSUserToGroupsReportData, parameter);
                grdUser.DataSource = list;
            }
            else
            {
                list = new List<PresentationADSUserToGroups>();
                grdUser.DataSource = list;
            }
        }

        protected void grdUser_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            e.DetailTableView.DataSource = grdUser.DataSource;
        }


        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.ExcelAll)
            {
                ExportGrid(ACLAction.Excel);
            }
            else if (e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.CSVAll)
            {
                ExportGrid(ACLAction.CSV);
            }
        }
        #endregion

    }
}   