using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Serialization
{
    public abstract class AutoSerializable : ISerializable
    {
        List<string> SerializablePropertyNames;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (var item in SerializablePropertyNames)
            {
                try
                {
                    info.AddValue(item, this.GetType().GetProperty(item).GetValue(this));
                }
                catch(Exception e)
                {
                    Logger.Logger.Instance.WriteLine(e.Message);
                }
            }
        }

        public AutoSerializable(SerializationInfo info, StreamingContext context)
        {
            if (SerializablePropertyNames == null)
            {
                var att = (this.GetType()).GetCustomAttribute<AutoSerializableAttribute>();
                if (att == null)
                    throw new AutoSerializableAttributeNotFoundException(this.GetType());

                this.SerializablePropertyNames = att.Properties;
            }

            foreach (var item in SerializablePropertyNames)
            {
                try
                {
                    var property = this.GetType().GetRuntimeProperty(item);
                    var value = info.GetValue(item, property.PropertyType);
                    property.SetValue(this, value);
                }
                catch (Exception e)
                {   
                    Logger.Logger.Instance.WriteLine(e.Message);
                }

            }
        }

        public AutoSerializable()
        {
            var att = (this.GetType()).GetCustomAttribute<AutoSerializableAttribute>();
            if (att == null)
                throw new AutoSerializableAttributeNotFoundException(this.GetType());

            this.SerializablePropertyNames = att.Properties;
        }
    }
}
