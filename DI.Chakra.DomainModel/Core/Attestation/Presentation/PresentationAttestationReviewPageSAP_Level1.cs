using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationAttestationReviewPageSAP_Level1
    {
        public string sapLevel1DetailId { get; set; }

        public string sapLevel1Detail { get; set; }

        public string sapLevel1DetailDesc { get; set; }

        public string sapLevel1DetailType { get; set; }


        public static PresentationAttestationReviewPageSAP_Level1 ORMap(EntityModel.Presentation.PresentationAttestationReviewPageSAP_Level1 presentationAttestationReviewPageSAP_Level1)
        {
            if (presentationAttestationReviewPageSAP_Level1 == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationAttestationReviewPageSAP_Level1, PresentationAttestationReviewPageSAP_Level1>(presentationAttestationReviewPageSAP_Level1);
        }

        public static List<PresentationAttestationReviewPageSAP_Level1> ORMapList(IList<EntityModel.Presentation.PresentationAttestationReviewPageSAP_Level1> presentationAttestationReviewPagesSAP_Level1)
        {
            return presentationAttestationReviewPagesSAP_Level1.Select(ORMap).ToList();
        }

        public static IList<PresentationAttestationReviewPageSAP_Level1> GetByParamEndpointSystemId(long paramEndpointSystemId, IList<SAPTCodeInfo> sapTCode)
        {
                List<EntityModel.SAPTCodeInfo> sapTCodeInfo = new List<EntityModel.SAPTCodeInfo>();
                foreach (SAPTCodeInfo codeInfo in sapTCode)
                {
                    sapTCodeInfo.Add(codeInfo.ORUnMap());
                }
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return PresentationAttestationReviewPageSAP_Level1.ORMapList(new AttestationRepository(dataAccessTransaction).GetByParamEndpointSystemId(paramEndpointSystemId, sapTCodeInfo));
                }   
                
        }

        public static IList<PresentationAttestationReviewPageSAP_Level1> GetByParamSingleRoleCode(string paramSingleRoleCode, long paramEndPointSystemId)
        {
            IList<PresentationAttestationReviewPageSAP_Level1> returnValue = new List<PresentationAttestationReviewPageSAP_Level1>();
                IList<SAPTCodeInfo> sapTCodes = (IList<SAPTCodeInfo>)SAPSingleRoleInfo.GetByEnpointSystemIdIncludingSAPTCodeInfo(paramEndPointSystemId, paramSingleRoleCode);

                if (sapTCodes.Count() > 0)
                {
                    returnValue = PresentationAttestationReviewPageSAP_Level1.GetByParamEndpointSystemId(paramEndPointSystemId, sapTCodes);
                }
                return returnValue;
        }

        public static IList<PresentationAttestationReviewPageSAP_Level1> GetAuthorization(string roleCode, long endPointSystemId)
        {
            IList<PresentationAttestationReviewPageSAP_Level1> returnValue = new List<PresentationAttestationReviewPageSAP_Level1>();


                List<PresentationSAPRoleToAuthorizationReport> RoleToAuthorizationData = PresentationSAPRoleToAuthorizationReport.GetSapRolesToAuthorizationData(roleCode, endPointSystemId);

                if (RoleToAuthorizationData.Count > 0)
                {
                    returnValue = (from Authorization in RoleToAuthorizationData
                                   select new PresentationAttestationReviewPageSAP_Level1
                                   {
                                       sapLevel1DetailId = "Authorization;" + Authorization.AuthCode + ";" + endPointSystemId,
                                       sapLevel1Detail = string.Format("Authorization Code: {0} Object: {1}", Authorization.AuthCode, Authorization.ObjectCode),
                                       sapLevel1DetailDesc = string.Format("Field: {0} From Value: {1}", Authorization.FieldCode, Authorization.FromValue),
                                       sapLevel1DetailType = "Auhtorization"
                                   }).ToList();
                }
            return returnValue;
        }
    }
}
