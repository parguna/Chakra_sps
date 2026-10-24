using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_MailTemplate")]
    public class MailTemplate : EntityBase,ICreatable,IUpdatable
    {
        public string TemplateName { get; set; }

        public string TemplateDescription { get; set; }

        public string MailFrom { get; set; }

        public string MailTo { get; set; }

        public string MailCC { get; set; }

        public string MailBcc { get; set; }

        public string MailSubject { get; set; }

        public string MailContent { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }
    }
}
