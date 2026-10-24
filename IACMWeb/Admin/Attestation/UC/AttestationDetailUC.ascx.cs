using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class AttestationDetailUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "ATTESTATION_ID";
        private const string V_TASKID = "TASKID";
        #endregion

        #region Properties
        public long TaskId
        {
            get
            {
                if (ViewState[V_TASKID] != null)
                {
                    return Convert.ToInt64(ViewState[V_TASKID]);
                }
                return 0;
            }
            set
            {
                ViewState[V_TASKID] = value;
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            try
            {
                var task = DomainApiClient<Dto.Task>.GetById(this.TaskId);
                Dto.AttestationCampaign campaign = null;
                if(task.AttestationCampaignId.HasValue)
                    campaign = DomainApiClient<Dto.AttestationCampaign>.GetById(task.AttestationCampaignId.Value);
                Dictionary<string, string> parameterId = new Dictionary<string, string>();
                parameterId.Add("id", Convert.ToString(task.AttestationId));
                Dto.Attestation attestation = DomainApiClient<Dto.Attestation>.GetByAction<Dto.Attestation>
                    (DomainApiAction.Attestation.GetByIdIncludingAttestationProcesses, parameterId);
                var currentApprover = attestation.WorkflowConfiguration.WorkflowConfigurationDetails.
                    FirstOrDefault(item => item.SequenceNumber == task.WorkflowStateId);
                var username = "-";
                if (task.ActionUserId.HasValue)
                {
                    username = DomainApiClient<Dto.User>.GetById(task.ActionUserId.Value).DisplayName;
                }
                else if (task.AssignedTo.HasValue)
                {
                    username = DomainApiClient<Dto.User>.GetById(task.AssignedTo.Value).DisplayName;
                }

                this.lblCurrentReviewer.Text = username;
                this.lblCurrentLevel.Text = task.WorkflowStateId.ToString();
                this.lblAttName.Text = attestation.AttestationName;
                this.lblType.Text = attestation.AttestationType.ToString();
                if (campaign != null)
                    this.lblCampaignName.Text = campaign.CampaignName;
                else
                    this.lblCampaignName.Text = "-";
                this.lblAttestationDesc.Text = attestation.AttestationDescription;
                this.lblReqDate.Text = task.CreatedDate.ToString();
                this.lblSubmitBefore.Text = task.CreatedDate.AddDays(currentApprover.ResponseTimeInDays).ToString();
            }
            catch (System.Exception ex)
            {

            }
        }
        #endregion
    }
}