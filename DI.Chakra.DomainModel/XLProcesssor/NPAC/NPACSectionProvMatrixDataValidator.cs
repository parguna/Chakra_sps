using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class NPACSectionProvMatrixDataValidator : XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get {
                if(endpointSystemId == 0)
                {
                    endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.NPAC).Id;
                }
                return endpointSystemId;
            }
            set { endpointSystemId = value; }
        }
        public override Type GetDomainModelType()
        {
            return typeof(NPACSectionInfo);
        }
        public override bool Validate(ref string validationMsg)
        {
            NPACSectionInfo obj = this.Data as NPACSectionInfo;

            bool isValid = true;
            bool isExist = true;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = string.IsNullOrEmpty(obj.HRSectionName);
            if (isExist)
            {
                isExist = false;
            }
            else
            {
                isExist = true;
            }
            isValid = isValid & isExist;
            if (isExist == false)
                missingFields.Add("HR SectionName");

            isExist = string.IsNullOrEmpty(obj.NPACSection);
            if (isExist)
            {
                isExist = false;
            }
            else
            {
                isExist = true;
            }
            isValid = isValid & isExist;
            if (isExist == false)
                missingFields.Add("M Code");

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

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                return new NPACSectionInfo()
                {
                    HRSectionName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    NPACSection = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    CreatedDate = DateTime.Now,
                    CreatedBy = "System",
                    LastModifiedBy = "System",
                    LastModifiedDate = DateTime.Now
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NPACSectionInfo _config = data as NPACSectionInfo;

            if (string.IsNullOrEmpty(_config.NPACSection + _config.HRSectionName))
            {
                return true;
            }

            return false;
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNPACXLProvisioningMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NPAC_SectionInfo";
            return base.TruncateTable();
            //return NPACSectionInfo.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                NPACSectionInfo obj = this.Data as NPACSectionInfo;
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
                NPACSectionInfo obj = this.Data as NPACSectionInfo;
                obj.EndPointSystemId = EndpointSystemId;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private NPACSectionInfo GetNPACXLProvisioningMatrixbyObject()
        {
            NPACSectionInfo obj = this.Data as NPACSectionInfo;
            return NPACSectionInfo.GetNPACXLProvisioningMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return NPACSectionInfo.GetWhoUpdatedData();
        }
    }
}
