using System;
using System.Linq;
using System.Web.UI;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Admin.Endpoint.Common
{
    public partial class EndpointConfigurationUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "V_ID";
        private const string V_CONTROLSOURCE = "V_CONTROLSOURCE";
        private const string V_ENDPOINTSYSTEMTYPEID = "V_ENDPOINTSYSTEMTYPEID";
        private EndpointConfiguration mConfiguration;
        #endregion

        #region Properties
        public bool IsWoGSelected
        {
            get
            {
                if (ViewState["WOG"] == null)
                    return false;
                return (bool)ViewState["WOG"];
            }
            set
            {
                ViewState["WOG"] = value;
            }
        }
        public bool IsRadioChecked
        {
            get
            {
                if (ViewState["ISRADIOCHECKED"] == null)
                {
                    return false;
                }
                return (bool)ViewState["ISRADIOCHECKED"];
            }
            set
            {
                ViewState["ISRADIOCHECKED"] = value;
            }
        }
        public long EndpointSystemTypeId
        {
            get
            {
                if (ViewState[V_ENDPOINTSYSTEMTYPEID] == null)
                    return 1;
                return (long)ViewState[V_ENDPOINTSYSTEMTYPEID];
            }
            set
            {
                ViewState[V_ENDPOINTSYSTEMTYPEID] = value;
            }
        }
        private List<EndpointKeyConfiguration> ControlSource
        {
            get
            {
                if (ViewState[V_CONTROLSOURCE] == null)
                    return new List<EndpointKeyConfiguration>();
                return (List<EndpointKeyConfiguration>)ViewState[V_CONTROLSOURCE];
            }
            set
            {
                ViewState[V_CONTROLSOURCE] = value;
            }
        }
        public long EntityId
        {
            get
            {
                if (ViewState[V_ID] == null)
                    return -1;
                return (long)ViewState[V_ID];
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }
        public EndpointConfiguration EndpointConfiguration
        {
            get
            {
                mConfiguration = GetConfiguration();
                return mConfiguration;
            }
            set
            {
                mConfiguration = value;
                if (mConfiguration != null)
                {
                    this.EntityId = mConfiguration.Id;
                    if (mConfiguration.ConfigurationDetails.Count > 0)
                        SetConfiguration();
                }
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            //string endpointType = DI.Chakra.Infrastructure.Enums.EndpointType.FileHRMS.ToString();
            EndpointType endpointType = (EndpointType)EndpointSystemTypeId;

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("endpointSystemTypeId", EndpointSystemTypeId.ToString());
            ControlSource = DomainApiClient<EndpointKeyConfiguration>.GetByAction<List<EndpointKeyConfiguration>>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointKeyConfiguration.GetByEndpointSystemTypeId, parameter).OrderBy(order => order.Order).ToList();
            // Dictionary<string, string> parameter = new Dictionary<string, string>();
            // parameter.Add("name", endpointType.ToString());
            // EndPointSystemType endpointSystemType = DomainApiClient<Dto.EndPointSystemType>.GetByAction<Dto.EndPointSystemType>("GetByName", parameter);
            //// Dictionary<string, string> parameter = new Dictionary<string, string>();
            // parameter.Add("endpointSystemTypeId", endpointSystemType.Id.ToString());
            // ControlSource = DomainApiClient<EndpointKeyConfiguration>.GetByAction<List<EndpointKeyConfiguration>>(DomainApiAction.Endpoint.EndPoint.Configuration.EndpointKeyConfiguration.GetByEndpointSystemTypeId, parameter).OrderBy(order => order.Order).ToList();
            ControlSource.ForEach(generateform =>
            {
                HtmlGenericControl divformgroup = new HtmlGenericControl("div");
                divformgroup.ID = Convert.ToString(generateform.EndpointConfigurationKey.KeyName) + "divformgroup";
                divformgroup.Attributes.Add("class", "form-group");
                Label caption = new Label();
                caption.ID = Convert.ToString(generateform.EndpointConfigurationKey.KeyName) + "Label";
                caption.AssociatedControlID = Convert.ToString(generateform.EndpointConfigurationKey.KeyName) + "Field";
                caption.Text = Convert.ToString(generateform.EndpointConfigurationKey.Caption);
                caption.CssClass = "col-xs-4 control-label";
                divformgroup.Controls.Add(caption);
                HtmlGenericControl divcolxs7 = new HtmlGenericControl("div");
                divcolxs7.ID = Convert.ToString(generateform.EndpointConfigurationKey.KeyName) + "divcolxs7";
                divcolxs7.Attributes.Add("class", "col-xs-8");
                ControlSelecion(generateform, divcolxs7);
                //divformgroup.Controls.Add(caption);
                divformgroup.Controls.Add(divcolxs7);
                FormLeftHorizontal.Controls.Add(divformgroup);
            });
        }
        public void ResetPassword()
        {
            TextBox password = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.Password.ToString() + "Field");
            if (password != null)
            {
                password.Text = string.Empty;
                password.Attributes.Add("value", password.Text);
            }
            TextBox azurePassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.AzureADUserPassword.ToString() + "Field");
            if (azurePassword != null)
            {
                azurePassword.Text = string.Empty;
                azurePassword.Attributes.Add("value", azurePassword.Text);
            }

            TextBox bizServicePassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.BizServicePassword.ToString() + "Field");
            if (bizServicePassword != null)
            {
                bizServicePassword.Text = string.Empty;
                bizServicePassword.Attributes.Add("value", bizServicePassword.Text);
            }
            TextBox iMUserPassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.IMUserPassword.ToString() + "Field");
            if (iMUserPassword != null)
            {
                iMUserPassword.Text = string.Empty;
                iMUserPassword.Attributes.Add("value", iMUserPassword.Text);
            }
        }
        public void DisplayViewMode()
        {
            ControlStatus(contentwrapper, true);
        }

        private void ControlSelecion(EndpointKeyConfiguration Configuration, HtmlGenericControl Div)
        {
            EndpointType endpointType = (EndpointType)EndpointSystemTypeId;
            switch (Configuration.DataType)
            {
                case "varchar":
                    TextBox TextBox = new TextBox();
                    if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString()
                        || Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString()
                        || Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString()
                        || Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                    {
                        TextBox.TextMode = TextBoxMode.Password;
                        TextBox.AutoCompleteType = AutoCompleteType.Disabled;
                        TextBox.Attributes.Add("autocomplete", "new-password");
                    }
                    if ((endpointType == EndpointType.ActiveDirectory) &&
                        (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.PortNo.ToString()))
                    {
                        TextBox.Text = "389";
                    }
                    if ((endpointType == EndpointType.ActiveDirectory) &&
                        (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.TrustSource.ToString()))
                    {
                        TextBox.Text = string.Empty;
                        TextBox.Enabled = false;
                    }
                    TextBox.ID = Configuration.EndpointConfigurationKey.KeyName + "Field";
                    if (Configuration.IsRequired && Configuration.EndpointConfigurationKey.KeyName != EndpointKey.TrustSource.ToString())
                        TextBox.Attributes.Add("required", "required");
                    // TextBox.Attributes.Add("pattern", Configuration.RegExValidationPattern);
                    if (!string.IsNullOrEmpty(Configuration.ClientScript))
                    {
                        Configuration.ClientScript.Split('#').ToList().ForEach(lq =>
                        {
                            var script = lq.Split(':');
                            TextBox.Attributes.Add(script[0], script[1]);
                        });
                    }
                    TextBox.CssClass = "form-control";
                    Div.Controls.Add(TextBox);
                    break;
                case "bit":
                    CheckBox CheckBox = new CheckBox();
                    CheckBox.ID = Configuration.EndpointConfigurationKey.KeyName + "Field";
                    if (endpointType == EndpointType.ActiveDirectory)
                    {
                        CheckBox.AutoPostBack = true;
                        CheckBox.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
                    }
                    if (endpointType == EndpointType.InhouseSystems)
                    {
                        CheckBox.AutoPostBack = true;
                        CheckBox.CheckedChanged += new EventHandler(InhCheckBox_CheckedChanged);
                    }
                    try
                    {
                        if (endpointType == EndpointType.SAP && Configuration.EndpointConfigurationKey.KeyName + "Field" == EndpointKey.EnableSSO.ToString()+ "Field")
                        {
                            CheckBox.AutoPostBack = true;
                            CheckBox.CheckedChanged += new EventHandler(CheckBox_SAPCheckedChanged);
                        }
                    }
                    catch
                    {

                    }
                    Div.Attributes.Add("class", "col-xs-4");
                    Div.Controls.Add(CheckBox);
                    HtmlGenericControl span = new HtmlGenericControl("span");
                    span.Attributes.Add("class", "lbl");
                    Div.Controls.Add(span);
                    break;
                case "label":
                    Label lbl = new Label();
                    lbl.ID = Configuration.EndpointConfigurationKey.KeyName + "Field";
                    lbl.Text = Configuration.RegExValidationPattern;
                    Div.Attributes.Add("class", "col-xs-4");
                    Div.Controls.Add(lbl);
                    HtmlGenericControl lblSpan = new HtmlGenericControl("span");
                    lblSpan.Attributes.Add("class", "lbl");
                    Div.Controls.Add(lblSpan);
                    break;

                default:
                    TextBox defaultfield = new TextBox();
                    if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString() ||
                        Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString()
                        || Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString()
                        || Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                        defaultfield.TextMode = TextBoxMode.Password;
                    defaultfield.ID = Configuration.EndpointConfigurationKey.KeyName + "Field";
                    if (Configuration.IsRequired)
                        defaultfield.Attributes.Add("required", "required");

                    if (!string.IsNullOrWhiteSpace(Configuration.RegExValidationPattern))
                        defaultfield.Attributes.Add("pattern", Configuration.RegExValidationPattern);

                    if (!string.IsNullOrEmpty(Configuration.ClientScript))
                    {
                        Configuration.ClientScript.Split('#').ToList().ForEach(lq =>
                        {
                            var script = lq.Split(':');
                            defaultfield.Attributes.Add(script[0], script[1]);
                        });
                    }
                    defaultfield.CssClass = "form-control";
                    Div.Controls.Add(defaultfield);
                    break;
            }
        }

        private void ControlValue(EndpointKeyConfiguration Configuration, EndpointConfigurationDetail ConfigurationObject, bool Get)
        {
            switch (Configuration.DataType)
            {
                case "varchar":
                    TextBox TextBox = (TextBox)FormLeftHorizontal.FindControl(Convert.ToString(Configuration.EndpointConfigurationKey.KeyName) + "Field");
                    if (TextBox != null)
                        if (Get)

                            if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServiceUserName.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                            {
                                ConfigurationObject.Value = RSAHelper.EncryptData(TextBox.Text);
                                ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                            }
                            else
                            {
                                ConfigurationObject.Value = TextBox.Text;
                                ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                            }

                        else
                            if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServiceUserName.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                        {
                            if (ConfigurationObject != null)
                            {
                                TextBox.Attributes.Add("value", RSAHelper.DecryptData(Convert.ToString(ConfigurationObject.Value)));
                            }
                        }
                        else
                        {
                            if (ConfigurationObject != null)
                                TextBox.Text = ConfigurationObject.Value;
                        }
                    break;
                case "bit":
                    CheckBox CheckBox = (CheckBox)FormLeftHorizontal.FindControl(Convert.ToString(Configuration.EndpointConfigurationKey.KeyName) + "Field");
                    if (CheckBox != null)
                        if (Get)
                        {
                            ConfigurationObject.Value = Convert.ToString(CheckBox.Checked);
                            ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                        }
                        else
                        {
                            if (ConfigurationObject != null)
                            {
                                CheckBox.Checked = Convert.ToBoolean(ConfigurationObject.Value);
                                CheckBox chkBoxIsWOG = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.IsWOG.ToString() + "Field");
                                if (chkBoxIsWOG != null)
                                    if (chkBoxIsWOG.Checked)
                                        IsWoGSelected = chkBoxIsWOG.Checked;

                                CheckBox chkBoxCanRunDisc = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.CanRunDiscovery.ToString() + "Field");
                                if (chkBoxCanRunDisc != null)
                                    SetInhouseConfiguration(chkBoxCanRunDisc.Checked);

                                CheckBox chkEnableSSO = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.EnableSSO.ToString() + "Field");
                                if (chkEnableSSO != null)
                                {
                                    try
                                    {
                                        SetSAPConfiguration(chkEnableSSO);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    break;
                case "label":
                    Label lbl = (Label)FormLeftHorizontal.FindControl(Convert.ToString(Configuration.EndpointConfigurationKey.KeyName) + "Field");

                    break;
                case "radio":
                    RadioButton radioButton = (RadioButton)FormLeftHorizontal.FindControl(Convert.ToString(Configuration.EndpointConfigurationKey.KeyName) + "Field");
                    if (radioButton != null)
                        if (Get)
                        {
                            ConfigurationObject.Value = Convert.ToString(radioButton.Checked);
                            ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                        }
                        else
                        {
                            radioButton.Checked = Convert.ToBoolean(ConfigurationObject.Value);
                        }
                    break;
                default:
                    TextBox defaultfield = (TextBox)FormLeftHorizontal.FindControl(Convert.ToString(Configuration.EndpointConfigurationKey.KeyName) + "Field");
                    if (defaultfield != null)
                        if (Get)
                            if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServiceUserName.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                            {
                                ConfigurationObject.Value = RSAHelper.EncryptData(defaultfield.Text);
                                ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                            }
                            else
                            {
                                ConfigurationObject.Value = defaultfield.Text;
                                ConfigurationObject.EndpointConfigurationKey = Configuration.EndpointConfigurationKey;
                            }
                        else
                            if (Configuration.EndpointConfigurationKey.KeyName == EndpointKey.Password.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.AzureADUserPassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServicePassword.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.BizServiceUserName.ToString() ||
                                Configuration.EndpointConfigurationKey.KeyName == EndpointKey.IMUserPassword.ToString())
                            defaultfield.Attributes.Add("value", RSAHelper.DecryptData(ConfigurationObject.Value));
                        else
                            defaultfield.Text = ConfigurationObject.Value;
                    break;
            }
        }

        private void SetConfiguration()
        {
            ControlSource.ForEach(setform =>
            {
                ControlValue(setform, mConfiguration.ConfigurationDetails.Where(lq => lq.KeyId == setform.KeyId).FirstOrDefault(), false);
            });
        }

        private EndpointConfiguration GetConfiguration()
        {
            Dto.EndpointConfiguration endpointconfig = new EndpointConfiguration();
            endpointconfig.Id = this.EntityId;
            List<EndpointConfigurationDetail> configdetails = new List<EndpointConfigurationDetail>();
            ControlSource.ForEach(getfromcontrol =>
            {
                EndpointConfigurationDetail configuration = new EndpointConfigurationDetail
                {
                    KeyId = getfromcontrol.KeyId,
                };
                ControlValue(getfromcontrol, configuration, true);
                configdetails.Add(configuration);
            });
            endpointconfig.ConfigurationDetails = configdetails;
            return endpointconfig;
        }

        private void ControlStatus(Control control, bool isDisable)
        {
            foreach (Control c in control.Controls)
            {
                if (c.HasControls())
                    ControlStatus(c, isDisable);
                else
                {
                    if (isDisable)
                    {
                        if (c is WebControl)
                            ((WebControl)c).Enabled = false;
                    }
                    else
                    {
                        if (c is WebControl)
                            ((WebControl)c).Enabled = true;
                    }
                }
            }
        }
        #endregion

        #region Event
        protected override void OnPreRender(EventArgs e)
        {
            EndpointType endpointType = (EndpointType)EndpointSystemTypeId;
            CheckBox checkbox = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.SSL.ToString() + "Field");
            CheckBox checkboxInh = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.CanRunDiscovery.ToString() + "Field");

            if ((endpointType == EndpointType.InhouseSystems) && (checkboxInh != null))
            {
                checkboxInh.AutoPostBack = true;
                checkboxInh.CheckedChanged += new EventHandler(InhCheckBox_CheckedChanged);
            }

            if ((endpointType == EndpointType.ActiveDirectory) && (checkbox != null))
            {
                checkbox.AutoPostBack = true;
                checkbox.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
            }
            CheckBox chkBoxIsWOG = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.IsWOG.ToString() + "Field");
            if ((endpointType == EndpointType.ActiveDirectory) && (chkBoxIsWOG != null))
            {
                chkBoxIsWOG.AutoPostBack = true;
                chkBoxIsWOG.CheckedChanged += new EventHandler(CheckBox_CheckedChanged);
            }

            TextBox password = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.Password.ToString() + "Field");
            if (password != null)
            {
                if ((ViewState["PASSWORD"] == null) || (ViewState["PASSWORD"] == ""))
                {
                    ViewState["PASSWORD"] = string.IsNullOrEmpty(password.Text) ? password.Attributes["value"] : password.Text;
                    password.Attributes.Add("value", (string)ViewState["PASSWORD"]);
                }
                else if (password.Text != "")
                {
                    ViewState["PASSWORD"] = string.IsNullOrEmpty(password.Text) ? password.Attributes["value"] : password.Text;
                    password.Attributes.Add("value", (string)ViewState["PASSWORD"]);
                }
            }
            TextBox azurePassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.AzureADUserPassword.ToString() + "Field");
            if (azurePassword != null)
            {
                if ((ViewState["AZUREPASSWORD"] == null) || (ViewState["AZUREPASSWORD"] == ""))
                {
                    ViewState["AZUREPASSWORD"] = string.IsNullOrEmpty(azurePassword.Text) ? azurePassword.Attributes["value"] : azurePassword.Text;
                    azurePassword.Attributes.Add("value", (string)ViewState["AZUREPASSWORD"]);
                }
                else if (azurePassword.Text != "")
                {
                    ViewState["AZUREPASSWORD"] = string.IsNullOrEmpty(azurePassword.Text) ? azurePassword.Attributes["value"] : azurePassword.Text;
                    azurePassword.Attributes.Add("value", (string)ViewState["AZUREPASSWORD"]);
                }
            }
            TextBox bizServicepassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.BizServicePassword.ToString() + "Field");
            if (bizServicepassword != null)
            {
                if ((ViewState["BizServicepassword"] == null) || (ViewState["BizServicepassword"] == ""))
                {
                    ViewState["BizServicepassword"] = string.IsNullOrEmpty(bizServicepassword.Text) ? bizServicepassword.Attributes["value"] : bizServicepassword.Text;
                    bizServicepassword.Attributes.Add("value", (string)ViewState["BizServicepassword"]);
                }
                else if (bizServicepassword.Text != "")
                {
                    ViewState["BizServicepassword"] = string.IsNullOrEmpty(bizServicepassword.Text) ? bizServicepassword.Attributes["value"] : bizServicepassword.Text;
                    bizServicepassword.Attributes.Add("value", (string)ViewState["BizServicepassword"]);
                }
            }
            TextBox imServicepassword = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.IMUserPassword.ToString() + "Field");
            if (imServicepassword != null)
            {
                if ((ViewState["IMUserPassword"] == null) || (ViewState["IMUserPassword"] == ""))
                {
                    ViewState["IMUserPassword"] = string.IsNullOrEmpty(imServicepassword.Text) ? imServicepassword.Attributes["value"] : imServicepassword.Text;
                    imServicepassword.Attributes.Add("value", (string)ViewState["IMUserPassword"]);
                }
                else if (imServicepassword.Text != "")
                {
                    ViewState["IMUserPassword"] = string.IsNullOrEmpty(imServicepassword.Text) ? imServicepassword.Attributes["value"] : imServicepassword.Text;
                    imServicepassword.Attributes.Add("value", (string)ViewState["IMUserPassword"]);
                }
            }

            base.OnPreRender(e);
        }
        #endregion
        protected void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            HtmlGenericControl divTrustedSrc = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.TrustSource + "divformgroup");
            TextBox textBoxTrustedSrc = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.TrustSource.ToString() + "Field");
            if (checkbox.ID == EndpointKey.SSL.ToString() + "Field")
            {
                TextBox textbox = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.PortNo.ToString() + "Field");
                if (textbox != null)
                {
                    if (checkbox.Checked)
                        textbox.Text = "636";
                    else
                        textbox.Text = "389";
                }
            }
            else if (checkbox.ID == EndpointKey.IsWOG.ToString() + "Field")
            {
                HtmlGenericControl div = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.IsOneWayTrust + "divformgroup");
                if (div != null) div.Visible = !checkbox.Checked;
                if (divTrustedSrc != null) divTrustedSrc.Visible = !checkbox.Checked;
                this.IsWoGSelected = checkbox.Checked;
                CheckBox chkBoxIsOneWayTrust = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.IsOneWayTrust.ToString() + "Field");
                if (checkbox.Checked && (chkBoxIsOneWayTrust != null))
                {
                    chkBoxIsOneWayTrust.Checked = !checkbox.Checked;
                    textBoxTrustedSrc.Text = string.Empty;
                }
            }
            else if (checkbox.ID == EndpointKey.IsOneWayTrust.ToString() + "Field")
            {
                HtmlGenericControl div1 = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.IsWOG + "divformgroup");
                CheckBox chkBoxIsWOG = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.IsWOG.ToString() + "Field");
                if (divTrustedSrc != null && textBoxTrustedSrc != null && chkBoxIsWOG != null)
                {
                    if (checkbox.Checked)
                    {
                        textBoxTrustedSrc.Attributes.Add("required", "required");
                        divTrustedSrc.Attributes.Add("class", "form-group required");
                        chkBoxIsWOG.Checked = IsWoGSelected = !checkbox.Checked;
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("key", "IsWOG");
                        parameter.Add("value", Boolean.TrueString);
                        var endpoint = DomainApiClient<Dto.EndPoint>.GetByAction<List<Dto.EndPoint>>
                           (DomainApiAction.Endpoint.EndPoint.GetByEndpointConfiguration, parameter).FirstOrDefault();
                        if (endpoint != null)
                            textBoxTrustedSrc.Text = endpoint.DisplayName;
                    }
                    else
                    {
                        textBoxTrustedSrc.Attributes.Remove("required");
                        divTrustedSrc.Attributes.Remove("class");
                        divTrustedSrc.Attributes.Add("class", "form-group");
                        textBoxTrustedSrc.Text = string.Empty;
                    }
                }
            }           
        }

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked) { IsRadioChecked = true; } else { IsRadioChecked = false; }

            TextBox textBoxFinance = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.FinanceFilePath.ToString() + "Field");
            TextBox textBoxProcurement = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.ProcurementFilePath.ToString() + "Field");
            HtmlGenericControl financeDiv = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.FinanceFilePath + "divformgroup");
            HtmlGenericControl procurementDiv = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.ProcurementFilePath + "divformgroup");
            if (radioButton.Checked && radioButton.ID == EndpointKey.Finance.ToString() + "Field")
            {
                if (financeDiv != null && procurementDiv != null)
                {
                    financeDiv.Visible = true;
                    procurementDiv.Visible = false;
                    textBoxFinance.Text = string.Empty;
                }
            }
            else if (radioButton.Checked && radioButton.ID == EndpointKey.Procurement.ToString() + "Field")
            {
                if (financeDiv != null && procurementDiv != null)
                {
                    financeDiv.Visible = false;
                    procurementDiv.Visible = true;
                    textBoxProcurement.Text = string.Empty;
                }
            }
        }

        protected void InhCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            //HtmlGenericControl divTrustedSrc = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.TrustSource + "divformgroup");
            //TextBox textBoxTrustedSrc = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.TrustSource.ToString() + "Field");
            if (checkbox.ID == EndpointKey.CanRunDiscovery.ToString() + "Field")
            {
                SetInhouseConfiguration(checkbox.Checked);
            }

        }

        private void SetInhouseConfiguration(bool canRunDiscovery)
        {
            TextBox textboxUsername = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.UserName.ToString() + "Field");
            TextBox textboxPwd = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.Password.ToString() + "Field");
            TextBox textboxUser = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.UserFilePath.ToString() + "Field");
            TextBox textboxRole = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.RoleFilePath.ToString() + "Field");
            TextBox textboxFunction = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.FunctionFilePath.ToString() + "Field");
            TextBox textboxPriv = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.PrivillegeAcctTransFilePath.ToString() + "Field");

            if (canRunDiscovery)
            {
                textboxUsername.Enabled = true;
                textboxPwd.Enabled = true;
                textboxUser.Enabled = true;
                textboxRole.Enabled = true;
                textboxFunction.Enabled = true;
                textboxPriv.Enabled = true;
            }
            else
            {
                textboxUsername.Enabled = false;
                textboxPwd.Enabled = false;
                textboxUser.Enabled = false;
                textboxRole.Enabled = false;
                textboxFunction.Enabled = false;
                textboxPriv.Enabled = false;
            }
        }
        protected void CheckBox_SAPCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox checkbox = (CheckBox)sender;
                if (checkbox.ID == EndpointKey.EnableSSO.ToString() + "Field")
                {
                    CheckBox chkBoxEnableSSO = (CheckBox)FormLeftHorizontal.FindControl(EndpointKey.EnableSSO.ToString() + "Field");
                    if (chkBoxEnableSSO != null)
                    {
                        SetSAPConfiguration(checkbox);
                    }
                }
            }
            catch 
            {
            }
        }
        private void SetSAPConfiguration(CheckBox chkSAPEnableSSO)
        {
            try
            {
                TextBox txtSNCNamePrefix = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.SNCNamePrefix.ToString() + "Field");
                HtmlGenericControl divSNCNamePrefix = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.SNCNamePrefix + "divformgroup");

                TextBox txtSNCNameSuffix = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.SNCNameSuffix.ToString() + "Field");
                HtmlGenericControl divSNCNameSuffix = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.SNCNameSuffix + "divformgroup");

                TextBox txtSNCParameterName = (TextBox)FormLeftHorizontal.FindControl(EndpointKey.SNCParameterName.ToString() + "Field");
                HtmlGenericControl divSNCParameterName = (HtmlGenericControl)FormLeftHorizontal.FindControl(EndpointKey.SNCParameterName + "divformgroup");
                if (chkSAPEnableSSO.Checked)
                {

                    if (txtSNCNamePrefix != null && divSNCNamePrefix != null)
                    {
                        txtSNCNamePrefix.Attributes.Add("required", "required");
                        divSNCNamePrefix.Attributes.Add("class", "form-group required");
                    }
                    if (txtSNCNameSuffix != null && divSNCNameSuffix != null)
                    {
                        txtSNCNameSuffix.Attributes.Add("required", "required");
                        divSNCNameSuffix.Attributes.Add("class", "form-group required");
                    }
                    if (txtSNCParameterName != null && divSNCParameterName != null)
                    {
                        txtSNCParameterName.Attributes.Add("required", "required");
                        divSNCParameterName.Attributes.Add("class", "form-group required");
                    }
                }
                else
                {
                    if (txtSNCNamePrefix != null && divSNCNamePrefix != null)
                    {
                        txtSNCNamePrefix.Attributes.Remove("required");
                        divSNCNamePrefix.Attributes.Add("class", "form-group");
                    }
                    if (txtSNCNameSuffix != null && divSNCNameSuffix != null)
                    {
                        txtSNCNameSuffix.Attributes.Remove("required");
                        divSNCNameSuffix.Attributes.Add("class", "form-group");
                    }
                    if (txtSNCParameterName != null && divSNCParameterName != null)
                    {
                        txtSNCParameterName.Attributes.Remove("required");
                        divSNCParameterName.Attributes.Add("class", "form-group");
                    }
                }

                if (txtSNCNamePrefix != null)
                {
                    txtSNCNamePrefix.Enabled = chkSAPEnableSSO.Checked;
                }
                if (txtSNCNameSuffix != null)
                {
                    txtSNCNameSuffix.Enabled = chkSAPEnableSSO.Checked;
                }
                if (txtSNCParameterName != null)
                {
                    txtSNCParameterName.Enabled = chkSAPEnableSSO.Checked;
                }
            }
            catch 
            {
            }
        }
    }
}