using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class NOrganisationHead : DtoBase
    {
        public long OrganisationId { get; set; }

        public long? HeadId { get; set; }

        public long? CoordinatorId { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public User Head { get; set; }

        public User Coordinator { get; set; }


    }
}
