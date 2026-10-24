using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class VPNMatrixDataValidator: XLDataValidator
    {
        private long endpointSystemId;
        private long EndpointSystemId
        {
            get
            {
                EndPoint endPoint = null;
                if (endpointSystemId == 0)
                {
                    endPoint = EndPoint.GetByApplicationType(ApplicationType.VPN);
                }
                return endPoint == null ? 0 : endpointSystemId;
            }
            set { endpointSystemId = value; }
        }

        public override Type GetDomainModelType()
        {
            return typeof(VPNMatrix);
        }
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            VPNMatrix _config = null;
            try
            {
                _config = new VPNMatrix();
                var strID = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString());
                long id;
                if(!string.IsNullOrWhiteSpace(strID) && Int64.TryParse(strID,out id))
                    _config.Id = id;
                var type = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString());
                if (!string.IsNullOrWhiteSpace(type))
                {
                    VPNType vpnType;
                    if (Enum.TryParse(type, true, out vpnType))
                    {
                        if(vpnType == VPNType.SSLVPNOnly)
                        {
                            _config.VPNMatrixType = VPNMatrixType.Application;
                        }
                        else if (vpnType == VPNType.VDI)
                        {
                            _config.VPNMatrixType = VPNMatrixType.Project;
                        }
                    }
                    VPNMatrixType matrixType;
                    if (Enum.TryParse(type, true,out matrixType))
                    {
                        _config.VPNMatrixType = matrixType;
                    }
                }
                else
                {
                    throw new Exception("Invalid VPN Matrix Type");
                }
                
                _config.Name = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString());
                _config.ProductionADgroups = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString());
                _config.QAADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString());
                _config.DevelpomentADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString());
                _config.ADGroups = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString());
                _config.Level1Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString());
                _config.Level1ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString()));
                _config.Level2Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString());
                _config.Level2ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[10].ToString()));
                _config.Level3Approver = CheckNullAndGetValueFromCell(dRow.ItemArray[11].ToString());
                _config.Level3ApproverType = GetApprovalUserTypesByDescription(CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString()));
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
                if(_config.VPNMatrixType == VPNMatrixType.Project)
                {
                    _config.ProductionADgroups = null;
                    _config.DevelpomentADGroups = null;
                    _config.QAADGroups = null;
                }
                else if (_config.VPNMatrixType == VPNMatrixType.Application)
                {
                    _config.ADGroups = null;
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
                VPNMatrix obj = this.Data as VPNMatrix;
                obj.VPNMatrixType = obj.VPNMatrixType;
                obj.EndPointSystemId = EndpointSystemId;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
                var tempResult = GetApplicationEnvironment(obj);
                foreach (var item in tempResult)
                {
                    if(item.MatrixId > 0)
                    {
                        var vpnEnv = VPNApplicationEnvironment.GetByMatrixId(item.MatrixId);
                        if(vpnEnv != null && vpnEnv.Count > 0)
                        {
                            var matched = vpnEnv.FirstOrDefault(i=> i.MatrixType == item.MatrixType && i.EnviornmentType == item.EnviornmentType);
                            if(matched != null)
                            {
                                item.Id = matched.Id;
                            }
                        }
                    }
                    
                    item.Save(currentUser);
                }
            }
            catch(Exception ex) { throw ex; }
        }
        private static List<VPNApplicationEnvironment> GetApplicationEnvironment(VPNMatrix vpnMatrixData)
        {
            List<VPNApplicationEnvironment> vpnApplnEnvs = new List<VPNApplicationEnvironment>();
            if (vpnMatrixData.VPNMatrixType == VPNMatrixType.Application)
            {
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.ProductionADgroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = VPNMatrixType.Application,
                        EnviornmentType = VPNMatrixEnviornmentType.Production,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.Production.ToString()),
                        ADGroups = vpnMatrixData.ProductionADgroups,
                    });
                }
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.QAADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = VPNMatrixType.Application,
                        EnviornmentType = VPNMatrixEnviornmentType.QA,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.QA.ToString()),
                        ADGroups = vpnMatrixData.QAADGroups,
                    });
                }
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.DevelpomentADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = vpnMatrixData.VPNMatrixType,
                        EnviornmentType = VPNMatrixEnviornmentType.Development,
                        Name = string.Format("{0}-{1}", vpnMatrixData.Name, VPNMatrixEnviornmentType.Development.ToString()),
                        ADGroups = vpnMatrixData.DevelpomentADGroups,
                    });
                }
            }
            else if (vpnMatrixData.VPNMatrixType == VPNMatrixType.Project)
            {
                if (!string.IsNullOrWhiteSpace(vpnMatrixData.ADGroups))
                {
                    vpnApplnEnvs.Add(new VPNApplicationEnvironment()
                    {
                        MatrixId = vpnMatrixData.Id,
                        MatrixType = vpnMatrixData.VPNMatrixType,
                        EnviornmentType = VPNMatrixEnviornmentType.ADGroup,
                        Name = vpnMatrixData.Name,
                        ADGroups = vpnMatrixData.ADGroups,
                    });
                }
            }
            return vpnApplnEnvs;
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
            this.ExistingData = GetVPNMatrixbyObject();
            if (this.ExistingData == null)
                return false;
            else
            {
                this.Data.Id = this.ExistingData.Id;
                return true;
            }
        }

        private VPNMatrix GetVPNMatrixbyObject()
        {
            var data = this.Data as VPNMatrix;
            if (data == null)
                return data;
            return VPNMatrix.GetByName(data.Name, data.VPNMatrixType);
        }

        public override bool TruncateTable()
        {
            return VPNMatrix.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            VPNMatrix _config = data as VPNMatrix;

            if (string.IsNullOrWhiteSpace(_config.Name + _config.ProductionADgroups + _config.QAADGroups + _config.DevelpomentADGroups + _config.ADGroups + _config.Level1Approver + _config.Level1ApproverType
                   + _config.Level2Approver + _config.Level2ApproverType + _config.Level3Approver + _config.Level3ApproverType))
            {
                return true;
            }

            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            VPNMatrix obj = this.Data as VPNMatrix;
            var errorMsg = string.Empty;
            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(obj.Name))
            {
                missingFields.Add("Name");
            }
            bool isExist = false;
            if(obj.Id > 0)
            {
                var existingData = VPNMatrix.GetById(obj.Id);
                if(existingData != null && existingData.Name.Trim().ToUpper() != obj.Name.Trim().ToUpper())
                {
                    var vpnUsrApplication = VPNUserApplicationEnviornment.GetByMatrixId(existingData.Id);
                    if (vpnUsrApplication.Count > 0)
                    {
                        errorMsg = $"You cannot update the Name. Because existing name:{existingData.Name} was mapped to user entitlement";
                    }
                }
                
            }
            if(obj.VPNMatrixType == VPNMatrixType.Application)
            {
                isExist = !(string.IsNullOrEmpty(obj.DevelpomentADGroups) && string.IsNullOrEmpty(obj.ProductionADgroups) && string.IsNullOrEmpty(obj.QAADGroups));
                if (isExist == false)
                    missingFields.Add("Production, Development, QA Group(s)");

                if (obj.ProductionADgroups != "Default" && !string.IsNullOrEmpty(obj.ProductionADgroups))
                {
                    isExist = this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.ProductionADgroups);
                    if (isExist == false)
                        missingFields.Add("Production Group(s)");
                }

                if (obj.DevelpomentADGroups != "Default" && !string.IsNullOrEmpty(obj.DevelpomentADGroups))
                {
                    isExist = this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.DevelpomentADGroups);
                    if (isExist == false)
                        missingFields.Add("Development Group(s)");
                }

                if (obj.QAADGroups != "Default" && !string.IsNullOrEmpty(obj.QAADGroups))
                {
                    isExist = this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.QAADGroups);
                    if (isExist == false)
                        missingFields.Add("QA Group(s)");
                }
            }
            else
            {
                if (obj.VPNMatrixType == VPNMatrixType.Project)
                {
                    isExist = !(string.IsNullOrEmpty(obj.ADGroups));
                    if (isExist == false)
                        missingFields.Add("ADGroups");

                    if (obj.QAADGroups != "Default" && !string.IsNullOrEmpty(obj.ADGroups))
                    {
                        isExist = this.IsADGroupValid(ApplicationType.ADSingaporePowerLocal, obj.ADGroups);
                        if (isExist == false)
                            missingFields.Add("AD Group(s)");
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
            if (!string.IsNullOrWhiteSpace(errorMsg))
            {
                validationMsg = errorMsg;
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

            if (!string.IsNullOrWhiteSpace(validationMsg))
            {
                missingFields.Add(validationMsg);
            }
            return missingFields.Count == 0;
        }
    }
}
