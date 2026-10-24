using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DeepIdentity.DataAccess.XLProcesssor;
using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;

namespace DeepIdentity.DataAccess
{
    public abstract class XLDataValidator
    {
        public const string NULL = "NULL";

        public IXLData Data { get; set; }

        public virtual IXLData ExistingData { get; set; }

        public abstract bool Validate(ref string validationMsg);

        public abstract bool IsExists();

        public abstract bool TruncateTable();

        public abstract void Add();

        public virtual void Update()
        {
        }

        public virtual void PostProcess()
        {
        }

        public virtual IXLData GetObjectFromData(DataRow dRow)
        {
            return null;
        }

        public virtual bool IsEmptyRow(IXLData data)
        {
            return false;
        }

        public virtual bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            return false;
        }

        public static XLDataValidator GetXLDataValidator(XLTemplates templateName)
        {
            switch (templateName)
            {
                case XLTemplates.ADS_PROVISIONINGMATRIX:
                    return new ADSProvMatrixDataValidator();

                case XLTemplates.LNS_PROVISIONINGMATRIX:
                    return new LNSProvMatrixDataValidator();

                case XLTemplates.SAP_PROVISIONINGMATRIX:
                    return new SAPProvMatrixDataValidator();

                case XLTemplates.SAP_Role_Catalog:
                    return new SAPRoleCatalogDataValidator();

                case XLTemplates.SAP_CriticalTCodesMatrix:
                    return new SAPCriticalTCodeMatrixDataValidator();

                case XLTemplates.SAP_PositionMatrix:
                    return new SAPPositionMatrixDataValidator();

                case XLTemplates.Inhouse_UserInfo:
                    return new InhouseUserXLValidator();

                case XLTemplates.Inhouse_Permissions:
                    return new InhousePermissionXLValidator();

                case XLTemplates.LNSMailConfig:
                    return new LNSMailQuotaXLValidator();

                case XLTemplates.VPNConfigurations:
                    return new VPNProvMatrixDataValidator();

                default:
                    break;
            }

            return null;
        }

        public static IList<string> GetFields(XLTemplates templateName)
        {
            var masterDA = new MasterDA();
            return
                masterDA.GetMasterConfigByKey(UtilityMethods.GetEnumAttribute<XLTemplateName>(templateName))
                    .Split(',')
                    .ToList();
        }

        protected string CheckNullAndGetValueFromCell(string cellText)
        {
            cellText = cellText.Trim();
            if (string.Compare(cellText, NULL, true) != 0)
                return cellText;
            return string.Empty;
        }

        public bool IsADGroupValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (var _context = new IACMDBContext())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = _context.ADSGroupInfos.Any(item => item.name == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPEPGroupValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (var _context = new IACMDBContext())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = _context.SAPEPGroupInfos.Any(item => item.GroupName == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPRoleValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (var _context = new IACMDBContext())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = _context.SAPRoleInfos.Any(item => item.ROLE_CODE == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsSAPTCodeValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (var _context = new IACMDBContext())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = _context.SAPTCodeInfos.Any(item => item.TCODE_Code == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        public bool IsLNSGroupValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (var _context = new IACMDBContext())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = _context.LotusGroupInfos.Any(item => item.Group == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        //TODO: Include endpointsystem id in where clause
        public bool IsADOUValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.ADSOuInfos.Any(item => item.distinguishedName == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsCompanyCodeValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.Companies.Any(item => item.CompanyCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsPersonnelAreaValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.Departments.Any(item => item.DepartmentCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsPersonnelSubAreaValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.Divisions.Any(item => item.DivisionCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsCostCenterValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.CostCenters.Any(item => item.CostCenterCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsOrgUnitValid(string value)
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.Organisations.Any(item => item.OrgUnitPrefix == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsUserValid(string Value, ApprovalUserType Type)
        {
            try
            {
                if (!string.IsNullOrEmpty(Value))
                {
                    using (var _context = new IACMDBContext())
                    {
                        if (Type == ApprovalUserType.User)
                        {
                            return _context.Users.Any(cmp => cmp.EmployeeId == Value);
                        }
                        return _context.Groups.Any(cmp => cmp.GroupName == Value);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ApprovalUserType GetApprovalUserTypesByDescription(string description)
        {
            try
            {
                return (ApprovalUserType) Enum.Parse(typeof (ApprovalUserType), description, true);
            }
            catch
            {
                return ApprovalUserType.None;
            }
        }
    }
}