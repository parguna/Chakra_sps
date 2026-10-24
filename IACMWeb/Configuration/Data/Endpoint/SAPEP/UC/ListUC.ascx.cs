// To Do : IMPLEMENT XLDataValidator.GetXLDataValidator IN DOMAIN MODEL AND CONTROLLER
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;

namespace DI.Chakra.Web.Configuration.Data.XLUpload.SAPEP.UC
{
    public partial class ListUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private static IList<SAPEPXLProvisioningMatrix> inValidList;
        string successMessage = "File uploaded sucessfully";
        AlertType alertType = AlertType.Success;
        string failureMessage = "Uploaded file has error, check the Validation Message column";

        #endregion
            
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            XLUploadUC1.TemplateName = XLTemplates.SAPEP_PROVISIONINGMATRIX;
            this.MasterPage.ActionClicked += ActionClicked;
            if (!Page.IsPostBack)
            {
                inValidList = new List<SAPEPXLProvisioningMatrix>();
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
                grdEntries.ExportSettings.FileName = "ProvisioningMatrix_SAPEP";
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
                    inValidList = new List<Dto.SAPEPXLProvisioningMatrix>();
                    foreach (var data in invalid)
                    {
                        Dto.SAPEPXLProvisioningMatrix catalog = new Dto.SAPEPXLProvisioningMatrix();
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

        protected void ValidRecordsSelectionChanged(object sender, EventArgs e)
        {
            grdEntries.Rebind();
        }

        protected void grdEntries_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            FillDataSouce();
        }

        protected void grdEntries_ItemCommand(object sender, GridCommandEventArgs e)
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
                GridEditableItem editItem = (GridEditableItem)e.Item;
                SAPEPXLProvisioningMatrix matrix = new SAPEPXLProvisioningMatrix();
                matrix.CompanyCode = (editItem["CompanyCode"].Controls[0] as TextBox).Text;
                matrix.DepartmentCode = (editItem["DepartmentCode"].Controls[0] as TextBox).Text;
                matrix.DivisionCode = (editItem["DivisionCode"].Controls[0] as TextBox).Text;
                matrix.OrganisationId = (editItem["OrganisationId"].Controls[0] as TextBox).Text;
                matrix.OrganisationName = (editItem["OrganisationName"].Controls[0] as TextBox).Text;
                matrix.CostCenterCode = (editItem["CostCenterCode"].Controls[0] as TextBox).Text;
                matrix.CostDistributionCode = (editItem["CostDistributionCode"].Controls[0] as TextBox).Text;
                matrix.OU = (editItem["OU"].Controls[0] as TextBox).Text;
                matrix.DefaultGroups = (editItem["DefaultGroups"].Controls[0] as TextBox).Text;
                matrix.Id = Convert.ToInt32(editItem.GetDataKeyValue("Id").ToString());

                string isEntryHasAnyError = DomainApiClient<Dto.SAPEPXLProvisioningMatrix>.GetByAction<string>
                    (DomainApiAction.Data.SAPEPXLProvisioningMatrix.IsValidEntry, null, matrix);

                if(!string.IsNullOrEmpty(isEntryHasAnyError))
                {
                    MasterPage.ShowAlert(AlertType.Error, isEntryHasAnyError);
                    return;
                }

                matrix.ValidFrom = DateTime.Now;
                matrix.ValidTo = DateTime.MaxValue;

                DomainApiClient<Dto.SAPEPXLProvisioningMatrix>.Save(matrix);
                MasterPage.ShowAlert(AlertType.Success, "SAPEPXL ProvisioningMatrix Updated Successfully");
                grdEntries.Rebind();                
            }
            else if (e.CommandName == RadGrid.DeleteCommandName)
            {
                GridDataItem item = (GridDataItem)e.Item;
                long Proid = (long)item.GetDataKeyValue("Id");
                DomainApiClient<Dto.SAPEPXLProvisioningMatrix>.DeleteById(Proid);
                grdEntries.Rebind();
                this.ContextObjectId = null;
                MasterPage.ShowAlert(AlertType.Success, "SAPEPXL ProvisioningMatrix deleted successfully");
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
        #endregion

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

                IList<SAPEPXLProvisioningMatrix> validList =
                    DomainApiClient<Dto.SAPEPXLProvisioningMatrix>.GetByAction<IList<SAPEPXLProvisioningMatrix>>
                    (DomainApiAction.Data.SAPEPXLProvisioningMatrix.GetAll, parameter);

                grdEntries.DataSource = validList;
                grdEntries.MasterTableView.Columns.FindByUniqueName("ValidationMessage").Display = false;
            }
        }
        #endregion 
    }
   }

