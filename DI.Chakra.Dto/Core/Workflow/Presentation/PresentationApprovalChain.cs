using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationApprovalChain
    {
        public PresentationApprovalChain()
        {
            this.ApproverDetails = new List<ApproverDetail>();
        }

        public byte ApprovalState { get; set; }
        public string ApproverName { get; set; }
        public string ApproverType { get; set; }
        public string Type
        {
            get
            {
                return !IsActionOnlyRequired ? "Approver" : "Processing Team";
            }
        }
        public StatusType Action { get; set; }

        public List<ApproverDetail> ApproverDetails { get; set; }

        public bool IsActionOnlyRequired { get; set; }
        public string Status
        {
            get
            {
                return IsActionOnlyRequired && Action == StatusType.Approved ? "Processed" : UtilityMethods.GetEnumAttribute<Description>(this.Action);
            }
        }
        public bool IsGroup
        {
            get
            {
                return ApproverType == null ? false : (ApproverType.Split('-')[0].Trim() == "Group");
            }
        }
        public string ForeColor
        {
            get
            {
                return (IsGroupApproval || IsGroup) ? "" : "#474545";
            }
        }
        public bool IsGroupApproval { get; set; }
        public override string ToString()
        {
            if (IsGroupApproval)
            {
                return $"Group - {ApproverName}";
            }
            return ApproverName;
        }
    }

    [Serializable]
    public class ApproverDetail
    {
        public byte ApprovalState { get; set; }
        public string TaskOwner { get; set; }
        public StatusType Action { get; set; }
        public DateTime? ActionDate { get; set; }
        public string Comments { get; set; }
        public string DelegatedFrom { get; set; }
        public string DelegatedBy { get; set; }
        public string DelegationComments { get; set; }
        public string Status
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(this.Action);
            }
        }
        //public bool IsGroup { get; set; }
        public string Remarks { get; set; }
    }
}
