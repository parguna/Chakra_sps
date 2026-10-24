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
    public class ServiceLog : DomainModelBase<ServiceLog, EntityModel.ServiceLog>, ILogger
    {
        public ServiceLog()
        {
            LogDate = DateTime.Now;
        }
        public ServiceLog(Exception ex)
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
        public long? ReferenceId { get; set; }
        public LogReferenceType ReferenceType { get; set; }
        public string LogLevel { get; set; }
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


        public static IList<ServiceLog> GetAllOrderByLogDate(bool initial)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (initial == true)
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).Take(100).ToList());

                else
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).ToList());
            }
        }
        public static IList<ServiceLog> GetByLogLevelOrderByLogDate(bool initial, LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (initial == true)
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Take(100).ToList());

                else
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).ToList());
            }

        }
        public static IList<ServiceLog> GetByLogleveltype(LogLevelType logleveltype, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList());
                }
                else
                {
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList());
                }

            }
        }

        public static IList<ServiceLog> GetByLogleveltype(LogLevelType logleveltype)
        {
            IEnumerable<EntityModel.ServiceLog> logs = GetByLogleveltypes(logleveltype);
            return ORMapList(logs.ToList());
        }

        public static IEnumerable<EntityModel.ServiceLog> GetByLogleveltypes(LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate);
                }
                else
                {
                    return new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate);
                }

            }
        }

        public static long GetCountByLogleveltype(LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return (new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Count());
                }
                else
                {
                    return (new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Count(x => x.LogLevel == logleveltype.ToString()));
                      //  .Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Count());
                }
            }
        }

        public static string GetByAccessRequestId(long accessRequestId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                string accessRequestString = "Access Request" + " " + accessRequestId;
                return (new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(m=>m.Message.Contains(accessRequestString)).OrderByDescending(m=>m.LastModifiedDate).Select(m=>m.Message).LastOrDefault());
            }
        }
        
        public static IList<ServiceLog> GetByReferenceType(LogReferenceType logReferenceType, LogLevelType logLevelType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logLevelType == LogLevelType.All)
                {
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType).OrderByDescending(x => x.LogDate).ToList());
                }
                else
                {
                    return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate).ToList());
                }
            }
        }
        public static IList<ServiceLog> GetByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression)
        {
            string obj = GetLogParameters(logReferenceType, dtFrom, dtTo, logLevelType);
            List<EntityModel.ServiceLog> output = new List<EntityModel.ServiceLog>();
            IEnumerable<EntityModel.ServiceLog> logs = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.ReferenceType == logReferenceType).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType).OrderByDescending(x => x.LogDate);
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceType":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType).OrderByDescending(x => x.LogDate);
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate);
                        }
                        else
                        {
                            logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        }
                        break;
                }

                if(!string.IsNullOrEmpty(FilterExpression))
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
                    output = logs.Take(100).ToList();
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

        public static IList<ServiceLog> GetLoginDetail()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(a => a.ReferenceType == LogReferenceType.Authentication).ToList());
            }
        }

        public static long GetCountByLogDate(LogReferenceType? logReferenceType, DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression)
        {
            string obj = GetLogParameters(logReferenceType, dtFrom, dtTo, logLevelType);
            long logCount = 0;
            IEnumerable<EntityModel.ServiceLog> logs;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString());
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType);
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.ReferenceType == logReferenceType);
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.ReferenceType == logReferenceType);
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceType":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.ReferenceType == logReferenceType);
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate >= dtFrom.Value);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value);
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).GetAll();
                        }
                        else
                        {
                            logs = (new RepositoryBase<EntityModel.ServiceLog>(dataAccessTransaction).Get(x => x.LogLevel == logLevelType.ToString()));
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
