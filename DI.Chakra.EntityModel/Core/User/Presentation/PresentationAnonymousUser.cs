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
    public class PresentationAnonymousUser 
    {
        public string UserName { get; set; }

        public string Password { get; set; }
        public Nullable<bool> FullTimeEmployee { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public string AuthenticationMode { get; set; }

        public EmployeeType UserEmployeeType { get; set; }
    }
}
