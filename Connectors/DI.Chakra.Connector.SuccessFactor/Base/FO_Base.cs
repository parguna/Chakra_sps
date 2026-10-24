using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Base
{
  public class FO_Base
    {
        protected readonly IdmsLogger idmsLogger;
        protected readonly string serviceUser;
        protected readonly SFOData.EntityContainer entityContainer;
        protected readonly DateTime MaxDate = new DateTime(9999, 12, 31);
        protected readonly DateTime CurrentDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        protected int pageSize = 1000;
        protected int page = 0;
         
        public FO_Base(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser)
        {
            idmsLogger = _idmsLogger;
            serviceUser = _serviceUser;
            entityContainer = _entityContainer; 
            if(entityContainer != null)
            {
                entityContainer.Timeout = MasterConfigHelper.SFAPITimeoutDuration;
            }
        }
    }
}
