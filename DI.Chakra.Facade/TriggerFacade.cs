using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Facade
{
    public class TriggerFacade
    {
        TriggerDA triggerDA = new TriggerDA();
       

        public IList<EmployeeEventGridData> GetEmployeeEventTriggerByEventType(EmployeeEvents eventType)
        {
            try
            {
                return triggerDA.GetEmployeeEventTriggerByEventType(eventType);
            }
            catch
            {
                throw;
            }
        }

        public IList<EmployeeEventGridData> GetEmployeeEventTriggers()
        {
            try
            {
                return triggerDA.GetEmployeeEventTriggers();
            }
            catch
            {
                throw;
            }
        }

        public List<EmployeeEventTrigger> GetUsersForPropagation()
        {
            try
            {
                return triggerDA.GetUsersForPropagation();
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
                triggerDA.CreateEmployeeEventTrigger(trigger);
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
                triggerDA.UpdateEmployeeEventTrigger(trigger);
            }
            catch (Exception ex)
            {
               DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while updating  the User", ex);
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(long userId, EmployeeEvents evnt)
        {
            try
            {
                return triggerDA.GetEmployeeEventTrigger(userId, evnt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EmployeeEventTrigger GetEmployeeEventTrigger(long userId, EmployeeEvents evnt, DateTime date)
        {
            try
            {
                return triggerDA.GetEmployeeEventTrigger(userId, evnt, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public void ValidateAndCreateTriggers(List<EmployeeEventTrigger> triggers)
        {
            if (triggers.Count > 0)
            {
                var onBoards = triggers.Where(item => item.EventType == EmployeeEvents.NewJoinee).ToList();
                var offBoards = triggers.Where(item => item.EventType == EmployeeEvents.Resignation).ToList();
                var ouChanges = triggers.Where(item => item.EventType == EmployeeEvents.OUChange).ToList();
                var reOrgs = triggers.Where(item => item.EventType == EmployeeEvents.ReOrganisation).ToList();

                foreach (EmployeeEventTrigger trigger in ouChanges)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null,LogLevelType.Error, "Error while creating transfer trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in reOrgs)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(null,LogLevelType.Error, "Error while creating re-org trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in onBoards)
                {
                    try
                    {
                        this.CreateEmployeeEventTrigger(trigger);

                        //active offboarding event exists for same user then disable off board event.
                        var offBoardTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.Resignation);
                        bool isOffBoardExists = offBoardTrigger != null;

                        if (isOffBoardExists)
                        {
                            offBoardTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(offBoardTrigger);
                        }

                        var postResignationTrigger = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.PostResignation);
                        bool isPostResignationEventExists = postResignationTrigger != null;

                        if (isPostResignationEventExists)
                        {
                            postResignationTrigger.IsActive = false;
                            this.UpdateEmployeeEventTrigger(postResignationTrigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating on-board trigger", ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in offBoards)
                {
                    try
                    {
                        //onboarding event exists for same user then don't create off board event.
                        bool isOnBoardExists = this.GetEmployeeEventTrigger(trigger.UserId, EmployeeEvents.NewJoinee) != null;

                        if (!isOnBoardExists)
                        {
                            this.CreateEmployeeEventTrigger(trigger);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(null, LogLevelType.Error, "Error while creating off-board trigger", ex);
                    }
                }
            }
        }

        public EmployeeEventGridData GetEmployeeEventTriggerDataById(int EventTriggerID)
        {
            try
            {
                return triggerDA.GetEmployeeEventTriggerDataById(EventTriggerID);
            }
            catch
            {
                throw;
            }
        }

        public void DeleteTriggerById(int triggerId)
        {
            try
            {
                triggerDA.DeleteTriggerById(triggerId);
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
                return triggerDA.GetEmployeeEventTriggerById(triggerId);
            }
            catch
            {
                throw;
            }
        }

    }
}
