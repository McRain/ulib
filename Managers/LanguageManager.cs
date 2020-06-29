using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
	//using System.Linq;
	//using UnityEngine;

	/// <summary>
	/// This class for add dynamic multilanguage support to game.<br/>
	/// Load language any time, change runtime.
	/// Provides methods for managing the list of translation, the translation of a key, etc.
	/// </summary>
	public sealed class LanguageManager:IEnumerable<UTranslationLanguage>
	{
		#region Variables

		private static LanguageManager _instance;

		private static string _currLanguage = string.Empty;
		private static string _module = string.Empty;

		private static readonly Dictionary<string, UTranslationLanguage> Cache = new Dictionary<string, UTranslationLanguage>();
		private static UTranslationSection _translation = new UTranslationSection();//actived translation with current language and current section

		/// <summary>
		/// Special tag in the language files: replaced with a call LanguageManager.GetReplacedLabel.
		/// default: %tag
		/// </summary>
		public static string ReplacedTag = "%tag";

		private static readonly List<string> _languages = new List<string>();
		private static List<string> _modules = new List<string>();

		#endregion

        #region Plugins

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPlugin"></param>
        /// <returns></returns>
        internal static bool PluginHandler(IUlibPlugin iPlugin)
        {
            return true;
        }

        internal static bool PluginRemove(IUlibPlugin iPlugin)
        {
            return true;
        }

        #endregion

        #region Events

        /// <summary>
		/// 
		/// </summary>
		/// <param name="lang"></param>
		/// <param name="mod"></param>
		public delegate void LanguageEvent(string lang,string mod);

		/// <summary>
		/// Call on changed current language and translation exists
		/// </summary>
		public static event LanguageEvent TranslationChanged;

		/// <summary>
		/// Call if translation for current language not exists
		/// </summary>
		public static event LanguageEvent QueryLanguage;

		/*/// <summary>
		/// Call if list of language change
		/// </summary>
		public static event LanguageEvent LanguagesChanged;*/

		#endregion

		#region Instances
		/// <summary>
		/// Return Instance of LanguageManager
		/// </summary>
		public static LanguageManager Instance
		{
			get { return _instance ?? (_instance = new LanguageManager()); }
		}

		/// <summary>
		/// Return Instance of LanguageManager
		/// </summary>
		public static LanguageManager GetInstance()
		{
			return _instance ?? (_instance = new LanguageManager());
		}

		#endregion

		#region Get/Set

		/// <summary>
		/// Set/Return current used language. 
		/// If translation exists - translation activated and will be called TranslationChanged. 
		/// If the translation is not available - will be called QueryLanguage.
		/// </summary>
		public static string Language
		{
			get { return _currLanguage; }
			set
			{
				if (!_currLanguage.Equals(value))
				{
					if (!_languages.Contains(value))
					{
						_languages.Add(value);
						Cache.Add(value, new UTranslationLanguage(value));
					}
					_currLanguage = value;
					Change(_currLanguage,_module);
				}
			}
		}

		/// <summary>
		/// Return list of avalaibled languages
		/// </summary>
		public static string[] Languages
		{
			get { return _languages.ToArray(); }
		}

		/// <summary>
		/// 
		/// </summary>
		public static string Module
		{
			get { return _module; }
			set
			{
				if (!_module.Equals(value))
				{
					if (!_modules.Contains(value))
					{
						_modules.Add(value);
						Cache[_currLanguage].AddSection(value);
					}
					_module = value;
					Change(_currLanguage, _module);
				}
			}
		}

		#endregion

		
		#region Methods Add

		/// <summary>
		/// Adds language with empty sections and translations to the list of available translations and cache
		/// </summary>
		/// <param name="newLanguage"></param>
		public static bool AddLanguage(string newLanguage)
		{
			if (_languages.Contains(newLanguage)) return false;
			_languages.Add(newLanguage);
			Cache.Add(newLanguage,new UTranslationLanguage(newLanguage));
			return true;
		}

		/// <summary>
		/// Adds languages with empty sections and translations to the list of available translations and cache
		/// </summary>
		/// <param name="newLanguages"></param>
		public static void AddLanguages(string[] newLanguages)
		{
			foreach (var newLanguage in newLanguages)
				AddLanguage(newLanguage);
		}

		/// <summary>
		/// Add/Replace phras in the language and section specified.<br/>
		/// If translation for this language exists and 'replaced' is true - translation will be rewritten<br/>
		/// If 'translation' not exist - pair will be added.
		/// If 'translation' exist and 'replaced' is false - pair will be not added.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="sectionName"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="replaced"></param>
		public static void AddTranslation(string language,string sectionName, string key,string value, bool replaced)
		{
			if (!Cache[language].ContainsSection(sectionName))
			{
				Cache[language].AddSection(new UTranslationSection
				                           	{
				                           		SectionName = sectionName, 
												Translations = new Dictionary<string, string>
												               	{
												               		{key,value}
												               	}
				                           	});
				_modules.Add(sectionName);
			}
			else
			{
				if (!Cache[language][sectionName].ContainsKey(key))
					Cache[language][sectionName].AddTranslation(key, value);
				else if(replaced)
					Cache[language][sectionName].SetTranslation(key,value);
			}
		}

		/// <summary>
		/// Add/Replace phrases pair in the existen language and new/existen section.<br/>
		/// If translations for this language exists and 'replaced' is true - translations will be rewritten for full section<br/>
		/// If translations not exist - pair will be added.
		/// If translations exist and 'replaced' is false - translations will be not added.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="sectionName"></param>
		/// <param name="pairs"></param>
		/// <param name="replaced"></param>
		public static void AddTranslations(string language, string sectionName, Dictionary<string,string> pairs , bool replaced)
		{
			if (!Cache[language].ContainsSection(sectionName))
			{
				Cache[language].AddSection(new UTranslationSection {SectionName = sectionName, Translations = pairs});
				_modules.Add(sectionName);
			}
			else if (replaced)
				Cache[language][sectionName] = new UTranslationSection { SectionName = sectionName, Translations = pairs };
		}

		#endregion

		#region Get
		/// <summary>
		/// Returns the translation by key or key if translation for key not exists
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetLabel(string key)
		{
			return _translation.ContainsKey(key) ? (_translation[key]).Replace("\\n", "\n") : key;
		}

		/// <summary>
		/// Returns the translation by key and section or key if translation or section for key not exists
		/// </summary>
		/// <param name="key"></param>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public static string GetLabel(string key,string sectionName)
		{
			if (Cache[_currLanguage].ContainsSection(sectionName))
				return Cache[_currLanguage][sectionName].ContainsKey(key) ? Cache[_currLanguage][sectionName][key].Replace("\\n", "\n") : key;
			return key;

			//return Cache[_currLanguage].ContainsSection(key) ? (_translation[key]).Replace("\\n", "\n") : key;
		}

		/// <summary>
		/// Returns the concat from two phrase in the current language on the two keys<br/>
		/// or return keys if translation not exists
		/// </summary>
		/// <param name="oneKey">One key for phrase</param>
		/// <param name="twoKey">Two key for phrase</param>
		/// <param name="spliter">Spliter for concat</param>
		/// <returns></returns>
		public static string GetLabels(string oneKey, string twoKey, string spliter)
		{
			return GetLabel(oneKey) + spliter + GetLabel(twoKey);
		}

		/// <summary>
		/// Returns the concat from two phrase in the current language on the two keys<br/>
		/// or return keys if translation not exists
		/// </summary>
		/// <param name="oneKey"></param>
		/// <param name="twoKey"></param>
		/// <param name="spliter"></param>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public static string GetLabels(string oneKey, string twoKey, string spliter,string sectionName)
		{
			return GetLabel(oneKey, sectionName) + spliter + GetLabel(twoKey, sectionName);
		}

		/// <summary>
		/// Returns the translation by key and replace LanguageManager.ReplacedTag in line by value of parameter "tag"<br/>
		/// or key if translation for key not exists
		/// </summary>
		/// <param name="key">key for phrase</param>
		/// <param name="tag">string for replace tag</param>
		/// <returns></returns>
		public static string GetReplacedLabel(string key, string tag)
		{
			return GetLabel(key).Replace(ReplacedTag, tag);
		}

		/// <summary>
		/// Returns the translation by key and section , and replace LanguageManager.ReplacedTag in line by value of parameter "tag"<br/>
		/// or key if translation or section for key not exists
		/// </summary>
		/// <param name="key">key for phrase</param>
		/// <param name="tag">string for replace tag</param>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public static string GetReplacedLabel(string key, string tag, string sectionName)
		{
			return GetLabel(key,sectionName).Replace(ReplacedTag, tag);
		}

		/// <summary>
		/// Returns a set of translated words , for CURRENT language and CURRENT section
		/// </summary>
		/// <returns>Set of  translated words from cache</returns>
		public static Dictionary<string, string> GetTranslations()
		{
			return GetTranslations(_module, _currLanguage);
		}

		/// <summary>
		/// Returns a set of translated words , for CURRENT language and section from parameter
		/// </summary>
		/// <param name="sectionName">Name of section</param>
		/// <returns>Set of  translated words</returns>
		public static Dictionary<string, string> GetTranslations(string sectionName)
		{
			return GetTranslations(sectionName, _currLanguage);
		}

		/// <summary>
		/// Returns a list of pairs of translations for the current language and from special sections 
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public static Dictionary<string, string> GetTranslations(string sectionName,string language)
		{
			if (_languages.Contains(language) && Cache[language].ContainsSection(sectionName))
				return new Dictionary<string, string>(Cache[language][sectionName].Translations);
			return new Dictionary<string, string>();
		}

		/// <summary>
		///  Returns a list of pairs of translations for the special language and special sections , if key containing a partKey.
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="language"></param>
		/// <param name="partKey"></param>
		/// <returns>GetKeyList('hop','menu') return all translated phrases with key 'shop','hopper', etc. from section 'menu' for language 'language'</returns>
		public static Dictionary<string, string> GetTranslations(string sectionName, string language,string partKey)
		{
			var result = new Dictionary<string, string>();
			if (_languages.Contains(language) && Cache[language].ContainsSection(sectionName))
			{
				var sectioned = Cache[_currLanguage][sectionName];
			    foreach (var str in sectioned)
			    {
			        if(str.Contains(partKey) && !result.ContainsKey(str))
			        {
			            result.Add(str,sectioned[str].Replace("\\n", "\n"));
			        }
			    }
				/*var tCount = sectioned.Count;
				for (var i = 0; i < tCount; i++)
				{

                    var key = sectioned.
					var key = sectioned.ElementAt(i);
					if (key.Contains(partKey) && !result.ContainsKey(key))
						result.Add(key, sectioned[key].Replace("\\n", "\n"));
				}*/
			}
			return result;
		}

		#endregion
        
		/// <summary>
		/// Change current language and current module  name.
		/// If language for module not exists - called QueryLanguage for load. 
		/// </summary>
		/// <param name="language"></param>
		/// <param name="sectionName"></param>
		[AccessAllow(true)]
		public static void Change(string language, string sectionName)
		{
			_currLanguage = language;
			_module = sectionName;
			if (_languages.Contains(_currLanguage) && Cache[_currLanguage].ContainsSection(_module))
			{
				_translation = Cache[_currLanguage][_module];
				if (TranslationChanged != null)
					TranslationChanged(_currLanguage, _module);
			}
			else
			{
				if (QueryLanguage != null)
					QueryLanguage(_currLanguage, _module);
			}
		}

		/*private static void SetOrQuery()
		{
			if (Cache.ContainsKey(_currLanguage) && Cache[_currLanguage].ContainsSection(_module))
			{
				_translation = Cache[_currLanguage][_module];
				if (TranslationChanged != null)
					TranslationChanged(_currLanguage, _module);
			}
			else
			{
				if (QueryLanguage != null)
					QueryLanguage(_currLanguage, _module);
			}
		}*/

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<UTranslationLanguage> GetEnumerator()
		{
			foreach (var lang in Cache.Keys)
				yield return Cache[lang];
		}

		/// <summary>
		/// Return translation for key and for current language and for current module
		/// <remarks>
		/// Use this only if you are sure that the translation for the current language of the current module and the specified key does exist.
		/// </remarks>
		/// </summary>
		/// <param name="key"></param>
		public string this[string key]
		{
			get { return Cache[_currLanguage][_module][key]; }
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	
    }
}