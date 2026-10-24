using DI.Chakra.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("AccountActionCategory")]
    public class AccountActionCategoryController : BaseController<DomainModel.AccountActionCategory, EntityModel.AccountActionCategory, AccountActionCategory>
    {
        [HttpGet]
        [Route("GetByIsActive")]
        public  List<AccountActionCategory> GetByIsActive()
        {
            return DtoMapList(DomainModel.AccountActionCategory.GetByIsActive());
        }
    }
}
