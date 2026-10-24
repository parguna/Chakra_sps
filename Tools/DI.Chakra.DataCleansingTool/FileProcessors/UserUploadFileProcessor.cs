
using DI.Chakra.DataAccess;
using DI.Chakra.Facade;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using DI.Chakra.Infrastructure;


namespace DataCleansingTool.FileProcessors
{
    public class UserUploadFileProcessor : FileProcessorBase
    {

        UserFacade userFacade = new UserFacade();
        TriggerFacade triggerFacade = new TriggerFacade();

        private List<DI.Chakra.DomainModel.EmployeeTerm> employeeTerms = new List<DI.Chakra.DomainModel.EmployeeTerm>();

        public override void UploadData(string fileName)
        {
            string validationMsg = string.Empty;

            if (!this.IsValidFile(fileName, ref validationMsg))
            {
                this.UploadLog = validationMsg;
                return;
            }

            DataTable dt = new ExcelConnect().ReadExcel(fileName);

            this.employeeTerms =DI.Chakra.DomainModel.EmployeeTerm.GetByIsActive();

            foreach (DataRow dRow in dt.Rows)
            {
                UserXLData userXlData = this.GetUserObjectFromData(dRow);
                if (userXlData != null)
                {
                    this.UploadLog += this.Process(userXlData);
                }
            }
        }

        private string Process(UserXLData userXlData)
        {
            string log = string.Empty;

            try
            {
                log += string.Format("Started processing user - {0}.", userXlData.KnownAs) + "\n";

                //get user by pernr
                User user = userFacade.GetUserByEmployeeId(userXlData.ReferenceUserPernr);

                if (user == null)
                {
                    log += string.Format("Invalid pernr - {0}.", userXlData.ReferenceUserPernr) + "\n";
                    return log;
                }

                //clean user specific properties from reference user.
                user.Id = 0;
                user.AuthenticationMode = "Local";
                UserExtension usrExt = new UserExtension();
                usrExt.PositionCode = string.Empty;
                usrExt.PositionName = string.Empty;
                user.BusinessRoleId = null;
                user.CreatedBy = Constants.CreatedBy;
                user.CreatedDate = DateTime.Now;
                user.LastModifiedBy = null;
                user.LastModifiedDate = null;
                usrExt.EEGroup = null;

                user.EmailId = string.Empty;
                usrExt.NeedEmailUpdateToHR = false;
                usrExt.FLINSLoginId = string.Empty;
                usrExt.NeedFLINSLoginIdUpdateToHR = false;

                user.FirstName = string.Empty;
                user.LastName = string.Empty;
                user.MiddleName = string.Empty;

                user.Ethinicity = null;
                usrExt.EmployeeStatus = "1";
                usrExt.IsExecutiveEmployee = false;
                user.IsActive = true;
                user.FullTimeEmployee = false;
                user.Fax = null;
                user.HomephoneNumber = null;
                user.TelephoneNumber = null;
                user.MobileNumber = null;
                usrExt.OneKeyTokenId = null;
                user.PostalAddress = null;
                user.PostalCode = null;

                user.UserName = userXlData.ADLogin;
                user.DisplayName = userXlData.KnownAs;
                user.KnownAs = userXlData.KnownAs;
                user.UserExtension = usrExt.Serialize();
                string nric = userXlData.IdentificationNo;
                string joinDate = userXlData.JoinDate;
                string terminiationDate = userXlData.TerminationDate;
                string employeeType = userXlData.EmployeeType;

                if (string.IsNullOrEmpty(joinDate))
                {
                    user.JoinDate = DateTime.Now;
                }
                else
                {
                    DateTime tempDate = DateTime.Now;

                    if (DateTime.TryParse(joinDate, out tempDate))
                    {
                        user.JoinDate = tempDate;
                    }
                    else
                    {
                        user.JoinDate = DateTime.Now;
                    }
                }

                if (string.IsNullOrEmpty(terminiationDate))
                {
                    user.TerminationDate = DateTime.Now;
                }
                else
                {
                    DateTime tempDate = DateTime.Now;

                    if (DateTime.TryParse(terminiationDate, out tempDate))
                    {
                        user.TerminationDate = tempDate;
                    }
                    else
                    {
                        user.TerminationDate = DateTime.Now;
                    }
                }

                var employeeTerm = this.employeeTerms.FirstOrDefault(item => item.EmployeeTermDesc == employeeType);

                if (employeeTerm == null)
                {
                    employeeTerm = this.employeeTerms.FirstOrDefault();
                }

                user.EmployeeType = (EmployeeType)employeeTerm.Id;

                if (nric.Length == 9)
                {
                    user.NRIC = nric;
                }
                else if (nric.Length == 8)
                {
                    user.NRIC = nric + "X";
                }
                else if (nric.Length == 10)
                {
                    user.NRIC = nric.Remove(9, 1);
                }
                else
                {
                    int num = userFacade.GetAutoGeneratedNRICCount() + 1;
                    string number = num.ToString("00000000");

                    user.NRIC = "Z" + number;
                }

                user.EmployeeId = this.ConstructUserName(user);

                if (string.IsNullOrEmpty(user.UserName))
                {
                    user.UserName = user.EmployeeId;
                }

                log += string.Format("Actual NRIC - {0}, Generated NRIC - {1}", userXlData.IdentificationNo, user.NRIC) + "\n";
                log += string.Format("Actual Join Date - {0}, Generated Join Date - {1}", userXlData.JoinDate, user.JoinDate) + "\n";
                log += string.Format("Actual Termination Date - {0}, Generated Termination Date - {1}", userXlData.TerminationDate, user.TerminationDate) + "\n";
                log += string.Format("Generated Employee Id - {0}", user.EmployeeId) + "\n";

                 userFacade.InsertUser(user);

                log += string.Format("User created successfully - {0}", user.KnownAs) + "\n";

                this.TrackEventTriggers(user);
            }
            catch (Exception ex)
            {
                log += string.Format("Error while creating user - {0}, Error info - {1} - {2}", userXlData.KnownAs, ex.Source, ex.Message);
                log += string.Format("Stack Trace - {0}", ex.StackTrace);
            }

            return log;
        }

        private string ConstructUserName(User user)
        {
            string userName = string.Empty;
            StringBuilder userNameSB = new StringBuilder();

            try
            {
                string DisplayName = user.KnownAs;
                userName = removeSpecialCharacters(DisplayName, false);
                userName.TrimStart();


                userNameSB.Append("T");

                if (!string.IsNullOrEmpty(userName)) // && userName.Length >= 3)
                {
                    string[] nameAry = userName.Split(' ');
                    if (nameAry.Length == 1)
                    {
                        if (nameAry[0].Length >= 3)
                            userNameSB.Append(nameAry[0].Substring(0, 3));
                        else
                            userNameSB.Append(nameAry[0]);
                    }
                    else if (nameAry.Length == 2)
                    {
                        if (nameAry[0].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 2));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 1));
                        }
                        else if (nameAry[1].Length >= 2)
                        {
                            if (nameAry[0] != "")
                            {
                                userNameSB.Append(nameAry[0].Substring(0, 1));
                            }
                            userNameSB.Append(nameAry[1].Substring(0, 2));
                        }
                        else
                        {
                            userNameSB.Append(nameAry[0]);
                            userNameSB.Append(nameAry[1]);
                        }
                    }
                    else if (nameAry.Length >= 3)
                    {
                        if (nameAry[0] != "")
                        {
                            userNameSB.Append(nameAry[0].Substring(0, 1));
                        }
                        userNameSB.Append(nameAry[1].Substring(0, 1));
                        userNameSB.Append(nameAry[2].Substring(0, 1));
                    }
                }
                if (!string.IsNullOrEmpty(user.NRIC) && user.NRIC.Length >= 5)
                {
                    userNameSB.Append(user.NRIC.Remove(0, user.NRIC.Length - 4));
                }

                userName = userNameSB.ToString().ToUpper();

                userName = this.CheckUserNameExists(userName);
            }
            catch
            {
                throw;
            }

            return userName;
        }

        private string CheckUserNameExists(string userName)
        {
            int no = 1;
            if (userName.Length > 8)
                userName = userName.Substring(0, 8);
            bool isExist = new ActiveDirectoryFacade().CheckADUserNameExist(userName);
            while (isExist)
            {
                userName = userName.Substring(0, userName.Length - 1) + no;
                isExist = new ActiveDirectoryFacade().CheckADUserNameExist(userName);
                no++;
            }

            return userName;
        }

        private string removeSpecialCharacters(string inPutStr, bool isRemoveSpace)
        {
            StringBuilder outPutSB = new StringBuilder();
            char[] spChars = new char[] { '~', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '{', '}', '[', ']', '|', '\\', '<', '>', '?', ',', '.', '/' };

            inPutStr = inPutStr != null ? inPutStr.Trim() : string.Empty;
            if (inPutStr.Length > 4 && inPutStr.StartsWith("Bin ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 4 && inPutStr.StartsWith("bte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(4, inPutStr.Length - 4);
            }
            if (inPutStr.Length > 6 && inPutStr.StartsWith("binte ", StringComparison.CurrentCultureIgnoreCase))
            {
                inPutStr = inPutStr.Substring(6, inPutStr.Length - 6);
            }

            outPutSB.Append(inPutStr);

            outPutSB.Replace("S/O", string.Empty);
            outPutSB.Replace("S/o", string.Empty);
            outPutSB.Replace("s/O", string.Empty);
            outPutSB.Replace("s/o", string.Empty);
            outPutSB.Replace("s o", string.Empty);
            outPutSB.Replace("S O", string.Empty);

            outPutSB.Replace("D/O", string.Empty);
            outPutSB.Replace("D/o", string.Empty);
            outPutSB.Replace("d/O", string.Empty);
            outPutSB.Replace("d/o", string.Empty);
            outPutSB.Replace("d o", string.Empty);
            outPutSB.Replace("D O", string.Empty);

            foreach (char spChar in spChars)
            {
                outPutSB.Replace(spChar, ' ');
            }

            if (isRemoveSpace)
            {
                outPutSB.Replace(" ", string.Empty);
            }
            else
            {
                outPutSB.Replace("   ", string.Empty);
                outPutSB.Replace("  ", string.Empty);
            }

            return outPutSB.ToString();
        }


        private void TrackEventTriggers(User user)
        {
            //trigger off board event if the termination date is greater than equal to today during first time discovery
            if (user.TerminationDate.HasValue)
            {
                if (user.TerminationDate.Value.Date >= DateTime.Today)
                {
                    this.PopulateOffBoardTrigger(user);
                }
            }
        }

        private void PopulateOffBoardTrigger(User user)
        {
            var evnt = EmployeeEvents.Resignation;
            var trigger = triggerFacade.GetEmployeeEventTrigger(user.Id, evnt);

            if (trigger != null)
            {
                trigger.EffectiveDate = user.TerminationDate.Value;
                trigger.LastModifiedBy = trigger.CreatedBy;
                trigger.LastModifiedDate = DateTime.Now;

                triggerFacade.UpdateEmployeeEventTrigger(trigger);
            }
            else
            {
                EmployeeEventTrigger newTrigger = new EmployeeEventTrigger()
                {
                    CreatedBy = user.CreatedBy,
                    CreatedDate = user.CreatedDate.Value,
                    EffectiveDate = user.TerminationDate.Value,
                    EventType = evnt,
                    IsRequestCreated = false,
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    IsActive = true
                };

                triggerFacade.CreateEmployeeEventTrigger(newTrigger);
            }
        }


    }
}
