using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess.XLProcesssor
{
    internal class SAPProvMatrixDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPXLProvisioningMatrix _config = null;
            try
            {
                _config = new SAPXLProvisioningMatrix();

                _config.CompanyCode = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.CompanyName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.DepartmentName = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.DivisionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.DivisionName = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.CostCenterCode = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.CostCenterName = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Role = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as SAPXLProvisioningMatrix;

            if (
                string.IsNullOrEmpty(_config.CompanyCode + _config.CompanyName + _config.DepartmentCode +
                                     _config.DivisionCode))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as SAPXLProvisioningMatrix;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            if (!obj.CompanyCode.Contains("ALL") & !string.IsNullOrEmpty(obj.CompanyCode))
            {
                isExist = IsCompanyCodeValid(obj.CompanyCode);
                if (!isExist)
                    missingFields.Add("Company Code");
            }

            if (!obj.DepartmentCode.Contains("ALL") & !string.IsNullOrEmpty(obj.DepartmentCode))
            {
                isExist = IsPersonnelAreaValid(obj.DepartmentCode);
                if (!isExist)
                    missingFields.Add("Department Code");
            }

            if (!obj.DivisionCode.Contains("ALL") & !string.IsNullOrEmpty(obj.DivisionCode))
            {
                isExist = IsPersonnelSubAreaValid(obj.DivisionCode);
                if (!isExist)
                    missingFields.Add("Division Code");
            }

            if (!obj.CostCenterCode.Contains("ALL") & !string.IsNullOrEmpty(obj.CostCenterCode))
            {
                isExist = IsCostCenterValid(obj.CostCenterCode);
                if (!isExist)
                    missingFields.Add("CostCenter Code");
            }

            isExist = (!string.IsNullOrEmpty(obj.Role)) &&
                      IsSAPRoleValid(obj.Role);
            if (!isExist && !string.IsNullOrEmpty(obj.Role))
                missingFields.Add("Roles");

           

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

        public override bool IsExists()
        {
            return false;
        }

        public override void Add()
        {
            try
            {
                var obj = Data as SAPXLProvisioningMatrix;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.SAPXLProvisioningMatrices.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public override bool TruncateTable()
        {
            var returnValue = false;
            try
            {
                using (var _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE DI_SAP_XLProvMatrix");
                    returnValue = true;
                }
            }
            catch
            {
                returnValue = false;
            }
            return returnValue;
        }

        private SAPXLProvisioningMatrix GetSAPRoleMatrixGroupbyObject()
        {
            var obj = Data as SAPXLProvisioningMatrix;

            using (var _context = new IACMDBContext())
            {
                return _context.SAPXLProvisioningMatrices.FirstOrDefault(sap => sap.CompanyCode == obj.CompanyCode
                                                                              &&sap.DepartmentCode == obj.DepartmentCode
                                                                              && sap.DivisionCode == obj.DivisionCode
                                                                              && sap.CostCenterCode == obj.CostCenterCode
                                                                                
                    );
            }
        }
    }
}