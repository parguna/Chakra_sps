using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.ISMS
{
    public static class Message
    {
        public const string ISMS_100 = "ISMS Connection suceeded";
        public const string ISMS_101 = "ISMS Connection failed";
        public const string ISMS_102 = "ISMS GetData Failed.";
        public const string ISMS_103 = "User Name";
        public const string ISMS_104 = "CSEL#";
        public const string ISMS_105 = "CSELDesc";
        public const string ISMS_106 = "StartDate";
        public const string ISMS_107 = "ExpiryDate";
        public const string ISMS_108 = "GetUsersFailed";
        public const string ISMS_109 = "NULL";
        public const string ISMS_110 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string ISMS_111 = "Badge Type";
        public const string ISMS_112 = "ID Type";
        public const string ISMS_113 = "ID No";
        public const string ISMS_114 = "DOB";
        public const string MANDATORY_COLS_LIST = "ID No,ID Type,Badge Type,HH";
    }
}
