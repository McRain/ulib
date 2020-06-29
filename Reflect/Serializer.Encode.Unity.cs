using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*#if UNITY_EDITOR
using UnityEditor;
#endif*/


namespace ULIB
{
	public partial class Serializer
	{
		//private Dictionary<int,Mesh> _meshes = new Dictionary<int, Mesh>();
		//private delegate byte[] EncoderComponentFunction(object data);
		
		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyType"></param>
		public static void AddShaderProperty(string propertyName, string propertyType)
		{
			if (!ShaderPropertys.ContainsKey(propertyName))
				ShaderPropertys.Add(propertyName, propertyType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		public static void RemoveShaderProperty(string propertyName)
		{
			if (ShaderPropertys.ContainsKey(propertyName))
				ShaderPropertys.Remove(propertyName);
		}

		
		#endregion

		#region Struct

        /// <summary>
        /// Return 8 bytes 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeVector2(Vector2 inObject)
		{
			var result = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, result, 4, 4);
			return result;
		}

        /// <summary>
        /// Return 12 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeVector3(Vector3 inObject)
		{
			var result = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, result, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, result, 8, 4);
			return result;
		}

        /// <summary>
        /// Return 16 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static byte[] EncodeVector4(Vector4 inObject)
		{
			var result = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, result, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, result, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.w), 0, result, 12, 4);
			return result;
		}

        /// <summary>
        /// Return 16 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static byte[] EncodeQuaternion(Quaternion inObject)
		{
			var result = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, result, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.z), 0, result, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.w), 0, result, 12, 4);
			return result;
		}

        /// <summary>
        /// Return 16 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static byte[] EncodeColor(Color inObject)
		{
			var result = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.r), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.g), 0, result, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.b), 0, result, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.a), 0, result, 12, 4);
			return result;
		}

        /// <summary>
        /// Return 16 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        public static byte[] EncodeRect(Rect inObject)
		{
			var result = new byte[16];
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.x), 0, result, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.y), 0, result, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.width), 0, result, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(inObject.height), 0, result, 12, 4);
			return result;
		}

        /*static byte[] EncodeRigidbodyConstraints(RigidbodyConstraints rigidbodyConstraints)
        {
            
        }*/

            /// <summary>
            /// 
            /// </summary>
            /// <param name="inObject"></param>
            /// <returns></returns>
        public static byte[] EncodeMatrix(Matrix4x4 inObject)
        {
            var result = new byte[4*4*4];
            var pos = 0;
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(inObject[i, j]), 0, result, pos, 4);
                    pos += 4;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBounds"></param>
        /// <returns></returns>
        public static byte[] EncodeBounds(Bounds inBounds)
        {

            var result = new byte[24];
            //Center
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.center.x), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.center.y), 0, result, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.center.z), 0, result, 8, 4);
            //size
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.size.x), 0, result, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.size.y), 0, result, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(inBounds.size.z), 0, result, 20, 4);

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iBoneWeight"></param>
        /// <returns></returns>
        public static byte[] EncodeBoneWeight(BoneWeight iBoneWeight)
        {
            var result = new byte[32];
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.weight0),0,result,0,4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.weight1), 0, result, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.weight2), 0, result, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.weight3), 0, result, 12, 4);

            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.boneIndex0), 0, result, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.boneIndex1), 0, result, 20, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.boneIndex2), 0, result, 24, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iBoneWeight.boneIndex3), 0, result, 28, 4);

            return result;
        }

        /*public byte[] EncodeKeyframe(Keyframe iKeyframe)
        {
            var result = new byte[20];
            Buffer.BlockCopy(BitConverter.GetBytes(iKeyframe.time),0,result,0,4);
            Buffer.BlockCopy(BitConverter.GetBytes(iKeyframe.value), 0, result, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iKeyframe.inTangent), 0, result, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iKeyframe.outTangent), 0, result, 12, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(iKeyframe.tangentMode), 0, result, 16, 4);
            return result;
        }*/
		#endregion

		#region Objects

        /// <summary>
        /// Return array of bytes.
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
		public static byte[] EncodePhysicMaterial(PhysicMaterial material)
		{
            //Debug.Log("ENCODE: EncodePhysicMaterial " + material);
			var nameBytes = EncodeString(material.name);
			var startPos = nameBytes.Length;
			var result = new byte[startPos + 52];
			Buffer.BlockCopy(nameBytes, 0, result, 0, startPos);
			Buffer.BlockCopy(EncodeFloat(material.dynamicFriction), 0, result, startPos, 4);
			startPos += 4;
			Buffer.BlockCopy(EncodeFloat(material.staticFriction), 0, result, startPos, 4);
			startPos += 4;
			Buffer.BlockCopy(EncodeFloat(material.bounciness), 0, result, startPos, 4);
			startPos += 4;
			//Buffer.BlockCopy(EncodeFloat(material.dynamicFriction2), 0, result, startPos, 4);
			startPos += 4;
			//Buffer.BlockCopy(EncodeFloat(material.staticFriction2), 0, result, startPos, 4);
			startPos += 4;
			/*Buffer.BlockCopy(EncodeVector3(material.frictionDirection), 0, result, startPos, 12);
			startPos += 12;*/
			//Buffer.BlockCopy(EncodeVector3(material.frictionDirection2), 0, result, startPos, 12);
			startPos += 12;
			Buffer.BlockCopy(EncodeInteger((int)material.frictionCombine), 0, result, startPos, 4);
			startPos += 4;
			Buffer.BlockCopy(EncodeInteger((int)material.bounceCombine), 0, result, startPos, 4);
			return result;
		}

        /// <summary>
        /// Return mesh as array of bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeMesh(Mesh inObject)
		{
			var startPos = 0;
			var vertexCount = inObject.vertices.Length;
			var uvCount = inObject.uv.Length;
			var trianglesCount = inObject.triangles.Length;

			var nameBytes = EncodeString(inObject.name);
			
			var result = new byte[nameBytes.Length + 12 + (vertexCount * 12) + (uvCount * 8) + (trianglesCount * 4)];
			Buffer.BlockCopy(nameBytes, 0, result, startPos, nameBytes.Length);
			startPos += nameBytes.Length;

			//UpdateEvent(startPos, result.Length);

			//----Vertex
			Buffer.BlockCopy(BitConverter.GetBytes(vertexCount), 0, result, startPos, 4);
			startPos += 4;
			Vector3 vec;
			byte[] vecBytes;
			var vectorArray = inObject.vertices;
			for (var i = 0; i < vertexCount; i++)
			{
				vec = vectorArray[i];
				vecBytes = EncodeVector3(vec);
				Buffer.BlockCopy(vecBytes, 0, result, startPos + i * 12, 12);
			}
			startPos += vertexCount * 12;//.Length;

			//UpdateEvent(startPos, result.Length);

			//----UV
			Buffer.BlockCopy(BitConverter.GetBytes(uvCount), 0, result, startPos, 4);
			startPos += 4;
			Vector2 vec2;
			byte[] vec2Bytes;
			var vecs = inObject.uv;
			for (var i = 0; i < uvCount; i++)
			{
				vec2 = vecs[i];
				vec2Bytes = EncodeVector2(vec2);
				Buffer.BlockCopy(vec2Bytes, 0, result, startPos + i * 8, 8);
			}
			startPos += uvCount * 8;

			//----triangles
			Buffer.BlockCopy(BitConverter.GetBytes(trianglesCount), 0, result, startPos, 4);
			startPos += 4;
			var intBytes = ConvertArray(inObject.triangles);
			Buffer.BlockCopy(intBytes, 0, result, startPos, trianglesCount * 4);
            
			return result;
		}
        
		byte[] EncodeTexture(Texture2D inObject)
		{
			var nameBytes = EncodeString(inObject.name);
			var cols = inObject.GetPixels();
			var colBytes = EncodeArray(cols);

			var result = new byte[4 + 4 + 1 + 4 + nameBytes.Length + colBytes.Length];
			Buffer.BlockCopy(EncodeInteger(inObject.width), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeInteger(inObject.height), 0, result, 4, 4);
			if (inObject.wrapMode == TextureWrapMode.Clamp)
				result[9] = 1;
			else
				result[9] = 0;
			Buffer.BlockCopy(EncodeInteger(colBytes.Length), 0, result, 9, 4);
			Buffer.BlockCopy(nameBytes, 0, result, 13, nameBytes.Length);
			Buffer.BlockCopy(colBytes, 0, result, 13 + nameBytes.Length, colBytes.Length);
			return result;
		}

	    public static byte[] EncodeSprite(Sprite sprite)
	    {
	        var result = new byte[91+4+ sprite.vertices.Length * 8+4+sprite.triangles.Length*2+4+sprite.uv.Length*8];
            Buffer.BlockCopy(EncodeVector4(sprite.border),0,result,0,16);
            Buffer.BlockCopy(EncodeBounds(sprite.bounds), 0, result, 16, 24);
            //Buffer.BlockCopy(EncodeBoolean(sprite.packed), 0, result, 40,1);
	        result[40] = (byte) sprite.packingMode;
            result[41] = (byte)sprite.packingRotation;
            Buffer.BlockCopy(EncodeVector2(sprite.pivot), 0, result, 42, 8);
	        result[50] = (byte) sprite.pixelsPerUnit;
            Buffer.BlockCopy(EncodeRect(sprite.rect), 0, result, 51, 16);
            Buffer.BlockCopy(EncodeRect(sprite.textureRect), 0, result, 67, 16);
            Buffer.BlockCopy(EncodeVector2(sprite.textureRectOffset), 0, result, 83, 8);

            var vertCount = sprite.vertices.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(vertCount), 0, result, 91, 4);
            var index = 95;
            for (int i = 0; i < vertCount; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(sprite.vertices[i].x), 0, result, index, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(sprite.vertices[i].y), 0, result, index + 4, 4);
                index += 8;
            }

            var trianglesCount = sprite.triangles.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(trianglesCount), 0, result, index, 4);
	        index += 4;
            for (int i = 0; i < trianglesCount; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(sprite.triangles[i]), 0, result, index, 4);
                index += 4;
            }

            var uvCount = sprite.uv.Length;
            Buffer.BlockCopy(BitConverter.GetBytes(uvCount), 0, result, index, 4);
	        index += 4;
            for (int i = 0; i < uvCount; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(sprite.uv[i].x), 0, result, index, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(sprite.uv[i].y), 0, result, index + 4, 4);
                index += 8;
            }
	        return result;
        }

        /// <summary>
        /// Return texture data as array of bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeTexture2D(Texture2D inObject)
		{
            //Debug.Log("EncodeTexture2D "+inObject.name);
			var nameBytes = EncodeString(inObject.name);
			var nameBytesLenght = nameBytes.Length;
            
            if(inObject.format!=TextureFormat.ARGB32 && inObject.format!=TextureFormat.RGBA32)
            {
                var neobj = new Texture2D(inObject.width, inObject.height, TextureFormat.ARGB32,
                                          inObject.mipmapCount > 0);
                var pxls = inObject.GetPixels();
                neobj.SetPixels(pxls);
                neobj.Apply();
                inObject = neobj;
            }
			var texByte = inObject.EncodeToPNG();
			var texBytesLenght = texByte.Length;
			var result = new byte[17 + nameBytesLenght + texBytesLenght];
			Buffer.BlockCopy(EncodeInteger(inObject.width), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeInteger(inObject.height), 0, result, 4, 4);
			Buffer.BlockCopy(EncodeInteger((int)inObject.format),0,result,8,4);
			if (inObject.wrapMode == TextureWrapMode.Clamp)
				result[12] = 1;
			else
				result[12] = 0;
			Buffer.BlockCopy(EncodeInteger(texBytesLenght), 0, result, 13, 4);
			Buffer.BlockCopy(nameBytes, 0, result, 17, nameBytesLenght);
			Buffer.BlockCopy(texByte, 0, result, 17 + nameBytesLenght, texBytesLenght);
			return result;
		}

		/*byte[] EncodeCubemap(Cubemap inObject)
		{
			//Debug.Log("EncodeCubemap " + inObject.name + "");
			var positiveX = EncodeArray(inObject.GetPixels(CubemapFace.PositiveX));
			var negativeX = EncodeArray(inObject.GetPixels(CubemapFace.NegativeX));
			var positiveY = EncodeArray(inObject.GetPixels(CubemapFace.PositiveY));
			var negativeY = EncodeArray(inObject.GetPixels(CubemapFace.NegativeY));
			var positiveZ = EncodeArray(inObject.GetPixels(CubemapFace.PositiveZ));
			var negativeZ = EncodeArray(inObject.GetPixels(CubemapFace.NegativeZ));

			var result = new byte[4 + positiveX.Length * 6];
			var startPos = 0;
			Buffer.BlockCopy(EncodeInteger(inObject.width), 0, result, startPos, 4);
			startPos += 4;
			var pos = positiveX.Length;
			Buffer.BlockCopy(positiveX, 0, result, startPos, pos);
			startPos += pos;
			Buffer.BlockCopy(negativeX, 0, result, startPos, pos);
			startPos += pos;
			Buffer.BlockCopy(positiveY, 0, result, startPos, pos);
			startPos += pos;
			Buffer.BlockCopy(negativeY, 0, result, startPos, pos);
			startPos += pos;
			Buffer.BlockCopy(positiveZ, 0, result, startPos, pos);
			startPos += pos;
			Buffer.BlockCopy(negativeZ, 0, result, startPos, pos);
			return result;
		}*/

        /// <summary>
        /// Return material ( include textures ) as array of bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		private byte[] EncodeMaterial(Material inObject)
		{
		    var matProperty = new Hashtable
		                          {
		                              {"name", inObject.name},
		                              {"shader", inObject.shader.name}
		                          };
		    foreach (var property in ShaderPropertys)
		    {
		        if (!inObject.HasProperty(property.Key)) continue;
		        object value = null;
		        switch (property.Value)
		        {
		            case "Texture":
		                //Debug.Log("Find texture parameters " + property.Key + " in material " + inObject.name);
		                var txtr = inObject.GetTexture(property.Key);
		                if (txtr != null) value = txtr;
		                break;
		            case "Float":
		                value = inObject.GetFloat(property.Key);
		                break;
		            default:
		                value = inObject.GetColor(property.Key);
		                break;
		        }
		        matProperty.Add(property.Key, value);
		    }
		    return EncodeHashtable(matProperty);
		}

	    byte[] EncodeProceduralMaterial(Material inObject)
		{
			var matProperty = new Hashtable
			                  	{
			                  		{ "name", inObject.name }, 
									{ "shader", inObject.shader.name }
			                  	};
			foreach (var property in ShaderPropertys)
			{
				if (inObject.HasProperty(property.Key))
				{
					object value = null;
					switch (property.Value)
					{
						case "Texture":
							//Debug.Log("Find texture parameters " + property.Key + " in material " + inObject.name);
							var txtr = inObject.GetTexture(property.Key);
							if (txtr != null) value = txtr;
							break;
						case "Float":
							value = inObject.GetFloat(property.Key);
							break;
						default:
							value = inObject.GetColor(property.Key);
							break;
					}
					matProperty.Add(property.Key, value);
				}
			}
			return EncodeHashtable(matProperty);
		}

		private byte[] EncodeGameObject(GameObject inObject)
		{
			var goParameters = new Hashtable
			                   	{
									{"name",inObject.name},
									{"layer",inObject.layer},
									{"active",inObject.active},
									{"tag",inObject.tag},
									{"childcount",0},
									{"componentscount",0}
			                   	};

			#region Child
            var childCount = inObject.transform.childCount;
		    goParameters["childcount"] = childCount;
			//Debug.Log(inObject.name+" find " + inObject.transform.childCount+" child");
			var childsBytes = new byte[inObject.transform.childCount*5];
			var childNames = new List<string>();
			var pos = 0;
            for (var i = 0; i < childCount; i++)
            {
                var child = inObject.transform.GetChild(i).gameObject;
                //Debug.Log("Find children "+child);
		        if(childNames.Contains(child.name))
		        {
		            child.name += _nameKey.ToString();
		            _nameKey++;
		        }
                childNames.Add(child.name);
                var childBytes = Encoding(child);
                Buffer.BlockCopy(childBytes, 0, childsBytes, pos, 5);//Get 5 bytes : X-252 X..X resource index
                pos += 5;
		    }

			/*foreach (Transform child in inObject.transform)
			{
				if (childNames.Contains(child.name))
				{
					child.name += _nameKey.ToString();
					_nameKey++;
				}
				childNames.Add(child.name);
				var childBytes = Encoding(child.gameObject);
				Buffer.BlockCopy(childBytes, 0, childsBytes, pos, 5);//Get 5 bytes : X-252 X..X resource index
				pos += 5;
			}*/
			#endregion

			#region Components

			var components = inObject.GetComponents<Component>();
		    var componentList = new List<Component>();
		    foreach (var srcComponent in components)
		    {
		        if(IgnoreComponents.Contains(srcComponent.GetType().FullName))
                    continue;
                componentList.Add(srcComponent);
		    }
            goParameters["componentscount"] = componentList.Count;
            var componentsBytes = new byte[componentList.Count * 5];
			pos = 0;
            foreach (var component in componentList)
			{
				Buffer.BlockCopy(EncodeComponent(component), 0, componentsBytes, pos, 5);//Get 5 bytes : X-252 X..X resource index
				pos += 5;
			}
            //Debug.Log(BitConverter.ToString(componentsBytes));
			
            #endregion

			var parameterBytes = EncodeHashtable(goParameters);
			var startPos = parameterBytes.Length;
			var result = new byte[startPos+childsBytes.Length + componentsBytes.Length];
			Buffer.BlockCopy(parameterBytes, 0, result, 0, startPos);
			Buffer.BlockCopy(childsBytes, 0, result, startPos, childsBytes.Length);
			startPos += childsBytes.Length;
			Buffer.BlockCopy(componentsBytes, 0, result, startPos, componentsBytes.Length);
            //Debug.Log(BitConverter.ToString(result));
			return result;
		}

        private byte[] EncodeTerrainData(TerrainData inObject)
        {
            var result = new byte[24];
            Buffer.BlockCopy(EncodeInteger(inObject.heightmapWidth),0,result,0,4);
            Buffer.BlockCopy(EncodeInteger(inObject.heightmapHeight), 0, result, 4, 4);
            Buffer.BlockCopy(EncodeInteger(inObject.heightmapResolution), 0, result, 8, 4);
            Buffer.BlockCopy(EncodeInteger(inObject.heightmapScale), 0, result, 12, 12);
            //heightmapWidth	 Width of the terrain in samples (Read Only). int 4
            //heightmapHeight	 Height of the terrain in samples (Read Only). int 4
            //heightmapResolution	 Resolution of the heightmap -  int 4
            //heightmapScale	 The size of each heightmap sample -  Vector3 12

            //size	 The total size in world units of the terrain -  Vector3 12
            //wavingGrassStrength	 Strength of the waving grass in the terrain. -   float 4
            //wavingGrassAmount	 Amount of waving grass in the terrain. -   float 4
            //wavingGrassSpeed	 Speed of the waving grass. -   float 4
            //wavingGrassTint	 Color of the waving grass that the terrain has. -  Color 16
            //detailWidth	 Detail width of the TerrainData. -  int 4
            //detailHeight	 Detail height of the TerrainData. -  int 4
            //detailResolution	 Detail Resolution of the TerrainData. -  int 4
            //alphamapLayers	 Number of alpha map layers -  int 4
            //alphamapResolution	 Resolution of the alpha map. -  int 4
            //alphamapWidth	 Width of the alpha map. -  int 4
            //alphamapHeight	 Height of the alpha map. -  int 4
            //baseMapResolution	 Resolution of the base map used for rendering far patches on the terrain -  int 4
            //detailPrototypes	 Contains the detail texture/meshes that the terrain has. -  DetailPrototype
            //treeInstances	 Contains the current trees placed in the terrain. -  TreeInstance[]
            //treePrototypes	 The list of tree prototypes this are the ones available in the inspector. -  TreePrototype[]
            
            //splatPrototypes Splat texture used by the terrain. -  SplatPrototype[]
            //var splatPrototypesBytes = EncodeArray(inObject.splatPrototypes);
            return result;
        }

        /*public byte[] EncodeAnimationCurve(AnimationCurve iAnimationCurve)
        {
            var keyBytes = EncodeArray(iAnimationCurve.keys);
            var keyBytesLenght = keyBytes.Length;
            var result = new byte[keyBytesLenght + 8];
            Buffer.BlockCopy(BitConverter.GetBytes((int)iAnimationCurve.preWrapMode), 0, result, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((int)iAnimationCurve.postWrapMode), 0, result, 4, 4);
            Buffer.BlockCopy(keyBytes, 0, result, 8, keyBytesLenght);
            return result;
        }

        public byte[] EncodeAnimationClip(AnimationClip iAnimationClip)
        {
            var result = new byte[0];
            #if UNITY_EDITOR
            var curves = AnimationUtility.GetAllCurves(iAnimationClip);

            #endif
            
            return result;
        }*/

        Hashtable DetailPrototype(DetailPrototype obj)
        {
            var table = new Hashtable();
            table.Add("prototype",obj.prototype);
            return table;
        }

        void EncodeTreeInstance(TreeInstance obj, ref byte[] bytes)
        {

        }

        void EncodeTreeInstances(TreeInstance[] obj, ref byte[] bytes)
        {

        }

        void EncodeTreePrototype(TreePrototype obj, ref byte[] bytes)
        {

        }

        void EncodeTreePrototypes(TreePrototype[] obj, ref byte[] bytes)
        {

        }

        void EncodeSplatPrototype(SplatPrototype obj, ref byte[] bytes)
        {

        }

        void EncodeSplatPrototypes(SplatPrototype[] obj, ref byte[] bytes)
        {

        }

		#endregion

		

		/*
		byte[] EncodeGuiStyleState(GUIStyleState inObject)
		{
			//var byteStyle = Encode(inObject.background);
			var byteCol = EncodeColor(inObject.textColor);
			//var result = new byte[byteStyle.Length + byteCol.Length];
			//Buffer.BlockCopy(byteStyle, 0, result, 0, byteStyle.Length);
			//Buffer.BlockCopy(byteCol, 0, result, byteStyle.Length, byteCol.Length);
			return byteCol;
		}

		byte[] EncodeGuiStyle(GUIStyle inObject)
		{
			//ULog.Log("EncodeGuiStyle");
			var gsType = typeof (GUIStyle);
			var props = gsType.GetProperties();
			var bytesList = new List<byte[]>();
			var bytesCount = 0;
			foreach (var propertyInfo in props.Where(propertyInfo => propertyInfo.CanWrite))
			{
				//ULog.Log(propertyInfo.Name);
				var bytesPropName = EncodeString(propertyInfo.Name);
				var bytesPropVal = Encoding(propertyInfo.GetValue(inObject, null));
				var bytesProp = new byte[bytesPropName.Length + bytesPropVal.Length];
				Buffer.BlockCopy(bytesPropName, 0, bytesProp, 0, bytesPropName.Length);
				Buffer.BlockCopy(bytesPropVal, 0, bytesProp, bytesPropName.Length, bytesPropVal.Length);
				bytesList.Add(bytesProp);
				bytesCount += bytesProp.Length;
			}
			var result = new byte[bytesCount];
			bytesCount = 0;
			foreach (var bytese in bytesList)
			{
				Buffer.BlockCopy(bytese, 0, result, bytesCount, bytese.Length);
				bytesCount += bytese.Length;
			}
			return result;
		}

		byte[] EncodeGuiSkin(GUISkin inObject)
		{
			//ULog.Log("EncodeGuiSkin");
			var gsType = typeof(GUISkin);
			var props = gsType.GetProperties();
			var bytesList = new List<byte[]>();
			var bytesCount = 0;
			foreach (var propertyInfo in props.Where(propertyInfo => propertyInfo.CanWrite))
			{
				//ULog.Log(propertyInfo.Name);
				var bytesPropName = EncodeString(propertyInfo.Name);
				var bytesPropVal = Encoding(propertyInfo.GetValue(inObject, null));
				var bytesProp = new byte[bytesPropName.Length + bytesPropVal.Length];
				Buffer.BlockCopy(bytesPropName, 0, bytesProp, 0, bytesPropName.Length);
				Buffer.BlockCopy(bytesPropVal, 0, bytesProp, bytesPropName.Length, bytesPropVal.Length);
				bytesList.Add(bytesProp);
				bytesCount += bytesProp.Length;
			}
			var result = new byte[bytesCount];
			bytesCount = 0;
			foreach (var bytese in bytesList)
			{
				Buffer.BlockCopy(bytese, 0, result, bytesCount, bytese.Length);
				bytesCount += bytese.Length;
			}
			return result;
		}

		byte[] EncodeTerrainData(TerrainData inObject)
		{
			var result = new byte[0];
			
			//heightmapWidth	 Width of the terrain in samples (Read Only). int
			//heightmapHeight	 Height of the terrain in samples (Read Only). int
			//heightmapResolution	 Resolution of the heightmap -  int
			//heightmapScale	 The size of each heightmap sample -  Vector3
			//size	 The total size in world units of the terrain -  Vector3
			//wavingGrassStrength	 Strength of the waving grass in the terrain. -   float
			//wavingGrassAmount	 Amount of waving grass in the terrain. -   float
			//wavingGrassSpeed	 Speed of the waving grass. -   float
			//wavingGrassTint	 Color of the waving grass that the terrain has. -  Color
			//detailWidth	 Detail width of the TerrainData. -  int
			//detailHeight	 Detail height of the TerrainData. -  int
			//detailResolution	 Detail Resolution of the TerrainData. -  int
			//detailPrototypes	 Contains the detail texture/meshes that the terrain has. -  DetailPrototype
			//treeInstances	 Contains the current trees placed in the terrain. -  TreeInstance[]
			//treePrototypes	 The list of tree prototypes this are the ones available in the inspector. -  TreePrototype[]
			//alphamapLayers	 Number of alpha map layers -  int 
			//alphamapResolution	 Resolution of the alpha map. -  int
			//alphamapWidth	 Width of the alpha map. -  int
			//alphamapHeight	 Height of the alpha map. -  int
			//baseMapResolution	 Resolution of the base map used for rendering far patches on the terrain -  int
			//splatPrototypes Splat texture used by the terrain. -  SplatPrototype[]
			var splatPrototypesBytes = EncodeArray(inObject.splatPrototypes);
			return result;
		}

		byte[] EncodeDetailPrototype(DetailPrototype inObject)
		{
			var result = new byte[0];
			//prototype	 GameObject used by the DetailPrototype.
			//prototypeTexture	 Texture used by the DetailPrototype
			//minWidth	 Minimum width of the grass billboards (if render mode is GrassBillboard).
			//maxWidth	 Maximum width of the grass billboards (if render mode is GrassBillboard).
			//minHeight	 Minimum height of the grass billboards (if render mode is GrassBillboard).
			//maxHeight	 Maximum height of the grass billboards (if render mode is GrassBillboard).
			//noiseSpread	 How spread out is the noise for the DetailPrototype.
			//bendFactor	 Bend factor of the detailPrototype.
			//healthyColor	 Color when the DetailPrototypes are "healthy".
			//dryColor	 Color when the DetailPrototypes are "dry".
			//renderMode	 Render mode for the DetailPrototype.
			return result;
		}

		byte[] EncodeSplatPrototype(SplatPrototype inObject)
		{
			//texture	 Texture of the splat applied to the Terrain - Texture2D
			//tileSize	 Size of the tile used in the texture of the SplatPrototype. - Vector2
			//tileOffset	 Offset of the tile texture of the SplatPrototype. - Vector2

			var textureBytes = EncodeTexture2D(inObject.texture);
			var count = textureBytes.Length;
			var result = new byte[count + 16];
			Buffer.BlockCopy(textureBytes, 0, result, 0, count);
			Buffer.BlockCopy(EncodeVector2(inObject.tileSize), 0, result, count, 8);
			count += 8;
			Buffer.BlockCopy(EncodeVector2(inObject.tileOffset), 0, result, count, 8);
			return result;
		}

		byte[] EncodeParticle(Particle inObject)
		{
			var result = new byte[56];
			Buffer.BlockCopy(EncodeVector3(inObject.position), 0, result, 0, 12);
			Buffer.BlockCopy(EncodeVector3(inObject.velocity), 0, result, 12, 12);
			Buffer.BlockCopy(EncodeFloat(inObject.energy), 0, result, 24, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.startEnergy), 0, result, 28, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.size), 0, result, 32, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.rotation), 0, result, 36, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.angularVelocity), 0, result, 40, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.startEnergy), 0, result, 44, 12);
			return result;
		}

		*/

		
	}
}
