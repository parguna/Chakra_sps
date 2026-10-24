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
    public class SAPEBSRoleMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPEBSXLRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                string val = string.Empty;

                SAPEBSXLRoleMatrix _config = new SAPEBSXLRoleMatrix();

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

                return _config;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPEBSXLRoleMatrix _config = data as SAPEBSXLRoleMatrix;

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
            SAPEBSXLRoleMatrix obj = data as SAPEBSXLRoleMatrix;
            ApplicationType applicationType = obj.Application == Constants.EBSISP ? ApplicationType.EBS_ISU : obj.Application == Constants.EBSCRP ? ApplicationType.EBS_CRM : ApplicationType.EBS_CRM;
            long endpointSystemId = GetEndpointSystemID(applicationType);
            validationMsg = string.Empty;
            bool isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && IsSAPRoleValid(endpointSystemId, obj.TechnicalRole);
            if (!isExist)
            {
                validationMsg = $"Found Invalid Technical role for the business role - {obj.BusinessRole}";
            }

            return validationMsg == string.Empty;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPEBSXLRoleMatrix obj = this.Data as SAPEBSXLRoleMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            ApplicationType applicationType = obj.Application == Constants.EBSISP ? ApplicationType.EBS_ISU : obj.Application == Constants.EBSCRP ? ApplicationType.EBS_CRM : ApplicationType.EBS_CRM;

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
            this.ExistingData = GetSAPEBSXLRoleMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_EBS_XL_RolesMatrix";
            return base.TruncateTable();
            //return SAPEBSXLRoleMatrix.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPEBSXLRoleMatrix obj = this.Data as SAPEBSXLRoleMatrix;
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
                SAPEBSXLRoleMatrix obj = this.Data as SAPEBSXLRoleMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPEBSXLRoleMatrix GetSAPEBSXLRoleMatrixbyObject()
        {
            SAPEBSXLRoleMatrix obj = this.Data as SAPEBSXLRoleMatrix;
            return SAPEBSXLRoleMatrix.GetSAPEBSXLRoleMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return SAPEBSXLRoleMatrix.IsEmployeeConfiguredAsApprover(employeeId);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPEBSXLRoleMatrix.GetWhoUpdatedData();
        }
    }
}
