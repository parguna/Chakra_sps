using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class TriggerDA
    {
        public IList<EmployeeEventGridData> GetEmployeeEventTriggerByEventType(EmployeeEvents eventType)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    var list = (from empevent in oIACMDBContext.EmployeeEventTriggers
                                join user in oIACMDBContext.Users
                                on empevent.UserId equals user.UserId
                                where empevent.EventType == eventType
                                select new EmployeeEventGridData
                                {
                                    TriggerId = empevent.EmployeeEventTriggerId,
                                    UserId = empevent.UserId,
                                    UserName = user.DisplayName,
                                    EventType = empevent.EventType,
                                    EffectiveDate = empevent.EffectiveDate,
                                    IsRequestCreated = empevent.IsRequestCreated,
                                    IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                                    IsRejoiningUser = empevent.IsRejoiningUser,
                                    IsActive = empevent.IsActive,
                                    CreatedDate = empevent.CreatedDate,
                                    CreatedBy = empevent.CreatedBy
                                }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IList<EmployeeEventGridData> GetEmployeeEventTriggers()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return (from empevent in oIACMDBContext.EmployeeEventTriggers
                            join user in oIACMDBContext.Users
                            on empevent.UserId equals user.UserId
                            select new EmployeeEventGridData
                            {
                                TriggerId = empevent.EmployeeEventTriggerId,
                                UserId = empevent.UserId,
                                UserName = user.DisplayName,
                                EventType = empevent.EventType,
                                EffectiveDate = empevent.EffectiveDate,
                                IsRequestCreated = empevent.IsRequestCreated,
                                IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                                IsRejoiningUser = empevent.IsRejoiningUser,
                                IsActive = empevent.IsActive,
                                CreatedDate = empevent.CreatedDate,
                                CreatedBy = empevent.CreatedBy
                            }).ToList();
                }
            }
            catch
            {
                throw;
            }
        }

        public EmployeeEventGridData GetEmployeeEventTriggerDataById(int triggerId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return (from empevent in oIACMDBContext.EmployeeEventTriggers
                            join user in oIACMDBContext.Users
                            on empevent.UserId equals user.UserId
                            where empevent.EmployeeEventTriggerId == triggerId
                            select new EmployeeEventGridData
                            {
                                TriggerId = empevent.EmployeeEventTriggerId,
                                UserId = empevent.UserId,
                                UserName = user.DisplayName,
                                EventType = empevent.EventType,
                                EffectiveDate = empevent.EffectiveDate,
                                IsRequestCreated = empevent.IsRequestCreated,
                                IsAdminCheckRequired = empevent.IsAdminCheckRequired,
                                IsRejoiningUser = empevent.IsRejoiningUser,
                                IsActive = empevent.IsActive,
                                CreatedDate = empevent.CreatedDate,
                                CreatedBy = empevent.CreatedBy,
                                UserDetails = empevent.UserDetails,
                                OldUserDetails = empevent.OldUserDetails
                            }).FirstOrDefault();
                }
            }
            catch
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetOUChangeUsers(bool forAdminCheck)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (forAdminCheck)
                    {
                        return _context.EmployeeEventTriggers.Where(trig => trig.IsAdminCheckRequired && trig.IsRequestCreated == false && trig.IsActive &&
                                      trig.EventType == EmployeeEvents.OUChange && trig.EffectiveDate <= DateTime.Now).ToList();
                    }
                    return _context.EmployeeEventTriggers.Where(trig => trig.IsAdminCheckRequired == false && trig.IsRequestCreated == false && trig.IsActive &&
                                                                        trig.EventType == EmployeeEvents.OUChange && trig.EffectiveDate <= DateTime.Now).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetReOrgUsers()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate <= DateTime.Now).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetUsersForPropagation()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.EmployeeEventTriggers.Where(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                trig.EventType == EmployeeEvents.Propagation && trig.EffectiveDate <= DateTime.Now).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CreateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    oIACMDBContext.EmployeeEventTriggers.Add(trigger);
                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var entry = _context.Entry(trigger);
                    if (entry.State == EntityState.Detached)
                    {
                        var set = _context.Set<EmployeeEventTrigger>();
                        EmployeeEventTrigger attached = set.Find(trigger.EmployeeEventTriggerId);

                        if (attached != null)
                        {
                            var attachedEntry = _context.Entry(attached);
                            attachedEntry.CurrentValues.SetValues(trigger);
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while updating  the User", ex);
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(int userId, EmployeeEvents evnt)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false && item.IsActive);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(int userId, EmployeeEvents evnt, DateTime date)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsActive && item.CreatedDate >= date);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteTriggerById(int triggerId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EmployeeEventTrigger trigger = _context.EmployeeEventTriggers.FirstOrDefault(item => item.EmployeeEventTriggerId == triggerId);
                    if (trigger != null)
                    {
                        _context.EmployeeEventTriggers.Remove(trigger);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTriggerById(int triggerId)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.EmployeeEventTriggers.FirstOrDefault(item => item.EmployeeEventTriggerId == triggerId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
