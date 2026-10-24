using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NOrganisationHead")]
    public class NOrganisationHead : EntityBase , ICreatable, IUpdatable
    {
        public long OrganisationId { get; set; }

        public long? HeadId { get; set; }

        public long? CoordinatorId { get; set; }
        
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("OrganisationId")]
        public NOrganisation Organisation { get; set; }

        [ForeignKey("HeadId")]
        public User Head { get; set; }

        //[ForeignKey("CoordinatorId")]
        //public User Coordinator { get; set; }
    }
}
