using System;


namespace DI.Chakra.Dto
{
    public class RoleTypes : DtoBase
    {
        public string DisplayName { get; set; }
        public string RoleTypeDesc { get; set; }

        public string ApplicableApprovalUserTypes { get; set; }

        public string ApplicableRequestTypes { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
