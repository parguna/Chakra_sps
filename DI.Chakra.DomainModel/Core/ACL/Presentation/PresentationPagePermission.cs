using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Repository.Core.ACL;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationPagePermission : PagePermission
    {
        public string PermissionName { get; set; }

        public string PageUrlName { get; set; }

        public static List<PresentationPagePermission> GetByPageId(long pageId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PagePermissionRepository(dataAccessTransaction).GetByPageId(pageId));
            }
        }

        public static PresentationPagePermission ORMap(EntityModel.Presentation.PresentationPagePermission presentationPagePermissionEntityModel)
        {
            if (presentationPagePermissionEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationPagePermission, PresentationPagePermission>(presentationPagePermissionEntityModel);
        }

        public static List<PresentationPagePermission> ORMapList(IList<EntityModel.Presentation.PresentationPagePermission> presentationPagePermissionEntityModel)
        {
            return presentationPagePermissionEntityModel.Select(ORMap).ToList();
        }
    }
}
