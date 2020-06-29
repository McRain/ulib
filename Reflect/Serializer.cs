using System;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
	/// <summary>
	/// Provides methods for serializing and deserializing objects, compression, encryption.
	/// </summary>
    public partial class Serializer : IUlibSerializer
	{
        #region Variables
        
        //public static byte[] HeadBytes = new byte[0];

        /// <summary>
        /// ULIB version
        /// </summary>
        public static byte Version
        {
            //get { return Assembly.GetCallingAssembly().GetName().Version.ToString(); }
            get
            {
                return 0x4;// assemblyName.Version.ToString(); ;//0x42
            }
        }

        private static int _nameKey = 2;//key for renaming childrens

        //private static Serializer _instance;

        /// <summary>
        /// Enable/disable debug (for Unity Editor used)
        /// </summary>
        public static bool debug;

		#endregion Variables
        
        #region Coder delegates

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <param name="serializer"></param>
	    public delegate byte[] ObjectEncoder(object obj, IUlibSerializer serializer);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="inBytes"></param>
	    /// <param name="startPos"></param>
	    /// <param name="serializer"></param>
	    public delegate object ObjectDecoder(byte[] inBytes, ref int startPos,IUlibSerializer serializer);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="component"></param>
	    /// <param name="serializer"></param>
	    public delegate byte[] ComponentEncoder(Component component,IUlibSerializer serializer);

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="inBytes"></param>
	    /// <param name="startPos"></param>
	    /// <param name="targetGameObject"></param>
	    /// <param name="serializer"></param>
	    public delegate Component ComponentDecoder(byte[] inBytes, ref int startPos, ref GameObject targetGameObject, IUlibSerializer serializer);

        #endregion
        
        #region Library lists

        /// <summary>
        /// List of types supported by component fileds encode/decode
        /// </summary>
        private static readonly Type[] ComponentTypes = new[] { typeof(Int32), typeof(String), typeof(float), typeof(Single), typeof(Double), 
			typeof(Boolean) ,typeof(Vector2), typeof(Vector3), typeof(Vector4), typeof(Quaternion), typeof(Rect),typeof(Color),
			typeof(Enum),typeof(Texture),typeof(Texture2D), typeof(Mesh),typeof(Collider),typeof(Material),
			typeof(int[]),typeof(string[]),typeof(float[]),typeof(Single[]),typeof(Color[]),typeof(GameObject),typeof(Component),
			typeof(Double[]),typeof(bool[]),typeof(Vector2[]),typeof(Vector3[]),typeof(Vector4[]),typeof(Quaternion[]),typeof(Rect[]),
			typeof(Texture2D[]),typeof(Mesh[]),typeof(Collider[]),typeof(Material[]),typeof(GameObject[]),typeof(Component[]),
            typeof(SkinnedMeshRenderer)
		};

        private static readonly List<string> ParticleEmitterMembers = new List<string>
		    {
				"emit","minSize","maxSize","minEnergy","maxEnergy","minEmission","maxEmission","emitterVelocityScale","worldVelocity",
				"localVelocity","rndVelocity","useWorldSpace","rndRotation","angularVelocity","rndAngularVelocity",/*"particles",*/"enabled"
			};

        /// <summary>
        /// List of used propertys for encode/decode material parameters.
        /// You can add your own property for encode/decode if you want. This list can be change in next version
        /// Default is: "_MainColor","_Color","_SpecColor","_Emission","_SpecularColor","_HighlightColor","_ReflectColor","_Shininess",
        /// "_MainTex","_ShadowTex","_FalloffTex","_BumpMap","_LightMap"
        /// </summary>
        public static readonly Dictionary<string, string> ShaderPropertys = new Dictionary<string, string>
			{
				{"_MainColor","Color"},{"_Color","Color"},{"_SpecColor","Color"},{"_Emission","Color"},{"_SpecularColor","Color"},{"_HighlightColor","Color"},{"_ReflectColor","Color"},
				{"_Shininess","Float"},
				{"_MainTex","Texture"},{"_ShadowTex","Texture"},{"_FalloffTex","Texture"},{"_BumpMap","Texture"},{"_LightMap","Texture"},
                {"_FrontTex","Texture"},{ "_BackTex","Texture"},{ "_LeftTex","Texture"},{ "_RightTex","Texture"},{ "_UpTex","Texture"},{ "_DownTex","Texture"}
		    };

        #endregion
        
        //public Dictionary<int,object> ObjectPack = new Dictionary<int, object>();
	    private readonly Dictionary<int, object> _objectList = new Dictionary<int, object>();
        private int _objectListCount;
	    private readonly Dictionary<int, byte[]> _bytesList = new Dictionary<int, byte[]>();
        private int _bytesPackCount;
        private int _bytesLenght;
        
        private void InitLists()
        {
            //Debug.Log("InitLists");
            _objectList.Clear();// = new Dictionary<int, object>();
            _bytesList.Clear();// = new Dictionary<int, byte[]>();
            _objectListCount = 0;
            _bytesPackCount = 0;
            _bytesLenght = 0;
        }

        /*
	    /// <summary>
	    /// 
	    /// </summary>
	    public static Serializer Instance
	    {
	        get { return _instance ?? (_instance = new Serializer()); }
	    }*/
        
		#region Tools

		/*static List<string> GetAllClasses(string nameSpace)
		{
			var asm = Assembly.GetExecutingAssembly();
			var namespaceList = new List<string>();
			var returnList = new List<string>();
			foreach (var type in asm.GetTypes())
			{
				if (type.Namespace == nameSpace)
					namespaceList.Add(type.Name);
			}
			foreach (var className in namespaceList)
				returnList.Add(className);
			return returnList;
		}*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeclassName"></param>
		/// <returns></returns>
		private Type FindType(string typeclassName)
		{
			var tp = Type.GetType(typeclassName);
			if (tp == null)
			{
				var appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var assembly in appAssemblies)
					if (tp == null)
						tp = assembly.GetType(typeclassName);
			}
			return tp ?? this.GetType().Assembly.GetType(typeclassName, false, true);
		}


        /// <summary>
        /// Check if the byte array is encrypted
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsBytesEncrypted(ref byte[] bytes)
        {
            return bytes[2] == 253;
        }

        /// <summary>
        /// Check if the byte array is compressed
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool IsBytesComressed(ref byte[] bytes)
        {
            return bytes[2] == 253;
        }

        /// <summary>
        /// Key for encryp/decrypt
        /// </summary>
        public byte[] CryptoKey
        {
            get { return _tdes.Key; }
            set { _tdes.Key = value; }
        }
        /// <summary>
        /// Key for encryp/decrypt
        /// </summary>
        public byte[] CryptoIv
        {
            get { return _tdes.IV; }
            set { _tdes.IV = value; }
        }

		#endregion

        #region Plugins

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        internal static bool PluginHandler(IUlibPlugin plugin)
        {
            var iPlugin = (ISerializePlugin) plugin;
            var tp = iPlugin.GetType();
            var ioface = tp.GetInterface("IObjectSerializer");
            if (ioface != null)
            {
                var externalPlugin = (IObjectSerializer)iPlugin;

                var encodeInfo = tp.GetMethod("ObjectEncode");
                var encodeDelegate =
                    (ObjectEncoder)
                    Delegate.CreateDelegate(typeof(ObjectEncoder), iPlugin, encodeInfo);

                TypeEncoders.Add(externalPlugin.ObjectType, encodeDelegate);

                var decodeInfo = tp.GetMethod("ObjectDecode");
                var decodeDelegate =
                    (ObjectDecoder)
                    Delegate.CreateDelegate(typeof(ObjectDecoder), iPlugin, decodeInfo);

                TypeDecoders.Add(externalPlugin.ObjectCode, decodeDelegate);
            }
            var icface = tp.GetInterface("IComponentSerializer");
            if (icface != null)
            {
                var externalPlugin = (IComponentSerializer)iPlugin;


                var encodeInfo = tp.GetMethod("ComponentEncode");
                var encodeDelegate =
                    (ComponentEncoder)
                    Delegate.CreateDelegate(typeof(ComponentEncoder), iPlugin, encodeInfo);

                ComponentEncoders.Add(externalPlugin.ClassName, encodeDelegate);

                var decodeInfo = tp.GetMethod("ComponentDecode");
                var decodeDelegate =
                    (ComponentDecoder)
                    Delegate.CreateDelegate(typeof(ComponentDecoder), iPlugin, decodeInfo);

                ComponentDecoders.Add(externalPlugin.ClassName, decodeDelegate);
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        internal static bool PluginRemove(IUlibPlugin plugin)
        {
            var tp = plugin.GetType();
            var ioface = tp.GetInterface("IObjectSerializer");
            if (ioface != null)
            {
                var externalPlugin = (IObjectSerializer)plugin;

                if (TypeEncoders.ContainsKey(externalPlugin.ObjectType))
                    lock (TypeEncoders)
                        TypeEncoders.Remove(externalPlugin.ObjectType);

                if (TypeDecoders.ContainsKey(externalPlugin.ObjectCode))
                    lock (TypeDecoders)
                        TypeDecoders.Remove(externalPlugin.ObjectCode);
            }
            var icface = tp.GetInterface("IComponentSerializer");
            if (icface != null)
            {
                var externalPlugin = (IComponentSerializer)plugin;

                if (ComponentEncoders.ContainsKey(externalPlugin.ClassName))
                    lock (ComponentEncoders)
                        ComponentEncoders.Remove(externalPlugin.ClassName);

                if (ComponentDecoders.ContainsKey(externalPlugin.ClassName))
                    lock (ComponentDecoders)
                        ComponentDecoders.Remove(externalPlugin.ClassName);
            }
            return true;
        }

        #endregion

        internal int GetResourceIndex(object obj)
        {
            foreach (var o in _objectList)
                if (o.Value.Equals(obj))
                    return o.Key;
            return -1;
        }

        public static Vector3 ParseVector3(string val, char splitter/* = '\0'*/)
        {
            if (splitter == '\0')
                splitter = ';';
            var a = val.Replace(".", ",").Split(splitter);
            return new Vector3(float.Parse(a[0]), float.Parse(a[1]), float.Parse(a[2]));
        }

        public static Quaternion ParseQuaternion(string val, char splitter/* = '\0'*/)
        {
            if (splitter == '\0')
                splitter = ';';
            var a = val.Replace(".",",").Split(splitter);
            return new Quaternion(float.Parse(a[0]), float.Parse(a[1]), float.Parse(a[2]), float.Parse(a[3]));
        }
    }

    

    internal sealed class UObjectPack
    {
        private object[] _objects = new object[0];
        private int _count;
        private int _precount;

        internal int Add(object val)
        {
            _precount = _count;
            _count++;
            var newPack = new object[_count];
            Array.Copy(_objects, newPack, _precount);
            newPack[_precount] = val;
            _objects = newPack;
            return _precount;
        }

        internal bool Contains(object val)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_objects[i] == val)
                    return true;
            }
            return false;
        }

        internal int IndexOf(object val)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_objects[i] == val)
                    return i;
            }
            return -1;
        }

        internal int Count
        {
            get { return _count; }
        }

        internal object Get(int index)
        {
            return _objects[index];
        }
    } 
}