using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPV3
{
    public class Profile
    {
        public ProcessorConfig ProcessorConfig { get; set; }
        private const string LangFilter = "LANGU EQ 'E'";
        string ServiceUser;
        public Profile(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public void StartProcess()
        {

            ProcessProfiles();

            var deletedProfiles = SAPProfileInfo.GetByModifiedDate(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
            SAPProfileInfo.DeleteList(deletedProfiles.ToList());
            deletedProfiles = null;
        }

        public IList<SAPProfileInfo> ProcessProfiles()
        {
            IList<SAPProfileInfo> lstProfileInfo = new List<SAPProfileInfo>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessProfiles.", DiscoveryObjectType.Profile);
            ProcessorConfig.sapUtil.ResetPaging();
            DataTable dtProfiles = null;
            try
            {
                string filter = LangFilter;
                SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();
                do
                {
                    // getting profiles from profile text definition table
                    dtProfiles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR11", null, null, filter);
                    foreach (DataRow row in dtProfiles.Rows)
                    {
                        SAPProfileInfo = new SAPProfileInfo();
                        SAPProfileInfo.PROFILE_CODE = row["PROFN"].ToString().Trim();
                        SAPProfileInfo.PROFILE_DESC = row["PTEXT"].ToString().Trim();
                        SAPProfileInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                        SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);

                        SAPProfileInfo.TargetSysId = null;

                        lstProfileInfo.Add(SAPProfileInfo);
                    }

                    if (lstProfileInfo.Count % ProcessorConfig.insertThreshold == 0 && lstProfileInfo.Count > 0)
                    {
                        SAPProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, ServiceUser);
                        lstProfileInfo.Clear();
                    }

                    ProcessorConfig.sapUtil.NextPage();


                } while (dtProfiles.Rows.Count > 0);

                if (lstProfileInfo.Count > 0)
                {
                    SAPProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, ServiceUser);
                    lstProfileInfo.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed ProcessProfiles.", DiscoveryObjectType.Profile);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            finally
            {
                dtProfiles?.Dispose();
                dtProfiles = null;
            }
            return lstProfileInfo;
        }

        //public IList<SAPProfileInfo> ProcessProfiles1()
        //{
        //    IList<SAPProfileInfo> lstProfileInfo = new List<SAPProfileInfo>();
        //    try
        //    {
        //        string filter = "LANGU EQ 'EN'";
        //        DataTable dtProfiles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR11", null, null, filter);
        //        foreach (DataRow row in dtProfiles.Rows)
        //        {
        //            SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();
        //            SAPProfileInfo.PROFILE_CODE = row["PROFN"].ToString();
        //            SAPProfileInfo.PROFILE_DESC = row["PTEXT"].ToString();
        //            SAPProfileInfo.EndPointSystemId = ProcessorConfig.EndpointId;
        //            SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);

        //            SAPProfileInfo.TargetSysId = null;

        //            lstProfileInfo.Add(SAPProfileInfo);

        //            if (lstProfileInfo.Count() % ProcessorConfig.insertThreshold == 0 && lstProfileInfo.Count > 0)
        //            {
        //                SAPProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, MasterConfigHelper.ServiceUser);
        //                lstProfileInfo.Clear();
        //            }

        //        }
        //        SAPProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, MasterConfigHelper.ServiceUser);
        //        lstProfileInfo.Clear();

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return lstProfileInfo;
        //}

        //For Testing
        public IList<SAPProfileInfo> GetProfiles()
        {
            IList<SAPProfileInfo> lstProfileInfo = new List<SAPProfileInfo>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetProfiles.", DiscoveryObjectType.Profile);
            try
            {
                string filter = LangFilter;
                DataTable dtProfiles = null;
                SAPProfileInfo SAPProfileInfo = new SAPProfileInfo();
                // getting profiles from profile text definition table
                dtProfiles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR11", null, null, filter);
                foreach (DataRow row in dtProfiles.Rows)
                {
                    SAPProfileInfo = new SAPProfileInfo();
                    SAPProfileInfo.PROFILE_CODE = row["PROFN"].ToString().Trim();
                    SAPProfileInfo.PROFILE_DESC = row["PTEXT"].ToString().Trim();
                    SAPProfileInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                    SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);

                    SAPProfileInfo.TargetSysId = null;

                    lstProfileInfo.Add(SAPProfileInfo);
                }

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Completed GetProfiles.", DiscoveryObjectType.Profile);
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Exception, ex.Message, ex);
            }
            return lstProfileInfo;
        }
    }
}
