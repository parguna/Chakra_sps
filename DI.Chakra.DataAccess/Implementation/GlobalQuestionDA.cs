using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure;
using DI.Chakra.Model;
using DI.Chakra.Model.SAP;
using DI.Chakra.Infrastructure.Enums;
using System.Data.Entity;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class GlobalQuestionDA
    {
      //  IACMDBContext oIACMDBContext = new IACMDBContext();
        GlobalQuestion _GlobalQuestion = new GlobalQuestion();
        GlobalAnswer GlobalAnswer = new GlobalAnswer();

        //public IList<GlobalQuestion> GetAllQuestions()
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.GlobalQuestions.ToList();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //public GlobalQuestion GetGlobalQuestionById(int? id)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.GlobalQuestions.Where(m => m.Id == id).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public void DeleteQuestion(int _questionID)
        {            
            try
            {
                //using (IACMDBContext oIACMDBContext = new IACMDBContext())
                //{
                //    _GlobalQuestion = oIACMDBContext.GlobalQuestions.FirstOrDefault(qus => qus.GlobalQuestionId == _questionID);
                //    GlobalAnswer = oIACMDBContext.GlobalAnswers.FirstOrDefault(ans => ans.GlobalQuestionId == _questionID);
                //    if (GlobalAnswer != null)//first delete the child then delete the parent
                //    {
                //        oIACMDBContext.GlobalAnswers.Remove(GlobalAnswer);
                //        oIACMDBContext.SaveChanges();
                //    }
                //    if (_GlobalQuestion != null)
                //    {
                //        oIACMDBContext.GlobalQuestions.Remove(_GlobalQuestion);
                //        oIACMDBContext.SaveChanges();
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public int SaveQuestion(GlobalQuestion Question)
        //{        
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            if (Question.Id == 0)
        //            {
        //                Context.GlobalQuestions.Attach(Question);
        //                Context.Entry(Question).State = EntityState.Added;
        //                Context.SaveChanges();                           
        //            }
        //            else
        //            {
        //                GlobalQuestion _question = Context.GlobalQuestions.Where(qns => qns.Id == Question.Id).First();
        //                Context.Entry(_question).CurrentValues.SetValues(Question);
        //                Context.SaveChanges();
        //            }
        //            return Question.Id;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
       
        //public IList<GlobalAnswer> GetAllQuestionsAnswersByUser(int UserId)
        //{
            //try
            //{
            //    using (IACMDBContext Context = new IACMDBContext())
            //    {
            //        return Context.GlobalAnswers.Include(c => c.GlobalQuestion).Where(x => x.UserId == UserId).ToList();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            //return null;
        //}
        //public void SaveGlobalAnswer(GlobalAnswer _Answer)
        //{
        //    try
        //    {
        //        using (IACMDBContext Context = new IACMDBContext())
        //        {
        //            if (_Answer.Id == 0)
        //            {
        //                Context.GlobalAnswers.Attach(_Answer);
        //                Context.Entry(_Answer).State = EntityState.Added;
        //                Context.SaveChanges(); 
        //            }
        //            else
        //            {
        //                GlobalAnswer answer = Context.GlobalAnswers.Where(ans => ans.Id ==_Answer.Id).First();
        //                Context.Entry(answer).CurrentValues.SetValues(_Answer);
        //                Context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Saving the Individual Question", ex);
        //        throw ex;
        //    }
        //}

        public string GetGlobalAnswer(int GlobalQuestionId)
        {
            //try
            //{
            //    using (IACMDBContext Context = new IACMDBContext())
            //    {
            //        return Context.GlobalAnswers.Where(x => x.GlobalQuestionId == GlobalQuestionId).Select(y => y.Answer).FirstOrDefault();
            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            return null;
        }
    }
}
