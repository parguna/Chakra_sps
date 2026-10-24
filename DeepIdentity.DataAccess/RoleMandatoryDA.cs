using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using DeepIdentity.Model;
using System.Data.OleDb;
using System.IO;

namespace DeepIdentity.DataAccess
{

    public class RoleMandatoryDA
    {
        public void InsertSapExcel(DataTable ds, string ClassName)
        {

            try
            {

                switch (ClassName)
                {
                    case "DI_SAP_RoleMandatoryFields":
                        {
                            using (IACMDBContext _context = new IACMDBContext())
                            {
                                List<string> coloumnNames = new List<string>{"Business Role","MandatorField1","MandatoryField2"};
                               
                                Boolean IsValid = checkIsValid(coloumnNames, ds);
                                if (IsValid)
                                {
                                    foreach (DataRow dr in ds.Rows)
                                    {
                                        SAPRoleMandatoryField objdbSetRoleMandatoryField = new SAPRoleMandatoryField();
                                        string strBusinessRole = Convert.ToString(dr[0]);
                                        var OldItems = (from Mandatories in _context.SAPRoleMandatoryFields
                                                        where Mandatories.ValidTo == null &&
                                                            Mandatories.BusinessRole == strBusinessRole
                                                        select Mandatories).FirstOrDefault();

                                        if (OldItems != null)
                                        {
                                            objdbSetRoleMandatoryField.ReferenceId = OldItems.Id;
                                            OldItems.ValidTo = DateTime.Now;
                                            OldItems.IsModifiedbyExcelUpload = true;
                                        }

                                        objdbSetRoleMandatoryField.BusinessRole = Convert.ToString(dr[0]);
                                        objdbSetRoleMandatoryField.MandatoryField1 = Convert.ToString(dr[1]);
                                        objdbSetRoleMandatoryField.MandatoryField2 = Convert.ToString(dr[2]);
                                        objdbSetRoleMandatoryField.ValidFrom = DateTime.Now;

                                        _context.SAPRoleMandatoryFields.Add(objdbSetRoleMandatoryField);
                                        _context.SaveChanges();
                                    }
                                }
                                else
                                {
                                    throw new Exception("Invalid File");
                                }
                            }

                            break;
                        }
                    case "DI_SAP_RoleMatrix":
                        {

                            using (IACMDBContext _context = new IACMDBContext())
                            {
                                List<string> coloumnNames = new List<string> { "Application", "Eligiblity Rule", "Company","Personnel Area","Personal Sub Area","Position","Org Unit","Cost center","Remarks" };
                                
                                Boolean IsValid = checkIsValid(coloumnNames, ds);
                                if (IsValid)
                                {
                                foreach (DataRow dr in ds.Rows)
                                {

                                    SAPRoleMatrix DISAPRoleMatrix = new SAPRoleMatrix();

                                    string strApplication = Convert.ToString(dr[0]);
                                    string strEligibilityRule = Convert.ToString(dr[1]);
                                    string strCompany = Convert.ToString(dr[2]);
                                    string strPersonnelArea = Convert.ToString(dr[3]);
                                    string strpersonnelSubArea = Convert.ToString(dr[4]);
                                    string strPosition = Convert.ToString(dr[5]);
                                    string strOrgUnit = Convert.ToString(dr[6]);
                                    string strCostCenter = Convert.ToString(dr[7]);

                                    var OldItems = (from Matrices in _context.SAPRoleMatrices
                                                    where Matrices.ValidTo == null
                                                        && Matrices.Application.Trim() == strApplication.Trim()
                                                        && Matrices.EligibilityRule.Trim() == strEligibilityRule.Trim()
                                                        && Matrices.Company.Trim() == strCompany.Trim()
                                                        && Matrices.PersonnelArea.Trim() == strPersonnelArea.Trim()
                                                        && Matrices.PersonnelSubArea.Trim() == strpersonnelSubArea.Trim()
                                                        && Matrices.Position.Trim() == strPosition.Trim()
                                                        && Matrices.OrgUnit.Trim() == strOrgUnit.Trim()
                                                        && Matrices.CostCenter.Trim() == strCostCenter.Trim()
                                                    select Matrices).FirstOrDefault();

                                    if (OldItems != null)
                                    {

                                        OldItems.ValidTo = DateTime.Now;
                                        OldItems.IsUpdatedbyExcel = true;
                                        DISAPRoleMatrix.ReferenceId = OldItems.Id;

                                    }


                                    DISAPRoleMatrix.Application = Convert.ToString(dr[0]);
                                    DISAPRoleMatrix.EligibilityRule = Convert.ToString(dr[1]);
                                    DISAPRoleMatrix.Company = Convert.ToString(dr[2]);
                                    DISAPRoleMatrix.PersonnelArea = Convert.ToString(dr[3]);
                                    DISAPRoleMatrix.PersonnelSubArea = Convert.ToString(dr[4]);
                                    DISAPRoleMatrix.Position = Convert.ToString(dr[5]);
                                    DISAPRoleMatrix.OrgUnit = Convert.ToString(dr[6]);
                                    DISAPRoleMatrix.CostCenter = Convert.ToString(dr[7]);
                                    DISAPRoleMatrix.Remarks = Convert.ToString(dr[8]);
                                    DISAPRoleMatrix.ValidFrom = DateTime.Now;
                                    _context.SAPRoleMatrices.Add(DISAPRoleMatrix);
                                    _context.SaveChanges();
                                }
                                }
                                else
                                {
                                    throw new Exception("Invalid File");
                                }
                            }
                            break;
                        }
                    case "DI_SAP_HRFLINSRoleMatrix":
                        {
                            using (IACMDBContext _context = new IACMDBContext())
                            {
                                List<string> coloumnNames = new List<string> { "Remarks", "Application", "Company","Cost center","Purchasing Org","Purchasing Grp","Plant","MRP Controller","Storage Location","Module","Sub Module","Business Role","Technical Role","Level 1 Approver","Level 2 Approver","Level 3 Approver","MainSupp" };
                                
                                Boolean IsValid = checkIsValid(coloumnNames, ds);
                                if (IsValid)
                                {
                                    foreach (DataRow dr in ds.Rows)
                                    {
                                        SAPHRFLINSRoleMatrix objHRFLINSRoleMatrix = new SAPHRFLINSRoleMatrix();

                                        string strRemarks = Convert.ToString(dr[0]);
                                        string strApplication = Convert.ToString(dr[1]);
                                        string strCompany = Convert.ToString(dr[2]);
                                        string strCostCenter = Convert.ToString(dr[3]);
                                        string strPurchasingOrg = Convert.ToString(dr[4]);
                                        string strPurchasingGrp = Convert.ToString(dr[5]);
                                        string strPlant = Convert.ToString(dr[6]);
                                        string strMRPControl = Convert.ToString(dr[7]);
                                        string strStorageLocation = Convert.ToString(dr[8]);
                                        string strModule = Convert.ToString(dr[9]);
                                        string strSubModule = Convert.ToString(dr[10]);
                                        string strBusinessRole = Convert.ToString(dr[11]);
                                        string strTechnicalRule = Convert.ToString(dr[12]);
                                        string strMainSupp = Convert.ToString(dr[13]);

                                        var OldItems = (from HRLINSRoleMatrix in _context.SAPHRFLINSRoleMatrices
                                                        where HRLINSRoleMatrix.ValidTo == null
                                                            && HRLINSRoleMatrix.Remarks == strRemarks
                                                            && HRLINSRoleMatrix.Application == strApplication
                                                            && HRLINSRoleMatrix.Company == strCompany
                                                            && HRLINSRoleMatrix.CostCenter == strCostCenter
                                                            && HRLINSRoleMatrix.PurchasingOrg == strPurchasingOrg
                                                            && HRLINSRoleMatrix.PurchasingGrp == strPurchasingGrp
                                                            && HRLINSRoleMatrix.Plant == strPlant
                                                            && HRLINSRoleMatrix.MRPControl == strMRPControl
                                                            && HRLINSRoleMatrix.StorageLocation == strStorageLocation
                                                            && HRLINSRoleMatrix.Module == strModule
                                                            && HRLINSRoleMatrix.SubModule == strSubModule
                                                            && HRLINSRoleMatrix.BusinessRole == strBusinessRole
                                                            && HRLINSRoleMatrix.TechnicalRule == strTechnicalRule
                                                            && HRLINSRoleMatrix.MainSupp == strMainSupp
                                                        select HRLINSRoleMatrix).FirstOrDefault();
                                        if (OldItems != null)
                                        {
                                            OldItems.ValidTo = DateTime.Now;
                                            OldItems.IsUpdatedbyExcelUpload = true;
                                            objHRFLINSRoleMatrix.ReferenceId = OldItems.Id;
                                        }


                                        objHRFLINSRoleMatrix.Remarks = strRemarks;
                                        objHRFLINSRoleMatrix.Application = strApplication;
                                        objHRFLINSRoleMatrix.Company = strCompany;
                                        objHRFLINSRoleMatrix.CostCenter = strCostCenter;
                                        objHRFLINSRoleMatrix.PurchasingOrg = strPurchasingOrg;
                                        objHRFLINSRoleMatrix.PurchasingGrp = strPurchasingGrp;
                                        objHRFLINSRoleMatrix.Plant = strPlant;
                                        objHRFLINSRoleMatrix.MRPControl = strMRPControl;
                                        objHRFLINSRoleMatrix.StorageLocation = strStorageLocation;
                                        objHRFLINSRoleMatrix.Module = strModule;
                                        objHRFLINSRoleMatrix.SubModule = strSubModule;
                                        objHRFLINSRoleMatrix.BusinessRole = strBusinessRole;
                                        objHRFLINSRoleMatrix.TechnicalRule = strTechnicalRule;
                                        objHRFLINSRoleMatrix.Level1Approver = Convert.ToString(dr[13]);
                                        objHRFLINSRoleMatrix.Level2Approver = Convert.ToString(dr[14]);
                                        objHRFLINSRoleMatrix.Level3Approver = Convert.ToString(dr[15]);
                                        objHRFLINSRoleMatrix.MainSupp = strMainSupp;
                                        objHRFLINSRoleMatrix.ValidFrom = DateTime.Now;
                                        _context.SAPHRFLINSRoleMatrices.Add(objHRFLINSRoleMatrix);

                                    }
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    throw new Exception("Invalid File");
                                }
                                
                            }

                            break;
                        }
                    case "DI_SAP_HRFLINSSuppRoleMatrix":
                        {
                            using (IACMDBContext _context = new IACMDBContext())
                            {
                                List<string> coloumnNames = new List<string> { "Rule", "Company", "Cost Centre","Purchasing Org","Purchasing Grp","Plant","MRP Controller","Storage Location","Role","Role Name"};
                                Boolean IsValid = checkIsValid(coloumnNames, ds);
                                if (IsValid)                                
                                {
                                foreach (DataRow dr in ds.Rows)
                                {
                                    SAPHRFLINSSuppRoleMatrix objDI_SAP_HRFLINSSuppRoleMatrix = new SAPHRFLINSSuppRoleMatrix();


                                    string strRule = Convert.ToString(dr[0]);
                                    string strCompany = Convert.ToString(dr[1]);
                                    string strCostCenter = Convert.ToString(dr[2]);
                                    string strPurchasingOrg = Convert.ToString(dr[3]);
                                    string strPurchasingGrp = Convert.ToString(dr[4]);
                                    string strPlant = Convert.ToString(dr[5]);
                                    string strMRPControl = Convert.ToString(dr[6]);
                                    string strStorageLocation = Convert.ToString(dr[7]);
                                    string strRole = Convert.ToString(dr[8]);

                                    var OldItems = (from HRLINSSuppRoleMatrix in _context.SAPHRFLINSSuppRoleMatrices
                                                    where
                                                        HRLINSSuppRoleMatrix.ValidTo == null
                                                        && HRLINSSuppRoleMatrix.Rule == strRule
                                                        && HRLINSSuppRoleMatrix.Company == strCompany
                                                        && HRLINSSuppRoleMatrix.CostCenter == strCostCenter
                                                        && HRLINSSuppRoleMatrix.PurchasingOrg == strPurchasingOrg
                                                        && HRLINSSuppRoleMatrix.PurchasingGrp == strPurchasingGrp
                                                        && HRLINSSuppRoleMatrix.Plant == strPlant
                                                        && HRLINSSuppRoleMatrix.MRPControl == strMRPControl
                                                        && HRLINSSuppRoleMatrix.StorageLocation == strStorageLocation
                                                        && HRLINSSuppRoleMatrix.Role == strRole
                                                    select HRLINSSuppRoleMatrix).FirstOrDefault();
                                    if (OldItems != null)
                                    {

                                        OldItems.ValidTo = DateTime.Now;
                                        OldItems.IsModifiedbyExcelUpdate = true;
                                        objDI_SAP_HRFLINSSuppRoleMatrix.ReferenceId = OldItems.Id;
                                    }


                                    objDI_SAP_HRFLINSSuppRoleMatrix.Rule = strRule;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.Company = strCompany;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.CostCenter = strCostCenter;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.PurchasingOrg = strPurchasingOrg;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.PurchasingGrp = strPurchasingGrp;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.Plant = strPlant;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.MRPControl = strMRPControl;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.StorageLocation = strStorageLocation;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.Role = strRole;
                                    objDI_SAP_HRFLINSSuppRoleMatrix.RoleName = Convert.ToString(dr[9]);
                                    objDI_SAP_HRFLINSSuppRoleMatrix.ValidFrom = DateTime.Now;
                                    _context.SAPHRFLINSSuppRoleMatrices.Add(objDI_SAP_HRFLINSSuppRoleMatrix);
                                }
                                _context.SaveChanges();
                                }
                                else
                                {
                                    throw new Exception("Invalid File");
                                }
                            }

                            break;
                        }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        private Boolean checkIsValid(List<string> coloumnNames, DataTable ds)
        {

            foreach (DataColumn dc in ds.Columns)
            {
                if (!coloumnNames.Contains(dc.ColumnName))
                {
                    return false;
                }

            }
          
            return true;
        }

        public object GetDataSource(string ClassName)
        {
            try
            {                
                object Items=new object();
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (ClassName == "DI_SAP_RoleMandatoryFields")
                    {
                        Items = (from ValItem in _context.SAPRoleMandatoryFields where (ValItem.ValidTo == null && ValItem.IsDeleted != true) select ValItem).ToList();                        
                    }
                    else if (ClassName == "DI_SAP_RoleMatrix")
                    {   
                        Items = (from ValItem in _context.SAPRoleMatrices where (ValItem.ValidTo == null && ValItem.IsDeleted != true) select ValItem).ToList();                        
                    }

                    else if (ClassName == "DI_SAP_HRFLINSRoleMatrix")
                    {
                        Items = (from ValItem in _context.SAPHRFLINSRoleMatrices where (ValItem.ValidTo == null && ValItem.IsDeleted != true) select ValItem).ToList();
                    }
                    else if (ClassName == "DI_SAP_HRFLINSSuppRoleMatrix")
                    {
                        Items = (from ValItem in _context.SAPHRFLINSSuppRoleMatrices where (ValItem.ValidTo == null && ValItem.IsDeleted != true) select ValItem).ToList();                       
                    }
                }

                return Items;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteRole(int Id, string ClassName)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (ClassName == "DI_SAP_RoleMandatoryFields")
                {

                    var DeletedRow = (from Item in _context.SAPRoleMandatoryFields where Item.Id == Id select Item).FirstOrDefault();

                    if (DeletedRow != null)
                    {
                        DeletedRow.IsDeleted = true;
                    }
                }
                else if (ClassName == "DI_SAP_RoleMatrix")
                {
                    var DeletedRow = (from Item in _context.SAPRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (DeletedRow != null)
                    {
                        DeletedRow.IsDeleted = true;
                    }
                }
                else if (ClassName == "DI_SAP_HRFLINSRoleMatrix")
                {
                    var DeletedRow = (from Item in _context.SAPHRFLINSRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (DeletedRow != null)
                    {
                        DeletedRow.IsDeleted = true;
                    }
                }
                else if (ClassName == "DI_SAP_HRFLINSSuppRoleMatrix")
                {
                    var DeletedRow = (from Item in _context.SAPHRFLINSSuppRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (DeletedRow != null)
                    {
                        DeletedRow.IsDeleted = true;
                    }
                }
                _context.SaveChanges();
            }
        }


        public void UpdateRow(int Id, string ClassName, string[] strValues)
        {
            Updateold(Id, ClassName);
            using (IACMDBContext _context = new IACMDBContext())
            {

                if (ClassName == "DI_SAP_RoleMandatoryFields")
                {
                    SAPRoleMandatoryField odiSapRoleMandatory = new SAPRoleMandatoryField();

                    
                    odiSapRoleMandatory.ReferenceId = Id;
                    odiSapRoleMandatory.BusinessRole = strValues[0];
                    odiSapRoleMandatory.MandatoryField1 = strValues[1];
                    odiSapRoleMandatory.MandatoryField2 = strValues[2];
                    odiSapRoleMandatory.ValidFrom = DateTime.Now;
                    _context.SAPRoleMandatoryFields.Add(odiSapRoleMandatory);
                    _context.SaveChanges();

                }
                else if (ClassName == "DI_SAP_RoleMatrix")
                {

                    SAPRoleMatrix odiSapRoleMatrix = new SAPRoleMatrix();                    
                    odiSapRoleMatrix.ReferenceId = Id;
                    odiSapRoleMatrix.Application = strValues[0];
                    odiSapRoleMatrix.EligibilityRule = strValues[1];
                    odiSapRoleMatrix.Company = strValues[2];
                    odiSapRoleMatrix.PersonnelArea = strValues[3];
                    odiSapRoleMatrix.PersonnelSubArea = strValues[4];
                    odiSapRoleMatrix.Position = strValues[5];
                    odiSapRoleMatrix.OrgUnit = strValues[6];
                    odiSapRoleMatrix.CostCenter = strValues[7];
                    odiSapRoleMatrix.Remarks = strValues[8];
                    odiSapRoleMatrix.ValidFrom = DateTime.Now;
                    _context.SAPRoleMatrices.Add(odiSapRoleMatrix);
                    _context.SaveChanges();
                }
                else if (ClassName == "DI_SAP_HRFLINSRoleMatrix")
                {
                    SAPHRFLINSRoleMatrix odiSaphrlins = new SAPHRFLINSRoleMatrix();
                    odiSaphrlins.ReferenceId = Id;
                    odiSaphrlins.Remarks = strValues[0];
                    odiSaphrlins.Application = strValues[1];
                    odiSaphrlins.Company = strValues[2];
                    odiSaphrlins.CostCenter = strValues[3];
                    odiSaphrlins.PurchasingOrg = strValues[4];
                    odiSaphrlins.PurchasingGrp = strValues[5];
                    odiSaphrlins.Plant = strValues[6];
                    odiSaphrlins.MRPControl = strValues[7];
                    odiSaphrlins.StorageLocation = strValues[8];
                    odiSaphrlins.Module = strValues[9];
                    odiSaphrlins.SubModule = strValues[10];
                    odiSaphrlins.BusinessRole = strValues[11];
                    odiSaphrlins.TechnicalRule = strValues[12];
                    odiSaphrlins.Level1Approver = strValues[13];
                    odiSaphrlins.Level2Approver = strValues[14];
                    odiSaphrlins.Level3Approver = strValues[15];
                    odiSaphrlins.MainSupp = strValues[16];
                    odiSaphrlins.ValidFrom = DateTime.Now;
                    _context.SAPHRFLINSRoleMatrices.Add(odiSaphrlins);
                    _context.SaveChanges();

                }
                else if (ClassName == "DI_SAP_HRFLINSSuppRoleMatrix")
                {
                    
                    SAPHRFLINSSuppRoleMatrix odiSaphrlinsSupp = new SAPHRFLINSSuppRoleMatrix();
                    odiSaphrlinsSupp.ReferenceId = Id;
                    odiSaphrlinsSupp.Rule = strValues[0];
                    odiSaphrlinsSupp.Company = strValues[1];
                    odiSaphrlinsSupp.CostCenter = strValues[2];
                    odiSaphrlinsSupp.PurchasingOrg = strValues[3];
                    odiSaphrlinsSupp.PurchasingGrp = strValues[4];
                    odiSaphrlinsSupp.Plant = strValues[5];
                    odiSaphrlinsSupp.MRPControl = strValues[6];
                    odiSaphrlinsSupp.StorageLocation = strValues[7];
                    odiSaphrlinsSupp.Role = strValues[8];
                    odiSaphrlinsSupp.RoleName = strValues[9];
                    odiSaphrlinsSupp.ValidFrom = DateTime.Now;
                    _context.SAPHRFLINSSuppRoleMatrices.Add(odiSaphrlinsSupp);
                    _context.SaveChanges();
                }
            }

        }

        private void insertnew(string ClassName, string[] strValues)
        {
            throw new NotImplementedException();
        }

        private void Updateold(int Id, string ClassName)
        {
            using (IACMDBContext _context = new IACMDBContext())
            {
                if (ClassName == "DI_SAP_RoleMandatoryFields")
                {
                    var UpdateRow = (from Item in _context.SAPRoleMandatoryFields where Item.Id == Id select Item).FirstOrDefault();

                    if (UpdateRow != null)
                    {
                        UpdateRow.IsModified = true;
                        UpdateRow.ValidTo = DateTime.Now;
                    }

                }
                else if (ClassName == "DI_SAP_RoleMatrix")
                {
                    var UpdateRow = (from Item in _context.SAPRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (UpdateRow != null)
                    {
                        UpdateRow.IsModified = true;
                        UpdateRow.ValidTo = DateTime.Now;
                    }

                }
                else if (ClassName == "DI_SAP_HRFLINSRoleMatrix")
                {
                    var UpdateRow = (from Item in _context.SAPHRFLINSRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (UpdateRow != null)
                    {
                        UpdateRow.IsModified = true;
                        UpdateRow.ValidTo = DateTime.Now;
                    }

                }
                else if (ClassName == "DI_SAP_HRFLINSSuppRoleMatrix")
                {
                    var UpdateRow = (from Item in _context.SAPHRFLINSSuppRoleMatrices where Item.Id == Id select Item).FirstOrDefault();

                    if (UpdateRow != null)
                    {
                        UpdateRow.IsModified = true;
                        UpdateRow.ValidTo = DateTime.Now;
                    }

                }
                _context.SaveChanges();
            }
        }

        public SAPHRFLINSRoleMatrix GetFLINSMatrixById(int matrixId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SAPHRFLINSRoleMatrices.FirstOrDefault(item => item.Id == matrixId);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
