
using AutoMapper;
using DI.Chakra.Dto.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationADSActiveAccountsWithInActiveUser")]
    
    public class PresentationADSActiveAccountsWithInActiveUserController : ApiController
    {
        public PresentationADSActiveAccountsWithInActiveUser DtoMap(DomainModel.Presentation.PresentationADSActiveAccountsWithInActiveUser tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationADSActiveAccountsWithInActiveUser, PresentationADSActiveAccountsWithInActiveUser>(tModel);
        }

        public  List<PresentationADSActiveAccountsWithInActiveUser> DtoMapList(IList<DomainModel.Presentation.PresentationADSActiveAccountsWithInActiveUser> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }

        [HttpGet]
        [Route("GetADSActiveAccountsWithInActiveUser")]
        public List<PresentationADSActiveAccountsWithInActiveUser> GetADSActiveAccountsWithInActiveUser(long endpointsystemid)
        {
            return DtoMapList(DomainModel.Presentation.PresentationADSActiveAccountsWithInActiveUser.GetADSActiveAccountsWithInActiveUser(endpointsystemid));
        }
    }
}
