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
    public class PresentationEndPoint : EndPoint
    {
        public long Endpointsystemid { get; set; }

        public string EndpointsystemType { get; set; }

        public string Endpointsystemdescription { get; set; }

        public string HostName { get; set; }

        

        public string DomainName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }

        public string Status { get; set; }

        //public static List<PresentationEndPoint> GetByDiUserid(long diUserId)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        var adsEndpointList = new EndPointRepository(dataAccessTransaction).GetADSEndpointByDiUserid(diUserId);
        //        var sapEndpointList = new EndPointRepository(dataAccessTransaction).GetSAPEndpointByDiUserid(diUserId);
        //        var lotusEndpointList = new EndPointRepository(dataAccessTransaction).GetLotusEndpointByDiUserid(diUserId);
        //        var sapepEndpointList = new EndPointRepository(dataAccessTransaction).GetSAPEPEndpointByDiUserid(diUserId);
        //        var inhEndpointList = new EndPointRepository(dataAccessTransaction).GetINHEndpointByDiUserid(diUserId);
        //        var concadeList1 = adsEndpointList.Union(sapEndpointList);
        //        var concadeList2 = concadeList1.Union(lotusEndpointList);
        //        var concadeList3 = concadeList2.Union(sapepEndpointList);
        //        var concadeTotalList = concadeList3.Union(inhEndpointList);

        //        PresentationEndPoint endPointSystem = new PresentationEndPoint();
        //        List<PresentationEndPoint> EndPointlist = null;

        //        if (concadeTotalList.Count() > 0)
        //        {
        //            foreach (var tmpList in concadeTotalList)
        //            {
        //                endPointSystem.Endpointsystemid = CheckNull.AgainstLong(tmpList.Endpointsystemid);
        //                endPointSystem.Endpointsystemdescription = CheckNull.AgainstString(tmpList.Endpointsystemdescription);
        //                endPointSystem.EndpointsystemType = CheckNull.AgainstString(tmpList.EndpointsystemType);
        //                endPointSystem.EndpointSystemTypeId = CheckNull.AgainstLong(tmpList.EndpointSystemTypeId);
        //                endPointSystem.HostName = CheckNull.AgainstString(tmpList.HostName);
        //                endPointSystem.UserName = CheckNull.AgainstString(tmpList.UserName);
        //                endPointSystem.Status = string.Empty;
        //                EndPointlist.Add(endPointSystem);
        //            }
        //        }

        //        return EndPointlist;
        //    }
        //}
        
        public static PresentationEndPoint ORMap(EntityModel.Presentation.PresentationEndPoint PresentationEndPointEntityModel)
        {
            if (PresentationEndPointEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationEndPoint, PresentationEndPoint>(PresentationEndPointEntityModel);
        }

        public static List<PresentationEndPoint> ORMapList(IList<EntityModel.Presentation.PresentationEndPoint> PresentationEndPointEntityModels)
        {
            return PresentationEndPointEntityModels.Select(ORMap).ToList();
        }
    }
}
