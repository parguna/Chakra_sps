using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class WebPage : DomainModelBase<WebPage, EntityModel.WebPage>
    {
        private PageUrl _pageUrl;

        public string UniqueName { get; set; }

        public string Name { get; set; }

        public long? PageUrlId { get; set; }

        public PageUrl PageUrl { get; set;  }

        public string CreatedBy { get; set; }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesAnyExists(string name, string uniqueName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction).Any(p => p.Name == name || p.UniqueName == uniqueName);
            }
        }

        public static bool HasAnyPageTabMenu(long webPageId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.TabMenu>(dataAccessTransaction).Any(tabmenu => tabmenu.PageId == webPageId);
            }
        }

        public static Dictionary<long, string> GetByName(string name)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new WebPageRepository(dataAccessTransaction).GetByName(CheckNull.AsteriskToString(name));
            }
        }

        public static WebPage GetByAbsolutePath(string absolutePath)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction).Get(t => t.PageUrl.Url.ToUpper() == absolutePath.ToUpper(),t =>t.PageUrl).FirstOrDefault());
            }
        }

        public static WebPage GetByIdInludePageUrl(long id)  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction)
                    .GetById(id,
                    attn => attn.PageUrl));
            }
        }

        public static WebPage GetByUniqueName(string name)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction)
                    .Get(a => a.UniqueName == name).FirstOrDefault());
            }
        }

        public static List<WebPage> GetAllInludePageUrl()  
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction)
                    .GetAll(attn => attn.PageUrl).ToList());
            }
        }
        public static WebPage GetByUrlId(long urlId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction)
                    .Get(p=>p.PageUrlId == urlId).FirstOrDefault());
            }
        }
        public static bool DeletePageById(int pageId)
        {

            int recordsAffected = 0;
            List<PagePermission> pageper = PagePermission.GetByPageId(pageId, true).ToList();
            foreach (var per in pageper)
            {
                AccessRights ar = AccessRights.GetByPagePermissionId(per.Id);

                if (ar != null)
                {
                    AccessRights.Delete(ar.Id);
                }

                PagePermission.Delete(per.Id);
            }

            WebPage delPage = WebPage.GetById(pageId);
            if (delPage != null)
            {
                 delPage.Delete();
            }

            if (recordsAffected >= 1)
                return true;
            return false;
        }     
    }
}
