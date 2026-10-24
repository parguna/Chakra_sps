using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DI.Chakra.Model
{
    [Serializable] 
    [Table("DI_M_Attestation")]
    public class Attestation : IAggregateRoot
    {

        public Attestation()
        {
            this.AttestationProcesses = new List< AttestationProcess>();
            this.AttestationEndpoint = new List<AttestationEndpoint>();
        }
    
        [Key]
        public long Id { get; set; }
        public string AttestationName { get; set; }
        public string AttestationDescription { get; set; }

        public string TenantId { get; set; }
        public string AttestationType { get; set; }
        public string AttestationScope { get; set; }
        public string Department { get; set; }
        public string BusinessRole { get; set; }
        public string Manager { get; set; }
        public string Group { get; set; }
        public string Company { get; set; }
        public string Section { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public Nullable<bool> AllUser { get; set; }
        public Nullable<int> UserName { get; set; }
        public long WorkflowConfigurationId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        [ForeignKey("WorkflowConfigurationId")]
        public virtual WorkflowConfiguration WorkflowConfiguration { get; set; }
        public virtual List<AttestationProcess> AttestationProcesses { get; set; }
        public virtual List<AttestationEndpoint> AttestationEndpoint { get; set; }
       
        public override string ToString()
        {
            return this.AttestationName;
        }
       
    }
}
