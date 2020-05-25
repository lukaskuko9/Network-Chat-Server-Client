using System;

namespace ChatApp.Serialization
{
    public class AutoSerializableAttributeNotFoundException : Exception
    {
        static string msg = $"AutoSerializable Attribute not found in class ";
        public AutoSerializableAttributeNotFoundException(Type type) : base(msg + type)
        {
            
        }
    }
}
