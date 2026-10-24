using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAPositionMatrixDataValidator : XLDataValidator
    {

        public override Type GetDomainModelType()
        {
            return typeof(SAPHANAXLPositionsMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPHANAXLPositionsMatrix _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPHANAXLPositionsMatrix();
                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.PositionText = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.Position = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Prefix_Employees = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Prefix_TempStaff = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.User_Group = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.Department_Field = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString());
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
            SAPHANAXLPositionsMatrix _config = data as SAPHANAXLPositionsMatrix;

            if (string.IsNullOrWhiteSpace(_config.Remarks + _config.PositionText + _config.Position + _config.Prefix_Employees + _config.Prefix_TempStaff
                   + _config.User_Group + _config.Department_Field + _config.Level1Approver + _config.Level2Approver + _config.Level3Approver))
            {
                if (_config.Level1ApproverType == null && _config.Level2ApproverType == null && _config.Level3ApproverType == null)
                    return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPHANAXLPositionsMatrix obj = this.Data as SAPHANAXLPositionsMatrix;

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
            ApplicationType applicationType = obj.Application == ep.ApplicationCode ? ApplicationType.EBS : obj.Application == ep.ApplicationCode ? ApplicationType.MSSL : ApplicationType.None;
            if (ep.ApplicationType == ApplicationType.None)
            {
                missingFields.Add($"Application not supported - {obj.Application}");
                return false;
            }
            obj.EndpointSystemId = ep.Id;

            if (string.IsNullOrWhiteSpace(obj.Position))
            {
                missingFields.Add("Position");
            }

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
            this.ExistingData = GetSAPHANAXLPositionsMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAPHANA_XLPositionsMatrix";
            return base.TruncateTable();
            //return SAPHANAXLPositionsMatrix.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPHANAXLPositionsMatrix obj = this.Data as SAPHANAXLPositionsMatrix;
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
                SAPHANAXLPositionsMatrix obj = this.Data as SAPHANAXLPositionsMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPHANAXLPositionsMatrix GetSAPHANAXLPositionsMatrixbyObject()
        {
            SAPHANAXLPositionsMatrix obj = this.Data as SAPHANAXLPositionsMatrix;
            return SAPHANAXLPositionsMatrix.GetSAPHANAXLPositionsMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return SAPHANAXLPositionsMatrix.IsEmployeeConfiguredAsApprover(employeeId);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHANAXLPositionsMatrix.GetWhoUpdatedData();
        }
    }
}
