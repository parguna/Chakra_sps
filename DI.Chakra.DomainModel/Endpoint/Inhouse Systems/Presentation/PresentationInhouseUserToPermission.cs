using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationInhouseUserToPermission
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string PermissionName { get; set; }
        public string PermissionDescription { get; set; }


        public static IList<PresentationInhouseUserToPermission> GetUserToPermissionsReportData(long endpointSystemid)
        {
            IList<PresentationInhouseUserToPermission> presentationINHUsersReportDataList = new List<PresentationInhouseUserToPermission>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                presentationINHUsersReportDataList = ORMapList(new InhouseUserInfoRepository(dataAccessTransaction).GetUserToPermissionsReportData(endpointSystemid));
            }

            return presentationINHUsersReportDataList;
        }

        public static PresentationInhouseUserToPermission ORMap(EntityModel.Presentation.PresentationInhouseUserToPermission presentationTabMenuEntityModel)
        {
            if (presentationTabMenuEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationInhouseUserToPermission, PresentationInhouseUserToPermission>(presentationTabMenuEntityModel);
        }

        public static List<PresentationInhouseUserToPermission> ORMapList(IList<EntityModel.Presentation.PresentationInhouseUserToPermission> presentationInhouseGridEntityModels)
        {
            return presentationInhouseGridEntityModels.Select(ORMap).ToList();
        }

    }
}
