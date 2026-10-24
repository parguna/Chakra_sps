using AutoMapper;
using DI.Chakra.DomainModel.Core;
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
    public class PresentationAccountReconcilationReport : AccountReconcilation
    {
        private DataAccessTransaction dataAccessTransaction;

        public PresentationAccountReconcilationReport(DataAccessTransaction dataAccessTransaction)
        {
            this.dataAccessTransaction = dataAccessTransaction;
        }
        public PresentationAccountReconcilationReport()
        {

        }
        public string EndPointName { get; set; }
        public long ReconciliationId { get; set; }
        public long EndpointSystemId { get; set; }
        public long EndpointUserId { get; set; }
        public string AccountName { get; set; }
        public string DisplayName { get; set; }

        public ReconcilationType ReconcilationType { get; set; }
        public string Entitlement { get; set; }

        public string EntitlementType { get; set; }

        public DateTime CreatedDate { get; set; }
        public long? UserId { get; set; }
        public int? Version { get; set; }
        public ExceptionTypes ExceptionType { get; set; }
        public string IsDeleted { get; set; }
        public string UserDisplayName { get; set; }
        public bool IsLocked { get; set; }
        public bool IsDeletedBoolean { get; set; }
        public AccountType AccountType { get; set; }
        public string OffboardingAction { get; set; }

        public static List<PresentationAccountReconcilationReport> GetExceptions(int endpointSystemId, byte exceptionType, ReconcilationType reconcilationType, bool? isResolved = null, bool? isDeleted = null)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if ((ExceptionTypes)exceptionType == ExceptionTypes.Added)
                {
                    return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetAddedExceptions(endpointSystemId, reconcilationType, isResolved, isDeleted));
                }
                else if ((ExceptionTypes)exceptionType == ExceptionTypes.Attribute)
                {
                    return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetAttributeExceptions(endpointSystemId, isResolved, isDeleted));
                }
                else
                {
                    return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetRemovedExceptions(endpointSystemId, reconcilationType, isResolved, isDeleted));
                }
            }
        }



        public static List<PresentationAccountReconcilationReport> GetResolvedReconcilations(DateTime? FromDate, DateTime? ToDate, ResolvedStatus status, bool IsResolved)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {

                return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetResolvedReconcilations(FromDate, ToDate, status, IsResolved));
            }
        }


        public static PresentationAccountReconcilationReport ORMap(EntityModel.Presentation.PresentationAccountReconcilationReport presentationAccReconEntityModel)
        {
            if (presentationAccReconEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationAccountReconcilationReport, PresentationAccountReconcilationReport>(presentationAccReconEntityModel);
        }
        public static List<PresentationAccountReconcilationReport> ORMapList(IList<EntityModel.Presentation.PresentationAccountReconcilationReport> presentationAccReconEntityModels)
        {
            return presentationAccReconEntityModels.Select(ORMap).ToList();
        }

        public static List<PresentationAccountReconcilationReport> GetEntitlementDetailsForAccessRequest(long accessRequestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetEntitlementDetailsForAccessRequest(accessRequestId));
            }
        }

        public static List<PresentationAccountReconcilationReport> GetBySystem(long systemId, Nullable<AccountType> accType, MappingType mappingTypes = MappingType.None, bool employeeHasMoreAccounts = false, bool eagerLoadAllProperties = true, bool isLocked = false, bool isDeleted = false)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new AccountReconcilationRepository(dataAccessTransaction).GetBySystem(systemId, accType, mappingTypes, employeeHasMoreAccounts, eagerLoadAllProperties, isLocked, isDeleted));
            }
        }

        public static PresentationGridPagedData<PresentationAccountReconcilationReport> GetLazyLoadAllSystem(int startRowIndex, int maximumRows, string filterExpression, string sortExpression, long systemId, Nullable<AccountType> accType, long mappingTypes)
        {
            PresentationGridPagedData<PresentationAccountReconcilationReport> result = new PresentationGridPagedData<PresentationAccountReconcilationReport>();
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new AccountReconcilationRepository(dataAccessTransaction).GetLazyLoadAllSystem(startRowIndex, maximumRows, filterExpression, sortExpression, systemId, accType, mappingTypes);
                result.TotalCount = dbResult.TotalCount;
                result.Data = ORMapList(dbResult.Data);
                return result;
            }
        }

    }
}
