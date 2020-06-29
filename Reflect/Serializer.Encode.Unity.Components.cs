using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ULIB
{
	public partial class Serializer
	{
	    /// <summary>
	    /// Externa encoders for components
	    /// </summary>
	    internal static readonly Dictionary<string, ComponentEncoder> ComponentEncoders = new Dictionary<string, ComponentEncoder>();

        /// <summary>
        /// 
        /// </summary>
        public static readonly List<string> IgnoreComponents = new List<string>()
                                                                   {
                                                                       "UnityEngine.AudioSource",
                                                                       "UnityEngine.AudioListener",
                                                                       "UnityEngine.GUILayer",
                                                                       "UnityEngine.FlareLayer",
                                                                       "UnityEngine.Behaviour"
                                                                   }; 

        byte[] EncodeComponent(Component component)
        {
            var resourceIndexBytes = new byte[] {251, 0, 0, 0, 0};
            if (!_objectList.ContainsValue(component))
            {
                //Debug.Log("EncodeComponent: Add to resource pack "+component);
                var objectBytes = EncodingComponent(component);
                _objectList.Add(_objectListCount, component);
                _bytesList.Add(_objectListCount, objectBytes);
                _objectListCount++;
                _bytesLenght += objectBytes.Length;
            }
            //var index = _objectPack.IndexOf(component);
            Buffer.BlockCopy(EncodeInteger(GetResourceIndex(component)), 0, resourceIndexBytes, 1, 4);
            return resourceIndexBytes;
        }

	    byte[] EncodingComponent(Component component)
        {
            //Debug.Log("EncodingComponent: "+component);
            var tp = component.GetType();
            var str = tp.FullName;
            var baseName = component.GetType().BaseType.FullName ;
            byte[] data;
            if (ComponentEncoders.ContainsKey(str))
                data = ComponentEncoders[str](component,this);
            else if(ComponentEncoders.ContainsKey(baseName))
                data = ComponentEncoders[baseName](component,this);
            else
            {
                switch (str)
                {
                    case "UnityEngine.RectTransform":
                        data = EncodeRectTransform((RectTransform)component);
                        break;
                    case "UnityEngine.Transform":
                        data = EncodeTransform((Transform) component);
                        break;
                    
                    case "UnityEngine.MeshFilter":
                        data = EncodeMeshFilter((MeshFilter) component);
                        break;
                    case "UnityEngine.MeshRenderer":
                        data = EncodeMeshRenderer(component);
                        break;
                    case "UnityEngine.Light":
                        data = EncodeLight((Light) component);
                        break;
                    case "UnityEngine.Camera":
                        data = EncodeCamera(component);
                        break;
                    case "UnityEngine.Rigidbody":
                        data = EncodeRigidbody(component);
                        break;
                    case "UnityEngine.Projector":
                        data = EncodeProjector(component);
                        break;
                    case "UnityEngine.ParticleEmitter":
                        data = EncodeParticleEmitter(component);
                        break;
                    case "UnityEngine.GUITexture":
                        data = EncodeGuiTexture(component);
                        break;
                    case "UnityEngine.GUIText":
                        data = EncodeGuiText(component);
                        break;
                    case "UnityEngine.Terrain":
                        data = EncodeTerrain(component);
                        break;
                    case "UnityEngine.SkinnedMeshRenderer":
                        data = EncodeSkinnedMeshRenderer((SkinnedMeshRenderer) component);
                        break;
                    default:
                        switch(baseName)
                        {
                            case "UnityEngine.Collider":
                                str = "UnityEngine.Collider";
                                data = EncodeColliders((Collider) component);
                                break;
                            case "UnityEngine.ParticleEmitter":
                                data = EncodeParticleEmitter((ParticleEmitter) component);
                                break;
                            default:
                                data = EncodeUnknowComponent(component);
                            break;
                        }

                        break;
                }
            }

	        var strBytes = EncodeString(str);
            var strLenght = strBytes.Length;
            var result = new byte[1 + strLenght + data.Length];
            result[0] = 199;
            Buffer.BlockCopy(strBytes, 0, result, 1, strLenght);
            strLenght++;
            Buffer.BlockCopy(data, 0, result, strLenght, data.Length);
            return result;
        }

	    byte[] EncodeUnknowComponent(Component component)
		{
	        ULog.Log("Component " + component + " from " + component.gameObject.name + " encode as unknow.");

	        //Debug.Log("EncodeUnknowComponent: "+component);
			var componentType = component.GetType();
			var componentProperties = new Hashtable
			                          	{
			                          		{"class",componentType.FullName}
			                          	};
			var componentMi = componentType.GetMembers();
            var ctCount = ComponentTypes.Length;

	        var props = componentType.GetProperties();
	        foreach (var propertyInfo in props)
	            if (propertyInfo.GetCustomAttributes(typeof (SerializeRequire), true).Length > 0)
	            {
                    //Debug.Log("Encode property "+propertyInfo.Name);
	                componentProperties.Add(propertyInfo.Name, propertyInfo.GetValue(component, null));
	            }
	                

	        foreach (var memberInfo in componentMi)
			{
                //Debug.Log("EncodeUnknowComponent member : "+memberInfo.Name);
				if (memberInfo is FieldInfo)
				{
					var fi = (FieldInfo)memberInfo;
                    if (!fi.IsNotSerialized)//[NonSerialized()]
					{
						var tp = fi.FieldType;
					    for (var i = 0; i < ctCount; i++)
					    {
                            if (ComponentTypes[i] == tp || ComponentTypes[i] == tp.BaseType)
					        {
                                var val = fi.GetValue(component);
                                if (val != null && val.ToString() != "null" && !componentProperties.ContainsKey(fi.Name))
                                    componentProperties.Add(fi.Name, val);
					        }
					    }
						/*if (ComponentTypes.Contains(tp) || ComponentTypes.Contains(tp.BaseType))
						{
							var val = fi.GetValue(component);
							if (val != null && val.ToString() != "null")
								componentProperties.Add(fi.Name, val);
						}*/
					}
				}
				else if (memberInfo is PropertyInfo)
				{
					var pi = (PropertyInfo)memberInfo;
					if (pi.CanWrite)
					{
						var tp = pi.PropertyType;
                        for (var i = 0; i < ctCount; i++)
                        {
                            if (ComponentTypes[i] == tp || ComponentTypes[i] == tp.BaseType)
                            {
                                var obj = pi.GetValue(component,null);
                                if (obj != null && obj.ToString() != "null" && !componentProperties.ContainsKey(pi.Name))
                                    componentProperties.Add(pi.Name, obj);
                            }
                        }
					}
				}
			}
            /*if (Gateway.Debug)
                ULog.Log("EncodeUnknowComponent END");*/
			return EncodeHashtable(componentProperties);
		}

        #region Encoderses

	    private static byte[] EncodeRectTransform(RectTransform rt)
	    {
	        var result = new byte[64];
            Buffer.BlockCopy(EncodeVector2(rt.anchoredPosition),0,result,0,8);
            Buffer.BlockCopy(EncodeVector2(rt.anchorMax), 0, result, 8, 8);
            Buffer.BlockCopy(EncodeVector2(rt.anchorMin), 0, result, 16, 8);
            Buffer.BlockCopy(EncodeVector2(rt.offsetMax), 0, result, 24, 8);
            Buffer.BlockCopy(EncodeVector2(rt.offsetMin), 0, result, 32, 8);
            Buffer.BlockCopy(EncodeVector2(rt.pivot), 0, result, 40, 8);
            Buffer.BlockCopy(EncodeRect(rt.rect), 0, result, 48, 8);
            Buffer.BlockCopy(EncodeVector2(rt.sizeDelta), 0, result, 56, 8);
	        return result;
	    }


	    private static byte[] EncodeTransform(Transform obj)
		{
            //Debug.Log("EncodeTransform "+obj.gameObject.name);
			var result = new byte[40];
            Buffer.BlockCopy(EncodeVector3(obj.localPosition), 0, result, 0, 12);
            Buffer.BlockCopy(EncodeQuaternion(obj.localRotation), 0, result, 12, 16);
            Buffer.BlockCopy(EncodeVector3(obj.localScale), 0, result, 28, 12);
			return result;
		}

	    private byte[] EncodeMeshFilter(MeshFilter component)
		{
			var meshBytes = new byte[0];
            if (component.sharedMesh != null)
                meshBytes = Encoding(component.sharedMesh);
			var mbLenght = meshBytes.Length;
			var result = new byte[mbLenght + 1];
			result[0] = (byte)((mbLenght > 0) ? 2 : 1);
			Buffer.BlockCopy(meshBytes, 0, result, 1, mbLenght);
			return result;
		}

	    private byte[] EncodeLight(Light light)
		{
            //var light = (Light)component;
            var result = new byte[61];//60
			Buffer.BlockCopy(EncodeInteger((int)light.type), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeColor(light.color), 0, result, 4, 16);
			Buffer.BlockCopy(EncodeFloat(light.intensity), 0, result, 20, 4);
			Buffer.BlockCopy(EncodeInteger((int)light.shadows), 0, result, 24, 4);
			Buffer.BlockCopy(EncodeFloat(light.shadowStrength), 0, result, 28, 4);
			Buffer.BlockCopy(EncodeFloat(light.shadowBias), 0, result, 32, 4);
			Buffer.BlockCopy(EncodeFloat(light.shadowSoftness), 0, result, 36, 4);
			Buffer.BlockCopy(EncodeFloat(light.shadowSoftnessFade), 0, result, 40, 4);
			Buffer.BlockCopy(EncodeFloat(light.range), 0, result, 44, 4);
			Buffer.BlockCopy(EncodeFloat(light.spotAngle), 0, result, 48, 4);
			Buffer.BlockCopy(EncodeInteger((int)light.renderMode), 0, result, 52, 4);
			Buffer.BlockCopy(EncodeInteger(light.cullingMask), 0, result, 56, 4);
            //Buffer.BlockCopy(EncodeVector2(light.areaSize),0,result,60,8);//new version
            Buffer.BlockCopy(EncodeBoolean(light.cookie!=null),0,result,60,1);//new version
            if(light.cookie!=null)//new
            {
                var cookieBytes = Encoding(light.cookie);//EncodeTexture2D((Texture2D)light.cookie);
                var resultCookie = new byte[result.Length + cookieBytes.Length];
                Buffer.BlockCopy(result,0,resultCookie,0,result.Length);
                Buffer.BlockCopy(cookieBytes, 0, resultCookie, result.Length, cookieBytes.Length);
                return resultCookie;
            }

			//Buffer.BlockCopy(Encoding(light.cookie), 0, result, 60, 4);
			return result;
		}
		
		static byte[] EncodeCamera(Component component)
		{
            var cam = (Camera)component;
			var result = new byte[65];
			Buffer.BlockCopy(EncodeInteger((int)cam.clearFlags), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeColor(cam.backgroundColor), 0, result, 4, 16);
			Buffer.BlockCopy(EncodeInteger(cam.cullingMask), 0, result, 20, 4);
			Buffer.BlockCopy(EncodeBoolean(cam.orthographic), 0, result, 24, 1);
			Buffer.BlockCopy(EncodeFloat(cam.orthographicSize), 0, result, 25, 4);
			Buffer.BlockCopy(EncodeFloat(cam.fieldOfView), 0, result, 29, 4);
			Buffer.BlockCopy(EncodeFloat(cam.nearClipPlane), 0, result, 33, 4);
			Buffer.BlockCopy(EncodeFloat(cam.farClipPlane), 0, result, 37, 4);
			Buffer.BlockCopy(EncodeRect(cam.rect), 0, result, 41, 16);
			Buffer.BlockCopy(EncodeFloat(cam.depth), 0, result, 57, 4);
			Buffer.BlockCopy(EncodeInteger((int)cam.renderingPath), 0, result, 61, 4);
			return result;
		}

        /*
        velocity	                The velocity vector of the rigidbody.
        angularVelocity	            The angular velocity vector of the rigidbody.
        drag	                    The drag of the object.
        angularDrag	                The angular drag of the object.
        mass	                    The mass of the rigidbody.
        useGravity	                Controls whether gravity affects this rigidbody.
        isKinematic	                Controls whether physics affects the rigidbody.
        freezeRotation	            Controls whether physics will change the rotation of the object.
        constraints	                Controls which degrees of freedom are alowed for the simulation of this Rigidbody.
        collisionDetectionMode	    The Rigidbody's collision detection mode.
        centerOfMass	            The center of mass relative to the transform's origin.
        inertiaTensorRotation	    The rotation of the inertia tensor.
        inertiaTensor	            The diagonal inertia tensor of mass relative to the center of mass.
        detectCollisions	        Should collision detection be enabled? (By default always enabled)
        useConeFriction	            Force cone friction to be used for this rigidbody.
        position	                The position of the rigidbody.
        rotation	                The rotation of the rigdibody.
        interpolation               Interpolation allows you to smooth out the effect of running physics at a fixed frame rate.
        solverIterationCount        Allows you to override the solver iteration count per rigidbody.
        sleepVelocity	            The linear velocity, below which objects start going to sleep. (Default 0.14) range { 0, infinity }
        sleepAngularVelocity	    The angular velocity, below which objects start going to sleep. (Default 0.14) range { 0, infinity }
        maxAngularVelocity          The maximimum angular velocity of the rigidbody. (Default 7) range { 0, infinity }
         */
        static byte[] EncodeRigidbody(Component component)
		{
            var rb = (Rigidbody) component;
            var result = new byte[137];
            Buffer.BlockCopy(EncodeVector3(rb.velocity),0,result,0,12);
            Buffer.BlockCopy(EncodeVector3(rb.angularVelocity), 0, result, 12, 12);
            Buffer.BlockCopy(EncodeFloat(rb.drag), 0, result, 24, 4);
            Buffer.BlockCopy(EncodeFloat(rb.angularDrag), 0, result, 28, 4);
            
            Buffer.BlockCopy(EncodeBoolean(rb.isKinematic), 0, result, 32, 1);
            
            Buffer.BlockCopy(EncodeInteger(rb.collisionDetectionMode), 0, result, 33, 4);
            Buffer.BlockCopy(EncodeVector3(rb.centerOfMass), 0, result, 37, 12);
            Buffer.BlockCopy(EncodeQuaternion(rb.inertiaTensorRotation), 0, result, 49, 16);
            Buffer.BlockCopy(EncodeVector3(rb.inertiaTensor), 0, result, 65, 12);
            Buffer.BlockCopy(EncodeBoolean(rb.detectCollisions), 0, result, 77, 1);
            Buffer.BlockCopy(EncodeBoolean(rb.useConeFriction), 0, result, 78, 1);
            Buffer.BlockCopy(EncodeVector3(rb.position), 0, result, 79, 12);
            Buffer.BlockCopy(EncodeQuaternion(rb.rotation), 0, result, 91, 16);
            Buffer.BlockCopy(EncodeInteger(rb.interpolation), 0, result, 107, 4);
            Buffer.BlockCopy(EncodeInteger(rb.solverIterationCount), 0, result, 111, 4);
            Buffer.BlockCopy(EncodeFloat(rb.sleepVelocity), 0, result, 115, 4);
            Buffer.BlockCopy(EncodeFloat(rb.sleepAngularVelocity), 0, result, 119, 4);
            Buffer.BlockCopy(EncodeFloat(rb.maxAngularVelocity), 0, result, 123, 4);

            Buffer.BlockCopy(EncodeFloat(rb.mass), 0, result, 127, 4);
            Buffer.BlockCopy(EncodeBoolean(rb.useGravity), 0, result, 131, 1);

            Buffer.BlockCopy(EncodeBoolean(rb.freezeRotation), 0, result, 132, 1);
            Buffer.BlockCopy(EncodeInteger(rb.constraints), 0, result, 133, 4);

            return result;
            /* previos
            var rb = (Rigidbody)component;
			var result = new byte[51];
			Buffer.BlockCopy(EncodeFloat(rb.drag), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeFloat(rb.angularDrag), 0, result, 4, 4);
			Buffer.BlockCopy(EncodeFloat(rb.mass), 0, result, 8, 4);
			Buffer.BlockCopy(EncodeBoolean(rb.isKinematic), 0, result, 12, 1);
			Buffer.BlockCopy(EncodeBoolean(rb.freezeRotation), 0, result, 13, 1);
			Buffer.BlockCopy(EncodeInteger((int)rb.constraints), 0, result, 14, 4);
			Buffer.BlockCopy(EncodeInteger((int)rb.collisionDetectionMode), 0, result, 18, 4);
			Buffer.BlockCopy(EncodeInteger((int)rb.interpolation), 0, result, 22, 4);
			Buffer.BlockCopy(EncodeBoolean(rb.useGravity), 0, result, 26, 1);
            Buffer.BlockCopy(EncodeVector3(rb.velocity), 0, result, 27, 12);
            Buffer.BlockCopy(EncodeVector3(rb.angularVelocity), 0, result, 39, 12);
			return result;*/
		}

        byte[] EncodeMeshRenderer(Component component)
		{
            var inObject = (MeshRenderer)component;
			var materials = inObject.sharedMaterials;
			var matLenght = materials.Length;

			var materialsBytes = new byte[materials.Length*5];
			var pos = -5;
			foreach (var material in materials)
			{
                Buffer.BlockCopy(Encoding(material), 0, materialsBytes, pos += 5, 5);
			}
			var result = new byte[6+materialsBytes.Length];
			result[0] = (byte) (inObject.castShadows ? 2 : 1);
			result[1] = (byte)(inObject.receiveShadows ? 2 : 1);
			//Buffer.BlockCopy(EncodeInteger(inObject.lightmapIndex), 0, result, 2, 4);
			//Buffer.BlockCopy(EncodeVector4(inObject.lightmapTilingOffset), 0, result, 6, 16);
			Buffer.BlockCopy(EncodeInteger(matLenght), 0, result, 2, 4);
			Buffer.BlockCopy(materialsBytes, 0, result, 6, materialsBytes.Length);
			return result;
		}

        byte[] EncodeColliders(Component component)
		{
            var collider = (Collider)component;
			switch (collider.GetType().FullName)
			{
				case "UnityEngine.MeshCollider":
					return EncodeMeshCollider((MeshCollider)collider);
				case "UnityEngine.CapsuleCollider":
					return EncodeCapsuleCollider((CapsuleCollider)collider);
				case "UnityEngine.SphereCollider":
					return EncodeSphereCollider((SphereCollider)collider);
				case "UnityEngine.BoxCollider":
					return EncodeBoxCollider((BoxCollider)collider);
				default:
					return EncodeCollider(collider);
			}
		}

		byte[] EncodeMeshCollider(MeshCollider inObject)
		{
			var param = new Hashtable
			            	{
			            		{"class", inObject.GetType().FullName},
			            		{"isTrigger", inObject.isTrigger},
                                {"sharedMaterial",inObject.sharedMaterial},

			            		{"smoothSphereCollisions", inObject.smoothSphereCollisions},
			            		{"convex", inObject.convex},
			            		{"sharedMesh", inObject.sharedMesh}
			            	};
            return EncodeHashtable(param);
		}

		byte[] EncodeCapsuleCollider(CapsuleCollider inObject)
		{
			var param = new Hashtable
			            	{
			            		{"class", inObject.GetType().FullName},
			            		{"isTrigger", inObject.isTrigger},
                                {"sharedMaterial",inObject.sharedMaterial},

			            		{"radius", inObject.radius},
			            		{"height", inObject.height},
			            		{"direction", inObject.direction},
			            		{"center", inObject.center}
			            	};
            return EncodeHashtable(param);
		}

		byte[] EncodeSphereCollider(SphereCollider inObject)
		{
			var param = new Hashtable
			            	{
			            		{"class", inObject.GetType().FullName},
			            		{"isTrigger", inObject.isTrigger},
                                {"sharedMaterial",inObject.sharedMaterial},

			            		{"radius", inObject.radius},
			            		{"center", inObject.center}
			            	};
            return EncodeHashtable(param);
		}

		byte[] EncodeBoxCollider(BoxCollider collider)
		{
			var parameter = new Hashtable
			             	{
								{"class",collider.GetType().FullName},
			             		{"isTrigger", collider.isTrigger},

			             		{"center", collider.center},
			             		{"size", collider.size}
			             	};
			if (collider.sharedMaterial != null)
				parameter.Add("sharedMaterial", collider.sharedMaterial);

            return EncodeHashtable(parameter);
		}

		byte[] EncodeCollider(Collider inObject)
		{
			var ht = new Hashtable
			         	{
			         		{"class",inObject.GetType().FullName},
			         		{"isTrigger",inObject.isTrigger},
                            {"material",inObject.material}
			         	};
            return EncodeHashtable(ht);
		}

        byte[] EncodeParticleEmitter(Component component)
		{
            var particleEmitter = (ParticleEmitter)component;
			var result = new Hashtable();
			if(particleEmitter.ToString().Contains("Ellipsoid"))
				result.Add("class", "UnityEngine.EllipsoidParticleEmitter");
			else if (particleEmitter.ToString().Contains("MeshParticle"))
				result.Add("class", "UnityEngine.MeshParticleEmitter");
			var emiterType = particleEmitter.GetType();
			foreach (var member in ParticleEmitterMembers)
				result.Add(member, emiterType.GetProperty(member).GetValue(particleEmitter, null));
            return EncodeHashtable(result);
		}

        #endregion

        #region Behaviour

        byte[] EncodeProjector(Component component)
		{
            var inObject = (Projector)component;
			var materialBytes = new byte[0];
			if (inObject.material != null)
                materialBytes = Encoding(inObject.material);// GetMaterialIndex(inObject.material);
			var materialBytesLength = materialBytes.Length;
			var result = new byte[26 + materialBytesLength];
			Buffer.BlockCopy(EncodeFloat(inObject.nearClipPlane), 0, result, 0, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.farClipPlane), 0, result, 4, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.fieldOfView), 0, result, 8, 4);
			Buffer.BlockCopy(EncodeFloat(inObject.aspectRatio), 0, result, 12, 4);
			Buffer.BlockCopy(EncodeBoolean(inObject.orthographic), 0, result, 16, 1);
			Buffer.BlockCopy(EncodeFloat(inObject.orthographicSize), 0, result, 17, 4);
			Buffer.BlockCopy(EncodeInteger(inObject.ignoreLayers), 0, result, 21, 4);
			Buffer.BlockCopy(EncodeBoolean(inObject.material != null), 0, result, 25, 1);
			Buffer.BlockCopy(materialBytes, 0, result, 26, materialBytesLength);
			return result;
		}

		byte[] EncodeGuiTexture(Component component)
		{
            var inObject = (GUITexture)component;
			//Debug.Log("EncodeGUITexture");
			var table = new Hashtable
			            	{
			            		{"name", inObject.name},
								{"texture",inObject.texture},
								{"color",inObject.color},
								{"pixelInset",inObject.pixelInset}
			            	};
            return EncodeHashtable(table);
		}

		byte[] EncodeGuiText(Component component)
		{
            var inObject = (GUIText)component;
			//Debug.Log("EncodeGUIText");
			//var member = component.GetType().GetMembers();
			var table = new Hashtable
			            	{
			            		{"name", inObject.name},
								{"text",inObject.text},
								/*{"material",inObject.material},*/
								{"pixelOffset",inObject.pixelOffset},
								{"alignment",inObject.alignment},
								{"anchor",inObject.anchor},
								{"lineSpacing",inObject.lineSpacing },
								{"tabSize",inObject.tabSize},
								{"fontSize",inObject.fontSize},
								{"fontStyle",inObject.fontStyle}
			            	};
            return EncodeHashtable(table);
		}

        byte[] EncodeTerrain(Component terrain)
        {
            var component = (Terrain) terrain;
            var table = new Hashtable
                            {
                                {"castShadows", component.castShadows},
                                {"lightmapIndex", component.lightmapIndex},
                                {"basemapDistance", component.basemapDistance},
                                {"heightmapMaximumLOD", component.heightmapMaximumLOD},
                                {"heightmapPixelError", component.heightmapPixelError},
                                {"detailObjectDensity", component.detailObjectDensity},
                                {"detailObjectDistance", component.detailObjectDistance},
                                {"treeMaximumFullLODCount", component.treeMaximumFullLODCount},
                                {"treeCrossFadeLength", component.treeCrossFadeLength},
                                {"treeBillboardDistance", component.treeBillboardDistance},
                                {"treeDistance", component.treeDistance}/*,
                                {"terrainData", component.terrainData}*/
                            };
            return EncodeHashtable(table);

        }

        byte[] EncodeSkinnedMeshRenderer(SkinnedMeshRenderer component)
        {
            var mesh = component.sharedMesh;

            var componentbones = component.bones.Length;
            var gos = new Transform[componentbones];

            for (var i = 0; i < componentbones; i++)
                gos[i] = component.bones[i];

            var table = new Hashtable
                            {
                                {"enabled", component.enabled},
                                {"vertices", mesh.vertices},
                                {"normals", mesh.normals},
                                {"tangents", mesh.tangents},
                                {"uv", mesh.uv},
                                /*{"uv1", mesh.uv1},*/
                                {"uv2", mesh.uv2},
                                {"bounds", mesh.bounds},
                                {"colors", mesh.colors},
                                {"triangles", mesh.triangles},
                                {"boneWeights", mesh.boneWeights},
                                {"bindposes", mesh.bindposes},
                                {"bones", gos},
                                {"materials", Application.isEditor ? component.sharedMaterials : component.materials}
                            };
            return EncodeHashtable(table);
        }



	    /*byte[] EncodeSkybox(Component component)
        {
            return EncodeMaterial(((Skybox) component).material);
        }*/

		#endregion

	}
}
