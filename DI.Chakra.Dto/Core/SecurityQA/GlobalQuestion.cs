using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    public class GlobalQuestion : DtoBase
    {
         public string Question { get; set; }
         public string CreatedBy { get; set; }
         public DateTime CreatedDate { get; set; }
         public string LastModifiedBy { get; set; }
         public DateTime LastModifiedDate { get; set; }
    }
}
