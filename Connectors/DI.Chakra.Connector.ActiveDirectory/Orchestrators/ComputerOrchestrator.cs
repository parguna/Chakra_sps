using System;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using System.DirectoryServices;
using System.Collections.Generic;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Connector
{
    public class ComputerOrchestrator
    {


        public ComputerOrchestrator()
        {

        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public ADSComputerInfo PopulateComputer(DirectoryEntry directoryEntry, DirectorySearcher _dirSearcher, ref IList<ADSComputerGroup> _listUserGroups)
        {
            if (null == directoryEntry)
                return null;



            ADSComputerInfo obj = new ADSComputerInfo();

            try
            {
                
                
                obj.EndPointSystemId = EndpointSystemId;
                obj.cn = ADHelper.GetProperty(directoryEntry, ADProperties.cn);
                obj.givenName = ADHelper.GetProperty(directoryEntry, ADProperties.givenName);
                obj.distinguishedName = ADHelper.GetProperty(directoryEntry, ADProperties.distinguishedName);
                obj.instanceType = ADHelper.GetProperty(directoryEntry, ADProperties.instanceType);
                obj.whenCreated = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenCreated);
                obj.whenChanged = ADHelper.GetProperty_DateTime(directoryEntry, ADProperties.whenChanged);
                obj.displayName = ADHelper.GetProperty(directoryEntry, ADProperties.displayName);
                obj.uSNCreated = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNCreated);
                obj.uSNChanged = ADHelper.GetCOMProperty_Int64(directoryEntry, ADProperties.uSNChanged);
                obj.name = ADHelper.GetProperty(directoryEntry, ADProperties.name);
                obj.objectGUID = ADHelper.GetProperty_GUID(directoryEntry, ADProperties.objectGUID);
                obj.userAccountControl = ADHelper.GetProperty(directoryEntry, ADProperties.userAccountControl);
                obj.badPwdCount = ADHelper.GetProperty(directoryEntry, ADProperties.badPwdCount);
                obj.codePage = ADHelper.GetProperty(directoryEntry, ADProperties.codePage);
                obj.countryCode = ADHelper.GetProperty(directoryEntry, ADProperties.countryCode);
                obj.badPasswordTime = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.badPasswordTime);
                obj.lastLogoff = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.lastLogoff);
                obj.lastLogon = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.lastLogon);
                obj.localPolicyFlags = ADHelper.GetProperty(directoryEntry, ADProperties.localPolicyFlags);
                obj.pwdLastSet = ADHelper.GetCOMProperty_DateTime(directoryEntry, ADProperties.pwdLastSet);
                obj.primaryGroupID = ADHelper.GetProperty(directoryEntry, ADProperties.primaryGroupID);
                obj.objectSid = ADHelper.GetProperty_SID(directoryEntry, ADProperties.objectSid);
                obj.accountExpires = ADAccountHelper.AccountExpiresOn(directoryEntry);
                obj.logonCount = ADHelper.GetProperty_Int(directoryEntry, ADProperties.logonCount);
                obj.sAMAccountName = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountName);
                obj.sAMAccountType = ADHelper.GetProperty(directoryEntry, ADProperties.sAMAccountType);
                obj.operatingSystem = ADHelper.GetProperty(directoryEntry, ADProperties.operatingSystem);
                obj.operatingSystemVersion = ADHelper.GetProperty(directoryEntry, ADProperties.operatingSystemVersion);
                obj.operatingSystemServicePack = ADHelper.GetProperty(directoryEntry, ADProperties.operatingSystemServicePack);
                obj.serverReferenceBL = ADHelper.GetProperty(directoryEntry, ADProperties.serverReferenceBL);
                obj.dNSHostName = ADHelper.GetProperty(directoryEntry, ADProperties.dNSHostName);
                obj.rIDSetReferences = ADHelper.GetProperty(directoryEntry, ADProperties.rIDSetReferences);
                obj.servicePrincipalName = ADHelper.GetProperty(directoryEntry, ADProperties.servicePrincipalName);
                obj.objectCategory = ADHelper.GetProperty(directoryEntry, ADProperties.objectCategory);
                obj.isCriticalSystemObject = ADHelper.GetProperty(directoryEntry, ADProperties.isCriticalSystemObject);
                obj.frsComputerReferenceBL = ADHelper.GetProperty(directoryEntry, ADProperties.frsComputerReferenceBL);
                //obj.nTSecurityDescriptor = ADHelper.GetProperty(directoryEntry, ADProperties.nTSecurityDescriptor);
                obj.Version = Convert.ToInt16(VersionNo.Available);
                


                ADSComputerGroup _computerGroup = null;



                _dirSearcher.Filter = String.Format("(distinguishedname={0})", obj.distinguishedName);
                _dirSearcher.PropertiesToLoad.Add("memberOf");
                StringBuilder groupsList = new StringBuilder();

                SearchResult result = _dirSearcher.FindOne();
                if (result != null)
                {
                    int groupCount = result.Properties["memberOf"].Count;

                    for (int counter = 0; counter < groupCount; counter++)
                    {

                        _computerGroup = new ADSComputerGroup();

                        _computerGroup.Id = 1;
                        _computerGroup.EndpointSystemId = EndpointSystemId;
                        _computerGroup.ComputerCN = obj.cn;
                        _computerGroup.ComputerDistinguishedName = obj.distinguishedName;
                       

                        _computerGroup.GroupDistinguishedName = result.Properties["memberOf"][counter].ToString();
                        _computerGroup.GroupCN = result.Properties["memberOf"][counter].ToString().Split(',').First().Split('=')[1].ToString();
                        _listUserGroups.Add(_computerGroup);

                    }
                }


            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
             //DILog.ProvisioningLogger.WriteLog(CurrentRequest.Id, Endpoint.Id,   LogLevelType.Error,
             //                  string.Format("Error occured in accessing the path {0}",
             //                  directoryEntry.Path), ex);


                return null;

            }

            return obj;

        }
    }
}