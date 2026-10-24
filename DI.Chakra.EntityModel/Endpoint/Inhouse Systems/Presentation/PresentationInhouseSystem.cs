using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationInhouseSystem
    {
        public long? DIUserID { get; set; }
        public string UserName { get; set; }
        public string SystemCode { get; set; }
        public string SystemName { get;  set; }
        public string Permission { get;  set; }
      
    }
}
