using System.Collections.Generic;
using System.Reflection;

namespace ULIB
{
	/// <summary>
	/// Contains methods to manage options
	/// </summary>
	public sealed class OptionsManager:BaseManager
	{
		private static OptionsManager _instance;

		/*static readonly Dictionary<string, object> Targets = new Dictionary<string, object>();
		static readonly Dictionary<string, MemberInfo> Members = new Dictionary<string, MemberInfo>();*/

        /*private static readonly Dictionary<string,Configuration> Configurations = new Dictionary<string, Configuration>();
	    
        private static Configuration _currentConfiguration;

	    private static Configuration _userConfiguration;*/

		private static Dictionary<string, UValue> _defaultOptions = new Dictionary<string, UValue>();

		private static Dictionary<string, UValue> _userOptions = new Dictionary<string, UValue>();

        /*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="vm"></param>
		public delegate void OptionChange(UValue vm);
		/// <summary>
		/// 
		/// </summary>
		public static event OptionChange OnOptionsChange;*/

		#region Instance

        /*public OptionsManager()
        {
            _currentConfiguration = new Configuration("default");
        }*/

		/// <summary>
		/// Return instance of OptionsManager
		/// </summary>
		public static OptionsManager Instance
		{
			get { return _instance ?? (_instance = new OptionsManager()); }
		}

		/// <summary>
		/// Return instance of OptionsManager
		/// </summary>
		public static OptionsManager GetInstance()
		{
			return _instance ?? (_instance = new OptionsManager());
		}

		#endregion

		/*/// <summary>
		/// Add object to TargetList and members from object to MemberList. 
		/// Members of target must contains AccessAllow attribute
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
		/// Remove target and members from lists
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

        /*public void AddConfiguration(Configuration conf)
        {
            if (!Configurations.ContainsKey(conf.Key))
                Configurations.Add(conf.Key, conf);
            else
                Configurations[conf.Key] = conf;
        }

        public void AddConfigSection(string configKey, ConfigSection newSection)
        {
            if (Configurations.ContainsKey(configKey))
                Configurations[configKey].AddSection(newSection);
        }

        public void AddConfigSections(string configKey, Dictionary<string,ConfigSection> newSections)
        {
            if (!Configurations.ContainsKey(configKey)) return;
            var cfg = Configurations[configKey];
            foreach (var sec in newSections)
                cfg.AddSection(sec.Value);
        }

        public void SetConfigSections(string configKey, Dictionary<string, ConfigSection> newSections)
        {
            if (!Configurations.ContainsKey(configKey)) return;
            Configurations[configKey].SetSections(newSections);
        }

        public void AddParameter(string configKey, string sectionName,UValue parameter)
        {
            if (Configurations.ContainsKey(configKey) && Configurations[configKey].ContainsSection(sectionName))
                Configurations[configKey][sectionName].AddParameter(parameter.key, parameter);
        }

        public void RemoveParameter(string configKey, string sectionName, string parameterKey)
        {
            if (Configurations.ContainsKey(configKey) && Configurations[configKey].ContainsSection(sectionName))
                Configurations[configKey][sectionName].RemoveParameter(parameterKey);
        }

        public void RemoveConfiguration(string key)
        {
            if (Configurations.ContainsKey(key))
                Configurations.Remove(key);
        }

        public void RemoveConfigSection(string configKey, string sectionName)
        {
            if (Configurations.ContainsKey(configKey))
                Configurations[configKey].RemoveSection(sectionName);
        }*/

        

	    /// <summary>
		/// 
		/// </summary>
		/// <param name="targetName"></param>
		/// <returns></returns>
		public static bool TargetExists(string targetName)
		{
			return Targets.ContainsKey(targetName);
		}

		/*static void FillMembers(string targetName, object target)
		{
			var tp = target.GetType();
			var members = tp.GetMembers();
			foreach (var member in members.Where(
						member => member is FieldInfo || (member is PropertyInfo && ((PropertyInfo)member).CanWrite && ((PropertyInfo)member).CanRead)))
				Members.Add(targetName + "." + member.Name, member);
			/*foreach (var member in members.Where(member => member.GetCustomAttributes(typeof(AccessAllow), false).Length > 0 &&
												((AccessAllow)member.GetCustomAttributes(typeof(AccessAllow), false)[0]).Allow))
				Members.Add(targetName + "." + member.Name, member);*/
		/*}

		static void RemoveMembers(string targetName)
		{
			var removedMembers = (from member in Members
								  where member.Key == targetName + "." + member.Value.Name
								  select targetName + "." + member.Value.Name).ToList();
			lock (Members)
				foreach (var removedMember in removedMembers)
					Members.Remove(removedMember);
		}*/

		/*#region SaveLoad

		public static void LoadDefaultsOptions()
		{
			_defaultOptions = new Dictionary<string, UValue>();
			LoadOptions("default", OnLoadDefaultOption);
		}

		public static void LoadUserOptions()
		{
			_userOptions = new Dictionary<string, UValue>();
			LoadOptions("user", OnLoadUserOption);
		}

		public static void LoadOptions(string nameOp, OnCompleted func)
		{
			Gateway.GetSender().Call(_instance, "load", nameOp, func);
		}

		static void OnLoadDefaultOption(object inData)
		{
			if (inData is Hashtable)
			{
				var ht = (Hashtable)inData;
				if (ht.ContainsKey("list") && ht["list"] != null)
					_defaultOptions = (Dictionary<string, UValue>)ht["list"];
				//ApplyOptions();
				//LoadOptions("user", OnLoadUserOption);
			}
		}

		static void OnLoadUserOption(object inData)
		{
			if (inData is Hashtable)
			{
				var ht = (Hashtable)inData;
				if (ht.ContainsKey("list") && ht["list"] != null)
					_userOptions = (Dictionary<string, UValue>)ht["list"];
				//ApplyOptions();
			}
		}

		#endregion*/

		#region GetSetOptions

		/// <summary>
		/// 
		/// </summary>
		public static Dictionary<string, UValue> DefaultOptions
		{
			get { return _defaultOptions; }
			set { _defaultOptions = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public static Dictionary<string, UValue> UserOptions
		{
			get { return _userOptions; }
			set { _userOptions= value; }
		}

		/// <summary>
		/// Return from user options or (if user options not contains) from default
		/// </summary>
		/// <param name="valueLabel"></param>
		/// <returns></returns>
		public static UValue GetOption(string valueLabel)
		{
			return _userOptions.ContainsKey(valueLabel) ? _userOptions[valueLabel] : _defaultOptions[valueLabel];
		}

		/// <summary>
		/// Return user options and (if user options not contains ) default options
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string,UValue> GetOptions()
		{
		    var result = new Dictionary<string, UValue>();
		    foreach (var option in _userOptions)
		        result.Add(option.Key, option.Value);
		    foreach (var defaultOption in _defaultOptions)
		        if (!result.ContainsKey(defaultOption.Key)) result.Add(defaultOption.Key, defaultOption.Value);
		    return result;
		}
		
		/// <summary>
		/// Return changed user options or null
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, UValue> GetChangedOptions()
		{
		    var dictionary = new Dictionary<string, UValue>();
		    foreach (var so in _userOptions)
		    {
		        if (so.Value.changed) dictionary.Add(so.Key, so.Value);
		    }
		    return dictionary;
		}

	    /// <summary>
		/// Return user options or default if not exists
		/// </summary>
		/// <param name="valueLabel"></param>
		/// <returns></returns>
		public static object GetOptionsValue(string valueLabel)
		{
			return _userOptions.ContainsKey(valueLabel) ? _userOptions[valueLabel].value : _defaultOptions[valueLabel].value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="label"></param>
		/// <param name="val"></param>
		public static void SetOptionsValue(string label, object val)
		{
			if (!_userOptions.ContainsKey(label))
			{
				var so = (UValue)_defaultOptions[label].GetCopy();
				_userOptions.Add(label, so);
			}
			_userOptions[label].value = val;
			if (!_userOptions[label].value.Equals(_defaultOptions[label].value))
				_userOptions[label].changed = true;
			/*if (OnOptionsChange != null)
				OnOptionsChange(_userOptions[label]);*/
		}
		
		#endregion

		/// <summary>
		/// Reset changed flag (set no changed ) for all user options
		/// </summary>
		public static void ResetChanged()
		{
			foreach (var userOption in _userOptions)
			{
				userOption.Value.changed = false;
			}
		}
		
		/*public static void LoadDefaultOptions()
		{
			defaultOptions = new Dictionary<string, UValue>();
			LoadOptions("default", OnLoadDefaultOption);
		}

		static void LoadOptions(string nameOp, OnCompleted func)
		{
			Gateway.GetSender().Call(_instance, "load", nameOp, func);
		}

		static void OnLoadDefaultOption(object inData)
		{
			if (inData is Hashtable)
			{
				var ht = (Hashtable)inData;
				if (ht.ContainsKey("list") && ht["list"] != null)
					defaultOptions = (Dictionary<string, UValue>)ht["list"];
				//LoadOptions("user", OnLoadUserOption);
			}
		}

		static void OnLoadUserOption(object inData)
		{
			if (inData is Hashtable)
			{
				var ht = (Hashtable)inData;
				if (ht.ContainsKey("list") && ht["list"] != null)
					_userOptions = (Dictionary<string, UValue>)ht["list"];
				SetOptions();
			}
		}*/

		/*public static void SaveOptions()
		{
			Dictionary<string, UValue> savedOptions = null;
			if (_userOptions.Count > 0)
			{
				savedOptions = new Dictionary<string, UValue>();
				foreach (var so in _userOptions)
				{
					if (so.Value.changed)
						savedOptions.Add(so.Key, so.Value);
				}
			}
			var param = new Hashtable();
			param["name"] = GameManager.Player.options.currentQuality;
			param["list"] = savedOptions;
			Gateway.GetSender().Call(_instance, "save", param, OnSaveOptions);

		}

		static void OnSaveOptions(object inData)
		{
			if (inData is string && inData.ToString().Equals(GameManager.Player.options.currentQuality))
				GameManager.ShowMessage(LanguageManager.GetLabel("saveDataOK"));
			else
				GameManager.ShowMessage(LanguageManager.GetLabel("saveDataFail"));
		}*/

		/*public static Dictionary<string, ScreenOptions> GetOptions(string qName)
		{
			return screens[qName];
		}*/
		
		#region Apply

		/*public static void SetOption(string valueLabel, ScreenOptions newSo)
		{
			if (!userQuality.ContainsKey(valueLabel))
				userQuality.Add(valueLabel, newSo);
			userQuality[valueLabel] = newSo;
		}*/

		/// <summary>
		/// Apply default options and user options
		/// </summary>
		public static void ApplyOptions()
		{
			ApplyOptions(_defaultOptions);
			ApplyOptions(_userOptions);
		}

		/// <summary>
		/// Apply one options
		/// </summary>
		/// <param name="optionValue"></param>
		static void ApplyOptions(UValue optionValue)
		{
			if (Targets.ContainsKey(optionValue.target))
			{
				var targetObject = Targets[optionValue.target];

				if (Members.ContainsKey(optionValue.target+"."+optionValue.member))
				{
					var targetMember = Members[optionValue.target + "." + optionValue.member];
					if (targetMember is FieldInfo)
						((FieldInfo)targetMember).SetValue(targetObject, optionValue.value);
					else if (targetMember is PropertyInfo)
						((PropertyInfo)targetMember).SetValue(targetObject, optionValue.value, null);
					/*else
					{
						try
						{
							/*if (evt.IsPrevios && evt.parameters.Length > evt.parameters.Length - 1)//???? >0 ???
							evt.parameters[evt.parameters.Length - 1] = previosResult;*/
							/*((MethodInfo)targetMember).Invoke(targetObject, optionValue.parameters);
						}
						catch (Exception e)
						{
							ULog.Log("OptionsManager:ApplyOptions error on "+optionValue.member+"  \n" + e.Message, ULogType.Error);
						}
					}*/
				}
				else
				{
					ULog.Log("ApplyOptions -2 " + optionValue.member+" : "+Members.Count);
				}
			}
		}

		/// <summary>
		/// Apply options
		/// This set default if user options not contains inOptions item
		/// </summary>
		/// <param name="inOptions"></param>
		static void ApplyOptions(Dictionary<string, UValue> inOptions)
		{
		    foreach (var so in inOptions)
		    {
		        if (inOptions == _userOptions || !_userOptions.ContainsKey(so.Key)) ApplyOptions(so.Value);
		    }
		}

	    #endregion
	}
}
