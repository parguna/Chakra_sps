using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using DI.Chakra.DomainModel;


namespace DI.Chakra.Helpers.TokenReplacer
{
    public class TokenProcessor
    {

        long EntityId { get; set; }

        object Entity { get; set; }

        string Token { get; set; }

        string VariablePath1 { get; set; }
        string VariablePath2 { get; set; }

        VariableScope Scope { get; set; }

        public TokenProcessor(long entityId, string token)
        {
            try
            {
                this.EntityId = entityId;
                this.Token = token;

                this.Initialize();
            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, string.Format("Error while creating token replacer object instance for token: {0}.", this.Token), ex);
            }

        }

        private void Initialize()
        {
            try
            {
                if (string.IsNullOrEmpty(this.Token))
                {
                    throw new Exception("Token cannot be empty.");
                }

                var variable = MailTemplateVariable.GetByVariableValue(this.Token);

                this.Scope = variable.VariableScope;
                this.VariablePath1 = variable.VariableValuePath1;
                this.VariablePath2 = variable.VariableValuePath2;

                if (this.Scope == VariableScope.Discovery)
                {
                    //this.Entity = DiscoveryProcess.GetByIdProcessIncluded((int)this.EntityId);
                }
                else if (this.Scope == VariableScope.Endpoint)
                {
                    this.Entity =EndPoint.GetByIdIncludingEndPointSystemType((int)this.EntityId);
                }
                else if (this.Scope == VariableScope.Request)
                {
                     this.Entity = AccessRequest.GetByIdIncludeAdHocAccessRequestAndUserEmail((int)this.EntityId);              
                }
                else if (this.Scope == VariableScope.TaskAttestaion)
                {
                    this.Entity = Task.GetByIdAttestationTaskIncludedForEmail((int)this.EntityId);
                }
                else if (this.Scope == VariableScope.TaskProvisioning)
                {
                    this.Entity = Task.GetByIdProvioningTaskIncluded((int)this.EntityId);
                }                
                else if (this.Scope == VariableScope.User)
                {
                    this.Entity = User.GetById(this.EntityId);                 
                }
                else if (this.Scope == VariableScope.InHouse)
                {
                    this.Entity = InhouseUserInfo.GetByIdIncludeEndPointWithOwner(this.EntityId);
                }
                if (this.Entity == null && this.Scope != VariableScope.None)
                {
                    throw new Exception(string.Format("Value no found for the given EntityId: {0} and Scope: {1}", this.EntityId.ToString(), this.Scope.ToString()));
                }
            }
            catch
            {
                throw;
            }
        }

        public string Execute()
        {
            try
            {
                List<string> properties1 = this.VariablePath1.Split('.').ToList();

                object value = this.Entity;

                foreach (string item in properties1)
                {
                    value = value.GetType().GetProperty(item).GetValue(value);
                }


                if (value.ToString().Trim() == ""  &&  this.VariablePath2 !=null  &&  this.VariablePath2.ToString().Trim() != "")
                {
                    List<string> properties2 = this.VariablePath2.Split('.').ToList();

                    value = this.Entity;

                    foreach (string item in properties2)
                    {
                        value = value.GetType().GetProperty(item).GetValue(value);
                    }
                }

                return value.ToString();

            }
            catch (Exception ex)
            {
                DILog.EmailLogger.WriteLog(null,LogLevelType.Exception, string.Format("Error while replacing token: {0}.", this.Token), ex);
                return string.Empty;
            }
        }

    }
}
