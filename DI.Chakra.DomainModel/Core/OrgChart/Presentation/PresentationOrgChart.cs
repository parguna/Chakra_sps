using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationOrgChart : DomainModelBase<PresentationOrgChart, EntityModel.Presentation.PresentationOrgChart>
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public byte[] Image { get; set; }
        public Nullable<long> ParentId { get; set; }
        public OrgChartNodeType NodeType { get; set; }
        public long OrgId { get; set; }
        public string OrgHeadCaption { get; set; }

        public static List<PresentationOrgChart> GetOrgChartNodes(long usrId, long parentOrgId)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                int depth = Convert.ToInt32( ConfigurationMaster.GetByConfigKey("OrgChartDepthSupported", Product.IACM).ConfigValue);
                int level = 1;
                List<PresentationOrgChart> resultNodes = new List<PresentationOrgChart>();
                while (level <= depth + 1)
                {
                    if (level == 1)
                    {
                        PresentationOrgChart headNode = ORMap(new PresentationOrgChartRepository(dataAccessTransaction).GetFirstLevelHeadNode(usrId, parentOrgId));
                        resultNodes.Add(headNode);
                        level++;
                    }
                    else
                    {
                        List<PresentationOrgChart> nodes = ORMapList(new PresentationOrgChartRepository(dataAccessTransaction).
                                                            GetOrgMembers(resultNodes[0].OrgId, resultNodes[0].Id,resultNodes[0].UserId));
                        resultNodes = resultNodes.Union(nodes).ToList();
                        level++;
                        if (level == depth + 1)
                            return resultNodes;

                        for (level = 3; level <= depth + 1; level++)
                        {
                            if (nodes.Any(n => n.NodeType == OrgChartNodeType.OrgHeadNode))
                            {
                                List<PresentationOrgChart> levelnodes = new List<PresentationOrgChart>();
                                foreach (PresentationOrgChart node in nodes)
                                {
                                    if (node.NodeType == OrgChartNodeType.OrgHeadNode)
                                    {
                                        List<PresentationOrgChart> childnodes = ORMapList(new PresentationOrgChartRepository(dataAccessTransaction).
                                                                                GetOrgMembers(node.OrgId, node.Id,node.UserId));
                                        levelnodes = levelnodes.Union(childnodes).ToList();
                                        childnodes.Clear();
                                    }
                                }
                                resultNodes = resultNodes.Union(levelnodes).ToList();
                                nodes = levelnodes;
                            }
                        }
                    }
                }
                return resultNodes;
            }
        }

        public static PresentationOrgChart ORMap(EntityModel.Presentation.PresentationOrgChart presentationOrgChartEntityModel)
        {
            if (presentationOrgChartEntityModel == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationOrgChart, PresentationOrgChart>(presentationOrgChartEntityModel);
        }

        public static List<PresentationOrgChart> ORMapList(IList<EntityModel.Presentation.PresentationOrgChart> presentationOrgChartEntityModel)
        {
            return presentationOrgChartEntityModel.Select(ORMap).ToList();
        }
    }
}
