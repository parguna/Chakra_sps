using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class IACMGroupInfoRepository : RepositoryBase<IACMGroupInfo>
    {
        public IACMGroupInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IACMGroupInfo GetByName(string grpName)
        {
            return (from grp in CurrentContext.IACMGroupInfos.Include("Users")
                    where grp.Name == grpName
                    select grp).FirstOrDefault();
        }

        public void ExecuteQuery(string query)
        {
            CurrentContext.Database.ExecuteSqlCommand(query);
        }
    }
}
