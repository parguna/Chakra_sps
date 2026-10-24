using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Service.MiscProcessor 
{
    public class PUBService 
    {
         public PUBService_Process2 Process2 { get; private set; }
         public PUBService_Process3 Process3 { get; private set; }

         public PUBService_Process1 Process1 { get; private set; }

        bool _disposed = false;
        public PUBService()
        {
            Process1 = new PUBService_Process1();
            Process2 = new PUBService_Process2();
            Process3 = new PUBService_Process3();
        }

        public void  Start()
        {
            Process1.Start();
            Process2.Start();
            Process3.Start();
        }

        public void Stop()
        {
            Process1.Stop();
            Process2.Stop();
            Process3.Stop();
        }

        #region IDisposable Implementation
        public void Dispose() => Dispose(true);


        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Process2 = null;
                Process3 = null;
            }

            _disposed = true;
        }
        #endregion

    }
}
