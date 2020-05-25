using System;
using System.Collections.Generic;
using System.Reflection;

namespace ChatApp.Serialization
{
    [System.AttributeUsage(System.AttributeTargets.Class,
                            AllowMultiple = true)  // Multiuse attribute.  
     ]
    class AutoSerializableAttribute : Attribute
    {
        public List<string> Properties = new List<string>();
        public AutoSerializableAttribute(Type classType)
        {
            var p = classType.GetProperties();
            foreach (var item in p)
            {
                Type t = item.PropertyType;

                if (t.IsSerializable && item.CanWrite && item.CanRead && item.MemberType == MemberTypes.Property)
                    Properties.Add(item.Name);
            }
        }
    }
}
