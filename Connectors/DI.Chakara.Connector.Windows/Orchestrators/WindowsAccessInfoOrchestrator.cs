using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using System.DirectoryServices;
using DI.Chakra.Infrastructure;
using System.Collections;
using System.Security.AccessControl;
namespace DeepIdentity.Connector
{
   public class WindowsAccessInfoOrchestrator
    {
        private const string PROCESSNAME = "DiscoveryProcess";
        public WindowsAccessInfoOrchestrator()
        {

        }
        public int EndpointSystemId { get; set; }
        
        public WindowsAccessInfo PopulateAccess(Int64 AutoIncrementValue, string objectSid, string permission, int Endpointsystemid, string Resourcename, string ResourceCompletePath, string AccessType, string PropagationFlags, bool IsInherited)
        {        

            WindowsAccessInfo AccessList = new WindowsAccessInfo();

            try
            {
                AccessList.EndPointSystemId = Endpointsystemid;
                AccessList.ResourceID = AutoIncrementValue;
                AccessList.ResouceName=Resourcename;
                AccessList.ResourceCompletePath = ResourceCompletePath;
                AccessList.objectSid = objectSid;
                AccessList.Access_Rights = permission;
                AccessList.AccessType = AccessType;
                AccessList.IsInherited = IsInherited;
                AccessList.PropagationFlags = PropagationFlags;
               
            }
            catch (Exception)
            {
                throw;
            }
            return AccessList;
        }
    }
}
