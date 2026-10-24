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
    public class PresentationNotification : Notification
    {
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public long? RequesterID { get; set; }
        public static List<PresentationNotification> GetByUserIdStatus(long userId, NotificationStatus status)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (status == NotificationStatus.UnRead)
                    return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetUnreadNotifications(userId));
                else if (status == NotificationStatus.Read)
                    return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetReadNotifications(userId));
                else if (status == NotificationStatus.Replied)
                    return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetRepliedNotifications(userId));
                else
                    return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetAllNotifications(userId));
            }
        }

        public static List<PresentationNotification> GetByTaskId(long taskId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetByTaskId(taskId));
            }
        }

        public static List<PresentationNotification> GetByRequestId(long requestId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new PresentationNotificationRepository(dataAccessTransaction).GetParentNotification(requestId));
            }
        }

        public static List<PresentationNotification> GetReplyChainById(long notificationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                PresentationNotification notify = ORMap(new PresentationNotificationRepository(dataAccessTransaction).GetPresentationNotificationById(notificationId));
                List<PresentationNotification> notification = new List<PresentationNotification>();
                notification.Add(notify);
                if (notify.ParentId == null)
                    return notification;
                else {
                    GetNotificationParent(notify, ref notification);
                    return notification.OrderByDescending(n => n.Id).ToList();
                }
            }
        }

        private static void GetNotificationParent(PresentationNotification notify, ref List<PresentationNotification> notification)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (notify.ParentId == null)
                    return;
                else
                {
                    PresentationNotification parent = ORMap(new PresentationNotificationRepository(dataAccessTransaction).GetPresentationNotificationById((long)notify.ParentId));
                    notification.Add(parent);
                    GetNotificationParent(parent, ref notification);
                }

            }
        }


        public static List<PresentationNotification> GetNotificationsById(long notificationId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                PresentationNotification notify = ORMap(new PresentationNotificationRepository(dataAccessTransaction).GetPresentationNotificationById(notificationId));
                List<PresentationNotification> notification = new List<PresentationNotification>();
                GetNotificationReply(notify, ref notification);
                return notification.OrderByDescending(n => n.Id).ToList();
            }
        }

        private static void GetNotificationReply(PresentationNotification notify, ref List<PresentationNotification> notification)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                PresentationNotification reply = ORMap(new PresentationNotificationRepository(dataAccessTransaction).GetByParentId((long)notify.Id));
                if (reply != null)
                {
                    notification.Add(reply);
                    GetNotificationReply(reply, ref notification);
                }
                else return;
            }
        }


        public static PresentationNotification ORMap(EntityModel.Presentation.PresentationNotification presentationNotificationEntityModel)
        {
            if (presentationNotificationEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationNotification, PresentationNotification>(presentationNotificationEntityModel);
        }

        public static List<PresentationNotification> ORMapList(IList<EntityModel.Presentation.PresentationNotification> presentationNotificationEntityModel)
        {
            return presentationNotificationEntityModel.Select(ORMap).ToList();
        }

    }
}
