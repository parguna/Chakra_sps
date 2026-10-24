using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DI.Chakra.Dto.Presentation;
using Telerik.Web.UI;
using System.Data;

namespace DI.Chakra.Web.Admin.Attestation
{
    public partial class ExportToExcel : DI.Chakra.Web.UIBasePage
    {     
        protected void Page_Load(object sender, EventArgs e)
        {          
            if (!Page.IsPostBack)
            {
                grdExport.ExportSettings.Pdf.PageHeader.MiddleCell.Text = "Attestation Export";
                grdExport.ExportSettings.Pdf.PageHeader.MiddleCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
                grdExport.ExportSettings.Pdf.PageHeader.LeftCell.Text = PDFFileLeftCell;
                grdExport.ExportSettings.Pdf.PageHeader.LeftCell.TextAlign = GridPdfPageHeaderFooterCell.CellTextAlign.Center;
            }

        }

        private int TaskId = 11;

        //protected void ExportExcelButton_Click(object sender, EventArgs e)
        //{
        //    //ExportGrid();
        //    Dictionary<string, string> parameter = new Dictionary<string, string>();
        //    parameter.Add("id", this.TaskId.ToString());
        //    parameter.Add("isInitialLoad", true.ToString());
        //    parameter.Add("currentUser", "iacmadmin");
        //    var attestationUserDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
        //            (DomainApiAction.Tasks.Task.GetUserForAttestationTaskReview, parameter);

        //    OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
        //    var workSheet = excel.Workbook.Worksheets.Add("Products");

        //    int printLevel = 1;
        //    int currentRow = 1;
        //    int colStartPosition = 1;
        //    int colNum = colStartPosition;
        //    long keyId = 0;
        //    string category = "";

        //    // loop top level users
        //    // add workcell entry for top level
        //    workSheet.Cells[currentRow, colNum].Value = "Name";
        //    workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //    workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //    workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //    currentRow++;
        //    foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail diUser in attestationUserDetails)
        //    {
        //        colNum = colStartPosition;
        //        // add workcell entry for top level
        //        workSheet.Cells[currentRow, colNum].Value = diUser.Name;
        //        workSheet.Cells[currentRow, ++colNum].Value = diUser.DisplayName;
        //        workSheet.Cells[currentRow, ++colNum].Value = diUser.Type;
        //        workSheet.Cells[currentRow, ++colNum].Value = diUser.ReviewCategory;
        //        currentRow++;

        //        keyId = diUser.EntityId;
        //        category = diUser.ReviewCategory;
        //        parameter = new Dictionary<string, string>();
        //        parameter.Add("entityId", keyId.ToString());
        //        parameter.Add("id", this.TaskId.ToString());
        //        parameter.Add("isInitialLoad", true.ToString());
        //        parameter.Add("currentUser", "iacmadmin");

        //        //if (category == "User")
        //        {
        //            List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
        //                        (DomainApiAction.Tasks.Task.GetEndpointUserForAttestationTaskReview, parameter);
        //            colStartPosition = 2;
        //            workSheet.Cells[currentRow, colNum].Value = "Name";
        //            workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //            workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
        //            workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
        //            workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //            workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //            currentRow++;
        //            foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epUser in gridNextLevelDetails)
        //            {
        //                colNum = colStartPosition;
        //                // add workcell entry for top level
        //                workSheet.Cells[currentRow, colNum].Value = epUser.Name;
        //                workSheet.Cells[currentRow, ++colNum].Value = epUser.DisplayName;
        //                workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointType.ToString();
        //                workSheet.Cells[currentRow, ++colNum].Value = epUser.EndpointSystemName;
        //                workSheet.Cells[currentRow, ++colNum].Value = epUser.Type;
        //                workSheet.Cells[currentRow, ++colNum].Value = epUser.ReviewCategory;
        //                currentRow++;

        //                keyId = epUser.EntityId;
        //                category = epUser.ReviewCategory;
        //                parameter = new Dictionary<string, string>();
        //                parameter.Add("entityId", keyId.ToString());
        //                parameter.Add("id", this.TaskId.ToString());
        //                parameter.Add("isInitialLoad", true.ToString());
        //                parameter.Add("currentUser", "iacmadmin");
        //                // Account level
        //                List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails2 = DomainApiClient<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>.GetByAction<List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>>
        //                        (DomainApiAction.Tasks.Task.GetUserEntitlementForAttestationTaskReview, parameter);
        //                colStartPosition = 3;
        //                workSheet.Cells[currentRow, colNum].Value = "Name";
        //                workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //                workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
        //                workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
        //                workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //                workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //                currentRow++;
        //                foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epAcnt in gridNextLevelDetails2)
        //                {
        //                    colNum = colStartPosition;
        //                    // add workcell entry for top level
        //                    workSheet.Cells[currentRow, colNum].Value = epAcnt.Name;
        //                    workSheet.Cells[currentRow, ++colNum].Value = epAcnt.DisplayName;
        //                    workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointType.ToString();
        //                    workSheet.Cells[currentRow, ++colNum].Value = epAcnt.EndpointSystemName;
        //                    workSheet.Cells[currentRow, ++colNum].Value = epAcnt.Type;
        //                    workSheet.Cells[currentRow, ++colNum].Value = epAcnt.ReviewCategory;
        //                    currentRow++;

        //                    keyId = epAcnt.EntityId;
        //                    // entitlement level
        //                    List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> gridNextLevelDetails3 = GetEntitlementDetailForAttestationTaskReview(keyId);
        //                    colStartPosition = 4;
        //                    workSheet.Cells[currentRow, colNum].Value = "Name";
        //                    workSheet.Cells[currentRow, ++colNum].Value = "DisplayName";
        //                    workSheet.Cells[currentRow, ++colNum].Value = "EndpointType";
        //                    workSheet.Cells[currentRow, ++colNum].Value = "EndpointName";
        //                    workSheet.Cells[currentRow, ++colNum].Value = "Type";
        //                    workSheet.Cells[currentRow, ++colNum].Value = "ReviewCategory";
        //                    currentRow++;
        //                    foreach (Dto.Presentation.PresentationAttestationEntitlementReviewDetail epEnt in gridNextLevelDetails3)
        //                    {
        //                        colNum = colStartPosition;
        //                        // add workcell entry for top level
        //                        workSheet.Cells[currentRow, colNum].Value = epEnt.Name;
        //                        workSheet.Cells[currentRow, ++colNum].Value = epEnt.DisplayName;
        //                        workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointType.ToString();
        //                        workSheet.Cells[currentRow, ++colNum].Value = epEnt.EndpointSystemName;
        //                        workSheet.Cells[currentRow, ++colNum].Value = epEnt.Type;
        //                        workSheet.Cells[currentRow, ++colNum].Value = epEnt.ReviewCategory;
        //                        currentRow++;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //Write it back to the client
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment;  filename=ExcelDemo.xlsx");
        //    Response.BinaryWrite(excel.GetAsByteArray());
        //    Response.End();

        //    Response.Clear();
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.Buffer = true;
        //    Response.AddHeader("Content-Disposition", "attachment;filename=xyz");
        //    Response.BinaryWrite(excel.GetAsByteArray());
        //    Response.End();
        //}

        public static List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> GetEntitlementDetailForAttestationTaskReview(long entityId)
        {
            List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail> output = new List<Dto.Presentation.PresentationAttestationEntitlementReviewDetail>();

            List<String> functions = new List<string>();

            switch (entityId)
            {
                case 1:
                    // Admin
                    functions = new List<string>() { "Lodgement", "CIT", "GST", "IIT/BIT", "Clearance", "PT", "Return Rebatch" };
                    break;
                case 2:
                    // Oper
                    functions = new List<string>() { "Clearance" };
                    break;
                case 3:
                    // supervisor
                    functions = new List<string>() { "Return Rebatch" };
                    break;
                case 4:
                    // Check
                    functions = new List<string>() { "Lodgement", "CIT", "GST", "IIT/BIT", "PT" };
                    break;
                default:
                    functions = new List<string>() { "Lodgement", "CIT", "GST", "IIT/BIT", "Clearance", "PT", "Return Rebatch" };
                    break;
            }
            int counter = 1;

            foreach (string str in functions)
            {
                var x = new Dto.Presentation.PresentationAttestationEntitlementReviewDetail
                {
                    KeyId = counter,
                    Entity = counter.ToString(),
                    Name = str,
                    DisplayName = str,
                    Type = "Function",
                    ReviewCategory = "EntitlementDetail",
                    IsChanged = false,
                    Approved = false,
                    Rejected = false,
                    Comments = String.Empty,
                };
                output.Add(x);
            }
            return output;

        }


        protected void grdExport_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //BindAll();
        }

        private void BindAll()
        {
            DI.Chakra.Dto.Task task;
            if (Request.QueryString["TaskId"] != null)
            {
                long TaskId = Convert.ToInt64(Request.QueryString["TaskId"]);            
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("taskId", TaskId.ToString());
                IList<Dto.Presentation.PresentationSapUserRoleTCodeAuthorization> SapUserToRoles = DomainApiClient<Dto.Presentation.
                    PresentationSapUserRoleTCodeAuthorization>.GetByAction<IList<Dto.Presentation.PresentationSapUserRoleTCodeAuthorization>>
                    (DomainApiAction.Attestation.Presentation.PresentationSapUserRoleTCodeAuthorization.GetSapUserRoleTCodeAuthorization, parameter);

                //var SapUserToRoles = (from taskReview in task.TaskReviews
                //                      from reviewEP in taskReview.AttestationReview.ReviewEndpointSystems
                //                      from reviewEPL2 in reviewEP.AttestationReviewEndpointSystem.AttestationReviewEndpointSystemL2Details
                //                      select new SapUserRoleTCodeAuthorization
                //                      {
                //                          UserName = reviewEP.AttestationReviewEndpointSystem.EndpointUserName,
                //                          RoleCode = reviewEPL2.AccessName,
                //                          EndpointSystemId = (int)reviewEP.AttestationReviewEndpointSystem.EndpointSystemId
                //                      });
                List<PresentationSapUserRoleTCodeAuthorization> SapObjects = new List<PresentationSapUserRoleTCodeAuthorization>();

                foreach (PresentationSapUserRoleTCodeAuthorization UserRole in SapUserToRoles)
                {                 
                    Dictionary<string, string> authorizationParameters = new Dictionary<string, string>();
                    authorizationParameters.Add("roleCode", UserRole.RoleCode);
                    authorizationParameters.Add("endpointSystemId", UserRole.EndpointSystemId.ToString());
                    List<Dto.SAPTCodeInfo> tCodes = DomainApiClient<Dto.SAPTCodeInfo>.GetByAction<List<Dto.SAPTCodeInfo>>
                        (DomainApiAction.Endpoint.SAP.SAPTCodeInfo.GetSapRolesToTCodeData, authorizationParameters);

                    List<PresentationSapUserRoleTCodeAuthorization> SapUserRoleTCode = (from tcode in tCodes
                                                                                        select new PresentationSapUserRoleTCodeAuthorization
                                                                                        {
                                                                                            UserName = UserRole.UserName,
                                                                                            RoleCode = UserRole.RoleCode,
                                                                                            TCode = tcode.TCODE_Code,
                                                                                            EndpointSystemId = UserRole.EndpointSystemId
                                                                                        }).ToList();
                    SapObjects = SapObjects.Union(SapUserRoleTCode).ToList();
                }
                grdExport.DataSource = SapObjects;
            }
        }
    }
}