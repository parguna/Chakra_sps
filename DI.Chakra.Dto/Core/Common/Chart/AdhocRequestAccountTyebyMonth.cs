using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    public class AdhocRequestAccountTyebyMonth : DtoBase
    {
        public AccountType Name { get; set; }
        public decimal Y { get; set; }
        public decimal LabelName { get; set; }
    }
}
