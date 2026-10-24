using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPRoleEligibilityMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPRoleMatrix _config = null;
            try
            {
                _config = new SAPRoleMatrix();

                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.EligibilityRule = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Company = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.PersonnelArea = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.PersonnelSubArea = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Position = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.OrgUnit = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.CostCenter = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Remarks = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPRoleMatrix _config = data as SAPRoleMatrix;
            if (string.IsNullOrEmpty(_config.Application + _config.EligibilityRule + _config.Company + _config.PersonnelArea + _config.PersonnelSubArea
                   + _config.Position + _config.OrgUnit + _config.CostCenter + _config.Remarks))
            {
                return true;
            }
            return false;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPRoleMatrix obj = this.Data as SAPRoleMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (!string.IsNullOrEmpty(obj.PersonnelArea) && !obj.PersonnelArea.Contains("*"))
            {
                isExist = this.IsPersonnelAreaValid(obj.PersonnelArea);
                if (isExist == false)
                    missingFields.Add("PersonnelArea");
            }

            if (!string.IsNullOrEmpty(obj.PersonnelSubArea) && !obj.PersonnelSubArea.Contains("*"))
            {
                isExist = this.IsPersonnelSubAreaValid(obj.PersonnelSubArea);
                if (isExist == false)
                    missingFields.Add("PersonnelSubArea");
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
        public override bool IsExists(string currentUser)
        {
            return false;
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SAP_RoleMatrix";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPRoleMatrix obj = this.Data as SAPRoleMatrix;
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
                SAPRoleMatrix obj = this.Data as SAPRoleMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPRoleMatrix GetSAPRoleMatrixbyObject()
        {
            SAPRoleMatrix obj = this.Data as SAPRoleMatrix;
            return SAPRoleMatrix.GetSAPRoleMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPRoleMatrix.GetWhoUpdatedData();
        }
    }
}
