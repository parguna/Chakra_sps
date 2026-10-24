using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.DomainModel
{
    public class ADSNestedGroup : DomainModelBase<ADSNestedGroup, EntityModel.ADSNestedGroup>
    {
        public long EndPointSystemId { get; set; }
        public string Supergroup { get; set; }
        public string Subgroup { get; set; }
        public int Depth { get; set; }
        public EndPoint EndPointSystem { get; set; }

        private static List<string> ChildGroupHistory;


        public static List<ADSNestedGroup> GetAllByEndpointSystemId(long endpointSystemId)
        {
            return  ADSNestedGroup.GetAll().Where(item => item.EndPointSystemId == endpointSystemId).ToList();
        }
        public static List<ADSGroupInfo> GetMembers(long endpointSystemId, long grpId)
        {
            List<ADSGroupInfo> members = new List<ADSGroupInfo>();
            
            var memberGroupIds = ADSNestedGroup.GetAll().Where(item => item.Supergroup == grpId.ToString() && item.EndPointSystemId == endpointSystemId).Select(a => Convert.ToInt64(a.Subgroup)).ToList();
                
            if (memberGroupIds.Count > 0)
            {
                members = ADSGroupInfo.GetByIds(memberGroupIds, endpointSystemId);
            }
            
            return members;
        }

        public static List<ADSGroupInfo> GetMembersIncludingUsers(long endpointSystemId, long grpId)
        {
            List<ADSGroupInfo> members = new List<ADSGroupInfo>();

            var memberGroupIds = ADSNestedGroup.GetAll().Where(item => item.Supergroup == grpId.ToString() && item.EndPointSystemId == endpointSystemId).Select(a => Convert.ToInt64(a.Subgroup)).ToList();

            if (memberGroupIds.Count > 0)
            {
                members = ADSGroupInfo.GetByIdsIncludingUsers(memberGroupIds, endpointSystemId);
            }

            return members;
        }

        public static List<ADSGroupInfo> GetMemberOfs(long endpointSystemId, long grpId)
        {
            List<ADSGroupInfo> memberOfs = new List<ADSGroupInfo>();
            
            var memberOfGroupIds = ADSNestedGroup.GetAll().Where(item => item.Subgroup == grpId.ToString() && item.EndPointSystemId == endpointSystemId).Select(a => Convert.ToInt64(a.Supergroup)).ToList();

            if (memberOfGroupIds.Count > 0)
            {
                memberOfs = ADSGroupInfo.GetByIds(memberOfGroupIds, endpointSystemId);
            }
            
            return memberOfs;
        }

        public static void SaveNestedGroups(IList<ADSGroupInfo> list, string currentUser, long endpointId)
        {
            try
            {

                using (var dataAccessTransaction = new DataAccessTransaction(currentUser))
                {

                    using (DataAccessTransaction dataNesstedGrpTransaction = new DataAccessTransaction())
                    {
                        new ADSGroupInfoRepository(dataNesstedGrpTransaction).ExecuteQuery(string.Format("DELETE FROM DI_ADS_NestedGroup WHERE EndpointSystemId = {0}", endpointId));
                        dataNesstedGrpTransaction.Commit();
                    }

                    StringBuilder bldrMemberOf = new StringBuilder();
                    int depth = 1;

                    IList<ADSGroupInfo> listExistingADgroups = new List<ADSGroupInfo>();
                    ADSGroupInfo adexistingGroups = null;
                    List<ADSNestedGroup> _objlist = new List<ADSNestedGroup>();
                    listExistingADgroups = ADSGroupInfo.GetAllByEndpointSystemId(endpointId);
                    foreach (ADSGroupInfo grp in list)
                    {
                        bldrMemberOf = new StringBuilder();

                        adexistingGroups = listExistingADgroups.Where(t => t.sAMAccountName == grp.sAMAccountName).FirstOrDefault();
                        if (adexistingGroups != null)
                        {
                            foreach (var item in grp.memberOf)
                            {
                                var grpDist = ADSGroupInfo.GetByDistinguishedNameAndEndpointId(item.ToString(), grp.EndPointSystemId);

                                if (grpDist == null)
                                    continue;

                                bldrMemberOf.AppendFormat("{0},", grpDist.Id);
                            }

                            string strMemOf = string.Empty;

                            if (bldrMemberOf.Length > 0)
                                strMemOf = bldrMemberOf.ToString().Remove(bldrMemberOf.Length - 1);

                            grp.Id = adexistingGroups.Id;
                            grp.UserDefinedDesc = strMemOf;
                            if (!string.IsNullOrEmpty(strMemOf))
                            {
                                List<string> strlist = strMemOf.Split(',').ToList<string>();
                                foreach (string str in strlist)
                                {
                                    ADSNestedGroup _obj = new ADSNestedGroup();
                                    _obj.Supergroup = (str);
                                    _obj.Subgroup = grp.Id.ToString();
                                    _obj.EndPointSystemId = grp.EndPointSystemId;
                                    ADSGroupInfo parentgrp = listExistingADgroups.FirstOrDefault(item => item.Id == Convert.ToInt16(_obj.Supergroup));
                                    if (list.Any(item=>item.distinguishedName== parentgrp.distinguishedName))
                                    {
                                        var child = list.FirstOrDefault(item => item.name == parentgrp.name);
                                        ADSGroupInfo adsgrp = list.FirstOrDefault(item => item.distinguishedName == parentgrp.distinguishedName);
                                        _obj.Depth = FindDepth(adsgrp, list, 2, grp, endpointId);
                                        _obj.Save(currentUser);
                                    }
                                    else
                                    {
                                        _obj.Depth = 1;
                                        _obj.Save(currentUser);
                                    }

                                }

                            }


                        }


                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static int FindDepth(ADSGroupInfo childgrp, IList<ADSGroupInfo> childgrplist, int depth,ADSGroupInfo superchildgroup, long endpointSystemId)
        {

            if (depth == 2)
            {

                ChildGroupHistory = new List<string>();

                ChildGroupHistory.Add(superchildgroup.distinguishedName);
            }
            ChildGroupHistory.Add(childgrp.distinguishedName);
            int detaileddepth = depth;
            if (childgrp.memberOf.Count() > 0)
            {
                for (int i = 0; i < childgrp.memberOf.Count(); i++)
                {
                    ADSGroupInfo ads = ADSGroupInfo.GetByDistinguishedNameAndEndpointId(childgrp.memberOf[i].ToString(), endpointSystemId);
                    if (ads != null && childgrplist.Any(item=>item.distinguishedName == ads.distinguishedName) )
                    {
                        var subchildgrp = new ADSGroupInfo();
                        subchildgrp = childgrplist.FirstOrDefault(item => item.name == ads.name);
                        //if(subchildgrp.distinguishedName == childgrp.distinguishedName || subchildgrp.distinguishedName == superchildgroup.distinguishedName)
                        //{
                        //    continue;
                        //}
                        if(ChildGroupHistory.Contains( subchildgrp.distinguishedName))
                        {
                            continue;
                        }
                        detaileddepth = detaileddepth + 1;
                        detaileddepth = FindDepth(subchildgrp, childgrplist, detaileddepth, superchildgroup, endpointSystemId);
                    }

                }

            }
            return detaileddepth;
        }

    }
}
