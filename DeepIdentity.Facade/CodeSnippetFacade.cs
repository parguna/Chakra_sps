using DeepIdentity.DataAccess;
using DeepIdentity.Model;
using System.Collections.Generic;

namespace DeepIdentity.Facade
{
    public class CodeSnippetFacade
    {
        CodeSnippetDA codeSnippetDA = new CodeSnippetDA();

        public void CreateCodeSnippet(RunTimeCodeSnippet codeSnippet)
        {
            try
            {
                codeSnippetDA.CreateCodeSnippet(codeSnippet);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCodeSnippet(RunTimeCodeSnippet codeSnippet)
        {
            try
            {
                codeSnippetDA.UpdateCodeSnippet(codeSnippet);
            }
            catch
            {
                throw;
            }
        }

        public RunTimeCodeSnippet GetCodeSnippetById(int snippetId)
        {
            try
            {
                return codeSnippetDA.GetCodeSnippetById(snippetId);
            }
            catch
            {
                throw;
            }
        }

        public RunTimeCodeSnippet GetCodeSnippetByName(string snippetName)
        {
            try
            {
                return codeSnippetDA.GetCodeSnippetByName(snippetName);
            }
            catch
            {
                throw;
            }
        }

        public List<CodeSnippetData> GetSnippetList()
        {
            try
            {
                return codeSnippetDA.GetSnippetList();
            }
            catch
            {
                throw;
            }
        }

    }
}
