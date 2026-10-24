using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPFLINSConflictedRoleMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPHRFLINSConflictingRole);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPHRFLINSConflictingRole _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPHRFLINSConflictingRole();
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.ConflictingBusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.ConflictingTechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPHRFLINSConflictingRole _config = data as SAPHRFLINSConflictingRole;
            if (string.IsNullOrEmpty(_config.BusinessRole + _config.ConflictingBusinessRole + _config.TechnicalRole + _config.ConflictingTechnicalRole))
            {
                return true;
            }
            return false;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPHRFLINSConflictingRole obj = this.Data as SAPHRFLINSConflictingRole;
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

            isExist = string.IsNullOrEmpty(obj.ConflictingBusinessRole);
            if (!isExist)
            {
                isExist = SAPRoleMandatoryField.CheckBusinessRoleExists(obj.ConflictingBusinessRole);
            }
            else
            {
                isExist = false;
            }
            isValid = isValid & isExist;
            if (isExist == false)
                missingFields.Add("Conflicting Business Role");

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
            this.ExistingData = GetSAPHRFLINSConflictingRolebyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SAP_SAPHRFLINSConflictingRole";
            return base.TruncateTable();
            //return SAPHRFLINSConflictingRole.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPHRFLINSConflictingRole obj = this.Data as SAPHRFLINSConflictingRole;
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
                SAPHRFLINSConflictingRole obj = this.Data as SAPHRFLINSConflictingRole;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPHRFLINSConflictingRole GetSAPHRFLINSConflictingRolebyObject()
        {
            SAPHRFLINSConflictingRole obj = this.Data as SAPHRFLINSConflictingRole;
            return SAPHRFLINSConflictingRole.GetSAPHRFLINSConflictingRolebyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHRFLINSConflictingRole.GetWhoUpdatedData();
        }
    }
}
