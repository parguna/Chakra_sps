using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using Telerik.Web.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace DI.Chakra.Web.DataManagement.DataViewer
{
    public partial class List : DI.Chakra.Web.UIBasePage
    {
        private XLDataConverterHelper xlHelper;
       

        public XLDataConverterHelper XlHelper
        {
            get
            {
                if (xlHelper == null)
                {
                    xlHelper = new XLDataConverterHelper();
                }
                return xlHelper;
            }
            set { xlHelper = value; }
        }
        private SessionStatusImpl sessionStatus;

        void SetColumns(string tableName)
        {
            var parameter = new Dictionary<string, string>();
            parameter.Add("tableName", tableName);
            var columns = DomainApiClient<Dto.GEMSRoleMatrix>.GetByAction<List<string>>(DomainApiAction.Matrix.GEMS.GetAllColumnsByTableName, parameter);
            ddlColumns.DataSource = columns;
            ddlColumns.DataBind();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sessionStatus = new SessionStatusImpl();
           
               
                XlHelper.InvisibleBetween(grdUser.FilterMenu);
                this.MasterPage.ActionClicked += ActionClicked;
                if (!IsPostBack)
                {
                    grdUser.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Data Viewer";
                    grdUser.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                    grdUser.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
                    grdUser.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                }

                if (sessionStatus.User.UserName.ToUpper() == "IACMADMIN")
                {
                    this.ExecuteQueryButton.Visible = true;
                }
                else
                {
                    this.ExecuteQueryButton.Visible = false;
                }
                //QueryTextBox.Text = "select * from di_M_User";
            }
            catch (Exception ex)
            {

            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdUser.ExportSettings.FileName = "ResultData";
                Export(grdUser, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdUser.ExportSettings.FileName = "ResultData";
                Export(grdUser, e.ActionType);
            }
        }

        DataTable GetDataTableByQuery()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("query", QueryTextBox.Text.Trim());
            var data = DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetByAction<DataTable>(DomainApiAction.ReportDefinitions.GetDataTableExecuteQuery, parameters);
            return data;
        }

        void ExecuteQuery()
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("query", QueryTextBox.Text.Trim());
            DomainApiClient<Dto.ADSXLProvisioningMatrix>.GetByAction<Dto.ADSXLProvisioningMatrix>(DomainApiAction.ReportDefinitions.ExecuteQuery, parameters);
        }

        protected void RetreiveDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                grdUser.VirtualItemCount = 0;
                grdUser.DataSource = null;
                grdUser.DataBind();
                string query = this.QueryTextBox.Text.Trim();

                if (!query.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                {
                    this.ErrorMessage("Query must start with select clause.", true, true);
                    return;
                }

                if (securitycheck(query) && sessionStatus.User.UserName.ToUpper() != "IACMADMIN")
                {
                    this.ErrorMessage("You are authorized to run select query only.", true, true);
                    return;
                }


                

                var data = GetDataTableByQuery();
                
                if (data != null && data.Rows.Count > 0)
                {
                    grdUser.AllowPaging = true;
                    grdUser.VirtualItemCount = data.Rows.Count;
                    grdUser.DataSource = data;
                    grdUser.DataBind();
                    this.ErrorInfo("Data retreived successfully.", true, true);
                }
                else
                {
                    grdUser.VirtualItemCount = 0;
                    grdUser.DataSource = null;
                    grdUser.DataBind();
                    this.ErrorMessage("Data retrieval is failed.check your query statement.", true, true);
                }

            }
            catch (Exception ex)
            {
                this.ErrorMessage(ex.Message, true, true);
            }
        }


        protected void ExecuteQueryButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteQuery();
                grdUser.VirtualItemCount = 0;
                grdUser.DataSource = null;
                grdUser.DataBind();

                this.ErrorInfo("Data updated successfully.", true, true);
            }
            catch (Exception ex)
            {
                this.ErrorMessage(ex.Message, true, true);
            }
        }

        private void ErrorMessage(string error, bool ErrorInfo, bool imageErr)
        {
            imgerror.Visible = imageErr;
            lblerror.Visible = ErrorInfo;
            lblerror.Text = error;
            imginfo.Visible = false;
            lblinfo.Visible = false;
            lblinfo.Text = string.Empty;
        }

        private void ErrorInfo(string info, bool ErrorInfo, bool imageinfo)
        {
            imginfo.Visible = imageinfo;
            lblinfo.Visible = ErrorInfo;
            lblinfo.Text = info;
            imgerror.Visible = false;
            lblerror.Visible = false;
            lblerror.Text = string.Empty;
        }

        protected void grdUser_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            try
            {
                string query = this.QueryTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(query))
                    return;

                if (!query.StartsWith("select", StringComparison.CurrentCultureIgnoreCase))
                {
                    return;
                }

                var data = GetDataTableByQuery();
                
                if (data != null && data.Rows.Count > 0)
                {
                    grdUser.VirtualItemCount = data.Rows.Count;
                    grdUser.DataSource = data;
                }

                this.ErrorInfo("Data retreived successfully.", true, true);
            }
            catch (Exception ex)
            {
                this.ErrorMessage(ex.Message, true, true);
            }
        }

        protected void ddlTables_EntryAdded(object sender, AutoCompleteEntryEventArgs e)
        {
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count == 2)
            {
                box.Entries.Remove(box.Entries[1]);
            }
            if (box.Entries.Count == 1)
            {
                var tableName = box.Entries[0].Value;
                SetColumns(tableName);
            }
        }

        protected void ddlTables_EntryRemoved(object sender, AutoCompleteEntryEventArgs e)
        {
            RadAutoCompleteBox box = (RadAutoCompleteBox)sender;
            if (box.Entries.Count == 0)
            {
                ddlColumns.Items.Clear();
            }
        }

        protected void btnBuildQuery_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlTables.Text))
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select the table");
                return;
            }
            var query = $"SELECT * FROM {ddlTables.Text}";
            if (ddlColumns.CheckedItems.Count > 0)
            {
                var strColumns = string.Join(",", ddlColumns.CheckedItems.Select(p => p.Text));
                QueryTextBox.Text = query.Replace("*", strColumns);
            }
            else
            {
                QueryTextBox.Text = query;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminUI/List.aspx");
        }
        
        public bool securitycheck(String sqlQuery)
        {
            string[] badSqlList = new string[] {"create table","alter table","insert into",
                                                "truncate table","delete from","drop table",
                                                "update ","grant ","revoke ",
                                                "dbcc ","exec ", "execute"};
           
                if (badSqlList.Any(r => sqlQuery.IndexOf(r, StringComparison.InvariantCultureIgnoreCase) >= 0))
                {
                    return true;
                }
           
            return false;
        }

    }
}