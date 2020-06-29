using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
	/// <summary>
	/// Code from 0 to 125 for Base Serializer.
	/// Code from 200 to 255 for ULIB
	/// For override Encode method use 126-198
	/// </summary>
	public partial class Serializer
	{
        /// <summary>
        /// External encoders for some type
        /// </summary>
        public static readonly Dictionary<Type, ObjectEncoder> BaseTypeEncoders = new Dictionary<Type, ObjectEncoder>();

        /// <summary>
        /// External encoders for objects 
        /// </summary>
        public static readonly Dictionary<Type, ObjectEncoder> TypeEncoders = new Dictionary<Type, ObjectEncoder>();


		/// <summary>
		/// Call once with root object for parameters.
		/// This is entry point for serialize
		/// </summary>
		/// <param name="inObject"></param>
		/// <returns></returns>
		public byte[] Encode(object inObject)
		{
            //Debug.Log("Entry point serialize "+inObject);
            InitLists();

		    var o = inObject as Transform;
		    var structureBytes = 
                o != null ? 
                    Encoding(o.gameObject) : 
                    Encoding(inObject);

		    var structureLenght = Buffer.ByteLength(structureBytes);
			
			if (_bytesLenght == 0)//if not exists resource bytes
			{
				var result = new byte[2 + structureLenght];//1b version + 1b headsize + data lenght
                result[0] = Version;//Version Serializer
				//Buffer.SetByte(result, 0, Version);//Version Serializer
                result[1] = 0x0;//Size of next head (bytes)//not used now
                //Buffer.SetByte(result, 1, 0x0);//Size of next head (bytes)//not used now
				#region Head
				//reserved
				//Buffer.SetByte(result, 2, 0x0);example head
				#endregion
				Buffer.BlockCopy(structureBytes, 0, result, 2, structureLenght);
				return result;
			}
			else//if exists resources bytes
			{
				var bytesPackCount = _bytesList.Count;
                //1b version + 1b headsize + 1b resource code + 4b resource count
				var result = new byte[7 + (bytesPackCount * 4) + _bytesLenght + structureLenght];
                result[0] = Version;//Version Serializer
                //Buffer.SetByte(result, 0, Version);//Version Serializer
                result[1] = 0x0;//Size of next head (bytes)//not used now
                //Buffer.SetByte(result, 1, 0x0);//Size of next head (bytes)//not used now
				#region Head
				//reserved
				//Buffer.SetByte(result, 2, 0x0);example head
				#endregion

                result[2] = 0xfa;//Pack code 250 - 1b
                //Buffer.SetByte(result, 2, 0xfa);//Pack code 250
				Buffer.BlockCopy(BitConverter.GetBytes(bytesPackCount), 0, result, 3, 4);//counter of pack in bytes pack - 4b

				var pos = 7;
				
                for (var index = 0; index < bytesPackCount; index++)
				{
                    var byteArray = _bytesList[index];
					var bLenght = Buffer.ByteLength(byteArray);
					Buffer.BlockCopy(BitConverter.GetBytes(bLenght), 0, result, pos, 4);
					pos += 4;
					Buffer.BlockCopy(byteArray, 0, result, pos, bLenght);
					pos += bLenght;
				}
				Buffer.BlockCopy(structureBytes, 0, result, pos, structureLenght);
				return result;
			}
		}

		internal byte[] Encoding(object sourceObject)
		{
            ULog.Log("Encoding " + sourceObject);
		    byte code = 0x0;//null
			byte[] dataBytes;
		    if (sourceObject==null || sourceObject.Equals(null))
		    {
                //Debug.LogWarning("Encoding IS NULL");
                return new[] { code }; 
		    }
		    var sourceType = sourceObject.GetType();
            //Not overrided rule serialization
			switch (sourceType.FullName)
            {
                #region Primitive 01-09

                case "System.Boolean":
					return EncodeBoolean((bool)sourceObject);
				case "System.Int32":
					dataBytes = EncodeInteger((int)sourceObject);
					code = 3;
					break;
				case "System.Single":
					dataBytes = EncodeFloat((float)sourceObject);
					code = 4;
					break;
				case "System.String":
					dataBytes = EncodeString(sourceObject);
					code = 5;
					break;
                case "System.DateTime":
                    dataBytes = EncodeDateTime((DateTime)sourceObject);
                    code = 9;
                    break;

                #endregion

                #region Unity Sctructures 16-~

                case "UnityEngine.Vector2":
					dataBytes = EncodeVector2((Vector2)sourceObject);
					code = 16;
					break;
				case "UnityEngine.Vector3":
					dataBytes = EncodeVector3((Vector3)sourceObject);
					code = 17;
					break;
				case "UnityEngine.Vector4":
					dataBytes = EncodeVector4((Vector4)sourceObject);
					code = 18;
					break;
				case "UnityEngine.Quaternion":
					dataBytes = EncodeQuaternion((Quaternion)sourceObject);
					code = 19;
					break;
				case "UnityEngine.Color":
					dataBytes = EncodeColor((Color)sourceObject);
					code = 20;
					break;
				case "UnityEngine.Rect": //Rect
					dataBytes = EncodeRect((Rect)sourceObject);
					code = 21;
					break;
                case "UnityEngine.Matrix4x4":
			        dataBytes = EncodeMatrix((Matrix4x4) sourceObject);
			        code = 28;
                    break;
                case "UnityEngine.Bounds":
			        dataBytes = EncodeBounds((Bounds) sourceObject);
			        code = 29;
                    break;
                case "UnityEngine.BoneWeight":
			        dataBytes = EncodeBoneWeight((BoneWeight) sourceObject);
			        code = 30;
                    break;
                /*case "UnityEngine.Keyframe":
                    dataBytes = EncodeKeyframe((Keyframe) sourceObject);
                    code = 31;
                    break;*/


                #endregion

                default:
					var baseType = sourceType.BaseType;
					if (baseType == typeof(Enum))
					{
						dataBytes = EncodeInteger((int)sourceObject);
						code = 3;
						break;
					}

                    //Debug.Log(_objectList);
                    //Debug.Log(sourceObject);

                    if (!_objectList.ContainsValue(sourceObject))
					{
                        ULog.Log("New object " + sourceObject);
						var objectBytes = ResourceEncoding(sourceObject, sourceType, baseType);
                        _objectList.Add(_objectListCount, sourceObject);
                        _bytesList.Add(_objectListCount, objectBytes);
						_bytesLenght += objectBytes.Length;
                        _objectListCount++;
					}
					var resourceIndexBytes = new byte[] { 251, 0, 0, 0, 0 };
                    Buffer.BlockCopy(EncodeInteger(GetResourceIndex(sourceObject)), 0, resourceIndexBytes, 1, 4);
					return resourceIndexBytes;
			}
			var dataBytesLength = dataBytes.Length;
			var result = new byte[1 + dataBytesLength];
			result[0] = code;
			Buffer.BlockCopy(dataBytes, 0, result, 1, dataBytesLength);
            ULog.Log("End encoding");
			return result;
		}

	    protected byte[] ResourceEncoding(object inObject,Type objectType, Type baseType)
		{
            //Debug.Log("ResourceEncoding "+inObject);
			byte code = 0;
			byte[] dataBytes;
		    if (inObject == null)
		        return new byte[] {0};
			if (objectType.IsArray) //10
			{
				dataBytes = EncodeArray((Array) inObject);
				code = 10;
			}
			else if (TypeEncoders.ContainsKey(objectType))
			    return TypeEncoders[objectType](inObject, this);
			else
			{
                //Debug.Log("objectType.Name" + objectType.Name);
			    switch (objectType.Name)
			    {
			            #region IEnumerable 11-15

			        case "ArrayList":
			            dataBytes = EncodeArrayList((ArrayList) inObject);
			            code = 11;
			            break;
			        case "Hashtable":
			            dataBytes = EncodeHashtable((Hashtable) inObject);
			            code = 12;
			            break;
			        case "List`1":
			            dataBytes = EncodeList((IList) inObject);
			            code = 13;
			            break;
			        case "Dictionary`2":
			            dataBytes = EncodeDictionary((IDictionary) inObject);
			            code = 14;
			            break;
                    case "ConcurrentDictionary`2":
                        dataBytes = EncodeDictionary((IDictionary)inObject);
                        code = 14;
                        break;

			            #endregion

			            #region Unity 16-199 (199-Component; 200-AnyObject)

			        case "Mesh":
			            dataBytes = EncodeMesh((Mesh) inObject);
			            code = 22;
			            break;
			        case "Texture2D":
			            dataBytes = EncodeTexture2D((Texture2D) inObject);
			            code = 23;
			            break;
			        case "PhysicMaterial":
			            dataBytes = EncodePhysicMaterial((PhysicMaterial) inObject);
			            code = 24;
			            break;
			        case "GameObject":
			            dataBytes = EncodeGameObject((GameObject) inObject);
			            code = 25;
			            break;
			        case "Material":
			            dataBytes = EncodeMaterial((Material) inObject);
			            code = 26;
			            break;
			        case "TerrainData":
			            dataBytes = EncodeTerrainData((TerrainData) inObject);
			            code = 27;
			            break;
                    case "Sprite":
			            dataBytes = EncodeSprite((Sprite) inObject);
                        break;
                        /*case "AnimationCurve":
				        dataBytes = EncodeAnimationCurve((AnimationCurve) inObject);
				        code = 32;
                        break;*/

                    #endregion

                    #region ULIBModel 201-249 (200-AnyObject)

                    /*case "ULabelModel": //ULabelModel
                    dataBytes = EncodeULabelModel((ULabelModel)inObject);
                    code = 201;
                    break;
                case "UModel": //UModel
                    dataBytes = EncodeUModel((UModel)inObject);
                    code = 202;
                    break;
                case "UCompare": //UCompare
                    dataBytes = EncodeUCompare((UCompare)inObject);
                    code = 203;
                    break;
                case "UCommand": //UCommand
                    dataBytes = EncodeUCommand((UCommand)inObject);
                    code = 204;
                    break;
                case "UValue": //UValue
                    dataBytes = EncodeUValue((UValue)inObject);
                    code = 205;
                    break;
                case "UMenu": //UMenu
                    dataBytes = EncodeUMenu((UMenu)inObject);
                    code = 206;
                    break;*/

                    #endregion

                    default:
			            //Debug.Log("BASETYPE: " + baseType+" for "+inObject);
			            if (baseType != null)
			            {
			                if (baseType == typeof (MonoBehaviour) || baseType == typeof (GUIElement) ||
			                    baseType == typeof (Component))
			                    return EncodeComponent((Component) inObject);

			                if (BaseTypeEncoders.ContainsKey(baseType))
			                    return BaseTypeEncoders[baseType](inObject, this);
			            }
			            //Debug.Log("Default " + baseType.FullName + ":" + objectType.FullName);

			            code = 200;
			            dataBytes = EncodeObject(inObject);
			            break;
			    }
			}
	        var dataBytesLength = dataBytes.Length;
			var result = new byte[1 + dataBytesLength];
			result[0] = code;
			Buffer.BlockCopy(dataBytes, 0, result, 1, dataBytesLength);
			return result;
		}

		byte[] EncodeObject(object inObject)
		{
			//Debug.Log("Encode Object "+inObject);
            
			var result = EncodeString(inObject.GetType().FullName);
			var tp = inObject.GetType();
			var fieldsInfo = tp.GetFields();
			var propertys = tp.GetProperties();
			var fieldCount = 0;
			var fieldByte = new byte[0];

			foreach (var field in fieldsInfo)
				if (!field.IsNotSerialized)
				{
					var fnameBytes = EncodeString(field.Name);
					var fieldValueBytes = Encoding(field.GetValue(inObject));
					var newBa = new byte[fieldByte.Length + fnameBytes.Length + fieldValueBytes.Length];

					Buffer.BlockCopy(fieldByte, 0, newBa, 0, fieldByte.Length);
					Buffer.BlockCopy(fnameBytes, 0, newBa, fieldByte.Length, fnameBytes.Length);
					Buffer.BlockCopy(fieldValueBytes, 0, newBa, fieldByte.Length + fnameBytes.Length, fieldValueBytes.Length);

					fieldByte = newBa;
					/*if (field.GetValue(inObject) != null)
					{
						var fieldValBytes = Encode(field.GetValue(inObject));
						newBa = new byte[fieldByte.Length + fieldValBytes.Length];

						Buffer.BlockCopy(fieldByte, 0, newBa, 0, fieldByte.Length);
						Buffer.BlockCopy(fieldValBytes, 0, newBa, fieldByte.Length, fieldValBytes.Length);
						fieldByte = newBa;
					}
					else
					{
						var code = new byte[1];
						code[0] = 0;

						newBa = new byte[fieldByte.Length + code.Length+1];

						Buffer.BlockCopy(fieldByte, 0, newBa, 0, fieldByte.Length);
						newBa[fieldByte.Length + code.Length] = 0;
						fieldByte = newBa;
					}*/
					fieldCount++;
				}

		    foreach (var prop in propertys)
		    {
                //Debug.Log(inObject+" Property: "+prop.Name+":"+prop);
		        if (!prop.CanWrite) continue;
		        if (prop.GetValue(inObject, null) != null)
		        {
		            var pos = fieldByte.Length;
		            var fnameBytes = EncodeString(prop.Name);
		            var fieldValBytes = Encoding(prop.GetValue(inObject, null));

		            var newBa = new byte[pos + fnameBytes.Length + fieldValBytes.Length];
		            Buffer.BlockCopy(fieldByte, 0, newBa, 0, pos);
		            Buffer.BlockCopy(fnameBytes, 0, newBa, pos, fnameBytes.Length);
		            pos += fnameBytes.Length;
		            Buffer.BlockCopy(fieldValBytes, 0, newBa, pos, fieldValBytes.Length);
		            fieldByte = newBa;

		                
		            //newBa = new byte[fieldByte.Length + fieldValBytes.Length];

		            //Buffer.BlockCopy(fieldByte, 0, newBa, 0, fieldByte.Length);
		                
		            //fieldByte = newBa;
		        }
		        else
		        {
		            var propNameBytes = EncodeString(prop.Name);
		            var propBytes = new byte[fieldByte.Length + propNameBytes.Length + 1];
		            Buffer.BlockCopy(fieldByte, 0, propBytes, 0, fieldByte.Length);
		            Buffer.BlockCopy(propNameBytes, 0, propBytes, fieldByte.Length, propNameBytes.Length);
		            propBytes[propBytes.Length - 1] = 0;
		            fieldByte = propBytes;
		        }
		        fieldCount++;
		    }
		    var fcb = BitConverter.GetBytes(fieldCount);
			var resLength = result.Length;
			var fcbLength = fcb.Length;
			var fbLength = fieldByte.Length;

			var newResult = new byte[resLength + fcbLength + fbLength];
			Buffer.BlockCopy(result, 0, newResult, 0, resLength);
			Buffer.BlockCopy(fcb, 0, newResult, resLength, fcbLength);
			Buffer.BlockCopy(fieldByte, 0, newResult, resLength + fcbLength, fbLength);
			//Debug.Log("EncodeObject return "+newResult+" : "+newResult.Length);
			return newResult;
		}
		
		
		/*public byte[] EncodeTexturePixels(Texture2D inObject)
		{
			//Debug.Log("EncodeTexture2D pixels " + inObject.name+" w: "+inObject.width.ToString()+" h: "+inObject.height.ToString());
			Color[] pixelsTexture = inObject.GetPixels();
			byte[] result = new byte[4 + 4 + pixelsTexture.Length * 16];
			System.Buffer.BlockCopy(EncodeInteger(inObject.width), 0, result, 0, 4);
			System.Buffer.BlockCopy(EncodeInteger(inObject.height), 0, result, 4, 4);
			int startPos = 8;
			foreach (Color pixels in pixelsTexture)
			{
				System.Buffer.BlockCopy(EncodeColor(pixels), 0, result, startPos, 16);
				startPos += 16;
			}
			return result;
		}*/

		/*public byte[] EncodeShader(Shader inObject)
		{
			return EncodeString(inObject.name);
		}*/

		/*string[] defaultPropertyNames = new string[12] { "_Color", "_MainTex", "_BumpMap", "_SpecColor", "_Shininess", "_Parallax", "_ParallaxMap",
													"_Cutoff","_Emission","_DecalTex","_Illum","_EmissionLM"};*/

		/*public byte[] EncodeMaterial(Material inObject)
		{
			Hashtable matProperty = new Hashtable();
			matProperty.Add("name", inObject.name);
			matProperty.Add("shader", inObject.shader.name);
			if (inObject.HasProperty("_MainColor"))
				matProperty.Add("color", inObject.color);
			if (inObject.HasProperty("_MainTex"))
			{
				matProperty.Add("mainTextureOffset", inObject.mainTextureOffset);
				matProperty.Add("mainTextureScale", inObject.mainTextureScale);
				if (inObject.mainTexture != null)
				{
					AddTextureToPack((Texture2D)inObject.mainTexture);
					matProperty.Add("_MainTex", ((Texture2D)inObject.mainTexture).name);
				}
			}
			if (inObject.HasProperty("_SpecularColor"))
			{
				matProperty.Add("_SpecularColor", inObject.GetColor("_SpecularColor"));
			}
			if (inObject.HasProperty("_Color"))
			{
				matProperty.Add("_Color", inObject.GetColor("_Color"));
			}

			if (inObject.HasProperty("_ShadowTex"))
			{
				AddTextureToPack((Texture2D)inObject.GetTexture("_ShadowTex"));
				matProperty.Add("_ShadowTex", inObject.GetTexture("_ShadowTex").name);
			}
			if (inObject.HasProperty("_FalloffTex"))
			{
				AddTextureToPack((Texture2D)inObject.GetTexture("_FalloffTex"));
				matProperty.Add("_FalloffTex", inObject.GetTexture("_FalloffTex").name);
			}
			if (inObject.HasProperty("_Cutoff"))
			{
				matProperty.Add("_Cutoff", inObject.GetFloat("_Cutoff"));
			}
			if (inObject.HasProperty("_ReflectColor"))
			{
				matProperty.Add("_ReflectColor", inObject.GetColor("_ReflectColor"));
			}
			return EncodeHashtable(matProperty);
		}*/

		/*

		public byte[] EncodeRigidbody(Rigidbody inObject)
		{
			byte[] result = null;
			return result;
		}*/

		/*byte[] EncodeSimple(object inObject)
		{
			Hashtable param = new Hashtable();
			Type tp = inObject.GetType();

			FieldInfo[] fis = tp.GetFields();
			foreach (FieldInfo fieldInfo in fis)
			{
				param.Add(fieldInfo.Name, fieldInfo.GetValue(inObject));
			}
			PropertyInfo[] pis = tp.GetProperties();
			foreach (PropertyInfo propertyInfo in pis.Where(propertyInfo => propertyInfo.CanWrite))
			{
				param.Add(propertyInfo.Name,propertyInfo.GetValue(inObject,null));
			}

			return EncodeHashtable(param);
		}*/

		/*string[] PhysicMaterialMembers = new string[8]
	                                 	{
	                                 		"dynamicFriction","staticFriction","bounciness",
											"frictionDirection2","staticFriction2","frictionCombine",
											"bounceCombine","frictionDirection"
										};

		byte[] EncodePhysicMaterial(PhysicMaterial inObject)
		{
			var param = new Hashtable();
			return EncodeHashtable(param);
		}*/

		
		/*public byte[] EncodeComponent(Component inObject)
		{
			byte[] result = new byte[0];
			switch (inObject.GetType().ToString())
			{
				/*case "UnityEngine.Projector":
					byte[] pByte = EncodeProjector((Projector)inObject);
					result = new byte[1 + pByte.Length];
					result[0] = 1;
					System.Buffer.BlockCopy(pByte, 0, result, 1, result.Length);
					break;*/
		/*case "UnityEngine.MeshFilter":
			result = EncodeMesh(((MeshFilter)inObject).sharedMesh);
			break;*/
		/*case "UnityEngine.MeshRenderer":
			result = EncodeMeshRenderer((MeshRenderer)inObject);
			break;*/
		/*case "UnityEngine.Rigidbody":
			result = EncodeRigidbody((Rigidbody)inObject);
			break;
		case "UnityEngine.Collider":
			result = EncodeCollider((Collider)inObject);
			break;
		case "UnityEngine.Camera":
			result = EncodeRigidbody((Rigidbody)inObject);
			break;*/

		/*default:
			break;
	}
	return result;
	}*/

	}
}
