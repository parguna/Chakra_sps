using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.LCS
{
    class Message
    {

        public const string LCS_101 = "EntryControlSystem Connector Connection succeeded.";
        public const string LCS_102 = "EntryControlSystem Connector Connection failed.";
        public const string LCS_103 = "File Connector Connection failed.";

        public const string LCS_105 = "Started Discovering EntryControlSystem Users";
        public const string LCS_106 = "Completed Discovering EntryControlSystem Users";
        public const string LCS_107 = "Started Discovering EntryControlSystem Functions";
        public const string LCS_108 = "Completed Discovering EntryControlSystem Functions";

        public const string LCS_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string LCS_110 = "Started Discovering EntryControlSystem Roles";
        public const string LCS_111 = "Completed Discovering EntryControlSystem Roles";

        public const string LCS_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string LCS_113 = "Started Discovering EntryControlSystem PrivillegeAcctTrans";
        public const string LCS_114 = "Completed Discovering EntryControlSystem PrivillegeAcctTrans";

        public const string LCS_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string LCS_ERROR_LCS_USERS = "Error occured while processing the Discover Users Method.";
        public const string LCS_104 = "AD ID";
        public const string LCS_116 = "RoleCode";
        public const string LCS_117 = "LastLoginDate";
        public const string LCS_118 = "Delete from LCSUserInfoLCSRoleInfo where LCSUserInfo_Id={0}";
        public const string LCS_119 = "INSERT INTO LCSUserInfoLCSRoleInfo(LCSUserInfo_Id,LCSRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string LCS_120 = "GetUsers Failed:";
        public const string LCS_121 = "FunctionName";
        public const string LCS_122 = "System";
        public const string LCS_123 = "FunctionDesc";
        public const string LCS_124 = "GetFunctions Failed :";
        public const string LCS_125 = "FunctionGrp";
        public const string LCS_126 = "RoleDesc";
        public const string LCS_127 = "FunctionCode";
        public const string LCS_128 = "Delete from LCSRoleInfoLCSFunctionInfo where LCSRoleInfo_Id={0}  AND  LCSFunctionInfo_Id={1}";
        public const string LCS_129 = "INSERT INTO LCSRoleInfoLCSFunctionInfo(LCSRoleInfo_Id,LCSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string LCS_130 = "GetRoles Failed : ";
        public const string LCS_131 = "IDASTnx_ID";
        public const string LCS_132 = "DateTime";
        public const string LCS_133 = "PerformedBy";
        public const string LCS_134 = "AppliedOn";
        public const string LCS_135 = "ActionTaken";
        public const string LCS_136 = "Activity";
        public const string LCS_137 = "GetPrivillegeAcctTrans Failed :";
        public const string LCS_138 = "NULL";
        public const string LCS_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string LCS_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string LCS_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string LCS_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string LCS_143 = "View";
        public const string LCS_144 = "Create";
        public const string LCS_145 = "Modify";
        public const string LCS_146 = "Delete";
        public const string LCS_147 = "Print";
        public const string LCS_148 = "Export";
        public const string LCS_149 = "Create";
        public const string LCS_150 = "RoleDesc";
        public const string LCS_151 = "FunctionalGroup";
        public const string LCS_152 = "RoleCode";
        public const string LCS_153 = "FNCode";
        public const string LCS_154 = "FNStatus";
        public const string LCS_155 = "FunctionDesc";
        public const string LCS_156 = "{0}-{1}";
        public const string LCS_ROLES_MANDATORY_COLS = "0,1,2";
        public const string LCS_USERS_MANDATORY_COLS = "0";
        public const string LCS_PRIV_MANDATORY_COLS = "0,1,2";
        public const string LCS_FUN_MANDATORY_COLS = "0,1,2,3,4,5,6,7,8,9";

        public const string LCS_DELETE_FUNCROLE_QUERY = "DELETE FROM LCSRoleInfoLCSFunctionInfo WHERE EndpointSystemId = {0}";

        public const string LCS_INSERT_FUNCROLE_QUERY = "INSERT INTO LCSRoleInfoLCSFunctionInfo (LCSRoleInfo_Id,LCSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
