using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.App
{
    public class FilterApp
    {
        public float uniqId { get; set; }
        public long Id { get; set; }
        public float? P_Id { get; set; }
        public string Name { get; set; }
    }

    public partial class FilterUC : DI.Chakra.Web.UIBaseUC
    {
        //public IList<long> ApplicationTypes
        //{
        //    get
        //    {
        //        IList<long> list = new List<long>();
        //        foreach (RadTreeNode node in treeApp.GetAllNodes())
        //        {
        //            if (node.Checked)
        //            {
        //                if (node.Text == "Applications")
        //                {
        //                    list = treeApp.GetAllNodes().Select(c => Convert.ToInt64(c.Value)).ToList();
        //                    return list;
        //                }
        //                list.Add(Convert.ToInt64(node.Value));
        //            }
        //        }
        //        return list;
        //    }
        //}

        public IList<long> EntitlementTypes
        {
            get
            {
                IList<long> list = new List<long>();
                foreach (RadTreeNode node in treeEnt.GetAllNodes())
                {
                    if (node.Checked)
                    {
                        list = treeEnt.GetAllNodes().Where(c => c.Level == 1 && c.Checked).Select(c => Convert.ToInt64(c.Value)).ToList();
                        return list;
                    }
                }
                return list;
            }
        }

        public IList<long> EntitlementSubTypes
        {
            get
            {
                IList<long> list = new List<long>();
                foreach (RadTreeNode node in treeEnt.GetAllNodes())
                {
                    if (node.Checked)
                    {
                        list = treeEnt.GetAllNodes().Where(c => c.Level == 2 && c.Checked).Select(c => Convert.ToInt64(c.Value)).ToList();
                        return list;
                    }
                }
                return list;
            }
        }

        public bool SetEndpointTreeData
        {
            set
            {
                bool isAddEnt = Session["IsAddEntitlement"] != null ? Convert.ToBoolean(Session["IsAddEntitlement"]) : true;
                IList<FilterApp> listApps = new List<FilterApp>()
            {
                new FilterApp(){ Id=0, P_Id=null,Name="Applications"},
            };
                IList<FilterApp> listEnts = new List<FilterApp>()
            {
                new FilterApp(){ Id=0, P_Id=null,Name="SubSystems"},
            };

                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("IsAddEntitlement", isAddEnt.ToString());

                var endpoints = DomainApiClient<Dto.Presentation.PresentationEndPointForFilter>.GetByAction<List<Dto.Presentation.PresentationEndPointForFilter>>
                    (DomainApiAction.Endpoint.PresentationEndpointForFilter.GetRequestableEndpoints, parameter);

                endpoints.OrderBy(lq => lq.DisplayName).ToList().ForEach(lq =>
                {
                    listApps.Add(new FilterApp()
                    {
                        uniqId = (float)(lq.Id * 9.8),
                        Id = lq.Id,
                        P_Id = 0,
                        Name = lq.DisplayName
                    });
                    listEnts.Add(new FilterApp()
                    {
                        uniqId = (float)(lq.Id * 9.8),
                        Id = lq.Id,
                        P_Id = 0,
                        Name = lq.DisplayName
                    });
                    if (lq.HasEntitlementTypes)
                    {
                        foreach (var item in lq.EntitlementTypes)
                        {
                            listEnts.Add(new FilterApp()
                            {
                                uniqId = 0,
                                Id = (int)item,
                                P_Id = (float)(lq.Id * 9.8),
                                Name = item.ToString()
                            });
                        }

                    }
                });
                //treeApp.DataSource = listApps;
                //treeApp.DataBind();
                treeEnt.DataSource = listEnts;
                treeEnt.DataBind();

                if (listEnts != null && listEnts.Count > 0)
                    treeEnt.Nodes[0].Expanded = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                Initialize();
        }

        public void Initialize()
        {
            SetEndpointTreeData = Session["IsAddEntitlement"] != null ? Convert.ToBoolean(Session["IsAddEntitlement"]) : true;
        }

        protected void treeEnt_NodeCheck(object sender, RadTreeNodeEventArgs e)
        {
            RadTreeNode oldNode = this.treeEnt.CheckedNodes.First();
            RadTreeNode newNode = e.Node;

            if (newNode.Parent != null)
            {
                foreach (RadTreeNode node in newNode.ParentNode.Nodes)
                {
                    if (node.Checked && node != newNode)
                    {
                        node.Checked = false;
                    }
                }
            }
        }
    }
}