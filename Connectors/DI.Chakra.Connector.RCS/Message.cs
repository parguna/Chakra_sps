using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.RCS
{
    class Message
    {

        public const string RCS_101 = "ResortsCardSystem Connector Connection succeeded.";
        public const string RCS_102 = "ResortsCardSystem Connector Connection failed.";
        public const string RCS_103 = "File Connector Connection failed.";

        public const string RCS_105 = "Started Discovering ResortsCardSystem Users";
        public const string RCS_106 = "Completed Discovering ResortsCardSystem Users";
        public const string RCS_107 = "Started Discovering ResortsCardSystem Functions";
        public const string RCS_108 = "Completed Discovering ResortsCardSystem Functions";

        public const string RCS_109 = "Error occured while processing the DiscoverFunctions Method.";
        public const string RCS_110 = "Started Discovering ResortsCardSystem Roles";
        public const string RCS_111 = "Completed Discovering ResortsCardSystem Roles";

        public const string RCS_112 = "Error occured while processing the DiscoverRoles Method.";
        public const string RCS_113 = "Started Discovering ResortsCardSystem PrivillegeAcctTrans";
        public const string RCS_114 = "Completed Discovering ResortsCardSystem PrivillegeAcctTrans";

        public const string RCS_115 = "Error occured while processing the DiscoverPrivillegeAcctTrans Method.";
        public const string RCS_ERROR_USERS = "Error occured while processing the Discover Users Method.";
        public const string RCS_104 = "AD ID";
        public const string RCS_116 = "RoleCode";
        public const string RCS_117 = "LastLoginDate";
        public const string RCS_118 = "Delete from RCSUserInfoRCSRoleInfo where RCSUserInfo_Id={0}";
        public const string RCS_119 = "INSERT INTO RCSUserInfoRCSRoleInfo(RCSUserInfo_Id,RCSRoleInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string RCS_120 = "GetUsers Failed:";
        public const string RCS_121 = "FunctionName";
        public const string RCS_122 = "System";
        public const string RCS_123 = "FunctionDesc";
        public const string RCS_124 = "GetFunctions Failed :";
        public const string RCS_125 = "FunctionGrp";
        public const string RCS_126 = "RoleDesc";
        public const string RCS_127 = "FunctionCode";
        public const string RCS_128 = "Delete from RCSRoleInfoRCSFunctionInfo where RCSRoleInfo_Id={0}  AND  RCSFunctionInfo_Id={1}";
        public const string RCS_129 = "INSERT INTO RCSRoleInfoRCSFunctionInfo(RCSRoleInfo_Id,RCSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";
        public const string RCS_130 = "GetRoles Failed : ";
        public const string RCS_131 = "IDASTnx_ID";
        public const string RCS_132 = "DateTime";
        public const string RCS_133 = "PerformedBy";
        public const string RCS_134 = "AppliedOn";
        public const string RCS_135 = "ActionTaken";
        public const string RCS_136 = "Activity";
        public const string RCS_137 = "GetPrivillegeAcctTrans Failed :";
        public const string RCS_138 = "NULL";
        public const string RCS_139 = "Not able get the Field From CSV File, Field Name {0}.";
        public const string RCS_140 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID> 102 </SystemID></InterfaceRequestHeaderDT><createUserVALParamDT><EmployeeID>{0}<EmployeeID> <AD-ID>{1}</AD-ID><FirstName>{2}</FirstName ><LastName>{3}</LastName><Email>{4}</Email ><IDAS_Tnx_ID>{5}</IDAS_Tnx_ID></createUserVALParamDT></UserIntegrationDS>";
        public const string RCS_141 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><updateRoleVALParamDT><AD-ID>{0}</AD-ID><Roles actiontype=Add'><RoleCode>{1}</RoleCode></Roles><Roles actiontype=Remove><RoleCode>{2}</RoleCode></Roles></updateRoleVALParamDT><KioskCampaignIntegrationDS>";
        public const string RCS_142 = "<UserIntegrationDS><InterfaceRequestHeaderDT><SystemID>102</SystemID></InterfaceRequestHeaderDT><deleteUserVALParamDT><AD-ID>{0}</AD-ID><IDAS_Tnx_ID>{1}</IDAS_Tnx_ID><deleteUserVALParamDT></UserIntegrationDS>";

        public const string RCS_143 = "View";
        public const string RCS_144 = "Create";
        public const string RCS_145 = "Modify";
        public const string RCS_146 = "Delete";
        public const string RCS_147 = "Print";
        public const string RCS_148 = "Export";
        public const string RCS_149 = "Create";
        public const string RCS_150 = "RoleDesc";
        public const string RCS_151 = "FunctionalGroup";
        public const string RCS_152 = "RoleCode";
        public const string RCS_153 = "FNCode";
        public const string RCS_154 = "FNStatus";
        public const string RCS_155 = "FunctionDesc";
        public const string RCS_156 = "{0}-{1}";
        public const string RCS_ROLES_MANDATORY_COLS = "0,1,2";
        public const string RCS_USERS_MANDATORY_COLS = "0";
        public const string RCS_PRIV_MANDATORY_COLS = "0,1,2";
        public const string RCS_FUN_MANDATORY_COLS = "0,1,2,3,4,5,6,7,8,9";

        public const string RCS_DELETE_FUNCROLE_QUERY = "DELETE FROM RCSRoleInfoRCSFunctionInfo WHERE EndpointSystemId = {0}";

        public const string RCS_INSERT_FUNCROLE_QUERY = "INSERT INTO RCSRoleInfoRCSFunctionInfo (RCSRoleInfo_Id,RCSFunctionInfo_Id,EndpointSystemId,Version) VALUES({0},{1},{2},{3})";

    }
}
