using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using ConcurrencyLib;
using MsgPack.Serialization;
using ProtoBuf;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializationWithTypeTest();
        }

        private static void MtTest()
        {
            var mtProcessor = new MtProcessor();
            var start = DateTime.Now;
            mtProcessor.Process(1000);
            while (!mtProcessor.AllDone())
            {
                Thread.Sleep(10);
            }
            var end = DateTime.Now;
            Console.WriteLine("time: {0}", end - start);
            Console.WriteLine("All done");
            Console.ReadLine();

            var asyncProcessor = new AsyncProcessor();
            start = DateTime.Now;
            asyncProcessor.Process(1000);
            while (!asyncProcessor.AllDone())
            {
                Thread.Sleep(10);
            }
            end = DateTime.Now;
            Console.WriteLine("time: {0}", end - start);
            Console.WriteLine("All done");

            Console.ReadLine();
        }

        private static void SerializationWithTypeTest()
        {
            var container = new ContainerClass
            {
                Name = "MyContainer",
                Members = new List<MemberBaseClass> {new MemberDerivedClass1(), new MemberDerivedClass2(), new MemberDerivedClass3()}
            };

            byte[] bytes = null;
            var bytes1 = BinarySerialize(container);
            var bytes2 = BinarySerialize2(container);
            var bytesx = CompressedBinarySerialize(container);
            //var bytes3 = JsonToBytesSerializer(container);
            var bytes4 = EncodedJsonSerializer(container);
            var bytes5 = CompressEncodedJsonSerializer(container);
            var bytes6 = ProtoBufSerailizer(container);

            var o = CompressedBinaryDeserialize<ContainerClass>(bytesx);
            o = EncodedJsonDeserializer<ContainerClass>(bytes4);
            o = ProtoBufDeserializer<ContainerClass>(bytes6);

            var start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = BinarySerialize(container);
            }
            var end = DateTime.Now;
            Console.WriteLine("BinarySerialize: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = BinarySerialize2(container);
            }
            end = DateTime.Now;
            Console.WriteLine("BinarySerialize2: {0}", end - start);

            //start = DateTime.Now;
            //for (int i = 0; i < 100000; ++i)
            //{
            //    bytes = JsonToBytesSerializer(container);
            //}
            //end = DateTime.Now;
            //Console.WriteLine("JsonToBytesSerializer: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = EncodedJsonSerializer(container);
            }
            end = DateTime.Now;
            Console.WriteLine("EncodedJsonSerializer: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = MsgPackSerializer(container);
            }
            end = DateTime.Now;
            Console.WriteLine("MsgPackSerializer: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = ProtoBufSerailizer(container);
            }
            end = DateTime.Now;
            Console.WriteLine("ProtoBufSerailizer: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = CompressEncodedJsonSerializer(container);
            }
            end = DateTime.Now;
            Console.WriteLine("CompressEncodedJsonSerializer: {0}", end - start);

            start = DateTime.Now;
            for (int i = 0; i < 100000; ++i)
            {
                bytes = CompressedBinarySerialize(container);
            }
            end = DateTime.Now;
            Console.WriteLine("CompressedBinarySerialize: {0}", end - start);

            //var deserializedContainer = BinaryDeserialize<ContainerClass>(bytes);
            var deserializedContainer = EncodedJsonDeserializer<ContainerClass>(bytes4);

            foreach (var m in deserializedContainer.Members)
            {
                m.ClassName();
            }

            Console.ReadLine();
        }

        private static byte[] BinarySerialize<T>(T t) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, t);
                return ms.GetBuffer();
            }
        }

        private static byte[] BinarySerialize2<T>(T t) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, t);
                return ms.ToArray();
            }
        }

        private static T BinaryDeserialize<T>(byte[] bytes) where T : class
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(ms);
            }
        }

        private static byte[] CompressedBinarySerialize<T>(T t) where T : class
        {
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionLevel.Fastest))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(gs, t);
                }
                return mso.ToArray();
            }
        }

        private static T CompressedBinaryDeserialize<T>(byte[] bytes) where T : class
        {
            using (var msi = new MemoryStream(bytes))
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    var formatter = new BinaryFormatter();
                    return (T)formatter.Deserialize(gs);
                }
            }
        }

        public static byte[] JsonToBytesSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                ser.WriteObject(ms, t);
                return ms.ToArray();
            }
        }

        public static T JsonToBytesDeserialize<T>(byte[] bytes)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }

        public static string JsonSerializer<T>(T t)
        {
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    ser.WriteObject(ms, t);
            //    string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            //    ms.Close();
            //    return jsonString;
            //}

            return new JavaScriptSerializer().Serialize(t);
        }

        public static T JsonDeserialize<T>(string jsonString)
        {
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            //using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            //{
            //    T obj = (T)ser.ReadObject(ms);
            //    return obj;
            //}

            return new JavaScriptSerializer().Deserialize<T>(jsonString);
        }

        public static byte[] EncodedJsonSerializer<T>(T t)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer(t));
        }

        public static T EncodedJsonDeserializer<T>(byte[] bytes)
        {
            return JsonDeserialize<T>(Encoding.UTF8.GetString(bytes));
        }

        public static byte[] CompressEncodedJsonSerializer<T>(T t)
        {
            var buffer = Encoding.UTF8.GetBytes(JsonSerializer(t));
            using (var msi = new MemoryStream(buffer))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return mso.ToArray();
            }
        }

        public static T CompressEncodedJsonDeserializer<T>(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return JsonDeserialize<T>(Encoding.UTF8.GetString(mso.ToArray()));
            }
        }

        public static byte[] MsgPackSerializer<T>(T t)
        {
            var serializer = MessagePackSerializer.Get<T>();
            // Pack obj to stream.
            using (var stream = new MemoryStream())
            {
                serializer.Pack(stream, t);
                return stream.ToArray();
            }
        }

        public static T MsgPackDeserializer<T>(byte[] bytes)
        {
            var serializer = MessagePackSerializer.Get<T>();
            using (var stream = new MemoryStream(bytes))
            {
                return serializer.Unpack(stream);
            }
        }

        public static byte[] ProtoBufSerailizer<T>(T t) where T: class 
        {
            using (var memStream = new MemoryStream())
            {
                Serializer.Serialize(memStream, t);
                return memStream.ToArray();
            }
        }

        public static T ProtoBufDeserializer<T>(byte[] array) where T : class
        {
            using (var memStream = new MemoryStream(array))
                return Serializer.Deserialize<T>(memStream);
        }
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(10, typeof(MemberDerivedClass1))]
    [ProtoInclude(11, typeof(MemberDerivedClass2))]
    public class MemberBaseClass
    {
        [ProtoMember(1)]
        public string Name = "MemberBaseClass";
        public virtual void ClassName()
        {
            Console.WriteLine(Name);
        }
    }

    [Serializable]
    [ProtoContract]
    public class MemberDerivedClass1 : MemberBaseClass
    {
        [ProtoMember(1)]
        public string Name1 = "MemberDerivedClass1";
        public override void ClassName()
        {
            Console.WriteLine(Name1);
        }
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(11, typeof(MemberDerivedClass3))]
    public class MemberDerivedClass2 : MemberBaseClass
    {
        [ProtoMember(1)]
        public string Name2 = "MemberDerivedClass2";
        public override void ClassName()
        {
            Console.WriteLine(Name2);
        }
    }

    [Serializable]
    [ProtoContract]
    public class MemberDerivedClass3 : MemberDerivedClass2
    {
        [ProtoMember(1)]
        public string Name3 = "MemberDerivedClass3";
        public override void ClassName()
        {
            Console.WriteLine(Name2);
        }
    }

    [Serializable]
    [ProtoContract]
    [ProtoInclude(11, typeof(Dictionary<string, string>))]
    public class ContainerClass
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public List<MemberBaseClass> Members;

        [ProtoMember(3)] 
        public List<Dictionary<string, string>> ListOfDict;
    }
}
