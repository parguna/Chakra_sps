using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationNotificationRepository : RepositoryBase<PresentationNotification>
    {
        public PresentationNotificationRepository(DataAccessTransaction transaction) : base(transaction) { }
        public List<PresentationNotification> GetUnreadNotifications(long userId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where !item.IsRead && item.ReceiverId == userId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).OrderByDescending(t => t.Id).ToList();
        }

        public List<PresentationNotification> GetReadNotifications(long userId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.IsRead && item.ReceiverId == userId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).OrderByDescending(t => t.Id).ToList();
        }

        public List<PresentationNotification> GetRepliedNotifications(long userId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.ParentId != null && item.SenderId == userId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).OrderByDescending(t => t.Id).ToList();
        }

        public List<PresentationNotification> GetAllNotifications(long userId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where (item.SenderId == userId || item.ReceiverId == userId)
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).OrderByDescending(t => t.Id).ToList();
        }

        public List<PresentationNotification> GetByTaskId(long taskId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.SenderTaskId == taskId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        SenderTaskId = item.SenderTaskId,
                        ParentId = item.ParentId,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).OrderByDescending(t => t.Id).ToList();
        }

        public List<PresentationNotification> GetParentNotification(long requestId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.RequestId == requestId && item.ParentId == null
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        SenderTaskId = item.SenderTaskId,
                        ReceiverTaskId = item.ReceiverTaskId,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).ToList();
        }

        public PresentationNotification GetPresentationNotificationById(long notificationId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.Id == notificationId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        SenderTaskId = item.SenderTaskId,
                        ReceiverTaskId = item.ReceiverTaskId,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,
                        IsRead=item.IsRead,
                        IsReplied =item.IsReplied
                    }).FirstOrDefault();
        }

        public PresentationNotification GetByParentId(long notificationId)
        {
            return (from item in CurrentContext.Notifications.Include("NotificationAttachment")
                    join usr in CurrentContext.User
                    on item.SenderId equals usr.Id
                    join usrr in CurrentContext.User
                    on item.ReceiverId equals usrr.Id
                    where item.ParentId == notificationId
                    select new PresentationNotification
                    {
                        Id = item.Id,
                        ParentId = item.ParentId,
                        SenderName = usr.DisplayName,
                        ReceiverName = usrr.DisplayName,
                        SenderTaskId = item.SenderTaskId,
                        ReceiverTaskId = item.ReceiverTaskId,
                        Subject = item.Subject,
                        Content = item.Content,
                        NotificationAttachment = item.NotificationAttachment,
                        RequestId = item.RequestId,

                    }).FirstOrDefault();
        }

        public List<PresentationRequestActionUsers> GetActionUsersByRequestId(long requestId)
        {
            return ((from item in CurrentContext.AccessRequest.Include("Requester")
                    where item.Id == requestId
                    select new PresentationRequestActionUsers
                    {
                        TaskId = null,
                        RequestId = item.Id,
                        ActionUser = item.Requester.DisplayName,
                        ActionUserId = item.RequesterId
                    }).Union(
                       from item in CurrentContext.Tasks
                       join usr in CurrentContext.User
                       on item.AssignedTo equals usr.Id into usrtmp
                       from ut in usrtmp.DefaultIfEmpty()
                       where item.AccessRequestId == requestId 
                       select new PresentationRequestActionUsers
                       {
                           TaskId = item.Id,
                           RequestId = item.AccessRequestId,
                           ActionUser = item.ActionUser.DisplayName ==null ? ut.DisplayName : item.ActionUser.DisplayName,
                           ActionUserId = item.ActionUserId ==null ? item.AssignedTo : item.ActionUserId
                       })).Distinct().ToList();
        }

        public List<PresentationRequestActionUsers> GetActionUsersByBulkRequestId(long requestId)
        {
            return ((from item in CurrentContext.BulkRequests 
                     join Requester in CurrentContext.User on item.RequestedBy equals Requester.Id
                     where item.Id == requestId
                     select new PresentationRequestActionUsers
                     {
                         TaskId = null,
                         RequestId = item.Id,
                         ActionUser = Requester.DisplayName,
                         ActionUserId = item.RequestedBy
                     }).Union(
                       from item in CurrentContext.Tasks
                       join usr in CurrentContext.User
                       on item.AssignedTo equals usr.Id into usrtmp
                       from ut in usrtmp.DefaultIfEmpty()
                       where item.BulkRequestId == requestId
                       select new PresentationRequestActionUsers
                       {
                           TaskId = item.Id,
                           RequestId = item.AccessRequestId,
                           ActionUser = item.ActionUser.DisplayName == null ? ut.DisplayName : item.ActionUser.DisplayName,
                           ActionUserId = item.ActionUserId == null ? item.AssignedTo : item.ActionUserId
                       })).Distinct().ToList();
        }
    }
}
