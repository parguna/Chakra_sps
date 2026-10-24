
using System;
namespace DI.Chakra.Model
{
    public class PasswordSyncConfigGridData
    {
        private bool _allowSync;
        private int _configId;
        private int _configDetailId;

        public long EndPointSystemTypeId { get; set; }
        public string EndpointSystemTypeName { get; set; }
        public long EndPointSystemId { get; set; }
        public string HostName { get; set; }
        public string EndPointSystemDescription { get; set; }

        public Nullable<bool> AllowSync {
            get
            {
                return _allowSync;
            }
            set
            {
                _allowSync = (value.HasValue && value.Value) ? true : false;
            }
        }

        public Nullable<int> ConfigId
        {
            get
            {
                return _configId;
            }
            set
            {
                _configId = value == null ? 0 : value.Value;
            }
        }

        public Nullable<int> ConfigDetailId
        {
            get
            {
                return _configDetailId;
            }
            set
            {
                _configDetailId = value == null ? 0 : value.Value;
            }
        }

    }
}
