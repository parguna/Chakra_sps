using DI.Chakra.Connector.SAPBase;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector.SAPV3
{
    public class IMProcessor : Connector.IMProcessor, IIMProcessor
    {
        public ProcessorConfig ProcessorConfig { get; set; }
        public EndpointConfiguration SAPConfiguration { get; set; }
        public EndPoint EndPoint { get; set; }
        public bool UserExists(string userName)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {


                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", userName);

                IRfcFunction fnUserExists = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_EXISTENCE_CHECK", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteStructure(fnUserExists, "RETURN");


                if (result.GetValue("MESSAGE").ToString().ToUpper().Contains("NOT EXIST"))
                    return false;

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }
        public override ProvisioningResult CreateAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {


                //string UserLogonName = fields["Username"].Value;
                string UserLogonName = fields["AccountName"].Value;


                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", UserLogonName);

                IRfcFunction fnCreate = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CREATE", dicParams);
                IRfcFunction fnAssignProfile = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_PROFILES_ASSIGN", dicParams);
                IRfcStructure structPassword = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "PASSWORD");
                IRfcStructure structLogon = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "LOGONDATA");
                IRfcStructure structAddress = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "ADDRESS");
                IRfcStructure structDefaults = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "DEFAULTS");
                IRfcTable tblParameters = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_CREATE", dicParams, "PARAMETER");
                IRfcTable tblProfiles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_PROFILES_ASSIGN", dicParams, "PROFILES");
                IRfcStructure structLicense = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CREATE", dicParams, "UCLASS");

                structPassword.SetValue("BAPIPWD", fields["Password"].Value);
                fnCreate.SetValue("PASSWORD", structPassword);

                if (!string.IsNullOrEmpty(fields["Lastname"].Value))
                {
                    structAddress.SetValue("Lastname", fields["Lastname"].Value); // Last Name
                    // the nsme format will take the value in 
                }
                fnCreate.SetValue("ADDRESS", structAddress);

                // License Type - SAP Employee
                structLicense.SetValue("LIC_TYPE", "CD");
                fnCreate.SetValue("UCLASS", structLicense);

                //NOTE: Set DateFormat field to value 1 for DD.MM.YYYY
                //NOTE: Set TimeFormat field to value 0 for 12 Hour Format(Example: 12:05:10 PM)
                // NewUser_DecimalNotation to 'X' for 1,234,567.89

                if (!string.IsNullOrEmpty(fields["DateFormat"].Value))
                {
                    structDefaults.SetValue("Datfm", fields["DateFormat"].Value);
                }
                if (!string.IsNullOrEmpty(fields["TimeFormat"].Value))
                {
                    structDefaults.SetValue("Timefm", fields["TimeFormat"].Value);
                }

                if (!string.IsNullOrEmpty(fields["NewUser_DecimalNotation"].Value))
                {
                    structDefaults.SetValue("DCPFM", fields["NewUser_DecimalNotation"].Value);
                }


                fnCreate.SetValue("DEFAULTS", structDefaults);

                //if (!string.IsNullOrEmpty(fields["TimeZone"].Value))
                //{
                //    structLogon.SetValue("Tzone", fields["TimeZone"].Value);
                //}

                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                {
                    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                }
                else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                structLogon.SetValue("Gltgb", expirationDate);
                structLogon.SetValue("Gltgv", startDate); // DateTime.Today.ToString(ProvisioningConstants.SAPDateFormater                

                var userType = fields["UserType"].Value;
                if (!String.IsNullOrWhiteSpace(userType))
                {
                    structLogon.SetValue("Ustyp", userType);
                }
                //if (!string.IsNullOrEmpty(fields["AuthorizationGroup"].Value))
                //{
                //    structLogon.SetValue("Class", fields["AuthorizationGroup"].Value);
                //}

                fnCreate.SetValue("LOGONDATA", structLogon);
                var createResult = ProcessorConfig.sapUtil.ExecuteTable(fnCreate, "RETURN");

                if (createResult.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                bool success = false;
                string errorMessage = string.Empty;
                for (int i = 0; i < createResult.Rows.Count; i++)
                {
                    if (createResult.Rows[i]["TYPE"].ToString() == "S")
                        success = true;
                    errorMessage += String.Format("Message {0}{1};", i, createResult.Rows[i]["MESSAGE"].ToString());
                }

                if (!success)
                {
                    return new ProvisioningResult { Result = false, Message = errorMessage };
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "User Created" };
        }

        private ProvisioningResult ResetAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                // remove all roles
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRfcRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                var tblRoles = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                var sapRoles = fields["Roles"].Value;
                List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList().ConvertAll(t => t.ToUpper());

                // clean all existing roles
                ProcessorConfig.sapUtil.DeleteAllRowsInTable(fnRolesAssign, tblRfcRoles, "ACTIVITYGROUPS");

                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                // DON'T USE VALID TO FOR SAP RESET AS THIS IS FOR PERMANENT EMP ABC ACCESS
                //if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                //{
                //    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                //}
                //else 
                if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                // append only seed roles
                foreach (var rowRole in lstRoles)
                {
                    tblRfcRoles.Append();
                    tblRfcRoles.SetValue("From_Dat", startDate);
                    tblRfcRoles.SetValue("To_Dat", expirationDate);
                    tblRfcRoles.SetValue("Agr_Name", rowRole.ToString());
                }
                fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRfcRoles);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };
                }

                foreach (DataRow row in result.Rows)
                {
                    if (row["TYPE"].ToString() == "S")
                    {
                        return new ProvisioningResult { Result = true, Message = "Account reset to default roles." };
                    }
                }

                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public override ProvisioningResult DeleteAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                // if this account type is not included in the account types allowed to be deleted via Account Delete
                if ((fields["AccountType"] != null))
                    return new ProvisioningResult()
                    {
                        Result = true,
                        Message = string.Format("Account Name: {0} - Account Delete not allowed for Account Type - {1}.",
                        fields["AccountName"].Value, fields["AccountType"].Value)
                    };

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                bool isEmployee = false;
                string requestSource = string.Empty;
                if (fields["RequestSource"] != null && !String.IsNullOrWhiteSpace(fields["RequestSource"].Value))
                {
                    requestSource = fields["RequestSource"].Value;
                }

                // if ad hoc delete request
                if (requestSource.ToUpper() == "AdhocRequest".ToUpper() && !String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                {
                    Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                    if (isEmployee)
                    {
                        // if full time employee, reset the account
                        var provResult = ResetAccount(fields);

                        // if role added
                        if (provResult.Result)
                        {
                            var accountName = fields["AccountName"].Value;
                            var sapSystemRoles = fields[Constants.SAP_System_Roles_Field].Value;
                            // update user type if required
                            CheckAccountUserType(accountName, sapSystemRoles, fields);
                        }

                        return provResult;
                    }
                }

                // if not adhoc request for permanent employee
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDel = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_DELETE", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                foreach (DataRow row in result.Rows)
                {
                    if (String.Compare(row["MESSAGE"].ToString(), String.Format("User {0} does not exist", fields["AccountName"].Value.ToString()), true) == 0)
                    {
                        return new ProvisioningResult { Result = true, Message = "User Deleted" };
                    }
                }

                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "User Deleted" };
        }

        public override ProvisioningResult LockAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDel = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_LOCK", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }

            return new ProvisioningResult { Result = true, Message = "User Locked" };
        }

        public override ProvisioningResult DisableAccount(ProvisioningParamCollection fields)
        {
            // if this account type is not included in the account types allowed to be disabled via Account Disable
            if ((fields["AccountType"] != null))
                return new ProvisioningResult()
                {
                    Result = true,
                    Message = string.Format("Account Name: {0} - Account Disable not allowed for Account Type - {1}.",
                    fields["AccountName"].Value, fields["AccountType"].Value)
                };

            var result = LockAccount(fields);
            if (result != null)
            {
                if (result.Result == true)
                {
                    result.Message = "Account disabled.";
                }
            }
            return result;
        }

        public override ProvisioningResult UnLockAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDel = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_UNLOCK", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "User unlocked" };
        }

        public override ProvisioningResult EnableAccount(ProvisioningParamCollection fields)
        {
            try
            {
                UnLockAccount(fields);
            }
            catch (Exception ex)
            {
                return new ProvisioningResult() { Result = false, Message = ex.Message };
            }
            return new ProvisioningResult() { Result = true, Message = string.Format("account {0} disabled in SAP.", fields["AccountName"].Value) };
        }

        public ProvisioningResult AddGroup(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();
            try
            {
                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcTable tblGroup = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_CHANGE", dicParams, "GROUPS");
                IRfcStructure structGroupX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "GROUPSX");

                var sapGroups = fields["Groups"].Value;

                List<string> lstGroups = string.IsNullOrEmpty(sapGroups) ? new List<string>() : sapGroups.Split(',').ToList();

                foreach (string item in lstGroups)
                {
                    tblGroup.Append();
                    tblGroup.SetValue("Usergroup", item);
                    structGroupX.SetValue("Usergroup", "X");
                }

                fnUserChange.SetValue("GROUPS", tblGroup);
                fnUserChange.SetValue("GROUPSX", structGroupX);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");


                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }

            }
            catch (Exception ex)
            {

                throw;
            }

            return new ProvisioningResult { Result = true, Message = "Group Assignment Completed" };
        }
        public ProvisioningResult AddProfile(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {


                string UserLogonName = fields["AccountName"].Value;

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRfcProfiles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "PROFILES");
                IRfcFunction fnAssignProfile = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_PROFILES_ASSIGN", dicParams);

                var sapProfiles = fields["Profiles"].Value;
                List<string> lstProfiles = string.IsNullOrEmpty(sapProfiles) ? new List<string>() : sapProfiles.Split(',').ToList();

                foreach (string profile in lstProfiles)
                {
                    tblRfcProfiles.Append();
                    tblRfcProfiles.SetValue("Bapiprof", profile);
                }
                fnAssignProfile.SetValue("PROFILES", tblRfcProfiles);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnAssignProfile, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }


            }



            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "User Created" };
        }

        public ProvisioningResult AddRole(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                var sapRoles = String.IsNullOrEmpty(fields["Roles"].Value) ? String.Empty : fields["Roles"].Value;

                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                {
                    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                }
                else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList();

                //foreach (string role in lstRoles)
                //{
                //    if (!String.IsNullOrWhiteSpace(role))
                //    {
                //        tblRoles.Append();
                //        tblRoles.SetValue("From_Dat", startDate);
                //        tblRoles.SetValue("To_Dat", expirationDate);
                //        tblRoles.SetValue("Agr_Name", role);
                //    }
                //}

                foreach (string role in lstRoles)
                {
                    if (String.IsNullOrWhiteSpace(role))
                        continue;

                    var existingRole = tblRoles.Where(x => string.Compare(x.GetString("Agr_Name"), role, true) == 0).FirstOrDefault();
                    if (existingRole != null)
                    {
                        existingRole.SetValue("From_Dat", startDate);
                        existingRole.SetValue("To_Dat", expirationDate);
                    }
                    else
                    {
                        tblRoles.Append();
                        tblRoles.SetValue("From_Dat", startDate);
                        tblRoles.SetValue("To_Dat", expirationDate);
                        tblRoles.SetValue("Agr_Name", role);
                    }

                }

                fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRoles);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Role Assignment Completed" };
        }
        public override ProvisioningResult AddEntitlement(ProvisioningParamCollection fields)
        {
            var sapRoles = fields["Roles"].Value;
            var userType = string.Empty;
            if (!string.IsNullOrEmpty(fields["Roles"].Value))
            {
                //if (!string.IsNullOrEmpty(fields["SAPUserType"].Value))
                //{
                //    userType = fields["SAPUserType"].Value;
                //}

                var result = AddRole(fields);

                // if role added
                if (result.Result)
                {
                    var accountName = fields["AccountName"].Value;
                    var sapSystemRoles = fields[Constants.SAP_System_Roles_Field].Value;
                    // update user type if required
                    CheckAccountUserType(accountName, sapSystemRoles, fields);
                }

                return result;
            }
            //if (!string.IsNullOrEmpty(fields["Groups"].Value))
            //{
            //var groupResult = AddGroup(fields);


            //}
            //if (!string.IsNullOrEmpty(fields["Profiles"].Value))
            //{
            //    var profResult = AddProfile(fields);
            //}

            //return null;
            return new ProvisioningResult { Result = true, Message = "Adding Entitlement Completed" };
        }
        public ProvisioningResult RemoveRole(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                if ((fields["AccountType"] != null) && (fields["AccountType"].Value.ToString() != AccountType.User.ToString()))
                    return new ProvisioningResult() { Result = true, Message = string.Format("Cannot delete access for Non-User account - {0}.", fields["AccountName"].Value) };

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRfcRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                var tblRoles = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                var sapRoles = fields["Roles"].Value;
                List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList().ConvertAll(t => t.ToUpper());

                ProcessorConfig.sapUtil.DeleteAllRowsInTable(fnRolesAssign, tblRfcRoles, "ACTIVITYGROUPS");

                foreach (DataRow rowRole in tblRoles.Rows)
                {
                    if (lstRoles.Contains(rowRole["Agr_Name"].ToString().ToUpper()))
                        continue;

                    tblRfcRoles.Append();
                    tblRfcRoles.SetValue("From_Dat", rowRole["From_Dat"].ToString());
                    tblRfcRoles.SetValue("To_Dat", rowRole["To_Dat"].ToString());
                    tblRfcRoles.SetValue("Agr_Name", rowRole["Agr_Name"].ToString());
                }

                fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRfcRoles);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Role Assignment Completed" };
        }

        public ProvisioningResult RemoveProfile(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblProfiles = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "PROFILES");
                var tblRfcProfiles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "PROFILES");
                IRfcFunction fnAssignProfile = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_PROFILES_ASSIGN", dicParams);


                var sapProfiles = fields["Profiles"].Value;
                List<string> lstProfiles = string.IsNullOrEmpty(sapProfiles) ? new List<string>() : sapProfiles.Split(',').ToList().ConvertAll(t => t.ToUpper());

                ProcessorConfig.sapUtil.DeleteAllRowsInTable(fnAssignProfile, tblRfcProfiles, "PROFILES");

                foreach (DataRow rowRole in tblProfiles.Rows)
                {
                    if (lstProfiles.Contains(rowRole["Bapiprof"].ToString().ToUpper()))
                        continue;

                    tblRfcProfiles.Append();
                    tblRfcProfiles.SetValue("Bapiprof", rowRole["Bapiprof"].ToString());
                }

                fnAssignProfile.SetValue("PROFILES", tblRfcProfiles);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnAssignProfile, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Profile Removal Completed" };
        }
        public override ProvisioningResult RemoveEntitlement(ProvisioningParamCollection fields)
        {
            // if this account type is not included in the account types allowed to be deleted via Account Delete
            if ((fields["AccountType"] != null))
                return new ProvisioningResult()
                {
                    Result = true,
                    Message = string.Format("Account Name: {0} - RemoveEntitlement not allowed for Account Type - {1}.",
                    fields["AccountName"].Value, fields["AccountType"].Value)
                };

            var sapRoles = fields["Roles"].Value;
            ProvisioningResult roleResult = null;

            if (!string.IsNullOrEmpty(fields["Roles"].Value))
            {
                roleResult = RemoveRole(fields);

                // if role removed
                if (roleResult.Result)
                {
                    var accountName = fields["AccountName"].Value;
                    var sapSystemRoles = fields[Constants.SAP_System_Roles_Field].Value;
                    CheckAccountUserType(accountName, sapSystemRoles, fields);
                }

                return roleResult;
            }
            else
            {
                roleResult = new ProvisioningResult { Result = true, Message = "Role Assignment Completed - No roles selected" };
            }
            //if (!string.IsNullOrEmpty(fields["Groups"].Value))
            //{
            //    var groupResult = RemoveGroup(fields);
            //}
            //if (!string.IsNullOrEmpty(fields["Profiles"].Value))
            //{
            //    var profResult = RemoveProfile(fields);
            //}

            return roleResult;
        }
        public ProvisioningResult RemoveGroup(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                var tblRfcGroups = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "GROUPS");
                var tblGroups = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "GROUPS");
                IRfcStructure structGroupX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "GROUPSX");
                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);

                var sapGroups = fields["Groups"].Value;
                List<string> lstRoles = string.IsNullOrEmpty(sapGroups) ? new List<string>() : sapGroups.Split(',').ToList().ConvertAll(t => t.ToUpper());

                ProcessorConfig.sapUtil.DeleteAllRowsInTable(fnUserChange, tblRfcGroups, "GROUPS");

                foreach (DataRow rowRole in tblGroups.Rows)
                {
                    if (lstRoles.Contains(rowRole["Usergroup"].ToString().ToUpper()))
                        continue;

                    tblRfcGroups.Append();
                    tblRfcGroups.SetValue("Usergroup", rowRole["Usergroup"].ToString());
                    structGroupX.SetValue("Usergroup", "X");
                }

                fnUserChange.SetValue("GROUPS", tblRfcGroups);
                fnUserChange.SetValue("GROUPSX", structGroupX);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Group Removal Completed" };
        }
        public override ProvisioningResult ResetPassword(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structPwd = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORD");
                IRfcStructure structPwdX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "PASSWORDX");

                structPwd.SetValue("Bapipwd", fields["Password"].Value);
                structPwdX.SetValue("Bapipwd", "X");

                fnUserChange.SetValue("PASSWORD", structPwd);
                fnUserChange.SetValue("PASSWORDX", structPwdX);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Password reset done" };
        }

        public override ProvisioningResult UpdateAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structAddress = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "ADDRESS");
                IRfcStructure structAddressX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "ADDRESSX");
                IRfcStructure structDefaults = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "DEFAULTS");
                IRfcStructure structDefaultsX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "DEFAULTSX");
                IRfcStructure structLogondata = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");

                if (!string.IsNullOrEmpty(fields["Lastname"].Value))
                {
                    structAddress.SetValue("Lastname", fields["Lastname"].Value);
                    structAddressX.SetValue("Lastname", "X");
                }

                fnUserChange.SetValue("ADDRESS", structAddress);
                fnUserChange.SetValue("ADDRESSX", structAddressX);

                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                {
                    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                }
                else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                if (!string.IsNullOrEmpty(fields["UserValidFrom"].Value))
                {
                    structLogondata.SetValue("Gltgv", startDate);
                    structLogondataX.SetValue("Gltgv", "X");
                }
                if (!string.IsNullOrEmpty(fields["UserValidTo"].Value))
                {
                    structLogondata.SetValue("Gltgb", expirationDate);
                    structLogondataX.SetValue("Gltgb", "X");
                }
                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "SAP User Update done." };
        }

        public override ProvisioningResult UpdateAccountValidity(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structLogondata = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");
                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                {
                    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                }
                else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                if (!string.IsNullOrEmpty(fields["UserValidFrom"].Value))
                {
                    DateTime dtValidFromDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                    structLogondata.SetValue("Gltgv", dtValidFromDate);
                    structLogondataX.SetValue("Gltgv", "X");
                }
                if (!string.IsNullOrEmpty(fields["UserValidTo"].Value))
                {
                    DateTime dtValidToDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                    structLogondata.SetValue("Gltgb", dtValidToDate);
                    structLogondataX.SetValue("Gltgb", "X");
                }
                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function returned null" };

                foreach (DataRow row in result.Rows)
                {
                    if (String.Compare(row["TYPE"].ToString(), "S", true) == 0)
                    {
                        return new ProvisioningResult { Result = true, Message = "SAP User validity update done" };
                    }
                }
                //if (result.Rows[0]["TYPE"].ToString() != "S")
                // if the above for each does not show success, return error
                return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "SAP User validity update done" };
        }

        public override ProvisioningResult UpdateEntitlementValidity(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                    return new ProvisioningResult { Result = false, Message = "User not found" };
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                var tblRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);



                var sapRoles = fields["Roles"].Value;
                List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList();

                // default now date
                DateTime startDate = DateTime.Now;

                // if valid from is set, use the value
                if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                {
                    startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                }
                // if join date is available, use that value
                else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                {
                    startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                }

                fields["UserValidFrom"].Value = startDate.ToString();

                // default max date
                DateTime expirationDate = new DateTime(9999, 12, 31);

                if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                {
                    expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                }
                else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                {
                    expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                    bool isEmployee = false;
                    if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                    {
                        Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                        if (isEmployee)
                        {
                            // if full time employee, set termination date - 1
                            expirationDate.AddDays(-1);
                        }
                    }
                }
                fields["UserValidTo"].Value = expirationDate.ToString();

                bool hasUpdated = false;

                foreach (string role in lstRoles)
                {
                    var matchedRole = tblRoles.ToList().FirstOrDefault(item => item.GetValue("Agr_Name").ToString() == role);
                    if (matchedRole != null)
                    {
                        matchedRole.SetValue("To_Dat", expirationDate);
                        hasUpdated = true;
                    }
                }

                fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRoles);
                var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                if (!hasUpdated)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Could not find role to update.");
                    return new ProvisioningResult { Result = false, Message = "Could not find role to update." };
                }

                if (result.Rows.Count <= 0)
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                }


                if (result.Rows[0]["TYPE"].ToString() != "S")
                {
                    DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                }

            }


            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "Role Validity Update Completed" };
        }

        /// <summary>
        /// Update the UserType of Account to Dialog (A) or System (B)
        /// </summary>
        /// <returns>true, if update successfully; else false.</returns>
        public bool UpdateAccountUserType(string accountName, string userType, ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(accountName);

                if (exists == false)
                {
                    return false;
                }

                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", accountName);

                IRfcFunction fnUserChange = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_CHANGE", dicParams);
                IRfcStructure structLogondata = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATA");
                IRfcStructure structLogondataX = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_CHANGE", dicParams, "LOGONDATAX");

                if (!string.IsNullOrEmpty(userType))
                {
                    structLogondata.SetValue("Ustyp", userType);
                    structLogondataX.SetValue("Ustyp", "X");
                }

                fnUserChange.SetValue("LOGONDATA", structLogondata);
                fnUserChange.SetValue("LOGONDATAX", structLogondataX);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserChange, "RETURN");

                if (result.Rows.Count <= 0)
                    return false;


                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return false;

                fields[Constants.SAPUserType].Value = userType;
            }


            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// Update the UserType of Account to Dialog (A) or System (B)
        /// </summary>
        /// <returns>true, if update successfully; else false.</returns>
        public bool CheckAccountUserType(string accountName, string sapSystemRoles, ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {

                var exists = UserExists(accountName);

                if (exists == false)
                {
                    return false;
                }

                // FIND USER TYPE
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", accountName);

                // get logon data to get user Type
                IRfcStructure structLogonData = ProcessorConfig.sapUtil.getRfcStructure("BAPI_USER_GET_DETAIL", dicParams, "LOGONDATA");
                var dtSAPUserRoleCodes = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");

                // get user type
                var userType = structLogonData.GetValue("Ustyp").ToString().Trim();

                if (dtSAPUserRoleCodes != null && !string.IsNullOrEmpty(sapSystemRoles))
                {
                    bool isSystemUser = true;
                    List<string> lstSystemRoles = string.IsNullOrEmpty(sapSystemRoles) ? new List<string>() : sapSystemRoles.Split(',').ToList();
                    if (dtSAPUserRoleCodes.Rows.Count > lstSystemRoles.Count)
                    {
                        isSystemUser = false;
                    }
                    else
                    {
                        for (int role = 0; role < dtSAPUserRoleCodes.Rows.Count; role++)
                        {
                            var objRole = dtSAPUserRoleCodes.Rows[role]["Agr_Name"].ToString().Trim();
                            if (objRole != null)
                            {
                                if (!lstSystemRoles.Any(x => x == objRole))
                                {
                                    // if the assigned role is not a system role
                                    isSystemUser = false;
                                    break;
                                }
                            }
                        }
                    }

                    // setting the user to System or Dialog type based on roles given
                    if (isSystemUser)
                    {
                        if (userType == Constants.SAP_Dialog_User_Type)
                        {
                            // set dialog user
                            UpdateAccountUserType(accountName, Constants.SAP_System_User_Type, fields);
                            return true;
                        }
                    }
                    else
                    {
                        if (userType == Constants.SAP_System_User_Type)
                        {
                            // set dialog user
                            UpdateAccountUserType(accountName, Constants.SAP_Dialog_User_Type, fields);
                            return true;
                        }
                    }

                    fields[Constants.SAPUserType].Value = userType;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }

        public ProvisioningResult AddHODRoles(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                var userIsHod = false;
                if (!String.IsNullOrEmpty(fields[Constants.Is_HOD].Value))
                {
                    Boolean.TryParse(fields[Constants.Is_HOD].Value, out userIsHod);
                }
                // if HOD user
                if (userIsHod)
                {
                    var exists = UserExists(fields["AccountName"].Value.ToString());

                    if (exists == false)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "User not found");
                        return new ProvisioningResult { Result = false, Message = "User not found" };
                    }

                    dicParams = new Dictionary<string, object>();
                    dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                    IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                    var tblRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                    IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                    var sapRoles = String.IsNullOrEmpty(fields[Constants.SAP_HOD_Roles_Field].Value) ? String.Empty : fields[Constants.SAP_HOD_Roles_Field].Value;

                    // default now date
                    DateTime startDate = DateTime.Now;

                    // if valid from is set, use the value
                    if ((fields["UserValidFrom"] != null && UtilityMethods.CheckDate(fields["UserValidFrom"].Value)))
                    {
                        startDate = Convert.ToDateTime(fields["UserValidFrom"].Value);
                    }
                    // if join date is available, use that value
                    else if (fields["UserJoinDate"] != null && UtilityMethods.CheckDate(fields["UserJoinDate"].Value))
                    {
                        startDate = Convert.ToDateTime(fields["UserJoinDate"].Value);
                    }

                    fields["UserValidFrom"].Value = startDate.ToString();

                    // default max date
                    DateTime expirationDate = new DateTime(9999, 12, 31);

                    if ((fields["UserValidTo"] != null && UtilityMethods.CheckDate(fields["UserValidTo"].Value)))
                    {
                        expirationDate = Convert.ToDateTime(fields["UserValidTo"].Value);
                    }
                    else if (fields["UserTerminationDate"] != null && UtilityMethods.CheckDate(fields["UserTerminationDate"].Value))
                    {
                        expirationDate = Convert.ToDateTime(fields["UserTerminationDate"].Value);

                        bool isEmployee = false;
                        if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                        {
                            Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                            if (isEmployee)
                            {
                                // if full time employee, set termination date - 1
                                expirationDate.AddDays(-1);
                            }
                        }
                    }
                    fields["UserValidTo"].Value = expirationDate.ToString();

                    List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList();

                    foreach (string role in lstRoles)
                    {
                        if (!String.IsNullOrWhiteSpace(role))
                        {
                            tblRoles.Append();
                            tblRoles.SetValue("From_Dat", startDate);
                            tblRoles.SetValue("To_Dat", expirationDate);
                            tblRoles.SetValue("Agr_Name", role);
                        }
                    }
                    fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRoles);
                    var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                    if (result.Rows.Count <= 0)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Function return null");
                        return new ProvisioningResult { Result = false, Message = "Function return null" };

                    }


                    if (result.Rows[0]["TYPE"].ToString() != "S")
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                        return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                    }

                    return new ProvisioningResult { Result = true, Message = "HOD Role Assignment Completed" };
                }
                else
                {
                    return new ProvisioningResult { Result = true, Message = "HOD Role Assignment Not Required." };
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ProvisioningResult AttestationDeleteAccount(ProvisioningParamCollection fields)
        {
            Dictionary<string, object> dicParams = new Dictionary<string, object>();

            try
            {
                // if this account type is not included in the account types allowed to be deleted via Attestation Remediation
                if ((fields["AccountType"] != null))
                    return new ProvisioningResult()
                    {
                        Result = true,
                        Message = string.Format("Account Name: {0} - Attestation Remediation not allowed for Account Type - {1}.",
                        fields["AccountName"].Value, fields["AccountType"].Value)
                    };

                var exists = UserExists(fields["AccountName"].Value.ToString());

                if (exists == false)
                {
                    return new ProvisioningResult { Result = false, Message = "SAP: User not found" };
                }

                bool isEmployee = false;
                string requestSource = string.Empty;
                if (fields["RequestSource"] != null && !String.IsNullOrWhiteSpace(fields["RequestSource"].Value))
                {
                    requestSource = fields["RequestSource"].Value;
                }

                // if ad hoc delete request
                if (!String.IsNullOrWhiteSpace(fields["UserFullTimeEmployee"].Value))
                {
                    Boolean.TryParse(fields["UserFullTimeEmployee"].Value, out isEmployee);
                    // if employee
                    if (isEmployee)
                    {
                        // if user account
                        if ((fields["AccountType"] != null) && (fields["AccountType"].Value.ToString() == AccountType.User.ToString()))
                        {
                            // if full time employee, reset the account
                            var provResult = ResetAccount(fields);

                            // if role added
                            if (provResult.Result)
                            {
                                var accountName = fields["AccountName"].Value;
                                var sapSystemRoles = fields[Constants.SAP_System_Roles_Field].Value;
                                // update user type if required
                                CheckAccountUserType(accountName, sapSystemRoles, fields);
                            }

                            return provResult;
                        }
                    }
                }

                // if not adhoc request for permanent employee
                dicParams = new Dictionary<string, object>();
                dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                IRfcFunction fnUserDel = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_DELETE", dicParams);

                var result = ProcessorConfig.sapUtil.ExecuteTable(fnUserDel, "RETURN");

                if (result.Rows.Count <= 0)
                    return new ProvisioningResult { Result = false, Message = "Function return null" };

                foreach (DataRow row in result.Rows)
                {
                    if (String.Compare(row["MESSAGE"].ToString(), String.Format("User {0} does not exist", fields["AccountName"].Value.ToString()), true) == 0)
                    {
                        return new ProvisioningResult { Result = true, Message = "User Deleted in SAP" };
                    }
                }

                if (result.Rows[0]["TYPE"].ToString() != "S")
                    return new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new ProvisioningResult { Result = true, Message = "User Deleted in SAP" };
        }

        public ProvisioningResult AttestationRemoveEntitlement(ProvisioningParamCollection fields)
        {
            ProvisioningResult roleResult = null;

            if (!string.IsNullOrEmpty(fields["Roles"].Value))
            {
                Dictionary<string, object> dicParams = new Dictionary<string, object>();

                try
                {

                    // if this account type is not included in the account types allowed to be deleted via Attestation Remediation
                    if ((fields["AccountType"] != null))
                        return new ProvisioningResult()
                        {
                            Result = true,
                            Message = string.Format("Account Name: {0} - Attestation Remediation not allowed for Account Type - {1}.",
                            fields["AccountName"].Value, fields["AccountType"].Value)
                        };

                    var exists = UserExists(fields["AccountName"].Value.ToString());

                    if (exists == false)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "SAP: User not found");
                        roleResult = new ProvisioningResult { Result = false, Message = "SAP: User not found" };
                    }

                    dicParams = new Dictionary<string, object>();
                    dicParams.Add("USERNAME", fields["AccountName"].Value.ToString());

                    IRfcFunction fnUserDetail = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_GET_DETAIL", dicParams);
                    var tblRfcRoles = ProcessorConfig.sapUtil.getRfcTable("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                    var tblRoles = ProcessorConfig.sapUtil.getTableFromRfcFunction("BAPI_USER_GET_DETAIL", dicParams, "ACTIVITYGROUPS");
                    IRfcFunction fnRolesAssign = ProcessorConfig.sapUtil.getRfcFunction("BAPI_USER_ACTGROUPS_ASSIGN", dicParams);

                    var sapRoles = fields["Roles"].Value;
                    List<string> lstRoles = string.IsNullOrEmpty(sapRoles) ? new List<string>() : sapRoles.Split(',').ToList().ConvertAll(t => t.ToUpper());

                    ProcessorConfig.sapUtil.DeleteAllRowsInTable(fnRolesAssign, tblRfcRoles, "ACTIVITYGROUPS");

                    foreach (DataRow rowRole in tblRoles.Rows)
                    {
                        if (lstRoles.Contains(rowRole["Agr_Name"].ToString().ToUpper()))
                            continue;

                        tblRfcRoles.Append();
                        tblRfcRoles.SetValue("From_Dat", rowRole["From_Dat"].ToString());
                        tblRfcRoles.SetValue("To_Dat", rowRole["To_Dat"].ToString());
                        tblRfcRoles.SetValue("Agr_Name", rowRole["Agr_Name"].ToString());
                    }

                    fnRolesAssign.SetValue("ACTIVITYGROUPS", tblRfcRoles);
                    var result = ProcessorConfig.sapUtil.ExecuteTable(fnRolesAssign, "RETURN");

                    if (result.Rows.Count <= 0)
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "SAP: Function returned null");
                        roleResult = new ProvisioningResult { Result = false, Message = "SAP: Function returned null" };

                    }


                    if (result.Rows[0]["TYPE"].ToString() != "S")
                    {
                        DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, result.Rows[0]["MESSAGE"].ToString());
                        roleResult = new ProvisioningResult { Result = false, Message = result.Rows[0]["MESSAGE"].ToString() };

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                roleResult = new ProvisioningResult { Result = true, Message = "SAP: Role Assignment Completed" };

                // if role removed
                if (roleResult.Result)
                {
                    var accountName = fields["AccountName"].Value;
                    var sapSystemRoles = fields[Constants.SAP_System_Roles_Field].Value;
                    CheckAccountUserType(accountName, sapSystemRoles, fields);
                }

                return roleResult;
            }
            else
            {
                roleResult = new ProvisioningResult { Result = true, Message = "SAP: Role Assignment Completed - No roles selected" };
            }

            return roleResult;


        }
    }
}
