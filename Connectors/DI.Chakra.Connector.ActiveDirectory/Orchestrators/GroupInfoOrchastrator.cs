using System;
using System.Text;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
using System.DirectoryServices;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;


namespace DI.Chakra.Connector
{
    public class GroupInfoOrchestrator
    {


        public GroupInfoOrchestrator()
        {

        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public ADSGroupInfoHistory PopulateGroup(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher)
        {
            if (null == directoryEntry)
                return null;



            ADSGroupInfoHistory adsgroupInfo = new ADSGroupInfoHistory();

            try
            {
                
                adsgroupInfo.HistoryId = HistoryId;
                adsgroupInfo.EndPointSystemId = EndpointSystemId;
                adsgroupInfo.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                adsgroupInfo.description = ADHelper.GetProperty(directoryEntry, ADProperties.description);
                adsgroupInfo.member = ADHelper.GetProperty(directoryEntry, ADProperties.member);
                adsgroupInfo.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                adsgroupInfo.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);
                adsgroupInfo.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                adsgroupInfo.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                adsgroupInfo.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);
                adsgroupInfo.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);
                adsgroupInfo.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                adsgroupInfo.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                adsgroupInfo.objectSid = ADHelper.GetProperty_SID(directoryEntry, ADProperties.objectSid);
                adsgroupInfo.sAMAccountName = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountName);
                adsgroupInfo.sAMAccountType = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountType);
                adsgroupInfo.groupType = ADHelper.GetProperty(directoryEntry, ADProperties.groupType);
                adsgroupInfo.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                //adsgroupInfo.nTSecurityDescriptor = ADHelper.GetProperty(directoryEntry, ADProperties.nTSecurityDescriptor);
                

                _dirSearcher.Filter = String.Format("(cn={0})", adsgroupInfo.cn);
                _dirSearcher.PropertiesToLoad.Add("member");
                StringBuilder groupsList = new StringBuilder();

                SearchResult result1 = _dirSearcher.FindOne();
                if (result1 != null)
                {
                    int groupCount = result1.Properties["member"].Count;

                    for (int counter = 0; counter < groupCount; counter++)
                    {
                        groupsList.Append((string)result1.Properties["member"][counter]);
                        groupsList.Append("|");
                    }

                    if (groupsList.ToString().Length > 0)
                        groupsList.Length -= 1; //remove the last '|' symbol
                }





            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                //DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id,  LogLevelType.Error,
                //                string.Format("Error occured in accessing the path {0}",
                //                directoryEntry.Path), ex);
                return null;
            }

            return adsgroupInfo;

        }
        public ADSGroupInfo PopulateGroupLive(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher)
        {
            if (null == directoryEntry)
                return null;

            ADSGroupInfo adsgroupInfo = new ADSGroupInfo();

            try
            {
                adsgroupInfo.EndPointSystemId = EndpointSystemId;
                adsgroupInfo.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                adsgroupInfo.description = ADHelper.GetProperty(directoryEntry, ADProperties.description);
                adsgroupInfo.member = ADHelper.GetProperty(directoryEntry, ADProperties.member);
                adsgroupInfo.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                adsgroupInfo.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);
                adsgroupInfo.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                adsgroupInfo.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                adsgroupInfo.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);
                adsgroupInfo.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);
                adsgroupInfo.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                adsgroupInfo.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                adsgroupInfo.objectSid = ADHelper.GetProperty_SID(directoryEntry, ADProperties.objectSid);
                adsgroupInfo.sAMAccountName = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountName);
                adsgroupInfo.sAMAccountType = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountType);
                adsgroupInfo.groupType = ADHelper.GetProperty(directoryEntry, ADProperties.groupType);
                if (adsgroupInfo.groupType == Constants.GROUP_TYPE)
                {
                    adsgroupInfo.groupTypeDesc = Constants.DISTRIBUTION_GROUP;
                }
                else
                    adsgroupInfo.groupTypeDesc = Constants.SECURITY_GROUP;
                adsgroupInfo.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                adsgroupInfo.Version = Convert.ToInt16(VersionNo.Available);
                
                if (!adsgroupInfo.cn.Contains(")") && !adsgroupInfo.cn.Contains("("))
                {
                    _dirSearcher.Filter = String.Format("(cn={0})", adsgroupInfo.cn);
                    _dirSearcher.PropertiesToLoad.Add("member");
                    StringBuilder groupsList = new StringBuilder();

                    SearchResult result1 = _dirSearcher.FindOne();
                    if (result1 != null)
                    {
                        int groupCount = result1.Properties["member"].Count;

                        for (int counter = 0; counter < groupCount; counter++)
                        {
                            groupsList.Append((string)result1.Properties["member"][counter]);
                            groupsList.Append("|");
                        }

                        if (groupsList.ToString().Length > 0)
                            groupsList.Length -= 1; //remove the last '|' symbol
                    }
                }                
            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error,
                                 $"Error occured in accessing the path {directoryEntry.Path}", ex);
                return null;
            }

            return adsgroupInfo;

        }

        public ADSGroupInfo PopulateNestedGroup(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher)
        {
            if (null == directoryEntry)
                return null;

            ADSGroupInfo adsgroupInfo = new ADSGroupInfo();

            try
            {
                StringBuilder groupsList = new StringBuilder();
                adsgroupInfo.EndPointSystemId = EndpointSystemId;
                adsgroupInfo.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                adsgroupInfo.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                adsgroupInfo.sAMAccountName = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountName);
                adsgroupInfo.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                adsgroupInfo.memberOf = ADHelper.GetProperty_Array(directoryEntry, ADProperties.memberOf).ToArray();


                if (!adsgroupInfo.cn.Contains(")") && !adsgroupInfo.cn.Contains("("))
                {
                    _dirSearcher.Filter = String.Format("(cn={0})", adsgroupInfo.cn);
                    _dirSearcher.PropertiesToLoad.Add("member");


                    SearchResult result1 = _dirSearcher.FindOne();
                    if (result1 != null)
                    {
                        int groupCount = result1.Properties["member"].Count;

                        for (int counter = 0; counter < groupCount; counter++)
                        {
                            groupsList.Append((string)result1.Properties["member"][counter]);
                            groupsList.Append("|");
                        }

                        if (groupsList.ToString().Length > 0)
                            groupsList.Length -= 1; //remove the last '|' symbol
                    }
                }
                adsgroupInfo.member = groupsList.ToString();
                adsgroupInfo.member = ADHelper.GetProperty(directoryEntry, ADProperties.member);
            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.DiscoveryLogger.WriteLog(LogLevelType.Error,
                                 $"Error occured in accessing the path {directoryEntry.Path}", ex);
                return null;
            }

            return adsgroupInfo;

        }



    }
}
