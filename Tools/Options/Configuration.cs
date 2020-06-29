using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
    /// <summary>
    /// Class for all configuration for application (ConfigSection - config for module)
    /// </summary>
    public class Configuration : IEnumerable<ConfigSection>
    {
        private readonly string _key = "";
        private string _label = "";
        private Dictionary<string, ConfigSection> _sections = new Dictionary<string, ConfigSection>();

        /// <summary>
        /// 
        /// </summary>
        public Configuration(string key)
        {
            _key = key;
            _label = key;
        }

        /// <summary>
        /// 
        /// </summary>
        public Configuration(string key, string label)
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
            set { _label = value; }
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
        internal ConfigSection this[string sectionName]
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
                _sections.Add(keyName, new ConfigSection { SectionName = keyName });
                return true;
            }
            return false;
        }

        /// <summary>
        /// Not replaced
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        internal bool AddSection(ConfigSection section)
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
        /// <param name="parameters"></param>
        /// <returns></returns>
        public void SetSection(string sectionName, Dictionary<string, UValue> parameters)
        {
            if (!ContainsSection(sectionName))
                _sections.Add(sectionName, new ConfigSection { SectionName = sectionName, Parameters = parameters });
            else
                _sections[sectionName].SetParameters(parameters);
        }

        public void SetSections(Dictionary<string,ConfigSection> newsections )
        {
            _sections = newsections;
        }

        public void RemoveSection(string sectionName)
        {
            if (_sections.ContainsKey(sectionName))
                _sections.Remove(sectionName);
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
        public IEnumerator<ConfigSection> GetEnumerator()
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