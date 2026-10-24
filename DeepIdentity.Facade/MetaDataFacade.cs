using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;

namespace DeepIdentity.Facade
{
    public class MetaDataFacade
    {
        MetaDataDA _metaDataDA = null;

        public MetaDataFacade()
        {
            _metaDataDA = new MetaDataDA();
        }

       
        #region ProvisioningFormDefaultGeneralData
        public IList<DefaultProvisioningFormGeneralData> loadDefaultProvisioningGeneralAttributes(int endpointTypeId)
        {
            try
            {
                return _metaDataDA.loadDefaultProvisioningGeneralAttributes(endpointTypeId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
