using System;
using System.Collections.Generic;

namespace ULIB
{
	public partial class Serializer
	{
        /// <summary>
        /// External decoders for some base type
        /// </summary>
        public static readonly Dictionary<byte, ObjectDecoder> BaseTypeDecoders = new Dictionary<byte, ObjectDecoder>();

        /// <summary>
        /// External decoders for objects
        /// </summary>
        public static readonly Dictionary<byte, ObjectDecoder> TypeDecoders = new Dictionary<byte, ObjectDecoder>();


		///<summary>Decode bytes to object
		///</summary>
		///<param name="inBytes"></param>
		///<returns></returns>
		public virtual object Decode(byte[] inBytes)
		{
			var startPos = 0;
			return Decode(inBytes, ref startPos);
		}
		
		/// <summary>
		/// Decode bytes to object
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public virtual object Decode(byte[] inBytes, ref int startPos)
		{
			InitLists();

			startPos++;//skip version byte
			startPos++;//skip headsize byte

			return Decoding(inBytes, ref startPos);
		}
		
		/// <summary>
		/// Return your object . Convert from string and deserialize . For Serialize use EncodeToString(object inObj)
		/// </summary>
		/// <param name="val">string</param>
		/// <returns>Your object</returns>
		public object Decode(string val)
		{
			return DecodeFromString(val, "-",true);
		}

		/// <summary>
		///  Return your object . Convert from string and deserialize . For Serialize use EncodeToString(object inObj,string splitter)
		/// </summary>
		/// <param name="val">string</param>
		/// <param name="splitter">any string</param>
		/// <returns>Your object</returns>
		public object Decode(string val, string splitter)
		{
			return DecodeFromString(val, splitter,true);
		}

	    /// <summary>
	    /// Deserialize an object from a string that is encoded using EncodeToString or
        /// return byte[] if decode==false
	    /// </summary>
	    /// <param name="val"></param>
	    /// <param name="spliter"></param>
	    /// <param name="decode"> </param>
	    /// <returns></returns>
	    public object DecodeFromString(string val, string spliter,bool decode /*= true*/)
        {
            byte[] array;
            if (spliter.Length > 0)
            {
                var arr = val.Split(spliter[0]);
                array = new byte[arr.Length];
                for (var i = 0; i < arr.Length; i++)
                    array[i] = Convert.ToByte(arr[i], 16);
            }
            else
            {
                array = new byte[val.Length];
                var bI = 0;
                for (var i = 0; i < array.Length; i += 2)
                {
                    array[bI] = Convert.ToByte(val.Substring(i, 2), 16);
                    bI++;
                }
            }
            return decode ? Decode(array) : array;
        }

	    /// <summary>
	    /// Decode string encoded by method 
	    /// </summary>
	    /// <param name="val"></param>
	    /// <returns></returns>
	    public static byte[] DecodeFromString(string val)
        {
            var array = new byte[val.Length];
            var bI = 0;
            for (var i = 0; i < array.Length; i += 2)
            {
                array[bI] = Convert.ToByte(val.Substring(i, 2), 16);
                bI++;
            }
            return array;
        }

		/// <summary>
		/// Convert string (from EncodeToString(byte[] inBytes)) back to bytes array
		/// </summary>
		/// <param name="val">string</param>
		/// <returns>Ready bytes</returns>
		public byte[] DecodeBytes(string val)
		{
			return DecodeFromString(val);
		}

		object Decoding(byte[] inBytes, ref int startPos)
		{
            
			var code = inBytes[startPos];
            //Debug.Log("Decoding "+code);
			startPos++;
			if (TypeDecoders.ContainsKey(code))
				return TypeDecoders[code](inBytes, ref startPos,this);
			switch (code)
			{
				case 00://null
					return null;

				#region Primitive 01-09
				case 01://bool false
					return false;
				case 02://bool true
					return true;
				case 03://integer 
					return DecodeInteger(inBytes, ref startPos);
				case 04://float
					return DecodeSingle(inBytes, ref startPos);
				case 05://string
                    //Debug.Log("DecodeString pos " + startPos);
					return DecodeString(inBytes, ref startPos);
				case 06://Double
					return null;
				case 07://reserved
					return null;
				case 08://Char
					return null;
				case 09://DateTime
					return DecodeDiteTime(inBytes,ref startPos);
				#endregion

				#region IEnumerable 10-15
				case 10://Array
					return DecodeArray(inBytes, ref startPos);
				case 11://ArrayList
					return DecodeArrayList(inBytes, ref startPos);
				case 12://Hashtable
					return DecodeHashtable(inBytes, ref startPos);
				case 13://List
					return DecodeList(inBytes, ref startPos);
				case 14://Dictionary
					return DecodeDictionary(inBytes, ref startPos);
				case 15://ByteArray
					return null;
				#endregion

				#region Unity 16-199

				case 16://Vector2
					return DecodeVector2(inBytes, ref startPos);
				case 17://Vector3
					return DecodeVector3(inBytes, ref startPos);
				case 18://Vector4
					return DecodeVector4(inBytes, ref startPos);
				case 19://Quaternion
					return DecodeQuaternion(inBytes, ref startPos);
				case 20://Color
					return DecodeColor(inBytes, ref startPos);
				case 21://Rect
					return DecodeRect(inBytes, ref startPos);
                case 28://Matrix
			        return DecodeMatrix(inBytes, ref startPos);
                case 29://Bounds
			        return DecodeBounds(inBytes, ref startPos);
                case 30://BoneWeight 
			        return DecodeBoneWeight(inBytes, ref startPos);

				case 22://Mesh
					return DecodeMesh(inBytes, ref startPos);
				case 23://Texture2D
					return DecodeTexture2D(inBytes, ref startPos);
				case 24://Physics
					return DecodePhysicMaterial(inBytes, ref startPos);
				case 25://GameObject
					return DecodeGameObject(inBytes, ref startPos);
				case 26://Material
					return DecodeMaterial(inBytes, ref startPos);
                case 27://TerrainData
			        return DecodeTerrainData(inBytes, ref startPos);
				#endregion

				case 200://Object
					return DecodeObject(inBytes, ref startPos);

				#region ULIB Models//201-249
				/*case 201://ULabelModel
					return DecodeULabelModel(inBytes, ref startPos);
				case 202://UModel
					return DecodeUModel(inBytes, ref startPos);
				case 203://UCompare
					return DecodeUCompare(inBytes, ref startPos);
				case 204://UCommand
					return DecodeUCommand(inBytes, ref startPos);
				case 205://UValue
					return DecodeUValue(inBytes, ref startPos);
				case 206://UMenu
					return DecodeUMenu(inBytes, ref startPos);*/
				#endregion

				#region ULIB servicecode//250-254

				case 250://Packets of bytes
                    //InitLists();
					var bytesPackCount = DecodeInteger(inBytes, ref startPos);//count of stored resources bytes
					for (var i = 0; i < bytesPackCount; i++)
					{
						var bytePackLenght = DecodeInteger(inBytes, ref startPos);//lenght of [i] bytes for read
						var bytePack = new byte[bytePackLenght];//bytes for resource number i
						Buffer.BlockCopy(inBytes,startPos,bytePack,0,bytePackLenght);
						_bytesList.Add(i,bytePack);
						startPos += bytePackLenght;
					}
					return Decoding(inBytes, ref startPos);

				case 251://One packet of bytes
					var index = DecodeInteger(inBytes, ref startPos);//Object UID
					if(!_objectList.ContainsKey(index))//if position not exists 
					{
						var pos = 0;
						var obj = Decoding(_bytesList[index], ref pos);
                        _objectList.Add(index,obj);
					    _objectListCount++;
					}
                    return _objectList[index];

				case 252://Previos loaded resource - reserved
					ULog.Log("Decoding: Recived code 252 - code is reserved for previos decoded resource",ULogType.Warning);
			        
                    return null;
				case 253://Crypto
					return Decrypt(inBytes,ref startPos);
				case 254://Compressed
					return UnzipBytes(inBytes, ref startPos);
				#endregion

				default:

					ULog.Log(string.Format("Recived unknow code {0} \n{1}", code, System.Text.Encoding.UTF8.GetString(inBytes, startPos, inBytes.Length)), ULogType.Warning);
					return null;
			}
		}

		object DecodeObject(byte[] inBytes, ref int startPos)
		{
			object result = null;
			string typeName = null;
			try
			{
				typeName = DecodeString(inBytes, ref startPos);
                ULog.Log("START decode Class " + typeName);
			}
			catch
			{
				ULog.Log("(209)DecodeObject can't decode class name. " + typeName,ULogType.Error);
				//Debug.LogError("DecodeObject can't decode class name. " + typeName );// + inBytes[startPos].ToString());
			}
			if (typeName != null && typeName.Length > 1)
			{
				var tp = Type.GetType(typeName) ?? FindType(typeName);
				try
				{
					result = Activator.CreateInstance(tp);
				}
				catch
				{
					ULog.Log("(221)DecodeObject can't create Instance of " + typeName,ULogType.Error);
				}
				var fieldCount = DecodeInteger(inBytes, ref startPos);
				for (var i = 0; i < fieldCount; i++)
				{
					var field = DecodeField(inBytes, ref startPos);
					//Debug.Log("Decode field " + field.name + " in class " + className);
					var objFieldInfo = tp.GetField(field.name);
					var objPropertyInfo = tp.GetProperty(field.name);
					if (objFieldInfo != null)
					{
						//Debug.Log("Decode Field ");
						try
						{
							objFieldInfo.SetValue(result, field.value);
						}
						catch(Exception e)
						{
							ULog.Log("ERROR on set FieldInfo: CLASS: " + typeName + " FIELD: " + field.name+"\n"+e.Message,ULogType.Warning);
							//Debug.LogError("ERROR on set FieldInfo: CLASS: " + typeName + " FIELD: " + field.name);// + " VALUE: (" + field.value.GetType().ToString() + ")" + field.value.ToString() + "");
						}
					}
					else if (objPropertyInfo != null)
					{
						//Debug.Log("Decode Property ");
						if (objPropertyInfo.CanWrite)
						{
							try
							{
								objPropertyInfo.SetValue(result, field.value, null);
							}
							catch(Exception e)
							{
								//if (field.value != null)
								//{
                                ULog.Log("ERROR on set PropertyInfo->" + objPropertyInfo.Name + ": CLASS: " + typeName + " FIELD: " + field.name + "\n" + e.Message /*+ " VALUE: (" + field.value.GetType() + ")" + field.value + ""*/, ULogType.Warning);
									//Debug.LogError("ERROR on set PropertyInfo->" + objPropertyInfo.Name + ": CLASS: " + typeName + " FIELD: " + field.name + " VALUE: (" + field.value.GetType() + ")" + field.value + "");
								/*}
								else
								{
									Debug.LogError("ERROR on set PropertyInfo->" + objPropertyInfo.Name + ": CLASS: " + typeName + " FIELD: " + field.name + " VALUE: NULL ");
								}*/
							}
						}
					}
					else
					{
						ULog.Log("Member " + field.name + " not find in " + typeName,ULogType.Error);
						//Debug.LogError("Member " + field.name + " not find in " + typeName);
					}
				}
			}

			return result;
		}

		UField DecodeField(byte[] inBytes, ref int startPos)
		{
			var result = new UField { name = DecodeString(inBytes, ref startPos) };
			//Debug.Log("Decode Field name " + result.name);
			//try
			//{
				result.value = Decoding(inBytes, ref startPos);
			//}
			//catch
			//{
				//Console.WriteLine("DecodeField Error");
				//Debug.LogError("DecodeField " + result.name + " can't decode value " + className +"/"+ methodName);
			//}
			return result;
		}


		
	}

	
}
