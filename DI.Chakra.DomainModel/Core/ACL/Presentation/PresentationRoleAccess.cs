using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationRoleAccess
    {
        public long AccessId { get; set; }
        public string RoleName { get; set; }
        public string PageName { get; set; }
        public string AccessRights { get; set; }
        public static IList<PresentationRoleAccess> GetAccessToRoles()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationRoleAccessRepository(dataAccessTransaction).GetAccessToRoles());
            }
        }
        public static IList<PresentationRoleAccess> GetAccessToRoles(string roleCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationRoleAccessRepository(dataAccessTransaction).GetAccessToRoles(roleCode));
            }
        }

        public static PresentationRoleAccess ORMap(EntityModel.Presentation.PresentationRoleAccess presentationRoleAccess)
        {
            if (presentationRoleAccess == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationRoleAccess, PresentationRoleAccess>(presentationRoleAccess);
        }
        public static List<PresentationRoleAccess> ORMapList(IList<EntityModel.Presentation.PresentationRoleAccess> presentationRoleAccess)
        {
            return presentationRoleAccess.Select(ORMap).ToList();
        }
    }
}
