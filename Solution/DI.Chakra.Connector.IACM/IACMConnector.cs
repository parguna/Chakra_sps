#region NameSpace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Data.SqlClient;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using CheckNulls = DI.Chakra.DomainModel.CheckNull;
using DI.Chakra.DBConfigHelper;

#endregion

namespace DI.Chakra.Connector

{
    public class IACMConnector : ConnectorBase
    {
        #region Members

        public EndpointConfiguration iacmConfiguration { get; set; }
        public EndPoint endpoint;
        List<EmployeeEventTrigger> triggers = new List<EmployeeEventTrigger>();

        #endregion

        #region Core

        public IACMConnector(EndPoint _endpoint, AccessRequest currentRequest = null)
        {
            this.iacmConfiguration = EndpointConfiguration.GetByEndpointSystemId(_endpoint.Id);
            EndPoint = _endpoint;
            this.CurrentRequest = currentRequest;
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                IACM.IMProcessor processor = new IACM.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor.IacmConfiguration = this.iacmConfiguration;
                return processor;
            }
        }

        #endregion

        #region Discovery

        public override bool Connect()
        {
            SqlConnection sqlCon = null;
            try
            {

                string IADsConStr = "";
                string UserName = "";
                string InstanceName = "";
                string Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));
                string DataSource = Configuration(EndpointKey.IPAddress);

                if (Convert.ToInt32(Configuration(EndpointKey.UserName)) != 0)
                {
                    UserName = Configuration(EndpointKey.UserName);
                }
                else
                {
                    UserName = "sa";
                }

                if (!String.IsNullOrEmpty(Configuration(EndpointKey.InstanceName)))
                {
                    InstanceName = Configuration(EndpointKey.InstanceName);
                }

                IADsConStr = string.Format("Data Source ={0}; Initial Catalog = {1}; Persist Security Info = True; User Id = {2}; Password = {3};" +
                    "Connect Timeout = 1000; providerName =System.Data.SqlClient", DataSource, InstanceName, Configuration(EndpointKey.UserName), Password);

                sqlCon = new SqlConnection(IADsConStr);

                WriteDiscoveryLog(LogLevelType.Exception, string.Format("IDAS connection opened"));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("IDAS connection failed to open"), ex);
                return false;
            }
            finally
            {
                sqlCon.Close();
                sqlCon.Dispose();
            }
            return true;
        }
        private string Configuration(EndpointKey Key)
        {
            var config = iacmConfiguration.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }
        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override void GetData()
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

