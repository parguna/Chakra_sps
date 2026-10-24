using System;
using System.Collections.Generic;

namespace DI.Chakra.Web.Admin.Attestation.UC
{
    public partial class AttestationReviewHistoryUC : DI.Chakra.Web.UIBaseUC
    {
        #region Members
        private const string V_TASKID = "TASKID";
        private const string V_ATTESTATIONACTIONID = "ATTESTATIONACTIONID";
        #endregion

        #region Property
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

        public long AttestationActionId
        {
            get
            {
                if (ViewState[V_ATTESTATIONACTIONID] != null)
                {
                    return Convert.ToInt64(ViewState[V_ATTESTATIONACTIONID]);
                }
                return 0;
            }
            set
            {
                ViewState[V_ATTESTATIONACTIONID] = value;
            }
        }
        #endregion

        #region Methods
        public void LoadForm()
        {
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("attestationActionId", AttestationActionId.ToString());
            parameter.Add("taskId", TaskId.ToString());
            var reviews = DomainApiClient<Dto.AttestationSnapshotActionHistory>.GetByAction<List<Dto.AttestationSnapshotActionHistory>>(DomainApiAction.Attestation.AttestationSnapshotActionHistory.GetAttestationActionHistory, parameter);

            AttestationActionHistory.Visible = true;
            AttestationActionHistory.DataSource = reviews;
            AttestationActionHistory.DataBind();
        }
        #endregion
    }
}