using AutoMapper;
using DI.Chakra.DomainModel.Core;
using DI.Chakra.Infrastructure;
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
    public class PresentationEmployeeEventTrigger : EmployeeEventTrigger
    {
        public long TriggerId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string ADID { get; set; }
        public string EmployeeId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public bool HasAttachment { get; set; }
        public bool HasAmendAttachment { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
        public string EmployeeEvent
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(EventType);
            }
        }

        public static PresentationGridPagedData<PresentationEmployeeEventTrigger> GetLazyLoadEmployeeEventTrigger(EmployeeEvents eventType, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, bool includeProcessed)
        {
            var result = new PresentationGridPagedData<PresentationEmployeeEventTrigger>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var dbResult = new EmployeeEventTriggerRepository(dataAccessTransaction).GetLazyLoadEmployeeEventTrigger(eventType, startRowIndex, maximumRows, filterExpression, sortExpression,includeProcessed);
                result.Data = ORMapList(dbResult.Data);
                result.TotalCount = dbResult.TotalCount;
                return result;
            }
        }

        public string RequestCreated
        {
            get
            {
                return IsRequestCreated ? "Yes" : "No";
            }
        }

        public string AdminCheckRequired
        {
            get
            {
                return IsAdminCheckRequired ? "Yes" : "No";
            }
        }

        public string Active
        {
            get
            {
                return IsActive ? "Yes" : "No";
            }
        }

        public static List<PresentationEmployeeEventTrigger> GetByEventType(EmployeeEvents eventType)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EmployeeEventTriggerRepository(dataAccessTransaction).GetByEventType(eventType));
            }
        }

        public static List<PresentationEmployeeEventTrigger> GetAll()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EmployeeEventTriggerRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationEmployeeEventTrigger GetById(long triggerId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new EmployeeEventTriggerRepository(dataAccessTransaction).GetById(triggerId));
            }
        }

        public static PresentationEmployeeEventTrigger ORMap(EntityModel.Presentation.PresentationEmployeeEventTrigger presentationEntitleReconEntityModel)
        {
            if (presentationEntitleReconEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationEmployeeEventTrigger, PresentationEmployeeEventTrigger>(presentationEntitleReconEntityModel);
        }

        public static List<PresentationEmployeeEventTrigger> ORMapList(IList<EntityModel.Presentation.PresentationEmployeeEventTrigger> presentationEntitleReconEntityModels)
        {
            return presentationEntitleReconEntityModels.Select(ORMap).ToList();
        }




    }
}
