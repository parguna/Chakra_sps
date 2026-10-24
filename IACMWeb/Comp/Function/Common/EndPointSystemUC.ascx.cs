using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.Comp.Function.Common
{
    /// <summary>
    /// User Control responsible for capturing SODFunction Items in Compliance Function Wizard.
    /// Author : Harihara - 3/11/2015
    /// </summary>
    public partial class EndPointSystemUC : UIBaseUC
    {
        #region Class Member Variable
        public List<SODFunctionItem> PolicyTemplates;
        private SessionStatusImpl session = new SessionStatusImpl();
       // private SODFunction Function = null;
        private string V_SODFunction = "SODFunction";
        private UIMode uiMode;
        #endregion

        #region Public Property

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODFunction SodFunction
        {
            get
            {
                if (ViewState[V_SODFunction] == null)
                    return null;
                return (SODFunction)ViewState[V_SODFunction];
            }
            set
            {
                ViewState[V_SODFunction] = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to set the mode of display
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        /// <summary>
        /// Method to get FunctionItems
        /// </summary>
        /// <returns></returns>
        public List<SODFunctionItem> GetSodFunctionItems()
        {
            foreach (SODFunctionItem p in PolicyTemplates)
            {
                p.endpoint = null;
                p.EndpointType = null;
            }
            return PolicyTemplates;
        }
        #endregion

        #region Intialize Section
        /// <summary>
        /// Method to initialise page
        /// </summary>
        public void InitilizePage()
        {
            if (PolicyTemplates == null)
                PolicyTemplates = new List<SODFunctionItem>();

            EndpointUC.Initialize();
            if (SodFunction != null && SodFunction.SodFunctionItems.Count > 0)
            {
                foreach (SODFunctionItem item in SodFunction.SodFunctionItems)
                {
                    AddTemplateToPolicyTemplates(item);
                }
                ViewState["PolicyTemplates"] = PolicyTemplates;
                BindPolicyTemplates();
            }           
        }
        #endregion

        #region Helper
        /// <summary>
        /// Method to add a Policy Template
        /// </summary>
        /// <param name="pTemplate"></param>
        private void AddTemplateToPolicyTemplates(SODFunctionItem pTemplate)
        {
            string ObjectCode = pTemplate.ObjectCode == null ? null : pTemplate.ObjectCode;
            string FieldCode = pTemplate.FieldCode == null ? null : pTemplate.FieldCode;
            string FromValue = pTemplate.FromValue == null ? null : pTemplate.FromValue;

            if (PolicyTemplates.Count > 0)
            {
                var ExistingEntries = PolicyTemplates.Where(m => m.SodFunctionType == pTemplate.SodFunctionType
                    && m.Endpointsystemid == pTemplate.Endpointsystemid && m.EndpointSystemTypeId == pTemplate.EndpointSystemTypeId
                    && m.SodRoleDesc == pTemplate.SodRoleDesc && m.ObjectCode == ObjectCode
                    && m.FieldCode == FieldCode && m.FromValue == FromValue).ToList();
                if (ExistingEntries.Count == 0)
                {
                    PolicyTemplates.Add(pTemplate);
                }
            }
            else
            {
                PolicyTemplates.Add(pTemplate);
            }
        }

        /// <summary>
        /// Method to bind data to grdPolicyTemplate
        /// </summary>
        private void BindPolicyTemplates()
        {
            if (PolicyTemplates != null)
            {
                grdPolicyTemplate.DataSource = PolicyTemplates;
                grdPolicyTemplate.DataBind();
            }
        }
        #endregion

        #region Delegates
        /// <summary>
        /// Method to handle ApplicationChanged in EndpointUC
        /// </summary>
        /// <param name="EndPointSystemType"></param>
        protected void EndpointUC_ApplicationChanged(EndPointSystemType EndPointSystemType)
        {
            AutoCompleteBoxRow.Visible = false;
            objectRow.Visible = false;
            SapObjectFieldrow.Visible = false;
        }

        /// <summary>
        /// Method to handle FunctionTypeChanged for EndpointSystem
        /// </summary>
        /// <param name="EndPointSystemType"></param>
        protected void EndpointSystem_FunctionTypeChanged(object EndpointObjectName)
        {
            AutoCompleteBoxRow.Visible = true;
            EndPointSystemType systemType = EndpointUC.SelectedApplication;
            if (EndpointObjectName != null && systemType != null)
            {
                switch (systemType.EndpointSystemTypeName)
                {
                    case EndpointType.ActiveDirectory:
                        lblFunctionTypeText.Text = "Group";
                        Session["EntitlementType"] = "Group";                        
                        GenericAutoCompleteBox.ServiceMethodName = "GetADUserGroups";
                        GenericAutoCompleteBox.ServicePath = "../../../Services/AutoCompleteDataService.svc";
                        break;
                    
                    //case EndpointType.LotusNotes:
                    //    lblFunctionTypeText.Text = "Group";
                    //    Session["EntitlementType"] = "Group";
                    //    GenericAutoCompleteBox.ServiceMethodName = "GetLNSGroupInfo";
                    //    GenericAutoCompleteBox.ServicePath = "../../../Services/AutoCompleteDataService.svc";
                    //    break;
                   
                    case EndpointType.SAP:
                        EndPoint endPoint = EndpointUC.SelectedEndpoint;
                        if (endPoint != null)
                        {
                            switch (EndpointObjectName.ToString())
                            {
                                case Constants.EndpointObject_SAP_ComplianceTCode:
                                    lblFunctionTypeText.Text = "TCodes";
                                     Session["EntitlementType"] = "T-Code";
                                    GenericAutoCompleteBox.Clear();
                                    GenericAutoCompleteBox.ServiceMethodName = "GetAllSAPEntitlements";
                                    GenericAutoCompleteBox.ServicePath = "../../../Services/AutoCompleteDataService.svc";
                                    objectRow.Visible = true;
                                    chkObject.Visible = true;
                                    chkObject.Checked = false;
                                    break;
                                case Constants.EndpointObject_SAP_ComplianceProfile:
                                    lblFunctionTypeText.Text = "Profiles";
                                    Session["EntitlementType"] = "Profiles";
                                    GenericAutoCompleteBox.Clear();
                                    GenericAutoCompleteBox.ServiceMethodName = "GetAllSAPEntitlements";
                                    GenericAutoCompleteBox.ServicePath = "../../../Services/AutoCompleteDataService.svc";
                                    objectRow.Visible = false;
                                    SapObjectFieldrow.Visible = false;
                                    break;
                                case Constants.EndpointObject_SAP_ComplianceRole:
                                    lblFunctionTypeText.Text = "Role";
                                    Session["EntitlementType"] = "Role";
                                    GenericAutoCompleteBox.Clear();
                                    GenericAutoCompleteBox.ServiceMethodName = "GetAllSAPEntitlements";
                                    GenericAutoCompleteBox.ServicePath = "../../../Services/AutoCompleteDataService.svc";
                                    objectRow.Visible = false;
                                    SapObjectFieldrow.Visible = false;
                                    break;
                            }
                        }
                        break;
                }           
            }
        }

        /// <summary>
        /// Method to handle EndpointChanged for EndpointUC
        /// </summary>
        /// <param name="EndPoint"></param>
        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            if (EndPoint != null)
            {
                Session["ComplianceEndpointId"] = EndPoint.Id;
            }
           
            AutoCompleteBoxRow.Visible = false;
            objectRow.Visible = false;
            SapObjectFieldrow.Visible = false;
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Method to load page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PolicyTemplates == null)
                PolicyTemplates = new List<SODFunctionItem>();
            else
                ViewState["PolicyTemplates"] = PolicyTemplates;
            if (!Page.IsPostBack)
            {
                AutoCompleteBoxRow.Visible = false;
                objectRow.Visible = false;
                SapObjectFieldrow.Visible = false;
                BindPolicyTemplates();
            }
            else
            {
                if (ViewState["PolicyTemplates"] != null)
                    PolicyTemplates = (List<SODFunctionItem>)ViewState["PolicyTemplates"];
            }
        }

        /// <summary>
        /// Method to handle click event for btnAdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (EndpointUC.SelectedApplication != null && GenericAutoCompleteBox.GetValue() != null)
            {
                SODFunctionItem pTemplate = new SODFunctionItem(); 
                if(SodFunction != null)
                    pTemplate.SodFunctionId = SodFunction.Id;
                EndPointSystemType systemType = EndpointUC.SelectedApplication;
                pTemplate.EndpointSystemTypeId = systemType.Id;
                pTemplate.Endpointsystemid = EndpointUC.SelectedEndpoint.Id;
                pTemplate.EndpointType = systemType;
                pTemplate.endpoint = EndpointUC.SelectedEndpoint;
                string Entitlement = GenericAutoCompleteBox.GetValue().Text;
                Entitlement = Entitlement.Replace(';', ' ');
                pTemplate.SodRoleId = Entitlement.Trim();
                pTemplate.SodRoleDesc = Entitlement.Trim();
                string EntitlementType = Session["EntitlementType"].ToString();
                if (systemType.EndpointSystemTypeName == EndpointType.SAP)
                {
                    if (EntitlementType == Constants.EndpointObject_SAP_TCode)
                    {
                        if (chkObject.Checked)
                        {
                            SAPObjectInfo sapObject = SAPObjectFieldSearch.SelectedObject;
                            if(sapObject == null)
                            {
                                MasterPage.ShowAlert(AlertType.Error, "Please select object.");
                                return;
                            }
                            pTemplate.ObjectCode = sapObject != null ? sapObject.SapObjectCode : "";
                            if(SAPObjectFieldSearch.SelectedField == null)
                            {
                                MasterPage.ShowAlert(AlertType.Error, "Please select field.");
                                return;
                            }
                            pTemplate.FieldCode = SAPObjectFieldSearch.SelectedField;
                            pTemplate.FromValue = txtFrom.Text;
                        }
                        pTemplate.SodFunctionType = "TCodes";
                    }
                    else if (EntitlementType == Constants.EndpointObject_SAP_ComplianceProfile)
                    {
                        pTemplate.SodFunctionType = "Profiles";
                    }
                    else if (EntitlementType == Constants.EndpointObject_SAP_ComplianceRole)
                    {
                        pTemplate.SodFunctionType = "Role";
                    }
                }
                else
                {
                    pTemplate.SodFunctionType = EntitlementType;
                }
                AddTemplateToPolicyTemplates(pTemplate);
                ViewState["PolicyTemplates"] = PolicyTemplates;
                GenericAutoCompleteBox.Clear();
                BindPolicyTemplates();
                EndpointUC.ClearApplication();
                EndpointUC.ClearApplicationType();
                SAPObjectFieldSearch.ClearFields();
                SAPObjectFieldSearch.ClearObejcts();
                txtFrom.Text = "";
                chkObject.Visible = false;
                AutoCompleteBoxRow.Visible = false;
                objectRow.Visible = false;
                SapObjectFieldrow.Visible = false;
            }
            else
            {
                MasterPage.ShowAlert(AlertType.Error, "Please select all the values.");
            }
        }

        /// <summary>
        /// Method to handle cheked event for chkObject
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkObject_CheckedChanged(object sender, EventArgs e)
        {
            if (chkObject.Checked)
                SapObjectFieldrow.Visible = true;
            else
                SapObjectFieldrow.Visible = false;
        }

        /// <summary>
        /// Method to handle Command for grdPolicyTemplate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdPolicyTemplate_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                GridDataItem item = e.Item as GridDataItem;
                string SodFunctionType = item["SodFunctionType"].Text == "&nbsp;" ? null : item["SodFunctionType"].Text;
                string SodRoleDesc = item["SodRoleDesc"].Text == "&nbsp;" ? null : item["SodRoleDesc"].Text;
                string EndPointSystemType = item["EndpointSystemTypeName"].Text == "&nbsp;" ? null : item["EndpointSystemTypeName"].Text;
                string EndPoint = item["DisplayName"].Text == "&nbsp;" ? null : item["DisplayName"].Text;
                string ObjectCode = item["ObjectCode"].Text == "&nbsp;" ? null : item["ObjectCode"].Text;
                string FieldCode = item["FieldCode"].Text == "&nbsp;" ? null : item["FieldCode"].Text;
                string FromValue = item["FromValue"].Text == "&nbsp;" ? null : item["FromValue"].Text;

                var SodFunctionItem = PolicyTemplates.Where(m => m.SodFunctionType == SodFunctionType
               && m.endpoint.DisplayName == EndPoint && m.EndpointType.DisplayName == EndPointSystemType
               && m.SodRoleDesc == SodRoleDesc && m.ObjectCode == ObjectCode && m.FieldCode == FieldCode && m.FromValue == FromValue).
               FirstOrDefault();

                if (SodFunctionItem != null)
                {
                    PolicyTemplates.Remove(SodFunctionItem);
                    BindPolicyTemplates();
                }
            }
        }
        #endregion

        protected void grdPolicyTemplate_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // setting header text based on application level values
                foreach (GridColumn col in grdPolicyTemplate.Columns)
                {
                    if (col.UniqueName == "DisplayName")
                        col.HeaderText = T("App.Common.Endpoint");
                    if (col.UniqueName == "EndpointSystemTypeName")
                        col.HeaderText = T("App.Common.EndpointType");
                }

                grdPolicyTemplate.Rebind();
            }
        }
    }
}