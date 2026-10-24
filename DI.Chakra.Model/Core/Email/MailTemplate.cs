using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.Model
{
    [Table("DI_M_MailTemplate")]
    public class MailTemplate : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDescription { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }
        public string MailCC { get; set; }
        public string MailBcc { get; set; }
        public string MailSubject { get; set; }
        public string MailContent { get; set; }
        public string ApplicableVariableScopes { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public VariableScope TemplateScope { get; set; }

        public EmailTemplate MailTemplateName { get; set; }


    }
}
