using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("SODFunction")]
    public class SODFunctionController : BaseController<DomainModel.SODFunction, EntityModel.SODFunction, Dto.SODFunction>
    {
        [HttpGet]
        [Route("GetByIdIncludeSodFunctionItem")]
        public SODFunction GetByIdIncludeSodFunctionItem(long id)
        {
            return DtoMap(DomainModel.SODFunction.GetByIdIncludeSodFunctionItem(id));
        }

        [HttpGet]
        [Route("GetAllIncludeSodFunctionItems")]
        public List<SODFunction> GetAllIncludeSodFunctionItems()
        {
            return DtoMapList(DomainModel.SODFunction.GetAllIncludeSodFunctionItems());
        }

        [HttpGet]
        [Route("DoesExists")]
        public bool DoesExists(string sodFunctionCode)
        {
            return DomainModel.SODFunction.DoesExists(sodFunctionCode);
        }

        [HttpGet]
        [Route("GetBySodFunctionCodeIncludingSodFunctionItems")]
        public SODFunction GetBySodFunctionCodeIncludingSodFunctionItems(string sodFunctionCode)
        {
            return DtoMap(DomainModel.SODFunction.GetBySodFunctionCodeIncludingSodFunctionItems(sodFunctionCode));
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public  Dictionary<long, string> GetBySearchString(string searchString)
        {
                return DomainModel.SODFunction.GetBySearchString(searchString);
        }

    }
}
