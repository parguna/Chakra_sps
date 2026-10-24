using DI.Chakra.Dto;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Application;

namespace DI.Chakra.Web.UsrControl.Common.Task.Applications
{
    public partial class InhouseMockTaskInfoUC : UIBaseUC
    {
        #region Members
        private const string V_ID = "RID";
        private const string V_INSTRUCTIONS = "INSTRUCTIONS";
        private const string V_REFERENCENUMBER = "REFERENCENUMBER";
        private const string V_APPROVEDDATE = "APPROVEDDATE";
        private const string V_ENDPOINT = "ENDPOINT";
        private const string V_TASKID = "TASKID";
        #endregion

        #region Properties
        public long? ReferenceAccessRequestId
        {
            get
            {
                if (ViewState[V_TASKID] != null)
                {
                    return Convert.ToInt64(ViewState[V_TASKID]);
                }
                return null;
            }
            set
            {
                ViewState[V_TASKID] = value;
            }
        }

        public Dto.Task Task
        {
            get
            {
                if (ViewState["Task"] != null)
                {
                    return (Dto.Task)ViewState["Task"];
                }
                return new Dto.Task();
            }
            set
            {
                ViewState["Task"] = value;
                if (ViewState["Task"] != null)
                {
                    SetForm((DI.Chakra.Dto.Task)ViewState["Task"]);
                }
            }
        }

        public string Instruction
        {
            get
            {
                if (ViewState[V_INSTRUCTIONS] != null)
                {
                    return Convert.ToString(ViewState[V_INSTRUCTIONS]);
                }
                return string.Empty;
            }
            set
            {
                ViewState[V_INSTRUCTIONS] = value;
            }
        }

        public string ReferenceNumber
        {
            get
            {
                if (ViewState[V_REFERENCENUMBER] != null)
                {
                    return Convert.ToString(ViewState[V_REFERENCENUMBER]);
                }
                return string.Empty;
            }
            set
            {
                ViewState[V_REFERENCENUMBER] = value;
            }
        }

        public DateTime ApprovedDate
        {
            get
            {
                if (ViewState[V_APPROVEDDATE] != null)
                {
                    return Convert.ToDateTime(ViewState[V_APPROVEDDATE]);
                }
                return DateTime.Now.Date;
            }
            set
            {
                ViewState[V_APPROVEDDATE] = value;
            }
        }

        public string RID
        {
            get
            {
                if (ViewState[V_ID] != null)
                {
                    return Convert.ToString(ViewState[V_ID]);
                }
                return string.Empty;
            }
            set
            {
                ViewState[V_ID] = value;
            }
        }

        public string ForEndpoint
        {
            get
            {
                if (ViewState[V_ENDPOINT] != null)
                {
                    return Convert.ToString(ViewState[V_ENDPOINT]);
                }
                return string.Empty;
            }
            set
            {
                ViewState[V_ENDPOINT] = value;
            }
        }
        #endregion

        #region Methods
        public void Initialize()
        {
            try
            {
                this.lblNumber.Text = RID;
                this.lblForEndpoint.Text = ForEndpoint;
                this.lblInstruction.Text = Instruction;
                this.lblReferenceNumber.Text = ReferenceNumber;
                this.lblApprovedDate.Text = ApprovedDate.ToShortDateString();
            }
            catch (System.Exception ex)
            {

            }
        }

        public bool SaveFinalApproval()
        {
            bool result = false;
            if (SaveUserInfo())
            {
                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("id", Task.AccessRequest.AdHocAccessRequest.RequesteeUserId.ToString());
                //User m_user = DomainApiClient<Dto.User>.GetByAction<User>(DomainApiAction.User.GetByIdIncludeBusinessRoleAndOrganisation, parameter);

                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private bool SaveUserInfo()
        {
            try
            {
                //AccountReconcilation newEntry = new AccountReconcilation();
                //if (Task != null && Task.AccessRequest != null)
                //{
                //    var accessRequest = Task.AccessRequest;
                //    newEntry.EndpointSystemId = accessRequest.EndpointSystemId.Value;
                //    newEntry.AccountName = RID;
                //    newEntry.FullName = ForEndpoint;
                //    newEntry.Comments = Instruction;
                //    newEntry.AccountCreatedDate = DateTime.Now;
                //    newEntry.AccountType = Infrastructure.Enums.AccountType.None;
                //    newEntry.CreatedBy = new SessionStatusImpl().User.DisplayName;
                //    newEntry.CreatedDate = DateTime.Now;
                //    newEntry.ProvisionedThrough = Infrastructure.Enums.ProvisionedThrough.Requested.ToString();

                //    if (accessRequest.ActivationDate.HasValue)
                //        newEntry.ValidFromDate = accessRequest.ActivationDate.Value;

                //    if (accessRequest.DeactivationDate.HasValue)
                //        newEntry.ValidFromDate = accessRequest.DeactivationDate.Value;

                //    DomainApiClient<Dto.AccountReconcilation>.Save(newEntry);
                //}

                //Dictionary<string, string> parameter = new Dictionary<string, string>();
                //parameter.Add("id", Task.Id.ToString());
                //parameter.Add("currentUser", new SessionStatusImpl().User.DisplayName);
                //bool saved = DomainApiClient<Dto.InhouseMockInfo>.GetByAction<bool>(DomainApiAction.Endpoint.InhouseSystems.SaveInhouseMockInfo, parameter);
                
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        private void SetForm(DI.Chakra.Dto.Task Task)
        {
            List<PresentationAccessRequestItem> items = (List<PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<PresentationAccessRequestItem>(Task.AccessRequest.AdHocAccessRequest.EntitlementDetail);
            if (items.Count > 0)
            {
                try
                {
                    dynamic detail = JsonConvert.DeserializeObject(items[0].AdditionalInfo);
                    Instruction = detail.Description;
                    ForEndpoint = detail.SubsystemName;
                    RID = detail.SequenceNumber;
                    ReferenceAccessRequestId = detail.ReferenceAccessRequetId;
                    ReferenceNumber = detail.ReferenceNumber;
                    ApprovedDate = Convert.ToDateTime(detail.ApprovedDate);
                    Initialize();
                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion
    }
}