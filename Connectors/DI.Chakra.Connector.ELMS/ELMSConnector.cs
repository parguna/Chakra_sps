using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.Logger;
using System.Data;
using System.IO;

using System.Globalization;
namespace DI.Chakra.Connector
{
    public class ELMSConnector : ConnectorBase
    {
        #region Members
        private DomainModel.EndPoint _endpoint;
        private EndpointConfiguration _elmsConfiguration = null;
        #endregion
        #region Configuration
        public ELMSConnector(EndPoint endpoint, AccessRequest currentRequest)
        {
            this._endpoint = endpoint;
            this.CurrentRequest = currentRequest;
            var elmsConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            elmsConfiguration.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.ELMSConfiguration = elmsConfiguration;
        }
        public EndpointConfiguration ELMSConfiguration
        {
            get { return _elmsConfiguration; }
            set { this._elmsConfiguration = value; }
        }
        private string GetConfiguration(EndpointKey Key)
        {
            var config = ELMSConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                ELMS.IMProcessor processor = new ELMS.IMProcessor();
                processor._endpoint = this.EndPoint;
                processor._elmsConfiguration = this._elmsConfiguration;
                return processor;
            }
        }
        public override bool Connect()
        {
            return false;
            // throw new NotImplementedException();
        }

        public override void Disconnect()
        {
            //throw new NotImplementedException();
        }

        public override void GetData()
        {
            //throw new NotImplementedException();
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            return false;
           // throw new NotImplementedException();
        }
    }
}
#endregion

