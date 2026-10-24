using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPMSSLCriticalTCodeMatrixDataValidator : XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPXLMSSLCriticalTCodeMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SAPXLMSSLCriticalTCodeMatrix _config = null;
            try
            {
                _config = new SAPXLMSSLCriticalTCodeMatrix();

                _config.Transaction = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.TransactionDescription = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.TransactionOwner = this.FormatEmployeeID(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()));
            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPXLMSSLCriticalTCodeMatrix _config = data as SAPXLMSSLCriticalTCodeMatrix;

            if (string.IsNullOrEmpty(_config.Transaction + _config.TransactionDescription + _config.TransactionOwner))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPXLMSSLCriticalTCodeMatrix obj = this.Data as SAPXLMSSLCriticalTCodeMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.Transaction)) && this.IsSAPTCodeValid(obj.Transaction);
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
            this.ExistingData = GetSAPXLMSSLCriticalTCodeMatrixbyObject();
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
            TableName = "DI_SAP_MSSL_XL_CriticalTCodeMatrix";
            return base.TruncateTable();
        }
        public override void Update(string currentUser)
        {
            try
            {
                SAPXLMSSLCriticalTCodeMatrix obj = this.Data as SAPXLMSSLCriticalTCodeMatrix;
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
                SAPXLMSSLCriticalTCodeMatrix obj = this.Data as SAPXLMSSLCriticalTCodeMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        private SAPXLMSSLCriticalTCodeMatrix GetSAPXLMSSLCriticalTCodeMatrixbyObject()
        {
            SAPXLMSSLCriticalTCodeMatrix obj = this.Data as SAPXLMSSLCriticalTCodeMatrix;
            return SAPXLMSSLCriticalTCodeMatrix.GetSAPXLMSSLCriticalTCodeMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPXLMSSLCriticalTCodeMatrix.GetWhoUpdatedData();
        }
    }
}
