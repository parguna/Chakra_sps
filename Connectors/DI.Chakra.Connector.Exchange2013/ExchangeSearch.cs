//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.DirectoryServices;

//namespace DI.Chakra.Connector
//{
//    public class ExSearch
//    {
//        #region Search Methods
//        /// <summary>
//        /// This will return a DirectoryEntry Object if the User Exists
//        /// </summary>
//        /// <param name="sUserName">Username to Get</param>
//        /// <returns></returns>
//        public DirectoryEntry GetUser(string sUserName)
//        {
//            //Create an Instance of the DirectoryEntry
//            oDE = GetDirectoryObject();

//            //Create Instance fo the Direcory Searcher
//            oDS = new DirectorySearcher();

//            oDS.SearchRoot = oDE;
//            //Set the Search Filter
//            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
//            oDS.SearchScope = SearchScope.Subtree;
//            oDS.PageSize = 10000;

//            //Find the First Instance
//            SearchResult oResults = oDS.FindOne();

//            //If found then Return Directory Object, otherwise return Null
//            if (oResults != null)
//            {
//                oDE = new DirectoryEntry(oResults.Path, sExUser, sExPassword, AuthenticationTypes.Secure);
//                return oDE;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        /// <summary>
//        /// Override method which will perfrom query based on combination of Username and Password
//        /// </summary>
//        /// <param name="sUserName">Username to Get</param>
//        /// <param name="sPassword">Password for the Username to Get</param>
//        /// <returns></returns>
//        public DirectoryEntry GetUser(string sUserName, string sPassword)
//        {
//            //Create an Instance of the DirectoryEntry
//            oDE = GetDirectoryObject(sUserName, sPassword);

//            //Create Instance fo the Direcory Searcher
//            oDS = new DirectorySearcher();
//            oDS.SearchRoot = oDE;

//            //Set the Search Filter
//            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
//            oDS.SearchScope = SearchScope.Subtree;
//            oDS.PageSize = 10000;

//            //Find the First Instance
//            SearchResult oResults = oDS.FindOne();

//            //If a Match is Found, Return Directory Object, Otherwise return Null
//            if (oResults != null)
//            {
//                oDE = new DirectoryEntry(oResults.Path, sExUser, sExPassword, AuthenticationTypes.Secure);
//                return oDE;
//            }
//            else
//            {
//                return null;
//            }

//        }

//        /// <summary>
//        /// This will take a Username and Query the Ex for the User.  
//        /// When found it will Transform the Results from the Property Collection into a Dataset.
//        /// </summary>
//        /// <param name="sUserName">Username to Get</param>
//        /// <returns>Users Dataset</returns>
//        public DataSet GetUserDataSet(string sUserName)
//        {
//            oDE = GetDirectoryObject();

//            //Create Instance fo the Direcory Searcher
//            oDS = new DirectorySearcher();
//            oDS.SearchRoot = oDE;

//            //Set the Search Filter
//            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
//            oDS.SearchScope = SearchScope.Subtree;
//            oDS.PageSize = 10000;

//            //Find the First Instance
//            SearchResult oResults = oDS.FindOne();

//            //Create Empty User Dataset
//            oDsUser = CreateUserDataSet();

//            //If Record is not Null, Then Populate DataSet
//            if (oResults != null)
//            {
//                oNewCustomersRow = oDsUser.Tables["User"].NewRow();
//                oNewCustomersRow = PopulateUserDataSet(oResults, oDsUser.Tables["User"]);

//                oDsUser.Tables["User"].Rows.Add(oNewCustomersRow);
//            }
//            oDE.Close();

//            return oDsUser;

//        }

//        /// <summary>
//        /// This Method will Return a Dataset of User Details Based on Criteria passed to the Query
//        /// The criteria is in the LDAP format
//        /// e.g.
//        /// (sAMAccountName='Test Account Name')(sn='Test Surname')
//        /// </summary>
//        /// <param name="sCriteria">Criteria to use for Searching</param>
//        /// <returns>Users Dataset</returns>
//        public DataSet GetUsersDataSet(string sCriteria)
//        {
//            oDE = GetDirectoryObject();

//            //Create Instance fo the Direcory Searcher
//            oDS = new DirectorySearcher();
//            oDS.SearchRoot = oDE;

//            //Set the Search Filter
//            oDS.Filter = "(&(objectClass=user)(objectCategory=person)(" + sCriteria + "))";
//            oDS.SearchScope = SearchScope.Subtree;
//            oDS.PageSize = 10000;

//            //Find the First Instance
//            oResults = oDS.FindAll();

//            //Create Empty User Dataset
//            oDsUser = CreateUserDataSet();

//            //If Record is not Null, Then Populate DataSet
//            try
//            {
//                if (oResults.Count > 0)
//                {
//                    foreach (SearchResult oResult in oResults)
//                    {
//                        oDsUser.Tables["User"].Rows.Add(PopulateUserDataSet(oResult, oDsUser.Tables["User"]));
//                    }
//                }
//            }
//            catch { }

//            oDE.Close();
//            return oDsUser;

//        }


//    }
//}
