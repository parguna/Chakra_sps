using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Repository
{
    public class PresentationEndpointProvisioningFieldRepository : RepositoryBase<EndPoint>
    {
        public PresentationEndpointProvisioningFieldRepository(DataAccessTransaction transaction) : base(transaction) { }

        public List<PresentationEndpointProvisioningField> GetEndpointProvisiongField()
        {
            List<PresentationEndpointProvisioningField> EndPointProvFieldlist = new List<PresentationEndpointProvisioningField>();
            List<PresentationEndpointProvisioningField> EndPointSystemTypeProvList = new List<PresentationEndpointProvisioningField>();
            List<PresentationEndpointProvisioningField> EndPointSystemProvList = new List<PresentationEndpointProvisioningField>();

            EndPointSystemTypeProvList = (from epSys in CurrentContext.EndpointSystemTypes
                                          join provField in CurrentContext.NProvisioningFields on epSys.Id equals provField.EndPointSystemTypeId
                                          orderby epSys.Id ascending
                                          select new PresentationEndpointProvisioningField
                                          {
                                              Id = epSys.Id,
                                              ParentRowID = 0,
                                              Name = epSys.DisplayName,
                                              Description = epSys.Description

                                          }).ToList();
            var groupedEndpointSystems = EndPointSystemTypeProvList.GroupBy(item => item.Id);
            var shortestEndpointSystems = groupedEndpointSystems.Select(grp => grp.OrderBy(item => item.Id).First());
            EndPointSystemTypeProvList = shortestEndpointSystems.ToList();

            long rowId = 1; long parentRowId = 1;
            foreach (PresentationEndpointProvisioningField EndpointSysTypeList in EndPointSystemTypeProvList)
            {
                PresentationEndpointProvisioningField EndpointSystemType = new PresentationEndpointProvisioningField();
                EndpointSystemType.Id = EndpointSysTypeList.Id;
                EndpointSystemType.RowID = rowId;
                EndpointSystemType.ParentRowID = null;
                EndpointSystemType.EndPointSystemTypeId = EndpointSysTypeList.Id;
                EndpointSystemType.Name = EndpointSysTypeList.Name;
                EndpointSystemType.Description = EndpointSysTypeList.Description;
                if (EndpointSystemType != null)
                {
                    EndPointProvFieldlist.Add(EndpointSystemType);
                    parentRowId = rowId;
                    List<PresentationEndpointProvisioningField> EndPointSystemList = getEndPointSystemByTypeId(EndpointSystemType.EndPointSystemTypeId);
                    foreach (PresentationEndpointProvisioningField endPointSystem in EndPointSystemList)
                    {
                        rowId++;
                        PresentationEndpointProvisioningField EndpointSystem = new PresentationEndpointProvisioningField();
                        EndpointSystem.Id = endPointSystem.Id;
                        EndpointSystem.RowID = rowId;
                        EndpointSystem.ParentRowID = parentRowId;
                        EndpointSystem.EndPointSystemId = endPointSystem.EndPointSystemId;
                        EndpointSystem.EndPointSystemTypeId = endPointSystem.Id;
                        EndpointSystem.Name = endPointSystem.Name;
                        EndpointSystem.Description = endPointSystem.Description;
                        EndpointSystem.EndPointHasValues = (((from ep in CurrentContext.NProvisioningFields
                                                              where ep.EndPointSystemId == endPointSystem.EndPointSystemId
                                                              select ep).Count() > 0) ? true : false);
                        EndPointProvFieldlist.Add(EndpointSystem);
                    }
                    rowId++;
                }
            }

            return EndPointProvFieldlist;
        }

        public List<PresentationEndpointProvisioningField> getEndPointSystemByTypeId(long endPointSystemTypeId)
        {
            List<PresentationEndpointProvisioningField> EndPointSystemProvFieldlist = new List<PresentationEndpointProvisioningField>();
            EndPointSystemProvFieldlist = (from ep in CurrentContext.EndPoint
                                           join epsys in CurrentContext.EndpointSystemTypes on ep.EndpointSystemTypeId equals epsys.Id
                                           where ep.EndpointSystemTypeId == endPointSystemTypeId && ep.IsTrustedSource == false
                                           orderby ep.EndpointSystemTypeId ascending
                                           select new PresentationEndpointProvisioningField
                                           {
                                               Id = ep.Id,
                                               EndPointSystemId = ep.Id,
                                               EndPointSystemTypeId = (long)ep.EndpointSystemTypeId,
                                               Name = ep.DisplayName,
                                               Description = ep.Description
                                           }).ToList();

            return EndPointSystemProvFieldlist;
        }
    }
}
