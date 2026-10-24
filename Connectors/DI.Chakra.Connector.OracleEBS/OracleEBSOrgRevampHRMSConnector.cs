using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System.Data;
using System.Data.OracleClient;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class OracleEBSOrgRevampHRMSConnector : ConnectorBase
    {

        public OracleEBSOrgRevampHRMSConnector(EndPoint _endpoint, AccessRequest currentRequest = null)
        {
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            this.EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;
        }

        public override bool Connect()
        {
            OracleConnection OrCon = null;
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started Connection Establish"));
                string OEBSConStr = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                OEBSConStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                OrCon = new OracleConnection(OEBSConStr);
                OrCon.Open();
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - Connection Established SuccessFully ", Configuration(EndpointKey.IPAddress)));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Error occured while Connecting the Connection {0} (Connect Method) ", Configuration(EndpointKey.IPAddress)));
                return false;
            }
            finally
            {
                OrCon.Close();
                OrCon.Dispose();
            }
            return true;
        }

        public override void GetData()
        {
            this.GetOrganization();

            this.GetOrganizationHead();

            this.GetJob();

            long recordCount = this.GetUser();

            WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} Users found in Trusted Source", recordCount.ToString()));

        }

        private int GetUser()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery started ", Configuration(EndpointKey.HostName)));

                DataTable oracleEBSUsers = GetHRMSUsers();

                foreach(DataRow oracleEBSUser in oracleEBSUsers.Rows)
                {
                    NHRMSUser user = NHRMSUser.GetByEmployeeId(CheckNull.AgainstString(oracleEBSUser["PersonID"]), this.EndPoint.Id) ?? new NHRMSUser();
                    List<NHRMSUserEvent> userEvent = new List<NHRMSUserEvent>();

                    user.EndpointSystemId = EndPoint.Id;

                    user.EmployeeId = CheckNull.AgainstString(oracleEBSUser["PersonID"]);
                    user.IdentificationNo = CheckNull.AgainstString(oracleEBSUser["SSN"]);

                    user.FirstName = CheckNull.AgainstString(oracleEBSUser["FirstName"]);
                    user.LastName = CheckNull.AgainstString(oracleEBSUser["LastName"]);
                    user.KnownAs = CheckNull.AgainstString(oracleEBSUser["KNOWN_AS"]);
                    user.DisplayName = CheckNull.AgainstString(oracleEBSUser["FULL_NAME"]).Replace(",", "");

                    user.MobileNumber = CheckNull.AgainstString(oracleEBSUser["TELEPHONE"]);
                    user.EmailId = CheckNull.AgainstString(oracleEBSUser["Email"]);


                    user.FullTimeEmployee = CheckNull.AgainstString(oracleEBSUser["PersonType"]) == "Employee" ? true:  false;
                    user.EmployeeType = EmployeeType.Permanent;
                    user.IsActive = true;

                    user.JoinDate = CheckNull.AgainstNullableDatetime(oracleEBSUser["EffStartDate"]);
                    user.TerminationDate = CheckNull.AgainstNullableDatetime(oracleEBSUser["EffEndDate"]);

                    user.OrgUnitCode = CheckNull.AgainstString(oracleEBSUser["OrganisationId"]);
                    user.PositionCode = CheckNull.AgainstString(oracleEBSUser["JOB_ID"]);

                    if (!this.IsFirstTimeDiscovery)
                    {
                        if (user.Id == 0)
                        {
                            userEvent.Add(new NHRMSUserEvent()
                            {
                                EmployeeEvent = EmployeeEvents.NewJoinee,
                            });
                        }
                        else
                        {
                            var oldUser = NHRMSUser.GetById(user.Id);

                            //check if for the same previously discovered user Termination action is already processed
                            if (oldUser.TerminationDate == null || oldUser.TerminationDate == DateTime.MaxValue)
                            {
                                if (user.TerminationDate != null && user.TerminationDate != DateTime.MaxValue)
                                {
                                    userEvent.Add(new NHRMSUserEvent()
                                    {
                                        EmployeeEvent = EmployeeEvents.Resignation,
                                    });
                                }
                            }

                            if (oldUser.OrgUnitCode != user.OrgUnitCode)
                            {
                                userEvent.Add(new NHRMSUserEvent()
                                {
                                    EmployeeEvent = EmployeeEvents.OUChange,
                                });
                            }
                        }
                    }

                    user.Save(MasterConfigHelper.ServiceUser);

                    userEvent.ForEach(evt => evt.UserId = user.Id);
                    NHRMSUserEvent.SaveList(userEvent, MasterConfigHelper.ServiceUser);
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("{0} - HRMSUsers discovery completed ", Configuration(EndpointKey.HostName)));

                return oracleEBSUsers != null ? oracleEBSUsers.Rows.Count : 0;
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Error, "Error in delta getPAInformation", ex);
                throw;
            }
        }

        private DataTable GetHRMSUsers()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GetHRMSUsers Mehtod"));

                string HRMSUserSQL = @" select
                        papf.person_id PersonID, papf.Last_Name LastName, papf.First_Name FirstName, papf.Title Title, hl_sex.meaning SEX, 
                        pptt.user_person_type PersonType, papf.national_identifier SSN, papf.date_of_birth DOB, hl_nat.meaning Nationality,
                        hl_mar.meaning MaritalStatus, papf.Effective_Start_Date EffStartDate,
                        papf.Effective_End_Date EffEndDate, papf.Email_address Email, papf.original_date_of_hire DateFirstHired,
                        papf.BUSINESS_GROUP_ID BGI, papf.START_DATE StartDate, papf.FULL_NAME FULL_NAME, papf.KNOWN_AS KNOWN_AS, papf.WORK_TELEPHONE TELEPHONE,
                        haou.ORGANIZATION_ID OrganisationId,haou.NAME OrganisationName, paaf.JOB_ID 
                    FROM
                        per_all_people_f papf,
                        hr_lookups hl_sex,
                        per_person_types_tl pptt,
                        hr_lookups hl_nat,
                        hr_lookups hl_mar,
                        hr_lookups hl_eth,
                        per_all_assignments_f  paaf,
                        hr_all_organization_units haou
                    WHERE 1=1
                        AND hl_sex.lookup_code(+) = papf.sex
                        AND hl_sex.lookup_type(+) = 'SEX'
                        AND hl_nat.lookup_code(+) = papf.nationality
                        AND hl_nat.lookup_type(+) = 'NATIONALITY'
                        AND hl_mar.lookup_code(+) = papf.marital_status
                        AND hl_mar.lookup_type(+) = 'MAR_STATUS'
                        AND hl_eth.lookup_code(+) = papf.per_information1
                        AND hl_eth.lookup_type(+) = 'US_ETHNIC_GROUP'
                        AND papf.person_type_id = pptt.person_type_id
                        AND pptt.user_person_type = 'Employee'
                        AND papf.PERSON_ID = paaf.PERSON_ID
                        AND paaf.ORGANIZATION_ID = haou.ORGANIZATION_ID
                        AND papf.current_employee_flag= 'Y'";
                //
                if (this.Process.LastRunOn.HasValue)
                {
                    string format = "dd.MM.yyyy";
                    string date = this.Process.LastRunOn.Value.ToString(format);
                    HRMSUserSQL = HRMSUserSQL + string.Format(" AND papf.LAST_UPDATE_DATE > TO_DATE('{0}','{1}')", date, format);
                }


                DataSet dsHRMSUserinfo = getDSFromOracleQuery(HRMSUserSQL);

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetHRMSUsers Mehtod"));

                return dsHRMSUserinfo.Tables[0];
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while get HRMSUser (getHRMSUsers Method)."));
                throw;
            }

        }

        private void GetOrganization()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GetOrganization Mehtod"));

                string HRMSOrganizationSQL = @"
                                    SELECT AOU.ORGANIZATION_ID, NAME, OSE.ORGANIZATION_ID_PARENT FROM hr_all_organization_units AOU
                                    LEFT JOIN PER_ORG_STRUCTURE_ELEMENTS OSE
                                    ON AOU.ORGANIZATION_ID = OSE.ORGANIZATION_ID_CHILD 
                                    ";

                DataSet dsHRMSOrginfo = getDSFromOracleQuery(HRMSOrganizationSQL);

                foreach (DataRow oracleEBSOrg in dsHRMSOrginfo.Tables[0].Rows)
                {
                    string organisationId = CheckNull.AgainstString(oracleEBSOrg["ORGANIZATION_ID"]);

                    if (string.IsNullOrEmpty(organisationId.Trim()))
                        continue;

                    var organisation = NHRMSOrganisation.GetByOrgCode(organisationId);

                    if (organisation == null)
                        organisation = new NHRMSOrganisation() { Code = organisationId };

                    organisation.Name = CheckNull.AgainstString(oracleEBSOrg["NAME"]);
                    organisation.ParentOrgCode = CheckNull.AgainstString(oracleEBSOrg["ORGANIZATION_ID_PARENT"]);
                    organisation.EndpointSystemId = this.EndPoint.Id;

                    organisation.Save(MasterConfigHelper.ServiceUser);

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetOrganization Mehtod"));
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while executing GetOrganization Method)."));
                throw;
            }

        }

        private void GetOrganizationHead()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GetOrganizationHead Method"));

                string HRMSOrganizationHeadSQL = @"
                            select
                             org_tl.name                                                         ""Organization"",
                             ppf.full_name                                                       ""Manager"",
                             fnd_date.canonical_to_date(org_info2.org_information3)             ""Start Date"",
                             fnd_date.canonical_to_date(org_info2.org_information4)             ""End Date"",
                             --
                             org.organization_id,
                             org.business_group_id,
                             ppf.person_id
                         from
                             hr_all_organization_units org,
                             hr_all_organization_units_tl org_tl,
                             hr_organization_information org_info2,
                             per_all_people_f ppf
                         where
                             1 = 1
                         and org_info2.organization_id = org.organization_id
                         and org_info2.org_information_context = 'Organization Name Alias'
                         and org_info2.org_information2 = to_char(ppf.person_id(+))
                         and org.organization_id = org_tl.organization_id
                         and org_tl.language = userenv('LANG')
                         and exists (select null
                             from hr_org_info_types_by_class oitbc, hr_organization_information org_info
                             where org_info.organization_id = org.organization_id
                             and org_info.org_information_context = 'CLASS'
                             and org_info.org_information2 = 'Y'
                             and oitbc.org_classification = org_info.org_information1
                             and oitbc.org_information_type = 'Organization Name Alias')
                         and(decode(hr_security.view_all,
                                 'Y', 'TRUE',
                                 hr_security.show_record('HR_ALL_ORGANIZATION_UNITS', org.organization_id)) = 'TRUE'
                             and decode(hr_general.get_xbg_profile,
                                     'Y', org.business_group_id,
                                     hr_general.get_business_group_id) = org.business_group_id
                             or decode(hr_security.view_all,
                                     'Y', 'TRUE',
                                     hr_security.show_record('PER_ALL_PEOPLE_F',
                                                        ppf.person_id,
                                                        ppf.person_type_id,
                                                        ppf.employee_number,
                                                        ppf.applicant_number)) = 'TRUE'
                                 and decode(hr_general.get_xbg_profile,
                                         'Y', ppf.business_group_id,
                                         hr_general.get_business_group_id) = ppf.business_group_id)
                                    ";

                DataSet dsHRMSOrgHeadinfo = getDSFromOracleQuery(HRMSOrganizationHeadSQL);

                foreach (DataRow oracleEBSOrgHead in dsHRMSOrgHeadinfo.Tables[0].Rows)
                {
                    string organisationId = CheckNull.AgainstString(oracleEBSOrgHead["organization_id"]);

                    if (string.IsNullOrEmpty(organisationId.Trim()))
                        continue;

                    var organisation = NHRMSOrganisation.GetByOrgCode(organisationId);

                    if (organisation == null)
                        continue;

                    organisation.Head = CheckNull.AgainstString(oracleEBSOrgHead["person_id"]);

                    organisation.Save(MasterConfigHelper.ServiceUser);

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetOrganizationHead Mehtod"));
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while executing GetOrganizationHead Method)."));
                throw;
            }
        }

        private void GetJob()
        {
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started GetJob Mehtod"));

                string HRMSJobSQL = @"
                                    select JOB_ID,NAME from PER_JOBS
                                    ";

                DataSet dsHRMSJobinfo = getDSFromOracleQuery(HRMSJobSQL);

                foreach (DataRow oracleEBSJOb in dsHRMSJobinfo.Tables[0].Rows)
                {
                    string jobId = CheckNull.AgainstString(oracleEBSJOb["JOB_ID"]);

                    if (string.IsNullOrEmpty(jobId.Trim()))
                        continue;

                    var job = NHRMSDesignation.GetByCode(jobId, this.EndPoint.Id);

                    if (job == null)
                        job = new NHRMSDesignation() { Code = jobId };

                    job.Description = CheckNull.AgainstString(oracleEBSJOb["NAME"]);
                    job.EndpointSystemId = this.EndPoint.Id;

                    job.Save(MasterConfigHelper.ServiceUser);

                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed GetJob Mehtod"));
            }
            catch (Exception)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while executing GetJob Method)."));
                throw;
            }

        }

        private DataSet getDSFromOracleQuery(string strQry)
        {
            DataSet dsResults = new DataSet();
            try
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Started getDSFromOracleQuery Method"));
                string oracleConnectionString = string.Empty;
                string portNo = Configuration(EndpointKey.PortNo);
                string InstanceName = Configuration(EndpointKey.InstanceName);
                string Password = Configuration(EndpointKey.Password);
                oracleConnectionString = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));" +
                    "User Id={3};Password={4};", Configuration(EndpointKey.IPAddress), portNo, InstanceName, Configuration(EndpointKey.UserName), Password);
                using (OracleConnection OrCon = new OracleConnection(oracleConnectionString))
                {
                    OrCon.Open();
                    using (OracleCommand OrCom = new OracleCommand())
                    {
                        OrCom.Connection = OrCon;
                        OrCom.CommandType = CommandType.Text;
                        OrCom.CommandText = strQry;
                        using (OracleDataAdapter OrAdapter = new OracleDataAdapter(OrCom.CommandText, OrCom.Connection))
                        {
                            OrAdapter.Fill(dsResults);
                        }
                    }
                }

                WriteDiscoveryLog(LogLevelType.Info, string.Format("Completed getDSFromOracleQuery Method"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Info, string.Format("Error occured while Establishing the Connection (getDSFromOracleQuery Method)."));
            }
            
            return dsResults;
        }

        #region NotImplementedException region

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}