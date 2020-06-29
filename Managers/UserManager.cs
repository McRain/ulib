using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace ULIB
{
	/// <summary>
	/// Provides the functions of registration and authorization
	/// </summary>
	public sealed class UserManager
	{
		private static UserManager _instance;

		/// <summary>
		/// The delegate for events is called when data is received from the server: registration and verify
		/// </summary>
		/// <param name="val"></param>
		/// <param name="data"></param>
		public delegate void UserEvent(int val, object data);

		/// <summary>
		/// Specifies that data sent UserManager and waits for a response. If true - the new request will not be sent.
		/// </summary>
		public static bool SendedRequest;

		/// <summary>
		/// 
		/// </summary>
		public static event UserEvent OnRegister;

		/// <summary>
		/// Called when recived answer from authorization query.
		/// Contains userId (or 0 if failed autorization) and Dictionary with your data if you used it.
		/// </summary>
		public static event UserEvent OnLogin;

		/// <summary>
		/// Called when recived answer from close session query.
		/// Conatins 0 if logout and Dictionary with your data if you used it.
		/// </summary>
		public static event UserEvent OnLogout;

		/// <summary>
		/// Key for login parameter in query.If you change you must to change the same on the server.
		/// </summary>
		public static string KeyLogin = "login";

		/// <summary>
		/// Key for password parameter in query.If you change you must to change the same on the server.
		/// </summary>
		public static string KeyPassword = "pass";

		/// <summary>
		/// Key for captcha parameter in query.If you change you must to change the same on the server.
		/// </summary>
		public static string KeyCaptcha = "captcha";

		/// <summary>
		/// Key for additional data parameter in query. If you change you must to change the same on the server.
		/// </summary>
		public static string KeyData = "data";

		/// <summary>
		/// Key for user id parameter in query. If you change you must to change the same on the server.
		/// </summary>
		public static string KeyId = "id";

	    /// <summary>
        /// Key for username parameter in query. If you change you must to change the same on the server.
	    /// </summary>
	    public static string KeyName = "username";

		/// <summary>
		/// The name of the server class to invoke methods of authorization.
		/// </summary>
		public static string ServerClass = "UserManager";

		/// <summary>
		/// The name of the function called on the server for registration
		/// </summary>
		public static string RegisterMethod = "Register";

		/// <summary>
		/// The name of the function called on the server for authorization
		/// </summary>
		public static string LoginMethod = "Login";

		/// <summary>
		/// The name of the function called on the server for close session
		/// </summary>
		public static string LogoutMethod = "Logout";

		/// <summary>
		/// Enable used captcha . If you change you must to change the same on the server (see UserManager.php).
		/// </summary>
		public static bool UseCaptcha = true;

	    private static IRemoteObject _www;

		private static Texture2D _captcha;

        /// <summary>
        /// 
        /// </summary>
        public static GatewayType gatewayType = GatewayType.Www;

		#region Instance

		/// <summary>
		/// Return instance of UserManager
		/// </summary>
		public static UserManager Instance
		{
			get { return _instance ?? (_instance = new UserManager()); }
		}

		/// <summary>
		/// Return instance of UserManager
		/// </summary>
		public static UserManager GetInstance()
		{
			return _instance ?? (_instance = new UserManager());
		}

		#endregion

		/// <summary>
		/// Image captcha. Can be null - check before view to OnGUI
		/// </summary>
		public static Texture2D captchaTexture
		{
			get { return _captcha; }
		}

		/// <summary>
		/// Contains 0 or user UID recived from server after autorization 
		/// </summary>
		public static int UserId { get; private set; }

		#region Register
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userLogin"></param>
        /// <param name="userPassword"></param>
        /// <param name="info"></param>
        /// <param name="gateway"></param>
        public static void Register(string userName, string userLogin, string userPassword, Dictionary<string, string> info, GatewayType gateway)
        {
            SendedRequest = true;
            var ht = new Hashtable { { KeyName, userName },{KeyLogin,userLogin}, { KeyPassword, userPassword },{ KeyData, info } };
            if (_www == null)
                _www = Gateway.GetSender(gateway);
            _www.Call(ServerClass, RegisterMethod, ht, OnReg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userLogin"></param>
        /// <param name="userPassword"></param>
        /// <param name="info"></param>
        public static void Register(string userName, string userLogin, string userPassword, Dictionary<string, string> info)
        {
            Register(userName, userLogin, userPassword, info, gatewayType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userLogin"></param>
        /// <param name="userPassword"></param>
        public static void Register(string userName, string userLogin, string userPassword)
        {
            Register(userName, userLogin, userPassword, null, gatewayType);
        }
        /*
	    /// <summary>
	    /// Send user data to server. Callback method.
	    /// </summary>
	    /// <param name="data"></param>
	    /// <param name="callback"></param>
	    public static void Register(Hashtable data,UserEvent callback)
	    {
	        SendedRequest = true;
	        //_onRegCallback = callback;
            if (_www == null)
                _www = (WwwObject)Gateway.GetSender(gatewayType);
            _www.Call(ServerClass, RegisterMethod, data, OnReg);
	    }
        */
        private static void OnReg(object inData)
        {
            SendedRequest = false;
            var id = 0;
            if (inData is Hashtable)
            {
                var hash = (Hashtable)inData;
                id = (int)hash[KeyId];
                if (OnRegister != null)
                    OnRegister(id, hash);
                return;
            }
            if (OnRegister != null)
                OnRegister(id, inData);

            
        }

		/*/// <summary>
	/// 
	/// </summary>
	/// <param name="login"></param>
	/// <param name="password"></param>
	public static void Register(string login,string password)
	{
		Register(login,password,null);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="login"></param>
	/// <param name="password"></param>
	/// <param name="data"></param>
	public static void Register(string login,string password,Dictionary<string,object> data)
	{
		SendedRequest = true;
		if(login.Length==0 || password.Length==0)
		{
			if (OnRegister != null)
				OnRegister(-1,"length");
			SendedRequest = false;
		}
		else
		{
			var ht = new Hashtable { { KeyLogin, login }, { KeyPassword, password } };
			if(data!=null)
				ht.Add(KeyData, data);
			Gateway.GetSender().Call(Instance, RegisterMethod, ht, OnReg);
		}
	}

	static void OnReg(object inData)
	{
		SendedRequest = false;
		if(inData is Hashtable)
		{
			var hash = (Hashtable) inData;
			var id = 0;
			if (hash.ContainsKey(KeyId))
				id = Int32.Parse(hash[KeyId].ToString());
			if (OnRegister != null)
				OnRegister(id, hash[KeyData]);
		}
		else if (OnRegister != null) 
			OnRegister(0,inData);
		
	}
	*/

		#endregion

		#region Login

		/// <summary>
		/// Start login/captcha verification with Gateway.DefaultGateway 
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <param name="captcha"></param>
		public static void Login(string login, string password, string captcha)
		{
			Login(login, password, captcha, null);
		}

		/// <summary>
		/// Start login/captcha verification with Gateway.DefaultGateway . YOu can send additional parameters for send to server
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <param name="captcha"></param>
		/// <param name="data">any your data for send to server</param>
		public static void Login(string login, string password, string captcha, Dictionary<string, object> data)
		{
            Login(login, password, captcha, data, gatewayType);
		}

		/// <summary>
		/// Start login/captcha verification. You can send additional parameters and gateway.
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <param name="captcha"></param>
		/// <param name="data"></param>
		/// <param name="gateway"></param>
		public static void Login(string login, string password, string captcha, Dictionary<string, object> data,
		                         GatewayType gateway)
		{
			SendedRequest = true;
			var ht = new Hashtable {{KeyLogin, login}, {KeyPassword, password}, {KeyCaptcha, captcha}, {KeyData, data}};
            if (_www == null)
                _www = Gateway.GetSender(gateway);

            _www.Call(ServerClass, LoginMethod, ht, OnLog);
		}

		private static void OnLog(object inData)
		{
			SendedRequest = false;
			if (inData is Hashtable)
			{
				var hash = (Hashtable) inData;
				var id = (int) hash[KeyId];
				UserId = id;
				if (!hash.ContainsKey(KeyCaptcha) && UserId > 0)
				{
					if (OnLogin != null)
						OnLogin(UserId, hash[KeyData]);
					return;
				}
				if (hash[KeyCaptcha] is Texture2D)
					_captcha = (Texture2D) hash[KeyCaptcha];
				if (OnLogin != null)
					OnLogin(UserId, hash[KeyData]);
			}
			else if (OnLogin != null)
				OnLogin(0, inData);
		}

		#endregion

		#region Logout

		/// <summary>
		/// Start logout with Gateway.DefaultGateway 
		/// </summary>
		/// <param name="userId"></param>
		public static void Logout(int userId)
		{
			Logout(userId, null);
		}

		/// <summary>
		/// Start logout with Gateway.DefaultGateway . You can set additional info for send to server.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="data"></param>
		public static void Logout(int userId, Dictionary<string, object> data)
		{
			SendedRequest = true;
			var ht = new Hashtable {{KeyId, userId}, {KeyData, data}};
            if (_www == null)
                _www = Gateway.GetSender(gatewayType);
			_www.Call(ServerClass, LogoutMethod, ht, OnOut);
		}

		/// <summary>
		/// Start logout.You can set additional info for send to server. You can set gateway
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="data"></param>
		/// <param name="gateway"></param>
		public static void Logout(int userId, Dictionary<string, object> data, GatewayType gateway)
		{
			SendedRequest = true;
			var ht = new Hashtable {{KeyId, userId}, {KeyData, data}};
            if (_www == null)
                _www = Gateway.GetSender(gateway);
            _www.Call(ServerClass, LogoutMethod, ht, OnOut);
		}

		private static void OnOut(object inData)
		{
			SendedRequest = false;
			if (inData is Hashtable)
			{
				var hash = (Hashtable) inData;
				var id = (int) hash[KeyId];
				if (OnLogout != null)
					OnLogout(id, hash[KeyData]);
			}
			else if (OnLogout != null)
				OnLogout(0, inData);
		}

		#endregion
	}
}