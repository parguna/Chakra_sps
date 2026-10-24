using DeepIdentity.Logger;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeepIdentity.DataAccess
{
    public class HouseKeepingDA
    {
        public int PurgeAllTasksData(int NumberOfDays)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DateTime PurgeRangeDate = DateTime.Now.AddDays(-NumberOfDays);
                    List<Task> TasksToBePurged = (from task in _context.Tasks
                                                  where task.CreatedDate < PurgeRangeDate
                                                  select task
                                                  ).ToList();

                    foreach (Task purgeTask in TasksToBePurged)
                    {
                        _context.Tasks.Attach(purgeTask);
                        _context.Tasks.Remove(purgeTask);
                        _context.SaveChanges();
                    }

                    return TasksToBePurged.Count;
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured While Purging Tasks Related Data", ex.InnerException.ToString(), "HouseKeeping", 0, 0, "HouseKeeping Service");

            }

            return 0;
        }

  

        public int PurgeAllLogsData(int NumberOfDays)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DateTime PurgeRangeDate = DateTime.Now.AddDays(-NumberOfDays);

                    IList<DiscoveryLog> discoveryLogs = (from discoveryLog in _context.DiscoveryLog
                                                         where discoveryLog.LogDate < PurgeRangeDate
                                                         select discoveryLog).ToList();
                    _context.DiscoveryLog.RemoveRange(discoveryLogs);
                    _context.SaveChanges();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Discovery Log Data", discoveryLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    IList<ProvisioningLog> provisioningLogs = (from provisionLog in _context.ProvisioningLog
                                                               where provisionLog.LogDate < PurgeRangeDate
                                                               select provisionLog).ToList();
                    _context.ProvisioningLog.RemoveRange(provisioningLogs);
                    _context.SaveChanges();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Provisioning Log Data", provisioningLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    IList<AttestationLog> attestationLogs = (from attestationLog in _context.AttestaionLog
                                                             where attestationLog.LogDate < PurgeRangeDate
                                                             select attestationLog).ToList();
                    _context.AttestaionLog.RemoveRange(attestationLogs);
                    _context.SaveChanges();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Attestation Log Data", attestationLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    IList<SchedulerLog> schedulerLogs = (from schedulerLog in _context.SchedulerLog
                                                         where schedulerLog.LogDate < PurgeRangeDate
                                                         select schedulerLog).ToList();
                    _context.SchedulerLog.RemoveRange(schedulerLogs);
                    _context.SaveChanges();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Scheduler Log Data", schedulerLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    IList<UILog> uiLogs = (from uiLog in _context.UILog
                                                          where uiLog.LogDate < PurgeRangeDate
                                                          select uiLog).ToList();
                    _context.UILog.RemoveRange(uiLogs);
                    _context.SaveChanges();

                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From UI Log Data", uiLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    IList<EmailLog> emailLogs = (from emailLog in _context.EmailLog
                                                 where emailLog.LogDate < PurgeRangeDate
                                                 select emailLog).ToList();
                    _context.EmailLog.RemoveRange(emailLogs);
                    _context.SaveChanges();
                    DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Email Log Data", emailLogs.Count), "", "HouseKeeping", 0, 1, "Housekeeping Engine");

                    return 0;
                }
            }
            catch (Exception ex)
            {
                DILog.SchedulerLogger.WriteLog(LogLevelType.Error, "Error Occured While Purging Log Related Data ", ex.InnerException.ToString(), "HouseKeeping", 0, 0, "HouseKeeping Service");
            }
            return 0;
        }

        public int PurgeAllAttestationData(int NumberOfDays)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    DateTime PurgeRangeDate = DateTime.Now.AddDays(-NumberOfDays);

                    List<Attestation> AttestationsToBePurged = (from attestation in _context.Attestations
                                                                .Include("AttestationEndpoint")
                                                                where attestation.CreatedDate < PurgeRangeDate
                                                                select attestation).ToList();

                    List<AttestationReview> ReviewsToBePurged = (from attestationReview in _context.AttestationReviews
                                                                 where attestationReview.CreatedDate < PurgeRangeDate
                                                                 select attestationReview).ToList();
                    foreach (AttestationReview aReview in ReviewsToBePurged)
                    {
                        foreach (ReviewEndpointSystem rEndpoint in aReview.ReviewEndpointSystems)
                        {
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview_EndpointSystemHistory where ReviewEndpointId = '{0}'", rEndpoint.ReviewEndpointId));
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview_EndpointSystem_L2DetailHistory where ReviewEndpointId = '{0}'", rEndpoint.ReviewEndpointId));
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview_EndpointSystem_L2Detail where ReviewEndpointId = '{0}'", rEndpoint.ReviewEndpointId));
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview_EndpointSystem where ReviewEndpointId = '{0}'", rEndpoint.ReviewEndpointId));
                        }
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_ReviewEndpointSystems where ReviewId = '{0}'", aReview.ReviewId));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview where ReviewId = '{0}'", aReview.ReviewId));

                    }

                    foreach (Attestation purgeAttestation in AttestationsToBePurged)
                    {
                        AttestationProcess attestationProcess = _context.AttestationProcesses.FirstOrDefault(aProcess => aProcess.AttestationId == purgeAttestation.AttestationId);
                        if (attestationProcess != null)
                        {
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_Scheduler where AttestationProcessId = '{0}'", attestationProcess.AttestationProcessId));
                        }
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationProcessHistory where AttestationId = '{0}'", purgeAttestation.AttestationId));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationProcess where AttestationId = '{0}'", purgeAttestation.AttestationId));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_Attestation where AttestationId = '{0}'", purgeAttestation.AttestationId));

                    }

                    return AttestationsToBePurged.Count;

                }
            }
            catch
            {
                throw;
            }
        }
    }
}
