using DI.Chakra.DataAccess;
using DI.Chakra.Model;
using DI.Chakra.Model.Discovery;
using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Model.List;

//using System.Linq.Dynamic;
namespace DI.Chakra.Facade
{
    public class ProcessFacade
    {
        public ProcessDA processDA { get; set; }
        public ProcessFacade()
        {
            processDA = new ProcessDA();
        }
        public void UpdateProcess(Process process)
        {
            try
            {
                processDA.UpdateProcess(process);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Process GetProcess(long ProcessTypeKeyId, ProcessType processType)
        {
            try
            {
                return processDA.GetProcess(ProcessTypeKeyId, processType);
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
                return processDA.AddProcessHistory(history);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int SaveProcess(Process process)
        {
            try
            {
                return processDA.SaveProcess(process);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ComplianceJobList> GetComplianceProcessJobs(out int count, int startRow, int PageSize, string filter)
        {
            try
            {
                return processDA.GetComplianceProcessJobs(out count, startRow, PageSize, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DiscoveryJobList> GetDiscoveryProcessJobs(out int count, int startRow, int PageSize, string filter)
        {
            try
            {
                return processDA.GetDiscoveryProcessJobs(out count, startRow, PageSize, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DiscoveryJobList> GetDiscoveryProcessJobsByType(int EndPointSystemId, out int count, int startRow, int PageSize, string filter)
        {
            try
            {
                return processDA.GetDiscoveryProcessJobsByType(EndPointSystemId, out count, startRow, PageSize, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ComplianceProcess GetComplianceProcess(int ProcessId)
        {
            try
            {
                return processDA.GetComplianceProcess(ProcessId);
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
                return processDA.GetDiscoveryProcess(ProcessId);
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
                processDA.DeleteDiscoveryProcess(ProcessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteComplianceProcess(int ProcessId)
        {
            try
            {
                processDA.DeleteComplianceProcess(ProcessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Scheduler GetScheduler(int ProcessId)
        {
            try
            {
                return processDA.GetScheduler(ProcessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Scheduler GetComplianceScheduler(int ProcessId)
        {
            try
            {
                return processDA.GetComplianceScheduler(ProcessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Scheduler GetAttestationScheduler(int ProcessId)
        {
            try
            {
                return processDA.GetAttestationScheduler(ProcessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #region Added by Yasila.H on 04.11.2015 for V5 Changes
        public void DeleteAttestationProcess(int processId)
        {
            try
            {
                processDA.DeleteAttestationProcess(processId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<AttestationJobList> GetAttestationProcessJobs(out int count, int startRow, int PageSize, string filter)
        {
            try
            {
                return processDA.GetAttestationProcessJobs(out count, startRow, PageSize, filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public AttestationProcess GetAttestationProcess(int attestaioonPocessId)
        {
            try
            {
                return processDA.GetAttestationProcess(attestaioonPocessId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        #endregion
    }
}
