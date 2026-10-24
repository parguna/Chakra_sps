using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class MetaDataRepository
    {
        public MetaDataRepository(DataAccessTransaction transaction) { }

        IACMDBContext CurrentContext = new IACMDBContext();

        public IList<MetaData> GetAccessRequestProgress()
        {

            return (from accreq in CurrentContext.AccessRequestProgress
                    select new MetaData
                    {
                        Id = (int)accreq.Id,
                        Description = accreq.Description,
                        IsActive = accreq.IsActive
                    }).ToList();
        }

        public IList<MetaData> GetAccessRequestSource()
        {
            return (from accreq in CurrentContext.AccessRequestSources
                    select new MetaData
                    {
                        Id = (int)accreq.Id,
                        Description = accreq.Description,
                        IsActive = accreq.IsActive
                    }).ToList();
        }

        public IList<MetaData> GetAccessRequestStatus()
        {
            return (from accreq in CurrentContext.AccessRequestStatus
                    select new MetaData
                    {
                        Id = (int)accreq.Id,
                        Description = accreq.Description,
                        IsActive = accreq.IsActive
                    }).ToList();
        }

        public IList<MetaData> GetEndpointsystemtype()
        {
            return (from endsystype in CurrentContext.EndpointSystemTypes
                    select new MetaData
                    {
                        Id = (int)endsystype.Id,
                        Description = endsystype.Description,
                        IsActive = (bool)endsystype.IsActive
                    }).ToList();              
        }
    }
}
