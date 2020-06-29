using System.Collections;

namespace ULIB
{
	/// <summary>
	/// Not used in current version
	/// </summary>
	public sealed class DatabaseManager
	{
		private static DatabaseManager _instance;

		/// <summary>
		/// 
		/// </summary>
		public static string keySql = "sql";
		/// <summary>
		/// 
		/// </summary>
		public static string keyBind = "bindparam";
		/// <summary>
		/// 
		/// </summary>
		public static string keyResultClass = "resultclass";

		#region Instance

		/// <summary>
		/// Return instance of DatabaseManager
		/// </summary>
		public static DatabaseManager Instance
		{
			get { return _instance ?? (_instance = new DatabaseManager()); }
		}

		/// <summary>
		/// Return instance of DatabaseManager
		/// </summary>
		public static DatabaseManager GetInstance()
		{
			return _instance ?? (_instance = new DatabaseManager());
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="bindType"></param>
		/// <param name="bindparam"></param>
		/// <param name="resultClass"></param>
		/// <param name="callback"></param>
        public static void Call(string procedureName, string bindType, object[] bindparam, string resultClass, RemoteCallback callback)
		{
			var obj = new object[bindparam.Length+1];
			obj[0] = bindType;
			for (var i = 1; i <= bindparam.Length; i++)
				obj[i] = bindparam[i - 1];
			Gateway.GetSender().Call(Instance, "Call", new Hashtable
			                                                   	{
			                                                   		{keySql,procedureName},
																	{keyBind,obj},
																	{keyResultClass,resultClass}
			                                                   	}, callback);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="procedureName"></param>
		/// <param name="bindType"></param>
		/// <param name="resultClass"></param>
		/// <param name="callback"></param>
		/// <param name="bindparam"></param>
        public static void Call(string procedureName, string bindType, string resultClass, RemoteCallback callback, params object[] bindparam)
		{
			var obj = new object[bindparam.Length + 1];
			obj[0] = bindType;
			for (var i = 1; i <= bindparam.Length; i++)
				obj[i] = bindparam[i - 1];
			Gateway.GetSender().Call(Instance, "Call", new Hashtable
			                                                   	{
			                                                   		{keySql,procedureName},
																	{keyBind,obj},
																	{keyResultClass,resultClass}
			                                                   	}, callback);
		}

	}
}
