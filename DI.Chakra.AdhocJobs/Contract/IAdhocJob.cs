using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.AdHocJobs
{
    public interface IAdhocJob
    {
        void BuildPreConditionResultSet();

        void Execute();

        Object Result { get; set; }
    }
}
