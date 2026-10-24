using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto.Presentation;
using AutoMapper;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationUserAccount")]
    public class PresentationUserAccountController : ApiController
    {
        [HttpGet]
        [Route("GetUserAccountDetail")]
        public IList<PresentationUserAccount> GetUserAccountDetail(long userID, bool pwdReset)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserAccount.GetUserAccountDetail(userID, pwdReset));
        }
        [HttpGet]
        [Route("GetUserSelfServiceAccountDetail")]
        public IList<PresentationUserAccount> GetUserSelfServiceAccountDetail(long userID, PasswordActionType selfServiceType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserAccount.GetUserSelfServiceAccountDetail(userID, selfServiceType));
        }

        [HttpGet]
        [Route("GetAccountDetailByAccountType")]
        public IList<PresentationUserAccount> GetAccountDetailByAccountType(long userID, AccountType accountType)
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserAccount.GetAccountDetailByAccountType(userID, accountType));
        }
        [HttpGet]
        [Route("GetAllUserAccountDetail")]
        public IList<PresentationUserAccount> GetAllUserAccountDetail()
        {
            return DtoMapList(DomainModel.Presentation.PresentationUserAccount.GetAllUserAccountDetail());
        }
        [HttpGet]
        [Route("GetUserAccountDetailByEndPointSystemId")]
        public PresentationUserAccount GetUserAccountDetailByEndPointSystemId(long userID, long endPointSystemId, long endPointUserId)
        {
            return DtoMap(DomainModel.Presentation.PresentationUserAccount.GetUserAccountDetailByEndPointSystemId(userID, endPointSystemId, endPointUserId));
        }
        public  PresentationUserAccount DtoMap(DomainModel.Presentation.PresentationUserAccount tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationUserAccount, PresentationUserAccount>(tModel);
        }
        public  List<PresentationUserAccount> DtoMapList(IList<DomainModel.Presentation.PresentationUserAccount> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
    }
}
