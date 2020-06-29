using System;
using UnityEngine;

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
		public static Vector2 DecodeVector2(byte[] inBytes, ref int startPos)
		{
			var result = new Vector2
			{
				x = BitConverter.ToSingle(inBytes, startPos),
				y = BitConverter.ToSingle(inBytes, startPos + 4)
			};
			startPos += 8;
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static Vector3 DecodeVector3(byte[] inBytes, ref int startPos)
		{
			//Debug.Log("DecodeVector3 ");
			var result = new Vector3
			{
				x = BitConverter.ToSingle(inBytes, startPos),
				y = BitConverter.ToSingle(inBytes, startPos + 4),
				z = BitConverter.ToSingle(inBytes, startPos + 8)
			};
			startPos += 12;
			//Debug.Log("DecodeVector3 decoded "+result.ToString());
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static Vector4 DecodeVector4(byte[] inBytes, ref int startPos)
		{
			var result = new Vector4
			{
				x = BitConverter.ToSingle(inBytes, startPos),
				y = BitConverter.ToSingle(inBytes, startPos + 4),
				z = BitConverter.ToSingle(inBytes, startPos + 8),
				w = BitConverter.ToSingle(inBytes, startPos + 12)
			};
			startPos += 16;
			return result;
		}

		static Quaternion DecodeQuaternion(byte[] inBytes, ref int startPos)
		{
			//Debug.Log("DecodeQuaternion");
			var result = new Quaternion
			{
				x = BitConverter.ToSingle(inBytes, startPos),
				y = BitConverter.ToSingle(inBytes, startPos + 4),
				z = BitConverter.ToSingle(inBytes, startPos + 8),
				w = BitConverter.ToSingle(inBytes, startPos + 12)
			};
			startPos += 16;
			return result;
		}

		public static Color DecodeColor(byte[] inBytes, ref int startPos)
		{
			var result = new Color
			{
				r = BitConverter.ToSingle(inBytes, startPos),
				g = BitConverter.ToSingle(inBytes, startPos + 4),
				b = BitConverter.ToSingle(inBytes, startPos + 8),
				a = BitConverter.ToSingle(inBytes, startPos + 12)
			};
			startPos += 16;
			return result;
		}

        public static Matrix4x4 DecodeMatrix(byte[] inBytes,ref int startPos)
       {
           var result = Matrix4x4.zero;
           for (var i = 0; i < 4; i++)
           {
               for (var j = 0; j < 4; j++)
               {
                   result[i, j] = BitConverter.ToSingle(inBytes, startPos);
                   startPos += 4;
               }
           }
           return result;
       }

        public static Bounds DecodeBounds(byte[] inBytes,ref int startPos)
        {
            return new Bounds(DecodeVector3(inBytes, ref startPos), DecodeVector3(inBytes, ref startPos));
        }

        public static BoneWeight DecodeBoneWeight(byte[] inBytes, ref int startPos)
        {
            return new BoneWeight
                             {
                                 weight0 = DecodeSingle(inBytes, ref startPos),
                                 weight1 = DecodeSingle(inBytes, ref startPos),
                                 weight2 = DecodeSingle(inBytes, ref startPos),
                                 weight3 = DecodeSingle(inBytes, ref startPos),
                                 boneIndex0 = DecodeInteger(inBytes, ref startPos),
                                 boneIndex1 = DecodeInteger(inBytes, ref startPos),
                                 boneIndex2 = DecodeInteger(inBytes, ref startPos),
                                 boneIndex3 = DecodeInteger(inBytes, ref startPos)
                             };
        }

		/*public Cubemap DecodeCubemap(byte[] inBytes, ref int startPos)
		{
			Cubemap result = new Cubemap(DecodeInteger(inBytes, ref startPos) / 12, TextureFormat.RGB24, true);

			Color[] PositiveX = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(PositiveX, CubemapFace.PositiveX);
			Color[] NegativeX = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(NegativeX, CubemapFace.NegativeX);
			Color[] PositiveY = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(PositiveY, CubemapFace.PositiveY);
			Color[] NegativeY = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(NegativeY, CubemapFace.NegativeY);
			Color[] PositiveZ = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(PositiveZ, CubemapFace.PositiveZ);
			Color[] NegativeZ = (Color[])DecodeArrayOf(inBytes, ref startPos);
			result.SetPixels(NegativeZ, CubemapFace.NegativeZ);

			return result;
		}*/

		/*
		int sideLength = inObject.width * inObject.height;
			byte[] result = new byte[sideLength * 6];
			int startPos = 0;
			Color[] PositiveX = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(PositiveX), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 16 + 5;
			Color[] NegativeX = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(NegativeX), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 16 + 5;
			Color[] PositiveY = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(PositiveY), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 16 + 5;
			Color[] NegativeY = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(NegativeY), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 16 + 5;
			Color[] PositiveZ = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(PositiveZ), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 16 + 5;
			Color[] NegativeZ = inObject.GetPixels(CubemapFace.PositiveX);
			System.Buffer.BlockCopy(EncodeArrayOf(NegativeZ), 0, result, startPos, sideLength * 16 + 5);
			startPos += sideLength * 1
		*/

		static Mesh DecodeMesh(byte[] inBytes, ref int startPos)
		{
			//Debug.Log("DecodeMesh");
			//progress = startPos;
			var result = new Mesh { name = DecodeString(inBytes, ref startPos) };

			var vertecCount = DecodeInteger(inBytes, ref startPos);
			var vertArray = new Vector3[vertecCount];
			for (var i = 0; i < vertecCount; i++)
			{
				vertArray[i] = DecodeVector3(inBytes, ref startPos);
			}
			result.vertices = vertArray;

			var uvCount = DecodeInteger(inBytes, ref startPos);
			var uvArray = new Vector2[uvCount];
			for (var i = 0; i < uvCount; i++)
			{
				uvArray[i] = DecodeVector2(inBytes, ref startPos);
			}
			result.uv = uvArray;

			var triangCount = DecodeInteger(inBytes, ref startPos);
			var triangArray = new int[triangCount];
			for (var i = 0; i < triangCount; i++)
			{
				triangArray[i] = DecodeInteger(inBytes, ref startPos);
			}
			result.triangles = triangArray;

			/*int normalsCount = DecodeInteger(inBytes, ref startPos);
			Vector3[] normalsArray = new Vector3[normalsCount];
			for (int i = 0; i < normalsCount; i++) {
				normalsArray[i] = DecodeVector3(inBytes, ref startPos);
			}
			result.normals = normalsArray;

			int tangentsCount = DecodeInteger(inBytes, ref startPos);
			Vector4[] tangentsArray = new Vector4[tangentsCount];
			for (int i = 0; i < tangentsCount; i++) {
				tangentsArray[i] = DecodeVector4(inBytes, ref startPos);
			}
			result.tangents = tangentsArray;*/

			result.RecalculateBounds();
			result.RecalculateNormals();
			result.Optimize();
            //ULog.Log("DecodeMesh OK "+result);
			return result;
		}

		public static Texture2D DecodeTexture2D(byte[] inBytes, ref int startPos)
		{
			var w = DecodeInteger(inBytes, ref startPos);
			var h = DecodeInteger(inBytes, ref startPos);
			var format = DecodeInteger(inBytes, ref startPos);
			var result = new Texture2D(w, h,(TextureFormat)format,false);
			var twrap = TextureWrapMode.Clamp;
			if (inBytes[startPos] == 0)
				twrap = TextureWrapMode.Repeat;
			startPos++;
			var texLength = DecodeInteger(inBytes, ref startPos);
			var texName = DecodeString(inBytes, ref startPos);
			var texByte = new byte[texLength];
			Buffer.BlockCopy(inBytes, startPos, texByte, 0, texLength);
			startPos += texLength;
			result.wrapMode = twrap;
			result.LoadImage(texByte);
			result.name = texName;
			return result;

		}

		/*public Texture DecodeTexturePixels(byte[] inBytes, ref int startPos)
		{
			int w = DecodeInteger(inBytes, ref startPos);
			int h = DecodeInteger(inBytes, ref startPos);
			int colorLength = w * h;
			Color[] colorPixels = new Color[colorLength];
			for (int i = 0; i < colorLength; i++)
			{
				colorPixels[i] = DecodeColor(inBytes, ref startPos);
			}

			Texture2D result = new Texture2D(w, h);
			result.SetPixels(colorPixels);
			result.Apply();

			return result;
		}*/

		Material DecodeMaterial(byte[] inBytes,ref int startPos)
		{
			//Debug.Log("DecodeMaterial");
			var materialParameters = DecodeHashtable(inBytes,ref startPos);
			var shaderName = materialParameters["shader"].ToString();
			var sh = Shader.Find(shaderName) ?? Shader.Find("Diffuse");
			var result = new Material(sh)
			       	{
			       		name = (string) materialParameters["name"]
			       	};
			
			foreach (var property in ShaderPropertys)
			{
				if (materialParameters.ContainsKey(property.Key))
				{
					//Debug.Log("Shader prop exists " + property.Key+" "+property.Value);
					switch (property.Value)
					{
						case "Texture":
							result.SetTexture(property.Key,(Texture2D)materialParameters[property.Key]);
							/*if(((int)materialParameters[property.Key])>=0)
								result.SetTexture(property.Key, (Texture)_objectPack[(int)materialParameters[property.Key]]);*/
							break;
						case "Float":
							result.SetFloat(property.Key,(float)materialParameters[property.Key]);
							break;
						default:
							result.SetColor(property.Key, (Color)materialParameters[property.Key]);
							break;
					}
				}
			}
			//Debug.Log("DecodeMaterial return "+result);
			return result;
		}

		/*Material DecodeMaterial(byte[] inBytes, ref int startPos)
		{
			Hashtable matParam = DecodeHashtable(inBytes, ref startPos);
			string shaderName = (string)matParam["shader"];
			Shader sh = Shader.Find(shaderName);
			if (sh == null)
			{
				sh = Shader.Find("Diffuse");
			}
			Material result = new Material(sh);
			result.name = (string)matParam["name"];
			foreach (DictionaryEntry prop in matParam)
			{
				if (result.HasProperty((string)prop.Key))
				{
					if (prop.Value is float)
						result.SetFloat((string)prop.Key, (float)prop.Value);
					else if (prop.Value is Color)
						result.SetColor((string)prop.Key, (Color)prop.Value);
					else if (prop.Value is Vector4)
						result.SetVector((string)prop.Key, (Vector4)prop.Value);
					else if (prop.Value is string && prop.Key.ToString() != "name" && prop.Key.ToString() != "shader")
					{
						if (texturePack != null)
						{
							if (texturePack.ContainsKey((string)prop.Value))
							{
								Texture2D txtr = (Texture2D)texturePack[(string)prop.Value];
								result.SetTexture((string)prop.Key, txtr);
							} else
							{
								//Debug.Log("TexturePack NOT has texture by name " + (string)prop.Value);
							}
						} else
						{
							//Debug.Log("TexturePack is NULL");
						}
					}
				}
			}
			return result;
		}*/

		public static Rect DecodeRect(byte[] inBytes, ref int startPos)
		{
			var result = new Rect
			{
				x = BitConverter.ToSingle(inBytes, startPos),
				y = BitConverter.ToSingle(inBytes, startPos + 4),
				width = BitConverter.ToSingle(inBytes, startPos + 8),
				height = BitConverter.ToSingle(inBytes, startPos + 12)
			};
			startPos += 16;
			return result;
		}

		/*Collider DecodeCollider(byte[] inBytes,ref  int startPos)
		{
			var param = DecodeHashtable(inBytes, ref startPos);
			var tp = Type.GetType(param["class"].ToString());
			var result = (Collider)Activator.CreateInstance(tp);
			foreach (DictionaryEntry entry in param)
			{
				if(entry.Key.ToString()!="class")
				{
					var prop = tp.GetProperty(entry.Key.ToString());
					prop.SetValue(result,entry.Value,null);
				}
			}
			return result;
		}*/

		GameObject DecodeGameObject(byte[] inBytes, ref int startPos)
		{
			var goParameters = DecodeHashtable(inBytes, ref startPos);
			var result = new GameObject(goParameters["name"].ToString());
			//Debug.Log("Decode GameObject " + result.name);
		    if (goParameters.ContainsKey("layer"))
		        result.layer = (int) goParameters["layer"];


		    if (goParameters.ContainsKey("tag"))
				result.tag = (string)goParameters["tag"];
			#region Child
			if (goParameters.ContainsKey("childcount"))
			{
				var childCount = (int)goParameters["childcount"];//DecodeInteger(inBytes, ref startPos);
				//Debug.Log(result.name + " check " + childCount + " child for decode ");
				for (var i = 0; i < childCount; i++)
				{
					//Debug.Log("Decode child "+i+" in "+result.name);
					var child = (GameObject)Decoding(inBytes, ref startPos);
					//Debug.Log("Decode child " + i + " in " + result.name + " end " + child.name);
					var childTransform = child.transform;
					var lpos = childTransform.position;
					var lrot = childTransform.rotation;
					var lscale = childTransform.localScale;

					childTransform.parent = result.transform;
					childTransform.position = lpos;
					childTransform.rotation = lrot;
					childTransform.localScale = lscale;
				}
			}
			#endregion

			#region Components
			if (goParameters.ContainsKey("componentscount"))
			{
				var componentsCount = (int) goParameters["componentscount"]; //DecodeInteger(inBytes, ref startPos);
				//Debug.Log(result.name + " check " + componentsCount + " components for decode");
				for (var i = 0; i < componentsCount; i++)
					SetComponent(inBytes, ref startPos, ref result);
			}

			#endregion

			return result;
		}

		/*MeshRenderer DecodeMeshRenderer(byte[] inBytes, ref int startPos)
		{
			var result = new MeshRenderer();
			var materials = DecodeArray(inBytes, ref startPos);
			result.sharedMaterials = new Material[materials.Length];
			result.castShadows = DecodeBoolean(inBytes, ref startPos);
			result.receiveShadows = DecodeBoolean(inBytes, ref startPos);
			result.lightmapIndex = DecodeInteger(inBytes, ref startPos);
			result.lightmapTilingOffset = DecodeVector4(inBytes, ref startPos);
			return result;
		}

		
		static MeshFilter DecodeMeshFilter(byte[] inBytes, ref int startPos)
		{
			var result = new MeshFilter();
			if (DecodeBoolean(inBytes, ref startPos))
				result.sharedMesh = DecodeMesh(inBytes, ref startPos);
			return result;
		}*/

		static PhysicMaterial DecodePhysicMaterial(byte[] inBytes,ref int startPos)
		{
			var result = new PhysicMaterial(DecodeString(inBytes, ref startPos))
			             	{
			             		dynamicFriction = DecodeSingle(inBytes, ref startPos),
			             		staticFriction = DecodeSingle(inBytes, ref startPos),
			             		bounciness = DecodeSingle(inBytes, ref startPos),
			             		/*dynamicFriction2 = DecodeSingle(inBytes, ref startPos),
			             		staticFriction2 = DecodeSingle(inBytes, ref startPos),
			             		frictionDirection = DecodeVector3(inBytes, ref startPos),
			             		frictionDirection2 = DecodeVector3(inBytes, ref startPos),*/
			             		frictionCombine = (PhysicMaterialCombine) DecodeInteger(inBytes, ref startPos),
			             		bounceCombine = (PhysicMaterialCombine) DecodeInteger(inBytes, ref startPos)
			             	};
		    startPos += 32;
			return result;
		}

		
		/*static Transform DecodeTransform(byte[] inBytes,ref int startPos)
		{
			DecodeVector3(inBytes, ref startPos);
			DecodeQuaternion(inBytes, ref startPos);
			DecodeVector3(inBytes, ref startPos);
			return null;
		}*/

        static TerrainData DecodeTerrainData(byte[] inBytes,ref int startPos)
        {
            var result = new TerrainData();

            return result;
        }
	}
}
