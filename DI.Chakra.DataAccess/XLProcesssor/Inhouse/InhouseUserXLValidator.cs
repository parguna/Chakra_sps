using DI.Chakra.Model;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;


namespace DI.Chakra.DataAccess
{
    public class InhouseUserXLValidator: XLDataValidator, IValidator
    {
       
        public override bool Validate(ref string validationMsg)
        {
            InhouseUserInfo obj = Data as InhouseUserInfo;

            bool isValid = true;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            using (IACMDBContext _context = new IACMDBContext())
            {
                if (_context.InhouseUserInfos.Count() > 0)
                {
                    isExist = _context.EndpointSystems.Any(dpt => dpt.ApplicationCode == obj.InhouseSystemCode);
                    if (!isExist) isExist = string.IsNullOrEmpty(obj.InhouseSystemCode);
                    isValid = isValid & isExist;
                    if (isExist == false)
                        missingFields.Add("Invalid InhouseSystem Code");

                    string SplChar = "!@#$%^&*()+=[]\\\';,./{}|\":<>? ";
                    for (int i = 0; i < SplChar.Length; i++)
                    {
                        if (obj.UserName.Trim().Contains(SplChar[i].ToString()) && string.IsNullOrEmpty(obj.UserName))
                        {
                            missingFields.Add("Invalid User Name");
                            isValid = false;
                        }
                    }
                }


                isExist = (!string.IsNullOrEmpty(obj.InhouseSystemCode)) && IsEndPointSystemIdValid(obj.InhouseSystemCode);
                if (!isExist)
                    missingFields.Add("EndPointSystem Id Not Found");

                isExist = (!string.IsNullOrEmpty(obj.InhouseSystemCode)) && IsInhouseSystemCodeValid(obj.InhouseSystemCode);
                if (!isExist)
                    missingFields.Add("INHouseSytem Code Not Found");

                isExist = (!string.IsNullOrEmpty(obj.NRIC)) && IsDuplicateInHouseFields(obj.NRIC, "NRIC");
                if (!isExist)
                    missingFields.Add("Duplicate NRIC Code");

                isExist = (!string.IsNullOrEmpty(obj.EmployeeId)) && IsDuplicateInHouseFields(obj.EmployeeId, "EmpId");
                if (!isExist)
                    missingFields.Add("Duplicate EmployeeId");

                string[] strPerm = obj.Permission.Split(',');

                foreach (string Perm in strPerm)
                {
                    string perm = Perm.Trim();
                    if(perm!="")
                    isExist = _context.InhousePermissions.Any(dpt => dpt.Name == perm);
                    isValid = isValid & isExist;
                    if (isExist == false)
                    {
                        missingFields.Add("Invalid Permission Name");
                        break;
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
            //if (cnt > 0)
            //    validationMsg = validationMsg + " Not Found ";

            return isValid;
        }

        public bool IsEndPointSystemIdValid(string value)  
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Any(item => item.ApplicationCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public bool IsInhouseSystemCodeValid(string value) 
        {
            try
            {
                using (var _context = new IACMDBContext())
                {
                    return _context.EndpointSystems.Any(item => item.ApplicationCode == value);
                }
            }
            catch
            {
                return false;
            }
        }

        public int getEndPointSystemIdbyApplicationCode(string value)
        {
            int EndPointSystemIdValue = 0;
            try
            {
                
                using (var _context = new IACMDBContext())
                {
                    EndPointSystemIdValue =Convert.ToInt32(_context.EndpointSystems.FirstOrDefault(item => item.ApplicationCode == value));
                }
                return EndPointSystemIdValue;
            }
            catch
            {
                return EndPointSystemIdValue;
            }
        }

        public override bool IsExists()
        {
            InhouseUserInfo _existConfigMatrixAD = GetExistingUser();
            if (_existConfigMatrixAD == null)
                return false;
            Update();
            return true;
        }

        public bool IsDuplicateInHouseFields(string value,string Mode)   
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                InhouseUserInfo _existNRIC = null;
                if (Mode == "NRIC")
                {
                    _existNRIC = _context.InhouseUserInfos.FirstOrDefault(cnfg => cnfg.NRIC == value);
                }
                else if (Mode == "EmpId")
                {
                    _existNRIC = _context.InhouseUserInfos.FirstOrDefault(cnfg => cnfg.EmployeeId == value);
                }

                if (_existNRIC == null)
                    return true;
                else
                    return false;
            }
        }

        public override bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Database.ExecuteSqlCommand("Truncate table DI_INH_UserInfo");
                    returnValue = true;
                }
            }
            catch
            {
            }
            return returnValue;
        }

        public override void Add()
        {
            try
            {
                InhouseUserInfo obj = Data as InhouseUserInfo;
                InhouseSystemDA INHda = new InhouseSystemDA();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (obj != null)
                    {
                        _context.InhouseUserInfos.Add(obj);
                        _context.SaveChanges();
                        long CurrentUser = obj.Id; 
                        INHda.MapWithMasterUser((int)obj.EndPointSystemId);
                        SaveAccountReconcilationWhileUpload(obj, CurrentUser);
                    }
                }
            }
            catch { throw; }
        }

      private void  SaveAccountReconcilationWhileUpload(InhouseUserInfo oINH ,long CurrentUserId)   
      {
          try
          {
              AccountReconcilation oAR = new AccountReconcilation();

              using (IACMDBContext _db= new IACMDBContext()) 
              {
                  oAR.EndpointSystemId = oINH.EndPointSystemId;
                  oAR.EndpointUserId =Convert.ToInt32(oINH.EmployeeId);
                  oAR.UserId = CurrentUserId;
                  oAR.AccountType = oINH.AccountType;
                  oAR.AccountName = oINH.AccountName;
                  oAR.FullName = oINH.DisplayName;
                  oAR.CreatedDate = oINH.CreatedDate;
                  _db.AccountReconcilations.Add(oAR);
                  _db.SaveChanges();
              }

          }
          catch (Exception)
          {
          }
      }

        private InhouseUserInfo GetExistingUser()
        {

            InhouseUserInfo obj = Data as InhouseUserInfo;

            using (IACMDBContext _context = new IACMDBContext())
            {
                    return _context.InhouseUserInfos.FirstOrDefault(cnfg => cnfg.EmployeeId == obj.EmployeeId
                            && cnfg.InhouseSystemCode == obj.InhouseSystemCode && obj.EmployeeId != null && obj.EmployeeId != "");

            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                DateTime? date = null;
                int? DIUid = null;
                string strUserName = string.Empty;
                InhouseSystemConfiguration INHSystem = GetInhouseSystemBySystemCode(CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()));
                long EPID = 0;
                if (INHSystem != null)
                    EPID = INHSystem.EndpointSystemId;
                    DateTime? AccountCreated = string.IsNullOrEmpty(dRow.ItemArray[8].ToString()) ? DateTime.Now : Convert.ToDateTime(dRow.ItemArray[9].ToString());
                    DateTime? LastLogin = string.IsNullOrEmpty(dRow.ItemArray[9].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[10].ToString());
                    //bool isLocked = string.IsNullOrEmpty(dRow.ItemArray[12].ToString()) ? false : Convert.ToBoolean(dRow.ItemArray[12].ToString());
                    //DateTime? Locked = string.IsNullOrEmpty(dRow.ItemArray[13].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[13].ToString());
                    //bool isDisabled = string.IsNullOrEmpty(dRow.ItemArray[14].ToString()) ? false : Convert.ToBoolean(dRow.ItemArray[14].ToString());
                    //DateTime? Disabled = string.IsNullOrEmpty(dRow.ItemArray[15].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[15].ToString());
                    //bool isDeleted= string.IsNullOrEmpty(dRow.ItemArray[16].ToString())?false: Convert.ToBoolean(dRow.ItemArray[16].ToString());
                    //DateTime? Deleted = string.IsNullOrEmpty(dRow.ItemArray[17].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[17].ToString());
                    //byte LockScenario = string.IsNullOrEmpty(dRow.ItemArray[18].ToString()) ? (byte)0 : Convert.ToByte(dRow.ItemArray[18].ToString());
                    byte accountType = string.IsNullOrEmpty(dRow.ItemArray[11].ToString()) ? (byte)0 : Convert.ToByte(dRow.ItemArray[11].ToString());
                    DateTime Created = string.IsNullOrEmpty(dRow.ItemArray[17].ToString()) ? DateTime.Now : Convert.ToDateTime(dRow.ItemArray[17].ToString());
                    DateTime? LastModified = string.IsNullOrEmpty(dRow.ItemArray[18].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[18].ToString());
                    //int? DIUId = string.IsNullOrEmpty(dRow.ItemArray[29].ToString()) ? DIUid : Convert.ToInt32(dRow.ItemArray[29].ToString());
                    strUserName = dRow.ItemArray[5].ToString();
                    
                return new InhouseUserInfo
                {

                    TenantId = CheckNullAndGetValueFromCell(dRow.ItemArray[0].ToString()),
                    EndPointSystemId = EPID,
                    InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    NRIC = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                    EmployeeId = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    Email = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    UserName = strUserName,
                    DisplayName = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                    FirstName = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                    LastName = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString()),
                    AccountCreatedDate = AccountCreated,
                    LastLoginDate = LastLogin,
                    //IsLocked = isLocked,
                    //LockedDate = Locked,
                    //IsDisabled = isDisabled,
                    //DisabledDate = Disabled,
                    //IsDeleted= isDeleted,
                    //DeletedDate = Deleted,
                    AccountType= (AccountType)accountType,
                    //LockScenario= (LockScenario)LockScenario,
                    Remarks1 = CheckNullAndGetValueFromCell(dRow.ItemArray[12].ToString()),
                    Remarks2 = CheckNullAndGetValueFromCell(dRow.ItemArray[13].ToString()),
                    Remarks3 = CheckNullAndGetValueFromCell(dRow.ItemArray[14].ToString()),
                    Remarks4 = CheckNullAndGetValueFromCell(dRow.ItemArray[15].ToString()),
                    Remarks5 = CheckNullAndGetValueFromCell(dRow.ItemArray[16].ToString()),
                    CreatedBy= CheckNullAndGetValueFromCell(dRow.ItemArray[17].ToString()),
                    CreatedDate = Created,
                    Permission = CheckNullAndGetValueFromCell(dRow.ItemArray[19].ToString()),
                    Version =Convert.ToInt32(CheckNullAndGetValueFromCell(dRow.ItemArray[20].ToString()))
                };
            }
            catch (Exception)
            {

                return null;
            }
        }

        private InhouseSystemConfiguration GetInhouseSystemBySystemCode(string SystemCode)
        {
            return new InhouseSystemDA().GetInhouseSystemBySystemCode(SystemCode); 
        }

        private void Update()
        {
            try
            {
                InhouseUserInfo obj = Data as InhouseUserInfo;
                AccountReconcilation objA =new AccountReconcilation(); 
                InhouseSystemDA INHda = new InhouseSystemDA();

                using (IACMDBContext _context = new IACMDBContext())
                {
                    InhouseUserInfo editUser = (from InhouseUserInfos in _context.InhouseUserInfos
                                join epSystems in _context.EndpointSystems
                                on InhouseUserInfos.EndPointSystemId equals epSystems.Id
                                where InhouseUserInfos.EmployeeId == obj.EmployeeId select InhouseUserInfos).FirstOrDefault();

                    if (editUser != null)
                    {
                        obj.Id = editUser.Id;
                        obj.LastModifiedDate = DateTime.Now;
                        _context.Entry(editUser).CurrentValues.SetValues(obj);
                        _context.SaveChanges();
                        INHda.MapWithMasterUser((int)obj.EndPointSystemId);
                    }

                    AccountReconcilation editAccRe = (from accre in _context.AccountReconcilations 
                                                      join epSystems in _context.EndpointSystems
                                                      on accre.EndpointSystemId equals epSystems.Id
                                                      where accre.UserId == editUser.Id
                                                      select accre).FirstOrDefault();

                    if (editAccRe != null)
                    {
                        objA.Id = editAccRe.Id;
                        objA.EndpointSystemId = editUser.EndPointSystemId;
                        objA.UserId = editUser.Id;
                        objA.EndpointUserId =Convert.ToInt32(editUser.EmployeeId);
                        objA.CreatedDate = editAccRe.CreatedDate;
                        objA.FullName = editUser.DisplayName;
                        objA.AccountType = editUser.AccountType;
                        objA.AccountName = editUser.AccountName;
                        _context.Entry(editAccRe).CurrentValues.SetValues(objA); 
                        _context.SaveChanges();
                    }
                }
            }
            catch { throw; }
        }
        public override bool IsEmptyRow(IXLData data)
        {
            InhouseUserInfo _config = data as InhouseUserInfo;

            if (string.IsNullOrEmpty(_config.InhouseSystemCode + _config.UserName))
            {
                return true;
            }
            return false;
        }

    }
}
