using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class AttestationRepository : RepositoryBase<Attestation>
    {
        public AttestationRepository(DataAccessTransaction transaction) : base(transaction) { }
        

        public IList<Attestation> GetByEndpointSystemType(EndpointType endpointSystemType)
        {
            return (from attestation in CurrentContext.Attestations
                    join attestationEP in CurrentContext.AttestationEndpoints
                    on attestation.Id equals attestationEP.AttestationId
                    where attestationEP.EndpointSystemType == endpointSystemType
                    select attestation).ToList();
        }
        public IList<PresentationSapUserRoleTCodeAuthorization> GetSapUserRoleTCodeAuthorization(Task task)
        {
            var SapUserToRoles = (from action in CurrentContext.AttestationSnapshotActions
                                  join snapshot in CurrentContext.AttestationSnapshots
                                  on action.AttestationSnapshotId equals snapshot.Id
                                  where action.TaskId == task.Id
                                  select new PresentationSapUserRoleTCodeAuthorization
                                  {
                                      UserName = snapshot.ParentEntity,
                                      RoleCode = snapshot.Name,
                                      EndpointSystemId = (int)snapshot.EndpointId
                                  }).ToList();

            //var SapUserToRoles = (from taskReview in task.TaskReviews
            //                      from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
            //                      from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
            //                      select new PresentationSapUserRoleTCodeAuthorization
            //                      {
            //                          UserName = reviewEP.AttestationReviewEndpointSystem.EndpointUserName,
            //                          RoleCode = reviewEPL2.AccessName,
            //                          EndpointSystemId = (int)reviewEP.AttestationReviewEndpointSystem.EndpointSystemId
            //                      }).ToList();
            return SapUserToRoles;
        }


        public IList<PresentationListAttestation> GetAllAttestationsForActionUsers(long paramActionUserId)
        {

            return (from attestation in CurrentContext.Attestations
                    join task in CurrentContext.Tasks
                        on attestation.Id equals task.AttestationId
                    where (task.ActionUserId == paramActionUserId || paramActionUserId == -1)
                    join attestationProcess in CurrentContext.AttestationProcesses
                        on attestation.Id equals attestationProcess.AttestationId
                    join attestationUser in CurrentContext.User
                        on task.ActionUserId equals attestationUser.Id
                    select new PresentationListAttestation
                    {
                        AttestationId = attestation.Id,
                        AttestationName = attestation.AttestationName,
                        RunDate = attestation.CreatedDate,
                        AttesationCreatedBy = attestation.CreatedBy
                    }).ToList();
        }

        public IList<PresentationListAttestation> GetAllAttestationList(long endpointSystemTypeId)
        {
            var attestations = (from attestation in CurrentContext.Attestations
                                    //join attPrcs in CurrentContext.AttestationProcesses on attestation.Id equals attPrcs.AttestationId
                                join attEndpoint in CurrentContext.AttestationEndpoints on attestation.Id equals attEndpoint.AttestationId
                                join end in CurrentContext.EndPoint on attEndpoint.EndpointSystemId equals end.Id
                                where endpointSystemTypeId == 0 || end.EndpointSystemTypeId == endpointSystemTypeId
                                select new PresentationListAttestation
                                {
                                    AttestationId = attestation.Id,
                                    AttestationName = attestation.AttestationName,
                                    AttestationDescription = attestation.AttestationDescription,
                                    //Status = attPrcs.WorkflowStatus,
                                    CreatedDate = attestation.CreatedDate,
                                    AttesationCreatedBy = attestation.CreatedBy,
                                    CreatedBy = attestation.CreatedBy,
                                    LastModifiedBy = attestation.LastModifiedBy,
                                    LastModifiedDate = attestation.LastModifiedDate,
                                    SplitCampaignUsing = attestation.SplitCampaignUsing,
                                    EndPointSystem = String.Empty,
                                    AttestationRemediation = (attestation.RequiresAttestationRemediation == true) ? "Yes" : "No"
                                }).OrderByDescending(attestation => attestation.AttestationId).Distinct().ToList();

            foreach (var att in attestations)
            {
                att.EndPointSystem = GetAttestationEndpointByAttestationId(att.AttestationId);
            }

            return attestations;
        }
        public string GetAttestationEndpointByAttestationId(long attestationId)
        {
            string endPointsString = string.Empty;
            var atts = (from atnEndpoint in CurrentContext.AttestationEndpoints
                        join endPointType in CurrentContext.EndpointSystemTypes on atnEndpoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                        where atnEndpoint.AttestationId == attestationId
                        select endPointType.DisplayName).Distinct().OrderByDescending(x => x).ToList();

            endPointsString = String.Join(", ", atts?.ToArray());

            return endPointsString;
        }
        public IList<PresentationListAttestation> GetAllAttestationByEndpointSystemTypeId(long endpointSystemTypeId)
        {
            return (from attestation in CurrentContext.Attestations
                        //join attPrcs in CurrentContext.AttestationProcesses on attestation.Id equals attPrcs.AttestationId
                    join attEndpoint in CurrentContext.AttestationEndpoints on attestation.Id equals attEndpoint.AttestationId
                    join end in CurrentContext.EndPoint on attEndpoint.EndpointSystemId equals end.Id
                    where end.EndpointSystemTypeId == endpointSystemTypeId
                    select new PresentationListAttestation
                    {
                        AttestationId = attestation.Id,
                        AttestationName = attestation.AttestationName,
                        AttestationDescription = attestation.AttestationDescription,
                        //Status = attPrcs.WorkflowStatus,
                        CreatedDate = attestation.CreatedDate,
                        AttesationCreatedBy = attestation.CreatedBy,
                        EndPointSystem = end.DisplayName,
                        AttestationRemediation = (attestation.RequiresAttestationRemediation == true) ? "Yes" : "No"
                    }).OrderByDescending(attestation => attestation.AttestationId).Distinct().ToList();
        }

        public IList<PresentationAttestationReviewPageSAP_Level1> GetByParamEndpointSystemId(long paramEndpointSystemId, IList<SAPTCodeInfo> sapTCodes)
        {
            return (from tCodes in sapTCodes
                    select new PresentationAttestationReviewPageSAP_Level1
                    {
                        sapLevel1DetailId = "TCODE;" + tCodes.TCODE_Code + ";" + paramEndpointSystemId,
                        sapLevel1Detail = tCodes.TCODE_Code,
                        sapLevel1DetailDesc = tCodes.TCODE_Desc,
                        sapLevel1DetailType = "TCODE"
                    }).ToList();
        }

        public PresentationGridPagedData<PresentationMyRequestData> GetByActionUserId(long actionUserId, int startRowIndex, int maximumRows, string filterExpression, string sortExpression)
        {
            PresentationGridPagedData<PresentationMyRequestData> dbResult = new PresentationGridPagedData<PresentationMyRequestData>();
            IQueryable<PresentationMyRequestData> query = null;
            List<PresentationMyRequestData> requestListAll = new List<PresentationMyRequestData>();

            var requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                               join acssrqst in CurrentContext.AccessRequest
                               on rqst.Id equals acssrqst.AdHocAccessRequestId
                               join rusr in CurrentContext.User.Include("NOrganisation")
                               on rqst.RequesteeUserId equals rusr.Id
                               join rqust in CurrentContext.User
                               on rqst.RequesterUserId equals rqust.Id
                               where rqst.RequesterUserId == actionUserId
                               select new PresentationMyRequestData
                               {
                                   AdHocAccessRequestId = rqst.Id,
                                   BulkRequestId = 0,
                                   Name = rqst.EndPoint.DisplayName,
                                   ApplicationType = rqst.ApplicationType,
                                   RequestSource = Constants.AccessRequest,
                                   RequestedDate = rqst.CreatedDate,
                                   ExecutionDate = acssrqst.ExecutionTime,
                                   RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                   Comments = rqst.Comments,
                                   RequestStatus = acssrqst.RequestStatus,
                                   AccessRequestID = acssrqst.Id,
                                   RequesteeName = rusr.DisplayName,
                                   RequesterName = rqust.DisplayName,
                                   AccountType = rqst.AccountType,
                                   ValidFrom = rqst.ValidFrom,
                                   ValidTo = rqst.ValidTo,
                                   AdhocActionType = rqst.AdhocActionType,
                                   ProgressStatus = acssrqst.ProgressStatus,
                                   IsAdhocRequest = true,
                                   IsBulkRequest = false,
                                   Entitlement = rqst.EntitlementDetail
                               });

            var bulkRequest = (from rqst in CurrentContext.BulkRequests.Include("BulkRequestDetails").Include("EndpointSystemId")
                               join usr in CurrentContext.User
                              on rqst.RequestedBy equals usr.Id
                               where rqst.RequestedBy == actionUserId
                               select new PresentationMyRequestData()
                               {
                                   AdHocAccessRequestId = 0,
                                   BulkRequestId = rqst.Id,
                                   Name = (rqst.EndPoint == null ? "" : rqst.EndPoint.DisplayName),
                                   ApplicationType = rqst.ApplicationType,
                                   RequestSource = Constants.BulkRequest,
                                   RequestedDate = rqst.CreatedDate,
                                   ExecutionDate = rqst.ExecutionTime,
                                   RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Amendment-Add" : "Amendment-Remove") : ((int)rqst.AdhocActionType == 1 ? "New Account" : "Delete Account"),
                                   Comments = rqst.Comments,
                                   RequestStatus = rqst.RequestStatus,
                                   AccessRequestID = rqst.Id,
                                   RequesteeName = string.Empty,
                                   RequesterName = usr.DisplayName,
                                   AccountType = AccountType.User,
                                   ValidFrom = rqst.ActivationDate,
                                   ValidTo = rqst.DeactivationDate,
                                   AdhocActionType = rqst.AdhocActionType,
                                   ProgressStatus = rqst.ProgressStatus,
                                   IsAdhocRequest = false,
                                   IsBulkRequest = true,
                                   Entitlement = rqst.EntitlementDetail,
                               });

            var employeeEvents = (from trigger in CurrentContext.EmployeeEventTriggers
                                  join rusr in CurrentContext.User
                                  on trigger.UserId equals rusr.Id
                                  join rqust in CurrentContext.User
                                  on trigger.CreatedBy equals rqust.UserName
                                  where rqust.Id == actionUserId &&
                                  (trigger.EventType == EmployeeEvents.NonEmployeeExtension || trigger.EventType == EmployeeEvents.NonEmployeeCreation)
                                  select new PresentationMyRequestData()
                                  {
                                      AdHocAccessRequestId = trigger.Id,
                                      BulkRequestId = 0,
                                      Name = "Non Employee",
                                      ApplicationType = ApplicationType.None,
                                      RequestSource = Constants.NonEmployee,
                                      RequestedDate = trigger.CreatedDate,
                                      ExecutionDate = null,
                                      RequestType = trigger.EventType == EmployeeEvents.NonEmployeeCreation ? "Non Employee Creation" : (trigger.EventType == EmployeeEvents.NonEmployeeExtension ? "Non Employee Extension / Shortening" : string.Empty),
                                      Comments = trigger.Comments,
                                      RequestStatus = trigger.RequestStatus,
                                      AccessRequestID = trigger.Id,
                                      RequesteeName = rusr.DisplayName,
                                      RequesterName = rqust.DisplayName,
                                      AccountType = AccountType.None,
                                      ValidFrom = null,
                                      ValidTo = null,
                                      AdhocActionType = AdHocActionType.None,
                                      ProgressStatus = AccessRequestProgress.None,
                                      IsAdhocRequest = false,
                                      IsBulkRequest = false,
                                      Entitlement = string.Empty
                                  });

            query = requestList.Union(bulkRequest).Union(employeeEvents);

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }

            dbResult.TotalCount = query.Count();

            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.RequestedDate);
            }

            if (maximumRows == -1)
                dbResult.Data = query.ToList();
            else
                dbResult.Data = query.Skip(startRowIndex).Take(maximumRows).ToList();

            var bulk = dbResult.Data.Where(i => i.IsBulkRequest).Select(i => i.BulkRequestId);

            var t = CurrentContext.BulkRequestDetails.Select(i => new { i.BulkRequestId, i.Name }).Where(x => bulk.Contains(x.BulkRequestId)).GroupBy(i => i.BulkRequestId).ToList().ToDictionary(g => g.Key, g => string.Join(",", g.Select(i => i.Name)));

            foreach (var item in dbResult.Data)
            {
                if (item.IsBulkRequest)
                {
                    item.RequesteeName = t.ContainsKey(item.BulkRequestId.Value) ? t[item.BulkRequestId.Value] : string.Empty;
                }
            }

            return dbResult;

        }

        public IList<PresentationMyRequestData> GetOnlyEventTriggedAccessRequests()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var eventTriggedAccessRequests = (from trigger in CurrentContext.EmployeeEventTriggers
                                                  join rusr in CurrentContext.User
                                                  on trigger.UserId equals rusr.Id
                                                  where trigger.CreatedBy == "System"
                                                  select new PresentationMyRequestData()
                                                  {
                                                      AdHocAccessRequestId = trigger.Id,
                                                      Name = "Employee Event",
                                                      ApplicationType = ApplicationType.None,
                                                      RequestSource = "Employee Event",
                                                      RequestedDate = trigger.CreatedDate,
                                                      ExecutionDate = null,
                                                      RequestType = trigger.EventType.ToString(),
                                                      Comments = trigger.Comments,
                                                      RequestStatus = trigger.RequestStatus,
                                                      AccessRequestID = trigger.Id,
                                                      RequesteeName = rusr.DisplayName,
                                                      RequesterName = trigger.CreatedBy,
                                                      AccountType = AccountType.None,
                                                      ValidFrom = null,
                                                      ValidTo = null,
                                                      IsAdhocRequest = false,
                                                      IsBulkRequest = false
                                                  }).ToList();

                if (eventTriggedAccessRequests == null)
                    return null;
                return eventTriggedAccessRequests.OrderByDescending(item => item.RequestedDate).ToList();
            }
        }

        public IList<PresentationMyRequestData> GetByActionUserDepartment(long actionUserId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var loginUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
                if (loginUser == null) return new List<PresentationMyRequestData>();
                IList<PresentationMyRequestData> requestList = new List<PresentationMyRequestData>();
                requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                               join acssrqst in CurrentContext.AccessRequest
                               on rqst.Id equals acssrqst.AdHocAccessRequestId
                               join rusr in CurrentContext.User
                               on rqst.RequesteeUserId equals rusr.Id
                               join rqust in CurrentContext.User
                               on rqst.RequesterUserId equals rqust.Id into rquster
                               from rqust in rquster.DefaultIfEmpty()
                               where rusr.NOrganisationId == loginUser.NOrganisationId && !acssrqst.IsDependentRequest
                               && !acssrqst.EmployeeEventTriggerId.HasValue
                               select new PresentationMyRequestData
                               {
                                   AdHocAccessRequestId = rqst.Id,
                                   Name = rqst.EndPoint.DisplayName,
                                   RequestedDate = rqst.CreatedDate,
                                   ExecutionDate = acssrqst.ExecutionTime,
                                   //  RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                                   RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : (int)rqst.AdhocActionType == 4 ? "Remove Login-ID" : (int)rqst.AdhocActionType == 5 ? "Standing Order" : (int)rqst.AdhocActionType == 6 ? "Extended Request" : ""),
                                   Comments = rqst.Comments,
                                   RequestStatus = acssrqst.RequestStatus,
                                   AccessRequestID = acssrqst.Id,
                                   RequesteeName = rusr.DisplayName,
                                   RequesteeUserName = rusr.UserName,
                                   RequesterName = rqust == null ? "" : rqust.DisplayName,
                                   RequesterUserName = rqust == null ? "" : rqust.UserName,
                                   AccountType = rqst.AccountType,
                                   ValidFrom = rqst.ValidFrom,
                                   ValidTo = rqst.ValidTo,
                                   AdhocActionType = rqst.AdhocActionType,
                                   ProgressStatus = acssrqst.ProgressStatus,
                                   ParentrequestId = acssrqst.ParentRequestId
                               }).ToList();

                foreach (PresentationMyRequestData _rqst in requestList)
                {
                    _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                }
                return requestList;
            }
        }

        public PagedResults<PresentationMyRequestData> GetByActionUserDepartmentPaging(long actionUserId, int? pageSize, int? pageNumber)
        {
            var loginUser = CurrentContext.User.Where(a => a.Id == actionUserId).FirstOrDefault();
            if (loginUser == null) return new PagedResults<PresentationMyRequestData>()
            {
                Results = new List<PresentationMyRequestData>(),
                TotalNumberOfRecords = 0,
                PageNumber = 0,
                PageSize = 0
            };
            IList<PresentationMyRequestData> requestList = new List<PresentationMyRequestData>();
            requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                           join acssrqst in CurrentContext.AccessRequest
                           on rqst.Id equals acssrqst.AdHocAccessRequestId
                           join rusr in CurrentContext.User
                           on rqst.RequesteeUserId equals rusr.Id
                           join rqust in CurrentContext.User
                           on rqst.RequesterUserId equals rqust.Id into rquster
                           from rqust in rquster.DefaultIfEmpty()
                           where rusr.NOrganisationId == loginUser.NOrganisationId && !acssrqst.IsDependentRequest
                           && !acssrqst.EmployeeEventTriggerId.HasValue
                           select new PresentationMyRequestData
                           {
                               AdHocAccessRequestId = rqst.Id,
                               Name = rqst.EndPoint.DisplayName,
                               RequestedDate = rqst.CreatedDate,
                               ExecutionDate = acssrqst.ExecutionTime,
                               RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : (int)rqst.AdhocActionType == 4 ? "Remove Login-ID" : (int)rqst.AdhocActionType == 5 ? "Standing Order" : (int)rqst.AdhocActionType == 6 ? "Extended Request" : ""),
                               Comments = rqst.Comments,
                               RequestStatus = acssrqst.RequestStatus,
                               AccessRequestID = acssrqst.Id,
                               RequesteeName = rusr.DisplayName,
                               RequesteeUserName = rusr.UserName,
                               RequesterName = rqust == null ? "" : rqust.DisplayName,
                               RequesterUserName = rqust == null ? "" : rqust.UserName,
                               AccountType = rqst.AccountType,
                               ValidFrom = rqst.ValidFrom,
                               ValidTo = rqst.ValidTo,
                               AdhocActionType = rqst.AdhocActionType,
                               ProgressStatus = acssrqst.ProgressStatus,
                               ParentrequestId = acssrqst.ParentRequestId
                           }).ToList();

            foreach (PresentationMyRequestData _rqst in requestList)
            {
                _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
            }

            var aa = requestList.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value).ToList();

            PagedResults<PresentationMyRequestData> paged = new PagedResults<PresentationMyRequestData>()
            {
                Results = aa,
                TotalNumberOfRecords = requestList.Count,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };

            return paged;
        }

        public IList<PresentationMyRequestData> GetByRequesterId(List<long> requesterIds)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<PresentationMyRequestData> requestList = new List<PresentationMyRequestData>();
                IList<PresentationMyRequestData> requestListAll = new List<PresentationMyRequestData>();
                if (requesterIds != null && requesterIds.Count > 0)
                {
                    foreach (int userid in requesterIds)
                    {
                        requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                       join acssrqst in CurrentContext.AccessRequest
                                       on rqst.Id equals acssrqst.AdHocAccessRequestId
                                       join usr in CurrentContext.User
                                       on acssrqst.RequesterId equals usr.Id
                                       join rusr in CurrentContext.User
                                       on rqst.RequesteeUserId equals rusr.Id
                                       where rqst.RequesterUserId == userid && !acssrqst.IsDependentRequest
                                       select new PresentationMyRequestData
                                       {
                                           AdHocAccessRequestId = rqst.Id,
                                           Name = rqst.EndPoint.DisplayName,
                                           RequestedDate = rqst.CreatedDate,
                                           ExecutionDate = acssrqst.ExecutionTime,
                                           RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                                           Comments = rqst.Comments,
                                           RequestStatus = acssrqst.RequestStatus,
                                           AccessRequestID = acssrqst.Id,
                                           RequesteeName = rusr.DisplayName,
                                           RequesterName = usr.DisplayName,
                                           AccountType = rqst.AccountType,
                                           ValidFrom = rqst.ValidFrom,
                                           ValidTo = rqst.ValidTo
                                       }).ToList();

                        foreach (PresentationMyRequestData _rqst in requestList)
                        {
                            _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                        }
                        foreach (PresentationMyRequestData listitem in requestList)
                        {
                            requestListAll.Add(listitem);
                        }
                    }
                }
                else
                {
                    requestListAll = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                                      join acssrqst in CurrentContext.AccessRequest
                                      on rqst.Id equals acssrqst.AdHocAccessRequestId
                                      join usr in CurrentContext.User
                                      on acssrqst.RequesterId equals usr.Id
                                      join rusr in CurrentContext.User
                                      on rqst.RequesteeUserId equals rusr.Id
                                      where !acssrqst.IsDependentRequest
                                      select new PresentationMyRequestData
                                      {
                                          AdHocAccessRequestId = rqst.Id,
                                          Name = rqst.EndPoint.DisplayName,
                                          RequestedDate = rqst.CreatedDate,
                                          ExecutionDate = acssrqst.ExecutionTime,
                                          RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                                          Comments = rqst.Comments,
                                          RequestStatus = acssrqst.RequestStatus,
                                          AccessRequestID = acssrqst.Id,
                                          RequesteeName = rusr.DisplayName,
                                          RequesterName = usr.DisplayName,
                                          AccountType = rqst.AccountType,
                                          ValidFrom = rqst.ValidFrom,
                                          ValidTo = rqst.ValidTo
                                      }).ToList();

                    foreach (PresentationMyRequestData _rqst in requestListAll)
                    {
                        _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
                    }
                }
                requestListAll = requestListAll.OrderByDescending(rqst => rqst.RequestedDate).ToList();
                return requestListAll;
            }
        }


        public PresentationMyRequestData GetByRequestId(long requestId, bool deepLoad)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (!deepLoad)
                {
                    //return oIACMDBContext.AdHocAccessRequests.FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                    return (from rqst in CurrentContext.AdHocAccessRequests
                            join acssrqst in CurrentContext.AccessRequest
                                on rqst.Id equals acssrqst.AdHocAccessRequestId
                            where rqst.Id == requestId
                            select new PresentationMyRequestData
                            {
                                AdHocAccessRequestId = rqst.Id,
                                AccessRequestID = acssrqst.Id,
                                Name = rqst.EndPoint.DisplayName + " - " + (((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? "Amendment" : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID")),
                                RequestedDate = rqst.CreatedDate,
                                RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                                Comments = rqst.Comments,
                                RequestStatus = acssrqst.RequestStatus,
                                ProgressStatus = acssrqst.ProgressStatus,
                                AdhocActionType = rqst.AdhocActionType,
                                EndpointSystemId = rqst.EndpointSystemId,
                                EndPoint = rqst.EndPoint,
                                AdHocAccessRequests = rqst,
                                StartDate = rqst.ValidFrom,
                                EndDate = rqst.ValidTo,
                                Remarks = acssrqst.Remarks,
                                AccessRequestComments = acssrqst.Comments,
                                ApplicationType = rqst.ApplicationType,
                                UserID = rqst.RequesteeUserId,
                                ConflictRemarks = acssrqst.ConflictsRemarks
                            }
                               ).FirstOrDefault();
                }
                //return oIACMDBContext.AdHocAccessRequests.Include("EndPoint").Include("MiscSystem").FirstOrDefault(item => item.AdHocAccessRequestId == requestId);
                return (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                        join acssrqst in CurrentContext.AccessRequest
                            on rqst.Id equals acssrqst.AdHocAccessRequestId
                        where rqst.Id == requestId
                        select new PresentationMyRequestData
                        {
                            AdHocAccessRequestId = rqst.Id,
                            AccessRequestID = acssrqst.Id,
                            Name = rqst.EndPoint.DisplayName + " - " + (((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? "Amendment" : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID")),
                            RequestedDate = rqst.CreatedDate,
                            RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                            Comments = rqst.Comments,
                            RequestStatus = acssrqst.RequestStatus,
                            ProgressStatus = acssrqst.ProgressStatus,
                            AdhocActionType = rqst.AdhocActionType,
                            EndpointSystemId = rqst.EndpointSystemId,
                            EndPoint = rqst.EndPoint,
                            AdHocAccessRequests = rqst,
                            StartDate = rqst.ValidFrom,
                            EndDate = rqst.ValidTo,
                            Remarks = acssrqst.Remarks,
                            ApplicationType = rqst.ApplicationType,
                            UserID = rqst.RequesteeUserId,
                            AccessRequestComments = acssrqst.Comments,
                            AccountType = rqst.AccountType,
                            ConflictRemarks = acssrqst.ConflictsRemarks
                        }
                    ).FirstOrDefault();
            }
        }

        public IList<PresentationMyRequestData> GetByProgressStatus(DI.Chakra.Infrastructure.Enums.AccessRequestProgress progressStatus)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (from rqst in CurrentContext.AdHocAccessRequests
                        join acssrqst in CurrentContext.AccessRequest.Include("User").Include("AdHocAccessRequest")
                        on rqst.Id equals acssrqst.AdHocAccessRequestId
                        join rusr in CurrentContext.User
                        on rqst.RequesteeUserId equals rusr.Id into allusr
                        from alusr in allusr.DefaultIfEmpty()
                        join rqust in CurrentContext.User
                        on rqst.RequesterUserId equals rqust.Id into allrqst
                        from alrqst in allrqst.DefaultIfEmpty()
                        where acssrqst.ProgressStatus == progressStatus
                        select new PresentationMyRequestData
                        {
                            AdHocAccessRequestId = rqst.Id,
                            AccessRequestID = acssrqst.Id,
                            Name = rqst.EndPoint.DisplayName + " - " + (((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? "Amendment" : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID")),
                            RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : "Remove Login-ID"),
                            ProgressStatus = acssrqst.ProgressStatus,
                        }).ToList();
            }
        }
        public List<PresentationAttestationStatusSummary> GetByEndPointTypeName(string endPointTypeName)
        {
            return (from endPointType in CurrentContext.EndpointSystemTypes
                    join endPointSystem in CurrentContext.EndPoint on endPointType.Id equals endPointSystem.EndpointSystemTypeId
                    join accessRequest in CurrentContext.AccessRequest on endPointSystem.Id equals accessRequest.EndpointSystemId
                    where (endPointType.DisplayName == endPointTypeName && accessRequest.RequestSource == AccessRequestSource.AttestationRemediation)
                    select new
                    {
                        ProgressStatus = accessRequest.ProgressStatus

                    }).ToList().Select(leTaskSummary => new PresentationAttestationStatusSummary
                    {
                        StatusCount = 1,

                        Status = ((leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Completed)) ? "Completed" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Executing)) ? "Executing" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ExecutionFailed)) ? "ExecutionFailed" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ReadytoExecute)) ? "ReadytoExecute" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.RequestRejected)) ? "RequestRejected" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ValidationFailed)) ? "ValidationFailed" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingforApproval)) ? "WaitingforApproval" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Withdrawn)) ? "Withdrawn" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingForSync)) ? "WaitingForSync":
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.None)) ? "None" : ""),

                        IsExploded = false,

                        BGColor = ((leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Completed)) ? "#90C3D4" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Executing)) ? "#C390D4" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ExecutionFailed)) ? "#D4A190" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ReadytoExecute)) ? "#A1D490" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.RequestRejected)) ? "#C2BF7A" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.ValidationFailed)) ? "#7AC29B" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingforApproval)) ? "#7A7DC2" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.Withdrawn)) ? "#C27AA1" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.WaitingForSync)) ? "#9AA2E4" :
                                   (leTaskSummary.ProgressStatus.Equals(AccessRequestProgress.None)) ? "black" : ""),


                    }).ToList();
        }
        public List<string> GetAttestationEndpoints()
        {
            return (from atnEndpoint in CurrentContext.AttestationEndpoints
                    join endPointType in CurrentContext.EndpointSystemTypes on atnEndpoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                    join task in CurrentContext.Tasks on atnEndpoint.AttestationId equals task.AttestationId
                    select endPointType.DisplayName).Distinct().OrderByDescending(x => x).ToList();

        }

        public List<string> GetAttestationEndpointsForActiveCampaign()
        {
            return (from atnEndpoint in CurrentContext.AttestationEndpoints
                    join endPointType in CurrentContext.EndpointSystemTypes on atnEndpoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                    join task in CurrentContext.Tasks on atnEndpoint.AttestationId equals task.AttestationId
                    join campaign in CurrentContext.AttestationCampaigns on task.AttestationCampaignId equals campaign.Id
                    where !campaign.EndDate.HasValue
                    orderby campaign.CloseDate descending
                    select endPointType.DisplayName).Distinct().ToList();

        }

        public List<string> GetAttestationByEndpointName(string endPointTypeName)
        {
            return (from atnEndpoint in CurrentContext.AttestationEndpoints
                    join endPointType in CurrentContext.EndpointSystemTypes on atnEndpoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                    join attestation in CurrentContext.Attestations on atnEndpoint.AttestationId equals attestation.Id
                    join campaign in CurrentContext.AttestationCampaigns on attestation.Id equals campaign.AttestationID
                    where (endPointType.DisplayName == endPointTypeName && !campaign.EndDate.HasValue)
                    orderby campaign.CloseDate
                    select campaign.CampaignName).Distinct().ToList();
        }

        public IList<PresentationAttestationStatusSummary> GetTaskSummaryByEndPointTypeName(string endPointTypeName)
        {
            return (from task in CurrentContext.Tasks
                    join attestation in CurrentContext.Attestations on task.AttestationId equals attestation.Id
                    join attestationEndPoint in CurrentContext.AttestationEndpoints on attestation.Id equals attestationEndPoint.AttestationId
                    join endPointType in CurrentContext.EndpointSystemTypes on attestationEndPoint.EndpointSystemType equals endPointType.EndpointSystemTypeName
                    where endPointType.DisplayName == endPointTypeName
                    select new
                    {
                        IsCompleted = task.IsCompleted,
                        IsDelegated = task.IsDelegated,
                        IsEscalated = task.IsEscalated

                    }).ToList().Select(leTaskSummary => new PresentationAttestationStatusSummary
                    {
                        StatusCount = 1,

                        Status = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "Pending" :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Completed" :
                                        (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "Delegated" :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "Escalated" : ""),


                        IsExploded = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? true :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                        (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? false :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? false : false),

                        BGColor = ((!leTaskSummary.IsCompleted && !leTaskSummary.IsCompleted) ? "#ffdd00" :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#279691" :
                                        (leTaskSummary.IsCompleted && leTaskSummary.IsDelegated && !leTaskSummary.IsEscalated) ? "#59F0BD" :
                                        (leTaskSummary.IsCompleted && !leTaskSummary.IsDelegated && leTaskSummary.IsEscalated) ? "#F51B31" : ""),


                    }).ToList();

        }

        public List<PresentationJobListAttestation> GetAttestationProcessJobs(int pageSize, int pageNumber, string filter)
        {
            List<PresentationJobListAttestation> jobs = new List<PresentationJobListAttestation>();

            if (filter.ToString() == Occurence.All.ToString())
            {
                jobs = (from attestationProcess in CurrentContext.AttestationProcesses
                        join attestation in CurrentContext.Attestations
                        on attestationProcess.AttestationId equals attestation.Id
                        join pr in CurrentContext.Process
                        on attestationProcess.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Attestation
                        orderby attestationProcess.ProcessName ascending
                        select new PresentationJobListAttestation
                        {
                            ProcessName = attestationProcess.ProcessName,
                            Description = attestationProcess.Description,
                            AttestationName = attestation.AttestationName,
                            occurence = s.Occurence.Value.ToString(),
                            AttestationProcessId = attestationProcess.Id,
                            Status = (pr.Status == null) ? "Pending" : pr.Status,
                            SchedulerDateTime = s.StartDate + " " + s.StartTime,
                            ErrorMessage = pr.ErrorMessage,
                            LastRunDate = pr.LastRunOn
                        }).ToList();
            }
            else
            {
                Occurence enumOccurence = (Occurence)Enum.Parse(typeof(Occurence), filter);

                jobs = (from attestationProcess in CurrentContext.AttestationProcesses
                        join attestation in CurrentContext.Attestations
                        on attestationProcess.AttestationId equals attestation.Id
                        join pr in CurrentContext.Process
                        on attestationProcess.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Attestation && s.Occurence == enumOccurence
                        orderby attestationProcess.ProcessName ascending
                        select new PresentationJobListAttestation
                        {
                            ProcessName = attestationProcess.ProcessName,
                            Description = attestationProcess.Description,
                            AttestationName = attestation.AttestationName,
                            occurence = s.Occurence.Value.ToString(),
                            AttestationProcessId = attestationProcess.Id,
                            Status = (pr.Status == null) ? "Pending" : pr.Status,
                            SchedulerDateTime = s.StartDate + " " + s.StartTime,
                            ErrorMessage = pr.ErrorMessage,
                            LastRunDate = pr.LastRunOn
                        }).ToList();
            }

            foreach (PresentationJobListAttestation process in jobs)
            {
                process.StartDateTime = Convert.ToDateTime(process.SchedulerDateTime);
            }
            if (pageSize != null && pageNumber != null)
            {
                jobs = jobs.Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList();
            }
            return jobs;
        }
        public List<PresentationJobListAttestation> GetAttestationJobs(string filter)
        {
            List<PresentationJobListAttestation> jobs = new List<PresentationJobListAttestation>();

            if (filter.ToString() == Occurence.All.ToString())
            {
                jobs = (from attestationProcess in CurrentContext.AttestationProcesses
                        join attestation in CurrentContext.Attestations
                        on attestationProcess.AttestationId equals attestation.Id
                        join pr in CurrentContext.Process
                        on attestationProcess.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Attestation
                        orderby attestationProcess.ProcessName ascending
                        select new PresentationJobListAttestation
                        {
                            ProcessName = attestationProcess.ProcessName,
                            Description = attestationProcess.Description,
                            AttestationName = attestation.AttestationName,
                            occurence = s.Occurence.Value.ToString(),
                            AttestationProcessId = attestationProcess.Id,
                            Status = (pr.Status == null) ? "Pending" : pr.Status,
                            SchedulerDateTime = s.StartDate + " " + s.StartTime,
                            ErrorMessage = pr.ErrorMessage,
                            LastRunDate = pr.LastRunOn
                        }).ToList();
            }
            else
            {
                Occurence enumOccurence = (Occurence)Enum.Parse(typeof(Occurence), filter);

                jobs = (from attestationProcess in CurrentContext.AttestationProcesses
                        join attestation in CurrentContext.Attestations
                        on attestationProcess.AttestationId equals attestation.Id
                        join pr in CurrentContext.Process
                        on attestationProcess.Id equals pr.ProcessTypeKeyId
                        join s in CurrentContext.Scheduler
                        on pr.Scheduler.Id equals s.Id
                        where pr.ProcessType == ProcessType.Attestation && s.Occurence == enumOccurence
                        orderby attestationProcess.ProcessName ascending
                        select new PresentationJobListAttestation
                        {
                            ProcessName = attestationProcess.ProcessName,
                            Description = attestationProcess.Description,
                            AttestationName = attestation.AttestationName,
                            occurence = s.Occurence.Value.ToString(),
                            AttestationProcessId = attestationProcess.Id,
                            Status = (pr.Status == null) ? "Pending" : pr.Status,
                            SchedulerDateTime = s.StartDate + " " + s.StartTime,
                            ErrorMessage = pr.ErrorMessage,
                            LastRunDate = pr.LastRunOn

                        }).ToList();
            }
            foreach (PresentationJobListAttestation process in jobs)
            {
                process.StartDateTime = Convert.ToDateTime(process.SchedulerDateTime);
            }
            return jobs;
        }
        public PagedResults<PresentationMyRequestData> GetByActionUserIdPaging(long actionUserId, int? pageSize, int? pageNumber)
        {
            IList<PresentationMyRequestData> requestList = new List<PresentationMyRequestData>();
            requestList = (from rqst in CurrentContext.AdHocAccessRequests.Include("EndPoint")
                           join acssrqst in CurrentContext.AccessRequest
                           on rqst.Id equals acssrqst.AdHocAccessRequestId
                           join rusr in CurrentContext.User.Include("NOrganisation")
                           on rqst.RequesteeUserId equals rusr.Id
                           join rqust in CurrentContext.User
                           on rqst.RequesterUserId equals rqust.Id
                           where rqst.RequesterUserId == actionUserId && !acssrqst.IsDependentRequest
                           select new PresentationMyRequestData
                           {
                               AdHocAccessRequestId = rqst.Id,
                               Name = rqst.EndPoint.DisplayName,
                               RequestedDate = rqst.CreatedDate,
                               ExecutionDate = acssrqst.ExecutionTime,
                               RequestType = ((int)rqst.AdhocActionType == 2 || (int)rqst.AdhocActionType == 3) ? ((int)rqst.AdhocActionType == 2 ? "Add Role" : "Remove Role") : ((int)rqst.AdhocActionType == 1 ? "Create Login-ID" : (int)rqst.AdhocActionType == 4 ? "Remove Login-ID" : (int)rqst.AdhocActionType == 5 ? "Standing Order" : (int)rqst.AdhocActionType == 6 ? "Extended Request" : ""),
                               Comments = rqst.Comments,
                               RequestStatus = acssrqst.RequestStatus,
                               AccessRequestID = acssrqst.Id,
                               RequesteeName = rusr.DisplayName,
                               RequesteeUserName = rusr.UserName,
                               RequesterName = rqust.DisplayName,
                               AccountType = rqst.AccountType,
                               ValidFrom = rqst.ValidFrom,
                               ValidTo = rqst.ValidTo,
                               AdhocActionType = rqst.AdhocActionType,
                               ProgressStatus = acssrqst.ProgressStatus,
                               ParentrequestId = acssrqst.ParentRequestId,
                               RequesteeEmployeeId = rusr.EmployeeId,
                               RequesteeDepartment = rusr.NOrganisation.Name
                           }).ToList();

            foreach (PresentationMyRequestData _rqst in requestList)
            {
                _rqst.Status = UtilityMethods.GetEnumAttribute<DisplayAs>(_rqst.RequestStatus);
            }

            var aa = requestList.Skip(pageNumber.Value * pageSize.Value).Take(pageSize.Value).ToList();

            PagedResults<PresentationMyRequestData> paged = new PagedResults<PresentationMyRequestData>()
            {
                Results = aa,
                TotalNumberOfRecords = requestList.Count,
                PageNumber = pageNumber.Value,
                PageSize = pageSize.Value
            };


            return paged;
        }

        public bool GetFlinsAttestationMessage(long id)
        {
            var retMessage = string.Empty;
            var query = from atts in CurrentContext.Attestations
                        join attenend in CurrentContext.AttestationEndpoints on atts.Id equals attenend.AttestationId
                        join endPoint in CurrentContext.EndPoint on attenend.EndpointSystemId equals endPoint.Id
                        where endPoint.ApplicationType == ApplicationType.HRMS_FLINS && atts.Id == id
                        select atts;

            return query.Count() > 0;
        }
        public Dictionary<int, string> GetSAPApplicationTypeFromAttestation(long id)
        {
            Dictionary<int, string> finalResult = new Dictionary<int, string>();
            var retMessage = string.Empty;
            var query = (from camp in CurrentContext.AttestationCampaigns
                         join atts in CurrentContext.AttestationSnapshots on camp.Id equals atts.AttestationCampaignId
                         where camp.Id == id
                         select atts.EndpointId).Distinct();
            var eps = CurrentContext.EndPoint.Where(i=> i.IsTrustedSource == false).Select(i => new { i.Id, i.ApplicationType, i.DisplayName, i.EndpointSystemTypeId }).ToList();
            var data = (from endPoint in CurrentContext.EndPoint
                        where ((endPoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP && query.Contains(endPoint.Id)) ||
                        (endPoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAPHANA && query.Contains(endPoint.Id))
                        || (query.Contains(endPoint.Id) && (endPoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.EAM_NOVA || endPoint.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SRM))) && endPoint.IsTrustedSource == false
                        select new { endPoint.DisplayName, endPoint.ApplicationType }).Distinct().ToDictionary(key=> key.ApplicationType, value=> value.DisplayName);

            foreach (var item in data)
            {
                if(item.Key == ApplicationType.SRM)
                {
                    var dependentSystems = eps.Where(i=> i.ApplicationType == ApplicationType.SAP_SRP || i.ApplicationType == ApplicationType.SAP_SUS || i.ApplicationType == ApplicationType.FLINS_BI);
                    foreach (var dependentSystem in dependentSystems)
                    {
                        finalResult.Add((int)dependentSystem.ApplicationType, dependentSystem.DisplayName);
                    }
                }
                else if(item.Key == ApplicationType.EAM)
                {
                    var dependentSystems = eps.Where(i => i.ApplicationType == ApplicationType.HRMS_FLINS || i.ApplicationType == ApplicationType.FLINS_BI);
                    foreach (var dependentSystem in dependentSystems)
                    {
                        finalResult.Add((int)dependentSystem.ApplicationType, dependentSystem.DisplayName);
                    }
                }
                else
                {
                    finalResult.Add((int)item.Key,item.Value);
                }
            }
            return finalResult;
        }
        public bool DoesExistsOtherSystemEndpoint(long taskId)
        {
            var retMessage = string.Empty;
            var query = from tsk in CurrentContext.Tasks
                        join snpAction in CurrentContext.AttestationSnapshotActions on tsk.Id equals snpAction.TaskId
                        join snpSt in CurrentContext.AttestationSnapshots on snpAction.AttestationSnapshotId equals snpSt.Id
                        join endPoint in CurrentContext.EndPoint on snpSt.EndpointId equals endPoint.Id
                        where endPoint.ApplicationType == ApplicationType.InhouseSystems && tsk.Id == taskId
                        select tsk.Id;

            return query.Any();
        }
    }
}