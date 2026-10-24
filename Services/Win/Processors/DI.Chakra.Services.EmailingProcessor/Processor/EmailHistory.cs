using System;
using System.Collections.Generic;
using System.Text;

namespace DI.Chakra.Services.EmailingProcessor
{

    public class EmailHistory 
    {
        public int Id { get; set; }
        public int HistoryId { get; set; } 
        public string ActivityType { get; set; }
        public string Priority { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public string Attachment { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? SentDateTime { get; set; }
    }
}
