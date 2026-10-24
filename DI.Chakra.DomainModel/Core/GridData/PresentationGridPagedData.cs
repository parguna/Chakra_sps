using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Core
{
    public class PresentationGridPagedData<T>
    {
        public List<T> Data = new List<T>();
        public int TotalCount = 0;
    }
    public class PresentationGridPagedDataParameter
    {
        public bool FullTimeEmp { get; set; }
        public int StartRowIndex { get; set; }
        public int MaximumRows { get; set; }
        public string FilterExpression { get; set; }
        public string SortExpression { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<long> Ids = new List<long>();
    }
}
