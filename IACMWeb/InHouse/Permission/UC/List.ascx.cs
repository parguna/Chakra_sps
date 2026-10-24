using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.IO;
using System.Net;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.InHouse.Permission.UC
{
    public partial class List : DI.Chakra.Web.UIBaseUC, IAccessControl
    {
        static WebClient client = null;

        #region Memebers

        AlertType alertType = AlertType.Success;
        string failureMessage = "Uploaded file has error, check the Validation Message column";
        string successMessage = "File uploaded sucessfully";
        private static IList<InhousePermission> inValidList;
        private static IList<InhousePermission> validList;
        private SessionStatusImpl Session = new SessionStatusImpl();

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            string strURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Files/CSV/Endpoint/INHROLE_TEMPLATE.csv";
            this.btnExportToExcel.NavigateUrl = strURL;
            lblDownloaded.Visible = false;
            XLUploadUC1.TemplateName = XLTemplates.Inhouse_Permissions;
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                inValidList = new List<InhousePermission>();
                rbInValid.Checked = false;
                rbValid.Checked = true;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }

        protected void ValidRecordsSelectionChanged(object sender, EventArgs e)
        {
            Permission.Rebind();
        }

        protected void Permission_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            FillDataSouce();
        }

        protected void Permission_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem Grid = Permission.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)Grid.GetDataKeyValue("Id");
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Inhouse Permission - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Upload)
            {
               modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Download)
            {
                Permission.MasterTableView.Columns.FindByUniqueName("CreatedBy").Visible = true;
                Permission.MasterTableView.Columns.FindByUniqueName("CreatedDate").Visible = true;
                Permission.MasterTableView.GetColumn("CreatedBy").Display = true;
                Permission.MasterTableView.GetColumn("CreatedDate").Display = true;
                Permission.Rebind();
                Permission.DownloadTemplate(Permission, e.ActionType); 
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                Permission.ExportSettings.FileName = "InhouseRoles";
                Permission.Export(Permission, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                if (rbInValid.Checked == true) 
                    Permission.Export<IList<InhousePermission>>(inValidList, e.ActionType, "InhouseRoles");
                else
                    Permission.Export<IList<InhousePermission>>(validList, e.ActionType, "InhouseRoles");
            }
        }

        protected void Permission_ItemDataBound(object sender, GridItemEventArgs e) 
        {
            if (e.Item is GridDataItem)
            {            
                GridDataItem item = (GridDataItem)e.Item;
                if (item != null)
                { 
                    if (item["IsActive"].Text == "True")
                        item["IsActive"].Text = "Yes";
                    else
                        item["IsActive"].Text = "No";
               }           
            }           
        }

        protected void XLUploadUC1_UserControlButtonClicked(object sender, EventArgs e)
        {
            UserControlButtonClicked(this, EventArgs.Empty);
            modalPopup.VisibleOnPageLoad = false;
        }

        private void UserControlButtonClicked(object sender, EventArgs e) 
        {
            try
            {
                IList<IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if ((invalid != null) && (invalid.Count > 0))
                {
                    inValidList = new List<Dto.InhousePermission>();
                    foreach (var data in invalid)
                    {
                        if (data != null)
                        {
                            Dto.InhousePermission inhPermission = (Dto.InhousePermission)data;
                            inValidList.Add(inhPermission);
                        }
                    }
                }
                else
                    inValidList.Clear();

                if (!(string.IsNullOrEmpty(errMsg)))
                    MasterPage.ShowAlert(AlertType.Error, errMsg);

               else
                {

                    string mssg = successMessage;
                    if (inValidList != null && inValidList.Count > 0)
                    {
                        rbInValid.Checked = true;
                        rbValid.Checked = false;
                        mssg = failureMessage;
                        alertType = AlertType.Warning;
                    }
                    else
                    {
                        rbInValid.Checked = false;
                        rbValid.Checked = true;
                    }
                    FillDataSouce();
                    Permission.Rebind();
                    MasterPage.ShowAlert(alertType, mssg);
                }               
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while uploading Excel File");
                modalPopup.VisibleOnPageLoad = false;
            }
        }

        private void BindInvalidGrid()
        {
            if (inValidList.Count > 0)
            {
                Permission.DataSource = inValidList;
                Permission.DataBind();
            }
        }

        #endregion

        #region Method

        private void FillDataSouce()
        {
            bool isValidRecords = rbValid.Checked;
            if (rbInValid.Checked == true)
            {
                Permission.DataSource = inValidList;
                Permission.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            }
            else
            {
                 validList = DomainApiClient<Dto.InhousePermission>.GetAll();
                Permission.DataSource = validList;
                Permission.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
            }
        }
        public void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();                
                parameter.Add("permissionId", this.ContextObjectId.Value.ToString());
                parameter.Add("currentUser", Session.User.DisplayName);
                string message = DomainApiClient<Dto.InhousePermission>.
                    GetByAction<string>("DeleteByPermissionId",
                    parameter);
                
                MasterPage.ShowAlert(AlertType.Success, message);
                FillDataSouce();
                Permission.DataBind();
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "InHouse Role cannot be Deleted");
            }
        }

        protected void DownloadFile(object sender, EventArgs e)
        {
        
            string strURL = Request.Url.GetLeftPart(UriPartial.Authority) + "/Files/CSV/Endpoint/INHROLE_TEMPLATE.csv";
                  
        }

        public static void DownloadFile(string urlAddress)
        {
            client = new WebClient();
            string pathUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string pathDownload = Path.Combine(pathUser, "Downloads")+"test.csv";
            client.DownloadFile(urlAddress, @""+pathDownload);

        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {

            string strFileName = "INHROLE.csv";
            string path = Server.MapPath("~/Files//CSV//Endpoint//" + strFileName);           
            if (File.Exists(path))

            {
                byte[] bts = System.IO.File.ReadAllBytes(path);
                MemoryStream mstream = new MemoryStream(bts);
                byte[] byteArray = mstream.ToArray();
                mstream.Flush();
                mstream.Close();
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=foo.csv");
                Response.AddHeader("Content-Length", byteArray.Length.ToString());
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(byteArray);
                Response.End();
            }
        }

        #endregion

        protected void IsActive_PreRender(object sender, EventArgs e)
        {
            RadComboBox rcbFilter = sender as RadComboBox;
            if (ViewState[rcbFilter.ID] != null)
            {
                rcbFilter.SelectedValue = ViewState[rcbFilter.ID].ToString();
            }
        }

        protected void IsActive_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox combo = sender as RadComboBox;
            ViewState[combo.ID] = combo.SelectedValue;
            if (combo.SelectedValue != "0")
            {
                string ColumnName = combo.ID;
                string filterExpression = "([" + ColumnName + "] = '" + combo.SelectedValue + "')";
                Permission.MasterTableView.FilterExpression = filterExpression;
            }
            else
            {
                Permission.MasterTableView.FilterExpression = string.Empty;
            }
            FillDataSouce();
            Permission.DataBind();
        }
    }
}