using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
	public partial class Serializer
	{

        /// <summary>
        /// External decoders for components
        /// </summary>
        public static readonly Dictionary<string, ComponentDecoder> ComponentDecoders = new Dictionary<string, ComponentDecoder>();

        /*private readonly Dictionary<string, ComponentDecoder> _componentTypeDecoders = new Dictionary<string, ComponentDecoder>
                                                                                        {
                                                                                            {"UnityEngine.Transform",SetTransform},
                                                                                            {"UnityEngine.MeshFilter",SetMeshFilter},
                                                                                            {"UnityEngine.MeshRenderer",SetMeshRenderer},
                                                                                            {"UnityEngine.Light",SetLight},
                                                                                            {"UnityEngine.Camera",SetCamera},
                                                                                            {"UnityEngine.Rigidbody",SetRigidbody},
                                                                                            {"UnityEngine.Projector",SetProjector},
                                                                                            {"UnityEngine.ParticleEmitter",SetParticleEmitter},
                                                                                            {"UnityEngine.GUITexture",DecodeGuiTexture},
                                                                                            {"UnityEngine.GUIText",DecodeGuiText},
                                                                                            {"UnityEngine.Terrain",DecodeTerrain},
                                                                                            {"UnityEngine.Collider",SetCollider},
                                                                                            {"UnityEngine.BoxCollider",SetCollider},
                                                                                            {"UnityEngine.CapsuleCollider",SetCollider},
                                                                                            {"UnityEngine.MeshCollider",SetCollider},
                                                                                            {"UnityEngine.SphereCollider",SetCollider},
                                                                                            {"UnityEngine.Skybox",DecodeSkybox}
                                                                                        };*/

		void SetComponent(byte[] inBytes, ref int startPos, ref GameObject result)
		{
			startPos++;//passed code 251 - resource
			var resourceIndex = DecodeInteger(inBytes, ref startPos);//index of resource
			var componentBytes = _bytesList[resourceIndex];
			var componentStartPos = 0;
			if(componentBytes[componentStartPos]!=199)
                ULog.Log("SetComponent: get not component code ( " + componentBytes[componentStartPos] + " )" + BitConverter.ToString(componentBytes, componentStartPos), ULogType.Warning);
				//Debug.LogWarning("SetComponent: get not component code (199) ");
			componentStartPos++;
			var componentType = DecodeString(componentBytes, ref componentStartPos);
			//ULog.Log("SetComponent : component type name  " + componentType);
			Component component = null;
		    if (ComponentDecoders.ContainsKey(componentType))
		        component = ComponentDecoders[componentType](componentBytes, ref componentStartPos, ref result,this);
		    else
		        switch (componentType)
		        {
		            case "UnityEngine.MeshFilter":
		                component = SetMeshFilter(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.MeshRenderer":
		                component = SetMeshRenderer(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Transform":
		                component = SetTransform(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Collider":
		                component = SetCollider(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Light":
		                component = SetLight(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Camera":
		                component = SetCamera(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Rigidbody":
		                component = SetRigidbody(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Projector":
		                component = SetProjector(componentBytes, ref componentStartPos, ref result);
		                break;
		            /*case "UnityEngine.ParticleEmitter":
		                component = SetParticleEmitter(componentBytes, ref componentStartPos, ref result);
		                break;*/
		            case "UnityEngine.GUITexture":
		                component = DecodeGuiTexture(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.GUIText":
		                component = DecodeGuiText(componentBytes, ref componentStartPos, ref result);
		                break;
		            case "UnityEngine.Terrain":
		                component = DecodeTerrain(componentBytes, ref componentStartPos, ref result);
		                break;
                    case "UnityEngine.SkinnedMeshRenderer":
		                component = DecodeSkinnedMeshRenderer(componentBytes, ref componentStartPos, ref result);
                        break;
                    /*case "UnityEngine.Animation":
                        component = DecodeAnimation(componentBytes, ref componentStartPos, ref result);
		                break;*/
		            default:
		                component = SetUnknowComponent(componentBytes, ref componentStartPos, ref result);
		                break;
		        }
		    if (_objectList.ContainsValue(component)) return;
		    _objectList.Add(_objectListCount, component);
		    _objectListCount++;
		}

        Component SetCollider(byte[] inBytes, ref int startPos, ref GameObject go)
        {
            var param = DecodeHashtable(inBytes, ref startPos);
            //Debug.Log("SetCollider "+param.Count+" parameters decoded");
            var className = param["class"].ToString();
            var tp = FindType(className);
            //Debug.Log("Collider class name "+className);
            var result = AddOrCreate(ref go, tp);
            /*_objectList.Add(_objectListCount, result);
            _objectListCount++;*/
            //Debug.Log("Collider class is " + param["class"]);
            foreach (DictionaryEntry entry in param)
            {
                //Debug.Log("Set collider parameters "+entry.Key+" = "+entry.Value);
                if (entry.Key.ToString() != "class" && entry.Key.ToString() != "sharedMaterial")
                {
                    var prop = tp.GetProperty(entry.Key.ToString());
                    //Debug.Log(result + " Property: " + prop + " = " + entry.Value);
                    prop.SetValue(result, entry.Value, null);
                }
                //Debug.Log("Set collider parameters " + entry.Key + " = " + entry.Value+" OK");
            }
            //ULog.Log("Collider parameters is set");
            var sharedMaterial = param["sharedMaterial"];
            if (sharedMaterial != null)
                ((Collider) result).sharedMaterial = (PhysicMaterial) sharedMaterial;
            //ULog.Log("SetCollider OK" + result);
            return result;
        }

	    static Component AddOrCreate(ref GameObject go,Type classType)
		{
		    var comp = go.GetComponent(classType);
            if (classType == typeof(Transform))
                comp = go.transform;
            if(comp==null)
            {
                comp = go.AddComponent(classType);
            }
		    return comp;

			/*Debug.Log("AddOrCreate " + classType);
			var component = go.GetComponent(classType);
			if (classType == typeof(Transform))
				component = go.transform;
		    return component ?? (component = go.AddComponent(classType));*/
		    //_objectPack.Add(component);
		}

		Component SetUnknowComponent(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			//Debug.Log("SetUnknowComponent");
			var parameters = DecodeHashtable(inBytes, ref startPos);
			//Debug.Log("SetUnknowComponent: class " + parameters["class"]+" on "+go.name);
			var newComponentType = FindType(parameters["class"].ToString());
			//Debug.Log("SetUnknowComponent Class: " + parameters["class"]+" Type: "+newComponentType+" Fullname: "+newComponentType.FullName);
			var newComponent = AddOrCreate(ref go, newComponentType);
			if (newComponent != null)
			{
				foreach (DictionaryEntry member in parameters)
				{
					var fi = newComponentType.GetField(member.Key.ToString());
					if (fi != null)
					{
						var tp = fi.FieldType;
						if (tp.BaseType == typeof(Enum))
							fi.SetValue(newComponent, Enum.ToObject(tp, member.Value));
						/*else if (ResourcedTypes.Contains(tp) || ResourcedTypes.Contains(tp.BaseType))
							fi.SetValue(newComponent, _resourcePack[(int)member.Value]);*/
						else
							fi.SetValue(newComponent, member.Value);
					}
					else
					{
						var pi = newComponentType.GetProperty(member.Key.ToString());
						if (pi != null)
						{
							var tp = pi.PropertyType;
							//Debug.Log(newComponent+"("+className+")."+pi.Name+"("+tp+")");
							if (tp.BaseType == typeof(Enum))
								pi.SetValue(newComponent, Enum.ToObject(tp, member.Value), null);
							/*else if (ResourcedTypes.Contains(tp) || ResourcedTypes.Contains(tp.BaseType))
								pi.SetValue(newComponent, _resourcePack[(int)member.Value], null);
							else if (tp == typeof(Material[]))
							{
								var matIndexes = (int[])member.Value;
								var mats = new Material[matIndexes.Length];
								for (var i = 0; i < matIndexes.Length; i++)
									mats[i] = (Material)_resourcePack[matIndexes[i]];
								pi.SetValue(newComponent, mats, null);
							}*/
							else
							{
                                //Debug.Log("Try set property "+pi.Name+" = "+member.Value);
								try
								{
									pi.SetValue(newComponent, member.Value, null);
								}
								catch (Exception e)
								{
									ULog.Log("Decode:SetComponentProps: Error on set property " + pi.Name + " in " + newComponent+"\n"+e.Message);
								}
							}
						}
					}
				}
			}
			return newComponent;
		}

        Component SetMeshRenderer(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var result = (MeshRenderer)AddOrCreate(ref go, typeof(MeshRenderer));
			result.castShadows = DecodeBoolean(inBytes, ref startPos);
			result.receiveShadows = DecodeBoolean(inBytes, ref startPos);
			//result.lightmapIndex = DecodeInteger(inBytes, ref startPos);
			//result.lightmapTilingOffset = DecodeVector4(inBytes, ref startPos);
			var materialCount = DecodeInteger(inBytes, ref startPos);
			var materials = new Material[materialCount];
			for (var i = 0; i < materialCount; i++)
			{
				materials[i] = (Material)Decoding(inBytes, ref startPos);
			}
			result.sharedMaterials = materials;
			return result;
		}

		Component SetMeshFilter(byte[] inBytes, ref int startPos, ref GameObject go)
		{
            //Debug.Log("SetMeshFilter");
			var result = (MeshFilter)AddOrCreate(ref go, typeof(MeshFilter));
			if (DecodeBoolean(inBytes, ref startPos))
			{
                //Debug.Log("SetMeshFilter shared mesh");
                result.sharedMesh = (Mesh)Decoding(inBytes, ref startPos);
				//result.sharedMesh = (Mesh)Decoding(inBytes, ref startPos);
                //Debug.Log("SetMeshFilter shared mesh : " + mesh);
				/*var mesh = DecodeMesh(inBytes, ref startPos);
				if(mesh!=null)
					result.sharedMesh = mesh;*/
			}
            //Debug.Log("SetMeshFilter OK "+result);
			return result;
		}

		static Component SetTransform(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			//Debug.Log("SetTransform ");
			var result = go.transform;
			if(go.transform==null)
				result = (Transform) AddOrCreate(ref go,typeof(Transform));
			result.localPosition = DecodeVector3(inBytes, ref startPos);
			result.localRotation = DecodeQuaternion(inBytes, ref startPos);
			result.localScale = DecodeVector3(inBytes, ref startPos);
			return result;
		}

		Component SetLight(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var result = (Light)AddOrCreate(ref go, typeof(Light));
			result.type = (LightType)DecodeInteger(inBytes, ref startPos);//4
			result.color = DecodeColor(inBytes, ref startPos);//16
			result.intensity = DecodeSingle(inBytes, ref startPos);//4
			result.shadows = (LightShadows)DecodeInteger(inBytes, ref startPos);//4
			result.shadowStrength = DecodeSingle(inBytes, ref startPos);//4
			result.shadowBias = DecodeSingle(inBytes, ref startPos);//4
			result.shadowSoftness = DecodeSingle(inBytes, ref startPos);//4
			result.shadowSoftnessFade = DecodeSingle(inBytes, ref startPos);//4
			result.range = DecodeSingle(inBytes, ref startPos);//4
			result.spotAngle = DecodeSingle(inBytes, ref startPos);//4
			result.renderMode = (LightRenderMode)DecodeInteger(inBytes, ref startPos);//4
			result.cullingMask = DecodeInteger(inBytes, ref startPos);//4

		    //result.areaSize = DecodeVector2(inBytes, ref startPos);//8
		    var hasCookie = DecodeBoolean(inBytes, ref startPos);
		    if (hasCookie)
                result.cookie = (Texture)Decoding(inBytes, ref startPos);
		    return result;
		}

		static Component SetCamera(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var cam = (Camera)AddOrCreate(ref go, typeof(Camera));
			cam.clearFlags = (CameraClearFlags)DecodeInteger(inBytes, ref startPos);
			cam.backgroundColor = DecodeColor(inBytes, ref startPos);
			cam.cullingMask = DecodeInteger(inBytes, ref startPos);
			cam.orthographic = DecodeBoolean(inBytes, ref startPos);
			cam.orthographicSize = DecodeSingle(inBytes, ref startPos);
			cam.fieldOfView = DecodeSingle(inBytes, ref startPos);
			cam.nearClipPlane = DecodeSingle(inBytes, ref startPos);
			cam.farClipPlane = DecodeSingle(inBytes, ref startPos);
			cam.rect = DecodeRect(inBytes, ref startPos);
			cam.depth = DecodeSingle(inBytes, ref startPos);
			cam.renderingPath = (RenderingPath)DecodeInteger(inBytes, ref startPos);
			return cam;
		}

		static Component SetRigidbody(byte[] inBytes, ref int startPos, ref GameObject go)
		{
		    var rb = go.AddComponent<Rigidbody>();
		    rb.useGravity = false;
		    rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

		    rb.velocity = DecodeVector3(inBytes, ref startPos);
            rb.angularVelocity = DecodeVector3(inBytes, ref startPos);
            rb.drag = DecodeSingle(inBytes, ref startPos);
            rb.angularDrag = DecodeSingle(inBytes, ref startPos);
            rb.isKinematic = DecodeBoolean(inBytes, ref startPos);
            rb.collisionDetectionMode = (CollisionDetectionMode)DecodeInteger(inBytes, ref startPos);
            rb.centerOfMass = DecodeVector3(inBytes, ref startPos);
            rb.inertiaTensorRotation = DecodeQuaternion(inBytes, ref startPos);
            rb.inertiaTensor = DecodeVector3(inBytes, ref startPos);
            rb.detectCollisions = DecodeBoolean(inBytes, ref startPos);
            rb.useConeFriction = DecodeBoolean(inBytes, ref startPos);
            rb.position = DecodeVector3(inBytes, ref startPos);
            rb.rotation = DecodeQuaternion(inBytes, ref startPos);
            rb.interpolation = (RigidbodyInterpolation)DecodeInteger(inBytes, ref startPos);
            rb.solverIterationCount = DecodeInteger(inBytes, ref startPos);
            rb.sleepVelocity = DecodeSingle(inBytes, ref startPos);
            rb.sleepAngularVelocity = DecodeSingle(inBytes, ref startPos);
            rb.maxAngularVelocity = DecodeSingle(inBytes, ref startPos);
            rb.mass = DecodeSingle(inBytes, ref startPos);
            rb.useGravity = DecodeBoolean(inBytes, ref startPos);
            rb.freezeRotation = DecodeBoolean(inBytes, ref startPos);
            rb.constraints = (RigidbodyConstraints)DecodeInteger(inBytes, ref startPos);

		    return rb;
		    /*previos
			var rb = (Rigidbody) AddOrCreate(ref go, typeof (Rigidbody));
			rb.useGravity = false;
			rb.drag = DecodeSingle(inBytes, ref startPos);
			rb.angularDrag = DecodeSingle(inBytes, ref startPos);
			rb.mass = DecodeSingle(inBytes, ref startPos);
			rb.isKinematic = DecodeBoolean(inBytes, ref startPos);
			rb.freezeRotation = DecodeBoolean(inBytes, ref startPos);
			rb.constraints = (RigidbodyConstraints)DecodeInteger(inBytes, ref startPos);
			rb.collisionDetectionMode =(CollisionDetectionMode) DecodeInteger(inBytes, ref startPos);
			rb.interpolation = (RigidbodyInterpolation)DecodeInteger(inBytes, ref startPos);
			rb.useGravity = DecodeBoolean(inBytes, ref startPos);
		    rb.velocity = DecodeVector3(inBytes, ref startPos);
		    rb.angularVelocity = DecodeVector3(inBytes, ref startPos);
			return rb;*/
		}

        Component SetProjector(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var pro = (Projector) AddOrCreate(ref go, typeof (Projector));
			pro.nearClipPlane = DecodeSingle(inBytes, ref startPos);
			pro.farClipPlane = DecodeSingle(inBytes, ref startPos);
			pro.fieldOfView = DecodeSingle(inBytes, ref startPos);
			pro.aspectRatio = DecodeSingle(inBytes, ref startPos);
			pro.orthographic = DecodeBoolean(inBytes, ref startPos);
			pro.orthographicSize = DecodeSingle(inBytes, ref startPos);
			pro.ignoreLayers = DecodeInteger(inBytes, ref startPos);
			if (DecodeBoolean(inBytes, ref startPos))
				pro.material = (Material) Decoding(inBytes, ref startPos);
			return pro;
		}

        /*Component SetParticleEmitter(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var parameters = DecodeHashtable(inBytes, ref startPos);

			var component = go.GetComponent(parameters["class"].ToString()) ?? go.AddComponent(parameters["class"].ToString());
		    var tp = component.GetType();
			foreach (DictionaryEntry parameter in parameters)
			{
				if ((string) parameter.Key != "class")
				{
					var prop = tp.GetProperty(parameter.Key.ToString());
					if (prop.CanWrite)
					{
						prop.SetValue(component, parameter.Value, null);
					}
				}
			}
			return component;
		}*/

        Component DecodeGuiTexture(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var parameters = DecodeHashtable(inBytes, ref startPos);
			var component = (GUITexture) AddOrCreate(ref go, typeof (GUITexture));
			component.texture = (Texture2D)parameters["texture"];
			component.color = (Color)parameters["color"];
			component.pixelInset = (Rect)parameters["pixelInset"];
			return component;
		}

        Component DecodeGuiText(byte[] inBytes, ref int startPos, ref GameObject go)
		{
			var parameters = DecodeHashtable(inBytes, ref startPos);
			var component = (GUIText)AddOrCreate(ref go, typeof(GUIText));
			component.text = (string)parameters["text"];
			/*component.material = (Material)parameters["material"];*/
			component.pixelOffset = (Vector2)parameters["pixelOffset"];
			component.alignment = (TextAlignment)parameters["alignment"];
			component.anchor = (TextAnchor)parameters["anchor"];
			component.lineSpacing = (float)parameters["lineSpacing"];
			component.tabSize = (float)parameters["tabSize"];
			component.fontSize = (int)parameters["fontSize"];
			component.fontStyle = (FontStyle)parameters["fontStyle"];
			return component;
		}

        Terrain DecodeTerrain(byte[] inBytes, ref int startPos, ref GameObject go)
        {
            var component = (Terrain) AddOrCreate(ref go, typeof (Terrain));
            var data = DecodeHashtable(inBytes, ref startPos);
            component.castShadows=(bool)data["castShadows"];
            component.lightmapIndex=(int)data["lightmapIndex"];
            component.basemapDistance=(float)data["basemapDistance"];
            component.heightmapMaximumLOD = (int)data["heightmapMaximumLOD"];
            component.heightmapPixelError = (float)data["heightmapPixelError"];
            component.detailObjectDensity = (float)data["detailObjectDensity"];
            component.detailObjectDistance = (float)data["detailObjectDistance"];
            component.treeMaximumFullLODCount = (int)data["treeMaximumFullLODCount"];
            component.treeCrossFadeLength = (float)data["treeCrossFadeLength"];
            component.treeBillboardDistance = (float)data["treeBillboardDistance"];
            component.treeDistance = (float)data["treeDistance"];
            //component.terrainData = data["terrainData"];
            /*
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
                                {"treeDistance", component.treeDistance},
                                {"terrainData", component.terrainData}
             */
             return component;
        }

        SkinnedMeshRenderer DecodeSkinnedMeshRenderer(byte[] inBytes, ref int startPos, ref GameObject go)
        {
            var component = (SkinnedMeshRenderer) AddOrCreate(ref go, typeof (SkinnedMeshRenderer));
            var data = DecodeHashtable(inBytes, ref startPos);

            var mesh = new Mesh
                           {
                               vertices = (Vector3[]) data["vertices"],
                               normals = (Vector3[]) data["normals"],
                               tangents = (Vector4[]) data["tangents"],
                               uv = (Vector2[]) data["uv"],
                               //uv1 = (Vector2[]) data["uv1"],
                               uv2 = (Vector2[]) data["uv2"],
                               bounds = (Bounds) data["bounds"],
                               colors = (Color[]) data["colors"],
                               triangles = (int[]) data["triangles"],
                               boneWeights = (BoneWeight[]) data["boneWeights"],
                               bindposes = (Matrix4x4[]) data["bindposes"]
                           };

            component.enabled = (bool) data["enabled"];

            var componentbones = (Transform[]) data["bones"];
            var cbLenght = componentbones.Length;
            var bones = new Transform[cbLenght];
            for (var i = 0; i < cbLenght; i++)
                bones[i] = componentbones[i];

            component.bones = bones;
            if(Application.isEditor)
                component.sharedMaterials = (Material[])data["materials"];
            else
                component.materials = (Material[])data["materials"];

            component.sharedMesh = mesh;
            return component;
        }

        /*Animation DecodeAnimation(byte[] inBytes, ref int startPos, ref GameObject go)
        {
            var component = (Animation)AddOrCreate(ref go, typeof(Animation));

            return component;
        }*/

        Skybox DecodeSkybox(byte[] inBytes, ref int startPos, ref GameObject go)
        {
            var component = (Skybox)AddOrCreate(ref go, typeof(Skybox));
            component.material = DecodeMaterial(inBytes, ref startPos);
            return component;
        }
	}
}
