using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class PresentationAccessRights : DtoBase
    {
        public string RoleName { get; set; }
        public string MenuPath { get; set; }
        public string Permissions { get; set; }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            switch (propertyName)
            {
                case "rolename": return RoleName.ToString() ?? string.Empty;
                case "menupath": return MenuPath ?? string.Empty;
                case "permissions": return Permissions ?? string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
