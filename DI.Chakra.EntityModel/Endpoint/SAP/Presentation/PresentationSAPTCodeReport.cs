using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationSAPTCodeReport
    {
        public long TCODE_Id { get; set; }

        public string TCODE_Code { get; set; }

        public string TCODE_Desc { get; set; }

        public string Institution { get; set; }

        public string HostName { get; set; }
    }
}
