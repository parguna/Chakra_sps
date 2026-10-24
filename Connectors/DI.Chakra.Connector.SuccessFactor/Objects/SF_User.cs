using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using System.Globalization;
using DI.Chakra.Connector.SF.Base;
using DI.Chakra.Infrastructure;
using JetBrains.Annotations;
using DI.Chakra.Infrastructure.Enums;
using System.Reflection;
using System.Data;

namespace DI.Chakra.Connector.SF.Objects
{
    public class SF_User : FO_Base
    {
        protected List<Company> companies = null;
        protected List<NOrganisation> organisations = null;
        protected List<NDesignation> positions = null;
        protected List<NCostCenter> costCenters = null;
        readonly DateTime MaxDate = new DateTime(9999, 12, 31);
        readonly DateTime CurrentDate;
        readonly string officeAddressType = "Office";
        readonly StringBuilder addressBuilder = new StringBuilder();
        readonly string AuthenticationMode = null;
        readonly List<EventTypeReason> OnBoardTriggersNotToBeConsideredForJoinDate;
        readonly List<string> propagationFields;
        readonly List<EventTypeReason> OnBoardTriggers;
        readonly List<EventTypeReason> OffBoardTriggers;
        readonly List<EventTypeReason> PropagationTriggers;
        readonly List<EventTypeReason> ReOrgTriggers;
        readonly List<EventTypeReason> TranferTriggers;
        readonly List<EventTypeReason> PromotionTriggers;
        readonly string[] ExecutiveEmployeeFlags;
        readonly string[] ActiveEmployeeFlags;
        readonly List<EventTypeReason> RejoinTriggers;
        readonly bool canRunPropagationTrigger;
        readonly PasswordPolicy iacmPolicy;
        readonly int MasterConfigHelperRoleUser;
        readonly EndPoint endPoint = null;
        readonly double onBoardDaysBeforeJoining;
        readonly List<EthnicityGroup> ethnicityGroups;
        readonly string _password = null;
        public readonly string PhoneTypeForTelephone = string.Empty;
        public readonly string PhoneTypeForMobile = string.Empty;

        protected SFOData.PickListValueV2 emailType = null;
        protected SFOData.PickListValueV2 officePhoneType = null;
        readonly SendmailDelegate sendmailDelegate;
        readonly string _idmsTransferNotes = null;
        public SF_User(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser, EndPoint ep, SendmailDelegate emailSend) : base(_entityContainer, _idmsLogger, _serviceUser)
        {

            CurrentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            AuthenticationMode = MasterConfigHelper.AuthenticationMode;
            OnBoardTriggersNotToBeConsideredForJoinDate = MasterConfigHelper.OnBoardTriggersNotToBeConsideredForJoinDate;
            propagationFields = MasterConfigHelper.PropagationFields;
            OnBoardTriggers = MasterConfigHelper.OnBoardTriggers;
            OffBoardTriggers = MasterConfigHelper.OffBoardTriggers;
            ReOrgTriggers = MasterConfigHelper.ReOrgTriggers;
            TranferTriggers = MasterConfigHelper.TranferTriggers;
            ExecutiveEmployeeFlags = MasterConfigHelper.SF_ExecutiveEmployeeFlags;
            ActiveEmployeeFlags = MasterConfigHelper.SF_ActiveEmployeeFlags;
            RejoinTriggers = MasterConfigHelper.RejoinTriggers;
            canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;
            PromotionTriggers = MasterConfigHelper.PromotionTriggers;
            iacmPolicy = PasswordPolicy.GetById(1);
            MasterConfigHelperRoleUser = MasterConfigHelper.RoleUser;
            endPoint = ep;
            onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
            PropagationTriggers = MasterConfigHelper.PropagationTriggers;
            PhoneTypeForMobile = MasterConfigHelper.PhoneTypeForMobile;
            PhoneTypeForTelephone = MasterConfigHelper.PhoneTypeForTelephone;
            ethnicityGroups = new List<EthnicityGroup>();
            var default_password = MasterConfigHelper.DefaultPassword;
            if (!string.IsNullOrWhiteSpace(default_password))
                _password = RSAHelper.EncryptData(default_password);

            sendmailDelegate = emailSend;

            _idmsTransferNotes = MasterConfigHelper.TranferTriggersNotes;
        }

        public void Initialize()
        {
            idmsLogger.WriteLog(LogLevelType.Info, "Load Master Data Started");
            companies = Company.GetAll();
            organisations = NOrganisation.GetAll();
            positions = NDesignation.GetAll();
            costCenters = NCostCenter.GetAll();
            idmsLogger.WriteLog(LogLevelType.Info, "Load Master Data Completed");
        }

        #region Full Discovery

        public void FullDiscovery()
        {
            try
            {
                idmsLogger.WriteLog(LogLevelType.Info, "Full Discovery Started");

                int page = 0;
                var totalCount = entityContainer.User.Select(u =>
                                    new SFOData.User()
                                    {
                                        userId = u.userId,
                                        status = u.status,
                                        state = u.state,
                                        username = u.username,
                                        manager = u.manager != null ? new SFOData.User()
                                        {
                                            userId = u.manager.userId,
                                            status = u.manager.status,
                                            state = u.manager.state
                                        } : null,
                                    }).ToList().Count();
                int processed = 0;
                int savedUserCount = 0;
                int pageSize = 500;
                var dt = DateTime.Now;
                DateTime effectiveDate = new DateTime(dt.Year, dt.Month, dt.Day);

                while (pageSize * page < totalCount)
                {

                    var userList = entityContainer.User.Skip(pageSize * page)
                        .Select(u =>
                                    new SFOData.User()
                                    {
                                        userId = u.userId,
                                        status = u.status,
                                        state = u.state,
                                        username = u.username,
                                        manager = u.manager != null ? new SFOData.User()
                                        {
                                            userId = u.manager.userId,
                                            status = u.manager.status,
                                            state = u.manager.state
                                        } : null,
                                    }).Take(pageSize).ToList();
                    processed = userList.Count() + processed;

                    foreach (var sfUser in userList)
                    {
                        savedUserCount = savedUserCount + getUser(sfUser, effectiveDate);
                    }
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"Full Discovery Ended. Total No. of Users : {totalCount}. Processed Users : {processed} and Saved Users : {savedUserCount} ");
            }
            catch (Exception e)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "Exception in Full Discovery", e);
                throw e;
            }

        }

        public void GetUserByPernr(string PERNR)
        {
            var sfUser = this.entityContainer.User.Where(u => u.userId == PERNR.TrimLeadingSeros()).Select(u =>
                                  new SFOData.User()
                                  {
                                      userId = u.userId,
                                      status = u.status,
                                      state = u.state,
                                      username = u.username,
                                      manager = u.manager != null ? new SFOData.User()
                                      {
                                          userId = u.manager.userId,
                                          status = u.manager.status,
                                          state = u.manager.state
                                      } : null,
                                  }).FirstOrDefault();
            getUser(sfUser, this.CurrentDate);
        }

        int getUser(SFOData.User sfUser, DateTime effectiveStartDate)
        {
            User diUser = constructDomainUser(sfUser, effectiveStartDate);

            if (diUser == null)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "Could not generate DomainModel.User object.", null);
                return 0;
            }

            getOrgInformation(diUser, diUser.EmployeeId.TrimLeadingSeros(), effectiveStartDate);

            return saveUser(diUser);
        }

        int saveUser(User diUser)
        {
            diUser.Save(serviceUser);
            User.DeriveCOEmployeeId(diUser.Id);
            idmsLogger.WriteLog(LogLevelType.Info, $"User saved for the PERNR : {diUser.EmployeeId}", null);
            User.InsertUserRole(diUser.Id, MasterConfigHelperRoleUser);
            User.DoAfterSaveUser(diUser, serviceUser);
            return 1;
        }

        #endregion Full Discovery

        #region Delta Discovery

        public int DeltaDiscovery(DateTime deltaDate)
        {
            List<string> changeSetPERNRs = new List<string>();
            idmsLogger.WriteLog(LogLevelType.Info, $"Delta Discovery Started. The delta is {deltaDate}");
            var failedDiscoveryUsers = new List<string>();
            try
            {

                changeSetPERNRs = getDeltaChangeSet(deltaDate);
                idmsLogger.WriteLog(LogLevelType.Info, $"Found {changeSetPERNRs.Count()} PERNRs on {deltaDate} delta discovery");
                SFDiscoveryStatus sfDiscoveryStatus = null;
                var notify = MasterConfigHelper.EnableSFFailedNotification;
                foreach (var PERNR in changeSetPERNRs)
                {
                    var empId = PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize);
                    if (empId.IsNumeric())
                    {
                        sfDiscoveryStatus = discoverUserByPERNR(empId, deltaDate);
                        if (sfDiscoveryStatus != null && sfDiscoveryStatus.SFDiscoveryStatusType == SFDiscoveryStatusType.Failure)
                        {
                            failedDiscoveryUsers.Add(empId);
                            try
                            {
                                idmsLogger.WriteLog(LogLevelType.Warning, $"Retry Disocvery failed for User:{PERNR} and message:{sfDiscoveryStatus?.Message}");
                                if (notify)
                                {
                                    sendmailDelegate.Invoke(this.endPoint, idmsLogger, sfDiscoveryStatus.Message, PERNR);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                if (failedDiscoveryUsers != null && failedDiscoveryUsers.Count > 0)
                {
                    var retryFailedUsers = new List<string>();
                    try
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"Failed Perno's count:{failedDiscoveryUsers.Count}");
                        foreach (var PERNR in failedDiscoveryUsers)
                        {
                            var empId = string.Empty;
                            try
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Retry failed Pernos count:{failedDiscoveryUsers.Count}");
                                empId = PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize);
                                if (empId.IsNumeric())
                                {
                                    sfDiscoveryStatus = discoverUserByPERNR(empId, deltaDate);
                                    if (sfDiscoveryStatus != null && sfDiscoveryStatus.SFDiscoveryStatusType == SFDiscoveryStatusType.Failure)
                                    {
                                        idmsLogger.WriteLog(LogLevelType.Warning, $"Disocvery failed for User:{PERNR} and message:{sfDiscoveryStatus?.Message}");
                                        retryFailedUsers.Add(empId);
                                    }
                                }
                            }
                            catch (OutOfMemoryException ex)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Retry Perno:{PERNR} {ex.Message}", ex);
                                retryFailedUsers.Add(empId);
                            }
                            catch (Exception ex)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Retry failed Perno:{PERNR}", ex);
                                retryFailedUsers.Add(empId);
                            }
                            
                        }
                        if (retryFailedUsers != null && retryFailedUsers.Count > 0)
                        {
                            idmsLogger.WriteLog(LogLevelType.Info, $"Send email for Retry failure");
                            sendmailDelegate.Invoke(this.endPoint, idmsLogger, "Retry failure for Employees", string.Join("<br/>", retryFailedUsers.Select(i => i)));
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        retryFailedUsers?.Clear();
                        retryFailedUsers = null;
                    }
                }
            }
            catch (Exception ee)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"Exception on Delta Discovery on the date : {deltaDate}", ee);
                throw ee;
            }

            return changeSetPERNRs.Count();
        }


        public SFDiscoveryStatus discoverUserByPERNR(string PERNR, DateTime? deltaDate = null)
        {
            var sfDiscoveryStatus = new SFDiscoveryStatus();
            User diUser = null;
            bool noEventsFound = false;
            if (string.IsNullOrWhiteSpace(PERNR))
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"discoverUserByPERNR: PERNR is Null or Empty");
                sfDiscoveryStatus.Message = $"PERNR is Null or Empty";
                return sfDiscoveryStatus;
            }

            if (!deltaDate.HasValue)
            {
                deltaDate = this.CurrentDate;
            }

            var orgChangeTracks = new List<OrgChangeTrack>();
            try
            {

                idmsLogger.WriteLog(LogLevelType.Info, $"PERNR :{PERNR} for the delta date { (deltaDate.HasValue ? deltaDate : DateTime.Now) } -  Discovery Started");

                SFOData.User sf_User = null;
                try
                {
                    sf_User = this.entityContainer.User.Where(u => u.userId == PERNR.TrimLeadingSeros()).Select(u =>
                     new SFOData.User()
                     {
                         userId = u.userId,
                         status = u.status,
                         state = u.state,
                         username = u.username,
                         manager = u.manager != null ? new SFOData.User()
                         {
                             userId = u.manager.userId,
                             status = u.manager.status,
                             state = u.manager.state
                         } : null,
                     }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    var msg = $"Error occured while reading SFOData.User for PERNR:{PERNR} Exception:{sfDiscoveryStatus.Message}";
                    idmsLogger.WriteLog(LogLevelType.Exception, msg, null);
                    throw new SuccessFactorAPIReadException(msg, ex);
                }

                if (sf_User == null)
                {
                    idmsLogger.WriteLog(LogLevelType.Exception, $"discoverUserByPERNR: Could not find SFOData.User object for the PERNR: {PERNR}", null);
                    sfDiscoveryStatus.Message = $"Could not find SFOData.User object for the PERNR: {PERNR}";
                    return sfDiscoveryStatus;
                }

                #region JobEvent Query

                var empJobsQuery = this.entityContainer.EmpJob
                    .Where(e => e.userId == PERNR.TrimLeadingSeros() &&
                               (e.endDate >= CurrentDate || e.endDate == MaxDate) &&
                               (e.createdDateTime >= deltaDate || e.lastModifiedOn >= deltaDate));

                empJobsQuery = empJobsQuery.Select(e => new SFOData.EmpJob()
                {
                    seqNumber = e.seqNumber,
                    userId = e.userId,
                    startDate = e.startDate,
                    endDate = e.endDate,
                    position = e.position,
                    costCenter = e.costCenter,
                    company = e.company,
                    businessUnit = e.businessUnit,
                    division = e.division,
                    department = e.department,
                    employmentType = e.employmentType,
                    employmentTypeNav = new SFOData.PicklistOption() { externalCode = e.employmentTypeNav.externalCode, id = e.employmentTypeNav.id },
                    employeeClass = e.employeeClass,
                    employeeClassNav = new SFOData.PicklistOption() { externalCode = e.employeeClassNav.externalCode, id = e.employeeClassNav.id },
                    emplStatus = e.emplStatus,
                    emplStatusNav = new SFOData.PicklistOption() { externalCode = e.emplStatusNav.externalCode, id = e.emplStatusNav.id },
                    customString3 = e.customString3,
                    eventReason = e.eventReason,
                    managerId = e.managerId,
                    eventNav = new SFOData.PicklistOption() { id = e.eventNav.id, externalCode = e.eventNav.externalCode },
                    customString2 = e.customString2,
                    customString4 = e.customString4,
                    customString5 = e.customString5,
                    customString6 = e.customString6,
                    customString7 = e.customString7,
                    customString8 = e.customString8,
                    customString9 = e.customString9,
                    localJobTitle = e.localJobTitle,
                    customString24 = e.customString24,
                    customString24Nav = e.customString24Nav != null ? new SFOData.PicklistOption() { externalCode = e.customString24Nav.externalCode, id = e.customString24Nav.id }:null,
                });

                var tempempJobs = empJobsQuery.ToList();
                //Same day if we get multiple event 
                var empJobs = (from j in tempempJobs
                               group j by new { j.userId, j.startDate, j.eventNav.externalCode, j.eventReason } into g
                               select g.OrderByDescending(o => o.seqNumber).Select(gi => gi).FirstOrDefault()).ToList();

                #endregion JobEvent Query

                #region Events
                if (empJobs != null && empJobs.Count > 0)
                {

                    var g = (from job in empJobs group job by new { job.startDate } into gg select gg.ToList());

                    foreach (var grpJobs in g)
                    {
                        var count = 0;
                        foreach (var e in grpJobs.ToList().OrderBy(t => t.seqNumber))
                        {
                            count = count + 1;
                            diUser = null;

                            #region OnBoard

                            var onboardJob = e.eventNav != null && OnBoardTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (onboardJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"OnBoard Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                var userSaved = getUser(sf_User, onboardJob.startDate);
                                if (userSaved > 0)
                                {
                                    diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                    if (diUser != null)
                                        orgChangeTracks.Add(diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.NewJoinee, onboardJob.eventReason, onboardJob.eventNav.externalCode, onboardJob.startDate, onboardJob.endDate, count));
                                }
                                continue;
                            }

                            #endregion OnBoard

                            #region Rejoin

                            var reHireJob = e.eventNav != null && RejoinTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (reHireJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Rejoin Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");
                                diUser = constructDomainUser(sf_User, reHireJob.startDate);

                                if (diUser == null)
                                {
                                    idmsLogger.WriteLog(LogLevelType.Exception, $"Rejoin {PERNR}: Could not generate DomainModel.User object.", null);
                                    sfDiscoveryStatus.Message = $"Rejoin {PERNR}: Could not generate DomainModel.User object.";
                                    return sfDiscoveryStatus;
                                }

                                diUser.TerminationDate = null;

                                if (diUser.Id == 0)
                                {
                                    // No Global user profile found, hence it should be saved 
                                    getOrgInformation(diUser, diUser.EmployeeId.TrimLeadingSeros(), reHireJob.startDate);
                                    var userSaved = saveUser(diUser);

                                    if (userSaved > 0)
                                    {
                                        idmsLogger.WriteLog(LogLevelType.Info, $"Rejoin: Since the PERNR :{diUser.EmployeeId} is not exist. A new User profile has been created.");
                                        var user = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                        if (user != null)
                                        {

                                            var orgChangeTrack = user.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.Rehire, reHireJob.eventReason, reHireJob.eventNav.externalCode, reHireJob.startDate, reHireJob.endDate, count);
                                            var comment = $"Rejoin: Since the PERNR :{orgChangeTrack.EmployeeId} is not exist. A new User profile has been created.";
                                            orgChangeTrack.Comment = comment;
                                            orgChangeTracks.Add(orgChangeTrack);
                                            idmsLogger.WriteLog(LogLevelType.Info, comment);

                                        }
                                    }
                                    else
                                    {
                                        idmsLogger.WriteLog(LogLevelType.Exception, $"Rejoin {PERNR}: Could not save DomainModel.User object.", null);
                                        sfDiscoveryStatus.Message = $"Rejoin {PERNR}: Could not save User";
                                        return sfDiscoveryStatus;
                                    }
                                }
                                else
                                {

                                    var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.Rehire, reHireJob.eventReason, reHireJob.eventNav.externalCode, reHireJob.startDate, reHireJob.endDate);
                                    getOrgInformation(orgChangeTrack, reHireJob);
                                    orgChangeTracks.Add(orgChangeTrack);

                                }

                                continue;
                            }


                            #endregion Rejoin

                            #region Transfer
                            var transferJob = e.eventNav != null && TranferTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (transferJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Transfer Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));

                                if (diUser == null)
                                {
                                    diUser = CreateUserIfNotExist(PERNR, sf_User, transferJob.startDate, "OUChange");

                                }
                                updateDiUser(diUser, sf_User, transferJob.startDate);
                                SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger);
                                var transferWithRetain = string.Empty;
                                if(transferJob.customString24Nav != null && !string.IsNullOrWhiteSpace(transferJob.customString24Nav.externalCode))
                                {
                                    transferWithRetain = transferJob.customString24Nav.externalCode;
                                }
                                var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.OUChange, transferJob.eventReason, transferJob.eventNav.externalCode, transferJob.startDate, transferJob.endDate, count, transferWithRetain);

                                getOrgInformation(orgChangeTrack, transferJob);
                                orgChangeTracks.Add(orgChangeTrack);
                                continue;
                            }

                            #endregion Transfer

                            #region Re-Org 

                            var re_OrgJob = e.eventNav != null && ReOrgTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (re_OrgJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Re-Org Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                if (diUser == null)
                                {
                                    diUser = CreateUserIfNotExist(PERNR, sf_User, re_OrgJob.startDate, "Re-Org");
                                }
                                updateDiUser(diUser, sf_User, re_OrgJob.startDate);
                                SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger);
                                var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.ReOrganisation, re_OrgJob.eventReason, re_OrgJob.eventNav.externalCode, re_OrgJob.startDate, re_OrgJob.endDate, count);
                                getOrgInformation(orgChangeTrack, re_OrgJob);
                                orgChangeTracks.Add(orgChangeTrack);
                                continue;
                            }

                            #endregion Re-Org

                            #region Promotion  
                            // Promotion would be considered and be created as Propagation event trigger  - on update Position Title

                            var promotionJob = e.eventNav != null && PromotionTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (promotionJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Promotion Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                if (diUser == null)
                                {
                                    diUser = CreateUserIfNotExist(PERNR, sf_User, promotionJob.startDate, "Promotion");
                                }
                                updateDiUser(diUser, sf_User, promotionJob.startDate);
                                SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger);
                                var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.Promotion,
                                                                              promotionJob.eventReason,
                                                                              promotionJob.eventNav.externalCode,
                                                                              promotionJob.startDate,
                                                                              promotionJob.endDate, count);
                                getOrgInformation(orgChangeTrack, promotionJob);
                                orgChangeTracks.Add(orgChangeTrack);

                                continue;
                            }

                            #endregion Promotion 

                            #region OffBoard

                            var offBoardJob = e.eventNav != null && OffBoardTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (offBoardJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"OffBoard Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                if (diUser == null)
                                {
                                    diUser = CreateUserIfNotExist(PERNR, sf_User, offBoardJob.startDate, "Off-Board");
                                }
                                updateDiUser(diUser, sf_User, offBoardJob.startDate);

                                // Since Termination date may not available in EmpEmployment in all the scenarios. So setting the termination date if not exist
                                if (!diUser.TerminationDate.HasValue)
                                {
                                    diUser.TerminationDate = offBoardJob.startDate;
                                }
                                SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger);

                                var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.Resignation,
                                                                              offBoardJob.eventReason,
                                                                              offBoardJob.eventNav.externalCode,
                                                                              offBoardJob.startDate,
                                                                              offBoardJob.endDate);
                                getOrgInformation(orgChangeTrack, offBoardJob);
                                orgChangeTracks.Add(orgChangeTrack);

                                continue;
                            }

                            #endregion Propagation

                            #region Propagation

                            var PropagationJob = e.eventNav != null && PropagationTriggers.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) ? e : null;

                            if (PropagationJob != null)
                            {
                                idmsLogger.WriteLog(LogLevelType.Info, $"Propagation Event : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");

                                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                                if (diUser == null)
                                {
                                    diUser = CreateUserIfNotExist(PERNR, sf_User, PropagationJob.startDate, "Propagation");
                                }
                                updateDiUser(diUser, sf_User, PropagationJob.startDate);

                                //SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger,true,PropagationJob);

                                var orgChangeTrack = diUser.GetOrgChangeTrack(Infrastructure.Enums.EmployeeEvents.Propagation,
                                                                              PropagationJob.eventReason,
                                                                              PropagationJob.eventNav.externalCode,
                                                                              PropagationJob.startDate,
                                                                              PropagationJob.endDate, count);
                                getOrgInformation(orgChangeTrack, PropagationJob);
                                orgChangeTracks.Add(orgChangeTrack);

                                continue;
                            }

                            #endregion Propagation

                            #region No Event Found

                            // if no Event change for the PERNR - need to update the User information
                            idmsLogger.WriteLog(LogLevelType.Info, $"Event is not matching with IDMS event configuraiton for the PERNR :{PERNR} for the delta date :{(deltaDate.HasValue ? deltaDate : DateTime.Now)}");

                            idmsLogger.WriteLog(LogLevelType.Info, $"The Event is, : PERNR :[{PERNR}], Event:[{e.eventNav.externalCode}], Event Reason:[{e.eventReason}], Effective Date:[{e.startDate}]");
                            noEventsFound = true;
                            diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                            if (diUser == null)
                            {
                                diUser = CreateUserIfNotExist(PERNR, sf_User, e.startDate, "No Event");
                            }
                            updateDiUser(diUser, sf_User, CurrentDate);
                            getOrgInformation(diUser, e);
                            updateDescription(diUser);
                            SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger, true, e);

                            #endregion No Event Found
                        }

                    }
                }
                else
                {
                    //this.idmsLogger.WriteLog(LogLevelType.Info, $"No Events found for the PERNR :{PERNR} for the delta date :{(deltaDate.HasValue ? deltaDate : DateTime.Now)}");
                    noEventsFound = true;
                    CreateAndSaveUser(PERNR, sf_User);
                }

                #endregion Events

                #region Populate Employee Events

                if (orgChangeTracks != null && orgChangeTracks.Count > 0)
                {
                    // if all triggers are in future dated and current dated data might not get updated.
                    if (noEventsFound == false && !orgChangeTracks.Any(eve => eve.EffectiveDate <= CurrentDate))
                    {
                        CreateAndSaveUser(PERNR, sf_User);
                    }

                    List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();
                    foreach (var orgChangeTrack in orgChangeTracks.OrderBy(t => ((byte)t.Event)).Select(e => e))
                    {
                        EmployeeEventTrigger trigger = PopulateEmployeeEventTrigger(orgChangeTrack,
                            (track) => {

                                if (track.Event == EmployeeEvents.Rehire)
                                {
                                    return orgChangeTracks.Any(o => o.Event == EmployeeEvents.Resignation && o.EffectiveDate <= track.EffectiveDate) || triggers.Any(t => t.EventType == EmployeeEvents.Resignation && t.EffectiveDate <= track.EffectiveDate);
                                }
                                else
                                {
                                    return true;
                                }
                            });
                        if (trigger != null)
                        {
                            this.idmsLogger.WriteLog(LogLevelType.Info, $"Employee Event Trigger created/updated: The detail are, {trigger.ToString()}");
                            triggers.Add(trigger);
                        }
                    }
                    EmployeeEventTrigger.ValidateAndCreateTriggers(triggers, new List<long>(), serviceUser);
                }

                #endregion  Populate Employee Events 

                return sfDiscoveryStatus;
            }
            catch (SuccessFactorAPIReadException ex)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"PERNR:{PERNR}  Delta date { (deltaDate.HasValue ? deltaDate : DateTime.Now) } Error: {ex.Message}", ex);
                //SP they dont want email to be send for such issue.
                sfDiscoveryStatus.SFDiscoveryStatusType = SFDiscoveryStatusType.Success;
                sfDiscoveryStatus.Message = null;
                return sfDiscoveryStatus;
            }
            catch (OutOfMemoryException ex)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"PERNR:{PERNR}  Delta date { (deltaDate.HasValue ? deltaDate : DateTime.Now) } Error: {ex.Message}", ex);
                throw ex;
            }
            catch (Exception ee)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, ee.Message, ee);
                sfDiscoveryStatus.Message = ee.Message;
                return sfDiscoveryStatus;
            }
            finally
            {
                idmsLogger.WriteLog(LogLevelType.Info, $"PERNR :{PERNR} for the delta date { (deltaDate.HasValue ? deltaDate : DateTime.Now) } -  Discovery Ended");
            }
        }

        private void CreateAndSaveUser(string PERNR, SFOData.User sf_User)
        {
            User diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
            if (diUser == null)
            {
                diUser = CreateUserIfNotExist(PERNR, sf_User, this.CurrentDate, "No Event");
            }
            updateDiUser(diUser, sf_User, CurrentDate);
            getOrgInformation(diUser, diUser.EmployeeId.TrimLeadingSeros(), this.CurrentDate);
            updateDescription(diUser);
            SF_Helper.UpdateUserFromDiscovery(diUser, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger, true, null);
        }

        private User CreateUserIfNotExist(string PERNR, SFOData.User sf_User, DateTime effectiveDate, string Event)
        {
            User diUser;
            //throw new Exception($"Could not find DomainModel.User for the PERNR - { PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize)}");
            this.idmsLogger.WriteLog(LogLevelType.Info, $"Could not find DomainModel.User for the PERNR - { PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize)} for {Event} Event.");
            var userSaved = getUser(sf_User, effectiveDate);
            if (userSaved > 0)
            {
                diUser = DomainModel.User.GetByEmployeeId(PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
            }
            else
            {
                throw new Exception($"discoverUserByPERNR: Could not find DomainModel.User for the PERNR - { PERNR.PadWithZeros(SF_Helper.EmployeeIdMaxSize)}");
            }

            return diUser;
        }

        List<string> getDeltaChangeSet(DateTime deltaDate)
        {
            List<string> CompleteList = new List<string>();
            List<string> HiringNotcompletedTrueList = new List<string>();

            try
            {
                // Changeset in EmpJob

                CompleteList = SF_Helper.getPagedData<SFOData.EmpJob>(this.entityContainer.EmpJob.AsQueryable(),
                                                                        (e) => e.createdDateTime >= deltaDate || e.lastModifiedOn >= deltaDate,
                                                                        (e) => new SFOData.EmpJob() { userId = e.userId },
                                                                        (e) => e.userId,
                                                                        CompleteList,
                                                                        this.idmsLogger);

                // Changeset in User
                CompleteList = SF_Helper.getPagedData<SFOData.User>(this.entityContainer.User.AsQueryable(),
                                                                        (e) => e.lastModifiedDateTime >= deltaDate,
                                                                        (e) => new SFOData.User() { userId = e.userId },
                                                                        (e) => e.userId,
                                                                        CompleteList,
                                                                        this.idmsLogger);

                // Changeset in PerPersonal

                CompleteList = SF_Helper.getPagedData<SFOData.PerPersonal>(this.entityContainer.PerPersonal.AsQueryable(),
                                                                       (e) => (e.endDate >= CurrentDate || e.endDate == MaxDate) && e.createdDateTime >= deltaDate || e.lastModifiedOn >= deltaDate,
                                                                       (e) => new SFOData.PerPersonal() { personIdExternal = e.personIdExternal },
                                                                       (e) => e.personIdExternal,
                                                                       CompleteList,
                                                                       this.idmsLogger);

                // Changeset in PerAddressDEFLT

                CompleteList = SF_Helper.getPagedData<SFOData.PerAddressDEFLT>(this.entityContainer.PerAddressDEFLT.AsQueryable(),
                                                                                    (a) => a.addressType == officeAddressType && (a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate),
                                                                                    (e) => new SFOData.PerAddressDEFLT() { personIdExternal = e.personIdExternal },
                                                                                    (a) => a.personIdExternal,
                                                                                    CompleteList,
                                                                                    this.idmsLogger);


                // Changeset in EmpEmployment

                CompleteList = SF_Helper.getPagedData<SFOData.EmpEmployment>(this.entityContainer.EmpEmployment.AsQueryable(),
                                                                                   (a) => a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate,
                                                                                   (e) => new SFOData.EmpEmployment() { personIdExternal = e.personIdExternal },
                                                                                   (a) => a.personIdExternal,
                                                                                   CompleteList,
                                                                                   this.idmsLogger);

                HiringNotcompletedTrueList = SF_Helper.getPagedData<SFOData.EmpEmployment>(this.entityContainer.EmpEmployment.AsQueryable(),
                                                                    (a) => a.hiringNotCompleted == true && (a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate),
                                                                    (e) => new SFOData.EmpEmployment() { personIdExternal = e.personIdExternal },
                                                                    (a) => a.personIdExternal,
                                                                    HiringNotcompletedTrueList,
                                                                    this.idmsLogger);

                // Changeset in EmpEmploymentTermination

                CompleteList = SF_Helper.getPagedData<SFOData.EmpEmploymentTermination>(this.entityContainer.EmpEmploymentTermination.AsQueryable(),
                                                                                   (a) => a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate,
                                                                                   (e) => new SFOData.EmpEmploymentTermination() { personIdExternal = e.personIdExternal },
                                                                                   (a) => a.personIdExternal,
                                                                                   CompleteList,
                                                                                   this.idmsLogger);

                // Changeset in PerPhone

                CompleteList = SF_Helper.getPagedData<SFOData.PerPhone>(this.entityContainer.PerPhone.AsQueryable(),
                                                                                  (a) => a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate,
                                                                                  (e) => new SFOData.PerPhone() { personIdExternal = e.personIdExternal },
                                                                                  (a) => a.personIdExternal,
                                                                                  CompleteList,
                                                                                  this.idmsLogger);

                // Changeset in PerEmail
                CompleteList = SF_Helper.getPagedData<SFOData.PerEmail>(this.entityContainer.PerEmail.AsQueryable(),
                                                                                 (a) => a.createdDateTime >= deltaDate || a.lastModifiedOn >= deltaDate,
                                                                                 (e) => new SFOData.PerEmail() { personIdExternal = e.personIdExternal },
                                                                                 (a) => a.personIdExternal,
                                                                                 CompleteList,
                                                                                 this.idmsLogger);

                if (MasterConfigHelper.ExcludeHiringNotCompleted)
                {
                    string skippedIds = string.Join(", ", HiringNotcompletedTrueList);
                    this.idmsLogger.WriteLog(LogLevelType.Info, $"Flag ExcludeHiringNotCompleted is true. Skipped Users: {skippedIds}");

                    CompleteList = CompleteList.Except(HiringNotcompletedTrueList).ToList();
                }

                return new HashSet<string>(CompleteList).ToList();
            }
            catch (Exception ee)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"Exception in generating DeltaChangeSet for the date - {deltaDate}", ee);
                throw ee;
            }
            finally
            {
                CompleteList.Clear();
                this.entityContainer.ClearPendingChanges();
            }
        }


        #endregion Delta Discovery

        #region Sync Method

        public void SyncTelephoneDirectory()
        {
            List<User> users = new List<User>();
            try
            {
                idmsLogger.WriteLog(LogLevelType.Info, "Sync Telphone Dir started for Pernr ");

               users = User.GetActiveFullTimeEmployees();
                var propagationFields = MasterConfigHelper.PropagationFields;
                bool canRunPropagationTrigger = MasterConfigHelper.CanRunPropagationTrigger;

                foreach (User usr in users)
                {
                    if (usr.FullTimeEmployee)
                    {
                        try
                        {
                            var empId = usr.EmployeeId.TrimLeadingSeros();
                            getUserContactInfo(usr, empId);
                            getOrgInformation(usr, empId, CurrentDate);
                            getUserOfficeAddress(usr, empId, CurrentDate);

                            SF_Helper.UpdateUserFromDiscovery(usr, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger);
                        }
                        catch (Exception ex)
                        {
                            idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while synchronising data to telephone directory for the user - {0}, Pernr - {1}.", usr.KnownAs, usr.EmployeeId), ex);
                        }
                    }

                }
                idmsLogger.WriteLog(LogLevelType.Info, "Completed preparing sync data for telephone directory.");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "Error while synchronising data to telephone directory.", ex);
            }
            finally
            {
                users?.Clear();
                users = null;
            }
        }

        #endregion Sync Method

        #region Discovery methods

        User constructDomainUser(SFOData.User user, DateTime effectiveStartDate)
        {
            DomainModel.User diUser = null;
            try
            {
                if (user == null)
                {
                    idmsLogger.WriteLog(LogLevelType.Exception, "SFOData.User is Null", null);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(user.userId))
                {
                    idmsLogger.WriteLog(LogLevelType.Exception, "SFOData.User.userId is Null or Empty", null);
                    return null;
                }

                var pernr = user.userId.PadWithZeros(SF_Helper.EmployeeIdMaxSize);
                diUser = DomainModel.User.GetByEmployeeId(pernr);

                if (diUser == null)
                {
                    this.idmsLogger.WriteLog(LogLevelType.Info, $"DomainModel.User new employee creation for the PERNR - {pernr}");
                    diUser = new User();
                    diUser.EndPointSystemId = this.endPoint.Id;
                    diUser.EmployeeType = EmployeeType.Permanent;
                    //if (!string.IsNullOrWhiteSpace(user.username))
                    //{
                    //    diUser.UserName = user.username;
                    //}
                    //else
                    //{
                    diUser.UserName = pernr;
                    //}
                    diUser.EmployeeId = pernr;
                    diUser.AuthenticationMode = AuthenticationMode;
                    diUser.IdentificationNo = pernr;
                    diUser.IsActive = true;
                    diUser.Reserved4 = Constants.EmployeeStatus.NewEmployee;
                    diUser.Password = getPassword();
                }
                //diUser.UserName = user.username;
                updateDiUser(diUser, user, effectiveStartDate);
            }
            catch (Exception ee)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"Exception in generating DomainModel.User for the PERNR - {diUser.EmployeeId}", ee);
                diUser = null;
            }

            return diUser;
        }

        void updateDiUser(DomainModel.User diUser, SFOData.User user, DateTime effectiveStartDate)
        {
            diUser.FullTimeEmployee = true;
            diUser.IsActive = string.IsNullOrWhiteSpace(user.status) == false && user.status.Equals("f", StringComparison.InvariantCultureIgnoreCase) == false;
            //diUser.ROEmployeeId = user.manager != null ? user.manager.userId.PadWithZeros(8) : null; 
            var empID = diUser.EmployeeId.TrimLeadingSeros();


            var persons = this.entityContainer.PerPerson.Where(p => p.personIdExternal == empID).Select(e => new SFOData.PerPerson() { personIdExternal = e.personIdExternal }).ToList();

            if (persons == null || persons.Count() == 0)
            {
                var s = $"Could not find PerPerson in SF HRMS for the PERNR - {diUser.EmployeeId}";
                this.idmsLogger.WriteLog(LogLevelType.Error, s);
                throw new Exception(s);
            }

            getUserInfo(diUser, empID, effectiveStartDate);

        }

        void getUserInfo(DomainModel.User diUser, string empID, DateTime effectiveStartDate)
        {
            try
            {
                getUserPersonalInfo(diUser, empID, effectiveStartDate, null);
                getUserOfficeAddress(diUser, empID, effectiveStartDate);
                getUserEmployeementInfo(diUser, empID);
                getUserContactInfo(diUser, empID);
                getUserEmailInfo(diUser, empID);
            }
            catch (Exception ee)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"Exception in getUserInfo for the PERNR - {diUser.EmployeeId}", ee);
                throw ee;
            }

        }

        void getUserEmployeementInfo(User diUser, string employeeId)
        {
            var query = this.entityContainer.EmpEmployment.Where(p => p.personIdExternal == employeeId && p.userId == employeeId).OrderByDescending(p => p.lastModifiedOn)
                               .Select(p =>
                                        new SFOData.EmpEmployment()
                                        {
                                            personIdExternal = p.personIdExternal,
                                            startDate = p.startDate,
                                            endDate = p.endDate,
                                            lastModifiedOn = p.lastModifiedOn
                                        });

            var emp_info = query.FirstOrDefault();

            if (emp_info != null)
            {
                if (emp_info.startDate.HasValue)
                {
                    diUser.JoinDate = emp_info.startDate;
                    // this.idmsLogger.WriteLog(LogLevelType.Error, $"EmpEmployment: JoinDate of PERNR [{diUser.EmployeeId}] is {emp_info.startDate}.");
                    var usrTemp = User.GetByEmployeeId(diUser.EmployeeId);
                    if (diUser.IsActive == false && usrTemp != null && usrTemp.IsActive == true && (usrTemp.Reserved4 == Constants.EmployeeStatus.OnBoarded || usrTemp.Reserved4 == Constants.EmployeeStatus.NewEmployee)
                        && diUser.JoinDate.HasValue && diUser.JoinDate.Value >= this.CurrentDate)
                    {
                        diUser.IsActive = true;
                    }
                    //If user offboarded by IAM through Trigger page, but Employee last date will be after N day for financial closing period. SF retian still active. The below code to prevent the user activation.
                    else if (diUser.IsActive == true && usrTemp != null && usrTemp.IsActive == false && (usrTemp.Reserved4 == Constants.EmployeeStatus.OffBoarded)
                        && usrTemp.TerminationDate.HasValue && usrTemp.JoinDate.HasValue && usrTemp.JoinDate.Value < this.CurrentDate && diUser.JoinDate.HasValue && diUser.JoinDate == usrTemp.JoinDate.Value)
                    {
                        diUser.IsActive = false;
                    }
                }

                if (emp_info.endDate.HasValue)
                {
                    diUser.TerminationDate = emp_info.endDate;
                    this.idmsLogger.WriteLog(LogLevelType.Error, $"EmpEmployment: TerminationDate of PERNR [{diUser.EmployeeId}] is {emp_info.endDate}.");
                }
                else
                {
                    diUser.TerminationDate = null;
                }
            }

            if (!diUser.JoinDate.HasValue)
            {
                this.idmsLogger.WriteLog(LogLevelType.Info, $"Could not find record in EmpEmployment for PERNR - {diUser.EmployeeId}. Hence JoinDate is not set.");
            }



            //var terminationquery = this.entityContainer.EmpEmploymentTermination.Where(p => p.personIdExternal == employeeId && p.userId == employeeId).OrderByDescending(p => p.lastModifiedOn)
            //                   .Select(p =>
            //                            new SFOData.EmpEmploymentTermination()
            //                            {
            //                                personIdExternal = p.personIdExternal,      
            //                                endDate = p.endDate,
            //                                lastModifiedOn = p.lastModifiedOn
            //                            });

            //var emp_termination_info = terminationquery.FirstOrDefault();

            //if (emp_termination_info != null)
            //{
            //    diUser.TerminationDate = emp_termination_info.endDate;
            //    this.idmsLogger.WriteLog(LogLevelType.Error, $"Termination Date for PERNR:[{diUser.EmployeeId}] is {diUser.TerminationDate}");
            //}            
        }

        void getUserContactInfo(User diUser, string employeeId)
        {
            var off_phoneType = PhoneTypeForTelephone;
            SFOData.PickListValueV2 officePhoneType = GetOfficePhoneType();

            if (officePhoneType == null || officePhoneType.optionId.HasValue == false)
            {
                idmsLogger.WriteLog(LogLevelType.Error, "Could not find officePhoneType.");
            }
            else
            {
                off_phoneType = officePhoneType.optionId.Value.ToString();
            }



            var phoneList = this.entityContainer.PerPhone.Where(p => p.personIdExternal == employeeId)
                                          .Select(p => new SFOData.PerPhone() { isPrimary = p.isPrimary, phoneType = p.phoneType, phoneNumber = p.phoneNumber }).ToList();

            if (phoneList != null)
            {

                var phone_info = phoneList.Where(e => e.phoneType == off_phoneType)
                                          .Select(e => e).FirstOrDefault();

                if (phone_info != null)
                {

                    diUser.TelephoneNumber = phone_info.phoneNumber;
                }
                else
                {
                    diUser.TelephoneNumber = string.Empty;
                }
                phone_info = phoneList.Where(e => e.phoneType == PhoneTypeForMobile)
                                      .Select(e => e).FirstOrDefault();

                if (phone_info != null)
                {

                    diUser.MobileNumber = phone_info.phoneNumber;
                }
                else
                {
                    diUser.MobileNumber = string.Empty;
                }
            }
        }

        void getUserEmailInfo(User diUser, string employeeId)
        {
            SFOData.PickListValueV2 emailType = GetEmailType();

            if (emailType == null || emailType.optionId.HasValue == false)
            {
                idmsLogger.WriteLog(LogLevelType.Error, "Could not find EmailType for Business Email.");
                return;
            }

            var query = this.entityContainer.PerEmail
                          .Where(e => e.personIdExternal == employeeId)
                          .Select(e =>
                              new SFOData.PerEmail()
                              {
                                  personIdExternal = e.personIdExternal,
                                  emailAddress = e.emailAddress,
                                  isPrimary = e.isPrimary,
                                  emailType = e.emailType
                              });

            var emails = query.ToList();
            if (emails != null && emails.Count > 0)
            {
                var email_info = emails.FirstOrDefault(e => e.emailType == emailType.optionId.Value.ToString());

                if (email_info != null)
                {
                    diUser.EmailId = email_info.emailAddress;
                }
            }
        }

        [CanBeNull]
        public SFOData.PickListValueV2 GetEmailType()
        {
            if (emailType == null)
            {
                emailType = this.entityContainer.PickListValueV2.Where(e => e.PickListV2_id == "ecEmailType" && e.externalCode == "B")
                                                        .Select(e => new SFOData.PickListValueV2()
                                                        {
                                                            PickListV2_id = e.PickListV2_id,
                                                            externalCode = e.externalCode,
                                                            optionId = e.optionId,
                                                            label_defaultValue = e.label_defaultValue
                                                        }).ToList().FirstOrDefault();
            }
            return emailType;
        }

        [CanBeNull]
        public SFOData.PickListValueV2 GetOfficePhoneType()
        {
            if (officePhoneType == null)
            {
                officePhoneType = this.entityContainer.PickListValueV2.Where(e => e.PickListV2_id == "ecPhoneType" && e.externalCode == "Office")
                                                    .Select(e => new SFOData.PickListValueV2()
                                                    {
                                                        PickListV2_id = e.PickListV2_id,
                                                        externalCode = e.externalCode,
                                                        optionId = e.optionId,
                                                        label_defaultValue = e.label_defaultValue
                                                    }).ToList().FirstOrDefault();
            }
            return officePhoneType;
        }


        public string GetEthnicity(long ethnicGroupCode)
        {
            var ethnicLabel = ethnicityGroups.FirstOrDefault(g => g.GroupCode == ethnicGroupCode);
            if (ethnicLabel == null)
            {
                var ethnicPicklistLabel = this.entityContainer.PicklistLabel.Where(e => e.optionId == ethnicGroupCode)
                                                      .Select(e => new SFOData.PicklistLabel()
                                                      {
                                                          optionId = e.optionId,
                                                          locale = e.locale,
                                                          label = e.label
                                                      }).ToList().FirstOrDefault();

                ethnicLabel = new EthnicityGroup()
                {
                    GroupCode = ethnicPicklistLabel.optionId,
                    Label = ethnicPicklistLabel.label,
                    Locale = ethnicPicklistLabel.locale
                };

            }
            if (ethnicLabel != null)
            {
                if (!ethnicityGroups.Any(g => g.GroupCode == ethnicLabel.GroupCode))
                {
                    ethnicityGroups.Add(ethnicLabel);
                }
                return ethnicLabel.Label;
            }
            else
            {
                return string.Empty;
            }
        }

        void getUserPersonalInfo(User diUser, string employeeId, DateTime effectiveStartDate, DateTime? effectiveEndDate)
        {
            var query = this.entityContainer.PerPersonal.Where(p => p.personIdExternal == employeeId &&
                                                                    p.startDate <= effectiveStartDate &&
                                                                    (p.endDate >= CurrentDate || p.endDate == MaxDate)
                                                              )
                                            .OrderByDescending(p => p.startDate)
                                .Select(p =>
                                         new SFOData.PerPersonal()
                                         {
                                             personIdExternal = p.personIdExternal,
                                             startDate = p.startDate,
                                             endDate = p.endDate,
                                             lastName = p.lastName,
                                             firstName = p.firstName,
                                             formalName = p.formalName,
                                             preferredName = p.preferredName,
                                             ethnicGroup = p.ethnicGroup,
                                             lastModifiedOn = p.lastModifiedOn
                                         });

            if (effectiveEndDate.HasValue)
            {
                var dt = new DateTime(effectiveEndDate.Value.Year, effectiveEndDate.Value.Month, effectiveEndDate.Value.Day);
                query = query.Where(p => p.endDate <= dt);
            }

            var p_info = query.FirstOrDefault();
            if (p_info != null)
            {
                diUser.LastName = p_info.lastName;
                diUser.FirstName = p_info.firstName;
                diUser.KnownAs = diUser.DisplayName = p_info.preferredName;
                //diUser.DisplayName = p_info.formalName;

                if (!string.IsNullOrWhiteSpace(p_info.ethnicGroup) && long.TryParse(p_info.ethnicGroup, out long ethnicCode))
                {
                    diUser.Ethinicity = GetEthnicity(ethnicCode);
                }
            }
            else
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"Could not find record in EmpEmployment for PERNR - {diUser.EmployeeId}");
            }
        }

        void getUserOfficeAddress(User diUser, string employeeId, DateTime effecctiveStartDate)
        {
            addressBuilder.Clear();
            addressBuilder.Length = 0;
            var query = this.entityContainer.PerAddressDEFLT
                                   .Where(e => e.addressType == officeAddressType
                                   && e.personIdExternal == employeeId
                                   && e.startDate <= effecctiveStartDate
                                   && (e.endDate >= CurrentDate || e.endDate == MaxDate)).OrderByDescending(o => o.startDate)
                                   .Select(e =>
                                          new SFOData.PerAddressDEFLT()
                                          {
                                              address1 = e.address1,
                                              address2 = e.address2,
                                              address3 = e.address3,
                                              address4 = e.address4,
                                              address5 = e.address5,
                                              zipCode = e.zipCode,
                                              country = e.country,
                                              countryNav = new SFOData.Territory()
                                              {
                                                  territoryCode = e.countryNav.territoryCode,
                                                  territoryName = e.countryNav.territoryName
                                              },
                                              city = e.city,
                                              addressType = e.addressType,
                                              endDate = e.endDate
                                          });

            var addresses = query.ToList();

            if (addresses != null)
            {
                var address = addresses.FirstOrDefault();

                if (address != null)
                {

                    if (!string.IsNullOrWhiteSpace(address.address1))
                        addressBuilder.Append($"{address.address1}, ");

                    if (!string.IsNullOrWhiteSpace(address.address2))
                        addressBuilder.Append($"{address.address2}, ");

                    if (!string.IsNullOrWhiteSpace(address.address3))
                        addressBuilder.Append($"{address.address3}, ");

                    if (!string.IsNullOrWhiteSpace(address.address4))
                        addressBuilder.Append($"{address.address4}, ");

                    if (!string.IsNullOrWhiteSpace(address.address5))
                        addressBuilder.Append(address.address5);


                    diUser.PostalAddress = addressBuilder.ToString();
                    if (address.countryNav != null)
                        diUser.Country = address.countryNav.territoryName;
                    diUser.PostalCode = address.zipCode;
                }
            }
        }

        void getOrgInformation(IOrgChangeTrack diUser, string empID, DateTime effectiveStartDate)
        {

            var query = entityContainer.EmpJob
                .Where(e => e.userId == empID &&
                           e.startDate <= effectiveStartDate &&
                           (e.endDate >= CurrentDate || e.endDate == MaxDate)
                       )
                .OrderByDescending(o => o.startDate)
                .Select(e =>
                new SFOData.EmpJob()
                {
                    seqNumber = e.seqNumber,
                    userId = e.userId,
                    startDate = e.startDate,
                    endDate = e.endDate,
                    position = e.position,
                    costCenter = e.costCenter,
                    company = e.company,
                    businessUnit = e.businessUnit,
                    division = e.division,
                    department = e.department,
                    employmentType = e.employmentType,
                    employmentTypeNav = new SFOData.PicklistOption() { externalCode = e.employmentTypeNav.externalCode, id = e.employmentTypeNav.id },
                    employeeClass = e.employeeClass,
                    employeeClassNav = new SFOData.PicklistOption() { externalCode = e.employeeClassNav.externalCode, id = e.employeeClassNav.id },
                    emplStatus = e.emplStatus,
                    emplStatusNav = new SFOData.PicklistOption() { externalCode = e.emplStatusNav.externalCode, id = e.emplStatusNav.id },
                    customString3 = e.customString3,
                    managerId = e.managerId,
                    eventReason = e.eventReason,
                    eventNav = new SFOData.PicklistOption() { id = e.eventNav.id, externalCode = e.eventNav.externalCode },
                    customString2 = e.customString2,
                    customString4 = e.customString4,
                    customString5 = e.customString5,
                    customString6 = e.customString6,
                    customString7 = e.customString7,
                    customString8 = e.customString8,
                    customString9 = e.customString9,
                    localJobTitle = e.localJobTitle,
                    createdDateTime = e.createdDateTime,
                    lastModifiedOn = e.lastModifiedOn
                });

            var tempempJobs = query.ToList();

            var empinfos = (from j in tempempJobs
                           group j by new { j.userId, j.startDate, j.eventNav.externalCode, j.eventReason } into g
                           select g.OrderByDescending(o => o.seqNumber).Select(gi => gi).FirstOrDefault()).ToList();

            if (empinfos == null || empinfos.Count() == 0)
            {
                this.idmsLogger.WriteLog(LogLevelType.Error, $"Could not find record in EmpJob for PERNR - {diUser.EmployeeId}");
                return;
            }
            var job_info = empinfos.OrderByDescending(e => e.lastModifiedOn).FirstOrDefault();

            getOrgInformation(diUser, job_info);
        }


        void getOrgInformation(IOrgChangeTrack diUser, SFOData.EmpJob job_info)
        {
            if (job_info != null)
            {
                diUser.EEGroup = job_info.employmentTypeNav.externalCode;

                diUser.IsExecutiveEmployee = ExecutiveEmployeeFlags.Contains(diUser.EEGroup) ? true : false;

                diUser.EmployeeStatus = job_info.emplStatusNav.externalCode;


                diUser.IsActive = ActiveEmployeeFlags.Contains(diUser.EmployeeStatus) ? true : false;

                var usrTemp = User.GetByEmployeeId(diUser.EmployeeId);
                if (diUser.IsActive == true && usrTemp != null && usrTemp.IsActive == false && (usrTemp.Reserved4 == Constants.EmployeeStatus.OffBoarded)
                        && usrTemp.TerminationDate.HasValue && usrTemp.JoinDate.HasValue && usrTemp.JoinDate.Value < this.CurrentDate && diUser.JoinDate.HasValue && diUser.JoinDate == usrTemp.JoinDate.Value)
                {
                    diUser.IsActive = false;
                }else if(diUser.IsActive == false && usrTemp != null && usrTemp.IsActive == true && (usrTemp.Reserved4 == Constants.EmployeeStatus.OnBoarded)
                        && usrTemp.TerminationDate.HasValue == false && usrTemp.JoinDate.HasValue && usrTemp.JoinDate.Value >= this.CurrentDate && diUser.JoinDate.HasValue && diUser.JoinDate == usrTemp.JoinDate.Value)
                {
                    diUser.IsActive = true;
                }
                diUser.PositionCode = job_info.position;
                //diUser.PositionName = job_info.localJobTitle;

                diUser.CostCenter = job_info.costCenter;
                diUser.CompanyCode = job_info.company;
                diUser.DepartmentCode = string.IsNullOrWhiteSpace(job_info.businessUnit) ? job_info.businessUnit : job_info.businessUnit.PadWithZeros(8);
                diUser.BranchCode = string.IsNullOrWhiteSpace(job_info.division) ? job_info.division : job_info.division.PadWithZeros(8);
                diUser.SectionCode = string.IsNullOrWhiteSpace(job_info.department) ? job_info.department : job_info.department.PadWithZeros(8);
                diUser.UnitCode = string.IsNullOrWhiteSpace(job_info.customString3) ? job_info.customString3 : job_info.customString3.PadWithZeros(8);
                diUser.ROEmployeeId = job_info.managerId.PadWithZeros(8);
            }

            updateDescription(diUser);

        }

        void updateDescription(IOrgChangeTrack diUser)
        {
            NOrganisation org = null;

            if (!string.IsNullOrWhiteSpace(diUser.PositionCode))
            {
                var position = positions.FirstOrDefault(p => p.Code == diUser.PositionCode);
                if (position != null)
                {
                    diUser.PositionName = position.Description;
                }
                else
                {
                    diUser.PositionName = null;
                }
            }
            else
            {
                diUser.PositionName = null;
            }

            if (!string.IsNullOrWhiteSpace(diUser.CostCenter))
            {
                var costcenter = costCenters.FirstOrDefault(c => c.Code == diUser.CostCenter);
                if (costcenter != null)
                {
                    diUser.CostCenterText = costcenter.Description;
                }
                else
                {
                    diUser.CostCenterText = null;
                }
            }
            else
            {
                diUser.CostCenterText = null;
            }


            if (!string.IsNullOrWhiteSpace(diUser.CompanyCode))
            {
                var company = companies.FirstOrDefault(c => c.Code == diUser.CompanyCode);

                if (company != null)
                {
                    diUser.OrgCompany = diUser.CompanyName = company.Text;
                    diUser.CompanyId = company.Id;
                    diUser.OrganisationCode = diUser.CompanyCode;
                }
                else
                {
                    diUser.OrgCompany = diUser.CompanyName = null;
                    diUser.CompanyId = 0;
                }
            }
            else
            {
                diUser.OrgCompany = diUser.CompanyName = null;
                diUser.CompanyId = 0;
            }

            if (!string.IsNullOrWhiteSpace(diUser.DepartmentCode))
            {
                org = SF_Helper.GetOrganisation(organisations, diUser.DepartmentCode);

                if (org != null)
                {
                    diUser.OrgDepartment = diUser.DepartmentName = org.Name;
                    diUser.DepartmentId = org.Id;
                    diUser.OrganisationCode = diUser.DepartmentCode;
                }
                else
                {
                    diUser.OrgDepartment = diUser.DepartmentName = null;
                    diUser.DepartmentId = 0;
                }
            }
            else
            {
                diUser.OrgDepartment = diUser.DepartmentName = null;
                diUser.DepartmentId = 0;
            }

            if (!string.IsNullOrWhiteSpace(diUser.BranchCode))
            {
                org = SF_Helper.GetOrganisation(organisations, diUser.BranchCode);

                if (org != null)
                {
                    diUser.OrgBranch = org.Name;
                    diUser.OrganisationCode = diUser.BranchCode;
                }
                else
                {
                    diUser.OrgBranch = null;
                }
            }
            else
            {
                diUser.OrgBranch = null;
            }

            if (!string.IsNullOrWhiteSpace(diUser.SectionCode))
            {
                org = SF_Helper.GetOrganisation(organisations, diUser.SectionCode);

                if (org != null)
                {
                    diUser.OrgSection = diUser.SectionName = org.Name;
                    diUser.OrganisationCode = diUser.SectionCode;
                }
                else
                {
                    diUser.OrgSection = diUser.SectionName = null;
                }
            }
            else
            {
                diUser.OrgSection = diUser.SectionName = null;
            }

            if (!string.IsNullOrWhiteSpace(diUser.UnitCode))
            {
                org = SF_Helper.GetOrganisation(organisations, diUser.UnitCode);

                if (org != null)
                {
                    diUser.OrgUnit = org.Name;
                    diUser.OrganisationCode = diUser.UnitCode;
                }
                else
                {
                    diUser.OrgUnit = null;
                }
            }
            else
            {
                diUser.OrgUnit = null;
            }
        }

        #endregion Discovery methods

        #region Helper Methods

        public DataTable GetDataTableInfo(string detail, string pernr)
        {
            string PERNR = pernr.TrimLeadingSeros();
            DataTable dt = new DataTable();
            switch (detail)
            {
                case "Address Details":
                    dt = SF_Helper.getAddressDetails(this.entityContainer, detail, PERNR, this.officeAddressType);
                    break;
                case "Contact Details":
                    dt = SF_Helper.getPhoneDetails(this.entityContainer, detail, PERNR);
                    break;
                case "Email Details":
                    dt = SF_Helper.getEmailTable(this.entityContainer, detail, PERNR);
                    break;
                case "Personal Details":
                    dt = SF_Helper.getPersonalInfo(this.entityContainer, detail, PERNR);
                    break;
                case "Employeement Details":
                    dt = SF_Helper.getEmploymentInfo(this.entityContainer, detail, PERNR);
                    break;
                case "Event Details":
                    dt = SF_Helper.getEmploymentEvents(this.entityContainer, detail, PERNR);
                    break;
            }

            if (dt != null && dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }


            return dt;
        }




        string getPassword()
        {
            if (!string.IsNullOrWhiteSpace(_password)) return _password;

            if (iacmPolicy == null) return null;

            return RSAHelper.EncryptData(new PasswordResolver().ConstructGeneralUserPassword
                                    (iacmPolicy.PasswordLength.HasValue ? iacmPolicy.PasswordLength.Value : 8,
                                    iacmPolicy.NoOfSpecialCharacters.HasValue ? iacmPolicy.NoOfSpecialCharacters.Value : 0,
                                    iacmPolicy.NoOfLowerCaseAlphabets.HasValue ? iacmPolicy.NoOfLowerCaseAlphabets.Value : 0,
                                    iacmPolicy.NoOfUpperCaseAlphabets.HasValue ? iacmPolicy.NoOfUpperCaseAlphabets.Value : 0,
                                    iacmPolicy.NoOfNumericCharacters.HasValue ? iacmPolicy.NoOfNumericCharacters.Value : 0));
        }

        [CanBeNull]
        public EmployeeEventTrigger PopulateEmployeeEventTrigger(OrgChangeTrack orgChangeTrack, Func<OrgChangeTrack, bool> canProceed)
        {

            if (canCreateEmployeeEvent(orgChangeTrack) == false) return null;

            var oldUser = orgChangeTrack.GetOldUser();
            var user = orgChangeTrack.GetNewUser();
            if (oldUser == null)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: Existing profile for the PERNR :{orgChangeTrack.EmployeeId} is not exist.");
                return null;
            }

            if (user == null)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: A new profile for the PERNR :{orgChangeTrack.EmployeeId} is not exist.");
                return null;
            }

            if (!user.JoinDate.HasValue)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: Join Date for the user - {orgChangeTrack.EmployeeId} is Null or Empty");
                return null;
            }

            DateTime triggerEffectiveDate = orgChangeTrack.EffectiveDate;
            if (orgChangeTrack.Event == EmployeeEvents.NewJoinee || orgChangeTrack.Event == EmployeeEvents.Rehire)
            {
                if (!string.IsNullOrWhiteSpace(oldUser.Reserved4) &&
                    oldUser.Reserved4.Equals(Constants.EmployeeStatus.OnBoarded, StringComparison.OrdinalIgnoreCase))
                {
                    if (orgChangeTrack.Event == EmployeeEvents.NewJoinee)
                    {
                        idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: The user - {orgChangeTrack.EmployeeId} is Active and already Onboarded. Hence cannot created OnBoard event.");
                        return null;
                    }

                    if (orgChangeTrack.Event == EmployeeEvents.Rehire)
                    {
                        var offboardtriggers = EmployeeEventTrigger.GetAllEventsByEmployeeIdAndEventType(orgChangeTrack.EmployeeId, EmployeeEvents.Resignation);


                        if (!(canProceed(orgChangeTrack) == true || (offboardtriggers != null && offboardtriggers.Count > 0 && offboardtriggers.Any(o => o.IsActive && o.IsRequestCreated == false && o.EffectiveDate <= orgChangeTrack.EffectiveDate)) == true))
                        {
                            idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: The user - {orgChangeTrack.EmployeeId} is Active, not offboarded and no Offboard trigger found. Hence cannot created Rehire event.");
                            return null;
                        }
                    }
                }

                DateTime priorBoardingDate = user.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));

                if (priorBoardingDate >= DateTime.Today)
                {
                    triggerEffectiveDate = priorBoardingDate;
                }
                else
                {
                    triggerEffectiveDate = DateTime.Today;
                }
            }

            EmployeeEventTrigger eventtrigger = null;
            // Since propagation can be multiple on the same day. It always create a trigger.
            if (orgChangeTrack.Event == EmployeeEvents.Propagation)
            {
                ///We cannot validate againt IsrequestrCreated and Isactive because during Trigger verification if trigger is not valid we mark isActive = false. Hence we check Effective Date is less that
                var  existingTrigger = EmployeeEventTrigger.GetOldByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate); 

                if(existingTrigger != null && orgChangeTrack.EffectiveDate < DateTime.Now)
                {
                    idmsLogger.WriteLog(LogLevelType.Info, $"Employee:{orgChangeTrack.EmployeeId} - Past Dated Event cannot be activated.An employee event trigger for {existingTrigger.EventType.ToString()} TriggerId:{existingTrigger.Id}");
                    return null;
                }
                
                var eventtriggers = EmployeeEventTrigger.GetAllByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate);
                eventtrigger = eventtriggers.FirstOrDefault(e => {
                    if (string.IsNullOrWhiteSpace(e.Comments)) return false;
                    var commentObj = EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(e.Comments);
                    if (commentObj == null) return false;

                    return commentObj.EventCode == orgChangeTrack.EventCode && commentObj.EventReason == orgChangeTrack.EventReason && commentObj.EffectiveDate == orgChangeTrack.EffectiveDate && commentObj.EmployeeId == orgChangeTrack.EmployeeId;
                });

            }
            else
            {
                if (orgChangeTrack.Event == EmployeeEvents.Promotion)
                {
                    var existingTrigger = EmployeeEventTrigger.GetOldByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate);

                    if (existingTrigger != null && orgChangeTrack.EffectiveDate < DateTime.Now && existingTrigger.IsRequestCreated)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"Employee:{orgChangeTrack.EmployeeId} - Past Dated Event cannot be activated.An employee event trigger for {existingTrigger.EventType.ToString()} TriggerId:{existingTrigger.Id}");
                        return null;
                    }

                    var eventtriggers = EmployeeEventTrigger.GetAllByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate);
                    eventtrigger = eventtriggers.FirstOrDefault(e => {
                        if (string.IsNullOrWhiteSpace(e.Comments)) return false;
                        var commentObj = EmployeeEventTriggerComment.GetEmployeeEventTriggerComment(e.Comments);
                        if (commentObj == null) return false;

                        return commentObj.EventCode == orgChangeTrack.EventCode && commentObj.EventReason == orgChangeTrack.EventReason && commentObj.EffectiveDate == orgChangeTrack.EffectiveDate && commentObj.EmployeeId == orgChangeTrack.EmployeeId;
                    });
                }
                else
                {
                    eventtrigger = EmployeeEventTrigger.GetByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate);
                }
            }


            if (eventtrigger == null)
            {
                eventtrigger = new EmployeeEventTrigger
                {
                    CreatedBy = "System",
                    CreatedDate = DateTime.Now
                };
            }
            

            eventtrigger.ActualEventDate = orgChangeTrack.EffectiveDate;
            eventtrigger.LastModifiedBy = "System";
            eventtrigger.LastModifiedDate = DateTime.Now;
            eventtrigger.EventType = orgChangeTrack.Event;
            eventtrigger.UserId = oldUser.Id;
            if (eventtrigger.EffectiveDate != DateTime.MinValue)
                eventtrigger.OldEffectiveDate = eventtrigger.EffectiveDate;
            //Order of execution for Reorg, propagation we should delay the effective date execution. So that provisining engine will pickup the event as per sequence
            eventtrigger.EffectiveDate = triggerEffectiveDate;
            eventtrigger.EventSequence = orgChangeTrack.EventSequence;
            eventtrigger.IsActive = true;
            eventtrigger.Comments = orgChangeTrack.GetComment();
            switch (orgChangeTrack.Event)
            {
                case EmployeeEvents.NewJoinee:
                case EmployeeEvents.Rehire:
                    {

                        if (orgChangeTrack.Event == EmployeeEvents.Rehire)
                        {
                            eventtrigger.UserDetails = UtilityMethods.Serialize<User>(user);
                        }

                        break;
                    }

                case EmployeeEvents.OUChange:
                case EmployeeEvents.ReOrganisation:
                case EmployeeEvents.Promotion:
                case EmployeeEvents.Propagation:
                    {
                        eventtrigger.UserDetails = UtilityMethods.Serialize<DomainModel.User>(user);
                        eventtrigger.OldUserDetails = UtilityMethods.Serialize<DomainModel.User>(oldUser);

                        break;
                    }

                case EmployeeEvents.Resignation:
                    {

                        break;
                    }

                default: break;
            }

            eventtrigger.Save(serviceUser);
            idmsLogger.WriteLog(LogLevelType.Info, $"PopulateEmployeeEventTrigger : An employee event trigger for {eventtrigger.EventType.ToString()} is created/updated for ther PERNR:{orgChangeTrack.EmployeeId}. The EmployeeEventTrigger Is is {eventtrigger.Id}");


            return eventtrigger;
        }

        bool canCreateEmployeeEvent(OrgChangeTrack orgChangeTrack)
        {
            if (orgChangeTrack == null)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: orgChangeTrack is Null");
                return false;
            }

            if (orgChangeTrack.Event == EmployeeEvents.None)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: Employee Event is None");
                return false;
            }

            if (string.IsNullOrWhiteSpace(orgChangeTrack.EmployeeId))
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: EmployeeId is Null or Empty");
                return false;
            }

            if (orgChangeTrack.EffectiveDate == DateTime.MinValue || orgChangeTrack.EffectiveDate == DateTime.MaxValue)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger: EffectiveDate is {orgChangeTrack.EffectiveDate}");
                return false;
            }

            try
            {
                if (orgChangeTrack.Event == EmployeeEvents.Propagation && orgChangeTrack.EffectiveDate != null && orgChangeTrack.EffectiveDate < DateTime.Now)
                {
                    ///We cannot validate againt IsrequestrCreated and Isactive because during Trigger verification if trigger is not valid we mark isCtive = false. Hence we check Effective Date is less that
                    var existingTrigger = EmployeeEventTrigger.GetOldByEmployeeIdAndEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event, orgChangeTrack.EffectiveDate);

                    if (existingTrigger != null && orgChangeTrack.EffectiveDate < DateTime.Now)
                    {
                        idmsLogger.WriteLog(LogLevelType.Info, $"Employee:{orgChangeTrack.EmployeeId} - Past Dated Event cannot be activated.An employee event trigger for {existingTrigger.EventType.ToString()} TriggerId:{existingTrigger.Id}");
                        return false;
                    }
                }
            }
            catch 
            {
            }

            var canHaveMultipleEventsInADay = new[] {
               // EmployeeEvents.Promotion,
                EmployeeEvents.Propagation };
            if (!canHaveMultipleEventsInADay.Contains(orgChangeTrack.Event))
            {
                var existingEvents = DomainModel.EmployeeEventTrigger.GetAllEventsByEmployeeIdAndEventType(orgChangeTrack.EmployeeId, orgChangeTrack.Event);

                var existingEvent = existingEvents.FirstOrDefault(e =>
                           (e.ActualEventDate.HasValue && e.ActualEventDate == orgChangeTrack.EffectiveDate) ||
                           ((orgChangeTrack.Event != EmployeeEvents.NewJoinee && orgChangeTrack.Event != EmployeeEvents.Rehire) &&
                               e.EffectiveDate == orgChangeTrack.EffectiveDate)
                           );

                if (existingEvent != null)
                {
                    if (existingEvent.IsActive == false)
                    {
                        try
                        {
                            //Production Fix 2844
                            ProvisioningUtility.ProvisioningEmailHelper.NotifyInactiveEmployeeEvent(orgChangeTrack.EmployeeId, orgChangeTrack.Event.ToString(), orgChangeTrack.EffectiveDate.ToString("dd/MM/yyyy"), existingEvent.Id.ToString());
                        }
                        catch
                        {
                        }
                        idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger:For the PERNR:{orgChangeTrack.EmployeeId}, The Event {orgChangeTrack.Event.ToString()} is already exist as Inactive - EmployeeEventTrigger Id  {existingEvent.Id}");
                        return false;
                    }

                    if (existingEvent.IsRequestCreated)
                    {
                        idmsLogger.WriteLog(LogLevelType.Error, $"PopulateEmployeeEventTrigger:For the PERNR:{orgChangeTrack.EmployeeId}, The Event {orgChangeTrack.Event.ToString()} is already honoured - EmployeeEventTrigger Id  {existingEvent.Id}");
                        return false;
                    }

                }
            }

            return true;
        }



        #endregion

        public string GetUserStatusByPernr(string PERNR)
        {
            return this.entityContainer.User.Where(u => u.userId == PERNR.TrimLeadingSeros()).Select(u => u.status).FirstOrDefault();
        }
    }
}
