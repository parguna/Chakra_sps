using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class NOVAExceptionalRoleMatrixXLValidator: XLDataValidator
    {
        readonly EndPoint FlinsEndpoint;

        readonly EndPoint FlinsBIEndpoint;
        readonly EndPoint endPoint;
        public NOVAExceptionalRoleMatrixXLValidator()
        {
            FlinsEndpoint = EndPoint.GetByApplicationType(ApplicationType.HRMS_FLINS);
            FlinsBIEndpoint = EndPoint.GetByApplicationType(ApplicationType.FLINS_BI);
            endPoint = EndPoint.GetByApplicationType(ApplicationType.EAM);
        }
        public override Type GetDomainModelType()
        {
            return typeof(NOVAExceptionalRoleMatrix);
        }
        private EndPoint EndpointSystem 
        {
            get
            {
                 
                return endPoint ;
            }
            
        }

        public override bool ValidateTechnicalRole(IXLData data, ref string validationMsg)
        {
            NOVAExceptionalRoleMatrix obj = data as NOVAExceptionalRoleMatrix;
            List<string> missingFields = new List<string>();

            var froles = obj.FLINSRoles.Split(new char[] { ',' });
            foreach (var role in froles)
            {
                if (!string.IsNullOrWhiteSpace(role) && !IsTCodeValid(FlinsEndpoint, role))
                {
                    missingFields.Add($"FLINSRoles :{role}");
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
            NOVAExceptionalRoleMatrix _config = null;
            try
            {
                _config = new NOVAExceptionalRoleMatrix();

                _config.JobDescription = CheckNullAndGetValueFromCell(dRow["JobDescription"].ToString());
                _config.CompanyCode = CheckNullAndGetValueFromCell(dRow["Company Code"].ToString());
                _config.FLINSRoles = CheckNullAndGetValueFromCell(dRow["FLINSRoles"].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow["Level1 Approver"].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level1 Approver Type"].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow["Level2 Approver"].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level2 Approver Type"].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow["Level3 Approver"].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow["Level3 Approver Type"].ToString()));
                _config.IsExclusiveForUserAccount = CheckNullAndGetValueFromCell(dRow["Is Exclusive For User Account Type"].ToString()).Equals("True",StringComparison.OrdinalIgnoreCase) ||
                                                       CheckNullAndGetValueFromCell(dRow["Is Exclusive For User Account Type"].ToString()).Equals("1", StringComparison.OrdinalIgnoreCase); 
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
                NOVAExceptionalRoleMatrix obj = this.Data as NOVAExceptionalRoleMatrix;
                var exObj = GetNOVAExceptionalRoleMatrixbyObject();
                if (exObj != null)
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

        public override void Update(string currentUser)
        {
            Add(currentUser);
        }

        public override string GetWhoUpdatedData()
        {
            return NOVAExceptionalRoleMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNOVAExceptionalRoleMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private NOVAExceptionalRoleMatrix GetNOVAExceptionalRoleMatrixbyObject()
        {
            return NOVAExceptionalRoleMatrix.GetNOVAExceptionalRoleMatrixbyObject(this.Data as NOVAExceptionalRoleMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NOVA_ExceptionalRoleMatrix";
            return base.TruncateTable();
            //return NOVAExceptionalRoleMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NOVAExceptionalRoleMatrix _config = data as NOVAExceptionalRoleMatrix;

            if (string.IsNullOrWhiteSpace(_config.CompanyCode + _config.FLINSRoles + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            NOVAExceptionalRoleMatrix obj = this.Data as NOVAExceptionalRoleMatrix;

            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.CompanyCode))
            {
                missingFields.Add("Company Code");
            }
           
            //else if (!IsCompanyCodeValid(obj.CompanyCode))
            //{
            //    missingFields.Add("Company Code");
            //}

            if (string.IsNullOrWhiteSpace(obj.FLINSRoles))
            {                
                missingFields.Add("FLINSRoles");
            }
            else
            {
                var roles = obj.FLINSRoles.Split(new char[] { ',' });
                foreach (var role in roles)
                {
                    //if (obj.IsExclusiveForUserAccount)
                    //{
                    //    if (!IsTCodeValid(FlinsBIEndpoint, role))
                    //        missingFields.Add(" FLINSRoles");
                    //}
                    //else
                    {
                        if (!IsTCodeValid(FlinsEndpoint, role))
                            missingFields.Add(" FLINSRoles");
                    }
                }
            }
            if (!this.IsUserValid(obj.Level1Approver, obj.Level1ApproverType))
            {
                missingFields.Add(" Level1 Approver");
            }

            if (obj.Level2ApproverType!= ApprovalUserType.None && !this.IsUserValid(obj.Level2Approver, obj.Level2ApproverType))
            {
                missingFields.Add(" Level2 Approver");
            }

            if (obj.Level3ApproverType != ApprovalUserType.None &&  !this.IsUserValid(obj.Level3Approver, obj.Level3ApproverType))
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
