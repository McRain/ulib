using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// Type of Gateway.
	/// </summary>
	public enum GatewayType
	{
		/// <summary>
		/// 
		/// </summary>
		Www = 0,
		/// <summary>
		/// 
		/// </summary>
		Socket = 1,
		/// <summary>
		/// 
		/// </summary>
		File = 2/*,
		/// <summary>
		/// 
		/// </summary>
		Web = 3*/
	}

    public struct GatewayData
    {
        public GatewayType TypeGateway;
        public string Host;
        public string Path;
        public string File;
        public int Port;
        public object Data;
    }

	/// <summary>
	/// 
	/// </summary>
	public enum FileType
	{
		/// <summary>
		/// 
		/// </summary>
		Binary = 0/*,
		Json = 1,
		Xml = 2,
		Text = 3*/
	}

	/// <summary>
	/// 
	/// </summary>
	public enum TransportFormat
	{
		/// <summary>
		/// 
		/// </summary>
		Binary = 0,
		/// <summary>
		/// 
		/// </summary>
		Json = 1,
		/// <summary>
		/// 
		/// </summary>
		Xml = 2,
		/// <summary>
		/// 
		/// </summary>
		Text = 3
	}

	/// <summary>
	/// 
	/// </summary>
    public static class Gateway
	{

		private static int _objectscount;

        /*
		/// <summary>
		/// 
		/// </summary>
		public static CookieContainer cookie;*/

		/// <summary>
		/// Enable /disable used session on server side. For get application name only.
		/// </summary>
		public static bool useSession = true;
		/// <summary>
		/// Name of parameter for send to server gateway application name (if you not use session)
		/// Some as U_APP_PARAM from ulib_config.php
		/// </summary>
        public static string AppParameter = "u_app_param";

		/// <summary>
		/// Application name for send to server gateway (if you not use session)
		/// </summary>
		public static string AppName = string.Empty;
		
        /// <summary>
		/// Name of session key 
		/// </summary>
		public static string KeyName = "PHPSESSID";
		
        /// <summary>
		/// SessionId
		/// </summary>
		public static string Key = string.Empty;
		
        /// <summary>
		/// Name of parameter for send class name to server gateway .
		/// Some as $g_targetRequestKey from ulib_config.php
		/// </summary>
		public const string ClassKey = "t";
		
        /// <summary>
		///  Name of parameter for send method name to server gateway .
		/// Some as $$g_methodRequestKey from ulib_config.php
		/// </summary>
		public const string MethodKey = "m";
		
        /// <summary>
		/// Gateway host name 
		/// </summary>
		public static string Host = string.Empty;
		
        /// <summary>
		/// Path to gateway.php on server
		/// </summary>
		public static string Path = string.Empty;
		
        /// <summary>
		/// Name of gateway.php if renamed
		/// </summary>
		public static string File = string.Empty;
		
        /*
        /// <summary>
		/// 
		/// </summary>
		public static IPAddress IpAddress;*/
		
        /// <summary>
		/// 
		/// </summary>
		public static int port;

	    //private static WwwObject _webObject;
		
		//private static SocketObject _socket;
		
		//public static string HomeFolder = "./";
		//public static string BinaryFileExtension = ".unp";

		private static GatewayType _defaultGateway = GatewayType.Www;

        private static readonly Dictionary<string,IRemoteObject> Gateways = new Dictionary<string, IRemoteObject>();

        #region Plugins

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPlugin"></param>
        /// <returns></returns>
        internal static bool PluginHandler(IUlibPlugin iPlugin)
        {
            //ULog.Log("PluginHandler");
            if (iPlugin != null)
            {
                var ioface = (IGatewayPlugin)iPlugin;

                var gatewayKey = ioface.Key;
                var gatewayObject = ioface.Gateway;
                if (gatewayObject != null)
                {
                    gatewayObject.Host = Host+Path;
                    gatewayObject.File = File;
                    //ULog.Log("PluginHandler " + gatewayKey);
                    if (!Gateways.ContainsKey(gatewayKey))
                        Gateways.Add(gatewayKey, gatewayObject);
                    else
                        throw new GatewayException(string.Format("A duplicate key : {0}.", gatewayKey));
                }
                else
                    throw new GatewayException(string.Format("Gateway is null in plugin."));
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
            var ioface = tp.GetInterface("IGatewayPlugin");
            if (ioface != null)
            {
                var gatewayKey = tp.GetProperty("Key");
                if (Gateways.ContainsKey(gatewayKey.ToString()))
                    Gateways.Remove(gatewayKey.ToString());
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool PluginExists(string key)
        {
            return Gateways.ContainsKey(key);
        }

        #endregion

        /// <summary>
		/// Debug mode enable.
		/// If true - gateway use $editorKeys from ulib_config.php for get application name
		/// Use only from editor!
		/// </summary>
		public static bool Debug
		{
			get { return Serializer.debug; }
			set { Serializer.debug = value; }
		}
		
        /// <summary>
		/// 
		/// </summary>
		public static readonly Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();

	   
        /*private static readonly Dictionary<int,IRemoteObject> RemoteObjects = new Dictionary<int, IRemoteObject>();
	    private static int _remoteObjectsKey;*/

		#region Init
		/// <summary>
		/// Default gateway type for gateway.
		/// If you use GetSender() - this return sender for default gateway
		/// </summary>
		public static GatewayType DefaultGateway
		{
			get { return _defaultGateway; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static void Init(GatewayData data)
        {
            switch (data.TypeGateway)
            {
                case GatewayType.File:
                    {
                        if (data.Data != null && data.Data.ToString() != string.Empty)
                            Key = data.Data.ToString();
                        Host = data.Host;
                        Path = data.Path;
                        File = data.File;
                        break;
                    }
                case GatewayType.Socket:
                    {
                        if (data.Data != null && data.Data.ToString() != string.Empty)
                            Key = data.Data.ToString();
                        Host = data.Host;
                        Path = data.Path;
                        File = data.File;
                        port = data.Port;
                        break;
                    }
                default:
                    {
                        if (data.Data!=null && data.Data.ToString()!=string.Empty)
                            Key = data.Data.ToString();
                        Host = data.Host;
                        Path = data.Path;
                        File = data.File;
                        foreach (var remoteObject in Gateways)
                        {
                            remoteObject.Value.Host = Host+Path;
                            remoteObject.Value.File = File;
                        }
                        break;
                    }
            }
            Init(data.TypeGateway);
        }

		private static void Init(GatewayType gatewayType)
		{
			_defaultGateway = gatewayType;
		    
			/*if (_defaultGateway == GatewayType.Web)
				cookie = new CookieContainer();*/
		}

		/// <summary>
		/// Init with default gatewat type = GatewayType.File
		/// </summary>
		/// <param name="path"></param>
		public static void Init(string path)
		{
			ResourceManager.BasePath = path;
			Init(GatewayType.File);
		}

		/// <summary>
		/// Init with default gatewat type = GatewayType.Www
		/// </summary>
		public static void Init(string host,string path,string gateFile)
		{
			Host = host;
			Path = path;
			File = gateFile;
            foreach (var remoteObject in Gateways)
            {
                remoteObject.Value.Host = Host + Path;
                remoteObject.Value.File = File;
            }
			//cookie = new CookieContainer();
			//cookie.Add(new Cookie(SessionName, SessionId, "/", WebUrl));
			//Init(Application.isWebPlayer ? GatewayType.Www : GatewayType.Web);
			Init(GatewayType.Www);
		}
		/// <summary>
		/// 
		/// </summary>
		public static void Init(string host, string path, string gateFile,GatewayType type)
		{
			Host = host;
			Path = path;
			File = gateFile;

			//cookie = new CookieContainer();
			//cookie.Add(new Cookie(SessionName, SessionId, "/", WebUrl));
			//Init(Application.isWebPlayer ? GatewayType.Www : GatewayType.Web);
			Init(type);
		}
		/*
        /// <summary>
		/// 
		/// </summary>
		public static void Init(IPAddress address,int socketPort)
		{
			IpAddress = address;
			port = socketPort;
			Init(GatewayType.Socket);
		}*/
		
		/*public static void Init(string folder)
		{
			HomeFolder = folder;
			Init(GatewayType.File);
		}*/

		#endregion



		#region Senders
		/// <summary>
		/// Return IRemoteObject for default gateway type
		/// </summary>
		public static IRemoteObject GetSender()
		{
			return GetSender(_defaultGateway.ToString());
		}
		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="GatewayException"></exception>
        public static IRemoteObject GetSender(string key)
        {
            if (Gateways.ContainsKey(key))
                return Gateways[key];
            throw new GatewayException(string.Format("The key : {0} not find.", key));
        }

	    /// <summary>
		/// Return IRemoteObject
		/// </summary>
        public static IRemoteObject GetSender(GatewayType gatewayType)
	    {
	        return GetSender(gatewayType.ToString());
            //return new GameObject("WWWObject_" + _objectscount++).AddComponent<WwwObject>();
		    /*switch (gatewayType)
		    {
		        /*case GatewayType.File:
		            return new GameObject("FileObject_" + _objectscount++).AddComponent<FileObject>();*/
		            /*case GatewayType.Socket:
                if (_socket == null)
                    _socket = new SocketObject(IpAddress, port);
                iRemoteObject = _socket;
                break;*/
		            /*case GatewayType.Web:
                    iRemoteObject = new WebObject();
                    break;*/
		        /*default:
		            return new GameObject("WWWObject_" + _objectscount++).AddComponent<WwwObject>();
		            /*if (!string.IsNullOrEmpty(AppName))
		                wwwObject.AddField(AppParameter, AppName);*/
		            /*if (!string.IsNullOrEmpty(Key))
		                wwwObject.AddCookie(KeyName, Key);*/
		            /*wwwObject.key = _irokey++;
                    RemoteObjects.Add(wwwObject.key, wwwObject);*/
		    //}
		}


        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IRemoteObject GetSender(int key)
        {
            return RemoteObjects.ContainsKey(key) ? RemoteObjects[key] : GetSender();
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="senderId"></param>
	    /// <param name="gatewayType"></param>
	    /// <returns></returns>
	    public static IRemoteObject GetSender(int senderId,GatewayType gatewayType)
	    {
	        return RemoteObjects.ContainsKey(senderId) ? RemoteObjects[senderId] : GetSender(gatewayType);
	    }*/

	    #endregion

		/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <param name="post"></param>
		/// <param name="get"></param>
		/// <param name="callback"></param>
		public static void DirectLoad(string url,Hashtable post,Hashtable get ,ResourceManager.OnGetResource callback)
		{
			new GameObject().AddComponent<ResourceLoader>().LoadFromUrl(
						url, "", ResourceSource.Text, callback, false, post, get);
		}*/

		/*public static SocketObject GetSocket()
		{
			return _socket;
		}*/
		/*/// <summary>
		/// 
		/// </summary>
		public static void CloseSocket()
		{
			if (_socket != null)
			{
				_socket.Disconnect();
				_socket = null;
			}
		}*/
	}
}
