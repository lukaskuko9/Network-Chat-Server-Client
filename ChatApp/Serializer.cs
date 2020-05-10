using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
namespace Serialiser
{
    public class Serializer
    {
        public static string SerializeObject<T>(T objectToSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream memStr = new MemoryStream();

            try
            {
                bf.Serialize(memStr, objectToSerialize);
                memStr.Position = 0;

                return Convert.ToBase64String(memStr.ToArray());
            }
            finally
            {
                memStr.Close();
            }
        }

        public static dynamic DeserializeObject(string str)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] b = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(b);

            try
            {
                return bf.Deserialize(ms);
            }
            finally
            {
                ms.Close();
            }
        }

        public static T DeserializeObject<T>(string str)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] b = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(b);

            try
            {
                return (T)bf.Deserialize(ms);
            }
            finally
            {
                ms.Close();
            }
        }

        public static void Serialize(NetworkStream stream, object obj)
        {
            // Create a hashtable of values that will eventually be serialized.
            /*Hashtable addresses = new Hashtable();

            addresses.Add("Jeff", "123 Main Street, Redmond, WA 98052");
            addresses.Add("Fred", "987 Pine Road, Phila., PA 19116");
            addresses.Add("Mary", "PO Box 112233, Palo Alto, CA 94301");*/

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //formatter.Serialize(stream, addresses);
                formatter.Serialize(stream, obj);
                Console.WriteLine("serializing");
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
              //  fs.Close();
            }
        }

        public static dynamic Deserialize(Stream stream)
        {
            // Declare the hashtable reference.
            dynamic addresses = null;
            try
            {
               BinaryFormatter formatter = new BinaryFormatter();
                // Deserialize the hashtable from the file and
                // assign the reference to the local variable.
                addresses = formatter.Deserialize(stream);
                //Console.WriteLine("deserializing");
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                
            }

            // To prove that the table deserialized correctly,
            // display the key/value pairs.
           /* foreach (DictionaryEntry de in addresses)
            {
                Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
            }*/
            return addresses;
        }
    }
}
