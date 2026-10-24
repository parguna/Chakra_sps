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
    public class PresentationSAPHANAUserinfo : SAPHANAUserinfo
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


        //public new static List<PresentationSAPHANAUserinfo> GetByDIUserId(long diUserId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new SAPUserInfoRepository(dataAccessTransaction).GetByDIUserId(diUserId));
        //    }
        //}

        public new static List<PresentationSAPHANAUserinfo> GetByEndPointSystemId(long diUserId, long endpointSystemId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPHANAUserInfoRepository(dataAccessTransaction).GetByEndPointSystemId(diUserId, endpointSystemId));
            }
        }

        public static PresentationSAPHANAUserinfo ORMap(EntityModel.Presentation.PresentationSAPHANAUserinfo presentationSapUserInfoEntityModel) 
        {
            if (presentationSapUserInfoEntityModel == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationSAPHANAUserinfo, PresentationSAPHANAUserinfo>(presentationSapUserInfoEntityModel); 
        }

        public static List<PresentationSAPHANAUserinfo> ORMapList(IList<EntityModel.Presentation.PresentationSAPHANAUserinfo> presentationTabMenuEntityModels)
        {
            return presentationTabMenuEntityModels.Select(ORMap).ToList();
        }
    }
}
