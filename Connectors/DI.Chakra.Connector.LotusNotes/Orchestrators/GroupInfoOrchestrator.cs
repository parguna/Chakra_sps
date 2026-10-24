using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using Domino;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using DI.Chakra.DomainModel;
namespace DI.Chakra.Connector.LotusNotes.Orchestrators
{
   public class GroupInfoOrchestrator
    {
        

        private const string PROCESSNAME = "DiscoveryProcess";

        public GroupInfoOrchestrator()
        {
        }
        public long EndpointSystemId { get; set; }
        
        public int HistoryId { get; set; }
        public LotusGroupInfo PopulateGroup(NotesViewEntry viewEntry)
        {
            IList<LotusGroupInfo > list = new List<LotusGroupInfo >();

            LotusGroupInfo  GroupsInfo = new LotusGroupInfo();

            try
            {

                GroupsInfo.EndPointSystemId = EndpointSystemId;
                GroupsInfo.Category = string.Empty;
                GroupsInfo.Version = Convert.ToInt16(VersionNo.Available);

                NotesDocument document = viewEntry.Document;
                object documentItems = document.Items;
                Array itemArray1 = (System.Array)documentItems;
                for (int itemCount = 0; itemCount < itemArray1.Length; itemCount++)
                {
                    NotesItem notesItem =
                    (Domino.NotesItem)itemArray1.GetValue(itemCount);

                    if (notesItem.Text != null)
                    {


                        NotesItem searchedNotesItem = (Domino.NotesItem)itemArray1.GetValue(itemCount);


                        if (searchedNotesItem.Name == "ListName")
                        {
                            GroupsInfo.Group = searchedNotesItem.Text.Trim();
                        }
                        if (searchedNotesItem.Name == "ListDescription")
                        {
                            GroupsInfo.Description = searchedNotesItem.Text.Trim();
                        }
                        if (searchedNotesItem.Name == "ListOwner")
                        {
                            GroupsInfo.Owner = searchedNotesItem.Text.Trim();
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                //NOTE: Dont throw the exception. Keep running all objects
                DILog.ProvisioningLogger.WriteLog(null, LogLevelType.Error, string.Format("Error occured in accessing the path {0}", "Error in Discovering Groups.."), ex);

            }
            return GroupsInfo;
        }
    }
}
