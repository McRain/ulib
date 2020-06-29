using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public class UTranslationLanguage : IEnumerable<UTranslationSection>
	{
		private readonly string _key = "";
		private readonly string _label = "";
		private readonly Dictionary<string,UTranslationSection> _sections = new Dictionary<string, UTranslationSection>();

		/// <summary>
		/// 
		/// </summary>
		public UTranslationLanguage(string key)
		{
			_key = key;
			_label = key;
		}

		/// <summary>
		/// 
		/// </summary>
		public UTranslationLanguage(string key,string label)
		{
			_key = key;
			_label = label;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Label
		{
			get
			{
				return _label;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return _sections.Count; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sectionName"></param>
		internal UTranslationSection this[string sectionName]
		{
			get { return _sections.ContainsKey(sectionName) ? _sections[sectionName] : null; }
			set
			{
				if (!_sections.ContainsKey(sectionName))
					_sections.Add(sectionName, value);
				else
					_sections[sectionName] = value;

			}
		}

		/// <summary>
		/// Not replaced
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		public bool AddSection(string keyName)
		{
			if (!ContainsSection(keyName))
			{
				_sections.Add(keyName, new UTranslationSection {SectionName = keyName});
				return true;
			}
			return false;
		}

		/// <summary>
		/// Not replaced
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		internal bool AddSection(UTranslationSection section)
		{
			if (!ContainsSection(section.SectionName))
			{
				_sections.Add(section.SectionName, section);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Replaced
		/// </summary>
		/// <param name="sectionName"></param>
		/// <param name="translations"></param>
		/// <returns></returns>
		public void SetSection(string sectionName,Dictionary<string,string> translations )
		{
			if (!ContainsSection(sectionName))
				_sections.Add(sectionName, new UTranslationSection { SectionName = sectionName, Translations = translations });
			else
				_sections[sectionName].SetTranslation(translations);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		public bool ContainsSection(string keyName)
		{
			return _sections.ContainsKey(keyName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<UTranslationSection> GetEnumerator()
		{
			foreach (var sectionName in _sections.Keys)
				yield return _sections[sectionName];
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
