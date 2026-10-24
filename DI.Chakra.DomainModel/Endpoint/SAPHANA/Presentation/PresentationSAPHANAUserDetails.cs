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
    public class PresentationSAPHANAUserDetails
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }
        public string UserLogonName { get; set; }
        
        public static PresentationSAPHANAUserDetails ORMap(EntityModel.Presentation.PresentationSAPHANAUserDetails presentationADSUsersEntityModel)
        {
            if (presentationADSUsersEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationSAPHANAUserDetails, PresentationSAPHANAUserDetails>(presentationADSUsersEntityModel);
        }

        public static List<PresentationSAPHANAUserDetails> ORMapList(IList<EntityModel.Presentation.PresentationSAPHANAUserDetails> presentationADSUsersEntityModels)
        {
            return presentationADSUsersEntityModels.Select(ORMap).ToList();
        }
        public static List<PresentationSAPHANAUserDetails> GetAllUserByEndpointSystemId(long endpointSystemId)
        {
            using (var transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new SAPHANAUserInfoRepository(transaction).GetAllUserByEndpointSystemId(endpointSystemId).ToList());
            }
        }
    }
}
