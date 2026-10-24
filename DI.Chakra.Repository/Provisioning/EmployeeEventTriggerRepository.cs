using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Logger;

namespace DI.Chakra.Repository
{
    public class EmployeeEventTriggerRepository : RepositoryBase<EmployeeEventTrigger>
    {
        public EmployeeEventTriggerRepository(DataAccessTransaction transaction) : base(transaction) { }


        public List<PresentationEmployeeEventTrigger> GetByEventType(EmployeeEvents eventType)
        {
            return (from empevent in CurrentContext.EmployeeEventTriggers
                    join user in CurrentContext.User
                    on empevent.UserId equals user.Id
                    where empevent.EventType == eventType
                    select new PresentationEmployeeEventTrigger
                    {
                        TriggerId = empevent.Id,
                        UserId = empevent.UserId,
                        UserName = user.DisplayName,
                        EmployeeId = user.EmployeeId,
                        EventType = empevent.EventType,
                        EffectiveDate = empevent.EffectiveDate,
                        OldEffectiveDate = empevent.OldEffectiveDate.HasValue ? empevent.OldEffectiveDate.ToString() : "",
                        IsRequestCreated = empevent.IsRequestCreated,
                        IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                        IsActive = empevent.IsActive,
                        Comments = empevent.Comments,
                        ActualEventDate = empevent.ActualEventDate
                    }).ToList();
        }

        public List<PresentationEmployeeEventTrigger> GetAll()
        {
            var resullt = (from empevent in CurrentContext.EmployeeEventTriggers
                           join user in CurrentContext.User
                           on empevent.UserId equals user.Id
                           //join attach in CurrentContext.TriggerAttachments.Where(x => x.IsAmendAttachment == false)
                           //on empevent.Id equals attach.TriggerId into HasAttachments
                           //from attach in HasAttachments.DefaultIfEmpty()
                           //join amendAttach in CurrentContext.TriggerAttachments.Where(x => x.IsAmendAttachment)
                           //on empevent.Id equals amendAttach.TriggerId into HasAmendAttachments
                           //from amendAttach in HasAmendAttachments.DefaultIfEmpty()
                           select new PresentationEmployeeEventTrigger
                           {
                               TriggerId = empevent.Id,
                               UserId = empevent.UserId,
                               UserName = user.DisplayName,
                               ADID = user.UserName,
                               EmployeeId = user.EmployeeId,
                               EventType = empevent.EventType,
                               EffectiveDate = empevent.EffectiveDate,
                               OldEffectiveDate = empevent.OldEffectiveDate.HasValue ? empevent.OldEffectiveDate.ToString() : "",
                               IsRequestCreated = empevent.IsRequestCreated,
                               IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                               IsActive = empevent.IsActive,
                               CreatedDate = empevent.CreatedDate,
                               CreatedBy = empevent.CreatedBy,
                               ActualEventDate = empevent.ActualEventDate
                           }).OrderByDescending(t => t.EffectiveDate).ToList();

            return resullt.Distinct().ToList();
        }

        public PresentationGridPagedData<PresentationEmployeeEventTrigger> GetLazyLoadEmployeeEventTrigger(EmployeeEvents eventType, int startRowIndex, int maximumRows, string filterExpression, string sortExpression, bool includeProcessed = false)
        {
            PresentationGridPagedData<PresentationEmployeeEventTrigger> result = new PresentationGridPagedData<PresentationEmployeeEventTrigger>();
            var userEvent = (from empevent in CurrentContext.EmployeeEventTriggers
                             join user in CurrentContext.User
                             on empevent.UserId equals user.Id
                             where (  ((byte)eventType == ((byte)EmployeeEvents.None)) ? true: ((byte)empevent.EventType == ((byte)eventType)))
                             
                             select new PresentationEmployeeEventTrigger
                             {
                                 TriggerId = empevent.Id,
                                 UserId = empevent.UserId,
                                 UserName = user.DisplayName,
                                 ADID = user.UserName,
                                 EmployeeId = user.EmployeeId,
                                 EventType = empevent.EventType,
                                 EffectiveDate = empevent.EffectiveDate,
                                 OldEffectiveDate = empevent.OldEffectiveDate.HasValue ? empevent.OldEffectiveDate.ToString() : "",
                                 IsRequestCreated = empevent.IsRequestCreated,
                                 IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                                 IsActive = empevent.IsActive,
                                 CreatedDate = empevent.CreatedDate,
                                 CreatedBy = empevent.CreatedBy,
                                 Comments = empevent.Comments,
                                 LastModifiedBy = empevent.LastModifiedBy,
                                 LastModifiedDate = empevent.LastModifiedDate,
                                 ActualEventDate = empevent.ActualEventDate
                             });

            if (includeProcessed == false)
            {               
                userEvent = userEvent.Where(t => t.IsRequestCreated == false && t.IsActive==true);
            }
             

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                userEvent = userEvent.Where(filterExpression);
            }
            result.TotalCount = userEvent.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                userEvent = userEvent.OrderBy(sortExpression);
            }
            else
            {
                userEvent = userEvent.OrderByDescending(t => t.EffectiveDate);
            }

            if (maximumRows == -1)
                result.Data = userEvent.ToList();
            else
                result.Data = userEvent.Skip(startRowIndex).Take(maximumRows).ToList();

            return result;
        }

        public List<EmployeeEventTrigger> GetEmployeeEventTriggersJob(int days, EmployeeEvents employeeEvent)
        {
            DateTime now = DateTime.Now.Date;
            List<EmployeeEventTrigger> EmployeeEventTrigList = (from empevent in CurrentContext.EmployeeEventTriggers
                                                                join user in CurrentContext.User
                                                                on empevent.UserId equals user.Id
                                                                where empevent.EventType == employeeEvent && empevent.IsRequestCreated == false &&
                                                                empevent.IsMailSent == false && empevent.IsActive &&
                                                                DbFunctions.DiffDays(now, empevent.EffectiveDate) <= days &&
                                                                (empevent.OldEffectiveDate.HasValue && empevent.OldEffectiveDate.Value > now)
                                                                select empevent).ToList();

            return EmployeeEventTrigList;
        }
        public PresentationEmployeeEventTrigger GetById(long triggerId)
        {
            return (from empevent in CurrentContext.EmployeeEventTriggers
                    join user in CurrentContext.User
                    on empevent.UserId equals user.Id
                    where empevent.Id == triggerId
                    select new PresentationEmployeeEventTrigger
                    {
                        TriggerId = empevent.Id,
                        UserId = empevent.UserId,
                        UserName = user.DisplayName,
                        EmployeeId = user.EmployeeId,
                        EventType = empevent.EventType,
                        EffectiveDate = empevent.EffectiveDate,
                        OldEffectiveDate = empevent.OldEffectiveDate.HasValue ? empevent.OldEffectiveDate.ToString() : "",
                        IsRequestCreated = empevent.IsRequestCreated,
                        IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                        IsActive = empevent.IsActive,
                        CreatedDate = empevent.CreatedDate,
                        CreatedBy = empevent.CreatedBy,
                        UserDetails = empevent.UserDetails,
                        OldUserDetails = empevent.OldUserDetails,
                        ActualEventDate = empevent.ActualEventDate
                    }).FirstOrDefault();
        }

        public List<EmployeeEventTrigger> GetOUChangeUsers(bool forAdminCheck)
        {
            try
            {

                if (forAdminCheck)
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsAdminCheckRequired == true && trig.IsRequestCreated == false && trig.IsActive == true &&
                                  trig.EventType == EmployeeEvents.OUChange && trig.EffectiveDate <= DateTime.Now).ToList();
                }
                else
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsAdminCheckRequired == false && trig.IsRequestCreated == false && trig.IsActive == true &&
                                  trig.EventType == EmployeeEvents.OUChange && trig.EffectiveDate <= DateTime.Now).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetReOrgUsers(bool forAdminCheck)
        {
            try
            { 
                if (forAdminCheck)
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsAdminCheckRequired == true && trig.IsActive == true &&
                                trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate <= DateTime.Now).ToList();
                }
                else
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsActive == true &&
                          trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate <= DateTime.Now).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetOnBoardUsers(bool forAdminCheck, EmployeeEvents employeeEvent = EmployeeEvents.NewJoinee)
        {
            try
            {
                if (forAdminCheck)
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsAdminCheckRequired == true && trig.IsActive == true &&
                                trig.EventType == employeeEvent && trig.EffectiveDate <= DateTime.Now).ToList();
                }
                else
                {
                    return CurrentContext.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsActive == true &&
                          trig.EventType == employeeEvent && trig.EffectiveDate <= DateTime.Now).ToList();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public EmployeeEventTrigger GetByEmployeeIdAndEventType(string employeeId, EmployeeEvents employeeEvents, DateTime? effectiveDate=null)
        {
            List<EmployeeEvents> CanReUseEmployeeEventIfExist = new List<EmployeeEvents> { EmployeeEvents.NewJoinee, EmployeeEvents.Rehire, EmployeeEvents.Resignation };

            var query = (from empEvent in this.CurrentContext.EmployeeEventTriggers
                         join user in this.CurrentContext.User on empEvent.UserId equals user.Id
                         where empEvent.EventType ==  employeeEvents && empEvent.IsRequestCreated == false &&
                         empEvent.IsActive && user.EmployeeId == employeeId
                         select empEvent);

            if (effectiveDate.HasValue && !CanReUseEmployeeEventIfExist.Contains(employeeEvents))
            {
                query = query.Where(e => e.EffectiveDate ==  effectiveDate).Select(e=>e);
            }

            //DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, query.ToTraceQuery());
                         
            return query.FirstOrDefault();

        }


        public List<EmployeeEventTrigger> GetAllByEmployeeIdAndEventType(string employeeId, EmployeeEvents employeeEvents, DateTime effectiveDate)
        { 

            var query = (from empEvent in this.CurrentContext.EmployeeEventTriggers
                         join user in this.CurrentContext.User on empEvent.UserId equals user.Id
                         where empEvent.EventType == employeeEvents && empEvent.IsRequestCreated == false &&
                         empEvent.IsActive && user.EmployeeId == employeeId && empEvent.EffectiveDate == effectiveDate
                         select empEvent); 

            return query.ToList();

        }


        public List<EmployeeEventTrigger> GetAllEventsByEmployeeIdAndEventType(string employeeId, EmployeeEvents employeeEvents)
        {
            
            var query = (from empEvent in this.CurrentContext.EmployeeEventTriggers
                         join user in this.CurrentContext.User on empEvent.UserId equals user.Id
                         where  user.EmployeeId == employeeId && empEvent.EventType == employeeEvents
                         select empEvent); 
            //DILog.DiscoveryLogger.WriteLog(LogLevelType.DebugStep, query.ToTraceQuery());

            return query.ToList();

        }
        public EmployeeEventTrigger GetOldEventByEmployeeIdAndEventType(string employeeId, EmployeeEvents employeeEvents, DateTime effectiveDate)
        {

            var query = (from empEvent in this.CurrentContext.EmployeeEventTriggers
                         join user in this.CurrentContext.User on empEvent.UserId equals user.Id
                         where empEvent.EventType == employeeEvents && user.EmployeeId == employeeId && empEvent.EffectiveDate == effectiveDate
                         select empEvent);

            return query.FirstOrDefault();

        }


    }
}
