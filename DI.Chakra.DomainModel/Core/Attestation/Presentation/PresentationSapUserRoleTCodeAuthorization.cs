using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Transaction;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationSapUserRoleTCodeAuthorization
    {
        public string UserName { get; set; }

        public string RoleCode { get; set; }

        public int EndpointSystemId { get; set; }

        public string TCode { get; set; }

        public static IList<PresentationSapUserRoleTCodeAuthorization> GetSapUserRoleTCodeAuthorization(long taskId)
        {
            var task = DomainModel.Task.GetAttestationTask(taskId, true);
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetSapUserRoleTCodeAuthorization(ORMapTask(task)));
            }
        }

        public static List<PresentationSapUserRoleTCodeAuthorization> ORMapList(IList<EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization> presentationSapUserRoleTCodeAuthorizations)
        {
            return presentationSapUserRoleTCodeAuthorizations.Select(ORMap).ToList();
        }
        public static PresentationSapUserRoleTCodeAuthorization ORMap(EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization presentationSapUserRoleTCodeAuthorization)
        {
            if (presentationSapUserRoleTCodeAuthorization == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSapUserRoleTCodeAuthorization, PresentationSapUserRoleTCodeAuthorization>(presentationSapUserRoleTCodeAuthorization);
        }
        public static EntityModel.Task ORMapTask(DomainModel.Task task)
        {
            if (task == null)
                return null;
            return Mapper.Map<Task, EntityModel.Task>(task);
        }

    }
}
