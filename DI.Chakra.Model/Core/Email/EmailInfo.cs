using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_Email")]
    public class EmailInfo
    {
        [Key]
        public long Id { get; set; }
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
        public DateTime? DispatchDateTime { get; set; }
        public DateTime? SentDateTime { get; set; }
        public int NoOfAttempts { get; set; }



        public EmailInfo()
        {
            this.CreatedDateTime = DateTime.Now;
            this.Priority = "Normal";
            this.Status = "Pending";
            this.IsHtml = true;
            this.DispatchDateTime = DateTime.Now;
        }
    }     
}
