using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class NOVAManagementRoleMatrixXLValidator : XLDataValidator
    {
        readonly EndPoint FlinsEndpoint;

        readonly EndPoint FlinsBIEndpoint;
        readonly EndPoint EAM_Endpoint;
        public NOVAManagementRoleMatrixXLValidator()
        {
            FlinsEndpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
            FlinsBIEndpoint = EndPoint.GetByApplicationType(ApplicationType.FLINS_BI);
            EAM_Endpoint = EndPoint.GetByApplicationType(ApplicationType.EAM);
        }

        public override Type GetDomainModelType()
        {
            return typeof(NOVAManagementRoleMatrix);
        }
        private EndPoint EndpointSystem 
        {
            get
            {
                 
                return EAM_Endpoint;
            }
             
        }

        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            NOVAManagementRoleMatrix obj = data as NOVAManagementRoleMatrix;
            List<string> missingFields = new List<string>();

            var froles = obj.FLINSRoles.Split(new char[] { ',' });
            foreach (var role in froles)
            {
                if (!string.IsNullOrWhiteSpace(role) && !IsTCodeValid(FlinsEndpoint, role))
                {
                    missingFields.Add($"FLINSRoles :{role}");
                }
            }

            if (!string.IsNullOrWhiteSpace(obj.BWAccess))
            {

                var fbroles = obj.BWAccess.Split(new char[] { ',' });
                foreach (var role in fbroles)
                {
                    if (!string.IsNullOrWhiteSpace(role) && !IsTCodeValid(FlinsBIEndpoint, role))
                    {
                        missingFields.Add($"BWAccess:{role}");
                    }
                }

            }

            if (!string.IsNullOrWhiteSpace(obj.ADGroups))
            {
                var ADGroups = obj.ADGroups.Split(new char[] { ',' });
                foreach (var grp in ADGroups)
                {
                    if (!string.IsNullOrWhiteSpace(grp) && !IsADGroupValid(ApplicationType.ADSingaporePowerLocal, grp))
                    {
                        missingFields.Add($"ADGroups : {grp}");
                    }
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
                validationMsg = $"Found invalid technicalroles in {validationMsg} for the position {obj.BusinessRole} ";

            return missingFields.Count == 0;
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            NOVAManagementRoleMatrix _config = null;
            try
            {
                _config = new NOVAManagementRoleMatrix();
                _config.JobDescription = CheckNullAndGetValueFromCell(dRow["JobDescription"].ToString());
                _config.Department = CheckNullAndGetValueFromCell(dRow["Department"].ToString());
                _config.Position = CheckNullAndGetValueFromCell(dRow["Position"].ToString());
                _config.FLINSRoles = CheckNullAndGetValueFromCell(dRow["FLINSRoles"].ToString());
                _config.BWAccess = CheckNullAndGetValueFromCell(dRow["BWAccess"].ToString());
                _config.ADGroups = CheckNullAndGetValueFromCell(dRow["AD Groups"].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow["Level1 Approver"].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level1 Approver Type"].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow["Level2 Approver"].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level2 Approver Type"].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow ["Level3 Approver"].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level3 Approver Type"].ToString()));
                _config.EndPointSystemId = EAM_Endpoint.Id;
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

            }
            catch (Exception)
            {

                return null;
            }
            return _config;
        }
        public override void Add(string currentUser)
        {
            try
            {
                NOVAManagementRoleMatrix obj = this.Data as NOVAManagementRoleMatrix;
                obj.EndPointSystemId = EndpointSystem.Id;
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
                NOVAManagementRoleMatrix obj = this.Data as NOVAManagementRoleMatrix;
                var exObj = GetNOVAManagementRoleMatrixbyObject();
                if(exObj != null)
                {
                    obj.Id = exObj.Id;
                }
                obj.EndPointSystemId = EndpointSystem.Id;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        public override string GetWhoUpdatedData()
        {
            return NOVAManagementRoleMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNOVAManagementRoleMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private NOVAManagementRoleMatrix GetNOVAManagementRoleMatrixbyObject()
        {
            return NOVAManagementRoleMatrix.GetNOVAManagementRoleMatrixbyObject(this.Data as NOVAManagementRoleMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NOVA_ManagementRoleMatrix";
            return base.TruncateTable();
            //return NOVAManagementRoleMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NOVAManagementRoleMatrix _config = data as NOVAManagementRoleMatrix;

            if (string.IsNullOrWhiteSpace(_config.Department + _config.Position + _config.FLINSRoles + _config.BWAccess??"" + _config.ADGroups + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            NOVAManagementRoleMatrix obj = this.Data as NOVAManagementRoleMatrix;

            List<string> missingFields = new List<string>();

            obj.EndPointSystemId = EAM_Endpoint.Id;

            if (string.IsNullOrWhiteSpace(obj.Department))
            {
                missingFields.Add("Department");
            }
            // else if (!IsPersonnelAreaValid(obj.Department))
            //{
            //    missingFields.Add("Department");
            //}

            if (string.IsNullOrWhiteSpace(obj.Position))
            {
                missingFields.Add("Position");
            }
            //else if (!IsPositionValid(obj.Position))
            //{
            //    missingFields.Add("Position");
            //}

            if (string.IsNullOrWhiteSpace(obj.FLINSRoles))
            {
                missingFields.Add("FLINSRoles");
            }

            var froles = obj.FLINSRoles.Split(new char[] { ',' });
            foreach (var role in froles)
            {
                if (!string.IsNullOrWhiteSpace(role) && !IsTCodeValid(FlinsEndpoint, role))
                {
                    missingFields.Add("FLINSRoles");
                }
            }



            if (!string.IsNullOrWhiteSpace(obj.BWAccess))
            {
                var fbroles = obj.BWAccess.Split(new char[] { ',' });
                foreach (var role in fbroles)
                {
                    if (!string.IsNullOrWhiteSpace(role) && !IsTCodeValid(FlinsBIEndpoint, role))
                    {
                        missingFields.Add("BWAccess");
                    }
                }

            }

            if (!string.IsNullOrWhiteSpace(obj.ADGroups))
            {
                var ADGroups = obj.ADGroups.Split(new char[] { ',' });
                foreach (var grp in ADGroups)
                {
                    if (!string.IsNullOrWhiteSpace(grp) && !IsADGroupValid(ApplicationType.ADSingaporePowerLocal, grp))
                    {
                        missingFields.Add("ADGroups");
                    }
                }
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
    }
}
