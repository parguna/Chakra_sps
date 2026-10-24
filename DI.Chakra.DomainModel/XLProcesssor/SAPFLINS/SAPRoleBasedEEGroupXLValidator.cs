using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPRoleBasedEEGroupXLValidator : XLDataValidator
    {
        readonly long endpointSystemId;

        public SAPRoleBasedEEGroupXLValidator()
        {
            endpointSystemId = GetEndpointSystemID(ApplicationType.HRMS_FLINS);
        }
        public override Type GetDomainModelType()
        {
            return typeof(ProvisioningConfigMatrixSapEEGroup);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            ProvisioningConfigMatrixSapEEGroup _config = null;
            try
            {
                _config = new ProvisioningConfigMatrixSapEEGroup();

                _config.EmployeeGroupNo = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.EmployeeGroup = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.PArea = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.DefaultRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Remark = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            ProvisioningConfigMatrixSapEEGroup _config = data as ProvisioningConfigMatrixSapEEGroup;

            if (string.IsNullOrEmpty(_config.EmployeeGroup + _config.EmployeeGroupNo))
            {
                return true;
            }

            return false;
        }
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {

            ProvisioningConfigMatrixSapEEGroup obj = data as ProvisioningConfigMatrixSapEEGroup;
            validationMsg = string.Empty;
            bool isExist = (!string.IsNullOrEmpty(obj.DefaultRole)) && this.IsSAPRoleValid(endpointSystemId, obj.DefaultRole);
            if (!isExist)
            {
                validationMsg = $"Found Invalid Technical role for the record -  EmployeeGroup: {obj.EmployeeGroup}; Department: {obj.PArea}; Remark: {obj.Remark}";
            }

            return validationMsg == string.Empty;

        }

        public override bool Validate(ref string validationMsg)
        {
            ProvisioningConfigMatrixSapEEGroup obj = this.Data as ProvisioningConfigMatrixSapEEGroup;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (!string.IsNullOrEmpty(obj.PArea) && !obj.PArea.Contains("*"))
            {
                isExist = this.IsPersonnelAreaValid(obj.PArea);
                if (isExist == false)
                    missingFields.Add("Department Code");
            }

            isExist = (!string.IsNullOrEmpty(obj.DefaultRole)) && this.IsSAPRoleValid(ApplicationType.HRMS_FLINS, obj.DefaultRole);
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultRole))
                missingFields.Add("Default Role");

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

        public override bool IsExists(string userName)
        {
            return false;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_M_ProvisioningConfigMatrixSAPEEGroup";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                ProvisioningConfigMatrixSapEEGroup obj = this.Data as ProvisioningConfigMatrixSapEEGroup;
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
                ProvisioningConfigMatrixSapEEGroup obj = this.Data as ProvisioningConfigMatrixSapEEGroup;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private ProvisioningConfigMatrixSapEEGroup getSAPRolesbyEEGroupObjects()
        {
            return ProvisioningConfigMatrixSapEEGroup.GetADSXLProvisioningMatrixbyObject(this.Data as ProvisioningConfigMatrixSapEEGroup);
        }

        public override string GetWhoUpdatedData()
        {
            return ProvisioningConfigMatrixSapEEGroup.GetWhoUpdatedData();
        }
    }
}
