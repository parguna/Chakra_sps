using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class NOrganisationScope : DtoBase
    {
        public string Scope { get; set; }

        public int Level { get; set; }

        public bool IsValidForApproval { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

    }
}
