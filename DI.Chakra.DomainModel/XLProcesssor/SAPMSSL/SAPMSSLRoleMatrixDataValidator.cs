using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPMSSLRoleMatrixDataValidator: XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPXLMSSLRolesMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPXLMSSLRolesMatrix _config = null;
            try
            {
                string val = string.Empty;

                _config = new SAPXLMSSLRolesMatrix();

                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Module = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.SubModule = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.RoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(val);

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
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPXLMSSLRolesMatrix _config = data as SAPXLMSSLRolesMatrix;

            if (string.IsNullOrEmpty(_config.Remarks + _config.Application + _config.Module + _config.SubModule + _config.RoleName + _config.TechnicalRole
                   + _config.Level1Approver + _config.Level2Approver + _config.Level3Approver))
            {
                if (_config.Level1ApproverType == null && _config.Level2ApproverType == null && _config.Level3ApproverType == null)
                    return true;
            }

            return false;
        }

        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SAPXLMSSLRolesMatrix obj = data as SAPXLMSSLRolesMatrix;
            ApplicationType applicationType = obj.Application == Constants.MSSLMCP ? ApplicationType.MSSL_CRM : obj.Application == Constants.MSSLMSP ? ApplicationType.MSSL_ISU : ApplicationType.MSSL_ISU;
            long endpointSystemId = GetEndpointSystemID(applicationType);
            validationMsg = string.Empty;
            bool isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsSAPRoleValid(endpointSystemId, obj.TechnicalRole);
            if (!isExist)
            {
                validationMsg = $"Found Invalid Technical role for the business role - {obj.BusinessRole}";
            }

            return validationMsg == string.Empty;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPXLMSSLRolesMatrix obj = this.Data as SAPXLMSSLRolesMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            ApplicationType applicationType = obj.Application == Constants.MSSLMCP ? ApplicationType.MSSL_CRM : obj.Application == Constants.MSSLMSP ? ApplicationType.MSSL_ISU : ApplicationType.MSSL_ISU;
            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsSAPRoleValid(applicationType, obj.TechnicalRole);
            if (!isExist)
                missingFields.Add("Technical Role");

            if (!this.IsUserValid(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (!this.IsUserValid(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }

            if (!this.IsUserValid(obj.Level3Approver, obj.Level3ApproverType))
            {
                missingFields.Add(" Level3 Approver");
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
            if (cnt > 0)
                validationMsg = validationMsg + " Not Found ";

            return missingFields.Count == 0;
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetSAPXLMSSLRolesMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_MSSL_XL_RolesMatrix";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPXLMSSLRolesMatrix obj = this.Data as SAPXLMSSLRolesMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        public override void Update(string currentUser)
        {
            try
            {
                SAPXLMSSLRolesMatrix obj = this.Data as SAPXLMSSLRolesMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPXLMSSLRolesMatrix GetSAPXLMSSLRolesMatrixbyObject()
        {
            SAPXLMSSLRolesMatrix obj = this.Data as SAPXLMSSLRolesMatrix;
            return SAPXLMSSLRolesMatrix.GetSAPXLMSSLRolesMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return SAPXLMSSLRolesMatrix.IsEmployeeConfiguredAsApprover(employeeId);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPXLMSSLRolesMatrix.GetWhoUpdatedData();
        }
    }
}
