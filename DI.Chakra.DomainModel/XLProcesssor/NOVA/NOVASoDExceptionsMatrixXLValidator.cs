using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class NOVASoDExceptionsMatrixXLValidator : XLDataValidator
    {

        readonly EndPoint endPoint;
        public NOVASoDExceptionsMatrixXLValidator()
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
            return typeof(NOVASoDExceptionsMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            NOVASoDExceptionsMatrix _config = null;
            try
            {
                _config = new NOVASoDExceptionsMatrix();
                _config.Exceptions = CheckNullAndGetValueFromCell(dRow["Exceptions"].ToString());
                _config.Exceptions = string.Join(",", _config.Exceptions.Split(',').Select(t =>  t.Trim()).ToList() );

                _config.AffectedUsers = CheckNullAndGetValueFromCell(dRow["AffectedUsers"].ToString());
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
                NOVASoDExceptionsMatrix obj = this.Data as NOVASoDExceptionsMatrix;
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
            return NOVASoDExceptionsMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetNOVASoDExceptionsMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private NOVASoDExceptionsMatrix GetNOVASoDExceptionsMatrixbyObject()
        {
            return NOVASoDExceptionsMatrix.GetNOVASoDExceptionsMatrixbyObject(this.Data as NOVASoDExceptionsMatrix);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_NOVA_SoDExceptionsMatrix";
            return base.TruncateTable();
            //return NOVASoDExceptionsMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            NOVASoDExceptionsMatrix _config = data as NOVASoDExceptionsMatrix;

            if (string.IsNullOrWhiteSpace(_config.Exceptions + _config.AffectedUsers))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            NOVASoDExceptionsMatrix obj = this.Data as NOVASoDExceptionsMatrix;

            List<string> missingFields = new List<string>();

            //if (string.IsNullOrWhiteSpace(obj.Exceptions))
            //{
            //    missingFields.Add("Exceptions Not Found");
            //}
            //else
            //{
            //    var postions = obj.Exceptions.Split(',');

            //    foreach (var p in postions)
            //    {
            //        var user = NOVAUserRoleMatrix.Get(t => t.Position.Equals(p.Trim(), StringComparison.InvariantCultureIgnoreCase),false,0,0).FirstOrDefault();

            //        if (user == null)
            //        {
            //            missingFields.Add($"Could not found the position : {p} for the section : {obj.AffectedUsers}");
            //            continue;
            //        }

            //    }

            //}

            if (string.IsNullOrWhiteSpace(obj.AffectedUsers))
            {
                missingFields.Add("Affected Users not found");
            }
            else
            {
                var user = NOVAUserRoleMatrix.Get(t => t.SectionCode.Equals(obj.AffectedUsers.Trim(), StringComparison.InvariantCultureIgnoreCase), false, 0, 0).FirstOrDefault();

                if (user == null)
                {
                    missingFields.Add($"Could not found the section : {obj.AffectedUsers} for the positions : {obj.Exceptions}"); 
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
            //if (cnt > 0)
            //    validationMsg = validationMsg + " Not Found ";

            return missingFields.Count == 0;
        }
    }
}
