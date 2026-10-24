// To Do : IMPLEMENT XLDataValidator.GetXLDataValidator IN DOMAIN MODEL AND CONTROLLER

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Configuration.Data.Endpoint.SAPEP.SAPEPCatalog.UC
{
    public partial class ListUC : UIBaseUC
    {
        #region Members
      
        private static IList<Dto.SAPEPCatalog> inValidList;
        string successMessage = "File uploaded sucessfully";
        AlertType alertType = AlertType.Success;
        string failureMessage = "Uploaded file has error, check the Validation Message column";
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            XLUploadUC1.TemplateName = XLTemplates.SAPEP_CATALOG;
            this.MasterPage.ActionClicked += ActionClicked;

            if (!IsPostBack)
            {
                inValidList = new List<Dto.SAPEPCatalog>();
                rbInValid.Checked = false;
                rbValid.Checked = true;
            }
        }
        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Upload)
            {
                modalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Download)
            {
                grdEntries.ExportSettings.FileName = "Catalog_SAPEP";
                DownloadTemplate(grdEntries, e.ActionType);
            }
        }

        private void UserControlButtonClicked(object sender, EventArgs e)
        {
            try
            {
                IList<IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if (invalid != null)
                {
                    inValidList = new List<Dto.SAPEPCatalog>();
                    foreach (var data in invalid)
                    {
                        Dto.SAPEPCatalog catalog = new Dto.SAPEPCatalog();
                        catalog.Id = data.Id;
                        catalog.ValidationMessage = data.ValidationMessage;
                        catalog.ValidFrom = data.ValidFrom;
                        catalog.ValidTo = data.ValidTo;
                        catalog.IsValid = data.IsValid;
                        inValidList.Add(catalog);
                    }
                }

                
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
                    grdEntries.Rebind();
                    
                    MasterPage.ShowAlert(alertType, mssg);
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while uploading Excel File");
            }

        }

        private void XLUploadUC1_UserControlButtonClicked1(object sender, EventArgs e)
        {
            try
            {
                IList<IXLData> invalid = XLUploadUC1.InvalidList;
                string errMsg = XLUploadUC1.ErrorMessage;
                if (invalid != null) inValidList = (IList<Dto.SAPEPCatalog>)(invalid.Select<IXLData, Dto.SAPEPCatalog>(item => (Dto.SAPEPCatalog)item).ToList());

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
                    MasterPage.ShowAlert(alertType, mssg);
                    grdEntries.Rebind();
                }
            }
            catch (Exception ex)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while uploading Excel File");
            }
        }
        protected void grdEntries_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            FillDataSouce();
        }
        protected void ValidRecordsSelectionChanged(object sender, EventArgs e)
        {
            grdEntries.Rebind();
        }
        #endregion

        protected void grdEntries_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (rbInValid.Checked) return;

            if (e.CommandName == RadGrid.ExportToExcelCommandName)
            {
                foreach (GridHeaderItem item in grdEntries.MasterTableView.GetItems(GridItemType.Header))
                {
                    foreach (GridBoundColumn col in grdEntries.MasterTableView.Columns)
                    {
                        if (col.UniqueName != string.Empty)
                            grdEntries.MasterTableView.GetColumn(col.UniqueName).HeaderText = col.UniqueName;
                    }
                }
                grdEntries.ExportSettings.OpenInNewWindow = true;
                grdEntries.ExportSettings.IgnorePaging = true;
                grdEntries.MasterTableView.ExportToExcel();
            }
            else if (e.CommandName == RadGrid.UpdateCommandName)
            {
                try
                {
                    GridEditableItem edititem = (GridEditableItem)e.Item;
                    Dto.SAPEPCatalog _sapepcatalog = new Dto.SAPEPCatalog();
                    _sapepcatalog.Application = (edititem["Application"].Controls[0] as TextBox).Text;
                    _sapepcatalog.BusinessRole = (edititem["BusinessRole"].Controls[0] as TextBox).Text;
                    _sapepcatalog.TechnicalRole = (edititem["TechnicalRole"].Controls[0] as TextBox).Text;
                    _sapepcatalog.Description = (edititem["Description"].Controls[0] as TextBox).Text;
                    _sapepcatalog.Level1Approver = (edititem["Level1Approver"].Controls[0] as TextBox).Text;
                    _sapepcatalog.Level1ApproverType = (ApprovalUserType)Enum.Parse(typeof(ApprovalUserType), (edititem["Level1ApproverType"].Controls[0] as TextBox).Text.ToString());
                    _sapepcatalog.Level2Approver = (edititem["Level2Approver"].Controls[0] as TextBox).Text;
                    _sapepcatalog.Level2ApproverType = (ApprovalUserType)Enum.Parse(typeof(ApprovalUserType), (edititem["Level2ApproverType"].Controls[0] as TextBox).Text.ToString());
                    _sapepcatalog.Level3Approver = (edititem["Level3Approver"].Controls[0] as TextBox).Text;
                    _sapepcatalog.Level3ApproverType = (ApprovalUserType)Enum.Parse(typeof(ApprovalUserType), (edititem["Level3ApproverType"].Controls[0] as TextBox).Text.ToString());
                    _sapepcatalog.Id = Convert.ToInt32(edititem.GetDataKeyValue("Id").ToString());

                    _sapepcatalog.ValidFrom = DateTime.Now;
                    _sapepcatalog.ValidTo = DateTime.MaxValue;

                    Dictionary<string, string> param = new Dictionary<string, string>();
                    param.Add("applicationCode", _sapepcatalog.Application.Trim());
                    EndPoint endpoint = DomainApiClient<EndPoint>.GetByAction<EndPoint>
                        (DomainApiAction.Endpoint.EndPoint.GetByApplicationCode, param);

                    if(endpoint != null && endpoint.Id > 0)
                    {
                        _sapepcatalog.EndpointSystemId = endpoint.Id;
                    }
                    else
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Application Endpoint not found.");
                        grdEntries.Rebind();
                        return;
                    }

                    string isEntryHasAnyError = DomainApiClient<Dto.SAPEPCatalog>.GetByAction<string>
                    (DomainApiAction.Data.SAPEPCatalog.IsValidEntry, null, _sapepcatalog);

                    if (!string.IsNullOrEmpty(isEntryHasAnyError))
                    {
                        MasterPage.ShowAlert(AlertType.Error, isEntryHasAnyError);
                        return;
                    }

                    DomainApiClient<Dto.SAPEPCatalog>.Save(_sapepcatalog);
                    MasterPage.ShowAlert(AlertType.Success, "SAPEP Catalog Updated Successfully");
                    grdEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
                    grdEntries.Rebind();
                }
                catch
                {
                }
            }
            else if (e.CommandName == RadGrid.DeleteCommandName)
            {
                GridDataItem item = (GridDataItem)e.Item;
                long Proid = (long)item.GetDataKeyValue("Id");
                DomainApiClient<Dto.SAPEPCatalog>.DeleteById(Proid);
                grdEntries.Rebind();
                MasterPage.ShowAlert(AlertType.Success, "SAPEP Catalog deleted successfully");
            }
        }
        protected void XLUploadUC1_UserControlButtonClicked(object sender, EventArgs e)
        {
            UserControlButtonClicked(this, EventArgs.Empty);
            modalPopup.VisibleOnPageLoad = false;
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            modalPopup.VisibleOnPageLoad = false;
        }

        #region Methods
        private void FillDataSouce()
        {
            bool isValidRecords = rbValid.Checked;
            if (rbInValid.Checked == true)
            {
                grdEntries.DataSource = inValidList;
                grdEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = true;
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("isValidRecords", isValidRecords.ToString());
                parameter.Add("isAllRecords", "true");

                IList<Dto.SAPEPCatalog> validList = 
                    DomainApiClient<Dto.SAPEPCatalog>.GetByAction<IList<Dto.SAPEPCatalog>>(DomainApiAction.Data.SAPEPCatalog.GetAll, parameter);

                grdEntries.DataSource = validList;
                grdEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
            }
        }
        #endregion
    }
}
