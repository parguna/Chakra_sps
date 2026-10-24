using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel.Presentation
{
     public class PresentationSAPHANAUsersToRolesReport
    {
         public long UserId { get; set; }

         public string UserName { get; set; }

         public string DisplayName { get; set; }

         public string Department { get; set; }

         public bool? IsActive { get; set; }

         public string RoleCode { get; set; }

         public string RoleDescription { get; set; }

         public string Role_FromDate { get; set; }

         public string Role_ToDate { get; set; }
        public long RoleId { get; set; }
     }
}
