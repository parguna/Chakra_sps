using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("EmployeeEventTrigger")]
    public class EmployeeEventTriggerController : BaseController<DomainModel.EmployeeEventTrigger, EntityModel.EmployeeEventTrigger, Dto.EmployeeEventTrigger>
    {
        [HttpGet]
        [Route("CreatePropagationTrigger")]
        public bool CreatePropagationTrigger(long userId, string createdBy)
        {
            DomainModel.EmployeeEventTrigger.CreatePropagationTrigger(userId, createdBy);
            return true;
        }
        [HttpGet]
        [Route("GetTriggerToExecute")]
        public List<EmployeeEventTrigger> GetTriggerToExecute(long userId, EmployeeEvents evnt)
        {
            return DtoMapList(DomainModel.EmployeeEventTrigger.GetTriggerToExecute(userId, evnt));
        }

        [HttpGet]
        [Route("GetByUserAndEvent")]
        public EmployeeEventTrigger GetByUserAndEvent(long userId, EmployeeEvents? evnt, DateTime? date)
        {
            return DtoMap(DomainModel.EmployeeEventTrigger.GetByUserAndEvent(userId, evnt, date));
        }

        [HttpGet]
        [Route("GetByUserAndEventIncludingInactive")]
        public EmployeeEventTrigger GetByUserAndEventIncludingInactive(long userId, EmployeeEvents? evnt)
        {
            return DtoMap(DomainModel.EmployeeEventTrigger.GetByUserAndEventIncludingInactive(userId, evnt));
        }

        [HttpGet]
        [Route("CreateEventTrigger")]
        public bool CreateEventTrigger(long userId, EmployeeEvents evnt, DateTime effectiveDate, string oldOrgCode, string newOrgCode, string newDesignationCode, DateTime? terminationDate, string companyId, string superVisorId, string employeeType, string costcenter)
        {
            DateTime? terminationdatevalue = (terminationDate.HasValue ? terminationDate : null);
            return DomainModel.EmployeeEventTrigger.CreateEventTrigger(userId, evnt, effectiveDate, oldOrgCode, newOrgCode, newDesignationCode, User.Identity.Name, companyId, superVisorId, employeeType, terminationdatevalue, costcenter);
        }
        [HttpPost]
        [Route("CreateEmployeeEventTrigger")]
        public bool CreateEmployeeEventTrigger(EmployeeEventTrigger eventTrigger)
        {
            return DomainModel.EmployeeEventTrigger.CreateEmployeeEventTrigger(DtoUnMap(eventTrigger));
        }

        [HttpPost]
        [Route("UdpateEventTrigger")]
        public bool UdpateEventTrigger(EmployeeEventTrigger eventTrigger, string currentUser)
        {
            return DomainModel.EmployeeEventTrigger.UpdateEventTrigger(DtoUnMap(eventTrigger), currentUser);
        }

        [HttpGet]
        [Route("UdpateResignationEventTrigger")]
        public bool UdpateResignationEventTrigger(long userId, string currentUser)
        {
            return DomainModel.EmployeeEventTrigger.UdpateResignationEventTrigger(userId, currentUser);
        }
        [HttpGet]
        [Route("GetEmployeeEventTriggerWaitingForApproval")]
        public Dto.EmployeeEventTrigger GetEmployeeEventTriggerWaitingForApproval(long userId, EmployeeEvents evnt)
        {
            return DtoMap(DomainModel.EmployeeEventTrigger.GetEmployeeEventTriggerWaitingForApproval(userId, evnt));
        }
    }
}
