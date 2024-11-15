using aurga.Common;

namespace aurga.Model
{
    public class DeviceStatus
    {
        public required string DUID { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
        public uint Capability { get; set; }
        public long Firmware { get; set; }
        public uint Version { get; set; }
        public byte Model { get; set; }

        public byte[] LocalAddr { get; set; } = new byte[28];
        public byte[] LocalAddr6 { get; set; } = new byte[28];

        public byte RemoteAddr6NatType { get; set; }
        public byte RemoteAddrNatType { get; set; }
        public byte[] RemoteAddr { get; set; } = new byte[28];
        public byte[] RemoteAddr6 { get; set; } = new byte[28];

        // DID  Name, Alive, Capabilities, Firmware, Version, Model Local, NAT Type Remote
        public byte[] Data { get; set; } = new byte[8 + 32 + 8 + 4 + 8 + 4 + 1 + 28 + 28 + 2 + 28 + 28];
        
        #region Non-serializable
        public byte[] RequestConnectionInfo { get; set; }
        public byte[] RequestWOLInfo { get; set; }

        public string Nonce { get; set; }

        #endregion

        //                                          
        public void Update()
        {
            long ticks = LastActive.ToUniversalTime().Ticks;
            var name = this.DeviceName;
            if (name == null) name = string.Empty;

            // DID
            var arr = Util.Hex2Bytes(this.DUID);
            Array.Copy(arr, Data, arr.Length);

            // Name
            arr = System.Text.Encoding.UTF8.GetBytes(name);
            Array.Copy(arr, 0, Data, 8, Math.Min(32, arr.Length));
            Data[8 + Math.Min(32, arr.Length)] = 0;

            // Alive
            arr = BitConverter.GetBytes(ticks);
            Array.Copy(arr, 0, Data, 40, arr.Length);  // Alive timestamp

            // Capability
            arr = BitConverter.GetBytes(Capability);
            Array.Copy(arr, 0, Data, 48, arr.Length);  // Alive timestamp

            // Firmware
            arr = BitConverter.GetBytes(this.Firmware);
            Array.Copy(arr, 0, Data, 52, arr.Length);

            // Version
            arr = BitConverter.GetBytes(this.Version);
            Array.Copy(arr, 0, Data, 60, arr.Length);

            // Model
            Data[64] = Model;

            Array.Copy(this.LocalAddr, 0, Data, 65, this.LocalAddr.Length);
            Array.Copy(this.LocalAddr6, 0, Data, 93, this.LocalAddr6.Length);
            
            Data[121] = this.RemoteAddrNatType;
            Data[122] = this.RemoteAddr6NatType;

            Array.Copy(this.RemoteAddr, 0, Data, 123, 28);
            Array.Copy(this.RemoteAddr6, 0, Data, 151, 28);
        }
    }
}
