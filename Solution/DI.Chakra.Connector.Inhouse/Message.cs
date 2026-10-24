using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.Inhouse
{
    class Message
    {
        public const string Inhouse_101 = "Inhouse Connector Connection succeeded.";
        public const string Inhouse_102 = "Inhouse Connector Connection failed.";
        public const string Inhouse_103 = "File Connector Connection failed.";

        public const string Inhouse_105 = "Started Discovering Inhouse Users";
        public const string Inhouse_106 = "Completed Discovering Inhouse Users";
        public const string Inhouse_107 = "Started Discovering Inhouse Functions";
        public const string Inhouse_108 = "Completed Discovering Inhouse Functions";

        public const string Inhouse_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string Inhouse_110 = "Started Discovering Inhouse Roles";
        public const string Inhouse_111 = "Completed Discovering Inhouse Roles";

        public const string Inhouse_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string Inhouse_113 = "Started Discovering Inhouse PrivillegeAcctTrans";
        public const string Inhouse_114 = "Completed Discovering Inhouse PrivillegeAcctTrans";

        public const string Inhouse_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string BIZCMS_ERROR_CMS_USERS = "Error occured while processing the Discover Users Method.";
        public const string Inhouse_104 = "AD ID";
        public const string Inhouse_116 = "RoleCode";
        public const string Inhouse_117 = "LastLoginDate";
        public const string Inhouse_118 = "Delete from InhouseUserInfoInhouseRoleInfo where InhouseUserInfo_Id={0}";
        public const string Inhouse_119 = "INSERT INTO InhouseUserInfoInhouseRoleInfo(InhouseUserInfo_Id,InhouseRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string Inhouse_120 = "GetUsers Failed:";
        public const string Inhouse_121 = "FunctionName";
        public const string Inhouse_122 = "System";
        public const string Inhouse_123 = "FunctionDesc";
        public const string Inhouse_124 = "GetFunctions Failed :";
        public const string Inhouse_125 = "FunctionGrp";
        public const string Inhouse_126 = "RoleDesc";
        public const string Inhouse_127 = "FunctionCode";
        public const string Inhouse_128 = "Delete from InhouseRoleInfoInhouseFunctionInfo where InhouseRoleInfo_Id={0}  AND  InhouseFunctionInfo_Id={1}";
        public const string Inhouse_129 = "INSERT INTO InhouseRoleInfoInhouseFunctionInfo(InhouseRoleInfo_Id,InhouseFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string Inhouse_130 = "GetRoles Failed : ";
        public const string Inhouse_131 = "IDASTnx_ID";
        public const string Inhouse_132 = "DateTime";
        public const string Inhouse_133 = "PerformedBy";
        public const string Inhouse_134 = "AppliedOn";
        public const string Inhouse_135 = "ActionTaken";
        public const string Inhouse_136 = "Activity";
        public const string Inhouse_137 = "GetPrivillegeAcctTrans Failed :";
        public const string Inhouse_138 = "NULL";
        public const string Inhouse_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string Inhouse_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD - ID>{1}</AD - ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string Inhouse_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string Inhouse_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string Inhouse_143 = "View";
        public const string Inhouse_144 = "Create";
        public const string Inhouse_145 = "Modify";
        public const string Inhouse_146 = "Delete";
        public const string Inhouse_147 = "Print";
        public const string Inhouse_148 = "Export";
        public const string Inhouse_149 = "Create";
        public const string Inhouse_150 = "RoleDesc";
        public const string Inhouse_151 = "FunctionalGroup";
        public const string Inhouse_152 = "RoleCode";
        public const string Inhouse_153 = "FNCode";
        public const string Inhouse_154 = "FNStatus";
        public const string Inhouse_155 = "FunctionDesc";
        public const string Inhouse_156 = "{0}-{1}";
        public const string Inhouse_ROLES_MANDATORY_COLS = "InhouseSystemCode";
        public const string Inhouse_USERS_MANDATORY_COLS = "InhouseSystemCode";
        public const string Inhouse_PRIV_MANDATORY_COLS = "InhouseSystemCode";
        public const string Inhouse_FUN_MANDATORY_COLS = "InhouseSystemCode";

        public const string Inhouse_DELETE_FUNCROLE_QUERY = "DELETE FROM InhouseRoleInfoInhouseFunctionInfo WHERE EndpointSystemId = {0}";

        public const string Inhouse_INSERT_FUNCROLE_QUERY = "INSERT INTO InhouseRoleInfoInhouseFunctionInfo (InhouseRoleInfo_Id,InhouseFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
