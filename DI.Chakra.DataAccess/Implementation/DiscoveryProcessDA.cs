using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model.ReportData;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class DiscoveryProcessDA// : IDiscoveryProcessDA
    {
        IACMDbOperator iacmOperator = null;

        QueryExecutor executor = null;
        IACMDBContext oIACMDBContext;

        #region IDiscoveryDA Members

        public long SaveDiscoveryProcess(DiscoveryProcess _discoveryProcess)
        {
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    if (_discoveryProcess.Id == 0)
                    {
                        oIACMDBContext.DiscoveryProcess.Attach(_discoveryProcess);
                        oIACMDBContext.Entry(_discoveryProcess).State = System.Data.Entity.EntityState.Added;
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        //oIACMDBContext.DiscoveryProcess.Attach(_discoveryProcess);
                        //oIACMDBContext.Entry(_discoveryProcess).State = System.Data.Entity.EntityState.Modified;
                        //oIACMDBContext.SaveChanges();
                        var dbDiscoveryProcess = oIACMDBContext.DiscoveryProcess
                                                 .Include("EndpointObjects")
                                                 .Single(c => c.Id == _discoveryProcess.Id);

                        oIACMDBContext.Entry(dbDiscoveryProcess).CurrentValues.SetValues(_discoveryProcess);

                        //var v = (from x in dbGroup.EndpointObjects
                        //         where !_discoveryProcess.EndpointObjects.Contains(x)
                        //         select x).ToList();
                        var v = dbDiscoveryProcess.EndpointObjects.ToList();
                        foreach (var item in v)
                        {
                            dbDiscoveryProcess.EndpointObjects.Remove(item);
                        }

                        if (_discoveryProcess.EndpointObjects != null)
                        {
                            foreach (var endPoint in _discoveryProcess.EndpointObjects)
                            {
                                var item = dbDiscoveryProcess.EndpointObjects.SingleOrDefault(s => s.Id == endPoint.Id);
                                if (item != null)
                                    oIACMDBContext.Entry(item).CurrentValues.SetValues(endPoint);
                                else
                                {
                                    var dbUser = oIACMDBContext.EndpointObjects.Where(t => t.Id == endPoint.Id).FirstOrDefault();

                                    if (dbUser != null)
                                        dbDiscoveryProcess.EndpointObjects.Add(dbUser);
                                }
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }
                    return _discoveryProcess.Id;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public DiscoveryProcess GetDiscovery(long _processId)
        {
            try
            {
                 using (oIACMDBContext = new IACMDBContext())
                {
                    DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                    objDiscoveryProcess = oIACMDBContext.DiscoveryProcess.Find(_processId);
                    return objDiscoveryProcess;

                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public bool IsDicoveryProcessNameExists(string Name)
        {
            bool isExists = false;
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    DiscoveryProcess process = oIACMDBContext.DiscoveryProcess.Where(m => m.ProcessName == Name).FirstOrDefault();
                    if (process != null)
                    {
                        isExists = true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isExists;
        }
       
        public IList<DiscoveryProcessHistory> GetDiscoveryHistory(int _EndPointId)
        {
            //string query = "DI_spGetDiscoveryProcessHistory";
            List<DiscoveryProcessHistory> discoveryHistory = new List<DiscoveryProcessHistory>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    discoveryHistory = oIACMDBContext.DiscoveryProcessHistorys.Where(dis => dis.EndpointSystemId == _EndPointId &&
                                                                                            dis.Status == "Completed").ToList();
                    return discoveryHistory;
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching DiscoveryHistroy Data", ex);
                throw ex;
            }
        }



        public void UpdateDiscovery(DiscoveryProcess _discoveryProcess)
        {

            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    var dbDiscoveryProcess = oIACMDBContext.DiscoveryProcess
                                                 .Include("EndpointObjects")
                                                 .Single(c => c.Id == _discoveryProcess.Id);

                    oIACMDBContext.Entry(dbDiscoveryProcess).CurrentValues.SetValues(_discoveryProcess);

                    //var v = (from x in dbGroup.EndpointObjects
                    //         where !_discoveryProcess.EndpointObjects.Contains(x)
                    //         select x).ToList();
                    var v = dbDiscoveryProcess.EndpointObjects.ToList();
                    foreach (var item in v)
                    {
                        dbDiscoveryProcess.EndpointObjects.Remove(item);
                    }

                    if (_discoveryProcess.EndpointObjects != null)
                    {
                        foreach (var endPoint in _discoveryProcess.EndpointObjects)
                        {
                            var item = dbDiscoveryProcess.EndpointObjects.SingleOrDefault(s => s.Id == endPoint.Id);
                            if (item != null)
                                oIACMDBContext.Entry(item).CurrentValues.SetValues(endPoint);
                            else
                            {
                                var dbUser = oIACMDBContext.EndpointObjects.Where(t => t.Id == endPoint.Id).FirstOrDefault();

                                if (dbUser != null)
                                    dbDiscoveryProcess.EndpointObjects.Add(dbUser);
                            }
                        }
                    }
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
           
        }
        
        
        public DiscoveryProcess GetDiscoveryByDefaultEndpoint(int _endpointId)
        {
            try
            {
                using (oIACMDBContext = new IACMDBContext())
                {
                    DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                    objDiscoveryProcess = oIACMDBContext.DiscoveryProcess.Where(e => e.Endpointsystemid == _endpointId && e.Default).FirstOrDefault();
                    return objDiscoveryProcess;

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<DiscoveryReportData> getAllDiscoveryProcess(string _status)
        {
            //IList<DiscoveryReportData> listDiscoveryProcess = new List<DiscoveryReportData>();

            //try
            //{
            //    using (IACMDBContext _context = new IACMDBContext())
            //    {
            //        if (_status == "All")
            //        {
            //            listDiscoveryProcess = (from es in _context.EndpointSystems
            //                                    join disprcs in _context.DiscoveryProcess on es.EndpointSystemId equals disprcs.Endpointsystemid
            //                                    join esType in _context.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
            //                                    orderby esType.EndpointSystemTypeName, disprcs.Status
            //                                    select new DiscoveryReportData()
            //                                    {
            //                                        endPointSystemType = esType.DisplayName,
            //                                        endPointHostName = es.DisplayName,
            //                                        tenantId = es.TenantId,
            //                                        endPointDomainName = es.DomainName,
            //                                        endPointIPAddress = es.IpAddress,
            //                                        discoverStartAt = disprcs.ProcessStartAt,
            //                                        discoverEndAt = disprcs.ProcessEndAt,
            //                                        discoveryStatus = disprcs.Status

            //                                    }).ToList();
            //        }
            //        else
            //        {
            //            listDiscoveryProcess = (from es in _context.EndpointSystems
            //                                    join disprcs in _context.DiscoveryProcess on es.EndpointSystemId equals disprcs.Endpointsystemid
            //                                    join esType in _context.EndpointSystemTypes on es.EndpointSystemTypeId equals esType.EndpointSystemTypeId
            //                                    where (disprcs.Status == _status)
            //                                    orderby esType.EndpointSystemTypeName
            //                                    select new DiscoveryReportData()
            //                                    {
            //                                        endPointSystemType = esType.DisplayName,
            //                                        endPointHostName = es.DisplayName,
            //                                        tenantId = es.TenantId,
            //                                        endPointDomainName = es.DomainName,
            //                                        endPointIPAddress = es.IpAddress,
            //                                        discoverStartAt = disprcs.ProcessStartAt,
            //                                        discoverEndAt = disprcs.ProcessEndAt,
            //                                        discoveryStatus = disprcs.Status

            //                                    }).ToList();
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //return listDiscoveryProcess;

            return null;
        }

        #endregion

        public IList<DiscoveryReportData> getDiscoveryProcessHistoryByEP(int paramEndpointTypeId)
        {
            //try
            //{
            //    using (IACMDBContext _iacmdbcontext = new IACMDBContext())
            //    {
            //        return (from epType in _iacmdbcontext.EndpointSystemTypes
            //                join epSystem in _iacmdbcontext.EndpointSystems
            //                on epType.EndpointSystemTypeId equals epSystem.EndpointSystemTypeId
            //                join discHistory in _iacmdbcontext.DiscoveryProcessHistorys
            //                on epSystem.EndpointSystemId equals discHistory.EndpointSystemId
            //                where ((epType.EndpointSystemTypeId == paramEndpointTypeId) || paramEndpointTypeId == -1)
            //                select new DiscoveryReportData
            //                {
            //                    discoveryId = discHistory.DiscoveryProcessHistoryId,
            //                    endPointSystemType = epSystem.EndPointSystemType.DisplayName,
            //                    endPointDomainName = epSystem.DomainName,
            //                    endPointHostName = epSystem.DisplayName,
            //                    endPointIPAddress = epSystem.IpAddress,
            //                    discoverStartAt = discHistory.ProcessStartAt.Value,
            //                    discoverEndAt = discHistory.ProcessEndAt.Value,
            //                    discoveryStatus = discHistory.Status
            //                }).OrderByDescending(dis => dis.discoverEndAt.Value).ToList();//OrderBy(dis => dis.endPointSystemType).OrderBy(dis => dis.endPointDomainName).OrderByDescending(dis => dis.discoverEndAt.Value).ToList();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return null;
        }

        public DiscoveryProcess GetDefaultDiscoveryProcess(int EndpointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DiscoveryProcess.FirstOrDefault(discovery => discovery.Endpointsystemid == EndpointSystemId
                                                                        && discovery.Default);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
