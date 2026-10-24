using DI.Chakra.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SFB_VoiceRoutingPolicy")]
    public class SkypeForBusinessVoiceRoutingPolicy : EntityBase, ICreatable, IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public string Identity { get; set; }
        public string PstnUsages { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
}
