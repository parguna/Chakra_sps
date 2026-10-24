using System;
using DI.Chakra.EntityModel; 
using DI.Chakra.Transaction; 
using System.Collections.Generic;
using System.Linq; 
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums; 
using System.Linq.Dynamic;

namespace DI.Chakra.Repository 
{
    public class DiscoveryLogRepository : RepositoryBase<DiscoveryLog>
    {
        public DiscoveryLogRepository(DataAccessTransaction transaction) : base(transaction) { }



        public long GetCountByLogDate(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression, long? endpointSystemId, DiscoveryObjectType objectType)
        {
            var query = getQuery(dtFrom, dtTo, logLevelType, FilterExpression, sortExpression, endpointSystemId, objectType);



            return query.Count();

        }

        public List<DiscoveryLog> GetLogData(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression, long? endpointSystemId, DiscoveryObjectType objectType)
        {
            var query = getQuery(dtFrom, dtTo, logLevelType, FilterExpression, sortExpression, endpointSystemId, objectType);
            return query.ToList();

        }

        public   IList<DiscoveryLog> GetDiscoveryLogByLogDate(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, int? pageSize, int? pageNumber, string FilterExpression, string sortExpression, long? endpointSystemId, DiscoveryObjectType objectType)
        {
            List<EntityModel.DiscoveryLog> output = new List<EntityModel.DiscoveryLog>();
            IEnumerable<EntityModel.DiscoveryLog> logs = null;

            var query = getQuery(dtFrom, dtTo, logLevelType, FilterExpression, sortExpression, endpointSystemId, objectType);



            if (!string.IsNullOrEmpty(FilterExpression))
            {
                logs = query.Where(FilterExpression);
            }
            else
                logs = query;

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

            return output;

        }
        IQueryable<DiscoveryLog> getQuery(DateTime? dtFrom, DateTime? dtTo, LogLevelType logLevelType, string FilterExpression, string sortExpression,  long? endpointSystemId, DiscoveryObjectType objectType)
        {
            var query = this.CurrentContext.DiscoveryLog.Select(d => d);

            if (dtFrom.HasValue)
            {
                query = query.Where(d => d.LogDate >= dtFrom.Value).Select(d => d);
            }

            if (dtTo.HasValue)
            {
                query = query.Where(d => d.LogDate <= dtTo.Value).Select(d => d);
            }


            if (logLevelType != LogLevelType.All)
            {
                query = query.Where(d => d.LogLevel == logLevelType.ToString()).Select(d => d);
            }

            if(endpointSystemId.HasValue && endpointSystemId.Value > 0)
            {
                query = query.Where(d => d.EndpointSystemId == endpointSystemId.Value).Select(d => d);
            }

            if (objectType != DiscoveryObjectType.None)
            {
                query = query.Where(d => d.EndpointSystemId == endpointSystemId.Value).Select(d => d);
            }


            return query;
        }
    }

}

