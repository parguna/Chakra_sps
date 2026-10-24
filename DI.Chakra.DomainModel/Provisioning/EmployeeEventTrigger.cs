using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DI.Chakra.DomainModel
{
    public class EmployeeEventTrigger : DomainModelBase<EmployeeEventTrigger, EntityModel.EmployeeEventTrigger>
    {

        public long UserId { get; set; }
        public EmployeeEvents EventType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Nullable<DateTime> OldEffectiveDate { get; set; }
        public bool IsRequestCreated { get; set; }
        //public bool IsExecuted { get; set; }
        public bool IsAdminCheckRequired { get; set; }
        //public bool IsRejoiningUser { get; set; }
        public string UserDetails { get; set; }
        public string OldUserDetails { get; set; }
        public bool IsActive { get; set; }
        public AccessRequestStatus RequestStatus { get; set; }
        public string Comments { get; set; }

        public bool IsMailSent { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public User User { get; set; }

        public Nullable<long> UserEventId { get; set; }
        public Nullable<DateTime> ActualEventDate { get; set; }
        public int EventSequence { get; set; } = 1;
        public Nullable<bool> IsTransferWithNotes { get; set; }
        public override string ToString()
        {
            return $"Id:[{Id}], Event:[{EventType.ToString()}], UserId:[{UserId}], EffectiveDate:[{EffectiveDate}]";
        }

        public static bool IsEventProcessWindowOpen(DateTime now, out DateTime dueCutoff)
        {
            dueCutoff = now.Date.AddDays(1);
            return now.TimeOfDay >= MasterConfigHelper.EventProcessTime;
        }

        public static EmployeeEventTrigger GetTriggerByEventId(long userEventId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.UserEventId == userEventId && trig.IsRequestCreated == false && trig.IsActive).FirstOrDefault());
            }
        }

        public static List<EmployeeEventTrigger> GetOUChangeUsers(bool forAdminCheck)
        {
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<EmployeeEventTrigger>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsAdminCheckRequired == forAdminCheck && trig.IsRequestCreated == false && trig.IsActive &&
                                     trig.EventType == EmployeeEvents.OUChange && trig.EffectiveDate < dueCutoff).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetRehireChangeUsers(bool forAdminCheck)
        {
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<EmployeeEventTrigger>();

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsAdminCheckRequired == forAdminCheck && trig.IsRequestCreated == false && trig.IsActive &&
                                     trig.EventType == EmployeeEvents.Rehire && trig.EffectiveDate < dueCutoff).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetReOrgChangeUsers(bool forAdminCheck)
        {
            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                if (forAdminCheck)
                    return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsAdminCheckRequired && trig.IsRequestCreated == false && trig.IsActive &&
                                         trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate <= DateTime.Now).ToList());

                else
                    return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsAdminCheckRequired == false && trig.IsRequestCreated == false && trig.IsActive &&
                                                                        trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate <= DateTime.Now).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetReOrgUsers()
        {
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<EmployeeEventTrigger>();

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                trig.EventType == EmployeeEvents.ReOrganisation && trig.EffectiveDate < dueCutoff).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetUsersForPropagation()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                (trig.EventType == EmployeeEvents.Propagation) && trig.EffectiveDate <= DateTime.Now).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetUsersForPropagation(EmployeeEvents employeeEvent)
        {
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<EmployeeEventTrigger>();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                (trig.EventType == employeeEvent) && trig.EffectiveDate < dueCutoff).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetUsersForExtension()
        {
            // No display-name bypass: extensions follow the same EventProcessTime window as every other event.
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(DateTime.Now, out dueCutoff))
                return new List<EmployeeEventTrigger>();

            using (var dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(trig => trig.IsRequestCreated == false && trig.IsActive &&
                                trig.EventType == EmployeeEvents.NonEmployeeExtension && trig.EffectiveDate < dueCutoff).ToList());
            }
        }

        public static EmployeeEventTrigger GetByUserIdAndEvent(long userId, EmployeeEvents evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false && item.IsActive).FirstOrDefault());
            }
        }

        public static EmployeeEventTrigger GetByEmployeeIdAndEvent(string employeeId, EmployeeEvents evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new EmployeeEventTriggerRepository(dataAccessTransaction).GetByEmployeeIdAndEventType(employeeId, evnt));
            }
        }

        public static EmployeeEventTrigger GetByEmployeeIdAndEvent(string employeeId, EmployeeEvents evnt, DateTime effectiveDate)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new EmployeeEventTriggerRepository(dataAccessTransaction).GetByEmployeeIdAndEventType(employeeId, evnt, effectiveDate));
            }
        }


        public static List<EmployeeEventTrigger> GetAllByEmployeeIdAndEvent(string employeeId, EmployeeEvents evnt, DateTime effectiveDate)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EmployeeEventTriggerRepository(dataAccessTransaction).GetAllByEmployeeIdAndEventType(employeeId, evnt, effectiveDate));
            }
        }

        public static List<EmployeeEventTrigger> GetAllEventsByEmployeeIdAndEventType(string employeeId, EmployeeEvents evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EmployeeEventTriggerRepository(dataAccessTransaction).GetAllEventsByEmployeeIdAndEventType(employeeId, evnt));
            }
        }

        public static EmployeeEventTrigger GetByEffectiveDate(long userId, EmployeeEvents evnt, DateTime effectiveDate)
        {

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.EffectiveDate == effectiveDate && item.IsActive).FirstOrDefault());

            }
        }


        public static EmployeeEventTrigger GetByEffectiveDateYetToBeExecuted(long userId, EmployeeEvents evnt, DateTime effectiveDate, bool adminCheckRequired = false)
        {
            // effectiveDate is the caller's "now"
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(effectiveDate, out dueCutoff))
                return null;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsAdminCheckRequired == adminCheckRequired
                        && item.IsRequestCreated == false && item.IsActive && item.EffectiveDate < dueCutoff).FirstOrDefault());
            }
        }

        /// <summary>
        /// First un-executed active trigger of the event for the user that is due under the EventProcessTime window
        /// (no admin-check filter). Returns null, without querying, while the window is closed.
        /// </summary>
        public static EmployeeEventTrigger GetDueTriggerByUserAndEvent(long userId, EmployeeEvents evnt, DateTime now)
        {
            DateTime dueCutoff;
            if (!IsEventProcessWindowOpen(now, out dueCutoff))
                return null;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false && item.IsActive && item.EffectiveDate < dueCutoff).FirstOrDefault());
            }
        }

        public static EmployeeEventTrigger GetByUserAndEvent(long userId, EmployeeEvents? evnt, DateTime? date)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (date == null)
                    return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                            item.EventType == evnt && item.IsRequestCreated == false && item.IsActive).FirstOrDefault());
                else
                    return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsActive && item.CreatedDate >= date).FirstOrDefault());
            }
        }

        public static EmployeeEventTrigger GetByUserAndEventIncludingInactive(long userId, EmployeeEvents? evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false).FirstOrDefault());
            }
        }

        public static List<EmployeeEventTrigger> GetTriggerToExecute(long userId, EmployeeEvents evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (evnt == EmployeeEvents.None)
                    return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                            item.IsRequestCreated == false && item.IsActive).ToList());
                else
                    return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                           item.EventType == evnt && item.IsRequestCreated == false && item.IsActive).ToList());
            }
        }

        public static List<EmployeeEventTrigger> GetAllUserEventsToRun(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var now = DateTime.Now.Date;
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId
                        && item.IsActive && item.EffectiveDate >= now && item.IsRequestCreated == false &&
                        (item.EventType == EmployeeEvents.OUChange || item.EventType == EmployeeEvents.Resignation)).ToList());
            }
        }

        public static EmployeeEventTrigger GetByUserIdTriggerToExecute(long userId, EmployeeEvents evnt)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && item.IsRequestCreated == false && item.IsActive).FirstOrDefault());
            }
        }

        public static List<EmployeeEventTrigger> GetByUserIdTrigger(long userId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId
                && item.IsRequestCreated == false && item.IsActive).ToList());
            }
        }


        public static List<EmployeeEventTrigger> GetEmployeeEventTriggersJob(int days, EmployeeEvents employeeEvent)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new EmployeeEventTriggerRepository(dataAccessTransaction).GetEmployeeEventTriggersJob(days, employeeEvent).ToList());
            }
        }


        public static List<DeptChangedUserGridData> GetReOrgUsersForAdminCheck()
        {
            List<DeptChangedUserGridData> deptChangedUserGridData = new List<DeptChangedUserGridData>();
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {


                    var triggers = new EmployeeEventTriggerRepository(dataAccessTransaction).GetReOrgUsers(true);
                    DeptChangedUserGridData _tmpDeptChangedUserGridData;
                    foreach (EntityModel.EmployeeEventTrigger trigger in triggers)
                    {
                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();
                        _tmpDeptChangedUserGridData.UserDetail = trigger.UserDetails;
                        User usr = UtilityMethods.Deserialize<User>(trigger.UserDetails);

                        if (usr == null) continue;

                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                        _tmpDeptChangedUserGridData.UserId = usr.Id;
                        _tmpDeptChangedUserGridData.DisplayName = usr.DisplayName;
                        _tmpDeptChangedUserGridData.FirstName = usr.FirstName;
                        _tmpDeptChangedUserGridData.LastName = usr.LastName;
                        _tmpDeptChangedUserGridData.Company = usr.CompanyName;
                        _tmpDeptChangedUserGridData.CostCenter = usr.CostCenter;
                        _tmpDeptChangedUserGridData.Department = usr.DepartmentName;
                        _tmpDeptChangedUserGridData.Division = usr.DivisionName;
                        _tmpDeptChangedUserGridData.Section = usr.SectionName;
                        _tmpDeptChangedUserGridData.EmployeeNumber = usr.EmployeeId;
                        _tmpDeptChangedUserGridData.Organisation = usr.NOrganisationId;
                        if (trigger != null)
                            _tmpDeptChangedUserGridData.EffectiveDate = trigger.EffectiveDate;


                        deptChangedUserGridData.Add(_tmpDeptChangedUserGridData);
                    }
                }
                return deptChangedUserGridData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<DeptChangedUserGridData> GetOnBoardUsersForAdminCheck()
        {
            List<DeptChangedUserGridData> deptChangedUserGridData = new List<DeptChangedUserGridData>();
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {


                    var triggers = new EmployeeEventTriggerRepository(dataAccessTransaction).GetOnBoardUsers(true);
                    DeptChangedUserGridData _tmpDeptChangedUserGridData;
                    foreach (EntityModel.EmployeeEventTrigger trigger in triggers)
                    {
                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                        User usr = User.GetById(trigger.UserId);

                        if (usr == null) continue;

                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                        _tmpDeptChangedUserGridData.UserId = usr.Id;
                        _tmpDeptChangedUserGridData.DisplayName = usr.DisplayName;
                        _tmpDeptChangedUserGridData.FirstName = usr.FirstName;
                        _tmpDeptChangedUserGridData.LastName = usr.LastName;
                        _tmpDeptChangedUserGridData.Company = usr.CompanyName;
                        _tmpDeptChangedUserGridData.CostCenter = usr.CostCenter;
                        _tmpDeptChangedUserGridData.Department = usr.DepartmentName;
                        _tmpDeptChangedUserGridData.Division = usr.DivisionName;
                        _tmpDeptChangedUserGridData.Section = usr.SectionName;
                        _tmpDeptChangedUserGridData.EmployeeNumber = usr.EmployeeId;
                        _tmpDeptChangedUserGridData.Organisation = usr.NOrganisationId;
                        if (trigger != null)
                            _tmpDeptChangedUserGridData.EffectiveDate = trigger.EffectiveDate;


                        deptChangedUserGridData.Add(_tmpDeptChangedUserGridData);
                    }
                }
                return deptChangedUserGridData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<DeptChangedUserGridData> GetOUChangeUsersForAdminCheck()
        {
            List<DeptChangedUserGridData> deptChangedUserGridData = new List<DeptChangedUserGridData>();
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {

                    var triggers = new EmployeeEventTriggerRepository(dataAccessTransaction).GetOUChangeUsers(true);
                    DeptChangedUserGridData _tmpDeptChangedUserGridData;
                    foreach (EntityModel.EmployeeEventTrigger trigger in triggers)
                    {
                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();
                        _tmpDeptChangedUserGridData.UserDetail = trigger.UserDetails;
                        User usr = UtilityMethods.Deserialize<User>(trigger.UserDetails);

                        if (usr == null) continue;

                        _tmpDeptChangedUserGridData.UserId = usr.Id;
                        _tmpDeptChangedUserGridData.DisplayName = usr.DisplayName;
                        _tmpDeptChangedUserGridData.FirstName = usr.FirstName;
                        _tmpDeptChangedUserGridData.LastName = usr.LastName;
                        _tmpDeptChangedUserGridData.Company = usr.CompanyName;
                        _tmpDeptChangedUserGridData.CostCenter = usr.CostCenter;
                        _tmpDeptChangedUserGridData.Department = usr.DepartmentName;
                        _tmpDeptChangedUserGridData.Division = usr.DivisionName;
                        _tmpDeptChangedUserGridData.Section = usr.SectionName;
                        _tmpDeptChangedUserGridData.EmployeeNumber = usr.EmployeeId;
                        _tmpDeptChangedUserGridData.Organisation = usr.NOrganisationId;


                        if (trigger != null)
                            _tmpDeptChangedUserGridData.EffectiveDate = trigger.EffectiveDate;


                        deptChangedUserGridData.Add(_tmpDeptChangedUserGridData);
                    }
                }
                return deptChangedUserGridData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<DeptChangedUserGridData> GetReHireUsersForAdminCheck()
        {
            List<DeptChangedUserGridData> deptChangedUserGridData = new List<DeptChangedUserGridData>();
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {


                    var triggers = new EmployeeEventTriggerRepository(dataAccessTransaction).GetOnBoardUsers(true, EmployeeEvents.Rehire);
                    DeptChangedUserGridData _tmpDeptChangedUserGridData;
                    foreach (EntityModel.EmployeeEventTrigger trigger in triggers)
                    {
                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                        User usr = User.GetById(trigger.UserId);

                        if (usr == null) continue;

                        _tmpDeptChangedUserGridData = new DeptChangedUserGridData();

                        _tmpDeptChangedUserGridData.UserId = usr.Id;
                        _tmpDeptChangedUserGridData.DisplayName = usr.DisplayName;
                        _tmpDeptChangedUserGridData.FirstName = usr.FirstName;
                        _tmpDeptChangedUserGridData.LastName = usr.LastName;
                        _tmpDeptChangedUserGridData.Company = usr.CompanyName;
                        _tmpDeptChangedUserGridData.CostCenter = usr.CostCenter;
                        _tmpDeptChangedUserGridData.Department = usr.DepartmentName;
                        _tmpDeptChangedUserGridData.Division = usr.DivisionName;
                        _tmpDeptChangedUserGridData.Section = usr.SectionName;
                        _tmpDeptChangedUserGridData.EmployeeNumber = usr.EmployeeId;
                        _tmpDeptChangedUserGridData.Organisation = usr.NOrganisationId;
                        if (trigger != null)
                            _tmpDeptChangedUserGridData.EffectiveDate = trigger.EffectiveDate;


                        deptChangedUserGridData.Add(_tmpDeptChangedUserGridData);
                    }
                }
                return deptChangedUserGridData;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static void InitiatePropagateTrigger(User user, List<string> propagationFields, bool canRunPropagationTrigger, string currentUser)
        {
            try
            {
                var columnNames = new List<string>();

                var existingUser = User.GetById(user.Id);

                bool isPropagationRequired = false;

                foreach (string attribute in propagationFields)
                {
                    PropertyInfo trsuserpropertyInfo = user.GetType().GetProperty(attribute);

                    string newValue = string.Empty;
                    string dt = string.Empty;

                    if (trsuserpropertyInfo != null && trsuserpropertyInfo.GetValue(user) != null)
                        newValue = trsuserpropertyInfo.GetValue(user).ToString();

                    PropertyInfo userpropertyInfo = existingUser.GetType().GetProperty(attribute);

                    string oldValue = string.Empty;

                    if (userpropertyInfo != null && userpropertyInfo.GetValue(existingUser) != null)
                        oldValue = userpropertyInfo.GetValue(existingUser).ToString();

                    if (string.Compare(oldValue, newValue, true) != 0)
                    {
                        Type userPropType = userpropertyInfo.PropertyType;
                        isPropagationRequired = true;
                        columnNames.Add(attribute);
                        continue;
                    }
                }

                if (isPropagationRequired && canRunPropagationTrigger)
                {
                    CreatePropagationTrigger(user, currentUser,string.Join(",", columnNames));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Employee Event Triggers
        public static bool CreateEventTrigger(long userId, EmployeeEvents evnt, DateTime effectiveDate, string oldOrgCode, string newOrgCode, string newDesignationCode, string currentUser, string companyId, string superVisorId, string employeeType, DateTime? terminationDate = null, string costcenter = null)
        {
            try
            {
                User usr = User.GetByIdIncludeNBusinessRoleAndNOrganisation(userId);
                if (usr == null) return false;

                if (evnt == EmployeeEvents.NewJoinee)
                    CreateOnboardTrigger(usr, effectiveDate, currentUser);
                else if (evnt == EmployeeEvents.Resignation)
                    CreateOffboardTrigger(usr, effectiveDate, currentUser);
                else if (evnt == EmployeeEvents.OUChange)
                    CreateTransferTrigger(usr, effectiveDate, newOrgCode, oldOrgCode, newDesignationCode, currentUser, null, companyId, superVisorId, employeeType, costcenter);
                else if (evnt == EmployeeEvents.ReOrganisation)
                    CreateReOrgTrigger(usr, effectiveDate, newOrgCode, oldOrgCode, currentUser);
                else if (evnt == EmployeeEvents.Rehire)
                {
                    CreateRejoinTrigger(usr, effectiveDate, newOrgCode, oldOrgCode, newDesignationCode, currentUser, null, terminationDate, companyId, superVisorId, employeeType);
                    CreateOffboardTrigger(usr, terminationDate.Value, currentUser);
                }
                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Error while creating event trigger", ex);
                return false;
            }
        }
        public static bool CreateEmployeeEventTrigger(EmployeeEventTrigger trigger)
        {
            try
            {
                trigger.Save(trigger.CreatedBy);
                return true;
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Error while creating event trigger", ex);
                throw ex;
            }
        }
        public static void CreateOnboardTrigger(User user, DateTime effectiveDate, string currentUser, long? userEventId = null, EmployeeEvents eventType = EmployeeEvents.None)
        {
            EmployeeEvents evnt = EmployeeEvents.None;
            if (eventType != EmployeeEvents.None)
            {
                evnt = eventType;
            }
            else
            {
                evnt = EmployeeEvents.NewJoinee;
            }

            if (user == null) return;

            var triggerExcuted = EmployeeEventTrigger.GetAll().Where(triggerExists => triggerExists.UserId == user.Id && triggerExists.EventType == evnt).FirstOrDefault();

            var userExclusion = UserExclusion.GetUserExclusionByUserId(user.Id, evnt);

            if (triggerExcuted != null) return;

            EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
            {
                CreatedBy = Constants.CreatedBy,
                CreatedDate = DateTime.Now,
                EventType = evnt,
                IsRequestCreated = false,
                UserId = user.Id,
                IsActive = true,
                UserDetails = string.Empty,
                EffectiveDate = effectiveDate,
                LastModifiedBy = Constants.CreatedBy,
                LastModifiedDate = DateTime.Now,
                UserEventId = userEventId
            };

            newTrigger.OldEffectiveDate = effectiveDate;
            if (userExclusion != null)
            {
                if (userExclusion.EffectiveDate > newTrigger.EffectiveDate)
                    newTrigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                else
                    newTrigger.EffectiveDate = effectiveDate;
            }

            newTrigger.Save(currentUser);
        }
        public static void CreateOffboardTrigger(User user, DateTime effectiveDate, string currentUser, long? userEventId = null)
        {
            var evnt = EmployeeEvents.Resignation;

            if (user == null) return;
            //var offboardHours = Convert.ToInt32(ConfigurationMaster.GetByConfigKey("OffBoardingRegularEmployeeHours", Product.IM).ConfigValue);
            var triggerExcuted = EmployeeEventTrigger.GetAll().Where(triggerExists =>
                                                                     triggerExists.UserId == user.Id && triggerExists.EventType == evnt &&
                                                                     !triggerExists.IsRequestCreated && triggerExists.IsActive).OrderByDescending(x => x.EffectiveDate).FirstOrDefault();

            var userExclusion = UserExclusion.GetUserExclusionByUserId(user.Id, evnt);

            if (triggerExcuted != null)
            {
                triggerExcuted.OldEffectiveDate = effectiveDate;
                effectiveDate = effectiveDate < DateTime.Today ? DateTime.Today : effectiveDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                    {
                        triggerExcuted.OldEffectiveDate = triggerExcuted.EffectiveDate;
                        triggerExcuted.EffectiveDate = userExclusion.EffectiveDate.Value;//.AddHours(offboardHours);
                        effectiveDate = userExclusion.EffectiveDate.Value;
                    }
                    else
                        triggerExcuted.EffectiveDate = effectiveDate;//.AddHours(offboardHours);
                }
                else
                {
                    triggerExcuted.EffectiveDate = effectiveDate;//.AddHours(offboardHours);
                }
                triggerExcuted.IsActive = true;
                triggerExcuted.LastModifiedBy = triggerExcuted.CreatedBy;
                triggerExcuted.LastModifiedDate = DateTime.Now;
                triggerExcuted.UserEventId = userEventId;
                triggerExcuted.Save(currentUser);
            }
            else
            {
                triggerExcuted = EmployeeEventTrigger.GetAll().Where(triggerExists =>
                                                                    triggerExists.UserId == user.Id && triggerExists.EventType == evnt &&
                                                                    triggerExists.IsRequestCreated && triggerExists.IsActive).OrderByDescending(x => x.EffectiveDate).FirstOrDefault();

                if (!(triggerExcuted != null && triggerExcuted.OldEffectiveDate.HasValue && triggerExcuted.OldEffectiveDate.Value.Date == effectiveDate.Date))
                {
                    triggerExcuted = new EmployeeEventTrigger()
                    {
                        CreatedBy = Constants.CreatedBy,
                        CreatedDate = DateTime.Now,
                        EventType = evnt,
                        IsRequestCreated = false,
                        UserId = user.Id,
                        IsActive = true,
                        UserDetails = string.Empty,
                        EffectiveDate = effectiveDate,
                        LastModifiedBy = Constants.CreatedBy,
                        LastModifiedDate = DateTime.Now,
                        UserEventId = userEventId
                    };

                    triggerExcuted.OldEffectiveDate = effectiveDate;
                    effectiveDate = effectiveDate < DateTime.Today ? DateTime.Today : effectiveDate;
                    triggerExcuted.EffectiveDate = effectiveDate;
                    if (userExclusion != null)
                    {
                        if (userExclusion.EffectiveDate > triggerExcuted.EffectiveDate)
                        {
                            triggerExcuted.OldEffectiveDate = triggerExcuted.EffectiveDate;
                            triggerExcuted.EffectiveDate = userExclusion.EffectiveDate.Value;//.AddHours(offboardHours);
                            effectiveDate = userExclusion.EffectiveDate.Value;
                        }
                        else
                            triggerExcuted.EffectiveDate = effectiveDate;//.AddHours(offboardHours);

                    }

                    triggerExcuted.Save(currentUser);
                }
            }
            User oldUser = User.GetById(user.Id);
            if (oldUser != null)
            {
                oldUser.TerminationDate = effectiveDate;
                oldUser.LastModifiedBy = currentUser;
                oldUser.LastModifiedDate = DateTime.Now;
                oldUser.Save(currentUser);
            }
        }
        public static void CreateTransferTrigger(User user, DateTime effectiveDate, string newOrgCode, string oldOrgCode,
            string newDesignationCode,
            string currentUser, long? userEventId = null, string companyId = null, string superVisorId = null,
            string employeeType = null, string costcenter = null, string businessUnitId = null)
        {
            EmployeeEvents evnt = EmployeeEvents.OUChange;

            if (user == null) return;


            var oldUser = User.GetByIdIncludeNBusinessRoleAndNOrganisation(user.Id);
            if (oldUser == null) return;

            var oldOrg = NOrganisation.GetByOrgCode(oldOrgCode);
            if (oldOrg != null)
                oldUser.NOrganisation = oldOrg;

            if (oldUser.NBusinessRoleId.HasValue && oldUser.NBusinessRole == null)
            {
                var oldNBusinessRole = NDesignation.GetById(oldUser.NBusinessRoleId.Value);
                if (oldNBusinessRole != null)
                    oldUser.NBusinessRole = oldNBusinessRole;
            }


            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);

            var newOrg = NOrganisation.GetByOrgCode(newOrgCode);
            if (newOrg != null)
            {
                user.NOrganisation = newOrg;
                user.NOrganisationId = newOrg.Id;
            }

            if (!string.IsNullOrEmpty(newDesignationCode))
            {
                var newDesignation = NDesignation.GetByCode(newDesignationCode);
                user.NBusinessRole = newDesignation;
                user.NBusinessRoleId = newDesignation.Id;
            }

            if (!string.IsNullOrEmpty(employeeType))
            {
                EmployeeType tempType;
                Enum.TryParse(employeeType, out tempType);
                user.EmployeeType = tempType;
            }

            var newUserDetail = UtilityMethods.Serialize<User>(user);

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);

            var userExclusion = UserExclusion.GetUserExclusionByUserId(user.Id, evnt);

            if (trigger != null)
            {
                if (trigger.IsRequestCreated)
                {
                    return;
                }

                trigger.OldEffectiveDate = effectiveDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }
                else
                    trigger.EffectiveDate = effectiveDate;
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;
                trigger.OldUserDetails = oldUserDetail;
                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.UserEventId = userEventId;
                trigger.Save(currentUser);
            }
            else
            {

                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    LastModifiedBy = Constants.CreatedBy,
                    LastModifiedDate = DateTime.Now,
                    UserDetails = newUserDetail,
                    OldUserDetails = oldUserDetail,
                    UserEventId = userEventId
                };
                trigger.OldEffectiveDate = effectiveDate;

                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > trigger.EffectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }

                trigger.Save(currentUser);
            }
            // We don't need to save user object for neworganization ... this is handled in CreateOUChangeTasksIfAdminCheckNotRequired.CreateOUChangeTasksIfAdminCheckNotRequired
            //user.Save(currentUser); // For Transfer we need to save neworganization id value in to user table.
        }

        public static void CreateReOrgTrigger(User user, DateTime effectiveDate, string newOrgCode, string oldOrgCode, string currentUser)
        {
            EmployeeEvents evnt = EmployeeEvents.ReOrganisation;

            if (user == null) return;

            var oldUser = User.GetByIdIncludeNBusinessRoleAndNOrganisation(user.Id);
            if (oldUser == null) return;

            var oldOrg = NOrganisation.GetByOrgCode(oldOrgCode);
            if (oldOrg != null)
                oldUser.NOrganisation = oldOrg;
            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);
            var userExclusion = UserExclusion.GetUserExclusionByUserId(user.Id, evnt);
            if (trigger != null)
            {
                if (trigger.IsRequestCreated)
                {
                    return;
                }
                trigger.OldEffectiveDate = effectiveDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }
                else
                    trigger.EffectiveDate = effectiveDate;
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;
                trigger.OldUserDetails = oldUserDetail;
                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.Save(currentUser);
            }
            else
            {

                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    LastModifiedBy = Constants.CreatedBy,
                    LastModifiedDate = DateTime.Now,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                    OldUserDetails = oldUserDetail

                };
                trigger.OldEffectiveDate = effectiveDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }

                trigger.Save(currentUser);
            }

        }
        public static void CreateRejoinTrigger(User user, DateTime effectiveDate, string newOrgCode, string oldOrgCode,
            string newDesignationCode, string currentUser, long? userEventId = null, DateTime? terminationDate = null,
            string companyId = null, string superVisorId = null, string employeeType = null, DateTime? originaleffctDate = null)
        {
            EmployeeEvents evnt = EmployeeEvents.Rehire;
            if (user == null) return;

            if (originaleffctDate == null)
                originaleffctDate = effectiveDate;

            var oldUser = User.GetByNRIC(user.IdentificationNo);
            if (oldUser == null) return;

            var oldOrg = NOrganisation.GetByOrgCode(oldOrgCode);
            if (oldOrg != null)
                oldUser.NOrganisation = oldOrg;

            if (oldUser.NBusinessRoleId.HasValue && oldUser.NBusinessRole == null)
            {
                var oldNBusinessRole = NDesignation.GetById(oldUser.NBusinessRoleId.Value);
                if (oldNBusinessRole != null)
                    oldUser.NBusinessRole = oldNBusinessRole;
            }

            var oldUserDetail = UtilityMethods.Serialize<User>(oldUser);

            var newOrg = NOrganisation.GetByOrgCode(newOrgCode);
            if (newOrg != null)
            {
                user.NOrganisation = newOrg;
                user.NOrganisationId = newOrg.Id; // Override new organization id value also
            }

            if (!string.IsNullOrEmpty(newDesignationCode))
            {
                var newDesignation = NDesignation.GetByCode(newDesignationCode);
                user.NBusinessRole = newDesignation;
                user.NBusinessRoleId = newDesignation.Id; // Override new organization id value also
            }

            if (terminationDate.HasValue)
                user.TerminationDate = terminationDate.Value;
            else
                user.TerminationDate = null;

            if (!string.IsNullOrEmpty(employeeType))
            {
                EmployeeType tempType;
                Enum.TryParse(employeeType, out tempType);
                user.EmployeeType = tempType;
            }

            var newUserDetail = UtilityMethods.Serialize<User>(user);

            var trigger = EmployeeEventTrigger.GetByUserAndEvent(user.Id, evnt, null);
            var userExclusion = UserExclusion.GetUserExclusionByUserId(user.Id, evnt);

            if (trigger != null)
            {
                if (trigger.IsRequestCreated)
                {
                    return;
                }
                trigger.OldEffectiveDate = originaleffctDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }
                else
                    trigger.EffectiveDate = effectiveDate;
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;
                trigger.OldUserDetails = oldUserDetail;
                trigger.UserDetails = UtilityMethods.Serialize<User>(user);
                trigger.UserEventId = userEventId;
                trigger.Save(currentUser);
            }
            else
            {

                trigger = new EmployeeEventTrigger()
                {
                    CreatedBy = Constants.CreatedBy,
                    CreatedDate = DateTime.Now,
                    EventType = evnt,
                    IsRequestCreated = false,
                    UserId = user.Id,
                    IsActive = true,
                    EffectiveDate = effectiveDate,
                    LastModifiedBy = Constants.CreatedBy,
                    LastModifiedDate = DateTime.Now,
                    UserDetails = UtilityMethods.Serialize<User>(user),
                    OldUserDetails = oldUserDetail,
                    UserEventId = userEventId
                };

                trigger.OldEffectiveDate = originaleffctDate;
                if (userExclusion != null)
                {
                    if (userExclusion.EffectiveDate > effectiveDate)
                        trigger.EffectiveDate = userExclusion.EffectiveDate.Value;
                    else
                        trigger.EffectiveDate = effectiveDate;
                }

                trigger.Save(currentUser);
            }
        }
        public static void CreatePropagationTrigger(User user, string currentUser, string columnNames, DateTime? startDate = null, string reason = null)
        {
            EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
            {
                CreatedBy = user.CreatedBy,
                CreatedDate = DateTime.Today,
                EffectiveDate = DateTime.Today,
                EventType = EmployeeEvents.Propagation,
                IsRequestCreated = false,
                UserId = user.Id,
                IsActive = true
            };

            if (startDate != null)
                newTrigger.EffectiveDate = startDate.Value;

            if(!string.IsNullOrWhiteSpace(columnNames))
                newTrigger.Comments = $"Changes in : {columnNames}";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                newTrigger.Comments = newTrigger.Comments + $" The reason is: {reason}";
            }

            newTrigger.Save(currentUser);
        }

        public static void CreatePropagationTrigger(long userId, string createdBy)
        {
            var oldTrigger = EmployeeEventTrigger.GetByUserIdAndEvent(userId, EmployeeEvents.Propagation);
            if (oldTrigger == null)
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.Today,
                    EffectiveDate = DateTime.Today,
                    EventType = EmployeeEvents.Propagation,
                    IsRequestCreated = false,
                    UserId = userId,
                    IsActive = true
                };
                newTrigger.Save(createdBy);
            }
            else
            {
                oldTrigger.EffectiveDate = DateTime.Now;
                oldTrigger.LastModifiedDate = DateTime.Now;
                oldTrigger.LastModifiedBy = createdBy;
                oldTrigger.IsRequestCreated = false;
                oldTrigger.IsActive = true;
                oldTrigger.Save(createdBy);
            }
        }

        public static bool UpdateEventTrigger(EmployeeEventTrigger eventTrigger, string currentUser)
        {
            try
            {
                if (eventTrigger != null)
                {
                    DateTime effectiveDate = eventTrigger.EffectiveDate;


                    var userExclusion = UserExclusion.GetUserExclusionByUserId(eventTrigger.UserId, eventTrigger.EventType);

                    if (userExclusion != null)
                    {
                        if (userExclusion.EffectiveDate > eventTrigger.EffectiveDate)
                        {
                            eventTrigger.EffectiveDate = userExclusion.EffectiveDate.Value;//.AddHours(offboardHours);
                            effectiveDate = userExclusion.EffectiveDate.Value;
                        }
                        else
                            eventTrigger.EffectiveDate = effectiveDate;//.AddHours(offboardHours);
                    }

                    using (var dataAccessTransaction = new DataAccessTransaction())
                    {
                        eventTrigger.Save(currentUser);

                        if (eventTrigger.EventType == EmployeeEvents.Resignation)
                        {
                            UpdateAccessRequest(eventTrigger.UserId, currentUser);

                            User user = User.GetById(eventTrigger.UserId);
                            if (user != null)
                            {
                                user.TerminationDate = eventTrigger.EffectiveDate;
                                user.Save(currentUser);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DILog.ProvisioningLogger.WriteLog(null, null, LogLevelType.Error, "Error while updating event trigger", ex);
                return false;
            }
            return true;
        }

        public static bool UdpateResignationEventTrigger(long userId, string currentUser)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                var resignation = ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                                 item.EventType == EmployeeEvents.Resignation && item.IsRequestCreated == false && item.IsActive).FirstOrDefault());
                if (resignation != null)
                {
                    resignation.IsActive = false;
                    resignation.Save(currentUser);

                    User user = User.GetById(userId);
                    if (user != null)
                    {
                        user.TerminationDate = resignation.EffectiveDate;
                        user.Save(currentUser);
                    }
                }
            }
            return true;
        }
        #endregion

        public static void ValidateAndCreateTriggers(List<EmployeeEventTrigger> triggers, string currentUser)
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
                        trigger.Save(currentUser);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating transfer trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in reOrgs)
                {
                    try
                    {
                        trigger.Save(currentUser);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating re-org trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in onBoards)
                {
                    try
                    {
                        trigger.Save(currentUser);
                        //active offboarding event exists for same user then disable off board event.
                        var offBoardTrigger = GetByUserAndEvent(trigger.UserId, EmployeeEvents.Resignation, null);
                        bool isOffBoardExists = offBoardTrigger != null;
                        if (isOffBoardExists)
                        {
                            offBoardTrigger.IsActive = false;
                            offBoardTrigger.Save(currentUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating on-board trigger", ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in offBoards)
                {
                    try
                    {
                        //onboarding event exists for same user then don't create off board event.
                        bool isOnBoardExists = GetByUserAndEvent(trigger.UserId, EmployeeEvents.NewJoinee, null) != null;

                        if (!isOnBoardExists)
                        {
                            trigger.Save(currentUser);
                        }
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating off-board trigger", ex);
                    }
                }
            }
        }

        public static bool IsAnyOnBoardTriggerExistsAndExecutedForUser(long userId, EmployeeEvents eventType = EmployeeEvents.NewJoinee)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction)
                    .Any(trig => trig.EventType == eventType
                                    && trig.UserId == userId
                                    && trig.IsRequestCreated == true);
            }
        }

        public static bool IsAnyRehireTriggerExistsAndExecutedForUser(long userId, DateTime? originalEffectiveDate, EmployeeEvents eventType = EmployeeEvents.NewJoinee)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction)
                    .Any(trig => trig.EventType == eventType
                                    && trig.UserId == userId
                                    && trig.IsRequestCreated == true && trig.OldEffectiveDate == originalEffectiveDate);
            }
        }

        private static void UpdateAccessRequest(long userid, string currentUser)
        {
            var user = User.GetById(userid);
            if (!user.FullTimeEmployee)
            {
                var IdasAccessRequest = AccessRequest.GetById(Convert.ToInt32(user.Reserved9));
                if (IdasAccessRequest != null)
                {
                    IdasAccessRequest.DeactivationDate = user.TerminationDate;
                    IdasAccessRequest.Save(currentUser);

                    var ADAccessRequest = AccessRequest.GetByParentRequestId(IdasAccessRequest.Id).FirstOrDefault();
                    if (ADAccessRequest != null)
                    {
                        ADAccessRequest.DeactivationDate = user.TerminationDate;
                        ADAccessRequest.Save(currentUser);

                        var ExchangeAccessRequest = AccessRequest.GetByParentRequestId(ADAccessRequest.Id).FirstOrDefault();
                        if (ExchangeAccessRequest != null)
                        {
                            ExchangeAccessRequest.DeactivationDate = user.TerminationDate;
                            ExchangeAccessRequest.Save(currentUser);
                        }
                    }
                }
            }
        }

        public static void ValidateAndCreateTriggers(List<EmployeeEventTrigger> triggers, List<long> rejoinWithSamePernrUsers, string currentUser)
        {
            if (triggers.Count > 0)
            {
                var onBoards = triggers.Where(item => item.EventType == EmployeeEvents.NewJoinee).ToList();
                var rehires = triggers.Where(item => item.EventType == EmployeeEvents.Rehire).ToList();
                var offBoards = triggers.Where(item => item.EventType == EmployeeEvents.Resignation).ToList();
                var ouChanges = triggers.Where(item => item.EventType == EmployeeEvents.OUChange).ToList();
                var reOrgs = triggers.Where(item => item.EventType == EmployeeEvents.ReOrganisation).ToList();

                foreach (EmployeeEventTrigger trigger in ouChanges)
                {
                    try
                    {
                        trigger.Save(currentUser);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating transfer trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in reOrgs)
                {
                    try
                    {
                        trigger.Save(currentUser);
                    }
                    catch (Exception ex)
                    {
                        var user = UtilityMethods.Deserialize<User>(trigger.UserDetails);
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating re-org trigger for user - " + user.DisplayName, ex);
                    }
                }

                foreach (EmployeeEventTrigger trigger in onBoards)
                {
                    try
                    {
                        trigger.Save(currentUser);
                    }
                    catch (Exception ex)
                    {
                        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating on-board trigger", ex);
                    }
                }

                //foreach (EmployeeEventTrigger trigger in rehires)
                //{
                //    try
                //    {
                //        trigger.Save(currentUser);

                //        //rejoining case - active offboarding event exists for same user then disable off board event.

                //        var offBoardTrigger = GetByUserIdAndEvent(trigger.UserId, EmployeeEvents.Resignation);
                //        bool isOffBoardExists = offBoardTrigger != null;

                //        if (isOffBoardExists)
                //        {
                //            offBoardTrigger.IsActive = false;
                //            offBoardTrigger.Save(currentUser);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating on-board trigger", ex);
                //    }
                //}

                //foreach (EmployeeEventTrigger trigger in offBoards)
                //{
                //    try
                //    {



                //        ////onboarding event exists for same user then invalidate off board event.
                //        var rehireTrigger = GetByUserIdAndEvent(trigger.UserId, EmployeeEvents.Rehire);

                //        if (rehireTrigger != null)
                //        {
                //            trigger.IsActive = false;
                //        }

                //        trigger.Save(currentUser);
                //    }
                //    catch (Exception ex)
                //    {
                //        DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while creating off-board trigger", ex);
                //    }
                //}

                foreach (int userId in rejoinWithSamePernrUsers)
                {
                    var user = User.GetById(userId);
                    DeactivateOffBoardTrigger(user, currentUser);
                }
            }
        }

        static void DeactivateOffBoardTrigger(User user, string currentUser)
        {
            try
            {
                var offBoardTrigger = GetByUserIdAndEvent(user.Id, EmployeeEvents.Resignation);

                if (offBoardTrigger != null)
                {
                    offBoardTrigger.IsActive = false;
                    offBoardTrigger.Save(currentUser);
                }
            }
            catch (Exception ex)
            {
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error, "Error while deactivating off-board trigger for the user - " + user.DisplayName, ex);
            }
        }

        public static List<EmployeeEventTrigger> GetSubmittedTriggers()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction)
                    .Get(trig => trig.RequestStatus == AccessRequestStatus.Submitted
                                    && !trig.IsActive).ToList());
            }
        }
        public static EmployeeEventTrigger GetEmployeeEventTriggerWaitingForApproval(long userId, EmployeeEvents evnt)
        {
            try
            {
                using (var dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Get(item => item.UserId == userId &&
                        item.EventType == evnt && (item.RequestStatus == AccessRequestStatus.Submitted ||
                        item.RequestStatus == AccessRequestStatus.Requested)).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<EmployeeEventTrigger> GetDeletedEmployeeEvetriggersByEventType(DateTime processStartDateTime, EmployeeEvents EventType, string createdBy)
        {
            List<EmployeeEventTrigger> listDeletedtriggers = new List<EmployeeEventTrigger>();

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                listDeletedtriggers = ORMapList(new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction)
                   .Get(trigger => trigger.EventType == EventType && trigger.CreatedBy == createdBy &&
                                   (
                                   (trigger.LastModifiedDate <= processStartDateTime && trigger.LastModifiedDate != null) ||
                                   (trigger.LastModifiedDate == null && trigger.CreatedDate <= processStartDateTime)
                                   ) &&
                                  !trigger.IsRequestCreated && trigger.IsActive).ToList());
            }

            return listDeletedtriggers;
        }
        /// <summary>
        /// Create dummy employeevent trigger for AD propagation only
        /// </summary>
        /// <param name="user"></param>
        /// <param name="currentUser"></param>
        /// <param name="columnNames"></param>
        /// <param name="startDate"></param>
        /// <param name="reason"></param>
        public static long CreateDummyPropagationTrigger(long userId, string currentUser, string columnNames, string reason = null)
        {
            EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
            {
                CreatedBy = "System",
                CreatedDate = DateTime.Today,
                EffectiveDate = DateTime.Now,
                EventType = EmployeeEvents.Propagation,
                IsRequestCreated = true,
                UserId = userId,
                IsActive = false,
            };


            if (!string.IsNullOrWhiteSpace(columnNames))
                newTrigger.Comments = $"Changes in : {columnNames}";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                newTrigger.Comments = newTrigger.Comments + $" The reason is: {reason}";
            }

            return newTrigger.Save(currentUser);
        }
        /// <summary>
        /// This method is used to check is there any event present before this.If there any event, then it must execute first.
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static bool IsMultipleEventOnSameDay(EmployeeEventTrigger trigger)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.EmployeeEventTrigger>(dataAccessTransaction).Any(i => i.UserId == trigger.UserId && trigger.IsActive && i.EffectiveDate == trigger.EffectiveDate && i.EventSequence < trigger.EventSequence && i.IsRequestCreated == false);
            }
        }
        public static EmployeeEventTrigger GetOldByEmployeeIdAndEvent(string employeeId, EmployeeEvents evnt, DateTime effectiveDate)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new EmployeeEventTriggerRepository(dataAccessTransaction).GetOldEventByEmployeeIdAndEventType(employeeId, evnt, effectiveDate));
            }
        }
    }

    public class DeptChangedUserGridData
    {
        public long UserId { get; set; }
        public string DisplayName { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string Section { get; set; }
        public long? Organisation { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ErrorMessage { get; set; }
        public string UserDetail { get; set; }
    }


    public class EmployeeEventTriggerComment
    {
        public DateTime EffectiveDate { get; set; }

        public string EventCode { get; set; }

        public string EventReason { get; set; }

        public string EventType { get; set; }

        public string EmployeeId { get; set; }

        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Used to track Notes for transfer scenario. If notes contains Retain then it should create as Reorg trigger
        /// </summary>
        public string RetainAccessForTransfer { get; set; }
        public static EmployeeEventTriggerComment GetEmployeeEventTriggerComment(string s)
        {
            EmployeeEventTriggerComment obj = null;
            if (string.IsNullOrWhiteSpace(s)) return obj;
            try { obj = JsonConvert.DeserializeObject<EmployeeEventTriggerComment>(s); }
            catch (Exception e)
            {
            }
            return obj;
        }

        public override string ToString()
        {
            return $"EventType:[{EventType}],EffectiveDate:[{EffectiveDate}],EventCode:[{EventCode}],EventReason:[{EventReason}],EmployeeId:[{EmployeeId},EndDate:[{EndDate}],EventCode:[{EventCode}],EventNotes:[{RetainAccessForTransfer}]";
        }

    }
}
