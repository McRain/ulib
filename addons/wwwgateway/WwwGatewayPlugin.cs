using UnityEngine;

namespace ULIB
{
    public class WwwGatewayPlugin : IGatewayPlugin
    {
        private GameObject _gameObject;

        public void Activate()
        {

        }

        public void Added()
        {

        }

        public void Remove()
        {

        }

        public string PluginType
        {
            get { return UPluginType.Gateway; }
        }

        public string Key
        {
            get { return GatewayType.Www.ToString(); }
        }

        public IRemoteObject Gateway
        {
            get
            {
                if (_gameObject != null)
                    return _gameObject.GetComponent<WwwObject>();
                _gameObject = new GameObject("WwwGateway");
                return _gameObject.AddComponent<WwwObject>();
            }
        }

        public GameObject gameObject
        {
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = new GameObject("WwwGateway");
                    _gameObject.AddComponent<WwwObject>();
                }
                return _gameObject;
            }
        }

        
    }
}
