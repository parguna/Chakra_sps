using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class InhouseFunctionXLValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(InhouseFunction);
        }
        public override bool Validate(ref string validationMsg)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                InhouseFunction obj = Data as InhouseFunction;

                bool isValid = true;
                List<string> missingFields = new List<string>();
                missingFields.Clear();

                var endpoint = EndPoint.GetByApplicationCode(obj.InhouseSystemCode);

                if (endpoint == null)
                {
                    missingFields.Add("Invalid inhouse system code");
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(obj.PermissionName))
                {
                    missingFields.Add("Invalid Permission Name");
                    isValid = false;
                }
                else if (!IsValidName(obj.PermissionName))
                {
                    missingFields.Add("Invalid Permission Name");
                    isValid = false;
                }
                else
                {
                    var inhousePermission = InhousePermission.GetByPermissionName(obj.PermissionName, obj.InhouseSystemCode);
                    if (inhousePermission == null)
                    {
                        missingFields.Add("Invalid permission name");
                        isValid = false;
                    }
                }

                if (string.IsNullOrWhiteSpace(obj.FunctionName))
                {
                    missingFields.Add("Invalid Function Name");
                    isValid = false;
                }
                else if (!IsValidName(obj.FunctionName))
                {
                    missingFields.Add("Invalid Function Name");
                    isValid = false;
                }

                if (endpoint != null && endpoint.CanAutomate)
                {
                    missingFields.Add("This endpoint can be automated, hence cannot have functions.");
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
        }

        public override bool IsExists(string currentUser)
        {
            InhouseFunction inhouseFunction = GetExistingFunction();
            if (inhouseFunction == null)
                return false;
            return true;
        }

        public bool IsValidName(string name)
        {
            string specialCharacters = @"%!@#$%^&*?/>.<,:;'\|}{~`+=" + "\"";
            char[] specialCharactersArray = specialCharacters.ToCharArray();

            int index = name.IndexOfAny(specialCharactersArray);
            if (index == -1)
                return true;
            else
                return false;
        }

        public override bool TruncateTable()
        {
            var returnValue = false;
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                InhouseFunction.TruncateDI_INH_Function();
                returnValue = true;
            }
            return returnValue;
        }

        public override void Add(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InhouseFunction obj = Data as InhouseFunction;
                if (obj != null)
                {
                    EndPoint INHSystem = getEndPointSystemIdbyApplicationCode(obj.InhouseSystemCode);
                    obj.EndpointSystemId = INHSystem.Id;
                    obj.CreatedBy = currentUser;
                    obj.CreatedDate = DateTime.Now;
                    obj.IsActive = true;
                    obj.Save(currentUser);
                }
                dataAccessTransaction.Commit();
            }
        }

        public override void Update(string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(currentUser))
            {
                InhouseFunction obj = Data as InhouseFunction;
                if (obj != null)
                {
                    EndPoint INHSystem = getEndPointSystemIdbyApplicationCode(obj.InhouseSystemCode);
                    obj.EndpointSystemId = INHSystem.Id;
                    obj.LastModifiedBy = currentUser;
                    obj.LastModifiedDate = DateTime.Now;
                    obj.IsActive = true;
                    obj.Save(currentUser);
                }
                dataAccessTransaction.Commit();
            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            return new InhouseFunction
            {
                FunctionName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                PermissionName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                Description = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                IsActive = ConvertStringToBool(dRow.ItemArray[4].ToString())
            };
        }

        public EndPoint getEndPointSystemIdbyApplicationCode(string value)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return EndPoint.GetByApplicationCode(value);
            }
        }

        #region PrivateMethods

        private InhouseFunction GetExistingFunction()
        {
            InhouseFunction obj = Data as InhouseFunction;
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return InhouseFunction.GetByObject(obj);
            }
        }
        public override void PostProcess(string currentUser)
        {
            //var systemCodes = ValidList.Select(a => ((InhousePermission)a).InhouseSystemCode).Distinct().ToList();
            //systemCodes.AddRange(InvalidList.Select(a => ((InhousePermission)a).InhouseSystemCode).Distinct().ToList());
            //systemCodes = systemCodes.Distinct().ToList();

            //foreach (var systemCode in systemCodes)
            //{
            //    EndPoint ep = EndPoint.GetByApplicationCode(systemCode);
            //    if (ep != null && ep.Id > 0)
            //    {
            //        ProcessDeletedPermissions(ep.Id, "System", UploadDateTime);
            //        ep.Save(currentUser);
            //    }
            //}
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
                }
            }
        }

        public override string GetWhoUpdatedData()
        {
            return InhouseFunction.GetWhoUpdatedData();
        }
        #endregion
    }
}
