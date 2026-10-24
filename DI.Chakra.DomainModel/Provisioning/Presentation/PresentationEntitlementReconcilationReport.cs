using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationEntitlementReconcilationReport : EntitlementReconcilation
    {
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public int EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public string EntitlementDescription { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
        public string PerformedBy { get; set; }
        public string EntitlementTypeValue
        {
            get
            {
                return ((EntitlementType)this.EntitlementType).ToString();
            }
        }
        public static List<PresentationEntitlementReconcilationReport> GetExceptions(int endpointSystemId, ExceptionTypes exceptionType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                EndPoint endPoint = EndPoint.GetById(endpointSystemId);
                long endPointSystemTypeId = (long)endPoint.EndpointSystemTypeId;

                if (exceptionType == ExceptionTypes.Added)
                {
                    if (endPointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetAddedExceptionActiveDirectory(endpointSystemId).ToList());
                    }
                    
                    else if (endPointSystemTypeId == (int)EndpointType.SAP)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetAddedExceptionSAP(endpointSystemId).ToList());
                    }
                    else if (endPointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetAddedExceptionSAPEP(endpointSystemId).ToList());
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (endPointSystemTypeId == (int)EndpointType.ActiveDirectory)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetRemovedExceptionActiveDirectory(endpointSystemId).ToList());
                    }
                  
                    else if (endPointSystemTypeId == (int)EndpointType.SAP)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetRemovedExceptionSAP(endpointSystemId).ToList());
                    }
                    else if (endPointSystemTypeId == (int)EndpointType.SAPEnterprisePortal)
                    {
                        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetRemovedExceptionSAPEP(endpointSystemId).ToList());
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        //public static List<EntitlementReconcilation> GetAccountReconcilationListForEndpoint(DI.Chakra.EntityModel.EndPoint _endPoint)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new EntitlementReconcilationRepository(dataAccessTransaction).GetEntitilementReconcilationListForEndpoint(_endPoint));
        //    }
        //}

        public static PresentationEntitlementReconcilationReport ORMap(EntityModel.Presentation.PresentationEntitlementReconcilationReport presentationEntitleReconEntityModel)
        {
            if (presentationEntitleReconEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationEntitlementReconcilationReport, PresentationEntitlementReconcilationReport>(presentationEntitleReconEntityModel);
        }
        public static List<PresentationEntitlementReconcilationReport> ORMapList(IList<EntityModel.Presentation.PresentationEntitlementReconcilationReport> presentationEntitleReconEntityModels)
        {
            return presentationEntitleReconEntityModels.Select(ORMap).ToList();
        }

    }
}
