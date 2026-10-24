using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
//using DI.Chakra.Infrastructure.;
using System.Collections;
using DI.Chakra.Model;
using DI.Chakra.DomainModel;


namespace Application
{
    public interface IQueryingService : IApplicationService
    {
        DI.Chakra.Model.Attestation GetAttestation(int _id);
        DI.Chakra.Model.Attestation GetAttestation(string _name);
        IList<DI.Chakra.Model.Attestation> GetAllAttestations();

        #region EndPoint

        int AddEndPoint(DI.Chakra.Model.EndPoint _EndPoint);

        void UpdateEndPointSystem(DI.Chakra.Model.EndPoint _endpoint);



        int CheckEndPointSystemDuplicate(int EndPointSystemTypeId, string Hostname, string Ipaddress);

        void PopulateIACM(int EndpointSystemId, bool PopulateIacm, string LastModifiedBy);


        #endregion

        #region Active Directory


        IList<DI.Chakra.Model.ADSUserInfo> GetAllADSUserInfo();

        #endregion


        #region User


        IList<DI.Chakra.DomainModel.Tenant> GetAllTenant();

        #endregion
     



    }
}
