using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_Organisation")]
    public class Organisation : EntityBase,ICreatable,IUpdatable
    {
        public string OrgUnitPrefix { get; set; }

        public string OrganisationName { get; set; }

        public string ParentOrganisation { get; set; }

        public string Scope { get; set; }

        public long? Head { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
