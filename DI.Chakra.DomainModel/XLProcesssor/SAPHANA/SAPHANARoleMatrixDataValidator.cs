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
    public class SAPHANARoleMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPHANAXLRolesMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                string val = string.Empty;

                SAPHANAXLRolesMatrix _config = new SAPHANAXLRolesMatrix();

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
            SAPHANAXLRolesMatrix _config = data as SAPHANAXLRolesMatrix;

            if (string.IsNullOrWhiteSpace(_config.Remarks + _config.Application + _config.Module + _config.SubModule + _config.RoleName + _config.TechnicalRole
                    + _config.Level1Approver + _config.Level2Approver + _config.Level3Approver))
            {
                if (_config.Level1ApproverType == null && _config.Level2ApproverType == null && _config.Level3ApproverType == null)
                    return true;
            }

            return false;
        }
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SAPHANAXLRolesMatrix obj = data as SAPHANAXLRolesMatrix;
            var ep = DomainModel.EndPoint.GetByApplicationCode(obj.Application);
            if(ep == null)
            {
                validationMsg = $"Application Not found - {obj.Application}";
                return validationMsg == string.Empty;
            }
            ApplicationType applicationType = obj.Application == ep.ApplicationCode ? ApplicationType.EBS : obj.Application == ep.ApplicationCode ? ApplicationType.MSSL : ApplicationType.None;
            if (ep.ApplicationType == ApplicationType.None)
            {
                validationMsg = $"Application not supported - {obj.Application}";
                return validationMsg == string.Empty;
            }
            long endpointSystemId = ep.Id;
            validationMsg = string.Empty;

            // Manii :based on application type handling technical role validation.

            if (ep.ApplicationType != ApplicationType.EBS && ep.ApplicationType != ApplicationType.MSSL)
            {
                bool isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && IsSAPRoleValid(endpointSystemId, obj.TechnicalRole);
                if (!isExist)
                {
                    validationMsg = $"Found Invalid Technical role for the business role - {obj.BusinessRole}";
                }
            }
            else
            {
                bool isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && IsSAPHanaRoleValid(endpointSystemId, obj.TechnicalRole);
                if (!isExist)
                {
                    validationMsg = $"Found Invalid Technical role for the business role - {obj.BusinessRole}";
                }
            }
            obj.EndpointSystemId = endpointSystemId;
            return validationMsg == string.Empty;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPHANAXLRolesMatrix obj = this.Data as SAPHANAXLRolesMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrWhiteSpace(obj.Application))
            {
                missingFields.Add("Application");
                return false;
            }
            var ep = DomainModel.EndPoint.GetByApplicationCode(obj.Application);
            if (ep == null)
            {
                missingFields.Add($"Application Not found - {obj.Application}");
                return false;
            }
            //ApplicationType applicationType = obj.Application == ep.ApplicationCode ? ApplicationType.EBS : obj.Application == ep.ApplicationCode ? ApplicationType.MSSL : ApplicationType.None;
            if (ep.ApplicationType == ApplicationType.None)
            {
                missingFields.Add($"Application not supported - {obj.Application}");
                return false;
            }
            ApplicationType applicationType = ep.ApplicationType;
            obj.EndpointSystemId = ep.Id;
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
            this.ExistingData = GetSAPHANAXLRolesMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAPHANA_XLRolesMatrix";
            return base.TruncateTable();
            //return SAPHANAXLRolesMatrix.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPHANAXLRolesMatrix obj = this.Data as SAPHANAXLRolesMatrix;
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
                SAPHANAXLRolesMatrix obj = this.Data as SAPHANAXLRolesMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPHANAXLRolesMatrix GetSAPHANAXLRolesMatrixbyObject()
        {
            SAPHANAXLRolesMatrix obj = this.Data as SAPHANAXLRolesMatrix;
            return SAPHANAXLRolesMatrix.GetSAPHANAXLRoleMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return SAPHANAXLRolesMatrix.IsEmployeeConfiguredAsApprover(employeeId);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHANAXLRolesMatrix.GetWhoUpdatedData();
        }
    }
}
