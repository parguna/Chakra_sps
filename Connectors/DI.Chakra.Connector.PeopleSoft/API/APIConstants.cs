using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.PeopleSoft
{
    public static class PeopleSoftDataString
    {
        //PsuserRepositiory

        public static string PSConnectionString = @"Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.51)(PORT = 1521)))(CONNECT_DATA = (SID = fscmdm92))); User Id=sysadm;Password=sysadm;";
        public static string GetPSUsers = "SELECT * FROM PSOPRDEFN Where OPRID like :OPRID";
        public static string InsertPSUser = @"insert into PSOPRDEFN  values(:OPERID,8,'DeJackome Jeanette','KU0039',:EMAIL,
                                                     'EPEX9000','s',:PASSWORD,'+TtZW3C9ec8flOHENcCdqg==',1  ,'SYSADM1','ENG',0,'INR'
                                                     ,'24-FEB-2000',0,'EPEX9000','EPE9000',0,0,0,:IDALIAS,'24-JUL-15','24-JUL-15','PPLSOFT',0)";
        public static string UpdatePSUser = @"UPDATE PSOPRDEFN  SET OPERPSWD =:OPERPSWD Where OPRID = :OPRID";
        public static string DeletePSUser = @"delete from PSOPRDEFN where OPRID=:OPRID";
        public static string LockuserPSUser = @"update psoprdefn set acctlock =1 where OPRID=:OPRID";
        public static string UnlockuserPSUser = @"update psoprdefn set acctlock =0 where OPRID=:OPRID";

        //Psroleuserrepository

        public static string GetPSRoleUser = "SELECT * FROM PSRoleUser where roleuser like :roleuser";
        public static string insertPSRoleUser = @"
                                                         Insert into PSRoleUser VALUES(:ROLEUSER  , :ROLENAME ,:DYNAMIC_SW)";
        public static string deletePSRoleUser = @"delete from PSRoleUser where ROLEUSER=:ROLEUSER";

        //PsroleRepository

        public static string GetRoles = @"SELECT * FROM psroledefn Where ROLENAME like :rolename";

        public static string InsertRole = @"insert into PSROLEDEFN values(:ROLENAME,1,'U',:DESCR,'N','H',
                                                'F','K','A','R','V','N','N','N','Y','Y','31-JAN-1994','VP1',:DESCRLONG)";
        public static string UpdateRole = @"update psroledefn set VERSION=:version,ROLETYPE=:ROLETYPE,DESCR=:DESCR,QRYNAME=:QRYNAME,
                                                ROLESTATUS=:ROLESTATUS, RECNAME=:RECNAME  where ROLENAME =:ROLENAME";
        public static string DeleteRole = (@"delete from psroledefn where ROLENAME =:ROLENAME");

        //PsRoleclassRepository

        public static string GetPSRoleclass = "SELECT * FROM PSRoleclass where rolename like :rolename"; // + psrole.ROLENAME + "'";
        public static string insertPSRoleclass = @"Insert into PSAUTHITEM VALUES(:ROLENAME,:CLASSID)";
        public static string deletePSRoleclass = @"delete from PSAUTHITEM where ROLENAME=:ROLENAME";

        // PSPermissionRepository  
        public static string GetPermissions = "SELECT * FROM PSCLASSDEFN Where CLASSID like :CLASSID";
        public static string InsertPSPermission = @"Insert into PSCLASSDEFN VALUES(:CLASSID,:VERSION,:CLASSDEFNDESC,:TIMEOUTMINUTES,'
                                                          :DEFAULTBPM',0, 0 , '24-feb-2015','vp1')";
        public static string UpdatePSPermission = @"UPDATE PSCLASSDEFN  SET CLASSDEFNDESC = :CLASSDEFNDESC, DEFAULTBPM = :DEFAULTBPM
                                                           Where CLASSID = :CLASSID and version = :version";
        public static string DeletePSPermission = @"delete from PSCLASSDEFN where CLASSID=:CLASSID";

        //PSPrsmpermRepository
        public static string GetPSPrsmperms = "SELECT * FROM PSPRSMPERM ";
        public static string InsertPrsmperm = @"insert into PSPRSMPERM values(:PORTAL_NAME,:PORTAL_REFTYPE,:PORTAL_OBJNAME,
                                                    :PORTAL_PERMNAME, :PORTAL_PERMTYPE,:PORTAL_ISCASCADE)";
        public static string DeletePrsmperm = @"delete from PSPRSMPERM where PORTAL_NAME = :PORTAL_NAME";

        //PSPagesRepository
        public static string GetPSPages = "SELECT * FROM PSMENUITEM Where MENUNAME like :MENUNAME";

        //PSAuthsignonRepository
        public static string GetPSAuthsignons = @"SELECT * FROM PSAUTHSIGNON";
        public static string Insertauthsignon = @"insert into PSAUTHSIGNON values(:CLASSID,:DAYOFWEEK,:STARTTIME,:ENDTIME)";
        public static string Deleteauthprcs = @"delete from PSAUTHSIGNON where CLASSID = :CLASSID";

        //PSAuthprcsRepository
        public static string GetPSAuthprcss = @"SELECT * FROM PSAUTHPRCS";
        public static string Insertauthprcs = @"insert into PSAUTHPRCS values(:CLASSID,:PRCSGRP)";

        //PSAuthoptnRepository
        public static string GetPSAuthoptn = "SELECT * FROM PSAuthoptn";
        public static string insertPSAuthoptn = @" Insert into PSAuthoptn VALUES(:CLASSID ,:OPTN_CATEGORY_LVL ,:USEROPTN )";
        public static string deletePSAuthoptn = @"delete from PSAuthoptn where CLASSID=:CLASSID";

        //PSAuthitemRepository
        public static string GetPSAuthitem = "SELECT * FROM PSAuthitem where classid = :classid";
        public static string InsertPSAuthitem = @"Insert into PSAUTHITEM VALUES(:CLASSID,:MENUNAME ,:BARNAME,:BARITEMNAME,:PNLITEMNAME,
                                                         :DISPLAYONLY,:AUTHORIZEDACTIONS)";
        public static string DeletePSAuthitem = @"delete from PSAUTHITEM where CLASSID=:CLASSID";

        //PSAuthbuscompRepository     
        public static string GetPSAuthbuscomp = "SELECT * FROM PSAuthbuscomp";
        public static string insertPSAuthbuscomp = @"Insert into PSAuthbuscomp VALUES(:CLASSID  , :BCNAME ,:BCMETHOD,:AUTHORIZEDACTIONS )";
        public static string deletePSAuthbuscomp = @"delete from PSAuthbuscomp where CLASSID=:CLASSID";


    }
}
