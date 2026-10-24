using System;
using Application;
using System.Collections.Generic;

namespace DI.Chakra.Web.Delegation
{
    public partial class AddAdvance :UIBasePage
    {
        #region Members
        private const string V_DELEGATION = "DELEGATION";
        private const string V_ID = "ENTITY_ID";
        private SessionStatusImpl SessionUser = new SessionStatusImpl();
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
                SetPage();
            }
        }

        public Dto.TaskDelegation Delegation
        {
            get
            {
                if (ViewState[V_DELEGATION] == null)
                    return new Dto.TaskDelegation();
                return (Dto.TaskDelegation)ViewState[V_DELEGATION];
            }
            set
            {
                ViewState[V_DELEGATION] = value;
            }
        }
        #endregion

        #region Methods
        private void SetPage()
        {
            Delegation = DomainApiClient<Dto.TaskDelegation>.GetById(EntityId);
            if (Delegation.Id == 0)
                Delegation = new Dto.TaskDelegation();

            Dictionary<String, string> parameter = new Dictionary<String, string>();
            parameter.Add("id", Delegation.DelegatedTo.ToString());
            DelegateTo.SelectedUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

            Dictionary<String, string> parameters = new Dictionary<String, string>();
            parameters.Add("id", Delegation.ActionUser.ToString());
                DelegateFrom.SelectedUser = DomainApiClient<Dto.User>.GetByAction<Dto.User>
                (DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameters);

            StartDate.SelectedDate = Delegation.StartDate;
            EndDate.SelectedDate = Delegation.EndDate;
            Attestation.Checked = Delegation.DelegateAttestationTask;
            Provisioning.Checked = Delegation.DelegateProvisioningTask;
            Comment.Text = Delegation.DelegationComments;
            IsActive.Checked = Delegation.IsActive;
        }

        private void Save()
        {
            Delegation.ActionUser = DelegateFrom.SelectedUser.Id;
            Delegation.DelegatedTo = DelegateTo.SelectedUser.Id;
            Delegation.StartDate = StartDate.SelectedDate.Value;
            Delegation.EndDate = EndDate.SelectedDate.Value;
            Delegation.DelegateAttestationTask = Attestation.Checked;
            Delegation.DelegateProvisioningTask = Provisioning.Checked;
            Delegation.DelegationComments = Comment.Text;
            Delegation.IsActive = IsActive.Checked;
            if (Delegation.Id != 0)
            {
                DomainApiClient<Dto.TaskDelegation>.Save(Delegation); ;
            }
            else
            {
                DomainApiClient<Dto.TaskDelegation>.Save(Delegation); ;
            }
            //send pending task email to new approver
            Dto.TaskDelegationWrapper wrapper = new Dto.TaskDelegationWrapper();
            wrapper.Delegation = Delegation;
            wrapper.SelectedtaskList = new List<long>() { Delegation.Id };
            DomainApiClient<Dto.TaskDelegation>.GetByAction<Dto.TaskDelegation>
                (DomainApiAction.Tasks.TaskDelegation.CreateAdvanceTasksDelegationEmail, null, wrapper);
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EntityId = Request.QueryString["Id"] != null ? Convert.ToInt64(Request.QueryString["Id"]) : 0;
                
            }
            if (SessionUser.isAdminGroup || SessionUser.isIACMAdminGroup)
                DelegateFrom.Disabled = false;
            else
            {
                DelegateFrom.SelectedUser = SessionUser.User;
                DelegateFrom.Disabled = true;
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
            Response.Redirect("List.aspx");
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("List.aspx");
        }
        #endregion
    }
}