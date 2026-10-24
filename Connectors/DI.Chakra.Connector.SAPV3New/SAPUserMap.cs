using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace DI.Chakra.Connector.SAPV3New
{
    public class ExistingRoleInfo
    {
        public string Role_Code;
        public string ValidFrom;
        public string ValidTo;
        public bool UsedInMultipleRoles;
        public string Org_Flag;
    }

    public class ExistingPositionUserInfo
    {
        public string ValidFrom;
        public string ValidTo;
        public string UserName;
    }

}
