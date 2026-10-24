using System.Web.Http;
using ChakraService.Models;
using System.Collections.Generic;

namespace ChakraService.Controllers
{
    public class TaskController : ApiController
    {
        #region Members
        private AccountControllerHelper Helper = new AccountControllerHelper();
        private TaskControllerHelper Taskhelper = new TaskControllerHelper();
        #endregion

        #region Methods
        [HttpGet]
        [HttpPost]
        public IEnumerable<TaskResponse> GetTasksCount(string UserName)
        {
            if(string.IsNullOrEmpty(UserName))
                return null;

            return Taskhelper.GetTasksCount(UserName);
        }

        [HttpGet]
        [HttpPost]
        public IEnumerable<TaskDTO> GetTasks(string UserName)
        {
            if (string.IsNullOrEmpty(UserName))
                return null;

            return Taskhelper.GetTasks(UserName);
        }
        [HttpGet]
        [HttpPost]
        public ProcessResponse ApproveTask(ApproveTaskRequest Request)
        {
            if (Request.TaskId == -1)
                return null;

            return Taskhelper.ApproveTask(Request);
        }
        #endregion
    }
}
