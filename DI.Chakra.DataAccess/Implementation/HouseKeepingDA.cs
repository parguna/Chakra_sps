using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
//using DI.Chakra.Model.Core.House_Keeping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DI.Chakra.DataAccess
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
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Error, "Error Occured While Purging Tasks Related Data", ex);

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

                    IList<ServiceLog> discoveryLogs = (from discoveryLog in _context.ServiceLog
                                                         where discoveryLog.ReferenceType==LogReferenceType.Discovery
                                                            && discoveryLog.LogDate < PurgeRangeDate
                                                         select discoveryLog).ToList();
                    _context.ServiceLog.RemoveRange(discoveryLogs);
                    _context.SaveChanges();
                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Discovery Log Data", discoveryLogs.Count));

                    IList<ServiceLog> provisioningLogs = (from provisionLog in _context.ServiceLog
                                                               where provisionLog.ReferenceType == LogReferenceType.Provisioning
                                                                  && provisionLog.LogDate < PurgeRangeDate
                                                               select provisionLog).ToList();
                    _context.ServiceLog.RemoveRange(provisioningLogs);
                    _context.SaveChanges();
                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Provisioning Log Data", provisioningLogs.Count));

                    IList<ServiceLog> attestationLogs = (from attestationLog in _context.ServiceLog
                                                         where attestationLog.ReferenceType == LogReferenceType.Attestation
                                                              && attestationLog.LogDate < PurgeRangeDate
                                                             select attestationLog).ToList();
                    _context.ServiceLog.RemoveRange(attestationLogs);
                    _context.SaveChanges();
                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Attestation Log Data", attestationLogs.Count));

                    IList<ServiceLog> schedulerLogs = (from schedulerLog in _context.ServiceLog
                                                       where schedulerLog.ReferenceType == LogReferenceType.Scheduler
                                                         && schedulerLog.LogDate < PurgeRangeDate
                                                         select schedulerLog).ToList();
                    _context.ServiceLog.RemoveRange(schedulerLogs);
                    _context.SaveChanges();
                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Scheduler Log Data", schedulerLogs.Count));

                    IList<UILog> uiLogs = (from uiLog in _context.UILog
                                                          where uiLog.LogDate < PurgeRangeDate
                                                          select uiLog).ToList();
                    _context.UILog.RemoveRange(uiLogs);
                    _context.SaveChanges();

                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From UI Log Data", uiLogs.Count));

                    IList<ServiceLog> emailLogs = (from emailLog in _context.ServiceLog
                                                   where emailLog.ReferenceType == LogReferenceType.Email
                                                 && emailLog.LogDate < PurgeRangeDate
                                                 select emailLog).ToList();
                    _context.ServiceLog.RemoveRange(emailLogs);
                    _context.SaveChanges();
                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Email Log Data", emailLogs.Count));

                    IList<ServiceLog> complianceLogs = (from complianceLog in _context.ServiceLog
                                                   where complianceLog.ReferenceType == LogReferenceType.Compliance
                                                 && complianceLog.LogDate < PurgeRangeDate
                                                        select complianceLog).ToList();
                    _context.ServiceLog.RemoveRange(complianceLogs);
                    _context.SaveChanges();

                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From Compliance Log Data", complianceLogs.Count));

                    IList<ServiceLog> housekeepLogs = (from houekeepLog in _context.ServiceLog
                                                       where houekeepLog.ReferenceType == LogReferenceType.HouseKeeping
                                                      && houekeepLog.LogDate < PurgeRangeDate
                                                       select houekeepLog).ToList();
                    _context.ServiceLog.RemoveRange(housekeepLogs);
                    _context.SaveChanges();

                    DILog.HouseKeepingLogger.WriteLog(LogLevelType.Info, string.Format("Housekeeping Engine Purged {0} Items From House Keeping Log Data", housekeepLogs.Count));

                    return 0;
                }
            }
            catch (Exception ex)
            {
                DILog.HouseKeepingLogger.WriteLog(LogLevelType.Error, "Error Occured While Purging Log Related Data ", ex);
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
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_ReviewEndpointSystems where ReviewId = '{0}'", aReview.Id));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationReview where ReviewId = '{0}'", aReview.Id));

                    }

                    foreach (Attestation purgeAttestation in AttestationsToBePurged)
                    {
                        AttestationProcess attestationProcess = _context.AttestationProcesses.FirstOrDefault(aProcess => aProcess.AttestationId == purgeAttestation.Id);
                        if (attestationProcess != null)
                        {
                            _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_Scheduler where AttestationProcessId = '{0}'", attestationProcess.Id));
                        }
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationProcessHistory where AttestationId = '{0}'", purgeAttestation.Id));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_AttestationProcess where AttestationId = '{0}'", purgeAttestation.Id));
                        _context.Database.ExecuteSqlCommand(string.Format("Delete from DI_M_Attestation where AttestationId = '{0}'", purgeAttestation.Id));

                    }

                    return AttestationsToBePurged.Count;

                }
            }
            catch
            {
                throw;
            }
        }
        //public List<DI.Chakra.Model.Core.House_Keeping.PurgeConfig> GetAllPurgeConfigData()
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            return Context.PurgeConfigs.ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public PurgeConfig GetPurgeConfigDatabyid(int id)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.PurgeConfigs.FirstOrDefault(upc => upc.Id == id);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public int SavePurgemode(PurgeConfig purgconfig)
        //{
        //    try
        //    {

        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            if (purgconfig.Id == 0)
        //            {
        //                oIACMDBContext.PurgeConfigs.Attach(purgconfig);
        //                oIACMDBContext.Entry(purgconfig).State = EntityState.Added;
        //                oIACMDBContext.SaveChanges();
        //            }
        //            else
        //            {
        //                PurgeConfig config = oIACMDBContext.PurgeConfigs.Where(qns => qns.Id == purgconfig.Id).First();
        //                oIACMDBContext.Entry(config).CurrentValues.SetValues(purgconfig);
        //                oIACMDBContext.SaveChanges();
        //            }
        //        }

        //        return purgconfig.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Inserting the Purge Configuration", ex);
        //        throw ex;
        //    }
        //}

        //public bool DeletePurgeConfig(int ID)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            int recordsAffected = 0;
        //            PurgeConfig delpurgeconfig = oIACMDBContext.PurgeConfigs.FirstOrDefault(Per => Per.Id == ID);
        //            if (delpurgeconfig != null)
        //            {
        //                oIACMDBContext.PurgeConfigs.Remove(delpurgeconfig);
        //                recordsAffected = oIACMDBContext.SaveChanges();
        //            }

        //            if (recordsAffected == 1)
        //                return true;

        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured While Deleting the Purge Configuration", ex);
        //        throw ex;
        //    }
        //}

        //public bool IsAlreadyExists(string _tablename)
        //{
        //    bool isExists = false;
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            PurgeConfig config = oIACMDBContext.PurgeConfigs.FirstOrDefault(uco => uco.TableName == _tablename);
        //            if (config != null)
        //            {
        //                isExists = true;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return isExists;
        //}
    }
}
