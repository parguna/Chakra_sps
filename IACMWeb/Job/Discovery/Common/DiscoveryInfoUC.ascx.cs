using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Job.Discovery.Common
{
    public partial class DiscoveryInfoUC : DI.Chakra.Web.UIBaseUC
    {
        #region Class Variables
        private Dto.DiscoveryProcess processConfig = null;
        private bool isEdit = false;
        private bool isView = false;
        private const string V_ID = "ENTITY_ID";

        #endregion

        #region Properties

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
        public bool IsEdit
        {
            set { isEdit = value; }
            get { return isEdit; }
        }
        public bool IsView
        {
            set { isView = value; }
            get { return isView; }
        }
        public Dto.DiscoveryProcess DiscoveryProcessConfig
        {
            set { processConfig = value; }
            get { return processConfig; }
        }
        public string ProcessName
        {
            get { return txtProcessName.Text; }

        }
        public string Description
        {
            get { return txtDescription.Text; }
        }
        public Dto.EndPoint EndPoint
        {
            get
            {
                return EndpointUC.SelectedEndpoint;
            }
        }

        #endregion

        #region Public Methods

        public void InitilizePage()
        {
            if (IsEdit || IsView)
            {
                txtProcessName.Text = processConfig.ProcessName;
                txtDescription.Text = processConfig.Description;
                chkDeltaDiscovery.Checked = processConfig.DisableDeltaDiscovery;
                EndpointUC.CheckActive = Convert.ToString(Parent.Page).Contains("add") ? true : false;
                EndpointUC.Initialize();
                EndpointUC.SelectedApplication = processConfig.Endpoint.EndPointSystemType;
                EndpointUC.SelectedEndpoint = processConfig.Endpoint;
                ddlIsQueryProcess.SelectedValue = "1";
                //if (isView)
                //    EndpointUC.DisabledApplication = true;
                if (processConfig != null && processConfig.process != null &&
                        processConfig.process.DiscoveryInstance == (int)DiscoveryServiceInstance.SAPInstance)
                {
                    ddlIsQueryProcess.SelectedValue = "2";
                }


                if (EndPoint == null)
                    return;

                long endpointSystemType = (long)EndPoint.EndpointSystemTypeId.Value;

                if (processConfig.Endpoint.IsTrustedSource)
                    GenerateTrustedSourceDynamicControls(endpointSystemType, null, null);
                else
                {
                    if (EndPoint != null)
                        GenerateDynamicControls(endpointSystemType, EndPoint.Id, null);
                    else
                        GenerateDynamicControls(endpointSystemType, null, null);
                }

                //   ViewState["Generated"] = "true";
            }
            else
            {
                EndpointUC.Initialize();
            }
        }

        public List<Dto.EndpointObjects> GetSelectedEndPointObjects()
        {
            List<Dto.EndpointObjects> objects = new List<Dto.EndpointObjects>();
            foreach (RepeaterItem item in rptEndPointObject.Items)
            {
                System.Web.UI.HtmlControls.HtmlInputCheckBox chkBox = GetCheckBoxControl(item.Controls);
                if (chkBox != null)
                {
                    if (chkBox.Checked)
                    {
                        HiddenField hdnField = GetHiddenControl(item.Controls);
                        long endpointObjectId = Convert.ToInt64(hdnField.Value);
                        Dto.EndpointObjects obj = DomainApiClient<Dto.EndpointObjects>.GetById(endpointObjectId);
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                }
            }

            FindControls(objects, pnlEndPointObjects.Controls);
            return objects;
        }

        public Dto.DiscoveryProcess getDiscoveryProcess()
        {
            Dto.DiscoveryProcess disovery = new Dto.DiscoveryProcess();
            disovery.ProcessName = ProcessName;
            disovery.Description = Description;

            if (EndPoint != null)
            {
                disovery.Endpoint = EndPoint;
                disovery.Endpointsystemid = EndPoint.Id;
            }

            disovery.DisableDeltaDiscovery = chkDeltaDiscovery.Checked;
            if (ddlIsQueryProcess.SelectedValue == "1")
            {
                disovery.DiscoveryInstance = 1;
            }
            else
                disovery.DiscoveryInstance = 2;

            List<Dto.EndpointObjects> objects = GetSelectedEndPointObjects();
            if (objects.Count > 0)
            {
                disovery.EndpointObjects = new List<Dto.EndpointObjects>();
                foreach (Dto.EndpointObjects obj in objects)
                {
                    disovery.EndpointObjects.Add(obj);
                }
            }
            return disovery;
        }
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }

        #endregion

        #region Page and Control Events
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (EndPoint != null)
            //{
            //    int endpointSystemType = EndPoint.EndpointSystemTypeId.Value;
            //    if (Convert.ToString(ViewState["Generated"]) == "true")
            //    {
            //        pnlEndPointObjects.Controls.Clear();
            //        if (IsEdit)
            //        {
            //            if (processConfig.EndpointObjects != null)
            //            {
            //                GenerateDynamicControls(endpointSystemType, processConfig.EndpointObjects);
            //            }
            //        }
            //        else
            //        {
            //            GenerateDynamicControls(endpointSystemType, null);
            //        }

            //    }
            //}
        }
        protected void EndpointUC_EndpointChanged(EndPoint _EndPoint)
        {
            if (_EndPoint.IsTrustedSource)
            {
                //if the endpoint is trusted source, then irrespective of endpoint system id, load only trusted source specific discovery objects
                GenerateTrustedSourceDynamicControls(_EndPoint.EndpointSystemTypeId.Value, null, null);
            }
            else if (_EndPoint.EndpointSystemTypeId == (long)Infrastructure.Enums.EndpointType.ActiveDirectory)
            {
                GenerateDynamicControls((long)Infrastructure.Enums.EndpointType.ActiveDirectory, _EndPoint.Id, null);
            }

            // pnlEndPointObjects.Controls.Clear();
            //if (_EndPoint != null)
            //{
            //    long endpointSystemType = (long)EndPoint.EndpointSystemTypeId.Value;
            //GenerateDynamicControls(endpointSystemType, null);
            //}
            //if (Convert.ToString(ViewState["Generated"]) != "true")
            //{
            //    GenerateDynamicControls(endpointSystemType,null);
            //    ViewState["Generated"] = "true";
            //}
        }


        protected void EndpointUC_ApplicationChanged(DI.Chakra.Dto.EndPointSystemType EndPointSystemType)
        {
            //  pnlEndPointObjects.Controls.Clear();

            if (EndPointSystemType != null)
            {
                long endpointSystemType = (long)EndPointSystemType.Id;
                GenerateDynamicControls(endpointSystemType, null, null);
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Method responsible for Find all the checkbox control inside panel.
        /// Created by Harihara : 21/7/2015.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="controls"></param>
        private void FindControls(List<Dto.EndpointObjects> objects, ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is CheckBox)
                {
                    CheckBox chk = (CheckBox)ctrl;
                    if (chk.Checked)
                    {
                        long endpointObjectId = Convert.ToInt64(chk.ID);
                        Dto.EndpointObjects obj = DomainApiClient<Dto.EndpointObjects>.GetById(endpointObjectId);
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                }
                else if (ctrl.Controls.Count > 0)
                {
                    FindControls(objects, ctrl.Controls);
                }
            }
        }
        private System.Web.UI.HtmlControls.HtmlInputCheckBox GetCheckBoxControl(ControlCollection controls)
        {
            System.Web.UI.HtmlControls.HtmlInputCheckBox chkBox = null;
            foreach (Control ctrl in controls)
            {
                if (ctrl is System.Web.UI.HtmlControls.HtmlInputCheckBox)
                {
                    chkBox = (System.Web.UI.HtmlControls.HtmlInputCheckBox)ctrl;
                    break;
                }
                else if (ctrl.Controls.Count > 0)
                {
                    GetCheckBoxControl(ctrl.Controls);
                }
            }
            return chkBox;
        }
        private HiddenField GetHiddenControl(ControlCollection controls)
        {
            HiddenField hdnField = null;
            foreach (Control ctrl in controls)
            {
                if (ctrl is HiddenField)
                {
                    hdnField = (HiddenField)ctrl;
                    break;
                }
                else if (ctrl.Controls.Count > 0)
                {
                    GetHiddenControl(ctrl.Controls);
                }
            }
            return hdnField;
        }

        /// <summary>
        /// Method responsible for Creating Endpoint Objects checkbox control based on selected endpointsystem
        /// Created by Harihara : 21/7/2015.
        /// </summary>
        /// <param name="endpointSystemType"></param>
        /// <param name="endPointObjects"></param>
        private void GenerateDynamicControls(long endpointSystemType, long? endpointId, IList<Dto.EndpointObjects> endPointObjects)
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("endpointId", endpointId.ToString());
            parameter.Add("endPointSystemTypeId", endpointSystemType.ToString());

            List<Dto.EndpointObjects> objects = DomainApiClient<Dto.EndpointObjects>.GetByAction<List<Dto.EndpointObjects>>
                (DomainApiAction.Endpoint.EndpointObjects.GetByEndpointSystemTypeId, parameter);

            var NonTRSObjs = objects.Where(t => t.IsTrustedSource == false).ToList();

            if (NonTRSObjs.Count > 0)
            {
                rptEndPointObject.DataSource = NonTRSObjs;
                rptEndPointObject.DataBind();
            }
        }
        private void GenerateTrustedSourceDynamicControls(long endpointSystemType, long? endpointId, IList<Dto.EndpointObjects> endPointObjects)
        {
            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("endPointSystemTypeId", endpointSystemType.ToString());

            List<Dto.EndpointObjects> objects = DomainApiClient<Dto.EndpointObjects>.GetByAction<List<Dto.EndpointObjects>>
                (DomainApiAction.Endpoint.EndpointObjects.GetTrustedSourceObjectsByEndpointSystemType, parameter);

            if (objects.Count > 0)
            {
                rptEndPointObject.DataSource = objects;
                rptEndPointObject.DataBind();
            }
        }
        #endregion

        #region Events

        protected void rptEndPointObject_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                System.Web.UI.HtmlControls.HtmlInputCheckBox chkBox = GetCheckBoxControl(e.Item.Controls);
                if (chkBox != null && processConfig != null)
                {
                    Dto.EndpointObjects selectedObj = processConfig.EndpointObjects != null ? processConfig.EndpointObjects.Where
                        (m => m.ObjectName == chkBox.Value).FirstOrDefault<Dto.EndpointObjects>() : null;
                    chkBox.Checked = selectedObj != null ? true : false;
                    if (IsView)
                        chkBox.Disabled = true;
                }
            }

        }



        private void ViewOnlyMode()
        {
            contentWrapper.Disabled = true;
        }

        #endregion


    }
}