using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class AttestationHierarchyRepository : RepositoryBase<AttestationHierarchy>
    {
        public AttestationHierarchyRepository(DataAccessTransaction transaction) : base(transaction) { }
        

    }
}
