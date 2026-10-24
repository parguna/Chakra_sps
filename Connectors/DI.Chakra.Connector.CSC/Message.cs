using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.CSC
{
    class Message
    {

        public const string CSC_101 = "EntryControlSystem Connector Connection succeeded.";
        public const string CSC_102 = "EntryControlSystem Connector Connection failed.";
        public const string CSC_103 = "File Connector Connection failed.";

        public const string CSC_105 = "Started Discovering EntryControlSystem Users";
        public const string CSC_106 = "Completed Discovering EntryControlSystem Users";
        public const string CSC_107 = "Started Discovering EntryControlSystem Functions";
        public const string CSC_108 = "Completed Discovering EntryControlSystem Functions";

        public const string CSC_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string CSC_110 = "Started Discovering EntryControlSystem Roles";
        public const string CSC_111 = "Completed Discovering EntryControlSystem Roles";

        public const string CSC_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string CSC_113 = "Started Discovering EntryControlSystem PrivillegeAcctTrans";
        public const string CSC_114 = "Completed Discovering EntryControlSystem PrivillegeAcctTrans";

        public const string CSC_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string CSC_ERROR_CSC_USERS = "Error occured while processing the Discover Users Method.";
        public const string CSC_104 = "AD ID";
        public const string CSC_116 = "RoleCode";
        public const string CSC_117 = "LastLoginDate";
        public const string CSC_118 = "Delete from CSCUserInfoCSCRoleInfo where CSCUserInfo_Id={0}";
        public const string CSC_119 = "INSERT INTO CSCUserInfoCSCRoleInfo(CSCUserInfo_Id,CSCRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string CSC_120 = "GetUsers Failed:";
        public const string CSC_121 = "FunctionName";
        public const string CSC_122 = "System";
        public const string CSC_123 = "FunctionDesc";
        public const string CSC_124 = "GetFunctions Failed :";
        public const string CSC_125 = "FunctionGrp";
        public const string CSC_126 = "RoleDesc";
        public const string CSC_127 = "FunctionCode";
        public const string CSC_128 = "Delete from CSCRoleInfoCSCFunctionInfo where CSCRoleInfo_Id={0}  AND  CSCFunctionInfo_Id={1}";
        public const string CSC_129 = "INSERT INTO CSCRoleInfoCSCFunctionInfo(CSCRoleInfo_Id,CSCFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string CSC_130 = "GetRoles Failed : ";
        public const string CSC_131 = "IDASTnx_ID";
        public const string CSC_132 = "DateTime";
        public const string CSC_133 = "PerformedBy";
        public const string CSC_134 = "AppliedOn";
        public const string CSC_135 = "ActionTaken";
        public const string CSC_136 = "Activity";
        public const string CSC_137 = "GetPrivillegeAcctTrans Failed :";
        public const string CSC_138 = "NULL";
        public const string CSC_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string CSC_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string CSC_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string CSC_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string CSC_143 = "View";
        public const string CSC_144 = "Create";
        public const string CSC_145 = "Modify";
        public const string CSC_146 = "Delete";
        public const string CSC_147 = "Print";
        public const string CSC_148 = "Export";
        public const string CSC_149 = "Create";
        public const string CSC_150 = "RoleDesc";
        public const string CSC_151 = "FunctionalGroup";
        public const string CSC_152 = "RoleCode";
        public const string CSC_153 = "FNCode";
        public const string CSC_154 = "FNStatus";
        public const string CSC_155 = "FunctionDesc";
        public const string CSC_156 = "{0}-{1}";
        public const string CSC_ROLES_MANDATORY_COLS = "0,1,2";
        public const string CSC_USERS_MANDATORY_COLS = "0";
        public const string CSC_PRIV_MANDATORY_COLS = "0,1,2";
        public const string CSC_FUN_MANDATORY_COLS = "0,1,2,3,4,5,6,7,8,9";

        public const string CSC_DELETE_FUNCROLE_QUERY = "DELETE FROM CSCRoleInfoCSCFunctionInfo WHERE EndpointSystemId = {0}";

        public const string CSC_INSERT_FUNCROLE_QUERY = "INSERT INTO CSCRoleInfoCSCFunctionInfo (CSCRoleInfo_Id,CSCFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
