using DI.Chakra.Model;
using DI.Chakra.Model.SAP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using DI.Chakra.Model.SOD;
using System.Data.Entity.Core.Metadata.Edm;


namespace DI.Chakra.DataAccess
{

    public class SodDA
    {

        #region Constructor

        #endregion

        #region SODDA Members


        #region Compliance Report



        public List<SodFunction>GetReportsModuleList()
        {

            using (IACMDBContext _context = new IACMDBContext())
            {
                var test = _context.SodFunction.ToList();
                return _context.SodFunction.ToList();

                //from sod in _context.Users
                //                    join org in _context.Organisations on users.OrganisationId equals org.OrganisationId
                //                    where users.ManagerId == managerId && users.FullTimeEmployee == true && users.IsActive.HasValue && users.IsActive.Value
                //                    select new UserListGridData()
                //                    {
                //                        UserId = users.UserId,
                //                        TenantId = users.TenantId,
                //                        UserName = users.UserName,
                //                        DisplayName = users.DisplayName,
                //                        EmployeeId = users.EmployeeId,
                //                        OrganisationName = org.OrganisationName,
                //                        OrganisationId = org.OrganisationId,
                //                        FirstName = users.FirstName,
                //                        EmployeeType = users.EmployeeType,
                //                        CompanyId = users.CompanyId,
                //                        DivisionId = users.DivisionId,
                //                        DepartmentId = users.DepartmentId,
                //                        CostCenter = users.CostCenterText,
                //                        DepartmentName = users.Department.DepartmentText,
                //                        DivisionName = users.Division.DivisionText,
                //                        CompanyName = users.Company.CompanyName,
                //                        BusinessRole = users.BusinessRole.DesignationDesc,
                //                        IsActive = users.IsActive.HasValue ? users.IsActive.Value : false,
                //                        FullTimeEmployee = users.FullTimeEmployee.HasValue ? users.FullTimeEmployee.Value : false,
                //                        SectionCode = users.SectionCode,
                //                        SectionName = users.SectionName,
                //                    }).ToList();
            }


        }



        public List<ComplianceReportModules> GetComplainceReportModule()
        {
            
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.ComplainceReports.ToList();
                }
            
          
        }



        public List<SODPolicyReport> GetComplainceSODPolicyReports()
        {

            using (IACMDBContext _context = new IACMDBContext())
            {

                return _context.SODPolicyReport.ToList();
            }


        }



        public List<SODReport> GetSODComplianceReport(int ReportId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SodComplianceReport
                        .Include("Function1")
                        .Include("Function2")
                        .Include("FunctionObjItem1")
                        .Include("FunctionObjItem1.EndpointType")
                        .Include("FunctionObjItem1.endpoint")
                        .Include("FunctionObjItem2")
                        .Include("FunctionObjItem2.EndpointType")
                        .Include("FunctionObjItem2.endpoint")
                        .Include("Conflict")
                        .Include("Conflict.SodRiskId")
                        .Include("exception")
                        .Include("RunPolicy")
                        .Include("RunPolicy.compliancepolicy")
                        .Where(s => s.SODRunPolicyId == ReportId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void AddComplianceReport(List<SODReport> ComplianceReport)
        {
            //List<SODReport> MatchingReports = _context.SodComplianceReport.ToList();


            //foreach (SODReport report in MatchingReports)
            //{
            //    _context.SodComplianceReport.Remove(report);
            //}

            //foreach (SODReport report in ComplianceReport)
            //{
            //    _context.SodComplianceReport.Add(report);
            //}

            //_context.SaveChanges();

        }

        public List<SODReport> GetComplianceReport(int ReportId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SodComplianceReport
                        .Include("Function1")
                        .Include("Function2")
                        .Include("FunctionObjItem1")
                        .Include("FunctionObjItem1.EndpointType")
                        .Include("FunctionObjItem1.endpoint")
                        .Include("FunctionObjItem2")
                        .Include("FunctionObjItem2.EndpointType")
                        .Include("FunctionObjItem2.endpoint")
                        .Include("Conflict")
                        .Include("Conflict.SodRiskId")
                        .Include("exception")
                        .Include("RunPolicy")
                        .Include("RunPolicy.compliancepolicy")
                        .Where(s => s.SODRunPolicyId == ReportId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveReport(List<SODPolicyReport> lstReport, string RunId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    foreach (SODPolicyReport report in lstReport)
                    {
                        report.UniqueRunCode = RunId;
                        report.PolicyDefinition = null;
                        report.Conflict = null;
                        report.exception = null;
                        report.Function1 = null;
                        report.Function2 = null;
                        report.FunctionObjItem1 = null;
                        report.FunctionObjItem2 = null;

                        _context.SODPolicyReport.Add(report);


                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }


        #endregion


        #region Risk Level

        public List<SODRiskLevel> GetAllRiskLevels()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRiskLevel.ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
       // public Dictionary<int, string> GetAllRiskLevels(string searchString)
       // {
       //     try
       //     {
       //         using (IACMDBContext _context = new IACMDBContext())
       //         {

       //             return _context.SodRiskLevel
       //                 .Where(item => item.RiskLevelCode.Contains(searchString))
       //                 .ToList()
       //                 .Select(x => new { Key = x.Id, Value = x.RiskLevelCode + " - " + x.RiskLevelDescription })
       //                 .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
       //         }
       //     }
       //     catch (Exception)
       //     {
       //         throw;
       //     }

       //}
        public void DeleteRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODRiskLevel SodRiskLevel = _context.SodRiskLevel.Find(RiskLevel.Id);
                    if (SodRiskLevel != null)
                    {
                        _context.SodRiskLevel.Remove(SodRiskLevel);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        public void DeleteRiskScore(SODRiskScore RiskScore)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODRiskScore sodriskscore = _context.SodRiskScore.Find(RiskScore.Id);
                    if (sodriskscore != null)
                    {
                        _context.SodRiskScore.Remove(sodriskscore);
                        _context.SaveChanges();
                    }                    
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        public void UpdateRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.SodRiskLevel.Attach(RiskLevel);
                    _context.Entry(RiskLevel).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODRiskLevel GetRiskLevel(int Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRiskLevel.Find(Id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        //public SODRiskLevel GetRiskLevel(string Id)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            SODRiskLevel risklevel = null;
        //            try
        //            {
        //                risklevel = _context.SodRiskLevel.First(s => s.RiskLevelCode == Id);
        //            }
        //            catch (Exception) { }

        //            return risklevel;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}


        public void AddRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    if (RiskLevel.Id == 0)
                    {

                        Context.SodRiskLevel.Add(RiskLevel);
                        Context.SaveChanges();

                        /* Context.SodRiskLevel.Attach(RiskLevel);
                         Context.Entry(RiskLevel).State = EntityState.Added;
                         Context.SaveChanges();*/
                    }
                    else
                    {
                        SODRiskLevel _risklevel = Context.SodRiskLevel.Where(rsk => rsk.Id == RiskLevel.Id).First();
                        Context.Entry(_risklevel).CurrentValues.SetValues(RiskLevel);
                        Context.SaveChanges();
                    }

                }

            }
            catch (Exception)
            {
                throw;

            }

        }

        #endregion

        #region Risk Score

        public List<SODRiskScore> GetAllRiskScores()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRiskScore.ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void DeleteRiskScore(int RiskScoreId)
        {
            try
            {
                using (IACMDBContext _dbcontext = new IACMDBContext())
                {
                    SODRiskScore RiskScore = _dbcontext.SodRiskScore.Find(RiskScoreId);
                    if (RiskScore != null)
                    {
                        _dbcontext.SodRiskScore.Remove(RiskScore);
                        _dbcontext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void UpdateRiskScore(SODRiskScore RiskScore)
        {
            try
            {
                using (IACMDBContext _dbcontext = new IACMDBContext())
                {
                    _dbcontext.SodRiskScore.Attach(RiskScore);
                    _dbcontext.Entry(RiskScore).State = EntityState.Modified;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        //public SODRiskScore GetRiskScore(int Id)
        //{
        //    try
        //    {
        //        using (IACMDBContext _dbcontext = new IACMDBContext())
        //        {
        //            return _dbcontext.SodRiskScore.Find(Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}

        public SODRiskScore GetRiskScoreByScore(int Score)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    SODRiskScore riskscore = null;
                    try
                    {
                        riskscore = _context.SodRiskScore.First(s => s.RiskScore == Score);
                    }
                    catch (Exception) { }

                    return riskscore;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }


        public void AddRiskScore(SODRiskScore RiskScore)
        {
            try
            {
                using (IACMDBContext _dbcontext = new IACMDBContext())
                {
                   
                    if (RiskScore.Id == 0)
                    {

                        _dbcontext.SodRiskScore.Add(RiskScore);
                        _dbcontext.SaveChanges();

                    }
                    else
                    {
                        SODRiskScore _riskscore = _dbcontext.SodRiskScore.Where(rsk => rsk.Id == RiskScore.Id).First();
                        _dbcontext.Entry(_riskscore).CurrentValues.SetValues(RiskScore);
                        _dbcontext.SaveChanges();
                    }

                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        #endregion

        #region Risk Code
        //public void DeleteRiskCode(int RiskId)
        //{
        //    try
        //    {
        //        using (IACMDBContext _dbcontext = new IACMDBContext())
        //        {
        //            SODRiskCode riskcode = _dbcontext.SodRiskId.Find(RiskId);

        //            _dbcontext.SodRiskId.Remove(riskcode);
        //            _dbcontext.SaveChanges();
        //        }
        //    }
        //    catch (DbUpdateException e)
        //    {
        //        var sqlException = e.GetBaseException() as SqlException;
        //        if (sqlException != null)
        //        {
        //            if (sqlException.Errors.Count > 0)
        //            {
        //                switch (sqlException.Errors[0].Number)
        //                {
        //                    case 547: // Foreign Key violation
        //                        throw new Exception("FK Exception");
        //                    default:
        //                        throw e;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw e;
        //        }
        //    }
        //}

        //public void UpdateRiskCode(SODRiskCode riskcode)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            SODRiskCode newRisk = _context.SodRiskId.Find(riskcode.Id);                
        //            newRisk.RiskCode = riskcode.RiskCode;
        //            newRisk.RiskDescription = riskcode.RiskDescription;
        //            newRisk.RiskLevelId = riskcode.RiskLevelId;
        //            newRisk.RiskScoreId = riskcode.RiskScoreId;
        //            newRisk.UserId = riskcode.UserId;
        //            newRisk.LastModifiedDate = riskcode.LastModifiedDate;
        //            newRisk.LastModifiedBy = riskcode.LastModifiedBy;
        //            newRisk.Status = riskcode.Status;
        //            _context.SodRiskId.Attach(newRisk);
        //            _context.Entry(newRisk).State = EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public SODRiskCode GetRiskCode(int Id)
        {
            try
            {
                using (IACMDBContext _dbcontext = new IACMDBContext())
                {
                    return _dbcontext.SodRiskId
                        .Include("sodrisklevel")
                        .Include("user")
                        .Include("Mitigations")
                        .Include("sodriskscore")
                        .FirstOrDefault(s => s.Id == Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SODRiskCode GetRiskCode(string Id)
        {

            try
            {
                using (IACMDBContext _dbcontext = new IACMDBContext())
                {
                    return _dbcontext.SodRiskId
                        .Include("sodrisklevel")
                        .Include("user")
                        .Include("Mitigations")
                        .Include("sodriskscore")
                        .FirstOrDefault(s => s.RiskCode == Id);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public void AddRiskCode(SODRiskCode RiskCode)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            SODRiskCode newRisk = new SODRiskCode();
        //            newRisk.RiskCode = RiskCode.RiskCode;
        //            newRisk.RiskDescription = RiskCode.RiskDescription;
        //            newRisk.Status = RiskCode.Status;
        //            newRisk.RiskLevelId = RiskCode.RiskLevelId;
        //            newRisk.RiskScoreId = RiskCode.RiskScoreId;
        //            newRisk.UserId = RiskCode.UserId;
        //            newRisk.CreatedBy = RiskCode.CreatedBy;
        //            newRisk.CreatedDate = RiskCode.CreatedDate;
        //            _context.SodRiskId.Add(newRisk);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<SODRiskCode> GetAllRiskCodes()
        {


            try
            {
                using (IACMDBContext _dbContext = new IACMDBContext())
                {
                    return _dbContext.SodRiskId
                        .Include("sodrisklevel")
                        .Include("user")
                        .Include("Mitigations")
                        .Include("sodriskscore")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Functions

        public void SaveFunction(SodFunction function)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (function.Id == 0)
                    {
                        _context.SodFunction.Add(function);
                        _context.SaveChanges();
                    }
                    else
                    {
                        SodFunction newFunction = _context.SodFunction.Include("SodFunctionItems").Where(func => func.Id == function.Id).FirstOrDefault();
                        newFunction.SodFunctionCode = function.SodFunctionCode;
                        newFunction.SodFunctionDescription = function.SodFunctionDescription;
                        newFunction.IsActive = function.IsActive;
                        newFunction.LastModifiedBy = function.LastModifiedBy;
                        newFunction.LastModifiedDate = function.LastModifiedDate;

                        //newFunction.SodFunctionItems = new List<SodFunctionItem>();
                        //newFunction.SodFunctionItems.Clear();

                        var functionItems = newFunction.SodFunctionItems.ToList();
                        foreach (var functionItem in functionItems)
                        {
                            _context.SodFunctionItem.Remove(functionItem);
                        }
                        _context.SaveChanges();

                        newFunction.SodFunctionItems = new List<SodFunctionItem>();
                        newFunction.SodFunctionItems.Clear();

                        foreach (SodFunctionItem item in function.SodFunctionItems)
                        {
                            SodFunctionItem newItem = new SodFunctionItem();

                            SodFunctionItem eItem = _context.SodFunctionItem.Find(item.Id);
                            if (eItem != null) newItem = eItem;
                            newItem.endpoint = item.endpoint;
                            newItem.Endpointsystemid = item.Endpointsystemid;
                            newItem.EndpointSystemTypeId = item.EndpointSystemTypeId;
                            newItem.EndpointType = item.EndpointType;
                            newItem.FieldCode = item.FieldCode;
                            newItem.FromValue = item.FromValue;
                            newItem.ObjectCode = item.ObjectCode;
                            newItem.SodFunctionId = item.SodFunctionId;
                            newItem.Id = item.Id;
                            newItem.SodFunctionType = item.SodFunctionType;
                            newItem.SodRoleDesc = item.SodRoleDesc;
                            newItem.SodRoleId = item.SodRoleId;
                            newItem.CreatedBy = item.CreatedBy;
                            newItem.CreatedDate = item.CreatedDate;
                            newFunction.SodFunctionItems.Add(newItem);

                        }

                        _context.SodFunction.Attach(newFunction);
                        _context.Entry(newFunction).State = EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SodFunction GetSodFunction(int Id, bool deepload)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (deepload)
                    {
                        return _context.SodFunction
                            .Include("SodFunctionItems")
                            .Include("SodFunctionItems.endpoint")
                            .Include("SodFunctionItems.EndpointType")
                            .Include("SodFunctionItems.endpoint.EndPointSystemType")
                            //.Include("ConflictFunctions")
                            //.Include("Exceptions")
                            .FirstOrDefault(s => s.Id == Id);
                    }
                    return _context.SodFunction.FirstOrDefault(s => s.Id == Id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        //public SodFunction GetSodFunction(string FunctionCode, bool deepload)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            SodFunction sodfunction = null;
        //            if (deepload)
        //            {
        //                sodfunction = _context.SodFunction
        //                    .Include("SodFunctionItems")
        //                    .Include("SodFunctionItems.endpoint")
        //                    .Include("SodFunctionItems.EndpointType")
        //                    .Include("SodFunctionItems.endpoint.EndPointSystemType")
        //                    .Include("ConflictFunctions")
        //                    .Include("Exceptions")
        //                    .FirstOrDefault(s => s.SodFunctionCode == FunctionCode);
        //            }
        //            else
        //            {
        //                sodfunction = _context.SodFunction.FirstOrDefault(s => s.SodFunctionCode == FunctionCode);
        //            }

        //            return sodfunction;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }




        //}

        public SodFunctionItem GetSodFunctionItem(long Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodFunctionItem
                        .Include("EndpointType")
                        .Include("endpoint")
                        .FirstOrDefault(s => s.Id == Id);

                }
            }
            catch (Exception)
            {
                throw;

            }

        }


        //public void UpdateSodFunction(SodFunction function)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            SodFunction newFunction = _context.SodFunction.Find(function.SodFunctionId);
        //            newFunction.SodFunctionCode = function.SodFunctionCode;
        //            newFunction.SodFunctionDescription = function.SodFunctionDescription;
        //            newFunction.IsActive = function.IsActive;

        //            newFunction.SodFunctionItems = new List<SodFunctionItem>();
        //            newFunction.SodFunctionItems.Clear();

        //            foreach (SodFunctionItem item in function.SodFunctionItems)
        //            {
        //                SodFunctionItem newItem = new SodFunctionItem();

        //                SodFunctionItem eItem = _context.SodFunctionItem.Find(item.SodFunctionItemId);
        //                if (eItem != null) newItem = eItem;
        //                newItem.endpoint = item.endpoint;
        //                newItem.Endpointsystemid = item.Endpointsystemid;
        //                newItem.EndpointSystemTypeId = item.EndpointSystemTypeId;
        //                newItem.EndpointType = item.EndpointType;
        //                newItem.FieldCode = item.FieldCode;
        //                newItem.FromValue = item.FromValue;
        //                newItem.ObjectCode = item.ObjectCode;
        //                newItem.SodFunctionId = item.SodFunctionId;
        //                newItem.SodFunctionItemId = item.SodFunctionItemId;
        //                newItem.SodFunctionType = item.SodFunctionType;
        //                newItem.SodRoleDesc = item.SodRoleDesc;
        //                newItem.SodRoleId = item.SodRoleId;

        //                newFunction.SodFunctionItems.Add(newItem);
        //            }

        //            _context.SodFunction.Attach(newFunction);
        //            _context.Entry(newFunction).State = EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}


        public List<SODReport> GetComplianceMgmtReportResult(int ReportId)
        {
            
        try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SodComplianceReport
                        .Include("Function1")
                        .Include("Function2")
                        .Include("FunctionObjItem1")
                        .Include("FunctionObjItem1.EndpointType")
                        .Include("FunctionObjItem1.endpoint")
                        .Include("FunctionObjItem2")
                        .Include("FunctionObjItem2.EndpointType")
                        .Include("FunctionObjItem2.endpoint")
                        .Include("Conflict")
                        .Include("Conflict.SodRiskId")
                        .Include("exception")
                        .Include("RunPolicy")
                        .Include("RunPolicy.compliancepolicy")
                        .Where(s => s.SODRunPolicyId == ReportId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<object> GetComplianceReportResult(string ReportType)
        {
            List<object> resultList = new List<object>();
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    switch (ReportType)
                    {
                        //case "Functions":
                        //   var functions = from function in _context.SodFunction
                        //    join functionitem in _context.SodFunctionItem
                        //    on function.Id equals functionitem.SodFunctionId
                        //    join endpointsystem in _context.EndpointSystems 
                        //    on functionitem.Endpointsystemid equals endpointsystem.Id
                        //    select new
                        //    {
                        //        FunctionCode = function.SodFunctionCode,
                        //        FunctionDescription = function.SodFunctionDescription,
                        //        EndPoint = endpointsystem.DisplayName,
                        //        FunctionType = functionitem.SodFunctionType,
                        //        FucntionValue = functionitem.SodRoleDesc,
                        //        ObjectCode = functionitem.ObjectCode,
                        //        FieldCode = functionitem.FieldCode,
                        //        FieldValue = functionitem.FromValue
                        //    };
                        //   resultList = functions.ToList<object>();
                        //break;
                        //case "RiskCode":
                        //var riskCodes = from riskCode in _context.SodRiskId
                        //                join riskLevel in _context.SodRiskLevel
                        //                on riskCode.RiskLevelId equals riskLevel.Id
                        //                join riskScore in _context.SodRiskScore
                        //                on riskCode.RiskScoreId equals riskScore.Id
                        //                join user in _context.Users 
                        //                on riskCode.UserId equals user.Id
                        //                 select new
                        //                 {
                        //                     RiskCode = riskCode.RiskCode,
                        //                     RiskDescription = riskCode.RiskDescription,
                        //                     RiskLevel = riskLevel.RiskLevelCode,
                        //                     RiskScore = riskScore.RiskScore,
                        //                     RiskCodeOwner = user.UserName
                                             
                        //                 };
                        //resultList = riskCodes.ToList<object>();
                        //break;

                        case "Conflicts":
                        var conflicts = from conflict in _context.SodConflict
                                        join function1 in _context.SodFunction
                                        on conflict.function1.Id equals function1.Id
                                        join function2 in _context.SodFunction
                                        on conflict.function2.Id equals function2.Id
                                        join riskCode in _context.SodRiskId
                                        on conflict.RiskCodeId equals riskCode.Id
                                        select new
                                        {
                                            SODConflictCode = conflict.SODConflictCode,
                                            SODConflictDescription = conflict.SODConflictDescription,
                                            SodConflictType = conflict.SodConflictType,
                                            Function1 = function1.SodFunctionCode,
                                            Function2 = function2.SodFunctionCode,
                                            RiskCode = riskCode.RiskCode
                                        };
                        resultList = conflicts.ToList<object>();
                        break;
                        //case "Exceptions":
                        //         var exceptions = from exception in _context.SodException
                        //                           join exceptionitem in _context.SodExceptionItem
                        //                           on exception.Id equals exceptionitem.ExceptionId
                        //                           join user in _context.Users 
                        //                           on exception.RiskOwner.Id equals user.Id
                        //                           select new
                        //                           {
                        //                               ExceptionCode = exception.ExceptionCode,
                        //                               ExceptionDescription = exception.ExceptionDesc,
                        //                               ExceptionOwner = user.UserName,
                        //                               User = exceptionitem.user.UserName,
                        //                               Function1 = exceptionitem.function1.SodFunctionCode,
                        //                               FunctionValue1 =  exceptionitem.functionitem1.SodRoleDesc,
                        //                               Function2 = exceptionitem.function2.SodFunctionCode,
                        //                               FunctionValue2 = exceptionitem.functionitem2.SodRoleDesc,
                        //                            };
                        //         resultList = exceptions.ToList<object>();
                        //break;
                        case "Mitigations":
                        var mitigations = _context.SodMitigation.Include("RiskCodes").ToList()
                                         .Select(emp => new
                                         {
                                             MitigationCode = emp.MitigationCode,
                                             MitigationDescription = emp.MitigationDescription,
                                             Document = emp.DocumentName,
                                             DocumentDate = emp.DocumentUploadedDate,
                                             RiskCode = emp.RiskCodes.Select(m => m.RiskCode)
                                         });
                        resultList = mitigations.ToList<object>();
                        break;
                        case "Policy":
                        var policies = from policy in _context.CompliancePolicy
                                      join user in _context.Users
                                      on (long)policy.OwnerId equals user.Id
                                          select new
                                          {
                                              PolicyName = policy.CompliancePolicyCode,
                                              PolicyCode = policy.CompliancePolicyCode,
                                              PolicyDescription = policy.CompliancePolicyDescription,
                                              PolicyOwner = user.UserName
                                          };
                        resultList = policies.ToList<object>();
                        break;
                        case "PolicyExecution":
                        var policyExecutions = from policy in _context.SODPolicyExceutionDefinitions.Include("CompliancePolicy")
                                       select new
                                       {
                                           PolicyCode = policy.CompliancePolicy.CompliancePolicyCode,
                                           PolicyDescription = policy.CompliancePolicy.CompliancePolicyDescription,
                                           UniqueRunCode = policy.UniqueRunCode,
                                           OutputType = policy.OutputType,
                                           RunAtObjectLevel = policy.RunAtObjectLevel.Value,
                                       };
                        resultList = policyExecutions.ToList<object>();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return resultList;
        }

        public void DeleteSodFunction(int sodfunctionid)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SodFunction function = _context.SodFunction
                        .Include("SodFunctionItems")
                        .Include("ConflictFunctions")
                        .FirstOrDefault(f => f.Id == sodfunctionid);

                    if (function != null)
                    {
                        //RemoveMatchingFunctions(function.SodFunctionId);
                        _context.SodFunction.Remove(function);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        //public List<SodFunction> GetAllSodFunctions()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return _context.SodFunction
        //                .Include("SodFunctionItems")
        //                .Include("SodFunctionItems.endpoint")
        //                .Include("SodFunctionItems.EndpointType")
        //                .Include("SodFunctionItems.endpoint.EndPointSystemType")
        //                .Include("ConflictFunctions")
        //                .Include("Exceptions")
        //                .ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        //public Dictionary<long, string> GetAllSodFunctions(string searchString)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return _context.SodFunction
        //                .Where(item => item.SodFunctionCode.Contains(searchString))
        //                .ToList()
        //                .Select(x => new { Key = x.Id, Value = x.SodFunctionCode + " - " + x.SodFunctionDescription })
        //                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        public Dictionary<long, string> GetAllCompliancePolicy(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.CompliancePolicy
                        .Where(item => item.CompliancePolicyCode.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.CompliancePolicyCode + " - " + x.CompliancePolicyDescription })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch (Exception)
            {
                throw;

            }
        }

        public Dictionary<long, string> GetAllSodConflictFunctions(string searchString,string filterString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    if(String.IsNullOrEmpty(filterString))
                    {
                    return _context.SodConflictFunction
                        .Where(item => item.SodConflictCode.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.SodConflictCode + " - " + x.SodConflictDescription })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => (long)o.Key, o => o.Value);
                    }
                    else
                    {
                        return _context.SodConflictFunction
                        .Where(item => item.SodConflictCode.Contains(searchString) && item.CompliancePolicyType==filterString)
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.SodConflictCode + " - " + x.SodConflictDescription })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => (long)o.Key, o => o.Value);
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public Dictionary<long, string> GetAllSodPolicyDefinitions(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {


                    return _context.SODPolicyExceutionDefinitions
                        .Where(item => item.UniqueRunCode.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.UniqueRunCode })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => (long)o.Key, o => o.Value);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public List<SodFunctionItem> GetMatchingFunctionItems(int FunctionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodFunctionItem.Where(s => s.SodFunctionId == FunctionId)
                        .Include("EndpointType")
                        .Include("endpoint")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public bool IsComplianceProcessNameExists(string Name)
        {
            bool isExists = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ComplianceProcess process = oIACMDBContext.ComplianceProcess.Where(m => m.ProcessName == Name).FirstOrDefault();
                    if (process != null)
                    {
                        isExists = true;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return isExists;
        }
        public long SaveComplianceProcess(ComplianceProcess complianceProcess)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (complianceProcess.Id == 0)
                    {
                        oIACMDBContext.ComplianceProcess.Attach(complianceProcess);
                        oIACMDBContext.Entry(complianceProcess).State = System.Data.Entity.EntityState.Added;
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var dbComplianceProcess = oIACMDBContext.ComplianceProcess.Find(complianceProcess.Id);
                        dbComplianceProcess.LastModifiedBy = complianceProcess.LastModifiedBy;
                        dbComplianceProcess.LastModifiedDate = complianceProcess.LastModifiedDate;
                        var dbPolicyExecutionDefinition = oIACMDBContext.SODPolicyExceutionDefinitions.Find(complianceProcess.PolicyExecutionDefinitionId);
                        dbComplianceProcess.PolicyExecutionDefinition = dbPolicyExecutionDefinition;
                        dbComplianceProcess.PolicyExecutionDefinitionId = dbPolicyExecutionDefinition.Id;
                        oIACMDBContext.SaveChanges();
                    }
                    return complianceProcess.Id;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void RemoveMatchingFunctions(int FunctionNo)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SodFunctionItem> templates = _context.SodFunctionItem.Where(s => s.SodFunctionId == FunctionNo).ToList();

                    foreach (SodFunctionItem pTemplate in templates)
                        _context.SodFunctionItem.Remove(pTemplate);

                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        #endregion

        #region Conflicts

        public bool CheckForDuplicateConflict(SODConflict conflict)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SODConflict> conflicts = _context.SodConflict.Where(s => s.SODConflictCode == conflict.SODConflictCode).ToList(); // && s.FunctionId1 == conflict.FunctionId1 && s.FunctionId2 == conflict.FunctionId2).ToList();
                    return conflicts.Count > 0;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public List<SODConflict> GetAllSodConflicts()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodConflict
                        .Include("SodRiskId")
                        .Include("function1")
                        .Include("function2")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }


        }


        public void DeleteSodConflict(SODConflict sodconflict)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODConflict conflict = _context.SodConflict.Find(sodconflict.Id);
                    if (conflict != null)
                    {
                        _context.SodConflict.Remove(conflict);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }

        }



        public SODConflict GetSodConflict(int id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodConflict
                        .Include("SodRiskId")
                        .Include("SodRiskId.sodrisklevel")
                        .Include("SodRiskId.sodriskscore")
                        .Include("function1")
                        .Include("function1.SodFunctionItems")
                        .Include("function1.SodFunctionItems.EndpointType")
                        .Include("function1.SodFunctionItems.endpoint")
                        .Include("function1.SodFunctionItems.endpoint.EndPointSystemType")
                        .Include("function2")
                        .Include("function2.SodFunctionItems")
                        .Include("function2.SodFunctionItems.EndpointType")
                        .Include("function2.SodFunctionItems.endpoint")
                        .Include("function2.SodFunctionItems.endpoint.EndPointSystemType")
                        .FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void AddSodConflict(SODConflict sodconflict)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    _context.SodConflict.Add(sodconflict);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public bool IsFunctionCodeExists(string Functioncode)
        {
            bool isExists = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    SodFunction function = oIACMDBContext.SodFunction.Where(item => item.SodFunctionCode == Functioncode).FirstOrDefault();
                    if (function != null)
                    {
                        isExists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExists;
        }
        //public bool IsFunctionConflictCodeExists(string ConflictFunctioncode)
        //{
        //    bool isExists = false;
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            SODConflictFunction function = oIACMDBContext.SodConflictFunction.Where(item => item.SodConflictCode == ConflictFunctioncode).FirstOrDefault();
        //            if (function != null)
        //            {
        //                isExists = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return isExists;
        //}
        public bool IsCompliancePolicyCodeExists(string CompliancePolicyCode)
        {
            bool isExists = false;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    CompliancePolicy policy = oIACMDBContext.CompliancePolicy.Where(item => item.CompliancePolicyCode == CompliancePolicyCode).FirstOrDefault();
                    if (policy != null)
                    {
                        isExists = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExists;
        }
        public void SavePolicyExceutionDefinition(SODPolicyExceutionDefinition policyDefinition)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (policyDefinition.Id == 0)
                    {
                        SODPolicyExceutionDefinition definition = new SODPolicyExceutionDefinition();
                        definition.CreatedBy = policyDefinition.CreatedBy;
                        definition.CreatedDate = policyDefinition.CreatedDate;
                        definition.UniqueRunCode = policyDefinition.UniqueRunCode;
                        definition.OutputType = policyDefinition.OutputType;
                        CompliancePolicy policy = oIACMDBContext.CompliancePolicy.Find(policyDefinition.CompliancePolicyId);
                        definition.CompliancePolicy = policy;
                        definition.CompliancePolicyId = policy.Id;
                        definition.RunAtObjectLevel = policyDefinition.RunAtObjectLevel;
                        definition.EndpointSelection = new List<SODEndPointPolicySelection>();
                        foreach (SODEndPointPolicySelection endpoint in policyDefinition.EndpointSelection)
                        {
                            definition.EndpointSelection.Add(endpoint);
                        }
                        definition.Exceptions = new List<SODException>();
                        definition.Exceptions.Clear();
                        foreach (SODException exception in policyDefinition.Exceptions)
                        {
                            SODException exceptionItem = oIACMDBContext.SodException.Find(exception.Id);
                            definition.Exceptions.Add(exceptionItem);
                        }


                        oIACMDBContext.SODPolicyExceutionDefinitions.Add(definition);
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var dbSODPolicyExceutionDefinition = oIACMDBContext.SODPolicyExceutionDefinitions
                                               .Include("Exceptions").Include("EndpointSelection")
                                               .Single(c => c.Id == policyDefinition.Id);

                        oIACMDBContext.Entry(dbSODPolicyExceutionDefinition).CurrentValues.SetValues(policyDefinition);

                        var v = dbSODPolicyExceutionDefinition.Exceptions.ToList();
                        foreach (var item in v)
                        {
                            dbSODPolicyExceutionDefinition.Exceptions.Remove(item);
                        }

                        if (policyDefinition.Exceptions != null)
                        {
                            foreach (var exception in policyDefinition.Exceptions)
                            {
                                var item = dbSODPolicyExceutionDefinition.Exceptions.SingleOrDefault(s => s.Id == exception.Id);
                                if (item != null)
                                    oIACMDBContext.Entry(item).CurrentValues.SetValues(exception);
                                else
                                {
                                    var dbSodException = oIACMDBContext.SodException.Where(t => t.Id == exception.Id).FirstOrDefault();

                                    if (dbSodException != null)
                                        dbSODPolicyExceutionDefinition.Exceptions.Add(dbSodException);
                                }
                            }
                        }

                        var endpoints = dbSODPolicyExceutionDefinition.EndpointSelection.ToList();
                        foreach (var endpoint in endpoints)
                        {
                            oIACMDBContext.SODEndPointPolicySelection.Remove(endpoint);
                        }
                        oIACMDBContext.SaveChanges();
                        foreach (var endpoint in policyDefinition.EndpointSelection)
                        {
                            dbSODPolicyExceutionDefinition.EndpointSelection.Add(endpoint);
                        }
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveConflictFunction(SODConflictFunction conflictFunction)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (conflictFunction.Id == 0)
                    {
                        //oIACMDBContext.SodConflictFunction.Add(conflictFunction);
                        //oIACMDBContext.SaveChanges();
                        //oIACMDBContext.SodConflictFunction.Attach(conflictFunction);
                        //oIACMDBContext.Entry(conflictFunction).State = System.Data.Entity.EntityState.Added;
                        //oIACMDBContext.Set<SODConflictFunction>().Add(conflictFunction);
                        //oIACMDBContext.SaveChanges();

                        SODConflictFunction conflict = new SODConflictFunction();
                        conflict.IsActive = conflictFunction.IsActive;
                        conflict.LastGeneratedDate = conflictFunction.LastGeneratedDate;
                        conflict.RiskCodeId = conflictFunction.RiskCodeId;
                        conflict.CompliancePolicyType = conflictFunction.CompliancePolicyType;
                        conflict.SodConflictCode = conflictFunction.SodConflictCode;
                        conflict.SodConflictDescription = conflictFunction.SodConflictDescription;
                        conflict.CreatedBy = conflictFunction.CreatedBy;
                        conflict.CreatedDate = conflictFunction.CreatedDate;
                        //conflict.SodConflictType = ConflictFunction.SodConflictType;
                        conflict.Status = conflictFunction.Status;
                        conflict.Functions = new List<SodFunction>();
                        conflict.Functions.Clear();
                        foreach (SodFunction function in conflictFunction.Functions)
                        {
                            SodFunction fun = oIACMDBContext.SodFunction.Find(function.Id);
                            conflict.Functions.Add(fun);
                        }


                        oIACMDBContext.SodConflictFunction.Add(conflict);
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var dbSODConflictFunction = oIACMDBContext.SodConflictFunction
                                               .Include("Functions")
                                               .Single(c => c.Id == conflictFunction.Id);

                        oIACMDBContext.Entry(dbSODConflictFunction).CurrentValues.SetValues(conflictFunction);

                        var v = dbSODConflictFunction.Functions.ToList();
                        foreach (var item in v)
                        {
                            dbSODConflictFunction.Functions.Remove(item);
                        }

                        if (conflictFunction.Functions != null)
                        {
                            foreach (var function in conflictFunction.Functions)
                            {
                                var item = dbSODConflictFunction.Functions.SingleOrDefault(s => s.Id == function.Id);
                                if (item != null)
                                    oIACMDBContext.Entry(item).CurrentValues.SetValues(function);
                                else
                                {
                                    var dbSodFunction = oIACMDBContext.SodFunction.Where(t => t.Id == function.Id).FirstOrDefault();

                                    if (dbSodFunction != null)
                                        dbSODConflictFunction.Functions.Add(dbSodFunction);
                                }
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveCompliancePolicy(CompliancePolicy policy)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (policy.Id == 0)
                    {
                        CompliancePolicy compliancePolicy = new CompliancePolicy();
                        compliancePolicy.IsActive = policy.IsActive;
                        compliancePolicy.CompliancePolicyCode = policy.CompliancePolicyCode;
                        compliancePolicy.CompliancePolicyDescription = policy.CompliancePolicyDescription;
                        compliancePolicy.CompliancePolicyType = policy.CompliancePolicyType;
                        compliancePolicy.Status = policy.Status;
                        User owner = oIACMDBContext.Users.Find(policy.OwnerId);
                        compliancePolicy.Owner = owner;
                        compliancePolicy.OwnerId = owner.Id;
                        compliancePolicy.CreatedBy = policy.CreatedBy;
                        compliancePolicy.CreatedDate = policy.CreatedDate;
                        compliancePolicy.Conflicts = new List<SODConflictFunction>();
                        compliancePolicy.Conflicts.Clear();
                        foreach (SODConflictFunction function in policy.Conflicts)
                        {
                            SODConflictFunction fun = oIACMDBContext.SodConflictFunction.Find(function.Id);
                            compliancePolicy.Conflicts.Add(fun);
                        }


                        oIACMDBContext.CompliancePolicy.Add(compliancePolicy);
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {
                        var dbCompliancePolicy = oIACMDBContext.CompliancePolicy
                                               .Include("Conflicts")
                                               .Single(c => c.Id == policy.Id);

                        oIACMDBContext.Entry(dbCompliancePolicy).CurrentValues.SetValues(policy);

                        var v = dbCompliancePolicy.Conflicts.ToList();
                        foreach (var item in v)
                        {
                            dbCompliancePolicy.Conflicts.Remove(item);
                        }

                        if (policy.Conflicts != null)
                        {
                            foreach (var function in policy.Conflicts)
                            {
                                var item = dbCompliancePolicy.Conflicts.SingleOrDefault(s => s.Id == function.Id);
                                if (item != null)
                                    oIACMDBContext.Entry(item).CurrentValues.SetValues(function);
                                else
                                {
                                    var dbSodConflictFunction = oIACMDBContext.SodConflictFunction.Where(t => t.Id == function.Id).FirstOrDefault();

                                    if (dbSodConflictFunction != null)
                                        dbCompliancePolicy.Conflicts.Add(dbSodConflictFunction);
                                }
                            }
                        }
                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public void AddConflictFunction(SODConflictFunction ConflictFunction)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {


        //            SODConflictFunction conflict = new SODConflictFunction();
        //            conflict.IsActive = ConflictFunction.IsActive;
        //            conflict.LastGeneratedDate = ConflictFunction.LastGeneratedDate;
        //            conflict.RiskCodeId = ConflictFunction.RiskCodeId;
        //            conflict.SodConflictCode = ConflictFunction.SodConflictCode;
        //            conflict.SodConflictDescription = ConflictFunction.SodConflictDescription;
        //            //conflict.SodConflictType = ConflictFunction.SodConflictType;
        //            conflict.Status = ConflictFunction.Status;
        //            conflict.Functions = new List<SodFunction>();
        //            conflict.Functions.Clear();
        //            foreach (SodFunction function in ConflictFunction.Functions)
        //            {
        //                SodFunction fun = _context.SodFunction.Find(function.SodFunctionId);
        //                conflict.Functions.Add(fun);
        //            }


        //            _context.SodConflictFunction.Add(conflict);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}

        public SODConflictFunction GetConflictFunction(int RowNo)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodConflictFunction
                        .Include("SodRiskCode")
                        .Include("Functions")
                        .Include("CompliancePolicies")
                        .FirstOrDefault(s => s.Id == RowNo);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        //public void UpdateConflictFunction(SODConflictFunction conflictfunction)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            SODConflictFunction conflict = _context.SodConflictFunction.Find(conflictfunction.SodConflictFunctionId);
        //            conflict.IsActive = conflictfunction.IsActive;
        //            conflict.LastGeneratedDate = conflictfunction.LastGeneratedDate;
        //            conflict.RiskCodeId = conflictfunction.RiskCodeId;
        //            conflict.SodConflictCode = conflictfunction.SodConflictCode;
        //            conflict.SodConflictDescription = conflictfunction.SodConflictDescription;
        //            //conflict.SodConflictType = conflictfunction.SodConflictType;
        //            conflict.Status = conflictfunction.Status;
        //            conflict.Functions = new List<SodFunction>();
        //            conflict.Functions.Clear();
        //            foreach (SodFunction function in conflictfunction.Functions)
        //            {
        //                SodFunction fun = _context.SodFunction.Find(function.SodFunctionId);
        //                conflict.Functions.Add(fun);
        //            }

        //            _context.SodConflictFunction.Attach(conflict);
        //            _context.Entry(conflict).State = EntityState.Modified;
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        public void UpdateLastGenerateddate(SODConflictFunction conflictFunction)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.SodConflictFunction.Attach(conflictFunction);
                    _context.Entry(conflictFunction).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }
        }
        public List<SODConflictFunction> GetAllSODConflictFunction()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodConflictFunction
                        .Include("SodRiskCode")
                        .Include("Functions")
                        //.Include("CompliancePolicies")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public List<SODPolicyExceutionDefinition> GetAllSODPolicyExecution()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SODPolicyExceutionDefinitions
                        .Include("CompliancePolicy")
                        //.Include("CompliancePolicies")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public SODPolicyExceutionDefinition GetSODPolicyExceutionDefinition(int sodPolicyExecutionDefinitionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SODPolicyExceutionDefinitions
                        .Include("CompliancePolicy").Include("EndpointSelection").Include("Exceptions").Where(m => m.Id == sodPolicyExecutionDefinitionId).FirstOrDefault();

                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        //public void DeleteSODConflictFunction(SODConflictFunction conflictfunction)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            SODConflictFunction cFunction = _context.SodConflictFunction.Find(conflictfunction.SodConflictFunctionId);
        //            if (cFunction != null)
        //            {
        //                _context.SodConflictFunction.Remove(cFunction);
        //                _context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        public void DeleteSODConflictFunction(int SodConflictFunctionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODConflictFunction cFunction = _context.SodConflictFunction.Find(SodConflictFunctionId);
                    if (cFunction != null)
                    {
                        _context.SodConflictFunction.Remove(cFunction);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }                
            }
        }

        public void DeleteSODPolicyExecution(int SODPolicyExecutionId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODPolicyExceutionDefinition definition = _context.SODPolicyExceutionDefinitions.Find(SODPolicyExecutionId);
                    if (definition != null)
                    {
                        _context.SODPolicyExceutionDefinitions.Remove(definition);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        public void RemoveMatchingConflicts(string ConflictCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SODConflict> lstConflict = _context.SodConflict.Where(s => s.SODConflictCode == ConflictCode).ToList();
                    foreach (SODConflict conflict in lstConflict)
                    {
                        _context.SodConflict.Remove(conflict);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }



        #endregion

        #region Mitigations


        public void DeleteSodMitigation(SODMitigation mitigation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODMitigation mitigate = _context.SodMitigation.Find(mitigation.Id);
                    if (mitigate != null)
                    {
                        _context.SodMitigation.Remove(mitigate);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void UpdateSodMitigation(SODMitigation mitigation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    SODMitigation mitigate = _context.SodMitigation.Find(mitigation.Id);
                    mitigate.ApproverGroupId = mitigation.ApproverGroupId;
                    mitigate.ApproverUserId = mitigation.ApproverUserId;
                    mitigate.DocumentName = mitigation.DocumentName;
                    mitigate.DocumentUploadedDate = mitigation.DocumentUploadedDate;
                    mitigate.DocumentUploadedPath = mitigation.DocumentUploadedPath;
                    mitigate.IsActive = mitigation.IsActive;
                    mitigate.MitigationCode = mitigation.MitigationCode;
                    mitigate.MitigationDescription = mitigation.MitigationDescription;
                    mitigate.MitigationSteps = mitigation.MitigationSteps;
                    mitigate.MonitorGroupId = mitigation.MonitorGroupId;
                    mitigate.MonitorUserId = mitigation.MonitorUserId;
                    mitigate.RiskCodes = new List<SODRiskCode>();
                    mitigate.RiskCodes.Clear();
                    foreach (SODRiskCode risk in mitigation.RiskCodes)
                    {
                        SODRiskCode newRisk = _context.SodRiskId.Find(risk.Id);
                        mitigate.RiskCodes.Add(newRisk);
                    }

                    _context.SodMitigation.Attach(mitigate);
                    _context.Entry(mitigate).State = EntityState.Modified;

                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODMitigation GetSodMitigation(int id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodMitigation
                        .Include("RiskCodes")
                        .FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODMitigation GetSodMitigation(string code)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    SODMitigation mitigation = null;
                    try
                    {
                        mitigation = _context.SodMitigation
                        .Include("RiskCodes")
                            .FirstOrDefault(s => s.MitigationCode == code);
                    }
                    catch (Exception ex) { }

                    return mitigation;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void AddSodMitigation(SODMitigation mitigation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODMitigation mitigate = new SODMitigation();

                    mitigate.ApproverGroupId = mitigation.ApproverGroupId;
                    mitigate.ApproverUserId = mitigation.ApproverUserId;
                    mitigate.DocumentName = mitigation.DocumentName;
                    mitigate.DocumentUploadedDate = mitigation.DocumentUploadedDate;
                    mitigate.DocumentUploadedPath = mitigation.DocumentUploadedPath;
                    mitigate.IsActive = mitigation.IsActive;
                    mitigate.MitigationCode = mitigation.MitigationCode;
                    mitigate.MitigationDescription = mitigation.MitigationDescription;
                    mitigate.MitigationSteps = mitigation.MitigationSteps;
                    mitigate.MonitorGroupId = mitigation.MonitorGroupId;
                    mitigate.MonitorUserId = mitigation.MonitorUserId;
                    mitigate.RiskCodes = new List<SODRiskCode>();
                    mitigate.RiskCodes.Clear();
                    foreach (SODRiskCode risk in mitigation.RiskCodes)
                    {
                        SODRiskCode newRisk = _context.SodRiskId.Find(risk.Id);
                        mitigate.RiskCodes.Add(newRisk);
                    }
                    _context.SodMitigation.Add(mitigate);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public List<SODMitigation> GetAllSodMitigations()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    List<SODMitigation> mitigationlist = _context.SodMitigation
                        .Include(m => m.RiskCodes).ToList();
                    return mitigationlist;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Compliance Policy


        public void DeleteCompliancePolicy(int policyId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    CompliancePolicy policy = _context.CompliancePolicy.Find(policyId);
                    if (policy != null)
                    {
                        _context.CompliancePolicy.Remove(policy);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        //    public static void AttachByIdValue<TEntity>(this DbContext context, TEntity rootEntity, HashSet<Type> childTypes)
        //where TEntity : class, IEntity
        //    {
        //        // mark root entity as added
        //        // this action adds whole graph and marks each entity in it as added
        //        context.Set<TEntity>().Add(rootEntity);
        //        // in case root entity has id value mark it as modified (otherwise it stays added)
        //        if (rootEntity.Id != 0)
        //        {
        //            context.Entry(rootEntity).State = EntityState.Modified;
        //        }
        //        // traverse all entities in context (hopefully they are all part of graph we just attached)
        //        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        //        {
        //            // we are only interested in graph we have just attached
        //            // and we know they are all marked as Added 
        //            // and we will ignore root entity because it is already resolved correctly
        //            if (entry.State == EntityState.Added && entry.Entity != rootEntity)
        //            {
        //                // if no child types are defined for saving then just mark all entities as unchanged)
        //                if (childTypes == null || childTypes.Count == 0)
        //                {
        //                    entry.State = EntityState.Unchanged;
        //                }
        //                else
        //                {
        //                    // request object type from context because we might got reference to dynamic proxy
        //                    // and we wouldn't want to handle Type of dynamic proxy
        //                    Type entityType = ObjectContext.GetObjectType(entry.Entity.GetType());
        //                    // if type is not child type than it should not be saved so mark it as unchanged
        //                    if (!childTypes.Contains(entityType))
        //                    {
        //                        entry.State = EntityState.Unchanged;
        //                    }
        //                    else if (entry.Entity.Id != 0)
        //                    {
        //                        // if entity should be saved with root entity
        //                        // than if it has id mark it as modified 
        //                        // else leave it marked as added
        //                        entry.State = EntityState.Modified;
        //                    }
        //                }
        //            }
        //        }
        //    }  

        public void UpdateCompliancePolicy(CompliancePolicy cPolicy)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    CompliancePolicy policy = _context.CompliancePolicy.Find(cPolicy.Id);
                    policy.CompliancePolicyCode = cPolicy.CompliancePolicyCode;
                    policy.CompliancePolicyDescription = cPolicy.CompliancePolicyDescription;
                    policy.CompliancePolicyType = cPolicy.CompliancePolicyType;
                    policy.OwnerId = cPolicy.OwnerId;
                    policy.IsActive = cPolicy.IsActive;
                    policy.Conflicts = new List<SODConflictFunction>();
                    policy.Conflicts.Clear();

                    foreach (SODConflictFunction conflict in cPolicy.Conflicts)
                    {
                        SODConflictFunction conflictfun = _context.SodConflictFunction.Find(conflict.Id);
                        policy.Conflicts.Add(conflictfun);
                    }
                    _context.CompliancePolicy.Attach(policy);
                    _context.Entry(policy).State = EntityState.Modified;
                    _context.SaveChanges();


                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public List<SodFunction> GetSodFunctionFromConflictFunction(string ConflictCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODConflictFunction sodfunction = _context.SodConflictFunction
                        .Include("Functions")
                        .Include("Functions.SodFunctionItems")
                        .Include("Functions.SodFunctionItems.EndpointType")
                        .Include("Functions.SodFunctionItems.endpoint")
                        .Include("Functions.SodFunctionItems.endpoint.EndPointSystemType")
                        .First(cFunction => cFunction.SodConflictCode == ConflictCode);
                    return sodfunction.Functions;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SODConflict> GetSodConflictList(string ConflictCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodConflict
                        .Include("SodRiskId")
                        .Include("SodRiskId.sodrisklevel")
                        .Include("SodRiskId.sodriskscore")
                        .Include("function1")
                        .Include("function1.SodFunctionItems")
                        .Include("function1.SodFunctionItems.EndpointType")
                        .Include("function1.SodFunctionItems.endpoint")
                        .Include("function1.SodFunctionItems.endpoint.EndPointSystemType")
                        .Include("function2")
                        .Include("function2.SodFunctionItems")
                        .Include("function2.SodFunctionItems.EndpointType")
                        .Include("function2.SodFunctionItems.endpoint")
                        .Include("function2.SodFunctionItems.endpoint.EndPointSystemType")
                        .Where(s => s.SODConflictCode == ConflictCode).ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public CompliancePolicy GetCompliancePolicy(int id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.CompliancePolicy
                        .Include("Owner")
                        .Include("Conflicts")
                        .FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public ComplianceProcess GetComplianceProcess(int id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.ComplianceProcess
                        .Include("PolicyExecutionDefinition")
                        .Include("PolicyExecutionDefinition.CompliancePolicy")
                        .Include("PolicyExecutionDefinition.CompliancePolicy.Conflicts")
                        .Include("PolicyExecutionDefinition.EndpointSelection")
                        .Include("PolicyExecutionDefinition.Exceptions")
                        .FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        public CompliancePolicy GetCompliancePolicy(string code)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    CompliancePolicy policy = null;
                    try
                    {
                        policy = _context.CompliancePolicy
                                .Include("Owner")
                                .Include("Conflicts")
                                .FirstOrDefault(s => s.CompliancePolicyCode == code);
                    }
                    catch (Exception ex) { }

                    return policy;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void AddCompliancePolicy(CompliancePolicy cPolicy)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    CompliancePolicy policy = new CompliancePolicy();
                    policy.CompliancePolicyCode = cPolicy.CompliancePolicyCode;
                    policy.CompliancePolicyDescription = cPolicy.CompliancePolicyDescription;
                    policy.CompliancePolicyType = cPolicy.CompliancePolicyType;
                    policy.OwnerId = cPolicy.OwnerId;
                    policy.Conflicts = new List<SODConflictFunction>();
                    policy.Conflicts.Clear();

                    foreach (SODConflictFunction conflict in cPolicy.Conflicts)
                    {
                        SODConflictFunction conflictfun = _context.SodConflictFunction.Find(conflict.Id);
                        policy.Conflicts.Add(conflictfun);
                    }
                    _context.CompliancePolicy.Add(policy);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public List<CompliancePolicy> GetAllCompliancePolicy()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.CompliancePolicy.ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }


        }

        public List<ComplianceType> GetAllComplianceTypes()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.ComplianceType.ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }


        }

        #endregion


        #region Exceptions

        public void SaveException(SODException Exception)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    if (Exception.Id == 0)
                    {
                        SODException newException = new SODException();
                        newException.ExceptionCode = Exception.ExceptionCode;
                        newException.ExceptionDesc = Exception.ExceptionDesc;
                        newException.Id = Exception.Id;
                        newException.RiskOwnerId = Exception.RiskOwnerId;
                        newException.ExceptionItems = new List<SodExceptionItem>();
                        newException.ExceptionItems.Clear();
                        foreach (SodExceptionItem item in Exception.ExceptionItems)
                        {
                            SodExceptionItem newItem = new SodExceptionItem();
                            newItem.ExceptionId = item.ExceptionId;
                            newItem.FunctionId1 = item.FunctionId1;
                            newItem.FunctionId2 = item.FunctionId2;
                            newItem.FunctionItemId1 = item.FunctionItemId1;
                            newItem.FunctionItemId2 = item.FunctionItemId2;
                            newItem.UserId = item.UserId;
                            newException.ExceptionItems.Add(newItem);
                        }
                        oIACMDBContext.SodException.Add(newException);
                        oIACMDBContext.SaveChanges();

                    }
                    else
                    {

                        SODException newException = oIACMDBContext.SodException.Include("ExceptionItems").Where(exc => exc.Id == Exception.Id).FirstOrDefault();
                        newException.ExceptionCode = Exception.ExceptionCode;
                        newException.ExceptionDesc = Exception.ExceptionDesc;
                        User owner = oIACMDBContext.Users.Find(Exception.RiskOwnerId);
                        newException.RiskOwner = owner;
                        newException.RiskOwnerId = owner.Id;

                        var exceptionItems = newException.ExceptionItems.ToList();
                        foreach (var exceptionItem in exceptionItems)
                        {
                            oIACMDBContext.SodExceptionItem.Remove(exceptionItem);
                        }
                        oIACMDBContext.SaveChanges();

                        newException.ExceptionItems = new List<SodExceptionItem>();
                        newException.ExceptionItems.Clear();

                        foreach (SodExceptionItem item in Exception.ExceptionItems)
                        {
                            SodExceptionItem newItem = new SodExceptionItem();

                            SodExceptionItem eItem = oIACMDBContext.SodExceptionItem.Find(item.Id);
                            if (eItem != null) newItem = eItem;
                            if (item.function1 != null)
                            {
                                SodFunction function1 = oIACMDBContext.SodFunction.Find(item.function1.Id);
                                newItem.function1 = function1;
                                newItem.FunctionId1 = function1.Id;
                            }
                            if (item.function2 != null)
                            {
                                SodFunction function2 = oIACMDBContext.SodFunction.Find(item.function2.Id);
                                newItem.function2 = function2;
                                newItem.FunctionId2 = function2.Id;
                            }
                            if (item.functionitem1 != null)
                            {
                                SodFunctionItem functionItem1 = oIACMDBContext.SodFunctionItem.Find(item.functionitem1.Id);
                                newItem.functionitem1 = functionItem1;
                                newItem.FunctionItemId1 = functionItem1.Id;
                            }
                            if (item.functionitem2 != null)
                            {
                                SodFunctionItem functionItem2 = oIACMDBContext.SodFunctionItem.Find(item.functionitem2.Id);
                                newItem.functionitem2 = functionItem2;
                                newItem.FunctionItemId2 = functionItem2.Id;
                            }
                            if (item.user != null)
                            {
                                User user = oIACMDBContext.Users.Find(item.user.Id);
                                newItem.user = user;
                                newItem.UserId = user.Id;
                            }
                            newException.ExceptionItems.Add(newItem);

                        }

                        oIACMDBContext.SodException.Attach(newException);
                        oIACMDBContext.Entry(newException).State = EntityState.Modified;
                        oIACMDBContext.SaveChanges();

                       
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void AddException(SODException Exception)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            SODException newException = new SODException();
        //            newException.ExceptionCode = Exception.ExceptionCode;
        //            newException.ExceptionDesc = Exception.ExceptionDesc;
        //            newException.ExceptionId = Exception.ExceptionId;
        //            newException.RiskOwnerId = Exception.RiskOwnerId;
        //            newException.ExceptionItems = new List<SodExceptionItem>();
        //            newException.ExceptionItems.Clear();
        //            foreach (SodExceptionItem item in Exception.ExceptionItems)
        //            {
        //                SodExceptionItem newItem = new SodExceptionItem();
        //                newItem.ExceptionId = item.ExceptionId;
        //                newItem.FunctionId1 = item.FunctionId1;
        //                newItem.FunctionId2 = item.FunctionId2;
        //                newItem.FunctionItemId1 = item.FunctionItemId1;
        //                newItem.FunctionItemId2 = item.FunctionItemId2;
        //                newItem.UserId = item.UserId;
        //                newException.ExceptionItems.Add(newItem);
        //            }
        //            _context.SodException.Add(newException);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }


        //}

        public void DeleteException(SODException exception)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODException SodException = _context.SodException
                        .Include("ExceptionItems")
                        .FirstOrDefault(s => s.Id == exception.Id);

                    if (SodException != null)
                    {
                        _context.SodException.Remove(SodException);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void UpdateException(SODException Exception)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODException newException = _context.SodException.Find(Exception.Id);
                    newException.ExceptionCode = Exception.ExceptionCode;
                    newException.ExceptionDesc = Exception.ExceptionDesc;
                    newException.Id = Exception.Id;
                    newException.RiskOwnerId = Exception.RiskOwnerId;
                    newException.ExceptionItems = new List<SodExceptionItem>();
                    newException.ExceptionItems.Clear();
                    foreach (SodExceptionItem item in Exception.ExceptionItems)
                    {
                        SodExceptionItem newItem = new SodExceptionItem();
                        SodExceptionItem exItem = _context.SodExceptionItem.Find(item.Id);
                        if (exItem != null) newItem = exItem;
                        newItem.ExceptionId = item.ExceptionId;
                        newItem.FunctionId1 = item.FunctionId1;
                        newItem.FunctionId2 = item.FunctionId2;
                        newItem.FunctionItemId1 = item.FunctionItemId1;
                        newItem.FunctionItemId2 = item.FunctionItemId2;
                        newItem.UserId = item.UserId;
                        newItem.Id = item.Id;
                        newItem.function1 = item.function1;
                        newItem.function2 = item.function2;
                        newItem.functionitem1 = item.functionitem1;
                        newItem.functionitem2 = item.functionitem2;
                        newItem.user = item.user;
                        newException.ExceptionItems.Add(newItem);
                    }


                    _context.SodException.Attach(newException);
                    _context.Entry(newException).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODException GetSODException(long id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodException
                        .Include("RiskOwner")
                        .Include("ExceptionItems")
                        .Include("ExceptionItems.function1")
                        .Include("ExceptionItems.function2")
                        .Include("ExceptionItems.functionitem1")
                        .Include("ExceptionItems.functionitem2")
                        .Include("ExceptionItems.user")

                        .FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODException GetSODException(string code)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    SODException exception = null;
                    try
                    {
                        exception = _context.SodException
                            .Include("RiskOwner")
                            .Include("ExceptionItems")
                            .Include("ExceptionItems.function1")
                            .Include("ExceptionItems.function2")
                            .Include("ExceptionItems.functionitem1")
                            .Include("ExceptionItems.functionitem2")
                            .Include("ExceptionItems.user")
                            .FirstOrDefault(s => s.ExceptionCode == code);
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    return exception;
                }
            }
            catch (Exception)
            {
                throw;

            }


        }


        public List<SodExceptionItem> GetAllSodExceptionItems()
        {
            return new List<SodExceptionItem>();
        }

        public List<SODException> GetAllSodExceptions()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodException
                        .GroupBy(s => s.ExceptionCode).Select(x => x.FirstOrDefault())
                        .Include("RiskOwner")
                        .Include("ExceptionItems")
                        .Include("ExceptionItems.function1")
                        .Include("ExceptionItems.function2")
                        .Include("ExceptionItems.functionitem1")
                        .Include("ExceptionItems.functionitem2")
                        .Include("ExceptionItems.user")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }


        }
        public Dictionary<long, string> GetAllSodExceptions(string searchString)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {


                    return _context.SodException
                        .Where(item => item.ExceptionCode.Contains(searchString))
                        .ToList()
                        .Select(x => new { Key = x.Id, Value = x.ExceptionCode + " - " + x.ExceptionDesc })
                        .Distinct().OrderBy(s => s.Value).ToDictionary(o => (long)o.Key, o => o.Value);
                }
            }
            catch (Exception)
            {
                throw;

            }
        }

        public void RemoveMatchingException(string ExceptionCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    List<SODException> lstException = _context.SodException.Where(s => s.ExceptionCode == ExceptionCode).ToList();
                    foreach (SODException exception in lstException)
                    {
                        _context.SodException.Remove(exception);
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public List<SODException> GetSODExceptionByExceptionCode(string ExceptionCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodException.Where(s => s.ExceptionCode == ExceptionCode)
                        .Include("RiskOwner")
                        .Include("ExceptionItems")
                        .Include("ExceptionItems.user")
                        .Include("ExceptionItems.function1")
                        .Include("ExceptionItems.function2")
                        .Include("ExceptionItems.functionitem1")
                        .Include("ExceptionItems.functionitem2")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }


        }

        #endregion


        #region Execution


        public bool CheckIfUserExistAtObjectLevel(string UserName, string TCode, string ObjectCode, string FieldCode, string Value)
        {
            bool IsUserExist = false;



            return IsUserExist;
        }

        public string GetObjectCode(long FunctionItem1)
        {
            try
            {
                SodFunctionItem item = GetSodFunctionItem(FunctionItem1);
                return item.SodRoleDesc;
            }
            catch (Exception)
            {
                throw;

            }

        }

        public string GetFieldValue(string Value, string AuthCode, string ObjectCode, string FieldCode)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    string MatchString = string.Empty;
                    SAPRoleToAuthMap roletoauthmap = _context.SapRoleToAuthMap.FirstOrDefault(s => s.AuthCode == AuthCode && s.ObjectCode == ObjectCode && s.FieldCode == FieldCode);

                    if (roletoauthmap != null)
                    {
                        SAPFieldValueMapping fieldvalue = _context.SAPFieldValueMap.FirstOrDefault(s => s.FieldCode == FieldCode && s.RoleCode == roletoauthmap.RoleName);
                        MatchString = fieldvalue.FromValue;
                    }

                    return MatchString;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public bool IsObjectValueEqual(string AuthCode, string ObjectCode, string FieldCode, string Value1, string Value2)
        {
            try
            {
                Value1 = Value1.TrimStart('0');
                Value2 = Value2.TrimStart('0');

                if (Value1[0] == '$')
                {
                    Value1 = GetFieldValue(Value1, AuthCode, ObjectCode, FieldCode);
                }

                if (Value2[0] == '$')
                {
                    Value2 = GetFieldValue(Value2, AuthCode, ObjectCode, FieldCode);
                }

                bool ReturnVal = true;

                if (Value1 != "*" && Value2 != "*")
                {
                    if (Value1 != Value2) ReturnVal = false;
                }


                return ReturnVal;
            }
            catch (Exception)
            {
                throw;

            }

        }
        public SAPObjectInfo GetSAPObject(int ObjectId)
        {
            try
            {
                SAPConnectorDA sapDa = new SAPConnectorDA();
                return sapDa.GetSAPObject(ObjectId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public List<SODPolicyReport> CheckConflictAtObjectLevel(List<SODPolicyReport> lstReport)
        {
            try
            {
                SAPConnectorDA sapDa = new SAPConnectorDA();
                List<SODAuthorizationReport> lstLeftAuthorizations = new List<SODAuthorizationReport>();
                List<SODAuthorizationReport> lstRightAuthorizations = new List<SODAuthorizationReport>();

                foreach (SODPolicyReport report in lstReport)
                {
                    if (report.FunctionType1 == "TCodes" && report.FunctionType2 == "TCodes" && report.UserName != string.Empty)
                    {
                        SodFunctionItem FunctionItem1 = GetSodFunctionItem(report.FunctionItem1.Value);
                        SodFunctionItem FunctionItem2 = GetSodFunctionItem(report.FunctionItem2.Value);

                        string ObjectCode1 = FunctionItem1.ObjectCode;
                        string FieldCode1 = FunctionItem1.FieldCode;
                        string FieldValue1 = FunctionItem1.FromValue;

                        string ObjectCode2 = FunctionItem2.ObjectCode;
                        string FieldCode2 = FunctionItem2.FieldCode;
                        string FieldValue2 = FunctionItem2.FromValue;

                        if (ObjectCode1 != string.Empty && ObjectCode2 != string.Empty)
                        {

                            List<string> SapAuthObjects1 = sapDa.GetListOfAuthorizations(FunctionItem1.SodRoleDesc, FunctionItem1.Endpointsystemid.Value);
                            List<string> SapAuthObjects2 = sapDa.GetListOfAuthorizations(FunctionItem2.SodRoleDesc, FunctionItem2.Endpointsystemid.Value);
                            if (SapAuthObjects1 == null || SapAuthObjects2 == null)
                            {
                                foreach (SODPolicyReport r in lstReport)
                                {
                                    if (GetObjectCode(r.FunctionItem1.Value) == ObjectCode1 || GetObjectCode(r.FunctionItem2.Value) == ObjectCode2)
                                    {
                                        lstReport.Remove(r);
                                    }
                                }
                            }
                            else
                            {
                                bool FieldFound1 = false, FieldFound2 = false;

                                #region Left Matching

                                foreach (string AuthCode in SapAuthObjects1)
                                {

                                    List<SAPAuthorizationToObject> lstAuthorizeObj = sapDa.GetListOfAuthorizationObjects((int)report.FunctionObjItem1.Endpointsystemid.Value, AuthCode); //, ObjectCode2, FieldCode2);
                                    List<SAPAuthorizationToObject> MatchingObject = new List<SAPAuthorizationToObject>();
                                    bool ItemFieldMatch = false;

                                    if (lstAuthorizeObj != null)
                                    {

                                        foreach (SAPAuthorizationToObject AuthorizeObj in lstAuthorizeObj)
                                        {
                                            AuthorizeObj.FromValue = AuthorizeObj.FromValue.TrimStart('0');
                                            List<SodFunctionItem> items = report.Function1.SodFunctionItems.Where(s => s.ObjectCode == AuthorizeObj.ObjectCode && s.FieldCode == AuthorizeObj.FieldCode).ToList();
                                            foreach (SodFunctionItem item in items)
                                            {
                                                //if (item.FromValue == AuthorizeObj.FromValue)
                                                if (IsObjectValueEqual(AuthorizeObj.AuthorizationCode, AuthorizeObj.ObjectCode, AuthorizeObj.FieldCode, item.FromValue, AuthorizeObj.FromValue))
                                                {
                                                    MatchingObject.Add(AuthorizeObj);
                                                    if (report.FunctionObjItem1.ObjectCode == AuthorizeObj.ObjectCode &&
                                                        report.FunctionObjItem1.FieldCode == AuthorizeObj.FieldCode &&
                                                        report.FunctionObjItem1.FromValue == AuthorizeObj.FromValue)
                                                        ItemFieldMatch = true;
                                                }

                                            }



                                        }

                                        FieldFound1 = true;

                                        foreach (SodFunctionItem item in report.Function1.SodFunctionItems)
                                        {
                                            SAPAuthorizationToObject obj = MatchingObject.FirstOrDefault(s => s.FieldCode == item.FieldCode && s.ObjectCode == item.ObjectCode);
                                            if (obj == null)
                                                FieldFound1 = false;
                                        }

                                        if (FieldFound1)
                                        {
                                            if (ItemFieldMatch)
                                            {
                                                report.AuthorizationCode1 = AuthCode;
                                                SODAuthorizationReport AuthReport = new SODAuthorizationReport();
                                                AuthReport.SODReportId = report.Id;
                                                AuthReport.LeftOrRight = 1;
                                                AuthReport.AuthorizationCode = AuthCode;
                                                lstLeftAuthorizations.Add(AuthReport);
                                            }
                                            report.PassedObjectLevelLeft = true;
                                        }
                                    }



                                }
                                #endregion

                                #region Right Matching
                                foreach (string AuthCode in SapAuthObjects2)
                                {
                                    List<SAPAuthorizationToObject> lstAuthorizeObj = sapDa.GetListOfAuthorizationObjects((int)report.FunctionObjItem2.Endpointsystemid.Value, AuthCode); //, ObjectCode2, FieldCode2);
                                    List<SAPAuthorizationToObject> MatchingObject = new List<SAPAuthorizationToObject>();
                                    bool ItemFieldMatch = false;

                                    if (lstAuthorizeObj != null)
                                    {

                                        foreach (SAPAuthorizationToObject AuthorizeObj in lstAuthorizeObj)
                                        {
                                            AuthorizeObj.FromValue = AuthorizeObj.FromValue.TrimStart('0');
                                            List<SodFunctionItem> items = report.Function2.SodFunctionItems.Where(s => s.ObjectCode == AuthorizeObj.ObjectCode && s.FieldCode == AuthorizeObj.FieldCode).ToList();
                                            foreach (SodFunctionItem item in items)
                                            {
                                                //if (item.FromValue == AuthorizeObj.FromValue)
                                                if (IsObjectValueEqual(AuthorizeObj.AuthorizationCode, AuthorizeObj.ObjectCode, AuthorizeObj.FieldCode, item.FromValue, AuthorizeObj.FromValue))
                                                {
                                                    MatchingObject.Add(AuthorizeObj);
                                                    if (report.FunctionObjItem2.ObjectCode == AuthorizeObj.ObjectCode &&
                                                        report.FunctionObjItem2.FieldCode == AuthorizeObj.FieldCode &&
                                                        report.FunctionObjItem2.FromValue == AuthorizeObj.FromValue)
                                                        ItemFieldMatch = true;
                                                }

                                            }



                                        }

                                        FieldFound2 = true;

                                        foreach (SodFunctionItem item in report.Function2.SodFunctionItems)
                                        {
                                            SAPAuthorizationToObject obj = MatchingObject.FirstOrDefault(s => s.FieldCode == item.FieldCode && s.ObjectCode == item.ObjectCode);
                                            if (obj == null)
                                                FieldFound2 = false;
                                        }

                                        if (FieldFound2)
                                        {
                                            if (ItemFieldMatch)
                                            {
                                                report.AuthorizationCode2 = AuthCode;
                                                SODAuthorizationReport AuthReport = new SODAuthorizationReport();
                                                AuthReport.SODReportId = report.Id;
                                                AuthReport.LeftOrRight = 2;
                                                AuthReport.AuthorizationCode = AuthCode;
                                                lstRightAuthorizations.Add(AuthReport);


                                            }
                                            report.PassedObjectLevelRight = true;
                                        }
                                    }



                                }

                                #endregion

                                #region Removing Similiar Non Conflict Items

                                if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                                {
                                    lstReport.Where(s => s.FunctionId1 == report.FunctionId1 &&
                                                                                            s.FunctionItem1 == report.FunctionItem1 &&
                                                                                            s.FunctionId2 == report.FunctionId2 &&
                                                                                            s.FunctionItem2 == report.FunctionItem2 &&
                                                                                            s.UserName != string.Empty).ToList()
                                                                                            .ForEach(s => s.UserName = string.Empty);
                                }
                                #endregion

                            }
                        }
                    }
                }




                foreach (SODPolicyReport report in lstReport)
                {
                    if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                    {
                        if (report.PassedObjectLevelLeft == false)
                        {
                            List<SODPolicyReport> lstMatchingFieldCode = lstReport.Where(s => s.Role2 == report.Role1 && s.Role2 == report.Role2 &&
                                                                                s.FunctionObjItem1.ObjectCode == report.FunctionObjItem1.ObjectCode &&
                                                                                s.FunctionObjItem1.FieldCode == report.FunctionObjItem1.FieldCode).ToList();

                            if (lstMatchingFieldCode != null && lstMatchingFieldCode.Count > 1)
                            {
                                foreach (SODPolicyReport SodMatchReport in lstMatchingFieldCode)
                                {
                                    if (SodMatchReport.PassedObjectLevelLeft)
                                    {
                                        report.PassedObjectLevelLeft = true;
                                        break;
                                    }
                                }
                            }

                        }

                        if (report.PassedObjectLevelRight == false)
                        {
                            List<SODPolicyReport> lstMatchingFieldCode = lstReport.Where(s => s.Role1 == report.Role1 && s.Role2 == report.Role2 &&
                                                                                s.FunctionObjItem2.ObjectCode == report.FunctionObjItem2.ObjectCode &&
                                                                                s.FunctionObjItem2.FieldCode == report.FunctionObjItem2.FieldCode).ToList();

                            if (lstMatchingFieldCode != null && lstMatchingFieldCode.Count > 1)
                            {
                                foreach (SODPolicyReport SodMatchReport in lstMatchingFieldCode)
                                {
                                    if (SodMatchReport.PassedObjectLevelRight)
                                    {
                                        report.PassedObjectLevelRight = true;
                                        break;
                                    }
                                }
                            }

                        }

                        if (report.PassedObjectLevelLeft == false || report.PassedObjectLevelRight == false)
                        {
                            List<SODPolicyReport> lstMatchingReports = lstReport.Where(s => s.FunctionId1 == report.FunctionId1 && s.FunctionId2 == report.FunctionId2).ToList();
                            foreach (SODPolicyReport MatchReport in lstMatchingReports)
                            {

                                MatchReport.UserName = string.Empty;

                            }
                        }


                    }
                }



                lstReport = lstReport.Where(s => s.UserName != string.Empty && s.AuthorizationCode1 != string.Empty && s.AuthorizationCode2 != string.Empty).ToList();

                List<SODPolicyReport> lstNewReport = new List<SODPolicyReport>();

                foreach (SODPolicyReport report1 in lstReport)
                {
                    List<SODAuthorizationReport> lstMatchAuthLeft = lstLeftAuthorizations.Where(s => s.SODReportId == report1.Id).ToList();
                    List<SODAuthorizationReport> lstMatchAuthRight = lstRightAuthorizations.Where(s => s.SODReportId == report1.Id).ToList();

                    foreach (SODAuthorizationReport leftAuthReport in lstMatchAuthLeft)
                    {
                        foreach (SODAuthorizationReport rightAuthReport in lstMatchAuthRight)
                        {
                            SODPolicyReport reportnew = new SODPolicyReport(report1);
                            reportnew.AuthorizationCode1 = leftAuthReport.AuthorizationCode;
                            reportnew.AuthorizationCode2 = rightAuthReport.AuthorizationCode;

                            lstNewReport.Add(reportnew);
                        }
                    }




                }

                foreach (SODPolicyReport report in lstNewReport)
                {
                    if (report.ProfileCode1 == null || report.ProfileCode1 == string.Empty)
                    {

                        SAPAuthorizationProfile AuthProf1 = sapDa.GetProfileByAuthCode(report.AuthorizationCode1);
                        SAPRoleToAuthMap AuthRole1 = sapDa.GetRoleByAuthCode(report.AuthorizationCode1);

                        if (AuthProf1 != null)
                        {
                            report.ProfileCode1 = AuthProf1.ProfileCode;

                            if (AuthRole1 != null)
                                report.RoleCode1 = AuthRole1.RoleName;

                            foreach (SODPolicyReport r1 in lstNewReport)
                            {
                                if (r1.AuthorizationCode1 == report.AuthorizationCode1)
                                {
                                    r1.ProfileCode1 = report.ProfileCode1;
                                    r1.RoleCode1 = report.RoleCode1;
                                }

                                if (r1.AuthorizationCode2 == report.AuthorizationCode1)
                                {
                                    r1.ProfileCode2 = report.ProfileCode1;
                                    r1.RoleCode2 = report.RoleCode1;
                                }
                            }
                        }
                    }

                    if (report.ProfileCode2 == null || report.ProfileCode2 == string.Empty)
                    {


                        SAPAuthorizationProfile AuthProf2 = sapDa.GetProfileByAuthCode(report.AuthorizationCode2);
                        SAPRoleToAuthMap AuthRole2 = sapDa.GetRoleByAuthCode(report.AuthorizationCode2);


                        if (AuthProf2 != null)
                        {
                            report.ProfileCode2 = AuthProf2.ProfileCode;
                            if (AuthRole2 != null)
                                report.RoleCode2 = AuthRole2.RoleName;


                            foreach (SODPolicyReport r1 in lstNewReport)
                            {
                                if (r1.AuthorizationCode1 == report.AuthorizationCode2)
                                {
                                    r1.ProfileCode1 = report.ProfileCode2;
                                    r1.RoleCode1 = report.RoleCode2;
                                }

                                if (r1.AuthorizationCode2 == report.AuthorizationCode2)
                                {
                                    r1.ProfileCode2 = report.ProfileCode2;
                                    r1.RoleCode2 = report.RoleCode2;
                                }
                            }
                        }
                    }

                }
                return lstNewReport;
            }
            catch (Exception)
            {
                throw;

            }


        }


        #endregion








        #region SOD Configuration
        public List<SODExecutionConfig> GetAllSodExecutionConfig()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.SodExecutionConfig
                        .Include("EndpointSystemType")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public List<SODRunPolicy> GetAllRunHistory()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRunPolicy
                        .Include("lstReport")
                        .Include("WorkflowConfiguration")
                        .Include("RiskLevel")
                        .Include("compliancepolicy")
                        .Include("EndpointSelection")
                        .Include("Exceptions")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        #region Execution Config

        public SODExecutionConfig GetExecutionConfig(int EndpointSystemTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodExecutionConfig
                        .Include("EndpointSystemType")
                        .FirstOrDefault(s => s.EndpointSystemTypeId == EndpointSystemTypeId);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void AddExecutionConfig(SODExecutionConfig sConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    _context.SodExecutionConfig.Add(sConfig);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public void UpdateExecutionConfig(SODExecutionConfig sConfig)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    _context.SodExecutionConfig.Attach(sConfig);
                    _context.Entry(sConfig).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        #endregion



        #endregion

        #region RunCode

        public long SaveRunPolicy(SODRunPolicy RunPolicy)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    //RunPolicy.WorkflowConfigurationId = 1;
                    RunPolicy.compliancepolicy = null;
                    RunPolicy.RiskLevel = null;


                    _context.SodRunPolicy.Add(RunPolicy);
                    _context.SaveChanges();
                    return RunPolicy.Id;
                }
            }
            catch (Exception)
            {
                throw;

            }

        }


        public List<SODRunPolicy> GetUniqueRunCodes()
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRunPolicy
                        .Include("lstReport")
                        .Include("WorkflowConfiguration")
                        .Include("RiskLevel")
                        .Include("compliancepolicy")
                        .Include("EndpointSelection")
                        .Include("Exceptions")
                        .ToList();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODRunPolicy GetSodRunPolicy(int Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodRunPolicy
                        .Include("lstReport")
                        .Include("WorkflowConfiguration")
                        .Include("RiskLevel")
                        .Include("compliancepolicy")
                        .Include("EndpointSelection")
                        .Include("Exceptions")
                        .FirstOrDefault(s => s.Id == Id);
                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SODRunPolicy GetSODRunPolicyById(int _runPolicyId, bool deepLoad)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.Configuration.ProxyCreationEnabled = false;

                    if (!deepLoad)
                    {
                        return _context.SodRunPolicy.FirstOrDefault(item => item.Id == _runPolicyId);
                    }
                    return (from item in _context.SodRunPolicy
                        .Include("lstReport")
                        .Include("compliancepolicy")
                        .Include("lstReport.Conflict")
                        .Include("lstReport.Conflict.SodRiskId")
                        .Include("lstReport.Conflict.SodRiskId.Mitigations")
                        .Include("lstReport.Function1")
                        .Include("lstReport.Function2")
                        .Include("lstReport.FunctionObjItem1")
                        .Include("lstReport.FunctionObjItem2")

                        where item.Id == _runPolicyId
                        select item).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }
        #endregion

        #region//Autocomplete box for Riskscore written by Karthik
        //public Dictionary<int, string> GetAllRiskScore(string searchString)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return _context.SodRiskScore
        //                .Where(item => item.RiskScore.ToString().Contains(searchString))
        //                .ToList()
        //                .Select(x => new { Key = x.Id, Value = x.RiskScore + " - " + x.RiskScoreDescription })
        //                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                    
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}
        /// <summary>
        /// Gettign SODFunctionItems written by Karthik
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public Dictionary<long, string> GetAllSodFunctionItems(string[] inputParams,string searchString)
        {
            Dictionary<long, string> values = new Dictionary<long, string>();
            try
            {
                string SODFunctionCode ;
                string SODFunctionDesc;
                int? FunctionId = 0;
                if (inputParams.Length > 1)
                {
                    SODFunctionCode = inputParams[0].Trim();
                    SODFunctionDesc = inputParams[1].Trim();

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        values = _context.SodFunctionItem

                         .Where(item => item.SodFunctionId == FunctionId)
                         .ToList()
                         .Select(x => new { Key = x.Id, Value = x.SodRoleDesc })
                         .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);

                    }
                }
                return values;
                //using (IACMDBContext _context = new IACMDBContext())
                //{   return _context.SodFunctionItem

                //        .Where(item => item.SodFunctionId==FunctionId)
                //        .ToList()
                //        .Select(x => new { Key = x.SodFunctionId.Value, Value = x.endpoint.DisplayName + " - " + x.EndpointType.DisplayName })
                //        .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);
                //}
                
            }
            catch (Exception)
            {
                throw;

            }

        }

        public SodFunctionItem GetSelectedSodFunctionItem(int Id)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    return _context.SodFunctionItem
                        .Include("EndpointType")
                        .Include("endpoint")
                        .FirstOrDefault(s => s.Id == Id);
                    

                }
            }
            catch (Exception)
            {
                throw;

            }

        }

        //public Dictionary<int, string> GetAllRiskCodes(string searchString)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {

        //            return _context.SodRiskId
        //                .Where(item => item.RiskCode.ToString().Contains(searchString))
        //                .ToList()
        //                .Select(x => new { Key = x.Id, Value = x.RiskCode + " - " + x.RiskDescription })
        //                .Distinct().OrderBy(s => s.Value).ToDictionary(o => o.Key, o => o.Value);

        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;

        //    }

        //}



        public void DeleteSODException(int ExceptionID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODException cException = _context.SodException.Find(ExceptionID);
                    SodExceptionItem cExceptionItem = _context.SodExceptionItem.Find(cException.Id);
                    if (cExceptionItem != null)
                    {
                        _context.SodExceptionItem.Remove(cExceptionItem);
                        _context.SaveChanges();
                    }
                    if (cException != null)
                    {
                        _context.SodException.Remove(cException);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }


            }

        }

        public void DeleteMitigation(int MitigationID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    SODMitigation mitigate = _context.SodMitigation.Find(MitigationID);
                    if (mitigate != null)
                    {
                        _context.SodMitigation.Remove(mitigate);
                        _context.SaveChanges();
                    }
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }      
        }



        public void DeleteSODExceptions(int ExceptionID)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {

                    //return _context.SodFunctionItem
                    //   .Include("EndpointType")
                    //   .Include("endpoint")
                    //   .FirstOrDefault(s => s.Id == Id);
                    SODException cException = _context.SodException.Include("ExceptionItems").FirstOrDefault(x=>x.Id==ExceptionID);
                   // List<SodExceptionItem> cExceptionItem = _context.SodException.Find(ExceptionID).ExceptionItems;

                    if (cException != null)
                    {
                        _context.SodException.Remove(cException);
                        _context.SaveChanges();
                    }

                    //if (cException.ExceptionItems!= null)
                    //{
                    //    foreach (SodExceptionItem expItem in cException.ExceptionItems)
                    //    {
                    //        _context.SodExceptionItem.Remove(expItem);
                    //        //_context.SaveChanges();
                    //    }
                    //}
                    //if (cException != null)
                    //{
                    //    _context.SodException.Remove(cException);
                    //    _context.SaveChanges();
                    //}
                }
            }
            catch (DbUpdateException e)
            {
                var sqlException = e.GetBaseException() as SqlException;
                if (sqlException != null)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                throw new Exception("FK Exception");
                            default:
                                throw e;
                        }
                    }
                }
                else
                {
                    throw e;
                }
            }
        }

        #endregion



        #region UpdateExceptionItem
        //public void SaveExceptionItem(SODException exception)
        //{
        //    using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //    {
        //        var dbSODException = oIACMDBContext.SodException
        //                               .Include("ExceptionItems")
        //                               .Single(c => c.ExceptionId == exception.ExceptionId);

        //        oIACMDBContext.Entry(dbSODException).CurrentValues.SetValues(exception);

        //        var v = dbSODException.ExceptionItems.ToList();
        //        foreach (var item in v)
        //        {
        //            dbSODException.ExceptionItems.Remove(item);
        //        }

        //        if (exception.ExceptionItems != null)
        //        {
        //            foreach (var exp in exception.ExceptionItems)
        //            {
        //                var item = dbSODException.ExceptionItems.SingleOrDefault(s => s.ExceptionItemId == exp.ExceptionItemId);
        //                if (item != null)
        //                    oIACMDBContext.Entry(item).CurrentValues.SetValues(exp);
        //                else
        //                {
        //                    var dbSodException = oIACMDBContext.SodExceptionItem.Where(t => t.ExceptionItemId == exp.ExceptionItemId).FirstOrDefault();

        //                    if (dbSodException != null)
        //                        dbSODException.ExceptionItems.Add(dbSodException);
        //                    else
        //                        dbSODException.ExceptionItems.Add(exp);
                                
        //                }
        //            }
        //        }
        //        oIACMDBContext.SaveChanges();
               
        //    }
        //}

        public void EditSodMitigation(SODMitigation mitigation)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    var dbMitigation = _context.SodMitigation.Include("RiskCodes").Single(c => c.Id == mitigation.Id); 
                    _context.Entry(dbMitigation).CurrentValues.SetValues(mitigation);

                    var v = dbMitigation.RiskCodes.ToList();
                    foreach (var item in v)
                    {
                        dbMitigation.RiskCodes.Remove(item);
                    }

                    if (mitigation.RiskCodes != null)
                    {
                        foreach (var mitig in mitigation.RiskCodes)
                        {
                            var item = dbMitigation.RiskCodes.SingleOrDefault(s => s.Id == mitig.Id);
                            if (item != null)
                                _context.Entry(item).CurrentValues.SetValues(item);

                            else
                            {
                                var dbSodMitig = _context.SodRiskId.Where(t => t.Id == mitig.Id).FirstOrDefault();
                                if (dbSodMitig != null)
                                    dbMitigation.RiskCodes.Add(dbSodMitig);
                                else
                                    dbMitigation.RiskCodes.Add(mitig);
                            }
                           
                        }
                    }

                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;

            }

        }


        
        #endregion


        #endregion


    }
}
