using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class SAPCriticalTCodeMatrixDataValidator : XLDataValidator, IValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPCriticalTCodeMatrix _config = null;
            try
            {
                _config = new SAPCriticalTCodeMatrix();

                _config.Transaction = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.TransactionDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.TransactionOwner = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as SAPCriticalTCodeMatrix;

            if (string.IsNullOrEmpty(_config.Transaction + _config.TransactionDescription + _config.TransactionOwner))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as SAPCriticalTCodeMatrix;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.Transaction)) && IsSAPTCodeValid(obj.Transaction);
            if (!isExist)
                missingFields.Add("Transaction");

            if (!IsUserValid(obj.TransactionOwner, ApprovalUserType.User))
                missingFields.Add("Transaction Owner");

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
            ExistingData = GetSAPCriticalTCodesMatrixbyObject();
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
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE DI_SAP_XLCriticalTCodeMatrix");
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
                var obj = Data as SAPCriticalTCodeMatrix;
                
                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsValid = true;
                    _context.SAPCriticalTCodeMatrices.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private SAPCriticalTCodeMatrix GetSAPCriticalTCodesMatrixbyObject()
        {
            var obj = Data as SAPCriticalTCodeMatrix;
            using (var _context = new IACMDBContext())
            {
                return _context.SAPCriticalTCodeMatrices.FirstOrDefault(cnfg => cnfg.Transaction == obj.Transaction);
            }
        }
    }
}