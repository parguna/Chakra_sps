using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPFLINSRoleMatrixDataValidator : XLDataValidator
    {
        readonly long endpointSystemId;

        public SAPFLINSRoleMatrixDataValidator()
        {
            endpointSystemId = GetEndpointSystemID(ApplicationType.HRMS_FLINS);
        }
        public override Type GetDomainModelType()
        {
            return typeof(SAPHRFLINSRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPHRFLINSRoleMatrix _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPHRFLINSRoleMatrix();

                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Company = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.CostCenter = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.PurchasingOrg = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.PurchasingGrp = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.Plant = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.MRPControl = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.StorageLocation = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.Module = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.SubModule = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                _config.TechnicalRule = CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[14].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[15].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[16].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[17].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[18].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(val);
                _config.MainSupp = CheckNullAndGetValueFromCell(dRow.ItemArray[19].ToString());

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
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SAPHRFLINSRoleMatrix obj = data as SAPHRFLINSRoleMatrix;
            validationMsg = string.Empty;
            if (obj.MainSupp.Trim().Equals("main", StringComparison.OrdinalIgnoreCase) && obj.MainSupp.Trim().Equals("general", StringComparison.OrdinalIgnoreCase))
            {
                bool isExist = (!string.IsNullOrEmpty(obj.TechnicalRule)) && this.IsSAPRoleValid(endpointSystemId, obj.TechnicalRule);
                if (!isExist)
                {
                    validationMsg = $"Found Invalid Technical role for the Business Role - {obj.BusinessRole}";
                }
            }
            return validationMsg == string.Empty;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPHRFLINSRoleMatrix _config = data as SAPHRFLINSRoleMatrix;

            if (string.IsNullOrEmpty(_config.Remarks + _config.Application + _config.Company + _config.CostCenter + _config.PurchasingOrg + _config.PurchasingGrp
                       + _config.Plant + _config.MRPControl + _config.StorageLocation + _config.Module + _config.SubModule + _config.BusinessRole + _config.TechnicalRule
                       + _config.Level1Approver + _config.Level2Approver + _config.Level3Approver + _config.MainSupp))
            {
                if (_config.Level1ApproverType == null && _config.Level2ApproverType == null && _config.Level3ApproverType == null)
                    return true;
            }

            return false;
        }

        SAPHRFLINSRoleMatrix obj = null;

        bool isValid = true;
        List<string> missingFields = new List<string>();
        int cnt = 0;
        public override bool Validate(ref string validationMsg)
        {
              obj = this.Data as SAPHRFLINSRoleMatrix;

              isValid = true;

                missingFields.Clear();
               cnt = 0;
            //if (string.IsNullOrWhiteSpace(obj.BusinessRole))
            //{
            //    missingFields.Add(" Business Role");
            //}
            //else
            //{
            //    var roleMandatory = SAPRoleMandatoryField.GetSAPRoleMandatoryFieldbyObject(new SAPRoleMandatoryField { BusinessRole = obj.BusinessRole });
            //    if(roleMandatory == null)
            //    {
            //        missingFields.Add(" Role Mandatory");
            //    }
            //}

            if (!IsUserValidCheck(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (!IsUserValidCheck(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }
            if (!IsUserValidCheck(obj.Level3Approver, obj.Level3ApproverType))
            {
                missingFields.Add(" Level3 Approver");
            }

            
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
            if (missingFields.Count() > 0)
            {
                isValid = false;
            }
            else
            {
                isValid = true;
            }
            return isValid;

        }

        public override bool IsExists(string currentUser)
        {
          return  SAPHRFLINSRoleMatrix.IsExist(this.Data as SAPHRFLINSRoleMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_HRFLINSRoleMatrix";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPHRFLINSRoleMatrix obj = this.Data as SAPHRFLINSRoleMatrix;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                SAPHRFLINSRoleMatrix.Add(obj);
            }
            catch { throw; }
        }
        public override void Update(string currentUser)
        {
            SAPHRFLINSRoleMatrix obj = this.Data as SAPHRFLINSRoleMatrix;
            obj.LastModifiedBy = currentUser;
            obj.LastModifiedDate = DateTime.Now;
            SAPHRFLINSRoleMatrix.ManualUpdate(obj);
        }

        private SAPHRFLINSRoleMatrix GetSAPHRFLINSRoleMatrixbyObject()
        {
            SAPHRFLINSRoleMatrix obj = this.Data as SAPHRFLINSRoleMatrix;
            return SAPHRFLINSRoleMatrix.GetSAPHRFLINSRoleMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                var res = SAPHRFLINSRoleMatrix.IsEmployeeConfiguredAsApprover(employeeId);
                return res.Count > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHRFLINSRoleMatrix.GetWhoUpdatedData();
        }
    }
}
