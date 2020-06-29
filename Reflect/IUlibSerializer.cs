using System;
using System.Collections;

namespace ULIB
{
    public interface IUlibSerializer
    {
        byte[] EncodeHashtable(Hashtable hashtable);
        Hashtable DecodeHashtable(byte[] bytes, ref int startPosition);

        byte[] EncodeArray(Array inObject);
        Array DecodeArray(byte[] inBytes, ref int startPos);

        byte[] EncodeArrayList(IList inObject);
        ArrayList DecodeArrayList(byte[] inBytes, ref int startPos);

        byte[] EncodeList(IList inObject);
        IList DecodeList(byte[] inBytes, ref int startPos);

        byte[] EncodeDictionary(IDictionary inObject);
        IDictionary DecodeDictionary(byte[] inBytes, ref int startPos);
    }
}