using DI.Chakra.Connector.Linux;
using DI.Chakra.Infrastructure;
using DI.Chakra.DomainModel;
using Tamir.SharpSsh;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.Connector
{
    public class LinuxConnector : ConnectorBase
    {
        #region Members

        SshConnectionInfo input;
        SshExec exec; public IList<LinuxUserGroupMap> UserGroups { get; set; }

        private EndpointConfiguration m_Config = null;
        public EndpointConfiguration LinuxConfig
        {
            get { return m_Config; }
            set { this.m_Config = value; }
        }

        #endregion

        #region Core

        public LinuxConnector(EndPoint endpoint, AccessRequest currentRequest)
        {

            this.CurrentRequest = currentRequest;
            this.EndPoint = endpoint;
            var config = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);
            config.ConfigurationDetails.ForEach(lq =>
            {
                if (lq.KeyId == Convert.ToInt64(EndpointKey.Password))
                {
                    lq.Value = RSAHelper.DecryptData(lq.Value);
                }
            });
            this.LinuxConfig = config;
        }
        public override IMProcessor IMProcessor
        {
            get
            {
                Unix.IMProcessor processor = new Unix.IMProcessor();
                processor.EndPoint = this.EndPoint;
                processor.LinuxConfig = this.LinuxConfig;

                return processor;
            }
        }

        #endregion

        #region Discovery

        public override bool Connect()
        {
            try
            {
                input = LinuxUtil.GetInput();
                exec = new SshExec(Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName));
                if (Configuration(EndpointKey.Password) != null) exec.Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                exec.Connect();

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Connection Established SuccessFully."));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        private string Configuration(EndpointKey Key)
        {
            var config = LinuxConfig.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }
        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            try
            {
                if (this.Connect())
                {

                    string groupcommand = "";
                    if (this.LinuxConfig.EndpointSystem.EndPointSystemType.Id == (int)DI.Chakra.Infrastructure.Enums.EndpointType.Linux)
                    {
                        groupcommand = string.Format("echo -e \"{1}\n{1}\" | passwd {0}", UserName, Password);
                    }
                    else
                    {
                        groupcommand = string.Format("echo -e \"{1}\n{1}\" | passwd {0}", UserName, Password);
                    }

                    string groupoutput = exec.RunCommand(groupcommand);

                    exec.Close();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }

        }

        public override void GetData()
        {


            try
            {

                long DiscoveryProcessId = base.DiscoveryProcess.Id;

                List<EndpointObjects> EndpointObjects = DI.Chakra.DomainModel.DiscoveryProcess.GetByDiscoveryProcessId(DiscoveryProcessId);

                bool IsUserSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LINUX_User) != null;
                bool IsGroupSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LINUX_Group) != null;
                bool IsMountSelected = EndpointObjects.FirstOrDefault(item => item.ObjectName == Constants.EndpointObject_LINUX_Mount) != null;


                if (IsUserSelected)
                {

                    List<LinuxUserInfo> Discoveredlist = this.getUsers();
                    List<LinuxUserInfo> Existinglist = LinuxUserInfo.GetByEndpointId(this.EndPoint.Id);
                    List<LinuxUserInfo> Deletedlist = Existinglist.Where(c => Discoveredlist.Count(d => d.UserName == c.UserName) == 0).ToList();
                    List<LinuxUserInfo> Newlist = Discoveredlist.Where(c => Existinglist.Count(d => d.UserName == c.UserName) == 0).ToList();
                    List<LinuxUserInfo> Otherlist = Discoveredlist.Where(c => Existinglist.Count(d => d.UserName == c.UserName) > 0).ToList();
                    Otherlist.Select(c => { c.Id = Existinglist.Where(d => d.UserName == c.UserName).Select(e => e.Id).FirstOrDefault(); return c; }).ToList();

                    List<LinuxUserInfo> savingUsers = Otherlist.Union(Newlist).ToList();
                    LinuxUserInfo.SaveUsers(savingUsers, this.EndPoint.Id, MasterConfigHelper.ServiceUser);

                }
                if (IsGroupSelected)
                {
                    List<LinuxGroupInfo> list = this.getGroups();

                    if (list.Count > 0)
                        LinuxGroupInfo.SaveGroups(list, this.EndPoint.Id, MasterConfigHelper.ServiceUser);

                    this.getUserGroup();

                }

                if (IsMountSelected)
                {
                    IList<LinuxResourceInfo> list = this.getResources();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private List<LinuxUserInfo> getUsers()
        {
            List<LinuxUserInfo> list = new List<LinuxUserInfo>();

            LinuxUserInfo _obj = null;

            try
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Trying to pull the linux users"));
                string Usercommand = "cat /etc/passwd";

                if (this.Connect())
                {
                    string output = exec.RunCommand(Usercommand);

                    output = output.Replace(":/:/", ":/null:/");
                    output = output.Replace("::", ":null:");
                    output = output.Replace(":\n", ":null\n");
                    char[] delimiters = new char[] { '\n' };
                    char[] newdelimiters = new char[] { ':' };
                    IList<string> parts = output.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                    LinuxUserInfoOrchestrator orchestrator = new LinuxUserInfoOrchestrator();


                    orchestrator.EndpointSystemId = (int)this.EndPoint.Id;


                    foreach (string str in parts)
                    {

                        LinuxUserInfo linuxUserInfo = orchestrator.getUser(str, EndPoint);

                        if (linuxUserInfo != null)
                            list.Add(linuxUserInfo);
                    }

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populated {0} LinuxUser(s) objects InMemory ", list.Count.ToString()));
                }
            }
            catch (Exception ex)
            {

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Users Population Error.", ex));
            }

            return list;
        }

        private List<LinuxGroupInfo> getGroups()
        {
            List<LinuxGroupInfo> list = new List<LinuxGroupInfo>();


            try
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Trying to pull the linux groups"));


                string Groupcommand = "cat /etc/group";

                if (this.Connect())
                {
                    string Groupoutput = exec.RunCommand(Groupcommand);
                    Groupoutput = Groupoutput.Replace(":/:/", ":/null:/");
                    Groupoutput = Groupoutput.Replace("::", ":null:");
                    Groupoutput = Groupoutput.Replace(":\n", ":null\n");
                    char[] gdelimiters = new char[] { '\n' };
                    char[] gnewdelimiters = new char[] { ':' };
                    string[] gparts = Groupoutput.Split(gdelimiters, StringSplitOptions.RemoveEmptyEntries);


                    IList<LinuxUserGroupMap> listUserGroups = new List<LinuxUserGroupMap>();

                    foreach (string str in gparts)
                    {

                        LinuxGroupInfo linuxGroupInfo = LinuxGroupInfoOrchestrator.getGroup(str, this.EndPoint, ref listUserGroups);

                        if (linuxGroupInfo != null)
                            list.Add(linuxGroupInfo);
                    }

                    this.UserGroups = listUserGroups;

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populated {0} LinuxGroup(s) objects InMemory ", list.Count.ToString()));

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populated {0} LinuxUserGroupMap(s) objects InMemory ", listUserGroups.Count.ToString()));
                }

            }
            catch (Exception ex)
            {

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Groups Population Error.", ex));
            }

            return list;
        }

        public string getUserGroup()
        {

            try
            {
                List<LinuxUserInfo> linuxUsers = LinuxUserInfo.GetByEndpointId(this.EndPoint.Id);

                if (linuxUsers == null)
                {
                    return "";
                }

                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Trying to pull the linux groups"));

                if (this.Connect())
                {

                    List<LinuxUserGroupMap> listUserGroups = new List<LinuxUserGroupMap>();
                    IList<LinuxUserInfoHistory> list = new List<LinuxUserInfoHistory>();
                    foreach (LinuxUserInfo users in linuxUsers)
                    {
                        string Groupcommand = string.Format("cd /etc; groups {0};cd /root", users.UserName);
                        //
                        string Groupoutput = exec.RunCommand(Groupcommand);

                        Groupoutput = Groupoutput.Replace(":/:/", ":/null:/");
                        Groupoutput = Groupoutput.Replace("::", ":null:");
                        Groupoutput = Groupoutput.Replace(":\n", ":null\n");
                        char[] delimiters = new char[] { '\n' };
                        char[] newdelimiters = new char[] { ':' };
                        IList<string> parts = Groupoutput.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();

                        LinuxUserInfoOrchestrator orchestrator = new LinuxUserInfoOrchestrator();


                        orchestrator.EndpointSystemId = (int)this.EndPoint.Id;

                        foreach (string str in parts)
                        {

                            List<LinuxUserGroupMap> usrGroups = orchestrator.getUserGroups(str, EndPoint);

                            if (usrGroups != null)
                            {
                                foreach (LinuxUserGroupMap items in usrGroups)
                                {
                                    items.EndpointSystemId = this.EndPoint.Id;
                                    items.Version = (int)VersionNo.Available;
                                    if (listUserGroups.Count(c => c.LinuxGroupInfo_Id == items.LinuxGroupInfo_Id && c.LinuxUserInfo_Id == items.LinuxUserInfo_Id) == 0)
                                    {
                                        listUserGroups.Add(items);
                                    }

                                }

                            }
                        }

                    }

                    LinuxGroupInfo.bulkInsert(listUserGroups, "LinuxUserInfoLinuxGroupInfo", true, "EndpointSystemId=" + this.EndPoint.Id);

                    this.UserGroups = listUserGroups;

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populated {0} LinuxGroup(s) objects InMemory ", list.Count.ToString()));

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Populated {0} LinuxUserGroupMap(s) objects InMemory ", listUserGroups.Count.ToString()));
                }
            }
            catch (Exception)
            { }
            return "Success";
        }
        private IList<LinuxResourceInfo> getResources()
        {
            List<LinuxResourceInfo> list = new List<LinuxResourceInfo>();
            LinuxResourceInfo _obj = null;
            try
            {


                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Trying to pull the linux Mount Resources"));
                SshConnectionInfo input = LinuxUtil.GetInput();
                SshExec exec = new SshExec(Configuration(EndpointKey.IPAddress), Configuration(EndpointKey.UserName));
                if (Configuration(EndpointKey.Password) != null) exec.Password = RSAHelper.DecryptData(Configuration(EndpointKey.Password));

                string Usercommand = string.Empty;
                exec.Connect();

                Usercommand = "df -h";

                string output = exec.RunCommand(Usercommand);

                // User data split code
                output = output.Replace(":/:/", ":/null:/");
                output = output.Replace("::", ":null:");
                output = output.Replace(":\n", ":null\n");
                char[] delimiters = new char[] { '\n' };
                char[] newdelimiters = new char[] { ':' };
                IList<string> parts = output.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
                char[] sharenewdelimiters = new char[] { ' ' };
                ArrayList mountparts = new ArrayList();
                mountparts.Add("/");
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Added Mount drive : {0}", mountparts[0].ToString()));


                LinuxResourceInfo linuxResourceInfo = null;

                foreach (string strMt in mountparts)
                {
                    Usercommand = "ls -ltR " + strMt;  //Works for solaris 10
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Command Started " + Usercommand);

                    string ShareOutPut = exec.RunCommand(Usercommand);
                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Command executed " + ShareOutPut);

                    ShareOutPut = ShareOutPut.Replace(":/:/", ":/null:/");
                    ShareOutPut = ShareOutPut.Replace("::", ":null:");
                    ShareOutPut = ShareOutPut.Replace(":\n", ":null\n");
                    char[] sdelimiters = new char[] { '\n' };
                    char[] snewdelimiters = new char[] { ' ' };
                    string[] sparts = ShareOutPut.Split(sdelimiters, StringSplitOptions.RemoveEmptyEntries);

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Total resources found on {0} = {1}", strMt, sparts.Count().ToString()));

                    foreach (string str in sparts)
                    {
                        linuxResourceInfo = LinuxResourceInfoOrchestrator.getMounts(str, this.EndPoint);

                        if (linuxResourceInfo != null)
                        {
                            linuxResourceInfo.MountDrive = strMt;
                            list.Add(linuxResourceInfo);
                        }
                        if (list.Count >= 5000)
                        {

                            DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Inserted 5000 LinuxResources(s) objects "));
                            list.Clear();
                        }

                    }


                }

                if (list.Count > 0)
                {
                    LinuxResourceInfo.AddLinuxResourceInfoBulk(list);

                    DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("Inserted {0} LinuxResources(s) objects ", list.Count.ToString()));
                    list.Clear();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, string.Format("LinuxResources Population Error.", ex));
            }
            return list;
        }

        #endregion
    }
}
