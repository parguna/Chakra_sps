using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPSRMBusinessRoleDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPSRMBusinessRoleMapping);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                string val = string.Empty;

                SAPSRMBusinessRoleMapping _config = new SAPSRMBusinessRoleMapping();
                var pernr = FormatEmployeeID(CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()));
                _config.EmployeeId = pernr;
                var user = User.GetByEmployeeId(pernr);
                if (user != null)
                {
                    _config.UserId = user.Id;
                }
                _config.UserName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.AccountType = (AccountType)(Convert.ToInt32(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString())));
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.POrg = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.PGrp = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                return _config;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPSRMBusinessRoleMapping _config = data as SAPSRMBusinessRoleMapping;

            if (string.IsNullOrEmpty(_config.BusinessRole + _config.POrg + _config.PGrp + _config.UserId + _config.UserName))
            {

                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPSRMBusinessRoleMapping obj = this.Data as SAPSRMBusinessRoleMapping;
            //  var srmroleExist = IsSAPSRMMainRoleExist(obj.SRMEnablerRule);
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            isExist = (obj.UserId != 0 && !string.IsNullOrEmpty(obj.UserId.ToString()));
            if (!isExist)
                missingFields.Add("Pernr");

            isExist = ((!string.IsNullOrEmpty(obj.UserName)) && IsSRMAcountnameValid(obj.UserName, obj.AccountType));
            if (!isExist)
                missingFields.Add("SAPLogonName");

            isExist = (!string.IsNullOrEmpty(obj.BusinessRole) && IsSRMBussinessRoleValid(obj.BusinessRole));
            if (!isExist)
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

            return missingFields.Count == 0;
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetSAPSRMBusinessRoleMappingbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        private SAPSRMBusinessRoleMapping GetSAPSRMBusinessRoleMappingbyObject()
        {
            SAPSRMBusinessRoleMapping obj = this.Data as SAPSRMBusinessRoleMapping;
            return SAPSRMBusinessRoleMapping.GetSAPSRMBusinessRoleMappingbyObject(obj);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_SAP_SAPSRMBusinessRoleMapping";
            return base.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPSRMBusinessRoleMapping obj = this.Data as SAPSRMBusinessRoleMapping;
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
                SAPSRMBusinessRoleMapping obj = this.Data as SAPSRMBusinessRoleMapping;
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
            return SAPSRMBusinessRoleMapping.GetWhoUpdatedData();
        }
    }
}
