using System;

namespace DI.Chakra.Dto
{
    public class PersonnelSubArea : DtoBase
    {
        public long PersonnelAreaId { get; set; }

        public string Code { get; set; }

        public string Text { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
