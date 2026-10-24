using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class UILogRepository : RepositoryBase<UILog>
    {
        public UILogRepository(DataAccessTransaction transaction) : base(transaction) { }

        public PresentationGridPagedData<UILog> GetUILog(int? pageSize, int? pageNumber, string FilterExpression, string sortExpression)
        {
            PresentationGridPagedData<UILog> result = new PresentationGridPagedData<UILog>();
            IEnumerable<EntityModel.UILog> query = null;
             query = (from log in CurrentContext.UILogs.AsEnumerable()
                         join usr in CurrentContext.User.AsEnumerable() on log.UserId equals usr.Id into uiLogs
                         from usrLog in uiLogs.DefaultIfEmpty()
                      where log.LogLevel != "ApplicationStarted" && (log.Message != null || log.ExceptionMessage != null)
                      select new UILog
                         {
                             UserId = log.UserId,
                             UserName = usrLog != null ? usrLog.UserName : log.UserName,
                             Message = log.Message,
                             LogDate = log.LogDate,
                             CreatedDate = log.CreatedDate,
                             CreatedBy = log.CreatedBy,
                             ExceptionMessage = log.ExceptionMessage,
                             ExceptionSource = log.ExceptionSource,
                             InnerExceptionMessage = log.InnerExceptionMessage,
                             StackTrace = log.StackTrace,
                             IP = log.IP,
                             LogLevel = log.LogLevel
                         });
            if (!string.IsNullOrEmpty(FilterExpression))
            {
                query = query.Where(FilterExpression);
            }

            if (!string.IsNullOrEmpty(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.LogDate);
            }
            result.TotalCount = query.Count();

            if (pageSize != null && pageNumber != null)
            {
                result.Data = query.Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList();
            }
            else
            {
                result.Data = query.ToList();
            }
            return result;
        }
    }
}
