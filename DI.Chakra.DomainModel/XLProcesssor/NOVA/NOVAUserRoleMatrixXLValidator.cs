using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class NOVAUserRoleMatrixXLValidator : XLDataValidator
    {
        readonly EndPoint FlinsEndpoint;

        readonly EndPoint FlinsBIEndpoint;
        readonly EndPoint endpoint;
        public NOVAUserRoleMatrixXLValidator()
        {
            FlinsEndpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
            FlinsBIEndpoint = EndPoint.GetByApplicationType(ApplicationType.FLINS_BI);
            endpoint = EndPoint.GetByApplicationType(ApplicationType.EAM);
        }

       
        private EndPoint EndpointSystem 
        {
            get
            {
                 
                return endpoint;
            }
            
        }
        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            NOVAUserRoleMatrix obj = data as NOVAUserRoleMatrix;
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
        public override Type GetDomainModelType()
        {
            return typeof(NOVAUserRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            NOVAUserRoleMatrix _config = null;
            try
            {
                _config = new NOVAUserRoleMatrix();
                _config.JobDescription = CheckNullAndGetValueFromCell(dRow["JobDescription"].ToString());
                _config.CompanyCode = CheckNullAndGetValueFromCell(dRow["Company Code"].ToString());
                _config.SectionCode = CheckNullAndGetValueFromCell(dRow["Section Code"].ToString());
                _config.Position = CheckNullAndGetValueFromCell(dRow["Position"].ToString());
                _config.FLINSRoles = CheckNullAndGetValueFromCell(dRow["FLINSRoles"].ToString());
                _config.BWAccess = CheckNullAndGetValueFromCell(dRow["BWAccess"].ToString());
                _config.ADGroups = CheckNullAndGetValueFromCell(dRow["AD Groups"].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow["Level1 Approver"].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level1 Approver Type"].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow["Level2 Approver"].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level2 Approver Type"].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow["Level3 Approver"].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level3 Approver Type"].ToString()));
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
                NOVAUserRoleMatrix obj = this.Data as NOVAUserRoleMatrix;
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
                NOVAUserRoleMatrix obj = this.Data as NOVAUserRoleMatrix;
                var exObj = GetNOVAUserRoleMatrixbyObject();
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
            return NOVAUserRoleMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNOVAUserRoleMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private NOVAUserRoleMatrix GetNOVAUserRoleMatrixbyObject()
        {
            return NOVAUserRoleMatrix.GetNOVAUserRoleMatrixbyObject(this.Data as NOVAUserRoleMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NOVA_UserRoleMatrix";
            return base.TruncateTable();
            //return NOVAUserRoleMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NOVAUserRoleMatrix _config = data as NOVAUserRoleMatrix;

            if (string.IsNullOrWhiteSpace( _config.CompanyCode + _config.SectionCode + _config.Position + _config.FLINSRoles + (_config.BWAccess??"") + _config.ADGroups + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            NOVAUserRoleMatrix obj = this.Data as NOVAUserRoleMatrix;

            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.CompanyCode))
            {
                missingFields.Add("Company Code");
            }
            //else if (!IsCompanyCodeValid(obj.CompanyCode))
            //{
            //    missingFields.Add("Company Code");
            //}

            //if (string.IsNullOrWhiteSpace(obj.SectionCode))
            //{
            //    missingFields.Add("Section Code");
            //}
            //else if (!IsSAPSectionCodeValid(obj.SectionCode))
            //{
            //    missingFields.Add("Section Code");
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
