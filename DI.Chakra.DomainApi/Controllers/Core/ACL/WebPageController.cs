using System.Collections.Generic;
using System.Web.Http;
using DI.Chakra.Dto;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("WebPage")]
    public class WebPageController : BaseController<DomainModel.WebPage, EntityModel.WebPage, Dto.WebPage>
    {
        
        [HttpGet]
        [Route("DoesAnyExists")]
        public bool DoesExists(string name, string uniqueName)
        {
            return DomainModel.WebPage.DoesAnyExists(name,uniqueName);
        }

        [HttpGet]
        [Route("GetByIdInludePageUrl")]
        public  Dto.WebPage GetByIdInludePageUrl(long id)
        {
            return DtoMap(DomainModel.WebPage.GetByIdInludePageUrl(id));
        }

        [HttpGet]
        [Route("GetAllInludePageUrl")]
        public  List<Dto.WebPage> GetAllInludePageUrl()
        {
            return DtoMapList(DomainModel.WebPage.GetAllInludePageUrl());
        }

        [HttpGet]
        [Route("DeletePageById")]
        public bool DeletePageById(int pageId)
        {
            return DomainModel.WebPage.DeletePageById(pageId);
        }

        [HttpGet]
        [Route("GetByAbsolutePath")]
        public WebPage GetByAbsolutePath(string absolutePath)
        {
            return DtoMap(DomainModel.WebPage.GetByAbsolutePath(absolutePath));
        }

        [HttpGet]
        [Route("GetByName")]
        public Dictionary<long, string> GetByName(string name) 
        {
            return DomainModel.WebPage.GetByName(name);
        }
    }
}
