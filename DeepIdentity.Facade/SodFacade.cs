using DeepIdentity.DataAccess;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class SodFacade
    {
        private SodDA sodDA {get;set;}

        #region Constructor

        public SodFacade()
        {
            sodDA = new SodDA();
        }
        #endregion

        #region SodFacade Members


        #region report

        public void AddComplianceReport(List<SODReport> ComplianceReport)
        {
            try
            {
                sodDA.AddComplianceReport(ComplianceReport);
            }
            catch (Exception)
            {
                throw;
            }


        }

        public List<SODReport> GetComplianceReport(int ReportId)
        {
            try
            {
                return sodDA.GetComplianceReport(ReportId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveReport(List<SODReport> lstReport, int RunId)
        {

            try
            {
                sodDA.SaveReport(lstReport, RunId);
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

 
        #region Risk Level

        public void DeleteRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                sodDA.DeleteRiskLevel(RiskLevel);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                sodDA.UpdateRiskLevel(RiskLevel);
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
                return sodDA.GetRiskLevel(Id);
            }
            catch (Exception)
            {
                throw;
            }

            
        }

        public SODRiskLevel GetRiskLevel(string Id)
        {
            try
            {
                return sodDA.GetRiskLevel(Id);
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public void AddRiskLevel(SODRiskLevel RiskLevel)
        {
            try
            {
                sodDA.AddRiskLevel(RiskLevel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SODRiskLevel> GetAllRiskLevels()
        {
            try
            {
                return sodDA.GetAllRiskLevels();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<int, string> GetAllRiskLevels(string searchString)
        {
            try
            {
                return sodDA.GetAllRiskLevels(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsRiskLevelAlreadyExists(string _RiskLevel)
        {
            var RiskLvl =sodDA.GetAllRiskLevels().Where(t => t.RiskLevelCode == _RiskLevel).FirstOrDefault();
            if (RiskLvl == null)
                return false;
            return true;
        }


        #endregion

        #region Risk Score

        public void DeleteRiskScore(int RiskScoreId)
        {
            try
            {
                sodDA.DeleteRiskScore(RiskScoreId);
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
                sodDA.UpdateRiskScore(RiskScore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SODRiskScore GetRiskScore(int Id)
        {
            try
            {
                return sodDA.GetRiskScore(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<SODRiskScore> GetAllRiskScores()
        {
            try
            {
                return sodDA.GetAllRiskScores();
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
                sodDA.AddRiskScore(RiskScore);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsRiskScoreAlreadyExists(int _RiskScore)
        {
            var Riskscr = sodDA.GetAllRiskScores().Where(t => t.RiskScore == _RiskScore).FirstOrDefault();
            if (Riskscr == null)
                return false;
            return true;
        }

        #endregion,

        #region Risk Code

        public bool DeleteRiskCode(int RiskId)
        {
            try
            {
                sodDA.DeleteRiskCode(RiskId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UpdateRiskCode(SODRiskCode riskcode)
        {
            try
            {
                sodDA.UpdateRiskCode(riskcode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SODRiskCode GetRiskCode(int Id)
        {
            try
            {
                return sodDA.GetRiskCode(Id);
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
                return sodDA.GetRiskCode(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddRiskCode(SODRiskCode RiskId)
        {
            try
            {
                sodDA.AddRiskCode(RiskId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SODRiskCode> GetAllRiskCodes()
        {
            try
            {
                return sodDA.GetAllRiskCodes();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public bool IsRiskCodeAlreadyExists(string _RiskCode)
        {
            var RiskCde = sodDA.GetAllRiskCodes().Where(t => t.RiskCode == _RiskCode).FirstOrDefault();
            if (RiskCde == null)
                return false;
            return true;
        }

        #endregion

        #region Function

        public void SaveFunction(SodFunction sodfunction)
        {
            try
            {
                sodDA.SaveFunction(sodfunction);
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
                return sodDA.GetSodFunction(Id, deepload);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public SodFunction GetSodFunction(string FunctionCode, bool deepload)
        {
            try
            {
                return sodDA.GetSodFunction(FunctionCode,deepload);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SodFunctionItem GetSodFunctionItem(int Id)
        {
            try
            {
                return sodDA.GetSodFunctionItem(Id);
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
        //        sodDA.UpdateSodFunction(function);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public void DeleteSodFunction(SodFunction sodfunction)
        {
            try
            {
                sodDA.DeleteSodFunction(sodfunction);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SodFunction> GetAllSodFunctions()
        {
            try
            {
                return sodDA.GetAllSodFunctions();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<int, string> GetAllSodFunctions(string searchString)
        {
            try
            {
                return sodDA.GetAllSodFunctions(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<int, string> GetAllCompliancePolicy(string searchString)
        {
            try
            {
                return sodDA.GetAllCompliancePolicy(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Dictionary<int, string> GetAllSodConflictFunctions(string searchString)
        {
            try
            {
                return sodDA.GetAllSodConflictFunctions(searchString);
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
                return sodDA.GetMatchingFunctionItems(FunctionId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveMatchingFunctions(int FunctionNo)
        {
            try
            {
                sodDA.RemoveMatchingFunctions(FunctionNo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Conflict

        public bool CheckForDuplicateConflict(SODConflict conflict)
        {
            try
            {
                return sodDA.CheckForDuplicateConflict(conflict);
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
                return sodDA.GetAllSodConflicts();
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
                sodDA.DeleteSodConflict(sodconflict);
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
                return sodDA.GetSodConflict(id);
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
                sodDA.AddSodConflict(sodconflict);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SODConflictFunction GetConflictFunction(int RowNo)
        {
            try
            {
                return sodDA.GetConflictFunction(RowNo);
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
        //        sodDA.UpdateConflictFunction(conflictfunction);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public void UpdateLastGenerateddate(SODConflictFunction conflictfunction)
        {
            try
            {
                sodDA.UpdateLastGenerateddate(conflictfunction);
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
                return sodDA.GetAllSODConflictFunction();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSODConflictFunction(int SodConflictFunctionId)
        {
            try
            {
                sodDA.DeleteSODConflictFunction(SodConflictFunctionId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Mitigation

        public void DeleteSodMitigation(SODMitigation mitigation)
        {
            try
            {
                sodDA.DeleteSodMitigation(mitigation);
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
                sodDA.UpdateSodMitigation(mitigation);
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
                return sodDA.GetSodMitigation(id);
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
                return sodDA.GetSodMitigation(code);
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
                sodDA.AddSodMitigation(mitigation);
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
                return sodDA.GetAllSodMitigations();
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
                sodDA.DeleteCompliancePolicy(policyId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateCompliancePolicy(CompliancePolicy cPolicy)
        {
            try
            {
                sodDA.UpdateCompliancePolicy(cPolicy);
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
                return sodDA.GetSodConflictList(ConflictCode);
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
                return sodDA.GetSodFunctionFromConflictFunction(ConflictCode);
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
                return sodDA.GetCompliancePolicy(id);
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
                return sodDA.GetCompliancePolicy(code);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AddCompliancePolicy(CompliancePolicy CompliancePolicy)
        {
            try
            {
                sodDA.AddCompliancePolicy(CompliancePolicy);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public void AddConflictFunction(SODConflictFunction ConflictFunction)
        //{
        //    try
        //    {
        //        sodDA.AddConflictFunction(ConflictFunction);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public void SaveConflictFunction(SODConflictFunction conflictFunction)
        {
            try
            {
                sodDA.SaveConflictFunction(conflictFunction);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SaveCompliancePolicy(CompliancePolicy policy)
        {
            try
            {
                sodDA.SaveCompliancePolicy(policy);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsFunctionConflictCodeExists(string ConflictFunctionCode)
        {
            try
            {
                return sodDA.IsFunctionConflictCodeExists(ConflictFunctionCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsFunctionCodeExists(string FunctionCode)
        {
            try
            {
                return sodDA.IsFunctionCodeExists(FunctionCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool IsCompliancePolicyCodeExists(string CompliancePolicyCode)
        {
            try
            {
                return sodDA.IsCompliancePolicyCodeExists(CompliancePolicyCode);
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
                return sodDA.GetAllCompliancePolicy();
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
                return sodDA.GetAllComplianceTypes();
            }
            catch (Exception)
            {
                throw;
            }

        }


        #endregion


        #region Exception


        public void AddException(SODException exception)
        {
            try
            {
                sodDA.AddException(exception);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteException(SODException exception)
        {
            try
            {
                sodDA.DeleteException(exception);
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
                sodDA.UpdateException(Exception);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SODException GetSODException(int id)
        {
            try
            {
                return sodDA.GetSODException(id);
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
                return sodDA.GetSODException(code);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SODException> GetAllSodExceptions()
        {
            try
            {
                return sodDA.GetAllSodExceptions();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<SodExceptionItem> GetAllSodExceptionItems()
        {
            try
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public Dictionary<int, string> GetAllSodExceptions(string searchString)
        {
            try
            {
                return sodDA.GetAllSodExceptions(searchString);
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
                sodDA.RemoveMatchingException(ExceptionCode);
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
                return sodDA.GetSODExceptionByExceptionCode(ExceptionCode);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion


        #region Conflict Checking


        public string GetObjectCode(int FunctionItem1)
        {
            try
            {
                return sodDA.GetObjectCode(FunctionItem1);
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
                return sodDA.GetFieldValue(Value, AuthCode, ObjectCode, FieldCode);
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
                return sodDA.IsObjectValueEqual(AuthCode, ObjectCode, FieldCode, Value1, Value2);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RemoveMatchingConflicts(string ConflictCode)
        {
            try
            {
                sodDA.RemoveMatchingConflicts(ConflictCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SODReport> CheckConflictAtObjectLevel(List<SODReport> lstReport)
        {
            try
            {
                return sodDA.CheckConflictAtObjectLevel(lstReport);
            }
            catch (Exception)
            {
                throw;
            }

        }


        #endregion

        #region Runcode

        public int SaveRunPolicy(SODRunPolicy RunPolicy)
        {
            try
            {
                return sodDA.SaveRunPolicy(RunPolicy);
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
                return sodDA.GetSODRunPolicyById(_runPolicyId, deepLoad);
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
                return sodDA.GetAllRunHistory();
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
                return sodDA.GetUniqueRunCodes();
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
                return sodDA.GetSodRunPolicy(Id);
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
                return sodDA.GetAllSodExecutionConfig();
            }
            catch (Exception)
            {
                throw;
            }
        }

 

        public SODExecutionConfig GetExecutionConfig(int EndpointSystemTypeId)
        {
            try
            {
                return sodDA.GetExecutionConfig(EndpointSystemTypeId);
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
                sodDA.AddExecutionConfig(sConfig);
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
                sodDA.UpdateExecutionConfig(sConfig);
            }
            catch (Exception)
            {
                throw;
            }
        }

 
  
        #endregion


        #region Autocomplete serach box done for Risk Score  and Risk owner -Written by Karthik
       
        public Dictionary<int, string> GetAllRiskScores(string searchString)
        {
            try
            {
                return sodDA.GetAllRiskScore(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Autocomplete search box for Obtaining All Sod Function Items-Written by Karthik
        /// </summary>
        /// <param name="inputParams"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetAllSodFunctionItems(string[]  inputParams,string searchString)
        {
            try
            {
                return sodDA.GetAllSodFunctionItems(inputParams, searchString);
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
                return sodDA.GetSelectedSodFunctionItem(Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteSODException(int ExceptionID)
        {
            try
            {
                sodDA.DeleteSODException(ExceptionID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SodExceptionItem GetSODExceptionItem(int ExceptionItemID)
        {
            try
            {
               return sodDA.GetSODExceptionItem(ExceptionItemID);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public Dictionary<int, string> GetAllRiskCodes(string searchString)
        {
            try
            {
                return sodDA.GetAllRiskCode(searchString);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Get Mitgation items through MitigationID
        public SODMitigation GetAllSodMitigations(int MitigatoinId)
        {
            try
            {
                return sodDA.GetAllSodMitigations(MitigatoinId);
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region 
        /// <summary>
        /// Delete Mitgation Using Mitigaiton ID
        /// </summary>
        /// <param name="MitigaitonID"></param>
        public void DeleteMitigation(int MitigaitonID)
        {
            sodDA.DeleteMitigation(MitigaitonID);
        }
        #endregion
        #endregion

        #endregion




    }
}
