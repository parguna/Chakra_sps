using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class SAPUserPositionInfoRepository : RepositoryBase<SAPUserinfo>
    {
        public SAPUserPositionInfoRepository(DataAccessTransaction transaction) : base(transaction) { }


        public Dictionary<long, string> GetPositionTextByUser(long endpointUserId, long endpointSystemId, int applicationType)
        {
            var entitlementRecons = from accRecon in CurrentContext.AccountReconcilations
                                    join
                                    entRecon in CurrentContext.EntitlementReconcilations on accRecon.Id equals entRecon.AccountReconcilationId
                                    where accRecon.EndpointSystemId == endpointSystemId &&
                                          entRecon.EntitlementType == (int)EntitlementType.Position &&
                                          entRecon.EndpointUserId == endpointUserId && !entRecon.IsDeleted
                                    //&& entRecon.ValidFromDate.HasValue && 
                                    //entRecon.ValidFromDate <= DateTime.Now &&
                                    //entRecon.ValidToDate.HasValue &&
                                    //entRecon.ValidToDate >= DateTime.Now
                                    select entRecon;


            try
            {

                if (applicationType == (int)ApplicationType.MSSL_ISU)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPXLMSSLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position_MSP
                            where entitlementRecon.IsDeleted == false
                            select hrp001Text)
                            .Select(position => new { key = position.Id, value = position.PositionText })
                            .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                else if (applicationType == (int)ApplicationType.MSSL_CRM)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPXLMSSLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position_MCP
                            where entitlementRecon.IsDeleted == false
                            select hrp001Text)
                           .Select(position => new { key = position.Id, value = position.PositionText })
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);

                }
                else if (applicationType == (int)ApplicationType.EBS_ISU)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPEBSXLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position_ISP
                            where entitlementRecon.IsDeleted == false
                            select hrp001Text)
                           .Select(position => new { key = position.Id, value = position.PositionText })
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);

                }
                else if (applicationType == (int)ApplicationType.EBS_CRM)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPEBSXLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position_CRP
                            where entitlementRecon.IsDeleted == false
                            select hrp001Text)
                           .Select(position => new { key = position.Id, value = position.PositionText })
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                else if (applicationType == (int)ApplicationType.EBS)
                {
                    return (from entitlementRecon in entitlementRecons
                            join hrp001Text in CurrentContext.SAPHANAXLPositionsMatrices on entitlementRecon.Entitlement equals hrp001Text.Position
                            where hrp001Text.EndpointSystemId == endpointSystemId && entitlementRecon.IsDeleted == false
                            select hrp001Text)
                           .Select(position => new { key = position.Id, value = position.PositionText })
                           .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
                }
                else if (applicationType == (int)ApplicationType.MSSL)
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
