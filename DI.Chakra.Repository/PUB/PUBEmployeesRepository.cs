using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository 
{
    public class PUBEmployeesRepository : RepositoryBase<PUBEmployees>
    {
        public PUBEmployeesRepository(DataAccessTransaction transaction) : base(transaction) { }

        
        public List<PUBEmployees> GetPUBEmployeesByStatus(long fileId, string[] statusArray)
        {
            return this.CurrentContext.PUBEmployees.Where(p => p.PUBFileId==fileId 
            && statusArray.Contains(p.Status)).Select(p => p).ToList();
        }
    }
}
