///<summary>
/// IACM v3.5 
/// 
/// Author        : I.Deepthi
/// Created Date  : 05/02/2011
/// Modified Date :
/// Description   :check  Version
///</summary>
///
#region Namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

using DeepIdentity.Model;
using System.Globalization;
using DeepIdentity.Infrastructure;
using System.Configuration;
using DeepIdentity.DataAccess;
using System.Transactions;
using System.Data.Entity;
using System.Linq;
using DeepIdentity.Model.GridData;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model.ReportData;
using DeepIdentity.Model.Core.Endpoint;
#endregion

namespace DeepIdentity.Facade
{
    public class EndPointFacade// : EndPointFacade
    {
        public EndPointDA EndPointDA { get; set; }
        public ActiveDirectoryDA ActiveDirectoryDA { get; set; }
        public SchedulerDA SchedulerDA { get; set; }
        public LotusNotesDA LotusNotesDA { get; set; }
        public EndpointObjectDA EndpointObjectDA { get; set; }
        public SAPConnectorDA SAPDA { get; set; }
        public SAPEPDA SAPEPDA { get; set; }
        public ORCLUMDA Oracle11gUserManagementDA { get; set; }
        private DiscoveryProcessDA discoveryDA;


        public EndPointFacade()
        {

            EndPointDA = new EndPointDA();
            ActiveDirectoryDA = new ActiveDirectoryDA();
            SchedulerDA = new SchedulerDA();
            LotusNotesDA = new LotusNotesDA();
            EndpointObjectDA = new EndpointObjectDA();
            SAPDA = new SAPConnectorDA();
            Oracle11gUserManagementDA = new ORCLUMDA();
            SAPEPDA = new SAPEPDA();
            discoveryDA = new DiscoveryProcessDA();
        }

        #region EndPointFacade Members

        /// <summary>
        /// Update EndPointSystem
        /// </summary>
        /// <param name="_endpoint"></param>
        public void UpdateEndPointSystem(EndPoint _endpoint)
        {
            try
            {
                EndPointDA.UpdateEndpoint(_endpoint);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteEndPointSystem(int EndPointSystemId)
        {
            try
            {
                EndPointDA.DeleteEndPointSystem(EndPointSystemId);

            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IList<EndPointFunctionType> GetEndPointFunctionType(int EndPointSystemId)
        {

            try
            {
                IList<EndPointFunctionType> functionTypes = EndPointDA.GetEndPointFunctionType(EndPointSystemId);

                return functionTypes;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        public IList<EndPointSystemType> GetEndPointSystemType()
        {

            try
            {
                IList<EndPointSystemType> platformType = EndPointDA.GetAllEndPointType();

                return platformType;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        public EndPointSystemType GetEndPointSystemType(EndpointType Name)
        {

            try
            {
                EndPointSystemType platformType = EndPointDA.GetAllEndPointType().Where(t => t.EndpointSystemTypeName == Name).FirstOrDefault();

                return platformType;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        public IList<EndPointSystemType> GetEndPointTypeExceptInhouse()
        {

            try
            {
                IList<EndPointSystemType> platformType = EndPointDA.GetEndPointTypeExceptInhouse();

                return platformType;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        public IList<EndPointSystemType> GetEndPointSystemTypeForProvisioning()
        {
            return EndPointDA.GetEndPointSystemTypeForProvisioning();
        }

        public IList<EndPointSystemType> GetEndPointSystemTypeForAttestation()
        {

            try
            {
                IList<EndPointSystemType> platformType = EndPointDA.GetEndPointSystemTypeForAttestation();

                return platformType;
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        //public IList<EndPoint> GetAllEndPointSystemsByTypeEF(int EndPointSystemTypeId, string HostName)
        //{
        //    try
        //    {
        //        IACMDBContext Context = new IACMDBContext();

        //        IList<EndPoint> EndPointSystems = Context.EndpointSystems.Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId && s.DisplayName == HostName).ToList();

        //        //IList<EndPoint> EndPointSystems = EndPointDA.GetAllEndPointByType(EndPointSystemTypeId, HostName);


        //        return EndPointSystems;


        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}



        public IList<EndPoint> GetAllEndPointSystemsByType(int EndPointSystemTypeId, string HostName)
        {
            try
            {

                IList<EndPoint> EndPointSystems = EndPointDA.GetAllEndPointByType(EndPointSystemTypeId, HostName);



                return EndPointSystems;


            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<EndPoint> GetAllEndPointSystemsByType(EndpointType EndpointSystemTypeName, string HostName)
        {
            try
            {

                IList<EndPoint> EndPointSystems = EndPointDA.GetAllEndPointByType(EndpointSystemTypeName, HostName);



                return EndPointSystems;


            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EndPoint GetEndPointSystemByKey(int _id)
        {
            try
            {
                return EndPointDA.GetEndPointSystemByKey(_id);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public DiscoveryProcess GetDiscoveryProcess(int DiscoveryProcessId)
        {
            try
            {
                return EndPointDA.GetDiscoveryProcess(DiscoveryProcessId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public EndPointSystemType GetEndpointSystemType(int EndpointTypeId)
        {
            try
            {
                return EndPointDA.GetEndpointSystemType(EndpointTypeId);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public EndPoint GetEndPointSystemByHostName(string HostName)
        {
            try
            {
                return EndPointDA.GetEndPointSystemByHostName(HostName);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<EndPointSystemType> GetAllEndPointType()
        {
            try
            {
                return EndPointDA.GetAllEndPointType();

            }
            catch (Exception)
            {
                throw;
            }
        }
        public EndPointSystemType GetEndPointSystemType(int EndPointSystemTypeId)
        {
            try
            {
                return EndPointDA.GetEndPointSystemType(EndPointSystemTypeId);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public int CheckEndPointSystemDuplicate(int EndPointSystemTypeId, string Hostname, string Ipaddress)
        {
            return 0;
        }



        public List<EndPoint> GetAllEndpoint()
        {
            try
            {
                List<EndPoint> EndPointSystems = EndPointDA.GetAllEndpoint();
                return EndPointSystems;

            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<EndPoint> GetAllEndpointBySystemTypeId(int EndpointSystemTypeId)
        {
            try
            {
                return EndPointDA.GetAllEndpoint().Where(ea => ea.EndpointSystemTypeId == EndpointSystemTypeId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EndPoint> GetAllEndpointsByTypeId(int EndpointSystemTypeId)
        {
            return EndPointDA.GetAllEndpointsByTypeId(EndpointSystemTypeId);
        }

        public List<EndPoint> GetAllEndpointForAttestation()
        {
            try
            {
                return EndPointDA.GetAllEndpointForAttestation(); ;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<EndPoint> GetAllEndpoint(User _user)
        {
            IList<EndPoint> _list = new List<EndPoint>();

            EndPoint _dto = new EndPoint();
            _dto.EndpointSystemId = 1;
            _dto.EndpointSystemTypeId = 1;

            _list.Add(_dto);

            _dto = new EndPoint();
            _dto.EndpointSystemId = 1;
            _dto.EndpointSystemTypeId = 2;

            _list.Add(_dto);

            return _list;
        }

        public void UnMapEndpointUsers(EndPoint Endpoint, string UserName)
        {
            try
            {
                EndPointDA.UnMapEndpointUsers(Endpoint, UserName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<UsersByEndPointGridData> GetUserByEndPoint(int EndPointSystemId, string EndPointSysType) // DataSet
        {
            return EndpointObjectDA.GetUserByEndPoint(EndPointSystemId, EndPointSysType);
        }
        public DataSet GetResourcesById(Int64 ResourceId, int EndPointSysId)
        {
            return EndpointObjectDA.GetResourcesById(ResourceId, EndPointSysId);
        }
        public DataSet GetResourcesHistoryById(Int64 ResourceId, int EndPointSysId, int HistoryId)
        {
            return EndpointObjectDA.GetResourcesHistoryById(ResourceId, EndPointSysId, HistoryId);
        }
        public DataSet GetAccessById(Int64 ResourceId, int EndPointSysId)
        {
            return EndpointObjectDA.GetAccessById(ResourceId, EndPointSysId);
        }



        public DataSet GetUXAccessById(Int64 ResourceId, int EndPointSysId)
        {
            return EndpointObjectDA.GetUXAccessById(ResourceId, EndPointSysId);
        }
        public DataSet GetResourcesByMount(String strMount, int EndPointSysId, string MountDrives)
        {
            return EndpointObjectDA.GetResourcesByMount(strMount, EndPointSysId, MountDrives);
        }
        public DataSet GetInActiveUserByEndPoint(string Name, string EndpointSystemTypeName)
        {
            return EndpointObjectDA.GetInActiveUserByEndPoint(Name, EndpointSystemTypeName);
        }
        /// <summary>
        /// Get Users By EndPoint  and based on search text in reassign user
        /// </summary>
        /// <param name="EndPointId"></param>
        /// <param name="EndPointType"></param>
        /// <param name="prefixText"></param>
        /// <returns></returns>
        public List<string> GetUserByEndPointId(int EndPointId, EndpointType EndPointType, string prefixText)
        {
            try
            {
                List<string> UserNames = new List<string>();

                IList<UsersByEndPointGridData> userList = GetUsersByEndPoint(EndPointId, EndPointType);
                if (userList == null || userList.Count == 0)
                    return null;

                IList<UsersByEndPointGridData> filteredUserList = new List<UsersByEndPointGridData>();
                filteredUserList = userList.Where(usr => usr.FullName.ToLower().Contains(prefixText.ToLower())).ToList();
                foreach (UsersByEndPointGridData _user in filteredUserList)
                {
                    UserNames.Add(_user.FullName);
                }
                return UserNames;
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion


        public List<EndpointInformation> GetEndpointsByDiUserid(int DIUserId)
        {
            return EndPointDA.GetEndpointsByDiUserid(DIUserId);
        }
        //public List<EndpointInformation> GetEndpointsByDiUseridForPwdSync(int DIUserId)
        //{
        //    return EndPointDA.GetEndpointsByDiUseridForPwdSync(DIUserId);
        //} 
        public void UpdatePasswordresetcount(int EndpointId, string UserName)
        {
            EndPointDA.UpdatePasswordresetcount(EndpointId, UserName);
        }

        public EndpointType GetEndPointTypeName(int EndpointTypeId) 
        {
            return EndPointDA.GetEndPointTypeName(EndpointTypeId);
        }

        public List<EndpointObjects> GetEndpointObjectsByTypeId(int EndpointSystemTypeId)
        {
            try
            {
                return EndPointDA.GetEndpointObjectsByTypeId(EndpointSystemTypeId);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EndpointObjects> GetEndpointObjectsByDiscoveryProcessId(int DiscoveryProcessId)
        {
            try
            {
                return EndPointDA.GetEndpointObjectsByDiscoveryProcessId(DiscoveryProcessId);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public EndpointObjects GetEndPointObject(int endpointObjectId)
        {
            try
            {
                return EndPointDA.GetEndPointObject(endpointObjectId);

            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<DiscoveryProcess> GetDiscoverysByEndpoint(int Endpointsystemid)
        {
            try
            {
                return EndPointDA.GetDiscoverysByEndpoint(Endpointsystemid);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DiscoveryProcess GetDiscoveryByEndpoint(int Endpointsystemid)
        {
            try
            {

                DiscoveryProcess DiscoveryProcess = EndPointDA.GetDiscoveryByEndpoint(Endpointsystemid);
                return DiscoveryProcess;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public DiscoveryProcess GetDiscoveryByDiscoveryProcessId(int DiscoveryProcessId)
        {
            try
            {
                return EndPointDA.GetDiscoveryByDiscoveryProcessId(DiscoveryProcessId);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public IList<DiscoveryReportData> getDiscoveryProcessHistoryByEP(int paramEndpointTypeId)
        {
            try
            {
                return discoveryDA.getDiscoveryProcessHistoryByEP(paramEndpointTypeId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Scheduler GetSchedulerByEndpointSystemId(int EndpointSystemId)
        {
            return EndPointDA.GetSchedulerByEndpointSystemId(EndpointSystemId);
        }

        public Scheduler GetSchedulerByDiscoveryProcessId(int DiscoveryProcessId)
        {
            return EndPointDA.GetSchedulerByDiscoveryProcessId(DiscoveryProcessId);
        }



        public void UpdateConfiguration(IEndpointConfiguration EndpointConfig)
        {
            try
            {
                EndPointDA.UpdateConfiguration(EndpointConfig);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetSAPConfigurationIDByEndpointSystemId(int _EndpointSytemId)
        {
            try
            {
                return EndPointDA.GetSAPConfigurationIDByEndpointSystemId(_EndpointSytemId);
            }
            catch
            {
                throw;
            }
        }

        public void AddConfiguration(IEndpointConfiguration EndpointConfig)
        {
            try
            {
                EndPointDA.AddConfiguration(EndpointConfig);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
        public IEndpointConfiguration GetConfiguration(int EndpointSystemId)
        {
            try
            {
                return EndPointDA.GetConfiguration(EndpointSystemId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEndpointConfiguration GetConfiguration(EndPoint Endpoint)
        {
            try
            {
                return EndPointDA.GetConfiguration(Endpoint);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EndPoint getEndPointOfType(string paramEndPointSystemType)
        {
            EndPoint retEndPoint = new EndPoint();
            try
            {
                IList<EndPointSystemType> epTypeList = EndPointDA.GetAllEndPointType();
                retEndPoint = epTypeList.FirstOrDefault(ep => ep.EndpointSystemTypeName.ToString() == paramEndPointSystemType).EndPoints.FirstOrDefault();
                return retEndPoint;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ExceptionReconGridData> GetExceptionReconData(EndpointType _type)
        {
            return new EndPointDA().GetExceptionReconData(_type);
        }

        public EndPoint GetEndpointByDiscoveryProcessId(int DiscoveryProcessId)
        {
            try
            {
                return EndPointDA.GetEndpointByDiscoveryProcessId(DiscoveryProcessId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DiscoveryJobGridData GetDiscoveryJobGridDataByEndpointSystemId(int EndpointSystemId)
        {
            return EndPointDA.GetDiscoveryJobGridDataByEndpointSystemId(EndpointSystemId);
        }

        public IList<EndPointsOfType> getListofEndPoints(int endpointTypeId)
        {
            try
            {
                return EndPointDA.getListofEndPoints(endpointTypeId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<EndPointPwdResetCountReportData> getPwdResetCountList(int endPointTypeId)
        {
            try
            {
                return EndPointDA.getPwdResetCountList(endPointTypeId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<UsersByEndPointReportData> getAllInActiveUsersByEndPoint(int endPointTypeId)
        {
            try
            {
                return EndPointDA.getAllInActiveUsersByEndPoint(endPointTypeId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSComputerObjectsReportData> getAllADSComputerObjects()
        {
            try
            {
                return EndPointDA.getAllADSComputerObjects();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSComputerObjectsReportData> getAllADSPrinterObjects()
        {
            try
            {
                return EndPointDA.getAllADSPrinterObjects();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<DiscoveryJobGridData> GetDiscoveryProcessByEndPoint(int EndPointSystemId)
        {
            try
            {
                return EndPointDA.GetDiscoveryProcessByEndPoint(EndPointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<DiscoveryReportData> getAllDiscoveryProcess(string _status)
        {
            try
            {
                return discoveryDA.getAllDiscoveryProcess(_status);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void DeleteDiscoveryJobSchedule(int _discoveryProcessId)
        {
            try
            {
                EndpointObjectDA.DeleteDiscoveryJobSchedule(_discoveryProcessId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        public void AddEndpointVPN(EndPoint _Endpoint)
        {
            EndpointObjectDA.AddEndpointVPN(_Endpoint);
        }

        public List<AttributeMappingColumns> GetAttributeMappingColumns(int _systemId)
        {
            return EndPointDA.GetAttributeMappingColumns(_systemId);
        }

        public EndPoint GetTrustedSourceEndPoint()
        {
            try
            {
                return EndPointDA.GetTrustedSourceEndPoint();
            }
            catch
            {
                throw;
            }
        }

        public IList<EndPoint> GetAllEndPointByType(int EndPointSystemTypeId)
        {
            try
            {
                return EndPointDA.GetAllEndPointByType(EndPointSystemTypeId);
            }
            catch
            {
                throw;
            }
        }

        public IList<EndPoint> GetAllEndPoint()
        {
            try
            {
                return EndPointDA.GetAllEndpoint();
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetAllEndPoints(string searchString)
        {
            try
            {
                return EndPointDA.GetAllEndpoint(searchString);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetAllEndPoints(bool ignoreTrustedSource)
        {
            try
            {
                return EndPointDA.GetAllEndPoints(ignoreTrustedSource);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetAllEndPointsByType(int endpointTypeId, bool ignoreTrustedSource)
        {
            try
            {
                return EndPointDA.GetAllEndPointsByType(endpointTypeId, ignoreTrustedSource);
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<int, string> GetAllEndPointByType(string searchString)
        {
            try
            {
                return EndPointDA.GetAllEndPointByType(searchString);

            }
            catch
            {
                throw;
            }
        }

        public void AddEndPoint(EndPoint endpoint)
        {
            try
            {
                EndPointDA.AddEndPoint(endpoint);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateEndPoint(EndPoint endpoint)
        {
            try
            {
                EndPointDA.UpdateEndpoint(endpoint);
            }
            catch
            {
                throw;
            }
        }

        public void KillDiscoveryProcess(int DiscoveryProcessId, string LastModifiedBy)
        {
            try
            {
                EndpointObjectDA.KillDiscoveryProcess(DiscoveryProcessId, LastModifiedBy);
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetNonConnectedAccount(int DIUserID, LockScenario lockScenario)
        {
            try
            {
                return EndPointDA.GetNonConnectedAccount(DIUserID, lockScenario);
            }
            catch
            {
                throw;
            }
        }

        public IList<UsersByEndPointGridData> GetUsersByEndPoint(int EndPointSystemId, EndpointType EndPointSysType)
        {
            try
            {
                return EndpointObjectDA.GetUsersByEndPoint(EndPointSystemId, EndPointSysType);
            }
            catch
            {
                throw;
            }
        }

        public string GetExceptionComment(int endpointSystemId, int endpointUserId, int userId, int version)
        {
            try
            {
                return EndpointObjectDA.GetExceptionComment(endpointSystemId, endpointUserId, userId, version);
            }
            catch
            {
                throw;
            }
        }

        public IList<AttributeMappingColumns> GetAttributes(int p, int _endPointTypeId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetAllEndPointSystemTypes()
        {
            return EndPointDA.GetAllEndPointSystemTypes();
        }

        public Dictionary<int, string> GetAllEndPointsBySelecetedSystemTypes(string searchString, List<int> EndpointTypeIds)
        {
            return EndPointDA.GetAllEndPointsBySelecetedSystemTypes(searchString, EndpointTypeIds);
        }

        public Dictionary<int, string> GetAllAttestationEndPointsBySelecetedSystemTypes(string searchString, List<int> EndpointTypeIds) 
        {
            return EndPointDA.GetAllAttestationEndPointsBySelecetedSystemTypes(searchString, EndpointTypeIds);
        }

        public void UpdateDiUserIdWithMappingInfo(IList<int> UserIdList, int DiUserId, string MapTo, int EndPointSystemId, EndpointType EndPointSystype, string UserName, AccountType accountType)
        {
            EndPointDA.UpdateDiUserIdWithMappingInfo(UserIdList, DiUserId, MapTo, EndPointSystemId, EndPointSystype, UserName, accountType);
        }

        public bool IsEndpointNameExists(string endpointName, int endpointid)  
        {
            return EndPointDA.IsEndpointNameExists(endpointName, endpointid); 
        }

        #region Added by Connector Team
        public void ExecuteQry(string strQry)
        {
            try
            {
                EndpointObjectDA.ExecuteQry(strQry);
            }
            catch
            {
                throw;
            }
        }

        public T1 GetDatabyId<T1>(int endpointUserId, string Includes = null) where T1 : class
        {
            try
            {
                return EndPointDA.GetDatabyId<T1>(endpointUserId, Includes);
            }
            catch
            {
                throw;
            }

        }

        public IList<T1> GetAllRecords<T1>(Func<T1, bool> Condition = null, string Includes = "") where T1 : class
        {
            try
            {
                return EndPointDA.GetAllRecords<T1>(Condition, Includes);
            }
            catch { throw; }
        }



        public void DeleteAllRecords<T1>() where T1 : class
        {
            EndPointDA.DeleteAllRecords<T1>();
        }

        public void bulkInsert<T>(List<T> ListforMapping, string TableName, bool IsDeleteAndInsert = true, string DeleteCondition = "1=1") where T : IAggregateRoot
        {

            EndPointDA.bulkInsert<T>(ListforMapping, TableName, IsDeleteAndInsert,DeleteCondition);
        }



        public bool getDatabyFunction<T1>(Func<T1, bool> Item, out object OutResult, bool IsDeepLoad = false, string Includes = "") where T1 : class
        {
            return EndPointDA.getDatabyFunction<T1>(Item, out OutResult, IsDeepLoad, Includes);
        }

        public int AddNewData<T1>(T1 InsertingData) where T1 : class
        {
            return EndPointDA.AddNewData<T1>(InsertingData);
        }
        #endregion

    }

}
