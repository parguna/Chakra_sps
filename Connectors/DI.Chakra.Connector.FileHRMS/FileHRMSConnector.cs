using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using System.Data;
using DI.Chakra.Infrastructure;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure.Enums;
using System.Globalization;

namespace DI.Chakra.Connector
{
    public class FileHRMSConnector : ConnectorBase
    {
        public FileHRMSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this.EndPoint = endpoint;
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
        }



        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        public override bool Connect()
        {
            try
            {
                if (File.Exists(Configuration(EndpointKey.EmployeeFilePath))
                    && File.Exists(Configuration(EndpointKey.OrganisationFilePath))
                    && File.Exists(Configuration(EndpointKey.BusinessRoleFilePath))
                   )
                {
                    //template needs to validate.
                    WriteDiscoveryLog(LogLevelType.Info, "File Connector Connection succeeded.");
                    return true;
                }

                WriteDiscoveryLog(LogLevelType.Error, "File Connector Connection failed.");
                return false;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "File Connector Connection failed.", ex);
                return false;
            }
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void GetData()
        {
            try
            {

                long DiscoveryProcessId = base.DiscoveryProcess.Id;
                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_FILEHRMS_User) != null;
                bool IsOrgSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_FILEHRMS_Organisation) != null;
                bool IsBUsinessRoleSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_FILEHRMS_Designation) != null;

                if (IsOrgSelected)
                    GetOrganisation();
                if (IsBUsinessRoleSelected)
                    GetBusinessRole();
                if (IsUserSelected)
                    GetUser();

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "File Connector error while doing GetData.", ex);
            }
        }

        void GetOrganisation()
        {
            try
            {
                //parse the file and get the user
                var dataTable = new TextDataConnect().ReadCSV(Configuration(EndpointKey.OrganisationFilePath));

                //loop through the file data and create user if not already exists.    
                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    try
                    {
                        var orgCode = GetColumnValue(currentWorkingRow, "OrganisationCode");

                        if (string.IsNullOrEmpty(orgCode))
                            continue;

                        var organisation = NHRMSOrganisation.GetByOrgCode(orgCode);

                        if (organisation == null)
                            organisation = new NHRMSOrganisation() { Code = orgCode };

                        organisation.EndpointSystemId = this.EndPoint.Id;

                        organisation.Name = GetColumnValue(currentWorkingRow, "OrganisationName"); ;
                        organisation.ParentOrgCode = GetColumnValue(currentWorkingRow, "ParentOrganisationCode");
                        organisation.Scope = GetColumnValue(currentWorkingRow, "Scope");
                        organisation.Head = GetColumnValue(currentWorkingRow, "HeadEmployeeId");

                        var isAcitve = true;
                        organisation.IsActive = isAcitve;

                        organisation.Save(MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get Organisation Failed : " + ex.Message);
                    }
                }
            }

            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "GetOrganisation Failed : " + ex.Message);
            }
        }

        string GetColumnValue(DataRow currentRow, string columnName)
        {
            try
            {
                if (currentRow.Table.Columns.Contains(columnName))
                {
                    var value = currentRow[columnName].ToString().Trim();

                    if (value.ToUpper() == "NULL") return "";

                    return value;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, string.Format("Not able get the Field From CSV File, Field Name {0}.", columnName), ex);
                return string.Empty;
            }
        }
        NHRMSUser ProcessUserFromRow(DataRow currentWorkingRow)
        {

            try
            {
                var employeeId = GetColumnValue(currentWorkingRow, "EmployeeId");
                if (string.IsNullOrEmpty(employeeId.Trim()))
                {
                    WriteDiscoveryLog(LogLevelType.Error, "Employeeid not found.");
                    return null;
                }

                NHRMSUser user = NHRMSUser.GetByEmployeeId(employeeId, this.EndPoint.Id)
                                        ?? new NHRMSUser() { EndpointSystemId = this.EndPoint.Id, EmployeeId = employeeId };
                List<NHRMSUserEvent> userEvent = new List<NHRMSUserEvent>();

                user.IdentificationNo = GetColumnValue(currentWorkingRow, "NRIC");
                user.FirstName = GetColumnValue(currentWorkingRow, "FirstName");
                user.MiddleName = GetColumnValue(currentWorkingRow, "MiddleName");
                user.LastName = GetColumnValue(currentWorkingRow, "LastName");
                user.DisplayName = GetColumnValue(currentWorkingRow, "DisplayName");
                user.KnownAs = user.DisplayName;

                user.OrgUnitCode = GetColumnValue(currentWorkingRow, "OrganisationCode");
                user.PositionCode = GetColumnValue(currentWorkingRow, "DesignationCode"); ;


                user.EmailId = GetColumnValue(currentWorkingRow, "EmailId");

                var employeeType = GetColumnValue(currentWorkingRow, "EmployeeType");
                if (!string.IsNullOrEmpty(employeeType))
                {
                    EmployeeType empType = EmployeeType.Permanent;

                    user.EmployeeType = empType; //default set to Permanent

                    if (Enum.TryParse<EmployeeType>(employeeType, out empType))
                        user.EmployeeType = empType;
                }

                // we are setting this flag as true so that it will shown in the UI under Employee menu
                user.FullTimeEmployee = true;

                DateTime joiningDate = DateTime.MaxValue;
                var jDate = GetColumnValue(currentWorkingRow, "JoinDate");
                if (!string.IsNullOrEmpty(jDate.Trim()) && DateTime.TryParseExact(jDate, "dd-MMM-yy",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out joiningDate))
                {
                    user.JoinDate = joiningDate;
                }
                else
                {
                    WriteDiscoveryLog(LogLevelType.Error, "Joining Date not available for Employeeid : " + employeeId);
                    return null;
                }

                DateTime terminationDate = DateTime.MaxValue;
                var tDate = GetColumnValue(currentWorkingRow, "TerminationDate").Trim();
                if (!string.IsNullOrEmpty(tDate) && DateTime.TryParseExact(tDate, "dd-MMM-yy",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out terminationDate))
                    user.TerminationDate = Convert.ToDateTime(terminationDate);

                var isActive = GetColumnValue(currentWorkingRow, "IsActive").Trim();
                if (!string.IsNullOrEmpty(isActive))
                    user.IsActive = Convert.ToBoolean(isActive);

                return user;

            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Creating New User Failed : " + ex.Message);
                return null;
            }

        }

        void GetUser()
        {
            try
            {
                //parse the file and get the user
                var dataTable = new TextDataConnect().ReadCSV(Configuration(EndpointKey.EmployeeFilePath));

                //loop through the file data and create user if not already exists.    
                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    string oldOrgCode = string.Empty;
                    List<NHRMSUserEvent> userEvent = new List<NHRMSUserEvent>();

                    try
                    {
                        var processedUser = ProcessUserFromRow(currentWorkingRow);

                        if (processedUser == null)
                            continue;

                        DateTime triggerEffectiveDate = DateTime.Now;
                        var effDate = GetColumnValue(currentWorkingRow, "EffectiveDate").Trim();
                        if (!string.IsNullOrEmpty(effDate) && DateTime.TryParseExact(effDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out triggerEffectiveDate))
                            triggerEffectiveDate = DateTime.Parse(effDate);
                        else
                            triggerEffectiveDate = DateTime.Now;

                        if (!this.IsFirstTimeDiscovery)
                        {

                            if (processedUser.Id == 0)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.NewJoinee,
                                });
                            }
                            else
                            {
                                var oldUser = NHRMSUser.GetById(processedUser.Id);

                                //check if for the same previously discovered user Termination action is already processed
                                if (oldUser.TerminationDate == null || oldUser.TerminationDate == DateTime.MaxValue)
                                {
                                    if (processedUser.TerminationDate != null && processedUser.TerminationDate != DateTime.MaxValue)
                                    {
                                        userEvent.Add(new NHRMSUserEvent()
                                        {
                                            EmployeeEvent = EmployeeEvents.Resignation,
                                        });
                                    }
                                }

                                if (oldUser.OrgUnitCode != processedUser.OrgUnitCode)
                                {
                                    //if there is change in org unit, find whether it due to Reorg or OUChange.

                                    var isReOrg = GetColumnValue(currentWorkingRow, "ReOrganisation");
                                    if (!string.IsNullOrEmpty(isReOrg.Trim()))
                                    {
                                        if (Convert.ToBoolean(isReOrg))
                                        {
                                            userEvent.Add(new NHRMSUserEvent()
                                            {
                                                EmployeeEvent = EmployeeEvents.ReOrganisation,
                                                EffectiveDate = triggerEffectiveDate
                                            });
                                        }
                                    }
                                    else
                                    {
                                        userEvent.Add(new NHRMSUserEvent()
                                        {
                                            EmployeeEvent = EmployeeEvents.OUChange,
                                            EffectiveDate = triggerEffectiveDate
                                        });
                                    }
                                }
                            }
                        }

                        processedUser.Save(MasterConfigHelper.ServiceUser);

                        userEvent.ForEach(evt => evt.UserId = processedUser.Id);

                        NHRMSUserEvent.SaveList(userEvent, MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(LogLevelType.Error, "Creating New User Failed : " + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Get User Failed : " + ex.Message);
            }
        }

        void GetBusinessRole()
        {
            try
            {
                //parse the file and get the user
                var dataTable = new TextDataConnect().ReadCSV(Configuration(EndpointKey.BusinessRoleFilePath));

                //loop through the file data and create user if not already exists.    
                foreach (DataRow currentWorkingRow in dataTable.Rows)
                {
                    try
                    {
                        var designationCode = GetColumnValue(currentWorkingRow, "DesignationCode");

                        if (string.IsNullOrEmpty(designationCode))
                            continue;

                        var designation = NHRMSDesignation.GetByCode(designationCode, this.EndPoint.Id);

                        if (designation == null)
                            designation = new NHRMSDesignation() { Code = designationCode, IsActive = true };

                        designation.EndpointSystemId = this.EndPoint.Id;
                        designation.Description = GetColumnValue(currentWorkingRow, "Description"); ;

                        designation.Save(MasterConfigHelper.ServiceUser);
                    }
                    catch (Exception ex)
                    {
                        WriteDiscoveryLog(DI.Chakra.Logger.LogLevelType.Error, "Get Organisation Failed : " + ex.Message);
                    }
                }
            }

            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "GetBusinessRole Failed : " + ex.Message);
            }
        }
    }
}
