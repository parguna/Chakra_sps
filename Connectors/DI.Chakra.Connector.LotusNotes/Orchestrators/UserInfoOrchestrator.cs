using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using Domino;
using DI.Chakra.Logger;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.DomainModel;
namespace DI.Chakra.Connector.LotusNotes.Orchestrators
{
    public class UserInfoOrchestrator
    {
        

        private const string PROCESSNAME = "DiscoveryProcess";

        public UserInfoOrchestrator()
        {
            
        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public LotusUserHistoryInfo PopulateUser(NotesViewEntry viewEntry)
        {
            IList<LotusUserHistoryInfo> list = new List<LotusUserHistoryInfo>();

            LotusUserHistoryInfo lotusUserInfo = new LotusUserHistoryInfo();

            try
            {
                    lotusUserInfo.EndPointSystemId = EndpointSystemId;
                    lotusUserInfo.Name = ((object[])(((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[0]))[0].ToString();
                    lotusUserInfo.ListNameFullName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[1].ToString();
                    lotusUserInfo.FullName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[2].ToString();
                    lotusUserInfo.ListName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[3].ToString();
                    lotusUserInfo.Members = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[4].ToString();
                    lotusUserInfo.MailServer = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[5].ToString();
                    lotusUserInfo.MailFile = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[6].ToString();
                    lotusUserInfo.MailDomain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[7].ToString();
                    lotusUserInfo.MailAddress = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[8].ToString();
                    lotusUserInfo.ShortName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[9].ToString();
                    lotusUserInfo.Location = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[10].ToString();
                    lotusUserInfo.EncryptIncomingMail = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[11].ToString();
                    lotusUserInfo.CalendarDomain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[12].ToString();
                    lotusUserInfo.NetUserName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[13].ToString();
                    lotusUserInfo.HTTPPassword = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[14].ToString();
                    lotusUserInfo.CertThumbprint = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[15].ToString();
                    lotusUserInfo.InternetAddress = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[16].ToString();
                    lotusUserInfo.NewMailServer = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[17].ToString();
                    lotusUserInfo.NewMailFile = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[18].ToString();
                    lotusUserInfo.AltFullName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[19].ToString();
                    lotusUserInfo.MessageStorage = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[20].ToString();
                    lotusUserInfo.ServerName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[21].ToString();
                    lotusUserInfo.AltFullNameLanguage = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[22].ToString();
                    lotusUserInfo.QuickPlace = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[23].ToString();
                    lotusUserInfo.SametimeServer = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[24].ToString();
                    lotusUserInfo.Type = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[25].ToString();
                    lotusUserInfo.MailSystem = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[26].ToString();
                    lotusUserInfo.AdminpOldWebName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[27].ToString();
                    lotusUserInfo.HTTPPasswordChangeDate = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[28].ToString();
                    lotusUserInfo.HTTPPasswordForceChange = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[29].ToString();
                    lotusUserInfo.Policy = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[30].ToString();
                    lotusUserInfo.LTPA_UsrNm = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[31].ToString();
                    lotusUserInfo.DB2UserName = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[32].ToString();
                    lotusUserInfo.CryptoCap = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[33].ToString();
                    lotusUserInfo.AuditUser = PROCESSNAME;
                    lotusUserInfo.AuditDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Error, string.Format("Error occured in accessing the path {0}", "Error in Discovering User.."), ex);
            }
            return lotusUserInfo;
        }


    }
}
