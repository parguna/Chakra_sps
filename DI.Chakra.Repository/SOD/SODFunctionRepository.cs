using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODFunctionRepository : RepositoryBase<SODFunction>
    {
        public SODFunctionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from functs in CurrentContext.SODFunctions
                    where functs.SodFunctionCode.ToString().Contains(searchString)
                    select new { funct = functs.Id, Value = functs.SodFunctionCode + " - " + functs.SodFunctionDescription })
                         .ToDictionary(c => c.funct, c => c.Value);
        }

        public IList<object> GetReport()
        {
            return (from function in CurrentContext.SODFunctions
                    join functionitem in CurrentContext.SODFuntionItem
                    on function.Id equals functionitem.SodFunctionId
                    join endpointsystem in CurrentContext.EndPoint
                    on functionitem.Endpointsystemid equals endpointsystem.Id
                    select new
                    {
                        FunctionCode = function.SodFunctionCode,
                        FunctionDescription = function.SodFunctionDescription,
                        Endpoint = endpointsystem.DisplayName,
                        FunctionType = functionitem.SodFunctionType,
                        FunctionValue = functionitem.SodRoleDesc,
                        ObjectCode = functionitem.ObjectCode,
                        FieldCode = functionitem.FieldCode,
                        FieldValue = functionitem.FromValue
                    }).ToList<object>();
        }
    }
}
