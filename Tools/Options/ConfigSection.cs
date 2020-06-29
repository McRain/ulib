using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
    /// <summary>
    /// Class for module configurations
    /// </summary>
    public class ConfigSection : IEnumerable<UValue>
    {
        private string _name = "";
        private Dictionary<string, UValue> _parameters = new Dictionary<string, UValue>();

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

        public UValue this[string keyName]
        {
            get
            {
                return _parameters.ContainsKey(keyName) ? _parameters[keyName] : null;
            }

            set
            {
                if (!_parameters.ContainsKey(keyName))
                    _parameters.Add(keyName, value);
                else
                    _parameters[keyName] = value;

            }
        }

        public int Count
        {
            get { return _parameters.Count; }
        }

        public void SetParameter(string key, UValue parameter)
        {
            if (!_parameters.ContainsKey(key))
                _parameters.Add(key, parameter);
            else
                _parameters[key] = parameter;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddParameter(string key, UValue value)
        {
            if (!_parameters.ContainsKey(key))
            {
                _parameters.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        public void RemoveParameter(string keyName)
        {
            if (_parameters.ContainsKey(keyName))
                _parameters.Remove(keyName);
        }


        /// <summary>
        /// Replaced
        /// </summary>
        /// <param name="parameters"></param>
        public void SetParameters(Dictionary<string, UValue> parameters)
        {
            _parameters = new Dictionary<string, UValue>(parameters);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public bool ContainsKey(string keyName)
        {
            return _parameters.ContainsKey(keyName);
        }

   
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, UValue> Parameters
        {
            get
            {
                return _parameters;
            }

            set
            {
                _parameters = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<UValue> GetEnumerator()
        {
            foreach (var key in _parameters.Keys)
                yield return _parameters[key];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}