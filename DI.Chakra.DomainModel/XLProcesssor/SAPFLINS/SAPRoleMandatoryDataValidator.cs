using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPRoleMandatoryDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPRoleMandatoryField);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPRoleMandatoryField _config = null;
            try
            {
                _config = new SAPRoleMandatoryField();
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.MandatoryField1 = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.MandatoryField2 = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPRoleMandatoryField _config = data as SAPRoleMandatoryField;
            if (string.IsNullOrEmpty(_config.BusinessRole + _config.MandatoryField1 + _config.MandatoryField2))
                return true;

            return false;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPRoleMandatoryField obj = this.Data as SAPRoleMandatoryField;
            bool isValid = true;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = string.IsNullOrEmpty(obj.BusinessRole);
            if (!isExist)
            {
                isExist = SAPRoleMandatoryField.CheckBusinessRoleExists(obj.BusinessRole);
            }
            else
            {
                isExist = false;
            }
            isValid = isValid & isExist;
            if (isExist == false)
                missingFields.Add("Business Role");

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

            return isValid;

        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetSAPRoleMandatoryFieldbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_RoleMandatoryFields";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPRoleMandatoryField obj = this.Data as SAPRoleMandatoryField;
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
                SAPRoleMandatoryField obj = this.Data as SAPRoleMandatoryField;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.CreatedDate = obj.CreatedDate > DateTime.MinValue ? obj.CreatedDate : DateTime.MinValue;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPRoleMandatoryField GetSAPRoleMandatoryFieldbyObject()
        {
            SAPRoleMandatoryField obj = this.Data as SAPRoleMandatoryField;
            return SAPRoleMandatoryField.GetSAPRoleMandatoryFieldbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPRoleMandatoryField.GetWhoUpdatedData();
        }
    }
}
