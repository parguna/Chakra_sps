using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using SAP.Middleware.Connector;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class SAPV3Connector : ConnectorBase
    {

        #region EFConjunction Classes

        IList<SAPUserinfo.SAPUserGroup> listUserGroup = null;
        IList<SAPUserinfo.SAPUserProfile> listUserProfile = null;
        IList<SAPUserinfo.SAPUserParameter> listUserParameter = null;
        IList<SAPUserinfo.SAPUserRole> listUserRole = null;
        IList<SAPUserinfo.SapSingleRoleTcodes> listSingleRoleTcodes = null;

        #endregion

        #region VARIABLES


        private const string PROCESSNAME = "DiscoveryProcess";
        private string curDate = DateTime.Now.ToString("ddMMMyyyy");
        public long EndpointSystemId { get; set; }
        


        #endregion

        #region Position Related

        //SAPUserGrp.BAPIHR0283Table Employee_Archived_Objects = new SAPUserGrp.BAPIHR0283Table();
        //SAPUserGrp.BAPIHR0009Table Employee_Bank_Details = new SAPUserGrp.BAPIHR0009Table();
        //SAPUserGrp.BAPIHR0315Table Employee_Cats_Default_Values = new SAPUserGrp.BAPIHR0315Table();
        //SAPUserGrp.BAPIHR0105Table Employee_Communications = new SAPUserGrp.BAPIHR0105Table();
        //SAPUserGrp.BAPIHR0222Table Employee_Company_Cars = new SAPUserGrp.BAPIHR0222Table();
        //SAPUserGrp.BAPIHR0001Table Employee_Org_Assignment = new SAPUserGrp.BAPIHR0001Table();
        //SAPUserGrp.BAPIHR0003Table Employee_Payroll_Status = new SAPUserGrp.BAPIHR0003Table();
        //SAPUserGrp.BAPIHR0002Table Employee_Personal_Data = new SAPUserGrp.BAPIHR0002Table();
        //SAPUserGrp.BAPIHR0000Table Employee_Personnel_Action = new SAPUserGrp.BAPIHR0000Table();
        //SAPUserGrp.BAPIHR0006Table Employee_Private_Address = new SAPUserGrp.BAPIHR0006Table();
        //SAPUserGrp.BAPIHR0900Table Employee_Sales_Org_Assignment = new SAPUserGrp.BAPIHR0900Table();
        //SAPUserGrp.BAPIHR0050Table Employee_Time_Recording_Info = new SAPUserGrp.BAPIHR0050Table();
        //SAPUserGrp.BAPIHR0017Table Employee_Trip_Privileges = new SAPUserGrp.BAPIHR0017Table();
        //SAPUserGrp.BAPIHRINFTable Hr_Infotype_Header = new SAPUserGrp.BAPIHRINFTable();
        //SAPUserGrp.BAPIHROBJTable Hr_Object_Header = new SAPUserGrp.BAPIHROBJTable();
        //SAPUserGrp.BAPIHR1000Table Pd_Object = new SAPUserGrp.BAPIHR1000Table();
        //SAPUserGrp.BAPIHR1028Table Pd_Object_Address = new SAPUserGrp.BAPIHR1028Table();
        //SAPUserGrp.BAPIHR1043Table Pd_Object_Appraisal_Sample = new SAPUserGrp.BAPIHR1043Table();
        //SAPUserGrp.BAPIHR1044Table Pd_Object_Appraisal_Scale = new SAPUserGrp.BAPIHR1044Table();
        //SAPUserGrp.BAPIHR1010Table Pd_Object_Authority_Resources = new SAPUserGrp.BAPIHR1010Table();
        //SAPUserGrp.BAPIHR1023Table Pd_Object_B_Event_Availability = new SAPUserGrp.BAPIHR1023Table();
        //SAPUserGrp.BAPIHR1041Table Pd_Object_B_Event_Blocks = new SAPUserGrp.BAPIHR1041Table();
        //SAPUserGrp.BAPIHR1024Table Pd_Object_B_Event_Capacity = new SAPUserGrp.BAPIHR1024Table();
        //SAPUserGrp.BAPIHR1036Table Pd_Object_B_Event_Costs = new SAPUserGrp.BAPIHR1036Table();
        //SAPUserGrp.BAPIHR1060Table Pd_Object_B_Event_Demand = new SAPUserGrp.BAPIHR1060Table();
        //SAPUserGrp.BAPIHR1026Table Pd_Object_B_Event_Info = new SAPUserGrp.BAPIHR1026Table();
        //SAPUserGrp.BAPIHR1034Table Pd_Object_B_Event_Name_Format = new SAPUserGrp.BAPIHR1034Table();
        //SAPUserGrp.BAPIHR1021Table Pd_Object_B_Event_Price = new SAPUserGrp.BAPIHR1021Table();
        //SAPUserGrp.BAPIHR1030Table Pd_Object_B_Event_Procedure = new SAPUserGrp.BAPIHR1030Table();
        //SAPUserGrp.BAPIHR1042Table Pd_Object_B_Event_Sched_Model = new SAPUserGrp.BAPIHR1042Table();
        //SAPUserGrp.BAPIHR1035Table Pd_Object_B_Event_Schedule = new SAPUserGrp.BAPIHR1035Table();
        //SAPUserGrp.BAPIHR1029Table Pd_Object_B_Event_Type = new SAPUserGrp.BAPIHR1029Table();
        //SAPUserGrp.BAPIHR1062Table Pd_Object_B_Eventknowledgelink = new SAPUserGrp.BAPIHR1062Table();
        //SAPUserGrp.BAPIHR1037Table Pd_Object_Cost_Alloc_Invoice = new SAPUserGrp.BAPIHR1037Table();
        //SAPUserGrp.BAPIHR1018Table Pd_Object_Cost_Distribution = new SAPUserGrp.BAPIHR1018Table();
        //SAPUserGrp.BAPIHR1015Table Pd_Object_Cost_Planning = new SAPUserGrp.BAPIHR1015Table();
        //SAPUserGrp.BAPIHR1003Table Pd_Object_Department_Staff = new SAPUserGrp.BAPIHR1003Table();
        //SAPUserGrp.BAPIHR1002Table Pd_Object_Description = new SAPUserGrp.BAPIHR1002Table();
        //SAPUserGrp.BAPIHR1013Table Pd_Object_Empl_Group_Subgroup = new SAPUserGrp.BAPIHR1013Table();
        //SAPUserGrp.BAPIHR1038Table Pd_Object_External_Key = new SAPUserGrp.BAPIHR1038Table();
        //SAPUserGrp.BAPIHR1019Table Pd_Object_Head_Count_Budget = new SAPUserGrp.BAPIHR1019Table();
        //SAPUserGrp.BAPIHR1009Table Pd_Object_Health_Examinations = new SAPUserGrp.BAPIHR1009Table();
        //SAPUserGrp.BAPIHR1051Table Pd_Object_Job_Market_Info = new SAPUserGrp.BAPIHR1051Table();
        //SAPUserGrp.BAPIHR1032Table Pd_Object_Mail_Address = new SAPUserGrp.BAPIHR1032Table();
        //SAPUserGrp.BAPIHR1014Table Pd_Object_Obsolete = new SAPUserGrp.BAPIHR1014Table();
        //SAPUserGrp.BAPIHR1040Table Pd_Object_Override_Reqmnt = new SAPUserGrp.BAPIHR1040Table();
        //SAPUserGrp.BAPIHR1017Table Pd_Object_Pd_Profiles = new SAPUserGrp.BAPIHR1017Table();
        //SAPUserGrp.BAPIHR1005Table Pd_Object_Planned_Compensation = new SAPUserGrp.BAPIHR1005Table();
        //SAPUserGrp.BAPIHR1008Table Pd_Object_Posting_Specs = new SAPUserGrp.BAPIHR1008Table();
        //SAPUserGrp.BAPIHR1047Table Pd_Object_Processing_Fm = new SAPUserGrp.BAPIHR1047Table();
        //SAPUserGrp.BAPIHR1048Table Pd_Object_Proficiency_Desc = new SAPUserGrp.BAPIHR1048Table();
        //SAPUserGrp.BAPIHR1001Table Pd_Object_Relationships = new SAPUserGrp.BAPIHR1001Table();
        //SAPUserGrp.BAPIHR1006Table Pd_Object_Restrictions = new SAPUserGrp.BAPIHR1006Table();
        //SAPUserGrp.BAPIHR1031Table Pd_Object_Room_Reserv_Info = new SAPUserGrp.BAPIHR1031Table();
        //SAPUserGrp.BAPIHR1039Table Pd_Object_Shift_Group = new SAPUserGrp.BAPIHR1039Table();
        //SAPUserGrp.BAPIHR1027Table Pd_Object_Site_Dependent_Info = new SAPUserGrp.BAPIHR1027Table();
        //SAPUserGrp.BAPIHR1016Table Pd_Object_Standard_Profiles = new SAPUserGrp.BAPIHR1016Table();
        //SAPUserGrp.BAPIHR1004Table Pd_Object_Task_Character = new SAPUserGrp.BAPIHR1004Table();
        //SAPUserGrp.BAPIHR1007Table Pd_Object_Vacancy = new SAPUserGrp.BAPIHR1007Table();
        //SAPUserGrp.BAPIHR1025Table Pd_Object_Validity = new SAPUserGrp.BAPIHR1025Table();
        //SAPUserGrp.BAPIHR1045Table Pd_Object_Valuation = new SAPUserGrp.BAPIHR1045Table();
        //SAPUserGrp.BAPIHR1050Table Pd_Object_Work_Evaluat_Result = new SAPUserGrp.BAPIHR1050Table();
        //SAPUserGrp.BAPIHR1033Table Pd_Object_Work_Reqmnt_Scale = new SAPUserGrp.BAPIHR1033Table();
        //SAPUserGrp.BAPIHR1011Table Pd_Object_Work_Schedule = new SAPUserGrp.BAPIHR1011Table();
        //SAPUserGrp.BAPIHRAD48Table Pd_Rel_Application_For = new SAPUserGrp.BAPIHRAD48Table();
        //SAPUserGrp.BAPIHRAD47Table Pd_Rel_Appraisal_Model = new SAPUserGrp.BAPIHRAD47Table();
        //SAPUserGrp.BAPIHRAD50Table Pd_Rel_Appraises = new SAPUserGrp.BAPIHRAD50Table();
        //SAPUserGrp.BAPIHRAD44Table Pd_Rel_Career_Develop_Action = new SAPUserGrp.BAPIHRAD44Table();
        //SAPUserGrp.BAPIHRAD21Table Pd_Rel_Is_Equipped = new SAPUserGrp.BAPIHRAD21Table();
        //SAPUserGrp.BAPIHRAD27Table Pd_Rel_Is_Prebooked_For = new SAPUserGrp.BAPIHRAD27Table();
        //SAPUserGrp.BAPIHRAD63Table Pd_Rel_Job_Reqmnts = new SAPUserGrp.BAPIHRAD63Table();
        //SAPUserGrp.BAPIHRADXNTable Pd_Rel_Message_Type = new SAPUserGrp.BAPIHRADXNTable();
        //SAPUserGrp.BAPIHRAD31Table Pd_Rel_Qualifications_Reqmnts = new SAPUserGrp.BAPIHRAD31Table();
        //SAPUserGrp.BAPIHRAD22Table Pd_Rel_Requires = new SAPUserGrp.BAPIHRAD22Table();
        //SAPUserGrp.BAPIHRAD23Table Pd_Rel_Reserves = new SAPUserGrp.BAPIHRAD23Table();
        //SAPUserGrp.BAPIHRAD51Table Pd_Rel_Session = new SAPUserGrp.BAPIHRAD51Table();
        //SAPUserGrp.BAPIHRADD2Table Pd_Rel_Specified_Substitute = new SAPUserGrp.BAPIHRADD2Table();
        //SAPUserGrp.BAPIHRAD53Table Pd_Rel_Still_Requires = new SAPUserGrp.BAPIHRAD53Table();
        //SAPUserGrp.BAPIHRAD25Table Pd_Rel_Takes_Part_In = new SAPUserGrp.BAPIHRAD25Table();
        //SAPUserGrp.BAPIRET2Table Return0 = new SAPUserGrp.BAPIRET2Table();

        #endregion

        #region  SAP INITAILIZE

        public int intmaxrows = 0;
        public TimeSpan tt1;
        public DateTime ScheduleStartdt;
        public DateTime ScheduleEnddt;
        string strLastLogin, hh, mm, ss;

        #endregion

        public EndpointConfiguration SAPConfiguration { get; set; }
        private List<SAPRoleInfo> SapRoles;
        private List<SAPProfileInfo> SapProfiles;
        private IList<string> DeltaUsers;
        private IList<string> DeltaRoles;

        RfcDestination SAPdestination = null;
        RfcConfigParameters configParameters = new RfcConfigParameters();
        RfcDestination rfcSAPDestination = null;


        string deltaDate = string.Empty;


        public SAPV3Connector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;

            var sapConfig = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            sapConfig.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.SAPConfiguration = sapConfig;

           
            SapRoles = new List<SAPRoleInfo>();
            SapProfiles = new List<SAPProfileInfo>();
            DeltaUsers = new List<string>();
            DeltaRoles = new List<string>();
            if (!this.IsFirstTimeDiscovery)
            {
                deltaDate = this.Process.LastRunOn.Value.ToString("yyyyMMdd");
            }

        }

        #region "Connector Base Members"
        private string Configuration(EndpointKey Key)
        {
            var config = SAPConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override bool Connect()
        {
            try
            {
                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    WriteDiscoveryLog(LogLevelType.Info, string.Format("Connected to SAP Server - {0}",
                //                                          this.SAPConfiguration.IPAddress));
                //    return true;
                //}
                return true;
            }

            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Could not connect to SAP Server - {0}",
                                                    Configuration(EndpointKey.IPAddress)), ex);
                return false;
            }
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void GetData()
        {
            try
            {
                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                long discoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(discoveryProcessId);
                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_User) != null;
                bool IsRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Role) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Group) != null;
                bool IsProfileSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Profile) != null;
                bool IsAuthorizationSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Authorization) != null;
                bool IsTCodeSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_TCode) != null;
                bool IsParametersSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Parameter) != null;
                bool IsPositionSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_SAP_Position) != null;

                if (!IsFirstTimeDiscovery)
                {
                    deltaDate = this.Process.LastRunOn.Value.ToString("yyyyMMdd");
                    DeltaUsers = GetDeltaUsers();
                    DeltaRoles = GetDeltaRoles();
                }

                if (IsUserSelected)
                {
                    DiscoverUsers();
                }

                if (IsRoleSelected)
                {
                    DiscoverRoles();
                }

                if (IsProfileSelected)
                {
                    DiscoverProfiles();
                }

                if (IsGroupSelected)
                {
                    DiscoverGroups();
                }

                if (IsTCodeSelected)
                {
                    DiscoverTCodes();
                }

                if (IsParametersSelected)
                {
                    DiscoverParameters();
                }

                if (IsPositionSelected)
                {
                    DiscoverPositions();
                }

                if (IsAuthorizationSelected)
                {
                    DiscoverAuthorizations();
                }

                if (!EndPoint.IsTrustedSource)
                {
                    if (IsUserSelected)
                    {
                        DiscoverMappingTables(true, true, true, true, IsTCodeSelected);
                    }
                    else
                    {
                        DiscoverMappingTables(IsUserSelected, IsRoleSelected, IsProfileSelected, IsGroupSelected, IsTCodeSelected);
                    }
                }

                IList<SAPUserinfo> listSAPUsers = null;

                if (IsUserSelected)
                {
                    listSAPUsers = SAPUserinfo.GetSAPUserByEndpointSystemID(EndPoint.Id);
                   // SAPUserinfo.MapWithMasterUser(EndPoint, SAPConfiguration.MapAll != null ? SAPConfiguration.MapAll : false);
                    SAPUserinfo.MapWithMasterUserForReconTable(EndPoint, SAPConfiguration.MapAll != null ? SAPConfiguration.MapAll : false, discoveryProcessId);
                    User.GetAllMasterUsers(listSAPUsers, SAPConfiguration.MapAll, MasterConfigHelper.ServiceUser);
                }
                //}
            }
            catch (Exception ab)
            {
                throw ab;
            }
        }

        private bool ChangePasswordForReactivation(string UserName, string Password, long DIUserId)
        {

            //SAPUserDetailsComponent.BAPIRET2 Sapusrcomp = new SAPUserDetailsComponent.BAPIRET2();
            //byte[] newByte = new byte[1];


            try
            {

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();
                //    #region SAP BAPI CLASSES

                //    BAPIADDR3 address = new BAPIADDR3();
                //    BAPIUSCOMP companyName = new BAPIUSCOMP();
                //    BAPIALIAS alias = new BAPIALIAS();
                //    BAPIDEFAUL defaults = new BAPIDEFAUL();
                //    BAPISLOCKD islocked = new BAPISLOCKD();
                //    BAPILOGOND logonData = new BAPILOGOND();
                //    BAPIPWD passworddata = new BAPIPWD();
                //    BAPIMODDAT lastmofified = new BAPIMODDAT();
                //    BAPIREFUS refuser = new BAPIREFUS();
                //    BAPISNCU snc = new BAPISNCU();
                //    BAPIUCLASS iuclass = new BAPIUCLASS();
                //    BAPIAGRTable activeGroups = new BAPIAGRTable();
                //    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                //    BAPIADFAXTable addfax = new BAPIADFAXTable();
                //    BAPIADPAGTable addpag = new BAPIADPAGTable();
                //    BAPIADPRTTable addprt = new BAPIADPRTTable();
                //    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                //    BAPIADRMLTable addrml = new BAPIADRMLTable();
                //    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                //    BAPIADSSFTable addssf = new BAPIADSSFTable();
                //    BAPIADTELTable addtel = new BAPIADTELTable();
                //    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                //    BAPIADTTXTable addttx = new BAPIADTTXTable();
                //    BAPIADURITable adduri = new BAPIADURITable();
                //    BAPIADX400Table addx400 = new BAPIADX400Table();
                //    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                //    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                //    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                //    BAPIPARAMTable parameter = new BAPIPARAMTable();
                //    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                //    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                //    BAPIPROFTable profiles = new BAPIPROFTable();
                //    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                //    //SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                //    BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                //    BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                //    BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                //    AGR_TCODESTable TCodes = new AGR_TCODESTable();
                //    AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                //    AGR_HIERTable HeirTable = new AGR_HIERTable();
                //    AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                //    AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                //    BAPILOGONX BAPILOGONX = new BAPILOGONX();
                //    BAPIPWDX BAPIPWDX = new BAPIPWDX();
                //    BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                //    BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                //    BAPIALIASX BAPIALIASX = new BAPIALIASX();
                //    BAPIFLAG BAPIFLAG = new BAPIFLAG();
                //    BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                //    BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                //    BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                //    BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                //    BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                //    BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                //    BAPISNCUX BAPISNCUX = new BAPISNCUX();
                //    BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                //    BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                //    SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                //    #endregion
                //    passworddata.Bapipwd = Password;
                //    BAPIPWDX.Bapipwd = Password;

                RfcRepository changeUserRepository = SAPdestination.Repository;
                IRfcFunction changeUserFunction = changeUserRepository.CreateFunction("BAPI_USER_CHANGE");

                changeUserFunction.SetValue("USERNAME", UserName);

                changeUserFunction.SetValue("PASSWORD", Password);

                IRfcStructure passwordX = changeUserFunction.GetStructure("PASSWORDX");
                passwordX.SetValue("BAPIPWD", Password);
                changeUserFunction.Invoke(SAPdestination);


                IRfcTable return0 = changeUserFunction.GetTable("RETURN");

                //proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, BAPIDEFAX, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, UserName, ref addcomrem,
                //                  ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);

                string UserStatus = return0.ToDataTable("RETURN").Rows[0]["MESSAGE"].ToString();
                string MessageType = return0.ToDataTable("RETURN").Rows[0]["TYPE"].ToString(); ;
                if (UserStatus.Contains(string.Format("User {0} has changed", UserName)))
                {
                    DI.Chakra.DomainModel.EndPoint.UpdatePasswordresetcount(this.EndPoint.Id, UserName, MasterConfigHelper.ServiceUser);
                    return true;
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, UserStatus);
                    return false;
                }


                //}
            }

            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }

        private bool ChangePassword(string UserName, string Password, long DIUserId)
        {
            string TempPassword = "@tmp4321";

            //Byte outPass = new byte();
            //SAPUserDetailsComponent.BAPIRET2 Sapusrcomp = new SAPUserDetailsComponent.BAPIRET2();
            //byte[] newByte = new byte[1];


            try
            {

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();
                //    #region SAP BAPI CLASSES

                //    BAPIADDR3 address = new BAPIADDR3();
                //    BAPIUSCOMP companyName = new BAPIUSCOMP();
                //    BAPIALIAS alias = new BAPIALIAS();
                //    BAPIDEFAUL defaults = new BAPIDEFAUL();
                //    BAPISLOCKD islocked = new BAPISLOCKD();
                //    BAPILOGOND logonData = new BAPILOGOND();
                //    BAPIPWD passworddata = new BAPIPWD();
                //    BAPIMODDAT lastmofified = new BAPIMODDAT();
                //    BAPIREFUS refuser = new BAPIREFUS();
                //    BAPISNCU snc = new BAPISNCU();
                //    BAPIUCLASS iuclass = new BAPIUCLASS();
                //    BAPIAGRTable activeGroups = new BAPIAGRTable();
                //    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                //    BAPIADFAXTable addfax = new BAPIADFAXTable();
                //    BAPIADPAGTable addpag = new BAPIADPAGTable();
                //    BAPIADPRTTable addprt = new BAPIADPRTTable();
                //    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                //    BAPIADRMLTable addrml = new BAPIADRMLTable();
                //    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                //    BAPIADSSFTable addssf = new BAPIADSSFTable();
                //    BAPIADTELTable addtel = new BAPIADTELTable();
                //    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                //    BAPIADTTXTable addttx = new BAPIADTTXTable();
                //    BAPIADURITable adduri = new BAPIADURITable();
                //    BAPIADX400Table addx400 = new BAPIADX400Table();
                //    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                //    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                //    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                //    BAPIPARAMTable parameter = new BAPIPARAMTable();
                //    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                //    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                //    BAPIPROFTable profiles = new BAPIPROFTable();
                //    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                //    SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                //    BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                //    BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                //    BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                //    AGR_TCODESTable TCodes = new AGR_TCODESTable();
                //    AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                //    AGR_HIERTable HeirTable = new AGR_HIERTable();
                //    AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                //    AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                //    BAPILOGONX BAPILOGONX = new BAPILOGONX();
                //    BAPIPWDX BAPIPWDX = new BAPIPWDX();
                //    BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                //    BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                //    BAPIALIASX BAPIALIASX = new BAPIALIASX();
                //    BAPIFLAG BAPIFLAG = new BAPIFLAG();
                //    BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                //    BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                //    BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                //    BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                //    BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                //    BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                //    BAPISNCUX BAPISNCUX = new BAPISNCUX();
                //    BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                //    BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                //    SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                //    #endregion
                //    passworddata.Bapipwd = TempPassword;
                //    BAPIPWDX.Bapipwd = TempPassword;


                //    proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, BAPIDEFAX, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, UserName, ref addcomrem,
                //                      ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);

                RfcRepository bapiRepository = SAPdestination.Repository;
                IRfcFunction changeUserFunction = bapiRepository.CreateFunction("BAPI_USER_CHANGE");

                changeUserFunction.SetValue("USERNAME", UserName);

                changeUserFunction.SetValue("PASSWORD", TempPassword);

                IRfcStructure passwordX = changeUserFunction.GetStructure("PASSWORDX");
                passwordX.SetValue("BAPIPWD", TempPassword);
                changeUserFunction.Invoke(SAPdestination);


                IRfcTable return0 = changeUserFunction.GetTable("RETURN");

                //proxy.Bapi_User_Change(BAPIADCOMX, address, BAPIADDR3X, alias, BAPIALIASX, BAPIFLAG, companyName, BAPIUSCOMX, defaults, BAPIDEFAX, BAPIUSEXTIDX, BAPIGROUPX, logonData, BAPILOGONX, BAPIPARAMX, passworddata, BAPIPWDX, "", refuser, BAPIREFUSX, snc, BAPISNCUX, iuclass, BAPIUCLASSX, UserName, ref addcomrem,
                //                  ref addfax, ref addpag, ref addprt, ref addrfc, ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx, ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter, ref parameter1, ref return0, ref BAPIUCLASSSYSTable);

                string UserStatus = return0.ToDataTable("RETURN").Rows[0]["MESSAGE"].ToString();
                string MessageType = return0.ToDataTable("RETURN").Rows[0]["TYPE"].ToString();

                //string UserStatus = return0.ToADODataTable().Rows[0]["Message"].ToString();
                //string MessageType = return0.ToADODataTable().Rows[0]["Type"].ToString();
                if (string.Compare(UserStatus, string.Format("User {0} has changed", UserName), true) == 0)
                //if (UserStatus.Contains(string.Format("User {0} has changed", UserName)))
                {
                    //proxy.Susr_User_Change_Password_Rfc(UserName, newByte, "", Password, TempPassword, (byte)'0', (byte)'0', out outPass, out  Sapusrcomp);

                    changeUserFunction = bapiRepository.CreateFunction("BAPI_USER_CHANGE");

                    changeUserFunction.SetValue("USERNAME", UserName);

                    changeUserFunction.SetValue("NEW_PASSWORD", Password);
                    changeUserFunction.SetValue("PASSWORD", TempPassword);

                    changeUserFunction.Invoke(SAPdestination);


                    return0 = changeUserFunction.GetTable("RETURN");
                    if (return0.ToDataTable("RETURN").Rows[0]["MESSAGE"].ToString() == "Password changed")
                    {
                        DI.Chakra.DomainModel.EndPoint.UpdatePasswordresetcount(this.EndPoint.Id, UserName, MasterConfigHelper.ServiceUser);
                        return true;
                    }
                    else
                    {
                        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, return0.ToDataTable("RETURN").Rows[0]["MESSAGE"].ToString());
                        return false;
                    }
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, UserStatus);
                    return false;
                }

                //}
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, ex.Message, ex);
                return false;
            }
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            if (isReactivation)
            {
                return ChangePasswordForReactivation(UserName, Password, DIUserId);
            }
            else
            {
                return ChangePassword(UserName, Password, DIUserId);
            }
        }

        //public override bool CreateUser(IEndpointUserInfo _userInfo, IProvisioningForm _formInfo)
        //{
        //    SAPUserinfo _SAPUserInfo = (SAPUserinfo)_userInfo;
        //    ProvisioningFormSAP _sapForm = (ProvisioningFormSAP)_formInfo;

        //    string UserExist = string.Empty;
        //    string RoleStatus = string.Empty;

        //    try
        //    {

        //        RfcRepository bapiRepository;
        //        IRfcFunction createUserFunction;

        //        SAPdestination = GetSAPDestination();
        //        bapiRepository = SAPdestination.Repository;

        //        IRfcFunction checkUserExistenceFunction = bapiRepository.CreateFunction("BAPI_USER_EXISTENCE_CHECK");

        //        checkUserExistenceFunction.SetValue("USERNAME", _SAPUserInfo.UserLogonName);

        //        checkUserExistenceFunction.Invoke(SAPdestination);

        //        IRfcStructure return0 = checkUserExistenceFunction.GetStructure("RETURN");

        //        UserExist = return0.GetValue("MESSAGE").ToString();

        //        //SAPUserDetailsComponent.BAPIRET2Table returnUserStatus = new SAPUserDetailsComponent.BAPIRET2Table();
        //        if ((UserExist.ToUpper().Contains("NOT EXIST")))
        //        {

        //            createUserFunction = bapiRepository.CreateFunction("BAPI_USER_CREATE");

        //            IRfcStructure sAddress = createUserFunction.GetStructure("address");
        //            sAddress.SetValue("Title", _SAPUserInfo.Title);
        //            sAddress.SetValue("Firstname", _SAPUserInfo.Firstname);
        //            sAddress.SetValue("Lastname", _SAPUserInfo.Lastname);
        //            sAddress.SetValue("Function", _SAPUserInfo.Function0);
        //            sAddress.SetValue("Department", _SAPUserInfo.Department);
        //            sAddress.SetValue("E_Mail", _SAPUserInfo.E_Mail);
        //            sAddress.SetValue("Tel1_Numbr", _SAPUserInfo.Tel1_Numbr);
        //            sAddress.SetValue("Pers_No", _SAPUserInfo.Pers_No);
        //            sAddress.SetValue("City", _SAPUserInfo.City);

        //            if (!string.IsNullOrEmpty(_sapForm.FormatCountry))
        //            {
        //                sAddress.SetValue("Langu_P", "E");
        //                sAddress.SetValue("Namcountry", _sapForm.FormatCountry);

        //            }

        //            createUserFunction.SetValue("address", sAddress);

        //            IRfcStructure sDefaults = createUserFunction.GetStructure("defaults");
        //            if (!string.IsNullOrEmpty(_SAPUserInfo.CostCenter))
        //            {
        //                sDefaults.SetValue("Kostl", _SAPUserInfo.CostCenter);
        //            }
        //            if (!string.IsNullOrEmpty(_sapForm.OutputDevice))
        //            {
        //                sDefaults.SetValue("Spld", _sapForm.OutputDevice);
        //            }
        //            if (!string.IsNullOrEmpty(_sapForm.NewUser_DecimalNotation))
        //            {
        //                sDefaults.SetValue("Dcpfm", _sapForm.NewUser_DecimalNotation);
        //            }
        //            if (!string.IsNullOrEmpty(_sapForm.LogonLanguage))
        //            {
        //                sDefaults.SetValue("Langu", _sapForm.LogonLanguage);
        //            }
        //            if (!string.IsNullOrEmpty(_sapForm.DateFormat))
        //            {
        //                sDefaults.SetValue("Datfm", _sapForm.DateFormat);
        //            }
        //            if (!string.IsNullOrEmpty(_sapForm.TimeFormat))
        //            {
        //                sDefaults.SetValue("Timefm", _sapForm.TimeFormat);
        //            }


        //            createUserFunction.SetValue("defaults", sDefaults);

        //            IRfcStructure sPassworddata = createUserFunction.GetStructure("password");
        //            sPassworddata.SetValue("Bapipwd", _SAPUserInfo.Password);

        //            createUserFunction.SetValue("password", sPassworddata);


        //            IRfcStructure sLogonData = createUserFunction.GetStructure("LOGONDATA");
        //            string userValidFrom = !string.IsNullOrEmpty(_SAPUserInfo.UserValidFrom) ? _SAPUserInfo.UserValidFrom : DateTime.Today.ToString("yyyy.MM.dd");
        //            sLogonData.SetValue("Gltgv", userValidFrom);
        //            sLogonData.SetValue("Gltgb", "9999.12.31");


        //            if (_SAPUserInfo.UserType == SAPUserType.Communication)
        //            {
        //                sLogonData.SetValue("Ustyp", "C");
        //                //logonData.Ustyp = "C";
        //            }
        //            else if (_SAPUserInfo.UserType == SAPUserType.Service)
        //            {
        //                sLogonData.SetValue("Ustyp", "S");
        //                //logonData.Ustyp = "S";
        //            }
        //            else if (_SAPUserInfo.UserType == SAPUserType.System)
        //            {
        //                sLogonData.SetValue("Ustyp", "B");
        //                //logonData.Ustyp = "B";
        //            }
        //            else if (_SAPUserInfo.UserType == SAPUserType.Reference)
        //            {
        //                sLogonData.SetValue("Ustyp", "L");
        //                //logonData.Ustyp = "L";
        //            }
        //            else
        //            {
        //                sLogonData.SetValue("Ustyp", "A");
        //                //logonData.Ustyp = "A";
        //            }


        //            if (_SAPUserInfo.Groups != null && _SAPUserInfo.Groups.Count != 0)
        //            {
        //                sLogonData.SetValue("Class", _SAPUserInfo.Groups[0]);
        //                //logonData.Class = _SAPUserInfo.Groups[0];
        //            }


        //            IRfcTable tParameters = createUserFunction.GetTable("PARAMETER");

        //            foreach (KeyValuePair<string, string> Param in _SAPUserInfo.Parameters)
        //            {

        //                //BAPIPARAM BAPIPARAM = new BAPIPARAM();
        //                tParameters.Append();
        //                tParameters.SetValue("Parid", Param.Key);
        //                tParameters.SetValue("Parva", Param.Value);
        //                //BAPIPARAM.Parid = Param.Key;
        //                //BAPIPARAM.Parva = Param.Value;
        //                //parameter.Add(BAPIPARAM);
        //            }



        //            createUserFunction.Invoke(SAPdestination);

        //            IRfcTable return1 = createUserFunction.GetTable("RETURN");

        //            DataTable Result = return1.ToDataTable("RETURN");

        //            if (Result != null && Result.Rows.Count > 0)
        //            {
        //                bool isUserCreated = false;
        //                foreach (DataRow row in Result.Rows)
        //                {
        //                    if (row["Message"].ToString().ToUpper() == ("User " + _SAPUserInfo.UserLogonName + " created").ToUpper())
        //                    {
        //                        isUserCreated = true;
        //                        break;
        //                    }
        //                }

        //                //SAPUserDetailsComponent.BAPIRET2Table returnRoleStatus = new SAPUserDetailsComponent.BAPIRET2Table();
        //                if (isUserCreated)
        //                {
        //                    //Assigning Default Roles

        //                    IRfcFunction assignUserRolesFunction = bapiRepository.CreateFunction("BAPI_USER_ACTGROUPS_ASSIGN");
        //                    assignUserRolesFunction.SetValue("USERNAME", _SAPUserInfo.UserLogonName);

        //                    IRfcTable tRoles = assignUserRolesFunction.GetTable("ACTIVITYGROUPS");

        //                    foreach (string Role in _SAPUserInfo.Roles)
        //                    {
        //                        //BAPIAGR = new BAPIAGR();
        //                        tRoles.Append();
        //                        tRoles.SetValue("Agr_Name", Role);
        //                        //BAPIAGR.Agr_Name = Role;
        //                        //
        //                        tRoles.SetValue("From_Dat", _SAPUserInfo.UserValidFrom);
        //                        //BAPIAGR.From_Dat = _SAPUserInfo.UserValidFrom;
        //                        tRoles.SetValue("To_Dat", _SAPUserInfo.UserValidTo);
        //                        //BAPIAGR.To_Dat = _SAPUserInfo.UserValidTo;
        //                        //Roles.Add(BAPIAGR);
        //                    }

        //                    assignUserRolesFunction.Invoke(SAPdestination);
        //                    IRfcTable return2 = assignUserRolesFunction.GetTable("RETURN");
        //                    RoleStatus = return2.ToDataTable("RETURN").Rows[0]["Message"].ToString();

        //                    //proxy.Bapi_User_Actgroups_Assign(_SAPUserInfo.UserLogonName, ref Roles, ref returnRoleStatus);
        //                    //RoleStatus = returnRoleStatus.ToADODataTable().Rows[0]["Message"].ToString();
        //                    DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Info, RoleStatus);

        //                    //Assigning Profiles

        //                    IRfcFunction assignUserProfilesFunction = bapiRepository.CreateFunction("BAPI_USER_PROFILES_ASSIGN");

        //                    assignUserProfilesFunction.SetValue("USERNAME", _SAPUserInfo.UserLogonName);

        //                    IRfcTable tProfiles = assignUserRolesFunction.GetTable("PROFILES");

        //                    List<string> lstProfiles = _SAPUserInfo.Profiles == null ? new List<string>() : _SAPUserInfo.Profiles;

        //                    foreach (string profile in lstProfiles)
        //                    {
        //                        //BAPIPROF bapiPROF = new BAPIPROF();
        //                        tProfiles.Append();
        //                        tProfiles.SetValue("Bapiprof", profile);
        //                        //bapiPROF.Bapiprof = profile;
        //                        //profiles.Add(bapiPROF);
        //                    }

        //                    assignUserProfilesFunction.Invoke(SAPdestination);
        //                    return true;
        //                }
        //                else
        //                {

        //                    foreach (DataRow row in Result.Rows)
        //                    {
        //                        if (row["Message"] != null)
        //                        {
        //                            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, "Return Msg ->" + row["Message"].ToString());
        //                        }
        //                    }

        //                    return false;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, String.Format("User {0} already exists.", _SAPUserInfo.UserLogonName));
        //            return false;
        //        }


        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, "Error while adding user in SAP.", ex);
        //        return false;
        //    }
        //}


        //private BAPILOGOND GetLogonInfoFromSAP(string userName)
        private IRfcStructure GetLogonInfoFromSAP(string userName)
        {
            try
            {
                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    #region SAP BAPI CLASSES

                //    BAPIADDR3 address = new BAPIADDR3();
                //    BAPIUSCOMP companyName = new BAPIUSCOMP();
                //    BAPIALIAS alias = new BAPIALIAS();
                //    BAPIDEFAUL defaults = new BAPIDEFAUL();
                //    BAPISLOCKD islocked = new BAPISLOCKD();
                //    BAPILOGOND logonData = new BAPILOGOND();
                //    BAPIMODDAT lastmofified = new BAPIMODDAT();
                //    BAPIREFUS refuser = new BAPIREFUS();
                //    BAPISNCU snc = new BAPISNCU();
                //    BAPIUCLASS iuclass = new BAPIUCLASS();
                //    BAPIAGRTable activeGroups = new BAPIAGRTable();
                //    BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                //    BAPIADFAXTable addfax = new BAPIADFAXTable();
                //    BAPIADPAGTable addpag = new BAPIADPAGTable();
                //    BAPIADPRTTable addprt = new BAPIADPRTTable();
                //    BAPIADRFCTable addrfc = new BAPIADRFCTable();
                //    BAPIADRMLTable addrml = new BAPIADRMLTable();
                //    BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                //    BAPIADSSFTable addssf = new BAPIADSSFTable();
                //    BAPIADTELTable addtel = new BAPIADTELTable();
                //    BAPIADTLXTable addtlx = new BAPIADTLXTable();
                //    BAPIADTTXTable addttx = new BAPIADTTXTable();
                //    BAPIADURITable adduri = new BAPIADURITable();
                //    BAPIADX400Table addx400 = new BAPIADX400Table();
                //    BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                //    BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                //    BAPIGROUPSTable groups = new BAPIGROUPSTable();
                //    BAPIPARAMTable parameter = new BAPIPARAMTable();
                //    BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                //    BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                //    BAPIPROFTable profiles = new BAPIPROFTable();
                //    BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                //    SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();

                //    #endregion

                RfcRepository bapiRepository;

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                IRfcFunction getUserDetailsFunction = bapiRepository.CreateFunction("BAPI_USER_GET_DETAIL");

                getUserDetailsFunction.SetValue("USERNAME", userName);

                getUserDetailsFunction.Invoke(SAPdestination);

                IRfcStructure sLogonData = getUserDetailsFunction.GetStructure("LOGONDATA");





                //proxy.Bapi_User_Get_Detail(userName, userName, out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out  snc, out iuclass,
                //                                                  ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
                //                                        ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
                //                                         ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
                //                                         ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);

                return sLogonData;
                //}

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Error, ex.Message, ex);
                return null;
            }
        }

        #endregion

        #region "Private Methods"

        //private SAPConnection GetSAPConnection()
        //{
        //    Destination destination = new Destination();

        //    destination.AppServerHost = SAPConfiguration.IPAddress; //Configuration.IPAddress;10.168.15.24";//
        //    destination.Client = Convert.ToInt16(SAPConfiguration.ClientNumber); //Configuration.IntClientNum; 300;//
        //    destination.Password = SAPConfiguration.Password;
        //    destination.SystemNumber = Convert.ToInt16(SAPConfiguration.SystemNumber); //Configuration.SystemNumber;00;//
        //    destination.Username = SAPConfiguration.UserName; //Configuration.UserName;sisuis";//"
        //    SAPConnection connection = new SAPConnection(destination);

        //    return connection;
        //}


        private RfcDestination GetSAPDestination(int temp)
        {


            if (configParameters.Count == 0)
            {
                //            string decryptedPassword = RSAHelper.DecryptData(this.SAPConfiguration.Password);

                configParameters.Add(RfcConfigParameters.AppServerHost, "192.168.15.183");
                configParameters.Add(RfcConfigParameters.SystemNumber, "00");
                configParameters.Add(RfcConfigParameters.User, "sapuser");
                configParameters.Add(RfcConfigParameters.Password, "pass1234");
                configParameters.Add(RfcConfigParameters.Client, "800");
                configParameters.Add(RfcConfigParameters.Language, "EN");
                configParameters.Add(RfcConfigParameters.PoolSize, "5");
                configParameters.Add(RfcConfigParameters.Name, "SAPDestination");


                rfcSAPDestination = RfcDestinationManager.GetDestination(configParameters);


            }

            return rfcSAPDestination;
        }

        private RfcDestination GetSAPDestination()
        {


            if (configParameters.Count == 0)
            {
                string decryptedPassword = Configuration(EndpointKey.Password);

                configParameters.Add(RfcConfigParameters.AppServerHost, Configuration(EndpointKey.IPAddress));
                configParameters.Add(RfcConfigParameters.SystemNumber, Configuration(EndpointKey.SystemNumber));
                configParameters.Add(RfcConfigParameters.User, Configuration(EndpointKey.UserName));
                configParameters.Add(RfcConfigParameters.Password, decryptedPassword);
                configParameters.Add(RfcConfigParameters.Client, Configuration(EndpointKey.ClientNumber));
                configParameters.Add(RfcConfigParameters.Language, "EN");
                configParameters.Add(RfcConfigParameters.PoolSize, "5");
                configParameters.Add(RfcConfigParameters.Name, "SAPDestination");


                rfcSAPDestination = RfcDestinationManager.GetDestination(configParameters);


            }

            return rfcSAPDestination;
        }

        private void DiscoverUsers()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  User Objects at {0}", DateTime.Now));
            List<SAPUserinfo> listUsers = this.Getusers();
            SAPUserinfo.SaveUsers(listUsers, EndPoint.Id, MasterConfigHelper.ServiceUser);
            listUsers.Clear();
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  User Objects at {0}", DateTime.Now));
            listUsers = SAPUserinfo.GetDeletedByEndPointSystemId(this.EndPoint.Id, this.Process);
            foreach (var item in listUsers)
            {
                item.Save(MasterConfigHelper.ServiceUser);
                WriteDiscoveryLog(LogLevelType.Warning, string.Format("Reconciliation Exception: SAP User {0} has been REMOVED without any request made in the system", item.UserLogonName));
            }
            SAPUserinfo.RemoveSAPUser(listUsers);
            listUsers = null;
        }

        private void DiscoverRoles()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Roles  at {0}", DateTime.Now));
            List<SAPRoleInfo> lstRoles = GetRoles().ToList();
            SAPRoleInfo.SaveRoles(lstRoles, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstRoles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Roles  at {0}", DateTime.Now));
            //lstRoles = SAPFacade.GetDeletedSapRoles(this.EndPoint.EndpointSystemId);
            //SAPFacade.RemoveSapRoles(lstRoles);
            lstRoles = null;

            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  SAPCompositeRoles  at {0}", DateTime.Now));
            // IList<SAPCompositeRoleInfo> listSAPCompositeRoles = GetCompositeRoles();
            IList<DomainModel.SAPCompositeRoleInfo> listSAPCompsiteRoles = GetCompositeRoles();
            // SAPFacade.SaveSAPCompositeRoles(listSAPCompositeRoles, EndPoint.Id);

            //listSAPCompositeRoles = null;
            listSAPCompsiteRoles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  SAPCompositeRoles  at {0}", DateTime.Now));
            // listSAPCompositeRoles = SAPFacade.GetDeletedSAPCompositeRoleInfo(this.EndPoint.Id);
            listSAPCompsiteRoles = DomainModel.SAPCompositeRoleInfo.GetDeletedSAPCompositeRoleInfo(this.EndPoint.Id);
            DomainModel.SAPCompositeRoleInfo.DeleteList(listSAPCompsiteRoles.ToList());
            // SAPFacade.RemoveSAPCompositeRole(listSAPCompositeRoles);
            // listSAPCompositeRoles = null;
            listSAPCompsiteRoles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  SAPSingleRoles  at {0}", DateTime.Now));
            IList<SAPSingleRoleInfo> listSAPSingleRoles = GetChildRoles();
            SAPSingleRoleInfo.SaveSingleRoles(listSAPSingleRoles, EndPoint.Id, MasterConfigHelper.ServiceUser);
            listSAPSingleRoles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed  Discovering  SAPSingleRoles  at {0}", DateTime.Now));
            listSAPSingleRoles = SAPSingleRoleInfo.GetDeletedSAPSingleRole(this.EndPoint.Id);
            SAPSingleRoleInfo.DeleteList(listSAPSingleRoles.ToList());
            listSAPSingleRoles = null;
            this.UpdateSAPRoleType();
        }

        private void DiscoverTCodes()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Tcodes  at {0}", DateTime.Now));
            IList<SAPTCodeInfo> SAPTCodes = GetTcodes();
            //SAPFacade.SaveTcodes(SAPTCodes, EndPoint.EndpointSystemId);
            SAPTCodes = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed  Discovering  Tcodes  at {0}", DateTime.Now));
            SAPTCodes = SAPTCodeInfo.GetDeletedSAPTCode(this.EndPoint.Id);
            SAPTCodeInfo.DeleteList(SAPTCodes.ToList());
            SAPTCodes = null;
        }

        private void DiscoverParameters()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  SAPParameter  at {0}", DateTime.Now));
            IList<SAPParameterInfo> parameters = GetParameters();
            SAPParameterInfo.SaveParameterInfos(parameters.ToList(), EndPoint.Id, MasterConfigHelper.ServiceUser);
            parameters.Clear();
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed  Discovering  SAPParameter  at {0}", DateTime.Now));
            parameters = SAPParameterInfo.GetDeletedSAPParameters(this.EndPoint.Id);
            SAPParameterInfo.DeleteList(parameters.ToList());
            parameters = null;
        }

        private void DiscoverGroups()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Group Objects at {0}", DateTime.Now));
            IList<SAPGroupInfo> lstGroups = GetGroups();
            SAPGroupInfo.SaveGroups(lstGroups, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstGroups = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Group Objects at {0}", DateTime.Now));
            lstGroups = SAPGroupInfo.GetDeletedByEndPointSystemId(this.EndPoint.Id);
            SAPGroupInfo.DeleteList(lstGroups.ToList());
            lstGroups = null;
        }

        private void DiscoverProfiles()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Profiles at {0}", DateTime.Now));
            IList<SAPProfileInfo> profiles = GetProfiles();
            SAPProfileInfo.SaveProfiles(profiles, EndPoint.Id, MasterConfigHelper.ServiceUser);
            profiles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Profiles at {0}", DateTime.Now));
            profiles = SAPProfileInfo.GetDeletedProfiles(this.EndPoint.Id);
            SAPProfileInfo.DeleteList(profiles.ToList());
            profiles = null;

            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Composite Profiles at {0}", DateTime.Now));

            //IList<Model.SAP.SAPCompositeProfileInfo> CompositeProfiles = GetCompositeProfiles();
            IList<DomainModel.SAPCompositeProfileInfo> compositeProfile = GetCompositeProfiles();

            // SAPFacade.SaveCompositeProfiles(compositeProfile, EndPoint.Id);
            DomainModel.SAPCompositeProfileInfo.SaveCompositeProfiles(compositeProfile, EndPoint.Id, MasterConfigHelper.ServiceUser);
            compositeProfile = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Composite Profiles at {0}", DateTime.Now));
            // compositeProfile =   SAPFacade.GetDeletedCompositeProfiles(this.EndPoint.Id);
            compositeProfile = DomainModel.SAPCompositeProfileInfo.GetDeletedCompositeProfiles(this.EndPoint.Id);
            DomainModel.SAPCompositeProfileInfo.DeleteList(compositeProfile.ToList());
            //SAPFacade.RemoveSAPCompositeProfiles(CompositeProfiles);
            //CompositeProfiles = null;
            compositeProfile = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering Users Profiles at {0}", DateTime.Now));
            IList<SapUserProfile> UserProfiles = GetProfileUsers();
            SapUserProfile.SaveProfiles(UserProfiles, EndPoint.Id, MasterConfigHelper.ServiceUser);
            UserProfiles = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Users Profiles at {0}", DateTime.Now));

            //DILog.Discovery.WriteLog(LogType.INFO, string.Format("Started Discovering Roles Profiles at {0}", DateTime.Now));
            //IList<SapRoleProfileMapping> RoleProfiles = GetRoleProfile();
            //SAPFacade.SaveSAPRoleProfile(RoleProfiles, EndPoint.EndpointSystemId);
            //UserProfiles = null;
            //DILog.Discovery.WriteLog(LogType.INFO, string.Format("Completed Discovering Roles Profiles at {0}", DateTime.Now));

        }

        private void DiscoverAuthorizations()
        {
            #region Discovering Role To Authorization Mapping (AGR_1251)
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Role To Auth Mapping at {0}", DateTime.Now));
            SAPRoleToAuthMap.DeleteByEndpointSystemId(EndPoint.Id);
            IList<SAPRoleToAuthMap> lstSapRoleAuthMap = GetSapRoleToAuthMap();
            SAPRoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleAuthMap, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstSapRoleAuthMap = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Role To Auth Mapping at {0}", DateTime.Now));
            #endregion

            #region Authorization To Profile
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Authorization Objects at {0}", DateTime.Now));
            SAPAuthorizationProfile.DeleteAuthorizations(EndPoint.Id);
            IList<DomainModel.SAPAuthorizationProfile> lstAuthorizations = GetAuthorizations();
            //IList<Model.SAP.SAPAuthorizationProfile> lstAuthorizations = GetAuthorizations();
            // SAPFacade.SaveAuthorizations(lstAuthorizations, EndPoint.Id);
            DomainModel.SAPAuthorizationProfile.SaveAuthorizations(lstAuthorizations, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstAuthorizations = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Authorization Objects at {0}", DateTime.Now));
            #endregion

            #region Authorization To Object Mapping
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Field Objects at {0}", DateTime.Now));
            SAPAuthorizationToObject.DeleteSAPObjectsAuthMap(EndPoint.Id);
            IList<SAPAuthorizationToObject> lstObjects = GetSapObjectsAuthMap();
            SAPAuthorizationToObject.SaveSAPObjectsAuthMap(lstObjects, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstObjects = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Field Objects at {0}", DateTime.Now));
            #endregion

            #region Sap Field Value Mapping

            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Field Mapping Objects at {0}", DateTime.Now));
            IList<SAPFieldValueMapping> lstFieldValueMap = GetSapFieldValueMap();
            SAPFieldValueMapping.SaveSAPFieldValueMap(lstFieldValueMap, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstFieldValueMap = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Field Mapping Objects at {0}", DateTime.Now));
            #endregion

            //#region Sap Object Values
            //WriteLog(EndPoint, LogLevelType.Info, string.Format("Started Discovering  Object Values at {0}", DateTime.Now), DiscoveryObjectType.Field, 0);
            //IList<SAPObjectValues> lstObjectValues = GetSapObjectValues();
            //SAPFacade.SaveSAPObjectValue(lstObjectValues, EndPoint.EndpointSystemId);
            //lstObjectValues = null;
            //WriteLog(EndPoint, LogLevelType.Info, string.Format("Completed Discovering Object Values at {0}", DateTime.Now), DiscoveryObjectType.Field, 0);
            //#endregion

            #region Field Values
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  TCode Object Map at {0}", DateTime.Now));
            SAPTCodeObjectMap.DeleteSAPTCodeObjectMap(EndPoint.Id);
            IList<SAPTCodeObjectMap> lstTCodeObjectMap = GetSapTCodeObjectMap();
            SAPTCodeObjectMap.SaveSAPTCodeObjectMap(lstTCodeObjectMap, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstTCodeObjectMap = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering TCode Object Map at {0}", DateTime.Now));
            #endregion

            #region Object Code and Matching Field Code
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Sap Objects at {0}", DateTime.Now));
            IList<SAPObjectInfo> lstSapObjects = GetSapObjects();
            SAPObjectInfo.SaveSapObject(lstSapObjects, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstSapObjects = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Sap Objects at {0}", DateTime.Now));
            #endregion

            //#region SAP Activity Info
            //WriteLog(EndPoint, LogLevelType.Info, string.Format("Started Discovering  Sap Activity Info at {0}", DateTime.Now), DiscoveryObjectType.SAP, 0);
            //IList<SAPActivityInfo> lstSapActivityInfo = GetSapActivityInfo();
            //SAPFacade.SaveActivtyInfo(lstSapActivityInfo, EndPoint.EndpointSystemTypeId.Value);
            //lstSapActivityInfo = null;
            //WriteLog(EndPoint, LogLevelType.Info, string.Format("Completed Discovering  Sap Activity Info at {0}", DateTime.Now), DiscoveryObjectType.SAP, 0);
            //#endregion

        }

        private void DiscoverPositions()
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Position Objects at {0}", DateTime.Now));
            //RetriveAndSavePositions();
            this.DumpHRP1001();
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Position Objects at {0}", DateTime.Now));
        }

        private void DumpHRP1001()
        {
            try
            {
                this.PopulatePositionRoleMapping();
                this.PopulatePositionPersonMapping();
            }
            catch
            {
                throw;
            }
        }

        private void PopulatePositionRoleMapping()
        {
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;


            try
            {
                DataTable m_dtHRP1001 = new DataTable();
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                //FLD_OBJID.Fieldname = "OBJID";

                //RFC_DB_FLD FLD_BEGDA = new RFC_DB_FLD();
                //FLD_BEGDA.Fieldname = "BEGDA";

                //RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                //FLD_ENDDA.Fieldname = "ENDDA";

                //dtFlds.Add(FLD_OBJID);
                //dtFlds.Add(FLD_SOBID);
                //dtFlds.Add(FLD_BEGDA);
                //dtFlds.Add(FLD_ENDDA);

                filter = "SCLAS = 'AG' AND OTYPE = 'S' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";

                //dtopts.Add(filter);

                //dtopts.Add(filter);


                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SOBID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BEGDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");

                //dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                //proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                dtTemp = readTable.ToDataTable("HRP1001");
                dtFields = fieldTable.ToDataTable("FIELDS");
                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);

                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    SAPHRP1001 hr1001 = new SAPHRP1001()
                    {
                        EndpointSystemId = this.EndPoint.Id,
                        ActualObjectCode = row["OBJID"].ToString().Trim(),
                        RelatedObjectCode = row["SOBID"].ToString().Trim(),
                        BeginDate = row["BEGDA"].ToString().Trim(),
                        EndDate = row["ENDDA"].ToString().Trim(),
                        RelatedObjectType = "AG"
                    };

                    var mapping = SAPHRP1001.GetByActualObjectCode(hr1001);

                    if (mapping == null)
                    {
                        hr1001.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        mapping.BeginDate = hr1001.BeginDate;
                        mapping.EndDate = hr1001.EndDate;
                        hr1001.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error occurred while discovering position to role mapping.", ex);
            }
        }

        private void PopulatePositionPersonMapping()
        {
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                DataTable m_dtHRP1001 = new DataTable();
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_OBJID = new RFC_DB_FLD();
                //FLD_OBJID.Fieldname = "OBJID";

                //RFC_DB_FLD FLD_BEGDA = new RFC_DB_FLD();
                //FLD_BEGDA.Fieldname = "BEGDA";

                //RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                //FLD_ENDDA.Fieldname = "ENDDA";

                //dtFlds.Add(FLD_OBJID);
                //dtFlds.Add(FLD_SOBID);
                //dtFlds.Add(FLD_BEGDA);
                //dtFlds.Add(FLD_ENDDA);

                filter = "SCLAS = 'US' AND OTYPE = 'S' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";

                //dtopts.Add(filter);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SOBID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "OBJID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BEGDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");

                //dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtTemp = readTable.ToDataTable("HRP1001");
                dtFields = fieldTable.ToDataTable("FIELDS");


                //proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //DataTable dtTemp = dt.ToADODataTable();
                //DataTable dtFields = dtFlds.ToADODataTable();
                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);

                foreach (DataRow row in m_dtHRP1001.Rows)
                {

                    SAPHRP1001 hr1001 = new SAPHRP1001()
                    {
                        EndpointSystemId = this.EndPoint.Id,
                        ActualObjectCode = row["OBJID"].ToString().Trim(),
                        RelatedObjectCode = row["SOBID"].ToString().Trim(),
                        BeginDate = row["BEGDA"].ToString().Trim(),
                        EndDate = row["ENDDA"].ToString().Trim(),
                        RelatedObjectType = "US"
                    };

                    var mapping = SAPHRP1001.GetByActualObjectCode(hr1001);

                    if (mapping == null)
                    {
                        hr1001.Save(MasterConfigHelper.ServiceUser);
                    }
                    else
                    {
                        mapping.BeginDate = hr1001.BeginDate;
                        mapping.EndDate = hr1001.EndDate;
                        hr1001.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, "Error occurred while discovering position to person mapping.", ex);
            }
        }

        private IList<string> GetDeltaUsers()
        {
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            DataTable dtUsers = null;

            try
            {

                IList<string> lstSAPDeltaUsers = new List<string>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = "MODDATE >= '" + deltaDate + "' ";

                //dtopts.Add(filter);

                //proxy.Rfc_Read_Table("|", null, "USRSTAMP", 0, 0, ref dt, ref dtFlds, ref dtopts);

                //DataTable dtTemp = dt.ToADODataTable();
                //DataTable dtFields = dtFlds.ToADODataTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "USRSTAMP");
                readTableFunction.SetValue("DELIMITER", "|");


                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                fieldTable = readTableFunction.GetTable("FIELDS");

                dtTemp = readTable.ToDataTable("USRSTAMP");
                dtFields = fieldTable.ToDataTable("FIELDS");


                dtUsers = convertPipesToColumnsUsers(dtFields, dtTemp);

                for (int user = 0; user < dtUsers.Rows.Count; user++)
                {
                    string UserName = dtUsers.Rows[user]["USERNAME"].ToString();
                    lstSAPDeltaUsers.Add(UserName);
                }

                lstSAPDeltaUsers = lstSAPDeltaUsers.Distinct().ToList();
                return lstSAPDeltaUsers;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private IList<string> GetDeltaRoles()
        {
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //DataTable dtUsers = null;
                IList<string> lstSAPDeltaRoles = new List<string>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = "CHANGE_DAT >= '" + deltaDate + "' ";
                //dtopts.Add(filter);

                //proxy.Rfc_Read_Table("|", null, "AGR_DEFINE", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_DEFINE");
                readTableFunction.SetValue("DELIMITER", "|");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("AGR_DEFINE");
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                //SAPdestination = GetSAPDestination();
                //bapiRepository = SAPdestination.Repository;


                //readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                //readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                //readTableFunction.SetValue("DELIMITER", "|");

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role][0].ToString();
                    string[] SplitRole = fullRole.Split('|');
                    string RoleCode = SplitRole[1].ToString().Trim();

                    lstSAPDeltaRoles.Add(RoleCode);
                }

                lstSAPDeltaRoles = lstSAPDeltaRoles.Distinct().ToList();
                return lstSAPDeltaRoles;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private List<SAPUserinfo> Getusers()
        {

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable readTable;


            try
            {
                //Not Working
                //proxy.Bapi_User_Getlist(intmaxrows, "", out intRows, ref return0, ref sextable, ref gettable, ref userNametable);
                // DataTable dtUsers = userNametable.ToADODataTable();

                DataTable dtUsers = null;
                List<SAPUserinfo> lstSAPUserinfo = new List<SAPUserinfo>();

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_BNAME = new RFC_DB_FLD();
                //FLD_BNAME.Fieldname = "BNAME";
                //dtFlds.Add(FLD_BNAME);

                //proxy.Rfc_Read_Table("|", null, "USR01", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "USR01");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BNAME");

                //optionsTable = readTableFunction.GetTable("OPTIONS");
                //optionsTable.Append();
                //optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");



                dtTemp = readTable.ToDataTable("USR01");
                dtFields = fieldTable.ToDataTable("FIELDS");
                dtUsers = convertPipesToColumnsUsers(dtFields, dtTemp);



                for (int user = 0; user < dtUsers.Rows.Count; user++)
                {
                   
                    string UserName = dtUsers.Rows[user]["BNAME"].ToString();
                    bool DiscoverUserDetail = true;
                    string UserLogonName = dtUsers.Rows[user]["BNAME"].ToString();

                    if (!IsFirstTimeDiscovery)
                    {
                        DiscoverUserDetail = DeltaUsers.FirstOrDefault(usr => usr.Trim() == UserLogonName.Trim()) != null;
                    }

                    if (DiscoverUserDetail && UserLogonName.Trim() != "SAP*")
                    {
                        //DILog.Discovery.WriteLog(LogType.INFO, string.Format("Discovering {0}", UserLogonName));

                        //#region SAP BAPI CLASSES

                        //BAPIADDR3 address = new BAPIADDR3();
                        //BAPIUSCOMP companyName = new BAPIUSCOMP();
                        //BAPIALIAS alias = new BAPIALIAS();
                        //BAPIDEFAUL defaults = new BAPIDEFAUL();
                        //BAPISLOCKD islocked = new BAPISLOCKD();
                        //BAPILOGOND logonData = new BAPILOGOND();
                        //BAPIPWD passworddata = new BAPIPWD();
                        //BAPIMODDAT lastmofified = new BAPIMODDAT();
                        //BAPIREFUS refuser = new BAPIREFUS();
                        //BAPISNCU snc = new BAPISNCU();
                        //BAPIUCLASS iuclass = new BAPIUCLASS();
                        //BAPIAGRTable activeGroups = new BAPIAGRTable();
                        //BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                        //BAPIADFAXTable addfax = new BAPIADFAXTable();
                        //BAPIADPAGTable addpag = new BAPIADPAGTable();
                        //BAPIADPRTTable addprt = new BAPIADPRTTable();
                        //BAPIADRFCTable addrfc = new BAPIADRFCTable();
                        //BAPIADRMLTable addrml = new BAPIADRMLTable();
                        //BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                        //BAPIADSSFTable addssf = new BAPIADSSFTable();
                        //BAPIADTELTable addtel = new BAPIADTELTable();
                        //BAPIADTLXTable addtlx = new BAPIADTLXTable();
                        //BAPIADTTXTable addttx = new BAPIADTTXTable();
                        //BAPIADURITable adduri = new BAPIADURITable();
                        //BAPIADX400Table addx400 = new BAPIADX400Table();
                        //BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                        //BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                        //BAPIGROUPSTable groups = new BAPIGROUPSTable();
                        //BAPIPARAMTable parameter = new BAPIPARAMTable();
                        //BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                        //BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                        //BAPIPROFTable profiles = new BAPIPROFTable();
                        //BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                        //SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                        //BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                        //BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                        //BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                        //AGR_TCODESTable TCodes = new AGR_TCODESTable();
                        //AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                        //AGR_HIERTable HeirTable = new AGR_HIERTable();
                        //AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                        //AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                        //BAPILOGONX BAPILOGONX = new BAPILOGONX();
                        //BAPIPWDX BAPIPWDX = new BAPIPWDX();
                        //BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                        //BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                        //BAPIALIASX BAPIALIASX = new BAPIALIASX();
                        //BAPIFLAG BAPIFLAG = new BAPIFLAG();
                        //BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                        //BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                        //BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                        //BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                        //BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                        //BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                        //BAPISNCUX BAPISNCUX = new BAPISNCUX();
                        //BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                        //BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                        //SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                        //#endregion



                        //proxy.Bapi_User_Get_Detail(UserLogonName, UserLogonName, out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out  snc, out iuclass,
                        //                                                      ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
                        //                                            ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
                        //                                             ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
                        //                                             ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);


                        IRfcFunction getUserDetailFunction = bapiRepository.CreateFunction("BAPI_USER_GET_DETAIL");

                        getUserDetailFunction.SetValue("USERNAME", UserName);
                        getUserDetailFunction.Invoke(SAPdestination);

                        string UserAlias = getUserDetailFunction.GetStructure("ALIAS").GetValue("Useralias").ToString();

                        IRfcStructure sAddress = getUserDetailFunction.GetStructure("ADDRESS");
                        IRfcStructure sIsLocked = getUserDetailFunction.GetStructure("ISLOCKED");
                        IRfcStructure sDefaults = getUserDetailFunction.GetStructure("DEFAULTS");


                        IRfcStructure sLogonData = getUserDetailFunction.GetStructure("LOGONDATA");

                        //string Last_logon_date, user_id;
                        //byte Lock_status;
                        //int Password_status;
                        //string alias1 = alias.Useralias.ToString();
                        //proxy.Susr_Get_Admin_User_Login_Info(UserAlias, "", dtUsers.Rows[user]["BNAME"].ToString(), out Last_logon_date, out Lock_status, out Password_status, out user_id);


                        IRfcFunction getAdminUserLoginInfoFunction = bapiRepository.CreateFunction("SUSR_GET_ADMIN_USER_LOGIN_INFO");

                        getAdminUserLoginInfoFunction.SetValue("ALIASNAME", UserAlias);
                        getAdminUserLoginInfoFunction.SetValue("USERID", UserLogonName);

                        getAdminUserLoginInfoFunction.Invoke(SAPdestination);
                        string Last_logon_date = getAdminUserLoginInfoFunction.GetValue("LAST_LOGON_DATE").ToString();


                        DateTime? validto = null;

                        try
                        {
                            validto = DateTime.ParseExact(sLogonData.GetValue("Gltgb").ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        { }

                        DateTime? fromdate = null;
                        try
                        {
                            fromdate = DateTime.ParseExact(sLogonData.GetValue("Gltgv").ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        { }



                        //hh = logonData.Ltime.Substring(0, 2);
                        //mm = logonData.Ltime.Substring(2, 2);
                        //ss = logonData.Ltime.Substring(4, 2);

                        hh = sLogonData.GetString("LTime").Substring(0, 2);
                        mm = sLogonData.GetString("LTime").Substring(3, 2);
                        ss = sLogonData.GetString("LTime").Substring(6, 2);



                        string year = Last_logon_date.Substring(0, 4);
                        string month = Last_logon_date.Substring(4, 2);
                        string day = Last_logon_date.Substring(6, 2);
                        if (year == "0000")
                        {
                            strLastLogin = "";
                        }
                        else
                        {
                            strLastLogin = year + "/" + month + "/" + day + " " + hh.ToString() + ":" + mm.ToString() + ":" + ss.ToString();
                        }


                        SAPUserinfo UserInfo = new SAPUserinfo();
                        //  UserInfo.HistoryId = this.HistoryId;
                        UserInfo.Firstname = sAddress.GetValue("Firstname").ToString();
                        
                        UserInfo.EndPointSystemId = this.EndPoint.Id;
                        UserInfo.Lastname = sAddress.GetValue("Lastname").ToString();
                        UserInfo.Initials = sAddress.GetValue("Initials").ToString();
                        UserInfo.Department = sAddress.GetValue("Department").ToString();
                        UserInfo.E_Mail = sAddress.GetValue("E_Mail").ToString();
                        UserInfo.Pers_No = sAddress.GetValue("Pers_No").ToString();
                        UserInfo.Tel1_Numbr = sAddress.GetValue("Tel1_Ext").ToString();
                        UserInfo.Function0 = sAddress.GetValue("Function0").ToString();
                        UserInfo.Title = sAddress.GetValue("Title").ToString();
                        UserInfo.City = sAddress.GetValue("City").ToString();
                        UserInfo.UserLogonName = dtUsers.Rows[user]["BNAME"].ToString();
                        UserInfo.Namcountry = sAddress.GetValue("Country").ToString();
                        UserInfo.EMPLOYEEID = string.Empty;
                        UserInfo.Pboxcit_No = sAddress.GetValue("Pboxcit_No").ToString();
                        UserInfo.Postl_Cod1 = sAddress.GetValue("Postl_Cod1").ToString();
                        UserInfo.Street = sAddress.GetValue("Street").ToString();
                        UserInfo.Start_Menu = sDefaults.GetValue("Start_Menu").ToString();
                        UserInfo.SapUserType = sLogonData.GetValue("Ustyp").ToString();
                        //if (dtRoleCodes.Rows.Count > 0)
                        //    UserInfo.ActivityGroup = dtRoleCodes.Rows[0]["Agr_Name"].ToString();
                        //else
                        //    UserInfo.ActivityGroup = string.Empty;

                        UserInfo.AccntLockNoPwd = sIsLocked.GetValue("No_User_Pw").ToString();
                        UserInfo.AccntLocDurPwd = sIsLocked.GetValue("Wrng_Logon").ToString();
                        if (strLastLogin == string.Empty)
                            UserInfo.LastLoginTime = null;
                        else
                            UserInfo.LastLoginTime = Convert.ToDateTime(strLastLogin);
                       

                        if (fromdate != null)
                            UserInfo.validFromDate = fromdate;
                        if (validto != null)
                            UserInfo.validToDate = validto; //.Value.ToString("dd-MM-yyyy");
                        #region MyRegion
                        //  UserInfo.IsActive = true; 
                        #endregion
                        //UserInfo.IsActive = !(islocked.Glob_Lock == "L" || islocked.Local_Lock == "L");
                        bool IsUserLocked = (sIsLocked.GetValue("Glob_Lock").ToString() == "L" || sIsLocked.GetValue("Local_Lock").ToString() == "L");
                        UserInfo.IsUserLocked = IsUserLocked;
                        UserInfo.IsActive = true;
                        if (validto != null)
                            UserInfo.IsActive = validto.Value > DateTime.Now;
                        UserInfo.DomainName = "";
                        lstSAPUserinfo.Add(UserInfo);
                    }
                    else
                    {
                        var sapUserInfo = SAPUserinfo.GetByUserLogonName(UserLogonName, this.EndPoint.Id);
                        if (sapUserInfo != null)
                        {
                            lstSAPUserinfo.Add(sapUserInfo);
                        }
                    }

                    if (lstSAPUserinfo.Count() % 500 == 0 && lstSAPUserinfo.Count > 0)
                    {
                        SAPUserinfo.SaveUsers(lstSAPUserinfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstSAPUserinfo.Clear();
                    }

                    if (lstSAPUserinfo.Count() % 50 == 0 && lstSAPUserinfo.Count > 0)
                    {
                        //Thread.Sleep(2000);
                    }

                }

                return lstSAPUserinfo;
            }

            catch
            {
                return null;
            }

        }

        private void RetriveAndSavePositions()
        {
            List<SAPPosition> lstPositions = new List<SAPPosition>();

            SAPPosition posObj = null;

            DataTable dt_ALL_Positions = null;
            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable optionsTable;
            IRfcTable fieldsTable;
            IRfcTable readTable;
            string filter;

            SAPPosition.DeleteByEndpointSystemId(EndPoint.Id);
            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "T528T", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "T528T");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldsTable = readTableFunction.GetTable("FIELDS");

                //DataTable dtParameters = readTable.ToDataTable("T528T");


                DataTable dtPos = readTable.ToDataTable("T528T");
                dtFields = fieldsTable.ToDataTable("FIELDS");
                //DataTable dtotps = dtopts.ToADODataTable();
                DataTable dtPositions = this.convertPipesToColumnsGeneral(dtFields, dtPos);

                dt_ALL_Positions = dtPositions.AsEnumerable()
                              .Where(a => Convert.ToString(a["OTYPE"]).Trim() == "S" && Convert.ToString(a["SPRSL"]).Trim() == "E")
                             .CopyToDataTable();


                for (int posIndex = 0; posIndex < dt_ALL_Positions.Rows.Count; posIndex++)
                {
                    posObj = new SAPPosition();

                    posObj.EndPointSystemId = (int)EndPoint.Id;
                  
                    posObj.Code = dt_ALL_Positions.Rows[posIndex]["PLANS"].ToString();
                    posObj.Name = dt_ALL_Positions.Rows[posIndex]["PLSTX"].ToString();
                    posObj.BEGDA = DateTime.ParseExact(dt_ALL_Positions.Rows[posIndex]["BEGDA"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    posObj.ENDDA = DateTime.ParseExact(dt_ALL_Positions.Rows[posIndex]["ENDDA"].ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    posObj.Version = Convert.ToInt16(VersionNo.Available);
                    lstPositions.Add(posObj);

                    if (lstPositions.Count() % 500 == 0)
                    {
                        lstPositions.ForEach(l => l.Save(MasterConfigHelper.ServiceUser));
                        lstPositions.Clear();
                    }
                }
                lstPositions.ForEach(l => l.Save(MasterConfigHelper.ServiceUser));
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPProfileInfo> GetProfiles()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable optionsTable;
            IRfcTable fieldsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "USR11", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "USR11");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldsTable = readTableFunction.GetTable("FIELDS");

                //DataTable dtParameters = readTable.ToDataTable("T528T");


                DataTable dtProfilesAll = readTable.ToDataTable("USR11");
                DataTable dtFields = fieldsTable.ToDataTable("FIELDS");


                //DataTable dtProfilesAll = ();
                //DataTable abc = dtFlds.ToADODataTable();
                DataTable dtProfilesforfilter = convertPipesToColumnsProfiles(dtFields, dtProfilesAll);

                //NEW FILTER

                //DataTable dtProfiles = dtProfilesforfilter.AsEnumerable()
                //               .Where(a => Convert.ToString(a["LANGU"]).Trim() == "E")
                //              .CopyToDataTable();


                DataTable dtProfiles = dtProfilesforfilter.AsEnumerable()
                              .CopyToDataTable();

                dtProfilesAll = null;

                IList<SAPProfileInfo> lstProfileInfo = new List<SAPProfileInfo>();
                for (int profile = 0; profile < dtProfiles.Rows.Count; profile++)
                {
                    //string fullProfile = dtProfiles.Rows[profile]["Wa"].ToString();
                    //string[] SplitProfile = fullProfile.Split('|');
                    SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();
                    //SAPProfileInfo.PROFILE_CODE = SplitProfile[2].ToString();
                    //SAPProfileInfo.PROFILE_DESC = SplitProfile[4].ToString();
                    SAPProfileInfo.PROFILE_CODE = dtProfiles.Rows[profile]["PROFN"].ToString();
                    SAPProfileInfo.PROFILE_DESC = dtProfiles.Rows[profile]["PTEXT"].ToString();

                    //  SAPProfileInfo.HistoryId = this.HistoryId;
                    SAPProfileInfo.EndPointSystemId = EndPoint.Id;
                    SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);
                 
                    SAPProfileInfo.TargetSysId = null;
                    
                    lstProfileInfo.Add(SAPProfileInfo);

                    if (lstProfileInfo.Count() % 500 == 0)
                    {
                        SAPProfileInfo.SaveProfiles(lstProfileInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstProfileInfo.Clear();
                    }

                    if (lstProfileInfo.Count() % 50 == 0)
                    {
                        //Thread.Sleep(2000);
                    }
                }
                return lstProfileInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        // private IList<Model.SAP.SAPCompositeProfileInfo> GetCompositeProfiles()
        private IList<DomainModel.SAPCompositeProfileInfo> GetCompositeProfiles()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable optionsTable;
            IRfcTable fieldsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "UST10C", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "UST10C");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldsTable = readTableFunction.GetTable("FIELDS");

                //DataTable dtParameters = readTable.ToDataTable("T528T");


                DataTable dtProfilesAll = readTable.ToDataTable("UST10C");
                DataTable dtFields = fieldsTable.ToDataTable("FIELDS");



                //DataTable dtProfilesAll = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();
                DataTable dtProfilesforfilter = convertPipesToColumnsProfiles(dtFields, dtProfilesAll);

                //NEW FILTER

                DataTable dtProfiles = dtProfilesforfilter.AsEnumerable()
                               .Where(a => Convert.ToString(a["AKTPS"]).Trim() == "A")
                              .CopyToDataTable();
                dtProfilesAll = null;

                //IList<Model.SAP.SAPCompositeProfileInfo> lstProfileInfo = new List<Model.SAP.SAPCompositeProfileInfo>();
                IList<DomainModel.SAPCompositeProfileInfo> lstProfileInfo = new List<DomainModel.SAPCompositeProfileInfo>();
                for (int profile = 0; profile < dtProfiles.Rows.Count; profile++)
                {
                    //string fullProfile = dtProfiles.Rows[profile]["Wa"].ToString();
                    //string[] SplitProfile = fullProfile.Split('|');
                    //  Model.SAP.SAPCompositeProfileInfo SAPProfileInfo = new Model.SAP.SAPCompositeProfileInfo();
                    DomainModel.SAPCompositeProfileInfo SAPProfileInfo = new DomainModel.SAPCompositeProfileInfo();
                    //SAPProfileInfo.PROFILE_CODE = SplitProfile[2].ToString();
                    //SAPProfileInfo.PROFILE_DESC = SplitProfile[4].ToString();
                    SAPProfileInfo.PROFILE_CODE = dtProfiles.Rows[profile]["PROFN"].ToString();
                    //SAPProfileInfo.PROFILE_DESC = dtProfiles.Rows[profile]["PTEXT"].ToString();
                    SAPProfileInfo.SUBProfile_Code = dtProfiles.Rows[profile]["SUBPROF"].ToString();
                    //  SAPProfileInfo.HistoryId = this.HistoryId;
                    SAPProfileInfo.EndPointSystemId = (int)EndPoint.Id;
                    SAPProfileInfo.TargetSysId = null;
                    SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);
                    lstProfileInfo.Add(SAPProfileInfo);

                    if (lstProfileInfo.Count() % 500 == 0)
                    {
                        // SAPFacade.SaveCompositeProfiles(lstProfileInfo, EndPoint.Id);
                        DomainModel.SAPCompositeProfileInfo.SaveCompositeProfiles(lstProfileInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstProfileInfo.Clear();
                    }
                }
                return lstProfileInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPGroupInfo> GetGroups()
        {
            RfcRepository bapiRepository;
            IRfcFunction readGroupsFunction;
            IRfcTable optionsTable;
            IRfcTable fieldsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "USGRP", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //DataTable dtGroups = dt.ToADODataTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readGroupsFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readGroupsFunction.SetValue("QUERY_TABLE", "USGRP");
                readGroupsFunction.SetValue("DELIMITER", "|");

                readGroupsFunction.Invoke(SAPdestination);

                readTable = readGroupsFunction.GetTable("DATA");
                DataTable dtGroups = readTable.ToDataTable("USGRP");



                //TAB512Table dtDesc = new TAB512Table();
                //RFC_DB_FLDTable dtFldsDesc = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtoptsDesc = new RFC_DB_OPTTable();
                //proxy.Rfc_Read_Table("|", null, "USGRPT", 0, 0, ref dtDesc, ref dtFldsDesc, ref dtoptsDesc);
                //DataTable dtGroupsDesc = dtDesc.ToADODataTable();

                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();


                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readGroupsFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readGroupsFunction.SetValue("QUERY_TABLE", "USGRPT");
                readGroupsFunction.SetValue("DELIMITER", "|");

                readGroupsFunction.Invoke(SAPdestination);

                readTable = readGroupsFunction.GetTable("DATA");



                DataTable dtGroupsDesc = readTable.ToDataTable("USGRPT");


                IList<SAPGroupInfo> lstGroupInfo = new List<SAPGroupInfo>();
                for (int group = 0; group < dtGroups.Rows.Count; group++)
                {
                    string fullGroup = dtGroups.Rows[group]["Wa"].ToString();
                    string[] SplitGroup = fullGroup.Split('|');
                    SAPGroupInfo SAPGroupInfo = new SAPGroupInfo();
                    //SAPGroupInfo.HistoryId = this.HistoryId;
                    SAPGroupInfo.GROUP_CODE = SplitGroup[1].ToString().Trim();

                    for (int groupdetl = 0; groupdetl < dtGroupsDesc.Rows.Count; groupdetl++)
                    {
                        string fullGroupDetl = dtGroupsDesc.Rows[groupdetl]["Wa"].ToString();
                        string[] SplitGroup2 = fullGroupDetl.Split('|');
                        if (SplitGroup2[2].ToString().Trim() == SAPGroupInfo.GROUP_CODE.Trim())
                        {
                            SAPGroupInfo.GROUP_DESC = SplitGroup2[3].ToString().Trim();
                        }
                    }
                    SAPGroupInfo.EndPointSystemId = EndPoint.Id;
                    SAPGroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                    SAPGroupInfo.TargetSysId = null;
                    lstGroupInfo.Add(SAPGroupInfo);
                }
                return lstGroupInfo;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPRoleInfo> GetRoles()
        {
            RfcRepository bapiRepository;
            IRfcFunction readAGR_DEFINEFunction;
            IRfcTable optionsTable;
            IRfcTable fieldsTable;
            IRfcTable readTable;
            string filter;



            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                if (!IsFirstTimeDiscovery)
                {
                    //RFC_DB_OPT filter = new RFC_DB_OPT();
                    filter = "CHANGE_DAT >= '" + deltaDate + "' ";
                    filter += "OR CREATE_DAT >= '" + deltaDate + "' ";
                    //dtopts.Add(filter);
                }
                else
                {
                    filter = string.Empty;
                }

                //proxy.Rfc_Read_Table("|", null, "AGR_DEFINE", 0, 0, ref dt, ref dtFlds, ref dtopts);

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readAGR_DEFINEFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readAGR_DEFINEFunction.SetValue("QUERY_TABLE", "AGR_DEFINE");
                readAGR_DEFINEFunction.SetValue("DELIMITER", "|");


                //dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readAGR_DEFINEFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readAGR_DEFINEFunction.Invoke(SAPdestination);

                readTable = readAGR_DEFINEFunction.GetTable("DATA");

                //dtTemp = readTable.ToDataTable("AGR_DEFINE");
                //dtFields = fieldTable.ToDataTable("FIELDS");

                DataTable dtRoles = readTable.ToDataTable("AGR_DEFINE");

                //TAB512Table dtRolest = new TAB512Table();
                //RFC_DB_FLDTable dtRoleFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtRoleopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "AGR_TEXTS", 0, 0, ref dtRolest, ref dtRoleFlds, ref dtRoleopts);


                IRfcFunction readAGR_TEXTSFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readAGR_TEXTSFunction.SetValue("QUERY_TABLE", "AGR_TEXTS");
                readAGR_TEXTSFunction.SetValue("DELIMITER", "|");


                //dtFields = fieldTable.ToDataTable("FIELDS");

                //optionsTable = readAGR_DEFINEFunction.GetTable("OPTIONS");
                //optionsTable.Append();
                //optionsTable.SetValue("TEXT", filter);

                readAGR_TEXTSFunction.Invoke(SAPdestination);

                readTable = readAGR_TEXTSFunction.GetTable("DATA");
                fieldsTable = readAGR_TEXTSFunction.GetTable("FIELDS");

                DataTable dtRolesWithDesc = readTable.ToDataTable("AGR_TEXTS"); //dtRolest.ToADODataTable();
                DataTable dtRoletFields = fieldsTable.ToDataTable("FIELDS");
                //DataTable dtRoletotps = dtRoleopts.ToADODataTable();
                DataTable DtAllRoleDesc = this.convertPipesToColumnsRoles(dtRoletFields, dtRolesWithDesc);


                DataTable DtRoleDesc = DtAllRoleDesc.AsEnumerable()
                            .Where(a => Convert.ToString(a["SPRAS"]).Trim() == "E")
                           .CopyToDataTable();

                DtAllRoleDesc = null;


                IList<SAPRoleInfo> lstRoleInfo = new List<SAPRoleInfo>();
                string Role_Desc = "";
                for (int Role = 0; Role < dtRoles.Rows.Count; Role++)
                {
                    string fullRole = dtRoles.Rows[Role]["Wa"].ToString();
                    string[] SplitRole = fullRole.Split('|');
                    SAPRoleInfo SAPRoleInfo = new SAPRoleInfo();
                    DataRow[] drRoles = DtRoleDesc.Select("AGR_NAME='" + SplitRole[1].ToString().Trim() + "'");
                    if (null != drRoles && drRoles.Count() > 0)
                    {
                        Role_Desc = CheckNull.AgainstString(drRoles[0]["TEXT"]).Trim();
                    }
                    else
                    {
                        Role_Desc = string.Empty;
                    }
                    SAPRoleInfo.ROLE_CODE = SplitRole[1].ToString().Trim();
                    SAPRoleInfo.ROLE_DESC = Role_Desc;
                    SAPRoleInfo.EndPointSystemId = EndPoint.Id;
                    SAPRoleInfo.TargetSysId = null;
                    SAPRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                    lstRoleInfo.Add(SAPRoleInfo);

                    if (lstRoleInfo.Count() % 500 == 0)
                    {
                        SAPRoleInfo.SaveRoles(lstRoleInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstRoleInfo.Clear();
                    }
                    if (lstRoleInfo.Count() % 50 == 0)
                    {
                        Thread.Sleep(100);
                    }
                }
                return lstRoleInfo;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPTCodeInfo> GetTcodes()
        {
            IList<SAPTCodeInfo> listTcodes = new List<SAPTCodeInfo>();
            long totalBytesOfMemoryUsed = 0;
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //TAB512Table dtTcodesdes = new TAB512Table();
                //RFC_DB_FLDTable dtTFlds1 = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtTopts1 = new RFC_DB_OPTTable();




                int rCount = 500;
                int rSkip = 0;

                //DataTable dtTCodesWithDesc = dtTcodesdes.ToADODataTable();
                DataTable dtTCodeFields = null;
                DataTable dtTcodetotps = null;
                DataTable DtTCodes = null;
                SAPTCodeInfo SapTCodeInfo = null;

                bool bStop = false;

                //RFC_DB_FLDTable dtTFlds2 = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtTopts2 = new RFC_DB_OPTTable();

                while (!bStop)
                {
                    //dtTcodesdes = new TAB512Table();

                    //proxy.Rfc_Read_Table("|", null, "TSTC", rCount, rSkip, ref dtTcodesdes, ref dtTFlds1, ref dtTopts1);

                    SAPdestination = GetSAPDestination();
                    bapiRepository = SAPdestination.Repository;


                    readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                    readTableFunction.SetValue("QUERY_TABLE", "TSTC");
                    readTableFunction.SetValue("DELIMITER", "|");


                    rSkip += rCount;

                    readTableFunction.Invoke(SAPdestination);

                    readTable = readTableFunction.GetTable("DATA");
                    fieldTable = readTableFunction.GetTable("FIELDS");
                    //dtTemp = readTable.ToDataTable("TSTC");
                    //dtFields = fieldTable.ToDataTable("FIELDS");

                    DataTable dtTcodesdes = readTable.ToDataTable("TSTC");
                    if (dtTcodesdes.Rows.Count == 0)
                        bStop = true;

                    dtTCodeFields = fieldTable.ToDataTable("FIELDS");
                    //dtTcodetotps = dtTopts1.ToADODataTable();
                    //DtTCodes = this.convertPipesToColumnsTCodes(dtTCodeFields, dtTcodesdes.ToADODataTable());
                    DtTCodes = this.convertPipesToColumnsTCodes(dtTCodeFields, dtTcodesdes);

                    dtTCodeFields = null;
                    dtTcodetotps = null;
                    listTcodes = new List<SAPTCodeInfo>();

                    for (int TCode = 0; TCode < DtTCodes.Rows.Count; TCode++)
                    {
                        SapTCodeInfo = new SAPTCodeInfo();

                        SapTCodeInfo.TCODE_Code = DtTCodes.Rows[TCode]["TCODE"].ToString().Trim();

                        //dtTFlds2 = new RFC_DB_FLDTable();
                        //dtTopts2 = new RFC_DB_OPTTable();
                        //TAB512Table dtTcodesdescription = new TAB512Table();

                        //RFC_DB_OPT clause1 = new RFC_DB_OPT();
                        filter = string.Format("SPRSL EQ 'EN' AND TCODE EQ '{0}'", SapTCodeInfo.TCODE_Code);
                        //dtTopts2.Add(clause1);

                        readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                        readTableFunction.SetValue("QUERY_TABLE", "TSTCT");
                        readTableFunction.SetValue("DELIMITER", "|");

                        optionsTable = readTableFunction.GetTable("OPTIONS");
                        optionsTable.Append();
                        optionsTable.SetValue("TEXT", filter);

                        readTableFunction.Invoke(SAPdestination);

                        readTable = readTableFunction.GetTable("DATA");


                        //proxy.Rfc_Read_Table("|", null, "TSTCT", 0, 0, ref dtTcodesdescription, ref dtTFlds2, ref dtTopts2);

                        DataTable TCodeDescription = readTable.ToDataTable("TSTCT");
                        if (TCodeDescription.Rows.Count > 0)
                        {
                            string fullProfile = TCodeDescription.Rows[0]["Wa"].ToString();
                            string[] SplitProfile = fullProfile.Split('|');
                            SapTCodeInfo.TCODE_Desc = SplitProfile[2];
                        }

                        SapTCodeInfo.EndPointSystemId = (int)EndPoint.Id;
                        SapTCodeInfo.Version = Convert.ToInt16(VersionNo.Available);

                        listTcodes.Add(SapTCodeInfo);

                        if (listTcodes.Count() % 50 == 0)
                        {
                            //Thread.Sleep(2000);
                        }
                    }

                    SAPTCodeInfo.SaveTcodes(listTcodes, EndPoint.Id, MasterConfigHelper.ServiceUser);
                    listTcodes.Clear();
                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, ex.Message + totalBytesOfMemoryUsed.ToString(), ex);
            }
            return listTcodes;
        }

        private IList<SapRoleTCodeMapping> GetRoleTCodeMapping()
        {
            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            DataTable dtTemp = new DataTable();
            DataTable dtFields = new DataTable();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            IList<SapRoleTCodeMapping> lstRoleTCodeMapping = new List<SapRoleTCodeMapping>();

            try
            {
                //proxy.Rfc_Read_Table("|", null, "AGR_TCODES", 0, 0, ref dt, ref dtFlds, ref dtopts);
                DataTable dtParameters;
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_TCODES");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                dtParameters = readTable.ToDataTable("AGR_TCODES");
                //dtFields = fieldTable.ToDataTable("FIELDS");


                for (int RoleTCode = 0; RoleTCode < dtParameters.Rows.Count; RoleTCode++)
                {
                    string fullRole = dtParameters.Rows[RoleTCode]["Wa"].ToString().Trim();

                    string[] SplitRole = fullRole.Split('|');

                    string RoleCode = SplitRole[1].ToString().Trim();
                    string TCode = SplitRole[3].ToString().Trim();


                    SapRoleTCodeMapping objSapSingleRoleTcodeMap = new SapRoleTCodeMapping();
                    objSapSingleRoleTcodeMap.TCode = TCode;
                    objSapSingleRoleTcodeMap.RoleCode = RoleCode;
                    objSapSingleRoleTcodeMap.EndPointSystemId = (int)this.EndPoint.Id;
                    objSapSingleRoleTcodeMap.Version = Convert.ToInt16(VersionNo.Available);
                    lstRoleTCodeMapping.Add(objSapSingleRoleTcodeMap);
                }

            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(this.CurrentRequest.Id, LogLevelType.Exception, ex.Message, ex);
            }
            return lstRoleTCodeMapping;
        }

        private IList<SAPParameterInfo> GetParameters()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;


            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "TPARA", 0, 0, ref dt, ref dtFlds, ref dtopts);
                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "TPARA");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("TPARA");

                //DataTable dtotps = dtopts.ToADODataTable();
                SAPParameterInfo.DeleteByEndpointSystemId(EndPoint.Id);
                IList<SAPParameterInfo> lstParameterInfo = new List<SAPParameterInfo>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPParameterInfo SAPParameter = new SAPParameterInfo();
                    SAPParameter.PARAM_CODE = SplitRole[0].ToString().Trim();
                    SAPParameter.PARAM_DESC = SplitRole[1].ToString().Trim();
                    SAPParameter.Version = Convert.ToInt16(VersionNo.Available);
                    SAPParameter.EndPointSystemId = (int)EndPoint.Id;
                    SAPParameter.TargetSysId = null;
                    
                    lstParameterInfo.Add(SAPParameter);
                    if (lstParameterInfo.Count() % 500 == 0)
                    {
                        DI.Chakra.DomainModel.SAPParameterInfo.SaveParameterInfos(lstParameterInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstParameterInfo.Clear();
                    }
                }
                return lstParameterInfo;
            }
            catch
            {
                throw;
            }
        }

        //private IList<Model.SAP.SAPAuthorizationProfile> GetAuthorizations()
        private IList<DomainModel.SAPAuthorizationProfile> GetAuthorizations()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "UST10S");
                readTableFunction.SetValue("DELIMITER", "|");

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "UST10S", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("UST10S");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                //  IList<Model.SAP.SAPAuthorizationProfile> lstAuthorizationInfo = new List<Model.SAP.SAPAuthorizationProfile>();
                IList<DomainModel.SAPAuthorizationProfile> lstAuthorizationInfo = new List<DomainModel.SAPAuthorizationProfile>();
                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    // Model.SAP.SAPAuthorizationProfile SAPAuthorize = new Model.SAP.SAPAuthorizationProfile();
                    DomainModel.SAPAuthorizationProfile SAPAuthorize = new DomainModel.SAPAuthorizationProfile();
                    SAPAuthorize.ProfileCode = SplitRole[1].ToString().Trim();
                    SAPAuthorize.ObjectCode = SplitRole[3].ToString().Trim();
                    SAPAuthorize.AuthorizationCode = SplitRole[4].ToString().Trim();
                    SAPAuthorize.EndPointSystemId = (int)EndPoint.Id;
                    SAPAuthorize.Version = Convert.ToInt16(VersionNo.Available);
                    lstAuthorizationInfo.Add(SAPAuthorize);
                    if (Role % 1000 == 0)
                    {
                        // SAPFacade.SaveAuthorizations(lstAuthorizationInfo, EndPoint.Id);
                        DomainModel.SAPAuthorizationProfile.SaveAuthorizations(lstAuthorizationInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        // lstAuthorizationInfo = new List<Model.SAP.SAPAuthorizationProfile>();
                        lstAuthorizationInfo = new List<DomainModel.SAPAuthorizationProfile>();
                    }
                }
                return lstAuthorizationInfo;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPTCodeObjectMap> GetSapTCodeObjectMap()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "USOBT_C");
                readTableFunction.SetValue("DELIMITER", "|");

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //proxy.Rfc_Read_Table("|", null, "USOBT_C", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("USOBT_C");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPTCodeObjectMap> lstSapTCodeObjects = new List<SAPTCodeObjectMap>();


                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');

                    SAPTCodeObjectMap TCodeObjectMap = new SAPTCodeObjectMap();

                    TCodeObjectMap.ObjectType = SplitRole[1].ToString().Trim();
                    TCodeObjectMap.ObjectCode = SplitRole[2].ToString().Trim();
                    TCodeObjectMap.FieldCode = SplitRole[3].ToString().Trim();
                    TCodeObjectMap.FieldValue = SplitRole[4].ToString().Trim();
                    TCodeObjectMap.EndPointSystemId = (int)EndPoint.Id;
                    TCodeObjectMap.Version = Convert.ToInt16(VersionNo.Available);

                    lstSapTCodeObjects.Add(TCodeObjectMap);
                    if (Role % 1000 == 0)
                    {
                        SAPTCodeObjectMap.SaveSAPTCodeObjectMap(lstSapTCodeObjects, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstSapTCodeObjects = new List<SAPTCodeObjectMap>();
                    }
                }
                return lstSapTCodeObjects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPAuthorizationToObject> GetSapObjectsAuthMap()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "UST12");
                readTableFunction.SetValue("DELIMITER", "|");

                //proxy.Rfc_Read_Table("|", null, "UST12", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("UST12");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPAuthorizationToObject> lstSapObjects = new List<SAPAuthorizationToObject>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPAuthorizationToObject SapAuthorizeToObject = new SAPAuthorizationToObject();
                    SapAuthorizeToObject.ObjectCode = SplitRole[1].ToString().Trim();
                    SapAuthorizeToObject.AuthorizationCode = SplitRole[2].ToString().Trim();
                    SapAuthorizeToObject.FieldCode = SplitRole[4].ToString().Trim();
                    SapAuthorizeToObject.FromValue = SplitRole[5].ToString().Trim();
                    SapAuthorizeToObject.ToValue = SplitRole[6].ToString().Trim();

                    SapAuthorizeToObject.EndPointSystemId = (int)EndPoint.Id;
                    SapAuthorizeToObject.Version = Convert.ToInt16(VersionNo.Available);
                    lstSapObjects.Add(SapAuthorizeToObject);

                    if (Role % 1000 == 0)
                    {
                        SAPAuthorizationToObject.SaveSAPObjectsAuthMap(lstSapObjects, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstSapObjects = new List<SAPAuthorizationToObject>();
                    }
                }
                return lstSapObjects;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPObjectValues> GetSapObjectValues()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "USOBX_C");
                readTableFunction.SetValue("DELIMITER", "|");


                //proxy.Rfc_Read_Table("|", null, "USOBX_C", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("USOBX_C");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPObjectValues> lstSapObjects = new List<SAPObjectValues>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPObjectValues sapObject = new SAPObjectValues();
                    sapObject.SapObjectType = SplitRole[1].ToString().Trim();
                    sapObject.ModificationID = SplitRole[7].ToString().Trim();
                    sapObject.SapOkFlag = SplitRole[6].ToString().Trim();
                    sapObject.SapObjectCode = SplitRole[2].ToString().Trim();

                    sapObject.EndPointSystemId = (int)EndPoint.Id;
                    sapObject.Version = Convert.ToInt16(VersionNo.Available);
                    lstSapObjects.Add(sapObject);
                }

                return lstSapObjects;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPRoleToAuthMap> GetSapRoleToAuthMap()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_1251");
                readTableFunction.SetValue("DELIMITER", "|");

                //proxy.Rfc_Read_Table("|", null, "AGR_1251", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("AGR_1251");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPRoleToAuthMap> lstSapRoleToAuthMap = new List<SAPRoleToAuthMap>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPRoleToAuthMap sapRoleToAuthMap = new SAPRoleToAuthMap();

                    sapRoleToAuthMap.RoleName = SplitRole[1].ToString().Trim();
                    sapRoleToAuthMap.BiwMenuCounter = SplitRole[2].ToString().Trim();
                    sapRoleToAuthMap.ObjectCode = SplitRole[3].ToString().Trim();
                    sapRoleToAuthMap.AuthCode = SplitRole[4].ToString().Trim();
                    sapRoleToAuthMap.ProfileGeneratorVariant = SplitRole[10].ToString().Trim();
                    sapRoleToAuthMap.FieldCode = SplitRole[6].ToString().Trim();
                    sapRoleToAuthMap.FromValue = SplitRole[7].ToString().Trim();
                    sapRoleToAuthMap.ToValue = SplitRole[8].ToString().Trim();
                    sapRoleToAuthMap.Version = Convert.ToInt16(VersionNo.Available);
                    sapRoleToAuthMap.EndPointSystemId = (int)EndPoint.Id;
                    lstSapRoleToAuthMap.Add(sapRoleToAuthMap);

                    if (Role % 1000 == 0)
                    {
                        SAPRoleToAuthMap.SaveSapRoleAuthMap(lstSapRoleToAuthMap, (int)EndPoint.Id, MasterConfigHelper.ServiceUser);
                        lstSapRoleToAuthMap = new List<SAPRoleToAuthMap>();
                    }
                }

                return lstSapRoleToAuthMap;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPObjectInfo> GetSapObjects()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();


                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "TOBJ");

                //proxy.Rfc_Read_Table("|", null, "TOBJ", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("TOBJ");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPObjectInfo> lstSapObjects = new List<SAPObjectInfo>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPObjectInfo sapObject = new SAPObjectInfo();

                    sapObject.SapObjectCode = SplitRole[0].ToString().Trim();
                    sapObject.FieldCode1 = SplitRole[1].ToString().Trim();
                    sapObject.FieldCode2 = SplitRole[2].ToString().Trim();
                    sapObject.FieldCode3 = SplitRole[3].ToString().Trim();
                    sapObject.FieldCode4 = SplitRole[4].ToString().Trim();
                    sapObject.FieldCode5 = SplitRole[5].ToString().Trim();
                    sapObject.FieldCode6 = SplitRole[6].ToString().Trim();
                    sapObject.FieldCode7 = SplitRole[7].ToString().Trim();
                    sapObject.FieldCode8 = SplitRole[8].ToString().Trim();
                    sapObject.FieldCode9 = SplitRole[9].ToString().Trim();
                    sapObject.FieldCode0 = SplitRole[10].ToString().Trim();

                    sapObject.EndPointSystemId = (int)EndPoint.Id;
                    sapObject.Version = Convert.ToInt16(VersionNo.Available);
                    lstSapObjects.Add(sapObject);
                }

                return lstSapObjects;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPActivityInfo> GetSapActivityInfo()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_OPT clause1 = new RFC_DB_OPT();

                filter = string.Format("SPRAS EQ 'EN'");

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("HRP1001");

                //proxy.Rfc_Read_Table("|", null, "TACTT", 0, 0, ref dt, ref dtFlds, ref dtopts);

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPActivityInfo> lstSapActivty = new List<SAPActivityInfo>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRow = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRow = fullRow.Split('|');
                    SAPActivityInfo sapActivity = new SAPActivityInfo();
                    sapActivity.ActivityValue = SplitRow[1].ToString().Trim();
                    sapActivity.ActivityDescription = SplitRow[2].ToString().Trim();
                    sapActivity.EndPointSystemId = (int)EndPoint.Id;
                    sapActivity.Version = Convert.ToInt16(VersionNo.Available);
                    lstSapActivty.Add(sapActivity);
                }

                return lstSapActivty;
            }
            catch
            {
                throw;
            }
        }

        private IList<SAPFieldValueMapping> GetSapFieldValueMap()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_1252");

                //proxy.Rfc_Read_Table("|", null, "AGR_1252", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("AGR_1252");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                IList<SAPFieldValueMapping> lstSapFieldValueMapping = new List<SAPFieldValueMapping>();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SAPFieldValueMapping SapFieldValueMapping = new SAPFieldValueMapping();
                    SapFieldValueMapping.RoleCode = SplitRole[1].ToString().Trim();
                    SapFieldValueMapping.FieldCode = SplitRole[3].ToString().Trim();
                    SapFieldValueMapping.FromValue = SplitRole[4].ToString().Trim();
                    SapFieldValueMapping.ToValue = SplitRole[5].ToString().Trim();

                    SapFieldValueMapping.EndPointSystemId = (int)EndPoint.Id;
                    SapFieldValueMapping.Version = Convert.ToInt16(VersionNo.Available);
                    lstSapFieldValueMapping.Add(SapFieldValueMapping);
                }
                return lstSapFieldValueMapping;

            }
            catch
            {
                throw;
            }
        }

        private void DiscoverMappingTables(bool IsUserSelected, bool IsRoleSelected, bool IsProfileSelected, bool IsGroupSelected, bool IsTCodeSelected)
        {
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Mapping Tables at {0}", DateTime.Now));

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;


            IList<SAPUserinfo> listDBSAPUsers = null;
            DataTable dtSAPUserGroups = null;
            DataTable dtSAPUserProfiles = null;
            DataTable dtSAPUserRoleCodes = null;
            listDBSAPUsers = new List<SAPUserinfo>();
            listDBSAPUsers = SAPUserinfo.GetSAPUserByEndpointSystemID(EndPoint.Id);
            listUserGroup = new List<SAPUserinfo.SAPUserGroup>();
            listUserProfile = new List<SAPUserinfo.SAPUserProfile>();
            listUserRole = new List<SAPUserinfo.SAPUserRole>();
            listUserParameter = new List<SAPUserinfo.SAPUserParameter>();
            listSingleRoleTcodes = new List<SAPUserinfo.SapSingleRoleTcodes>();

            SapRoles = (List<SAPRoleInfo>)SAPRoleInfo.GetByEndpointSystemId(EndPoint.Id);
            SapProfiles = (List<SAPProfileInfo>)SAPProfileInfo.GetByEndpointSystemId(EndPoint.Id);

            SAPdestination = GetSAPDestination();
            bapiRepository = SAPdestination.Repository;



            if (!IsFirstTimeDiscovery)
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                filter = "CHANGE_DAT >= '" + deltaDate + "' ";
                //dtopts.Add(filter);

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_USERS");
                readTableFunction.SetValue("DELIMITER", "|");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("HRP1001");


                //proxy.Rfc_Read_Table("|", null, "AGR_USERS", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();

                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role][0].ToString();
                    string[] SplitRole = fullRole.Split('|');
                    string UserLogonName = SplitRole[2].ToString().Trim();
                    if (DeltaUsers.FirstOrDefault(deltausr => deltausr.Trim() == UserLogonName) == null)
                    {
                        DeltaUsers.Add(UserLogonName);
                    }
                }
            }

            if (IsFirstTimeDiscovery)
            {
                if (IsGroupSelected)
                    SAPUserinfo.DeleteSAPUserGroupMap(this.EndPoint.Id);

                if (IsProfileSelected)
                    SAPUserinfo.DeleteSAPUserProfileMap(this.EndPoint.Id);

                if (IsRoleSelected)
                    SAPUserinfo.DeleteSAPUserRoleMap(this.EndPoint.Id);
            }

            int Counter = 0;


            try
            {
                foreach (SAPUserinfo user in listDBSAPUsers)
                {
                    bool DiscoverUserDetail = true;

                    if (!IsFirstTimeDiscovery)
                    {
                        DiscoverUserDetail = DeltaUsers.FirstOrDefault(usr => usr.Trim() == user.UserLogonName.Trim()) != null;
                    }
                    if (DiscoverUserDetail && user.UserLogonName.Trim() != "SAP*")
                    {

                        //#region SAP BAPI CLASSES

                        //BAPIADDR3 address = new BAPIADDR3();
                        //BAPIUSCOMP companyName = new BAPIUSCOMP();
                        //BAPIALIAS alias = new BAPIALIAS();
                        //BAPIDEFAUL defaults = new BAPIDEFAUL();
                        //BAPISLOCKD islocked = new BAPISLOCKD();
                        //BAPILOGOND logonData = new BAPILOGOND();
                        //BAPIPWD passworddata = new BAPIPWD();
                        //BAPIMODDAT lastmofified = new BAPIMODDAT();
                        //BAPIREFUS refuser = new BAPIREFUS();
                        //BAPISNCU snc = new BAPISNCU();
                        //BAPIUCLASS iuclass = new BAPIUCLASS();
                        //BAPIAGRTable activeGroups = new BAPIAGRTable();
                        //BAPICOMREMTable addcomrem = new BAPICOMREMTable();
                        //BAPIADFAXTable addfax = new BAPIADFAXTable();
                        //BAPIADPAGTable addpag = new BAPIADPAGTable();
                        //BAPIADPRTTable addprt = new BAPIADPRTTable();
                        //BAPIADRFCTable addrfc = new BAPIADRFCTable();
                        //BAPIADRMLTable addrml = new BAPIADRMLTable();
                        //BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
                        //BAPIADSSFTable addssf = new BAPIADSSFTable();
                        //BAPIADTELTable addtel = new BAPIADTELTable();
                        //BAPIADTLXTable addtlx = new BAPIADTLXTable();
                        //BAPIADTTXTable addttx = new BAPIADTTXTable();
                        //BAPIADURITable adduri = new BAPIADURITable();
                        //BAPIADX400Table addx400 = new BAPIADX400Table();
                        //BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
                        //BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

                        //BAPIGROUPSTable groups = new BAPIGROUPSTable();
                        //BAPIPARAMTable parameter = new BAPIPARAMTable();
                        //BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
                        //BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
                        //BAPIPROFTable profiles = new BAPIPROFTable();
                        //BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
                        //SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
                        //BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
                        //BAPIUSSRGETable gettable = new BAPIUSSRGETable();
                        //BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

                        //AGR_TCODESTable TCodes = new AGR_TCODESTable();
                        //AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
                        //AGR_HIERTable HeirTable = new AGR_HIERTable();
                        //AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
                        //AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

                        //BAPILOGONX BAPILOGONX = new BAPILOGONX();
                        //BAPIPWDX BAPIPWDX = new BAPIPWDX();
                        //BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
                        //BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
                        //BAPIALIASX BAPIALIASX = new BAPIALIASX();
                        //BAPIFLAG BAPIFLAG = new BAPIFLAG();
                        //BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
                        //BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
                        //BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
                        //BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
                        //BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
                        //BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
                        //BAPISNCUX BAPISNCUX = new BAPISNCUX();
                        //BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
                        //BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

                        //SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

                        //#endregion

                        //proxy.Bapi_User_Get_Detail(user.UserLogonName.Trim(), user.UserLogonName.Trim(), out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out  snc, out iuclass,
                        //                                                     ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
                        //                                           ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
                        //                                            ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
                        //                                            ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);

                        IRfcFunction getUserDetailFunction = bapiRepository.CreateFunction("BAPI_USER_GET_DETAIL");

                        getUserDetailFunction.SetValue("USERNAME", user.UserLogonName);
                        getUserDetailFunction.Invoke(SAPdestination);


                        IRfcStructure sAddress = getUserDetailFunction.GetStructure("ADDRESS");
                        IRfcStructure sIsLocked = getUserDetailFunction.GetStructure("ISLOCKED");
                        IRfcStructure sDefaults = getUserDetailFunction.GetStructure("DEFAULTS");



                        if (IsGroupSelected)
                        {
                            IRfcTable tGroups = getUserDetailFunction.GetTable("GROUPS");

                            dtSAPUserGroups = tGroups.ToDataTable("GROUPS");//groups.ToADODataTable();
                            if (dtSAPUserGroups.Rows.Count > 0)
                            {
                                AddUserGroups(dtSAPUserGroups, user.Id);

                                if (!IsFirstTimeDiscovery)
                                {
                                    SAPUserinfo.DeleteSAPUserGroupMap(this.EndPoint.Id, user.Id);
                                }
                                SAPUserinfo.SaveSAPUserGroupMap(listUserGroup, this.EndPoint.Id);

                                //update entitlement recon
                                this.UpdateGroupToEntitlementRecon(listUserGroup);
                            }
                        }

                        if (IsProfileSelected)
                        {
                            IRfcTable tProfiles = getUserDetailFunction.GetTable("PROFILES");
                            dtSAPUserProfiles = tProfiles.ToDataTable("PROFILES");
                            if (dtSAPUserProfiles.Rows.Count > 0)
                            {
                                AddUserProfiles(dtSAPUserProfiles, user.Id);
                                if (!IsFirstTimeDiscovery)
                                {
                                    SAPUserinfo.DeleteSAPUserProfileMap(this.EndPoint.Id, user.Id);
                                }
                                SAPUserinfo.SaveSAPUserProfileMap(listUserProfile, this.EndPoint.Id);

                                //update entitlement recon
                                this.UpdateProfileToEntitlementRecon(listUserProfile);
                            }
                        }


                        //dtSAPUserParameters = parameter.ToADODataTable();
                        //if (dtSAPUserParameters.Rows.Count > 0)
                        //{
                        //    AddUserParameters(dtSAPUserParameters, user.UserId);
                        //}

                        if (IsRoleSelected)
                        {
                            IRfcTable tRoles = getUserDetailFunction.GetTable("ACTIVITYGROUPS");
                            dtSAPUserRoleCodes = tRoles.ToDataTable("ACTIVITYGROUPS");

                            //dtSAPUserRoleCodes = activeGroups.ToADODataTable();
                            if (dtSAPUserRoleCodes.Rows.Count > 0)
                            {
                                DataRow[] dtRolesDirectlyAttached = dtSAPUserRoleCodes.Select("Org_Flag <> 'C'");

                                AddUserRoles(dtSAPUserRoleCodes, dtRolesDirectlyAttached, user.Id);
                                if (!IsFirstTimeDiscovery)
                                {
                                    SAPUserinfo.DeleteSAPUserRoleMap(this.EndPoint.Id, user.Id);
                                }
                                SAPUserinfo.SaveSAPUserRoleMap(listUserRole, this.EndPoint.Id);
                                //  AddUserCompositeAndSingleRoles(user.UserId);

                                //update entitlement recon
                                this.UpdateRoleToEntitlementRecon(listUserRole);
                            }
                        }

                        listUserGroup = new List<SAPUserinfo.SAPUserGroup>();
                        listUserProfile = new List<SAPUserinfo.SAPUserProfile>();
                        listUserRole = new List<SAPUserinfo.SAPUserRole>();

                        if (Counter++ % 50 == 0) Thread.Sleep(2000);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            listDBSAPUsers = null;

            if (IsTCodeSelected)
                AddSingleRoleTcodes();

            IList<SAPUserinfo.SAPCompositeChildRoleMap> listCompositeChildRoleMap = new List<SAPUserinfo.SAPCompositeChildRoleMap>();
            listCompositeChildRoleMap = this.GetCompositeChildRolesMAP();
            SAPUserinfo.SaveCompositeChildRoleMap(listCompositeChildRoleMap, this.EndPoint.Id);
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering  Mapping Tables at {0}", DateTime.Now));
        }


        //Unused function, so didnt upgrade the code to 3.0

        //private void PopulateMappingTables()
        //{
        //    IList<SAPUserinfo> listDBSAPUsers = null;
        //    DataTable dtSAPUserGroups = null;
        //    DataTable dtSAPUserProfiles = null;
        //    DataTable dtSAPUserRoleCodes = null;

        //    listDBSAPUsers = new List<SAPUserinfo>();
        //    listDBSAPUsers = SAPUserinfo.GetAll(true);
        //    listUserGroup = new List<SAPUserinfo.SAPUserGroup>();
        //    listUserProfile = new List<SAPUserinfo.SAPUserProfile>();
        //    listUserRole = new List<SAPUserinfo.SAPUserRole>();
        //    listUserParameter = new List<SAPUserinfo.SAPUserParameter>();
        //    listSingleRoleTcodes = new List<SAPUserinfo.SapSingleRoleTcodes>();

        //    SAPUserinfo.DeleteSAPUserGroupMap(this.EndPoint.Id);
        //    SAPUserinfo.DeleteSAPUserProfileMap(this.EndPoint.Id);
        //    SAPUserinfo.DeleteSAPUserRoleMap(this.EndPoint.Id);



        //    try
        //    {
        //        foreach (SAPUserinfo user in listDBSAPUsers)
        //        {
        //            //if (user.DIUserId != null)
        //            //{
        //            #region SAP BAPI CLASSES

        //            BAPIADDR3 address = new BAPIADDR3();
        //            BAPIUSCOMP companyName = new BAPIUSCOMP();
        //            BAPIALIAS alias = new BAPIALIAS();
        //            BAPIDEFAUL defaults = new BAPIDEFAUL();
        //            BAPISLOCKD islocked = new BAPISLOCKD();
        //            BAPILOGOND logonData = new BAPILOGOND();
        //            BAPIPWD passworddata = new BAPIPWD();
        //            BAPIMODDAT lastmofified = new BAPIMODDAT();
        //            BAPIREFUS refuser = new BAPIREFUS();
        //            BAPISNCU snc = new BAPISNCU();
        //            BAPIUCLASS iuclass = new BAPIUCLASS();
        //            BAPIAGRTable activeGroups = new BAPIAGRTable();
        //            BAPICOMREMTable addcomrem = new BAPICOMREMTable();
        //            BAPIADFAXTable addfax = new BAPIADFAXTable();
        //            BAPIADPAGTable addpag = new BAPIADPAGTable();
        //            BAPIADPRTTable addprt = new BAPIADPRTTable();
        //            BAPIADRFCTable addrfc = new BAPIADRFCTable();
        //            BAPIADRMLTable addrml = new BAPIADRMLTable();
        //            BAPIADSMTPTable addsmtp = new BAPIADSMTPTable();
        //            BAPIADSSFTable addssf = new BAPIADSSFTable();
        //            BAPIADTELTable addtel = new BAPIADTELTable();
        //            BAPIADTLXTable addtlx = new BAPIADTLXTable();
        //            BAPIADTTXTable addttx = new BAPIADTTXTable();
        //            BAPIADURITable adduri = new BAPIADURITable();
        //            BAPIADX400Table addx400 = new BAPIADX400Table();
        //            BAPIUSEXTIDHEADTable exithead = new BAPIUSEXTIDHEADTable();
        //            BAPIUSEXTIDPARTTable extidpart = new BAPIUSEXTIDPARTTable();

        //            BAPIGROUPSTable groups = new BAPIGROUPSTable();
        //            BAPIPARAMTable parameter = new BAPIPARAMTable();
        //            BAPIPARAM1Table parameter1 = new BAPIPARAM1Table();
        //            BAPIRCVSYSTable systems = new BAPIRCVSYSTable();
        //            BAPIPROFTable profiles = new BAPIPROFTable();
        //            BAPIUCLASSSYSTable uclClassSys = new BAPIUCLASSSYSTable();
        //            SAPUserDetailsComponent.BAPIRET2Table return0 = new SAPUserDetailsComponent.BAPIRET2Table();
        //            BAPIUSSEXPTable sextable = new BAPIUSSEXPTable();
        //            BAPIUSSRGETable gettable = new BAPIUSSRGETable();
        //            BAPIUSNAMETable userNametable = new BAPIUSNAMETable();

        //            AGR_TCODESTable TCodes = new AGR_TCODESTable();
        //            AGR_SHIER_BORTable SHIERBOR = new AGR_SHIER_BORTable();
        //            AGR_HIERTable HeirTable = new AGR_HIERTable();
        //            AGR_BUFFITable BuffitTable = new AGR_BUFFITable();
        //            AGR_HIERTTable HeirTTable = new AGR_HIERTTable();

        //            BAPILOGONX BAPILOGONX = new BAPILOGONX();
        //            BAPIPWDX BAPIPWDX = new BAPIPWDX();
        //            BAPIADCOMX BAPIADCOMX = new BAPIADCOMX();
        //            BAPIADDR3X BAPIADDR3X = new BAPIADDR3X();
        //            BAPIALIASX BAPIALIASX = new BAPIALIASX();
        //            BAPIFLAG BAPIFLAG = new BAPIFLAG();
        //            BAPIUSCOMX BAPIUSCOMX = new BAPIUSCOMX();
        //            BAPIDEFAX BAPIDEFAX = new BAPIDEFAX();
        //            BAPIUSEXTIDX BAPIUSEXTIDX = new BAPIUSEXTIDX();
        //            BAPIGROUPX BAPIGROUPX = new BAPIGROUPX();
        //            BAPIPARAMX BAPIPARAMX = new BAPIPARAMX();
        //            BAPIREFUSX BAPIREFUSX = new BAPIREFUSX();
        //            BAPISNCUX BAPISNCUX = new BAPISNCUX();
        //            BAPIUCLASSX BAPIUCLASSX = new BAPIUCLASSX();
        //            BAPIUCLASSSYSTable BAPIUCLASSSYSTable = new BAPIUCLASSSYSTable();

        //            SAPUserDetailsComponent.BAPIRET2 return2 = new SAPUserDetailsComponent.BAPIRET2();

        //            #endregion
        //            proxy.Bapi_User_Get_Detail(user.UserLogonName.Trim(), user.UserLogonName.Trim(), out address, out alias, out companyName, out defaults, out islocked, out lastmofified, out logonData, out refuser, out  snc, out iuclass,
        //                                                                 ref activeGroups, ref addcomrem, ref addfax, ref addpag, ref addprt, ref addrfc,
        //                                                       ref addrml, ref addsmtp, ref addssf, ref addtel, ref addtlx, ref addttx,
        //                                                        ref adduri, ref addx400, ref exithead, ref extidpart, ref groups, ref parameter,
        //                                                        ref parameter1, ref profiles, ref return0, ref systems, ref uclClassSys);


        //            dtSAPUserGroups = groups.ToADODataTable();
        //            if (dtSAPUserGroups.Rows.Count > 0)
        //            {
        //                AddUserGroups(dtSAPUserGroups, user.Id);
        //            }

        //            dtSAPUserProfiles = profiles.ToADODataTable();
        //            if (dtSAPUserProfiles.Rows.Count > 0)
        //            {
        //                AddUserProfiles(dtSAPUserProfiles, user.Id);
        //            }

        //            //dtSAPUserParameters = parameter.ToADODataTable();
        //            //if (dtSAPUserParameters.Rows.Count > 0)
        //            //{
        //            //    AddUserParameters(dtSAPUserParameters, user.UserId);
        //            //}

        //            dtSAPUserRoleCodes = activeGroups.ToADODataTable();
        //            if (dtSAPUserRoleCodes.Rows.Count > 0)
        //            {

        //                DataRow[] arrRoleCodes = dtSAPUserRoleCodes.Select("Org_Flag <> 'C'");

        //                AddUserRoles(dtSAPUserRoleCodes, arrRoleCodes, user.Id);
        //                //  AddUserCompositeAndSingleRoles(user.UserId);
        //            }
        //            //}

        //            SAPUserinfo.SaveSAPUserGroupMap(listUserGroup, this.EndPoint.Id);
        //            SAPUserinfo.SaveSAPUserProfileMap(listUserProfile, this.EndPoint.Id);
        //            SAPUserinfo.SaveSAPUserRoleMap(listUserRole, this.EndPoint.Id);
        //            listUserGroup = new List<SAPUserinfo.SAPUserGroup>();
        //            listUserProfile = new List<SAPUserinfo.SAPUserProfile>();
        //            listUserRole = new List<SAPUserinfo.SAPUserRole>();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    listDBSAPUsers = null;

        //    //Bulk Insert all mapping Tables

        //    //AddSingleRoleTcodes();

        //    IList<SAPUserinfo.SAPCompositeChildRoleMap> listCompositeChildRoleMap = new List<SAPUserinfo.SAPCompositeChildRoleMap>();
        //    listCompositeChildRoleMap = this.GetCompositeChildRolesMAP();
        //    SAPUserinfo.SaveCompositeChildRoleMap(listCompositeChildRoleMap, this.EndPoint.Id);

        //    //SAPFacade.SaveSAPSingleRoleTcodeMap(listSingleRoleTcodes, this.EndPoint.EndpointSystemId);
        //}

        public IList<SapUserProfile> GetProfileUsers()
        {

            //TAB512Table dt = new TAB512Table();
            //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
            //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;
            List<SapUserProfile> profileinfo = new List<SapUserProfile>();

            try
            {

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "UST04");
                readTableFunction.SetValue("DELIMITER", "|");

                //proxy.Rfc_Read_Table("|", null, "UST04", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtParameters = readTable.ToDataTable("UST04");

                //DataTable dtParameters = dt.ToADODataTable();
                //DataTable abc = dtFlds.ToADODataTable();
                //DataTable dtotps = dtopts.ToADODataTable();




                for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
                {
                    string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
                    string[] SplitRole = fullRole.Split('|');
                    SapUserProfile sapprofile = new SapUserProfile();
                    sapprofile.UserName = SplitRole[1].ToString().Trim();
                    sapprofile.ProfileName = SplitRole[2].ToString().Trim();

                    sapprofile.EndPointSystemId = (int)EndPoint.Id;
                    profileinfo.Add(sapprofile);

                }
            }
            catch (Exception ex)
            {

            }
            return profileinfo;
        }

        //Unused function, so didnt upgrade the code to 3.0
        //public IList<SapRoleProfileMapping> GetRoleProfile()
        //{

        //    TAB512Table dt = new TAB512Table();
        //    RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
        //    RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

        //    proxy.Rfc_Read_Table("|", null, "AGR_PROF", 0, 0, ref dt, ref dtFlds, ref dtopts);
        //    DataTable dtParameters = dt.ToADODataTable();
        //    DataTable abc = dtFlds.ToADODataTable();
        //    DataTable dtotps = dtopts.ToADODataTable();


        //    List<SapRoleProfileMapping> lstroleprofile = new List<SapRoleProfileMapping>();

        //    for (int Role = 0; Role < dtParameters.Rows.Count; Role++)
        //    {
        //        string fullRole = dtParameters.Rows[Role]["Wa"].ToString().Trim();
        //        string[] SplitRole = fullRole.Split('|');
        //        SapRoleProfileMapping roleprofile = new SapRoleProfileMapping();
        //        roleprofile.RoleName = SplitRole[1].ToString().Trim();
        //        roleprofile.ProfileName = SplitRole[3].ToString().Trim();

        //        roleprofile.EndPointSystemId = (int)EndPoint.Id;
        //        roleprofile.CreatedBy = PROCESSNAME;
        //        roleprofile.CreatedDate = DateTime.Now;
        //        roleprofile.LastModifiedBy = PROCESSNAME;
        //        roleprofile.LastModifiedDate = DateTime.Now;

        //        lstroleprofile.Add(roleprofile);

        //    }
        //    return lstroleprofile;
        //}


        private IList<SAPUserinfo.SAPCompositeChildRoleMap> GetCompositeChildRolesMAP()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            IList<SAPUserinfo.SAPCompositeChildRoleMap> listCompositeChildRoleMap = new List<SAPUserinfo.SAPCompositeChildRoleMap>();

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_AGRS");
                readTableFunction.SetValue("DELIMITER", "|");

                //proxy.Rfc_Read_Table("|", null, "AGR_AGRS", 0, 0, ref dt, ref dtFlds, ref dtopts);
                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldTable = readTableFunction.GetTable("FIELDS");

                DataTable dtFields = fieldTable.ToDataTable("FIELDS");
                DataTable dtTemp = readTable.ToDataTable("AGR_AGRS");

                //DataTable dtTemp = dt.ToADODataTable();
                //DataTable dtFields = dtFlds.ToADODataTable();
                DataTable m_CompositeRoles = convertPipesToColumns(dtFields, dtTemp);

                IList<DomainModel.SAPCompositeRoleInfo> lstSapCompositeRoles = DomainModel.SAPCompositeRoleInfo.GetByEndPointSystemId(EndPoint.Id);
                //SAPFacade.GetSapCompositeRoles(EndPoint.Id);

                IList<SAPSingleRoleInfo> lstSapSingleRoles = SAPSingleRoleInfo.GetByEndPointSystemId(EndPoint.Id);

                for (int i = 0; i < m_CompositeRoles.Rows.Count; i++)
                {
                    SAPUserinfo.SAPCompositeChildRoleMap SAPCompositeChildRoleMap = new SAPUserinfo.SAPCompositeChildRoleMap();
                    DomainModel.SAPCompositeRoleInfo objSAPCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();
                    SAPSingleRoleInfo objSAPSingleRoleInfo = new SAPSingleRoleInfo();
                    //objSAPCompositeRoleInfo = SAPFacade.GetSAPCompositeRoleByCompositeRole_Code(m_CompositeRoles.Rows[i].ItemArray[1].ToString().Trim(), EndPoint.EndpointSystemId);
                    objSAPCompositeRoleInfo = lstSapCompositeRoles.FirstOrDefault(compositerole => compositerole.CompositeRole_Code == m_CompositeRoles.Rows[i].ItemArray[1].ToString().Trim());

                    if (objSAPCompositeRoleInfo != null)
                    {
                        SAPCompositeChildRoleMap.SAPCompositeRoleInfo_CompositeRole_Id = objSAPCompositeRoleInfo.Id;
                        objSAPSingleRoleInfo = lstSapSingleRoles.FirstOrDefault(singlerole => singlerole.SingleRole_Code == m_CompositeRoles.Rows[i].ItemArray[2].ToString().Trim());
                        //objSAPSingleRoleInfo = SAPFacade.GetSAPSingleRoleBySingleRole_Code(, EndPoint.EndpointSystemId);
                        if (objSAPSingleRoleInfo != null)
                        {
                            SAPCompositeChildRoleMap.SAPSingleRoleInfo_Id = objSAPSingleRoleInfo.Id;
                            SAPCompositeChildRoleMap.EndpointSystemId = EndPoint.Id;
                            SAPCompositeChildRoleMap.Version = (int)VersionNo.Available;

                            if (listCompositeChildRoleMap.FirstOrDefault(CompChildRole =>
                                                                    CompChildRole.SAPCompositeRoleInfo_CompositeRole_Id == objSAPCompositeRoleInfo.Id
                                                                    && CompChildRole.SAPSingleRoleInfo_Id == objSAPSingleRoleInfo.Id
                                                                    && CompChildRole.EndpointSystemId == EndPoint.Id) == null)
                                listCompositeChildRoleMap.Add(SAPCompositeChildRoleMap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return listCompositeChildRoleMap;
        }

        //Unused function, so not ported to 3.0 
        //private IList<SAPUserinfo.SapSingleRoleTcodes> GetSingleRoleTcodeMAP(IList<SAPSingleRoleInfo> ListSingleRole, )
        //{
        //    TAB512Table dtTcode = new TAB512Table();
        //    RFC_DB_FLDTable dtTFlds = new RFC_DB_FLDTable();
        //    RFC_DB_OPTTable dtTopts = new RFC_DB_OPTTable();

        //    RFC_DB_OPT clause = new RFC_DB_OPT();
        //    clause.Text = "OBJECT EQ 'S_TCODE'";
        //    dtTopts.Add(clause);

        //    proxy.Rfc_Read_Table("|", null, "AGR_1251", 0, 0, ref dtTcode, ref dtTFlds, ref dtTopts);
        //    DataTable dtTcodes = dtTcode.ToADODataTable();
        //    dtTcode = null;
        //    DataTable TFlds = dtTFlds.ToADODataTable();
        //    DataTable Topts = dtTopts.ToADODataTable();
        //    DataTable dtAlltcode = this.convertPipesToColumns(TFlds, dtTcodes);

        //    if (dtAlltcode != null && dtAlltcode.Rows.Count > 0)
        //    {
        //        SAPUserinfo.AddSingleRoleTCodes(dtAlltcode, EndPoint.Id, DeltaRoles, IsFirstTimeDiscovery);
        //    }
        //    return listSingleRoleTcodes;
        //}

        // private IList<SAPCompositeRoleInfo> GetCompositeRoles()

        private IList<DomainModel.SAPCompositeRoleInfo> GetCompositeRoles()
        {

            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            IList<DomainModel.SAPCompositeRoleInfo> listSAPCompositeRoleInfo = new List<DomainModel.SAPCompositeRoleInfo>();

            try
            {

                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                //proxy.Rfc_Read_Table("|", null, "AGR_AGRS", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_AGRS");
                readTableFunction.SetValue("DELIMITER", "|");

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldTable = readTableFunction.GetTable("FIELDS");

                DataTable dtTemp = readTable.ToDataTable("AGR_AGRS");


                //DataTable dtTemp = dt.ToADODataTable();
                DataTable dtFields = fieldTable.ToDataTable("FIELDS");
                DataTable m_CompositeRoles = convertPipesToColumns(dtFields, dtTemp);


                for (int i = 0; i < m_CompositeRoles.Rows.Count; i++)
                {
                    DomainModel.SAPCompositeRoleInfo SAPCompositeRoleInfo = new DomainModel.SAPCompositeRoleInfo();

                    SAPCompositeRoleInfo.CompositeRole_Code = m_CompositeRoles.Rows[i].ItemArray[1].ToString().Trim();  // m_CompositeRoles.Rows["AGR_NAME"].ToString();
                    //SAPCompositeRoleInfo.CHILD_ROLE_CODE = m_CompositeRoles.Rows[i].ItemArray[2].ToString();//m_CompositeRoles.Rows["CHILD_AGR"].ToString();
                    //SAPCompositeRoleInfo.CHILD_ROLE_CODE_DESCRIPTION = string.Empty;
                    SAPCompositeRoleInfo.CompositeRole_Desc = "";
                    SAPCompositeRoleInfo.EndPointSystemId = (int)EndPoint.Id;
                    SAPCompositeRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                    //SAPCompositeRoleInfo.SAP_USERID = "";
                    bool exists = listSAPCompositeRoleInfo.Any(c => c.CompositeRole_Code == SAPCompositeRoleInfo.CompositeRole_Code);
                    if (!exists)
                    {
                        listSAPCompositeRoleInfo.Add(SAPCompositeRoleInfo);
                    }

                    if (listSAPCompositeRoleInfo.Count() % 500 == 0)
                    {
                        //  SAPFacade.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, EndPoint.Id);
                        DomainModel.SAPCompositeRoleInfo.SaveSAPCompositeRoles(listSAPCompositeRoleInfo, EndPoint.Id);
                        listSAPCompositeRoleInfo.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listSAPCompositeRoleInfo;
        }

        private IList<SAPSingleRoleInfo> GetChildRoles()
        {
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            IList<SAPSingleRoleInfo> listSAPSingleRoleInfo = new List<SAPSingleRoleInfo>();

            try
            {
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();


                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "AGR_AGRS");
                readTableFunction.SetValue("DELIMITER", "|");

                //proxy.Rfc_Read_Table("|", null, "AGR_AGRS", 0, 0, ref dt, ref dtFlds, ref dtopts);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");
                fieldTable = readTableFunction.GetTable("FIELDS");

                DataTable dtTemp = readTable.ToDataTable("AGR_AGRS");

                //DataTable dtTemp = dt.ToADODataTable();
                DataTable dtFields = fieldTable.ToDataTable("FIELDS");
                DataTable m_CompositeRoles = convertPipesToColumns(dtFields, dtTemp);


                for (int i = 0; i < m_CompositeRoles.Rows.Count; i++)
                {
                    SAPSingleRoleInfo SAPSingleRoleInfo = new SAPSingleRoleInfo();

                    SAPSingleRoleInfo.SingleRole_Code = m_CompositeRoles.Rows[i].ItemArray[2].ToString().Trim();  // m_CompositeRoles.Rows["AGR_NAME"].ToString();
                    //SAPSingleRoleInfo.CHILD_ROLE_CODE = m_CompositeRoles.Rows[i].ItemArray[2].ToString();//m_CompositeRoles.Rows["CHILD_AGR"].ToString();
                    //SAPSingleRoleInfo.CHILD_ROLE_CODE_DESCRIPTION = string.Empty;
                    SAPSingleRoleInfo.SingleRole_Desc = "";
                    SAPSingleRoleInfo.Version = Convert.ToInt16(VersionNo.Available);
                    SAPSingleRoleInfo.EndPointSystemId = (int)EndPoint.Id;
                    //  SAPSingleRoleInfo.SAP_USERID = "";
                    bool exists = listSAPSingleRoleInfo.Any(c => c.SingleRole_Code == SAPSingleRoleInfo.SingleRole_Code);
                    if (!exists)
                    {
                        listSAPSingleRoleInfo.Add(SAPSingleRoleInfo);
                    }

                    if (listSAPSingleRoleInfo.Count() % 500 == 0)
                    {
                        DI.Chakra.DomainModel.SAPSingleRoleInfo.SaveSingleRoles(listSAPSingleRoleInfo, EndPoint.Id, MasterConfigHelper.ServiceUser);
                        listSAPSingleRoleInfo.Clear();
                    }

                    if (listSAPSingleRoleInfo.Count() % 50 == 0)
                    {
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return listSAPSingleRoleInfo;
        }

        private void AddUserGroups(DataTable dtSAPUserGroups, long userId)
        {
            for (int group = 0; group < dtSAPUserGroups.Rows.Count; group++)
            {
                SAPUserinfo.SAPUserGroup objSAPUserGroup = new SAPUserinfo.SAPUserGroup();

                SAPGroupInfo objgrp = new SAPGroupInfo();
                objgrp = SAPGroupInfo.GetByEndPointSystemId(EndPoint.Id, dtSAPUserGroups.Rows[group]["UserGroup"].ToString());
                if (objgrp != null)
                {
                    SAPUserinfo.SAPUserGroup objUserGroup = listUserGroup.FirstOrDefault(usergroup => usergroup.SAPGroupInfo_Id == objgrp.Id && usergroup.SAPUserinfo_Id == userId);
                    if (objUserGroup == null)
                    {
                        objSAPUserGroup.SAPGroupInfo_Id = objgrp.Id;
                        objSAPUserGroup.SAPUserinfo_Id = userId;
                        objSAPUserGroup.EndpointSystemId = this.EndPoint.Id;
                        objSAPUserGroup.Version = Convert.ToInt16(VersionNo.Available);
                        listUserGroup.Add(objSAPUserGroup);
                    }
                }
            }
        }

        private void AddUserProfiles(DataTable dtSAPUserProfiles, long userId)
        {
            for (int Profile = 0; Profile < dtSAPUserProfiles.Rows.Count; Profile++)
            {
                SAPUserinfo.SAPUserProfile objSAPUserProfile = new SAPUserinfo.SAPUserProfile();

                SAPProfileInfo objProfile = new SAPProfileInfo();
                objProfile = SapProfiles.FirstOrDefault(profile => profile.PROFILE_CODE == dtSAPUserProfiles.Rows[Profile]["Bapiprof"].ToString()); //SAPFacade.GetProfileByPROFILE_CODE(dtSAPUserProfiles.Rows[Profile]["Bapiprof"].ToString(), EndPoint.EndpointSystemId);
                if (objProfile != null)
                {
                    SAPUserinfo.SAPUserProfile objUserProfile = listUserProfile.FirstOrDefault(profile => profile.SAPProfileInfo_Id == objProfile.Id && profile.SAPUserinfo_Id == userId);
                    if (objUserProfile == null)
                    {
                        objSAPUserProfile.SAPProfileInfo_Id = objProfile.Id;
                        objSAPUserProfile.SAPUserinfo_Id = userId;
                        objSAPUserProfile.EndpointSystemId = this.EndPoint.Id;
                        objSAPUserProfile.Version = Convert.ToInt16(VersionNo.Available);
                        listUserProfile.Add(objSAPUserProfile);
                    }
                }
            }
        }

        private void AddUserParameters(DataTable dtSAPUserParameters, int userId)
        {
            for (int Parameter = 0; Parameter < dtSAPUserParameters.Rows.Count; Parameter++)
            {
                SAPUserinfo.SAPUserParameter objSAPUserParameter = new SAPUserinfo.SAPUserParameter();

                SAPParameterInfo objparameter = new SAPParameterInfo();
                objparameter = SAPParameterInfo.GetByParamCode(dtSAPUserParameters.Rows[Parameter]["Parid"].ToString(), EndPoint.Id);
                if (objparameter != null)
                {
                    // SAPUserParameterMapHistoryInfo.SAP_PARAM_VALUE = dtSAPUserParameters.Rows[Parameter]["Parva"].ToString();
                    SAPUserinfo.SAPUserParameter objUserParameter = listUserParameter.FirstOrDefault(parameter => parameter.SAPParameterInfo_Id == objparameter.Id && parameter.SAPUserInfo_Id == userId);
                    if (objUserParameter == null)
                    {
                        objSAPUserParameter.SAPParameterInfo_Id = objparameter.Id;
                        objSAPUserParameter.SAPUserInfo_Id = userId;
                        objSAPUserParameter.EndpointSystemId = this.EndPoint.Id;
                        objSAPUserParameter.Version = Convert.ToInt16(VersionNo.Available);
                        listUserParameter.Add(objSAPUserParameter);
                    }
                }
            }
        }

        private void AddUserRoles(DataTable dtSAPUserRoles, DataRow[] dtRolesDirectlyAttached, long userId)
        {
            SAPUserinfo.SAPUserRole objSAPUserRole = null;
            SAPRoleInfo objRole = null;

            for (int role = 0; role < dtSAPUserRoles.Rows.Count; role++)
            {
                objSAPUserRole = new SAPUserinfo.SAPUserRole();

                objRole = SapRoles.FirstOrDefault(saprole => saprole.ROLE_CODE == dtSAPUserRoles.Rows[role]["Agr_Name"].ToString().Trim());
                if (objRole != null)
                {
                    SAPUserinfo.SAPUserRole objUserRole = listUserRole.FirstOrDefault(userrole => userrole.SAPRoleInfo_Id == objRole.Id && userrole.SAPUserinfo_Id == userId);
                    if (objUserRole == null)
                    {
                        objSAPUserRole.SAPRoleInfo_Id = objRole.Id;
                        objSAPUserRole.SAPUserinfo_Id = userId;
                        objSAPUserRole.EndpointSystemId = this.EndPoint.Id;
                        objSAPUserRole.Version = Convert.ToInt16(VersionNo.Available);
                        objSAPUserRole.IsDirectlyAttached = false;
                        DateTime fromdate = DateTime.ParseExact(dtSAPUserRoles.Rows[role]["from_dat"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                        DateTime todate = DateTime.ParseExact(dtSAPUserRoles.Rows[role]["to_dat"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture).Date;
                        objSAPUserRole.Role_FromDate = fromdate.ToString(ProvisioningConstants.SAPDateFormater);

                        objSAPUserRole.Role_ToDate = todate.ToString(ProvisioningConstants.SAPDateFormater);
                        if (dtRolesDirectlyAttached.FirstOrDefault(directrole => directrole["Agr_Name"].ToString().Equals(dtSAPUserRoles.Rows[role]["Agr_Name"].ToString().Trim(), StringComparison.CurrentCultureIgnoreCase)) != null)
                        {
                            objSAPUserRole.IsDirectlyAttached = true;
                        }
                        listUserRole.Add(objSAPUserRole);
                    }
                }
            }
        }

        private void AddSingleRoleTcodes()
        {
            //IList<SAPSingleRoleInfo> listSingleRoles = new List<SAPSingleRoleInfo>();
            //listSingleRoles = SAPFacade.GetSingleRolesFromDB(EndPoint.EndpointSystemId);

            //listSingleRoleTcodes = this.GetSingleRoleTcodeMAP(listSingleRoles, proxy);

            #region Discovering Role To TCode (AGR_TCODES)
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Discovering  Role To TCode Mapping at {0}", DateTime.Now));
            IList<SapRoleTCodeMapping> lstSapRoleTCodeMap = GetRoleTCodeMapping();
            SapRoleTCodeMapping.SaveSapRoleTCodeMapping(lstSapRoleTCodeMap, EndPoint.Id, MasterConfigHelper.ServiceUser);
            lstSapRoleTCodeMap = null;
            WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed Discovering Role To TCode Mapping at {0}", DateTime.Now));
            #endregion


        }

        private DataTable convertPipesToColumnsGeneral(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');

                        if (SplitProfile.Count() > dtFields.Rows.Count)
                            continue;

                        dt.Rows.Add(SplitProfile);
                    }
                }
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumns(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= 14)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumnsTCodes(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= 6)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumnsUsers(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        //if (SplitProfile.Length <= 34)
                        //{
                        dt.Rows.Add(SplitProfile);
                        //}
                    }
                }
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumnsProfiles(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= 5)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumnsRoles(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= 5)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable convertPipesToColumns1(DataTable dtFields, DataTable dtPipes)
        {
            DataTable dtTemp = null;
            DataTable dt = new DataTable();
            string fullProfile;
            string[] SplitProfile;
            try
            {
                if (!(dtFields != null && dtFields.Rows != null && dtFields.Rows.Count > 0))
                    return dtTemp;

                foreach (DataRow dRow in dtFields.Rows)
                {
                    dt.Columns.Add(dRow["FieldName"].ToString());
                }

                for (int index = 0; index < dtPipes.Rows.Count; index++)
                {
                    fullProfile = dtPipes.Rows[index]["Wa"].ToString();
                    if (fullProfile.IndexOf("|J3G|") < 0)
                    {
                        SplitProfile = fullProfile.Split('|');
                        if (SplitProfile.Length <= 3)
                        {
                            dt.Rows.Add(SplitProfile);
                        }
                    }
                }

                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IList<SAPRoleInfo> GeRolesFromDB(int EndpointSystemId)
        {
            try
            {
                return SAPRoleInfo.GetByEndpointSystemId(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void UpdateSAPRoleType()
        {
            IList<SAPRoleInfo> listSAPRoles = new List<SAPRoleInfo>();
            IList<DomainModel.SAPCompositeRoleInfo> listSAPCompositeRoles = new List<DomainModel.SAPCompositeRoleInfo>();

            listSAPRoles = SAPRoleInfo.GetByEndpointSystemId(EndPoint.Id);
            listSAPCompositeRoles = DomainModel.SAPCompositeRoleInfo.GetByEndPointSystemId(EndPoint.Id);
            //SAPFacade.SAPCompositeRolesFromDB(EndPoint.Id);

            var composite = (from l in listSAPRoles
                             from m in listSAPCompositeRoles
                             where l.ROLE_CODE == m.CompositeRole_Code
                             select l);

            if (composite.Count() > 0)
            {
                foreach (SAPRoleInfo role in composite)
                {
                    role.RoleType = Constants.COMPOSITE;
                }
            }

            SAPRoleInfo.SaveRoles(composite.ToList<SAPRoleInfo>(), EndPoint.Id, MasterConfigHelper.ServiceUser);
            listSAPCompositeRoles = null;


            var single = (listSAPRoles.Where(r => string.IsNullOrEmpty(r.RoleType)).ToList());


            if (single.Count() > 0)
            {
                foreach (SAPRoleInfo role in single)
                {
                    role.RoleType = Constants.SINGLE;
                }
            }
            SAPRoleInfo.UpdateRolesAsSingleIfNull(EndPoint.Id, MasterConfigHelper.ServiceUser);
            //SAPFacade.SaveRoles(single.ToList<SAPRoleInfo>());
            listSAPRoles = null;
        }

        #endregion

        //private List<string> GetDefaultRolesByEndpoint()
        //{
        //    List<string> defaultRoles = new List<string>();
        //    var form = ProvisioningForm.GetByEndpointSystem(EndPoint.Id, AccountType.User);
        //    ProvisioningForm fullForm = ProvisioningForm.GetByFormId(form.Id);
        //    ProvisioningFormSAP sapForm = ProvisioningFormSAP.GetByProvisioningFormIdIncludinigProvisionForm(form.Id);
        //    if (sapForm != null)
        //    {
        //        defaultRoles = sapForm.roles.Split(',').ToList();
        //    }
        //    return defaultRoles;
        //}

        private List<ExistingPositionUserInfo> GetExistingUsersByPosition(string positionCode)
        {
            List<ExistingPositionUserInfo> posUsers = new List<ExistingPositionUserInfo>();
            RfcRepository bapiRepository;
            IRfcFunction readTableFunction;
            IRfcTable fieldTable;
            IRfcTable optionsTable;
            IRfcTable readTable;
            string filter;

            try
            {
                DataTable m_dtHRP1001 = new DataTable();
                //TAB512Table dt = new TAB512Table();
                //RFC_DB_FLDTable dtFlds = new RFC_DB_FLDTable();
                //RFC_DB_OPTTable dtopts = new RFC_DB_OPTTable();

                //RFC_DB_FLD FLD_SOBID = new RFC_DB_FLD();
                //FLD_SOBID.Fieldname = "SOBID";

                //RFC_DB_FLD FLD_BEGDA = new RFC_DB_FLD();
                //FLD_BEGDA.Fieldname = "BEGDA";

                //RFC_DB_FLD FLD_ENDDA = new RFC_DB_FLD();
                //FLD_ENDDA.Fieldname = "ENDDA";

                //dtFlds.Add(FLD_SOBID);
                //dtFlds.Add(FLD_BEGDA);
                //dtFlds.Add(FLD_ENDDA);

                //RFC_DB_OPT filter = new RFC_DB_OPT();
                //filter.Text = "RSIGN = 'B' AND RELAT = '007' AND OTYPE = 'S' AND ISTAT = '1' AND PLVAR = '01' AND ENDDA = '" + maxDate + "' ";
                filter = "SCLAS = 'US' AND OTYPE = 'S' AND OBJID = '" + positionCode + "' ";


                SAPdestination = GetSAPDestination();
                bapiRepository = SAPdestination.Repository;


                readTableFunction = bapiRepository.CreateFunction("RFC_READ_TABLE");
                readTableFunction.SetValue("QUERY_TABLE", "HRP1001");
                readTableFunction.SetValue("DELIMITER", "|");

                fieldTable = readTableFunction.GetTable("FIELDS");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "SOBID");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "BEGDA");
                fieldTable.Append();
                fieldTable.SetValue("FIELDNAME", "ENDDA");

                //dtFields = fieldTable.ToDataTable("FIELDS");

                optionsTable = readTableFunction.GetTable("OPTIONS");
                optionsTable.Append();
                optionsTable.SetValue("TEXT", filter);

                readTableFunction.Invoke(SAPdestination);

                readTable = readTableFunction.GetTable("DATA");

                DataTable dtTemp = readTable.ToDataTable("HRP1001");
                DataTable dtFields = fieldTable.ToDataTable("FIELDS");
                m_dtHRP1001 = ConvertPipesToColumns(dtFields, dtTemp);


                //dtopts.Add(filter);

                //using (SAPProxy1 proxy = new SAPProxy1())
                //{
                //    proxy.Connection = this.GetSAPConnection();
                //    proxy.Connection.Open();

                //    proxy.Rfc_Read_Table("|", null, "HRP1001", 0, 0, ref dt, ref dtFlds, ref dtopts);
                //DataTable dtTemp = dt.ToADODataTable();
                //DataTable dtFields = dtFlds.ToADODataTable();

                //}

                foreach (DataRow row in m_dtHRP1001.Rows)
                {
                    ExistingPositionUserInfo posUser = new ExistingPositionUserInfo()
                    {
                        ValidFrom = row["BEGDA"].ToString().Trim(),
                        ValidTo = row["ENDDA"].ToString().Trim(),
                        UserName = row["SOBID"].ToString().Trim(),
                    };

                    posUsers.Add(posUser);
                }
            }
            catch
            {
                throw;
            }
            return posUsers;
        }

        private void UpdateGroupToEntitlementRecon(IList<SAPUserinfo.SAPUserGroup> userGroupList)
        {
            try
            {
                foreach (SAPUserinfo.SAPUserGroup item in userGroupList)
                {
                    var epUserInfo = SAPUserinfo.GetByIdIncludeSAPGroupInfo(item.SAPUserinfo_Id);

                    if (epUserInfo.SAPGroupInfos != null)
                    {
                        foreach (SAPGroupInfo entitlement in epUserInfo.SAPGroupInfos)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(epUserInfo.EndPointSystemId, epUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(epUserInfo.Id, epUserInfo.EndPointSystemId, entitlement.GROUP_CODE.Trim(), EntitlementType.Group);

                            if (entRecon != null)
                            {
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (epUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Group,string.Join(",", entitlements!=null?entitlements.ToArray():new string[] {}));
                                entRecon.Entitlement = entitlement.GROUP_CODE.Trim();
                                entRecon.ExceptionType = ExceptionTypes.Added;
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }

        private void UpdateRoleToEntitlementRecon(IList<SAPUserinfo.SAPUserRole> userGroupList)
        {
            try
            {
                foreach (SAPUserinfo.SAPUserRole item in userGroupList)
                {
                    var epUserInfo = SAPUserinfo.GetByIdIncludeSAPRoleInfo(item.SAPUserinfo_Id);

                    if (epUserInfo.SAPRoleInfo != null)
                    {
                        foreach (SAPRoleInfo entitlement in epUserInfo.SAPRoleInfo)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(epUserInfo.EndPointSystemId, epUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(epUserInfo.Id, epUserInfo.EndPointSystemId, entitlement.ROLE_CODE.Trim(), EntitlementType.Role);

                            if (entRecon != null)
                            {
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (epUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role,string.Join(",", entitlements!=null?entitlements.ToArray():new string[] {}));
                                entRecon.Entitlement = entitlement.ROLE_CODE.Trim();
                                entRecon.ExceptionType = ExceptionTypes.Added;
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }

        private void UpdateProfileToEntitlementRecon(IList<SAPUserinfo.SAPUserProfile> userProfileList)
        {
            try
            {
                foreach (SAPUserinfo.SAPUserProfile item in userProfileList)
                {
                    var epUserInfo = SAPUserinfo.GetByIdIncludeSAPProfileInfo(item.SAPUserinfo_Id);

                    if (epUserInfo.SAPProfileInfos != null)
                    {
                        foreach (SAPProfileInfo entitlement in epUserInfo.SAPProfileInfos)
                        {
                            var acctRecon = AccountReconcilation.GetByUser(epUserInfo.EndPointSystemId, epUserInfo.Id);
                            var entRecon = EntitlementReconcilation.GetByUser(epUserInfo.Id, epUserInfo.EndPointSystemId, entitlement.PROFILE_CODE.Trim(), EntitlementType.Profile);

                            if (entRecon != null)
                            {
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                            else
                            {
                                var entitlements = (epUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Profile,string.Join(",", entitlements!=null?entitlements.ToArray():new string[] {}));
                                entRecon.Entitlement = entitlement.PROFILE_CODE.Trim();
                                entRecon.ExceptionType = ExceptionTypes.Added;
                                entRecon.Save(MasterConfigHelper.ServiceUser);
                            }
                        }
                    }
                }

                EntitlementReconcilation.ProcessDeletedEntitlements(this.EndPoint.Id, this.Process, MasterConfigHelper.ServiceUser);
            }
            catch (Exception ex)
            {
                this.WriteDiscoveryLog(LogLevelType.Exception, "Error while updating Entitlement reconciliation.", ex);
            }
        }

    }


    //public static class IRfcTableExtentions
    //{
    //    /// <summary>
    //    /// Converts SAP table to .NET DataTable table
    //    /// </summary>
    //    /// <param name="sapTable">The SAP table to convert.</param>
    //    /// <returns></returns>
    //    public static DataTable ToDataTable(this IRfcTable sapTable, string name)
    //    {
    //        DataTable adoTable = new DataTable(name);
    //        //... Create ADO.Net table.
    //        for (int liElement = 0; liElement < sapTable.ElementCount; liElement++)
    //        {
    //            RfcElementMetadata metadata = sapTable.GetElementMetadata(liElement);
    //            adoTable.Columns.Add(metadata.Name, GetDataType(metadata.DataType));
    //        }

    //        //Transfer rows from SAP Table ADO.Net table.
    //        foreach (IRfcStructure row in sapTable)
    //        {
    //            DataRow ldr = adoTable.NewRow();
    //            for (int liElement = 0; liElement < sapTable.ElementCount; liElement++)
    //            {
    //                RfcElementMetadata metadata = sapTable.GetElementMetadata(liElement);

    //                switch (metadata.DataType)
    //                {
    //                    case RfcDataType.DATE:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name).Substring(0, 4) + row.GetString(metadata.Name).Substring(5, 2) + row.GetString(metadata.Name).Substring(8, 2);
    //                        break;
    //                    case RfcDataType.BCD:
    //                        ldr[metadata.Name] = row.GetDecimal(metadata.Name);
    //                        break;
    //                    case RfcDataType.CHAR:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                    case RfcDataType.STRING:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                    case RfcDataType.INT2:
    //                        ldr[metadata.Name] = row.GetInt(metadata.Name);
    //                        break;
    //                    case RfcDataType.INT4:
    //                        ldr[metadata.Name] = row.GetInt(metadata.Name);
    //                        break;
    //                    case RfcDataType.FLOAT:
    //                        ldr[metadata.Name] = row.GetDouble(metadata.Name);
    //                        break;
    //                    default:
    //                        ldr[metadata.Name] = row.GetString(metadata.Name);
    //                        break;
    //                }
    //            }
    //            adoTable.Rows.Add(ldr);
    //        }
    //        return adoTable;
    //    }

    //    private static Type GetDataType(RfcDataType rfcDataType)
    //    {
    //        switch (rfcDataType)
    //        {
    //            case RfcDataType.DATE:
    //                return typeof(string);
    //            case RfcDataType.CHAR:
    //                return typeof(string);
    //            case RfcDataType.STRING:
    //                return typeof(string);
    //            case RfcDataType.BCD:
    //                return typeof(decimal);
    //            case RfcDataType.INT2:
    //                return typeof(int);
    //            case RfcDataType.INT4:
    //                return typeof(int);
    //            case RfcDataType.FLOAT:
    //                return typeof(double);
    //            default:
    //                return typeof(string);
    //        }
    //    }
    //}
}
