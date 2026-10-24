using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ApprovalExceptionInfoXLValidator: XLDataValidator
    {
        public override IXLData GetObjectFromData(DataRow dRow)
        {
            ApproverExceptionInfo _approverException = null;
            try
            {
                _approverException = new ApproverExceptionInfo();
                _approverException.Approver = FormatEmployeeID(CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()));
                _approverException.Person = FormatEmployeeID(CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()));
                _approverException.ApprovalLevel = Convert.ToInt32(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()));
                return _approverException;

            }
            catch (Exception)
            {

                return null;
            }
        }

        public override Type GetDomainModelType()
        {
            return typeof(ApproverExceptionInfo);
        }
        public override void Add(string currentUser)
        {
            try
            {
                ApproverExceptionInfo obj = this.Data as ApproverExceptionInfo;
                obj.ValidFrom = DateTime.Now;
                obj.ValidTo = DateTime.MaxValue;
                obj.IsValid = true;
                obj.CreatedBy = currentUser;
                obj.CreatedDate = DateTime.Now;
                obj.Save(currentUser);
            }
            catch { throw; }
        }

        public override void Update(string currentUser)
        {
            Add(currentUser);
        }

        public override string GetWhoUpdatedData()
        {
            return ApproverExceptionInfo.GetWhoUpdatedData();
        }

        public override bool IsExists(string currentUser)
        {
            this.ExistingData = GetApproverExceptionInfobyObject();
            if (this.ExistingData == null)
                return false;
            else
                return true;
        }

        private ApproverExceptionInfo GetApproverExceptionInfobyObject()
        {
            return ApproverExceptionInfo.GetApproverExceptionInfobyObject(this.Data as ApproverExceptionInfo);
        }

        public override bool TruncateTable()
        {
            TableName = "DI_M_XLApproverExceptionInfo";
            return base.TruncateTable();
            //return ApproverExceptionInfo.TruncateTable();
        }

        public override bool IsEmptyRow(IXLData data)
        {
            ApproverExceptionInfo _approverException = data as ApproverExceptionInfo;

            if (string.IsNullOrEmpty(_approverException.Approver + _approverException.Person + _approverException.ApprovalLevel))
            {
                return true;
            }
            return false;
        }

        public override bool Validate(ref string validationMsg)
        {
            ApproverExceptionInfo obj = this.Data as ApproverExceptionInfo;
            bool isExist = false;
            string msg1 = string.Empty;
            string msg2 = string.Empty;
            string msg3 = string.Empty;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            if (string.IsNullOrEmpty(obj.Approver))
            {
                missingFields.Add("Approver");
            }


            if (string.IsNullOrEmpty(obj.Person))
            {

                missingFields.Add("Person");
            }

            if (obj.ApprovalLevel == 0)
            {

                missingFields.Add("ApprovalLevel");
            }

            if (!string.IsNullOrEmpty(obj.Approver))
            {
                isExist = this.IsUserValid(obj.Approver, ApprovalUserType.User);
                if (!isExist)
                {
                    missingFields.Add("Approver");

                }
            }
            if (!string.IsNullOrEmpty(obj.Person))
            {
                isExist = this.IsUserValid(obj.Person, ApprovalUserType.User);
                if (!isExist)
                {
                    missingFields.Add("Person");
                }
            }
            if (obj.ApprovalLevel != null)
            {
                isExist = this.IsValidApproverLevel(obj.ApprovalLevel);
                if (!isExist)
                {
                    missingFields.Add("ApprovalLevel");
                }
            }

            if (missingFields.Count == 0)
            {
                if (CheckPesronHasSameapproversForSameLevel())
                {
                    if (missingFields.Count == 0)
                    {
                        msg3 = "Person has Same Approver for Same level";
                        missingFields.Add(msg3);
                    }
                }
                if (CheckPesronHasDiffrentapproversForSameLevel())
                {
                    if (missingFields.Count == 0)
                    {
                        msg1 = "Person has diffrent Approver for Same level";
                        missingFields.Add(msg1);
                    }
                }

                if (CheckPesronHasSameApproverDiffentLevel())
                {
                    if (missingFields.Count == 0)
                    {
                        msg2 = "Person has Same Approver for Diffrent level";
                        missingFields.Add(msg2);
                    }
                }
            }

            int cnt = 0;
            foreach (string field in missingFields)
            {
                if (cnt == 0)
                    validationMsg = field;
                else if (cnt > 0 && cnt != missingFields.Count - 1)
                    validationMsg = validationMsg + ", " + field;
                else if (cnt == missingFields.Count - 1)
                    validationMsg = validationMsg + " & " + field;
                cnt++;
            }
            if (cnt > 0)
            {
                if (validationMsg == msg1)
                {
                    validationMsg = msg1;
                }
                else if (validationMsg == msg2)
                {
                    validationMsg = msg2;
                }
                else if (validationMsg == msg3)
                {
                    validationMsg = msg3;
                }
                else
                {
                    validationMsg = validationMsg + " Not Found ";

                }
            }

            return missingFields.Count == 0;
        }

        private bool CheckPesronHasDiffrentapproversForSameLevel()
        {
            ApproverExceptionInfo obj = this.Data as ApproverExceptionInfo;
            return ApproverExceptionInfo.CheckPesronHasDiffrentapproversForSameLevel(obj);
        }
        private bool CheckPesronHasSameapproversForSameLevel()
        {
            ApproverExceptionInfo obj = this.Data as ApproverExceptionInfo;
            return ApproverExceptionInfo.CheckPesronHasSameapproversForSameLevel(obj);
        }

        private bool CheckPesronHasSameApproverDiffentLevel()
        {
            ApproverExceptionInfo obj = this.Data as ApproverExceptionInfo;
            return ApproverExceptionInfo.CheckPesronHasSameApproverDiffentLevel(obj);
        }
    }
}
