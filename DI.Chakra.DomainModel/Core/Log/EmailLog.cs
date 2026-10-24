using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
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
    public class EmailLog : DomainModelBase<EmailLog, EntityModel.EmailLog>
    {
        public Nullable<long> EmailId { get; set; }
        public string EmailSubject { get; set; }
        public string LogLevel { get; set; }
        public string LogType { get; set; }
        public string Reviewer { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string InnerExceptionMessage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }

        public static IList<EmailLog> GetEmailLogByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression)
        {
            string obj = GetLogParameters(logReferenceType, dtFrom, dtTo, logLevelType);
            List<EntityModel.EmailLog> output = new List<EntityModel.EmailLog>();
            IEnumerable<EntityModel.EmailLog> logs = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value  && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value ).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value  && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value ).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value  && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value ).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "RefernceType":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.CreatedDate);
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value).OrderByDescending(x => x.CreatedDate);
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.CreatedDate);
                        }
                        else
                        {
                            logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.CreatedDate);
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    logs = logs.AsQueryable().Where(FilterExpression);
                }

                if (!string.IsNullOrEmpty(sortExpression))
                {
                    logs = logs.OrderBy(sortExpression);
                }

                if (pageSize != null && pageNumber != null)
                {
                    output = logs.Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList();
                }
                else
                {
                    output = logs.ToList();
                }

                return ORMapList(output);
            }
        }

        public static string GetLogParameters(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType)
        {
            string parameters = (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType != LogLevelType.All && dtFrom.HasValue && dtTo.HasValue ? "ALL&Level" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType == LogLevelType.All && dtFrom.HasValue && dtTo.HasValue ? "ALL" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType != LogLevelType.All && dtFrom.HasValue) ? "RefernceTypeFrom&Level" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType == LogLevelType.All && dtFrom.HasValue) ? "RefernceTypeFrom" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType != LogLevelType.All && dtTo.HasValue) ? "RefernceTypeTo&Level" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType == LogLevelType.All && dtTo.HasValue) ? "RefernceTypeTo" :
                           (dtFrom.HasValue && dtFrom.HasValue && logLevelType != LogLevelType.All) ? "FromTo&Level" :
                           (dtFrom.HasValue && dtFrom.HasValue && logLevelType == LogLevelType.All) ? "FromTo" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType != LogLevelType.All) ? "RefernceType&Level" :
                           (!string.IsNullOrEmpty(logReferenceType.ToString()) && logLevelType == LogLevelType.All) ? "RefernceType" :
                           (dtFrom.HasValue && logLevelType != LogLevelType.All) ? "From&Level" :
                           (dtFrom.HasValue && logLevelType == LogLevelType.All) ? "From" :
                           (dtTo.HasValue && logLevelType != LogLevelType.All) ? "To&Level" :
                           (dtTo.HasValue && logLevelType == LogLevelType.All) ? "To" : ""));

            return parameters;
        }
        

        public static long GetCountByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression)
        {
            string obj = GetLogParameters(logReferenceType, dtFrom, dtTo, logLevelType);
            long logCount = 0;
            IEnumerable<EntityModel.EmailLog> logs;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value  && x.LogLevel == logLevelType.ToString());
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value );
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value  && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value );
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value  && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value );
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.CreatedDate <= dtTo.Value);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x =>  x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceType":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).GetAll();
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate >= dtFrom.Value);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.CreatedDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).GetAll();
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).GetAll();
                        }
                        else
                        {
                            logs = (new RepositoryBase<EntityModel.EmailLog>(dataAccessTransaction).Get(x => x.LogLevel == logLevelType.ToString()));
                            //  .Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Count());
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    logCount = logs.AsQueryable().Where(FilterExpression).Count();
                }
                else
                    logCount = logs.Count();
            }
            return logCount;
        }
    }
}
