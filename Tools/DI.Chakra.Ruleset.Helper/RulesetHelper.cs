using System;
using System.Data;
using System.Data.SqlClient;
using System.Workflow.Activities.Rules;

namespace DI.Chakra.Ruleset.Helper
{
    public class RulesetHelper
    {
        public void ExecuteRuleset(int ruleSetId, object target)
        {
            RuleSetData tmpRuleSet = this.GetRuleSetById(ruleSetId);

            RuleSetData ruleSet = this.GetRuleSetByName(tmpRuleSet.Name);

            this.ExecuteRuleset(ruleSet.RuleSet, target);
        }

        private RuleSetData GetRuleSetByName(string ruleSetName)
        {
            RuleSetData ruleSet = new RuleSetData();

            string query = string.Format("SELECT Name, MajorVersion, MinorVersion, RuleSet, [Status], AssemblyPath, ActivityName, LastModifiedDate, Active, ID 	FROM DI_M_RuleSet WHERE Name = '{0}' AND Active = 1", ruleSetName);

            using (SqlConnection connection = new SqlConnection(ConfigHelper.RuleSetStoreConnectionString))
            {
                //Geting Command object by passing storedproc name and its connection
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                ruleSet.Name = Convert.ToString(reader["Name"]);
                                ruleSet.OriginalName = ruleSet.Name; // will be used later to see if one of these key values changed                       
                                ruleSet.MajorVersion = Convert.ToInt32(reader["MajorVersion"]);
                                ruleSet.OriginalMajorVersion = ruleSet.MajorVersion;
                                ruleSet.MinorVersion = Convert.ToInt32(reader["MinorVersion"]);
                                ruleSet.OriginalMinorVersion = ruleSet.MinorVersion;

                                ruleSet.RuleSetDefinition = Convert.ToString(reader["RuleSet"]);
                                ruleSet.Status = Convert.ToInt16(reader["Status"]);
                                ruleSet.AssemblyPath = Convert.ToString(reader["AssemblyPath"]);
                                ruleSet.ActivityName = Convert.ToString(reader["ActivityName"]);
                                ruleSet.ModifiedDate = Convert.ToDateTime(reader["LastModifiedDate"]);
                                ruleSet.Active = Convert.ToBoolean(reader["Active"]);
                                ruleSet.ID = Convert.ToInt32(reader["ID"]);
                                ruleSet.Dirty = false;
                            }
                        }
                    }
                }
            }
            return ruleSet;
        }

        private RuleSetData GetRuleSetById(int ruleSetId)
        {
            RuleSetData ruleSet = new RuleSetData();

            string query = string.Format("SELECT Name, MajorVersion, MinorVersion, RuleSet, [Status], AssemblyPath, ActivityName, LastModifiedDate, Active, ID 	FROM DI_M_RuleSet WHERE ID = {0}", ruleSetId);

            using (SqlConnection connection = new SqlConnection(ConfigHelper.RuleSetStoreConnectionString))
            {
                //Geting Command object by passing storedproc name and its connection
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.Text;
                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader != null)
                        {
                            while (reader.Read())
                            {
                                ruleSet.Name = Convert.ToString(reader["Name"]);
                                ruleSet.OriginalName = ruleSet.Name; // will be used later to see if one of these key values changed                       
                                ruleSet.MajorVersion = Convert.ToInt32(reader["MajorVersion"]);
                                ruleSet.OriginalMajorVersion = ruleSet.MajorVersion;
                                ruleSet.MinorVersion = Convert.ToInt32(reader["MinorVersion"]);
                                ruleSet.OriginalMinorVersion = ruleSet.MinorVersion;

                                ruleSet.RuleSetDefinition = Convert.ToString(reader["RuleSet"]);
                                ruleSet.Status = Convert.ToInt16(reader["Status"]);
                                ruleSet.AssemblyPath = Convert.ToString(reader["AssemblyPath"]);
                                ruleSet.ActivityName = Convert.ToString(reader["ActivityName"]);
                                ruleSet.ModifiedDate = Convert.ToDateTime(reader["LastModifiedDate"]);
                                ruleSet.Active = Convert.ToBoolean(reader["Active"]);
                                ruleSet.ID = Convert.ToInt32(reader["ID"]);
                                ruleSet.Dirty = false;
                            }
                        }
                    }
                }
            }
            return ruleSet;
        }

        private void ExecuteRuleset(RuleSet ruleSet, object target)
        {
            RuleValidation validation = new RuleValidation(target.GetType(), null);
            if (ruleSet.Validate(validation))
            {
                RuleExecution execution = new RuleExecution(validation, target, null);
                ruleSet.Execute(execution);
            }
        }

    }
}
