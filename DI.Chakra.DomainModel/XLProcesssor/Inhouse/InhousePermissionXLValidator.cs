using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class InhousePermissionXLValidator : XLDataValidator
    {
        private static readonly char[] SpecialChars = ",|=>".ToCharArray();
        readonly List<EndPoint> nonConnectedEndpoints;
        public InhousePermissionXLValidator()
        {
            var endpointsystemTypeId = (long)DI.Chakra.Infrastructure.Enums.EndpointType.InhouseSystems;
            nonConnectedEndpoints = EndPoint.Get(t => t.EndpointSystemTypeId == endpointsystemTypeId, false, 0, 0);
        }

        public override bool Validate(ref string validationMsg)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                InhousePermission obj = Data as InhousePermission;

                bool isValid = true;
                bool isValidName = true;
                bool isExist = false;
                List<string> missingFields = new List<string>();
                missingFields.Clear();
                if (!IsUserValidCheck(obj.Level1Approver, obj.Level1ApproverType))
                {
                    missingFields.Add("Missing/Invalid Level1 Approver");
                }

                if (!IsUserValidCheck(obj.Level2Approver, obj.Level2ApproverType))
                {
                    missingFields.Add("Missing/Invalid Level2 Approver");
                }
                if (!IsUserValidCheck(obj.Level3Approver, obj.Level3ApproverType))
                {
                    missingFields.Add("Missing/Invalid Level3 Approver");
                }

                isExist = nonConnectedEndpoints.Any(a => a.ApplicationCode.ToLower() == obj.InhouseSystemCode.ToLower());
                if (InhousePermission.GetAllCount() > 0)
                {
                    if (!isExist)
                    {
                        isExist = !string.IsNullOrEmpty(obj.InhouseSystemCode);
                    }
                    isValid = isValid & isExist;
                    if (isExist == false)
                        missingFields.Add("Invalid InhouseSystem Code");
                }
                else
                {
                    isExist = !string.IsNullOrEmpty(obj.Name);
                    isValid = isValid & isExist;
                    if (isExist == false)
                        missingFields.Add("Invalid Permission Name");
                }
                isExist = nonConnectedEndpoints.Any(a => a.ApplicationCode.ToLower() == obj.InhouseSystemCode.ToLower());
                if (!isExist) isExist = string.IsNullOrEmpty(obj.InhouseSystemCode);
                isValid = isValid & isExist;
                isValidName = IsValidPermissionName(obj.Name);
                if (isExist == false)
                    missingFields.Add("Invalid InhouseSystem Code");

                if ((string.IsNullOrEmpty(obj.Name)) || (isValidName == false))
                {
                    missingFields.Add("Invalid Permission Name");
                    isValid = false;
                }

                var endpoint = nonConnectedEndpoints.FirstOrDefault(a => a.ApplicationCode.ToLower() == obj.InhouseSystemCode.ToLower());
                if (endpoint == null)
                {
                    missingFields.Add("Invalid Endpoint system");
                    isValid = false;
                }
                else if (endpoint.CanAutomate)
                {
                    if (string.IsNullOrWhiteSpace(obj.ADGroups) && string.IsNullOrWhiteSpace(obj.EPGroups) && string.IsNullOrWhiteSpace(obj.EPRoles)
                        && string.IsNullOrWhiteSpace(obj.CEGroups) && string.IsNullOrWhiteSpace(obj.CERoles))
                    {
                        missingFields.Add("ADGroups/EPGroups/EPRoles/CEGroups/CERoles should have value");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.ADGroups))
                {
                    if (!IsADGroupValid(obj.ADGroups))
                    {
                        missingFields.Add("Invalid ADGroups");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.EPGroups))
                {
                    if (!IsSAPEPGroupValid(obj.EPGroups))
                    {
                        missingFields.Add("Invalid EPGroups");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.EPRoles))
                {
                    if (!IsSAPEPRoleValid(obj.EPRoles))
                    {
                        missingFields.Add("Invalid EPRoles");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.CEGroups))
                {
                    if (!IsSAPEPGroupValid(obj.CEGroups))
                    {
                        missingFields.Add("Invalid CEGroups");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(obj.CERoles))
                {
                    if (!IsSAPEPRoleValid(obj.CERoles))
                    {
                        missingFields.Add("Invalid CERoles");
                        isValid = false;
                    }
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

                if (cnt > 0 && !string.IsNullOrWhiteSpace(validationMsg))
                    isValid = false;
                else
                    isValid = true;

                return isValid;
            }
        }

        public override Type GetDomainModelType()
        {
            return typeof(InhousePermission);
        }

        public override bool IsExists(string currentUser)
        {
            InhousePermission _existConfigMatrixAD = GetExistingUser();
            if (_existConfigMatrixAD == null)
                return false;
            //Update(currentUser);
            return true;
        }

        public bool IsValidPermissionName(string permissionName)
        {
            int index = permissionName.IndexOfAny(SpecialChars);
            if (index == -1)
                return true;
            else
                return false;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_INH_Permission";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InhousePermission obj = Data as InhousePermission;
                if (obj != null)
                {
                    if (obj.IsDeleted) //Decommissioning the system we should delete the details
                    {
                        // obj.Name/currentUser come from an XL-upload row / the uploading user,
                        // so they are bound as real SqlParameters ("{0}" is an EF token, not a
                        // string interpolation substitution) instead of being escaped and
                        // concatenated into the SQL text.
                        InhousePermission.ExecuteQuery($"Delete from DI_INH_Functions where EndpointSystemId = {obj.EndPointSystemId} and PermissionName ={{0}}", obj.Name);
                        InhousePermission.ExecuteQuery($"Delete from DI_INH_Permission where EndpointSystemId = {obj.EndPointSystemId} and Name ={{0}}", obj.Name);
                        InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set IsDeleted=1,DeletedDate=GETDATE(),LastModifiedDate=GETDATE(),LastModifiedBy={{0}} where EndpointSystemId = {obj.EndPointSystemId} and Entitlement ={{1}}", currentUser, obj.Name);
                    }
                    else
                    {
                        var endpoint = getEndPointSystemIdbyApplicationCode(obj.InhouseSystemCode);
                        long endpointSystemId = 0;
                        if (endpoint != null)
                            endpointSystemId = endpoint.Id;

                        obj.EndPointSystemId = endpointSystemId;
                        obj.CreatedBy = currentUser;
                        obj.CreatedDate = DateTime.Now;
                        obj.Save(currentUser);
                    }
                }
                dataAccessTransaction.Commit();
            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            EndPoint INHSystem;
            if (!string.IsNullOrEmpty(dRow.ItemArray[0].ToString()))
                INHSystem = getEndPointSystemIdbyApplicationCode(CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()));
            else
                INHSystem = null;
            long EPID = 0;
            if (INHSystem != null)
                EPID = INHSystem.Id;

            var _config = new InhousePermission
            {
                EndPointSystemId = EPID,
                Name = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                Description = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                ADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                EPGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                EPRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                CEGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                CERoles = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                IsActive = ConvertStringToBool(dRow.ItemArray[8].ToString()),
                Level1Approver = dRow.ItemArray[9].ToString(),
                Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString())),
                Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString()),
                Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString())),
                Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString()),
                Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[14].ToString())),
                IsDeleted = ConvertStringToBool(CheckNullAndGetValueFromCell(dRow.ItemArray[15].ToString())),
            };
            if (_config.Level1ApproverType == ApprovalUserType.User)
            {
                _config.Level1Approver = this.FormatEmployeeID(_config.Level1Approver);
            }
            if (_config.Level2ApproverType == ApprovalUserType.User)
            {
                _config.Level2Approver = this.FormatEmployeeID(_config.Level2Approver);
            }
            if (_config.Level3ApproverType == ApprovalUserType.User)
            {
                _config.Level3Approver = this.FormatEmployeeID(_config.Level3Approver);
            }
            return _config;
        }

        public EndPoint getEndPointSystemIdbyApplicationCode(string value)
        {
            return nonConnectedEndpoints.FirstOrDefault(a => a.ApplicationCode.ToLower() == value.ToLower());
        }

        #region PrivateMethods
        private InhousePermission GetExistingUser()
        {
            InhousePermission obj = Data as InhousePermission;
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return InhousePermission.GetByObject(obj);
            }
        }
        public override void PostProcess(string currentUser)
        {
            var systemCodes = ValidList.Select(a => ((InhousePermission)a).InhouseSystemCode).Distinct().ToList();
            systemCodes.AddRange(InvalidList.Select(a => ((InhousePermission)a).InhouseSystemCode).Distinct().ToList());
            systemCodes = systemCodes.Distinct().ToList();

            foreach (var systemCode in systemCodes)
            {
                EndPoint ep = getEndPointSystemIdbyApplicationCode(systemCode);
                if (ep != null && ep.Id > 0)
                {
                    //ProcessDeletedPermissions(ep.Id, "System", UploadDateTime);
                    InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set IsDeleted = 1,DeletedDate = GetDate() Where EndpointSystemId = {ep.Id} AND EntitlementId NOT IN(Select Id from DI_INH_Permission Where EndpointSystemId = {ep.Id}) ");
                    InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set IsDeleted = 1,DeletedDate = GetDate() Where EndpointSystemId = {ep.Id} AND Entitlement NOT IN(Select Name from DI_INH_Permission Where EndpointSystemId = {ep.Id}) ");
                    //ep.Save(currentUser);
                }
            }
            try
            {

            }
            catch (Exception ex)
            {
            }
        }

        void ProcessDeletedPermissions(long endpointId, string currentUser, DateTime runtime)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                List<InhousePermission> listDeletedPermissions = new List<InhousePermission>();

                listDeletedPermissions = InhousePermission.GetDeletedByEndPointSystemId(endpointId, runtime, currentUser).ToList();

                if (listDeletedPermissions != null && listDeletedPermissions.Count > 0)
                {
                    string cmd = "DELETE FROM [InhousePermissionInhouseFunction] WHERE InhousePermission_Id in ( " + string.Join(",", listDeletedPermissions.Select(a => a.Id)) + " )";
                    InhousePermission.ExecuteQuery(cmd);

                    cmd = "DELETE FROM [InhouseUserInfoInhousePermission] WHERE InhousePermission_Id in ( " + string.Join(",", listDeletedPermissions.Select(a => a.Id)) + " )";
                    InhousePermission.ExecuteQuery(cmd);

                    InhousePermission.DeleteList(listDeletedPermissions);

                    InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set IsDeleted = 1,DeletedDate = GetDate() Where EndpointSystemId = {endpointId} AND Entitlement NOT IN(Select Name from DI_INH_Permission Where EndpointSystemId = {endpointId}) ");
                }
            }
        }

        public override void Update(string currentUser)
        {
            InhousePermission obj = Data as InhousePermission;
            using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                long endpointSystemId = getEndPointSystemIdbyApplicationCode(obj.InhouseSystemCode).Id;
                obj.EndPointSystemId = endpointSystemId;
                InhousePermission editPerm = null;
                if(obj.Id > 0)
                {
                    editPerm = InhousePermission.GetById(obj.Id);
                }
                else
                {
                    editPerm = InhousePermission.GetByEndPoint(obj);
                }
                if (editPerm != null)
                {
                    obj.Id = editPerm.Id;
                    obj.LastModifiedDate = DateTime.Now;
                }
                if (obj.IsDeleted) //Decommissioning the system we should delete the details
                {
                    // obj.Name/obj.Description/currentUser/editPerm.Name come from an XL-upload
                    // row / the uploading user, so they are bound as real SqlParameters ("{0}" is
                    // an EF token, not a string interpolation substitution) instead of being
                    // escaped and concatenated into the SQL text.
                    InhousePermission.ExecuteQuery($"Delete from DI_INH_Functions where EndpointSystemId = {obj.EndPointSystemId} and PermissionName ={{0}}", obj.Name);
                    if(obj.Id > 0)
                        InhousePermission.ExecuteQuery($"Delete from DI_INH_UserPermissionFunction where EndPointSystemId =  {obj.EndPointSystemId} and PermissionId = {obj.Id})");

                    InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set IsDeleted=1,DeletedDate=GETDATE(),LastModifiedDate=GETDATE(),LastModifiedBy={{0}} where EndpointSystemId = {obj.EndPointSystemId} and Entitlement ={{1}}", currentUser, obj.Name);
                    InhousePermission.ExecuteQuery($"Delete from DI_INH_Permission where EndpointSystemId = {obj.EndPointSystemId} and Name ={{0}}", obj.Name);
                }
                else
                {
                    if (obj.Description == null) obj.Description = "";
                    InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set EntitlementDescription={{0}} where EndpointSystemId = {obj.EndPointSystemId} and Entitlement ={{1}}", obj.Description, obj.Name);
                    obj.Save(currentUser);
                    if (editPerm != null)
                    {
                        InhousePermission.ExecuteQuery($"Update DI_M_EntitlementReconcilation Set EntitlementDescription={{0}}, Entitlement={{1}} where EndpointSystemId = {obj.EndPointSystemId} and Entitlement ={{2}}", obj.Description, obj.Name, editPerm.Name);
                    }
                }
                dataAccessTransaction.Commit();
            }
        }

        public override string GetWhoUpdatedData()
        {
            return InhousePermission.GetWhoUpdatedData();
        }

        #endregion
    }
}
