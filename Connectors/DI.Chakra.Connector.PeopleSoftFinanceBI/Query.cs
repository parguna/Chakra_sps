using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.PeopleSoftFinanceBI
{
    public static class Query
    {
        public const string UserSQL = @"select OPRID, OPRDEFNDESC, EMPLID,EMAILID, OPRCLASS, ROWSECCLASS, ENCRYPTED, SYMBOLICID,
                                        LANGUAGE_CD, MULTILANG, CURRENCY_CD, LASTPSWDCHANGE, ACCTLOCK, PRCSPRFLCLS, DEFAULTNAVHP,
                                        FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTSIGNONDTTM, LASTUPDDTTM, LASTUPDOPRID,
                                        PTALLOWSWITCHUSER from PSOPRDEFN ";
        public const string RolesSQL = @"SELECT ROLENAME, VERSION, ROLETYPE, DESCR, QRYNAME, ROLESTATUS, RECNAME, FIELDNAME, 
                            PC_EVENT_TYPE, QRYNAME_SEC, PC_FUNCTION_NAME, ROLE_PCODE_RULE_ON, ROLE_QUERY_RULE_ON, LDAP_RULE_ON,
                            DESCRLONG, ALLOWNOTIFY, ALLOWLOOKUP, LASTUPDDTTM, LASTUPDOPRID from PSROLEDEFN";
        public const string PermissionQuery = @"SELECT CLASSID, VERSION, CLASSDEFNDESC, TIMEOUTMINUTES, DEFAULTBPM, STARTAPPSERVER,
                                        ALLOWPSWDEMAIL, LASTUPDDTTM, LASTUPDOPRID from PSCLASSDEFN";
        public const string AccesslogQuery = @"select * from PSaccesslog where (OPRID, LOGINDTTM) in (select OPRID, MAX(LOGINDTTM) from PSaccesslog group by OPRID)";
        public const string RoleUserQuery = @"SELECT  ROLENAME, ROLEUSER, DYNAMIC_SW
                                         from PSRoleUser";
        public const string RoleClassquery = @"SELECT * FROM PSRoleclass";
        public const string Menuquery = @"SELECT MENUNAME, BARNAME, ITEMNAME, ITEMNUM, ITEMTYPE, PNLGRPNAME,
                                        MARKET, BARLABEL, ITEMLABEL,SEARCHRECNAME from PSMENUITEM";
        public const string PortalQuery = @"SELECT PORTAL_NAME, PORTAL_REFTYPE, PORTAL_OBJNAME, PORTAL_PERMNAME, PORTAL_PERMTYPE, PORTAL_ISCASCADE
                                        from PSPRSMPERM";
        public const string RolePermissionQuery = "insert into PeopleSoftFinanceBIRoleInfoPeopleSoftFinanceBIPermissionInfo([PeopleSoftFinanceBIRoleInfo_Id],[PeopleSoftFinanceBIPermissionInfo_Id],[EndpointSystemId],[Version]) values ({0},{1},{2},{3})";
        public const string Deletequery = "Delete from PeopleSoftFinanceBIRoleInfoPeopleSoftFinanceBIUserInfo where EndpointSystemId={0}";
        public const string RoleUserMapQuery = "insert into PeopleSoftFinanceBIRoleInfoPeopleSoftFinanceBIUserInfo(PeopleSoftFinanceBIUserInfo_Id,PeopleSoftFinanceBIRoleInfo_Id,EndpointSystemId,Version) values ({0},{1},{2},{3})";
        public const string CreateLoginQuery = "update PSOPRDEFN set acctlock=1 where  OPRID= '?OPRID'";
        public const string CreateLoginquery = "update PSOPRDEFN set acctlock=0 where  OPRID= ?OPRID";
        public const string CreatingQuery = @"delete PSRoleUser where  ROLEUSER='?AccountName' and ROLENAME='?permission'";
        public const string Creatingquery = @"Insert into PSRoleUser VALUES('?AccountName'  , '?permission' ,'N')";
        public const string UpdateQuery = "update PSOPRDEFN set acctlock = 0 where OPRID = ?ORPID";
        public const string Updatequery = @"UPDATE PSOPRDEFN  SET OPERPSWD =?Password, OPERPSWDSALT =?DIUserId, ENCRYPTED = 1 Where OPRID = ?USERNAME";
        //public const string CreateUserQuery = @"insert into PSOPRDEFN (OPRID, VERSION, OPRDEFNDESC, EMPLID, EMAILID, OPRCLASS, ROWSECCLASS,
        //            OPERPSWD, OPERPSWDSALT, ENCRYPTED , SYMBOLICID, LANGUAGE_CD, MULTILANG , CURRENCY_CD , LASTPSWDCHANGE, ACCTLOCK , 
        //            PRCSPRFLCLS, DEFAULTNAVHP, FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTUPDOPRID, PTALLOWSWITCHUSER)
        //                values ('?OPRID', 8 , '?OPRDEFNDESC', '?EMPLID', '?EMAILID', 'HCPPALL','HCDPALL','?DATETIME','?ACCTLOCK',1 ,'SYSADM1' ,'ENG'  ,
        //            0 ,'USD' ,'?ACCOUNTPASSWORD',0 ,'EPEX9000','HCSPNAVHP',0,0,0,'?USERIDALIAS','PPLSOFT',0)";
        public const string CreateUserQuery = @"insert into PSOPRDEFN (OPRID, VERSION, OPRDEFNDESC, EMPLID, EMAILID, OPRCLASS, ROWSECCLASS,
                    OPERPSWD, OPERPSWDSALT, ENCRYPTED , SYMBOLICID, LANGUAGE_CD, MULTILANG , CURRENCY_CD , ACCTLOCK , 
                    PRCSPRFLCLS, DEFAULTNAVHP, FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTUPDOPRID, PTALLOWSWITCHUSER,LASTPSWDCHANGE)
                        values ('?OPRID', 8 , '?OPRDEFNDESC', '?EMPLID', '?EMAILID', 'HCPPALL','HCDPALL','?OPERPSWD','?OPERPSWDSALT',0 ,'SYSADM1' ,'ENG'  ,
                    0 ,'USD' ,0 ,'EPEX9000','HCSPNAVHP',0,0,0,'?USERIDALIAS','PPLSOFT',0,SYSDATE)";
    }
}
