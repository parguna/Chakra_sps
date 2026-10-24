using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_Catalog")]
    public class LNSCatalog : IApprover, IXLData
    {
        [Key]
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string Application { get; set; }
        public string BusinessRole { get; set; }
        public string Description { get; set; }
        public string TechnicalRole { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }

        public string SystemName
        {
            get
            {
                return "LNS";
            }
        }


        public DateTime? ValidFrom
        {
            get;
            set;

        }

        public DateTime? ValidTo
        {
            get;
            set;

        }
    }
}
