using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class GemsRoleMatrixDataValidator: XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get
            {
                if (endpointSystemId == 0)
                {
                    endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.GEMS).Id;
                }
                return endpointSystemId;
            }
            set { endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(GEMSRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            GEMSRoleMatrix _config = null;
            try
            {
                _config = new GEMSRoleMatrix();

                _config.BusinessRoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.ADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString()));
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

        private ApprovalUserType GetApprovalType(string ApprovalTypeExcelData)
        {
            return (ApprovalUserType)Enum.Parse(typeof(ApprovalUserType), ApprovalTypeExcelData);
        }

        public override bool IsEmptyRow(IXLData data)
        {
            GEMSRoleMatrix _config = data as GEMSRoleMatrix;

            if (string.IsNullOrEmpty(_config.BusinessRoleName + _config.TechnicalRole + _config.ADGroups + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            GEMSRoleMatrix obj = this.Data as GEMSRoleMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (string.IsNullOrEmpty(obj.BusinessRoleName))
            {
                missingFields.Add("Business Role");
            }

            if (string.IsNullOrEmpty(obj.TechnicalRole) && string.IsNullOrEmpty(obj.ADGroups))
            {
                missingFields.Add("Technical Role");
                missingFields.Add("AD Group");
            }
            else
            {
                if (!string.IsNullOrEmpty(obj.TechnicalRole))
                {
                    isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsGEMSRoleValid(obj.TechnicalRole);
                    if (!isExist)
                        missingFields.Add("Technical Role");
                }

                if (!string.IsNullOrEmpty(obj.ADGroups))
                {
                    isExist = (!string.IsNullOrEmpty(obj.ADGroups)) && this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.ADGroups);
                    if (!isExist)
                        missingFields.Add("AD Group");
                }
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
            this.ExistingData = GetGEMSRoleMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_GEMS_RoleMatrix";
            return base.TruncateTable();
            //return GEMSRoleMatrix.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                GEMSRoleMatrix obj = this.Data as GEMSRoleMatrix;
                obj.EndPointSystemId = EndpointSystemId;
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
                GEMSRoleMatrix obj = this.Data as GEMSRoleMatrix;
                obj.EndPointSystemId = EndpointSystemId;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private GEMSRoleMatrix GetGEMSRoleMatrixbyObject()
        {
            GEMSRoleMatrix obj = this.Data as GEMSRoleMatrix;
            return GEMSRoleMatrix.GetGEMSRoleMatrixbyObject(obj);
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return GEMSRoleMatrix.IsEmployeeConfiguredAsApprover(employeeId);
        }

        public override string GetWhoUpdatedData()
        {
            return GEMSRoleMatrix.GetWhoUpdatedData();
        }
    }
}
