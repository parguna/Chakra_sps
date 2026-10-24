using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class XLDataProcessor
    {
        private List<XLTemplates> lstMatrixTemplates;
        public List<XLTemplates> LstMatrixTemplates
        {
            get
            {
                if (lstMatrixTemplates == null)
                {
                    lstMatrixTemplates = new List<XLTemplates>();
                    lstMatrixTemplates.Add(XLTemplates.ADS_PROVISIONINGMATRIX);
                    lstMatrixTemplates.Add(XLTemplates.SAP_RoleBasedOnOrgMgt);
                    lstMatrixTemplates.Add(XLTemplates.SAP_BIRoleCategories);
                    lstMatrixTemplates.Add(XLTemplates.SAP_PIP_SAP_PI_Role);
                    lstMatrixTemplates.Add(XLTemplates.SAP_RoleBasedOnEEGroup);
                    lstMatrixTemplates.Add(XLTemplates.SAP_RoleBasedOnPosition);
                    lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSSuppRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_RoleMandatoryFields);
                    lstMatrixTemplates.Add(XLTemplates.SAP_RoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_HRFLINSConflictedBusinessRoles);
                    lstMatrixTemplates.Add(XLTemplates.SAP_PGR_GRC_Role);
                    lstMatrixTemplates.Add(XLTemplates.SAP_MSSLCriticalTCodesMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_MSSLPositionMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_MSSLRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_EBSCriticalTCodesMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_EBSPositionMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_EBSRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAP_EPRoleInfo);
                    lstMatrixTemplates.Add(XLTemplates.SAP_EPGroupInfo);
                    lstMatrixTemplates.Add(XLTemplates.CE_GroupInfo);
                    lstMatrixTemplates.Add(XLTemplates.CE_RoleInfo);
                    lstMatrixTemplates.Add(XLTemplates.NPACConfigurations);
                    lstMatrixTemplates.Add(XLTemplates.NPAC_Designation);
                    lstMatrixTemplates.Add(XLTemplates.SAPSRMMainRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAPSRMEnablerRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAPSRMBusinessRoleMapping);
                    lstMatrixTemplates.Add(XLTemplates.GEMSRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.Office365License);
                    lstMatrixTemplates.Add(XLTemplates.SMSProvisioningMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SMSRoleCatalogue);
                    lstMatrixTemplates.Add(XLTemplates.Inhouse_Permissions);
                    lstMatrixTemplates.Add(XLTemplates.Inhouse_Functions);
                    lstMatrixTemplates.Add(XLTemplates.NOVAUserRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.NOVAManagementRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.NOVAExceptionalRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.NOVASoDExceptionsMatrix);
                    lstMatrixTemplates.Add(XLTemplates.Inhouse_UserPermissionFunctionMatrix);
                    lstMatrixTemplates.Add(XLTemplates.ApprovalException);
                    lstMatrixTemplates.Add(XLTemplates.VPNConfigurations);
                    lstMatrixTemplates.Add(XLTemplates.NOVACompositeRoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SkypeForBusinessConfigurations);
                    lstMatrixTemplates.Add(XLTemplates.Inhouse_UserInfo);
                    lstMatrixTemplates.Add(XLTemplates.SAP_SODConflictedBusinessRoles);
                    lstMatrixTemplates.Add(XLTemplates.SAPHANA_RoleMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAPHANA_PositionMatrix);
                    lstMatrixTemplates.Add(XLTemplates.SAPHANA_CriticalTCodesMatrix);
                }
                return lstMatrixTemplates;
            }
            set { lstMatrixTemplates = value; }
        }
        private List<XLTemplates> lstTemplatesCanUpdate;
        public List<XLTemplates> LstTemplatesCanUpdate
        {
            get
            {
                if (lstTemplatesCanUpdate == null)
                {
                    lstTemplatesCanUpdate = new List<XLTemplates>();
                    lstTemplatesCanUpdate.Add(XLTemplates.SAP_EPRoleInfo);
                    lstTemplatesCanUpdate.Add(XLTemplates.SAP_EPGroupInfo);
                    lstTemplatesCanUpdate.Add(XLTemplates.CE_GroupInfo);
                    lstTemplatesCanUpdate.Add(XLTemplates.CE_RoleInfo);
                    lstTemplatesCanUpdate.Add(XLTemplates.VPNConfigurations);
                    lstTemplatesCanUpdate.Add(XLTemplates.SkypeForBusinessConfigurations);
                    lstTemplatesCanUpdate.Add(XLTemplates.Inhouse_Permissions);
                    lstTemplatesCanUpdate.Add(XLTemplates.ADS_PROVISIONINGMATRIX);
                    lstTemplatesCanUpdate.Add(XLTemplates.SAP_SODConflictedBusinessRoles);
                    lstTemplatesCanUpdate.Add(XLTemplates.SAP_MSSLCriticalTCodesMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.SAP_EBSCriticalTCodesMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.NOVAUserRoleMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.NOVAManagementRoleMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.NOVAExceptionalRoleMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.NOVASoDExceptionsMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.NOVACompositeRoleMatrix);
                    lstTemplatesCanUpdate.Add(XLTemplates.SMSRoleCatalogue);
                }
                return lstTemplatesCanUpdate;
            }
            set { lstTemplatesCanUpdate = value; }
        }
        private readonly IList<IXLData> _invalidList = new List<IXLData>();
        private readonly IList<IXLData> _validList = new List<IXLData>();

        public bool canClearListobj { get; set; }
        public XLTemplates TemplateName { get; set; }

        public string FileName { get; set; }
        public string xmlData { get; set; }
        public DateTime UploadDateTime { get; set; }
        public bool PartialUpload { get; set; }

        public bool invalidColumnStatus { get; set; }

        public bool HasInvalidData
        {
            get { return InvalidList.Count > 0; }
        }

        public bool HasInvalidColumn
        {
            get { return invalidColumnStatus; }
        }

        public IList<IXLData> InvalidList
        {
            get { return _invalidList; }
        }

        public IList<IXLData> ValidList
        {
            get { return _validList; }
        }


        public bool IsValidFile(ref string _validationMsg)
        {
            invalidColumnStatus = false;
            IList<string> ColumnList = new ExcelConnect().GetHeaderColumnListFromExcel(FileName);
            if (ColumnList.Contains("ValidationMessage"))
            {
                ColumnList.Remove("ValidationMessage");
            }

            return true;
        }

        public static bool CheckTemplate(IList<string> lst1, IList<string> lst2)
        {
            bool status = true;
            if (lst1.Count == lst2.Count)
            {
                for (int i = 0; i <= lst1.Count - 1; i++)
                {
                    if (string.Compare(lst1[i].Trim(), lst2[i].Trim(), true) != 0)
                    {
                        status = false;
                        break;
                    }
                }
            }
            return status;
        }

        public void Truncate()
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            validator.TruncateTable();
        }

        public virtual bool Validate(IXLData data, XLDataValidator validator)
        {
            var validationMsg = string.Empty;
            validator.canClearListobj = this.canClearListobj;
            if (!validator.Validate(ref validationMsg))
            {
                data.IsValid = false;
                data.ValidationMessage += validationMsg;
                _invalidList.Add(data);
                return false;
            }
            else if (!string.IsNullOrEmpty(data.ValidationMessage))
            {
                data.IsValid = false;
                data.ValidationMessage += validationMsg;
                _invalidList.Add(data);
                return true;
            }
            _validList.Add(data);
            return true;
        }

        public IXLData GetObjectFromData(DataRow row, XLDataValidator validator)
        {
            return validator.GetObjectFromData(row);
        }

        public IXLData GetObjectFromData(DataRow row, long endpointSystemId, XLDataValidator validator)
        {
            return validator.GetObjectFromData(row, endpointSystemId);
        }

        public bool IsEmptyRow(IXLData data, XLDataValidator validator)
        {
            //var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            return validator.IsEmptyRow(data);
        }

        public virtual void Process(IXLData data, XLTemplates templateName, string currentUser, bool isEditMode = false, bool fromFile = false)
        {

            var validator = XLDataValidator.GetXLDataValidator(templateName);
            Process(data, templateName, currentUser, validator, isEditMode, fromFile);
        }

        public virtual void Process(IXLData data, XLTemplates templateName, string currentUser, XLDataValidator validator, bool isEditMode = false, bool fromFile = false)
        {
            validator.Data = data;
            validator.canClearListobj = this.canClearListobj;
            var isValid = Validate(data, validator);

            if (LstMatrixTemplates.Contains(templateName))
            {
                if (isValid)
                {
                    if (!isEditMode)
                    {
                        if (!validator.IsExists(currentUser))
                        {
                            validator.Add(currentUser);
                        }
                        else if (LstTemplatesCanUpdate.Contains(templateName))
                        {
                            validator.Update(currentUser);
                        }
                        else if (!fromFile)
                        {
                            data.IsValid = false;
                            data.ValidationMessage = "Record already exists.";
                            _invalidList.Add(data);
                        }
                    }
                    else
                    {
                        validator.Update(currentUser);
                    }
                }
            }
            else if (isValid && templateName.ToString() != "PROVISIONINGDEFAULTS" && templateName.ToString() != "CATALOGMANAGEMENT" && templateName.ToString() != "ENTITLEMENTS")
            {
                if (!validator.IsExists(currentUser))
                    validator.Add(currentUser);
            }
        }

        public virtual void PostProcess(string currentUser)
        {
            var validator = XLDataValidator.GetXLDataValidator(TemplateName);
            validator.ValidList = ValidList;
            validator.InvalidList = InvalidList;
            validator.UploadDateTime = UploadDateTime;
            validator.PartialUpload = PartialUpload;
            validator.PostProcess(currentUser);
        }
    }
}
