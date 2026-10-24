using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPEPRoleInfoXLValidator : XLDataValidator
    {
        private long? _endpointSystemId;

        private long? EndpointSystemId
        {
            get {
                if (_endpointSystemId == null)
                    _endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.SAP_EP).Id;
                return _endpointSystemId;
            }
            set { _endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(SAPEPRoleInfo);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPEPRoleInfo _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPEPRoleInfo();
                _config.RoleId = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.RoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.RoleDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.UniqueRoleName = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Version = string.IsNullOrEmpty(CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString())) ? 0 : Convert.ToInt32(CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()));

            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPEPRoleInfo _config = data as SAPEPRoleInfo;

            if (string.IsNullOrEmpty(_config.RoleId + _config.RoleName + _config.UniqueRoleName))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            //  return true;

            SAPEPRoleInfo obj = this.Data as SAPEPRoleInfo;
            obj = this.Data as SAPEPRoleInfo;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrEmpty(obj.RoleId))
            {
                missingFields.Add(" RoleId");
            }

            if (string.IsNullOrEmpty(obj.RoleName))
            {
                missingFields.Add(" Role Name");
            }

            if (string.IsNullOrEmpty(obj.UniqueRoleName))
            {
                missingFields.Add(" Role UniqueName");
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
            this.ExistingData = GetSAPEPRoleObject();

            if (ExistingData == null)
                return false;
            else
                return true;
        }
        public override bool TruncateTable()
        {
            return SAPEPRoleInfo.TruncateTable(EndpointSystemId);
        }
        public override void Update(string currentUser)
        {
            try
            {
                SAPEPRoleInfo obj = GetSAPEPRoleObject();
                obj.LastModifiedDate = DateTime.Now;
                obj.IsXLUploaded = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPEPRoleInfo obj = this.Data as SAPEPRoleInfo;
                //obj.TenantId = "SINGAPOREPOWER";
                obj.CreatedDate = DateTime.Now;
                obj.EndPointSystemId = EndpointSystemId.Value;
                obj.IsXLUploaded = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPEPRoleInfo GetSAPEPRoleObject()
        {
            SAPEPRoleInfo obj = this.Data as SAPEPRoleInfo;
            return SAPEPRoleInfo.GetSAPEPRoleObject(EndpointSystemId, obj.UniqueRoleName);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPEPRoleInfo.GetWhoUpdatedData();
        }
    }
}
