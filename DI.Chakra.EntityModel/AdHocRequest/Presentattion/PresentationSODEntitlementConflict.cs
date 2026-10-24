using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
    [NotMapped]
    public class PresentationSODEntitlementConflict : EntityBase
    {
        public string Role1 { get; set; }

        public string Role2 { get; set; }

        public string UserName { get; set; }


    }
}
