using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel 
{
   public  class ProvisioningLog : DomainModelBase<ProvisioningLog, EntityModel.ProvisioningLog>, ILogger
    {
        public ProvisioningLog()
        {
            LogDate = DateTime.Now;
        }
        public ProvisioningLog(Exception ex)
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
        public long? AccessRequestId { get; set; }
        public string LogLevel { get; set; }
        public long? EndpointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public long? EndpointSystemId { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string InnerExceptionMessage { get; set; }
        public DateTime LogDate { get; set; }
        public string CreatedBy { get; set; }


        public static IList<ProvisioningLog> GetAllOrderByLogDate(bool initial)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (initial == true)
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).Take(100).ToList());

                else
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).ToList());
            }
        }
        public static IList<ProvisioningLog> GetByLogLevelOrderByLogDate(bool initial, LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (initial == true)
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Take(100).ToList());

                else
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).ToList());
            }
        }

        public static IList<ProvisioningLog> GetByLogleveltype(LogLevelType logleveltype, int pageSize, int pageNumber)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList());
                }
                else
                {
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize).ToList());
                }

            }
        }

        public static IList<ProvisioningLog> GetByLogleveltype(LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).GetAll().OrderByDescending(x => x.LogDate).ToList());
                }
                else
                {
                    return ORMapList(new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).ToList());
                }

            }
        }

        public static long GetCountByLogleveltype(LogLevelType logleveltype)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if (logleveltype == LogLevelType.All)
                {
                    return (new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Count());
                }
                else
                {
                    return (new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Count(x => x.LogLevel == logleveltype.ToString()));
                    //  .Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Count());
                }
            }
        }

         

        public static IList<ProvisioningLog> GetByLogDate(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression, long? RequestId)
        {
            string obj = GetLogParameters(dtFrom, dtTo, logLevelType);
            List<EntityModel.ProvisioningLog> output = new List<EntityModel.ProvisioningLog>();
            IEnumerable<EntityModel.ProvisioningLog> logs = null;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value).OrderByDescending(x => x.LogDate);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value).OrderByDescending(x => x.LogDate);
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId)).OrderByDescending(x => x.LogDate);
                        }
                        else
                        {
                            logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogLevel == logLevelType.ToString()).OrderByDescending(x => x.LogDate);
                        }
                        break;
                }

                //if (!string.IsNullOrEmpty(FilterExpression))
                //{
                //    logs = logs.AsQueryable().Where(FilterExpression);
                //}

                //if (!string.IsNullOrEmpty(sortExpression))
                //{
                //    logs = logs.OrderBy(sortExpression);
                //}

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

        public static string GetLogParameters(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType)
        {
            string parameters = (logLevelType != LogLevelType.All && dtFrom.HasValue && dtTo.HasValue ? "ALL&Level" :
                           (logLevelType == LogLevelType.All && dtFrom.HasValue && dtTo.HasValue ? "ALL" :
                           (logLevelType != LogLevelType.All && dtFrom.HasValue) ? "RefernceTypeFrom&Level" :
                           (logLevelType == LogLevelType.All && dtFrom.HasValue) ? "RefernceTypeFrom" :
                           (logLevelType != LogLevelType.All && dtTo.HasValue) ? "RefernceTypeTo&Level" :
                           (logLevelType == LogLevelType.All && dtTo.HasValue) ? "RefernceTypeTo" :
                           (dtFrom.HasValue && dtFrom.HasValue && logLevelType != LogLevelType.All) ? "FromTo&Level" :
                           (dtFrom.HasValue && dtFrom.HasValue && logLevelType == LogLevelType.All) ? "FromTo" :
                           (logLevelType != LogLevelType.All) ? "RefernceType&Level" :
                           (logLevelType == LogLevelType.All) ? "RefernceType" :
                           (dtFrom.HasValue && logLevelType != LogLevelType.All) ? "From&Level" :
                           (dtFrom.HasValue && logLevelType == LogLevelType.All) ? "From" :
                           (dtTo.HasValue && logLevelType != LogLevelType.All) ? "To&Level" :
                           (dtTo.HasValue && logLevelType == LogLevelType.All) ? "To" : ""));

            return parameters;
        }

        public static long GetCountByLogDate(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression, long? RequestId)
        {
            string obj = GetLogParameters(dtFrom, dtTo, logLevelType);
            long logCount = 0;
            IEnumerable<EntityModel.ProvisioningLog> logs;

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                switch (obj)
                {
                    case "ALL&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x =>(RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "ALL":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value);
                        break;

                    case "RefernceTypeFrom&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeFrom":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value);
                        break;

                    case "RefernceTypeTo&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId)   && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "RefernceTypeTo":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value);
                        break;

                    case "FromTo&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "FromTo":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogDate <= dtTo.Value);
                        break;

                    case "RefernceType&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogLevel == logLevelType.ToString());
                        break;

                    case "From&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "From":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate >= dtFrom.Value);
                        break;

                    case "To&Level":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => x.LogDate <= dtTo.Value && x.LogLevel == logLevelType.ToString());
                        break;

                    case "To":
                        logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogDate <= dtTo.Value);
                        break;

                    default:
                        if (logLevelType == LogLevelType.All)
                        {
                            logs = new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId));
                        }
                        else
                        {
                            logs = (new RepositoryBase<EntityModel.ProvisioningLog>(dataAccessTransaction).Get(x => (RequestId == null || x.AccessRequestId == RequestId) && x.LogLevel == logLevelType.ToString()));
                            //  .Get(x => x.LogLevel == logleveltype.ToString()).OrderByDescending(x => x.LogDate).Count());
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(FilterExpression))
                {
                    logCount = logs.AsQueryable().Count();
                }
                else
                    logCount = logs.Count();
            }
            return logCount;
        }

    }
}
