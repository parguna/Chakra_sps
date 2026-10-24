using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
//using DI.Chakra.Infrastructure.;
using DI.Chakra.Facade;
using DI.Chakra.Model;
using System.Collections;

namespace Application
{
    public class QueryingService : IQueryingService
    {
        public AttestationFacade AttestationFacade { get; set; }
        public EndPointFacade EndPointFacade { get; set; }
        public ActiveDirectoryFacade ActiveDircotryFacade { get; set; }
        public UserFacade UserFacade { get; set; }
        public GroupFacade GroupFacade { get; set; }

        public QueryingService()
        {
            AttestationFacade = new AttestationFacade();
            EndPointFacade = new EndPointFacade();
            ActiveDircotryFacade = new ActiveDirectoryFacade();
            UserFacade = new UserFacade();
            GroupFacade = new GroupFacade();
        }

        #region IQueryingService Members

        #region Attestation


        public Attestation GetAttestation(int _id)
        {
            //try
            //{
            //    return AttestationFacade.GetAttestation(_id);
            //}
            //catch (Exception)
            //{

            //    throw;


            return null;
        }

        public Attestation GetAttestation(string _name)
        {
            //try
            //{
            //    return AttestationFacade.GetAttestation(_name);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            return null;
        }

        //public AttestationType GetAttestationType(int _id)
        //{
        //    try
        //    {
        //        return AttestationFacade.GetAttestationType(_id);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //public AttestationType GetAttestationType(string _name)
        //{
        //    try
        //    {
        //        return AttestationFacade.GetAttestationType(_name);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public IList<Attestation> GetAllAttestations()
        {

            //Logger.WriteLog(LogLevel.INFO, "Facade invoked");

            //try
            //{
            //    return AttestationFacade.GetAllAttestations();
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            return null;
        }

        //public bool AddAttestation(Attestation _attestation)
        //{

        //    try
        //    {
        //        AttestationFacade.AddAttestation(_attestation);

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        ///<summary>
        /// Author        : I.Deepthi
        /// Created Date  : 28/01/2011
        /// Modified Date :
        /// Description   :check  Version
        ///</summary>
        #region EndPoint

        public int AddEndPoint(DI.Chakra.Model.EndPoint _endpoint)
        {
            try
            {
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CheckEndPointSystemDuplicate(int EndPointSystemTypeId, string Hostname, string Ipaddress)
        {
            return 0;
        }

        #endregion

        #region EndPoint Active Dirctory
        public bool AddADSConfiguration(ADSConfiguration _ADSConfig)
        {
            try
            {
                ActiveDircotryFacade.AddADSConfiguration(_ADSConfig);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

     
        public IList<ADSUserInfo> GetAllADSUserInfo()
        {
            try
            {
                return ActiveDircotryFacade.GetAllADSUsers();


            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        #region User


        public IList<DI.Chakra.DomainModel.Tenant> GetAllTenant()
        {
            try
            {
                return DI.Chakra.DomainModel.Tenant.GetAll();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


      
     
     
        #endregion





        public void UpdateEndPointSystem(DI.Chakra.Model.EndPoint _endpoint)
        {
            throw new NotImplementedException();
        }

        public IList<AttributeMappingColumns> GetAttributeMappingColumns(string EndPointSystemType)
        {
            throw new NotImplementedException();
        }

        public void PopulateIACM(int EndpointSystemId, bool PopulateIacm, string LastModifiedBy)
        {
            throw new NotImplementedException();
        }
    }
}
