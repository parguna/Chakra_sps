using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPProvMatrixByOrgDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(ProvisioningConfigMatrixSAPOrg);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            ProvisioningConfigMatrixSAPOrg _config = null;
            try
            {
                _config = new ProvisioningConfigMatrixSAPOrg();

                _config.CompanyCode = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.CompanyName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.personnelArea = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.DivisionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.PersonnelSubArea = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.CostCenterCode = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.CostCenterName = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.DefaultHRMSGeneralRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.DefaultEPGroup = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            ProvisioningConfigMatrixSAPOrg _config = data as ProvisioningConfigMatrixSAPOrg;

            if (string.IsNullOrEmpty(_config.CompanyCode + _config.CompanyName + _config.DepartmentCode + _config.DivisionCode))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrEmpty(obj.CompanyCode))
            {
            }
            if (!string.IsNullOrEmpty(obj.CompanyCode) && !obj.CompanyCode.Contains("*"))
            {
                isExist = this.IsCompanyCodeValid(obj.CompanyCode);
                if (!isExist)
                    missingFields.Add("Company Code");
            }

            if (!string.IsNullOrEmpty(obj.DepartmentCode) && !obj.DepartmentCode.Contains("*"))
            {
                isExist = this.IsPersonnelAreaValid(obj.DepartmentCode);
                if (!isExist)
                    missingFields.Add("Department Code");
            }

            if (!string.IsNullOrEmpty(obj.DivisionCode) && !obj.DivisionCode.Contains("*"))
            {
                isExist = this.IsPersonnelSubAreaValid(obj.DivisionCode);
                if (!isExist)
                    missingFields.Add("Branch Code");
            }

            if (!string.IsNullOrEmpty(obj.CostCenterCode) && !obj.CostCenterCode.Contains("*"))
            {
                isExist = this.IsCostCenterValid(obj.CostCenterCode);
                if (!isExist)
                    missingFields.Add("CostCenter Code");
            }

            isExist = (!string.IsNullOrEmpty(obj.DefaultHRMSGeneralRoles)) && this.IsSAPRoleValid(ApplicationType.HRMS_FLINS, obj.DefaultHRMSGeneralRoles);
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultHRMSGeneralRoles))
                missingFields.Add("Default HRMS General Roles");

            isExist = (!string.IsNullOrEmpty(obj.DefaultEPGroup)) && this.IsSAPEPGroupValid(ApplicationType.SAP_EP, obj.DefaultEPGroup);
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultEPGroup))
                missingFields.Add("Default EPGroup");

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
            this.ExistingData = GetSAPRoleMatrixGroupbyObject();

            if (ExistingData == null)
                return false;
            else
            {
                return true;
            }
        }

        public override void Add(string currentUser)
        {
            try
            {
                ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;
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
                ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        public override bool TruncateTable()
        {
            TableName = "DI_M_ProvisioningConfigMatrixSAPORG";
            return base.TruncateTable();
        }

        private ProvisioningConfigMatrixSAPOrg GetSAPRoleMatrixGroupbyObject()
        {
            ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;
            return ProvisioningConfigMatrixSAPOrg.GetSAPRoleMatrixGroupbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return ProvisioningConfigMatrixSAPOrg.GetWhoUpdatedData();
        }
    }
}
