using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
namespace DI.Chakra.Connector.SAPV3
{
    public class AuthObjects
    {
        string ServiceUser;
        public AuthObjects(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public ProcessorConfig ProcessorConfig { get; set; }

        public void StartProcess()
        {
            SAPObjectInfo.DeleteSAPObjectsByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessObjects();

            SAPAuthorizationProfile.DeleteSAPAuthorizationProfileByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessAuthorizations();

            SAPAuthorizationToObject.DeleteSAPAuthorizationToObjectByEndpoint(ProcessorConfig.EndpointId, ServiceUser);
            ProcessObjectsAuthMap();
        }

        private IList<DomainModel.SAPAuthorizationProfile> ProcessAuthorizations()
        {
            IList<DomainModel.SAPAuthorizationProfile> lstAuthorizationInfo = new List<DomainModel.SAPAuthorizationProfile>();
            DataTable dtParameters = null;

            try
            {
                ProcessorConfig.sapUtil.ResetPaging();
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessAuthorizations.", DiscoveryObjectType.Authorization);
                long count = 0;
                var listExistingObjects = SAPAuthorizationProfile.GetByEndpointId(ProcessorConfig.EndpointId);
                do
                {
                    // clear previous object
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("UST10S", null, null, null);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        DomainModel.SAPAuthorizationProfile SAPAuthorize = new DomainModel.SAPAuthorizationProfile();
                        SAPAuthorize.ProfileCode = row["PROFN"].ToString().Trim();
                        SAPAuthorize.ObjectCode = row["OBJCT"].ToString().Trim();
                        SAPAuthorize.AuthorizationCode = row["AUTH"].ToString().Trim();
                        SAPAuthorize.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SAPAuthorize.Version = Convert.ToInt16(VersionNo.Available);
                        lstAuthorizationInfo.Add(SAPAuthorize);
                        count++;

                        if (lstAuthorizationInfo.Count > 0 && lstAuthorizationInfo.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            DomainModel.SAPAuthorizationProfile.SaveAuthorizations(lstAuthorizationInfo, ServiceUser);
                            lstAuthorizationInfo.Clear();
                        }
                    }
                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                if (lstAuthorizationInfo.Count > 0)
                {
                    DomainModel.SAPAuthorizationProfile.SaveAuthorizations(lstAuthorizationInfo, ServiceUser);
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
        private IList<SAPAuthorizationToObject> ProcessObjectsAuthMap()
        {
            var lstSapObjects = new List<SAPAuthorizationToObject>();
            DataTable dtParameters = null;

            try
            {
                ProcessorConfig.sapUtil.ResetPaging();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessObjectsAuthMap.", DiscoveryObjectType.Authorization);
                long count = 0;
                var listExistingSAPObjectsAuthMaps = SAPAuthorizationToObject.GetByEndpointId(ProcessorConfig.EndpointId);
                do
                {
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("UST12", null, null, null);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        SAPAuthorizationToObject SapAuthorizeToObject = new SAPAuthorizationToObject();
                        SapAuthorizeToObject.ObjectCode = row["OBJCT"].ToString().Trim();
                        SapAuthorizeToObject.AuthorizationCode = row["AUTH"].ToString().Trim();
                        SapAuthorizeToObject.FieldCode = row["FIELD"].ToString().Trim();
                        SapAuthorizeToObject.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SapAuthorizeToObject.Version = Convert.ToInt16(VersionNo.Available);
                        lstSapObjects.Add(SapAuthorizeToObject);
                        count++;

                        if (lstSapObjects.Count > 0 && lstSapObjects.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            SAPAuthorizationToObject.SaveSAPObjectsAuthMap(lstSapObjects, ServiceUser);
                            lstSapObjects.Clear();
                        }
                    }
                    ProcessorConfig.sapUtil.NextPage();


                } while (dtParameters.Rows.Count > 0);

                if (lstSapObjects.Count > 0)
                {
                    SAPAuthorizationToObject.SaveSAPObjectsAuthMap(lstSapObjects, ServiceUser);
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
        public IList<SAPObjectInfo> ProcessObjects()
        {
            DataTable dtParameters = null;
            IList<SAPObjectInfo> lstSapObjects = new List<SAPObjectInfo>();
            try
            {
                ProcessorConfig.sapUtil.ResetPaging();

                DILog.DiscoveryLogger.WriteLog(LogLevelType.Info, "Started ProcessObjects.", DiscoveryObjectType.Authorization);
                long count = 0;
                do
                {
                    dtParameters = null;
                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TOBJ", null, null, null);

                    foreach (DataRow row in dtParameters.Rows)
                    {
                        SAPObjectInfo sapObject = new SAPObjectInfo();
                        sapObject.SapObjectCode = row["OBJCT"].ToString().Trim();
                        sapObject.FieldCode1 = row["FIEL1"].ToString().Trim();
                        sapObject.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        sapObject.Version = Convert.ToInt16(VersionNo.Available);
                        lstSapObjects.Add(sapObject);
                        count++;
                        if (lstSapObjects.Count > 0 && lstSapObjects.Count % ProcessorConfig.insertThreshold == 0)
                        {
                            SAPObjectInfo.SaveSapObject(lstSapObjects, ServiceUser);
                            lstSapObjects.Clear();
                        }
                    }
                    ProcessorConfig.sapUtil.NextPage();

                } while (dtParameters.Rows.Count > 0);

                if (lstSapObjects.Count > 0)
                {
                    SAPObjectInfo.SaveSapObject(lstSapObjects, ServiceUser);
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
