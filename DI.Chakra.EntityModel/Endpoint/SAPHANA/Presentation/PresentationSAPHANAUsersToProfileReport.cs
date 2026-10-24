using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{

    [NotMapped]
    public class PresentationSAPHANAUsersToProfileReport
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public string Department { get; set; }

        public string IsActive { get; set; }

        public string ProfileCode { get; set; }

        public string ProfileDescription { get; set; }
    }
}
