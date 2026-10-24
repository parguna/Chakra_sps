using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
   
    public class SkypeForBusinessProvisioningMatrixDataValidator : XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get
            {
                EndPoint endPoint = null;
                if (endpointSystemId == 0)
                {
                    endPoint = EndPoint.GetByApplicationType(ApplicationType.MicrosoftTeams);
                }
                return endPoint == null ? 0 : endPoint.Id;
            }
            set { endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(SkypeForBusinessProvisioningMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            SkypeForBusinessProvisioningMatrix _config = null;
            try
            {
                _config = new SkypeForBusinessProvisioningMatrix();
               
                if (string.IsNullOrWhiteSpace(CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString())))
                {
                    _config.RequestType =EntitlementType.ResponseGroup;
                }else
                {
                    _config.RequestType = GetRequestTypesByDescription(dRow.ItemArray[0].ToString());
                }
               
                _config.Name = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                _config.Description = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.ADID = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString()));
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
                var obj = this.Data as SkypeForBusinessProvisioningMatrix;
                obj.RequestType = obj.RequestType;
                obj.Name = obj.Name;
                obj.Description = obj.Description;
                obj.ADID = obj.ADID;
                obj.EndPointSystemId = EndpointSystemId;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.IsActive = true;
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
            return VPNMatrix.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetSkypeForBusinessProvisioningMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private SkypeForBusinessProvisioningMatrix GetSkypeForBusinessProvisioningMatrixbyObject()
        {
            var data = this.Data as SkypeForBusinessProvisioningMatrix;
            if (data == null)
                return data;
            return SkypeForBusinessProvisioningMatrix.GetByName(data.Name, data.RequestType);
        }

        public override bool TruncateTable()
        {
            return SkypeForBusinessProvisioningMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            var _config = data as SkypeForBusinessProvisioningMatrix;

            if (string.IsNullOrWhiteSpace(_config.Name + _config.ADID + _config.RequestType.ToString() + _config.Description + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            var obj = this.Data as SkypeForBusinessProvisioningMatrix;

            List<string> missingFields = new List<string>();
            if((obj.RequestType == EntitlementType.ResponseGroup || obj.RequestType == EntitlementType.Default || obj.RequestType == EntitlementType.DCC 
                || obj.RequestType == EntitlementType.DCCS || obj.RequestType == EntitlementType.IDD || obj.RequestType == EntitlementType.Move
                ))
            {
               //Commented due to UAT finding observation: 
               //if(obj.RequestType == EntitlementType.ResponseGroup && string.IsNullOrWhiteSpace(obj.ADID))
               // {
               //     missingFields.Add("ADID should be mandatory for ResponseGroup");
               // }
            }else
            {
                missingFields.Add("RequestType is mandatory and should be valid type(ResponseGroup,Telephone,DCC,DCCS,IDD,Move");
            }
            if (string.IsNullOrWhiteSpace(obj.Name))
            {
                missingFields.Add("Name");
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
