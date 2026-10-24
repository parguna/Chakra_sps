using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;

namespace DeepIdentity.DataAccess
{
    public class SAPRoleCatalogDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPRoleCatalog _config = null;
            try
            {
                var val = string.Empty;
                _config = new SAPRoleCatalog();

                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Module = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.SubModule = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.BusinessRoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(val);
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as SAPRoleCatalog;

            if (
                string.IsNullOrEmpty(_config.Remarks + _config.Application + _config.Module + _config.SubModule +
                                     _config.BusinessRoleName + _config.TechnicalRole
                                     + _config.Level1Approver + _config.Level2Approver + _config.Level3Approver))
            {
                if (_config.Level1ApproverType == ApprovalUserType.None &&
                    _config.Level2ApproverType == ApprovalUserType.None &&
                    _config.Level3ApproverType == ApprovalUserType.None)
                    return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as SAPRoleCatalog;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && IsSAPRoleValid(obj.TechnicalRole);
            if (!isExist)
                missingFields.Add("Technical Role");

            if (!IsUserValid(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (!IsUserValid(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }

            if (!IsUserValid(obj.Level3Approver, obj.Level3ApproverType))
            {
                missingFields.Add(" Level3 Approver");
            }

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
            ExistingData = GetSAPRoleCatalogbyObject();
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_SAP_RoleCatalog");
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
                var obj = Data as SAPRoleCatalog;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.SAPRoleCatalogs.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private SAPRoleCatalog GetSAPRoleCatalogbyObject()
        {
            var obj = Data as SAPRoleCatalog;
            using (var _context = new IACMDBContext())
            {
                return _context.SAPRoleCatalogs.FirstOrDefault(cnfg => cnfg.BusinessRoleName == obj.BusinessRoleName);
            }
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.SAPRoleCatalogs.Where(
                        sap => sap.Level1Approver == employeeId
                               || sap.Level2Approver == employeeId
                               || sap.Level3Approver == employeeId).ToList().Count > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}