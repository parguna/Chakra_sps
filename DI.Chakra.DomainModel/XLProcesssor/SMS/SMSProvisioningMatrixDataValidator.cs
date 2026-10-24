using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SMSProvisioningMatrixDataValidator : XLDataValidator
    {
        readonly long endpointSystemId;

        public SMSProvisioningMatrixDataValidator()
        {
            endpointSystemId = EndPoint.GetEndPointByApplicationType(ApplicationType.OneCard).Id;
        }

        public override Type GetDomainModelType()
        {
            return typeof(SMSXLProvisioningMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SMSXLProvisioningMatrix _config = null;
            try
            {

                string val = string.Empty;
                _config = new SMSXLProvisioningMatrix();
                _config.PostalCode = FormatLeadingZeros(CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()), 6);
                _config.Location = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.DepartmentName = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.DivisionCode = FormatLeadingZeros(CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()), 4);
                _config.DivisionName = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.DefaultGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());

            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SMSXLProvisioningMatrix _config = data as SMSXLProvisioningMatrix;

            if (string.IsNullOrEmpty(_config.PostalCode + _config.Location + _config.DepartmentCode + _config.DefaultGroups + _config.DepartmentName + _config.DivisionCode + _config.DivisionName))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SMSXLProvisioningMatrix obj = this.Data as SMSXLProvisioningMatrix;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrEmpty(obj.PostalCode) || !User.IsPostalCodeExist(obj.PostalCode))
            {
                missingFields.Add(" Postal Code");
            }
            bool isExist = true;
            isExist = (!string.IsNullOrEmpty(obj.DepartmentCode)) && this.IsPersonnelAreaValid(obj.DepartmentCode);
            if (!isExist)
                missingFields.Add("Department Code");

            isExist = (!string.IsNullOrEmpty(obj.DivisionCode)) && this.IsPersonnelSubAreaValid(obj.DivisionCode);
            if (!isExist)
                missingFields.Add("Branch Code");

            isExist = (!string.IsNullOrEmpty(obj.DefaultGroups)) && (this.IsSMSRoleValid(endpointSystemId, obj.DefaultGroups)); //Check and update
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultGroups))
                missingFields.Add("Default Groups");

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
            TableName = "DI_SMS_XLProvMatrix";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SMSXLProvisioningMatrix obj = this.Data as SMSXLProvisioningMatrix;
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
                SMSXLProvisioningMatrix obj = this.Data as SMSXLProvisioningMatrix;
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
            return SMSXLProvisioningMatrix.GetWhoUpdatedData();
        }
    }
}
