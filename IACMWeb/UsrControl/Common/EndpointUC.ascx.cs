using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class EndpointUC : UIBaseUC
    {
        #region Members
        private string V_CHECKACTIVE = "V_CHECKACTIVE";
        private string V_ENDPOINTTYPE = "V_ENDPOINTTYPE";
        private string V_DISPLAYCONTROL = "V_DISPLAYCONTROL";
        private string V_ELEMENTLENGTHCSS = "V_ELEMENTLENGTHCSS";
        private string V_ENDPOINTSONLY = "V_ENDPOINTSONLY";
        private string V_NOTTRUSTEDSOURCE = "V_NOTTRUSTEDSOURCE";
        private string V_SHOWTRUSTEDSOURCE = "V_SHOWTRUSTEDSOURCE";
        private string V_SHOWALLCHILDEP = "V_SHOWALLCHILDEP";
        private string V_ALLENDPOINTS = "V_ALLENDPOINTS";
        private string V_CANEDIT = "V_CANEDIT";
        private EndPoint _mEndpoint = null;
        private EndPointSystemType _mEndPointSystemType = null;
        public delegate void ApplicationIndexChanged(EndPointSystemType EndPointSystemType);
        public event ApplicationIndexChanged ApplicationChanged;
        public delegate void EndpointIndexChanged(EndPoint EndPoint);
        public event EndpointIndexChanged EndpointChanged;
        #endregion

        #region Properties
        public string ElementLengthCSS
        {
            get
            {
                if (ViewState[V_ELEMENTLENGTHCSS] == null)
                    return "col-xs-7";
                return (string)ViewState[V_ELEMENTLENGTHCSS];
            }
            set
            {
                ViewState[V_ELEMENTLENGTHCSS] = value;
            }
        }
        public DisplayControl DisplayControl
        {
            get
            {
                if (ViewState[V_DISPLAYCONTROL] == null)
                    return DisplayControl.Both;
                return (DisplayControl)ViewState[V_DISPLAYCONTROL];
            }
            set
            {
                ViewState[V_DISPLAYCONTROL] = value;
            }
        }

        public bool canEditDropdown
        {
            get
            {
                if (ViewState[V_CANEDIT] == null)
                    return true;
                return (bool)ViewState[V_CANEDIT];
            }
            set
            {
                ViewState[V_CANEDIT] = value;
            }
        }
        public bool IsApplicationTypeRequired
        {
            set
            {
                ApplicationTypeField.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool IsApplicationRequired
        {
            set
            {
                ApplicationField.Attributes.Add("required", value ? "required" : "");
            }
        }
        public bool CheckActive
        {
            get
            {
                if (ViewState[V_CHECKACTIVE] == null)
                    return true;
                return (bool)ViewState[V_CHECKACTIVE];
            }
            set
            {
                ViewState[V_CHECKACTIVE] = value;
            }
        }
        public string SetClass
        {
            get
            {
                if (ViewState["SETCLASS"] == null)
                    return "col-xs-7";
                return (string)ViewState["SETCLASS"];
            }
            set
            {
                DivApplicationDropDown.Attributes.Add("class", value);
            }
        }
        public bool EndpointsOnly
        {
            get
            {
                if (ViewState[V_ENDPOINTSONLY] == null)
                    return false;
                return (bool)ViewState[V_ENDPOINTSONLY];
            }
            set
            {
                ViewState[V_ENDPOINTSONLY] = value;
            }
        }

        public bool IncludeInhouse
        {
            get
            {
                if (ViewState[V_ALLENDPOINTS] == null)
                    return false;
                return (bool)ViewState[V_ALLENDPOINTS];
            }
            set
            {
                ViewState[V_ALLENDPOINTS] = value;
            }
        }

        public bool NotTrustedSource
        {
            get
            {
                if (ViewState[V_NOTTRUSTEDSOURCE] == null)
                    return false;
                return (bool)ViewState[V_NOTTRUSTEDSOURCE];
            }
            set
            {
                ViewState[V_NOTTRUSTEDSOURCE] = value;
            }
        }

        public bool ShowTrustedSource
        {
            get
            {
                if (ViewState[V_SHOWTRUSTEDSOURCE] == null)
                    return false;
                return (bool)ViewState[V_SHOWTRUSTEDSOURCE];
            }
            set
            {
                ViewState[V_SHOWTRUSTEDSOURCE] = value;
            }
        }

        public string MethodName
        {
            get; set;
        }

        public EndpointType EndpointType
        {
            get
            {
                if (ViewState[V_ENDPOINTTYPE] == null)
                    return EndpointType.ActiveDirectory;
                return (EndpointType)ViewState[V_ENDPOINTTYPE];
            }
            set
            {
                ViewState[V_ENDPOINTTYPE] = value;
            }
        }


        public Dto.EndPoint SelectedEndpoint
        {
            get
            {
                if (ApplicationField.SelectedIndex <= 0)
                    return null;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("id", Convert.ToInt64(ApplicationField.SelectedValue).ToString());
                _mEndpoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetByIdIncludingOwner, parameter);

                return _mEndpoint;
            }
            set
            {
                _mEndpoint = value;
                if (_mEndpoint != null)
                    ApplicationField.SelectedValue = Convert.ToString(Convert.ToInt64(_mEndpoint.Id));
            }
        }
        public Dto.EndPointSystemType SelectedApplication
        {
            get
            {
                if (ApplicationTypeField.SelectedIndex <= 0)
                    return null;
                _mEndPointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetById(Convert.ToInt64(ApplicationTypeField.SelectedValue));

                return _mEndPointSystemType;
            }
            set
            {
                _mEndPointSystemType = value;
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("endpointSystemTypeName", _mEndPointSystemType == null ? "0" : _mEndPointSystemType.EndpointSystemTypeName.ToString());
                _mEndPointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetByAction<EndPointSystemType>
                    (DomainApiAction.Endpoint.EndPointSystemType.GetByEndpointSystemTypeName, parameter);

                ApplicationTypeField.SelectedValue = _mEndPointSystemType != null ? Convert.ToString(Convert.ToInt64(_mEndPointSystemType.Id)) : "";
                BindApplication();
            }
        }
        public bool DisabledApplicationType
        {
            set
            {
                ApplicationTypeField.Enabled = !value;
            }
        }
        public bool DisabledApplication
        {
            set
            {
                ApplicationField.Enabled = !value;
            }
        }
        #endregion

        #region Events
        protected void ApplicationTypeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindApplication();
            if (ApplicationChanged != null)
                ApplicationChanged(SelectedApplication);
        }

        protected void ApplicationField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EndpointChanged != null)
                EndpointChanged(SelectedEndpoint);
        }
        #endregion

        #region Methods
        public void ClearApplicationType()
        {
            ApplicationTypeField.ClearSelection();
            BindApplication();
        }
        public void ClearApplication()
        {
            ApplicationField.ClearSelection();
        }
        public void Initialize()
        {
            if (canEditDropdown)
                ApplicationField.Enabled = true;
            else
                ApplicationField.Enabled = false;
            DivApplicationTypeDropDown.Attributes["class"] = ElementLengthCSS;
            DivApplicationDropDown.Attributes["class"] = ElementLengthCSS;
            ApplicationType.Visible = (DisplayControl == DisplayControl.Both || DisplayControl == DisplayControl.ApplicationType) ? true : false;
            Application.Visible = (DisplayControl == DisplayControl.Both || DisplayControl == DisplayControl.Application) ? true : false;
            if (NotTrustedSource)
            {
                ApplicationType.Attributes["class"] = "form-group required";
                Application.Attributes["class"] = "form-group";
            }
            if (EndpointsOnly)
            {
                BindEndpointsOnly();
                SetClass = "col-xs-6";
            }
            else
            {
                BindApplicationType();
                BindApplication();
            }
        }

        private void BindEndpointsOnly()
        {
            try
            {
                ApplicationField.Items.Clear();
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                List<EndPoint> endpoints = new List<EndPoint>();
                if (!ShowTrustedSource)
                {
                    parameter.Add("includeInhouse", IncludeInhouse.ToString());
                    endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>
                        (DomainApiAction.Endpoint.EndPoint.GetActiveNonTrustedSrcNonInhouseEndpoints, parameter);
                }
                else
                {
                    endpoints = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>
                        (DomainApiAction.Endpoint.EndPoint.GetActiveEndpoints, null);
                }

                ApplicationField.DataTextField = "DisplayName";
                ApplicationField.DataValueField = "Id";
                ApplicationField.DataSource = endpoints;
                ApplicationField.DataBind();
                ApplicationField.Items.Insert(0, "--Select--");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BindApplicationType()
        {
            try
            {
                ApplicationTypeField.Items.Clear();

                var data = DomainApiClient<Dto.EndPointSystemType>.GetByAction<List<EndPointSystemType>>
                    (DomainApiAction.Endpoint.EndPointSystemType.EndPointSystemMethod(MethodName), null);
                data = data.OrderBy(i => i.DisplayName).ToList();
                ApplicationTypeField.DataSource = data;
                ApplicationTypeField.DataBind();
                ApplicationTypeField.Items.Insert(0, "--Select--");
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void BindApplication()
        {
            try
            {
                if ((DisplayControl == DisplayControl.Both && ApplicationTypeField.SelectedIndex > 0) ||
                    (DisplayControl == DisplayControl.Application))
                {
                    long typeId = 0;
                    if (DisplayControl == DisplayControl.Application && ApplicationTypeField.SelectedIndex <= 0)
                        typeId = Convert.ToInt64(EndpointType);
                    else
                        typeId = Convert.ToInt64(ApplicationTypeField.SelectedValue);
                    ApplicationField.Items.Clear();
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("id", typeId.ToString());
                    var ll = DomainApiClient<Dto.EndPoint>.GetByAction<List<EndPoint>>
                        (DomainApiAction.Endpoint.EndPoint.GetByEndpointSystemTypeId, parameter).Where(lq => CheckActive ? lq.IsActive : 1 == 1);
                    if (NotTrustedSource)
                    {
                        ll = ll.Where(e => !e.IsTrustedSource);
                    }

                    ApplicationField.DataTextField = "DisplayName";
                    ApplicationField.DataValueField = "Id";
                    ApplicationField.DataSource = ll.OrderBy(x => x.DisplayName).ToList();
                    ApplicationField.DataBind();
                    ApplicationField.Items.Insert(0, "--Select--");
                }
                else
                {
                    ApplicationField.Items.Clear();
                    ApplicationField.Items.Insert(0, "--Select--");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}