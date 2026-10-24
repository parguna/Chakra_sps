using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Connector.PeopleSoft
{
    public static class Query
    {
        public const string UserSQL = @"select OPRID, OPRDEFNDESC, EMPLID,EMAILID, OPRCLASS, ROWSECCLASS, ENCRYPTED, SYMBOLICID,
                                    LANGUAGE_CD, MULTILANG, CURRENCY_CD, LASTPSWDCHANGE, ACCTLOCK, PRCSPRFLCLS, DEFAULTNAVHP,
                                    FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTSIGNONDTTM, LASTUPDDTTM, LASTUPDOPRID,
                                    PTALLOWSWITCHUSER from PSOPRDEFN ";
        public const string RolesSQL = @"select OPRID, OPRDEFNDESC, EMPLID,EMAILID, OPRCLASS, ROWSECCLASS, ENCRYPTED, SYMBOLICID,
                                    LANGUAGE_CD, MULTILANG, CURRENCY_CD, LASTPSWDCHANGE, ACCTLOCK, PRCSPRFLCLS, DEFAULTNAVHP,
                                    FAILEDLOGINS, EXPENT, OPRTYPE, USERIDALIAS, LASTSIGNONDTTM, LASTUPDDTTM, LASTUPDOPRID,
                                    PTALLOWSWITCHUSER from PSOPRDEFN ";
        public const string RolesSQLQuery = @"SELECT CLASSID, VERSION, CLASSDEFNDESC, TIMEOUTMINUTES, DEFAULTBPM, STARTAPPSERVER,
                                    ALLOWPSWDEMAIL, LASTUPDDTTM, LASTUPDOPRID from PSCLASSDEFN";
        public const string RolesSqlQuery = @"select * from PSaccesslog where (OPRID, LOGINDTTM) in (select OPRID, MAX(LOGINDTTM) from PSaccesslog group by OPRID)";
        public const string RolesSql = @"SELECT  ROLENAME, ROLEUSER, DYNAMIC_SW
                                     from PSRoleUser";
        public const string RolesSqlquery = @"SELECT * FROM PSRoleclass";
        public const string RolesSQLquery = @"SELECT MENUNAME, BARNAME, ITEMNAME, ITEMNUM, ITEMTYPE, PNLGRPNAME,
                                    MARKET, BARLABEL, ITEMLABEL,SEARCHRECNAME from PSMENUITEM";
        public const string RolessqlQuery = @"SELECT PORTAL_NAME, PORTAL_REFTYPE, PORTAL_OBJNAME, PORTAL_PERMNAME, PORTAL_PERMTYPE, PORTAL_ISCASCADE
                                    from PSPRSMPERM";
        public const string UserSQLQuery = @"select A.EMPLID EMPLID, A.EMPL_RCD, A.EFFDT, A.DEPTID DEPTID, A.JOBCODE, A.POSITION_NBR, A.SuperVisor_ID SUPERVISOR_ID, A.JOB_ENTRY_DT, 
                                           A.HIRE_DT HIRE_DT, A.BUSINESS_UNIT, A.SETID_DEPT, A.SETID_JOBCODE,  A.TERMINATION_DT TERMINATION_DT, A.ACTION, 
                                           A.FULL_PART_TIME FULL_PART_TIME, A.COMPANY COMPANY, C.DESCR DEPTNAME,
                                           B.NAME NAME, B.FIRST_NAME FIRST_NAME, B.LAST_NAME LAST_NAME, B.MIDDLE_NAME MIDDLE_NAME, B.NAME_AC, 
                                           B.Address1 ADDRESS1, B.ADDRESS2 ADDRESS2, B.ADDRESS3 ADDRESS3, B.ADDRESS4 ADDRESS4, B.CITY, B.NUM1 NUM1 ,
                                           B.NUM2 NUM2, B.COUNTY COUNTRY, B.STATE, B.POSTAL POSTAL, B.PHONE HOME_PHONE, B.SEX 
                                    from PS_JOB A INNER JOIN PS_PERSONAL_DATA B on B.emplID = A.EmPLID
                                                Inner JOIN ps_dept_tbl C on c.SETID = A.SETID_DEPT and C.DEPTID = A.DEPTID
                                          and C.ROWID in (select MAX(ROWID) from ps_dept_tbl group by SETID, DEPTID )
                                    where A.HR_STATUS in ('A','I') and EMPL_STATUS in ('A','T') and ACTION in( 'HIR', 'XFR', 'TER')
                                            and A.ROWID in (select max(ROWID) from PS_JOB Group by EMPLID)
                                            {0}
                                    order by EmplId asc, EMPL_RCD asc";
        public const string UserSQLquery = @"and (TO_CHAR(A.LASTUPDDTTM,'YYYY-MM-DD') >= '?DELTADATE' OR (TO_CHAR(B.LASTUPDDTTM, 'YYYY-MM-DD') >= '?DELTADATE'))";
        public const string UserSqlQuery = "insert into PeopleSoftRoleInfoPeopleSoftPermissionInfo([PeopleSoftRoleInfo_Id],[PeopleSoftPermissionInfo_Id],[EndpointSystemId],[Version]) values (?PeopleSoftRoleInfo_Id,?PeopleSoftPermissionInfo_Id,?EndpointSystemId,?Version)";
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
