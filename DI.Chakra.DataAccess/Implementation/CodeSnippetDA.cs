using DI.Chakra.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DI.Chakra.DataAccess
{
    public class CodeSnippetDA
    {

        //public void CreateCodeSnippet(RunTimeCodeSnippet codeSnippet)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            _context.RunTimeCodeSnippets.Add(codeSnippet);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public void UpdateCodeSnippet(RunTimeCodeSnippet codeSnippet)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            var accReqEntry = _context.Entry(codeSnippet);
        //            if (accReqEntry.State == EntityState.Detached)
        //            {
        //                var set = _context.Set<RunTimeCodeSnippet>();
        //                RunTimeCodeSnippet attachedAccReq = set.Find(codeSnippet.Id);

        //                if (attachedAccReq != null)
        //                {
        //                    var attachedEntry = _context.Entry(attachedAccReq);
        //                    attachedEntry.CurrentValues.SetValues(codeSnippet);
        //                }
        //            }
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public RunTimeCodeSnippet GetCodeSnippetById(int snippetId)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.RunTimeCodeSnippets.FirstOrDefault(item => item.Id == snippetId);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public RunTimeCodeSnippet GetCodeSnippetByName(string snippetName)
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return _context.RunTimeCodeSnippets.FirstOrDefault(item => item.CodeSnippetName == snippetName);
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        //public List<CodeSnippetData> GetSnippetList()
        //{
        //    try
        //    {
        //        using (IACMDBContext _context = new IACMDBContext())
        //        {
        //            return (from item in _context.RunTimeCodeSnippets
        //                    select new CodeSnippetData
        //                    {
        //                        CodeSnippetId = item.Id,
        //                        CodeSnippetName = item.CodeSnippetName,
        //                    }).ToList();
                    
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

    }
}
