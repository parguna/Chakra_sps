using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPHANACriticalTCodeMatrixDataValidator : XLDataValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPHANAXLCriticalTCodeMatrix _config = null;
            try
            {
                _config = new SAPHANAXLCriticalTCodeMatrix();

                _config.Application = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.Transaction = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.TransactionDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.TransactionOwner = this.FormatEmployeeID(CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()));
            }
            catch (Exception)
            {
                return null;
            }
            return _config;
        }
        public override Type GetDomainModelType()
        {
            return typeof(SAPHANAXLCriticalTCodeMatrix);
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPHANAXLCriticalTCodeMatrix _config = data as SAPHANAXLCriticalTCodeMatrix;

            if (string.IsNullOrWhiteSpace(_config.Application + _config.Transaction + _config.TransactionDescription + _config.TransactionOwner))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            SAPHANAXLCriticalTCodeMatrix obj = this.Data as SAPHANAXLCriticalTCodeMatrix;
            if (string.IsNullOrWhiteSpace(obj.Application))
            {
                missingFields.Add("Application");
                return false;
            }
            var ep = DomainModel.EndPoint.GetByApplicationCode(obj.Application);
            if (ep == null)
            {
                missingFields.Add($"Application Not found - {obj.Application}");
                return false;
            }
            ApplicationType applicationType = obj.Application == ep.ApplicationCode ? ApplicationType.EBS : obj.Application == ep.ApplicationCode ? ApplicationType.MSSL : ApplicationType.None;
            if (ep.ApplicationType == ApplicationType.None)
            {
                missingFields.Add($"Application not supported - {obj.Application}");
                return false;
            }
            long endpointSystemId = ep.Id;
            obj.EndpointSystemId = endpointSystemId;
            bool isExist = false;
            

            isExist = (!string.IsNullOrEmpty(obj.Transaction)) && this.IsSAPHANATCodeValid(obj.Transaction);
            if (!isExist)
                missingFields.Add("Transaction");

            if (!this.IsUserValid(obj.TransactionOwner, ApprovalUserType.User))
                missingFields.Add("Transaction Owner");

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
            this.ExistingData = GetSAPHANAXLCriticalTCodeMatrixbyObject();
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
            TableName = "DI_SAPHANA_XLCriticalTCodeMatrix";
            return base.TruncateTable();
            //return SAPHANAXLCriticalTCodeMatrix.TruncateTable();
        }
        public override void Update(string currentUser)
        {
            try
            {
                SAPHANAXLCriticalTCodeMatrix obj = this.Data as SAPHANAXLCriticalTCodeMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        public override void Add(string currentUser)
        {
            try
            {
                SAPHANAXLCriticalTCodeMatrix obj = this.Data as SAPHANAXLCriticalTCodeMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        private SAPHANAXLCriticalTCodeMatrix GetSAPHANAXLCriticalTCodeMatrixbyObject()
        {
            SAPHANAXLCriticalTCodeMatrix obj = this.Data as SAPHANAXLCriticalTCodeMatrix;
            return SAPHANAXLCriticalTCodeMatrix.GetSAPHANAXLCriticalTCodesMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPHANAXLCriticalTCodeMatrix.GetWhoUpdatedData();
        }
        public override bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                return SAPHANAXLCriticalTCodeMatrix.IsEmployeeConfiguredAsApprover(employeeId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
