using DeepIdentity.ConfigHelper;
using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.SAP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using DeepIdentity.Model.ReportData;

namespace DeepIdentity.Facade
{
    public class RoleMandatoryFacade
    {
        RoleMandatoryDA oRoleMandatoryDA = new RoleMandatoryDA();
        public void InsertSapExcel(DataTable ds, string ClassName)
        {
            try
            {
                 oRoleMandatoryDA.InsertSapExcel(ds, ClassName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public object GetDataSource(string ClassName)
        {
            try
            {
                return oRoleMandatoryDA.GetDataSource(ClassName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void DeleteRole(int Id, string ClassName)
        {
            try {
                oRoleMandatoryDA.DeleteRole(Id, ClassName);
            }
            catch (Exception ex)
            { }
        }

        public void UpdateRole(int Id, string ClassName,string[] strValues)
        {
            oRoleMandatoryDA.UpdateRow(Id, ClassName,strValues);
        }

        public void ReadExcel(string Import_FileName,string ClassName)
        {
            oRoleMandatoryDA.InsertSapExcel(new ExcelConnect().ReadExcel(Import_FileName, ClassName), ClassName);
        }

        public SAPHRFLINSRoleMatrix GetFLINSMatrixById(int matrixId)
        {
            try
            {
                return oRoleMandatoryDA.GetFLINSMatrixById(matrixId);
            }
            catch (Exception)
            {
                throw;
            }
        }
       
    }
}
