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
    public class PresentationSAPUserDetails
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string UserLogonName { get; set; }
        public static List<PresentationSAPUserDetails> GetSAPUsersToAssignEPGroup(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetSAPUsersToAssignEPGroup(endpointSystemId).ToList());
            }
        }
        public static PresentationSAPUserDetails ORMap(EntityModel.Presentation.PresentationSAPUserDetails presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPUserDetails, PresentationSAPUserDetails>(presentationADSUsersEntityModel);
        }

        public static List<PresentationSAPUserDetails> ORMapList(IList<EntityModel.Presentation.PresentationSAPUserDetails> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }
        public static List<PresentationSAPUserDetails> GetAllUserByEndpointSystemId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPUserInfoRepository(transaction).GetAllUserByEndpointSystemId(endpointSystemId).ToList());
            }
        }
    }
}
