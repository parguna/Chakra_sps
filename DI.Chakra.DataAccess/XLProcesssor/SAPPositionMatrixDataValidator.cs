using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class SAPPositionMatrixDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPPositionsMatrix _config = null;
            try
            {
                var val = string.Empty;

                _config = new SAPPositionsMatrix();

                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.PositionText = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.PositionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
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
            var _config = data as SAPPositionsMatrix;

            if (
                string.IsNullOrEmpty(_config.Remarks + _config.PositionText + _config.PositionCode +
                                     _config.Level1Approver + _config.Level2Approver + _config.Level3Approver))
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
            var obj = Data as SAPPositionsMatrix;

            var missingFields = new List<string>();
            missingFields.Clear();

            if (string.IsNullOrEmpty(obj.PositionCode))
            {
                missingFields.Add("Position Code");
            }

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
            ExistingData = GetSAPPositionsMatrixbyObject();
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
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE DI_SAP_XLPositionsMatrix");
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
                var obj = Data as SAPPositionsMatrix;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.SAPPositionsMatrices.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private SAPPositionsMatrix GetSAPPositionsMatrixbyObject()
        {
            var obj = Data as SAPPositionsMatrix;
            using (var _context = new IACMDBContext())
            {
                return _context.SAPPositionsMatrices.FirstOrDefault(cnfg => cnfg.PositionText == obj.PositionText);
            }
        }

        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.SAPPositionsMatrices.Where(
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