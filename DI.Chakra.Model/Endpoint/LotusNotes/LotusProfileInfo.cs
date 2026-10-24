using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    public class LotusProfileInfo : IAggregateRoot
    {
        public string ProfileName { get; set; }
        public int? Version { get; set; }

        public static LotusProfileInfo CreateLotusProfileInfo()
        {
            LotusProfileInfo lotusProfileInfo = new LotusProfileInfo();

            return lotusProfileInfo;
        }
    }
}
