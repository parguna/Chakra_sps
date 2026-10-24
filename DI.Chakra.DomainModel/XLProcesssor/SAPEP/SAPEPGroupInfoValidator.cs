using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPEPGroupInfoValidator: XLDataValidator
    {
        private long? _endpointSystemId;

        private long? EndpointSystemId
        {
            get
            {
                if (_endpointSystemId == null)
                    _endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.SAP_EP).Id;
                return _endpointSystemId;
            }
            set { _endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(SAPEPGroupInfo);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPEPGroupInfo _config = null;
            try
            {
                string val = string.Empty;
                _config = new SAPEPGroupInfo();
                _config.GroupId = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.GroupName = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.GroupDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.GroupUniqueName = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
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
            SAPEPGroupInfo _config = data as SAPEPGroupInfo;

            if (string.IsNullOrEmpty(_config.GroupId + _config.GroupName + _config.GroupUniqueName))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPEPGroupInfo obj = this.Data as SAPEPGroupInfo;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrEmpty(obj.GroupId))
            {
                missingFields.Add(" GroupId");
            }

            if (string.IsNullOrEmpty(obj.GroupName))
            {
                missingFields.Add(" Group Name");
            }

            if (string.IsNullOrEmpty(obj.GroupUniqueName))
            {
                missingFields.Add(" Group UniqueName");
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
            this.ExistingData = GetSAPEPGroupObject();

            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            return SAPEPGroupInfo.TruncateTable(EndpointSystemId);
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPEPGroupInfo obj = this.Data as SAPEPGroupInfo;
                //obj.TenantId = "SINGAPOREPOWER";
                obj.CreatedDate = DateTime.Now;
                obj.EndPointSystemId = EndpointSystemId;
                obj.IsXLUploaded = true;
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
                SAPEPGroupInfo obj = GetSAPEPGroupObject();
                obj.LastModifiedDate = DateTime.Now;
                obj.IsXLUploaded = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPEPGroupInfo GetSAPEPGroupObject()
        {
            SAPEPGroupInfo obj = this.Data as SAPEPGroupInfo;
            return SAPEPGroupInfo.GetSAPEPGroupObject(EndpointSystemId, obj.GroupUniqueName);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPEPGroupInfo.GetWhoUpdatedData();
        }
    }
}
