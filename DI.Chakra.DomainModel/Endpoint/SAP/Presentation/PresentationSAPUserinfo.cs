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
    public class PresentationSAPUserinfo : SAPUserinfo
    {

        public long EndpointUserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public long EndpointSystemId { get; set; }

        public string EndPointSystemType { get; set; }

        public string EndpointSystemName { get; set; }

        public string Category { get; set; }

        public string RoleCode { get; set; }

        public string RoleDescription { get; set; }


        //public new static List<PresentationSAPUserinfo> GetByDIUserId(long diUserId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetByDIUserId(diUserId));
        //    }
        //}

        public new static List<PresentationSAPUserinfo> GetByEndPointSystemId(long diUserId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(diUserId, endpointSystemId));
            }
        }

        public static PresentationSAPUserinfo ORMap(EntityModel.Presentation.PresentationSAPUserinfo presentationSapUserInfoEntityModel) 
        {
            if (presentationSapUserInfoEntityModel == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPUserinfo, PresentationSAPUserinfo>(presentationSapUserInfoEntityModel); 
        }

        public static List<PresentationSAPUserinfo> ORMapList(IList<EntityModel.Presentation.PresentationSAPUserinfo> presentationTabMenuEntityModels)
        {
            return presentationTabMenuEntityModels.Select(ORMap).ToList();
        }
    }
}
