using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.ReportData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DeepIdentity.Model.SAP;
using DeepIdentity.Model.GridData;
using DeepIdentity.Logger;
using DeepIdentity.DataAccess;

namespace DeepIdentity.DataAccess
{
    public class AttestationDA
    {

        public bool IsAttestationNameExists(string attestationName, int attestationId)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    if ((attestationId == 0) || (attestationId == -1))
                    {
                        return c.Attestations.Any(item => item.AttestationName.Equals(attestationName, StringComparison.CurrentCultureIgnoreCase));
                    }
                    return c.Attestations.Any((item => item.AttestationName.Equals(attestationName, StringComparison.CurrentCultureIgnoreCase) && item.AttestationId == attestationId));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Attestation GetAttestation(int _id, bool deepLoad)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    if (!deepLoad)
                    {
                        return c.Attestations.FirstOrDefault(item => item.AttestationId == _id);
                    }
                    return (from attestation in c.Attestations
                        .Include("AttestationProcesses")
                        .Include("AttestationEndpoint")
                        .Include("AttestationEndpoint.Endpoint")
                        .Include("AttestationEndpoint.Endpoint.EndPointSystemType")
                        .Include("WorkflowConfiguration")
                        .Include("WorkflowConfiguration.WorkflowConfigurationDetails")
                            where attestation.AttestationId == _id
                            select attestation).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<Attestation> GetAllAttestations()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attestation in c.Attestations
                            select attestation).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Attestation> GetAllAttestationsOfEndPointType(EndpointType paramEPType)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attestation in c.Attestations
                            join attestationEP in c.AttestationEndpoints
                                on attestation.AttestationId equals attestationEP.AttestationId
                            where attestationEP.EndpointSystemType == paramEPType
                            select attestation).ToList();

                }
            }
            catch
            {
                throw;
            }

        }

        public IList<ListAttestationGridData> GetAllAttestationsForActionUsers(int paramActionUserId)
        {
            try
            {

                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attestation in c.Attestations
                            join task in c.Tasks
                                on attestation.AttestationId equals task.AttestationId
                            where (task.ActionUserId == paramActionUserId || paramActionUserId == -1)
                            join attestationProcess in c.AttestationProcesses
                                on attestation.AttestationId equals attestationProcess.AttestationId
                            join attestationUser in c.Users
                                on task.ActionUserId equals attestationUser.UserId
                            select new ListAttestationGridData
                            {
                                AttestationId = attestation.AttestationId,
                                AttestationName = attestation.AttestationName,
                                AttestationType = attestation.AttestationType,
                                RunDate = attestation.CreatedDate,
                                //Status = attestationProcess.Status,
                                AttesationCreatedBy = attestation.CreatedBy
                            }).ToList();


                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Attestation> GetAllAttestations(string _typeName)
        {
            throw new NotImplementedException();
        }

        public void AddAttestation(Attestation _attestation)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    c.Attestations.Add(_attestation);
                    c.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddEndpointsystemReview(IList<AttestationReviewEndpointSystem> _list)
        {
            // SQLBulkUploader<AttestationReviewEndpointSystem> sql = null;
            try
            {
                // string tableName = "DI_M_AttestationReview_EndpointSystem";

                //sql = new SQLBulkUploader<AttestationReviewEndpointSystem>(tableName);

                // sql.Flush(_list);

                // DILog.AttestationLogger.WriteLog(LogLevelType.Info, string.Format("Total {0} Attestation review Endpoints added", _list.Count.ToString()),string.Empty,string.Empty,0,LogType.INFO.ToString(),"Attestation Service",string.Empty,string.Empty,string.Empty);
            }
            catch (Exception Ex)
            {

                throw;
            }

        }

        public List<User> GetUsersForReviewer(long _taskId)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from task in c.Tasks
                            where task.TaskId == _taskId
                            from taskReview in task.TaskReviews
                            join user in c.Users on taskReview.AttestationReview.UserId equals user.UserId
                            select user).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<AttestationReviewEndpointSystemGridData> GetUsersofEndpointForReviewer(long _taskId, int _userId)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from task in c.Tasks
                            where task.TaskId == _taskId
                            from taskReview in task.TaskReviews
                            where taskReview.AttestationReview.UserId == _userId
                            from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems

                            select new AttestationReviewEndpointSystemGridData
                            {
                                Approve = reviewEP.AttestationReviewEndpointSystem.Approve,
                                Comments = reviewEP.AttestationReviewEndpointSystem.Comments,
                                EndpointSystemId = reviewEP.AttestationReviewEndpointSystem.EndpointSystemId,
                                HostName = reviewEP.AttestationReviewEndpointSystem.HostName,
                                EndpointSystemTypeId = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeId,
                                EndpointSystemTypeName = reviewEP.AttestationReviewEndpointSystem.EndpointSystemTypeName,
                                EndpointUserId = reviewEP.AttestationReviewEndpointSystem.EndpointUserId,
                                EndpointUserName = reviewEP.AttestationReviewEndpointSystem.EndpointUserName,
                                ReviewEndpointId = reviewEP.AttestationReviewEndpointSystem.ReviewEndpointId
                            }
                        ).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<AttestationEndpoint> GetAttestationEndpoint(int AttestationId)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return c.AttestationEndpoints.Where(item => item.AttestationId == AttestationId).ToList();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IList<ListAttestationGridData> GetAllAttestationsWithDetails(int _ApplicationType)
        {
            try
            {
                IList<ListAttestationGridData> List = new List<ListAttestationGridData>();
                using (IACMDBContext c = new IACMDBContext())
                {

                    if ((_ApplicationType == 0) || (_ApplicationType == -1))
                    {
                        var list = (from attestation in c.Attestations
                                    join attPrcs in c.AttestationProcesses on attestation.AttestationId equals attPrcs.AttestationId
                                    join pr in c.Process on attPrcs.AttestationProcessId equals pr.ProcessTypeKeyId
                                    join schdlr in c.Scheduler on pr.Scheduler.SchedulerId equals schdlr.SchedulerId
                                    join attEndpoint in c.AttestationEndpoints on attestation.AttestationId equals attEndpoint.AttestationId
                                    join end in c.EndpointSystems on attEndpoint.EndpointSystemId equals end.EndpointSystemId
                                    where pr.ProcessType == ProcessType.Attestation
                                    select new ListAttestationGridData
                                    {
                                        AttestationId = attestation.AttestationId,
                                        AttestationName = attestation.AttestationName,
                                        AttestationDescription = attestation.AttestationDescription,
                                        Status = attPrcs.WorkflowStatus,
                                        RunDate = schdlr.StartDate,
                                        CreatedDate = attestation.CreatedDate,
                                        //ResponseTimeInDays = workflow.ResponseTimeInDays,
                                        AttesationCreatedBy = attestation.CreatedBy,
                                        EndPointSystem = end.DisplayName
                                    }
                                 ).Distinct().ToList();
                        Dictionary<int, int> uniqueStore = new Dictionary<int, int>();

                        foreach (ListAttestationGridData Gridlist in list)
                        {
                            if (!uniqueStore.ContainsKey(Gridlist.AttestationId))
                            {
                                uniqueStore.Add(Gridlist.AttestationId, 0);
                                List.Add(Gridlist);
                            }
                        }
                    }
                    else
                    {
                        var list = (from attestation in c.Attestations
                                    join attPrcs in c.AttestationProcesses on attestation.AttestationId equals attPrcs.AttestationId
                                    join pr in c.Process on attPrcs.AttestationProcessId equals pr.ProcessTypeKeyId
                                    join schdlr in c.Scheduler on pr.Scheduler.SchedulerId equals schdlr.SchedulerId
                                    join attEndpoint in c.AttestationEndpoints on attestation.AttestationId equals attEndpoint.AttestationId
                                    join end in c.EndpointSystems on attEndpoint.EndpointSystemId equals end.EndpointSystemId
                                    where pr.ProcessType == ProcessType.Attestation && end.EndpointSystemTypeId == _ApplicationType
                                    select new ListAttestationGridData
                                    {
                                        AttestationId = attestation.AttestationId,
                                        AttestationName = attestation.AttestationName,
                                        AttestationDescription = attestation.AttestationDescription,
                                        Status = attPrcs.WorkflowStatus,
                                        RunDate = schdlr.StartDate,
                                        CreatedDate = attestation.CreatedDate,
                                        //ResponseTimeInDays = workflow.ResponseTimeInDays,
                                        AttesationCreatedBy = attestation.CreatedBy,
                                        EndPointSystem = end.DisplayName
                                    }
                                 ).Distinct().ToList();

                        Dictionary<int, int> uniqueStore = new Dictionary<int, int>();

                        foreach (ListAttestationGridData Gridlist in list)
                        {
                            if (!uniqueStore.ContainsKey(Gridlist.AttestationId))
                            {
                                uniqueStore.Add(Gridlist.AttestationId, 0);
                                List.Add(Gridlist);
                            }
                        }

                    }

                    return List;
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXRevokeRequestReportData> GetAllADEXRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join ads in c.ADSUserInfos
                           on rvwEPS.EndpointUserName equals ads.sAMAccountName
                            where rvwEPS.Revoke && rvwEPS.EndpointSystemTypeId == (byte)EndpointType.ActiveDirectory
                            select new ADEXRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = ads.displayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPS.LastModifiedBy,
                                RevokedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXAccountRequests> GetAllADEXAccountRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join ads in c.ADSUserInfos
                           on rvwEPS.EndpointUserName equals ads.sAMAccountName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.ActiveDirectory
                            select new ADEXAccountRequests
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = ads.displayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPS.LastModifiedBy,
                                ReviewedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments,
                                Approve = rvwEPS.Approve,
                                Revoke = rvwEPS.Revoke,
                                IsChanged = rvwEPS.IsChanged
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXGroupRevokeRequestReportData> GetAllADEXGroupRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join ads in c.ADSUserInfos
                                on rvwEPS.EndpointUserName equals ads.sAMAccountName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.ActiveDirectory
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId
                            where rvwEPSL2.Revoke
                            select new ADEXGroupRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = ads.displayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPSL2.LastModifiedBy,
                                RevokedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXGroupRequestReportData> GetAllADEXGroupRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join ads in c.ADSUserInfos
                                on rvwEPS.EndpointUserName equals ads.sAMAccountName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.ActiveDirectory
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId

                            select new ADEXGroupRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = ads.displayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPSL2.LastModifiedBy,
                                ReviewedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName,
                                Approve = rvwEPSL2.Approve,
                                Revoke = rvwEPSL2.Revoke,
                                IsChanged = rvwEPSL2.IsChanged
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseRevokeRequestReportData> GetAllInhouseRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join inhouse in c.InhouseUserInfos
                           on rvwEPS.EndpointUserName equals inhouse.UserName
                            where rvwEPS.Revoke && rvwEPS.EndpointSystemTypeId == (byte)EndpointType.InhouseSystems
                            select new InhouseRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = inhouse.DisplayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPS.LastModifiedBy,
                                RevokedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseGroupRevokeRequestReportData> GetAllInhouseGroupRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join inhouse in c.InhouseUserInfos
                                on rvwEPS.EndpointUserName equals inhouse.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.InhouseSystems
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId
                            where rvwEPSL2.Revoke
                            select new InhouseGroupRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = inhouse.DisplayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPSL2.LastModifiedBy,
                                RevokedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseGroupRequestReportData> GetAllInhouseGroupRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join inhouse in c.InhouseUserInfos
                                on rvwEPS.EndpointUserName equals inhouse.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.InhouseSystems
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId

                            select new InhouseGroupRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = inhouse.DisplayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPSL2.LastModifiedBy,
                                ReviewedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName,
                                Approve = rvwEPSL2.Approve,
                                Revoke = rvwEPSL2.Revoke,
                                IsChanged = rvwEPSL2.IsChanged
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseAccountRequests> GetAllInhouseAccountRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join inhouse in c.InhouseUserInfos
                           on rvwEPS.EndpointUserName equals inhouse.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.InhouseSystems
                            select new InhouseAccountRequests
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = inhouse.DisplayName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPS.LastModifiedBy,
                                ReviewedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments,
                                Approve = rvwEPS.Approve,
                                Revoke = rvwEPS.Revoke,
                                IsChanged = rvwEPS.IsChanged
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LNSAccountRequests> GetAllLNSAccountRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join lns in c.LotusUserInfos
                           on rvwEPS.EndpointUserName equals lns.ShortName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.LotusNotes
                            select new LNSAccountRequests
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = lns.Name + lns.LastName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPS.LastModifiedBy,
                                ReviewedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments,
                                Approve = rvwEPS.Approve,
                                Revoke = rvwEPS.Revoke,
                                IsChanged = rvwEPS.IsChanged
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LNSRevokeRequestReportData> GetAllLNSRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join lns in c.LotusUserInfos
                           on rvwEPS.EndpointUserName equals lns.ShortName
                            where rvwEPS.Revoke && rvwEPS.EndpointSystemTypeId == (byte)EndpointType.LotusNotes
                            select new LNSRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = lns.FullName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPS.LastModifiedBy,
                                RevokedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LNSGroupRevokeRequestReportData> GetAllLNSGroupRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join lns in c.LotusUserInfos
                                on rvwEPS.EndpointUserName equals lns.ShortName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.LotusNotes
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId
                            where rvwEPSL2.Revoke
                            select new LNSGroupRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = lns.FullName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPSL2.LastModifiedBy,
                                RevokedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LNSGroupRequestReportData> GetAllLNSGroupRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join lns in c.LotusUserInfos
                                on rvwEPS.EndpointUserName equals lns.ShortName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.LotusNotes
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId

                            select new LNSGroupRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = lns.FullName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPSL2.LastModifiedBy,
                                ReviewedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Group = rvwEPSL2.AccessName,
                                Approve = rvwEPSL2.Approve,
                                Revoke = rvwEPSL2.Revoke,
                                IsChanged = rvwEPSL2.IsChanged
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPRoleRevokeRequestReportData> GetAllSAPEPRoleRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sapep in c.SAPEPUserInfos
                                on rvwEPS.EndpointUserName equals sapep.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId
                            where rvwEPSL2.Revoke
                            select new SAPEPRoleRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sapep.UserName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPSL2.LastModifiedBy,
                                RevokedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Role = rvwEPSL2.AccessName
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPRevokeRequestReportData> GetAllSAPEPRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sapep in c.SAPEPUserInfos
                           on rvwEPS.EndpointUserName equals sapep.UserName
                            where rvwEPS.Revoke && rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal
                            select new SAPEPRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sapep.UserName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPS.LastModifiedBy,
                                RevokedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPRoleRequestReportData> GetAllSAPEPRoleRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sapep in c.SAPEPUserInfos
                                on rvwEPS.EndpointUserName equals sapep.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId

                            select new SAPEPRoleRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sapep.UserName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPSL2.LastModifiedBy,
                                ReviewedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Role = rvwEPSL2.AccessName,
                                Approve = rvwEPSL2.Approve,
                                Revoke = rvwEPSL2.Revoke,
                                IsChanged = rvwEPSL2.IsChanged
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPAccountRequestsReportData> GetAllSAPEPAccountRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sapep in c.SAPEPUserInfos
                           on rvwEPS.EndpointUserName equals sapep.UserName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal
                            select new SAPEPAccountRequestsReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sapep.UserName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPS.LastModifiedBy,
                                ReviewedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments,
                                Approve = rvwEPS.Approve,
                                Revoke = rvwEPS.Revoke,
                                IsChanged = rvwEPS.IsChanged
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public IList<Attestation_ListOfAttestation> getAllAttestationProcessForReport(EndpointType paramEPType)
        {
            IList<Attestation_ListOfAttestation> returnList = new List<Attestation_ListOfAttestation>();
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    returnList = (from attestationEP in c.AttestationEndpoints
                                  where (attestationEP.EndpointSystemType == paramEPType)
                                  join attestation in c.Attestations
                                    on attestationEP.AttestationId equals attestation.AttestationId
                                  join attestationProcess in c.AttestationProcesses
                                      on attestation.AttestationId equals attestationProcess.AttestationId
                                  join tsk in c.Tasks
                                      on attestation.AttestationId equals tsk.AttestationId
                                  where tsk.WorkflowStateId == (c.Tasks.Where(MaxTsk => MaxTsk.AttestationId == attestation.AttestationId).Select(lk => lk.WorkflowStateId).Max())
                                  select new Attestation_ListOfAttestation
                                  {
                                      attestationId = attestation.AttestationId,
                                      attestationDescription = attestation.AttestationDescription,
                                      attestationName = attestation.AttestationName,
                                      attestationProcessStartsAt = attestationProcess.ProcessStartAt.Value,
                                      attestationProcessEndsAt = attestationProcess.ProcessEndAt.Value,
                                      workFlowStatus = attestationProcess.WorkflowStatus,
                                      SubmittedDate = tsk.LastModifiedDate,
                                      SubmittedStatus = tsk.IsCompleted ? "Submitted" : "Pending",
                                      Level = tsk.WorkflowStateId,
                                      WorkFlowConfigId = attestation.WorkflowConfigurationId
                                  }).OrderByDescending(endsATSort => endsATSort.attestationProcessEndsAt).ToList();

                    IList<WorkflowConfigurationDetail> wfDetails = new List<WorkflowConfigurationDetail>();
                    int reviewerCount = 0;
                    UserDA usrDA = new UserDA();
                    MasterDA masterDA = new MasterDA();
                    GroupDA grpDA = new GroupDA();
                    string strTemp = "";

                    foreach (Attestation_ListOfAttestation item in returnList)
                    {
                        wfDetails = c.WorkflowConfigurationDetails.Where(wf => wf.WorkflowConfigurationId == item.WorkFlowConfigId).ToList();
                        reviewerCount = 0;
                        foreach (WorkflowConfigurationDetail wfDtlsItem in wfDetails)
                        {
                            if (wfDtlsItem.ApprovalUserType == ApprovalUserType.User)//User
                            {
                                User usr = usrDA.GetUser(wfDtlsItem.Approver);
                                if (usr != null)
                                {
                                    ++reviewerCount;
                                    if (string.IsNullOrEmpty(item.attestationReviewers) == false)
                                    {
                                        item.attestationReviewers = item.attestationReviewers + "<br/>";
                                    }
                                    item.attestationReviewers = item.attestationReviewers + string.Format("{0}. {1} - [{2}]", reviewerCount, usr.UserName, usr.DisplayName);
                                }
                            }
                            else if (wfDtlsItem.ApprovalUserType == ApprovalUserType.Role)//Role
                            {
                                strTemp = masterDA.GetRoleTypes().Where(appr => appr.RoleTypeId == wfDtlsItem.Approver).First().RoleTypeDesc;
                                if (strTemp != "None")
                                {
                                    ++reviewerCount;
                                    if (string.IsNullOrEmpty(item.attestationReviewers) == false)
                                    {
                                        item.attestationReviewers = item.attestationReviewers + "<br/>";
                                    }
                                    item.attestationReviewers = item.attestationReviewers + string.Format("{0}. {1}", reviewerCount, strTemp);
                                }
                            }
                            else if (wfDtlsItem.ApprovalUserType == ApprovalUserType.Group)//Group
                            {
                                strTemp = grpDA.GetGroup(wfDtlsItem.Approver).GroupName;
                                if (string.IsNullOrEmpty(strTemp) == false)
                                {
                                    ++reviewerCount;
                                    if (string.IsNullOrEmpty(item.attestationReviewers) == false)
                                    {
                                        item.attestationReviewers = item.attestationReviewers + "<br/>";
                                    }
                                    item.attestationReviewers = item.attestationReviewers + string.Format("{0}. {1}", reviewerCount, strTemp);
                                }
                            }
                        }
                    }
                    return returnList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<Attestation_ListOfUsersByAttestationStatusReportData> getAllUsersAttestationProcessStatusForReport(int ipAttestationId, string ipAttestationStatus)
        {
            IList<AttestationEndpoint> listOfEndPoints = new List<AttestationEndpoint>();
            IList<Attestation_ListOfUsersByAttestationStatusReportData> returnList = new List<Attestation_ListOfUsersByAttestationStatusReportData>();
            IList<Attestation_ListOfUsersByAttestationStatusReportData> tempList = new List<Attestation_ListOfUsersByAttestationStatusReportData>();
            EndPoint ep = new EndPoint();
            User dimUser = new User();
            ADSUserInfo ads = new ADSUserInfo();
            LotusUserInfo lns = new LotusUserInfo();
            SAPUserinfo sap = new SAPUserinfo();
            SAPEPUserInfo sapep = new SAPEPUserInfo();
            InhouseUserInfo inhouse = new InhouseUserInfo();
            bool canBeUsed;

            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    tempList = (from attest in c.Attestations
                                where attest.AttestationId == ipAttestationId
                                join tsk in c.Tasks
                                    on attest.AttestationId equals tsk.AttestationId
                                join ReP in c.ReviewEndpointSystems
                                    on tsk.TaskId equals ReP.TaskId
                                join AReP in c.AttestationReviewEndpointSystems
                                    on ReP.ReviewEndpointId equals AReP.ReviewEndpointId
                                join usr in c.Users
                                    on tsk.ActionUserId equals usr.UserId
                                select new Attestation_ListOfUsersByAttestationStatusReportData
                                {
                                    userId = AReP.EndpointUserId,
                                    attestationStatus = tsk.IsCompleted ? (AReP.Approve ? "Approved" : (AReP.Revoke ? "Revoked" : "")) : "Not Attempted",
                                    endPointSystemId = AReP.EndpointSystemId,
                                    endPointTypeId = AReP.EndpointSystemTypeId,
                                    WorkflowConfigurationId = attest.WorkflowConfigurationId,
                                    ActionUser = usr.DisplayName,
                                    Level = tsk.WorkflowStateId
                                }).ToList();

                    foreach (Attestation_ListOfUsersByAttestationStatusReportData item in tempList)
                    {
                        canBeUsed = false;
                        if (item.endPointTypeId == (int)EndpointType.ActiveDirectory)
                        {
                            if (ipAttestationStatus == "All")
                            {
                                canBeUsed = true;
                            }
                            else
                            {
                                if (item.attestationStatus == ipAttestationStatus)
                                    canBeUsed = true;
                            }

                            if (canBeUsed)
                            {
                                ep = c.EndpointSystems.FirstOrDefault(E => E.EndpointSystemId == item.endPointSystemId);
                                item.endPointSystemName = ep.EndPointSystemType.DisplayName;
                                item.endPointHostName = ep.DisplayName;

                                ads = c.ADSUserInfos.FirstOrDefault(AD => AD.UserId == item.userId);

                                if (ads != null && ads.DIUserId != null)
                                {
                                    item.endPointUserName = ads.sAMAccountName;
                                    dimUser = c.Users.FirstOrDefault(U => U.UserId == ads.DIUserId);
                                    if (dimUser != null)
                                    {
                                        item.displayName = dimUser.DisplayName;
                                        item.employeeId = dimUser.EmployeeId;
                                        item.firstName = dimUser.FirstName;
                                        item.lastName = dimUser.LastName;
                                        item.userName = dimUser.UserName;
                                        item.organizationId = dimUser.Organisation.OrganisationName;
                                    }
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (item.endPointTypeId == (int)EndpointType.LotusNotes)
                        {
                            if (ipAttestationStatus == "All")
                                canBeUsed = true;
                            else
                            {
                                if (item.attestationStatus == ipAttestationStatus)
                                    canBeUsed = true;
                            }

                            if (canBeUsed)
                            {
                                ep = c.EndpointSystems.FirstOrDefault(E => E.EndpointSystemId == item.endPointSystemId);
                                item.endPointSystemName = ep.EndPointSystemType.DisplayName;
                                item.endPointHostName = ep.DisplayName;

                                lns = c.LotusUserInfos.FirstOrDefault(L => L.UserId == item.userId);

                                if (lns != null && lns.DIUserId != null)
                                {
                                    item.endPointUserName = lns.ShortName;
                                    dimUser = c.Users.FirstOrDefault(U => U.UserId == lns.DIUserId);
                                    if (dimUser != null)
                                    {
                                        item.displayName = dimUser.DisplayName;
                                        item.employeeId = dimUser.EmployeeId;
                                        item.firstName = dimUser.FirstName;
                                        item.lastName = dimUser.LastName;
                                        item.userName = dimUser.UserName;
                                        item.organizationId = dimUser.Organisation.OrganisationName;
                                    }
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (item.endPointTypeId == (int)EndpointType.SAP)
                        {
                            if (ipAttestationStatus == "All")
                                canBeUsed = true;
                            else
                            {
                                if (item.attestationStatus == ipAttestationStatus)
                                    canBeUsed = true;
                            }

                            if (canBeUsed)
                            {
                                ep = c.EndpointSystems.FirstOrDefault(E => E.EndpointSystemId == item.endPointSystemId);
                                item.endPointSystemName = ep.EndPointSystemType.DisplayName;
                                item.endPointHostName = ep.DisplayName;

                                sap = c.SAPUserinfos.FirstOrDefault(N => N.UserId == item.userId);

                                if (sap != null && sap.DIUserId != null)
                                {
                                    item.endPointUserName = sap.UserLogonName;
                                    dimUser = c.Users.FirstOrDefault(U => U.UserId == sap.DIUserId);
                                    if (dimUser != null)
                                    {
                                        item.displayName = dimUser.DisplayName;
                                        item.employeeId = dimUser.EmployeeId;
                                        item.firstName = dimUser.FirstName;
                                        item.lastName = dimUser.LastName;
                                        item.userName = dimUser.UserName;
                                        item.organizationId = dimUser.Organisation.OrganisationName;
                                    }
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (item.endPointTypeId == (int)EndpointType.SAPEnterprisePortal)
                        {
                            if (ipAttestationStatus == "All")
                                canBeUsed = true;
                            else
                            {
                                if (item.attestationStatus == ipAttestationStatus)
                                    canBeUsed = true;
                            }

                            if (canBeUsed)
                            {
                                ep = c.EndpointSystems.FirstOrDefault(E => E.EndpointSystemId == item.endPointSystemId);
                                item.endPointSystemName = ep.EndPointSystemType.DisplayName;
                                item.endPointHostName = ep.DisplayName;

                                sapep = c.SAPEPUserInfos.FirstOrDefault(N => N.UserId == item.userId);

                                if (sapep != null && sapep.DIUserId != null)
                                {
                                    item.endPointUserName = sapep.UserName;
                                    dimUser = c.Users.FirstOrDefault(U => U.UserId == sapep.DIUserId);
                                    if (dimUser != null)
                                    {
                                        item.displayName = dimUser.DisplayName;
                                        item.employeeId = dimUser.EmployeeId;
                                        item.firstName = dimUser.FirstName;
                                        item.lastName = dimUser.LastName;
                                        item.userName = dimUser.UserName;
                                        item.organizationId = dimUser.Organisation.OrganisationName;
                                    }
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (item.endPointTypeId == (int)EndpointType.InhouseSystems)
                        {
                            if (ipAttestationStatus == "All")
                                canBeUsed = true;
                            else
                            {
                                if (item.attestationStatus == ipAttestationStatus)
                                    canBeUsed = true;
                            }

                            if (canBeUsed)
                            {
                                ep = c.EndpointSystems.FirstOrDefault(E => E.EndpointSystemId == item.endPointSystemId);
                                item.endPointSystemName = ep.EndPointSystemType.DisplayName;
                                item.endPointHostName = ep.DisplayName;

                                inhouse = c.InhouseUserInfos.FirstOrDefault(N => N.UserId == item.userId);

                                if (inhouse != null && inhouse.DIUserId != null)
                                {
                                    item.endPointUserName = inhouse.UserName;
                                    dimUser = c.Users.FirstOrDefault(U => U.UserId == inhouse.DIUserId);
                                    if (dimUser != null)
                                    {
                                        item.displayName = dimUser.DisplayName;
                                        item.employeeId = dimUser.EmployeeId;
                                        item.firstName = dimUser.FirstName;
                                        item.lastName = dimUser.LastName;
                                        item.userName = dimUser.UserName;
                                        item.organizationId = dimUser.Organisation.OrganisationName;
                                    }
                                }
                                returnList.Add(item);
                            }
                        }
                    }
                }
                return returnList.Distinct().OrderByDescending(eps => eps.endPointSystemName).ThenByDescending(res => res.Level).ThenBy(name => name.displayName).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationActionReportData> getAttestationActionReportData(int paramAttestationId)
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from tsk in c.Tasks
                            where tsk.AttestationId == paramAttestationId && tsk.TaskId == (c.Tasks.Where(maxTsk => maxTsk.AttestationId == paramAttestationId).Select(max => max.TaskId).Max())
                            join tskReview in c.TaskReviews
                                on tsk.TaskId equals tskReview.TaskId
                            join attReview in c.AttestationReviews
                                on tskReview.ReviewId equals attReview.ReviewId
                            join ReP in c.ReviewEndpointSystems
                                on attReview.ReviewId equals ReP.ReviewId
                            where ReP.TaskId == tsk.TaskId
                            join AReP in c.AttestationReviewEndpointSystems
                                on ReP.ReviewEndpointId equals AReP.ReviewEndpointId
                            join EPS in c.EndpointSystems
                                on AReP.EndpointSystemId equals EPS.EndpointSystemId
                            select new AttestationActionReportData
                            {
                                reviewId = attReview.ReviewId,
                                useName = attReview.User.UserName,
                                displayName = attReview.User.DisplayName,
                                endPointSystemName = EPS.EndPointSystemType.DisplayName,
                                endPointId = EPS.EndpointSystemId,
                                endPointHostName = EPS.DisplayName,
                                actionId = attReview.ActionId,
                                actionTakenBy = attReview.ActionTakenBy,
                                actionComments = attReview.ActionComments
                            }).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationsByGroupAndRoleLevelReportData> getAttestationsByGroupAndRoleLevel(int _attestationId, int _actionId)
        {
            try
            {
                IList<AttestationEndpoint> listOfEndPoints = new List<AttestationEndpoint>();
                IList<AttestationsByGroupAndRoleLevelReportData> returnList = new List<AttestationsByGroupAndRoleLevelReportData>();
                IList<AttestationsByGroupAndRoleLevelReportData> tempList = new List<AttestationsByGroupAndRoleLevelReportData>();
                EndPoint ep = new EndPoint();
                User dimUser = new User();
                ADSUserInfo ads = new ADSUserInfo();
                LotusUserInfo lns = new LotusUserInfo();
                SAPUserinfo sap = new SAPUserinfo();
                SAPEPUserInfo sapep = new SAPEPUserInfo();
                bool canBeUsed;

                using (IACMDBContext c = new IACMDBContext())
                {
                    tempList = (from attest in c.Attestations
                                where attest.AttestationId == _attestationId
                                join tsk in c.Tasks
                                    on attest.AttestationId equals tsk.AttestationId
                                join ReP in c.ReviewEndpointSystems
                                    on tsk.TaskId equals ReP.TaskId
                                join AReP in c.AttestationReviewEndpointSystems
                                    on ReP.ReviewEndpointId equals AReP.ReviewEndpointId
                                join usr in c.Users
                                    on tsk.ActionUserId equals usr.UserId
                                join ARePL2 in c.AttestationReviewEndpointSystemL2Details
                                    on ReP.ReviewEndpointId equals ARePL2.ReviewEndpointId
                                select new AttestationsByGroupAndRoleLevelReportData
                                {
                                    //Userr = usr.UserName,
                                    //DisplayName = usr.DisplayName,
                                    HostName = AReP.HostName,
                                    EndpointSystemTypeName = AReP.EndpointSystemTypeName,
                                    AccessType = ARePL2.AccessType,
                                    AccessName = ARePL2.AccessName,
                                    SubmittedBy = usr.DisplayName,
                                    RequestedAction = tsk.IsCompleted ? (ARePL2.Approve ? "Approved" : (ARePL2.Revoke ? "Revoked" : "")) : "Not Attempted",
                                    Comments = ARePL2.Comments,
                                    UserId = AReP.EndpointUserId,
                                    EndpointSystemName = AReP.HostName,
                                    EndpointSystemId = AReP.EndpointSystemId,
                                    EndpointSystemTypeId = AReP.EndpointSystemTypeId,
                                    WorkflowConfigurationId = attest.WorkflowConfigurationId,
                                    Level = tsk.WorkflowStateId
                                }).ToList();

                    foreach (AttestationsByGroupAndRoleLevelReportData item in tempList)
                    {
                        canBeUsed = false;
                        if (_actionId == 4) // All
                            canBeUsed = true;
                        else if (_actionId == 1 && item.RequestedAction == "Approved")
                            canBeUsed = true;
                        else if (_actionId == 2 && item.RequestedAction == "Revoked")
                            canBeUsed = true;
                        else if (_actionId == 3 && item.RequestedAction == "Not Attempted")
                            canBeUsed = true;

                        if (canBeUsed && item.EndpointSystemTypeId == (int)EndpointType.ActiveDirectory)
                        {
                            ADSUserInfo usr = c.ADSUserInfos.Where(adsusr => adsusr.UserId == item.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                item.EndpointUserName = usr.sAMAccountName;
                                item.AccessType = EntitlementType.Group;
                                dimUser = c.Users.FirstOrDefault(di => di.UserId == usr.DIUserId);
                                if (dimUser != null)
                                {
                                    item.Userr = dimUser.UserName;
                                    item.DisplayName = dimUser.DisplayName;
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (canBeUsed && item.EndpointSystemTypeId == (int)EndpointType.SAP)
                        {
                            SAPUserinfo usr = c.SAPUserinfos.Where(epusr => epusr.UserId == item.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                item.EndpointUserName = usr.UserLogonName;
                                item.AccessType = EntitlementType.Role;
                                dimUser = c.Users.FirstOrDefault(di => di.UserId == usr.DIUserId);
                                if (dimUser != null)
                                {
                                    item.Userr = dimUser.UserName;
                                    item.DisplayName = dimUser.DisplayName;
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (canBeUsed && item.EndpointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                        {
                            SAPEPUserInfo usr = c.SAPEPUserInfos.Where(epusr => epusr.UserId == item.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                item.EndpointUserName = usr.UserName;
                                item.AccessType = EntitlementType.Role;
                                dimUser = c.Users.FirstOrDefault(di => di.UserId == usr.DIUserId);
                                if (dimUser != null)
                                {
                                    item.Userr = dimUser.UserName;
                                    item.DisplayName = dimUser.DisplayName;
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (canBeUsed && item.EndpointSystemTypeId == (int)EndpointType.LotusNotes)
                        {
                            LotusUserInfo usr = c.LotusUserInfos.Where(epusr => epusr.UserId == item.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                item.EndpointUserName = usr.ShortName;
                                item.AccessType = EntitlementType.Group;
                                dimUser = c.Users.FirstOrDefault(di => di.UserId == usr.DIUserId);
                                if (dimUser != null)
                                {
                                    item.Userr = dimUser.UserName;
                                    item.DisplayName = dimUser.DisplayName;
                                }
                                returnList.Add(item);
                            }
                        }
                        else if (canBeUsed && item.EndpointSystemTypeId == (int)EndpointType.InhouseSystems)
                        {
                            InhouseUserInfo usr = c.InhouseUserInfos.Where(epusr => epusr.UserId == item.UserId).FirstOrDefault();
                            if (usr != null)
                            {
                                item.EndpointUserName = usr.UserName;
                                item.AccessType = EntitlementType.Role;
                                dimUser = c.Users.FirstOrDefault(di => di.UserId == usr.DIUserId);
                                if (dimUser != null)
                                {
                                    item.Userr = dimUser.UserName;
                                    item.DisplayName = dimUser.DisplayName;
                                }
                                returnList.Add(item);
                            }
                        }
                    }

                    return returnList.Distinct().OrderByDescending(eps => eps.EndpointSystemName).ThenByDescending(res => res.Level).ThenBy(name => name.DisplayName).ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> getAuthorizationDataForAttestationReviewGrid(string paramL2DetailId, int paramEndPointSystemId)
        {
            IList<AttestationReviewPageSAP_Level1GridData> returnValue = new List<AttestationReviewPageSAP_Level1GridData>();
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    int EndpointSystemId = paramEndPointSystemId;
                    string RoleCode = paramL2DetailId;

                    List<SapRoleToAuthorizationReportData> RoleToAuthorizationData = new SAPConnectorDA().GetSapRolesToAuthorizationData(RoleCode, EndpointSystemId);

                    if (RoleToAuthorizationData.Count > 0)
                    {

                        returnValue = (from Authorization in RoleToAuthorizationData
                                       select new AttestationReviewPageSAP_Level1GridData
                                       {
                                           sapLevel1DetailId = "Authorization;" + Authorization.AuthCode + ";" + paramEndPointSystemId,
                                           sapLevel1Detail = string.Format("Authorization Code: {0} Object: {1}", Authorization.AuthCode, Authorization.ObjectCode),
                                           sapLevel1DetailDesc = string.Format("Field: {0} From Value: {1}", Authorization.FieldCode, Authorization.FromValue),
                                           sapLevel1DetailType = "Auhtorization"
                                       }).ToList();
                    }




                }
            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public IList<AttestationReviewPageSAP_Level1GridData> getSAPLevel1InfosForAttestationReviewGrid(long paramL2DetailId, int paramEndPointSystemId)
        {
            try
            {
                IList<AttestationReviewPageSAP_Level1GridData> returnValue = new List<AttestationReviewPageSAP_Level1GridData>();
                using (IACMDBContext c = new IACMDBContext())
                {
                    AttestationReviewEndpointSystemL2Detail l2 = c.AttestationReviewEndpointSystemL2Details.FirstOrDefault(AT => AT.L2DetailId == paramL2DetailId);

                    int EndpointSystemId = paramEndPointSystemId;
                    string RoleCode = l2.AccessName;

                    List<SAPTCodeInfo> sapTCodes = new SAPConnectorDA().GetSapRolesToTCodeData(RoleCode, EndpointSystemId);

                    if (sapTCodes.Count > 0)
                    {
                        sapTCodes.Add(new SAPTCodeInfo { TCODE_Code = "Authorization" });
                        returnValue = (from tCodes in sapTCodes
                                       select new AttestationReviewPageSAP_Level1GridData
                                       {
                                           sapLevel1DetailId = "TCODE;" + tCodes.TCODE_Code + ";" + "Role;" + RoleCode + ";" + paramEndPointSystemId,
                                           sapLevel1Detail = tCodes.TCODE_Code,
                                           sapLevel1DetailDesc = tCodes.TCODE_Desc,
                                           sapLevel1DetailType = "TCODE"
                                       }).ToList();
                        returnValue[returnValue.Count - 1].sapLevel1DetailType = "Authorization Role:" + RoleCode;
                    }



                    return returnValue;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> getSAPLevel2InfosForAttestationReviewGrid(string paramSingleRoleCode, int paramEndPointSystemId)
        {
            try
            {
                IList<AttestationReviewPageSAP_Level1GridData> returnValue = new List<AttestationReviewPageSAP_Level1GridData>();
                using (IACMDBContext c = new IACMDBContext())
                {
                    IList<SAPTCodeInfo> sapTCodes = c.SAPSingleRoleInfos.FirstOrDefault(rle => rle.EndPointSystemId == paramEndPointSystemId &&
                                                                                                                                     rle.SingleRole_Code == paramSingleRoleCode).SAPTCodes.ToList();

                    if (sapTCodes.Count() > 0)
                    {
                        returnValue = (from tCodes in sapTCodes
                                       select new AttestationReviewPageSAP_Level1GridData
                                       {
                                           sapLevel1DetailId = "TCODE;" + tCodes.TCODE_Code + ";" + paramEndPointSystemId,
                                           sapLevel1Detail = tCodes.TCODE_Code,
                                           sapLevel1DetailDesc = tCodes.TCODE_Desc,
                                           sapLevel1DetailType = "TCODE"
                                       }).ToList();
                    }
                    return returnValue;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string getRoleDescription(long paramL2DetailId)
        {
            string returnValue = "";
            SAPSingleRoleInfo sRoleInfo = new SAPSingleRoleInfo();
            SAPCompositeRoleInfo cRoleInfo = new SAPCompositeRoleInfo();

            SAPRoleInfo sapRole = new SAPRoleInfo();
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    AttestationReviewEndpointSystemL2Detail l2 = c.AttestationReviewEndpointSystemL2Details.FirstOrDefault(ATL2 => ATL2.L2DetailId == paramL2DetailId);

                    sapRole = c.SAPRoleInfos.FirstOrDefault(roleInfo => roleInfo.ROLE_CODE == l2.AccessName);
                    if (sapRole != null)
                        returnValue = sapRole.ROLE_DESC;

                    //if (l2 != null && l2.AccessType.Equals("Single", StringComparison.InvariantCultureIgnoreCase) == true)
                    //{
                    //    sRoleInfo = _context.SAPSingleRoleInfos.FirstOrDefault(sRole => sRole.SingleRole_Code == l2.AccessName);
                    //    if (sRoleInfo != null)
                    //        returnValue = sRoleInfo.SingleRole_Desc;
                    //}
                    //else if (l2 != null && l2.AccessType.Equals("Composite", StringComparison.InvariantCultureIgnoreCase) == true)
                    //{
                    //     cRoleInfo = _context.SAPCompositeRoleInfos.FirstOrDefault(cRole => cRole.CompositeRole_Code == l2.AccessName);
                    //     if (cRoleInfo != null)
                    //         returnValue = cRoleInfo.CompositeRole_Desc;
                    //}
                }
                return returnValue;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SAPRoleRequestReportData> GetAllSAPRoleRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sap in c.SAPUserinfos
                                on rvwEPS.EndpointUserName equals sap.UserLogonName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAP
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId

                            select new SAPRoleRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sap.Firstname + " " + sap.Lastname,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPSL2.LastModifiedBy,
                                ReviewedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Role = rvwEPSL2.AccessName,
                                Approve = rvwEPSL2.Approve,
                                Revoke = rvwEPSL2.Revoke,
                                IsChanged = rvwEPSL2.IsChanged
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRoleRequestReportData> GetAllSapEXRoleRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {

                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sap in c.SAPUserinfos
                                on rvwEPS.EndpointUserName equals sap.UserLogonName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAP
                            join rvwEPSL2 in c.AttestationReviewEndpointSystemL2Details
                                on rvwEPS.ReviewEndpointId equals rvwEPSL2.ReviewEndpointId
                            where rvwEPSL2.Revoke
                            select new SAPRoleRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sap.Firstname + " " + sap.Lastname,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPSL2.LastModifiedBy,
                                RevokedOn = rvwEPSL2.LastModifiedDate,
                                L2DetailId = rvwEPSL2.L2DetailId,
                                Comments = rvwEPSL2.Comments,
                                Role = rvwEPSL2.AccessName
                            }).GroupBy(rvw => rvw.L2DetailId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRevokeRequestReportData> GetAllSAPRevokeRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sap in c.SAPUserinfos
                           on rvwEPS.EndpointUserName equals sap.UserLogonName
                            where rvwEPS.Revoke && rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAP
                            select new SAPRevokeRequestReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sap.UserLogonName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                RevokedBy = rvwEPS.LastModifiedBy,
                                RevokedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public object GetAllSAPAccountRequests()
        {
            try
            {
                using (IACMDBContext c = new IACMDBContext())
                {
                    return (from attPro in c.AttestationProcesses
                            join attestattion in c.Attestations
                                on attPro.AttestationId equals attestattion.AttestationId
                            join task in c.Tasks
                                on attestattion.AttestationId equals task.AttestationId.Value
                            join taskRvw in c.TaskReviews
                                on task.TaskId equals taskRvw.TaskId
                            join attRvw in c.AttestationReviews
                                on taskRvw.ReviewId equals attRvw.ReviewId
                            where attRvw.HistoryId == attPro.HistoryId.Value
                            join rvwEP in c.ReviewEndpointSystems
                                on attRvw.ReviewId equals rvwEP.ReviewId
                            join rvwEPS in c.AttestationReviewEndpointSystems
                                on rvwEP.ReviewEndpointId equals rvwEPS.ReviewEndpointId
                            join sap in c.SAPUserinfos
                           on rvwEPS.EndpointUserName equals sap.UserLogonName
                            where rvwEPS.EndpointSystemTypeId == (byte)EndpointType.SAP
                            select new SAPAccountRequestsReportData
                            {
                                AttestationName = attestattion.AttestationName,
                                AccountName = rvwEPS.EndpointUserName,
                                DisplayName = sap.UserLogonName,
                                Endpoint = rvwEPS.EndpointSystemTypeName,
                                HostName = rvwEPS.HostName,
                                ReviewEndpointId = rvwEPS.ReviewEndpointId,
                                ReviewedBy = rvwEPS.LastModifiedBy,
                                ReviewedOn = rvwEPS.LastModifiedDate,
                                Comments = rvwEPS.Comments,
                                Approve = rvwEPS.Approve,
                                Revoke = rvwEPS.Revoke,
                                IsChanged = rvwEPS.IsChanged
                            }).GroupBy(rvw => rvw.ReviewEndpointId).Select(rvw => rvw.FirstOrDefault()).OrderByDescending(rvw => rvw.IsChanged).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public void Save(Attestation _objTobeUpdated)
        {
            try
            {
                using (IACMDBContext db = new IACMDBContext())
                {
                    if ((_objTobeUpdated.AttestationId == -1) | (_objTobeUpdated.AttestationId == 0))
                    {
                        _objTobeUpdated.WorkflowConfiguration = null;
                        db.Attestations.Add(_objTobeUpdated);
                    }
                    else
                    {
                        _objTobeUpdated.WorkflowConfiguration = null;
                        db.Attestations.Attach(_objTobeUpdated);

                        IList<AttestationEndpoint> delattEnd = new List<AttestationEndpoint>();
                        delattEnd = db.AttestationEndpoints.Where(item => item.AttestationId == _objTobeUpdated.AttestationId).ToList();

                        if (delattEnd != null)
                        {
                            foreach (var end in delattEnd)
                            {
                                db.AttestationEndpoints.Remove(end);
                                db.SaveChanges();
                            }
                        }

                        foreach (AttestationEndpoint attEnd in _objTobeUpdated.AttestationEndpoint)
                        {
                            db.AttestationEndpoints.Attach(attEnd);
                            db.Entry(attEnd).State = EntityState.Added;
                        }

                        foreach (AttestationProcess ap in _objTobeUpdated.AttestationProcesses)
                        {
                            db.AttestationProcesses.Attach(ap);
                            db.Entry(ap).State = EntityState.Modified;
                        }
                        db.Entry(_objTobeUpdated).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Saving the Attestation", ex);
                throw ex;
            }
        }

        public void DeleteAttestation(int _AttestationId)
        {
            try
            {
                using (IACMDBContext _db = new IACMDBContext())
                {
                    Attestation att = _db.Attestations.Where(item => item.AttestationId == _AttestationId).FirstOrDefault();
                    if (att != null)
                    {
                        AttestationProcess attprocess = _db.AttestationProcesses.Where(item => item.AttestationId == _AttestationId).FirstOrDefault();
                        Process process = _db.Process.Where(processid => processid.ProcessTypeKeyId == attprocess.AttestationProcessId).FirstOrDefault();

                        if ((attprocess != null) && (process != null))
                        {
                            _db.AttestationProcesses.Remove(attprocess);
                            _db.Process.Remove(process);
                        }
                        _db.Attestations.Remove(att);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsAttestationExistInTask(int _AttestationId)
        {
            using (IACMDBContext _db = new IACMDBContext())
            {
                bool attIdExists = false;
                if (_AttestationId > 0)
                {
                    attIdExists = _db.Tasks.Any(attId => attId.AttestationId == _AttestationId);
                }
                return attIdExists;
            }
        }

        #region Added by Yasila.H on 02.11.2015 for V5 Changes
        public List<Attestation> GetAttestationName()
        {
            using (var db = new IACMDBContext())
            {
                return db.Attestations.ToList();
            }
        }

        public bool IsProcessNameExists(string processName)
        {
            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    return context.AttestationProcesses.Any(item => item.ProcessName.Equals(processName, StringComparison.CurrentCultureIgnoreCase));
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
