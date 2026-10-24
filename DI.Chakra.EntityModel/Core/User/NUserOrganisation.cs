using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_NUserOrganisation")]
    public class NUserOrganisation : EntityBase
    {
        public long UserId { get; set; }

        public long OrganisationId { get; set; }

        public long TypeId { get; set; }

        public string Category { get; set; }

        public Nullable<System.DateTime> ValidFrom { get; set; }

        public Nullable<System.DateTime> ValidTo { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("OrganisationId")]
        public NOrganisation Organisation { get; set; }        

        [ForeignKey("TypeId")]
        public NScopeFor ScopeFor { get; set; }
    }
}
