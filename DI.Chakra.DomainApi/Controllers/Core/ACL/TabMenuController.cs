
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("TabMenu")]
    public class TabMenuController : BaseController<DomainModel.TabMenu, EntityModel.TabMenu, Dto.TabMenu>
    {
        [HttpGet]
        [Route("GetTabMenu")]
        public List<Dto.TabMenu> GetTabMenu(long userId)
        {
            return DtoMapList(DomainModel.TabMenu.GetTabMenu(userId));
        }

        [HttpGet]
        [Route("GetMenu")]
        public List<TabMenu> GetMenu(long userId, string menuUniqueName)
        {
            return DtoMapList(DomainModel.TabMenu.GetMenu(userId, menuUniqueName));
        }

        [HttpGet]
        [Route("GetByMenuname")]
        public  TabMenu GetByMenuname(string menuName)
        {
            return DtoMap(DomainModel.TabMenu.GetByMenuname(menuName));
        }
        [HttpGet]
        [Route("GetCategoryById")]
        public TabMenu GetCategoryById(int menuId)
        {
            return DtoMap(DomainModel.TabMenu.GetCategoryById(menuId));
        }

        [HttpGet]
        [Route("GetAllReportMenu")]
        public Dictionary<long, string> GetAllReportMenu(string searchString)
        {
            return DomainModel.TabMenu.GetAllReportMenu(searchString);
        }

        [HttpGet]
        [Route("DoesChildExists")]
        public bool DoesChildExists(int id)
        {
            return DomainModel.TabMenu.DoesChildExists(id);
        }

        [HttpGet]
        [Route("DoesAnyExists")]
        public bool DoesAnyExists(string menuName, string uniqueName) 
        {
            return DomainModel.TabMenu.DoesAnyExists(menuName,uniqueName);
        }

        [HttpGet]
        [Route("GetBySearchString")]
        public Dictionary<long,string> GetBySearchString(string menuName) 
        {
            return DomainModel.TabMenu.GetBySearchString(menuName);
        }
    }
}
