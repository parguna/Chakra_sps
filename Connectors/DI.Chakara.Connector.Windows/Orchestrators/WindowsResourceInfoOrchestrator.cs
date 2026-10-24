using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.EntityModel;
using System.DirectoryServices;
using DI.Chakra.Infrastructure;
using System.Collections;
using System.IO;
namespace DeepIdentity.Connector
{
    public class WindowsResourceInfoOrchestrator
    {
        private const string PROCESSNAME = "DiscoveryProcess";
        public WindowsResourceInfoOrchestrator()
        {

        }
        public int EndpointSystemId { get; set; }
        

        public WindowsResourceHistoryInfo PopulateResources(Int64 AutoIncrementValue, Int64 ParentID, String ResourceName, String path, DateTime CreationTime, DateTime LastAccessTime, DateTime LastWriteTime,String Type,int IsShare)
        {

            WindowsResourceHistoryInfo ResourceList = new WindowsResourceHistoryInfo();
            try
            {
                ResourceList.ResouceID = AutoIncrementValue;
                ResourceList.EndPointSystemId =EndpointSystemId;
                ResourceList.ParentID = ParentID;
                ResourceList.Name = ResourceName;
                ResourceList.ResourceCompletePath = path;
                ResourceList.ResourceType = Type;
                ResourceList.Resource_CreatedDate = CreationTime;
                ResourceList.Resource_LastAccessedDate = LastAccessTime;
                ResourceList.Resource_LasteWriteDate = LastWriteTime;
                ResourceList.IsShare = (IsShare == 1) ? true : false;
                

                if (-1 == DateTime.Compare(Convert.ToDateTime(CreationTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_CreatedDate = null;

                if (-1 == DateTime.Compare(Convert.ToDateTime(LastAccessTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_LastAccessedDate = null;

                if (-1 == DateTime.Compare(Convert.ToDateTime(LastWriteTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_LasteWriteDate = null;

            }                 
            catch (Exception ex)
            {
                
                throw ex;
            }
            return ResourceList;
        }

        public WindowsResourceInfo PopulateResourceInfos(Int64 AutoIncrementValue, Int64 ParentID, String ResourceName, String path, DateTime CreationTime, 
                                                                                   DateTime LastAccessTime, DateTime LastWriteTime, String Type, int IsShare, int paramLevel)
        {

            WindowsResourceInfo ResourceList = new WindowsResourceInfo();
            try
            {
                ResourceList.Id = AutoIncrementValue;
                ResourceList.EndPointSystemId = EndpointSystemId;
                ResourceList.ParentID = ParentID;
                ResourceList.Name = ResourceName;
                ResourceList.ResourceCompletePath = path;
                ResourceList.ResourceType = Type;
                ResourceList.Resource_CreatedDate = CreationTime;
                ResourceList.Resource_LastAccessedDate = LastAccessTime;
                ResourceList.Resource_LasteWriteDate = LastWriteTime;
                ResourceList.IsShare = (IsShare == 1) ? true : false;
                if (paramLevel != -1)
                    ResourceList.Level = paramLevel;
                

                if (-1 == DateTime.Compare(Convert.ToDateTime(CreationTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_CreatedDate = null;

                if (-1 == DateTime.Compare(Convert.ToDateTime(LastAccessTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_LastAccessedDate = null;

                if (-1 == DateTime.Compare(Convert.ToDateTime(LastWriteTime), Convert.ToDateTime(Convert.ToDateTime("1/1/1753 12:00:00 AM"))))
                    ResourceList.Resource_LasteWriteDate = null;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return ResourceList;
        }
    }
}
