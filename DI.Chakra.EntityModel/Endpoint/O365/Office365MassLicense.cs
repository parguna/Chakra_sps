 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{ 
    [Table("DI_M_Office365MassLicense")]
    public class Office365MassLicense : EntityBase, ICreatable , IUpdatable
    {
        public long EndPointSystemId { get; set; }
        public bool IsMassAdd { get; set; }
        public bool IsCompleted { get; set; }
        public string Licenses { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        [ForeignKey("EndPointSystemId")]
        public EndPoint EndPoint { get; set; }
    }
    
}
