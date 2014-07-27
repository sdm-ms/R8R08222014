using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization.Json;
using System.Text;

public static class JsonSerializer
{
    public static string Serialize<T>(T objectToSerialize) where T : class
    {
        MemoryStream stream1 = new MemoryStream();
        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
        ser.WriteObject(stream1, objectToSerialize);
        string resultString = Encoding.UTF8.GetString(stream1.GetBuffer()).Replace("\x00","");
        return resultString;
    }

    public static T Deserialize<T>(string theString) where T : class
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(theString));
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        T myT = serializer.ReadObject(ms) as T;
        ms.Close();
        return myT;
    }
}

public static class BinarySerializer
{
    public static byte[] Serialize<T>(T objectToSerialize)
    {
        BinaryFormatter binFormat = new BinaryFormatter();
        using (MemoryStream mStream = new MemoryStream())
        {
            binFormat.Serialize(mStream, objectToSerialize);
            byte[] theData = mStream.GetBuffer();
            return theData;
        }
    }
    public static T Deserialize<T>(byte[] byteArray)
    {
        MemoryStream mStream = new MemoryStream(byteArray);
        BinaryFormatter binFormat = new BinaryFormatter();
        return (T) binFormat.Deserialize(mStream);
    }
}

public static class ByteArrayConversions
{
    // Convert an object to a byte array
    public static byte[] ObjectToByteArray(Object obj)
    {
        if (obj == null)
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }
    // Convert a byte array to an Object
    public static Object ByteArrayToObject(byte[] arrBytes)
    {
        MemoryStream memStream = new MemoryStream();
        BinaryFormatter binForm = new BinaryFormatter();
        memStream.Write(arrBytes, 0, arrBytes.Length);
        memStream.Seek(0, SeekOrigin.Begin);
        Object obj = (Object)binForm.Deserialize(memStream);
        return obj;
    }

}