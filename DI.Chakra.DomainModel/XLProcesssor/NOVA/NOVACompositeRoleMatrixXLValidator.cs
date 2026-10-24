using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class NOVACompositeRoleMatrixXLValidator: XLDataValidator
    {
        readonly EndPoint endPoint;
        public NOVACompositeRoleMatrixXLValidator()
        {
            endPoint = EndPoint.GetByApplicationType(ApplicationType.EAM);
        }
        
        private EndPoint EndpointSystem 
        {
            get
            {
                
                return endPoint;
            }
           
        }

        public override Type GetDomainModelType()
        {
            return typeof(NOVACompositeRoleMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            NOVACompositeRoleMatrix _config = null;
            try
            {
                _config = new NOVACompositeRoleMatrix();
                _config.Job = CheckNullAndGetValueFromCell(dRow["Job"].ToString());
                _config.CompositeRoleName = CheckNullAndGetValueFromCell(dRow["CompositeRoleName"].ToString());
                _config.ModuleWiseRoles = CheckNullAndGetValueFromCell(dRow["ModuleWiseRoles"].ToString());
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
                NOVACompositeRoleMatrix obj = this.Data as NOVACompositeRoleMatrix;
                var exObj = GetNOVACompositeRoleMatrixbyObject();
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
            return NOVACompositeRoleMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNOVACompositeRoleMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private NOVACompositeRoleMatrix GetNOVACompositeRoleMatrixbyObject()
        {
            return NOVACompositeRoleMatrix.GetNOVACompositeRoleMatrixbyObject(this.Data as NOVACompositeRoleMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NOVA_CompositeRoleMatrix";
            return base.TruncateTable();
            //return NOVACompositeRoleMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NOVACompositeRoleMatrix _config = data as NOVACompositeRoleMatrix;

            if (string.IsNullOrWhiteSpace(_config.Job + _config.CompositeRoleName + _config.ModuleWiseRoles))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            NOVACompositeRoleMatrix obj = this.Data as NOVACompositeRoleMatrix;

            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.Job))
            {
                missingFields.Add("Job");
            }

            if (string.IsNullOrWhiteSpace(obj.CompositeRoleName))
            {
                missingFields.Add("Composite Role");
            }

            if (string.IsNullOrWhiteSpace(obj.ModuleWiseRoles))
            {
                missingFields.Add("Module-wise Role");
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
