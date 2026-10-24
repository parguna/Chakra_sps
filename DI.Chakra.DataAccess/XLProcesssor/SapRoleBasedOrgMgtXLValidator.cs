using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace DeepIdentity.DataAccess.XLProcesssor
{
    class SapRoleBasedOrgMgtXLValidator : XLDataValidator, IValidator
    {

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

            if (!obj.CompanyCode.Contains("*") & !string.IsNullOrEmpty(obj.CompanyCode))
            {
                isExist = this.IsCompanyCodeValid(obj.CompanyCode);
                if (!isExist)
                    missingFields.Add("Company Code");
            }

            if (!obj.DepartmentCode.Contains("*") & !string.IsNullOrEmpty(obj.DepartmentCode))
            {
                isExist = this.IsPersonnelAreaValid(obj.DepartmentCode);
                if (!isExist)
                    missingFields.Add("Department Code");
            }

            if (!obj.DivisionCode.Contains("*") & !string.IsNullOrEmpty(obj.DivisionCode))
            {
                isExist = this.IsPersonnelSubAreaValid(obj.DivisionCode);
                if (!isExist)
                    missingFields.Add("Division Code");
            }

            if (!obj.CostCenterCode.Contains("*") & !string.IsNullOrEmpty(obj.CostCenterCode))
            {
                isExist = this.IsCostCenterValid(obj.CostCenterCode);
                if (!isExist)
                    missingFields.Add("CostCenter Code");
            }

            isExist = (!string.IsNullOrEmpty(obj.DefaultHRMSGeneralRoles)) && this.IsSAPRoleValid(ApplicationType.SAP, obj.DefaultHRMSGeneralRoles);
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

        public override bool IsExists()
        {
            return false;
        }

        public override void Add()
        {
            try
            {
                ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;

                using (IACMDBContext _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.ProvisioningConfigMatrixSAPOrgs.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }

        public override bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("Truncate table DI_M_ProvisioningConfigMatrixSAPORG");
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        private ProvisioningConfigMatrixSAPOrg GetSAPRoleMatrixGroupbyObject()
        {
            ProvisioningConfigMatrixSAPOrg obj = this.Data as ProvisioningConfigMatrixSAPOrg;

            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.ProvisioningConfigMatrixSAPOrgs.FirstOrDefault(sap => sap.CompanyCode == obj.CompanyCode
                                                    && sap.CompanyName == obj.CompanyName
                                                    && sap.DepartmentCode == obj.DepartmentCode
                                                    && sap.DivisionCode == obj.DivisionCode
                                                    && sap.CostCenterCode == obj.CostCenterCode
                                                    );
            }
        }

    }
}
