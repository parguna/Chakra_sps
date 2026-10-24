using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSUserToGroups 
    {
        public string UserName { get; set; }

        public string DisplayName { get; set; }

        public Nullable<DateTime> AccountCreatedDate { get; set; }

        public Nullable<DateTime> PasswordExpirationDate { get; set; }

        public Nullable<DateTime> LastLoginDate { get; set; }

        public string Disabled { get; set; }

        public string Locked { get; set; }

        public string Email { get; set; }

        public string Department { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string GroupOU { get; set; }

        public string UserOU { get; set; }
        public string TotalUsage { get; set; }
        public string UsageInPercentage { get; set; }

        public static PresentationADSUserToGroups ORMap(EntityModel.Presentation.PresentationADSUserToGroups presentationADSUserToGroupsEntityModel)
        {
            if (presentationADSUserToGroupsEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSUserToGroups, PresentationADSUserToGroups>(presentationADSUserToGroupsEntityModel);
        }

        public static List<PresentationADSUserToGroups> ORMapList(IList<EntityModel.Presentation.PresentationADSUserToGroups> presentationADSUserToGroupsEntityModels)
        {
            return presentationADSUserToGroupsEntityModels.Select(ORMap).ToList();
        }

        public static IList<PresentationADSUserToGroups> GetADSUserToGroupsReportData(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSUserInfoRepository(dataAccessTransaction).GetADSUserToGroupsReportData(endPointSystemId));
            }
        }

        public static IList<PresentationADSUserToGroups> GetADSUserToGroupsReportDataByDepartment(long endPointSystemId, long orgUnitId, long CuttOff)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSUserInfoRepository(dataAccessTransaction).GetADSUserToGroupsReportData(endPointSystemId, orgUnitId, CuttOff));
            }
        }

        public static IList<PresentationADSUserToGroups> GetADSUserToGroupsReportDataByDepartmentAndGroup(long endPointSystemId, long DepartmentCode, string GroupName)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSUserInfoRepository(dataAccessTransaction).GetADSUserToGroupsReportDataByDepartmentAndGroup(endPointSystemId, DepartmentCode, GroupName));
            }
        }
    }
}
