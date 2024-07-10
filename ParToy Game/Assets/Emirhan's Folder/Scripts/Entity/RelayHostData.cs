using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Emirhan_s_Folder.Scripts.Entity
{
    public struct RelayHostData
    {
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid AllocationID;
        public byte[] AllocationIdBytes;
        public byte[] ConnectionData;
        public byte[] Key;

    }
}
