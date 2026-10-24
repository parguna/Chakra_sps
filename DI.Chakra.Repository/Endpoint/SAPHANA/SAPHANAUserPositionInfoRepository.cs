using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class SAPHANAUserPositionInfoRepository : RepositoryBase<SAPHANAUserinfo>
    {
        public SAPHANAUserPositionInfoRepository(DataAccessTransaction transaction) : base(transaction) { }


        public Dictionary<long, string> GetPositionTextByUser(long endpointUserId, long endpointSystemId)
        {
            var ep = CurrentContext.EndPoint.FirstOrDefault(i=>i.Id == endpointSystemId);
            var entitlementRecons = from accRecon in CurrentContext.AccountReconcilations
                                    join
                                    entRecon in CurrentContext.EntitlementReconcilations on accRecon.Id equals entRecon.AccountReconcilationId
                                    where accRecon.EndpointSystemId == endpointSystemId &&
                                          entRecon.EntitlementType == (int)EntitlementType.Position &&
                                          entRecon.EndpointUserId == endpointUserId && !entRecon.IsDeleted
                                    select entRecon;


            try
            {

                if (ep.ApplicationType == ApplicationType.MSSL)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPHANAXLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position
                            where hrp001Text.EndpointSystemId == endpointSystemId && entitlementRecon.IsDeleted == false
                            select hrp001Text)
                            .Select(position => new { key = position.Id, value = position.PositionText })
                            .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                
                else if (ep.ApplicationType == ApplicationType.EBS)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPHANAXLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position
                            where hrp001Text.EndpointSystemId == endpointSystemId && entitlementRecon.IsDeleted == false
                            select hrp001Text)
                           .Select(position => new { key = position.Id, value = position.PositionText })
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                
            }
            catch
            {
                throw;
            }

            return null;
        }

        public List<string> GetPositionsByUser(string logonName, long endpointSystemId)
        {

            return (from hrp001 in CurrentContext.SAPHRP1001Entries
                    where hrp001.EndpointSystemId == endpointSystemId && hrp001.RelatedObjectCode == logonName &&
                            hrp001.RelatedObjectType == "US" && hrp001.ActualObjectType == "S"
                    select hrp001)
                            .Select(pos => pos.ActualObjectCode).Distinct().ToList();

        }
    }
}
