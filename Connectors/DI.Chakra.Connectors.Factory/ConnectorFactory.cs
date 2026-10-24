
using DI.Chakra.Connector;
using DI.Chakra.Connector.SAPHANA;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector
{
    public class ConnectorFactory
    {
        public static ConnectorBase GetConnector(EndPoint endpoint, AccessRequest currentRequest, IdmsLogger logger = null)
        {
            ConnectorBase connector = null;

            if (endpoint.EndPointSystemType == null)
                endpoint = EndPoint.GetByIdIncludingEndPointSystemType(endpoint.Id);

            if (logger == null)
            {
                logger = new ProvisioningLogger(currentRequest?.Id, endpoint.Id, endpoint.EndpointSystemTypeId, endpoint.DisplayName);
            }

            if (MasterConfigHelper.SkipConnectorCall && (EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.IDMS &&
                 (EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.EAM_NOVA && (EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.SuccessFactor && (EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.MicrosoftTeams && (EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName != EndpointType.Office365)
            {
                return new GeneralConnector(endpoint, currentRequest, logger);
            }

            switch ((EndpointType)endpoint.EndPointSystemType.EndpointSystemTypeName)
            {
                case EndpointType.SuccessFactor:
                    connector = new SuccessFactorConnector(endpoint, logger);
                    break;

                case EndpointType.ActiveDirectory:
                    connector = new ActiveDirectoryConnector(endpoint, currentRequest, logger);
                    break;

                case EndpointType.SAP:

                    if (endpoint.IsTrustedSource)
                    {
                        connector = new SAPHRMSConnector(endpoint, logger);
                    }
                    else
                    {
                        connector = new SAPV3.SAPV3Connector(endpoint, currentRequest, logger);
                    }
                    break;


                case EndpointType.SAPEnterprisePortal:
                    connector = new SAPEPConnector(endpoint, currentRequest, logger);
                    break;

                case EndpointType.VPN:
                    connector = new VPNConnector(endpoint, currentRequest, logger);
                    break;

                case EndpointType.InhouseSystems:
                    connector = new GeneralConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.IDMS:
                    connector = new IACMConnector(endpoint, logger, currentRequest);
                    break;
                case EndpointType.GEMS:
                    connector = new GEMSConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.NPAC:
                    connector = new NPACConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.Office365:
                    connector = new Office365Connector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.OneCard:
                    connector = new LenelConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SSIDS:
                    connector = new SSIDSConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.EAM_NOVA:
                    connector = new EamNovaConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.MicrosoftTeams:
                    connector = new SkypeForBusinessConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SRM:
                    connector = new SRMConnector(endpoint, currentRequest, logger);
                    break;
                case EndpointType.SAPHANA:
                    connector = new SAPHANAConnector(endpoint, currentRequest, logger);
                    break;
                default:
                    break;
            }
            return connector;
        }
    }
}