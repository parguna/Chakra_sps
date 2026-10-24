using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Web.Report.MySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using Application;
using DI.Chakra.Dto.Core;
using System.Text.RegularExpressions;

namespace DI.Chakra.Web.Report.Attestation
{
    public partial class AttestationCampaignReport : ReportBasePage, IContextObjectEnabled, IAccessControl
    {
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
        private static IList<AttestationCampaign> campaignReportList = new List<AttestationCampaign>();
        private const string V_id = "id";

        protected void Page_Load(object sender, EventArgs e)
        {            
            this.MasterPage.ActionClicked += ActionClicked;
            SessionUser.RedirectFrom = "AttestationCampaignReport";
        }
        
        protected void btnPageCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void CampaignReportGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            int PageIndex = CampaignReportGrid.CurrentPageIndex;
            int PageSize = CampaignReportGrid.MasterTableView.PageSize;
            int startRowIndex = PageIndex * PageSize;
            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                StartRowIndex = startRowIndex,
                MaximumRows = PageSize,
                FilterExpression = GetFilterExpression(CampaignReportGrid.MasterTableView.FilterExpression),
                SortExpression = GetSortExpression(CampaignReportGrid.MasterTableView.SortExpressions)
            };

            var list = DomainApiClient<Dto.Presentation.PresentationAttetstationCampaign>
             .GetByAction<PresentationGridPagedData<Dto.Presentation.PresentationAttetstationCampaign>>("GetPagedCampaignReport", null, param);

            CampaignReportGrid.VirtualItemCount = list.TotalCount;
            CampaignReportGrid.DataSource = list.Data;
            
        }

        private long CampaignId
        {
            get
            {
                if (ViewState["CampaignId"] != null)
                {
                    return Convert.ToInt64(ViewState["CampaignId"]);
                }
                return 0;
            }
            set
            {
                ViewState["CampaignId"] = value;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //report = RptSchd.GetReportingSchedule();
                //scheduler = RptSchd.Scheduler;
                //attributeParameter.Add("EndPointReport", "Users");
                //if (Save())
                //{
                //    RptSchd.ClearpopupData();
                //    usrModal.VisibleOnPageLoad = false;
                //}
            }
            catch (System.Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Schduler is not filled with schedule values");
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            usrModal.VisibleOnPageLoad = false;
            //RptSchd.InitializeSched();
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            //if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            //{
            //    CampaignReportGrid.ExportSettings.FileName = "CampaignReport";
            //    Export(CampaignReportGrid, e.ActionType);
            //}
            //else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            //{
            //    CampaignReportGrid.Export<IList<AttestationCampaign>>(campaignReportList, e.ActionType, "CampaignReport");
            //}
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                int PageIndex = CampaignReportGrid.CurrentPageIndex;
                int PageSize = -1;
                int startRowIndex = PageIndex * PageSize;
                PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
                {
                    StartRowIndex = startRowIndex,
                    MaximumRows = PageSize,
                    FilterExpression = GetFilterExpression(CampaignReportGrid.MasterTableView.FilterExpression),
                    SortExpression = GetSortExpression(CampaignReportGrid.MasterTableView.SortExpressions)
                };

                var data = DomainApiClient<Dto.Presentation.PresentationAttetstationCampaign>
                 .GetByAction<PresentationGridPagedData<Dto.Presentation.PresentationAttetstationCampaign>>("GetPagedCampaignReport", null, param);
                if (data.TotalCount > 0)
                {
                    GenericExport(data.Data, e.ActionType, "AttestationCampaignReport");
                }
            }
            if (e.ActionType == ACLAction.Schedule)
            {
                // if (EndpointUC.SelectedEndpoint != null)
                {
                    usrModal.VisibleOnPageLoad = true;
                    //RptSchd.InitializeSched();
                }
                // RptSchd.RptType = EndpointUC.SelectedApplication.DisplayName.ToString() + "-User";

            }
            else if (e.ActionType == Infrastructure.Enums.ACLAction.Remark)
            {
                AttestationUploadUC.AllowAddComments = true;
                AttestationUploadUC.CampaignId = CampaignId;
                AttestationUploadUC.CurrentUser = SessionUser.User.UserName;
                AttestationUploadUC.Initialize();
                AttestationUploadUC.BindComments();
                AttestationUpload.VisibleOnPageLoad = true;
                //string script = "function f(){$find(\"" + AttestationUpload.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
                return;
            }
            else
            {
                //  MasterPage.ShowAlert(AlertType.Error, "Selected any Endpoint");
            }

        }

        protected void Save_Comment_Upload_Click(object sender, EventArgs e)
        {
            AttestationUploadUC.AllowAddComments = true;
            AttestationUploadUC.CampaignId = CampaignId;
            AttestationUploadUC.CurrentUser = SessionUser.User.UserName;
            AttestationUploadUC.SaveData();
            AttestationUpload.VisibleOnPageLoad = false;
        }
        protected void Cancel_Comment_Upload_Click(object sender, EventArgs e)
        {
            AttestationUpload.VisibleOnPageLoad = false;
        }

        protected void EmployeeGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.ToUpper() == "RowClick".ToUpper()) return;
                HttpContext.Current.Session["ExportStatus"] = "false";
                if (e.Item is GridDataItem)
                {

                    //downloadFile.Attributes["src"] = "";
                    GridDataItem ditem = (GridDataItem)e.Item;
                    string campaignId = Convert.ToString(ditem.GetDataKeyValue("AttestationCampaignId"));

                    // TODO : get report from table . add method name to domain api action
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    // cannot use this.CampaignID as the clicked button may not be on the selected row.
                    // Have to use the CampaignID from the row where the button was selected i.e. from this event
                    parameter.Add("attestationCampaignId", Convert.ToString(campaignId));

                    string filename = ditem["SnapshotFileName"].Text; // get the filename from the row in which the download button is clicked   

                    if (e.CommandName == "DownloadExcelFlat")
                    {
                        parameter.Add("reportType", AttestationReportType.ExcelFlat.ToString());
                        filename = string.Format("{0}_Flat.xlsx", filename);
                    }
                    else if (e.CommandName == "DownloadPdf")
                    {
                        parameter.Add("reportType", AttestationReportType.PDF.ToString());
                        filename = string.Format("{0}.pdf", filename);
                    }
                    else if (e.CommandName == "DownloadCsv")
                    {
                        parameter.Add("reportType", AttestationReportType.CSV.ToString());
                        filename = string.Format("{0}.csv", filename);
                    }
                    else if (e.CommandName == "DownloadExcelHierarchy")
                    {
                        parameter.Add("reportType", AttestationReportType.ExcelHierarchical.ToString());
                        filename = string.Format("{0}_Hierarchy.xlsx", filename);
                    }

                    Dto.AttestationCampaignReport attReport = DomainApiClient<Dto.AttestationCampaignReport>.GetByAction<Dto.AttestationCampaignReport>(DomainApiAction.Attestation.AttestationCampaignReport.GetReportByType, parameter);

                    if (attReport == null || (attReport.AttachmentFile != null && attReport.AttachmentFile.Length ==0))
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please wait.Report generation in progress");
                        return;
                    }

                    // Point the IFRAME to GenerateFile
                    //downloadFile.Attributes["src"] = string.Format("~/DownloadFile.aspx?filepath={0}", filename);
                    if (!string.IsNullOrWhiteSpace(attReport.FileName))
                    {
                        HttpResponse response = HttpContext.Current.Response;
                        response.Clear();
                        response.ClearContent();
                        response.ClearHeaders();
                        response.Buffer = true;
                        response.AddHeader("Content-Disposition", "attachment;filename=" + attReport.FileName);
                        response.BinaryWrite(attReport.AttachmentFile);
                        response.End();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                HttpContext.Current.Session["ExportStatus"] = "true";
            }
        }

        protected void UpdatePanelDownloadPdf_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void UpdatePanelDownloadExl_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void UpdatePanelDownloadExH_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void UpdatePanelDownloadCsv_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        
        protected void CampaignReportGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = CampaignReportGrid.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("AttestationCampaignId");
            this.CampaignId = (long)g.GetDataKeyValue("AttestationCampaignId");
        }
        protected string GetFilterExpression(string filterExpression)
        {
            if (filterExpression.Contains("DateTime.Parse"))
            {
                List<string> finalOutput = new List<string>();
                foreach (string condition in filterExpression.Split(new string[] { " AND " }, StringSplitOptions.None))
                {
                    string rawExpression = condition;
                    var findMatch = Regex.Match(rawExpression, "DateTime.Parse(.*)", RegexOptions.IgnoreCase);
                    if (findMatch.Success)
                    {
                        var dtExtract = DateTime.Parse(findMatch.Value.Split('\"')[1]);
                        var conditionDate = string.Format("DateTime({0}, {1}, {2})", dtExtract.Year, dtExtract.Month, dtExtract.Day);
                        finalOutput.Add(Regex.Replace(rawExpression, "DateTime.Parse(.*)", string.Format("{0})", conditionDate), RegexOptions.IgnoreCase));
                    }
                    else
                        finalOutput.Add(rawExpression);
                }
                filterExpression = string.Join(" AND ", finalOutput);
                return filterExpression;
            }
            return filterExpression;
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("CampaignId", "AttestationCampaignId", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Campaign Name", "CampaignName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Attestation Name", "AttestationName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Start Date", "CreatedDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("End Date", "EndDate", "dd-MM-yyyy hh:mm tt"));
            return param;
        }
    }
}