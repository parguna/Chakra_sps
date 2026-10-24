using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Helpers.TokenReplacer
{
    public class TokenReplacer
    {

        public static Dictionary<string, string> CollectPlaceHolders(int templateId,object objectId)
        {
            Dictionary<string, string> placeHolders = new Dictionary<string, string>();

            DomainModel.MailTemplate template = DomainModel.MailTemplate.GetByTemplateId(templateId);

            string[] variablevaluesSubject = System.Text.RegularExpressions.Regex.Split(template.MailSubject, @"[^@@(\w+)@@$]");

            string[] variablevaluesContent = System.Text.RegularExpressions.Regex.Split(template.MailContent, @"[^@@(\w+)@@$]");

            foreach (string variableval in variablevaluesSubject)
            {
                if (variableval.StartsWith("@@") && variableval.EndsWith("@@"))
                {
                    DomainModel.MailTemplateVariable templateVariable = DomainModel.MailTemplateVariable.GetByVariableValue(variableval);
                    placeHolders.Add(templateVariable.VariableValue, templateVariable.VariableValuePath1);
                }
            }

            foreach (string variableval in variablevaluesContent)
            {
                try
                {
                    if (variableval.StartsWith("@@") && variableval.EndsWith("@@"))
                    {
                        DomainModel.MailTemplateVariable templateVariable = DomainModel.MailTemplateVariable.GetByVariableValue(variableval);
                        placeHolders.Add(templateVariable.VariableValue, templateVariable.VariableValuePath1);
                    }
                }
                catch (System.ArgumentException)
                {
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return placeHolders;

        }
        public static StringBuilder[] Execute(int templateId,object objectId)
        {
            Dictionary<string, string> placeHolders = CollectPlaceHolders(templateId,objectId);
            StringBuilder[] subjectContent = ReplacePlaceHolders(placeHolders, templateId, objectId);
            return subjectContent;
        }
        public static  StringBuilder[] ReplacePlaceHolders(Dictionary<string, string> placeHolders, int templateId, object objectId)
        {
            DomainModel.MailTemplate template = DomainModel.MailTemplate.GetByTemplateId(templateId);
            StringBuilder subject = new StringBuilder(template.MailSubject);
            StringBuilder content = new StringBuilder(template.MailContent);
            foreach (var placeholder in placeHolders)
            {
                TokenProcessor tokenProcessor = new TokenProcessor(Convert.ToInt32(objectId), placeholder.Key);
                string palceholderValue = tokenProcessor.Execute();
                subject.Replace(placeholder.Key, palceholderValue);
                content.Replace(placeholder.Key, palceholderValue);

                //subject.Replace(placeholder.Key, placeholder.Value.ToString());
                // content.Replace(placeholder.Key, placeholder.Value.ToString());
            }

            StringBuilder[] replacedSubAndBody = { subject, content };

            return replacedSubAndBody;
        }
    }
}
