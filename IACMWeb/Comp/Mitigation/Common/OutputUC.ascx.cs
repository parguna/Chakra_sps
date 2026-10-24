using Application;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Dto;
using System;

namespace DI.Chakra.Web.Comp.Mitigation.Common
{
    public partial class OutputUC : UIBaseUC
    {
        #region Class Member Variables
        SessionStatusImpl session = new SessionStatusImpl();
        private bool isEdit = false;
        private bool isView = false;
        SODMitigation mitigation;
        private const string V_ID = "ENTITY_ID";
        private UIMode uiMode;
        #endregion

        #region Public Properties
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

        public UIMode UIMode
        {
            get { return uiMode; }
            set { uiMode = value; }
        }

        public SODMitigation SODMitigation
        {
            get { return mitigation; }
            set { mitigation = value; }
        }

        public User SelectedRevUser
        {
            get
            {
                return Reviewer.SelectedUser;
            }
            set
            {
                Reviewer.SelectedUser = value;
            }
        }

        public Group SelectedRevGroup
        {
            get
            {
                return Reviewer.SelectedGroup;
            }
            set
            {
                Reviewer.SelectedGroup = value;
            }
        }

        public User SelectedAppUser
        {
            get { return Approver.SelectedUser; }
            set { Approver.SelectedUser = value; }
        }

        public Group SelectedAppGroup
        {
            get
            {
                return Approver.SelectedGroup;
            }
            set
            {
                Approver.SelectedGroup = value;
            }
        }
        #endregion

        #region Page and Control Events
        /// <summary>
        /// Loading the Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Event to handle the Radio button closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radClosed_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.Reviewer.DataMethodField = "GetAllGroups";
                radActive.Checked = false;
                this.Reviewer.ClearAppUser();
            }
        }

        /// <summary>
        /// Event to handle the Radio button Active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void radActive_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.Reviewer.DataMethodField = "GetUsers";
                radClosed.Checked = false;
                this.Reviewer.ClearAppUser();
            }
        }

        /// <summary>
        /// Event to handle the Radio button Active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadAppr1_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.Approver.DataMethodField = "GetUsers";
                RadAppr2.Checked = false;
                this.Approver.ClearAppUser();
            }
        }

        /// <summary>
        /// Event to handle the Radio button Closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadAppr2_CheckedChanged(object sender, EventArgs e)
        {
            if (((System.Web.UI.WebControls.CheckBox)(sender)).Checked.Equals(true))
            {
                this.Approver.DataMethodField = "GetAllGroups";
                RadAppr1.Checked = false;
                this.Approver.ClearAppUser();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gettting Mitigation object
        /// </summary>
        /// <param name="mitigation"></param>
        public void GetMitigOutput(ref SODMitigation mitigation)
        {
            if (radActive.Checked)
            {
                mitigation.MonitorUserId = this.SelectedRevUser != null ? this.SelectedRevUser.Id : (long?)null;
                mitigation.MonitorGroupId = null;
            }
            else
            {
                mitigation.MonitorGroupId = this.SelectedRevGroup != null ? this.SelectedRevGroup.Id : (long?)null;
                mitigation.MonitorUserId = null;
            }

            if (RadAppr1.Checked)
            {
                mitigation.ApproverUserId = this.SelectedAppUser != null ? this.SelectedAppUser.Id : (long?)null;
                mitigation.ApproverGroupId = null;
            }
            else
            {
                mitigation.ApproverGroupId = this.SelectedAppGroup != null ? this.SelectedAppGroup.Id : (long?)null;
                mitigation.ApproverUserId = null;
            }
        }

        /// <summary>
        /// Method to show the screen in the View Mode
        /// </summary>
        public void DisplayMode()
        {
            UtilityMethods.DisableControls(contentWrapper);
        }
        #endregion

        #region Initialise Section
        /// <summary>
        /// Method to Initialize the page
        /// </summary>
        public void IntializePage()
        {
            if (uiMode == UIMode.Edit || uiMode == UIMode.View)
            {
                this.radActive.Checked = this.SODMitigation.MonitorUserId != null ? true : false;
                if (radActive.Checked)
                {
                    if (this.Reviewer.SelectedUser != null)
                        this.Reviewer.SelectedUser.Id = this.SODMitigation.MonitorUserId.Value;
                    else
                        this.Reviewer.SelectedUser = DomainApiClient<User>.GetById((this.SODMitigation.MonitorUserId.Value));
                    this.Reviewer.DataMethodField = "GetUsers";
                    this.radClosed.Checked = false;
                }
                else
                {
                    if (this.Reviewer.SelectedGroup != null)
                        this.Reviewer.SelectedUser.Id = this.SODMitigation.MonitorGroupId.Value;
                    else
                        this.Reviewer.SelectedGroup = DomainApiClient<Group>.GetById((this.SODMitigation.MonitorGroupId.Value));
                    this.radClosed.Checked = true;
                    this.radActive.Checked = false;
                    this.Reviewer.DataMethodField = "GetAllGroups";
                }
                this.RadAppr1.Checked = this.SODMitigation.ApproverUserId != null ? true : false;
                if (RadAppr1.Checked)
                {
                    if (this.Approver.SelectedUser != null)
                        this.Approver.SelectedUser.Id = this.SODMitigation.ApproverUserId.Value;
                    else
                        this.Approver.SelectedUser = DomainApiClient<User>.GetById((this.SODMitigation.ApproverUserId.Value));
                    this.Approver.DataMethodField = "GetUsers";
                    this.RadAppr2.Checked = false;
                }
                else
                {
                    if (this.Approver.SelectedGroup != null)
                        this.Approver.SelectedUser.Id = this.SODMitigation.ApproverGroupId.Value;
                    else
                        this.Approver.SelectedGroup = DomainApiClient<Group>.GetById((this.SODMitigation.ApproverGroupId.Value)); ;
                    RadAppr2.Checked = true;
                    RadAppr1.Checked = false;
                    this.Approver.DataMethodField = "GetAllGroups";
                }
            }
            else
            {
                this.Reviewer.DataMethodField = "GetUsers";
                this.Approver.DataMethodField = "GetUsers";
            }
        }
        #endregion
    }
}