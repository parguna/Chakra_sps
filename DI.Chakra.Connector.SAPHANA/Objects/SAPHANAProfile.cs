using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAProfile
    {
        public SAPHANAProcessorConfig ProcessorConfig { get; set; }
        private const string LangFilter = "LANGU EQ 'E'";
        string ServiceUser;
        public SAPHANAProfile(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public void StartProcess()
        {

            ProcessProfiles();

            var deletedProfiles = SAPHANAProfileInfo.GetByModifiedDate(ProcessorConfig.EndpointId, ProcessorConfig.Process.ProcessStartAt);
            SAPHANAProfileInfo.DeleteList(deletedProfiles.ToList());
        }

        public IList<SAPHANAProfileInfo> ProcessProfiles()
        {
           var lstProfileInfo = new List<SAPHANAProfileInfo>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessProfiles.", DiscoveryObjectType.Profile);
            ProcessorConfig.sapUtil.ResetPaging();
            DataTable dtProfiles = null;
            try
            {
                string filter = LangFilter;
                var SAPProfileInfo = new SAPHANAProfileInfo();
                do
                {
                    // getting profiles from profile text definition table
                    dtProfiles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR11", null, null, filter, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtProfiles.Rows)
                    {
                        SAPProfileInfo = new SAPHANAProfileInfo();
                        SAPProfileInfo.PROFILE_CODE = row["PROFN"].ToString().Trim();
                        SAPProfileInfo.PROFILE_DESC = row["PTEXT"].ToString().Trim();
                        SAPProfileInfo.EndPointSystemId = ProcessorConfig.EndpointId;
                        SAPProfileInfo.Version = Convert.ToInt16(VersionNo.Available);

                        SAPProfileInfo.TargetSysId = null;

                        lstProfileInfo.Add(SAPProfileInfo);
                    }

                    if (ProcessorConfig.sapUtil.IsPaginationEnabled)
                    {
                        if (lstProfileInfo.Count % ProcessorConfig.insertThreshold == 0 && lstProfileInfo.Count > 0)
                        {
                            SAPHANAProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, ServiceUser);
                            lstProfileInfo.Clear();
                        }

                        ProcessorConfig.sapUtil.NextPage();
                    }


                } while (dtProfiles.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstProfileInfo.Count > 0)
                {
                    SAPHANAProfileInfo.SaveProfiles(lstProfileInfo, ProcessorConfig.EndpointId, ServiceUser);
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

        //For Testing
        public IList<SAPHANAProfileInfo> GetProfiles()
        {
            var lstProfileInfo = new List<SAPHANAProfileInfo>();
            DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started GetProfiles.", DiscoveryObjectType.Profile);
            try
            {
                string filter = LangFilter;
                DataTable dtProfiles = null;
                var SAPProfileInfo = new SAPHANAProfileInfo();
                // getting profiles from profile text definition table
                dtProfiles = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("USR11", null, null, filter);
                foreach (DataRow row in dtProfiles.Rows)
                {
                    SAPProfileInfo = new SAPHANAProfileInfo();
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
