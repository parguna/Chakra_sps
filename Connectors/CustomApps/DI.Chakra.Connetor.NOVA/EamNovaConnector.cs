 
 
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class EamNovaConnector : ConnectorBase
    {
        readonly EamNovaHelper _novaHelper;
        readonly string _currentuser; 
        public EamNovaConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger) : base(logger)
        {           
            this.Endpoint = endpoint;
            _novaHelper = new EamNovaHelper(NovaMatrixType.All, endpoint, this.Process, logger);             
            this.CurrentRequest = currentRequest;
            this.EndpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            _currentuser = _novaHelper.Currentuser;
        }
     
      
        public override bool Connect()
        {
            //since it will be discovered data from FLINS, FLINSBI and AD data
            return true;
        }

        public override bool ChangePassword(string userName, string newPassword, long DIUserId, bool isReactivation)
        {
            //since it uses from FLINS Account
            return true;
        }

        public override void Disconnect()
        {
            return;
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            return true;
            
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            return true;
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            return true;  
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            return true;
        }


        public override void GetData()
        {
            Dictionary<long, string> dic = null;
            try
            {
                _novaHelper.SetProcess(this.Process);
                dic = _novaHelper.DiscoverUsers();
                _novaHelper.UpdateEntitlementRecon(dic);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dic?.Clear();
                dic = null;
                GC.Collect();
            }
        }

        public override bool UndoOffBoardEvent(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            _novaHelper.GetSpecificUsers(this.CurrentRequest.UserId);
            return base.UndoOffBoardEvent(_userInfo, fields);
        }


    }
}
