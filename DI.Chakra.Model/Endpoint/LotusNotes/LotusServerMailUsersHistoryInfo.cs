using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
namespace DI.Chakra.Model
{
    public class LotusServerMailUsersHistoryInfo : IAggregateRoot
    {
        public string Name { get; set; }
        public string MailAddress { get; set; }
        public string MailFile { get; set; }
        public int? Version { get; set; }

        public static LotusServerMailUsersHistoryInfo CreateLotusServerMailInfo()
        {
            LotusServerMailUsersHistoryInfo lotusServerMailInfo = new LotusServerMailUsersHistoryInfo();

            return lotusServerMailInfo;
        }

    }
}
