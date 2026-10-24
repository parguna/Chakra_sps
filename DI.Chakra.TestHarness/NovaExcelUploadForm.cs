using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic; 
using System.Data; 
using System.Linq; 
using System.Windows.Forms;
using DI.Chakra.Logger;

namespace TestHarness
{
    public partial class NovaExcelUploadForm : Form
    {
        public const string NULL = "NULL";
        public NovaExcelUploadForm()
        {
            InitializeComponent();
        }
        XLDataValidator validator;
        string CheckNullAndGetValueFromCell(object drObj)
        {
            if (drObj == null || drObj == DBNull.Value) return string.Empty;

            var cellText = drObj.ToString();

            if(string.IsNullOrWhiteSpace(cellText) || cellText.Trim() =="N/A") return string.Empty;

            cellText = cellText.Trim();
            if (string.Compare(cellText, NULL, true) != 0)
            {
                cellText = string.Join(",", cellText.Split(';').Select(r=>r.Trim()));
                return cellText;
            };
            return string.Empty;
        }
        private void u_Click(object sender, EventArgs e)
        {
            var senderName = ((System.Windows.Forms.Button)sender).Name;
            OpenFileDialog o = new OpenFileDialog();
            o.DefaultExt = "xlsx;xls*";
            var d = o.ShowDialog();
            string s = o.FileName;

            if (senderName == "u")
                txtUserMatrixPath.Text = s;
            else if (senderName == "m")
                txtManagementMatrixPath.Text = s;
            else if (senderName == "e")
                txtExceptionalMatrixPath.Text = s;
            else if (senderName == "s")
                txtSODMatrixPath.Text = s;
            else if (senderName == "c")
                txtCompositeMatrixPath.Text = s;
            
        }

        DataTable GetDataTableFromExcel(string path)
        {
            TextDataConnect connect = new TextDataConnect();
            System.Data.DataTable dataTableFromExcel = connect.ReadCSV(path);
            return dataTableFromExcel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtUserMatrixPath.Text))
                {
                    processUsermatrix(txtUserMatrixPath.Text);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Exception in UserMatrix : {ee.GetOriginalException()}");
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(txtManagementMatrixPath.Text))
                { processManagementmatrix(txtManagementMatrixPath.Text); }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Exception in ManagementMatrix : {ee.GetOriginalException()}");
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(txtExceptionalMatrixPath.Text))
                { processExceptionalmatrix(txtExceptionalMatrixPath.Text); }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Exception in ExceptionalMatrix : {ee.GetOriginalException()}");
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(txtCompositeMatrixPath.Text))
                {
                    processCompositematrix(txtCompositeMatrixPath.Text);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Exception in CompositeMatrix : {ee.GetOriginalException()}");
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSODMatrixPath.Text))
                {
                    processSODmatrix(txtSODMatrixPath.Text);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show($"Exception in SODMatrix : {ee.GetOriginalException()}");
            }
        }


        void processUsermatrix(string path)
        {
            validator = new NOVAUserRoleMatrixXLValidator();
            var dt = NovaTables.GetNovaUserMatrixTable();
            var dataTableFromExcel = GetDataTableFromExcel(path);

            var groups = dataTableFromExcel.AsEnumerable()
              .Select(r => new { CompanyCode = r.Field<string>("Company Code"),
                                SectionCode = r.Field<string>("Section Code"),
                                Position = r.Field<string>("Jobs (Positions)"),
                                UserRoles = r.Field<string>("User Roles"),
                                BWAccess = r.Field<string>("BW Access"),
                                xECMAccess = r.Field<string>("xECM Access"),
                                RoleDescription = r.Field<string>("Role Description")
              }).GroupBy(g => new { g.CompanyCode, g.SectionCode, g.Position }).Select(g=>new { Key = g.Key, Values = g.ToList() }).ToList();
                
              
            groups.ForEach(g =>  
                             { 
                                 DataRow dr = dt.NewRow();

                                 dr["Company Code"] = g.Key.CompanyCode;
                                 dr["Section Code"] = g.Key.SectionCode;
                                 dr["Position"] = g.Key.Position;
                                 dr["JobDescription"] = g.Values.Where(r => string.IsNullOrWhiteSpace(r.RoleDescription)).Select(r => r.RoleDescription).FirstOrDefault();

                                 List<string> flins = new List<string>(), flinsBi = new List<string>(), Adgroups = new List<string>();
                                 foreach (var r in g.Values)
                                 {
                                     var f = CheckNullAndGetValueFromCell(r.UserRoles);
                                     if (!string.IsNullOrWhiteSpace(f))
                                         flins.Add(f);
                                     var fbli = CheckNullAndGetValueFromCell(r.BWAccess);
                                     if (!string.IsNullOrWhiteSpace(fbli)) flinsBi.Add(fbli);
                                     var a = CheckNullAndGetValueFromCell(r.xECMAccess);
                                     if (!string.IsNullOrWhiteSpace(a)) Adgroups.Add(a);
                                 }

                                 dr["FLINSRoles"] = string.Join(",", flins);
                                 dr["BWAccess"] = string.Join(",", flinsBi);
                                 dr["AD Groups"] = string.Join(",", Adgroups);
                                 dr["Level1 Approver"] = "00000010";
                                 dr["Level1 Approver Type"] = "1";
                                 dr["Level2 Approver"] = "00000010";
                                 dr["Level2 Approver Type"] = "1";
                                 dr["Level3 Approver"] = "00000010";
                                 dr["Level3 Approver Type"] = "1";
                                 dt.Rows.Add(dr);

                             });

            validator.TruncateTable();
            foreach (DataRow dr in dt.Rows)
            {
                validator.Data = validator.GetObjectFromData(dr);
                string msg = "";
                if (validator.Validate(ref msg))
                {
                    validator.Add("System");
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(null, 0, LogLevelType.Exception, $"Error while adding NOVA User matrics for CompanyCode - {dr["Company Code"]}, SectionCode - {dr["Section Code"]}, Position - {dr["Position"]} - Message - {msg}",null);
 
                }
            }

        }

        void processManagementmatrix(string path)
        {
            validator = new NOVAManagementRoleMatrixXLValidator();
            var dt = NovaTables.GetNovaManagementMatrixTable();
            var dataTableFromExcel = GetDataTableFromExcel(path);
              
            var groups = dataTableFromExcel.AsEnumerable()
              .Select(r => new {
                  CompanyCode = r.Field<string>("Company Code"),
                  Department = r.Field<string>("Department/Division /Branch"),
                  Position = r.Field<string>("JOBS (Position)"),
                  UserRoles = r.Field<string>("User Roles"),
                  BWAccess = r.Field<string>("BW Access"),
                  xECMAccess = r.Field<string>("xECM Access"),
                  RoleDescription = r.Field<string>("Role Description")
              }).GroupBy(g => new { g.CompanyCode, g.Department, g.Position }).Select(g => new { Key = g.Key, Values = g.ToList() }).ToList();


            groups.ForEach(g =>
            {
                DataRow dr = dt.NewRow();

                //dr["Company Code"] = g.Key.CompanyCode;
                dr["Department"] = g.Key.Department;
                dr["Position"] = g.Key.Position;
                dr["JobDescription"] = g.Values.Where(r => string.IsNullOrWhiteSpace(r.RoleDescription)).Select(r => r.RoleDescription).FirstOrDefault();

                List<string> flins = new List<string>(), flinsBi = new List<string>(), Adgroups = new List<string>();
                foreach (var r in g.Values)
                {
                    var f = CheckNullAndGetValueFromCell(r.UserRoles);
                    if (!string.IsNullOrWhiteSpace(f))
                        flins.Add(f);
                    var fbli = CheckNullAndGetValueFromCell(r.BWAccess);
                    if (!string.IsNullOrWhiteSpace(fbli)) flinsBi.Add(fbli);
                    var a = CheckNullAndGetValueFromCell(r.xECMAccess);
                    if (!string.IsNullOrWhiteSpace(a)) Adgroups.Add(a);
                }

                dr["FLINSRoles"] = string.Join(",", flins);
                dr["BWAccess"] = string.Join(",", flinsBi);
                dr["AD Groups"] = string.Join(",", Adgroups);
                dr["Level1 Approver"] = "00000010";
                dr["Level1 Approver Type"] = "1";
                dr["Level2 Approver"] = "00000010";
                dr["Level2 Approver Type"] = "1";
                dr["Level3 Approver"] = "00000010";
                dr["Level3 Approver Type"] = "1";
                dt.Rows.Add(dr);

            });

            validator.TruncateTable();
            foreach (DataRow dr in dt.Rows)
            {
                validator.Data = validator.GetObjectFromData(dr);
                string msg = "";
                if (validator.Validate(ref msg))
                {
                    validator.Add("System");
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(null, 0, LogLevelType.Exception, $"Error while adding NOVA User matrics for Department - {dr["Department"]}, Position - {dr["Position"]} - Message - {msg}", null);

                }
            }

        }

        void processExceptionalmatrix(string path)
        {
            validator = new NOVAExceptionalRoleMatrixXLValidator();
            var dt = NovaTables.GetNovaExceptionalMatrixTable();
            var dataTableFromExcel = GetDataTableFromExcel(path);

            var groups = dataTableFromExcel.AsEnumerable()
              .Select(r => new {
                  CompanyCode = r.Field<string>("Company Code"),
                  FLINSRoles = r.Field<string>("Role Name"), 
                  RoleDescription = r.Field<string>("Role Description")
              }).GroupBy(g => new { g.CompanyCode, g.RoleDescription }).Select(g => new { Key = g.Key, Values = g.ToList() }).ToList();


            groups.ForEach(g =>
            {
                DataRow dr = dt.NewRow();
                dr["Is Exclusive For User Account Type"] = "False";
                dr["Company Code"] = g.Key.CompanyCode;
               // dr["FLINSRoles"] = g.Key.FLINSRoles;
                dr["JobDescription"] = g.Key.RoleDescription;

                List<string> flins = new List<string>(), flinsBi = new List<string>(), Adgroups = new List<string>();
                foreach (var r in g.Values)
                {
                    var f = CheckNullAndGetValueFromCell(r.FLINSRoles);
                    if (!string.IsNullOrWhiteSpace(f))
                        flins.Add(f);
                    //var fbli = CheckNullAndGetValueFromCell(r.BWAccess);
                    //if (!string.IsNullOrWhiteSpace(fbli)) flinsBi.Add(fbli);
                    //var a = CheckNullAndGetValueFromCell(r.xECMAccess);
                    //if (!string.IsNullOrWhiteSpace(a)) Adgroups.Add(a);
                }

                dr["FLINSRoles"] = string.Join(",", flins); 
                dr["Level1 Approver"] = "00000010";
                dr["Level1 Approver Type"] = "1";
                dr["Level2 Approver"] = "00000010";
                dr["Level2 Approver Type"] = "1";
                dr["Level3 Approver"] = "00000010";
                dr["Level3 Approver Type"] = "1";
                dt.Rows.Add(dr);

            });

            validator.TruncateTable();
            foreach (DataRow dr in dt.Rows)
            {
                validator.Data = validator.GetObjectFromData(dr);
                string msg = "";
                if (validator.Validate(ref msg))
                {
                    validator.Add("System");
                }
                else
                {
                    DILog.ProvisioningLogger.WriteLog(null, 0, LogLevelType.Exception, $"Error while adding NOVA User matrics for JobDescription - {dr["JobDescription"]}  - Message - {msg}", null);

                }
            }
             
        }

        void processCompositematrix(string path)
        {
            validator = new  NOVACompositeRoleMatrixXLValidator();
            var dt = NovaTables.GetNovaCompositeMatrixTable();
            var dataTableFromExcel = GetDataTableFromExcel(path);

            var groups = dataTableFromExcel.AsEnumerable()
              .Select(r => new {
                  Job = r.Field<string>("JOB"),
                  CompositeRoleName = r.Field<string>("Composite Role Name"),
                  ModuleWiseRole  = r.Field<string>("Module Wise Role")
              }).GroupBy(g => new { g.Job, g.CompositeRoleName }).Select(g => new { Key = g.Key, Values = g.ToList() }).ToList();


            groups.ForEach(g =>
            {
                DataRow dr = dt.NewRow();


                dr["Job"] = g.Key.Job;
                dr["CompositeRoleName"] = g.Key.CompositeRoleName;
                
                List<string> flins = new List<string>(), flinsBi = new List<string>(), Adgroups = new List<string>();
                foreach (var r in g.Values)
                {
                    var f = CheckNullAndGetValueFromCell(r.ModuleWiseRole);
                    if (!string.IsNullOrWhiteSpace(f))
                        flins.Add(f); 
                }

                dr["ModuleWiseRoles"] = string.Join(",", flins);
            
                dt.Rows.Add(dr);

            });

            validator.TruncateTable();
            foreach (DataRow dr in dt.Rows)
            {
                validator.Data = validator.GetObjectFromData(dr);
                string msg = "";
                if (validator.Validate(ref msg))
                {
                    validator.Add("System");
                }
            }

        }

        void processSODmatrix(string path)
        {
            validator = new NOVASoDExceptionsMatrixXLValidator();
            var dt = NovaTables.GetNovaSODMatrixTable();
            var dataTableFromExcel = GetDataTableFromExcel(path);

            var groups = dataTableFromExcel.AsEnumerable()
              .Select(r => new {
                  Exceptions = r.Field<string>("Exceptions"),
                  AffectedUsers = r.Field<string>("Affected Users") 
              }).ToList();


            groups.ForEach(g =>
            {
                DataRow dr = dt.NewRow();


                dr["Exceptions"] = g.Exceptions;
                dr["AffectedUsers"] = g.AffectedUsers;
                 

                dt.Rows.Add(dr);

            });

            validator.TruncateTable();
            foreach (DataRow dr in dt.Rows)
            {
                validator.Data = validator.GetObjectFromData(dr);
                string msg = "";
                if (validator.Validate(ref msg))
                {
                    validator.Add("System");
                }
            }

        }

    }



    public class NovaTables
    {
        public static System.Data.DataTable GetNovaUserMatrixTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("JobDescription"));
            dt.Columns.Add(new DataColumn("Company Code"));
            dt.Columns.Add(new DataColumn("Section Code"));
            dt.Columns.Add(new DataColumn("Position"));
            dt.Columns.Add(new DataColumn("FLINSRoles"));
            dt.Columns.Add(new DataColumn("BWAccess"));
            dt.Columns.Add(new DataColumn("AD Groups"));
            dt.Columns.Add(new DataColumn("Level1 Approver"));
            dt.Columns.Add(new DataColumn("Level1 Approver Type"));
            dt.Columns.Add(new DataColumn("Level2 Approver"));
            dt.Columns.Add(new DataColumn("Level2 Approver Type"));
            dt.Columns.Add(new DataColumn("Level3 Approver"));            
            dt.Columns.Add(new DataColumn("Level3 Approver Type"));
            return dt;
        }

        public static System.Data.DataTable GetNovaManagementMatrixTable()
        {  
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("JobDescription"));
            dt.Columns.Add(new DataColumn("Department")); 
            dt.Columns.Add(new DataColumn("Position"));
            dt.Columns.Add(new DataColumn("FLINSRoles"));
            dt.Columns.Add(new DataColumn("BWAccess"));
            dt.Columns.Add(new DataColumn("AD Groups"));
            dt.Columns.Add(new DataColumn("Level1 Approver"));
            dt.Columns.Add(new DataColumn("Level1 Approver Type"));
            dt.Columns.Add(new DataColumn("Level2 Approver"));
            dt.Columns.Add(new DataColumn("Level2 Approver Type"));
            dt.Columns.Add(new DataColumn("Level3 Approver"));
            dt.Columns.Add(new DataColumn("Level3 Approver Type"));
            return dt;
        }

        public static System.Data.DataTable GetNovaExceptionalMatrixTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("JobDescription"));
            dt.Columns.Add(new DataColumn("Company Code")); 
            dt.Columns.Add(new DataColumn("FLINSRoles")); 
            dt.Columns.Add(new DataColumn("Level1 Approver"));
            dt.Columns.Add(new DataColumn("Level1 Approver Type"));
            dt.Columns.Add(new DataColumn("Level2 Approver"));
            dt.Columns.Add(new DataColumn("Level2 Approver Type"));
            dt.Columns.Add(new DataColumn("Level3 Approver"));
            dt.Columns.Add(new DataColumn("Level3 Approver Type"));
            dt.Columns.Add(new DataColumn("Is Exclusive For User Account Type"));
            return dt;
        }

        public static System.Data.DataTable GetNovaSODMatrixTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("Exceptions"));
            dt.Columns.Add(new DataColumn("AffectedUsers")); 
            return dt;
        }

        public static System.Data.DataTable GetNovaCompositeMatrixTable()
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add(new DataColumn("Job"));
            dt.Columns.Add(new DataColumn("CompositeRoleName"));
            dt.Columns.Add(new DataColumn("ModuleWiseRoles"));
            
            return dt;
        }
    }
}
