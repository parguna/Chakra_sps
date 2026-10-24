using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAAuthObject
    {
        string ServiceUser;
        public SAPHANAAuthObject(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public SAPHANAProcessorConfig ProcessorConfig { get; set; }

        public void StartProcess()
        {
            SAPHANAObjectInfo.DeleteSAPObjectsByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessObjects();

            SAPHANAAuthorizationProfile.DeleteSAPHANAAuthorizationProfileByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessAuthorizations();

            SAPHANAAuthorizationToObject.DeleteSAPHANAAuthorizationToObjectByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessObjectsAuthMap();
        }

        private IList<DomainModel.SAPHANAAuthorizationProfile> ProcessAuthorizations()
        {
            var lstAuthorizationInfo = new List<DomainModel.SAPHANAAuthorizationProfile>();
            DataTable dtParameters = null;

            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessAuthorizations.", DiscoveryObjectType.Authorization);
                long count = 0;
                var listExistingObjects = SAPHANAAuthorizationProfile.GetByEndpointId(ProcessorConfig.EndpointId);
                do
                {
                    // clear previous object
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("UST10S", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var SAPAuthorize = new DomainModel.SAPHANAAuthorizationProfile();
                        SAPAuthorize.ProfileCode = row["PROFN"].ToString().Trim();
                        SAPAuthorize.ObjectCode = row["OBJCT"].ToString().Trim();
                        SAPAuthorize.AuthorizationCode = row["AUTH"].ToString().Trim();
                        SAPAuthorize.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SAPAuthorize.Version = Convert.ToInt16(VersionNo.Available);
                        lstAuthorizationInfo.Add(SAPAuthorize);
                        count++;

                        if (lstAuthorizationInfo.Count > 0 && lstAuthorizationInfo.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            DomainModel.SAPHANAAuthorizationProfile.SaveAuthorizations(lstAuthorizationInfo, ServiceUser);
                            lstAuthorizationInfo.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstAuthorizationInfo.Count > 0)
                {
                    DomainModel.SAPHANAAuthorizationProfile.SaveAuthorizations(lstAuthorizationInfo, ServiceUser);
                    lstAuthorizationInfo.Clear();
                }
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessAuthorizations. Count: {0}", count), DiscoveryObjectType.Authorization);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtParameters?.Dispose();
                dtParameters = null;
            }
            return lstAuthorizationInfo;
        }
        private IList<SAPHANAAuthorizationToObject> ProcessObjectsAuthMap()
        {
            var lstSapObjects = new List<SAPHANAAuthorizationToObject>();
            DataTable dtParameters = null;

            try
            {
                ProcessorConfig.sapUtil.ResetPaging();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessObjectsAuthMap.", DiscoveryObjectType.Authorization);
                long count = 0;
                var listExistingSAPObjectsAuthMaps = SAPHANAAuthorizationToObject.GetByEndpointId(ProcessorConfig.EndpointId);
                do
                {
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("UST12", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var SapAuthorizeToObject = new SAPHANAAuthorizationToObject();
                        SapAuthorizeToObject.ObjectCode = row["OBJCT"].ToString().Trim();
                        SapAuthorizeToObject.AuthorizationCode = row["AUTH"].ToString().Trim();
                        SapAuthorizeToObject.FieldCode = row["FIELD"].ToString().Trim();
                        SapAuthorizeToObject.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SapAuthorizeToObject.Version = Convert.ToInt16(VersionNo.Available);
                        lstSapObjects.Add(SapAuthorizeToObject);
                        count++;

                        if (lstSapObjects.Count > 0 && lstSapObjects.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            SAPHANAAuthorizationToObject.SaveSAPObjectsAuthMap(lstSapObjects, ServiceUser);
                            lstSapObjects.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstSapObjects.Count > 0)
                {
                    SAPHANAAuthorizationToObject.SaveSAPObjectsAuthMap(lstSapObjects, ServiceUser);
                    lstSapObjects.Clear();
                }

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessObjectsAuthMap. Count: {0}", count), DiscoveryObjectType.Authorization);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtParameters?.Clear();
                dtParameters = null;
            }
            return lstSapObjects;
        }
        public IList<SAPHANAObjectInfo> ProcessObjects()
        {
            DataTable dtParameters = null;
            var lstSapObjects = new List<SAPHANAObjectInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessObjects.", DiscoveryObjectType.Authorization);
                long count = 0;
                do
                {
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TOBJ", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var sapObject = new SAPHANAObjectInfo();
                        sapObject.SapObjectCode = row["OBJCT"].ToString().Trim();
                        sapObject.FieldCode1 = row["FIEL1"].ToString().Trim();
                        sapObject.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapObject.Version = Convert.ToInt16(VersionNo.Available);
                        lstSapObjects.Add(sapObject);
                        count++;
                        if (lstSapObjects.Count > 0 && lstSapObjects.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            SAPHANAObjectInfo.SaveSapObject(lstSapObjects, ServiceUser);
                            lstSapObjects.Clear();
                        }
                    }

                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);

                if (lstSapObjects.Count > 0)
                {
                    SAPHANAObjectInfo.SaveSapObject(lstSapObjects, ServiceUser);
                    lstSapObjects.Clear();
                }

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, String.Format("Completed ProcessObjects. Count: {0}", count), DiscoveryObjectType.Authorization);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dtParameters?.Dispose();
                dtParameters = null;
            }
            return lstSapObjects;
        }
    }
}
