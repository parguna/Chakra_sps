using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    public class UserGlobalQuestionGridData
    {
        public int GlobalQuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int UserId { get; set; }

    }
}
