using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Dto.Presentation;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI.WebControls;
using DI.Chakra.Web.UsrControl.Common;


namespace DI.Chakra.Web.Attestation.List
{
    public partial class AttestationListUC : UIBaseUC, IAccessControl
    {
        private static IList<PresentationListAttestation> list;
        //private string strApplicationtype = string.Empty;
        private int intApplicationtype;
        private EndPointSystemType _mEndPointSystemType = null;
       // public delegate void ApplicationIndexChanged(EndPointSystemType EndPointSystemType);
       // public event ApplicationIndexChanged ApplicationChanged;
        public IList<PresentationListAttestation> AttestationList
        {
            get
            {
                if (ViewState["AttestationList"] == null)
                    return new List<PresentationListAttestation>();
                return ViewState["AttestationList"] as List<PresentationListAttestation>;
            }
            set
            {
                ViewState["AttestationList"] = value;
            }
        }

        public Dto.EndPointSystemType SelectedApplication
        {
            get
            {
                if (ddlAttestationAppTypeField.SelectedIndex <= 0)
                    return null;
                _mEndPointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetById(Convert.ToInt64(ddlAttestationAppTypeField.SelectedValue));

                return _mEndPointSystemType;
            }
            set
            {
                _mEndPointSystemType = value;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemTypeName", _mEndPointSystemType == null ? "0" : _mEndPointSystemType.EndpointSystemTypeName.ToString());
                _mEndPointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByEndpointSystemTypeName, parameter);

                ddlAttestationAppTypeField.SelectedValue = _mEndPointSystemType != null ? Convert.ToString(Convert.ToInt64(_mEndPointSystemType.Id)) : "";
            }
        }

        public string MethodName
        {
            get; set;
        }

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                //EndpointUC.Initialize();
                BindApplicationType();
                if (!string.IsNullOrWhiteSpace(this.Request.QueryString["applicationType"]))
                {
                    //DropDownList ddlApplicationField = (DropDownList)EndpointUC.FindControl("ApplicationTypeField");

                    if (ddlAttestationAppTypeField != null)
                    {
                        foreach (ListItem item in ddlAttestationAppTypeField.Items)
                        {
                            if (item.Value.Equals(this.Request.QueryString["applicationType"]))
                            {
                                item.Selected = true;
                                break;
                            }
                        }
                    }
                    
                }
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Delete");
                delete();
            }
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV || e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                var data = GetAttestationData();
                if (data.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(grdAttestation.MasterTableView.FilterExpression))
                        data = data.AsEnumerable().AsQueryable().Where(grdAttestation.MasterTableView.FilterExpression).ToList();
                    if (grdAttestation.MasterTableView.SortExpressions != null && grdAttestation.MasterTableView.SortExpressions.Count > 0)
                    {
                        data = data.AsEnumerable().AsQueryable().OrderBy(GetSortExpression(grdAttestation.MasterTableView.SortExpressions)).ToList();
                    }
                    GenericExport(data, e.ActionType, "Attestations");
                }
                else
                {
                    MasterPage.ShowAlert(AlertType.Error, "No Records found for export");
                }
            }
        }

        protected void grdAttestation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (grdAttestation.SelectedItems.Count > 0)
            {
                GridDataItem g = grdAttestation.SelectedItems[0] as GridDataItem;
                this.ContextObjectId = (long)g.GetDataKeyValue("AttestationId");
            }
        }

        protected void grdAttestation_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            BindAttestationList();
        }

        protected void grdAttestation_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            grdAttestation.FilterMenu.Skin = "IACMSkinFilter";
        }

        protected void btnCreateAttestation_Click(object sender, EventArgs e)
        {
            DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Create");
            Response.Redirect("~/Admin/Attestation/Add.aspx");
        }

        protected void btnAttestationCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Attestation/List.aspx");
        }

        protected void EndpointTypeUC_OnEndpointTypeChanged(EndPointSystemType EndPointSystemType)
        {
            grdAttestation.Rebind();
        }

        protected void btncancel_Click(object sender, EventArgs e)
        {
            //DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Admin/Attestation - Cancel");
            Response.Redirect("~/Home/Home.aspx");
        }

        protected void ddlAttestationAppTypeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            grdAttestation.Rebind();
            //if (ApplicationChanged != null)
                //ApplicationChanged(SelectedApplication);
            
        }
        #endregion

        #region Methods
        private void BindAttestationList()
        {
           // DropDownList ddlApplicationField = (DropDownList)EndpointUC.FindControl("ApplicationTypeField");

            if (ddlAttestationAppTypeField != null)
            {
                intApplicationtype = Convert.ToInt32(ddlAttestationAppTypeField.SelectedItem.Value);
            }
            list = GetAttestationData();
            grdAttestation.DataSource = list;
            grdAttestation.VirtualItemCount = list.Count;
            //foreach (GridColumn col in grdAttestation.Columns)
            //{
            //    if (col.UniqueName == "EndPointSystem")
            //        col.HeaderText = T("App.Common.Endpoint");
            //}

            var queryString = string.Empty;
            if (intApplicationtype > 0)
            {
                queryString = $"&applicationType={intApplicationtype}";
            }
            else
            {
                queryString = $"&applicationType=0";
            }

            this.MasterPage.AdditionalQueryString = queryString;
        }

        private IList<PresentationListAttestation> GetAttestationData()
        {
            long EndPointSystemTypeId = SelectedApplication != null ? Convert.ToInt64(SelectedApplication.Id) : 0;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemTypeId", EndPointSystemTypeId.ToString());
            return DomainApiClient<Dto.Presentation.PresentationListAttestation>.
                GetByAction<IList<PresentationListAttestation>>(DomainApiAction.Attestation.Presentation.PresentationListAttestation.
                GetAllByEnpointSystemTypeId, parameter);
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select an item.");
                return;
            }
            try
            {
                if (!AttestationExistInTask())
                {
                    bool isAttProcessExist;
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("attesationId", ContextObjectId.Value.ToString());
                    isAttProcessExist= DomainApiClient<Dto.Attestation>.GetByAction<bool>(DomainApiAction.Attestation.DeleteAllNavigation, parameter);
                    if(isAttProcessExist)
                    MasterPage.ShowAlert(AlertType.Success, "Attestation Deleted Successfully");
                    else
                    MasterPage.ShowAlert(AlertType.Error, "Attestation Job Exists for this Process ");
                    ContextObjectId = null;
                    grdAttestation.Rebind();
                }
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "Attestation cannot be Deleted");
            }
        }

        private bool AttestationExistInTask()
        {
            bool attIdExists = false;
            try
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("attestationId", this.ContextObjectId.Value.ToString());
                attIdExists = DomainApiClient<Dto.Task>.GetByAction<bool>(DomainApiAction.Tasks.Task.DoesAttestationTasksAlreadyExists, parameter);
                if (attIdExists)
                {
                    MasterPage.ShowAlert(AlertType.Error, "Attestation is in progress, so cannot be Deleted");
                }
            }
            catch
            {
            }
            return attIdExists;
        }
        protected override IDMSReportParam GetReportHeaderColumnMapping()
        {
            var param = new Dto.IDMSReportParam();
            param.ReportDataColumns = new List<Dto.IDMSReportDataColumn>();
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Attestation Name", "AttestationName", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Description", "AttestationDescription", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Endpoint Systems", "EndPointSystem", null)); 
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Remediation Enabled", "AttestationRemediation", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Split Campaign By", "SpilitCampaignScope", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created Date", "CreatedDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Created By", "CreatedBy", null));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Last Modified Date", "LastModifiedDate", "dd-MM-yyyy hh:mm tt"));
            param.ReportDataColumns.Add(new Dto.IDMSReportDataColumn("Last Modified By", "LastModifiedBy", null));
            return param;
        }

        private void BindApplicationType()
        {
            try
            {

                ddlAttestationAppTypeField.Items.Clear();

                var data = DomainApiClient<Dto.EndPointSystemType>.GetByAction<List<EndPointSystemType>>
                    (DomainApiAction.Endpoint.EndPointSystemType.EndPointSystemMethod(MethodName), null);
                data = data.OrderBy(i => i.DisplayName).ToList();
                ddlAttestationAppTypeField.DataSource = data;
                ddlAttestationAppTypeField.DataBind();
                ddlAttestationAppTypeField.Items.Insert(0, new ListItem( "--ALL--","0"));

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}