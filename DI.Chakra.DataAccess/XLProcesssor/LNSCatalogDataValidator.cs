using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DataAccess.XLProcesssor
{
    class LNSCatalogDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            LNSCatalog _config = null;
            try
            {
                var val = string.Empty;
                _config = new LNSCatalog();

                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Description = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
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
            var _config = data as LNSCatalog;

            if (
                string.IsNullOrEmpty(_config.Application +
                                     _config.TechnicalRole
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
            var obj = Data as LNSCatalog;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && IsLNSGroupValid(obj.TechnicalRole);
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
            ExistingData = GetLNSCatalogbyObject();
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_LNS_Catalog");
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
                var obj = Data as LNSCatalog;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.LNSCatalogs.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private LNSCatalog GetLNSCatalogbyObject()
        {
            var obj = Data as LNSCatalog;
            using (var _context = new IACMDBContext())
            {
                return _context.LNSCatalogs.FirstOrDefault(cnfg => cnfg.BusinessRole == obj.BusinessRole);
            }
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.LNSCatalogs.Where(
                        lnscatalog => lnscatalog.Level1Approver == employeeId
                               || lnscatalog.Level2Approver == employeeId
                               || lnscatalog.Level3Approver == employeeId).ToList().Count > 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
