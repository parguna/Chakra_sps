using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Services.DiscoveryProcessor
{
    public class UserProcessor
    {
        private long _endPointSystemId;
        private Process _process;

        public UserProcessor(long endpointSystemId, Process process)
        {
            _endPointSystemId = endpointSystemId;
            _process = process;
        }

        public void Start()
        {
            ProcessHRMSUsers();
            //new OrganisationProcessor(_endPointSystemId, _process).ProcessOrganisationHead();
        }

        public void ProcessHRMSUsers()
        {
            List<NHRMSUser> hrmsUsers = _process.LastRunOn.HasValue ?
                                            NHRMSUser.GetUserUpdatedAfter(_process.LastRunOn.Value, _endPointSystemId) :
                                            NHRMSUser.GetByEndpointSystemId(_endPointSystemId);

            List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();

            PasswordPolicy iacmPolicy = PasswordPolicy.GetById(1);

            foreach (NHRMSUser hrmsUser in hrmsUsers)
            {
                try
                {
                    bool newUser = false;

                    User iacmUser = User.GetByEmployeeId(hrmsUser.EmployeeId, _endPointSystemId);

                    List<NHRMSUserEvent> unprocessedUserEvent = NHRMSUserEvent.GetUnProcessUserEvent(hrmsUser.Id, _process.ProcessStartAt.Value);

                    // if user not found by employee id
                    if (iacmUser == null)
                    {
                        iacmUser = new User();
                        newUser = true;
                    }

                    CopyHRMSUserToIACMUser(hrmsUser, iacmUser);

                    if (newUser)
                    {
                        // if user is marked as re-employed or there is an active user for this NRIC, do not create new user
                        // else
                        if (User.GetByNRIC(hrmsUser.IdentificationNo) == null)
                        {
                            if (!iacmUser.JoinDate.HasValue)
                            {
                                //continue;
                                iacmUser.JoinDate = DateTime.Now;
                            }
                            iacmUser.UserName = hrmsUser.EmployeeId;//new logic

                            if (iacmPolicy != null)
                            {
                                iacmUser.Password = RSAHelper.EncryptData(new PasswordResolver().ConstructGeneralUserPassword
                                                        (iacmPolicy.PasswordLength.HasValue ? iacmPolicy.PasswordLength.Value : 8,
                                                        iacmPolicy.NoOfSpecialCharacters.HasValue ? iacmPolicy.NoOfSpecialCharacters.Value : 0,
                                                        iacmPolicy.NoOfLowerCaseAlphabets.HasValue ? iacmPolicy.NoOfLowerCaseAlphabets.Value : 0,
                                                        iacmPolicy.NoOfUpperCaseAlphabets.HasValue ? iacmPolicy.NoOfUpperCaseAlphabets.Value : 0,
                                                        iacmPolicy.NoOfNumericCharacters.HasValue ? iacmPolicy.NoOfNumericCharacters.Value : 0));

                                iacmUser.Save(MasterConfigHelper.ServiceUser);
                                User.InsertUserRole(iacmUser.Id, MasterConfigHelper.RoleUser);
                            }
                            else
                            {
                                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, string.Format("IACM default password policy not available for user {0}" + iacmUser.DisplayName));
                                throw new EntityNotFoundException(string.Format("IACM default password policy not available for user {0}" + iacmUser.DisplayName));
                            }
                        }
                    }

                    else
                    {
                        // if no ou change or re-org events to be processed
                        if (!unprocessedUserEvent.Exists(usrEvnt => usrEvnt.EmployeeEvent == EmployeeEvents.OUChange || usrEvnt.EmployeeEvent == EmployeeEvents.ReOrganisation))
                        {
                            User.UpdateUserFromDiscovery(iacmUser, MasterConfigHelper.PropagationFields, MasterConfigHelper.CanRunPropagationTrigger, MasterConfigHelper.ServiceUser);
                        }
                        else // if there are any ou change or re-org events for this user
                        {
                            // Verify whether the event has alredy been exceuted
                            // This is because SAP HRMS will keep the OU Change trigger as active till another trigger is added to PA0000 table
                            // IACM can start updating information as soon as we have executed the Transfer trigger
                            User oldUser = User.GetByEmployeeId(iacmUser.EmployeeId);

                            string newOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(iacmUser.NOrganisationId.Value).Code : string.Empty;
                            string oldOrgCode = oldUser.NOrganisationId.HasValue ? NOrganisation.GetById(oldUser.NOrganisationId.Value).Code : string.Empty;

                            // if the current org code is same as new org code to be changed to, this event has been executed already
                            if (newOrgCode == oldOrgCode)
                            {
                                // save the HRMS User data to global user
                                User.UpdateUserFromDiscovery(iacmUser, MasterConfigHelper.PropagationFields, MasterConfigHelper.CanRunPropagationTrigger, MasterConfigHelper.ServiceUser);
                            }
                        }
                    }

                    ProcessUserEvent(unprocessedUserEvent, iacmUser);
                }
                catch (Exception ex)
                {
                    DILog.DiscoveryLogger.WriteLog(this._process.Id, LogLevelType.Exception, string.Format("Error while processing user :{0}", hrmsUser.DisplayName), ex);
                }
            }
        }

        private void ProcessUserEvent(List<NHRMSUserEvent> userEvents, User iacmUser)
        {
            try
            {
                foreach (NHRMSUserEvent userEvent in userEvents)
                {
                    if (userEvent.EmployeeEvent == EmployeeEvents.NewJoinee)
                    {
                        if (!iacmUser.JoinDate.HasValue)
                            continue;

                        // if this is a converted employee, do not process new joinee event
                        //double onBoardDaysBeforeJoining = MasterConfigHelper.OnBoardDaysBeforeJoining;
                        //DateTime priorBoardingDate = iacmUser.JoinDate.Value.AddDays(-(onBoardDaysBeforeJoining));

                        //if (priorBoardingDate < DateTime.Today)
                        //    priorBoardingDate = DateTime.Today;

                        //User oldUser = User.GetByNRIC(iacmUser.NRIC);

                        //if (oldUser != null && iacmUser.JoinDate > oldUser.TerminationDate && EmployeeEventTrigger.IsAnyOnBoardTriggerExistsAndExecutedForUser(oldUser.Id))
                        //{
                        //    iacmUser.Id = oldUser.Id;
                        //    iacmUser.UserName = oldUser.UserName;
                        //    iacmUser.Password = oldUser.Password;
                        //    iacmUser.EmailId = oldUser.EmailId;
                        //    iacmUser.AuthenticationMode = oldUser.AuthenticationMode;

                        //    string newOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(iacmUser.NOrganisationId.Value).Code : string.Empty;
                        //    string oldOrgCode = oldUser.NOrganisationId.HasValue ? NOrganisation.GetById(oldUser.NOrganisationId.Value).Code : string.Empty;

                        //    string newDesignationCode = iacmUser.NBusinessRoleId.HasValue ? NDesignation.GetById(iacmUser.NBusinessRoleId.Value).Code : string.Empty;

                        //    EmployeeEventTrigger.CreateRejoinTrigger(iacmUser, iacmUser.JoinDate.Value, newOrgCode, oldOrgCode, newDesignationCode, MasterConfigHelper.ServiceUser);

                        //    oldUser.NeedIT105Delimit = true;
                        //    oldUser.Save(MasterConfigHelper.ServiceUser);
                        //}
                        //else
                    }
                    // rehire
                    else if (userEvent.EmployeeEvent == EmployeeEvents.Rehire)
                    {
                        if (!iacmUser.JoinDate.HasValue)
                            continue;
                        //get new or old user
                        User oldUser = User.GetByNRIC(iacmUser.IdentificationNo);
                        // create trigger only if employee id is different
                        if (oldUser != null && oldUser.EmployeeId != iacmUser.EmployeeId)
                        {
                            iacmUser.Id = oldUser.Id;
                            iacmUser.UserName = oldUser.UserName;
                            iacmUser.Password = oldUser.Password;
                            iacmUser.Reserved4 = oldUser.Reserved4;
                            iacmUser.Reserved5 = oldUser.Reserved5;
                            iacmUser.EmailId = oldUser.EmailId;
                            iacmUser.AuthenticationMode = oldUser.AuthenticationMode;
                            iacmUser.LastLogonDate = iacmUser.LastLogonDate;

                            string newOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(iacmUser.NOrganisationId.Value).Code : string.Empty;
                            string oldOrgCode = oldUser.NOrganisationId.HasValue ? NOrganisation.GetById(oldUser.NOrganisationId.Value).Code : string.Empty;

                            string newDesignationCode = iacmUser.NBusinessRoleId.HasValue ? NDesignation.GetById(iacmUser.NBusinessRoleId.Value).Code : string.Empty;

                            EmployeeEventTrigger.CreateRejoinTrigger(iacmUser, iacmUser.JoinDate.Value, newOrgCode, oldOrgCode, newDesignationCode, MasterConfigHelper.ServiceUser);

                            //oldUser.NeedIT105Delimit = true;
                            //oldUser.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    else if (userEvent.EmployeeEvent == EmployeeEvents.Resignation)
                    {
                        if (!iacmUser.TerminationDate.HasValue)
                            continue;

                        // if the current user is already inactive, do not execute below
                        User oldUser = User.GetByEmployeeId(iacmUser.EmployeeId);
                        if (oldUser.IsActive == false)
                            continue;

                        // if TerminationDate for current user in db is empty or not same as latest discovered Termination date
                        if (!oldUser.TerminationDate.HasValue || oldUser.TerminationDate.Value != iacmUser.TerminationDate.Value)
                        {
                            // updating current user's Termination Date in database
                            oldUser.TerminationDate = iacmUser.TerminationDate;
                            oldUser.Save(MasterConfigHelper.ServiceUser);
                        }

                        // offboard on termination date + 1
                        EmployeeEventTrigger.CreateOffboardTrigger(iacmUser, iacmUser.TerminationDate.Value, MasterConfigHelper.ServiceUser);

                    }
                    else if (userEvent.EmployeeEvent == EmployeeEvents.OUChange)
                    {
                        User oldUser = User.GetByEmployeeId(iacmUser.EmployeeId);

                        string newOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(iacmUser.NOrganisationId.Value).Code : string.Empty;
                        string oldOrgCode = oldUser.NOrganisationId.HasValue ? NOrganisation.GetById(oldUser.NOrganisationId.Value).Code : string.Empty;

                        //This scenerio could happen when HRMS discovery run muliple time for the same day. (Note, in current SAP HRMS delta discovery
                        //is based on date and not based on datetime.
                        if (newOrgCode == oldOrgCode)
                            continue;

                        string newDesignationCode = iacmUser.NBusinessRoleId.HasValue ? NDesignation.GetById(iacmUser.NBusinessRoleId.Value).Code : string.Empty;
                        EmployeeEventTrigger.CreateTransferTrigger(iacmUser, userEvent.EffectiveDate ?? DateTime.Today, newOrgCode, oldOrgCode, newDesignationCode, MasterConfigHelper.ServiceUser);
                    }
                    //else if (userEvent.EmployeeEvent == EmployeeEvents.ReOrganisation)
                    //{
                    //    User oldUser = User.GetByEmployeeId(iacmUser.EmployeeId);

                    //    string newOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(iacmUser.NOrganisationId.Value).Code : string.Empty;
                    //    string oldOrgCode = iacmUser.NOrganisationId.HasValue ? NOrganisation.GetById(oldUser.NOrganisationId.Value).Code : string.Empty;

                    //    EmployeeEventTrigger.CreateReOrgTrigger(iacmUser, userEvent.EffectiveDate ?? DateTime.Today, newOrgCode, oldOrgCode, MasterConfigHelper.ServiceUser);
                    //}
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(this._process.Id, LogLevelType.Exception, string.Format("Error while processing HR events for user :{0}", iacmUser.DisplayName), ex);
            }
        }

        private void CopyHRMSUserToIACMUser(NHRMSUser hrmsUser, User iacmUser)
        {
            iacmUser.IdentificationNo = hrmsUser.IdentificationNo;
            iacmUser.FirstName = hrmsUser.FirstName;
            iacmUser.MiddleName = hrmsUser.MiddleName;
            iacmUser.LastName = hrmsUser.LastName;
            iacmUser.DisplayName = hrmsUser.DisplayName;
            iacmUser.KnownAs = hrmsUser.KnownAs;
            iacmUser.EmployeeId = hrmsUser.EmployeeId;
            iacmUser.FullTimeEmployee = hrmsUser.FullTimeEmployee;

            iacmUser.MobileNumber = hrmsUser.MobileNumber;

            if (hrmsUser.JoinDate.HasValue)
                iacmUser.JoinDate = hrmsUser.JoinDate;

            iacmUser.TerminationDate = hrmsUser.TerminationDate;

            iacmUser.IsActive = hrmsUser.IsActive;
            iacmUser.Ethinicity = hrmsUser.Ethinicity;
            iacmUser.EmployeeType = hrmsUser.EmployeeType;

            // update following only if iacm user has no value
            // i.e. prefer IGA values over HRMS values
            if (String.IsNullOrWhiteSpace(iacmUser.EmailId))
                iacmUser.EmailId = hrmsUser.EmailId;

            //retain soe_sid, soe_name, soe_dept from flat file
            //iacmUser.Reserved6 = hrmsUser.SOE_SID;
            //iacmUser.Reserved7 = hrmsUser.SOE_NAME;
            //iacmUser.Reserved8 = hrmsUser.SOE_DEPT;
            //iacmUser.Reserved9 = hrmsUser.Company;
            iacmUser.EndPointSystemId = hrmsUser.EndpointSystemId;

            NDesignation designation = NDesignation.GetByCode(hrmsUser.PositionCode);
            if (designation != null)
                iacmUser.NBusinessRoleId = designation.Id;

            NOrganisation organisation = NOrganisation.GetByOrgCode(hrmsUser.SectionCode);
            if (organisation != null)
            {
                iacmUser.NOrganisationId = organisation.Id;

                //populate org info
                this.FillUserOrgInfo(iacmUser);
            }
        }

        private void FillUserOrgInfo(User iacmUser)
        {
            var userOrg = NOrganisation.GetById(iacmUser.NOrganisationId.Value, true);

            if (userOrg != null && userOrg.OrganisationScope != null)
            {
                while (true)
                {
                    if (userOrg.OrganisationScope != null)
                    {
                        //if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Company)
                        //{
                        //    iacmUser.SectionName = userOrg.Id;
                        //    iacmUser.OrgCompanyCode = userOrg.Code;
                        //    iacmUser.OrgCompanyShortDesc = userOrg.Name;
                        //    iacmUser.OrgCompanyLongDesc = userOrg.Description;
                        //}
                        //else if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Department)
                        //{
                        //    iacmUser.OrgDepartmentId = userOrg.Id;
                        //    iacmUser.OrgDepartmentCode = userOrg.Code;
                        //    iacmUser.OrgDepartmentShortDesc = userOrg.Name;
                        //    iacmUser.OrgDepartmentLongDesc = userOrg.Description;
                        //}
                        //else if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Division)
                        //{
                        //    iacmUser.OrgClusterId = userOrg.Id;
                        //    iacmUser.OrgClusterCode = userOrg.Code;
                        //    iacmUser.OrgClusterShortDesc = userOrg.Name;
                        //    iacmUser.OrgClusterLongDesc = userOrg.Description;
                        //}
                        //else if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Branch)
                        //{
                        //    iacmUser.OrgGroupId = userOrg.Id;
                        //    iacmUser.OrgGroupCode = userOrg.Code;
                        //    iacmUser.OrgGroupShortDesc = userOrg.Name;
                        //    iacmUser.OrgGroupLongDesc = userOrg.Description;
                        //}
                        //else if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Section)
                        //{
                        //    iacmUser.OrgSectionId = userOrg.Id;
                        //    iacmUser.OrgSectionCode = userOrg.Code;
                        //    iacmUser.OrgSectionShortDesc = userOrg.Name;
                        //    iacmUser.OrgSectionLongDesc = userOrg.Description;
                        //}
                        //else if (userOrg.OrganisationScope.Level == (int)OrgScopeLevel.Unit)
                        //{
                        //    iacmUser.OrgSectionId = userOrg.Id;
                        //    iacmUser.OrgSectionCode = userOrg.Code;
                        //    iacmUser.OrgSectionShortDesc = userOrg.Name;
                        //    iacmUser.OrgSectionLongDesc = userOrg.Description;
                        //}
                    }

                    if (string.IsNullOrEmpty(userOrg.ParentOrgCode))
                    {
                        return;
                    }
                    else
                    {
                        userOrg = NOrganisation.GetByOrgCode(userOrg.ParentOrgCode);

                        if (userOrg == null)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
