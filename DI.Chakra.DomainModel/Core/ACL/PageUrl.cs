using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PageUrl : DomainModelBase<PageUrl, EntityModel.PageUrl>
    {
        public string Url { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static bool DoesExists(string url)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.PageUrl>(dataAccessTransaction).Any(pageUrl => pageUrl.Url == url);
            }
        }

        public static bool HasAnyWebPage(long pageUrlId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.WebPage>(dataAccessTransaction).Any(page => page.PageUrlId == pageUrlId);
            }
        }

        public static PageUrl GetByWebPageURL(string url)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.PageUrl>(dataAccessTransaction).Get(pageUrl => pageUrl.Url == url).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetByUrl(string url)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new PageUrlRepository(dataAccessTransaction).GetByUrl(CheckNull.AsteriskToString(url));
            }
        }
    }
}
