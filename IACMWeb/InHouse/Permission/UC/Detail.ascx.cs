using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Linq;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web.UI;
using DI.Chakra.Web.Services;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;
using DI.Chakra.Logger;
using Application;

namespace DI.Chakra.Web.InHouse.Permission.UC
{
    public partial class Detail : UIBaseUC
    {
        #region Members

        private const string V_EntityID = "V_EntityID";
        private const string V_DISPLAYMODE = "V_DISPLAYMODE";
        private const string V_INHPERMISSION = "INHPERMISSION";

        #endregion

        #region Properties
        public long EntityId
        {
            get
            {
                if (ViewState[V_EntityID] == null)
                    return -1;
                return (long)ViewState[V_EntityID];
            }
            set
            {
                ViewState[V_EntityID] = value;
                SetPage();
            }
        }
        public InhousePermission INHPermission
        {
            get
            {
                if (ViewState[V_INHPERMISSION] == null)
                    return new InhousePermission();
                return (InhousePermission)ViewState[V_INHPERMISSION];
            }
            set
            {
                ViewState[V_INHPERMISSION] = value;
            }
        }
        public UIMode UIMode
        {
            get
            {
                if (ViewState[V_DISPLAYMODE] == null)
                    return UIMode.View;

                return (UIMode)ViewState[V_DISPLAYMODE];
            }
            set
            {
                ViewState[V_DISPLAYMODE] = value;
            }
        }

        #endregion

        #region Methods
        private void SetPage()
        {
            EndpointUC.Initialize();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("id", EntityId.ToString());
            INHPermission = DomainApiClient<Dto.InhousePermission>.GetByAction<InhousePermission>
                (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.GetByIdIncludeEndpoint, parameter);
            if (INHPermission.Id == 0)
            {
                INHPermission = new InhousePermission();
                return;
            }
            EndpointUC.SelectedEndpoint = INHPermission.EndPoint;

            if (!string.IsNullOrEmpty(INHPermission.InhouseSystemCode))
                txtInHouseSystemCode.Entries.Add(new AutoCompleteBoxEntry
                    (INHPermission.InhouseSystemCode, INHPermission.InhouseSystemCode));
            
            txtName.Text = INHPermission.Name;
            txtDescription.Text = INHPermission.Description;
            IsActive.Checked = INHPermission.IsActive;
        }
     
        private void Save()
        {
            try
            {
                if (EndpointUC.SelectedEndpoint == null)
                {
                    this.MasterPage.ShowAlert(AlertType.Error, string.Format("Please Select {0}.", T("App.Common.Endpoint")));
                    return;
                }
                else if (txtInHouseSystemCode.Text == string.Empty)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please Enter Inhouse System Code");
                    return;
                }
                else if (txtName.Text == string.Empty)
                {
                    MasterPage.ShowAlert(AlertType.Warning, "Please Enter Name");
                    return;
                }
                if ((UIMode == UIMode.Add) && (DoesExist(txtName.Text.Trim())))
                {
                    MasterPage.ShowAlert(AlertType.Error, "Permission Name Already Exists.");
                    return;
                }
                else if (UIMode == UIMode.Edit)
                {
                    var dbGroup = DomainApiClient<Dto.InhousePermission>.GetById(EntityId);

                    if (dbGroup.Name != txtName.Text.Trim())
                    {
                        if (DoesExist(txtName.Text.Trim()))
                        {
                            MasterPage.ShowAlert(AlertType.Error, "Group Name Already Exists.");
                            return;
                        }
                    }
                }
                INHPermission.EndPointSystemId = EndpointUC.SelectedEndpoint.Id;
                INHPermission.InhouseSystemCode = txtInHouseSystemCode.Entries.Count > 0 ? txtInHouseSystemCode.Entries[0].Text : "";
                INHPermission.Name = txtName.Text;
                INHPermission.Description = txtDescription.Text;
                INHPermission.IsActive = IsActive.Checked;
                //INHPermission.CanRequest = IsActive.Checked;
                if (INHPermission.Id == 0)
                {
                    //INHPermission.CanRequest = true;
                }

                DomainApiClient<Dto.InhousePermission>.Save(INHPermission);

                if ((UIMode == UIMode.Add))
                {
                    MasterPage.ShowAlert(AlertType.Success, "Record Saved Successfully");
                }
                if (((UIMode == UIMode.Edit)))
                {
                    MasterPage.ShowAlert(AlertType.Success, "Record Updated Successfully");
                }
                Response.Redirect("~/Inhouse/Permission/List.aspx");
            }
            catch (Exception)
            {
                MasterPage.ShowAlert(AlertType.Error, "Error while saving");
            }
        }

        private bool DoesExist(string name)
        {
            Dto.Group group = new Dto.Group();
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("value", name);
            parameter.Add("endPointSystemId", Session["InhouseSystemEndpointId"].ToString());
            return DomainApiClient<Dto.InhousePermission>.GetByAction<bool>
                (DomainApiAction.Endpoint.InhouseSystems.InhousePermission.DoesExistsByEndPointSystemId, parameter);
        }

        private void disableControlsForView()
        {
            EndpointUC.DisabledApplicationType = true;
            EndpointUC.DisabledApplication = true;
            txtInHouseSystemCode.Enabled = false;
            txtName.Enabled = false;
            txtDescription.Enabled = false;
            IsActive.Enabled = false;
        }

        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
            if (!IsPostBack)
            {
                EntityId = ContextObjectId != null ? Convert.ToInt64(ContextObjectId) : 0;
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication =new Dto.EndPointSystemType { EndpointSystemTypeName = Infrastructure.Enums.EndpointType.InhouseSystems };

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                string id = EndpointUC.SelectedApplication != null ? EndpointUC.SelectedApplication.Id.ToString() : "0";
                parameter.Add("id", id);
                var Endpoint = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByIdIncludingEndpoints, parameter);

                if (Endpoint == null || Endpoint.EndPoints.Count == 0)
                {
                    ShowAlertMessage(AlertType.Error, "First Create InHouse Endpoint");
                }
                else
                {
                    EndpointUC.DisabledApplicationType = true;
                    if (UIMode == UIMode.View)
                    {
                        disableControlsForView();
                    }
                    else if (UIMode == UIMode.Edit)
                    {
                       txtInHouseSystemCode.Enabled = false;
                    }
                }
            }
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Save)
            {
                Save();
                DILog.UILogger.WriteLog(new SessionStatusImpl().User.Id, LogLevelType.Info, "Inouse Permission - Save");
            }
        }

        protected void EndpointUC_EndpointChanged(EndPoint EndPoint)
        {
            if (EndPoint != null)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemId", EndPoint.Id.ToString());
                EndpointConfiguration epConfig = DomainApiClient<EndpointConfiguration>.GetByAction<EndpointConfiguration>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointConfiguration.GetByEndpointSystemId, parameter);
                var config = epConfig.ConfigurationDetails.Where(lq => lq.KeyId == Convert.ToInt64(EndpointKey.CanRequestMultipleRoles)).Select(select => select.Value).FirstOrDefault();
                bool IsMultipeRole = config != null ? Convert.ToBoolean(config) : false;

                txtInHouseSystemCode.TextSettings.SelectionMode = IsMultipeRole ? RadAutoCompleteSelectionMode.Multiple
                    : RadAutoCompleteSelectionMode.Single;
                Session["InhouseSystemEndpointId"] = EndPoint.Id;
                txtInHouseSystemCode.Entries.Clear();

                AutoCompleteBoxData resultAutobox = new AutoCompleteBoxData(); 
                Dictionary<string, object> parameterautocomplete = new Dictionary<string, object>();
                parameterautocomplete.Add("Text", "*");
                parameterautocomplete.Add("AutoCompleteBoxData", resultAutobox);
                AutoCompleteDataService au = new AutoCompleteDataService();
                resultAutobox = au.GetInhouseSystemCodeByEndPointID(parameterautocomplete);

                txtInHouseSystemCode.Entries.Add(new AutoCompleteBoxEntry
                  (resultAutobox.Items[0].Text, resultAutobox.Items[0].Value));
            }
            else
            {
                Session["InhouseSystemEndpointId"] = null;
                txtInHouseSystemCode.Entries.Clear();
            }          
        }

        protected void EndpointUC_ApplicationChanged(EndPointSystemType EndPointSystemType)
        {
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~//Inhouse//Permission//List.aspx");
        }

        #endregion
    }
}