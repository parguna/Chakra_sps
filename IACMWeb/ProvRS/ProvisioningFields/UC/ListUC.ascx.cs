using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using DI.Chakra.Dto;
using Application;
using DI.Chakra.Logger;

namespace DI.Chakra.Web.ProvRS.ProvisioningFields.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        #region Members

        public IList<Dto.Presentation.PresentationEndpointProvisioningField> EndPointlist;
        private const string V_ENDPOINTTYPEID = "V_ENDPOINTTYPEID";
        private const string V_ENDPOINTSYSTEMID = "V_ENDPOINTSYSTEMID";
        private const string V_ENTITYID = "V_ENTITYID";
        private const string V_NODELEVEL = "V_NODELEVEL";
        private const string V_SELECTEDNODE = "V_SELECTEDNODE";

        #endregion

        #region Properties

        public long SelectedEndPointTypeId
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPEID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTTYPEID];
            }
            set
            {
                ViewState[V_ENDPOINTTYPEID] = value;
            }
        }
        public long SelectedEndPointSystemId
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEMID] == null)
                    return 0;
                return (long)ViewState[V_ENDPOINTSYSTEMID];
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEMID] = value;
            }
        }
        public long EntityId
        {
            get
            {
                if (ViewState[V_ENTITYID] == null)
                    return 0;
                return (long)ViewState[V_ENTITYID];
            }
            set
            {
                ViewState[V_ENTITYID] = value;
            }
        }
        public int NodelLevel
        {
            get
            {
                if (ViewState[V_NODELEVEL] == null)
                    return -1;
                return (int)ViewState[V_NODELEVEL];
            }
            set
            {
                ViewState[V_NODELEVEL] = value;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            RadTreeViewProvField.Visible = true;
            if (!IsPostBack)
            {
                BindEndPointProvisiongFieldList();
                gvProvisioningField.DataBind();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            AddUpdateModalPopup.VisibleOnPageLoad = false;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (Save())
                {
                    MasterPage.ShowAlert(AlertType.Success, "Provisioning Field saved successfully");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Provisioning Fields - Delete");
                delete();
            }
            else if (e.ActionType == ACLAction.Add)
            {
                if (RadTreeViewProvField.SelectedNode == null)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an EndpointSystem");
                    return;
                }
                if (SelectedEndPointSystemId > 0)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("endPointSystemId", SelectedEndPointSystemId.ToString());
                    bool Exist = DomainApiClient<Dto.NProvisioningField>.GetByAction<bool>(DomainApiAction.Ruleset.NProvisioningField.HasAnyByEndPointsystemId, parameter);
                    if (!Exist)
                    {
                        MasterPage.ShowAlert(AlertType.Warning, "Please generate base provisioning field");
                        return;
                    }
                }
                DetailUC.UIMode = UIMode.Add;
                DetailUC.Clearvalue();
                H1.InnerText = "Add Provisioning Field";
                btnSubmit.Text = "Add";
                AddUpdateModalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Edit)
            {
                if (!ContextObjectId.HasValue)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                    return;
                }
                SetPage();
                H1.InnerText = "Edit Provisioning Level";
                btnSubmit.Text = "Update";
                AddUpdateModalPopup.VisibleOnPageLoad = true;
            }
            else if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                gvProvisioningField.ExportSettings.FileName = "Provisioning Fields";
                gvProvisioningField.Export(gvProvisioningField, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                List<NProvisioningField> listProvList = BindProvisioningFieldList(SelectedEndPointTypeId, (long)SelectedEndPointSystemId, "");
                gvProvisioningField.Export<IList<Dto.NProvisioningField>>(listProvList, e.ActionType, "Provisioning Fields");
            }
        }

        protected void lnkGenerates_Click(object sender, EventArgs e)
        {
            List<Dto.NProvisioningField> generatedProvFieldList = BindProvisioningFieldList(SelectedEndPointTypeId, 0, "DataBind");
            if (generatedProvFieldList.Count > 0)
            {
                long _selectedEndPointSystemId = (long)SelectedEndPointSystemId;
                generatedProvFieldList.ForEach(provFld => 
                {
                    provFld.Id = 0;
                    provFld.CreatedDate = DateTime.Now;
                    provFld.EndPointSystemId = _selectedEndPointSystemId;
                    provFld.EndPoint = null;
                });
                DomainApiClient<NProvisioningField>.SaveList(generatedProvFieldList);
            }
            divGenerate.Visible = false;
            gvProvisioningField.Visible = true;
            BindProvisioningFieldList(SelectedEndPointTypeId, (long)SelectedEndPointSystemId, "DataBind");

            SetDeleteVisibility(RadTreeViewProvField.SelectedNode, true);
        }

        #endregion

        #region Gridview Events

        protected void gvProvisioningField_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (NodelLevel >= 0)
            {
                BindProvisioningFieldList(SelectedEndPointTypeId, (long)SelectedEndPointSystemId, "");
            }
        }

        protected void gvProvisioningField_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvProvisioningField.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        #endregion

        #region Methods

        private void BindEndPointProvisiongFieldList()
        {
            EndPointlist = DomainApiClient<Dto.Presentation.PresentationEndpointProvisioningField>.GetByAction<List<Dto.Presentation.PresentationEndpointProvisioningField>>(DomainApiAction.Endpoint.Presentation.PresentationEndpointProvisioningField.GetEndpointProvisiongField, null);
            RadTreeViewProvField.DataSource = EndPointlist;
            RadTreeViewProvField.DataBind();
            if (RadTreeViewProvField.Nodes.Count > 0)
            {
                RadTreeViewProvField.Nodes[0].Selected = true;
                NodelLevel = RadTreeViewProvField.Nodes[0].Level;
                SelectedEndPointTypeId = Convert.ToInt64(RadTreeViewProvField.Nodes[0].Value);
            }
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
            {
                MasterPage.ShowAlert(AlertType.Warning, "Please select an item");
                return;
            }
            NProvisioningField objProvField = DomainApiClient<NProvisioningField>.GetById((long)(this.ContextObjectId));
            
            DomainApiClient<NProvisioningField>.DeleteById((long)this.ContextObjectId);
            gvProvisioningField.Rebind();
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "Provisioning Field Deleted Successfully");
        }

        private bool Save()
        {
            NProvisioningField provField = DetailUC.provisioningField;
            if (NodelLevel > 0) provField.EndPointSystemId = SelectedEndPointSystemId;
            provField.EndPointSystemTypeId = SelectedEndPointTypeId;

            if (Validate(provField))
            {
                var endPoint = DomainApiClient<Dto.EndPoint>.GetById(SelectedEndPointSystemId);
                if(provField.Id > 0)
                {
                    var exisiting = DomainApiClient<NProvisioningField>.GetById(provField.Id);
                    var changes = UtilityMethods.PublicInstancePropertiesEqual<Dto.NProvisioningField>(exisiting, provField, new string[] {"CreatedBy","CreatedDate","LastModifiedBy","LastModifiedDate","Id" });
                    if(!string.IsNullOrWhiteSpace(changes))
                        DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"NProvisioningField Modified for {endPoint?.DisplayName}: {changes}");
                }
                else
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"NProvisioningField added for Endpoint.{endPoint?.DisplayName}");
                }
                DomainApiClient<NProvisioningField>.Save(provField);
                AddUpdateModalPopup.VisibleOnPageLoad = false;
                this.ContextObjectId = null;
                List<Dto.NProvisioningField> list = BindProvisioningFieldList(SelectedEndPointTypeId, (long)SelectedEndPointSystemId, "Rebind");
                if ((SelectedEndPointSystemId != 0) && (SelectedEndPointSystemId != null) && (list.Count > 0))
                {
                    divGenerate.Visible = false;
                    gvProvisioningField.Visible = true;
                }
                return true;
            }
            else
                return false;
        }

        private bool Validate(NProvisioningField provField)
        {
            bool toSave = true;
            string ErrorMessage = "";
            if ((string.IsNullOrEmpty(provField.FieldName) && (string.IsNullOrEmpty(provField.FieldName))))
            {
                ErrorMessage = "Field Name,Value should not be empty";
                toSave = false;
            }
            else
            {
                if ((string.IsNullOrEmpty(provField.FieldName)))
                {
                    ErrorMessage = "Field Name should not be Empty";
                    toSave = false;
                }
                //else if ((string.IsNullOrEmpty(provField.Value)))
                //{
                //    ErrorMessage = "Value should not be Empty";
                //    toSave = false;
                //}
                else if ((string.IsNullOrEmpty(provField.AccountType.ToString())))
                {
                    ErrorMessage = "Account Type should not be Empty";
                    toSave = false;
                }
            }

            if ((btnSubmit.Text == "Add") && (DoesExist(provField)))
            {
                ErrorMessage = "Field Name already exists for the selected EndpointSystem";
                toSave = false;
            }
            else if ((btnSubmit.Text == "Update"))
            {
                var dbProvField = DomainApiClient<Dto.NProvisioningField>.GetById(EntityId);
                provField.Id = dbProvField.Id;
                if (dbProvField.FieldName != provField.FieldName)
                {
                    if (DoesExist(provField))
                    {
                        MasterPage.ShowAlert(AlertType.Error, "Field Name already exists for the selected EndpointSystem");
                        toSave = false;
                    }
                }
            }
            
            if ((!toSave) && (!string.IsNullOrEmpty(ErrorMessage)))
            {
                MasterPage.ShowAlert(AlertType.Warning, ErrorMessage);
                return false;
            }
            return toSave;
        }

        private bool DoesExist(NProvisioningField provField)
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("fieldName", provField.FieldName.ToString());
            parameter.Add("AccountType", provField.AccountType.ToString());
            parameter.Add("endPointSystemTypeId", SelectedEndPointTypeId.ToString());
            parameter.Add("endPointSystemId", SelectedEndPointSystemId == 0 ? null : SelectedEndPointSystemId.ToString());
            return DomainApiClient<Dto.NProvisioningField>.GetByAction<bool>(DomainApiAction.Ruleset.NProvisioningField.DoesExists, parameter);
        }

        private void SetPage()
        {
            var ProvisioningField = DomainApiClient<Dto.NProvisioningField>.GetById((long)this.ContextObjectId);
            if (null == ProvisioningField)
                return;
            EntityId = ProvisioningField.Id;
            DetailUC.provisioningField = ProvisioningField;
        }

        private List<Dto.NProvisioningField> BindProvisioningFieldList(long endPointSystemTypeId, long endPointSystemId, string bindMode)
        {
            List<Dto.NProvisioningField> listProvField = null;
            if (endPointSystemTypeId > 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endPointSystemTypeId", endPointSystemTypeId.ToString());
                parameter.Add("endPointSystemId", endPointSystemId == 0 ? null : endPointSystemId.ToString());
                listProvField = DomainApiClient<Dto.NProvisioningField>.GetByAction<List<Dto.NProvisioningField>>(DomainApiAction.Ruleset.NProvisioningField.GetByEndPointSystemIncludingValueDisplayName, parameter);
                if ((NodelLevel > 0) && (listProvField.Count > 0))
                {
                    listProvField.ForEach(provFld => provFld.EndPointSystemId = SelectedEndPointSystemId);
                }
                gvProvisioningField.DataSource = listProvField;
                if (bindMode == "DataBind")
                    gvProvisioningField.DataBind();
                if (bindMode == "Rebind")
                    gvProvisioningField.Rebind();
            }
            return listProvField;
        }

        private void DeleteByEndPointSystemId(long endPointSystemId)
        {
            var endPoint = DomainApiClient<EndPoint>.GetById(endPointSystemId);
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endPointSystemTypeId", endPoint.EndpointSystemTypeId.ToString());
            parameter.Add("endPointSystemId", endPointSystemId.ToString());
            var deleteList = DomainApiClient<NProvisioningField>.GetByAction<List<NProvisioningField>>(DomainApiAction.Ruleset.NProvisioningField.GetByEndPointSystemIncludingValueDisplayName, parameter);
            DomainApiClient<NProvisioningField>.GetByAction<object>(DomainApiAction.Ruleset.NProvisioningField.DeleteByEndPointSystemId, parameter);
            if (deleteList != null && deleteList.Count > 0)
            {
                foreach (var item in deleteList)
                {
                    DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, $"{endPoint?.DisplayName} ProvisioningField Deleted: FieldName:{item.FieldName} Value:{item.Value} isActive:{item.IsActive} ");
                }
            }
            MasterPage.ShowAlert(AlertType.Success, "EndPointSystem Provisioning Fields deleted successfully");
            divGenerate.Visible = true;
            gvProvisioningField.Visible = false;
            this.ContextObjectId = null;
        }

        private void SetDeleteVisibility(Telerik.Web.UI.RadTreeNode node, bool isVisible)
        {
            ImageButton img = (ImageButton)node.FindControl("imgNode");
            img.Visible = isVisible;
        }

        #endregion

        #region TreeView Events

        protected void RadTreeViewProvField_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {
            RadTreeNode selectedParentNode = e.Node.ParentNode;
            RadTreeNode selectedNode = RadTreeViewProvField.SelectedNode;
            NodelLevel = selectedNode.Level;

            if (selectedParentNode != null)
            {
                SelectedEndPointTypeId = Convert.ToInt64(selectedParentNode.Value);
                SelectedEndPointSystemId = Convert.ToInt64(selectedNode.Value);
            }
            else
            {
                SelectedEndPointTypeId = Convert.ToInt64(selectedNode.Value);
                SelectedEndPointSystemId = 0;
            }

            List<Dto.NProvisioningField> list = BindProvisioningFieldList(SelectedEndPointTypeId, (long)SelectedEndPointSystemId, "DataBind");
            if ((SelectedEndPointSystemId != 0) && (list.Count == 0))
            {
                divGenerate.Visible = true;
                lblEndpointSystem.Text = selectedNode.Text;
                gvProvisioningField.Visible = false;
            }
            else
            {
                divGenerate.Visible = false;
                gvProvisioningField.Visible = true;
            }
        }

        protected void RadTreeViewProvField_NodeCreated(object sender, RadTreeNodeEventArgs e)
        {
            RadTreeNode node = e.Node;
            ImageButton img = (ImageButton)e.Node.FindControl("imgNode");
            img.Click += new ImageClickEventHandler(imgChildNode_Click);
        }

        protected void RadTreeViewProvField_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            RadTreeNode node = e.Node;
            ImageButton img = (ImageButton)e.Node.FindControl("imgNode");
            HiddenField hdn = (HiddenField)e.Node.FindControl("hdnEndpointHasValues");
            if (node.Level > 0)
                if (hdn.Value == Boolean.TrueString)
                    img.Visible = true;
                else
                    img.Visible = false;
            else
                img.Visible = false;
        }

        protected void imgChildNode_Click(object sender, ImageClickEventArgs e)
        {
            DeleteByEndPointSystemId(Convert.ToInt64(((Telerik.Web.UI.RadTreeNode)((ImageButton)sender).Parent).Value));
            SetDeleteVisibility((Telerik.Web.UI.RadTreeNode)((ImageButton)sender).Parent, false);
        }
        #endregion
    }
}