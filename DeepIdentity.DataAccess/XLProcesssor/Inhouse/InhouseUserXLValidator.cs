using DeepIdentity.Model;
using DeepIdentity.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;


namespace DeepIdentity.DataAccess
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
                        if (obj.UserName.Trim().Contains(SplChar[i].ToString()) || string.IsNullOrEmpty(obj.UserName))
                        {
                            missingFields.Add("Invalid User Name");
                            isValid = false;
                        }
                    }

                }
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
            return isValid;
        }

        public override bool IsExists()
        {
            InhouseUserInfo _existConfigMatrixAD = GetExistingUser();
            if (_existConfigMatrixAD == null)
                return false;
            Update();
            return true;
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
                        INHda.MapWithMasterUser(obj.EndPointSystemId);
                    }
                }
            }
            catch { throw; }
        }

    

        private InhouseUserInfo GetExistingUser()
        {

            InhouseUserInfo obj = Data as InhouseUserInfo;

            using (IACMDBContext _context = new IACMDBContext())
            {
                    return _context.InhouseUserInfos.FirstOrDefault(cnfg => cnfg.UserName == obj.UserName
                            && cnfg.InhouseSystemCode == obj.InhouseSystemCode);

            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                DateTime? date = null;
                int? DIUid = null;
                InhouseSystemConfiguration INHSystem = GetEndPointSystemID(CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()));
                int EPID = 0;
                if (INHSystem != null)
                    EPID = INHSystem.EndpointSystemId;
                    DateTime? AccountCreated = string.IsNullOrEmpty(dRow.ItemArray[10].ToString()) ? DateTime.Now : Convert.ToDateTime(dRow.ItemArray[10].ToString());
                    DateTime? LastLogin = string.IsNullOrEmpty(dRow.ItemArray[11].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[11].ToString());
                    bool isLocked = string.IsNullOrEmpty(dRow.ItemArray[12].ToString()) ? false : Convert.ToBoolean(dRow.ItemArray[12].ToString());
                    DateTime? Locked = string.IsNullOrEmpty(dRow.ItemArray[13].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[13].ToString());
                    bool isDisabled = string.IsNullOrEmpty(dRow.ItemArray[14].ToString()) ? false : Convert.ToBoolean(dRow.ItemArray[14].ToString());
                    DateTime? Disabled = string.IsNullOrEmpty(dRow.ItemArray[15].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[15].ToString());
                    bool isDeleted= string.IsNullOrEmpty(dRow.ItemArray[16].ToString())?false: Convert.ToBoolean(dRow.ItemArray[16].ToString());
                    DateTime? Deleted = string.IsNullOrEmpty(dRow.ItemArray[17].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[17].ToString());
                    byte LockScenario = string.IsNullOrEmpty(dRow.ItemArray[18].ToString()) ? (byte)0 : Convert.ToByte(dRow.ItemArray[18].ToString());
                    byte accountType = string.IsNullOrEmpty(dRow.ItemArray[19].ToString()) ? (byte)0 : Convert.ToByte(dRow.ItemArray[19].ToString());
                    DateTime Created = string.IsNullOrEmpty(dRow.ItemArray[21].ToString()) ? DateTime.Now : Convert.ToDateTime(dRow.ItemArray[21].ToString());
                    DateTime? LastModified = string.IsNullOrEmpty(dRow.ItemArray[23].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[23].ToString());
                    int? DIUId = string.IsNullOrEmpty(dRow.ItemArray[29].ToString()) ? DIUid : Convert.ToInt32(dRow.ItemArray[29].ToString());
                return new InhouseUserInfo
                {

                    TenantId = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[2].ToString()),
                    NRIC = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    EmployeeId = CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    Email = CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    UserName = CheckNullAndGetValueFromCell(dRow.ItemArray[6].ToString()),
                    DisplayName = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                    FirstName = CheckNullAndGetValueFromCell(dRow.ItemArray[8].ToString()),
                    LastName = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString()),
                    AccountCreatedDate = AccountCreated,
                    LastLoginDate = LastLogin,
                    IsLocked = isLocked,
                    LockedDate = Locked,
                    IsDisabled = isDisabled,
                    DisabledDate = Disabled,
                    IsDeleted= isDeleted,
                    DeletedDate = Deleted,
                    AccountType= (AccountType)accountType,
                    LockScenario= (LockScenario)LockScenario,
                    CreatedBy= CheckNullAndGetValueFromCell(dRow.ItemArray[20].ToString()),
                    CreatedDate = Created,
                    LastModifiedBy= CheckNullAndGetValueFromCell(dRow.ItemArray[22].ToString()),
                    LastModifiedDate = LastModified,
                    Remarks1= CheckNullAndGetValueFromCell(dRow.ItemArray[24].ToString()),
                    Remarks2= CheckNullAndGetValueFromCell(dRow.ItemArray[25].ToString()),
                    Remarks3= CheckNullAndGetValueFromCell(dRow.ItemArray[26].ToString()),
                    Remarks4= CheckNullAndGetValueFromCell(dRow.ItemArray[27].ToString()),
                    Remarks5= CheckNullAndGetValueFromCell(dRow.ItemArray[28].ToString()),
                    DIUserId = DIUId,
                    Permission = CheckNullAndGetValueFromCell(dRow.ItemArray[30].ToString()),
                    EndPointSystemId = EPID
                };
            }
            catch (Exception)
            {

                return null;
            }
        }

        private InhouseSystemConfiguration GetEndPointSystemID(string SystemCode)
        {
            return new InhouseSystemDA().GetInhouseSystemBySystemCode(SystemCode);
        }

        private void Update()
        {
            try
            {
                InhouseUserInfo obj = Data as InhouseUserInfo;
                InhouseSystemDA INHda = new InhouseSystemDA();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    InhouseUserInfo editUser = (from InhouseUserInfos in _context.InhouseUserInfos
                                join epSystems in _context.EndpointSystems
                                on InhouseUserInfos.EndPointSystemId equals epSystems.EndpointSystemId
                                where InhouseUserInfos.UserName == obj.UserName select InhouseUserInfos).FirstOrDefault();

                    if (editUser != null)
                    {
                        obj.UserId = editUser.UserId;
                        obj.LastModifiedDate = DateTime.Now;
                        _context.Entry(editUser).CurrentValues.SetValues(obj);
                        _context.SaveChanges();
                        INHda.MapWithMasterUser(obj.EndPointSystemId);
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
