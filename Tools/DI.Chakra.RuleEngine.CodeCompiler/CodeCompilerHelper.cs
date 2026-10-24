using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace DI.Chakra.RuleEngine.CodeCompiler
{
    public class CodeCompilerHelper
    {

        public string ExecuteExternalCode(int codeSnippetId, object context)
        {
            try
            {
                var result = this.Execute(codeSnippetId, context);
                return result.ToString();
            }
            catch
            {
                throw;
            }
        }

        private object Execute(int codeSnippetId, object context)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                // Reference to System.Drawing library
                parameters.ReferencedAssemblies.Add("DI.Chakra.DomainModel.dll");
                parameters.ReferencedAssemblies.Add("DI.Chakra.EntityModel.dll");
                parameters.ReferencedAssemblies.Add("DI.Chakra.Infrastructure.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.dll");
                // True - memory generation, false - external file generation
                parameters.GenerateInMemory = true;
                // True - exe file generation, false - dll file generation
                parameters.GenerateExecutable = false;

                string codeSnippet = this.GetCodeSnippetById(codeSnippetId);

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeSnippet);

                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    throw new InvalidOperationException(sb.ToString());
                }

                Assembly assembly = results.CompiledAssembly;
                Type program = assembly.GetType("Program");
                MethodInfo main = program.GetMethod("Main");

                return main.Invoke(null, new object[] { context });
            }
            catch
            {
                throw;
            }            
        }

        //private string GetCodeSnippetByName(string snippetName)
        //{
        //    try
        //    {
        //        CodeSnippetFacade snippetFacade = new CodeSnippetFacade();

        //        var snippet = snippetFacade.GetCodeSnippetByName(snippetName);

        //        return UtilityMethods.GetStringFromByteArray(snippet.CodeSnippet);

        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        private string GetCodeSnippetById(int snippetId)
        {
            try
            {
                var runTimeCodeSnippets = RunTimeCodeSnippet.GetById(snippetId);

                return UtilityMethods.GetStringFromByteArray(runTimeCodeSnippets.CodeSnippet);

            }
            catch
            {
                throw;
            }
        }

    }
}
