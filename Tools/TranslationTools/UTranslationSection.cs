using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public class UTranslationSection : IEnumerable<string>
	{
		private string _name = "";
		private Dictionary<string,string> _translations = new Dictionary<string, string>();

		/// <summary>
		/// 
		/// </summary>
		public string SectionName
		{
			get
			{
				return _name;
			}

			set
			{
				if (value != string.Empty)
					_name = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyName"></param>
		public string this[string keyName]
		{
			get
			{
				if (_translations.ContainsKey(keyName))
					return _translations[keyName];

				return keyName;
			}

			set
			{
				if (!_translations.ContainsKey(keyName))
					_translations.Add(keyName, value);
				else
					_translations[keyName] = value;

			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return _translations.Count; }
		}

		/// <summary>
		/// Replaced
		/// </summary>
		/// <param name="key"></param>
		/// <param name="translation"></param>
		public void SetTranslation(string key, string translation)
		{
			if (!_translations.ContainsKey(key))
				_translations.Add(key, translation);
			else
				_translations[key] = translation;

		}

		/// <summary>
		/// Replaced
		/// </summary>
		/// <param name="translations"></param>
		public void SetTranslation(Dictionary<string,string> translations )
		{
			_translations = new Dictionary<string, string>(translations);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="translation"></param>
		/// <returns></returns>
		public bool AddTranslation(string key,string translation)
		{
			if (!_translations.ContainsKey(key))
			{
				_translations.Add(key, translation);
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		public bool ContainsKey(string keyName)
		{
			/*if(_translations==null)
				_translations = new Dictionary<string, string>();*/

			return _translations.ContainsKey(keyName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyName"></param>
		public void RemoveKey(string keyName)
		{
			if (_translations.ContainsKey(keyName))
				_translations.Remove(keyName);
		}

		/// <summary>
		/// 
		/// </summary>
		public Dictionary<string, string> Translations
		{
			get
			{
				return _translations;
			}

			set
			{
				_translations = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<string> GetEnumerator()
		{
			foreach (var key in _translations.Keys)
				yield return _translations[key];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _translations.GetEnumerator();
		}
	}
}
