using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Model;

namespace DI.Chakra.DataAccess
{
    public class VPNProvMatrixDataValidator : XLDataValidator, IValidator
    {
        public override bool Validate(ref string validationMsg)
        {
            var obj = Data as VPNMaster;

            var isExist = false;
            var missingFields = new List<string>();
            missingFields.Clear();

            var Application = obj.ApplicationName;
            var DevGroups = obj.DevADGroups;
            var QAGroups = obj.QAADGroups;
            var ProdGroups = obj.ProductionADGroups;

            if (string.IsNullOrEmpty(obj.ApplicationName))
            {
                missingFields.Add("Application");
            }

            isExist =
                !(string.IsNullOrEmpty(obj.DevADGroups) && string.IsNullOrEmpty(obj.ProductionADGroups) &&
                  string.IsNullOrEmpty(obj.QAADGroups));
            if (isExist == false)
                missingFields.Add("Production, Development, QA Group(s)");

            if (obj.ProductionADGroups != "Default" && !string.IsNullOrEmpty(obj.ProductionADGroups))
            {
                isExist = IsADGroupValid(obj.ProductionADGroups);
                if (isExist == false)
                    missingFields.Add("Production Group(s)");
            }

            if (obj.DevADGroups != "Default" && !string.IsNullOrEmpty(obj.DevADGroups))
            {
                isExist = IsADGroupValid(obj.DevADGroups);
                if (isExist == false)
                    missingFields.Add("Development Group(s)");
            }

            if (obj.QAADGroups != "Default" && !string.IsNullOrEmpty(obj.QAADGroups))
            {
                isExist = IsADGroupValid(obj.QAADGroups);
                if (isExist == false)
                    missingFields.Add("QA Group(s)");
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

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                return new VPNMaster
                {
                    ApplicationName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    ProductionADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    DevADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                    QAADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    CreatedDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as VPNMaster;

            if (
                string.IsNullOrEmpty(_config.ApplicationName + _config.ProductionADGroups + _config.DevADGroups +
                                     _config.QAADGroups))
            {
                return true;
            }

            return false;
        }

        public override bool IsExists()
        {
            ExistingData = GetVPNXLProvisioningMatrixbyObject();
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_VPN_Master");
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
                var obj = Data as VPNMaster;

                using (var _context = new IACMDBContext())
                {
                    obj.ValidFrom = DateTime.Now;
                    obj.ValidTo = DateTime.MaxValue;
                    obj.IsActive = true;

                    _context.VPNMasters.Add(obj);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        private VPNMaster GetVPNXLProvisioningMatrixbyObject()
        {
            var obj = Data as VPNMaster;

            using (var _context = new IACMDBContext())
            {
                return _context.VPNMasters.FirstOrDefault(cnfg => cnfg.ApplicationName == obj.ApplicationName);
            }
        }
    }
}