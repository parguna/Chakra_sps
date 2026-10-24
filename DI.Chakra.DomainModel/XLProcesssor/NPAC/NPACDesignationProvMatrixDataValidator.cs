using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class NPACDesignationProvMatrixDataValidator : XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get
            {
                if (endpointSystemId == 0)
                {
                    endpointSystemId = EndPoint.GetByApplicationType(ApplicationType.NPAC).Id;
                }
                return endpointSystemId;
            }
            set { endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(NPACDesignationInfo);
        }
        public override bool Validate(ref string validationMsg)
        {
            NPACDesignationInfo obj = this.Data as NPACDesignationInfo;

            bool isValid = true;
            bool isExist = true;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = string.IsNullOrEmpty(obj.NPACDesignation);
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
                missingFields.Add("NPAC Designation");

            isExist = string.IsNullOrEmpty(obj.HRDesignationName);
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
                missingFields.Add("HR Designation");

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
                return new NPACDesignationInfo()
                {
                    HRDesignationName = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    NPACDesignation = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
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
            NPACDesignationInfo _config = data as NPACDesignationInfo;

            if (string.IsNullOrEmpty(_config.NPACDesignation + _config.HRDesignationName))
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
            TableName = "DI_NPAC_DesignationInfo";
            return base.TruncateTable();
            //return NPACDesignationInfo.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                NPACDesignationInfo obj = this.Data as NPACDesignationInfo;
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
                NPACDesignationInfo obj = this.Data as NPACDesignationInfo;
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
        private NPACDesignationInfo GetNPACXLProvisioningMatrixbyObject()
        {
            NPACDesignationInfo obj = this.Data as NPACDesignationInfo;
            return NPACDesignationInfo.GetNPACXLProvisioningMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return NPACDesignationInfo.GetWhoUpdatedData();
        }
    }
}
