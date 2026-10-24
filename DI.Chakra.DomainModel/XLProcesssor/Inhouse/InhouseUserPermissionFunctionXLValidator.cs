using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class InhouseUserPermissionFunctionXLValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(InhUserPermissionFunction);
        }
        public override bool Validate(ref string validationMsg)
        {
                InhUserPermissionFunction obj = Data as InhUserPermissionFunction;

                bool isValid = true;
                List<string> missingFields = new List<string>();
                missingFields.Clear();

                var endpoint = EndPoint.GetByApplicationCode(obj.InhouseSystemCode);

                if (endpoint == null || !endpoint.IsActive || !endpoint.IsFunctionLevel)
                {
                    missingFields.Add("Invalid inhouse system code");
                    isValid = false;
                    return isValid;
                }
                if (string.IsNullOrWhiteSpace(obj.UserName) && string.IsNullOrWhiteSpace(obj.EmployeeId))
                {
                    missingFields.Add("Invalid Username/EmployeeId/NRIC");
                    isValid = false;
                }
                else if (!IsValidUsername(endpoint.Id, obj.UserName))
                {
                    missingFields.Add("Invalid User Name");
                    isValid = false;
                }
                else if (!IsValidEmployeeId(endpoint.Id, obj.EmployeeId))
                {
                    missingFields.Add("Invalid EmployeeId");
                    isValid = false;
                }
                //else if (!IsValidNRIC(endpoint.Id, obj.NRIC))
                //{
                //    missingFields.Add("Invalid NRIC");
                //    isValid = false;
                //}

                var inhousePermission = InhousePermission.GetByPermissionName(obj.Permission, obj.InhouseSystemCode);
                if (inhousePermission == null)
                {
                    missingFields.Add("Invalid permission name");
                    isValid = false;
                }

                if (endpoint != null && endpoint.CanAutomate && !string.IsNullOrWhiteSpace(obj.Functions))
                {
                    missingFields.Add("This endpoint can be automated, hence cannot have functions.");
                    isValid = false;
                }
                else if (string.IsNullOrWhiteSpace(obj.Functions))
                {
                    missingFields.Add("Invalid Function Name");
                    isValid = false;
                }
                else if (!IsFunctionsValid(obj.Permission, obj.Functions, obj.InhouseSystemCode))
                {
                    missingFields.Add("Invalid Function Name");
                    isValid = false;
                }

                int cnt = 0;
                foreach (string field in missingFields)
                {
                    if (cnt == 0)
                        validationMsg = field;
                    else if (cnt > 0 && cnt != missingFields.Count - 1)
                        validationMsg = validationMsg + ", " + field;
                    else if (cnt == missingFields.Count - 1)
                        validationMsg = validationMsg + " & " + field;
                    cnt++;
                }
                return isValid;
        }

        public bool IsFunctionsValid(string permission, string function, string applicationCode)
        {
            var entitlements = function.Split(',');
            var isExists = false;
                foreach (var entitlement in entitlements)
                {
                    if (!string.IsNullOrEmpty(entitlement.Trim()))
                    {
                        isExists = InhouseFunction.DoesExist(permission, function, applicationCode);
                        if (!isExists)
                        {
                            break;
                        }
                    }
                }
            return isExists;
        }

        public override bool IsExists(string currentUser)
        {
            InhUserPermissionFunction InhUserPermissionFunction = GetExistingFunction();
            if (InhUserPermissionFunction == null)
                return false;
            return true;
        }

        public bool IsValidUsername(long endpointSystemId, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var user = InhouseUserInfo.GetByUserName(endpointSystemId, username);
            return user != null;
        }

        public bool IsValidEmployeeId(long endpointSystemId, string employeeId)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return false;

            var user = InhouseUserInfo.GetByEmployeeId(endpointSystemId, employeeId);
            return user != null;
        }

        public bool IsValidNRIC(long endpointSystemId, string nric)
        {
            if (string.IsNullOrWhiteSpace(nric))
                return false;

            var user = InhouseUserInfo.GetByNRIC(endpointSystemId, nric);
            return user != null;
        }

        public override bool TruncateTable()
        {
            var returnValue = false;
                InhUserPermissionFunction.TruncateDI_INH_UserPermissionFunction();
                returnValue = true;
            return returnValue;
        }

        public override void Add(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InhUserPermissionFunction obj = Data as InhUserPermissionFunction;
                if (obj != null)
                {
                    var inhUser = InhouseUserInfo.GetByUsernameOrEmployeeIdOrNRIC(obj.UserName, obj.EmployeeId, obj.InhouseSystemCode);
                    obj.EndpointUserId = inhUser.Id;
                    obj.EndPointSystemId = inhUser.EndPointSystemId;
                    obj.PermissionId = InhousePermission.GetByNameAndEndpointSystemId(obj.Permission, obj.EndPointSystemId).Id;
                    if (obj.Id == 0)
                    {
                        obj.CreatedBy = currentUser;
                        obj.CreatedDate = DateTime.Now;
                    }
                    else
                    {
                        obj.LastModifiedBy = currentUser;
                        obj.LastModifiedDate = DateTime.Now;
                    }

                    if (!string.IsNullOrWhiteSpace(obj.Functions))
                    {
                        var inhFunctions = obj.Functions.Split(',');
                        foreach (var function in inhFunctions)
                        {
                            var functionObj = InhouseFunction.GetInhouseFunctionByApplicationCode(function, obj.InhouseSystemCode, obj.Permission);
                            obj.FunctionId = functionObj.Id;
                            obj.Save(dataAccessTransaction);
                        }
                    }
                    else
                        obj.Save(currentUser);
                }
                dataAccessTransaction.Commit();
            }
        }

        public override void Update(string currentUser)
        {
            Add(currentUser);
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            EndPoint INHSystem;
            if (!string.IsNullOrEmpty(dRow.ItemArray[0].ToString()))
                INHSystem = getEndPointSystemIdbyApplicationCode(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()));
            else
                INHSystem = null;
            long EPID = 0;
            if (INHSystem != null)
                EPID = INHSystem.Id;

            return new InhUserPermissionFunction
            {
                UserName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                EmployeeId = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                Permission = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                Functions = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString())
            };
        }

        public EndPoint getEndPointSystemIdbyApplicationCode(string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return EndPoint.GetByApplicationCode(value);
            }
        }

        public override void PostProcess(string currentUser)
        {
                var accountReconSaved = InhUserPermissionFunction.SaveAccountReconcilation(currentUser);
                if (accountReconSaved)
                {
                    InhUserPermissionFunction.SaveEntitlementReconcilation(currentUser);
                }
        }

        #region PrivateMethods

        private InhUserPermissionFunction GetExistingFunction()
        {
            InhUserPermissionFunction obj = Data as InhUserPermissionFunction;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return InhUserPermissionFunction.GetByObject(obj);
            }
        }
        public override string GetWhoUpdatedData()
        {
            return InhUserPermissionFunction.GetWhoUpdatedData();
        }
        #endregion
    }
}