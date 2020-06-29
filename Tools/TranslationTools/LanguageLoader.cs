using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
	
	/// <summary>
	/// 
	/// </summary>
	public class LanguageLoader
	{
		/// <summary>
		/// code/Name pair of translations
		/// </summary>
		public static Dictionary<string, string> languageList = new Dictionary<string, string>();
		
		private static string _translationsExtensions = ".txt";
		/// <summary>
		/// Default folder for load translations
		/// </summary>
		public static string languagesFolder = "";
		/// <summary>
		/// Loaded specific section from translation file (if used text file in *.ini format)
		/// </summary>
		public static string moduleSection = "";

		private static bool _useLocalFiles = true;
		private static string _serverClass = "LanguageManager";
		private static string _listMethod = "LoadList";
		private static string _translationMethod = "LoadTranslation";
		private static string _textureMethod = "LoadTexture";
		private static bool _translationLoaded;


		/// <summary>
		/// 
		/// </summary>
		public static bool UseLocal
		{
			get { return _useLocalFiles; }
		}

		/// <summary>
		/// 
		/// </summary>
		public static string TextureLoadMethod
		{
			get { return _textureMethod; }
		}

		/// <summary>
		/// 
		/// </summary>
		public static string ServerClass
		{
			get { return _serverClass; }
		}

		/// <summary>
		/// Extensions of translations file 
		/// </summary>
		public static string TranslationsExtensions
		{
			get { return _translationsExtensions; }
			set { _translationsExtensions = "." + value.Replace(".", ""); }
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool TranslationLoaded
		{
			get { return _translationLoaded; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetLanguageLabel(string key)
		{
			return languageList.ContainsKey(key) ? languageList[key] : key;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="localLoad"></param>
		public static void Init(bool localLoad)
		{
			if (localLoad)
				Init(ResourceManager.ResourcePaths[ResourceSource.Translations]);
			else
				Init(_serverClass, _listMethod, _translationMethod, _textureMethod);
		}

		private static void Init(string serverClass, string listMethod, string translationMethod, string textureMethod)
		{
			_serverClass = serverClass;
			_listMethod = listMethod;
			_translationMethod = translationMethod;
			_textureMethod = textureMethod;
			LanguageManager.QueryLanguage += EventOnQuery;
			_useLocalFiles = false;
		}

		private static void Init(string langFolder)
		{
			languagesFolder = langFolder;
			LanguageManager.QueryLanguage += EventOnQuery;
			_useLocalFiles = true;
		}
		
		/// <summary>
		/// Load list of languages , store pair to languageList
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="parameter">parameter for send to server class, if you used not LanguageManager on server side.
		/// Default:null</param>
		public static void LoadList(string fileName, object parameter)
		{
			if (_useLocalFiles && !Application.isWebPlayer)
			{
                /*((FileObject)Gateway.GetSender()).Call(null, ResourceManager.BasePath + "/" + languagesFolder + "/" + fileName,
                                                        ResourceSource.Text, OnLoadListLocal, false);*/
			    var data = FileManager.LoadText(ResourceManager.BasePath + "/" + languagesFolder + "/" + fileName);
                if(!string.IsNullOrEmpty(data))
                    OnLoadListLocal(data);
                else
                    ULog.Log(ResourceManager.BasePath + "/" + languagesFolder + "/" + fileName+" is empty or not exist.",ULogType.Warning);
			}

				
			else
			{
				//Debug.Log("LoadList remote ");
				Gateway.GetSender().Call(_serverClass, _listMethod, parameter, OnLoadListRemote);
			}
		}

		private static void OnLoadListRemote(object inData)
		{
			languageList = (Dictionary<string, string>)inData;
            if(Gateway.Debug)
                ULog.Log(string.Format("OnLoadListRemote {0} : count {1}", languageList,languageList.Count));
            
		    var langListCount = languageList.Count;
            var arr = new string[langListCount];
		    var i = 0;
		    foreach (var keyVal in languageList)
		    {
		        arr[i] = keyVal.Key;
		        i++;
		    }
            LanguageManager.AddLanguages(arr);
           // LanguageManager.AddLanguages(languageList.Keys.ToArray());
			if(LanguageManager.Languages.Length>0)
				LanguageManager.Change(LanguageManager.Languages[0], moduleSection); 
		}

		private static void OnLoadListLocal(object inData)
		{
			//Debug.Log(string.Format("OnLoadListLocal {0}", inData));
			OnLoadListRemote(FileManager.ParseIni(inData.ToString(), null));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryLanguage"></param>
		/// <param name="section"></param>
		public static void LoadTranslation(string queryLanguage, string section)
		{
			LanguageManager.AddLanguage(queryLanguage);
			EventOnQuery(queryLanguage,section);
		}

		private static void EventOnQuery(string queryLanguage, string section)
		{
			//Debug.Log(string.Format("EventOnQuery {0}:{1}", queryLanguage,section));
			var table = new Hashtable { { "lang", queryLanguage }, { "section", section } };
			if (_useLocalFiles && !Application.isWebPlayer)
			{
				/*((FileObject)Gateway.GetSender()).Call(
					table, 
					ResourceManager.BasePath + "/" + languagesFolder + "/" + queryLanguage + _translationsExtensions,
					ResourceSource.Text,
					OnGetTranslationRemote, 
					false);*/

				//table.Add("list", FileManager.LoadIni(languagesFolder + queryLanguage + _translationsExtensions, section));
				//OnGetTranslation(table);
			}
			else
				Gateway.GetSender().Call(_serverClass, _translationMethod, table, OnGetTranslationRemote);
		}



		private static void OnGetTranslationRemote(object inData)
		{
			//Debug.Log(string.Format("OnGetTranslationRemote"));
			if (inData is Hashtable) //Validates load
			{
				var result = (Hashtable)inData;
				var translation = new Dictionary<string, string>();

				if (result["result"] is string)
					translation = FileManager.ParseIni(result["result"].ToString(), result["section"].ToString());
				else if (result["result"] is Dictionary<string, string>)
				{
					//Debug.Log(string.Format("OnGetTranslationRemote result is "));
					translation = (Dictionary<string, string>) result["result"];
					//Debug.Log("OnGetTranslation " + translation.Count + " for " + result["lang"].ToString() + " : " + result["section"].ToString());
					/*foreach (var tr in translation)
						Debug.Log(tr.Key + ":" + tr.Value);*/
				}
					/*else if (result["list"] is Dictionary<string, Dictionary<string, object>>)
				{
					var translations = (Dictionary<string, Dictionary<string, object>>)result["list"];
					foreach (var translate in translations)
					{
						var dict = translate.Value.ToDictionary(trans => trans.Key, trans => trans.Value.ToString());
						LanguageManager.AddTranslations(result["lang"].ToString(), translate.Key, dict, true);
					}
					LanguageManager.Change(result["lang"].ToString(), result["section"].ToString());
					_translationLoaded = true;
				}*/
				LanguageManager.AddTranslations(result["lang"].ToString(), result["section"].ToString(), translation, true);
				LanguageManager.Change(result["lang"].ToString(), result["section"].ToString());
				_translationLoaded = true;
					
				
			}
			else
			{
			    if(inData!=null)
			    {
                    ULog.Log("LanguageLoader:Error on OnGetTranslation : Type "+inData.GetType()+" : as string  - "+inData, ULogType.Error);
			    }else
			    {
                    ULog.Log("LanguageLoader:Error on OnGetTranslation : NULL", ULogType.Error);
			    }
			}
				
		}


	}
}
