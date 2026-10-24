using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DI.Chakra.DomainModel
{
    public class InhouseUserXLValidator : XLDataValidator
    {
        readonly List<InhousePermission> allPermissions = new List<InhousePermission>();
        readonly List<EndPoint> nonConnectedEndpoints = new List<EndPoint>();
        readonly string SplChar = "!@#$%^&*()+=[]\\\';,./{}|\":<>? ";
        readonly bool isConsiderForInActiveAccount = false;
        readonly Dictionary<long, bool> mapAllConfiguration = new Dictionary<long, bool>();
        public InhouseUserXLValidator()
        {
            var endpointsystemTypeId = (long)DI.Chakra.Infrastructure.Enums.EndpointType.InhouseSystems;
            nonConnectedEndpoints = EndPoint.Get(t => t.EndpointSystemTypeId == endpointsystemTypeId, false, 0, 0);
            allPermissions = InhousePermission.GetAll();
            isConsiderForInActiveAccount = MasterConfigHelper.ConsiderInActiveAccountForReconcilation;
            mapAllConfiguration = EndpointConfiguration.GetDictionaryByEndpointSystemTypeId(nonConnectedEndpoints.Select(x => x.Id).ToList());
        }

        public override Type GetDomainModelType()
        {
            return typeof(InhousePermission);
        }

        public override bool Validate(ref string validationMsg)
        {

            InhouseUserInfo obj = Data as InhouseUserInfo;
            bool isValid = true;
            bool isExist = false;

            List<string> missingFields = new List<string>();
            for (int i = 0; i < SplChar.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(obj.UserName))
                {
                    missingFields.Add("User Name should  not be blank");
                }
                if (obj.UserName.Trim().Contains(SplChar[i].ToString()) && string.IsNullOrEmpty(obj.UserName))
                {
                    missingFields.Add("User Name should not contain special characters");
                }
            }

            if (string.IsNullOrWhiteSpace(obj.InhouseSystemCode))
            {
                missingFields.Add("Application Code Empty");
            }

            var isValidEmployeeId = false;

            //Employee ID can't be empty
            if (string.IsNullOrWhiteSpace(obj.EmployeeId))
            {
                missingFields.Add("Employee Id should not be empty");
            }
            else
            {
                //Check if employee Id exists in user table
                User user =IsEmployeeIDValid(obj.EmployeeId);
                if (user == null)
                {
                    missingFields.Add("Employee Id Not Found");
                }
                else
                {
                    if (user != null)
                    {
                        if(user.FullTimeEmployee || (obj.AccountType == AccountType.Service || obj.AccountType == AccountType.Shared))
                        {
                            obj.ValidTo = DateTime.MaxValue;
                        }
                        else if (!user.FullTimeEmployee && user.TerminationDate.HasValue)
                        {
                            obj.ValidTo = user.TerminationDate;
                        }
                    }
                    isValidEmployeeId = true;
                }
            }

            var epId = IsInhouseSystemCodeValid(obj.InhouseSystemCode);
            isExist = epId != null;
            
            //Same employee can't hold two user accounts
            if (isValidEmployeeId && epId != null && obj.IsDeleted == false)
            {
               InhouseUserInfo inhUser = GetExistingUserAccountByEmployeeId(epId.Value); 
                if (inhUser != null && inhUser.IsLocked == false)
                    missingFields.Add($"Employee have already had a user account : {inhUser.AccountName}");
            }

            
            if (!isExist)
            {
                missingFields.Add("Application Code Not Found");
            }
            else
            {  
                if (!string.IsNullOrWhiteSpace(obj.Permission))
                {
                    var ep = nonConnectedEndpoints.FirstOrDefault(x => x.Id == epId.Value);
                    string[] entitlements = null;
                    if (ep.IsFunctionLevel)
                    {
                        entitlements = obj.Permission.Split('|');
                        foreach (var entitlement in entitlements)
                        {
                            var ent = entitlement.Split(new string[] { Constants.SPLITTER_FUCNTIONS }, StringSplitOptions.RemoveEmptyEntries);
                            var permission = ent[0].Trim();
                            var functions = ent.Length > 1 ? ent[1] : string.Empty;

                            var per = GetPermission(permission.Trim(), obj.InhouseSystemCode.Trim());
                            if (per == null)
                            {
                                missingFields.Add($"Invalid Permission Name: {permission.Trim()}");
                                break;
                            }

                            var strFunctions = InhouseFunction.GetByPermissionName(permission, obj.InhouseSystemCode);

                            foreach (var function in functions.Split(','))
                            {
                                if (!strFunctions.Contains(function))
                                {
                                    missingFields.Add($"Invalid function name or permission-function mapping not found Function: {function} Permission: {permission}");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        entitlements = obj.Permission.Split(',');
                        foreach (var entitlement in entitlements)
                        {
                            if (!string.IsNullOrWhiteSpace(entitlement))
                            {
                                var per = GetPermission(entitlement.Trim(), obj.InhouseSystemCode.Trim());
                                if (per == null)
                                {
                                    missingFields.Add($"Invalid Permission Name: {entitlement.Trim()}");
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            int cnt = 0;
            foreach (string field in missingFields)
            {
                isValid = false;
                if (cnt == 0)
                    validationMsg = field;
                else if (cnt > 0 && cnt != missingFields.Count - 1)
                    validationMsg = validationMsg + ", " + field;
                else if (cnt == missingFields.Count - 1)
                    validationMsg = validationMsg + " & " + field;
                cnt++;
            }

            if (isValid)
                obj.EndPointSystemId = Convert.ToInt64(epId);

            if (cnt > 0 && !string.IsNullOrWhiteSpace(validationMsg))
                isValid = false;
            else
                isValid = true;

            return isValid;
        }

        public long? IsInhouseSystemCodeValid(string value)
        {
            return nonConnectedEndpoints.Where(x => x.ApplicationCode.Equals(value, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()?.Id;
        }
     
        public InhousePermission GetPermission(string permissionName, string applicationCode)
        {
            return allPermissions.Where(x => x.Name.ToLower() == permissionName.ToLower() && x.InhouseSystemCode.Equals(applicationCode, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
        }

        public override bool IsExists(string currentUser)
        {
            var _existConfigMatrixAD = GetExistingUser();
            if (_existConfigMatrixAD == null)
                return false;
            if (this.Data != null)
            {
                this.Data.Id = _existConfigMatrixAD.Id;
            }
            Update(currentUser);
            return true;
        }

        public override void PostProcess(string currentUser)
        {
            var systemCodes = ValidList.Union(InvalidList).Select(a => ((InhouseUserInfo)a).InhouseSystemCode).Distinct().ToList();
            List<long> endpointIds = nonConnectedEndpoints.Where(x => systemCodes.Contains(x.ApplicationCode, StringComparer.InvariantCultureIgnoreCase)).Select(x => x.Id).ToList();

            if (!PartialUpload)
            {
                List<AccountReconcilation> accRecons = AccountReconcilation.GetAccountRecons(endpointIds, UploadDateTime);

                foreach (var accRecon in accRecons)
                {
                    InhouseUserInfo.DeleteByAccReconId(accRecon.Id, currentUser);
                }
            }

            AccountReconcilation.UpdateValidTo(endpointIds);
            try
            {
                foreach (var item in this.InvalidList)
                {
                    var usr = item as InhouseUserInfo;
                    DILog.ProvisioningLogger.WriteLog(null,usr.EndPointSystemId,LogLevelType.Error, $"InhouseSystem:{usr?.InhouseSystemCode} {usr?.UserName} {item.ValidationMessage}");
                }
            }
            catch (Exception)
            {
            }
        }

        public override bool TruncateTable()
        {
            TableName = "DI_INH_UserInfo";
            base.TruncateTable();

            TableName = "DI_INH_UserPermissionFunction";
            base.TruncateTable();

            StringBuilder cleanupQuery = new StringBuilder();

            cleanupQuery.AppendLine("DELETE FROM DI_M_EntitlementReconcilation WHERE EndpointSystemId IN (SELECT id FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId=9);");
            cleanupQuery.AppendLine("DELETE FROM DI_M_AccountReconcilation WHERE EndpointSystemId IN (SELECT ID FROM DI_M_EndpointSystem WHERE EndpointSystemTypeId=9);");

            InhouseUserInfo.ExecuteQuery(cleanupQuery.ToString());
            return true;
        }

        public override void Add(string currentUser)
        {
            InhouseUserInfo obj = Data as InhouseUserInfo;
            if (obj != null)
            {
                obj.IsLocked = obj.IsDisabled = obj.IsDeleted;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            SaveAccountReconcilationWhileUpload(obj, currentUser);
        }

        private void SaveAccountReconcilationWhileUpload(InhouseUserInfo oINH, string CurrentUserId)
        {
            var mapAll = mapAllConfiguration.ContainsKey(oINH.EndPointSystemId) ? mapAllConfiguration[oINH.EndPointSystemId] : false;
            AccountReconcilation.SaveWhileUpload(oINH, CurrentUserId, allPermissions, isConsiderForInActiveAccount, mapAll);
        }

        private InhouseUserInfo GetExistingUser()
        {
            InhouseUserInfo obj = Data as InhouseUserInfo;
            return InhouseUserInfo.GetBySimilarObject(obj);
        }

        //same employee can't hold two different user account at the same time
        private InhouseUserInfo GetExistingUserAccountByEmployeeId(long endpointId)
        {
            InhouseUserInfo obj = Data as InhouseUserInfo;
            if (obj != null)
            {
                if (obj.AccountType == AccountType.User)
                {
                    InhouseUserInfo inhUsers = InhouseUserInfo.GetAccountsByEmployeeIdAccountType(endpointId, obj.EmployeeId, obj.AccountType);
                    if (inhUsers != null && inhUsers.UserName.ToUpper() != obj.UserName.ToUpper())
                        return inhUsers;
                }
            }
            return null;
        }
        private User IsEmployeeIDValid(string value)
        {
            return User.GetByEmployeeId(value);
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            long EPID = 0;
            string strUserName = string.Empty;

            EndPoint ep = nonConnectedEndpoints.Where(x => x.ApplicationCode == CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString())).FirstOrDefault();

            EPID = ep != null ? ep.Id : 0;

            var output = new InhouseUserInfo
            {
                EndPointSystemId = EPID,
                EndPoint = ep,
                UserName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                EmployeeId = ConstructEmployeeIdIfNotProper(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString())),
                Email = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                DisplayName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                FirstName = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                LastName = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                AccountType = getAccountTypeByName(dRow.ItemArray[7].ToString()),
                AccountCreatedDate = ConvertStringToDate(dRow.ItemArray[8].ToString(), true),
                LastLoginDate = ConvertStringToDate(dRow.ItemArray[9].ToString(),true),
                IsLocked = ConvertStringToBool(dRow.ItemArray[10].ToString()),
                IsDisabled = ConvertStringToBool(dRow.ItemArray[11].ToString()),
                IsDeleted = ConvertStringToBool(dRow.ItemArray[12].ToString()),
                Remarks1 = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString()),
                Remarks2 = CheckNullAndGetValueFromCell(dRow.ItemArray[14].ToString()),
                Remarks3 = CheckNullAndGetValueFromCell(dRow.ItemArray[15].ToString()),
                Remarks4 = CheckNullAndGetValueFromCell(dRow.ItemArray[16].ToString()),
                Remarks5 = CheckNullAndGetValueFromCell(dRow.ItemArray[17].ToString()),
                Permission = CheckNullAndGetValueFromCell(dRow.ItemArray[18].ToString()),
            };

            return output;
        }
        private string ConstructEmployeeIdIfNotProper(string givenString)
        {
            givenString = givenString?.Trim();
            if (!string.IsNullOrWhiteSpace(givenString) && givenString.Length < 8)
            {
                
                Int64 empId;
                if (Int64.TryParse(givenString, out empId))
                {
                    givenString = givenString.PadLeft(8, '0');
                }
            }
            
            return givenString;
        }
        public AccountType getAccountTypeByName(string accountType)
        {
            AccountType resultEnum;
            if (string.IsNullOrEmpty(accountType))
            {
                resultEnum = AccountType.User;
            }
            else
            {
                resultEnum = UtilityMethods.GetEnumValue<AccountType>(accountType);
            }

            return resultEnum;
        }

        public override void Update(string currentUser)
        {
            InhouseUserInfo obj = Data as InhouseUserInfo;
            Cleanup(obj);
            Add(currentUser);
        }

        public override bool IsEmptyRow(IXLData data)
        {
                InhouseUserInfo _config = data as InhouseUserInfo;

                if (string.IsNullOrEmpty(_config.InhouseSystemCode + _config.UserName))
                {
                    return true;
                }
                return false;
        }

        public override string GetWhoUpdatedData()
        {
            return InhouseUserInfo.GetWhoUpdatedData();
        }

        private static void Cleanup(InhouseUserInfo oINH)
        {
            StringBuilder cleanupQuery = new StringBuilder();
            cleanupQuery.AppendLine($"UPDATE DI_M_EntitlementReconcilation SET IsDeleted=1, DeletedDate=GETDATE() WHERE EndpointUserId={oINH.Id} AND EndpointSystemId={oINH.EndPointSystemId}");
            cleanupQuery.AppendLine($"DELETE FROM DI_INH_UserPermissionFunction WHERE EndpointUserId={oINH.Id} AND EndPointSystemId={oINH.EndPointSystemId}");
            InhouseUserInfo.ExecuteQuery(cleanupQuery.ToString());
        }
    }
}
