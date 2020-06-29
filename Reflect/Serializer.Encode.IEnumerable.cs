using System;
using System.Collections;

namespace ULIB
{
	public partial class Serializer
	{
		/// <summary>
        /// Return array of bytes with array data.
        /// Any object (exclude bool,int,string,float, struct and enum) return as resource (251 and 4 bytes with resource number) 
		/// </summary>
		/// <param name="inObject"></param>
		/// <returns></returns>
		public byte[] EncodeArray(Array inObject)
		{
            //Debug.Log("EncodeArray " + inObject);
			var arrayClassNameByte = EncodeString(inObject.GetType().GetElementType().FullName);
			//var arrayLength = inObject.Length;
			var arrayLenghtByte = EncodeInteger(inObject.Length);
			var result = new byte[4 + arrayClassNameByte.Length];
			Buffer.BlockCopy(arrayLenghtByte, 0, result, 0, 4);
			Buffer.BlockCopy(arrayClassNameByte, 0, result, 4, arrayClassNameByte.Length);
			foreach (var t in inObject)
			{
				var targetBytes = Encoding(t);
				var valueBytes = new byte[result.Length + targetBytes.Length];
				Buffer.BlockCopy(result, 0, valueBytes, 0, result.Length);
				Buffer.BlockCopy(targetBytes, 0, valueBytes, result.Length, targetBytes.Length);
				result = valueBytes;
			}
			/*for (var i = 0; i < arrayLength; i++)
			{
				var item = Encode(inObject.GetValue(i));
				var newBa = new byte[result.Length + item.Length];
				Buffer.BlockCopy(result, 0, newBa, 0, result.Length);
				Buffer.BlockCopy(item, 0, newBa, result.Length, item.Length);
				result = newBa;
			}*/
			return result;
		}

        /// <summary>
        /// Return array of bytes with arraylist data.
        /// Any object (exclude bool,int,string,float, struct and enum) return as resource (251 and 4 bytes with resource number) 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public byte[] EncodeArrayList(IList inObject)
		{
			var result = EncodeInteger(inObject.Count);
			var listCount = inObject.Count;
			for (var i = 0; i < listCount; i++)
			{
				var item = Encoding(inObject[i]);
				var newBa = new byte[result.Length + item.Length];
				Buffer.BlockCopy(result, 0, newBa, 0, result.Length);
				Buffer.BlockCopy(item, 0, newBa, result.Length, item.Length);
				result = newBa;
			}
			return result;
		}

        /// <summary>
        /// Return array of bytes with hashtable data.
        /// Any object (exclude bool,int,string,float, struct and enum) return as resource (251 and 4 bytes with resource number) 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public byte[] EncodeHashtable(Hashtable inObject)
		{
            //Debug.Log("EncodeHashtable " + inObject);
			var result = EncodeInteger(inObject.Count);
			foreach (DictionaryEntry de in inObject)
			{
				var key = Encoding(de.Key);
				//Debug.Log("EncodeHashtable get value for "+de.Value+" key "+de.Key);
				var val = Encoding(de.Value);

				var newBa = new byte[result.Length + key.Length + val.Length];
				Buffer.BlockCopy(result, 0, newBa, 0, result.Length);
				Buffer.BlockCopy(key, 0, newBa, result.Length, key.Length);
				Buffer.BlockCopy(val, 0, newBa, result.Length + key.Length, val.Length);
				result = newBa;
			}
			return result;
		}

        /// <summary>
        /// Return array of bytes with list data.
        /// Any object (exclude bool,int,string,float, struct and enum) return as resource (251 and 4 bytes with resource number) 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public byte[] EncodeList(IList inObject)
		{
			var valClassNameByte = EncodeString(inObject.GetType().GetGenericArguments()[0].FullName);
			var result = new byte[4 + valClassNameByte.Length];
			Buffer.BlockCopy(EncodeInteger(inObject.Count), 0, result, 0, 4);
			Buffer.BlockCopy(valClassNameByte, 0, result, 4, valClassNameByte.Length);
			foreach (var t in inObject)
			{
				var targetBytes = Encoding(t);
				var valueBytes = new byte[result.Length + targetBytes.Length];
				Buffer.BlockCopy(result, 0, valueBytes, 0, result.Length);
				Buffer.BlockCopy(targetBytes, 0, valueBytes, result.Length, targetBytes.Length);
				result = valueBytes;
			}
			return result;
		}

        /// <summary>
        /// Return array of bytes with dictionary data.
        /// Any object (exclude bool,int,string,float, struct and enum) return as resource (251 and 4 bytes with resource number) 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public byte[] EncodeDictionary(IDictionary inObject)
		{
			var typeDict = inObject.GetType();
			var keyClassName = EncodeString(typeDict.GetGenericArguments()[0].FullName);
			var valClassName = EncodeString(typeDict.GetGenericArguments()[1].FullName);
			var result = new byte[4 + keyClassName.Length + valClassName.Length];
			Buffer.BlockCopy(EncodeInteger(inObject.Count), 0, result, 0, 4);
			Buffer.BlockCopy(keyClassName, 0, result, 4, keyClassName.Length);
			Buffer.BlockCopy(valClassName, 0, result, 4 + keyClassName.Length, valClassName.Length);

			foreach (var objKey in inObject.Keys)
			{
				var key = Encoding(objKey);
				var val = Encoding(inObject[objKey]);
				var resLengt = result.Length;
				var keyLength = key.Length;
				var valLength = val.Length;
				var newBa = new byte[resLengt + keyLength + valLength];
				Buffer.BlockCopy(result, 0, newBa, 0, resLengt);
				Buffer.BlockCopy(key, 0, newBa, resLengt, keyLength);
				Buffer.BlockCopy(val, 0, newBa, resLengt + keyLength, valLength);
				result = newBa;
			}
			return result;
		}

		private static byte[] ConvertArray(int[] array)
		{
			var newarray = new byte[array.Length * 4];

			for (var i = 0; i < array.Length; i++)
			{
				newarray[i * 4] = (byte)array[i];
				newarray[i * 4 + 1] = (byte)(array[i] >> 8);
				newarray[i * 4 + 2] = (byte)(array[i] >> 16);
				newarray[i * 4 + 3] = (byte)(array[i] >> 24);

			}
			return newarray;
		}
	}
}
