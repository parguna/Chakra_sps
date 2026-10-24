
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using DI.Chakra.Infrastructure;
using DI.Chakra.Model.SAP;
using System.Data.Entity;
using DI.Chakra.Model.GridData;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Model.Core.Endpoint;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class EndPointDA// : IEndPointDA
    {
        List<EndpointInformation> EndPointlist = new List<EndpointInformation>();


        #region V5_METHODS
        public IList<EndPointFunctionType> GetEndPointFunctionType(int EndPointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var functionTypes = _context.EndPointFunctionTypes.Where(eps => eps.EndpointSystemTypeId == EndPointSystemId);
                    return functionTypes.ToList();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                throw ex;
            }
        }
        public IList<EndPointSystemType> GetAllEndPointType()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ll = _context.EndpointSystemTypes
                        .Include("EndPoints").Where(eps => eps.IsActive == true);
                    return ll.ToList();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                throw ex;
            }
        }
        public EndPointSystemType GetEndPointSystemType(int EndPointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystemTypes.Where(item => item.Id == EndPointSystemTypeId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                throw ex;
            }
        }

        public string GetObjectName(int EndPointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var result = _context.EndpointSystemTypes.Where(item => item.Id == EndPointSystemTypeId).FirstOrDefault();
                    return result.ObjectName;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Error, ex.Message, ex);
                throw ex;
            }
        }

        public List<EndPoint> GetAllEndpointByType(int typeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    return _context.EndpointSystems
                        .Where(t => t.EndpointSystemTypeId == typeId)
                        .Include("EndPointSystemType")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public List<EndPoint> GetAllEndpoint()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;
                    return _context.EndpointSystems
                        .Include("EndPointSystemType").Include("Owner")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion



        public void AddEndPoint(EndPoint _endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _endpoint.EndPointSystemType = null;
                    _context.EndpointSystems.Add(_endpoint);
                    _context.SaveChanges();

                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        public IList<DiscoveryProcess> GetDiscoverysByEndpoint(int Endpointsystemid)
        {
            IList<DiscoveryProcess> listDiscovery = new List<DiscoveryProcess>();
            using (IACMDBContext _context = new IACMDBContext())
                listDiscovery = _context.DiscoveryProcess.Where(dis => dis.Endpointsystemid == Endpointsystemid).ToList();
            return listDiscovery;
        }


        public DiscoveryProcess GetDiscoveryByEndpoint(int Endpointsystemid)
        {
            DiscoveryProcess DiscoveryProcess = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DiscoveryProcess = _context.DiscoveryProcess.FirstOrDefault(dis => dis.Endpointsystemid == Endpointsystemid);
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DiscoveryProcess.FirstOrDefault(Discovery => Discovery.Id == DiscoveryProcessId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Scheduler GetSchedulerByEndpointSystemId(int EndpointSystemId)
        {

            //TODO: CHECK HARI
            //using (IACMDBContext _context = new IACMDBContext())
            //{
            //    var schList = (from sch in _context.Scheduler
            //                   .Include("DiscoveryProcess")
            //                   where sch.DiscoveryProcess.Endpoint.EndpointSystemId == EndpointSystemId
            //                   select sch).ToList();

            //    if (schList.Count <= 0)
            //        return null;

            //    return schList.First();
            //}
            return null;

        }

        public void AddScheduler(Scheduler scheduler)
        {
            try
            {

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Scheduler.Add(scheduler);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Scheduler GetSchedulerByDiscoveryProcessId(int DiscoveryProcessId)
        {
            //TODO: CHECK HARI
            //Scheduler scheduler = null;


            //try
            //{

            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        scheduler = _context.Scheduler.FirstOrDefault(sch => sch.DiscoveryProcessId == DiscoveryProcessId);
            //        return scheduler;
            //    }

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return null;

        }

        public void UpdateScheduler(Scheduler sch)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //sch.AttestationProcess = null;
                    //sch.DiscoveryProcess = null;
                    _context.Scheduler.Attach(sch);
                    _context.Entry(sch).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }


        }
        public void UpdateDiscoveryProcess(DiscoveryProcess dis)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                try
                {
                    dis.Endpoint = null;
                    _context.DiscoveryProcess.Attach(dis);
                    _context.Entry(dis).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    throw;
                }
            }


        }

        public IList<EndPointSystemType> GetEndPointTypeExceptInhouse()
        {
            //string query = "DI_spgetEndPointSystemType";
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    int EPSTypeID = Convert.ToInt32(EndpointType.InhouseSystems);
                    return _context.EndpointSystemTypes
                        .Include("EndPoints").Where(eps => eps.IsActive == true && eps.Id != EPSTypeID)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint System Type Data", ex);
                throw ex;
            }
        }

        public IList<EndPointSystemType> GetEndPointSystemTypeForProvisioning()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<long> EPSTypeIDList = new List<long>();
                    EPSTypeIDList.Add(Convert.ToInt32(EndpointType.InhouseSystems));
                    EPSTypeIDList.Add(Convert.ToInt32(EndpointType.VPN));

                    return _context.EndpointSystemTypes.Include("EndPoints")
                        .Where(eps => eps.IsActive == true && !EPSTypeIDList.Contains(eps.Id))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint System Type Data", ex);
                throw ex;
            }
        }

        public IList<EndPointSystemType> GetEndPointSystemTypeForAttestation()
        {
            //string query = "DI_spgetEndPointSystemType";
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from item in _context.EndpointSystemTypes
                               .Include("EndPoints")
                            where item.IsActive == true
                            select item).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint System Type Data", ex);
                throw ex;
            }
        }


        /// <summary>
        /// Inserting Discovery Process
        /// </summary>
        /// <param name="_discoveryprocess"></param>
        /// <returns></returns>
        public int AddDiscoveryProcess(DiscoveryProcess _discoveryprocess)
        {
            // List<int> listIds = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    _context.DiscoveryProcess.Add(_discoveryprocess);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the DiscoveryProcess");

                throw;
            }

            return -1;
        }

        public EndPointSystemType GetEndpointSystemType(int EndpointTypeId)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystemTypes.Include(lq => lq.EndPoints).Where(lq => lq.Id == EndpointTypeId).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EndPointSystemType GetEndpointSystemTypeByType(EndpointType EndpointType)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystemTypes.Include(lq => lq.EndPoints).Where(lq => lq.EndpointSystemTypeName == EndpointType).FirstOrDefault();
                }
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
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DiscoveryProcess.Find(DiscoveryProcessId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public EndpointType GetEndPointTypeName(long EndpointTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType eType = new EndPointSystemType();
                    eType = _context.EndpointSystemTypes.Find(EndpointTypeId);
                    return eType.EndpointSystemTypeName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// GetAllEndPoint Systems By EndPointSystemTypeId 
        /// </summary>
        /// <param name="EndPointSystemTypeId"></param>
        /// <param name="DomainName"></param>
        /// <returns></returns>


        public Dictionary<long, string> GetAllEndpoint(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(item => item.Description.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.Description })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllEndPoints(bool ignoreTrustedSource)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (ignoreTrustedSource)
                    {
                        return _context.EndpointSystems.Where(item => !item.IsTrustedSource && !string.IsNullOrEmpty(item.DisplayName))
                       .Select(x => new { Key = x.Id, Value = x.DisplayName })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    return _context.EndpointSystems.Where(item => !string.IsNullOrEmpty(item.DisplayName))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllEndPointsByType(long endpointTypeId, bool ignoreTrustedSource)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (ignoreTrustedSource)
                    {
                        return _context.EndpointSystems.Where(item => !item.IsTrustedSource && item.EndpointSystemTypeId == endpointTypeId)
                       .Select(x => new { Key = x.Id, Value = x.Description })
                       .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    }
                    return _context.EndpointSystems.Where(item => item.EndpointSystemTypeId == endpointTypeId)
                        .Select(x => new { Key = x.Id, Value = x.Description })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EndPoint> GetAllEndpointsByTypeId(int EndpointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(endpoint => endpoint.EndpointSystemTypeId == EndpointSystemTypeId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }


        public List<EndpointObjects> GetEndpointObjectsByTypeId(int EndpointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from epobjects in _context.EndpointObjects
                            where epobjects.EndpointSystemTypeId == EndpointSystemTypeId
                            select epobjects).ToList();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint Objects", ex);
                throw ex;
            }
        }

        public List<EndpointObjects> GetEndpointObjectsByDiscoveryProcessId(long DiscoveryProcessId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    //DiscoveryProcess procees = _context.DiscoveryProcess.Include("EndpointObjects").FirstOrDefault(item => item.DiscoveryProcessId == DiscoveryProcessId);
                    //if (procees != null)
                    //{
                    //    var endpointObjects = procees.EndpointObjects;
                    //}
                    return _context.DiscoveryProcess.Include("EndpointObjects").FirstOrDefault(item => item.Id == DiscoveryProcessId).EndpointObjects;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint Objects", ex);
                throw ex;
            }
        }
        public EndpointObjects GetEndPointObject(int endpointObjectId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointObjects.Where(item => item.Id == endpointObjectId).FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint Objects", ex);
                throw ex;
            }
        }

        public void UnMapEndpointUsers(EndPoint Endpoint, string UserName)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    string Query = null;
                    EndpointType EndpointTypeName = GetEndPointTypeName(Endpoint.EndpointSystemTypeId.Value);
                    if (EndpointTypeName == EndpointType.ActiveDirectory)
                    {
                        Query = string.Format("UPDATE DI_ADS_UserInfo set DIUserId = null,LastModifiedBy = '{0}' , LastModifiedDate='{1}' where EndPointSystemId ={2} ", UserName, DateTime.Now.Date, Endpoint.Id);

                    }
                    else if (EndpointTypeName == EndpointType.SAP)
                    {
                        Query = string.Format("UPDATE DI_SAP_UserInfo set DIUserId = null,LastModifiedBy = '{0}' , LastModifiedDate='{1}' where EndPointSystemId ={2} ", UserName, DateTime.Now.Date, Endpoint.Id);

                    }
                    else if (EndpointTypeName == EndpointType.SAPEnterprisePortal)
                    {
                        Query = string.Format("UPDATE DI_SAPEP_UserInfo set DIUserId = null,LastModifiedBy = '{0}' , LastModifiedDate='{1}' where EndPointSystemId ={2} ", UserName, DateTime.Now.Date, Endpoint.Id);

                    }
                    else if (EndpointTypeName == EndpointType.LotusNotes)
                    {
                        Query = string.Format("UPDATE DI_LNS_USERS set DIUserId = null,LastModifiedBy = '{0}' , LastModifiedDate='{1}' where EndPointSystemId ={2} ", UserName, DateTime.Now.Date, Endpoint.Id);

                    }


                    if (Query != null)
                    {
                        oIACMDBContext.Database.ExecuteSqlCommand(Query);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<EndPoint> GetAllEndpointForAttestation()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return (from eps in _context.EndpointSystems
                            join epst in _context.EndpointSystemTypes
                               on eps.EndpointSystemTypeId equals epst.Id
                            where eps.IncludeInAttestation
                            select eps).ToList();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching EndPoint System Data", ex);
                throw ex;
            }
        }

        public void AddConfiguration(IEndpointConfiguration EndpointConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (EndpointConfig.GetType().Name == "ADSConfiguration")
                    {
                        _context.ADSConfiguration.Add((ADSConfiguration)EndpointConfig);
                    }
                    else if (EndpointConfig.GetType().Name == "SAPConfiguration")
                    {
                        _context.SapConfiguration.Add((SAPConfiguration)EndpointConfig);
                    }
                    else if (EndpointConfig.GetType().Name == "SAPEPConfiguration")
                    {
                        _context.SapEpConfiguration.Add((SAPEPConfiguration)EndpointConfig);
                    }
                    else if (EndpointConfig.GetType().Name == "LNSConfiguration")
                    {
                        _context.lnsConfiguration.Add((LNSConfiguration)EndpointConfig);
                    }

                    else if (EndpointConfig.GetType().Name == "InhouseSystemConfiguration")
                    {
                        _context.InhouseSystems.Add((InhouseSystemConfiguration)EndpointConfig);
                    }

                    else if (EndpointConfig.GetType().Name == "OracleEBSConfiguration")
                    {
                        _context.OracleEBSConfiguration.Add((OracleEBSConfiguration)EndpointConfig);
                    }

                    else if (EndpointConfig.GetType().Name == "LinuxConfiguration")
                    {
                        _context.linuxConfigurations.Add((LinuxConfiguration)EndpointConfig);
                    }

                    else if (EndpointConfig.GetType().Name == "OracleUMConfiguration")
                    {
                        _context.OracleUMConfiguration.Add((OracleUMConfiguration)EndpointConfig);
                    }

                    else if (EndpointConfig.GetType().Name == "SqlConfiguration")
                    {
                        _context.SqlConfiguration.Add((SqlConfiguration)EndpointConfig);
                    }


                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Adding Endpoint Configuration", ex);
                throw ex;
            }
        }



        public void UpdateEndpoint(EndPoint Endpoint)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Endpoint.EndPointSystemType = null;
                    _context.EndpointSystems.Attach(Endpoint);
                    _context.Entry(Endpoint).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetSAPConfigurationIDByEndpointSystemId(long _EndpointSytemId)
        {
            long SAPConfigurationId = 0;
            if ((_EndpointSytemId > 0))
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SAPConfigurationId = _context.lnsConfiguration.Where(lq => lq.EndPointSystemId == _EndpointSytemId).Select(lq => lq.Id).FirstOrDefault();
                }

            }
            return SAPConfigurationId;
        }

        public void UpdateConfiguration(IEndpointConfiguration EndpointConfig)
        {
            if (EndpointConfig.GetType().Name.Contains("ADSConfiguration"))
            {
                ADSConfiguration config = null;

                config = (ADSConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.ADSConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();
                    if (ExistingConfig != null)
                    {
                        _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                        _context.SaveChanges();
                    }
                }
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (config.EndpointSystem.ListAttributeMappingColumns != null)
                    {
                        foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                        {
                            //Temp Fix:
                            item.EndPoint = null;
                            _context.Entry(item).State = EntityState.Added;
                        }

                    }
                    config.EndpointSystemId = config.EndpointSystem.Id;
                    _context.ADSConfiguration.Attach(config);

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }
            else if (EndpointConfig.GetType().Name.Contains("LNSConfiguration"))
            {
                LNSConfiguration config = null;
                config = (LNSConfiguration)EndpointConfig;
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.lnsConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();
                    if (ExistingConfig != null)
                    {
                        _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                        _context.SaveChanges();
                    }
                }

                using (IACMDBContext _context = new IACMDBContext())
                {

                    if (config.EndpointSystem.ListAttributeMappingColumns != null)
                    {
                        foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                        {
                            item.EndPoint = null;
                            _context.Entry(item).State = EntityState.Added;
                        }

                        _context.lnsConfiguration.Attach(config);
                        _context.Entry(config.EndpointSystem).State = EntityState.Modified;
                        _context.Entry(config).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            else if (EndpointConfig.GetType().Name.Contains("SAPConfiguration"))
            {
                SAPConfiguration config = null;
                config = (SAPConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.SapConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();
                    if (ExistingConfig != null)
                    {
                        _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                        _context.SaveChanges();
                    }
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (config.EndpointSystem.ListAttributeMappingColumns != null)
                    {
                        foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                        {
                            item.EndPoint = null;
                            _context.Entry(item).State = EntityState.Added;
                        }
                    }


                    _context.SapConfiguration.Attach(config);
                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;
                    _context.Entry(config).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            else if (EndpointConfig.GetType().Name.Contains("SAPEPConfiguration"))
            {
                SAPEPConfiguration config = null;
                config = (SAPEPConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.SapEpConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();

                    _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                    _context.SaveChanges();
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.SapEpConfiguration.Attach(config);

                    foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                    {
                        _context.Entry(item).State = EntityState.Added;
                    }

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }

            else if (EndpointConfig.GetType().Name.Contains("OracleEBSConfiguration"))
            {
                OracleEBSConfiguration config = null;
                config = (OracleEBSConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.OracleEBSConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();

                    _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                    _context.SaveChanges();
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.OracleEBSConfiguration.Attach(config);

                    foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                    {
                        _context.Entry(item).State = EntityState.Added;
                    }

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }
            else if (EndpointConfig.GetType().Name.Contains("OracleUserManagement"))
            {
                OracleUMConfiguration config = null;
                config = (OracleUMConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.OracleUMConfiguration.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();

                    _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                    _context.SaveChanges();
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.OracleUMConfiguration.Attach(config);

                    foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                    {
                        _context.Entry(item).State = EntityState.Added;
                    }

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }

            else if (EndpointConfig.GetType().Name.Contains("LinuxConfiguration"))
            {
                LinuxConfiguration config = null;
                config = (LinuxConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.linuxConfigurations.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();

                    _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                    _context.SaveChanges();
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.linuxConfigurations.Attach(config);

                    foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                    {
                        _context.Entry(item).State = EntityState.Added;
                    }

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }

            else if (EndpointConfig.GetType().Name.Contains("SqlConfiguration"))
            {
                SqlConfiguration config = null;
                config = (SqlConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.SqlConfiguration.Include(lq => lq.EndpointSystem).Include(lq => lq.EndpointSystem.ListAttributeMappingColumns).Where(b => b.Id == config.Id).FirstOrDefault();

                    _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                    _context.SaveChanges();
                }

                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.SqlConfiguration.Attach(config);

                    foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                    {
                        _context.Entry(item).State = EntityState.Added;
                    }

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }
            if (EndpointConfig.GetType().Name.Contains("InhouseSystemConfiguration"))
            {
                InhouseSystemConfiguration config = null;

                config = (InhouseSystemConfiguration)EndpointConfig;


                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ExistingConfig = _context.InhouseSystems.Include("EndpointSystem").Include("EndpointSystem.ListAttributeMappingColumns").Where(b => b.Id == config.Id).FirstOrDefault();
                    if (ExistingConfig != null)
                    {
                        _context.AttributeMappingColumnss.RemoveRange(ExistingConfig.EndpointSystem.ListAttributeMappingColumns);
                        _context.SaveChanges();
                    }
                }
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (config.EndpointSystem.ListAttributeMappingColumns != null)
                    {
                        foreach (var item in config.EndpointSystem.ListAttributeMappingColumns)
                        {
                            //Temp Fix:
                            item.EndPoint = null;
                            _context.Entry(item).State = EntityState.Added;
                        }

                    }
                    _context.InhouseSystems.Attach(config);

                    _context.Entry(config.EndpointSystem).State = EntityState.Modified;

                    _context.Entry(config).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }


        }

        //public void UpdateConfiguration_1(IEndpointConfiguration EndpointConfig)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            if (EndpointConfig.GetType().Name.Contains("ADSConfiguration"))
        //            {
        //                ADSConfiguration config = EndpointConfig as ADSConfiguration;

        //                var adConfig = _context.Entry(config);
        //                if (adConfig.State == EntityState.Detached)
        //                {
        //                    var set = _context.Set<ADSConfiguration>();
        //                    ADSConfiguration _ExistingAdConf = set.Find(config.Id);

        //                    if (_ExistingAdConf != null)
        //                    {
        //                        var _adEntry = _context.Entry(_ExistingAdConf);
        //                        _adEntry.State = EntityState.Modified;
        //                        _adEntry.CurrentValues.SetValues(config);
        //                    }
        //                }
        //            }
        //            else if (EndpointConfig.GetType().Name.Contains("SAPConfiguration"))
        //            {
        //                SAPConfiguration config = EndpointConfig as SAPConfiguration;

        //                var adConfig = _context.Entry(config);
        //                if (adConfig.State == EntityState.Detached)
        //                {
        //                    var set = _context.Set<SAPConfiguration>();
        //                    SAPConfiguration _ExistingConf = set.Find(config.Id);

        //                    if (_ExistingConf != null)
        //                    {
        //                        var _Entry = _context.Entry(_ExistingConf);
        //                        _Entry.State = EntityState.Modified;
        //                        _Entry.CurrentValues.SetValues(config);
        //                    }
        //                }
        //            }
        //            else if (EndpointConfig.GetType().Name.Contains("SAPEPConfiguration"))
        //            {
        //                SAPEPConfiguration config = EndpointConfig as SAPEPConfiguration;

        //                var _conf = _context.Entry(config);
        //                if (_conf.State == EntityState.Detached)
        //                {
        //                    var set = _context.Set<SAPEPConfiguration>();
        //                    SAPEPConfiguration _ExistingConf = set.Find(config.Id);

        //                    if (_ExistingConf != null)
        //                    {
        //                        var _Entry = _context.Entry(_ExistingConf);
        //                        _Entry.State = EntityState.Modified;
        //                        _Entry.CurrentValues.SetValues(config);
        //                    }
        //                }
        //            }
        //            else if (EndpointConfig.GetType().Name.Contains("LNSConfiguration"))
        //            {
        //                LNSConfiguration config = EndpointConfig as LNSConfiguration;

        //                var _conf = _context.Entry(config);
        //                if (_conf.State == EntityState.Detached)
        //                {
        //                    var set = _context.Set<LNSConfiguration>();
        //                    LNSConfiguration _ExistingConf = set.Find(config.Id);

        //                    if (_ExistingConf != null)
        //                    {
        //                        var _Entry = _context.Entry(_ExistingConf);
        //                        _Entry.State = EntityState.Modified;
        //                        _Entry.CurrentValues.SetValues(config);
        //                    }
        //                }
        //            }

        //            if (EndpointConfig.EndpointSystem != null)
        //            {
        //                EndPoint ep = EndpointConfig.EndpointSystem;


        //                var epEntry = _context.Entry(ep);
        //                if (epEntry.State == EntityState.Detached)
        //                {
        //                    var set = _context.Set<EndPoint>();
        //                    EndPoint attachedEndpoint = set.Find(ep.Id);

        //                    if (attachedEndpoint != null)
        //                    {
        //                        var attachedEntry = _context.Entry(attachedEndpoint);
        //                        attachedEntry.State = EntityState.Modified;
        //                        attachedEntry.CurrentValues.SetValues(ep);
        //                    }
        //                }


        //            }

        //            if (EndpointConfig.EndpointSystem.ListAttributeMappingColumns != null)
        //            {
        //                foreach (AttributeMappingColumns att in EndpointConfig.EndpointSystem.ListAttributeMappingColumns)
        //                {
        //                    var taskReviewEntry = _context.Entry(att);
        //                    if (taskReviewEntry.State == EntityState.Detached)
        //                    {
        //                        var set = _context.Set<AttributeMappingColumns>();
        //                        AttributeMappingColumns attachedAtt = set.Find(att.Id);

        //                        if (attachedAtt != null)
        //                        {
        //                            var attachedEntry = _context.Entry(attachedAtt);
        //                            attachedEntry.CurrentValues.SetValues(att);
        //                        }
        //                        else
        //                            _context.AttributeMappingColumnss.Add(att);
        //                    }


        //                }
        //            }

        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating Endpoint Configuration", ex);
        //        throw ex;
        //    }

        //}

        //public IEndpointConfiguration GetConfiguration(int EndpointSystemId)
        //{
        //    try
        //    {
        //        IEndpointConfiguration EndpointConfig = null;

        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            EndPoint ep = GetEndPointSystemByKey(EndpointSystemId);
        //            return GetConfiguration(ep);
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IEndpointConfiguration GetConfiguration(EndPoint endpoint)
        {
            try
            {
                IEndpointConfiguration EndpointConfig = null;

                using (IACMDBContext _context = new IACMDBContext())
                {

                    if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.ActiveDirectory)
                    {
                        EndpointConfig = _context.ADSConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.SAP)
                    {
                        EndpointConfig = _context.SapConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.SAPEnterprisePortal)
                    {
                        EndpointConfig = _context.SapEpConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);

                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.LotusNotes)
                    {
                        EndpointConfig = _context.lnsConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.InhouseSystems)
                    {
                        EndpointConfig = _context.InhouseSystems.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }

                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.OracleEBS)
                    {
                        EndpointConfig = _context.OracleEBSConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.Linux)
                    {
                        EndpointConfig = _context.linuxConfigurations.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);

                    }

                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.ORCLUM)
                    {
                        EndpointConfig = _context.OracleUMConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }
                    else if (endpoint.EndPointSystemType.EndpointSystemTypeName == EndpointType.CustomApplication)
                    {
                        EndpointConfig = _context.SqlConfiguration.Include("EndpointSystem").Include("EndpointSystem.EndPointSystemType").Include("EndpointSystem.ListAttributeMappingColumns").FirstOrDefault(b => b.EndpointSystem.Id == endpoint.Id);
                    }

                    return EndpointConfig;
                }
            }
            catch (Exception)
            {
                throw;
            }


        }

        public EndPoint GetEndPointSystemByKey(long EndPointSystemId)
        {
            EndPoint EndPointSystem = null;


            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystem = (from ep in _context.EndpointSystems.Include(lq => lq.Owner)
                                .Include("EndPointSystemType")

                                      where ep.Id == EndPointSystemId
                                      select ep).FirstOrDefault(); ;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return EndPointSystem;
        }
        public EndPoint GetEndPointSystemByHostName(string HostName)
        {
            EndPoint EndPointSystem = null;


            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystem = (from ep in _context.EndpointSystems
                                .Include("EndPointSystemType")
                                      where ep.DisplayName == HostName
                                      select ep).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return EndPointSystem;
        }


        public void DeleteEndPointSystem(int EndPointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPoint Endpoint = _context.EndpointSystems.Include(s => s.EndPointSystemType).Where(dd => dd.Id == EndPointSystemId).FirstOrDefault();
                    if (Endpoint != null)
                    {

                        switch (Endpoint.EndPointSystemType.EndpointSystemTypeName)
                        {
                            case EndpointType.CustomApplication:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_M_SqlConfiguration where EndpointSystem_EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.ActiveDirectory:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_ADS_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.LotusNotes:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_LNS_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.SAP:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_SAP_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.SAPEnterprisePortal:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_SAPEP_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.InhouseSystems:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_INH_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.OracleEBS:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_OEBS_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.Linux:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_Linux_Configuration where EndpointSystem_EndpointSystemId={0}", Endpoint.Id));
                                break;
                            case EndpointType.ORCLUM:
                                _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_Oracle11gUM_Configuration where EndpointSystemId={0}", Endpoint.Id));
                                break;
                        }
                        _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_M_AttributeMapping where EndpointSystemId={0}", Endpoint.Id));
                        _context.Database.ExecuteSqlCommand(string.Format("DELETE FROM DI_M_EndpointSystem where Id={0}", Endpoint.Id));
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("FK_dbo.DI_M_AdHocAccessRequest"))
                {
                    throw new Exception("This Endpoint is in use with AdhocReququest");
                }
                else if (ex.Message.Contains("FK_dbo.DI_M_ProvisioningForm"))
                { 
                    throw new Exception("This Endpoint is in use with Provisioning Form");
                }
                throw;
            }


        }

        public List<EndPoint> GetAllEndPointByType(int EndPointSystemTypeId, string HostName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<EndPoint> list = new List<EndPoint>();
                    if (!string.IsNullOrEmpty(HostName) && EndPointSystemTypeId > -1)
                        list = _context.EndpointSystems.Include(est => est.EndPointSystemType).Include(ow => ow.Owner).Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId && s.DisplayName == HostName).ToList();
                    else
                    {
                        if (EndPointSystemTypeId > -1)
                            list = _context.EndpointSystems.Include(est => est.EndPointSystemType).Include(ow => ow.Owner).Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId).ToList();
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }

        }

        public List<EndPoint> GetAllEndPointByType(int EndPointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<EndPoint> list = new List<EndPoint>();
                    if (EndPointSystemTypeId > -1)
                    {
                        list = _context.EndpointSystems
                            .Include("EndpointSystemType")
                            .Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId && s.IsTrustedSource != true)
                            .ToList();
                    }
                    return list;

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }

        }

        public Dictionary<long, string> GetAllEndPointByType(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(item => item.Description.Contains(searchString))
                        .Select(x => new { Key = x.Id, Value = x.Description })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EndPoint> GetAllEndPointByType(EndpointType EndpointSystemTypeName, string HostName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    long EndPointSystemTypeId = _context.EndpointSystemTypes.FirstOrDefault(ep => ep.EndpointSystemTypeName == EndpointSystemTypeName).Id;

                    List<EndPoint> list = new List<EndPoint>();
                    if (!string.IsNullOrEmpty(HostName) && EndPointSystemTypeId > -1)
                        list = _context.EndpointSystems.Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId && s.DisplayName == HostName).ToList();
                    else
                    {
                        if (EndPointSystemTypeId > -1)
                            list = _context.EndpointSystems.Where(s => s.EndpointSystemTypeId == EndPointSystemTypeId).ToList();
                    }
                    return list;

                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }

        }



        public List<EndpointInformation> GetEndpointsByDiUserid(long DIUserId)
        {
            try
            {
                EndpointInformation EndPointSystem = null;
                //string query = "DI_spGetEndPointSystemsByDIUserid";

                using (IACMDBContext _context = new IACMDBContext())
                {
                    var ADSquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                    join usr in _context.ADSUserInfos on endpointsys.Id equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked && !usr.IsAccountDisabled

                                    select new
                                    {
                                        Endpointsystemid = endpointsys.Id,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.EndpointSystemTypeName,
                                        UserName = usr.sAMAccountName
                                    }).ToList();

                    var SAPquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                    join usr in _context.SAPUserinfos on endpointsys.Id equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
                                    select new
                                    {
                                        Endpointsystemid = endpointsys.Id,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.EndpointSystemTypeName,
                                        UserName = usr.UserLogonName
                                    }).ToList();

                    var Lotusquery = (from endpointsys in _context.EndpointSystems
                                      join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                      join usr in _context.LotusUserInfos on endpointsys.Id equals usr.EndPointSystemId
                                      where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
                                      select new
                                      {
                                          Endpointsystemid = endpointsys.Id,
                                          EndPointSystemDescription = endpointsys.Description,
                                          endpointsys.TenantId,
                                          endpointsys.EndpointSystemTypeId,
                                          HostName = endpointsys.DisplayName,
                                          endpointsys.DisplayName,
                                          Name = endpointsystype.EndpointSystemTypeName,
                                          UserName = usr.ShortName
                                      }).ToList();

                    var SAPEPquery = (from endpointsys in _context.EndpointSystems
                                      join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                      join usr in _context.SAPEPUserInfos on endpointsys.Id equals usr.EndPointSystemId
                                      where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
                                      select new
                                      {
                                          Endpointsystemid = endpointsys.Id,
                                          EndPointSystemDescription = endpointsys.Description,
                                          endpointsys.TenantId,
                                          endpointsys.EndpointSystemTypeId,
                                          HostName = endpointsys.DisplayName,
                                          endpointsys.DisplayName,

                                          Name = endpointsystype.EndpointSystemTypeName,
                                          usr.UserName
                                      }).ToList();

                    var INHquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                    join usr in _context.InhouseUserInfos on endpointsys.Id equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserId && (usr.AccountType == AccountType.User || usr.AccountType == AccountType.Secondary) && !usr.IsLocked
                                    select new
                                    {
                                        Endpointsystemid = endpointsys.Id,
                                        EndPointSystemDescription = endpointsys.Description,
                                        endpointsys.TenantId,
                                        endpointsys.EndpointSystemTypeId,
                                        HostName = endpointsys.DisplayName,
                                        endpointsys.DisplayName,

                                        Name = endpointsystype.EndpointSystemTypeName,
                                        usr.UserName
                                    }).ToList();


                    //var listTotal = ADSquery.Union(SAPquery).Union(Lotusquery).Union(SAPEPquery);
                    var listTotal1 = ADSquery.Union(SAPquery);
                    var listTotal2 = listTotal1.Union(Lotusquery);
                    var listTotal3 = listTotal2.Union(SAPEPquery);
                    var listTotal = listTotal3.Union(INHquery);

                    if (listTotal.Count() > 0)
                    {
                        foreach (var tmpList in listTotal)
                        {
                            EndPointSystem = new EndpointInformation();

                            EndPointSystem.Endpointsystemid = CheckNull.AgainstInt(tmpList.Endpointsystemid);
                            EndPointSystem.Endpointsystemdescription = CheckNull.AgainstString(tmpList.EndPointSystemDescription);
                            EndPointSystem.TenantId = CheckNull.AgainstString(tmpList.TenantId); // AgainstInt
                            EndPointSystem.EndpointsystemType = CheckNull.AgainstString(tmpList.Name);
                            EndPointSystem.EndpointSystemTypeId = CheckNull.AgainstInt(tmpList.EndpointSystemTypeId);
                            EndPointSystem.HostName = CheckNull.AgainstString(tmpList.HostName);
                            EndPointSystem.UserName = CheckNull.AgainstString(tmpList.UserName);
                            EndPointSystem.Status = string.Empty;
                            EndPointlist.Add(EndPointSystem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndPointSystem", ex);
                throw ex;
            }
            return EndPointlist;
        }

        public EndPoint GetEndpointSystemByName(string EndpointSystemName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.FirstOrDefault(s => s.DisplayName == EndpointSystemName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdatePasswordresetcount(long EndpointId, string UserName)
        {
            try
            {
                //string query = "DI_spUpdatePasswordresetcount";

                int recordsAffected = 0;
                int _pwdResetCnt;
                using (IACMDBContext _context = new IACMDBContext())
                {
                    long _endPointSystemTypeId = _context.EndpointSystems.FirstOrDefault(eps => eps.Id == EndpointId).EndpointSystemTypeId.Value;
                    EndpointType _endPointName = _context.EndpointSystemTypes.FirstOrDefault(eps => eps.Id == _endPointSystemTypeId).EndpointSystemTypeName;

                    if (_endPointName == EndpointType.ActiveDirectory)
                    {
                        ADSUserInfo userInfo = null;
                        userInfo = _context.ADSUserInfos.FirstOrDefault(usr => usr.sAMAccountName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            _pwdResetCnt = userInfo.PwdResetCnt;
                            if (_pwdResetCnt == null)
                                userInfo.PwdResetCnt = 1;
                            else
                                userInfo.PwdResetCnt++;
                            _context.SaveChanges();
                            recordsAffected = 1;
                        }
                    }
                    else if (_endPointName == EndpointType.SAP)
                    {
                        SAPUserinfo userInfo = null;
                        userInfo = _context.SAPUserinfos.FirstOrDefault(usr => usr.UserLogonName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            _pwdResetCnt = userInfo.PwdResetCnt;
                            if (_pwdResetCnt == null)
                                userInfo.PwdResetCnt = 1;
                            else
                                userInfo.PwdResetCnt++;
                            _context.SaveChanges();
                            recordsAffected = 1;
                        }
                    }
                    else if (_endPointName == EndpointType.LotusNotes)
                    {
                        LotusUserInfo userInfo = null;
                        userInfo = _context.LotusUserInfos.FirstOrDefault(usr => usr.ShortName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            _pwdResetCnt = userInfo.PwdResetCnt;
                            if (_pwdResetCnt == null)
                                userInfo.PwdResetCnt = 1;
                            else
                                userInfo.PwdResetCnt++;
                            _context.SaveChanges();
                            recordsAffected = 1;
                        }
                    }
                    else if (_endPointName == EndpointType.SAPEnterprisePortal)
                    {
                        SAPEPUserInfo userInfo = null;
                        userInfo = _context.SAPEPUserInfos.FirstOrDefault(usr => usr.UserName == UserName && usr.EndPointSystemId == EndpointId);
                        if (userInfo != null)
                        {
                            _pwdResetCnt = userInfo.PwdResetCnt;
                            if (_pwdResetCnt == null)
                                userInfo.PwdResetCnt = 1;
                            else
                                userInfo.PwdResetCnt++;
                            _context.SaveChanges();
                            recordsAffected = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while updating Password reset count.", ex);
                throw ex;
            }
        }

        public IList<ExceptionReconGridData> GetExceptionReconData(EndpointType _type)
        {
            IList<ExceptionReconGridData> listRecon = new List<ExceptionReconGridData>();

            IList<UserToEndPointsReportData> retList = new List<UserToEndPointsReportData>();
            IList<User> usersList = new List<User>();
            UserToEndPointsReportData aEPDetails = new UserToEndPointsReportData();
            ADSUserInfo adsDetail = new ADSUserInfo();
            Organisation orgDetail = new Organisation();

            IList<ADSUserInfo> listADSUserInfo = new List<ADSUserInfo>();
            IList<SAPUserinfo> listSAPUserInfo = new List<SAPUserinfo>();
            IList<LotusUserInfo> listLNSUserInfo = new List<LotusUserInfo>();

            try
            {
                switch (_type)
                {
                    case EndpointType.ActiveDirectory:
                        using (IACMDBContext _context = new IACMDBContext())
                        {
                            listRecon = (from ad in _context.ADSUserInfos

                                         where ad.Version == 2
                                         select new ExceptionReconGridData
                                         {
                                             EndpointSystemType = ad.EndPointSystem.EndPointSystemType.DisplayName,
                                             EndpointSystemName = ad.EndPointSystem.DisplayName,
                                             AccountName = ad.sAMAccountName,
                                             DisplayName = ad.displayName,
                                             CreatedDate = ad.whenCreated.Value,
                                             LastModifiedDate = null,
                                             ExceptionType = "Created",
                                             EndpointSystemId = ad.EndPointSystem.Id,
                                             EndpointUserId = ad.Id,
                                             UserId = ad.DIUserId,
                                             Version = ad.Version

                                         }).Union(from adHistory in _context.ADSUserInfoHistorys
                                                  from ep in _context.EndpointSystems
                                                  where adHistory.EndPointSystemId == ep.Id
                                                  from eptype in _context.EndpointSystemTypes
                                                  where ep.EndpointSystemTypeId == eptype.Id
                                                  where adHistory.Version == 2
                                                  select new ExceptionReconGridData
                                                  {
                                                      EndpointSystemType = ep.EndPointSystemType.DisplayName,
                                                      EndpointSystemName = ep.DisplayName,
                                                      AccountName = adHistory.sAMAccountName,
                                                      DisplayName = adHistory.displayName,
                                                      CreatedDate = adHistory.whenCreated.Value,
                                                      LastModifiedDate = adHistory.AuditDate,
                                                      ExceptionType = "Deleted",
                                                      EndpointSystemId = adHistory.EndPointSystemId,
                                                      EndpointUserId = adHistory.UserId,
                                                      UserId = adHistory.DIUserId,
                                                      Version = adHistory.Version
                                                  }
                            ).ToList();
                        }
                        break;

                    case EndpointType.LotusNotes:
                        using (IACMDBContext _context = new IACMDBContext())
                        {
                            listRecon = (from lt in _context.LotusUserInfos

                                         where lt.Version == 2
                                         select new ExceptionReconGridData
                                         {
                                             EndpointSystemType = lt.EndPointSystem.EndPointSystemType.DisplayName,
                                             EndpointSystemName = lt.EndPointSystem.DisplayName,
                                             AccountName = lt.ShortName,
                                             DisplayName = lt.FullName,
                                             CreatedDate = lt.CreatedDate,
                                             LastModifiedDate = null,
                                             ExceptionType = "Created",
                                             EndpointSystemId = lt.EndPointSystem.Id,
                                             EndpointUserId = lt.Id,
                                             UserId = lt.DIUserId,
                                             Version = lt.Version
                                         }).Union(from lnHistory in _context.LotusUserHistoryInfos
                                                  from ep in _context.EndpointSystems
                                                  where lnHistory.EndPointSystemId == ep.Id
                                                  from eptype in _context.EndpointSystemTypes
                                                  where ep.EndpointSystemTypeId == eptype.Id
                                                  where lnHistory.Version == 2
                                                  select new ExceptionReconGridData
                                                  {
                                                      EndpointSystemType = ep.EndPointSystemType.DisplayName,
                                                      EndpointSystemName = ep.DisplayName,
                                                      AccountName = lnHistory.ShortName,
                                                      DisplayName = lnHistory.FullName,
                                                      LastModifiedDate = lnHistory.AuditDate,
                                                      ExceptionType = "Deleted",
                                                      EndpointSystemId = lnHistory.EndPointSystemId,
                                                      EndpointUserId = lnHistory.UserId,
                                                      UserId = lnHistory.DIUserId,
                                                      Version = lnHistory.Version
                                                  }
                            ).ToList();
                        }
                        break;

                    case EndpointType.SAP:
                        using (IACMDBContext _context = new IACMDBContext())
                        {
                            listRecon = (from sap in _context.SAPUserinfos

                                         where sap.Version == 2
                                         select new ExceptionReconGridData
                                         {
                                             EndpointSystemType = sap.EndPointSystem.EndPointSystemType.DisplayName,
                                             EndpointSystemName = sap.EndPointSystem.DisplayName,
                                             AccountName = sap.UserLogonName,
                                             DisplayName = sap.Firstname,
                                             CreatedDate = sap.CreatedDate,
                                             LastModifiedDate = null,
                                             ExceptionType = "Created",
                                             EndpointSystemId = sap.EndPointSystem.Id,
                                             EndpointUserId = sap.Id,
                                             UserId = sap.DIUserId,
                                             Version = sap.Version
                                         }).Union(from sapHistory in _context.SAPUserHistoryinfos
                                                  from ep in _context.EndpointSystems
                                                  where sapHistory.EndPointSystemId == ep.Id
                                                  from eptype in _context.EndpointSystemTypes
                                                  where ep.EndpointSystemTypeId == eptype.Id
                                                  where sapHistory.Version == 2
                                                  select new ExceptionReconGridData
                                                  {
                                                      EndpointSystemType = ep.EndPointSystemType.DisplayName,
                                                      EndpointSystemName = ep.DisplayName,
                                                      AccountName = sapHistory.UserLogonName,
                                                      DisplayName = sapHistory.Firstname,
                                                      LastModifiedDate = sapHistory.AuditDate,
                                                      ExceptionType = "Deleted",
                                                      EndpointSystemId = sapHistory.EndPointSystemId,
                                                      EndpointUserId = sapHistory.UserId,
                                                      UserId = sapHistory.DIUserId,
                                                      Version = sapHistory.Version
                                                  }
                            ).ToList();
                        }
                        break;

                    case EndpointType.SAPEnterprisePortal:
                        using (IACMDBContext _context = new IACMDBContext())
                        {
                            listRecon = (from sapep in _context.SAPEPUserInfos

                                         where sapep.Version == 2
                                         select new ExceptionReconGridData
                                         {
                                             EndpointSystemType = sapep.EndPointSystem.EndPointSystemType.DisplayName,
                                             EndpointSystemName = sapep.EndPointSystem.DisplayName,
                                             AccountName = sapep.UserName,
                                             DisplayName = sapep.DisplayName,
                                             CreatedDate = sapep.CreatedDate,
                                             LastModifiedDate = null,
                                             ExceptionType = "Created",
                                             EndpointSystemId = sapep.EndPointSystem.Id,
                                             EndpointUserId = sapep.Id,
                                             UserId = sapep.DIUserId,
                                             Version = sapep.Version
                                         }).Union(from sapepHistory in _context.SAPEPUserInfoHistorys
                                                  from ep in _context.EndpointSystems
                                                  where sapepHistory.EndPointSystemId == ep.Id
                                                  from eptype in _context.EndpointSystemTypes
                                                  where ep.EndpointSystemTypeId == eptype.Id
                                                  where sapepHistory.Version == 2
                                                  select new ExceptionReconGridData
                                                  {
                                                      EndpointSystemType = ep.EndPointSystemType.DisplayName,
                                                      EndpointSystemName = ep.DisplayName,
                                                      AccountName = sapepHistory.UserName,
                                                      DisplayName = sapepHistory.DisplayName,
                                                      LastModifiedDate = sapepHistory.AuditDate,
                                                      ExceptionType = "Deleted",
                                                      EndpointSystemId = sapepHistory.EndPointSystemId,
                                                      EndpointUserId = sapepHistory.UserId,
                                                      UserId = sapepHistory.DIUserId,
                                                      Version = sapepHistory.Version
                                                  }
                            ).ToList();
                        }
                        break;

                    case EndpointType.InhouseSystems:
                        using (IACMDBContext _context = new IACMDBContext())
                        {
                            listRecon = (from inhs in _context.InhouseUserInfos

                                         where inhs.Version == 2
                                         select new ExceptionReconGridData
                                         {
                                             //EndpointSystemType = inhs.EndPointSystem.EndPointSystemType.EndpointSystemTypeName,
                                             //EndpointSystemName = inhs.EndPointSystem.HostName,
                                             AccountName = inhs.UserName,
                                             DisplayName = inhs.DisplayName,
                                             CreatedDate = inhs.CreatedDate,
                                             LastModifiedDate = null,
                                             ExceptionType = "Created",
                                             EndpointSystemId = inhs.EndPointSystemId,
                                             EndpointUserId = inhs.Id,
                                             UserId = inhs.DIUserId,
                                             Version = inhs.Version
                                         }
                            ).ToList();
                        }
                        break;

                }
            }
            catch (Exception)
            {
                throw;
            }
            return listRecon;
        }

        public EndPoint GetEndpointByDiscoveryProcessId(int DiscoveryProcessId)
        {
            EndPoint _endPoint = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DiscoveryProcess _discoveryProcess = _context.DiscoveryProcess.FirstOrDefault(dis => dis.Id == DiscoveryProcessId);
                    if (_discoveryProcess != null)
                    {
                        _endPoint = _discoveryProcess.Endpoint;
                        if (_endPoint == null && _discoveryProcess.Endpointsystemid > 0)
                            _endPoint = _context.EndpointSystems.FirstOrDefault(ep => ep.Id == _discoveryProcess.Endpointsystemid);
                    }
                }
                return _endPoint;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DiscoveryJobGridData GetDiscoveryJobGridDataByEndpointSystemId(int EndpointSystemId)
        {
            return null;
            //TODO:CHECK HARI
            //using (IACMDBContext _context = new IACMDBContext())
            //{
            //    var discoveryJobGridDataList = (from sch in _context.Scheduler
            //                   .Include("DiscoveryProcess")
            //                                    join eps in _context.EndpointSystems on sch.DiscoveryProcess.Endpointsystemid equals eps.EndpointSystemId
            //                                    where sch.DiscoveryProcess.Endpoint.EndpointSystemId == EndpointSystemId
            //                                    select new DiscoveryJobGridData()
            //                                    {
            //                                        DiscoveryProcessId = sch.DiscoveryProcessId,
            //                                        EndpointSystemId = eps.EndpointSystemId,
            //                                        EndpointSystemName = eps.HostName,
            //                                        Message = sch.DiscoveryProcess.Message,
            //                                        ProcessStartAt = sch.DiscoveryProcess.ProcessStartAt,
            //                                        ProcessEndAt = sch.DiscoveryProcess.ProcessEndAt,
            //                                        RunDate = sch.RunDate,
            //                                        Status = sch.DiscoveryProcess.Status
            //                                    }).ToList();

            //    if (discoveryJobGridDataList.Count <= 0)
            //        return null;

            //    return discoveryJobGridDataList.FirstOrDefault(); // First();
            //}


        }

        public IList<EndPointsOfType> getListofEndPoints(int endpointTypeId)
        {
            IList<EndPointsOfType> returnList = new List<EndPointsOfType>();
            try
            {
                using (IACMDBContext _iacmDBContext = new IACMDBContext())
                {
                    if (endpointTypeId == 0)
                    {
                        returnList = (from eps in _iacmDBContext.EndpointSystems
                                      select new EndPointsOfType
                                      {
                                          endPointId = eps.Id,
                                          endPointHostName = eps.DisplayName,
                                          endPointTypeName = eps.EndPointSystemType.DisplayName
                                      }).ToList();

                    }
                    else
                    {
                        returnList = (from eps in _iacmDBContext.EndpointSystems
                                      where eps.EndpointSystemTypeId == endpointTypeId
                                      select new EndPointsOfType
                                      {
                                          endPointId = eps.Id,
                                          endPointHostName = eps.DisplayName,
                                          endPointTypeName = eps.EndPointSystemType.DisplayName
                                      }).ToList();
                    }

                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<EndPointPwdResetCountReportData> getPwdResetCountList(int endPointTypeId)
        {
            IList<EndPointPwdResetCountReportData> returnList = new List<EndPointPwdResetCountReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    switch (endPointTypeId)
                    {
                        case (int)EndpointType.ActiveDirectory:
                            returnList = (from adsUserInfo in _iacmdbcontext.ADSUserInfos
                                          select new EndPointPwdResetCountReportData
                                          {
                                              endPointUserId = adsUserInfo.Id,
                                              endPointType = adsUserInfo.EndPointSystem.EndPointSystemType.DisplayName,
                                              endPointPwdResetCount = adsUserInfo.PwdResetCnt,
                                              endPointUserName = adsUserInfo.sAMAccountName
                                          }).ToList();
                            break;
                        case (int)EndpointType.SAP:
                            returnList = (from sapUserInfo in _iacmdbcontext.SAPUserinfos
                                          select new EndPointPwdResetCountReportData
                                          {
                                              endPointUserId = sapUserInfo.Id,
                                              endPointType = sapUserInfo.EndPointSystem.EndPointSystemType.DisplayName,
                                              endPointPwdResetCount = sapUserInfo.PwdResetCnt,
                                              endPointUserName = sapUserInfo.UserLogonName
                                          }).ToList();
                            break;
                        case (int)EndpointType.LotusNotes:
                            returnList = (from lnsUserInfo in _iacmdbcontext.LotusUserInfos
                                          select new EndPointPwdResetCountReportData
                                          {
                                              endPointUserId = lnsUserInfo.Id,
                                              endPointType = lnsUserInfo.EndPointSystem.EndPointSystemType.DisplayName,
                                              endPointPwdResetCount = lnsUserInfo.PwdResetCnt,
                                              endPointUserName = lnsUserInfo.ShortName
                                          }).ToList();
                            break;

                    }
                }

                return returnList;
            }
            catch (Exception)
            {

                throw;
            }
        }



        public IList<UsersByEndPointReportData> getAllInActiveUsersByEndPoint(int endPointTypeId)
        {
            IList<UsersByEndPointReportData> returnList = new List<UsersByEndPointReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    switch (endPointTypeId)
                    {
                        case (int)EndpointType.ActiveDirectory:
                            returnList = (from ep in _iacmdbcontext.ADSUserInfos
                                          join usrs in _iacmdbcontext.Users
                                          on ep.DIUserId equals usrs.Id
                                          where usrs.IsActive == false &&
                                          usrs.Id != 1
                                          select new UsersByEndPointReportData
                                          {
                                              endPointName = ep.EndPointSystem.Description,
                                              userDisplayName = usrs.DisplayName,
                                              userEmployeeId = usrs.EmployeeId,
                                              userId = usrs.Id,
                                              userMailId = usrs.EmailId,
                                              userUserName = ep.sAMAccountName,
                                              usrFirstName = usrs.FirstName,
                                              usrLastName = usrs.LastName
                                          }).ToList();
                            break;
                        case (int)EndpointType.SAP:
                            returnList = (from ep in _iacmdbcontext.SAPUserinfos
                                          join usrs in _iacmdbcontext.Users
                                          on ep.DIUserId equals usrs.Id
                                          where usrs.IsActive == false &&
                                          usrs.Id != 1
                                          select new UsersByEndPointReportData
                                          {
                                              endPointName = ep.EndPointSystem.Description,
                                              userDisplayName = usrs.DisplayName,
                                              userEmployeeId = usrs.EmployeeId,
                                              userId = usrs.Id,
                                              userMailId = usrs.EmailId,
                                              userUserName = ep.UserLogonName,
                                              usrFirstName = usrs.FirstName,
                                              usrLastName = usrs.LastName
                                          }).ToList();
                            break;
                        case (int)EndpointType.LotusNotes:
                            returnList = (from ep in _iacmdbcontext.LotusUserInfos
                                          join usrs in _iacmdbcontext.Users
                                          on ep.DIUserId equals usrs.Id
                                          where usrs.IsActive == false &&
                                          usrs.Id != 1
                                          select new UsersByEndPointReportData
                                          {
                                              endPointName = ep.EndPointSystem.Description,
                                              userDisplayName = usrs.DisplayName,
                                              userEmployeeId = usrs.EmployeeId,
                                              userId = usrs.Id,
                                              userMailId = usrs.EmailId,
                                              userUserName = ep.ShortName,
                                              usrFirstName = usrs.FirstName,
                                              usrLastName = usrs.LastName
                                          }).ToList();
                            break;

                        case (int)EndpointType.SAPEnterprisePortal:
                            returnList = (from ep in _iacmdbcontext.SAPEPUserInfos
                                          join usrs in _iacmdbcontext.Users
                                          on ep.DIUserId equals usrs.Id
                                          where usrs.IsActive == false &&
                                          usrs.Id != 1
                                          select new UsersByEndPointReportData
                                          {
                                              endPointName = ep.EndPointSystem.Description,
                                              userDisplayName = usrs.DisplayName,
                                              userEmployeeId = usrs.EmployeeId,
                                              userId = usrs.Id,
                                              userMailId = usrs.EmailId,
                                              userUserName = ep.UserName,
                                              usrFirstName = usrs.FirstName,
                                              usrLastName = usrs.LastName
                                          }).ToList();
                            break;
                    }
                }
                return returnList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSComputerObjectsReportData> getAllADSComputerObjects()
        {
            IList<ADSComputerObjectsReportData> returnList = new List<ADSComputerObjectsReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    returnList = (from objs in _iacmdbcontext.ADSComputerInfo
                                  join eps in _iacmdbcontext.EndpointSystems
                                  on objs.EndPointSystemId equals eps.Id
                                  select new ADSComputerObjectsReportData
                                  {
                                      objectID = objs.Id,
                                      objectHostName = eps.DisplayName,
                                      objectInstitutionName = objs.TenantId,
                                      objectLastLogoff = objs.lastLogoff.Value,
                                      objectLastLogOn = objs.lastLogon.Value,
                                      objectName = objs.sAMAccountName,
                                      objectWhenCreated = objs.whenCreated.Value
                                  }).ToList();
                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<ADSComputerObjectsReportData> getAllADSPrinterObjects()
        {
            IList<ADSComputerObjectsReportData> returnList = new List<ADSComputerObjectsReportData>();

            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    returnList = (from objs in _iacmdbcontext.ADSPrinterInfos
                                  join eps in _iacmdbcontext.EndpointSystems
                                  on objs.EndPointSystemId equals eps.Id
                                  select new ADSComputerObjectsReportData
                                  {
                                      objectID = objs.Id,
                                      objectHostName = eps.DisplayName,
                                      objectInstitutionName = objs.TenantId,
                                      objectName = objs.name,
                                      objectWhenCreated = objs.whenCreated.Value
                                  }).ToList();
                    return returnList;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<DiscoveryJobGridData> GetDiscoveryProcessByEndPoint(int EndPointSystemId)
        {
            IList<DiscoveryJobGridData> listDiscoveryProcess = new List<DiscoveryJobGridData>();

            //TODO: CHECK HARI
            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        listDiscoveryProcess = (from ep in _context.DiscoveryProcess
            //                                join sched in _context.Scheduler
            //                                on ep.DiscoveryProcessId equals sched.DiscoveryProcessId
            //                                where ep.Endpointsystemid == EndPointSystemId
            //                                select new DiscoveryJobGridData()
            //                                {

            //                                    DiscoveryProcessId = ep.DiscoveryProcessId,
            //                                    ProcessEndAt = ep.ProcessEndAt,
            //                                    ProcessStartAt = ep.ProcessStartAt,
            //                                    Message = ep.Message,
            //                                    Status = ep.Status,
            //                                    RunDate = sched.RunDate,
            //                                    Default = ep.Default,
            //                                    ProcessName = ep.ProcessName,
            //                                    Description = ep.Description,
            //                                    SchedulerID = sched.SchedulerId,
            //                                }
            //                                ).ToList<DiscoveryJobGridData>();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return listDiscoveryProcess;
        }

        public EndPoint GetTrustedSourceEndPoint()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Include("EndPointSystemType").FirstOrDefault(item => item.IsTrustedSource);
                }
            }
            catch
            {
                throw;
            }
        }

        public List<AttributeMappingColumns> GetAttributeMappingColumns(long _systemId)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.AttributeMappingColumnss.Where(item => item.EndPoint.Id == _systemId).ToList();
            }
        }

        public List<string> GetNonConnectedAccount(long DIUserID, LockScenario lockScenario)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var INHquery = (from endpointsys in _context.EndpointSystems
                                    join endpointsystype in _context.EndpointSystemTypes on endpointsys.EndpointSystemTypeId equals endpointsystype.Id
                                    join usr in _context.InhouseUserInfos on endpointsys.Id equals usr.EndPointSystemId
                                    where usr.DIUserId == DIUserID && usr.AccountType == AccountType.User && usr.LockScenario == lockScenario
                                    select new
                                    {
                                        SystemName = endpointsys.DisplayName,
                                        UserLogin = usr.UserName
                                    }).ToList();
                    return INHquery.Select(item => item.SystemName + "," + item.UserLogin).Distinct().ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllEndPointSystemTypes()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystemTypes.Where(item => item.IsActive == true)
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllEndPointsBySelecetedSystemTypes(string searchString, List<long> EndpointTypeIds)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(item => item.DisplayName.Contains(searchString) && EndpointTypeIds.Contains(item.EndPointSystemType.Id))
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public Dictionary<long, string> GetAllAttestationEndPointsBySelecetedSystemTypes(string searchString, List<long> EndpointTypeIds)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Where(item => item.DisplayName.Contains(searchString) && EndpointTypeIds.Contains(item.EndPointSystemType.Id) && item.IncludeInAttestation == true && item.IsActive == true)
                        .Select(x => new { Key = x.Id, Value = x.DisplayName })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateDiUserIdWithMappingInfo(IList<int> UserIdList, int DiUserId, string MapTo, int EndPointSystemId, EndpointType EndPointSystemType, string UserName, AccountType accountType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    foreach (int UserId in UserIdList)
                    {
                        User DIUser = _context.Users.FirstOrDefault(diusr => diusr.Id == DiUserId);
                        if (EndPointSystemType == EndpointType.ActiveDirectory)
                        {
                            ADSUserInfo ADSUInfo = _context.ADSUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                                   ENDPT.EndPointSystemId == EndPointSystemId);
                            if (ADSUInfo != null)
                            {
                                ADSUInfo.MappingType = "Manual";
                                ADSUInfo.DIUserId = DiUserId;
                                ADSUInfo.AccountType = accountType;
                                ADSUInfo.IfUnmappedMapTo = MapTo;
                                ADSUInfo.LastModifiedDate = DateTime.Today;
                                ADSUInfo.LastModifiedBy = UserName;

                                if (accountType == AccountType.User)
                                    DIUser.UserName = ADSUInfo.sAMAccountName;

                                _context.SaveChanges();
                            }
                        }
                        else if (EndPointSystemType == EndpointType.LotusNotes)
                        {
                            LotusUserInfo LOTUSUInfo = _context.LotusUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                                ENDPT.EndPointSystemId == EndPointSystemId);
                            if (LOTUSUInfo != null)
                            {
                                LOTUSUInfo.DIUserId = DiUserId;
                                LOTUSUInfo.LastModifiedDate = DateTime.Today;
                                LOTUSUInfo.MappingType = "Manual";
                                LOTUSUInfo.AccountType = accountType;
                                LOTUSUInfo.IfUnmappedMapTo = MapTo;
                                LOTUSUInfo.LastModifiedBy = UserName;
                                _context.SaveChanges();
                            }
                        }

                        else if (EndPointSystemType == EndpointType.SAP)
                        {
                            SAPUserinfo SAPUInfo = _context.SAPUserinfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                                ENDPT.EndPointSystemId == EndPointSystemId);
                            if (SAPUInfo != null)
                            {
                                SAPUInfo.DIUserId = DiUserId;
                                SAPUInfo.LastModifiedDate = DateTime.Today;
                                SAPUInfo.MappingType = "Manual";
                                SAPUInfo.AccountType = accountType;
                                SAPUInfo.IfUnmappedMapTo = MapTo;
                                SAPUInfo.LastModifiedBy = UserName;
                                _context.SaveChanges();
                            }
                        }
                        else if (EndPointSystemType == EndpointType.SAPEnterprisePortal)
                        {
                            SAPEPUserInfo SAPEPUInfo = _context.SAPEPUserInfos.FirstOrDefault(ENDPT => ENDPT.Id == UserId &&
                                                                                                ENDPT.EndPointSystemId == EndPointSystemId);
                            if (SAPEPUInfo != null)
                            {
                                SAPEPUInfo.DIUserId = DiUserId;
                                SAPEPUInfo.LastModifiedDate = DateTime.Today;
                                SAPEPUInfo.MappingType = "Manual";
                                SAPEPUInfo.AccountType = accountType;
                                SAPEPUInfo.IfUnmappedMapTo = MapTo;
                                SAPEPUInfo.LastModifiedBy = UserName;
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Updating The DiUserId ", ex);
                throw;
            }
        }

        public bool IsEndpointNameExists(string endpointName, int endpointId, int EpSystemID)
        {
            try
            {
                using (IACMDBContext db = new IACMDBContext())
                {
                    if ((endpointId == 0) || (endpointId == -1))
                    {
                        return db.EndpointSystems.Any(item => item.DisplayName.Equals(endpointName, StringComparison.CurrentCultureIgnoreCase));
                    }
                    if (EpSystemID == 0)
                        return db.EndpointSystems.Any((item => item.DisplayName.Equals(endpointName, StringComparison.CurrentCultureIgnoreCase) && item.EndpointSystemTypeId == endpointId));
                    else
                        return db.EndpointSystems.Any((item => item.DisplayName.Equals(endpointName, StringComparison.CurrentCultureIgnoreCase) && item.EndpointSystemTypeId == endpointId && item.Id !=EpSystemID));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Added by Connector Team

        private int GetPrimaryKey(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var myObject = entry.Entity;
            var property =
                myObject.GetType()
                    .GetProperties()
                    .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));
            return (int)property.GetValue(myObject, null);
        }

        public void DeleteAllRecords<T>() where T : class
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    _iacmdbcontext.Set<T>().RemoveRange(_iacmdbcontext.Set<T>());
                    _iacmdbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void bulkInsert<T>(List<T> ListforMapping, string tableName, bool IsDeleteAndInsert = true, string DeleteCondition = "1=1") where T : IAggregateRoot
        {
            if (IsDeleteAndInsert)
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(string.Format("Delete {0} where {1}", tableName, DeleteCondition));
                }
            }

            SQLBulkUploader<T> Data = null;
            try
            {
                Data = new SQLBulkUploader<T>(tableName);
                Data.Flush(ListforMapping);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Data.Close();
            }
        }

        public bool getDatabyFunction<T1>(Func<T1, bool> Item, out object OutResult, bool IsDeepLoad = false, string Includes = "") where T1 : class
        {
            try
            {

                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    bool currentState = _iacmdbcontext.Configuration.LazyLoadingEnabled;

                    _iacmdbcontext.Configuration.LazyLoadingEnabled = !IsDeepLoad;

                    if (!string.IsNullOrEmpty(Includes))
                        OutResult = _iacmdbcontext.Set<T1>().Include(Includes).Where(Item).ToList();
                    else
                        OutResult = _iacmdbcontext.Set<T1>().Where(Item).ToList();
                    _iacmdbcontext.Configuration.LazyLoadingEnabled = currentState;

                    return true;
                }
            }
            catch (Exception ex)
            {
                OutResult = null;
                return false;
            }
        }

        public int AddNewData<T1>(T1 InsertingData) where T1 : class
        {
            try
            {

                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    _iacmdbcontext.Set<T1>().Add(InsertingData);
                    _iacmdbcontext.SaveChanges();

                    return GetPrimaryKey(_iacmdbcontext.Entry(InsertingData));
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<T> GetAllRecords<T>(Func<T, bool> Condition = null, string Includes = "") where T : class
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    //DbSet<T> dbSet = _iacmdbcontext.Set<T>();
                    if (Condition != null)
                    {
                        if (Includes != "")
                            return _iacmdbcontext.Set<T>().Include(Includes).Where(Condition).ToList();
                        return _iacmdbcontext.Set<T>().Where(Condition).ToList();
                    }
                    if (Includes != "")
                        return _iacmdbcontext.Set<T>().Include(Includes).ToList();
                    return _iacmdbcontext.Set<T>().ToList();
                }
            }
            catch (Exception)
            {
                return null;
                // throw;
            }
        }

        public void RenewData<T1>(IList<T1> lstValues) where T1 : class
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    _iacmdbcontext.Set<T1>().RemoveRange(_iacmdbcontext.Set<T1>().ToList());
                    _iacmdbcontext.Set<T1>().AddRange(lstValues);
                    _iacmdbcontext.SaveChanges();
                }
            }
            catch (Exception)
            {
                //  return null;
                // throw;
            }
        }

        public T1 GetDatabyId<T1>(long Id, string Includes = null) where T1 : class
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {
                    System.Data.Entity.Infrastructure.DbQuery<T1> Result = _iacmdbcontext.Set<T1>();// .Find(Id);
                    if (!string.IsNullOrEmpty(Includes))
                    {
                        foreach (string inc in Includes.Split(','))
                        {
                            Result = Result.Include(inc);
                        }
                    }
                    return ((DbSet<T1>)Result).Find(Id);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void DiscoveryChanges<T>(IList<T> otherValues = null, IList<T> NewValues = null, IList<T> DeletedValues = null, string PreExecutables = null) where T : class
        {
            try
            {
                using (IACMDBContext _iacmdbcontext = new IACMDBContext())
                {

                    if (PreExecutables != null)
                    {
                        try
                        {
                            _iacmdbcontext.Database.ExecuteSqlCommand(PreExecutables);
                        }
                        catch (Exception ex)
                        {
                            //NeedToConfirm
                            // Log.Provisioning.WriteMessage(LogLevel.INFO, "Invalid PreExecutable query hasbeen passed {" + PreExecutables + "}.");
                        }

                    }

                    if (otherValues != null)
                    {
                        foreach (T OtherItems in otherValues)
                        {
                            //var entitykey =EntityHelper.GetEntityKey<T>(_iacmdbcontext, OtherItems);
                            //if (_iacmdbcontext.Entry(OtherItems).State != EntityState.Detached)
                            //{
                            //    _iacmdbcontext.Set<T>().Attach(OtherItems);
                            //}
                            //    _iacmdbcontext.Entry(OtherItems).State = EntityState.Modified;

                            //    _iacmdbcontext.SaveChanges();

                            var entry = _iacmdbcontext.Entry(OtherItems);
                            var key = this.GetPrimaryKey(entry);

                            if (entry.State == EntityState.Detached)
                            {
                                var currentEntry = _iacmdbcontext.Set<T>().Find(key);
                                if (currentEntry != null)
                                {
                                    var attachedEntry = _iacmdbcontext.Entry(currentEntry);
                                    attachedEntry.CurrentValues.SetValues(OtherItems);
                                }
                                else
                                {
                                    _iacmdbcontext.Set<T>().Attach(OtherItems);
                                    entry.State = EntityState.Modified;
                                }
                            }


                            _iacmdbcontext.SaveChanges();

                            // T ExistingItem = _iacmdbcontext.Set<T>().Find(entitykey);
                            // var attachedEntry = _context.Entry(ExistingItem);
                            // attachedEntry.CurrentValues.SetValues(OtherItems);

                        }
                    }

                    if (DeletedValues != null)
                    {
                        foreach (T OtherItems in DeletedValues)
                        {
                            //  var entitykey = EntityHelper.GetEntityKey<T>(_iacmdbcontext, OtherItems);
                            //T ExistingItem = _iacmdbcontext.Set<T>().Find(entitykey);
                            //_iacmdbcontext.Set<T>().Remove(ExistingItem);

                            _iacmdbcontext.Entry(OtherItems).State = EntityState.Deleted;

                        }
                    }

                    if (NewValues != null)
                        _iacmdbcontext.Set<T>().AddRange(NewValues);

                    _iacmdbcontext.SaveChanges();
                }
            }
            catch (Exception)
            {


                // throw;
            }
        }

        #endregion


    }
}
