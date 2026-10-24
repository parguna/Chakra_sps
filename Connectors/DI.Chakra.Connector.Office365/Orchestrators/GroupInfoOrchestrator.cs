using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector
{
    public class GroupInfoOrchestrator
    {
    
        public Office365GroupInfo PopulateGroup(PSObject psMemberInfo, long endpointId)
        {
            if (null == psMemberInfo)
                return null;

            Office365GroupInfo of365groupInfo = new Office365GroupInfo();

            try
            {
                of365groupInfo.EndPointSystemId = endpointId;
                of365groupInfo.member = Office365Helper.GetProperty(psMemberInfo, Office365Properties.DisplayName);
                of365groupInfo.distinguishedName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.DistinguishedName);
                of365groupInfo.instanceType = Office365Helper.GetProperty(psMemberInfo, Office365Properties.instanceType);
                of365groupInfo.whenCreated = Office365Helper.GetProperty_DateTime(psMemberInfo, Office365Properties.WhenCreated);
                of365groupInfo.whenChanged = Office365Helper.GetProperty_DateTime(psMemberInfo, Office365Properties.whenChanged);
                of365groupInfo.name = Office365Helper.GetProperty(psMemberInfo, Office365Properties.Name);
                of365groupInfo.sAMAccountName = Office365Helper.GetProperty(psMemberInfo, Office365Properties.SamAccountName);
                of365groupInfo.sAMAccountType = Office365Helper.GetProperty(psMemberInfo, Office365Properties.sAMAccountType);
                of365groupInfo.groupType = Office365Helper.GetProperty(psMemberInfo, Office365Properties.groupType);
                of365groupInfo.objectCategory = Office365Helper.GetProperty(psMemberInfo, Office365Properties.objectCategory);

            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                //Log.Discovery.WriteMessage(LogLevel.ERROR,
                //               string.Format("Error occured in accessing the path {0}",
                //               directoryEntry.Path), ex);

            }

            return of365groupInfo;



        }
    }
}
