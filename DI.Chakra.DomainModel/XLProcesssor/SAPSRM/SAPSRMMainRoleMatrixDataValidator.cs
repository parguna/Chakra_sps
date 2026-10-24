using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class SAPSRMMainRoleMatrixDataValidator: XLDataValidator
    {
        readonly long flinsBI_endpointSystemId;
        readonly long sus_endpointSystemId;
        readonly long srp_endpointSystemId;
        public SAPSRMMainRoleMatrixDataValidator()
        {
            flinsBI_endpointSystemId = GetEndpointSystemID(ApplicationType.FLINS_BI);
            sus_endpointSystemId = GetEndpointSystemID(ApplicationType.SAP_SUS);
            srp_endpointSystemId = GetEndpointSystemID(ApplicationType.SAP_SRP);
        }
        public override Type GetDomainModelType()
        {
            return typeof(SAPSRMMainRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                string val = string.Empty;
                SAPSRMMainRoleMatrix _config = new SAPSRMMainRoleMatrix();
                _config.RoleType = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                _config.BusinessRole = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.InternalPortalRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.InternalPortalGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.ExternalPortalRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.ExternalPortalGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.SRMRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.SUSRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.SRMEnablerRule = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.BIRoles = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.ADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString());

                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[14].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(val);
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[15].ToString());
                val = CheckNullAndGetValueFromCell(dRow.ItemArray[16].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(val);
                _config.DepartmentCode = CheckNullAndGetValueFromCell(dRow.ItemArray[17].ToString());

                if (_config.Level1ApproverType == ApprovalUserType.User)
                {
                    _config.Level1Approver = this.FormatEmployeeID(_config.Level1Approver);
                }
                if (_config.Level2ApproverType == ApprovalUserType.User)
                {
                    _config.Level2Approver = this.FormatEmployeeID(_config.Level2Approver);
                }
                if (_config.Level3ApproverType == ApprovalUserType.User)
                {
                    _config.Level3Approver = this.FormatEmployeeID(_config.Level3Approver);
                }

                return _config;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            SAPSRMMainRoleMatrix obj = data as SAPSRMMainRoleMatrix;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (!string.IsNullOrWhiteSpace(obj.SRMRoles))
            {
                bool srmRoleExist = IsSAPRoleValid(srp_endpointSystemId, obj.SRMRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add($"SRM roles");
                }
            }
            if (!string.IsNullOrWhiteSpace(obj.SUSRoles))
            {
                bool srmRoleExist = IsSAPRoleValid(sus_endpointSystemId, obj.SUSRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add($"SUS roles");
                }
            }
            if (!string.IsNullOrWhiteSpace(obj.BIRoles))
            {
                bool srmRoleExist = IsSAPRoleValid(flinsBI_endpointSystemId, obj.BIRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add($"BI roles");
                }
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
                validationMsg = $"Found Invalid {validationMsg} for the Business Role { obj.BusinessRole}";

            return missingFields.Count == 0;
        }

        public override bool IsEmptyRow(IXLData data)
        {
            SAPSRMMainRoleMatrix _config = data as SAPSRMMainRoleMatrix;

            if (string.IsNullOrEmpty(_config.BusinessRole + _config.RoleType + _config.SRMEnablerRule + _config.SRMRoles + _config.SUSRoles + _config.InternalPortalGroups + _config.InternalPortalRoles + _config.ExternalPortalGroups + _config.ExternalPortalRoles))
            {

                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            SAPSRMMainRoleMatrix obj = this.Data as SAPSRMMainRoleMatrix;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (obj.BusinessRole == string.Empty)
            {
                missingFields.Add(" Business Role");
            }
            if (!this.IsUserValid(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (!this.IsUserValid(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }

            if (!this.IsUserValid(obj.Level3Approver, obj.Level3ApproverType))
            {
                missingFields.Add(" Level3 Approver");
            }

            if (obj.ADGroups != null && obj.ADGroups != string.Empty)
            {
                bool adGroupExist = IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.ADGroups);
                if (!adGroupExist)
                {
                    missingFields.Add("Ad groups");
                }
            }
            if (obj.DepartmentCode != null && obj.DepartmentCode != string.Empty)
            {
                bool depCodeExist = IsPersonnelAreaValid(obj.DepartmentCode);
                if (!depCodeExist)
                {
                    missingFields.Add("Department Code");
                }
            }

            if (obj.SRMRoles != null && obj.SRMRoles != string.Empty)
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.SAP_SRP, obj.SRMRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add("SRM roles");
                }
            }
            if (obj.SUSRoles != null && obj.SUSRoles != string.Empty)
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.SAP_SUS, obj.SUSRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add("SUS roles");
                }
            }
            if (obj.BIRoles != null && obj.BIRoles != string.Empty)
            {
                bool srmRoleExist = IsSAPRoleValid(ApplicationType.FLINS_BI, obj.BIRoles);
                if (!srmRoleExist)
                {
                    missingFields.Add("BI roles");
                }
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
            this.ExistingData = GetSAPSRMMainRoleMatrixbyObject();
            if (ExistingData == null)
                return false;
            else
                return true;
        }
        public override bool TruncateTable()
        {
            TableName = "DI_SAP_SRMMainRoleMatrix";
            return base.TruncateTable();
        }
        public override void Add(string currentUser)
        {
            try
            {
                SAPSRMMainRoleMatrix obj = this.Data as SAPSRMMainRoleMatrix;
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
                SAPSRMMainRoleMatrix obj = this.Data as SAPSRMMainRoleMatrix;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.LastModifiedBy = currentUser;
                obj.LastModifiedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }
        private SAPSRMMainRoleMatrix GetSAPSRMMainRoleMatrixbyObject()
        {
            SAPSRMMainRoleMatrix obj = this.Data as SAPSRMMainRoleMatrix;
            return SAPSRMMainRoleMatrix.GetSAPSRMMainRoleMatrixbyObject(obj);
        }

        public override string GetWhoUpdatedData()
        {
            return SAPSRMMainRoleMatrix.GetWhoUpdatedData();
        }
    }
}
