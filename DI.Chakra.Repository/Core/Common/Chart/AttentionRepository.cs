using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
namespace DI.Chakra.Repository
{
    public class AttentionRepository : RepositoryBase<Attention>
    {
        public AttentionRepository(DataAccessTransaction transaction) : base(transaction) { }

        public IList<Attention> GetAttention(long userId, bool AdminMode)
        {
            IList<Attention> chartData = new List<Attention>();

            try
            {
                var myTask = CurrentContext.Tasks.Count(a => a.Action == Infrastructure.Enums.StatusType.Pending
                && ((a.ActionUserId == userId && (!a.IsGroupTask || a.IsDelegated))
                || (a.IsGroupTask && a.AssignedTo == userId)));
                chartData.Add(new Attention() { Name = "My Tasks", CommandName = "MyTask", Count = myTask });
            }
            catch (Exception ex)
            {
                chartData.Add(new Attention() { Name = "My Tasks", CommandName = "MyTask", Count = 0 });
            }

            try
            {
                var myRequest = CurrentContext.AccessRequest.Count(a => (a.ProgressStatus == Infrastructure.Enums.AccessRequestProgress.WaitingforApproval)
                && a.RequesterId == userId);
                chartData.Add(new Attention() { Name = "My Requests", CommandName = "MyRequest", Count = myRequest });
            }
            catch (Exception ex)
            {
                chartData.Add(new Attention() { Name = "My Requests", CommandName = "MyRequest", Count = 0 });
            }

            try
            {
                var myNotification = CurrentContext.Notifications.Count(a => a.ReceiverId == userId && !a.IsRead);
                chartData.Add(new Attention() { Name = "My Notifications", CommandName = "MyNotification", Count = myNotification });
            }
            catch (Exception ex)
            {
                chartData.Add(new Attention() { Name = "My Notifications", CommandName = "MyNotification", Count = 0 });
            }

            try
            {
                var myDelegation = CurrentContext.TaskDelegations.Count(a => a.ActionUser == userId
                && a.IsActive
                && (a.EndDate.HasValue && DbFunctions.TruncateTime(a.EndDate.Value) > DbFunctions.TruncateTime(DateTime.Now)));
                chartData.Add(new Attention() { Name = "My Delegations", CommandName = "MyDelegation", Count = myDelegation });
            }
            catch (Exception ex)
            {
                chartData.Add(new Attention() { Name = "My Delegations", CommandName = "MyDelegation", Count = 0 });
            }

            if (AdminMode)
            {
                try
                {
                    var task = CurrentContext.Tasks.Count(a => a.Action == Infrastructure.Enums.StatusType.Pending);
                    chartData.Add(new Attention() { Name = "Tasks", CommandName = "Task", Count = task });
                }
                catch (Exception ex)
                {
                    chartData.Add(new Attention() { Name = "Tasks", CommandName = "Task", Count = 0 });
                }

                try
                {
                    var request = CurrentContext.AccessRequest.Count(a => a.ProgressStatus == Infrastructure.Enums.AccessRequestProgress.WaitingforApproval
                   );
                    chartData.Add(new Attention() { Name = "Requests", CommandName = "Request", Count = request });
                }
                catch (Exception ex)
                {
                    chartData.Add(new Attention() { Name = "Requests", CommandName = "Request", Count = 0 });
                }

                try
                {
                    var JAC = CurrentContext.Tasks.Count(a => a.Action == Infrastructure.Enums.StatusType.Pending
                    && a.IsActionOnlyRequiredTask);
                    chartData.Add(new Attention() { Name = "JAC", CommandName = "JAC", Count = JAC });
                }
                catch (Exception ex)
                {
                    chartData.Add(new Attention() { Name = "JAC", CommandName = "JAC", Count = 0 });
                }

            }

            return chartData;
        }
    }
}
