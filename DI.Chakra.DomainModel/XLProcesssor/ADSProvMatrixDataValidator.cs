using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ADSProvMatrixDataValidator : XLDataValidator
    {
        public override bool Validate(ref string validationMsg)
        {
            ADSXLProvisioningMatrix obj = this.Data as ADSXLProvisioningMatrix;

            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            isExist = (!string.IsNullOrEmpty(obj.CompanyCode)) && this.IsCompanyCodeValid(obj.CompanyCode);
            if (!isExist)
                missingFields.Add("Company Code");

            isExist = (!string.IsNullOrEmpty(obj.DepartmentCode)) && this.IsPersonnelAreaValid(obj.DepartmentCode);
            if (!isExist)
                missingFields.Add("Department Code");

            isExist = ((!string.IsNullOrEmpty(obj.DivisionCode)) && this.IsPersonnelSubAreaValid(obj.DivisionCode)) || obj.DivisionCode.Trim(new char[] { '0' }).Length == 0;
            if (!isExist)
                missingFields.Add("Branch Code");

            isExist = (!string.IsNullOrEmpty(obj.OrganisationId)) && this.IsOrgUnitValid(obj.OrganisationId);
            if (!isExist)
                missingFields.Add("Organisation Id");

            //isExist = (!string.IsNullOrEmpty(obj.CostCenterCode)) && this.IsCostCenterValid(obj.CostCenterCode);
            //if (!isExist)
            //    missingFields.Add("CostCenter Code");

            //if (!string.IsNullOrEmpty(obj.CostDistributionCode))
            //{
            //    isExist = this.IsCostCenterValid(obj.CostDistributionCode);
            //    if (!isExist)
            //        missingFields.Add("CostDistribution Code");
            //}

            isExist = (!string.IsNullOrEmpty(obj.OU)) && (this.IsADOUValid(ApplicationType.ADSingaporePowerLocal, obj.OU) || this.IsADOUValid(ApplicationType.ADSDCLocal, obj.OU));
            if (!isExist)
                missingFields.Add("OU");

            isExist = (!string.IsNullOrEmpty(obj.DefaultGroups)) && (this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.DefaultGroups) || this.IsADGroupValid(ApplicationType.ADSDCLocal, obj.DefaultGroups));
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

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                return new ADSXLProvisioningMatrix()
                {
                    CompanyCode = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    DivisionCode = FormatLeadingZeros(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()), 4),
                    OrganisationId = FormatLeadingZeros(CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()), 8),
                    OrganisationName = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    //CostCenterCode = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    //CostDistributionCode = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    OU = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    DefaultGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString())
                    //,Id = !string.IsNullOrWhiteSpace(Convert.ToString(dRow.ItemArray.LastOrDefault())) ? Convert.ToInt64(CheckNullAndGetValueFromCell(Convert.ToString(dRow.ItemArray.LastOrDefault()))) : 0
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public override bool IsEmptyRow(IXLData data)
        {
            ADSXLProvisioningMatrix _config = data as ADSXLProvisioningMatrix;

            if (string.IsNullOrEmpty(_config.CompanyCode + _config.DepartmentCode + _config.DivisionCode + _config.OrganisationId + _config.OrganisationName +
                  _config.CostCenterCode + _config.CostDistributionCode + _config.OU + _config.DefaultGroups))
            {
                return true;
            }

            return false;
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetADSXLProvisioningMatrixbyObject();

            if (ExistingData == null)
                return false;
            else
            {
                return true;
            }
        }

        public override bool TruncateTable()
        {
            TableName = "DI_ADS_XLProvMatrix";
            return base.TruncateTable();
            //return ADSXLProvisioningMatrix.TruncateTable();
        }

        public override void Add(string currentUser)
        {
            try
            {
                ADSXLProvisioningMatrix obj = this.Data as ADSXLProvisioningMatrix;
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
                ADSXLProvisioningMatrix obj = this.Data as ADSXLProvisioningMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private ADSXLProvisioningMatrix GetADSXLProvisioningMatrixbyObject()
        {
            ADSXLProvisioningMatrix obj = this.Data as ADSXLProvisioningMatrix;
            return ADSXLProvisioningMatrix.GetADSXLProvisioningMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return ADSXLProvisioningMatrix.GetWhoUpdatedData();
        }

        public override Type GetDomainModelType()
        {
            return typeof(ADSXLProvisioningMatrix);
        }

        
    }
}
