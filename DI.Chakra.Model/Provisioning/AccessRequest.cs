using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]

    [Table("DI_M_AccessRequest")]
    public class AccessRequest 
    {

        public AccessRequest()
        {
            this.CreatedDate = DateTime.Now;
            this.LastModifiedDate = DateTime.Now;
        }

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long RequesterId { get; set; }
        public long UserId { get; set; }
        public string Description { get; set; }
        public Nullable<long> ProvisioningFormId { get; set; }
        public Nullable<long> AdHocAccessRequestId { get; set; }
        public Nullable<long> EndpointSystemId { get; set; }
        public Nullable<long> EmployeeEventTriggerId { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public AccessRequestSource RequestSource { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public Nullable<System.DateTime> ExecutionTime { get; set; }
       // public Nullable<bool> IsRequestedToRemove { get; set; }
        public string Remarks { get; set; }
        public string ApproverType { get; set; }

        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public Guid TransactionId { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public string Comments { get; set; }

        public string AdditionalInfo { get; set; }
       
        public long? ParentRequestId { get; set; }
        public bool IsParentRequest { get; set; }
        public bool IsDependentRequest{ get; set; }
        public bool IsDependentRequestCreated { get; set; }

        public Nullable<long> AttestationReviewId { get; set; }

        [XmlIgnore]
        public virtual ProvisioningForm ProvisioningForm { get; set; }
        [ForeignKey("AttestationReviewId")]
        public virtual AttestationReview AttestationReview { get; set; }
        [ForeignKey("AdHocAccessRequestId")]
        public virtual AdHocAccessRequest AdHocAccessRequest { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string RequestSourceString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((AccessRequestSource)this.RequestSource);
            }
        }

        public string ProgressStatusString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((AccessRequestProgress)this.ProgressStatus);
            }
        }
        
    }
}