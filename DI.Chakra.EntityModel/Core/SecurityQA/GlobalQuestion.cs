using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_GlobalQuestion")]
    public class GlobalQuestion : EntityBase,ICreatable,IUpdatable
    {
         public string Question { get; set; }
         public string CreatedBy { get; set; }
         public DateTime CreatedDate { get; set; }
         public string LastModifiedBy { get; set; }
         public DateTime? LastModifiedDate { get; set; }
    }
}
