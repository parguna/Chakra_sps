using DeepIdentity.Model;
using System;
using System.IO;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class TestDA
    {

        public void ResetAttestationProcess(int processId)
        {
            try
            {
                string query = "UPDATE DI_M_AttestationProcess SET WorkflowStatus = NULL WHERE AttestationId = " + processId;
                string query1 = "DELETE FROM DI_M_AttestationReview_EndpointSystem_L3DetailHistory; " +
                                "DELETE FROM DI_M_AttestationReview_EndpointSystem_L3Detail; " +
                                "DELETE FROM DI_M_AttestationReview_EndpointSystem_L2DetailHistory; " +
                                "DELETE FROM DI_M_AttestationReview_EndpointSystem_L2Detail; " +
                                "DELETE FROM DI_M_ReviewEndpointSystems; " +
                                "DELETE FROM DI_M_TaskReviews; " +
                                "DELETE FROM DI_M_AttestationReview_EndpointSystemHistory; " +
                                "DELETE FROM DI_M_AttestationReview_EndpointSystem; " +
                                "DELETE FROM DI_M_AttestationReview; " +
                                "DELETE FROM DI_M_Task; " +
                                "DELETE FROM DI_M_EmailControl; " +
                                "DELETE FROM DI_M_Email; " +
                                "DELETE FROM DI_M_EmailHistory; " +
                                "DELETE FROM DI_M_InterimStore; " +
                                "DELETE FROM DI_Log_Service; ";
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand(query);
                    _context.Database.ExecuteSqlCommand(query1);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public long GetLatestTaskId()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var task = _context.Tasks.FirstOrDefault(item => item.IsCompleted == false && item.IsActive);
                    if (task != null)
                    {
                        return task.TaskId;
                    }
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetLatestAttestationProcessId()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var lastAttestation = _context.AttestationProcesses.OrderByDescending(item => item.LastModifiedDate).FirstOrDefault();

                    if (null != lastAttestation)
                        return lastAttestation.AttestationId.Value;

                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateTriggers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    foreach (var filePath in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "\\SqlScripts\\Triggers\\", "*.sql"))
                    {
                        _context.Database.ExecuteSqlCommand(File.ReadAllText(filePath));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
