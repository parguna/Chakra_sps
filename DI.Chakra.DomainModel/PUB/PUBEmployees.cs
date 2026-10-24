using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;

namespace DI.Chakra.DomainModel
{
    public class PUBEmployees : DomainModelBase<DomainModel.PUBEmployees, EntityModel.PUBEmployees>
    {
        public Nullable<long> DIUserId { get; set; }
        public Nullable<long> PUBFileId { get; set; }
        public string EmployeeId { get; set; }
        public string EBSUserId { get; set; }
        public string DisplayName { get; set; }
        public string CompanyName { get; set; }
        public Nullable<DateTime> ActualTerminationDate { get; set; }
        public Nullable<DateTime> NewTerminationDate { get; set; }
        public string Status { get; set; }
        public string ValidationMsg { get; set; }
        public Nullable<long> EmployeeEventTriggerId { get; set; }
        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set;}
        
        public static void ValidateAndSave(long fileId, string serviceUser, SchedulerLogger schedulerLogger)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                List<PUBEmployees> employees = ORMapList(new PUBEmployeesRepository(dataAccessTransaction).GetPUBEmployeesByStatus(fileId, new[] { PUBConstants.EmployeeStatus.Received }));
                foreach (var employee in employees)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(employee.ValidationMsg))
                        {
                            stringBuilder.AppendLine(employee.ValidationMsg);
                        }

                        User user = null;

                        if (!string.IsNullOrWhiteSpace(employee.EBSUserId))
                        {
                            var list = AccountReconcilation.GetUserByActiveEBSAccountRecon(employee.EBSUserId);

                            if (list == null || list.Count == 0)
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.InvalidEBSUserId);
                            }

                            if (list != null && list.All(a => a.IsDeleted == true))
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.InvalidEBSUserId);
                            }

                            if (list.Any(a => a.UserId.HasValue && a.IsDeleted == false))
                            {
                                foreach (var acc in list)
                                {
                                    if (user != null)
                                    {
                                        if (!acc.UserId.HasValue || acc.UserId.Value == user.Id) continue;
                                        else
                                        {
                                            var duplicateUser = User.GetById(acc.UserId.Value);
                                            if (duplicateUser != null && duplicateUser.IsActive && duplicateUser.FullTimeEmployee == false)
                                                stringBuilder.Append($"Multiple User profile mapped with the EBS account. The Account Name [{employee.EBSUserId}] is mapped with User [EmployeeId :{duplicateUser.EmployeeId}, DisplayName:{duplicateUser.DisplayName}] in the Endpoint system [{acc.EndPoint.DisplayName}]. ");
                                        }
                                    }
                                    else
                                    {
                                        if (!acc.UserId.HasValue) continue;

                                        var usr = User.GetById(acc.UserId.Value);

                                        if (usr.IsActive == false && usr.FullTimeEmployee)
                                        {
                                            stringBuilder.Append($"The EBS account mapped with inactive permanant employee user profile. The user is, [EmployeeId :{usr.EmployeeId}, DisplayName:{usr.DisplayName}]. ");
                                        }
                                        else if (usr.IsActive == false && usr.FullTimeEmployee == false)
                                        {
                                            stringBuilder.Append($"The EBS account mapped with inactive non-employee user profile. The user is, [EmployeeId :{usr.EmployeeId}, DisplayName:{usr.DisplayName}]. ");
                                        }
                                        else if (usr.IsActive == true && usr.FullTimeEmployee == true)
                                        {
                                            stringBuilder.Append($"The EBS account mapped with active permanant employee user profile. The user is, [EmployeeId :{usr.EmployeeId}, DisplayName:{usr.DisplayName}]. ");
                                        }
                                        else
                                        {
                                            user = usr;
                                        }

                                    }

                                }
                            }

                        }


                        if (user == null)
                        {
                            employee.DIUserId = null;
                        }
                        else
                        {
                            employee.EmployeeId = user.EmployeeId;
                            employee.DIUserId = user.Id;
                            employee.ActualTerminationDate = user.TerminationDate;

                            if (!employee.DisplayName.Trim().Equals(user.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.InvalidEBSUsername);
                            }

                            if (!user.NCompany.Trim().Equals(employee.CompanyName, StringComparison.OrdinalIgnoreCase))
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.Company_Name_is_not_PUB);
                            }


                            if (user.FullTimeEmployee) { stringBuilder.Append($"User is a permanant employee. The Employee Id is :[{employee.EmployeeId}]. "); }

                            if (!user.IsActive)
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.InactiveEBSUserID);
                                //stringBuilder.Append("Already User is inactive. ");
                            }
                            if (!employee.NewTerminationDate.HasValue)
                            {
                                AddValidation(stringBuilder, PUBConstants.PUB_ValidationMsg.InvalidDate);
                                //stringBuilder.Append("Termination Date is Invalid or not specified. ");
                            }
                            if (employee.NewTerminationDate.HasValue &&
                                employee.NewTerminationDate.Value < DateTime.Today)
                            {

                                AddValidation(stringBuilder, string.Format(PUBConstants.PUB_ValidationMsg.TerminationDateIsPastDate, employee.NewTerminationDate.Value.ToString(PUBConstants.PUBDateFormat)));
                                //stringBuilder.Append($"Termination Date is Past Date. Proposed TerminationDate:[{employee.NewTerminationDate.Value}]. ");
                            }
                            if (employee.NewTerminationDate.HasValue && employee.NewTerminationDate.Value > DateTime.Today.AddDays(3))
                            {
                                AddValidation(stringBuilder, string.Format("Termination Date is More than 3 days. Proposed TerminationDate:[{0}]. ", employee.NewTerminationDate.Value.ToString(PUBConstants.PUBDateFormat)));
                            }
                        }
                        if (stringBuilder.Length > 0)
                        {
                            employee.ValidationMsg = stringBuilder.ToString();
                            employee.Status = PUBConstants.EmployeeStatus.ValidationFailed;
                        }
                        else
                        {
                            employee.Status = PUBConstants.EmployeeStatus.YetToBeProcessed;
                        }

                    }
                    catch (Exception ee)
                    {
                        schedulerLogger.WriteLog(fileId, LogLevelType.Exception, $"{PUBConstants.FileNames.Process3_FileName}: Exception in ValidateAndSave for the EBSUserID -[{employee.EBSUserId}]", ee);
                    }
                    finally
                    {
                        stringBuilder.Length = 0;
                        stringBuilder.Clear();
                    }
                    employee.Save(serviceUser);
                }
            }
        }


        static void AddValidation(StringBuilder stringBuilder, string msg)
        {
            if (stringBuilder.Length == 0) { stringBuilder.Append(msg); return; }

            if (stringBuilder.ToString().Contains(msg)) return;

            stringBuilder.Append(msg);

        }

        public static void Process(long fileId, string serviceUser, SchedulerLogger schedulerLogger)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                StringBuilder stringBuilder = new StringBuilder();
                List<PUBEmployees> pub_Employees = ORMapList(new PUBEmployeesRepository(dataAccessTransaction).GetPUBEmployeesByStatus(fileId, new[] { PUBConstants.EmployeeStatus.YetToBeProcessed }));

                TimeSpan time = new TimeSpan(0, 18, 0, 0);

                foreach (var pubEmployee in pub_Employees)
                {
                    try
                    {
                        if (!pubEmployee.DIUserId.HasValue || !pubEmployee.NewTerminationDate.HasValue)
                        {
                            pubEmployee.ValidationMsg = (!pubEmployee.DIUserId.HasValue) ? "User Not Found" : "Invalid/Empty Termination Date";
                            pubEmployee.Status = PUBConstants.EmployeeStatus.ValidationFailed;
                            pubEmployee.Save(serviceUser);
                            continue;
                        }



                        var trigger = EmployeeEventTrigger.GetByUserIdAndEvent(pubEmployee.DIUserId.Value, Infrastructure.Enums.EmployeeEvents.Resignation);

                        if (trigger == null)
                        {
                            trigger = new EmployeeEventTrigger()
                            {
                                CreatedBy = "PUB Service",
                                CreatedDate = DateTime.Now,
                                EffectiveDate = pubEmployee.NewTerminationDate.Value.Date != DateTime.Now.Date ? pubEmployee.NewTerminationDate.Value.Add(time) : pubEmployee.NewTerminationDate.Value,
                                EventType = Infrastructure.Enums.EmployeeEvents.Resignation,
                                IsRequestCreated = false,
                                UserId = pubEmployee.DIUserId.Value,
                                IsActive = true,
                                Comments = $"OffBoard Event Trigger - Created from PUB File. The file Id :{fileId}."
                            };

                            schedulerLogger.WriteLog(fileId, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: OffBoard Event Trigger created for the EBSUserID -[{pubEmployee.EBSUserId}]", null);
                        }
                        else
                        {
                            trigger.OldEffectiveDate = trigger.EffectiveDate;
                            trigger.EffectiveDate = pubEmployee.NewTerminationDate.Value.Date != DateTime.Now.Date ? pubEmployee.NewTerminationDate.Value.Add(time) : pubEmployee.NewTerminationDate.Value;
                            trigger.Comments = $@"OffBoard Event Trigger - Updated from PUB File. The file Id :{fileId}. The Old Effective Date was {trigger.OldEffectiveDate.Value.ToString("dd/MM/yyyy hh:mm:ss tt",
                      CultureInfo.InvariantCulture)}. {Environment.NewLine} {trigger.Comments}";

                            schedulerLogger.WriteLog(fileId, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: OffBoard Event Trigger update for the EBSUserID -[{pubEmployee.EBSUserId}]", null);
                        }

                        var user = User.GetById(pubEmployee.DIUserId.Value);
                        user.TerminationDate = pubEmployee.NewTerminationDate.Value.Add(time);
                        user.Save(serviceUser);
                        trigger.Save(serviceUser);
                        pubEmployee.EmployeeEventTriggerId = trigger.Id;
                        if (pubEmployee.NewTerminationDate.Value.Date != DateTime.Now.Date)
                        {
                            pubEmployee.Status = PUBConstants.EmployeeStatus.ProcessedCompleted;
                            pubEmployee.ValidationMsg = string.Format(PUBConstants.PUB_ValidationMsg.FutureTerminationDateMsg, pubEmployee.NewTerminationDate.Value.ToString(PUBConstants.PUBDateFormat));
                            schedulerLogger.WriteLog(fileId, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: PUB Employee Status updated as FuturedatedTermination for the EBSUserID -[{pubEmployee.EBSUserId}]", null);

                        }
                        else
                        {
                            pubEmployee.Status = PUBConstants.EmployeeStatus.PendingInProcess;
                            schedulerLogger.WriteLog(fileId, LogLevelType.Info, $"{PUBConstants.FileNames.Process3_FileName}: PUB Employee Status updated as ProcessCompleted for the EBSUserID -[{pubEmployee.EBSUserId}]", null);
                        }
                        pubEmployee.Save(serviceUser);

                    }
                    catch (Exception ee)
                    {
                        schedulerLogger.WriteLog(fileId, LogLevelType.Exception, $"{PUBConstants.FileNames.Process3_FileName}: Exception in Process for the EBSUserID -[{pubEmployee.EBSUserId}]", ee);
                    }
                }
            }
            
        }
    }



}
