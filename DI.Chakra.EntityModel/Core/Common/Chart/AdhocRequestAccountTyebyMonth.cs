using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class AdhocRequestAccountTyebyMonth : EntityBase
    {
        public AdHocActionType Name { get; set; }
        public decimal Y { get; set; }
        public AccountType LabelName { get; set; }

    }
}
