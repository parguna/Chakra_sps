using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class GlobalQuestion : DomainModelBase<GlobalQuestion, EntityModel.GlobalQuestion>
    {
        public string Question { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public static bool DoesExists(string question)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.GlobalQuestion>(dataAccessTransaction).Any(questn => questn.Question == question);
            }
        }
    }
}
