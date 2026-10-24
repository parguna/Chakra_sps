using System;
using System.Linq;
using DI.Chakra.Facade;
using ChakraService.Models;
using DI.Chakra.Infrastructure;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Workflow.ServiceHelpers;
using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel.Presentation;

namespace ChakraService
{
    public class TaskControllerHelper
    {
        #region Members
        private TaskFacade TFacade = new TaskFacade();
        private AccessRequestFacade ARFacade = new AccessRequestFacade();
        #endregion
        
        #region Methods
        public IList<TaskResponse> GetTasksCount(string UserName)
        {
            var TaskList = TFacade.GetAllTasks().Where(lq => lq.UserName == UserName).ToList();
            List<TaskResponse> Returnvalue = new List<TaskResponse>();
            
            foreach (RequestType item in Enum.GetValues(typeof(RequestType)))
            {
                var description = UtilityMethods.GetEnumAttribute<Description>(item);
                if (item != RequestType.None)
                {
                    var Count = TaskList.Where(lq => lq.RequestType == description).Count();
                    Returnvalue.Add(new TaskResponse() { Type = description, Count = Count});
                }
            }
            return Returnvalue;
        }

        public IList<TaskDTO> GetTasks(string UserName)
        {
            var TaskList = TFacade.GetTasksByStatus(StatusType.Pending).Where(lq => lq.UserName == UserName).ToList();
            List<TaskDTO> Returnvalue = new List<TaskDTO>();
            TaskList.ForEach(lq =>
                {
                    Returnvalue.Add(new TaskDTO() { Id = lq.TaskId, Name = string.IsNullOrEmpty(lq.Type) ? lq.TaskName : lq.TaskName + " " + lq.Type });
                });
            return Returnvalue;
        }

        public ProcessResponse ApproveTask(ApproveTaskRequest Request)
        {
            ProcessResponse Returnvalue = new ProcessResponse();
            try
            {
                //var criteria = ARFacade.GetWorkFlowCriteria(Request.Action == "Approve" ? AccessType.Approve : AccessType.Revoke, Request.UserName, Request.TaskId, Request.Comment, MasterConfigHelper.AccountCreatedNotificationTO);
                var criteria = PresentationWorkflowCriteriaBase.GetWorkFlowCriteria(Request.Action == "Approve" ? AccessType.Approve : AccessType.Revoke, Request.UserName, Request.TaskId, Request.Comment, MasterConfigHelper.AccountCreatedNotificationTO);
                ServiceAccessStrategy _serviceAccessStrategy = new ServiceAccessStrategy(criteria);
                _serviceAccessStrategy.AccessWorkflowService();
                Returnvalue.Message = Request.Action + "Action Success";
                Returnvalue.Status = "Success";
            }
            catch
            {
                Returnvalue.Message = Request.Action + "Action Failed";
                Returnvalue.Status = "Failed";
            }
            return Returnvalue;
        }
        #endregion
    }
}