using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPSRMEnablerRoleMatrixDataValidator: XLDataValidator
    {
        public override Type GetDomainModelType()
        {
            return typeof(SAPSRMEnablerRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                string val = string.Empty;

                SAPSRMEnablerRoleMatrix _config = new SAPSRMEnablerRoleMatrix();
                _config.SRMEnablerRule = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.POrg = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.PGrp = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.SRMTechRole = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.BIPOrgRole = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.BIPGrpRole = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                return _config;
            }
            catch (Exception)
            {

                return null;
            }
        }
        public override bool IsEmptyRow(IXLData data)
        {
            SAPSRMEnablerRoleMatrix _config = data as SAPSRMEnablerRoleMatrix;

            if (string.IsNullOrEmpty(_config.SRMEnablerRule + _config.POrg + _config.PGrp + _config.SRMTechRole + _config.BIPGrpRole + _config.BIPOrgRole))
            {

                return true;
            }

            return false;
        }
        public override bool Validate(ref string validationMsg)
        {
            SAPSRMEnablerRoleMatrix obj = this.Data as SAPSRMEnablerRoleMatrix;
            //  var srmroleExist = IsSAPSRMMainRoleExist(obj.SRMEnablerRule);
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (string.IsNullOrEmpty(obj.SRMEnablerRule))
            {
                missingFields.Add(" Business Role");
            }

            if (!string.IsNullOrEmpty(obj.SRMTechRole))
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.SAP_SRP, obj.SRMTechRole);
                if (!srmRoleExist)
                {
                    missingFields.Add("SRM enabler roles");
                }
            }

            if (!string.IsNullOrEmpty(obj.BIPGrpRole))
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.FLINS_BI, obj.BIPGrpRole);
                if (!srmRoleExist)
                {
                    missingFields.Add("BI Grp roles");
                }
            }

            if (!string.IsNullOrEmpty(obj.BIPOrgRole))
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.FLINS_BI, obj.BIPOrgRole);
                if (!srmRoleExist)
                {
                    missingFields.Add("BI Org roles");
                }
            }

            //if (!srmroleExist)
            //    missingFields.Add("SRM Technical Role");
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
            this.ExistingData = GetSAPSRMEnablerRoleMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SAP_SRMEnablerRoleMatrix";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPSRMEnablerRoleMatrix obj = this.Data as SAPSRMEnablerRoleMatrix;
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
                SAPSRMEnablerRoleMatrix obj = this.Data as SAPSRMEnablerRoleMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPSRMEnablerRoleMatrix GetSAPSRMEnablerRoleMatrixbyObject()
        {
            SAPSRMEnablerRoleMatrix obj = this.Data as SAPSRMEnablerRoleMatrix;
            return SAPSRMEnablerRoleMatrix.GetSAPSRMEnablerRoleMatrixbyObject(obj);
        }
        private bool IsSAPSRMMainRoleExist(string techRole)
        {
            SAPSRMEnablerRoleMatrix obj = this.Data as SAPSRMEnablerRoleMatrix;
            return SAPSRMEnablerRoleMatrix.IsSAPSRMMainRoleExist(techRole);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPSRMEnablerRoleMatrix.GetWhoUpdatedData();
        }
    }
}
