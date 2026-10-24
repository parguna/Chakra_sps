using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class LNSProvMatrixDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                return new LNSXLProvisioningMatrix
                {
                    CompanyCode = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    DivisionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                    OrganisationId = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    OrganisationName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    CostCenterCode = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    CostDistributionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                    MailServer = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                    CertifierIdFile = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString()),
                    CertifierOU = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString()),
                    DefaultGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString()),
                    PolicyName = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString()),
                    ReplicatSever = CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString())
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as LNSXLProvisioningMatrix;

            if (
                string.IsNullOrEmpty(_config.CompanyCode + _config.DepartmentCode + _config.DivisionCode +
                                     _config.OrganisationId + _config.OrganisationName +
                                     _config.CostCenterCode + _config.CostDistributionCode + _config.MailServer +
                                     _config.CertifierIdFile + _config.CertifierOU + _config.DefaultGroups +
                                     _config.PolicyName + _config.ReplicatSever))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as LNSXLProvisioningMatrix;

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

            isExist = (!string.IsNullOrEmpty(obj.DefaultGroups)) && IsLNSGroupValid(obj.DefaultGroups);
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

        public override bool IsExists()
        {
            ExistingData = GetLNSXLProvisioningMatrixbyObject();
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_LNS_XLProvMatrix");
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
                var obj = Data as LNSXLProvisioningMatrix;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.LNSXLProvisioningMatrices.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }


        private LNSXLProvisioningMatrix GetLNSXLProvisioningMatrixbyObject()
        {
            var obj = Data as LNSXLProvisioningMatrix;

            using (var _context = new IACMDBContext())
            {
                return _context.LNSXLProvisioningMatrices.FirstOrDefault(cnfg => cnfg.CompanyCode == obj.CompanyCode
                                                                                 &&
                                                                                 cnfg.DepartmentCode ==
                                                                                 obj.DepartmentCode &&
                                                                                 cnfg.DivisionCode == obj.DivisionCode &&
                                                                                 cnfg.OrganisationId ==
                                                                                 obj.OrganisationId
                                                                                 &&
                                                                                 cnfg.OrganisationName ==
                                                                                 obj.OrganisationName &&
                                                                                 cnfg.CostCenterCode ==
                                                                                 obj.CostCenterCode
                                                                                 &&
                                                                                 cnfg.CostDistributionCode ==
                                                                                 obj.CostDistributionCode &&
                                                                                 cnfg.PolicyName == obj.PolicyName &&
                                                                                 cnfg.ReplicatSever == obj.ReplicatSever &&
                                                                                 cnfg.ValidTo.HasValue &&
                                                                                 cnfg.ValidTo.Value.Year ==
                                                                                 DateTime.MaxValue.Year);
            }
        }
    }
}