using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ExceptionAttributeChange : DomainModelBase<ExceptionAttributeChange, EntityModel.ExceptionAttributeChange>
    {
        public long ReconcilationId { get; set; }

        public long ExceptionAttributeId { get; set; }

        public string Attributes { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public AccountReconcilation AccountReconcilations { get; set; }

        public ExceptionAttribute ExceptionAttributes { get; set; }

        public static List<ExceptionAttributeChange> GetByReconcilationId(long ReconcilationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ExceptionAttributeChange>(dataAccessTransaction).
                    Get(ec => ec.ReconcilationId == ReconcilationId).ToList());
            }
        }
        public static ExceptionAttributeChange GetExceptionAttributes(long ReconcilationId,long exceptionAttributeId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.ExceptionAttributeChange>(dataAccessTransaction).Get(ar => ar.ReconcilationId == ReconcilationId && ar.ExceptionAttributeId == exceptionAttributeId ).FirstOrDefault());
            }
        }
    }
}
