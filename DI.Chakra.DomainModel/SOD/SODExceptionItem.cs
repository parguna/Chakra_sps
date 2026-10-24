using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODExceptionItem : DomainModelBase<SODExceptionItem, EntityModel.SODExceptionItem>
    {
        public Nullable<long> ExceptionId { get; set; }

        public Nullable<long> UserId { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public Nullable<long> FunctionItemId1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public Nullable<long> FunctionItemId2 { get; set; }

        public User user { get; set; }

        public SODFunction function1 { get; set; }

        public SODFunctionItem functionitem1 { get; set; }

        public SODFunction function2 { get; set; }

        public SODFunctionItem functionitem2 { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        //  public SODException SODException { get; set; }

        public static SODExceptionItem GetByExceptionId(long exceptionId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SODExceptionItem>(tranaction)
                    .Get(exp => exp.ExceptionId == exceptionId).FirstOrDefault());
            }
        }

    }
}
