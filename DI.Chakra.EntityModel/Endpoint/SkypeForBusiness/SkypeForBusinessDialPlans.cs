using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_DialPlans")]
    public class SkypeForBusinessDialPlans : EntityBase, ICreatable, IUpdatable
    { 
        public long EndPointSystemId { get; set; }
        public string Identity { get; set; }
        public string Description { get; set; }
        public string DialinConferencingRegion { get; set; }
        public string NormalizationRules { get; set; }
        public string CountryCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ExternalAccessPrefix { get; set; }
        public string SimpleName { get; set; }
        public bool OptimizeDeviceDialing { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
