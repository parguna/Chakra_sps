using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model.Custom.GridData
{
    [Serializable]
    public class ACLPagePermissionGridData
    {
        public long Id { get; set; }
        public long PermissionId { get; set; }
        public string PermissionName { get; set; }
        public long? UrlId { get; set; }
        public string PageUrl { get; set; }

    }
    public class MenuGridData
    {
        public long Id { get; set; }
        public string UniqueName { get; set; }
        public string MenuName{ get; set; }
        public string ParentMenuName { get; set; }
        public string PageName { get; set; }
    }
}
