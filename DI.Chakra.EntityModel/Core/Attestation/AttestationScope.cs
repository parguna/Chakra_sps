using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_AttestationScope")]
    public class AttestationScope : EntityBase, ICreatable, IUpdatable
    {
        public long AttestationId { get; set; }

        public Nullable<long> OrganisationScopeId { get; set; }

        public string AttestationScopeName { get; set; }

        public Nullable<long> AttestationScopeValue { get; set; } 

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [ForeignKey("AttestationId")]
        public Attestation Attestation { get; set; }

        [ForeignKey("OrganisationScopeId")]
        public NOrganisation NOrganisation { get; set; }  
    }
}
