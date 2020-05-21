using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp
{
    [Serializable()]
    public class ConnectionInfo : ISerializable
    {
        public enum ConnectionStatus
        {
            Connected, Kicked, Disconnected, Unknown
        }
        private static uint _numberOfConnections = 1;
        public uint ID { get; set; }
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Unknown;
        public ConnectionInfo()
        {
            ID = _numberOfConnections++;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ID), this.ID);
            info.AddValue(nameof(Status), this.Status);
        }

        public ConnectionInfo(SerializationInfo info, StreamingContext context)
        {
            ID = (uint)info.GetValue(nameof(ID), typeof(uint));
            Status = (ConnectionStatus)info.GetValue(nameof(Status), typeof(ConnectionStatus));
        }

    }
}
