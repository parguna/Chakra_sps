using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Linq;


namespace DI.Chakra.DomainModel
{
    public class MailTemplate : DomainModelBase<MailTemplate, EntityModel.MailTemplate>
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

        public static bool DoesNameAlreadyExists(string templateName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.MailTemplate>(dataAccessTransaction)
                       .Any(pc => pc.TemplateName == templateName);
            }
        }

        public static MailTemplate GetByTemplateName(string templateName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.MailTemplate>(dataAccessTransaction)
                    .Get(temp => temp.TemplateName == templateName).FirstOrDefault());
            }
        }

    }
}
