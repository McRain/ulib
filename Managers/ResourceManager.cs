using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ULIB
{
    /// <summary>
    /// Used for load , cache and return resource
    /// </summary>
    public sealed class ResourceManager
    {
        private static ResourceManager _instance;

        /// <summary>
        /// Delegate called on resource find or loaded.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        public delegate void OnGetResource(string key, object resource);
        /*
		/// <summary>
		/// 
		/// </summary>
		public static string defaultProtocol = ResourceProtocol.Gateway;

		/// <summary>
		/// 
		/// </summary>
		public static string iconsExtension = ".png";
		/// <summary>
		/// 
		/// </summary>
		public static bool iconsDecode;

		/// <summary>
		/// Based texture folder.
		/// </summary>
		public static string textureFolder = "";*/

        /// <summary>
        /// Based texture extension.
        /// </summary>
        public static string textureExtension = "png";

        /// <summary>
        /// Is texture encoded to ULIB format.
        /// </summary>
        public static bool textureEncoded;

        /// <summary>
        /// 
        /// </summary>
        public static string serverClassName = "ResourceManager";

        /// <summary>
        /// 
        /// </summary>
        public static string resourceLoadMethod = "LoadResource";

        /// <summary>
        /// 
        /// </summary>
        public static string audioExtension = "ogg";
        /*
		/// <summary>
		/// 
		/// </summary>
		public static string audioFolder = "ogg";*/

        /// <summary>
        /// 
        /// </summary>
        public static bool audioEncoded;

        /// <summary>
        /// 
        /// </summary>
        public static string BasePath = "";

        /*public static readonly Dictionary<ResourceSource> */
        /*internal static class ResourceType
    {
        internal const string Texture = "texture";
        internal const string Text = "text";
        internal const string AudioClip = "audioClip";
        internal const string AssetBundle = "assetBundle";
        internal const string Movie = "movie";
        internal const string Bytes = "bytes";
    }*/
        /*
	    /// <summary>
	    /// 
	    /// </summary>
	    public static readonly Dictionary<ResourceSource, string> ResourceTypes = new Dictionary<ResourceSource, string>();/*{
		                                                                          		{ResourceSource.Assets, "AssetBundle"},
		                                                                          		{ResourceSource.AudioClips, "AudioClip"},
		                                                                          		{ResourceSource.Textures, "Texture2D"},
		                                                                          		{ResourceSource.Movies, "MovieTexture"}
		                                                                          	};*/


        /// <summary>
        /// 
        /// </summary>
        [System.Obsolete("Use ResourcesPaths")]
        public static readonly Dictionary<ResourceSource, string> ResourcePaths = new Dictionary<ResourceSource, string>(){
		                                                                          		{ResourceSource.Assets, "assets"},
		                                                                          		{ResourceSource.AudioClips, "sound"},
		                                                                          		{ResourceSource.GameObjects, "assets"},
		                                                                          		{ResourceSource.Gui, "gui"},
		                                                                          		{ResourceSource.Icons, "icons"},
		                                                                          		{ResourceSource.Meshes, "assets"},
		                                                                          		{ResourceSource.Movies, "movies"},
		                                                                          		{ResourceSource.Music, "music"},
		                                                                          		{ResourceSource.Textures, "textures"},
		                                                                          		{ResourceSource.Text, "assets"},
		                                                                          		{ResourceSource.Translations, "languages"},
		                                                                          		{ResourceSource.Resource, "assets"}
		                                                                          	};

        /// <summary>
        /// Folders name for loaded the resources via ResourceManager . 
        /// You can change value.
        /// If change - please change on server side too. 
        /// Resource search by path: ResourceManager.BasePath+ResourceManager.ResourcesPaths[ResourceType]+key+extension
        /// </summary>
        public static readonly Dictionary<ResourceType, string> ResourcesPaths = new Dictionary<ResourceType, string>()
                                                                                    {
                                                                                        {ResourceType.Assets, "assets"},
		                                                                          		{ResourceType.AudioClips, "sound"},
		                                                                          		{ResourceType.GameObjects, "assets"},
		                                                                          		{ResourceType.Gui, "gui"},
		                                                                          		{ResourceType.Icons, "icons"},
		                                                                          		{ResourceType.Meshes, "assets"},
		                                                                          		{ResourceType.Movies, "movies"},
		                                                                          		{ResourceType.Music, "music"},
		                                                                          		{ResourceType.Textures, "textures"},
		                                                                          		{ResourceType.Text, "assets"},
		                                                                          		{ResourceType.Translations, "languages"},
		                                                                          		{ResourceType.Resource, "assets"}
                                                                                    };

        private static readonly Dictionary<ResourceType, object> ResourcesNewObject = new Dictionary<ResourceType, object>(){
                                                                                        /*{ResourceType.Assets,"assets"},*/
	                                                                                    {ResourceType.AudioClips,new AudioClip()},
	                                                                                    {ResourceType.GameObjects,new GameObject()},
	                                                                                    /*{ResourceType.Gui, "gui"},*/
                                                                                        {ResourceType.Icons, new Texture2D(1, 1)},
	                                                                                    {ResourceType.Meshes,new Texture2D(1, 1)},
	                                                                                    /*{ResourceType.Movies,"movies"},*/
	                                                                                    {ResourceType.Music,new AudioClip()},
	                                                                                    {ResourceType.Textures,new Texture2D(1, 1)},
	                                                                                    /*{ResourceType.Text, "assets"},*/
	                                                                                    {ResourceType.Translations,"languages"}/*,
	                                                                                    {ResourceType.Resource,"assets"}*/
	                                                                                };

        private static readonly Dictionary<string, AssetBundle> AssetBundles = new Dictionary<string, AssetBundle>();

        private static readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        //private static readonly Dictionary<string, Texture2D> Icons = new Dictionary<string, Texture2D>();

        private static readonly Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();

        //private static readonly Dictionary<string, MovieTexture> MovieTextures = new Dictionary<string, MovieTexture>();

        private static readonly Dictionary<string, Mesh> Meshes = new Dictionary<string, Mesh>();

        private static readonly Dictionary<string, GameObject> GameObjects = new Dictionary<string, GameObject>();

        private static readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        private static readonly Dictionary<string, GUISkin> GuiSkins = new Dictionary<string, GUISkin>();

        private static readonly Dictionary<string, object> UnknowTypes = new Dictionary<string, object>();

        private static ResourceLoader _loader;

        #region Instances

        /// <summary>
        /// Return Instance of ResourceManager
        /// </summary>
        public static ResourceManager Instance
        {
            get { return _instance ?? (_instance = new ResourceManager()); }
        }

        /// <summary>
        /// Return Instance of ResourceManager
        /// </summary>
        public static ResourceManager GetInstance()
        {
            return _instance ?? (_instance = new ResourceManager());
        }

        #endregion

        private static GatewayType _resourceGateway = GatewayType.Www;
        /// <summary>
        /// 
        /// </summary>
        public static GatewayType resourceGateway
        {
            get { return _resourceGateway; }
            set { _resourceGateway = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="decode"></param>
        /// <param name="typeForPath"></param>
        /// <param name="resourceTypes"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static object GetResource(string key, string extension, bool decode, ResourceType typeForPath, ResourceType resourceTypes, OnGetResource callback)
        {
            IDictionary cacheDict = Textures;
            var resObject = ResourcesNewObject[resourceTypes];
            var resExt = extension ?? textureExtension;
            switch (resourceTypes)
            {
                case ResourceType.AudioClips:

                    cacheDict = AudioClips;
                    //resObject = new AudioClip();
                    if (resExt == textureExtension)
                        resExt = audioExtension;
                    break;
                /*case ResourceType.GameObjects:

                     cacheDict = GameObjects;
                     //resObject = new AudioClip();
                     if (resExt == textureExtension)
                         resExt = audioExtension;
                     break;*/
                default:
                    //resObject = new Texture2D(1, 1);
                    break;
            }

            if (!cacheDict.Contains(key))
            {
                cacheDict.Add(key, resObject);
                if (Gateway.Debug)
                    ULog.Log("ResourceManager.GetResource:( " + key + "," + extension + "," + decode + "," + typeForPath + "," + resourceTypes + "," + callback + ")");
                if (resourceGateway == GatewayType.File)
                {
                    if (_loader == null)
                        _loader = new GameObject().AddComponent<ResourceLoader>();
                    _loader.LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcesPaths[typeForPath],
                        key,
                        resExt,
                        typeForPath,
                        callback,
                        decode,
                        resourceTypes);
                    //,resourceTypes,callback, decode
                }
                else
                {
                    if (_loader == null)
                        _loader = new GameObject().AddComponent<ResourceLoader>();
                    _loader.LoadFromGateway(
                    typeForPath,
                    key,
                    resExt,
                    resourceTypes,
                    callback);
                }

            }
            else
                resObject = cacheDict[key];

            if (callback != null)
                callback(key, resObject);
            return resObject;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="extensions"></param>
        /// <param name="decodes"></param>
        /// <param name="typeForPaths"></param>
        /// <param name="resourceTypes"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static object[] GetResources(string[] keys, string[] extensions, bool[] decodes, ResourceType[] typeForPaths, ResourceType[] resourceTypes, OnGetResource callback)
        {
            var keyCount = keys.Length;
            var result = new object[keyCount];
            for (var i = 0; i < keyCount; i++)
                result[i] = GetResource(keys[i], extensions[i], decodes[i], typeForPaths[i], resourceTypes[i], callback);
            return result;
        }

        /*private static Dictionary<int,object> _resources = new Dictionary<int, object>(); 

        public static object GetResource(UResource resource)
        {
            if (_resources.ContainsKey(resource.rid))
            {
                return _resources[resource.rid];
            }
        }*/

        /*
	    /// <summary>
	    /// A method for universal load any resource.
	    /// </summary>
	    /// <param name="key"></param>
	    /// <param name="extension"></param>
	    /// <param name="callback"></param>
	    /// <param name="decode"></param>
	    /// <param name="path"></param>
	    /// <param name="resourceSource"></param>
	    /// <returns></returns>
	    public static object GetResource(string key,string extension, OnGetResource callback, bool decode, ResourceSource path, ResourceSource resourceSource)
        {
            var contains = false;
	        object resObject = new Texture2D(1, 1);
	        var srcExt = ResourceSource.Textures;
	        var resExt = extension;

            switch (resourceSource)
            {
                case ResourceSource.AudioClips:
                    {
                        contains = AudioClips.ContainsKey(key);
                        if (!contains)
                            AudioClips.Add(key, new AudioClip());

                        srcExt = ResourceSource.AudioClips;
                        resExt = resExt ?? audioExtension;

                        resObject = AudioClips[key];
                    }
                    break;
                case ResourceSource.Icons:
                    {
                        contains = AudioClips.ContainsKey(key);
                        if (!contains)
                            AudioClips.Add(key, new AudioClip());

                        srcExt = ResourceSource.AudioClips;
                        resExt = resExt ?? audioExtension;

                        resObject = AudioClips[key];
                    }
                    break;
                default:
                    {
                        if (Gateway.Debug)
                            ULog.Log("ResourceManager.GetResource: type Texture2D");
                        contains = Textures.ContainsKey(key);
                        if (!contains)
                            Textures.Add(key, (Texture2D)resObject);
                        else if (Gateway.Debug)
                            ULog.Log("ResourceManager.GetResource: Textures contains key "+key+" - not start load");

                        resExt = resExt ?? textureExtension;
                    }
                    break;
            }
            if (!contains)
            {
                if(Gateway.Debug)
                    ULog.Log("ResourceManager.GetResource: start load");
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        resExt,
                        srcExt,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        resExt,
                        srcExt,
                        callback);
            }
            if (callback != null)
                callback(key, resObject);
	        return resObject;
        }
        */

        #region Texture

        /// <summary>
        /// Load texture to textures cache
        /// </summary>
        /// <param name="key"></param>
        public static void LoadTexture(string key)
        {
            GetTexture(key, null);
        }

        /// <summary>
        /// Load textures to textures cache
        /// </summary>
        /// <param name="keys"></param>
        public static void LoadTextures(List<string> keys)
        {
            foreach (var key in keys)
                GetTexture(key, null);
        }

        /// <summary>
        /// Loads all the textures of the specified directory/path. 
        /// If you specify the file extension - will be loaded only files with this extension.
        /// 
        /// Loaded textures set as DontDestroyOnLoad
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="path"></param>
        public static void LoadTextures(string extension, OnGetResource callback, bool decode, ResourceSource path)
        {
            if (resourceGateway == GatewayType.File)
            {
                var paths = BasePath + "/" + ResourcePaths[path];
                var fileList = FileManager.GetFiles(paths, false, "", extension);
                foreach (var fName in fileList)//.Select(fileName => new FileInfo(fileName)).Select(fi => fi.Name))
                {
                    var fInfo = new FileInfo(fName);
                    var fileName = fInfo.Name.Replace(fInfo.Extension, "");
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        paths.Replace(":", ""),
                        fileName,
                        string.IsNullOrEmpty(extension) ? fInfo.Extension.Replace(".", string.Empty) : extension,
                        ResourceSource.Textures,
                        callback,
                        decode);
                }
            }
            else
                new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                    path,
                    "",
                    extension ?? textureExtension,
                    ResourceSource.Textures,
                    callback);
        }

        /// <summary>
        /// Returns the texture from the cache by key. 
        /// If the texture does not exist - return the default texture and starts loading the texture from resourceGateway. 
        /// After loading the texture is sended to the specified callback method.
        /// Loaded texture set as DontDestroyOnLoad
        /// 
        /// Default parameter used for load:
        /// Gateway: resourceGateway;
        /// Path: BasePath + ResourcePaths[ResourceSource.Textures]
        /// Extension: textureExtension
        /// Type: ResourceSource.Textures
        /// Encode after load: textureEncoded
        /// </summary>
        /// <param name="key">Key of texture</param>
        /// <param name="callback">Callback method</param>
        /// <returns>Texture2D</returns>
        public static Texture2D GetTexture(string key, OnGetResource callback)
        {
            return (Texture2D)GetResource(key, textureExtension, textureEncoded, ResourceType.Textures, ResourceType.Textures, callback);
        }

        /// <summary>
        /// Returns the texture from the cache by key. 
        /// If the texture does not exist - return the default texture and starts loading the texture from resourceGateway and by key from path. 
        /// After loading the texture is sended to the specified callback method.
        /// Loaded texture set as DontDestroyOnLoad
        /// 
        /// Default parameter used for load:
        /// Gateway: resourceGateway;
        /// Extension: textureExtension
        /// Type: ResourceSource.Textures
        /// Encode after load: textureEncoded
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string key, OnGetResource callback, ResourceSource path)
        {
            return (Texture2D)GetResource(key, textureExtension, textureEncoded, ResourceType.Textures, ResourceType.Textures, callback);
            /*if (!Textures.ContainsKey(key))
            {
                Textures.Add(key, new Texture2D(1, 1));
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback, textureEncoded);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback);
            }
            if (callback != null)
                callback(key, Textures[key]);
            return Textures[key];*/
        }

        /// <summary>
        /// Returns the texture from the cache by key. 
        /// If the texture does not exist - return the default texture and starts loading the texture from resourceGateway. 
        /// After loading the texture is can be decoded and sended to the specified callback method.
        /// Loaded texture set as DontDestroyOnLoad
        /// 
        /// Default parameter used for load:
        /// Gateway: resourceGateway;
        /// Path: BasePath + ResourcePaths[ResourceSource.Textures]
        /// Extension: textureExtension
        /// Type: ResourceSource.Textures
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string key, OnGetResource callback, bool decode)
        {
            if (!Textures.ContainsKey(key))
            {
                Textures.Add(key, new Texture2D(1, 1));
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[ResourceSource.Textures],
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        ResourceSource.Textures,
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback);
            }
            if (callback != null)
                callback(key, Textures[key]);
            return Textures[key];
        }

        /// <summary>
        /// Returns the texture from the cache by key. 
        /// If the texture does not exist - return the default texture and starts loading the texture from resourceGateway. 
        /// After loading the texture is can be decoded and sended to the specified callback method.
        /// Loaded texture set as DontDestroyOnLoad
        /// 
        /// Default parameter used for load:
        /// Gateway: resourceGateway;
        /// Extension: textureExtension
        /// Type: ResourceSource.Textures
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string key, OnGetResource callback, bool decode, ResourceSource path)
        {
            if (!Textures.ContainsKey(key))
            {
                if (Gateway.Debug)
                    ULog.Log(string.Format("GetTexture key: {0}  path: count {1}", key, path));
                Textures.Add(key, new Texture2D(1, 1));
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        textureExtension,
                        ResourceSource.Textures,
                        callback);
            }
            if (callback != null)
                callback(key, Textures[key]);
            return Textures[key];
        }

        /// <summary>
        /// Returns the texture from the cache by key. 
        /// If the texture does not exist - return the default texture and starts loading the texture from resourceGateway. 
        /// After loading the texture is sended to the specified callback method.
        /// Loaded texture set as DontDestroyOnLoad
        /// 
        /// Default parameter used for load:
        /// Gateway: resourceGateway;
        /// Type: ResourceSource.Textures
        /// </summary>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string key, string extension, OnGetResource callback, bool decode, ResourceSource path)
        {
            if (!Textures.ContainsKey(key))
            {
                Textures.Add(key, new Texture2D(1, 1));
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        extension ?? textureExtension,
                        ResourceSource.Textures,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        extension ?? textureExtension,
                        ResourceSource.Textures,
                        callback);
            }
            if (callback != null)
                callback(key, Textures[key]);
            return Textures[key];
        }

        /// <summary>
        /// Remove texture from cache and destroy it.
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveTexture(string key)
        {
            if (Textures.ContainsKey(key))
            {
                Object.Destroy(Textures[key]);
                Textures[key] = null;
                Textures.Remove(key);
            }
        }
        #endregion

        /*/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="key"></param>
		/// <param name="extension"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static Texture2D GetTexture(ResourceSource path, string key, string extension, OnGetResource callback)
		{
			if (!Textures.ContainsKey(key))
			{
				Textures.Add(key, new Texture2D(1, 1));
				new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
					path,
					key,
					extension,
					ResourceSource.Textures,
					callback);
			}
			callback(key, Textures[key]);
			return Textures[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="key"></param>
		/// <param name="extension"></param>
		/// <param name="callback"></param>
		/// <param name="decode"></param>
		/// <returns></returns>
		public static Texture2D GetTextureFile(ResourceSource path, string key, string extension, OnGetResource callback, bool decode)
		{
			if (!Textures.ContainsKey(key))
			{
				Textures.Add(key, new Texture2D(1, 1));
				new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
					path,
					key,
					extension,
					ResourceSource.Textures,
					callback,
					decode);

			}
			callback(key, Textures[key]);
			return Textures[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="key"></param>
		/// <param name="extension"></param>
		/// <param name="callback"></param>
		/// <param name="decode"></param>
		/// <param name="postData"></param>
		/// <param name="getData"></param>
		/// <returns></returns>
		public static Texture2D GetTextureUrl(string path, string key, string extension, OnGetResource callback, bool decode,
			Hashtable postData, Hashtable getData)
		{
			if (!Textures.ContainsKey(key))
			{
				Textures.Add(key, new Texture2D(1, 1));
				new GameObject().AddComponent<ResourceLoader>().LoadFromUrl(
					ResourceProtocol.Http + path.Replace("http://", "").Replace("https://", ""),
					key,
					ResourceSource.Textures,
					callback,
					decode,
					postData,
					getData);
			}
			callback(key, Textures[key]);
			return Textures[key];
		}

		/// <summary>
		/// Return texture and return texture to callback. If texture not exist - return empty texture and start load.
		/// </summary>
		/// <param name="protocol">'file://','http://','https://' or ''. You can use constants from ResourceProtocol</param>
		/// <param name="resourcePath"></param>
		/// <param name="path">part of texture url for direct load, or value from ResourceManager.ResourcePaths for gateway or file</param>
		/// <param name="key">part of texture url for direct load or name of file for use Gateway or load from file</param>
		/// <param name="extension">extension of texture for all download. must contains "." (dot) if need used</param>
		/// <param name="callback">method for recoved key of texture and texture </param>
		/// <param name="decode">use or not decode recived texture (for direct load from url or file)</param>
		/// <param name="postData">POST parameters for direct url load only</param>
		/// <param name="getData">GET parameters for direct url load only</param>
		/// <returns></returns>
		public static Texture2D GetTexture(string protocol,ResourceSource resourcePath, string path, string key, string extension, OnGetResource callback, 
			bool decode ,Hashtable postData,Hashtable getData)
		{
			if (!Textures.ContainsKey(key))
			{
				Textures.Add(key, new Texture2D(1, 1));
				var loader = new GameObject().AddComponent<ResourceLoader>();
				switch (protocol)
				{
					case ResourceProtocol.Gateway:
						loader.LoadFromGateway(resourcePath, key,extension, ResourceSource.Textures, callback);
						break;
					case ResourceProtocol.File:
						loader.LoadFromFile(resourcePath, key, extension, ResourceSource.Textures, callback, decode);
						break;
					default:
						loader.LoadFromUrl(protocol+path.Replace("http://", "").Replace("https://", ""), key,ResourceSource.Textures, callback, decode, postData, getData);
						break;
				}
				//LoadResource(protocol, path.Replace("http://", "").Replace("https://", "").Replace(":/", "/"), name, extension, callback, ResourceSource.Textures, decode, postData, getData);
			}
			if (callback != null)
				callback(key, Textures[key]);
			return Textures[key];
		}*/
        //#endregion

        #region Icons

        /*
		/// <summary>
		/// Return icon from cashe. If icon not exist - load icon from Gateway.DefaultGateway.
		/// </summary>
		/// <param name="key">Name of icon</param>
		/// <param name="callback">Callback</param>
		/// <returns></returns>
		public static Texture2D GetIcon(string key, OnGetResource callback)
		{
			/*--
             
            if (!Icons.ContainsKey(key))
			{
				Icons.Add(key, new Texture2D(1, 1));
				if (resourceGateway == GatewayType.File)
				{
					new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
						BasePath.Replace(":", "") + "/" + ResourcePaths[ResourceSource.Icons],
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback, iconsDecode);
				}
				else
					new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
						ResourceSource.Icons,
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback);
			}
			callback(key, Icons[key]);*/
        /*return Icons[key];
    }*/
        /*--
		/// <summary>
		/// Return icon from cashe. If icon not exist - load icon from Gateway.DefaultGateway and path.
		/// </summary>
		/// <param name="key">Name of icon</param>
		/// <param name="callback">Callback</param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Texture2D GetIcon(string key, OnGetResource callback,ResourceSource path)
		{
			if (!Icons.ContainsKey(key))
			{
				Icons.Add(key, new Texture2D(1, 1));
				if (resourceGateway == GatewayType.File)
				{
					new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
						BasePath.Replace(":", "") + "/" + ResourcePaths[path],
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback, iconsDecode);
				}
				else
					new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
						path,
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback);
			}
			callback(key, Icons[key]);
			return Icons[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		/// <param name="decode"></param>
		/// <returns></returns>
		public static Texture2D GetIcon(string key, OnGetResource callback, bool decode)
		{
			if (!Icons.ContainsKey(key))
			{
				Icons.Add(key, new Texture2D(1, 1));
				if (resourceGateway == GatewayType.File)
				{
					new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
						BasePath.Replace(":", "") + "/" + ResourcePaths[ResourceSource.Icons],
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback, decode);
				}else
					new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
						ResourceSource.Icons,
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback);
			}
			callback(key, Icons[key]);
			return Icons[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="callback"></param>
		/// <param name="decode"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Texture2D GetIcon(string key, OnGetResource callback, bool decode,ResourceSource path)
		{
			if (!Icons.ContainsKey(key))
			{
				Icons.Add(key, new Texture2D(1, 1));
				if (resourceGateway == GatewayType.File)
				{
					new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
						BasePath.Replace(":", "") + "/" + ResourcePaths[path],
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback, decode);
				}
				else
					new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
						path,
						key,
						iconsExtension,
						ResourceSource.Textures,
						callback);
			}
			callback(key, Icons[key]);
			return Icons[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="extension"></param>
		/// <param name="callback"></param>
		/// <param name="decode"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Texture2D GetIcon(string key,string extension, OnGetResource callback, bool decode, ResourceSource path)
		{
			if (!Icons.ContainsKey(key))
			{
				Icons.Add(key, new Texture2D(1, 1));
				if (resourceGateway == GatewayType.File)
				{
					new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
						BasePath.Replace(":", "") + "/" + ResourcePaths[path],
						key,
						extension,
						ResourceSource.Textures,
						callback, decode);
				}
				else
					new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
						path,
						key,
						extension,
						ResourceSource.Textures,
						callback);
			}
			callback(key, Icons[key]);
			return Icons[key];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public static void RemoveIcon(string key)
		{
			if (Icons.ContainsKey(key))
				Icons.Remove(key);
		}*/

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Texture2D GetIcon(ResourceSource path, string key, string extension, OnGetResource callback)
        {
            if (!Icons.ContainsKey(key))
            {
                Icons.Add(key, new Texture2D(1,1));
                new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                    path,
                    key,
                    extension,
                    ResourceSource.Textures,
                    callback);
            }
            callback(key, Icons[key]);
            return Icons[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static Texture2D GetIconFile(ResourceSource path, string key, string extension, OnGetResource callback, bool decode)
        {
            if (!Icons.ContainsKey(key))
            {
                Icons.Add(key, new Texture2D(1,1));
                new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                    path,
                    key,
                    extension,
                    ResourceSource.Textures,
                    callback,
                    decode);

            }
            callback(key, Icons[key]);
            return Icons[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="postData"></param>
        /// <param name="getData"></param>
        /// <returns></returns>
        public static Texture2D GetIconUrl(string path, string key, string extension, OnGetResource callback, bool decode,
            Hashtable postData, Hashtable getData)
        {
            if (!Icons.ContainsKey(key))
            {
                Icons.Add(key, new Texture2D(1,1));
                new GameObject().AddComponent<ResourceLoader>().LoadFromUrl(
                    ResourceProtocol.Http + path.Replace("http://", "").Replace("https://", ""),
                    key,
                    ResourceSource.Textures,
                    callback,
                    decode,
                    postData,
                    getData);
            }
            callback(key, Icons[key]);
            return Icons[key];
        }


        /// <summary>
        /// Return icon and return icon to callback. If icon not exist - return empty icon and start load.
        /// </summary>
        /// <param name="protocol">'file://','http://','https://' or ''. You can use constants from ResourceProtocol</param>
        /// <param name="resourcePath"></param>
        /// <param name="path">part of icon url for direct load, or value from ResourceManager.ResourcePaths for gateway or file</param>
        /// <param name="key">part of icon url for direct load or name of file for use Gateway or load from file</param>
        /// <param name="extension">extension of icon for all download. must contains "." (dot)</param>
        /// <param name="callback">method for recoved key of icon and icon </param>
        /// <param name="decode">use or not decode recived icon (for direct load from url or file)</param>
        /// <param name="postData">POST parameters for direct url load only</param>
        /// <param name="getData">GET parameters for direct url load only</param>
        /// <returns></returns>
        public static Texture2D GetIcon(string protocol, ResourceSource resourcePath, string path, string key, string extension, OnGetResource callback,
            bool decode, Hashtable postData, Hashtable getData)
        {
            if (!Icons.ContainsKey(key))
            {
                Icons.Add(key, new Texture2D(1, 1));
                var loader = new GameObject().AddComponent<ResourceLoader>();
                switch (protocol)
                {
                    case ResourceProtocol.Gateway:
                        loader.LoadFromGateway(resourcePath, key,extension, ResourceSource.Icons, callback);
                        break;
                    case ResourceProtocol.File:
                        loader.LoadFromFile(resourcePath, key, extension, ResourceSource.Icons, callback, decode);
                        break;
                    default:
                        loader.LoadFromUrl(protocol + path.Replace("http://", "").Replace("https://", ""), key, ResourceSource.Icons, callback, decode, postData, getData);
                        break;
                }
            }
            if (callback != null)
                callback(key, Icons[key]);
            return Icons[key];
        }*/
        #endregion



        #region AudioClip

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string key, OnGetResource callback)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                if (Gateway.DefaultGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + ResourcePaths[ResourceSource.AudioClips],
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback, audioEncoded);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        ResourceSource.AudioClips,
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string key, OnGetResource callback, ResourceSource path)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback, audioEncoded);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string key, OnGetResource callback, bool decode)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[ResourceSource.AudioClips],
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        ResourceSource.AudioClips,
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string key, OnGetResource callback, bool decode, ResourceSource path)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        audioExtension,
                        ResourceSource.AudioClips,
                        callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string key, string extension, OnGetResource callback, bool decode, ResourceSource path)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                if (resourceGateway == GatewayType.File)
                {
                    new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                        BasePath.Replace(":", "") + "/" + ResourcePaths[path],
                        key,
                        extension,
                        ResourceSource.AudioClips,
                        callback, decode);
                }
                else
                    new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                        path,
                        key,
                        extension,
                        ResourceSource.AudioClips,
                        callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveAudioClip(string key)
        {
            if (AudioClips.ContainsKey(key))
                AudioClips.Remove(key);
        }

        /*/// <summary>
        /// Return audio and call delegate if audio in cache exist. Or call delegate after loaded audio
        /// </summary>
        /// <param name="audioClipName">Name of file or key for server</param>
        /// <param name="callback">Callback</param>
        /// <returns>audio from cache</returns>
        public static AudioClip GetAudioClip(string audioClipName, OnGetResource callback)
        {
            return GetAudioClip(audioClipName, ResourceSource.AudioClips, callback, audioExtension, audioEncoded);
        }

        /// <summary>
        /// Return audio and call delegate if audio in cache exist. Or call delegate after loaded audio
        /// </summary>
        /// <param name="audioClipName">Name of file or key for server</param>
        /// <param name="callback">Callback</param>
        /// <param name="extension">Extension of audio file.</param>
        /// <returns>audio from cache</returns>
        public static AudioClip GetAudioClip(string audioClipName, OnGetResource callback, string extension)
        {
            return GetAudioClip(audioClipName, ResourceSource.AudioClips, callback, extension, audioEncoded);
        }

        /// <summary>
        /// Return audio and call delegate if audio in cache exist. Or call delegate after loaded audio.
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="callback">Callback</param>
        /// <param name="extension">Extension of audio file.</param>
        /// <param name="decode">Decode local audio</param>
        /// <returns>audio from cache</returns>
        public static AudioClip GetAudioClip(string audioClipName, OnGetResource callback, string extension, bool decode)
        {
            return GetAudioClip(audioClipName, ResourceSource.AudioClips, callback, extension, decode);
        }*/

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string audioClipName, OnGetResource callback)
        {
            return GetAudioClip(audioClipName, ResourceSource.AudioClips, callback, audioExtension, audioEncoded);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioClipName"></param>
        /// <param name="srcType"></param>
        /// <param name="callback"></param>
        /// <param name="extension"></param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string audioClipName, ResourceSource srcType, OnGetResource callback, string extension, bool decode)
        {
            if (!AudioClips.ContainsKey(audioClipName))
            {
                AudioClips.Add(audioClipName, new AudioClip());
                LoadResource(ResourcePaths[srcType], 
                    audioClipName, callback, ResourceSource.AudioClips,extension, decode);
            }
            if (callback != null)
                callback(audioClipName, AudioClips[audioClipName]);
            return AudioClips[audioClipName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <param name="postData"></param>
        /// <param name="getData"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string url, string key, OnGetResource callback, Hashtable postData, Hashtable getData)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                LoadUrlResource(url, key, callback, ResourceSource.AudioClips, postData, getData);
            }
            if (callback != null)
                callback(key, AudioClips[key]);
            return AudioClips[key];
        }
        */

        /*/// <summary>
        /// Return audioClip. If audioClip not exist - load from ULIB gateway and return to callback
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(ResourceSource path, string key, string extension, OnGetResource callback)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                new GameObject().AddComponent<ResourceLoader>().LoadFromGateway(
                    path, 
                    key,
                    extension, 
                    ResourceSource.AudioClips, 
                    callback);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClipFile(ResourceSource path, string key, string extension, OnGetResource callback, bool decode)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                new GameObject().AddComponent<ResourceLoader>().LoadFromFile(
                    path, 
                    key, 
                    extension, 
                    ResourceSource.AudioClips, 
                    callback, 
                    decode);
				
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="extension"></param>
        /// <param name="callback"></param>
        /// <param name="decode"></param>
        /// <param name="postData"></param>
        /// <param name="getData"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClipUrl(string path, string key, string extension, OnGetResource callback,bool decode, 
            Hashtable postData, Hashtable getData)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                new GameObject().AddComponent<ResourceLoader>().LoadFromUrl(
                    ResourceProtocol.Http + path.Replace("http://", "").Replace("https://", ""), 
                    key, 
                    ResourceSource.AudioClips, 
                    callback, 
                    decode, 
                    postData, 
                    getData);
            }
            callback(key, AudioClips[key]);
            return AudioClips[key];
        }

        /// <summary>
        /// Return audioClip and return audioClip to callback. If audioClip not exist - return empty audioClip and start load.
        /// </summary>
        /// <param name="protocol">'file://','http://','https://' or ''. You can use constants from ResourceProtocol</param>
        /// <param name="resourcePath"></param>
        /// <param name="path">part of audioClip url for direct load, or value from ResourceManager.ResourcePaths for gateway or file</param>
        /// <param name="key">part of audioClip url for direct load or name of file for use Gateway or load from file</param>
        /// <param name="extension">extension of audioClip for all download. must contains "." (dot) if need used</param>
        /// <param name="callback">method for recoved key of audioClip and audioClip </param>
        /// <param name="decode">use or not decode recived audioClip (for direct load from url or file)</param>
        /// <param name="postData">POST parameters for direct url load only</param>
        /// <param name="getData">GET parameters for direct url load only</param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string protocol,ResourceSource resourcePath, string path, string key, string extension, OnGetResource callback,
            bool decode, Hashtable postData, Hashtable getData)
        {
            if (!AudioClips.ContainsKey(key))
            {
                AudioClips.Add(key, new AudioClip());
                var loader = new GameObject().AddComponent<ResourceLoader>();
                switch (protocol)
                {
                    case ResourceProtocol.Gateway:
                        loader.LoadFromGateway(resourcePath, key, extension, ResourceSource.AudioClips, callback);
                        break;
                    case ResourceProtocol.File:
                        loader.LoadFromFile(resourcePath, key, extension, ResourceSource.AudioClips, callback, decode);
                        break;
                    default:
                        loader.LoadFromUrl(protocol + path.Replace("http://", "").Replace("https://", ""), key, ResourceSource.AudioClips, callback, decode, postData, getData);
                        break;
                }
                //LoadResource(protocol, path.Replace("http://", "").Replace("https://", "").Replace(":/", "/"), name, extension, callback, ResourceSource.AudioClips, decode, postData, getData);
            }
            if (callback != null)
                callback(key, AudioClips[key]);
            return AudioClips[key];
        }*/

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="voiceText"></param>
        /// <param name="callback"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static AudioClip GetVoice(string voiceText, OnGetResource callback,string language)
        {
            if (!AudioClips.ContainsKey(voiceText))
            {
                AudioClips.Add(voiceText, new AudioClip());
                //LoadUrlResource("http://translate.google.com/translate_tts?tl="+language+"&q="+voiceText, voiceText, callback, ResourceSource.AudioClips);
                LoadUrlResource("http://yourhost.sytes.net/ogg.ogg", voiceText, callback, ResourceSource.AudioClips,
                    new Hashtable { { "fupload", "http://translate.google.com/translate_tts?tl=" + language + "&q=" + voiceText }, { "mp3","" } });
            }
            if (callback != null)
                callback(voiceText, AudioClips[voiceText]);
            return AudioClips[voiceText];
        }*/

        #endregion

        #region GUISkin

        /*/// <summary>
		/// 
		/// </summary>
		/// <param name="skinName"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public static GUISkin GetGUI(string skinName,OnGetResource callback)
		{
			if(!GUISkins.ContainsKey(skinName))
			{
				GUISkins.Add(skinName,new GUISkin());
				LoadResource(BasePath.Replace(":", "") + "/" + ResourcePaths[ResourceSource.AudioClips], skinName, callback, "guiskin", ResourceSource.Gui, false);
			}
			if (callback != null)
				callback(skinName, GUISkins[skinName]);
			return GUISkins[skinName];
		}*/

        #endregion

        #region Models

        public static Mesh CreatePlane()
        {
            return new Mesh
                       {
                           vertices = new[]
                                          {
                                              new Vector3(0.5f, 0.5f, 0.0f), new Vector3(-0.5f, 0.5f, 0.0f),
                                              new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(0.5f, -0.5f, 0.0f)
                                          },
                           uv = new[]
                                    {
                                        new Vector2(1.0f, 1.0f), new Vector2(0.0f, 1.0f),
                                        new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f)
                                    },
                           triangles = new[]
                                           {
                                               3, 2, 0, 0, 2, 1
                                           },
                           normals = new[]
                                         {
                                             new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f),
                                             new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f),
                                         }
                       };
        }

        public static Mesh CreatePlane9()
        {
            return new Mesh
                       {
                           vertices = new[]
                                          {
                                              new Vector3(-0.5f, -0.5f, 0.0f), new Vector3(-0.2f, -0.5f, 0.0f),
                                              new Vector3(0.2f, -0.5f, 0.0f), new Vector3(0.5f, -0.5f, 0.0f),
                                              new Vector3(-0.5f, -0.2f, 0.0f), new Vector3(-0.2f, -0.2f, 0.0f),
                                              new Vector3(0.2f, -0.2f, 0.0f), new Vector3(0.5f, -0.2f, 0.0f),
                                              new Vector3(-0.5f, 0.2f, 0.0f), new Vector3(-0.2f, 0.2f, 0.0f),
                                              new Vector3(0.2f, 0.2f, 0.0f), new Vector3(0.5f, 0.2f, 0.0f),
                                              new Vector3(-0.5f, 0.5f, 0.0f), new Vector3(-0.2f, 0.5f, 0.0f),
                                              new Vector3(0.2f, 0.5f, 0.0f), new Vector3(0.5f, 0.5f, 0.0f)
                                          },
                           uv = new[]
                                    {
                                        new Vector2(0.0f, 0.0f), new Vector2(0.3f, 0.0f),new Vector2(0.7f, 0.0f),
                                        new Vector2(1.0f, 0.0f),new Vector2(0.0f, 0.3f), new Vector2(0.3f, 0.3f),
                                        new Vector2(0.7f, 0.3f),new Vector2(1.0f, 0.3f),new Vector2(0.0f, 0.7f), 
                                        new Vector2(0.3f, 0.7f),new Vector2(0.7f, 0.7f),new Vector2(1.0f, 0.7f),
                                        new Vector2(0.0f, 1.0f), new Vector2(0.3f, 1.0f),new Vector2(0.7f, 1.0f),
                                        new Vector2(1.0f, 1.0f)
                                    },
                           triangles = new[]
                                           {
                                               14, 15, 10, 10, 15, 11, 10, 11, 6, 6, 11, 7, 6, 7, 2, 2, 7, 3, 1,
                                               0, 5, 5, 0, 4, 5, 4,
                                               9,
                                               9, 4, 8, 9, 8, 13, 13, 8, 12, 13, 14, 9, 9, 14, 10, 9, 10, 5, 5,
                                               10, 6, 5, 6, 1, 1, 6,
                                               2
                                           }
                       };
        }



        #endregion

        /*private static void LoadResource(string protocol, ResourceSource path, string key, string extension, OnGetResource onLoad, 
			ResourceSource resType, 
			bool decode,Hashtable post,Hashtable gets)
		{
			
		}*/


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static object AddResource(string key, object resource)
        {
            //Debug.Log("AddResource "+key+" : "+resource);

            switch (resource.GetType().FullName)
            {
                case "UnityEngine.Texture2D":
                    if (!Textures.ContainsKey(key))
                        Textures.Add(key, (Texture2D)resource);
                    else
                        Textures[key] = (Texture2D)resource;
                    Object.DontDestroyOnLoad(Textures[key]);
                    return Textures[key];
                case "UnityEngine.AudioClip":
                    if (!AudioClips.ContainsKey(key))
                        AudioClips.Add(key, (AudioClip)resource);
                    else
                        AudioClips[key] = (AudioClip)resource;
                    Object.DontDestroyOnLoad(AudioClips[key]);
                    return AudioClips[key];
                case "UnityEngine.AssetBundle":
                    if (!AssetBundles.ContainsKey(key))
                        AssetBundles.Add(key, (AssetBundle)resource);
                    else
                        AssetBundles[key] = (AssetBundle)resource;
                    Object.DontDestroyOnLoad(AssetBundles[key]);
                    return AssetBundles[key];
                /*case "UnityEngine.MovieTexture":
                    if (!MovieTextures.ContainsKey(key))
                        MovieTextures.Add(key, (MovieTexture)resource);
                    else
                        MovieTextures[key] = (MovieTexture)resource;
                    Object.DontDestroyOnLoad(MovieTextures[key]);
                    return MovieTextures[key];*/
                case "UnityEngine.Mesh":
                    if (!Meshes.ContainsKey(key))
                        Meshes.Add(key, (Mesh)resource);
                    else
                        Meshes[key] = (Mesh)resource;
                    Object.DontDestroyOnLoad(Meshes[key]);
                    return Meshes[key];
                case "UnityEngine.GameObject":
                    if (!GameObjects.ContainsKey(key))
                        GameObjects.Add(key, (GameObject)resource);
                    else
                        GameObjects[key] = (GameObject)resource;
                    Object.DontDestroyOnLoad(GameObjects[key]);
                    return GameObjects[key];
                case "UnityEngine.Material":
                    if (!Materials.ContainsKey(key))
                        Materials.Add(key, (Material)resource);
                    else
                        Materials[key] = (Material)resource;
                    Object.DontDestroyOnLoad(Materials[key]);
                    return Materials[key];
                case "UnityEngine.GUISkin":
                    if (!GuiSkins.ContainsKey(key))
                        GuiSkins.Add(key, (GUISkin)resource);
                    else
                        GuiSkins[key] = (GUISkin)resource;
                    Object.DontDestroyOnLoad(GuiSkins[key]);
                    return GuiSkins[key];
                default:
                    if (!UnknowTypes.ContainsKey(key))
                        UnknowTypes.Add(key, resource);
                    else
                        UnknowTypes[key] = resource;
                    return UnknowTypes[key];
            }
            //return UnknowTypes[key];//---
        }

        /*--

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="resourceType"></param>
		public static void RemoveResource(string key, ResourceSource resourceType)
		{
			switch (resourceType)
			{
				case ResourceSource.Textures:
					if (Textures.ContainsKey(key))
						Textures.Remove(key);
					break;
				case ResourceSource.AudioClips:
					if (AudioClips.ContainsKey(key))
						AudioClips.Remove(key);
					break;
				case ResourceSource.Icons:
					if (Icons.ContainsKey(key))
						Icons.Remove(key);
					break;
				default:
					if (UnknowTypes.ContainsKey(key))
						UnknowTypes.Remove(key);
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		public static void RemoveResources(string key)
		{
			RemoveKeyFromContainer(AssetBundles, key);
			RemoveKeyFromContainer(AudioClips, key);
			RemoveKeyFromContainer(GUISkins, key);
			RemoveKeyFromContainer(GameObjects, key);
			RemoveKeyFromContainer(Icons, key);
			RemoveKeyFromContainer(Materials, key);
			RemoveKeyFromContainer(Meshes, key);
			RemoveKeyFromContainer(MovieTextures, key);
			RemoveKeyFromContainer(Textures, key);
			RemoveKeyFromContainer(UnknowTypes, key);
		}

		private static void RemoveKeyFromContainer(IDictionary dict,string key)
		{
			if(dict.Contains(key))
				dict.Remove(key);
		}*/
    }
}
