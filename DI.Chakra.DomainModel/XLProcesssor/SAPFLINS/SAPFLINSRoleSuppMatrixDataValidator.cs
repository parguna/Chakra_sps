using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPFLINSRoleSuppMatrixDataValidator : XLDataValidator
    {
        readonly long endpointSystemId;

        public SAPFLINSRoleSuppMatrixDataValidator()
        {
            endpointSystemId = GetEndpointSystemID(ApplicationType.HRMS_FLINS);
        }
        public override Type GetDomainModelType()
        {
            return typeof(SAPHRFLINSSuppRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPHRFLINSSuppRoleMatrix _config = null;
            try
            {
                _config = new SAPHRFLINSSuppRoleMatrix();

                _config.Rule = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Company = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.CostCenter = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.PurchasingOrg = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.PurchasingGrp = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Plant = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.MRPControl = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.StorageLocation = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Role = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.RoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SAPHRFLINSSuppRoleMatrix obj = data as SAPHRFLINSSuppRoleMatrix;
            validationMsg = string.Empty;

            bool isExist = (!string.IsNullOrEmpty(obj.Role)) && this.IsSAPRoleValid(endpointSystemId, obj.Role);
            if (!isExist)
            {
                validationMsg = $"Found Invalid Technical role for the Role Name - {obj.RoleName}";
            }

            return validationMsg == string.Empty;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPHRFLINSSuppRoleMatrix _config = data as SAPHRFLINSSuppRoleMatrix;

            if (string.IsNullOrEmpty(_config.Rule + _config.Company + _config.CostCenter + _config.PurchasingOrg + _config.PurchasingGrp
                       + _config.Plant + _config.MRPControl + _config.StorageLocation + _config.Role + _config.RoleName))
            {
                return true;
            }
            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPHRFLINSSuppRoleMatrix obj = this.Data as SAPHRFLINSSuppRoleMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.Role)) && this.IsSAPRoleValid(ApplicationType.HRMS_FLINS, obj.Role);
            if (!isExist)
                missingFields.Add("Technical Role");

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
            return false;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_HRFLINSSuppRoleMatrix";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPHRFLINSSuppRoleMatrix obj = this.Data as SAPHRFLINSSuppRoleMatrix;
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
                SAPHRFLINSSuppRoleMatrix obj = this.Data as SAPHRFLINSSuppRoleMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPHRFLINSSuppRoleMatrix GetSAPHRFLINSSuppRoleMatrixbyObject()
        {
            SAPHRFLINSSuppRoleMatrix obj = this.Data as SAPHRFLINSSuppRoleMatrix;
            return SAPHRFLINSSuppRoleMatrix.GetSAPHRFLINSSuppRoleMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHRFLINSSuppRoleMatrix.GetWhoUpdatedData();
        }
    }
}
