using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.DataAccess
{
    public class InhousePermissionXLValidator : XLDataValidator, IValidator
    {
        
        public override bool Validate(ref string validationMsg)
        {
            InhousePermission obj = Data as InhousePermission;

            bool isValid = true;
            bool isExist = false;
            List<string> missingFields = new List<string>();
            missingFields.Clear();

            using (IACMDBContext _context = new IACMDBContext())
            {
                if (_context.InhouseSystems.Count() > 0)
                {
                    isExist = _context.EndpointSystems.Any(dpt => dpt.ApplicationCode == obj.InhouseSystemCode);
                    if (!isExist)
                    {
                        isExist = !string.IsNullOrEmpty(obj.InhouseSystemCode);
                    }
                    isValid = isValid & isExist;
                    if (isExist == false)
                        missingFields.Add("Invalid InhouseSystem Code");
                }
                else
                {
                    isExist = !string.IsNullOrEmpty(obj.Name);
                    isValid = isValid & isExist;
                    if (isExist == false)
                        missingFields.Add("Invalid Permission Name");
                }


                isExist = _context.EndpointSystems.Any(dpt => dpt.ApplicationCode == obj.InhouseSystemCode);
                if (!isExist) isExist = string.IsNullOrEmpty(obj.InhouseSystemCode);
                isValid = isValid & isExist;
                if (isExist == false)
                    missingFields.Add("Invalid InhouseSystem Code");

                if (string.IsNullOrEmpty(obj.Name))
                {
                    missingFields.Add("Invalid Permission Name");
                    isValid = false;
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
            InhousePermission _existConfigMatrixAD = GetExistingUser();
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
                    _context.Database.ExecuteSqlCommand("Truncate table DI_INH_Permission");
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
                InhousePermission obj = Data as InhousePermission;
                InhouseSystemDA INHda = new InhouseSystemDA();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (obj != null)
                    {
                        _context.InhousePermissions.Add(obj);
                        _context.SaveChanges();
                    }
                }
            }
            catch { throw; }
        }

       
        private InhousePermission GetExistingUser()
        {

            InhousePermission obj = Data as InhousePermission;

            using (IACMDBContext _context = new IACMDBContext())
            {
                return _context.InhousePermissions.FirstOrDefault(cnfg => cnfg.Name == obj.Name
                        && cnfg.InhouseSystemCode == obj.InhouseSystemCode);
            }
        }

        public override IXLData GetObjectFromData(DataRow dRow)
        {
            try
            {
                DateTime? date = null;
                InhouseSystemConfiguration INHSystem;
                if(!string.IsNullOrEmpty(dRow.ItemArray[3].ToString()))
                    INHSystem = GetEndPointSystemID(CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()));
                else
                    INHSystem=null;
                int EPID = 0;
                if (INHSystem != null)
                    EPID = INHSystem.EndpointSystemId;

                bool active = string.IsNullOrEmpty(dRow.ItemArray[6].ToString()) ? false : Convert.ToBoolean(dRow.ItemArray[6].ToString());
                DateTime Created = string.IsNullOrEmpty(dRow.ItemArray[8].ToString()) ? DateTime.Now : Convert.ToDateTime(dRow.ItemArray[8].ToString());
                DateTime? LastModified = string.IsNullOrEmpty(dRow.ItemArray[10].ToString()) ? date : Convert.ToDateTime(dRow.ItemArray[10].ToString());
                return new InhousePermission
                {

                    TenantId = CheckNullAndGetValueFromCell(dRow.ItemArray[1].ToString()),
                    EndPointSystemID=EPID,
                    InhouseSystemCode = CheckNullAndGetValueFromCell(dRow.ItemArray[3].ToString()),
                    Name=CheckNullAndGetValueFromCell(dRow.ItemArray[4].ToString()),
                    Description=CheckNullAndGetValueFromCell(dRow.ItemArray[5].ToString()),
                    IsActive=active,
                    CreatedBy = CheckNullAndGetValueFromCell(dRow.ItemArray[7].ToString()),
                    CreatedDate = Created,
                    LastModifiedBy = CheckNullAndGetValueFromCell(dRow.ItemArray[9].ToString()),
                    LastModifiedDate = LastModified
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

        public override bool IsEmptyRow(IXLData data)
        {
            InhousePermission _config = data as InhousePermission;

            if (string.IsNullOrEmpty(_config.InhouseSystemCode + _config.Name))
            {
                return true;
            }
            return false;
        }

        private void Update()
        {
            try
            {
                InhousePermission obj = Data as InhousePermission;
                InhouseSystemDA INHda = new InhouseSystemDA();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    InhousePermission editPerm = (from inhousePermissions in _context.InhousePermissions
                                join epSystems in _context.EndpointSystems
                                on inhousePermissions.EndPointSystemID equals epSystems.EndpointSystemId
                                                  where inhousePermissions.Name == obj.Name
                                                  select inhousePermissions).FirstOrDefault();

                    if (editPerm != null)
                    {
                        obj.Id = editPerm.Id;
                        obj.LastModifiedDate = DateTime.Now;
                        _context.Entry(editPerm).CurrentValues.SetValues(obj);
                    }
                    _context.SaveChanges();
                }
            }
            catch { throw; }
        }
    }
}
