using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPSODConflictedRoleMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPSODConflictingRole);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPSODConflictingRole _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPSODConflictingRole();
                _config.EndpointSystemName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.TechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.ConflictingTechnicalRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.ConflictingBusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.ConflictingRemarks = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.SODOwner = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());

            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPSODConflictingRole _config = data as SAPSODConflictingRole;
            if (string.IsNullOrEmpty(_config.EndpointSystemName + _config.BusinessRole + _config.ConflictingBusinessRole + _config.TechnicalRole + _config.ConflictingTechnicalRole))
            {
                return true;
            }
            return false;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPSODConflictingRole obj = this.Data as SAPSODConflictingRole;
            bool isValid = true;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            ApplicationType applicationType = ApplicationType.None;
            switch (obj.EndpointSystemName)
            {
                case Constants.MSSLMCP:
                    applicationType = ApplicationType.MSSL_CRM;
                    break;
                case Constants.MSSLMSP:
                    applicationType = ApplicationType.MSSL_ISU;
                    break;
                case Constants.EBSCRP:
                    applicationType = ApplicationType.EBS_CRM;
                    break;
                case Constants.EBSISP:
                    applicationType = ApplicationType.EBS_ISU;
                    break;
                case Constants.EBS:
                    applicationType = ApplicationType.EBS;
                    break;
                case Constants.MSSL:
                    applicationType = ApplicationType.MSSL;
                    break;
                default:
                    break;
            }

            if (applicationType == ApplicationType.None)
            {
                missingFields.Add("Endpoint Code");
                isValid = false;
            }

            isExist = (!string.IsNullOrEmpty(obj.TechnicalRole)) && this.IsSAPRoleValid(applicationType, obj.TechnicalRole);
            if (!isExist)
            {
                missingFields.Add("Technical Role");
                isValid = isValid & isExist;
            }
            
            

            isExist = (!string.IsNullOrEmpty(obj.ConflictingTechnicalRole)) && this.IsSAPRoleValid(applicationType, obj.ConflictingTechnicalRole);
            if (!isExist)
            {
                missingFields.Add("Conflicting Technical Role");
                isValid = isValid & isExist;
            }


            if (!this.IsUserValid(obj.SODOwner, ApprovalUserType.User))
            {
                missingFields.Add(" SOD Owner");
                isValid = false;
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
            return isValid;
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetSAPHRFLINSConflictingRolebyObject();
            if (ExistingData == null)
                return false;
            else
            {
                this.Data.Id = ExistingData.Id;
                return true;
            }
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SAP_SODConflictingRole";
            return base.TruncateTableWithBackup();
            //return SAPSODConflictingRole.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPSODConflictingRole obj = this.Data as SAPSODConflictingRole;
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
                SAPSODConflictingRole obj = this.Data as SAPSODConflictingRole;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPSODConflictingRole GetSAPHRFLINSConflictingRolebyObject()
        {
            SAPSODConflictingRole obj = this.Data as SAPSODConflictingRole;
            return SAPSODConflictingRole.GetSAPSODConflictingRolebyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPSODConflictingRole.GetWhoUpdatedData();
        }
    }
}
