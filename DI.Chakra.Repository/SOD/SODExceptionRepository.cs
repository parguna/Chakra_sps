using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class SODExceptionRepository : RepositoryBase<SODException>
    {
        public SODExceptionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public Dictionary<long, string> GetBySearchString(string searchString)
        {
            return (from expt in CurrentContext.SODException
                    where expt.ExceptionCode.ToString().Contains(searchString)
                    select new { expts = expt.Id, Value = expt.ExceptionCode + " - " + expt.ExceptionDesc })
                    .Distinct().OrderBy(s => s.Value)
                         .ToDictionary(c => c.expts, c => c.Value);
        }

        public List<object> GetReportforException()
        {
            return (from exception in CurrentContext.SODException
                    join exceptionitem in CurrentContext.SODExceptionItem
                    on exception.Id equals exceptionitem.ExceptionId
                    join user in CurrentContext.User
                    on exception.RiskOwner.Id equals user.Id
                    select new
                    {
                        ExceptionCode = exception.ExceptionCode,
                        ExceptionDescription = exception.ExceptionDesc,
                        ExceptionOwner = user.UserName,
                        User = exceptionitem.user.UserName,
                        Function1 = exceptionitem.function1.SodFunctionCode,
                        FunctionValue1 = exceptionitem.functionitem1.SodRoleDesc,
                        Function2 = exceptionitem.function2.SodFunctionCode,
                        FunctionValue2 = exceptionitem.functionitem2.SodRoleDesc,
                    }).ToList<object>();
        }
        public override long Insert(SODException exception)
        {
            //Harihara 28/2/2014 -> Since i am facing some issue (Dupliacte entries) with inserting this model using base class Save/Add method i am writing my own.
            if (exception.ExceptionItems.Count > 0)
            {
                var expections = exception.ExceptionItems.ToList();
                exception.ExceptionItems.Clear();
                foreach (var sodExceptionItem in expections)
                {
                    User owner = this.CurrentContext.User.Find(sodExceptionItem.user.Id);
                    if (owner != null)
                    {
                        sodExceptionItem.user = owner;
                    }
                    SODFunction function1 = this.CurrentContext.SODFunctions.Find(sodExceptionItem.function1.Id);
                    if (function1 != null)
                    {
                        sodExceptionItem.function1 = function1;
                    }
                    SODFunctionItem functionItem1 = this.CurrentContext.SODFuntionItem.Find(sodExceptionItem.functionitem1.Id);
                    if (functionItem1 != null)
                    {
                        sodExceptionItem.functionitem1 = functionItem1;
                    }
                    SODFunction function2 = this.CurrentContext.SODFunctions.Find(sodExceptionItem.function2.Id);
                    if (function2 != null)
                    {
                        sodExceptionItem.function2 = function2;
                    }
                    SODFunctionItem functionItem2 = this.CurrentContext.SODFuntionItem.Find(sodExceptionItem.functionitem2.Id);
                    if (functionItem2 != null)
                    {
                        sodExceptionItem.functionitem2 = functionItem2;
                    }
                    exception.ExceptionItems.Add(sodExceptionItem);
                    exception.RiskOwner.Roles = null;
                }
            }
            return base.Insert(exception);

        }
        public override void Update(SODException exception)
        {
            //Harihara 28/2/2014 -> Since i am facing some issue (Dupliacte entries) with updating this model using base class Save/Update method i am writing my own.
            var dbSODException = this.GetById(exception.Id, r => r.ExceptionItems);

            var newSodExceptionItems = exception.ExceptionItems.ToList<SODExceptionItem>();
            var existingSodExceptionItems = dbSODException.ExceptionItems.ToList<SODExceptionItem>();
            var addedSodExceptionItems = newSodExceptionItems.Where(item => item.Id == 0);
          
            var deletedSodExceptionItems = dbSODException.ExceptionItems.Except<SODExceptionItem>(newSodExceptionItems, new IdBaseEqualityComparer<SODExceptionItem>());
            var updatedSodExceptionItems = dbSODException.ExceptionItems.Except<SODExceptionItem>(deletedSodExceptionItems, new IdBaseEqualityComparer<SODExceptionItem>());

            // Add Newley Added SODFunction Items
            foreach(SODExceptionItem sodExceptionItem in addedSodExceptionItems)
            {
                User owner = this.CurrentContext.User.Find(sodExceptionItem.user.Id);
                if (owner != null)
                {
                    sodExceptionItem.user = owner;
                }
                SODFunction function1 = this.CurrentContext.SODFunctions.Find(sodExceptionItem.function1.Id);
                if (function1 != null)
                {
                    sodExceptionItem.function1 = function1;
                }
                SODFunctionItem functionItem1 = this.CurrentContext.SODFuntionItem.Find(sodExceptionItem.functionitem1.Id);
                if (functionItem1 != null)
                {
                    sodExceptionItem.functionitem1 = functionItem1;
                }
                SODFunction function2 = this.CurrentContext.SODFunctions.Find(sodExceptionItem.function2.Id);
                if (function2 != null)
                {
                    sodExceptionItem.function2 = function2;
                }
                SODFunctionItem functionItem2 = this.CurrentContext.SODFuntionItem.Find(sodExceptionItem.functionitem2.Id);
                if (functionItem2 != null)
                {
                    sodExceptionItem.functionitem2 = functionItem2;
                }
                this.CurrentContext.SODExceptionItem.Add(sodExceptionItem);
            }

            // Update Existing SODExceptionItems in the List
            foreach (SODExceptionItem t in updatedSodExceptionItems)
            {
                if (t.Id > 0)
                {
                    this.CurrentContext.SODExceptionItem.Attach(t);
                    this.CurrentContext.Entry(t).State = System.Data.Entity.EntityState.Modified;
                }
            }

            // Delete Existing SODExceptionItems from db.
            deletedSodExceptionItems.ToList<SODExceptionItem>().ForEach(t => this.CurrentContext.SODExceptionItem.Remove(t));

            base.Update(exception);

        }
    }
}

