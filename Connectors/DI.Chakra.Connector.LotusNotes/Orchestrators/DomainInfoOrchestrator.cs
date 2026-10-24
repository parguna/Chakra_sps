using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using Domino;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel.Presentation;
namespace DI.Chakra.Connector.LotusNotes.Orchestrators
{
    public class DomainInfoOrchestrator
    {
        private const string PROCESSNAME = "DiscoveryProcess";
        public DomainInfoOrchestrator()
        {
            
        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public PresentationLotusDomainInfo PopulateDomain(NotesViewEntry viewEntry)
        {
            IList<PresentationLotusDomainHistoryInfo> list = new List<PresentationLotusDomainHistoryInfo>();
            PresentationLotusDomainInfo DomainsInfo = new PresentationLotusDomainInfo();
            try
            {
                DomainsInfo.EndPointSystemId = EndpointSystemId;
                DomainsInfo.Domain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[0].ToString();
                DomainsInfo.NextDomain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[1].ToString();
                DomainsInfo.MailServer = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[2].ToString();
                DomainsInfo.MailFile =((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[3].ToString();
                DomainsInfo.Allow = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[4].ToString();
                DomainsInfo.Deny = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[5].ToString();
                DomainsInfo.Country = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[6].ToString();
                DomainsInfo.ADMD = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[7].ToString();
                DomainsInfo.PRMD = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[8].ToString();
                DomainsInfo.Org = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[9].ToString();
                DomainsInfo.OrgUnit1 = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[10].ToString();
                DomainsInfo.OrgUnit2 = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[11].ToString();
                DomainsInfo.OrgUnit3 = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[12].ToString();
                DomainsInfo.OrgUnit4 = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[13].ToString();
                DomainsInfo.Type = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[14].ToString();
                DomainsInfo.Description = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[15].ToString();
                DomainsInfo.CalendarServer = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[16].ToString();
                DomainsInfo.CalendarSystem = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[17].ToString();
                DomainsInfo.DefaultGlobalDomain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[18].ToString();
                DomainsInfo.LocalPrimaryDomain = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[19].ToString();
                DomainsInfo.LocalDomainAliases = ((object[])(((Domino.NotesViewEntry)(viewEntry)).ColumnValues))[20].ToString();
                DomainsInfo.Version = Convert.ToInt16(VersionNo.Available);
            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Error, string.Format("Error occured in accessing the path {0}", "Error in Discovering Domain.."), ex);

            }
            return DomainsInfo;
        }
    }
}
