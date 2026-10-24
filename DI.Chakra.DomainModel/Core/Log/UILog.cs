using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class UILog : DomainModelBase<UILog, EntityModel.UILog>, ILogger
    {
        public UILog()
        {
            LogDate = DateTime.Now;
        }

        public UILog(Exception ex)
        {
            if (null != ex)
            {
                ExceptionMessage = ex.Message;
                ExceptionSource = ex.Source;
                StackTrace = ex.StackTrace;
                if (null != ex.InnerException)
                    InnerExceptionMessage = string.Format(@"Source={0}\nStackTrace={1}\nMessage={2}", ex.InnerException.Source,
                        ex.InnerException.StackTrace, ex.InnerException.Message);
            }
            LogDate = DateTime.Now;
        }

        public Nullable<long> UserId { get; set; }

        public string LogLevel { get; set; }

        public string UserName { get; set; }

        public string IP { get; set; }

        public string Message { get; set; }

        public string ExceptionMessage { get; set; }

        public string StackTrace { get; set; }

        public string ExceptionSource { get; set; }

        public string InnerExceptionMessage { get; set; }

        public DateTime LogDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static IList<UILog> GetLessThanGivenDateTime(DateTime logDate)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.UILog>(dataAccessTransaction).Get(UIL => UIL.LogDate < logDate && UIL.LogLevel != "ApplicationStarted").ToList());
            }
        }

        public static PresentationGridPagedData<UILog> GetAllOrderByLogDate(int? pageSize, int? pageNumber, string FilterExpression, string sortExpression)
        {
            PresentationGridPagedData<UILog> endResult = new PresentationGridPagedData<UILog>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                var result = new UILogRepository(dataAccessTransaction).GetUILog(pageSize,pageNumber,FilterExpression,sortExpression);
                if(result != null && result.Data != null)
                {
                    endResult.Data = ORMapList(result.Data);
                    endResult.TotalCount = result.TotalCount;
                }
            }
            return endResult;
        }
        //Commented below code. Implemented server side pagenation.
        [Obsolete]
        public static long GetAllCount(int? pagesize, int? pagenumber, string FilterExpression, string sortExpression)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return 0;
                //var result = new UILogRepository(dataAccessTransaction).GetUILog(pagesize, pagenumber, FilterExpression, sortExpression, true);
                //return result.TotalCount;
            }
        }

        public static void SaveUILog(UILog uiLog, string currentUser)
        {

            if (uiLog.LogLevel == "ApplicationStarted")
            {
                AppSettingsHelper.IsFirstTimeDBExecute = true;
                return;
            }

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                uiLog.Save(dataAccessTransaction);
                dataAccessTransaction.Commit();
            }
        }
    }
}
