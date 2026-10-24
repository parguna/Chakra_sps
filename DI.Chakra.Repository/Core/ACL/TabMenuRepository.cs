using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Logger;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class TabMenuRepository : RepositoryBase<TabMenu>
    {
        public TabMenuRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationTabMenu> GetAll()
        {
            return (from item in CurrentContext.TabMenu
                    join men in CurrentContext.TabMenu
                    on item.ParentTabMenuId equals (int)men.Id into menu
                    from mn in menu.DefaultIfEmpty()
                    join page in CurrentContext.Pages
                    on item.PageId equals (int)page.Id
                    select new PresentationTabMenu
                    {
                        Id = item.Id,
                        UniqueName = item.UniqueName,
                        MenuName = item.MenuName,
                        ParentMenuName = mn.MenuName,
                        PageName = page.Name
                    }).ToList();
        }

        public List<PresentationPagePermission> GetByPageId(long pageId)
        {
            return (from item in CurrentContext.PagePermissions
                    join per in CurrentContext.Permissions
                    on item.PermissionId equals per.Id
                    join url in CurrentContext.PageUrls
                    on item.UrlId equals url.Id into lq
                    from urlLeft in lq.DefaultIfEmpty()
                    where item.PageId == pageId
                    select new PresentationPagePermission
                    {
                        Id = item.Id,
                        PermissionId = item.PermissionId,
                        PermissionName = per.Name,
                        UrlId = item.UrlId,
                        PageUrlName = urlLeft.Url
                    }).ToList();
        }

        public Dictionary<long, string> GetBySearchString(string menuName) 
        {
            return (from menu in CurrentContext.TabMenu
                    where menu.MenuName.Contains(menuName)
                    select new { menu.MenuName, menu.Id })
                         .ToDictionary(c => c.Id, c => c.MenuName);
        }

        public TabMenu GetTabMenu(string absolutePath)
        {
            return (from TM in CurrentContext.TabMenu
                    where TM.WebPage.PageUrl.Url.ToUpper() == absolutePath.ToUpper()
                    select TM).FirstOrDefault();

       }

        public Dictionary<long, string> GetAllReportMenu(string filter = "")
        {
            try
            {
                //return (from TM in CurrentContext.TabMenu
                //        where TM.UniqueName.Trim().ToUpper().Contains("REPORT")
                //        select new { value = TM.UniqueName + "(" + TM.MenuName + ")", key = TM.Id }).Distinct().OrderBy(a => a.value).ToDictionary(c => c.key, c => c.value);
                using (var transaction = new DataAccessTransaction())
                {
                    var result = new Dictionary<long, string>();
                    var tmpFilter = " 1=1 AND ";
                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();
                    if (!string.IsNullOrWhiteSpace(filter) && filter != "*")
                    {
                        tmpFilter = " (UniqueName like @filterPattern OR MenuName like @filterPattern) AND ";
                        sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@filterPattern", "%" + filter + "%"));
                    }
                    var query = $"SELECT Id,UniqueName, MenuName FROM DI_M_TabMenu where {tmpFilter} ParentTabMenuId in ( SELECT Id FROM DI_M_TabMenu where ParentTabMenuId = (SELECT id FROM DI_M_TabMenu where UniqueName = 'REPORT'))";
                    var dtResult = new RepositoryBase<EntityModel.EntityBase>(transaction).GetDataTableByQuery(query, sqlParameters.ToArray());
                    if(dtResult != null)
                    {
                        foreach (DataRow dtRow in dtResult.Rows)
                        {
                            result.Add(Convert.ToInt64(dtRow["Id"]), $"{Convert.ToString(dtRow["UniqueName"])} ({Convert.ToString(dtRow["MenuName"])})");
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<long, string>();
            }
        }

        public List<TabMenu> GetAllTabMenu(Role roleInfo)
        {
            return (from TM in CurrentContext.TabMenu.Include("WebPage").Include("WebPage.PageUrl")
                    join AR in CurrentContext.AccessRights.Include("PagePermission").Include("PagePermission.WebPage")
                            on TM.WebPage.Id equals AR.PagePermission.WebPage.Id
                    join R in CurrentContext.Roles
                            on AR.RoleId equals R.Id
                            where TM.UniqueName != "REPORT" && R.Id == roleInfo.Id || TM.WebPage == null
                     select TM).Include("WebPage").Include("WebPage.PageUrl").Distinct().ToList();
        }

        public List<TabMenu> GetMenu(Role roleInfo, string menuUniqueName)
        {
            return (from TM in CurrentContext.TabMenu.Include("WebPage").Include("WebPage.PageUrl")
                    join AR in CurrentContext.AccessRights.Include("PagePermission").Include("PagePermission.WebPage")
                    on TM.WebPage.Id equals AR.PagePermission.WebPage.Id
                    join R in CurrentContext.Roles
                    on AR.RoleId equals R.Id
                    where R.Id == roleInfo.Id || TM.WebPage == null
                    select TM).Include("WebPage").Include("WebPage.PageUrl").Distinct().ToList();
        }


        public List<TabMenu> GetTabMenu(long roleId)
        {
            return (from TM in CurrentContext.TabMenu.Include("WebPage").Include("WebPage.PageUrl")
                    join AR in CurrentContext.AccessRights.Include("PagePermission").Include("PagePermission.WebPage")
                    on TM.WebPage.Id equals AR.PagePermission.WebPage.Id
                    join R in CurrentContext.Roles
                    on AR.RoleId equals R.Id
                    where TM.UniqueName != "REPORT" && R.Id == roleId || TM.WebPage == null
                    select TM).Include("WebPage").Include("WebPage.PageUrl").Distinct().OrderBy(t => t.Id).ToList();
        }
    }
}