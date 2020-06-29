using System;
using System.Collections;

namespace ULIB
{
	public partial class Serializer
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public Array DecodeArray(byte[] inBytes, ref int startPos)
		{
			var arrLength = DecodeInteger(inBytes, ref startPos);
			var arrayClassName = DecodeString(inBytes, ref startPos);

			var tp = FindType(arrayClassName);
			var result = Array.CreateInstance(tp, arrLength);
			for (var i = 0; i < arrLength; i++)
				result.SetValue(Decoding(inBytes, ref startPos), i);
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public ArrayList DecodeArrayList(byte[] inBytes, ref int startPos)
		{
			var result = new ArrayList();
			var arrLength = DecodeInteger(inBytes, ref startPos);
			for (var i = 0; i < arrLength; i++)
			{
				result.Add(Decoding(inBytes, ref startPos));
			}
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public Hashtable DecodeHashtable(byte[] inBytes, ref int startPos)
		{
			var result = new Hashtable();
			var arrLength = DecodeInteger(inBytes, ref startPos);
			//Debug.Log("DecodeHashtable "+arrLength);
			for (var i = 0; i < arrLength; i++)
			{
				var hkey = Decoding(inBytes, ref startPos);
                //Debug.Log("DecodeHashtable "+i+":"+hkey);
				result.Add(hkey, Decoding(inBytes, ref startPos));
			}
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public IList DecodeList(byte[] inBytes, ref int startPos)
		{
			var arrCount = DecodeInteger(inBytes, ref startPos);
			var keyType = DecodeString(inBytes, ref startPos);

			var tp = FindType(keyType);
			var result = (IList)GeneratorList(tp);
			for (var i = 0; i < arrCount; i++)
			{
				result.Add(Decoding(inBytes, ref startPos));
			}
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public IDictionary DecodeDictionary(byte[] inBytes, ref int startPos)
		{
			var arrCount = DecodeInteger(inBytes, ref startPos);
			var keyType = DecodeString(inBytes, ref startPos);
			var valType = DecodeString(inBytes, ref startPos);
			var typeKey = Type.GetType(keyType);
			var typeVal = Type.GetType(valType);

			if (typeKey == null)
				typeKey = FindType(keyType);

			if (typeVal == null)
				typeVal = FindType(valType);

			var result = (IDictionary)GeneratorDict(typeKey, typeVal);
			for (var i = 0; i < arrCount; i++)
			{
				var hkey = Decoding(inBytes, ref startPos);
				result.Add(hkey, Decoding(inBytes, ref startPos));
				//Debug.Log("value at "+i);
			}
			return result;
		}		
	}
}
