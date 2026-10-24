using System;
using System.Collections.Generic;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    public class MailGridData
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string ActivityType { get; set; }
        public string Status { get; set; }
        public DateTime? SentDateTime { get; set; }
    }
}
