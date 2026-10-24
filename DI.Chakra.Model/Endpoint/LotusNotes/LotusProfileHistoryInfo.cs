using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    public class LotusProfileHistoryInfo : IAggregateRoot
    {
        public string ProfileName { get; set; }
        public int? Version { get; set; }

        public static LotusProfileHistoryInfo CreateLotusProfileInfo()
        {
            LotusProfileHistoryInfo lotusProfileInfo = new LotusProfileHistoryInfo();

            return lotusProfileInfo;
        }
    }
}
