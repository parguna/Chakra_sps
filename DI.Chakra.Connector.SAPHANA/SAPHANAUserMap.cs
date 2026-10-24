using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAExistingRoleInfo
    {
        public string Role_Code;
        public string ValidFrom;
        public string ValidTo;
        public bool UsedInMultipleRoles;
        public string Org_Flag;
    }

    public class SAPHANAExistingPositionUserInfo
    {
        public string ValidFrom;
        public string ValidTo;
        public string UserName;
        public string SUBTY;
    }

}
