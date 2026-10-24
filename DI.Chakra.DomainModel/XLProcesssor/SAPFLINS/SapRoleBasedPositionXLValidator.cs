using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SapRoleBasedPositionXLValidator: XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(ProvisioningConfigMatrixSAPPosition);
        }
        public override bool TruncateTable()
        {
            TableName = "DI_M_ProvisioningConfigMatrixSAPPosition";
            return base.TruncateTable();
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            ProvisioningConfigMatrixSAPPosition _config = null;
            try
            {
                _config = new ProvisioningConfigMatrixSAPPosition();

                _config.PositionText = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.DefaultRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            ProvisioningConfigMatrixSAPPosition _config = data as ProvisioningConfigMatrixSAPPosition;

            if (string.IsNullOrEmpty(_config.DefaultRole))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            ProvisioningConfigMatrixSAPPosition obj = this.Data as ProvisioningConfigMatrixSAPPosition;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.DefaultRole)) && this.IsSAPRoleValid(ApplicationType.HRMS_FLINS, obj.DefaultRole);
            if (!isExist && !string.IsNullOrEmpty(obj.DefaultRole))
                missingFields.Add("Default Role");

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

        public override void Add(string currentUser)
        {
            try
            {
                ProvisioningConfigMatrixSAPPosition obj = this.Data as ProvisioningConfigMatrixSAPPosition;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                ProvisioningConfigMatrixSAPPosition.Add(obj);
            }
            catch { throw; }
        }
        public override void Update(string currentUser)
        {
            ProvisioningConfigMatrixSAPPosition obj = this.Data as ProvisioningConfigMatrixSAPPosition;
            obj.LastModifiedBy = currentUser;
            obj.LastModifiedDate = DateTime.Now;
            ProvisioningConfigMatrixSAPPosition.ManualUpdate(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return ProvisioningConfigMatrixSAPPosition.GetWhoUpdatedData();
        }
    }
}
