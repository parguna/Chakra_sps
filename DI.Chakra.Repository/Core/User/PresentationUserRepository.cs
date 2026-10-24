using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;

namespace DI.Chakra.Repository
{
    public class PresentationUserRepository : RepositoryBase<PresentationUser>
    {
        public PresentationUserRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<PresentationUser> GetEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()
                        //where usr.IsActive == true && usr.EmployeeType == EmployeeType.Permanent
                    select new PresentationUser
                    {
                        Id = usr.Id,
                        BusinessRoleName = brole.Description,
                        Country = usr.Country,
                        DisplayName = usr.DisplayName,
                        EmailId = usr.EmailId,
                        EmployeeId = usr.EmployeeId,
                        Ethinicity = usr.Ethinicity.ToString(),
                        JoinDate = usr.JoinDate,
                        MobileNumber = usr.MobileNumber,
                        OrganisationName = org.Name,
                        TelephoneNumber = usr.TelephoneNumber,
                        UserName = usr.UserName,

                        Reserved1 = usr.Reserved1,
                        Reserved2 = usr.Reserved2,
                        Reserved3 = usr.Reserved3,
                        Reserved4 = usr.Reserved4,
                        Reserved5 = usr.Reserved5,
                        EmployeeType = usr.EmployeeType,
                        IsActive = usr.IsActive,
                        TerminationDate = usr.TerminationDate.HasValue ? usr.TerminationDate.Value : (DateTime?)null,
                    }).ToList();
        }

        public PresentationGridPagedData<PresentationUser> GetLazyLoadFullTimeEmployees(bool fullTimeEmp, int startRowIndex, int maximumRows, string FilterExpression, string sortExpression, string departmentCode = "", bool showAllNonEmployees = false)
        {
            PresentationGridPagedData<PresentationUser> result = new PresentationGridPagedData<PresentationUser>();

            try
            {
                IQueryable<PresentationUser> userList = null;
                var excludeUser = "IACMAdmin";
                CurrentContext.Database.CommandTimeout = 300;
                if (fullTimeEmp)
                {
                    userList = (from usr in CurrentContext.User
                                join ro in CurrentContext.User
                                on usr.ROEmployeeId equals ro.EmployeeId into detail
                                from ro in detail.DefaultIfEmpty()
                                join co in CurrentContext.User on usr.COEmployeeId equals co.EmployeeId into coDetail
                                from co in coDetail.DefaultIfEmpty()
                                where usr.FullTimeEmployee == fullTimeEmp && usr.UserName != excludeUser
                                select new PresentationUser
                                {
                                    Id = usr.Id,
                                    Country = usr.Country,
                                    DisplayName = usr.DisplayName,
                                    EmailId = usr.EmailId,
                                    EmployeeId = usr.EmployeeId,
                                    Ethinicity = usr.Ethinicity.ToString(),
                                    JoinDate = usr.JoinDate,
                                    MobileNumber = usr.MobileNumber,
                                    TelephoneNumber = usr.TelephoneNumber,
                                    UserName = usr.UserName,
                                    EmployeeType = usr.EmployeeType,
                                    IsActive = usr.IsActive,
                                    TerminationDate = usr.TerminationDate,
                                    CostCenter = usr.CostCenter,
                                    Company = usr.OrgCompany,
                                    NCompany = usr.NCompany,
                                    Division = usr.OrgDivision,
                                    Branch = usr.OrgBranch,
                                    Department = usr.OrgDepartment,
                                    OrganisationUnit = usr.OrgUnit,
                                    ReportingOfficer = ro.DisplayName,
                                    CreatedDate = usr.CreatedDate,
                                    ProjectDescription = usr.ProjectDescription,
                                    CreatedBy = usr.CreatedBy,
                                    ReferenceUser = "",
                                    Status = usr.RequestStatus.ToString(),
                                    SectionName = usr.OrgSection,
                                    CompanyCode = usr.CompanyCode,
                                    DepartmentCode = usr.DepartmentCode,
                                    BranchCode = usr.BranchCode,
                                    SectionCode = usr.SectionCode,
                                    UnitCode = usr.UnitCode,
                                    OrganisationCode = usr.OrganisationCode,
                                    BusinessRoleName = usr.PositionName,
                                    CounterSigningOfficer = co != null ? co.DisplayName : string.Empty,
                                    EmployeeTypeString = usr.EmployeeType.ToString()
                                });
                }
                else
                {
                    userList = (from usr in CurrentContext.User
                                join ro in CurrentContext.User
                                on usr.ROEmployeeId equals ro.EmployeeId into detail
                                from ro in detail.DefaultIfEmpty()
                                join refUsr in CurrentContext.User
                                on usr.Reserved5 equals refUsr.EmployeeId into refUsrDetail
                                join co in CurrentContext.User on usr.COEmployeeId equals co.EmployeeId into coDetail
                                from co in coDetail.DefaultIfEmpty()
                                from refUsr in refUsrDetail.DefaultIfEmpty()
                                where usr.FullTimeEmployee == fullTimeEmp && usr.UserName != excludeUser && (showAllNonEmployees || usr.DepartmentCode == departmentCode)
                                select new PresentationUser
                                {
                                    Id = usr.Id,
                                    Country = usr.Country,
                                    DisplayName = usr.DisplayName,
                                    EmailId = usr.EmailId,
                                    EmployeeId = usr.EmployeeId,
                                    Ethinicity = usr.Ethinicity.ToString(),
                                    JoinDate = usr.JoinDate,
                                    MobileNumber = usr.MobileNumber,
                                    TelephoneNumber = usr.TelephoneNumber,
                                    UserName = usr.UserName,
                                    EmployeeType = usr.EmployeeType,
                                    IsActive = usr.IsActive,
                                    TerminationDate = usr.TerminationDate,
                                    CostCenter = usr.CostCenter,
                                    Company = usr.OrgCompany,
                                    NCompany = usr.NCompany,
                                    Division = usr.OrgDivision,
                                    Branch = usr.OrgBranch,
                                    Department = usr.OrgDepartment,
                                    OrganisationUnit = usr.OrgUnit,
                                    ReportingOfficer = ro.DisplayName,
                                    CreatedDate = usr.CreatedDate,
                                    ProjectDescription = usr.ProjectDescription,
                                    CreatedBy = usr.CreatedBy,
                                    ReferenceUser = refUsr.DisplayName,
                                    Status = usr.RequestStatus.ToString(),
                                    SectionName = usr.OrgSection,
                                    CompanyCode = usr.CompanyCode,
                                    DepartmentCode = usr.DepartmentCode,
                                    BranchCode = usr.BranchCode,
                                    SectionCode = usr.SectionCode,
                                    UnitCode = usr.UnitCode,
                                    OrganisationCode = usr.OrganisationCode,
                                    CounterSigningOfficer = co != null ? co.DisplayName : string.Empty,
                                    EmployeeTypeString = usr.EmployeeType == EmployeeType.TempStaff ? "Temp or Contact Staff" : (usr.EmployeeType == EmployeeType.Contractor ? "Third Party Contractor" : usr.EmployeeType.ToString()),
                                });
                }

                if (!string.IsNullOrWhiteSpace(FilterExpression))
                {
                    userList = userList.Where(FilterExpression);
                }
                result.TotalCount = userList.Count();
                if (!string.IsNullOrWhiteSpace(sortExpression))
                {
                    userList = userList.OrderBy(sortExpression);
                }
                else
                {
                    userList = userList.OrderByDescending(a => a.Id);
                }

                if (maximumRows == -1)
                    result.Data = userList.ToList();
                else
                    result.Data = userList.Skip(startRowIndex).Take(maximumRows).ToList();
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public IList<PresentationUser> GetAllEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()
                    select new PresentationUser
                    {
                        BusinessRoleName = brole.Description,
                        Country = usr.Country,
                        DisplayName = usr.DisplayName,
                        EmailId = usr.EmailId,
                        EmployeeId = usr.EmployeeId,
                        Ethinicity = usr.Ethinicity.ToString(),
                        JoinDate = usr.JoinDate,
                        MobileNumber = usr.MobileNumber,
                        OrganisationName = org.Name,
                        TelephoneNumber = usr.TelephoneNumber,
                        UserName = usr.UserName
                    }).ToList();
        }
        public IList<PresentationUser> GetAllActiveEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()
                    where usr.IsActive == true
                    select new PresentationUser
                    {
                        EmployeeId = usr.EmployeeId,
                        DisplayName = usr.DisplayName,
                        UserName = usr.UserName,
                        OrganisationName = org.Name,
                        BusinessRoleName = brole.Description,
                        EmailId = usr.EmailId,
                        MobileNumber = usr.MobileNumber,
                        EmployeeType = usr.EmployeeType,
                        JoinDate = usr.JoinDate,
                        TerminationDate = usr.TerminationDate,
                        isActiveUser = usr.IsActive
                    }).ToList();
        }

        public IList<PresentationUser> GetActiveEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()
                    where usr.IsActive == true && usr.EmployeeType == EmployeeType.Permanent
                    select new PresentationUser
                    {
                        EmployeeId = usr.EmployeeId,
                        DisplayName = usr.DisplayName,
                        UserName = usr.UserName,
                        OrganisationName = org.Name,
                        BusinessRoleName = brole.Description,
                        EmailId = usr.EmailId,
                        MobileNumber = usr.MobileNumber,
                        EmployeeType = usr.EmployeeType,
                        JoinDate = usr.JoinDate,
                        TerminationDate = usr.TerminationDate,
                        isActiveUser = usr.IsActive
                    }).ToList();
        }
        public IList<PresentationUser> GetActiveNonEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()
                    where usr.IsActive == true && usr.EmployeeType != EmployeeType.Permanent
                    select new PresentationUser
                    {
                        EmployeeId = usr.EmployeeId,
                        DisplayName = usr.DisplayName,
                        UserName = usr.UserName,
                        OrganisationName = org.Name,
                        BusinessRoleName = brole.Description,
                        EmailId = usr.EmailId,
                        MobileNumber = usr.MobileNumber,
                        EmployeeType = usr.EmployeeType,
                        JoinDate = usr.JoinDate,
                        TerminationDate = usr.TerminationDate,
                        isActiveUser = usr.IsActive
                    }).ToList();
        }

        public IList<PresentationUser> GetEmployees(DateTime? FromDate, DateTime? ToDate, bool? IsActive)
        {
            if (FromDate.HasValue && ToDate.HasValue)
            {
                return (from usr in CurrentContext.User
                        join org in CurrentContext.NOrganisations
                        on usr.NOrganisationId equals org.Id into Orgdetail
                        join brole in CurrentContext.NDesignations
                        on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                        from org in Orgdetail.DefaultIfEmpty()
                        from brole in BusinessRoledetail.DefaultIfEmpty()
                            //where usr.IsActive == true && usr.EmployeeType == EmployeeType.Permanent
                        select new PresentationUser
                        {
                            Id = usr.Id,
                            BusinessRoleName = brole.Description,
                            Country = usr.Country,
                            DisplayName = usr.DisplayName,
                            EmailId = usr.EmailId,
                            EmployeeId = usr.EmployeeId,
                            Ethinicity = usr.Ethinicity.ToString(),
                            JoinDate = usr.JoinDate,
                            MobileNumber = usr.MobileNumber,
                            OrganisationName = org.Name,
                            TelephoneNumber = usr.TelephoneNumber,
                            UserName = usr.UserName,

                            Reserved1 = usr.Reserved1,
                            Reserved2 = usr.Reserved2,
                            Reserved3 = usr.Reserved3,
                            Reserved4 = usr.Reserved4,
                            Reserved5 = usr.Reserved5,
                            EmployeeType = usr.EmployeeType,
                            IsActive = usr.IsActive,
                            TerminationDate = usr.TerminationDate.HasValue ? usr.TerminationDate.Value : (DateTime?)null,
                            CompanyCode = usr.CompanyCode,
                            DepartmentCode = usr.DepartmentCode,
                            BranchCode = usr.BranchCode,
                            SectionCode = usr.SectionCode,
                            UnitCode = usr.UnitCode,
                            OrganisationCode = usr.OrganisationCode

                        }).ToList();
            }
            else if (IsActive.HasValue)
            {
                return (from usr in CurrentContext.User
                        join org in CurrentContext.NOrganisations
                        on usr.NOrganisationId equals org.Id into Orgdetail
                        join brole in CurrentContext.NDesignations
                        on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                        from org in Orgdetail.DefaultIfEmpty()
                        from brole in BusinessRoledetail.DefaultIfEmpty()
                        where usr.IsActive == IsActive
                        select new PresentationUser
                        {
                            BusinessRoleName = brole.Description,
                            Country = usr.Country,
                            DisplayName = usr.DisplayName,
                            EmailId = usr.EmailId,
                            EmployeeId = usr.EmployeeId,
                            Ethinicity = usr.Ethinicity.ToString(),
                            JoinDate = usr.JoinDate,
                            MobileNumber = usr.MobileNumber,
                            OrganisationName = org.Name,
                            TelephoneNumber = usr.TelephoneNumber,
                            UserName = usr.UserName

                        }).ToList();
            }
            else
            {
                return (from usr in CurrentContext.User
                        join org in CurrentContext.NOrganisations
                        on usr.NOrganisationId equals org.Id into Orgdetail
                        join brole in CurrentContext.NDesignations
                        on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail
                        from org in Orgdetail.DefaultIfEmpty()
                        from brole in BusinessRoledetail.DefaultIfEmpty()
                        select new PresentationUser
                        {
                            BusinessRoleName = brole.Description,
                            Country = usr.Country,
                            DisplayName = usr.DisplayName,
                            EmailId = usr.EmailId,
                            EmployeeId = usr.EmployeeId,
                            Ethinicity = usr.Ethinicity.ToString(),
                            JoinDate = usr.JoinDate,
                            MobileNumber = usr.MobileNumber,
                            OrganisationName = org.Name,
                            TelephoneNumber = usr.TelephoneNumber,
                            UserName = usr.UserName

                        }).ToList();
            }
        }

        public IList<PresentationUser> GetNonEmployees()
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail


                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()

                    where usr.IsActive == true && usr.EmployeeType != EmployeeType.Permanent
                    select new PresentationUser
                    {
                        Id = usr.Id,
                        BusinessRoleName = brole.Description,
                        Country = usr.Country,
                        DisplayName = usr.DisplayName,
                        EmailId = usr.EmailId,
                        EmployeeId = usr.EmployeeId,
                        Ethinicity = usr.Ethinicity.ToString(),
                        JoinDate = usr.JoinDate,

                        MobileNumber = usr.MobileNumber,
                        OrganisationName = org.Name,
                        TelephoneNumber = usr.TelephoneNumber,
                        UserName = usr.UserName,
                        Reserved1 = usr.TelephoneNumber,
                        Reserved2 = usr.Reserved2,
                        Reserved3 = usr.Reserved3,
                        Reserved4 = usr.Reserved4
                    }).ToList();
        }

        public IList<PresentationUser> GetUserGroup()
        {
            return (from user in CurrentContext.User
                    from grp in user.Groups

                    select new PresentationUser
                    {
                        UserName = user.UserName,
                        DisplayName = user.DisplayName,
                        GroupName = grp.GroupName,
                        Description = grp.GroupDescription,
                        Owner = grp.Owner.DisplayName,
                        IsDefault = grp.IsDefaultGroup,
                        JoinDate = user.JoinDate,
                        CreatedDate = grp.CreatedDate
                    }).ToList();
        }
        public IList<PresentationUser> GetLeftEmployees()
        {
            return (from usr in CurrentContext.User
                    where usr.IsActive == false
                    select new PresentationUser
                    {
                        Id = usr.Id,
                        DisplayName = usr.DisplayName,
                        EmployeeId = usr.EmployeeId,
                        JoinDate = usr.JoinDate,
                        UserName = usr.UserName,
                        TerminationDate = usr.TerminationDate.Value,

                        Reserved1 = usr.Reserved1,
                        Reserved2 = usr.Reserved2,
                        Reserved3 = usr.Reserved3,
                        Reserved4 = usr.Reserved4,
                        Reserved5 = usr.Reserved5
                    }).ToList();
        }

        public IList<PresentationUser> GetUserRole()
        {
            return (from user in CurrentContext.User
                    from role in user.Roles
                    select new PresentationUser
                    {
                        UserName = user.UserName,
                        DisplayName = user.DisplayName,
                        RoleName = role.RoleName,
                        Description = role.RoleDescription,
                        Owner = role.Owner.DisplayName,
                        IsDefault = role.IsDefaultRole,
                        JoinDate = user.JoinDate,
                        CreatedDate = role.CreatedDate
                    }).ToList();
        }

        public IList<PresentationUser> GetMembers(long userId)
        {
            return (from usr in CurrentContext.User
                    join org in CurrentContext.NOrganisations
                    on usr.NOrganisationId equals org.Id into Orgdetail
                    join brole in CurrentContext.NDesignations
                    on usr.NBusinessRoleId equals brole.Id into BusinessRoledetail


                    from org in Orgdetail.DefaultIfEmpty()
                    from brole in BusinessRoledetail.DefaultIfEmpty()

                        //where usr.ManagerId == userId
                    select new PresentationUser
                    {
                        Id = usr.Id,
                        BusinessRoleName = brole.Description,
                        Country = usr.Country,
                        DisplayName = usr.DisplayName,
                        EmailId = usr.EmailId,
                        EmployeeId = usr.EmployeeId,
                        Ethinicity = usr.Ethinicity.ToString(),
                        JoinDate = usr.JoinDate,

                        MobileNumber = usr.MobileNumber,
                        OrganisationName = org.Name,
                        TelephoneNumber = usr.TelephoneNumber,
                        UserName = usr.UserName,

                        EmployeeType = usr.EmployeeType,
                        IsActive = usr.IsActive,
                        TerminationDate = usr.TerminationDate,
                    }).AsQueryable().ToList();
        }
    }
}
