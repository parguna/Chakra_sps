using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using DeepIdentity.Model.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using DeepIdentity.Model.List;

namespace DeepIdentity.DataAccess
{
    public class ProcessDA
    {
        public void UpdateProcess(Process _Process)
        {

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    Process objProcess = new Process();
                    objProcess = oIACMDBContext.Process.Include("Scheduler").FirstOrDefault(p => p.ProcessId == _Process.ProcessId);
                    //oIACMDBContext.Entry(objProcess.Scheduler).CurrentValues.SetValues(_Process.Scheduler);
                    oIACMDBContext.Entry(objProcess).CurrentValues.SetValues(_Process);
                    oIACMDBContext.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public Process GetProcess(int ProcessTypeKeyId, ProcessType processType)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.Process.Include("Scheduler").FirstOrDefault(item => item.ProcessTypeKeyId == ProcessTypeKeyId && item.ProcessType == processType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DiscoveryProcess GetDiscoveryProcess(int ProcessId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DiscoveryProcess.Include("EndPoint").Include("EndpointObjects").Include("EndPoint.EndPointSystemType").FirstOrDefault(item => item.DiscoveryProcessId == ProcessId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteDiscoveryProcess(int ProcessId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DiscoveryProcess discoveryProcess = _context.DiscoveryProcess.Where(item => item.DiscoveryProcessId == ProcessId).FirstOrDefault();
                    if (discoveryProcess != null)
                    {
                        Process process = _context.Process.Where(item => item.ProcessTypeKeyId == ProcessId && item.ProcessType == ProcessType.Discovery).FirstOrDefault();
                        if (process != null)
                        {
                            _context.Process.Remove(process);
                        }
                        _context.DiscoveryProcess.Remove(discoveryProcess);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int AddProcessHistory(ProcessHistory history)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.ProcessHistory.Add(history);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }

            return -1;
        }
        public int SaveProcess(Process process)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (process.ProcessId == 0)
                    {
                        oIACMDBContext.Process.Add(process);
                        // oIACMDBContext.Entry(process).State = System.Data.Entity.EntityState.Added;
                        oIACMDBContext.SaveChanges();
                    }
                    else
                    {
                        Process objProcess = new Process();
                        objProcess = oIACMDBContext.Process.Find(process.ProcessId);
                        oIACMDBContext.Entry(objProcess.Scheduler).CurrentValues.SetValues(process.Scheduler);
                        oIACMDBContext.Entry(objProcess).CurrentValues.SetValues(process);
                        oIACMDBContext.SaveChanges();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }

            return -1;
        }
        public List<DiscoveryJobList> GetDiscoveryProcessJobs(out int count, int startRow, int PageSize, string filter)
        {
            List<DiscoveryJobList> jobs = new List<DiscoveryJobList>();
            count = 0;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var query = from discovery in _context.DiscoveryProcess
                                join endpoint in _context.EndpointSystems
                                on discovery.Endpointsystemid equals endpoint.EndpointSystemId
                                join pr in _context.Process
                                on discovery.DiscoveryProcessId equals pr.ProcessTypeKeyId
                                join s in _context.Scheduler
                                on pr.Scheduler.SchedulerId equals s.SchedulerId
                                where pr.ProcessType == ProcessType.Discovery
                                select new DiscoveryJobList
                                {
                                    ProcessName = discovery.ProcessName,
                                    Description = discovery.Description,
                                    EndPointSystem = endpoint.DisplayName,
                                    occurence = s.Occurence.Value,
                                    DiscoveryProcessId = discovery.DiscoveryProcessId,
                                    Status = pr.Status,
                                    StartDateTime = s.StartDate + " " + s.StartTime
                                };
                    jobs = query.ToList();
                    //if (filter.Equals(string.Empty))
                    //{
                    //    count = query.Count();
                    //    jobs = query.OrderBy(i => i.DiscoveryProcessId).Skip(startRow).Take(PageSize).ToList();
                    //}
                    //else
                    //{
                    //    jobs = query.ToList().AsQueryable().Where(filter.ToString()).ToList();
                    //    count = jobs.Count;
                    //    jobs = jobs.Skip(startRow).Take(PageSize).ToList();
                    //}
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
            return jobs;
        }
        public List<DiscoveryJobList> GetDiscoveryProcessJobsByType(int EndPointSystemId, out int count, int startRow, int PageSize, string filter)
        {
            List<DiscoveryJobList> jobs = new List<DiscoveryJobList>();
            count = 0;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var query = from discovery in _context.DiscoveryProcess
                                join endpoint in _context.EndpointSystems
                                on discovery.Endpointsystemid equals endpoint.EndpointSystemId
                                join pr in _context.Process
                                on discovery.DiscoveryProcessId equals pr.ProcessTypeKeyId
                                join s in _context.Scheduler
                                on pr.Scheduler.SchedulerId equals s.SchedulerId
                                where pr.ProcessType == ProcessType.Discovery
                                && discovery.Endpoint.EndPointSystemType.EndpointSystemTypeId == EndPointSystemId
                                select new DiscoveryJobList
                                {
                                    ProcessName = discovery.ProcessName,
                                    Description = discovery.Description,
                                    EndPointSystem = endpoint.DisplayName,
                                    occurence = s.Occurence.Value,
                                    DiscoveryProcessId = discovery.DiscoveryProcessId,
                                    Status = pr.Status,
                                    StartDateTime = s.StartDate + " " + s.StartTime
                                };
                    jobs = query.ToList();
                    //if (filter.Equals(string.Empty))
                    //{
                    //    count = query.Count();
                    //    jobs = query.OrderBy(i => i.DiscoveryProcessId).Skip(startRow).Take(PageSize).ToList();
                    //}
                    //else
                    //{
                    //    jobs = query.ToList().AsQueryable().Where(filter.ToString()).ToList();
                    //    count = jobs.Count;
                    //    jobs = jobs.Skip(startRow).Take(PageSize).ToList();
                    //}
                }


            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
            return jobs;
        }
        public Scheduler GetScheduler(int ProcessId)
        {
            Scheduler sch = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var query = from discovery in _context.DiscoveryProcess
                                join pr in _context.Process
                                on discovery.DiscoveryProcessId equals pr.ProcessTypeKeyId
                                join s in _context.Scheduler
                                on pr.Scheduler.SchedulerId equals s.SchedulerId
                                where pr.ProcessType == ProcessType.Discovery
                                && discovery.DiscoveryProcessId == ProcessId
                                select s;

                    sch = query.ToList().FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
            return sch;
        }

        public Scheduler GetAttestationScheduler(int ProcessId)
        {
            Scheduler sch = null;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var query = from attestation in _context.AttestationProcesses
                                join pr in _context.Process
                                on attestation.AttestationProcessId equals pr.ProcessTypeKeyId
                                join s in _context.Scheduler
                                on pr.Scheduler.SchedulerId equals s.SchedulerId
                                where pr.ProcessType == ProcessType.Attestation
                                && attestation.AttestationProcessId == ProcessId
                                select s;

                    sch = query.ToList().FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
            return sch;
        }

        #region Added by Yasila.H on 04.11.2015 for V5 Changes
        public void DeleteAttestationProcess(int processId)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    AttestationProcess attestationProcess = context.AttestationProcesses.Where(item => item.AttestationProcessId == processId).FirstOrDefault();
                    if (attestationProcess != null)
                    {
                        Process process = context.Process.Where(item => item.ProcessTypeKeyId == processId && item.ProcessType == ProcessType.Attestation).FirstOrDefault();
                        if (process != null)
                        {
                            context.Process.Remove(process);
                        }
                        context.AttestationProcesses.Remove(attestationProcess);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttestationJobList> GetAttestationProcessJobs(out int count, int startRow, int PageSize, string filter)
        {
            List<AttestationJobList> jobs = new List<AttestationJobList>();
            count = 0;
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    var query = from attestationProcess in context.AttestationProcesses
                                join attestation  in context.Attestations
                                on attestationProcess.AttestationId equals attestation.AttestationId
                                join pr in context.Process
                                on attestationProcess.AttestationProcessId equals pr.ProcessTypeKeyId
                                join s in context.Scheduler
                                on pr.Scheduler.SchedulerId equals s.SchedulerId
                                where pr.ProcessType == ProcessType.Attestation
                                select new AttestationJobList
                                {
                                    ProcessName = attestationProcess.ProcessName,
                                    Description = attestationProcess.Description,
                                    AttestationName = attestation.AttestationName,
                                    occurence = s.Occurence.Value,
                                    AttestationProcessId = attestationProcess.AttestationProcessId,
                                    Status = pr.Status,
                                    StartDateTime = s.StartDate + " " + s.StartTime
                                };
                    jobs = query.ToList();
                    
                }

            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, ex.Message);
            }
            return jobs;
        }

        public AttestationProcess GetAttestationProcess(int attestaionProcessId)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    return context.AttestationProcesses.Include("Attestation").FirstOrDefault(item => item.AttestationProcessId == attestaionProcessId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
