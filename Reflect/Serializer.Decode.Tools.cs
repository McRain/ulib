using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.BZip2;
using UnityEngine;

namespace ULIB
{
	public partial class Serializer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="vtype"></param>
		/// <returns></returns>
		private static object GeneratorList(Type vtype)
		{
			//Debug.Log("GeneratorList " + vtype.Name);
			var listType = Type.GetType("System.Collections.Generic.List`1");
			Type genericType = null;
			try
			{
				genericType = listType.MakeGenericType(vtype);
			}
			catch
			{
				ULog.Log("Serializer:GeneratorList - can't create the generic type",ULogType.Error);
				//Debug.LogError("Serializer:GeneratorList - can't create the generic type");
			}
			object result = new List<object>();
			try
			{
				result = genericType != null ? Activator.CreateInstance(genericType) : new object();
			}
			catch
			{
				ULog.Log("Serializer:GeneratorList - can't instantiate generic list",ULogType.Error);
				//Debug.LogError("Serializer:GeneratorList - can't instantiate generic list");
			}
			return result;
			/*Type specListType = typeof (IList<>).MakeGenericType(vtype);
			return Activator.CreateInstance(specListType);*/
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kType"></param>
        /// <param name="vType"></param>
        /// <returns></returns>
        private static object GeneratorDict(Type kType, Type vType)
		{
			var specDictType = typeof(Dictionary<,>).MakeGenericType(kType, vType);
			return Activator.CreateInstance(specDictType);
		}

		

		private object UnzipBytes(byte[] compbytes, ref int startPos)
		{
			//Debug.Log("UNZIP");
			var result = new byte[0];
			var spos = startPos;
			//if (UNP.Registered)
			//{
			var packLength = DecodeInteger(compbytes, ref spos);
			var unpackLength = DecodeInteger(compbytes, ref spos);
			//try
			//{
			//Debug.Log("Unpack: "+unpackLength.ToString()+" Pack: "+packLength.ToString());
			var buffer = new byte[packLength];
			Buffer.BlockCopy(compbytes, spos, buffer, 0, packLength);
			var msUncompressed = new MemoryStream(buffer);
			var zisUncompressed = new BZip2InputStream(msUncompressed);
			result = new byte[unpackLength];
			zisUncompressed.Read(result, 0, unpackLength);
			zisUncompressed.Close();
			msUncompressed.Close();
			startPos += 8 + packLength;
			//}
			//else
			//{
			//	Debug.LogError("Error REG U " + className + " " + methodName);
			//}
			return Decode(result);
		}

		private object Decrypt(byte[] inBytes,ref int startPos)
		{
			//Debug.Log("Decrypt");
			var tdes = new TripleDESCryptoServiceProvider { Key = _tdes.Key, IV = _tdes.IV, Padding = PaddingMode.Zeros };
			var encryptor = tdes.CreateDecryptor();
			//Debug.Log("Decrypt 2");
			var ms = new MemoryStream();
			var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
			cs.Write(inBytes, startPos, inBytes.Length-startPos);
			cs.FlushFinalBlock();
			ms.Position = 0;
			var result = new byte[ms.Length];
			ms.Read(result, 0, (int)ms.Length);
			cs.Close();
			startPos += inBytes.Length;
			//var qwe = Convert.ToBase64String(result);
			//var qwe1 = Convert.FromBase64String(qwe);
			//return Convert.ToBase64String(result);
			return Decode(result);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="component"></param>
        public static void HashtableToObject(Hashtable table,object component )
        {
            var cType = component.GetType();
            foreach (DictionaryEntry entry in table)
            {
                var cField = cType.GetField(entry.Key.ToString());
                if(cField!=null)
                {
                    cField.SetValue(component,entry.Value);
                    continue;
                }
                var cProp = cType.GetProperty(entry.Key.ToString());
                if(cProp!=null)
                {
                    
                    cProp.SetValue(component,entry.Value,null);
                    continue;
                }
                var cMethod = cType.GetMethod(entry.Key.ToString());
                if (cMethod != null)
                {
                    if (entry.Value is object[])
                        cMethod.Invoke(component, (object[]) entry.Value);
                    else
                        cMethod.Invoke(component, new[] {entry.Value});
                    continue;
                }
            }
        }

	    public static void HashtableToObject(Hashtable table, ref object component,string[] order)
        {
            var cType = component.GetType();
            foreach (var s in order)
            {
                if(table.ContainsKey(s))
                {
                    var value = table[s];

                    var cField = cType.GetField(s);
                    if (cField != null)
                    {
                        cField.SetValue(component, value);
                        continue;
                    }
                    var cProp = cType.GetProperty(s);
                    if (cProp != null)
                    {
                        cProp.SetValue(component, value, null);
                        continue;
                    }
                    var cMethod = cType.GetMethod(s);
                    if (cMethod != null)
                    {
                        if (value is object[])
                            cMethod.Invoke(component, (object[])value);
                        else
                            cMethod.Invoke(component, new[] { value });
                        continue;
                    }
                }
            }
        }

		/*private object UnPack(byte[] inBytes,ref int startPos)
		{
			byte[] result;
			var packSize = DecodeInteger(inBytes, ref startPos);
			startPos += packSize;
			var compressed = new byte[packSize];
			Buffer.BlockCopy(inBytes,startPos,compressed,0,packSize);
			using (var fd = new MemoryStream())
			using (var fs = new MemoryStream(compressed))
			using (Stream csStream = new GZipStream(fs, CompressionMode.Decompress))
			{
				var buffer = new byte[packSize];
				int nRead;
				while ((nRead = csStream.Read(buffer, 0, packSize)) > 0)
				{
					fd.Write(buffer, 0, nRead);
				}
				result = fd.ToArray();
			}
			return result;
		}*/
	}
}
