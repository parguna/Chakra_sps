using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using DI.Chakra.DataAccess;
using System.Data;


namespace DI.Chakra.Facade
{
    public class PasswordManagementFacade
    {
        public PasswordPolicyDA PasswordPolicyDA = new PasswordPolicyDA();
        public GlobalAnswerDA GlobalAnswerDA = new GlobalAnswerDA();
         public GlobalQuestionDA GlobalQuestionDA = new GlobalQuestionDA();
        public PasswordSynchronizationDA PasswordSynchronizationDA = new PasswordSynchronizationDA();

        public PasswordManagementFacade()
        {
            
        }
        //PasswordPolicy model starts
        public IList<PasswordPolicy> FindAllPolicies()
        {
            return PasswordPolicyDA.FindAllPolicies();
        }
        public PasswordPolicy FindAllPoliciesByEndpointSystemId(long EndpointSystemId)
        {
            return PasswordPolicyDA.FindAllPoliciesByEndpointSystemId(EndpointSystemId);
        }
        public PasswordPolicy FindAllPoliciesById(int EndpointSystemTypeId)
        {
            return PasswordPolicyDA.FindAllPoliciesById(EndpointSystemTypeId);
        }
        public PasswordPolicy FindAllPoliciesById(int EndpointSystemTypeId, int EndpointSystemId)
        {
            return PasswordPolicyDA.FindAllPoliciesById(EndpointSystemTypeId, EndpointSystemId);
        }
        public PasswordPolicy GetPolicy(long PolicyId)
        {
            return PasswordPolicyDA.GetPolicy(PolicyId);
        }
        public bool IsPolicyNameAlreadyExists(string PolicyName)
        {
            var policy = PasswordPolicyDA.FindAllPolicies().Where(t => t.Name == PolicyName).FirstOrDefault();
            if (policy == null)
                return false;
            return true;
        }
        public bool ChangePassword(string NewPwd, string username)
        {
            try
            {
                return PasswordPolicyDA.ChangePassword(NewPwd, username);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void SavePasswordPolicyEndPoint(List<PasswordPolicyEndpoint> _PasswordPolicyEndPoint)
        {
            try
            {
                PasswordPolicyDA.SavePasswordPolicyEndPoint(_PasswordPolicyEndPoint);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void SavePolicy(PasswordPolicy _PasswordPolicy)
        {
            try
            {
                PasswordPolicyDA.SavePolicy(_PasswordPolicy);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool DeletePasswordPolicy(int _PolicyId)
        {
            try
            {
                return PasswordPolicyDA.DeletePasswordPolicy(_PolicyId);
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        //public string GetAnswer(int Id)
        //{
        //    try
        //    {
        //        return GlobalAnswerDA.GetAnswer(Id).Answer;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
      
        public IList<EndPointSystemType> GetEndPointSystemType()
        {
            try
            {
                return PasswordPolicyDA.GetAllEndPointType();
            }
            catch (DomainException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        public List<PasswordPolicyEndpoint> GetPasswordPolicyEndpointByPolicyId(long PolicyId)
        {
            try
            {
                return PasswordPolicyDA.GetPasswordPolicyEndpointByPolicyId(PolicyId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<EndPoint> GetAllEndPointSystemsByType(int EndPointSystemTypeId, string HostName)
        {
            try
            {
                return PasswordPolicyDA.GetAllEndPointByType(EndPointSystemTypeId, HostName);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public IList<GlobalQuestion> GetAllQuestions()
        //{
        //    try
        //    {
        //        return GlobalQuestionDA.GetAllQuestions();
        //    }

        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
        //    }
        //}
        public void DeleteQuestion(int _globalQuestionID)
        {

            try
            {
                GlobalQuestionDA.DeleteQuestion(_globalQuestionID);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }
        //public GlobalQuestion GetGlobalQuestionById(int? id)
        //{
        //    try
        //    {
        //        return GlobalQuestionDA.GetGlobalQuestionById(id);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public int SaveQuestion(GlobalQuestion _Question)
        //{
        //    try
        //    {
        //        return GlobalQuestionDA.SaveQuestion(_Question);
        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
        //    }

        //}
        //PasswordSynchronization model starts
        public string GetIACMLastPasswordChangeByEndpointUser(long EndPointSystemId, string UserName)
        {
            return PasswordPolicyDA.GetIACMLastPasswordChangeByEndpointUser(EndPointSystemId, UserName);
        }
       
        //public IList<GlobalAnswer> GetAllQuestionsAnswersByUser(int userId)
        //{
        //    try
        //    {
        //        return GlobalAnswerDA.GetAllQuestionsAnswersByUser(userId);
        //    }

        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
        //    }

        //}
       
        //public void SaveGlobalAnswer(GlobalAnswer newAns)
        //{
        //    try
        //    {
        //        GlobalQuestionDA.SaveGlobalAnswer(newAns);
        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
        //    }
        //}
        //public bool IsGlobalQuestionAlreadyExists(string _Question)
        //{
        //    var GlobalQn = GlobalQuestionDA.GetAllQuestions().Where(t => t.Question == _Question).FirstOrDefault();

        //    if (GlobalQn == null)
        //        return false;

        //    return true;
        //}
       
        
        public EndPoint GetEndPointSystemByKey(int _id)
        {
            try
            {
                return PasswordSynchronizationDA.GetEndPointSystemByKey(_id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public MailTemplate GetMailTemplate(string templatename)
        //{
        //    try
        //    {
        //        return PasswordSynchronizationDA.GetMailTemplate(templatename);
        //    }
        //    catch (DomainException)
        //    {
        //        throw;
        //    }
        //}

        public EndpointType GetEndPointTypeName(int EndpointTypeId)
        {
            return PasswordSynchronizationDA.GetEndPointTypeName(EndpointTypeId);
        }

      
        //public GlobalAnswer GetQuestionAnswerbyId(int globalQuestionId)
        //{
        //    return GlobalAnswerDA.GetQuestionAnswerbyId(globalQuestionId);
        //}

        //public List<GlobalAnswer> GetAllGlobalAnswersByUserID(int intUserID)
        //{
        //    return GlobalAnswerDA.GetAllGlobalAnswersByUserID(intUserID);
        //}

        //public List<GlobalAnswer> GetAllGlobalAnswers()
        //{
        //    return GlobalAnswerDA.GetAllGlobalAnswers();
        //}

        //public void SaveQuestionAnswer(GlobalAnswer _QuestionAns)
        //{
        //    try
        //    {
        //        GlobalAnswerDA.SaveQuestionAnswer(_QuestionAns);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public void DeleteQnandAns(int ID)
        //{
        //    try
        //    {
        //        GlobalAnswerDA.DeleteQnandAns(ID);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public bool IsQuestionAlreadyExists(int UserId, string Qns)
        //{
        //    var Question = GlobalAnswerDA.GetAllQuestionsAnswersByUser(UserId).Where(t => t.Question == Qns).FirstOrDefault(); ;

        //    if (Question == null)
        //        return false;

        //    return true;
        //}
    }
}
