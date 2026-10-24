using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class ADSProvMatrixDataValidator : XLDataValidator, IValidator
    {
        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as ADSXLProvisioningMatrix;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.CompanyCode)) && IsCompanyCodeValid(obj.CompanyCode);
            if (!isExist)
                missingFields.Add("Company Code");

            isExist = (!string.IsNullOrEmpty(obj.DepartmentCode)) && IsPersonnelAreaValid(obj.DepartmentCode);
            if (!isExist)
                missingFields.Add("Department Code");

            isExist = (!string.IsNullOrEmpty(obj.DivisionCode)) && IsPersonnelSubAreaValid(obj.DivisionCode);
            if (!isExist)
                missingFields.Add("Division Code");

            isExist = (!string.IsNullOrEmpty(obj.OrganisationId)) && IsOrgUnitValid(obj.OrganisationId);
            if (!isExist)
                missingFields.Add("Organisation Id");

            isExist = (!string.IsNullOrEmpty(obj.CostCenterCode)) && IsCostCenterValid(obj.CostCenterCode);
            if (!isExist)
                missingFields.Add("CostCenter Code");

            if (!string.IsNullOrEmpty(obj.CostDistributionCode))
            {
                isExist = IsCostCenterValid(obj.CostDistributionCode);
                if (!isExist)
                    missingFields.Add("CostDistribution Code");
            }

            isExist = (!string.IsNullOrEmpty(obj.OU)) && (IsADOUValid(obj.OU));
            if (!isExist)
                missingFields.Add("OU");

            isExist = (!string.IsNullOrEmpty(obj.DefaultGroups)) && (IsADGroupValid(obj.DefaultGroups));
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultGroups))
                missingFields.Add("Default Groups");

            var cnt = 0;
            foreach (var field in missingFields)
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

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                return new ADSXLProvisioningMatrix
                {
                    CompanyCode = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    DivisionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                    OrganisationId = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    OrganisationName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    CostCenterCode = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    CostDistributionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                    OU = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                    DefaultGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString())
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as ADSXLProvisioningMatrix;

            if (
                string.IsNullOrEmpty(_config.CompanyCode + _config.DepartmentCode + _config.DivisionCode +
                                     _config.OrganisationId + _config.OrganisationName +
                                     _config.CostCenterCode + _config.CostDistributionCode + _config.OU +
                                     _config.DefaultGroups))
            {
                return true;
            }

            return false;
        }

        public override bool IsExists()
        {
            ExistingData = GetADSXLProvisioningMatrixbyObject();

            if (ExistingData == null)
                return false;
            return true;
        }

        public override bool TruncateTable()
        {
            var returnValue = false;
            try
            {
                using (var _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("Truncate table DI_ADS_XLProvMatrix");
                    returnValue = true;
                }
            }
            catch
            {
            }
            return returnValue;
        }

        public override void Add()
        {
            try
            {
                var obj = Data as ADSXLProvisioningMatrix;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.ADSXLProvisioningMatrices.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private ADSXLProvisioningMatrix GetADSXLProvisioningMatrixbyObject()
        {
            var obj = Data as ADSXLProvisioningMatrix;

            using (var _context = new IACMDBContext())
            {
                return _context.ADSXLProvisioningMatrices.FirstOrDefault(cnfg => cnfg.CompanyCode == obj.CompanyCode
                                                                                 &&
                                                                                 cnfg.DepartmentCode ==
                                                                                 obj.DepartmentCode &&
                                                                                 cnfg.DivisionCode == obj.DivisionCode &&
                                                                                 cnfg.OrganisationId ==
                                                                                 obj.OrganisationId
                                                                                 &&
                                                                                 cnfg.CostCenterCode ==
                                                                                 obj.CostCenterCode &&
                                                                                 cnfg.CostDistributionCode ==
                                                                                 obj.CostDistributionCode);
            }
        }
    }
}