using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace DI.Chakra.ProvisioningUtility
{
    public class UndoProvisioningEntitlements
    {
        public long Requestor { get; set; }

        public long Requestee { get; set; }

        readonly JavaScriptSerializer _serializer;
        readonly string MasterConfigHelperServiceUser;
        public UndoProvisioningEntitlements(long userId, long requestor)
        {
            this.Requestee = userId;
            this.Requestor = requestor;
            _serializer = new JavaScriptSerializer();
            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;
        }

        public void Execute()
        {
            List<OUChangeSnapshot> ouSnapShotList = OUChangeSnapshot.GetByUserId(this.Requestee);
            OrganizeSnapShotList(ouSnapShotList);
        }

        void OrganizeSnapShotList(List<OUChangeSnapshot> OUChangeSnapshot)
        {
            try
            {
                if (OUChangeSnapshot.Count == 0)
                    return;

                List<OUChangeSnapshot> SnapsotListbyEndpoint = new List<OUChangeSnapshot>();
                List<OUChangeSnapshot> SnapsotListbyEndpointUser = new List<OUChangeSnapshot>();
                List<long> endpointIds = new List<long>();
                List<long> endpointUserIds = new List<long>();
                List<AccessRequest> accessRequestList = new List<AccessRequest>();
                Guid transactionId = Guid.NewGuid();

                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.DebugStep, $"OrganizeSnapShotList : Number of SnapShots taken :{OUChangeSnapshot.Count()}");

                var idmsEp = EndPoint.GetByApplicationType(ApplicationType.IDMS);
                var idmsSnap = OUChangeSnapshot.FirstOrDefault(i => i.EndpointSystemId == idmsEp.Id);
                DateTime? deactivationDate = null;
                if (idmsSnap != null && !string.IsNullOrEmpty(idmsSnap.Entitlement))
                {
                    var idmsUserDetail = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(idmsSnap.Entitlement);
                    if(idmsUserDetail?.UserInfo != null && !string.IsNullOrWhiteSpace(idmsUserDetail?.UserInfo?.Details))
                    {
                        deactivationDate = Convert.ToDateTime(idmsUserDetail.UserInfo.Details);
                    }
                }

                foreach (var ousnap in OUChangeSnapshot)
                {
                    long endpointsystemid = ousnap.EndpointSystemId;
                    if (!endpointIds.Contains(endpointsystemid))
                    {
                        endpointIds.Add(endpointsystemid);
                        SnapsotListbyEndpoint.AddRange(OUChangeSnapshot.Where(item => item.EndpointSystemId == endpointsystemid).ToList());
                        foreach (var snapshotbyendpoint in SnapsotListbyEndpoint)
                        {
                            long endpointuserid = snapshotbyendpoint.EndpointUserId;
                            if (!endpointUserIds.Contains(endpointuserid))
                            {
                                endpointUserIds.Add(snapshotbyendpoint.EndpointUserId);
                                SnapsotListbyEndpointUser.AddRange(SnapsotListbyEndpoint.Where(item => item.EndpointUserId == endpointuserid).ToList());
                                var accessreq = CreateAccountAccessRequest(SnapsotListbyEndpointUser, transactionId);
                                //We should not be putting the old termination date as de-activation date for undo event. It should be new termination date
                                //accessreq.DeactivationDate = deactivationDate;
                                accessRequestList.Add(accessreq);
                                SnapsotListbyEndpointUser.Clear();
                            }
                        }
                        SnapsotListbyEndpoint.Clear();
                        endpointUserIds.Clear();
                    }
                }
                AccessRequest.SaveAccessRequestList(accessRequestList);

                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.DebugStep, $"OrganizeSnapShotList: Access Request created - {accessRequestList.Count}");
                if (accessRequestList.Exists(i => i.ApplicationType == ApplicationType.Office365))
                {
                    var ad = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.ADSingaporePowerLocal);
                    if(ad != null && ad.Id > 0)
                    {
                        var o365 = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.Office365);
                        if(o365 != null && o365.Id > 0)
                        {
                            o365 = AccessRequest.GetById(o365.Id);
                            o365.IsParentRequest = true;
                            o365.ParentRequestId = ad.Id;
                            o365.Save(MasterConfigHelperServiceUser);
                        }
                    }
                }
                if (accessRequestList.Exists(i => i.ApplicationType == ApplicationType.MicrosoftTeams))
                {
                    var office365 = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.Office365);
                    if (office365 != null)
                    {
                        var sfb = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.MicrosoftTeams);
                        if (sfb != null && sfb.Id > 0)
                        {
                            sfb = AccessRequest.GetById(sfb.Id);
                            sfb.IsParentRequest = true;
                            sfb.ParentRequestId = office365.Id;
                            sfb.ExecutionTime = DateTime.Now.AddHours(MasterConfigHelper.O365ADSyncWaitTime);
                            sfb.Save(MasterConfigHelperServiceUser);
                        }
                    }
                }
                if (accessRequestList.Exists(i => (i.ApplicationType == ApplicationType.SAP_EP || i.ApplicationType == ApplicationType.SAP_EP) && i.AccountType == AccountType.User))
                {
                    var hrmsEP = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
                    var hrms = accessRequestList.FirstOrDefault(i=>i.EndpointSystemId == hrmsEP.Id);
                    if(hrms != null)
                    {
                        
                        var eps = new []{ ApplicationType.CE , ApplicationType.SAP_EP };
                        foreach (var ep in eps)
                        {
                            var sapEP = EndPoint.GetByApplicationType(ep);
                            var accReqEP = accessRequestList.FirstOrDefault(i => i.EndpointSystemId == sapEP.Id);
                            if (accReqEP != null)
                            {
                                var savedAccessRequest = AccessRequest.GetById(accReqEP.Id);
                                savedAccessRequest.IsParentRequest = true;
                                savedAccessRequest.ParentRequestId = hrms.Id;
                                savedAccessRequest.ExecutionTime = DateTime.Now.AddMinutes(10);
                                savedAccessRequest.Save(MasterConfigHelperServiceUser);
                            }
                        }
                    }
                }
                if (accessRequestList.Exists(i => i.ApplicationType == ApplicationType.EAM && i.AccountType == AccountType.User))
                {
                    var adRequest = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.ADSingaporePowerLocal && i.AccountType == AccountType.User);
                    long parentId = 0;
                    if (adRequest != null)
                    {
                        parentId = adRequest.Id;
                    }
                    var temp = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.HRMS_FLINS && i.AccountType == AccountType.User);
                    if(temp != null && parentId>0)
                    {
                        var flinsRequest = AccessRequest.GetById(temp.Id);
                        if (flinsRequest != null && flinsRequest.Id > 0)
                        {
                            flinsRequest.IsParentRequest = true;
                            flinsRequest.ParentRequestId = parentId;
                            flinsRequest.Save(MasterConfigHelperServiceUser);
                            parentId = flinsRequest.Id;
                        }
                    }

                    var tempFlins = accessRequestList.FirstOrDefault(i => i.ApplicationType == ApplicationType.FLINS_BI && i.AccountType == AccountType.User);
                    if (tempFlins != null && tempFlins.Id > 0)
                    {
                        var flinsBiRequest = AccessRequest.GetById(tempFlins.Id);
                        if (flinsBiRequest != null && flinsBiRequest.Id > 0 && parentId > 0)
                        {
                            flinsBiRequest.IsParentRequest = true;
                            flinsBiRequest.ParentRequestId = parentId;
                            flinsBiRequest.Save(MasterConfigHelperServiceUser);
                            parentId = flinsBiRequest.Id;
                        }
                    }
                    accessRequestList.ForEach(i =>
                    {
                        var accessReq = AccessRequest.GetById(i.Id);
                        if (parentId > 0 && accessReq != null && accessReq.ApplicationType == ApplicationType.EAM && accessReq.AccountType == AccountType.User)
                        {
                            accessReq.IsParentRequest = true;
                            accessReq.ParentRequestId = parentId;
                            accessReq.Save(MasterConfigHelperServiceUser);
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList", ex);
                throw ex;
            }

        }

        AccessRequest CreateAccountAccessRequest(List<OUChangeSnapshot> SnapshotByEndpointUser, Guid transactionId)
        {
            try
            {
                string TechnicalRole = string.Empty;
                string TechnicalGroup = string.Empty;
                string TechnicalPosition = string.Empty;

                foreach (var snapshotbyEndpointUser in SnapshotByEndpointUser)
                {
                    if (snapshotbyEndpointUser.EntitlementType == EntitlementType.Role)
                    {
                        TechnicalRole = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.Group)
                    {
                        TechnicalGroup = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.Position)
                    {
                        TechnicalPosition = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.AccessLevel)
                    {
                        TechnicalGroup = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.Level)
                    {
                        TechnicalGroup = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.ResponseGroup)
                    {
                        TechnicalGroup = snapshotbyEndpointUser.Entitlement;
                    }
                    else if (snapshotbyEndpointUser.EntitlementType == EntitlementType.Application)
                    {
                        TechnicalGroup = snapshotbyEndpointUser.Entitlement;
                    }
                    if (snapshotbyEndpointUser.EntitlementType == EntitlementType.HandPhone)
                    {
                        TechnicalRole = snapshotbyEndpointUser.Entitlement;
                    }
                }
                var accessRequest = PopulateAccessRequest(SnapshotByEndpointUser[0], TechnicalRole, TechnicalGroup, TechnicalPosition, transactionId);
                return accessRequest;
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest for the OUSnapShot EndpointUserId - {SnapshotByEndpointUser[0].EndpointUserId} and transaction ID - {transactionId}", ex);

                throw ex;
            }
         
        }

        AccessRequest PopulateAccessRequest(OUChangeSnapshot snapShot, string TechnicalRole, string TechnicalGroup, string TechnicalPosition, Guid transactionId)
        {
            try
            {
                var executionTime = DateTime.Now.AddMinutes(2);
                User Requestor = User.GetById(this.Requestor);
                User Requestee = User.GetById(snapShot.UserId);
                EndPoint endpoint = EndPoint.GetById(snapShot.EndpointSystemId);
                AccessRequest accessrequest = new AccessRequest();
                accessrequest.CreatedBy = Requestor.UserName;
                accessrequest.CreatedDate = DateTime.Now;
                accessrequest.LastModifiedBy = Requestor.UserName;
                accessrequest.LastModifiedDate = executionTime;
                if (EndpointType.InhouseSystems == (EndpointType)endpoint.EndpointSystemTypeId)
                {
                    accessrequest.UserId = snapShot.UserId;
                    accessrequest.RequesterId = this.Requestor;
                    accessrequest.RequestSource = AccessRequestSource.UndoEvent;
                    accessrequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    accessrequest.RequestStatus = AccessRequestStatus.Granted;
                    var adhocReq = PopulateAdhocRequest(snapShot, AdHocActionType.NewAccount);
                    accessrequest.Description = string.Format("This Request is Created for Undo the {0}", snapShot.EmployeeeEventType.ToString());
                    accessrequest.AdHocAccessRequest = adhocReq;
                    accessrequest.ExecutionTime = executionTime;
                    accessrequest.EndpointSystemId = snapShot.EndpointSystemId;
                }
                else if (EndpointType.GEMS == (EndpointType)endpoint.EndpointSystemTypeId || EndpointType.EAM_NOVA == (EndpointType)endpoint.EndpointSystemTypeId)
                {
                    if (snapShot.EmployeeeEventType == EmployeeEvents.Resignation || snapShot.EmployeeeEventType == EmployeeEvents.OUChange)
                    {
                        accessrequest.EndpointSystemId = snapShot.EndpointSystemId;
                        accessrequest.UserId = snapShot.UserId;
                        accessrequest.RequesterId = this.Requestor;
                        accessrequest.RequestSource = AccessRequestSource.UndoEvent;
                        accessrequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                        accessrequest.RequestStatus = AccessRequestStatus.Granted;
                        accessrequest.ExecutionTime = executionTime;
                        accessrequest.Description = string.Format("This Request is Created for Undo the {0}", snapShot.EmployeeeEventType.ToString());
                        var adhocReq = PopulateAdhocRequest(snapShot, AdHocActionType.NewAccount);
                        accessrequest.AdHocAccessRequest = adhocReq;
                    }
                }
                else
                {
                    accessrequest.EndpointSystemId = snapShot.EndpointSystemId;
                    accessrequest.RequestSource = AccessRequestSource.UndoEvent;
                    EventUndoRequest undoReq = PopulateEventUndoRequest(snapShot, TechnicalRole, TechnicalGroup, TechnicalPosition);
                    accessrequest.Description = string.Format("This Request is Created for Undo the {0}", snapShot.EmployeeeEventType.ToString());
                    accessrequest.EventUndoRequest = undoReq;
                    accessrequest.UserId = snapShot.UserId;
                    accessrequest.RequesterId = this.Requestor;
                    accessrequest.ProgressStatus = AccessRequestProgress.ReadytoExecute;
                    accessrequest.RequestStatus = AccessRequestStatus.Granted;
                    accessrequest.ExecutionTime = executionTime;
                }

                accessrequest.OrgCompany = Requestee?.OrgCompany;
                accessrequest.OrgDepartment = Requestee?.OrgDepartment;
                accessrequest.OrgDivision = Requestee?.OrgDivision;
                accessrequest.OrgBranch = Requestee?.OrgBranch;
                accessrequest.OrgSection = Requestee?.OrgSection;
                accessrequest.AccountType = snapShot.AccountType;
                accessrequest.ApplicationType = endpoint.ApplicationType;
                if(accessrequest.ApplicationType == ApplicationType.MicrosoftTeams)
                {
                    accessrequest.TransactionId = Guid.NewGuid();
                }
                accessrequest.DeactivationDate = Requestee.TerminationDate;
                return accessrequest;
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest.PopulateAccessRequest for the OUSnapShot EndpointUserId - {snapShot.EndpointUserId} and transaction ID - {transactionId}", ex);

                throw ex;
            }
        }

        AdHocAccessRequest PopulateAdhocRequest(OUChangeSnapshot snapShot, AdHocActionType adhocActionType)
        {
            try
            {
                EndPoint endpoint = EndPoint.GetById(snapShot.EndpointSystemId);
                AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();

                adhocaccessrequest.AccountType = AccountType.User;
                adhocaccessrequest.AdhocActionType = adhocActionType;
                adhocaccessrequest.Comments = string.Format("This Request is Created for Undo the {0}", snapShot.EmployeeeEventType.ToString());
                var user = User.GetById(this.Requestor);
                adhocaccessrequest.CreatedBy = user != null ? user.UserName : string.Empty;
                adhocaccessrequest.CreatedDate = DateTime.Now; ;
                adhocaccessrequest.EndpointSystemId = endpoint.Id;
                adhocaccessrequest.EndpointSystemType = (EndpointType)endpoint.EndpointSystemTypeId;
                adhocaccessrequest.ApplicationType = endpoint.ApplicationType;
                if (adhocActionType == AdHocActionType.NewAccount)
                {
                    //set Entitlements from SnapShot
                    IEntitlementDetail entitlementDetail = PopulateAdhocEntitlementObject(endpoint, snapShot.Entitlement, snapShot.EntitlementType, snapShot.EndpointUserId, snapShot.AccountType, snapShot.AccountName);
                    entitlementDetail.DisplayAccountName = snapShot.AccountName;
                    adhocaccessrequest.EntitlementDetail = adhocaccessrequest.GetSerializedString(entitlementDetail);
                }
                adhocaccessrequest.IsSelfRequest = true;
                adhocaccessrequest.RequesteeUserId = snapShot.UserId;
                adhocaccessrequest.RequesterUserId = this.Requestor;
                adhocaccessrequest.ValidFrom = DateTime.Now;
                adhocaccessrequest.ValidTo = null;
                return adhocaccessrequest;
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest.PopulateAccessRequest.PopulateAdhocRequest for the OUSnapShot EndpointUserId - {snapShot.EndpointUserId}.", ex);

                throw ex;
            }
        }

        IEntitlementDetail PopulateAdhocEntitlementObject(EndPoint endpoint, string entitlement, EntitlementType entitlementtype, long endpointuserid, AccountType accountType, string AccountName)
        {
            try
            {
                IEntitlementDetail entitlementDetail = null;

                if (endpoint.EndpointSystemTypeId == (int)EndpointType.GEMS)
                {
                    entitlementDetail = new AdHocGEMSRequest()
                    {
                        EndpointUserId = endpointuserid,
                        Entitlement = entitlement,
                        EntitlementType = entitlementtype,
                    };
                }
                else if (endpoint.EndpointSystemTypeId == (long)EndpointType.InhouseSystems)
                {
                    var adhocEntitlementDetail = new AdHocINHRequest()
                    {
                        EndpointUserId = endpointuserid,
                        ApplicationCode = endpoint.ApplicationCode,
                        Entitlement = entitlement,
                    };
                    if(accountType != AccountType.User)
                    {
                        adhocEntitlementDetail.AdHocINHSharedAccount = new AdHocINHSharedAccount();
                        adhocEntitlementDetail.AdHocINHSharedAccount.AccountName = AccountName;
                        adhocEntitlementDetail.AdHocINHSharedAccount.DisplayName = AccountName;
                        adhocEntitlementDetail.AdHocINHSharedAccount.Description = AccountName;
                    }
                    entitlementDetail = adhocEntitlementDetail;
                }
                else if (endpoint.EndpointSystemTypeId == (long)EndpointType.EAM_NOVA)
                {
                    try
                    {
                        var detail = new AdHocNovaRequest();
                        detail.EndpointUserId = endpointuserid;
                        var snapShot_EAM = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(entitlement);
                        if (snapShot_EAM == null) return detail;
                        var accountName = snapShot_EAM.UserInfo;
                        detail.RequestDetails = new List<AdHocNovaRequestDetails>();
                        if (snapShot_EAM.Entitlements != null && snapShot_EAM.Entitlements.Count > 0)
                            snapShot_EAM.Entitlements.ForEach(item =>
                            {
                                if (!string.IsNullOrWhiteSpace(item.Entitlement))
                                {
                                    var role = JsonConvert.DeserializeObject<SAPEamNovaUserRoleInfo>(item.Entitlement);
                                    detail.RequestDetails.Add(new AdHocNovaRequestDetails()
                                    {
                                        CompanyCode = role.Company,
                                        SectionCode = role.Section,
                                        DepartmentCode = role.DepartmentBranch,
                                        NovaMatrixType = role.NovaPositionMatrixType,
                                        RoleName = role.RoleName
                                    });
                                }

                            });

                        entitlementDetail = detail;
                    }
                    catch (Exception ee)
                    {
                        DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest.PopulateAccessRequest.PopulateAdhocRequest.PopulateAdhocEntitlementObject for EAM NOVA", ee);

                        throw ee;
                    }
                }

                return entitlementDetail;

            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest.PopulateAccessRequest.PopulateAdhocRequest.PopulateAdhocEntitlementObject for the OUSnapShot EndpointUserId - {endpointuserid}.", ex);

                throw ex;
            }
        }

        EventUndoRequest PopulateEventUndoRequest(OUChangeSnapshot snapShot, string TechnicalRoles, string TechnicalGroups, string TechnicalPositions)
        {
            try
            {
                EventUndoRequest undoReq = new EventUndoRequest();
                undoReq.EmployeeEventType = snapShot.EmployeeeEventType;
                undoReq.EndpointSystemId = snapShot.EndpointSystemId;
                undoReq.EndpointUserId = snapShot.EndpointUserId;

                undoReq.AdditionalInfo = snapShot.AdditionalInfo;

                var endpoint = EndPoint.GetById(snapShot.EndpointSystemId);
                undoReq.EndpointType = (EndpointType)endpoint.EndpointSystemTypeId;

                undoReq.TechnicalRoles = TechnicalRoles;
                undoReq.TechnicalGroups = TechnicalGroups;
                undoReq.TechnicalPositions = TechnicalPositions;

                undoReq.CreatedBy = "System";
                undoReq.CreatedDate = DateTime.Now;
                return undoReq;
            }
            catch (Exception ex)
            {
                DI.Chakra.Logger.DILog.ProvisioningLogger.WriteLog(Logger.LogLevelType.Exception, $"Exception Occured in OrganizeSnapShotList.CreateAccountAccessRequest.PopulateAccessRequest.PopulateEventUndoRequest for the OUSnapShot EndpointUserId - {snapShot.EndpointUserId}.", ex);

                throw ex;
            }
        }

    }
}