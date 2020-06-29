using System;

namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    public delegate void OnCompleted(object data);


    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class SerializeRequire : Attribute
    {

    }

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RemoteAllow : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public bool Allow;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		public RemoteAllow(bool val)
		{
			Allow = val;
		}
	}

	/// <summary>
	/// Enables / disables the use of the method (fields, properties) for external call via ULIB.
	/// Used on server side.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	public class AccessAllow : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public bool Allow;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		public AccessAllow(bool val)
		{
			Allow = val;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class UsernameRequest : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public bool value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		public UsernameRequest(bool val)
		{
			value = val;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class UseridRequest : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public bool value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		public UseridRequest(bool val)
		{
			value = val;
		}
	}
}
