using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Attention : DomainModelBase<Attention, EntityModel.Attention>
    {

        public string Name { get; set; }
        public string CommandName { get; set; }
        public long Count { get; set; }

        public static IList<Attention> GetAttention(long userId, bool AdminMode)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AttentionRepository(transaction).GetAttention(userId, AdminMode));
            }
        }

    }
}
