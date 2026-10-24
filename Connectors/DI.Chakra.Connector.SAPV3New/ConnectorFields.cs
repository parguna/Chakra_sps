using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SAPV3
{
    public class ConnectorFields
    {
        public static List<string> USOBT_C_FIELDS
        {
            get
            {
                return new List<string>() { "NAME", "OBJECT", "FIELD", "LOW" };
            }
        }

        public static List<string> POSITION_FIELDS
        {
            get
            {
                return new List<string>() { "SOBID", "OBJID", "BEGDA", "ENDDA" };
            }
        }

        public static List<string> SAP_PHONE_FIELDS
        {
            get
            {
                return new List<string>() { "OBJID" };
            }
        }

        public static List<string> SECTION_FIELDS
        {
            get
            {
                return new List<string>() { "OBJID", "VBUKR", "ZZSECNAME","DDTEXT" };
            }
        }
    }
}
