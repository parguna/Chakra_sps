using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Office365LicenseValidator: XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get
            {
                if (endpointSystemId == 0)
                {
                    endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.Office365).Id;
                }
                return endpointSystemId;
            }
            set { endpointSystemId = value; }
        }
        public override Type GetDomainModelType()
        {
            return typeof(Office365LicenseConfig);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            Office365LicenseConfig _config = null;
            try
            {
                string val = string.Empty;
                _config = new Office365LicenseConfig();
                _config.AccountSkuId = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.LicenseDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.ServicePlan = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.ServicePlanDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            Office365LicenseConfig _config = data as Office365LicenseConfig;

            if (string.IsNullOrEmpty(_config.AccountSkuId + _config.LicenseDescription + _config.ServicePlan + _config.ServicePlanDescription))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            Office365LicenseConfig obj = this.Data as Office365LicenseConfig;
            List<string> missingFields = new List<string>();
            missingFields.Clear();
            if (string.IsNullOrEmpty(obj.AccountSkuId))
            {
                missingFields.Add(" AccountSkuId");
            }

            if (string.IsNullOrEmpty(obj.LicenseDescription))
            {
                missingFields.Add(" License Description");
            }

            //if (string.IsNullOrEmpty(obj.ServicePlan))
            //{
            //    missingFields.Add(" Service Plan");
            //}
            //if (string.IsNullOrEmpty(obj.ServicePlanDescription))
            //{
            //    missingFields.Add(" Service Plan Description");
            //}

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
            this.ExistingData = GetOffice365LicenseConfig();

            if (ExistingData == null)
                return false;
            else
                return true;
        }


        public override bool TruncateTable()
        {
            TableName = "DI_Office365_LicenseConfig";
            return base.TruncateTable();
            //return Office365LicenseConfig.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                Office365LicenseConfig obj = this.Data as Office365LicenseConfig;
                obj.EndPointSystemId = EndpointSystemId;
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
                Office365LicenseConfig obj = this.Data as Office365LicenseConfig;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private Office365LicenseConfig GetOffice365LicenseConfig()
        {
            Office365LicenseConfig obj = this.Data as Office365LicenseConfig;
            return Office365LicenseConfig.GetOffice365LicenseConfig(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return Office365LicenseConfig.GetWhoUpdatedData();
        }
    }
}
