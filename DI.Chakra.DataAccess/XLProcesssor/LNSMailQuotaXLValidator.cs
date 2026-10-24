using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class LNSMailQuotaXLValidator : XLDataValidator, IValidator
    {
        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as LNSMailQuotaConfig;

            var isValid = true;
            var missingFields = new List<string>();
            missingFields.Clear();

            using (var _context = new IACMDBContext())
            {
                if (string.IsNullOrEmpty(obj.Position))
                {
                    missingFields.Add("Position is empty");
                    isValid = false;
                }
                int num;
                if (!string.IsNullOrEmpty(obj.MailQuotaSizeInMB.ToString()))
                {
                    if (!int.TryParse(obj.MailQuotaSizeInMB.ToString(), out num))
                    {
                        missingFields.Add("Numbers only allowed in MailQuota");
                        isValid = false;
                    }
                }
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
            return isValid;
        }

        public override bool IsExists()
        {
            var _existConfigMatrixAD = GetExisting();
            if (_existConfigMatrixAD == null)
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_LNS_MailQuotaConfig");
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
                var obj = Data as LNSMailQuotaConfig;
                using (var _context = new IACMDBContext())
                {
                    if (obj != null)
                    {
                        obj.ValidFrom = DateTime.Now;
                        obj.ValidTo = DateTime.MaxValue;
                        obj.IsValid = true;
                        _context.LNSMailQuotaConfigs.Add(obj);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private LNSMailQuotaConfig GetExisting()
        {
            var obj = Data as LNSMailQuotaConfig;

            using (var _context = new IACMDBContext())
            {
                return _context.LNSMailQuotaConfigs.FirstOrDefault(cnfg => cnfg.Position == obj.Position
                    );
            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                var size = 0;
                if (dRow.ItemArray[1] != null)
                {
                    var sizee = dRow.ItemArray[1].ToString();
                    size = Convert.ToInt32(sizee.Replace("MB", ""));
                }

                return new LNSMailQuotaConfig
                {
                    Position = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    MailQuotaSizeInMB = size
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as LNSMailQuotaConfig;

            if (string.IsNullOrEmpty(_config.Position + _config.MailQuotaSizeInMB))
            {
                return true;
            }
            return false;
        }
    }
}