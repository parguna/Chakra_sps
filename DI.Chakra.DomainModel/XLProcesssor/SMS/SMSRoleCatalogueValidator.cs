using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SMSRoleCatalogueValidator : XLDataValidator
    {
        readonly long endpointsystemId;
        public SMSRoleCatalogueValidator()
        {
            endpointsystemId = GetEndpointSystemID(ApplicationType.OneCard);
        }

        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SMSXLRoleCatalogue obj = data as SMSXLRoleCatalogue;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsSMSRoleValid(endpointsystemId, obj.TechnicalRole);
            if (!isExist)
                validationMsg = $"Found Invalid Technical Role for the record Building:{obj.Building}, Section:{obj.Section} and RoleName:{obj.RoleName}";

            return string.IsNullOrWhiteSpace(validationMsg);
        }
        public override Type GetDomainModelType()
        {
            return typeof(SMSXLRoleCatalogue);
        }
        public override bool Validate(ref string validationMsg)
        {
            SMSXLRoleCatalogue obj = this.Data as SMSXLRoleCatalogue;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsSMSRoleValid(endpointsystemId, obj.TechnicalRole);
            if (!isExist)
                missingFields.Add("Technical Role");

            if (!this.IsUserValid(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (!this.IsUserValid(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }

            if (!this.IsUserValid(obj.Level3Approver, obj.Level3ApproverType))
            {
                missingFields.Add(" Level3 Approver");
            }


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

        public override IXLData GetObjectFromData(DataRow dRow)
        {

            SMSXLRoleCatalogue _config = null;
            try
            {

                string val = string.Empty;
                _config = new SMSXLRoleCatalogue();
                _config.Building = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Section = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.RoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.Description = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(val);

                if (_config.Level1ApproverType == ApprovalUserType.User)
                {
                    _config.Level1Approver = this.FormatEmployeeID(_config.Level1Approver);
                }
                if (_config.Level2ApproverType == ApprovalUserType.User)
                {
                    _config.Level2Approver = this.FormatEmployeeID(_config.Level2Approver);
                }
                if (_config.Level3ApproverType == ApprovalUserType.User)
                {
                    _config.Level3Approver = this.FormatEmployeeID(_config.Level3Approver);
                }

            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SMSXLRoleCatalogue _config = data as SMSXLRoleCatalogue;

            if (string.IsNullOrEmpty(_config.Building + _config.Description + _config.RoleName + _config.Description + _config.Level1Approver + _config.TechnicalRole))
            {
                return true;
            }

            return false;
        }
        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetOneCardRole();
            if (this.ExistingData == null)
                return false;
            else
            {
                this.Data.Id = this.ExistingData.Id;
                return true;
            }
        }
        private SMSXLRoleCatalogue GetOneCardRole()
        {
            var data = this.Data as SMSXLRoleCatalogue;
            if (data == null)
                return data;
            return SMSXLRoleCatalogue.GetOneCardRole(data.Building, data.Section, data.RoleName, data.TechnicalRole);
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SMS_XLRoleCatalogue";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SMSXLRoleCatalogue obj = this.Data as SMSXLRoleCatalogue;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        public override void Update(string currentUser)
        {
            try
            {
                SMSXLRoleCatalogue obj = this.Data as SMSXLRoleCatalogue;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        public override string GetWhoUpdatedData()
        {
            return SMSXLRoleCatalogue.GetWhoUpdatedData();
        }
    }
}
