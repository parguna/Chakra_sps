using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.DomainModel;
using System.Text;
using DI.Chakra.Services.EmailingProcessor; 
using DI.Chakra.DomainModel.Presentation;
using System.Threading;

namespace DI.Chakra.Services.AttestationProcessor
{
    public class AttestationService
    {

        private int m_historyId = -1;
        public List<EndPoint> defaultApplications = new List<EndPoint>();
        public List<ApplicationType> dependentApplications = new List<ApplicationType>();
        private string serviceUser = string.Empty;
        private const string errorMessage = "There is no matching Endpoint Users for Attestation";
        public AttestationService() {
            defaultApplications = EndPoint.GetDefaultApplication();
            List<ApplicationType> applnTypes = new List<ApplicationType>();
            applnTypes.Add(ApplicationType.SAP_EP);
            applnTypes.Add(ApplicationType.CE);
            applnTypes.Add(ApplicationType.IDMS);

            var eps = DomainModel.EndPoint.GetByApplicationTypes(applnTypes);
            foreach (var ep in eps)
            {
                if(ep != null && ep.IsActive)
                {
                    //Below list are added because those system are handled manually
                    dependentApplications.Add(ep.ApplicationType);
                }
            }
            serviceUser = MasterConfigHelper.ServiceUser;
        }

        public List<User> usersOrg = new List<User>();
        public IList<PresentationUserEntitlement> FindEntitlments(Attestation _attestation)
        {
            List<PresentationUserEntitlement> entitlements = new List<PresentationUserEntitlement>();
            var attestationEndpoints = AttestationEndpoint.GetByAttestationId(_attestation.Id).ToList();
            foreach (var attestationEndpoint in attestationEndpoints)
            {
                var endpoint = EndPoint.GetById(attestationEndpoint.EndpointSystemId, true);
                var entitlementTypes = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetEntitlementList().Distinct().ToList();
                foreach (var entitlementType in entitlementTypes)
                {
                    entitlements.AddRange(EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetAllEntitlementByType(entitlementType, endpoint.Id).ToList());
                }
            }
            return entitlements;
        }

        public IList<User> FindUsers(Attestation _attestation)
        {
            List<User> users = new List<User>();
            try
            {
                List<PresentationAttestationScope> listAttScope = new List<PresentationAttestationScope>();
                listAttScope = PresentationAttestationScope.GetAttestationScopeByAttestationId(_attestation.Id);
                if (listAttScope == null || listAttScope.Count == 0) return users;
                foreach (PresentationAttestationScope attScope in listAttScope)
                {
                    List<NOrganisation> nOrgList = new List<NOrganisation>();
                    NOrganisation nOrg = new NOrganisation();
                    usersOrg.Clear();

                    if ((attScope.AttestationScopeName != AttestationScopeEnum.IACMGroup.ToString()) &&
                    (attScope.AttestationScopeName != AttestationScopeEnum.User.ToString()) &&
                    (attScope.AttestationScopeName != AttestationScopeEnum.RO.ToString()))
                    {
                        if ((attScope.OrganisationScopeId == 0) || (attScope.OrganisationScopeId == null))
                        {
                            List<NOrganisation> listNOrg = NOrganisation.GetByScopeId(attScope.AttestationScopeValue);
                            if (listNOrg.Count > 0)
                            {
                                FindUsersByNOrganisations(listNOrg);
                                users.AddRange(usersOrg.Except(users));
                            }
                        }
                        else
                        {
                            nOrg = NOrganisation.GetById(attScope.OrganisationScopeId);
                            if (nOrg != null)
                            {
                                FindNOrganisationUsers(nOrg, attScope);
                                users.AddRange(usersOrg.Except(users));
                            }
                        }
                    }
                    else if (attScope.AttestationScopeName == AttestationScopeEnum.IACMGroup.ToString())
                    {
                        if ((attScope.AttestationScopeValue == 0) || (attScope.AttestationScopeValue == null))
                        {
                            List<Group> listgroup = Group.GetByIsActive();
                            foreach (Group grp in listgroup)
                            {
                                List<User> usersGroup = Group.GetByIdIncludingUsers(grp.Id).Users;
                                users = users.Concat(usersGroup).ToList();
                            }
                        }
                        else
                        {
                            List<User> usersGroup = Group.GetByIdIncludingUsers(attScope.AttestationScopeValue).Users;
                            users = users.Concat(usersGroup).ToList();
                        }
                    }
                    else if (attScope.AttestationScopeName == AttestationScopeEnum.User.ToString())
                    {
                        if ((attScope.AttestationScopeValue == 0) || (attScope.AttestationScopeValue == null))
                        {
                            List<User> scopeUsers = User.GetActiveEmployees();
                            users = users.Concat(scopeUsers).ToList();
                        }
                        else
                        {
                            User usr = User.GetById(attScope.AttestationScopeValue);
                            users.Add(usr);
                        }
                    }
                    else if (attScope.AttestationScopeName == AttestationScopeEnum.RO.ToString())
                    {
                        if (((attScope.OrganisationScopeId == 0) || (attScope.OrganisationScopeId == null)) && (attScope.AttestationScopeValue == 0))
                        {
                            var usr = User.GetAllUserUnderReportingOfficer(null);
                            if(usr != null && usr.Count > 0)
                                users.AddRange(usr.Except(users));
                        }
                        else
                        {
                            var usr = User.GetAllUserUnderReportingOfficer(attScope.AttestationScopeValue);
                            if (usr != null && usr.Count > 0)
                                users.AddRange(usr.Except(users));
                        }
                    }
                }
                if(_attestation.AttestationEndpoint != null && _attestation.AttestationEndpoint.Count == 1 && _attestation.AttestationEndpoint.Any(i=> i.EndPoint.ApplicationType == ApplicationType.IDMS))
                {
                    var includeList = User.GetIACMUserForAttestation();
                    if(includeList != null && includeList.Count > 1)
                    {
                        users = users.Where(i=> includeList.Contains(i.Id)).ToList();
                    }
                    includeList = null;
                }
                else if (_attestation.AttestationEndpoint != null && _attestation.AttestationEndpoint.Count == 1 && _attestation.AttestationEndpoint.Any(i => i.EndPoint.ApplicationType == ApplicationType.OneCard))
                {
                    var endPointId = _attestation.AttestationEndpoint.FirstOrDefault().EndpointSystemId;
                    var includeList = User.GetOneCardUserForAttestation(endPointId);
                    if (includeList != null && includeList.Count > 1)
                    {
                        users = users.Where(i => includeList.Contains(i.Id)).ToList();
                    }
                    includeList = null;
                }

                return users.Where(usr => usr.IsActive == true && !string.IsNullOrEmpty(usr.UserName)).Distinct().ToList();
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(_attestation.Id, LogLevelType.Exception, ex.Message, ex);
                throw;
            }
        }

        public void FindUsersByNOrganisations(List<NOrganisation> nOrganisationInputList)
        {
            List<User> usersListOrgLevel = new List<User>();
            List<NOrganisation> nOrganisationResultList = new List<NOrganisation>();

            foreach (NOrganisation org in nOrganisationInputList)
            {
                if (org.ScopeId.HasValue)
                {
                    var scope = NOrganisationScope.GetById(org.ScopeId.Value);

                    if (scope.Scope.ToLower() == "00-company".ToLower())
                    {
                        usersOrg.AddRange(User.GetByCompanyCode(org.Code).Except(usersOrg));
                    }
                    else if (scope.Scope.ToLower() == "organisationunit".ToLower())
                    {
                        usersOrg.AddRange(User.GetByDepartmentCode(org.Code).Except(usersOrg));
                    }
                    else if (scope.Scope.ToLower() == "10-Dept".ToLower())
                    {
                        usersOrg.AddRange(User.GetByDepartmentCode(org.Code).Except(usersOrg));
                    }
                    else if (scope.Scope.ToLower() == "30-Branch".ToLower())
                    {
                        usersOrg.AddRange(User.GetByBranchCode(org.Code).Except(usersOrg));
                    }
                    else if (scope.Scope.ToLower() == "40-Section".ToLower())
                    {
                        usersOrg.AddRange(User.GetBySectionCode(org.Code).Except(usersOrg));
                    }
                    else if (scope.Scope.ToLower() == "50-Unit".ToLower())
                    {
                        usersOrg.AddRange(User.GetByUnitCode(org.Code).Except(usersOrg));
                    }
                }
            }

            if (nOrganisationResultList.Count > 0)
                FindUsersByNOrganisations(nOrganisationResultList);

        }
        public void FindNOrganisationUsers(NOrganisation NOrganisation, PresentationAttestationScope attScope)
        {
            if (NOrganisation != null)
            {
                if (attScope.AttestationScopeName.ToLower() == "00-Company".ToLower())
                {
                    usersOrg = User.GetByCompanyCode(NOrganisation.Code);
                }
                else if (attScope.AttestationScopeName.ToLower() == "organisationunit")
                {
                    usersOrg = User.GetByDepartmentCode(NOrganisation.Code);
                }
                else if (attScope.AttestationScopeName.ToLower() == "10-Dept".ToLower())
                {
                    usersOrg = User.GetByDepartmentCode(NOrganisation.Code);
                }
                else if (attScope.AttestationScopeName.ToLower() == "30-branch".ToLower())
                {
                    usersOrg = User.GetByBranchCode(NOrganisation.Code);
                }
                else if (attScope.AttestationScopeName.ToLower() == "40-section".ToLower())
                {
                    usersOrg = User.GetBySectionCode(NOrganisation.Code);
                }
                else if (attScope.AttestationScopeName.ToLower() == "50-unit".ToLower())
                {
                    usersOrg = User.GetByUnitCode(NOrganisation.Code);
                }
                
            }
            usersOrg = usersOrg.Where(usr => usr.UserName != string.Empty).ToList();
        }

        public long CreateAttestationCampaign(Attestation attestation, bool isParentCampaign, long campaignId, string splitValue)
        {
            try
            {
                string serviceUser = MasterConfigHelper.ServiceUser;
                if (isParentCampaign)
                {
                    var time = DateTime.Now;
                    string suffix = time.ToString("dd-MM-yyyy");
                    //string suffix = String.Format("{0:0000}-{1:00}-{2:00}-{3:00}-{4:00}-{5:00}",
                    //    time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

                    if (!string.IsNullOrEmpty(attestation.CampaignNameFormat))
                    {
                        suffix = DateTime.Now.ToString(attestation.CampaignNameFormat);
                        suffix = suffix.Replace("Q?", string.Format("Q{0}", (int)Math.Ceiling(time.Month / 3.0)));
                        suffix = suffix.Replace("Z?", string.Format("H{0}", (int)Math.Ceiling(time.Month / 6.0)));

                        //if already a campaign exists with that name, create the last number
                    }
                    var tmpCampaignName = AttestationCampaign.GetAllWithCampaignName(String.Format("{0}_{1}", attestation.AttestationName, suffix));
                    if (tmpCampaignName != null && tmpCampaignName.Count > 0)
                    {
                        suffix = string.Format("{0}({1})", suffix, tmpCampaignName.Count);
                    }
                    long daysToComplete = WorkflowConfigurationDetail.GetDaysToComplete(attestation.WorkflowConfigurationId);
                    DateTime closeDate = time.AddDays(daysToComplete);
                    if (attestation != null)
                    {
                        AttestationCampaign campaign = new AttestationCampaign
                        {
                            // TODO: Cmapaign name to be more intelligent
                            CampaignName = String.Format("{0}_{1}", attestation.AttestationName, suffix),
                            Description = attestation.AttestationDescription,
                            Comments = String.Empty,
                            AttestationID = attestation.Id,
                            DefaultReviewStatus = attestation.DefaultReviewStatus,
                            WorkflowInstanceID = Guid.Empty,
                            RequiresAttestationRemediation = attestation.RequiresAttestationRemediation,
                            CloseDate = closeDate,
                            Status = AttestationCampaignStatus.Initiated,
                        };

                        campaign.Save(serviceUser);

                        return campaign.Id;
                    }
                    else
                    {
                        throw new Exception("Attestation instance is null");
                    }
                }
                else
                {
                    var parentCampaign = AttestationCampaign.GetById(campaignId);

                    AttestationCampaign campaign = new AttestationCampaign
                    {
                        // TODO: Cmapaign name to be more intelligent
                        CampaignName = string.Format("{0}{1}", parentCampaign.CampaignName, string.IsNullOrEmpty(splitValue) ? string.Empty : string.Format("_{0}", splitValue)),
                        Description = parentCampaign.Description,
                        Comments = parentCampaign.Comments,
                        AttestationID = parentCampaign.AttestationID,
                        DefaultReviewStatus = parentCampaign.DefaultReviewStatus,
                        WorkflowInstanceID = Guid.Empty,
                        RequiresAttestationRemediation = parentCampaign.RequiresAttestationRemediation,
                        CloseDate = parentCampaign.CloseDate,
                        ParentCampaign = parentCampaign.Id,
                        Status = parentCampaign.Status,
                        SplitUsing = splitValue,
                    };

                    campaign.Save(serviceUser);

                    var parenCampaign = AttestationCampaign.GetById(parentCampaign.Id);
                    var allChildCampaign = AttestationCampaign.GetChildCampaigns(parentCampaign.Id);
                    if (parenCampaign != null)
                    {
                        parenCampaign.ChildSplitCount = allChildCampaign.Count();
                        AttestationCampaign.UpdateCampaignChildCount(parenCampaign.Id, parenCampaign.ChildSplitCount, serviceUser);
                    }

                    return campaign.Id;
                }
            }
            catch (Exception ex)
            {
                string message = "Error while executing the CreateAttestationCampaignInstance.";

                DILog.AttestationLogger.WriteLog(null, LogLevelType.Exception, message, ex);
            }
            return 0;
        }

        private void ChangeAttestationCampaignStatus(long campaignId, AttestationCampaignStatus attestationCampaignStatus, string comments)
        {
            var attesttionCampaign = AttestationCampaign.GetById(campaignId);
            if (attesttionCampaign != null && attesttionCampaign.ParentCampaign.HasValue)
            {
                var parentCampaignId = attesttionCampaign.ParentCampaign;
                //attesttionCampaign.Status = attestationCampaignStatus;
                //attesttionCampaign.Comments = comments;
                //attesttionCampaign.Save(serviceUser);

                AttestationCampaign.UpdateCampaignStatusWithComments(campaignId, attestationCampaignStatus, comments);

                var parenCampaign = AttestationCampaign.GetById(parentCampaignId.Value);
                var allChildCampaign = AttestationCampaign.GetChildCampaigns(parentCampaignId.Value);
                if (parenCampaign != null)
                {
                    //parenCampaign.ChildSplitCount = allChildCampaign.Count();
                    //parenCampaign.Save(serviceUser);
                    AttestationCampaign.UpdateCampaignChildCount(parentCampaignId.Value, allChildCampaign.Count(), serviceUser);
                }
                parenCampaign = null;
                attesttionCampaign = null;
                allChildCampaign?.Clear();
                allChildCampaign = null;
            }
        }

        private void UpdateSplitChildCount(long parentCampaignId)
        {
            List<AttestationCampaign> allChildCampaign = null;
            try
            {
                allChildCampaign = AttestationCampaign.GetChildCampaigns(parentCampaignId);
                if (allChildCampaign != null)
                {
                    //parenCampaign.ChildSplitCount = allChildCampaign.Count();
                    //parenCampaign.Save(serviceUser);
                    AttestationCampaign.UpdateCampaignChildCount(parentCampaignId, allChildCampaign.Count(), serviceUser);
                }
                
            }
            catch (Exception)
            {
            }
            finally
            {
                allChildCampaign?.Clear();
                allChildCampaign = null;
            }
        }

        private void ChangeAttestationStatus(Process process, SchedulerStatus _status, AttestationProcess attestationProcess)
        {
            string serviceUser = MasterConfigHelper.ServiceUser;
            if (_status == SchedulerStatus.InProgress)
            {
                process.ProcessStartAt = DateTime.Now;
                process.Status = Enum.GetName(typeof(SchedulerStatus), _status);
                attestationProcess.process = process;
                attestationProcess.WorkflowStatus = Enum.GetName(typeof(SchedulerStatus), _status);
            }

            else if (_status == SchedulerStatus.Failed)
            {
                process.ProcessEndAt = DateTime.Now;
                process.Status = Enum.GetName(typeof(SchedulerStatus), _status);
                attestationProcess.WorkflowStatus = Enum.GetName(typeof(SchedulerStatus), _status);
                attestationProcess.process = process;
                attestationProcess.Save(serviceUser);
                DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Attestation Process {0} changed to {1}", process.ProcessTypeKeyId, process.Status));
                //new EmailComposer().Compose(EmailTemplate.AttestationJobFailed, attestationProcess.Id);
                SendEmail(attestationProcess);
                return;
            }
            else if (_status == SchedulerStatus.Pending)
            {

            }
            else
            {
                process.ProcessEndAt = DateTime.Now;
                process.LastRunOn = DateTime.Now;
                process.Status = Enum.GetName(typeof(SchedulerStatus), _status);
                attestationProcess.process = process;
                attestationProcess.WorkflowStatus = Enum.GetName(typeof(SchedulerStatus), _status);
            }

            attestationProcess.Save(serviceUser);
            DILog.SchedulerLogger.WriteLog(LogLevelType.Info, string.Format("Attestation Process {0} changed to {1}", process.ProcessTypeKeyId, process.Status));
        }

        public void StartWorkFlow(Process process)
        {
            AttestationProcess attestationprocess = new AttestationProcess();
            Attestation _attestation = null;
            try
            {
                attestationprocess = AttestationProcess.GetById(process.ProcessTypeKeyId, true);

                if (null == attestationprocess)
                    throw new EntityNotFoundException(string.Format("Attestation information NOT found for schedulerId - {0}", process.ProcessTypeKeyId.ToString()));

                if (attestationprocess.WorkflowStatus == SchedulerStatus.Completed.ToString())
                {
                    DILog.AttestationLogger.WriteLog(LogLevelType.Warning, string.Format("Attestation Id {0}  already  completed ", attestationprocess.AttestationId.ToString()));
                    throw new Exception(string.Format("Attestation Id {0}  already  completed ", attestationprocess.AttestationId.ToString()));
                }
                _attestation = Attestation.GetByIdIncludingAttestationProcesses(attestationprocess.AttestationId.Value);

                if (null == _attestation)
                    throw new Exception(string.Format("Cannot find Attestation for Id: {0}", attestationprocess.AttestationId));
                if (null == _attestation.AttestationProcesses || _attestation.AttestationProcesses.Count == 0)
                    throw new Exception(string.Format("Cannot find Attestation process for attestation Id: {0}", attestationprocess.AttestationId));
                if (null == _attestation.WorkflowConfiguration)
                    throw new Exception(string.Format("Cannot find workflow for attestation Id: {0}", attestationprocess.AttestationId));
                if (null == _attestation.AttestationEndpoint || _attestation.AttestationEndpoint.Count == 0)
                    throw new Exception(string.Format("Cannot find Attestation endpoint for attestation Id: {0}", attestationprocess.AttestationId));
                if(_attestation.AttestationEndpoint.All(i=>i.EndPoint.IsActive == false))
                {
                    throw new Exception(string.Format("Attestation endpoint for attestation Id: {0} is InActive.", attestationprocess.AttestationId));
                }    
                DoAttestation(_attestation, process);

            }
            catch (Exception ex)
            {
                process.ErrorMessage = ex.Message;
                DILog.AttestationLogger.WriteLog(attestationprocess.Id, LogLevelType.Exception, string.Format("Error in executing the attestation process {0}", process.ProcessTypeKeyId), ex);
                ChangeAttestationStatus(process, SchedulerStatus.Failed, attestationprocess);
                ProcessHistory history = GetHistory(process.Id, "Failed", ex.Message);
                history.Save(MasterConfigHelper.ServiceUser);
            }
        }

        #region Attestation Campaign Snapshot        
        List<User> CreateAttestationCampaignSnapshot(Attestation attestation, long campaignId, List<User> users, bool includeUnmappedAccount)
        {
            try
            {
                string serviceUser = MasterConfigHelper.ServiceUser;
                List<User> candidateUser = new List<User>();
                // get selected endpoints for this attestation campaign
                var attestationEndpoints = AttestationEndpoint.GetByAttestationId(attestation.Id).ToList();
                var attestationHierarchy = AttestationHierarchy.GetAttestationHierarchy(attestation.Id).ToList();
                DateTime dormantDate = DateTime.Today.Subtract(new TimeSpan((int)attestation.DormantAccountDays, 0, 0, 0));
                bool isAllowForActionable;
                Nullable<bool> isUnmappedAccount = null;
                foreach (var attestationEndpoint in attestationEndpoints)
                {
                    // get attestation hierarchy (endpoint levels to display) for this attestation
                    var attestationHierarchyForEndpoint = attestationHierarchy.Where(a => a.EndpointId == attestationEndpoint.EndpointSystemId).ToList();
                    // get the highest (lowest tree node) to consider for this attestation hierarchy. User = 1, Role =2, ...
                    var maxLevelToConsider = attestationHierarchyForEndpoint.GroupBy(a => a.IsVisible).Select(g => g.Max(row => row.EndpointHierarchy.Level)).First();
                    // get the endpoint hierarchy to consider based on the Max level
                    var attestationHierarchyLevelsToConsider = attestationHierarchyForEndpoint.Where(a => a.EndpointHierarchy.Level <= maxLevelToConsider).OrderBy(b => b.EndpointHierarchy.Level).ToList();
                    var endpoint = EndPoint.GetById(attestationEndpoint.EndpointSystemId, true);
                    if(endpoint == null || endpoint.IsActive == false)
                    {
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Endpoint {endpoint?.DisplayName} inactive or deleted.");
                        continue;
                    }
                    var entitlementType = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName).GetEntitlementList().Distinct().ToList();
                    
                    foreach (var attestationHierarchyToBuild in attestationHierarchyLevelsToConsider)
                    {
                        if (attestationHierarchyToBuild.IsVisible || loadInVisibleLevelAlso(attestationHierarchyToBuild, attestationHierarchyLevelsToConsider))
                        {
                            if (attestationHierarchyToBuild.EndpointHierarchy.EntitlementType == EntitlementType.Account)
                            {
                                var allRecons = AccountReconcilation.GetByEndpointSystemIdForAttestation(attestationEndpoint.EndpointSystemId, false, attestationEndpoint.EndPoint.ApplicationType);
                                #region Level One
                                //Level one always based on the User (Account Reconcilation)
                                foreach (var user in users)
                                {
                                    var accountRecons = allRecons.Where(i=> i.UserId == user.Id && i.EndpointSystemId == attestationEndpoint.EndpointSystemId).ToList();

                                    if (accountRecons.Count == 0)
                                    {
                                        // log it and continue to next iteration
                                        //DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Warning, string.Format("{0}-{1} removed from Attestation {2} since he/she does not have account in any endpoints.", user.DisplayName, user.UserName, attestation.AttestationName));
                                        continue;
                                    }
                                    

                                    foreach (var accountRecon in accountRecons)
                                    {
                                        if (string.IsNullOrWhiteSpace(accountRecon.AccountName))
                                        {
                                            DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Warning, string.Format("{0}-{1} removed from  Attestation {2} since Accountname is empty", user.DisplayName, user.UserName, attestation.AttestationName));
                                            continue;
                                        }
                                        isAllowForActionable = IsAllowAccountLevelActionable(endpoint, user, attestationHierarchyToBuild.IsActionAllowed, accountRecon);
                                        //if (attestation.IncludeDormantAccount)
                                        //{
                                        //    //var lastlogon = accountRecon.LastLogonDate;
                                        //    //if (!lastlogon.HasValue || DateTime.Compare(lastlogon.Value, dormantDate) > 0)
                                        //    //{
                                        //    //    // log it and continue to next iteration
                                        //    //    DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Warning, string.Format("{0}-{1} removed from Attestation {2} since he/she does not fall under dormant period.", user.DisplayName, user.UserName, attestation.AttestationName));
                                        //    //    continue;
                                        //    //}
                                        //}

                                        if (!candidateUser.Any(a => a.Id == user.Id))
                                            candidateUser.Add(user);
                                        createAttestationSnapshotForAccount(campaignId, accountRecon, attestationHierarchyToBuild, user, endpoint, isAllowForActionable);
                                    }
                                }

                                //include unmapped accounts
                                if (attestation.IncludeUnmappedAccount && includeUnmappedAccount)
                                {
                                    
                                    //Since all the provisioning required User object and unmapped account doesnt. Hence marking actionable as false
                                    isAllowForActionable = false;
                                    //var unmappedAccounts = AccountReconcilation.GetUnmappedAccountsByEndpointSystemId(endpoint.Id).ToList();
                                    var unmappedAccounts = AccountReconcilation.GetByEndpointSystemIdForAttestation(attestationEndpoint.EndpointSystemId, true, attestationEndpoint.EndPoint.ApplicationType);
                                    foreach (var unmappedAccount in unmappedAccounts)
                                    {
                                        if (string.IsNullOrWhiteSpace(unmappedAccount.AccountName))
                                        {
                                            DILog.AttestationLogger.WriteLog(campaignId, LogLevelType.Warning, string.Format("{0}-{1} removed from  Attestation {2} since unmapped account name is empty", unmappedAccount.FullName, unmappedAccount.AccountName, attestation.AttestationName));
                                            continue;
                                        }
                                        createAttestationSnapshotForAccount(campaignId, unmappedAccount, attestationHierarchyToBuild, null, endpoint, isAllowForActionable);
                                    }
                                    unmappedAccounts?.Clear();
                                    unmappedAccounts = null;
                                }
                                #endregion
                                allRecons = null;
                            }
                            else
                            {
                                
                                var parentEntitlements = AttestationSnapshot.GetAttestationSnapshotParentObject(campaignId, endpoint.Id, attestationHierarchyToBuild.EndpointHierarchy.Level - 1);
                                var entType = attestationHierarchyToBuild.EndpointHierarchy.EntitlementType;
                                if (entitlementType.Contains(attestationHierarchyToBuild.EndpointHierarchy.EntitlementType))
                                {
                                    #region Level Two
                                    //Level two always based on the User entitlement (Entitlement Reconcilation)
                                    if (parentEntitlements != null && parentEntitlements.Count > 0)
                                    {
                                        foreach (var parentEntitlement in parentEntitlements)
                                        {
                                            List<EntitlementReconcilation> entitlementRecons = new List<EntitlementReconcilation>();
                                            
                                            if (parentEntitlement.UserId <= 0)
                                            {
                                                entitlementRecons = EntitlementReconcilation.GetByReconcilationIdAndType(endpoint.Id, parentEntitlement.KeyId, endpoint.ApplicationType, entType).Where(a => a.IsDeleted == false && (EntitlementType)a.EntitlementType == attestationHierarchyToBuild.EndpointHierarchy.EntitlementType).ToList();
                                            }
                                            else
                                            {
                                                entitlementRecons = EntitlementReconcilation.GetByUserAndReconcilationId(parentEntitlement.UserId,endpoint.Id, parentEntitlement.KeyId, endpoint.ApplicationType, parentEntitlement.AccountType, entType).Where(a => a.IsDeleted == false && (EntitlementType)a.EntitlementType == entType).ToList();
                                            }
                                            
                                            foreach (var entitlementRecon in entitlementRecons)
                                            {
                                                AttestationSnapshot attestationSnapshot = new AttestationSnapshot()
                                                {
                                                    AttestationCampaignId = campaignId,
                                                    EntitlementType = (EntitlementType)entitlementRecon.EntitlementType,
                                                    Name = entitlementRecon.Entitlement,
                                                    Description = entitlementRecon.EntitlementDescription,
                                                    EndpointId = entitlementRecon.EndpointSystemId,
                                                    EndpointName = endpoint.DisplayName,
                                                    EndpointType = entitlementRecon.EndpointType,
                                                    AccountStatus = true,
                                                    UserId = entitlementRecon.UserId != null ? (long)entitlementRecon.UserId : 0,
                                                    AccountName = parentEntitlement.AccountName,
                                                    DIUserName = parentEntitlement.DIUserName,
                                                    DIUserDisplayName = parentEntitlement.DIUserDisplayName,
                                                    AccountType = parentEntitlement.AccountType,
                                                    KeyId = entitlementRecon.Id, // entitlement recon ID
                                                    Entity = entitlementRecon.Entitlement,
                                                    ParentEntity = parentEntitlement.AccountName,
                                                    Level = attestationHierarchyToBuild.EndpointHierarchy.Level,
                                                    HasRisk = entitlementRecon.IsSensitiveAccess || entitlementRecon.IsSODConflict,
                                                    RootSnapshotId = parentEntitlement.RootSnapshotId.HasValue ? parentEntitlement.RootSnapshotId.Value : parentEntitlement.Id,
                                                    //Commented as per kanchana request Ticket: 11863
                                                    //Reserved3 = entitlementRecon.ValidToDate?.ToString("dd/MM/yyyy")
                                                };
                                                if (endpoint.ApplicationType == ApplicationType.NPAC)
                                                {
                                                    //This is for showing NPAC Section, SectionName and Zone.
                                                    attestationSnapshot.Reserved1 = entitlementRecon.Functions;
                                                }
                                                if (endpoint.ApplicationType == ApplicationType.OneCard)
                                                {
                                                    //This is for showing OneCard Building and Section.
                                                    attestationSnapshot.Reserved1 = entitlementRecon.Functions;
                                                }else if (endpoint.ApplicationType == ApplicationType.MSSL_CRM || endpoint.ApplicationType == ApplicationType.MSSL_ISU ||
                                                    endpoint.ApplicationType == ApplicationType.EBS_CRM || endpoint.ApplicationType == ApplicationType.EBS_ISU)
                                                {
                                                    //FOR EBS MSSL need to handled the dependent position removal
                                                    attestationSnapshot.Reserved8 = entitlementRecon.Functions;
                                                }
                                                attestationSnapshot.Save(serviceUser);
                                                var snapshotId = attestationSnapshot.Id;

                                                AttestationSnapshotAction attestationSnapshotAction = new AttestationSnapshotAction()
                                                {
                                                    AttestationSnapshotId = snapshotId,
                                                    Approve = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Retain,
                                                    Revoke = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Revoke,
                                                    IsDelta = !entitlementRecon.LastAttestedOn.HasValue,
                                                    IsVisible = attestationHierarchyToBuild.IsVisible,
                                                    IsActionable = entitlementRecon.IsDefaultRole ? false : attestationHierarchyToBuild.IsActionAllowed,
                                                    IsActionExtendable = attestationHierarchyToBuild.IsActionExtendable,
                                                    RootSnapshotId = attestationSnapshot.RootSnapshotId,
                                                };
                                                attestationSnapshotAction.Save(serviceUser);
                                                var snapshotActionId = attestationSnapshotAction.Id;

                                                AttestationSnapshotActionInterim attestationSnapshotActioninterim = new AttestationSnapshotActionInterim()
                                                {
                                                    AttestationSnapshotActionId = snapshotActionId,
                                                    UnSavedApprove = attestationSnapshotAction.Approve,
                                                    UnSavedRevoke = attestationSnapshotAction.Revoke,
                                                    UnSavedComment = attestationSnapshotAction.Comment,
                                                    UnSavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                    UnSavedIsChanged = attestationSnapshotAction.IsChanged,
                                                    UnSavedReserved1 = attestationSnapshotAction.Reserved1,
                                                    UnSavedReserved2 = attestationSnapshotAction.Reserved2,
                                                    UnSavedReserved3 = attestationSnapshotAction.Reserved3,
                                                    SavedApprove = attestationSnapshotAction.Approve,
                                                    SavedRevoke = attestationSnapshotAction.Revoke,
                                                    SavedComment = attestationSnapshotAction.Comment,
                                                    SavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                    SavedIsChanged = attestationSnapshotAction.IsChanged,
                                                    SavedReserved1 = attestationSnapshotAction.Reserved1,
                                                    SavedReserved2 = attestationSnapshotAction.Reserved2,
                                                    SavedReserved3 = attestationSnapshotAction.Reserved3,
                                                };
                                                attestationSnapshotActioninterim.Save(serviceUser);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Additional Levels
                                    //Additional Levels always based on the Endpoint.
                                    //having an assumption saying these level wont attached to user (but configure via EndpointHierarchy.IsAssociatedWithUser).
                                    foreach (var parentEntitlement in parentEntitlements)
                                    {
                                        isUnmappedAccount = null;
                                        if (attestationHierarchyToBuild.IsActionAllowed && !string.IsNullOrWhiteSpace(parentEntitlement.DIUserName) && parentEntitlement.DIUserName.Equals("Unmapped", StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            isUnmappedAccount = true;
                                        }
                                        List<PresentationUserEntitlement> entitlementLevelDetails = new List<PresentationUserEntitlement>();
                                        var applnType = endpoint.ApplicationType;
                                        if(endpoint.ApplicationType == ApplicationType.EAM && parentEntitlement.EntitlementType== EntitlementType.TechnicalRole && attestationHierarchyToBuild.EndpointHierarchy.EntitlementType == EntitlementType.TCodes && !string.IsNullOrWhiteSpace(parentEntitlement.Reserved5))
                                        {
                                            //Reserved 5 in businessrole meant for ApplicationType of dependent system.
                                            //For AD we no need to fetch 
                                            applnType = UtilityMethods.ParseEnum<ApplicationType>(parentEntitlement.Reserved5);
                                            if(applnType == ApplicationType.ADSingaporePowerLocal)
                                                continue;
                                        }
                                        var desc = parentEntitlement.Description;

                                        if (endpoint.ApplicationType == ApplicationType.SRM)
                                        {
                                            if (parentEntitlement.EntitlementType == EntitlementType.TechnicalRole && attestationHierarchyToBuild.EndpointHierarchy.EntitlementType == EntitlementType.TCodes)
                                            {
                                                desc = parentEntitlement.Reserved1;
                                            }
                                            else if (parentEntitlement.EntitlementType == EntitlementType.BusinessRole && attestationHierarchyToBuild.EndpointHierarchy.EntitlementType == EntitlementType.TechnicalRole)
                                            {
                                                desc = parentEntitlement.Description;
                                            }
                                        }else if(endpoint.ApplicationType == ApplicationType.HRMS_FLINS)
                                        {
                                            if (parentEntitlement.EntitlementType == EntitlementType.BusinessRole && attestationHierarchyToBuild.EndpointHierarchy.EntitlementType == EntitlementType.TechnicalRole)
                                            {
                                                desc = parentEntitlement.Reserved1;
                                            }
                                        }
                                        entitlementLevelDetails = EndPointFactory.GetEndPoint(endpoint.EndPointSystemType.EndpointSystemTypeName)
                                        .GetEntitlementHierachyDetailByLevel(endpoint,parentEntitlement.AccountName,parentEntitlement.KeyId,
                                            parentEntitlement.Entity,parentEntitlement.EntitlementType,
                                            attestationHierarchyToBuild.EndpointHierarchy.EntitlementType, parentEntitlement.Reserved5,
                                            parentEntitlement.Reserved6, desc, applnType);

                                        if (entitlementLevelDetails != null && entitlementLevelDetails.Count() > 0)
                                        {
                                            foreach (var entitlementLevelDetail in entitlementLevelDetails)
                                            {
                                                bool isEntryAvailable = false;
                                                bool isAssociatedWithUser = attestationHierarchyToBuild.EndpointHierarchy.IsAssociatedWithUser;

                                                if (!isAssociatedWithUser)
                                                {
                                                    isEntryAvailable = AttestationSnapshot.IsAttestationSnapshotHasAnEntry(
                                                        campaignId,
                                                        endpoint.Id,
                                                        attestationHierarchyToBuild.EndpointHierarchy.EntitlementType,
                                                        entitlementLevelDetail.Name,
                                                        parentEntitlement.Entity,applnType,entitlementLevelDetail.Remarks1);
                                                }

                                                if (isEntryAvailable) continue;

                                                #region For BusinessRoleOnly
                                                if (entType == EntitlementType.BusinessRole)
                                                {
                                                    var attestationSnapshot = new AttestationSnapshot()
                                                    {
                                                        AttestationCampaignId = campaignId,
                                                        EntitlementType = entType,
                                                        Name = entitlementLevelDetail.Name,
                                                        Description = entitlementLevelDetail.Description,
                                                        EndpointId = endpoint.Id,
                                                        EndpointName = endpoint.DisplayName,
                                                        EndpointType = (EndpointType)endpoint.EndpointSystemTypeId,
                                                        AccountStatus = true,
                                                        UserId = parentEntitlement.UserId,
                                                        AccountName = parentEntitlement.AccountName,
                                                        DIUserName = parentEntitlement.DIUserName,
                                                        DIUserDisplayName = parentEntitlement.DIUserDisplayName,
                                                        AccountType = parentEntitlement.AccountType,
                                                        KeyId = entitlementLevelDetail.EntitlementId,
                                                        Entity = entitlementLevelDetail.Name,
                                                        ParentEntity = parentEntitlement.Entity,
                                                        HasRisk = false,
                                                        Level = attestationHierarchyToBuild.EndpointHierarchy.Level,
                                                        Reserved1 = entitlementLevelDetail.Remarks1,
                                                        Reserved2 = entitlementLevelDetail.Remarks2,
                                                        //Commented as per kanchana request Ticket: 11863
                                                        //Reserved3 = entitlementLevelDetail.ValidTo?.ToString("dd/MM/yyyy"),
                                                        Reserved4 = entitlementLevelDetail.Remarks4,
                                                        Reserved5 = entitlementLevelDetail.Remarks5,
                                                        Reserved6 = entitlementLevelDetail.Remarks6,
                                                        Reserved7 = entitlementLevelDetail.Remarks7,
                                                        Reserved8 = entitlementLevelDetail.Remarks8,
                                                        Reserved9 = entitlementLevelDetail.Remarks9,
                                                        Reserved10 = entitlementLevelDetail.Remarks10,
                                                        RootSnapshotId = parentEntitlement.RootSnapshotId.HasValue ? parentEntitlement.RootSnapshotId.Value : parentEntitlement.Id,
                                                    };
                                                    attestationSnapshot.Save(serviceUser);
                                                    var snapshotId = attestationSnapshot.Id;

                                                    AttestationSnapshotAction attestationSnapshotAction = new AttestationSnapshotAction()
                                                    {
                                                        AttestationSnapshotId = snapshotId,
                                                        Approve = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Retain,
                                                        Revoke = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Revoke,
                                                        IsDelta = !entitlementLevelDetail.LastAttestedOn.HasValue,
                                                        IsVisible = attestationHierarchyToBuild.IsVisible,
                                                        IsActionable = isUnmappedAccount != null ? false : attestationHierarchyToBuild.IsActionAllowed,
                                                        IsActionExtendable = attestationHierarchyToBuild.IsActionExtendable,
                                                        RootSnapshotId = attestationSnapshot.RootSnapshotId,
                                                    };
                                                    attestationSnapshotAction.Save(serviceUser);
                                                    var snapshotActionId = attestationSnapshotAction.Id;

                                                    AttestationSnapshotActionInterim attestationSnapshotActioninterim = new AttestationSnapshotActionInterim()
                                                    {
                                                        AttestationSnapshotActionId = snapshotActionId,
                                                        UnSavedApprove = attestationSnapshotAction.Approve,
                                                        UnSavedRevoke = attestationSnapshotAction.Revoke,
                                                        UnSavedComment = attestationSnapshotAction.Comment,
                                                        UnSavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                        UnSavedIsChanged = attestationSnapshotAction.IsChanged,
                                                        UnSavedReserved1 = attestationSnapshotAction.Reserved1,
                                                        UnSavedReserved2 = attestationSnapshotAction.Reserved2,
                                                        UnSavedReserved3 = attestationSnapshotAction.Reserved3,
                                                        SavedApprove = attestationSnapshotAction.Approve,
                                                        SavedRevoke = attestationSnapshotAction.Revoke,
                                                        SavedComment = attestationSnapshotAction.Comment,
                                                        SavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                        SavedIsChanged = attestationSnapshotAction.IsChanged,
                                                        SavedReserved1 = attestationSnapshotAction.Reserved1,
                                                        SavedReserved2 = attestationSnapshotAction.Reserved2,
                                                        SavedReserved3 = attestationSnapshotAction.Reserved3,
                                                    };
                                                    attestationSnapshotActioninterim.Save(serviceUser);
                                                }
                                                #endregion
                                                else
                                                {
                                                    #region Non-BusinessRole
                                                    

                                                    AttestationSnapshot attestationSnapshot = new AttestationSnapshot()
                                                    {
                                                        AttestationCampaignId = campaignId,
                                                        EntitlementType = attestationHierarchyToBuild.EndpointHierarchy.EntitlementType,
                                                        Name = entitlementLevelDetail.Name,
                                                        Description = entitlementLevelDetail.Description,
                                                        EndpointId = parentEntitlement.EndpointId,
                                                        EndpointName = endpoint.DisplayName,
                                                        EndpointType = (EndpointType)endpoint.EndpointSystemTypeId,
                                                        AccountStatus = true,
                                                        UserId = isAssociatedWithUser ? parentEntitlement.UserId : 0,
                                                        AccountName = isAssociatedWithUser ? parentEntitlement.AccountName : null,
                                                        DIUserName = isAssociatedWithUser ? parentEntitlement.DIUserName : null,
                                                        DIUserDisplayName = isAssociatedWithUser ? parentEntitlement.DIUserDisplayName : null,
                                                        AccountType = isAssociatedWithUser ? parentEntitlement.AccountType : AccountType.None,
                                                        KeyId = entitlementLevelDetail.EntitlementId,
                                                        Entity = entitlementLevelDetail.Name,
                                                        ParentEntity = parentEntitlement.Entity,
                                                        HasRisk = false,
                                                        Level = attestationHierarchyToBuild.EndpointHierarchy.Level,
                                                        Reserved1 = entitlementLevelDetail.Remarks1,
                                                        Reserved2 = entitlementLevelDetail.Remarks2,
                                                        //Commented as per kanchana request Ticket: 11863
                                                        //Reserved3 = entitlementLevelDetail.ValidTo?.ToString("dd/MM/yyyy"),
                                                        Reserved4 = entitlementLevelDetail.Remarks4,
                                                        Reserved5 = entitlementLevelDetail.Remarks5,
                                                        Reserved6 = entitlementLevelDetail.Remarks6,
                                                        Reserved7 = entitlementLevelDetail.Remarks7,
                                                        Reserved8 = entitlementLevelDetail.Remarks8,
                                                        Reserved9 = entitlementLevelDetail.Remarks9,
                                                        Reserved10 = entitlementLevelDetail.Remarks10,
                                                        RootSnapshotId = isAssociatedWithUser ? parentEntitlement.UserId : 0,
                                                    };

                                                    attestationSnapshot.Save(serviceUser);
                                                    var snapshotId = attestationSnapshot.Id;

                                                    AttestationSnapshotAction attestationSnapshotAction = new AttestationSnapshotAction()
                                                    {
                                                        AttestationSnapshotId = snapshotId,
                                                        Approve = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Retain,
                                                        Revoke = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Revoke,
                                                        IsDelta = !entitlementLevelDetail.LastAttestedOn.HasValue,
                                                        IsVisible = attestationHierarchyToBuild.IsVisible,
                                                        IsActionable = isUnmappedAccount != null ? false : attestationHierarchyToBuild.IsActionAllowed,
                                                        IsActionExtendable = attestationHierarchyToBuild.IsActionExtendable,
                                                        RootSnapshotId = attestationSnapshot.RootSnapshotId,
                                                    };
                                                    attestationSnapshotAction.Save(serviceUser);
                                                    var snapshotActionId = attestationSnapshotAction.Id;

                                                    AttestationSnapshotActionInterim attestationSnapshotActioninterim = new AttestationSnapshotActionInterim()
                                                    {
                                                        AttestationSnapshotActionId = snapshotActionId,
                                                        UnSavedApprove = attestationSnapshotAction.Approve,
                                                        UnSavedRevoke = attestationSnapshotAction.Revoke,
                                                        UnSavedComment = attestationSnapshotAction.Comment,
                                                        UnSavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                        UnSavedIsChanged = attestationSnapshotAction.IsChanged,
                                                        UnSavedReserved1 = attestationSnapshotAction.Reserved1,
                                                        UnSavedReserved2 = attestationSnapshotAction.Reserved2,
                                                        UnSavedReserved3 = attestationSnapshotAction.Reserved3,
                                                        SavedApprove = attestationSnapshotAction.Approve,
                                                        SavedRevoke = attestationSnapshotAction.Revoke,
                                                        SavedComment = attestationSnapshotAction.Comment,
                                                        SavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                                                        SavedIsChanged = attestationSnapshotAction.IsChanged,
                                                        SavedReserved1 = attestationSnapshotAction.Reserved1,
                                                        SavedReserved2 = attestationSnapshotAction.Reserved2,
                                                        SavedReserved3 = attestationSnapshotAction.Reserved3,
                                                    };
                                                    attestationSnapshotActioninterim.Save(serviceUser);

                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
                return candidateUser;
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Exception, string.Format("CampaignID - {0}, Exception - {1}", campaignId, ex.Message));
                throw ex;
            }
        }
        /// <summary>
        /// Check whether selected endpoint is part of default system or not
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private bool IsAllowAccountLevelActionable(EndPoint endpoint, User user, bool IsActionAllowed, AccountReconcilation accountRecon)
        {
            //SAPEP/CE account created automatically through FLINS. Hence Action is not required at Account level
            //IDMS also created automatically hence account be removed
            if (dependentApplications.Contains(endpoint.ApplicationType))
            {
                return false;
            }
            else if (user != null && user.FullTimeEmployee && accountRecon.AccountType == AccountType.User && 
                (defaultApplications != null && defaultApplications.Exists(i=> i.Id == endpoint.Id)))
            {
                return false;
            }
            return IsActionAllowed;
        }

        bool loadInVisibleLevelAlso(AttestationHierarchy attestationHierarchyToBuild, List<AttestationHierarchy> attestationHierarchy)
        {
            bool load = false;
            var curLevel = attestationHierarchyToBuild.EndpointHierarchy.Level;
            var parent = attestationHierarchyToBuild.EndpointHierarchy.Id;
            var nextLevelsToLoad = attestationHierarchy.Where(a => a.EndpointHierarchy.Level > curLevel && a.EndpointHierarchy.ParentId == parent).ToList();
            if (nextLevelsToLoad != null && nextLevelsToLoad.Count > 0 && nextLevelsToLoad.First().IsVisible)
                return true;
            else if (nextLevelsToLoad != null && nextLevelsToLoad.Count > 0)
                return loadInVisibleLevelAlso(nextLevelsToLoad.First(), attestationHierarchy);
            return load;
        }

        void createAttestationSnapshotForAccount(long campaignId, AccountReconcilation accountDetail, AttestationHierarchy attestationHierarchyToBuild, User user, EndPoint endpoint, bool customActionable = false)
        {
            string serviceUser = MasterConfigHelper.ServiceUser;
            AttestationSnapshot attestationSnapshot = new AttestationSnapshot()
            {
                AttestationCampaignId = campaignId,
                EntitlementType = EntitlementType.Account,
                Name = accountDetail.AccountName,
                Description = accountDetail.FullName,
                EndpointId = accountDetail.EndpointSystemId,
                EndpointName = accountDetail.EndPoint != null ? accountDetail.EndPoint.DisplayName : endpoint.DisplayName,
                EndpointType = accountDetail.EndPoint.EndPointSystemType != null ? accountDetail.EndPoint.EndPointSystemType.EndpointSystemTypeName : endpoint.EndPointSystemType.EndpointSystemTypeName,
                LastLogon = accountDetail.LastLogonDate,
                AccountStatus = (accountDetail.IsDeleted == false && accountDetail.IsLocked == false),
                UserId = accountDetail.UserId.HasValue ? (long)accountDetail.UserId : 0,
                AccountType = accountDetail.AccountType,
                KeyId = accountDetail.Id,
                Entity = accountDetail.AccountName,
                Level = attestationHierarchyToBuild.EndpointHierarchy.Level,
                HasRisk = accountDetail.IsSensitiveAccess || accountDetail.IsSODConflict,
                AccountName = accountDetail.AccountName,
                DIUserName = user != null ? user.UserName : "Unmapped",
                DIUserDisplayName = user != null ? user.DisplayName : "Unmapped Accounts",
                Reserved3 = accountDetail.ValidToDate?.ToString("dd/MM/yyyy"),
                Reserved2 = (accountDetail.IsDeleted == false && accountDetail.IsLocked == false) ? null : "Locked"
            };

            attestationSnapshot.Save(serviceUser);
            var snapshotId = attestationSnapshot.Id;

            AttestationSnapshotAction attestationSnapshotAction = new AttestationSnapshotAction()
            {
                AttestationSnapshotId = snapshotId,
                Approve = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Retain,
                Revoke = attestationHierarchyToBuild.DefaultAttestationAction == AttestationDefaultReviewStatus.Revoke,
                IsDelta = !accountDetail.LastAttestedOn.HasValue,
                IsVisible = attestationHierarchyToBuild.IsVisible,
                IsActionable = customActionable,
                IsActionExtendable = customActionable == false ? false : attestationHierarchyToBuild.IsActionExtendable,
            };
            attestationSnapshotAction.Save(serviceUser);
            var snapshotActionId = attestationSnapshotAction.Id;

            AttestationSnapshotActionInterim attestationSnapshotActioninterim = new AttestationSnapshotActionInterim()
            {
                AttestationSnapshotActionId = snapshotActionId,
                UnSavedApprove = attestationSnapshotAction.Approve,
                UnSavedRevoke = attestationSnapshotAction.Revoke,
                UnSavedComment = attestationSnapshotAction.Comment,
                UnSavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                UnSavedIsChanged = attestationSnapshotAction.IsChanged,
                UnSavedReserved1 = attestationSnapshotAction.Reserved1,
                UnSavedReserved2 = attestationSnapshotAction.Reserved2,
                UnSavedReserved3 = attestationSnapshotAction.Reserved3,
                SavedApprove = attestationSnapshotAction.Approve,
                SavedRevoke = attestationSnapshotAction.Revoke,
                SavedComment = attestationSnapshotAction.Comment,
                SavedEffectiveDate = attestationSnapshotAction.EffectiveDate,
                SavedIsChanged = attestationSnapshotAction.IsChanged,
                SavedReserved1 = attestationSnapshotAction.Reserved1,
                SavedReserved2 = attestationSnapshotAction.Reserved2,
                SavedReserved3 = attestationSnapshotAction.Reserved3,
            };
            attestationSnapshotActioninterim.Save(serviceUser);
        }
        #endregion

        public void DoAttestation(Attestation _attestation, Process process)
        {
            AttestationProcess attestationprocess = null;
            DILog.AttestationLogger.WriteLog(LogLevelType.Info, string.Format("Started Excecuting  AttestationId - {0} Triggered by {1}", _attestation.Id.ToString(), _attestation.CreatedBy));
            attestationprocess = _attestation.AttestationProcesses.FirstOrDefault();
            m_historyId = attestationprocess.HistoryId.HasValue ? attestationprocess.HistoryId.Value : 0;

            if (null == attestationprocess)
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Warning, string.Format("Attestation Process information NOT found for AttestationId - {0}", _attestation.Id.ToString()));

                throw new EntityNotFoundException(string.Format("Attestation Process information NOT found for AttestationId - {0}", _attestation.Id.ToString()));
            }
            if (attestationprocess.WorkflowStatus == SchedulerStatus.InProgress.ToString())
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Warning, string.Format("Attestation {0}:{1} already in progress by workflow id {2}", _attestation.Id.ToString(), _attestation.AttestationName, _attestation.WorkflowConfigurationId.ToString()));

                throw new Exception(string.Format("Attestation {0}:{1} already in progress by workflow id {2}", _attestation.Id.ToString(),
                               _attestation.AttestationName,
                               _attestation.WorkflowConfigurationId.ToString()));
            }

            m_historyId += 1;

            ChangeAttestationStatus(process, SchedulerStatus.InProgress, _attestation.AttestationProcesses.FirstOrDefault());

            // create Attestation Campaign and get Campaign id
            var organizations = NOrganisation.GetAllOrganisations();
            //var idmsUsrs = User.GetAllActiveUser();
            
            var users = FindUsers(_attestation).ToList();
            if (_attestation.AttestationType == AttestationType.User)
            {
                bool splitCampaign = true;
                DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Total No.of Active User:{users.Count}");
                if (((users == null || users.Count == 0) && _attestation.IncludeUnmappedAccount) || (users != null && users.Count > 0))
                {
                    long campaignId = CreateAttestationCampaign(_attestation, true, 0, null);
                    AttestationLevel1Service level1Service = new AttestationLevel1Service(_attestation, m_historyId, campaignId);
                    #region Department
                    if (_attestation.SplitCampaignUsing == AttestationScopeEnum.Department)
                    {
                        var grpUsrColls = users.GroupBy(a => a.DepartmentCode).ToList();
                        bool includeUnmappedAccount = false; long splitCount = grpUsrColls.Count;
                        
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Departement Count:{splitCount}");
                        
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            var splitValue = !string.IsNullOrWhiteSpace(grpUsrColl.Key) && !grpUsrColl.Key.Equals("00000000") ? grpUsrColl.Key.ToString() : "No Department";

                            if (!string.IsNullOrWhiteSpace(grpUsrColl.Key))
                            {
                                var department = organizations.FirstOrDefault(i=> i.Code.Equals(grpUsrColl.Key,StringComparison.InvariantCultureIgnoreCase));
                                if (department != null)
                                    splitValue = $"{department.Name}({department.Code})";
                            }
                            
                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, includeUnmappedAccount, campaignId, splitValue);
                        }

                        if (_attestation.IncludeUnmappedAccount)
                        {
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                    }
                    #endregion
                    #region Company
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.Company)
                    {
                        var companies = Company.GetAll();
                        var grpUsrColls = users.GroupBy(a => a.CompanyId).ToList();
                        bool includeUnmappedAccount = false; long count = 0; long splitCount = grpUsrColls.Count;
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Company Count:{splitCount}");
                        
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            count += 1;
                            var splitValue = grpUsrColl.Key.HasValue && grpUsrColl.Key.Value > 0 ? grpUsrColl.Key.ToString() : "No Company";

                            if (grpUsrColl.Key.HasValue)
                            {
                                var company = companies.FirstOrDefault(i=> i.Id == grpUsrColl.Key);
                                if (company != null)
                                    splitValue = $"{company.Text}({company.Code})";
                            }

                            //if (splitCount > 1 && count == splitCount && _attestation.IncludeUnmappedAccount)
                            //    includeUnmappedAccount = true;
                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, includeUnmappedAccount, campaignId, splitValue);
                        }
                        if (_attestation.IncludeUnmappedAccount)
                        {
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                    }
                    #endregion
                    #region Branch
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.Branch)
                    {
                        var grpUsrColls = users.GroupBy(a => a.BranchCode).ToList();
                        bool includeUnmappedAccount = false; long count = 0; long splitCount = grpUsrColls.Count;
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Branch Count:{splitCount}");
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            count += 1;
                            var splitValue = !string.IsNullOrWhiteSpace(grpUsrColl.Key) && !grpUsrColl.Key.Equals("00000000") ? grpUsrColl.Key : "No BranchCode";

                            if (!string.IsNullOrWhiteSpace(grpUsrColl.Key))
                            {
                                var branch = organizations.FirstOrDefault(i=> i.Code.Equals(grpUsrColl.Key,StringComparison.InvariantCultureIgnoreCase));
                                if (branch != null)
                                    splitValue = $"{branch.Name}({branch.Code})";
                            }

                            //if (splitCount > 1 && count == splitCount && _attestation.IncludeUnmappedAccount && !_attestation.CreateTaskOnNoReviewer)
                            //    includeUnmappedAccount = true;

                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, includeUnmappedAccount, campaignId, splitValue);
                        }

                        if (_attestation.IncludeUnmappedAccount)
                        {
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                    }
                    #endregion
                    #region Section
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.Section)
                    {
                        var grpUsrColls = users.GroupBy(a => a.SectionCode).ToList();
                        bool includeUnmappedAccount = false; long count = 0; long splitCount = grpUsrColls.Count;
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Section Count:{splitCount}");
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            count += 1;
                            var splitValue = !string.IsNullOrWhiteSpace(grpUsrColl.Key) && !grpUsrColl.Key.Equals("00000000") ? grpUsrColl.Key.ToString() : "No Section";
                            if (!string.IsNullOrWhiteSpace(grpUsrColl.Key))
                            {
                                var section = organizations.FirstOrDefault(i=>i.Code.Equals(grpUsrColl.Key,StringComparison.InvariantCultureIgnoreCase));
                                if (section != null)
                                    splitValue = splitValue = $"{section.Name}({section.Code})";
                            }

                            //if (splitCount > 1 && count == splitCount && _attestation.IncludeUnmappedAccount && !_attestation.CreateTaskOnNoReviewer)
                            //    includeUnmappedAccount = true;

                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, includeUnmappedAccount, campaignId, splitValue);
                        }

                        if ( _attestation.IncludeUnmappedAccount)
                        {
                            //createUserCampaignTask(users, level1Service, _attestation, process, _attestation.IncludeUnmappedAccount, campaignId);
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                    }
                    #endregion
                    #region Unit
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.Unit)
                    {

                        var grpUsrColls = users.GroupBy(a => a.UnitCode).ToList();
                        bool includeUnmappedAccount = false; long count = 0; long splitCount = grpUsrColls.Count;
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"Unit Count:{splitCount}");
                        
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            count += 1;
                            var splitValue = !string.IsNullOrWhiteSpace(grpUsrColl.Key) && !grpUsrColl.Key.Equals("00000000") ? grpUsrColl.Key.ToString() : "No Unit";

                            if (!string.IsNullOrWhiteSpace(grpUsrColl.Key))
                            {
                                var unitOrg = organizations.FirstOrDefault(i=> i.Code.Equals(grpUsrColl.Key,StringComparison.InvariantCultureIgnoreCase));
                                if (unitOrg != null)
                                    splitValue = $"{unitOrg.Name}({unitOrg.Code})";
                            }

                            //if (splitCount > 1 && count == splitCount && _attestation.IncludeUnmappedAccount && !_attestation.CreateTaskOnNoReviewer)
                            //    includeUnmappedAccount = true;

                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, includeUnmappedAccount, campaignId, splitValue);
                        }

                        if (_attestation.IncludeUnmappedAccount)
                        {
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                    }
                    #endregion
                    #region Manager
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.RO)
                    {
                        var grpUsrColls = users.GroupBy(a => a.ROEmployeeId).ToList();
                        long splitCount = grpUsrColls.Count;
                        DILog.AttestationLogger.WriteLog(LogLevelType.Info, $"RO Count:{splitCount}");
                        var roUser = User.GetAllRO();
                        foreach (var grpUsrColl in grpUsrColls)
                        {
                            var splitValue = !string.IsNullOrWhiteSpace(grpUsrColl.Key) && !grpUsrColl.Key.Equals("00000000") ? grpUsrColl.Key.ToString() : "No Reporting Officer";

                            if (!string.IsNullOrWhiteSpace(grpUsrColl.Key))
                            {
                                var mgr = roUser.FirstOrDefault(i => i.EmployeeId.Equals(grpUsrColl.Key, StringComparison.InvariantCultureIgnoreCase));
                                if (mgr != null)
                                    splitValue = $"{mgr.DisplayName}({(!string.IsNullOrWhiteSpace(mgr.UserName)? mgr.UserName : mgr.EmployeeId)})";
                            }

                            createUserCampaignTask(grpUsrColl.ToList(), level1Service, _attestation, process, true, false, campaignId, splitValue);
                        }

                        if (_attestation.IncludeUnmappedAccount)
                        {
                            createUserCampaignTask(new List<User>(), level1Service, _attestation, process, true, true, campaignId, "Unmapped Account");
                        }
                        roUser = null;
                    }
                    #endregion
                    #region None
                    else if (_attestation.SplitCampaignUsing == AttestationScopeEnum.None)
                    {
                        splitCampaign = false;
                        createUserCampaignTask(users, level1Service, _attestation, process, false, _attestation.IncludeUnmappedAccount, campaignId, null);
                    }
                    if (splitCampaign)
                    {
                        if (AttestationCampaign.IsAnyChildCampaignInProgress(campaignId))
                        {
                            AttestationCampaign.ExecuteQuery($"Update DI_M_AttestationCampaign SET status=0 WHERE Id={campaignId}");
                        }
                        else
                        {
                            AttestationCampaign.ExecuteQuery($"Update DI_M_AttestationCampaign SET status=3 WHERE Id={campaignId}");
                        }
                        Thread.Sleep(120000);
                        UpdateSplitChildCount(campaignId);
                    }
                    #endregion
                    this.UpdateStatus(_attestation.AttestationProcesses.FirstOrDefault(), process);

                    ChangeAttestationStatus(process, SchedulerStatus.Submitted, _attestation.AttestationProcesses.FirstOrDefault());
                    ProcessHistory history = GetHistory(process.Id, Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Submitted), "");
                    history.Save(MasterConfigHelper.ServiceUser);

                    Thread.Sleep(10000);
                    //Commented By Siva. this code No longer valid.
                    //level1Service.AutoSubmitLevel1Task(campaignId);
                }
                else
                {
                    process.ErrorMessage = "There is no matching Users for Attestation";
                    ChangeAttestationStatus(process, SchedulerStatus.Failed, _attestation.AttestationProcesses.FirstOrDefault());
                }
            }
            //create Campaign Attestation Snapshot

            //var users = FindUsers(_attestation).ToList();

            //if (((users == null || users.Count == 0) && _attestation.IncludeUnmappedAccount) || (users != null && users.Count > 0))
            //{
            //    //commented below line, so that we can create campaign based on the split
            //    //var candidateUsers = CreateAttestationCampaignSnapshot(_attestation, campaignId, users);

            //    createUserCampaignTask(users, level1Service, _attestation, process, _attestation.IncludeUnmappedAccount, campaignId);

            //    this.UpdateStatus(_attestation.AttestationProcesses.FirstOrDefault(), process);

            //    ChangeAttestationStatus(process, SchedulerStatus.Submitted, _attestation.AttestationProcesses.FirstOrDefault());
            //    ProcessHistory history = GetHistory(process.Id, Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Submitted), "");
            //    history.Save(MasterConfigHelper.ServiceUser);

            //    Thread.Sleep(10000);

            //    level1Service.AutoSubmitLevel1Task(campaignId);
            //}
            //else
            //{
            //    process.ErrorMessage = "There is no matching Users for Attestation";
            //    ChangeAttestationStatus(process, SchedulerStatus.Failed, _attestation.AttestationProcesses.FirstOrDefault());
            //}


            attestationprocess.HistoryId = m_historyId;
            organizations = null;
            users = null;
            //idmsUsrs = null;
        }

        void createUserCampaignTask(List<User> candidateUsers, AttestationLevel1Service level1Service, Attestation _attestation,
            Process process, bool isSplitCampaignNeeded, bool includeUnmappedAccount, long campaignId, string splitValue)
        {
            try
            {
                long subCampaignId = 0;
                if (isSplitCampaignNeeded)
                {
                    subCampaignId = CreateAttestationCampaign(_attestation, false, campaignId, splitValue);
                }
                else
                    subCampaignId = campaignId;
                try
                {
                    
                    var snapshotCandidateUsers = CreateAttestationCampaignSnapshot(_attestation, subCampaignId, candidateUsers, includeUnmappedAccount);

                    level1Service.AttestationCampaignID = subCampaignId;
                    if ((((snapshotCandidateUsers == null || snapshotCandidateUsers.Count == 0) && _attestation.IncludeUnmappedAccount) ||
                        (snapshotCandidateUsers != null &&
                        snapshotCandidateUsers.Count > 0)) &&
                        level1Service.RetrieveReviews(snapshotCandidateUsers.ToList()))
                    {
                        bool saved = level1Service.Save();
                        if (!saved)
                        {
                            var errorMessage = "There is no matching Endpoint Users for Attestation";
                            ChangeAttestationCampaignStatus(subCampaignId, AttestationCampaignStatus.IgnoreNoUserFound, errorMessage);
                        }
                        else
                        {
                            //var campaign = AttestationCampaign.GetById(subCampaignId);
                            //campaign.Status = AttestationCampaignStatus.WorkInProgress;
                            //campaign.Save(serviceUser);
                            AttestationCampaign.UpdateCampaignStatus(subCampaignId, AttestationCampaignStatus.WorkInProgress);
                            Thread.Sleep(MasterConfigHelper.DelayAttestationCampaignCreation);
                        }
                    }
                    else
                    {
                        if (isSplitCampaignNeeded && includeUnmappedAccount)
                        {
                            //var errorMessage = "There is no matching Endpoint Users for Attestation";
                            ChangeAttestationCampaignStatus(subCampaignId, AttestationCampaignStatus.IgnoreNoUserFound, errorMessage);
                        }
                        else if (isSplitCampaignNeeded)
                        {
                            //var errorMessage = "There is no matching Endpoint Users for Attestation";
                            ChangeAttestationCampaignStatus(subCampaignId, AttestationCampaignStatus.IgnoreNoUserFound, errorMessage);
                        }
                        else
                        {
                            process.ErrorMessage = errorMessage;
                            ChangeAttestationStatus(process, SchedulerStatus.Failed, _attestation.AttestationProcesses.FirstOrDefault());
                        }
                    }
                }
                catch (Exception ex)
                {
                    //var errorMessage = "There is no matching Endpoint Users for Attestation";
                    ChangeAttestationCampaignStatus(subCampaignId, AttestationCampaignStatus.IgnoreNoUserFound, "Error Occured while Submitting Campaign");
                    DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Error occured while creating campaign :{subCampaignId} {ex.Message}", ex);
                }

            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"createUserCampaignTask  {_attestation.Id}:{_attestation.AttestationName} Error while updating status workflow id {_attestation.WorkflowConfigurationId} {ex.Message}",ex);
            }
        }

        private void UpdateStatus(AttestationProcess objAtnProcess, Process objProcess)
        {
            string serviceUser = MasterConfigHelper.ServiceUser;
            ProcessHistory objPrcHistory = ProcessHistory.GetByProcessType(objProcess.Id, ProcessType.Attestation, objProcess.Status);

            objAtnProcess.WorkflowStatus = Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Completed);
            objProcess.Status = Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Completed);
            objAtnProcess.process = objProcess;

            objAtnProcess.Save(serviceUser);

            if (objPrcHistory != null)
            {
                objPrcHistory.Status = Enum.GetName(typeof(SchedulerStatus), SchedulerStatus.Completed);
                objPrcHistory.Save(serviceUser.ToString());
            }
        }

        private ProcessHistory GetHistory(long ProcessId, string Status, string ErrMessage)
        {
            ProcessHistory history = new ProcessHistory();
            history.ProcessType = ProcessType.Attestation;
            history.ProcessId = ProcessId;
            history.Status = Status;
            history.ErrorMessage = ErrMessage;
            return history;
        }

        private void SendEmail(AttestationProcess attestationProcess)
        {
            string strMessage = string.Empty;
            StringBuilder strBldr = new StringBuilder();
            DomainModel.MailTemplate template = null;
            StringBuilder subject = null;

            try
            {
                User user = User.GetByUserNameFromIMProcessor(attestationProcess.CreatedBy);
                var attestationEndPoints = AttestationEndpoint.GetByAttestationId((long)attestationProcess.AttestationId).ToList();
                if (attestationEndPoints != null)
                {
                    string[] endptNames = new string[attestationEndPoints.Count];
                    string[] endptTypeNames = new string[attestationEndPoints.Count];
                    int i = 0;
                    foreach (var attestationEndPoint in attestationEndPoints)
                    {
                        var endPoint = EndPoint.GetByIdIncludingEndPointSystemType(attestationEndPoint.EndpointSystemId);
                        if (endPoint != null)
                        {
                            endptNames[i] = endPoint.DisplayName;
                            endptTypeNames[i] = endPoint.EndPointSystemType.DisplayName;
                            i++;
                        }
                    }

                    string processEndPt = String.Join(",", endptNames);
                    string processEndPtType = String.Join(",", endptTypeNames);

                    Process process = Process.GetByProcessType((long)attestationProcess.Id, ProcessType.Attestation);
                    EmailInfo emailInfo = new EmailInfo();
                    template = MailTemplate.GetByTemplateName(Constants.AttestationJobFailed);
                    subject = new StringBuilder(template.MailSubject);
                    subject.Replace("@@PROCESSNAME@@", attestationProcess.ProcessName);
                    subject.Replace("@@ENDPOINTNAME@@", processEndPt);
                    StringBuilder body = new StringBuilder(template.MailContent);
                    if (process != null)
                    {
                        body.Replace("@@ERRORMESSAGE@@", process.ErrorMessage);
                    }
                    body.Replace("@@RECEIVER@@", processEndPtType);
                    body.Replace("@@ENDPOINTTYPE@@", processEndPtType);
                    body.Replace("@@ENDPOINTNAME@@", processEndPt);
                    body.Replace("@@PROCESSNAME@@", attestationProcess.ProcessName);
                    body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);
                    emailInfo.Body = body.ToString();
                    emailInfo.Subject = subject.ToString();
                    emailInfo.To = $"{user.EmailId},{template.MailTo}";
                    emailInfo.From = template.MailFrom;
                    emailInfo.Cc = $"{string.Join(",", MasterConfigHelper.AlertNotificationReceivers)}";
                    emailInfo.ActivityType = EmailActivityType.Attestation.ToString();
                    EmailHandler.Send(emailInfo);
                }
            }
            catch (Exception ex)
            {
                DILog.AttestationLogger.WriteLog((int)attestationProcess.AttestationId, LogLevelType.Exception, "Failed in composing mail from template", ex);
            }
        }
    }
}
