using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using DeepIdentity.Model.SAP;
using DeepIdentity.Infrastructure.Enums;
using System.Data.Entity;

namespace DeepIdentity.DataAccess
{
    public class GlobalAnswerDA
    {
        public IList<GlobalAnswer> GetAllQuestionsAnswersByUser(int UserId)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    return Context.GlobalAnswers.Where(x => x.UserId == UserId).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public GlobalAnswer GetAnswer(int Id)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    return Context.GlobalAnswers.Where(x => x.Id == Id).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public GlobalAnswer GetQuestionAnswerbyId(int globalQuestionId)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    return Context.GlobalAnswers.Where(item => item.Id == globalQuestionId).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<GlobalAnswer> GetAllGlobalAnswers()
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    return Context.GlobalAnswers.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void DeleteQnandAns(int ID)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    var QuesAns = Context.GlobalAnswers.First(qn => qn.Id == ID);
                    if (QuesAns == null)
                        return;
                    Context.GlobalAnswers.Remove(QuesAns);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    

        public void SaveQuestionAnswer(GlobalAnswer _QuestionAns)
        {
            try
            {
                using (IACMDBContext Context = new IACMDBContext())
                {
                    if (_QuestionAns.Id == 0)
                    {
                        Context.GlobalAnswers.Attach(_QuestionAns);
                        Context.Entry(_QuestionAns).State = EntityState.Added;
                        Context.SaveChanges();
                    }
                    else
                    {
                        GlobalAnswer answer = Context.GlobalAnswers.Where(ans => ans.Id == _QuestionAns.Id && ans.Id == _QuestionAns.Id).First();
                        Context.Entry(answer).CurrentValues.SetValues(_QuestionAns);
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Deleting  the Tenant", ex);
                throw ex;
            }
        }

       
    }
}

    






    

