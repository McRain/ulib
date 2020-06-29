using System.Collections.Generic;
using System.Reflection;

namespace ULIB
{
	/// <summary>
	/// Contains methods for managing quality settings
	/// </summary>
	public sealed class QualityManager:BaseManager
	{
		private static QualityManager _instance;

		private static string[] _qualityNames; 

		/*static readonly Dictionary<string, object> Targets = new Dictionary<string, object>();
		static readonly Dictionary<string, MemberInfo> Members = new Dictionary<string, MemberInfo>();*/

		static readonly Dictionary<string,Dictionary<string ,UValue>> GameQuality = new Dictionary<string, Dictionary<string,UValue>>();
		static readonly Dictionary<string, UValue> UserQuality = new Dictionary<string, UValue>();

        static readonly Dictionary<string,IQualityPlugin> Plugins = new Dictionary<string, IQualityPlugin>();

		static int _currentQualityKey;

		//public delegate void QualityChange(UValue so);
		//public static event QualityChange OnQualityChange;

		/// <summary>
		/// 
		/// </summary>
		public static QualityManager Instance
		{
			get { return _instance ?? (_instance = new QualityManager()); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static QualityManager GetInctance()
		{
			return _instance ?? (_instance = new QualityManager());
		}

		#region TargetMembers

		/*/// <summary>
		/// 
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="newTarget"></param>
		public static void AddTarget(string targetName, object newTarget)
		{
			if (!Targets.ContainsKey(targetName))
				Targets.Add(targetName, newTarget);
			else
			{
				RemoveMembers(targetName);
				Targets[targetName] = newTarget;
			}
			FillMembers(targetName, newTarget);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetName"></param>
		public static void RemoveTarget(string targetName)
		{
			if (Targets.ContainsKey(targetName))
			{
				RemoveMembers(targetName);
				Targets.Remove(targetName);
			}

		}*/

		/*private static void FillMembers(string targetName, object target)
		{
			var tp = target.GetType();
			var members = tp.GetMembers(BindingFlags.DeclaredOnly|BindingFlags.GetField|BindingFlags.GetProperty);
			/*foreach (var member in members.Where(
						member =>member is FieldInfo || (member is PropertyInfo && ((PropertyInfo) member).CanWrite && ((PropertyInfo) member).CanRead)))
				Members.Add(targetName + "." + member.Name, member);*/
		    /*foreach (var member in members)
		        if(member is FieldInfo || (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead))
                    Members.Add(targetName+"."+member.Name,member);
		}

		private static void RemoveMembers(string targetName)
		{
			var removedMembers = (from member in Members
								  where member.Key == targetName + "." + member.Value.Name
								  select targetName + "." + member.Value.Name).ToList();
			lock (Members)
				foreach (var removedMember in removedMembers)
					Members.Remove(removedMember);
		}*/

		#endregion

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
                var ioface = (IQualityPlugin)iPlugin;
                

                /*var gatewayKey = ioface.Key;
                var gatewayObject = ioface.Gateway;
                if (gatewayObject != null)
                {
                    gatewayObject.Host = Host;
                    gatewayObject.Path = Path;
                    gatewayObject.File = File;
                    //ULog.Log("PluginHandler " + gatewayKey);
                    if (!_gateways.ContainsKey(gatewayKey))
                        _gateways.Add(gatewayKey, gatewayObject);
                    else
                        throw new GatewayException(string.Format("A duplicate key : {0}.", gatewayKey));
                }
                else
                    throw new GatewayException(string.Format("Gateway is null in plugin."));*/
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
            var ioface = tp.GetInterface("IQualityPlugin");
            if (ioface != null)
            {
                /*var gatewayKey = tp.GetProperty("Key");
                if (_gateways.ContainsKey(gatewayKey.ToString()))
                    _gateways.Remove(gatewayKey.ToString());*/
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool PluginExists(object key)
        {
            //return Loaders.ContainsKey(key.ToString());
            return false;
        }

        #endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="names"></param>
		public static void SetQualityNames(string[] names)
		{
			_qualityNames = names;
		}

		/// <summary>
		/// 
		/// </summary>
		public static void SetNextQuality()
		{
			_currentQualityKey++;
			if (_currentQualityKey >= _qualityNames.Length)
				_currentQualityKey = 0;
			SetQuality(_qualityNames[_currentQualityKey]);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void SetPreviosQuality()
		{
			_currentQualityKey--;
			if (_currentQualityKey < 0)
				_currentQualityKey = _qualityNames.Length - 1;
			SetQuality(_qualityNames[_currentQualityKey]);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void ClearUserQuality()
		{
			UserQuality.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lsoName"></param>
		public static void SetQuality(string lsoName)
		{
			ApplyQuality(GameQuality[lsoName]);
			ApplyQuality(UserQuality);
		}

		static void ApplyQuality(UValue uValue)
		{
			if(Targets.ContainsKey(uValue.target))
			{
				if(Members.ContainsKey(uValue.member))
				{
					var qTarget = Targets[uValue.target];
					var targetMember = Members[uValue.target + "." + uValue.member];
					if (targetMember is FieldInfo)
						((FieldInfo)targetMember).SetValue(qTarget, uValue.value);
					else if (targetMember is PropertyInfo)
						((PropertyInfo)targetMember).SetValue(qTarget, uValue.value, null);
				}
			}
		}

		static void ApplyQuality(Dictionary<string, UValue> qset)
		{
		    foreach (var so in qset)
		    {
		        if (qset == UserQuality || !UserQuality.ContainsKey(so.Key))
		        {
		            ApplyQuality(so.Value);
		        }
		    }
		}

	    /// <summary>
		/// 
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		public static bool TargetExists(string targetName)
		{
			return Targets.ContainsKey(targetName);
		}

	}
}
