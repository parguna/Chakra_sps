using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    public class AccessRequestGridData
    {
        public long AccessRequestId { get; set; }
        public string TenantId { get; set; }
        public long RequesterId { get; set; }
        public string Requester { get; set; }
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProvisioningRoleId { get; set; }
        public string RoleName { get; set; }
        public string RequestStatus { get; set; } // AccessRequestStatus
        public string ProgressStatus { get; set; } // AccessRequestProgress
        public string RequestSource { get; set; } // AccessRequestSource
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public Nullable<System.DateTime> ExecutionTime { get; set; }
        public string Remarks { get; set; }
        public string ApproverType { get; set; }
        public string IsRequestedToRemove { get; set; }
        public string IsNewUser { get; set; }
        public Nullable<int> NewUserId { get; set; }
        public string NewUserName { get; set; }
        //public string CreatedBy { get; set; }
        //public System.DateTime CreatedDate { get; set; }
        //public string LastModifiedBy { get; set; }
        //public Nullable<System.DateTime> LastModifiedDate { get; set; }

        //public Guid TransactionId { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public string Comments { get; set; }

        //public string RoleName
        //{
        //    get
        //    {
        //        if (this.ProvisioningRoleId.HasValue)
        //        {
        //            return this.RoleName;
        //        }
        //        return string.Empty;
        //    }
        //}
    }
}
