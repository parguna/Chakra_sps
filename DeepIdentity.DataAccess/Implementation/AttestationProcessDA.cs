using DeepIdentity.Model;
using System;
using System.Data.Entity;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class AttestationProcessDA
    {
        public AttestationProcess GetAttestationProcessById(int _processId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationProcesses.FirstOrDefault(item => item.AttestationProcessId == _processId);
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddAttestationProcessHistory(AttestationProcessHistory history)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.AttestationProcessHistories.Add(history);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateAttestationProcess(AttestationProcess _attestationProcess)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    _context.AttestationProcesses.Attach(_attestationProcess);
                    _context.Entry(_attestationProcess).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public void UpdateProcess(Process _Process)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    Process objProcess = new Process();
                    objProcess = _context.Process.Find(_Process.ProcessId);
                    _context.Entry(objProcess).CurrentValues.SetValues(_Process);
                    _context.SaveChanges();

                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        public AttestationProcess GetAttestation(int _processId)
        {

            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.AttestationProcesses.FirstOrDefault(item => item.AttestationProcessId == _processId);
                }
            }
            catch
            {
                throw;
            }
        }

        #region Added by Yasila.H on 02.11.2015 for V5 Changes
        public AttestationProcess GetPrcoessByAttestationId(int attestationId)
        {

            try
            {
                using (IACMDBContext context = new IACMDBContext())
                {
                    return context.AttestationProcesses.FirstOrDefault(item => item.AttestationId == attestationId);
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

    }
}

