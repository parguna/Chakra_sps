using DI.Chakra.DomainModel.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Logger;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SAPTCodeInfo : DomainModelBase<SAPTCodeInfo, EntityModel.SAPTCodeInfo>
    {
        public string TCODE_Code { get; set; }

        public string TCODE_Desc { get; set; }



        public long EndPointSystemId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
        public int? Version { get; set; }

        public IList<SAPSingleRoleInfo> SAPSingleRoleInfos { get; set; }

        public string Role { get; set; }
        
        public static SAPTCodeInfo GetByTCode(string tCode)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPTCodeInfo>(dataAccessTransaction).
                    Get(codeInfo => codeInfo.TCODE_Code == tCode).FirstOrDefault());
            }
        }

        public static List<SAPTCodeInfo> GetByEndpointId(long endPointsystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPTCodeInfo>(dataAccessTransaction).
                    Get(codeInfo => codeInfo.EndPointSystemId == endPointsystemId).ToList());
            }
        }
        public static Dictionary<long, string> GetSAPTCodeBySearchString(long endpointId, string searchString)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new SAPTCodeInfoRepository(transaction).GetSAPTCodeBySearchString
                    (endpointId, CheckNull.AsteriskToString(searchString));
            }
        }
        public static Dictionary<long, string> GetSapTCodes(long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new SAPTCodeInfoRepository(dataAccessTransaction).GetAllSapTCodes(endpointSystemId));
            }
        }

        public static List<SAPTCodeInfo> GetByModifiedDate(long endPointId, DateTime? processStartAt)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPTCodeInfo>(dataAccessTransaction).
                    Get(codeInfo => codeInfo.EndPointSystemId == endPointId && codeInfo.LastModifiedDate < processStartAt).ToList());
            }
        }

        public static bool isExists(string entitlement)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return (new RepositoryBase<EntityModel.SAPTCodeInfo>(dataAccessTransaction).
                    GetAll().Any(obj => obj.TCODE_Code == entitlement));
            }
        }

        public static IList<SAPTCodeInfo> GetByUserId(int sapUserId)
        {
            IList<SAPTCodeInfo> tempTCodeList = new List<SAPTCodeInfo>();
            IList<SAPTCodeInfo> retunTCodeList = new List<SAPTCodeInfo>();
            IList<SAPSingleRoleInfo> tempSapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPSingleRoleInfo> sapSingleRoleList = new List<SAPSingleRoleInfo>();
            IList<SAPCompositeRoleInfo> sapCompRoleList = new List<SAPCompositeRoleInfo>();
            List<SAPTCodeInfo> lstSapTCodeInfo = new List<SAPTCodeInfo>();
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPUserinfo aSAPUser = SAPUserinfo.GetById(sapUserId);
                if (aSAPUser != null)
                {
                    //sapCompRoleList = aSAPUser.SAPCompositeRoleInfos.ToList();

                    List<SAPRoleInfo> lstSapRoles = aSAPUser.SAPRoleInfo.ToList();

                    foreach (SAPRoleInfo RoleInfo in lstSapRoles)
                    {
                        List<SAPRoleToAuthMap> lstRoleAuth = SAPRoleToAuthMap.GetByCodes(RoleInfo.ROLE_CODE, "S_TCODE", "TCD").ToList();
                        foreach (SAPRoleToAuthMap RoleAuth in lstRoleAuth)
                        {
                            SAPTCodeInfo TCodeInfo = SAPTCodeInfo.GetByTCode(RoleAuth.FromValue);

                            if (TCodeInfo != null)
                                lstSapTCodeInfo.Add(TCodeInfo);
                        }
                    }
                }
            }
            return lstSapTCodeInfo;
        }

        public static List<SAPTCodeInfo> GetByRoleCode(string roleCode, long endpointSystemId)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPTCodeInfoRepository(dataAccessTransaction).GetSapRolesToTCodeData(roleCode, endpointSystemId));
            }
        }

        public static List<SAPTCodeInfo> GetByRoleCodeIncludingRoles(string roleCode, long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new SAPTCodeInfoRepository(dataAccessTransaction).GetSapRolesToTCodeDataWithRoles(roleCode, endpointSystemId));
            }
        }

        public static List<SAPTCodeInfo> GetSapRolesToTCodeData(string roleCode, long endpointSystemId)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                //List<SAPTCodeInfo> SAPTCodeInfos = SAPTCodeInfo.GetSapRolesToTCodeData(roleCode, endpointSystemId);
                List<SAPTCodeInfo> SAPTCodeInfos = ORMapList(new SAPTCodeInfoRepository(dataAccessTransaction).GetSapRolesToTCodeData(roleCode, endpointSystemId));
                List<SAPTCodeInfo> ResultTCodeInfo = new List<SAPTCodeInfo>();
                foreach (SAPTCodeInfo tcode in SAPTCodeInfos)
                {
                    tcode.Role = roleCode;
                    ResultTCodeInfo.Add(tcode);
                }

                return ResultTCodeInfo;
            }

        }

        public static SAPTCodeInfo GetByTCode(string tCode, long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPTCodeInfo>(dataAccessTransaction).Get(codeInfo => codeInfo.TCODE_Code == tCode && codeInfo.EndPointSystemId == endPointSystemId).FirstOrDefault());
            }
        }

        public void SaveTCodes(IList<SAPTCodeInfo> list, IList<SAPTCodeInfo> lstSapTCodes, long endpointSystemId)
        {

            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                SAPTCodeInfo sapexistingTcode = null;

                foreach (SAPTCodeInfo tcode in list)
                {

                    sapexistingTcode = SAPTCodeInfo.GetByTCode(tcode.TCODE_Code, endpointSystemId);

                    if (sapexistingTcode == null)
                    {
                        sapexistingTcode.Save(dataAccessTransaction);
                    }
                    else
                    {
                        //tcode.TCODE_Id = sapexistingTcode.TCODE_Id;
                        //tcode.TCODE_Code = sapexistingTcode.TCODE_Code;
                        //tcode.CreatedBy = sapexistingTcode.CreatedBy;
                        //tcode.CreatedDate = sapexistingTcode.CreatedDate;

                        sapexistingTcode.TCODE_Desc = tcode.TCODE_Desc;
                        sapexistingTcode.LastModifiedDate = tcode.LastModifiedDate;
                        sapexistingTcode.LastModifiedBy = tcode.LastModifiedBy;

                        sapexistingTcode.Save(dataAccessTransaction);

                        //_context.SAPTCodeInfos.Attach(sapexistingTcode);
                        //_context.Entry(sapexistingTcode).State = EntityState.Modified;
                        //_context.SaveChanges();
                    }
                }

                dataAccessTransaction.Commit();
            }


        }

        public Dictionary<long, string> GetAllAssignedTCodes(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new SAPTCodeInfoRepository(dataAccessTransaction).GetAllAssignedTCodes(endPointSystemId, "S_TCODE", "TCD");
            }
        }

        public bool IsSAPTCodeValid(string value)
        {
            try
            {
                var entitlements = value.Split(',');
                var isExists = false;

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    foreach (var entitlement in entitlements)
                    {
                        if (!string.IsNullOrEmpty(entitlement))
                        {
                            isExists = SAPTCodeInfo.GetAll().Any(item => item.TCODE_Code == entitlement);
                            //_context.SAPTCodeInfos.Any(item => item.TCODE_Code == entitlement);
                            if (!isExists)
                            {
                                break;
                            }
                        }
                    }
                }
                return isExists;
            }
            catch
            {
                return false;
            }
        }

        public static IList<SAPTCodeInfo> GetDeletedSAPTCode(long endPointId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                DiscoveryProcess objDiscoveryProcess = new DiscoveryProcess();
                IList<SAPTCodeInfo> listDeletedSingleRole = new List<SAPTCodeInfo>();
                objDiscoveryProcess = DiscoveryProcess.GetByEndpointsystemId(endPointId).FirstOrDefault();
                if (objDiscoveryProcess != null)
                {
                    Process process = Process.GetByProcessTypeKeyId(objDiscoveryProcess.Id, Infrastructure.Enums.ProcessType.Discovery);

                    if (process != null)
                    {
                        listDeletedSingleRole = SAPTCodeInfo.GetByModifiedDate(endPointId, process.ProcessStartAt);
                    }
                }

                return listDeletedSingleRole;

            }
        }



        public static void SaveTcodes(IList<SAPTCodeInfo> list, long endpointSystemId, string currentUser)
        {
            //using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPTCodeInfo> listExistingSAPTCodeInfos = new List<SAPTCodeInfo>();
                SAPTCodeInfo sapexistingSAPTCodeInfo = null;

                listExistingSAPTCodeInfos = SAPTCodeInfo.GetByEndpointId(endpointSystemId);

                foreach (SAPTCodeInfo saptcodes in list)
                {
                    sapexistingSAPTCodeInfo = listExistingSAPTCodeInfos.Where(t => t.TCODE_Code == saptcodes.TCODE_Code).FirstOrDefault();
                    if (sapexistingSAPTCodeInfo == null)
                    {
                        saptcodes.Save(currentUser);
                    }
                    else
                    {
                        //saptcodes.TCODE_Desc = sapexistingSAPTCodeInfo.TCODE_Desc;
                        //saptcodes.LastModifiedDate = sapexistingSAPTCodeInfo.LastModifiedDate;
                        //saptcodes.LastModifiedBy = sapexistingSAPTCodeInfo.LastModifiedBy;
                        saptcodes.Id = sapexistingSAPTCodeInfo.Id;
                        saptcodes.Save(currentUser);
                    }
                }
                //dataAccessTransaction.Commit();
            }
        }

        public static void SaveTcodes(IList<SAPTCodeInfo> list, string currentUser)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                foreach (SAPTCodeInfo saptcodes in list)
                {
                    saptcodes.Save(dataAccessTransaction);
                }
                dataAccessTransaction.Commit();
            }
        }

        public static void DeleteTCodesByEndpoint(long endpointSystemId, string currentUser)
        {
            string query = "DELETE [DI_SAP_TCODEINFO] where [EndPointSystemId] = {0}";
            using(DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                ExecuteQuery(String.Format(query,endpointSystemId));
            }
        }

        public static string GenerateRoleTCodeForReport(long endpointId)
        {
            var retMsg = string.Empty;
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                try
                {
                    var ep = EndPoint.GetById(endpointId);
                    if (ep != null && ep.EndpointSystemTypeId == (int)DI.Chakra.Infrastructure.Enums.EndpointType.SAP)
                    {
                        var reportData = new SAPTCodeInfoRepository(transaction).GetRoleTCodeForReport(endpointId);
                        var reportPath = MasterConfigHelper.TCodeReportFilePath;
                        if(Directory.Exists(reportPath) == false)
                            Directory.CreateDirectory(reportPath);
                        GenerateReport(reportPath, ep.DisplayName, reportData);
                    }
                    else
                    {
                        retMsg = "Cannot generate Report for Non SAP System";
                    }
                }
                catch (Exception ex)
                {
                    retMsg = "Error while generating RoleToTCode report";
                    DILog.AttestationLogger.WriteLog(LogLevelType.Exception, $"Error while generating RoleToTCode report.{ex.Message}",ex);
                }
            }
            return retMsg;
        }
        private static void GenerateReport(string path,string displayName, List<EntityModel.PresentationSAPRoleTCode> reportData)
        {
            var reportSheetName = displayName.GetValidFileName();
            var temp = path;
            string filePath = string.Empty;
            try
            {
                if (!System.IO.Directory.Exists(temp)) System.IO.Directory.CreateDirectory(temp);
                filePath = string.Format("{0}\\{1}_RoleToTCode.xlsx", temp, reportSheetName);
                if (File.Exists(filePath)) File.Delete(filePath);
                using (FileStream stream = new FileStream(filePath, FileMode.Create,FileAccess.ReadWrite))
                {
                    using (var objExcelPackage = new ExcelPackage(stream))
                    {
                        int currentRow = 1;
                        int colStartPosition = 1;
                        int colNum = colStartPosition;
                        ExcelWorksheet workSheet = objExcelPackage.Workbook.Worksheets.Add(reportSheetName);
                        workSheet.Row(1).Height = 20;
                        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Cells[currentRow, colNum].Value = "Application";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "RoleCode";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "Role Description";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "TCode";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        workSheet.Cells[currentRow, ++colNum].Value = "TCode_Description";
                        workSheet.Cells[currentRow, colNum].Style.Font.Bold = true;
                        currentRow++;
                        foreach (var data in reportData)
                        {
                            colStartPosition = 1;
                            colNum = colStartPosition;
                            workSheet.Cells[currentRow, colNum].Value = data.EndpointName;
                            workSheet.Cells[currentRow, ++colNum].Value = data.RoleName;
                            workSheet.Cells[currentRow, ++colNum].Value = data.RoleDescription;
                            workSheet.Cells[currentRow, ++colNum].Value = data.TCODE_Code;
                            workSheet.Cells[currentRow, ++colNum].Value = data.TCODE_Desc;
                            currentRow++;
                        }
                        workSheet.Cells.AutoFitColumns();
                        objExcelPackage.Workbook.Properties.Title = $"RoleToTCode Report for {displayName}";
                        objExcelPackage.Save();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }
    }
}
