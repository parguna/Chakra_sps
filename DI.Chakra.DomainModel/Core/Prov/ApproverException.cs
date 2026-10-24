using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ApproverException : DomainModelBase<ApproverException, EntityModel.ApproverException>
    {
        public string Person { get; set; }

        public string Approver { get; set; }

        public int ApprovalLevel { get; set; }


        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }


        public static string CheckForApproverInException(string pernr, int level)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                ApproverException exception = ORMap(new RepositoryBase<EntityModel.ApproverException>(transaction).Get(item => item.Person == pernr && item.ApprovalLevel == level).FirstOrDefault());

                if (exception != null)
                {
                    return exception.Approver;
                }
                return string.Empty;
            }
        }
    }
}
