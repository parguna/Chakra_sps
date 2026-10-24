using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PageUrlRepository : RepositoryBase<PageUrl>
    {
        public PageUrlRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetByUrl(string url)
        {
            return (from pageUrl in CurrentContext.PageUrls
                    where pageUrl.Url.Contains(url)
                    select new { pageUrl.Url, pageUrl.Id }).Distinct().Take(100)
                         .ToDictionary(c => c.Id, c => c.Url);
        }
    }
}
