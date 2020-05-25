using ChatApp.Chat;
using ChatApp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Chat
{
    [Serializable()]
    [AutoSerializableAttribute(typeof(ConnectionInfo))]
    public class ConnectionInfo : AutoSerializable
    {
        static uint _totalConnections = 0;
        public uint ID { get; internal set; }
        public enum ConnectionStatus
        {
            Connected, Kicked, Disconnected, Unknown
        }
        public ConnectionStatus Status { get; set; } = ConnectionStatus.Unknown;

        public ConnectionInfo()
        {
            this.ID = ++_totalConnections;
        }
        internal ConnectionInfo(SerializationInfo info, StreamingContext context) : base(info,context)
        {
        }

    }
}
