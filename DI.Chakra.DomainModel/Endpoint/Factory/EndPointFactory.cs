using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.DomainModel
{
    public class EndPointFactory
    {
        public static IEndPointFactory GetEndPoint(EndpointType type)
        {
            IEndPointFactory endpoint = null;
            switch (type)
            {
                case EndpointType.ActiveDirectory:
                    endpoint = new ADSEndPoint();
                    break;
                case EndpointType.SAP:
                case EndpointType.SRM:
                    endpoint = new SAPEndPoint();
                    break;

                case EndpointType.SAPEnterprisePortal:
                    endpoint = new SAPEPEndpoint();
                    break;

                case EndpointType.InhouseSystems:
                    endpoint = new InhouseEndPoint();
                    break;

                case EndpointType.IDMS:
                    endpoint = new IDASEndpoint();
              
                    break;
                case EndpointType.GEMS:
                    endpoint = new GEMSEndPoint();

                    break;
                case EndpointType.NPAC:
                    endpoint = new NPACEndPoint();

                    break;
                case EndpointType.Office365:
                   endpoint = new Office365EndPoint();

                    break;
                case EndpointType.SSIDS:
                    endpoint = new SSIDSEndPoint();

                    break;
                case EndpointType.OneCard:
                    endpoint = new OneEndPoint();
                    break;
                case EndpointType.VPN:
                    endpoint = new VPNEndPoint();
                    break;
                case EndpointType.MicrosoftTeams:
                    endpoint = new SkypeForBusinessEndPoint();
                    break;
                case EndpointType.EAM_NOVA:
                    endpoint = new EamNovaEndpoint();
                    break;
                case EndpointType.SAPHANA:
                    endpoint = new SAPHANAEndPoint();
                    break;
                default:
                    throw new ApplicationException("Endpoint type : " + type.ToString() + " not supported.");
            }

            return endpoint;
        }
    }
}
