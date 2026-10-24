using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class WebPageRepository : RepositoryBase<WebPage>
    {
        public WebPageRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetByName(string name)
        {
            return (from page in CurrentContext.Pages
                    where page.Name.Contains(name)
                    select new { page.Name, page.Id })
                         .ToDictionary(c => c.Id, c => c.Name);
        }
    }
}
