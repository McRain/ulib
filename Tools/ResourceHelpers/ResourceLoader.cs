using System;
using System.Collections;
using UnityEngine;

namespace ULIB
{
	internal class ResourceLoader:MonoBehaviour
	{
		private ResourceManager.OnGetResource _callback;
		private string _key = "";
		private bool _decode = true;
		//private ResourceSource _type;
	    private ResourceType _resourceType;
		private Hashtable _getTable;
	    private IRemoteObject _loader;

		/*/// <summary>
		/// 
		/// </summary>
		/// <param name="path">langiage,icons,sound,music...</param>
		/// <param name="key">name of file</param>
		/// <param name="ext">ogg,png,jpg...</param>
		/// <param name="resType">movie,audioclip,texture... for www</param>
		/// <param name="callback">callback</param>
		internal void LoadResource(string path, string key, string ext, ResourceSource resType, ResourceManager.OnGetResource callback)
		{
			
		}

		internal void LoadResource(string path, string key, string ext, ResourceSource resType, ResourceManager.OnGetResource callback,
			bool decode)
		{
			//LoadResource(path, key, ext, resType, callback, decode,null, null);

			//Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
			/*_callback = callback;
			_key = key;
			_decode = decode;
			_type = resType;
			StartCoroutine(
					WaitLoadLocalResource(
							"file://" + path.Replace("http://", "").Replace("https://", "").Replace(":", "") + "/" + _key +  ext,
							resType, null, null));
		}

		internal void LoadResource(string protocol, string path, string key, string ext, ResourceSource resType, ResourceManager.OnGetResource callback,
			bool decode,Hashtable post, Hashtable gets)
		{
			//Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
			_callback = callback;
			_key = key;
			_decode = decode;
			_type = resType;
			_getTable = gets;
			StartCoroutine(
					WaitLoadResource(protocol+"/"+path + "/" + _key + ext,
											resType, post));
		}*/

		/*internal void LoadFromUrl(string url, string key, ResourceSource resType, ResourceManager.OnGetResource callback,
			bool decode, Hashtable post, Hashtable gets)
		{
			//Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
			//Debug.Log("LoadFromUrl " + url+" : " + key);
			_callback = callback;
			_key = key;
			_decode = decode;
			_type = resType;
			_getTable = gets;
			StartCoroutine(WaitLoadResource(url,resType, post));
		}*/

        internal void LoadFromFile(string path, string key, string ext, ResourceType resPath, ResourceManager.OnGetResource callback, bool decode,ResourceType resourceType)
        {
            if(Gateway.Debug)
            {
                /*Debug.Log("LoadFromFile " + path + "/" + key + "." + ext);
                Debug.Log("Resource path : "+Enum.GetName(typeof(ResourceType), resPath));
                Debug.Log("Resource type : " + Enum.GetName(typeof(ResourceType), resourceType));*/
            }
            
            _callback = callback;
            _key = key;
            _decode = decode;
            _resourceType = resourceType;
            StartCoroutine(WaitLoadResources(string.Format("{0}{1}/{2}.{3}", ResourceProtocol.File, path, _key, ext),
                                            resourceType, null));
        }

        [Obsolete("Use other LoadFromFile")]
		internal void LoadFromFile(string path, string key, string ext, ResourceSource resType, ResourceManager.OnGetResource callback,
			bool decode)
		{
			//Debug.Log("LoadFromFile " + path + " + " + key + " + " + ext);
			//Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
			_callback = callback;
			_key = key;
			_decode = decode;
			StartCoroutine(WaitLoadResource(string.Format("{0}{1}/{2}.{3}", ResourceProtocol.File, path, _key, ext),
                                            resType, null));
		}

        [Obsolete("Use LoadFromGateway")]
		internal void LoadFromGateway(ResourceSource path,string key, string extension, ResourceSource resType, ResourceManager.OnGetResource callback)
		{
            if (Gateway.Debug)
                ULog.Log(string.Format("LoadFromGateway ResourceSource: {0}  path: {1} , key {2} , ext: {3}", Enum.GetName(typeof(ResourceSource), resType), path, key, extension));
			//Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
			_callback = callback;
			_key = key;
			_decode = true;
            if (_loader==null)
                _loader = Gateway.GetSender();
            _loader.Call(
                    ResourceManager.serverClassName, ResourceManager.resourceLoadMethod,
					new Hashtable{
							{"path",path},
							{"key",_key},
							{"type",resType},
							{"ext",extension}
						}, OnGetResources);
		}


        internal void LoadFromGateway(ResourceType path, string key, string extension, ResourceType resType, ResourceManager.OnGetResource callback)
        {
            if (Gateway.Debug)
                ULog.Log(string.Format("LoadFromGateway ResourceSource: {0}  path: {1} , key {2} , ext: {3}", Enum.GetName(typeof(ResourceSource), resType), path, key, extension));
            //Debug.Log(Enum.GetName(typeof (ResourceSource), resType));
            _callback = callback;
            _key = key;
            _decode = true;
            _resourceType = resType;
            if (_loader == null)
                _loader = Gateway.GetSender();
            _loader.Call(
                    ResourceManager.serverClassName, ResourceManager.resourceLoadMethod,
                    new Hashtable{
							{"path",path},
							{"key",_key},
							{"type",resType},
							{"ext",extension}
						}, OnGetResources);
        }

        /// <summary>
        /// Result may be:
        /// - from gateway: back hashtable with resource in ['result'], back hashtable with ur in ['result'] - use direct download
        /// - from file: hashtable with resource in ['result']
        /// - from direct download: hashtable with resource in ['result']
        /// </summary>
        /// <param name="file"></param>
        /// <param name="resType"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        IEnumerator WaitLoadResources(string file, ResourceType resType, Hashtable post)
        {
            if(Gateway.Debug)
                ULog.Log("WaitLoadResource: " + file + " " + Enum.GetName(typeof(ResourceType), resType));

            WWW loader;
            if (_getTable != null && _getTable.Count > 0)
            {
                file += "?";
                foreach (DictionaryEntry entry in _getTable)
                    file = file + (entry.Key + "=" + entry.Value + "&");
                file = file.Remove(file.Length - 2, 1);
            }
            if (post != null && post.Count > 0)
            {
                var form = new WWWForm();
                foreach (DictionaryEntry entry in post)
                    form.AddField(entry.Key.ToString(), entry.Value.ToString());
                if (Gateway.Key != String.Empty)
                    form.AddField(Gateway.KeyName, Gateway.Key);
                loader = new WWW(file, form);
            }
            else
            {
                //Debug.Log(file);
                loader = new WWW(file);
            }

            yield return loader;
            /*while (!loader.isDone)
                yield return loader;*/
            if (loader.error != null)
            {
                ULog.Log("ResourceLoader.WaitLoadResources : " + loader.error);
                Destroy(gameObject);
            }
            else
            {
                if(Gateway.Debug)
                    ULog.Log("ResourceLoader "+loader.url+" is OK.");
                object val;
                if (_decode)
                    val = new Serializer().Decode(loader.bytes);
                else
                {
                    switch (resType)
                    {
                        case ResourceType.Textures:
                            val = loader.texture;
                            break;
                        case ResourceType.Text:
                            val = loader.text;
                            break;
                        case ResourceType.AudioClips:
                            val = loader.audioClip;
                            break;
                        case ResourceType.Assets:
                            val = loader.assetBundle;
                            break;
                        /*case ResourceType.Movies:
                            val = loader.movie;
                            break;*/
                        default:
                            val = loader.bytes;
                            break;
                    }
                }
                if (Gateway.Debug)
                    ULog.Log("ResourceLoader " + loader.url + "  to "+val+" is OK");
                OnGetResources(new Hashtable { { "result", val } });
            }
        }

		/// <summary>
		/// Result may be:
		/// - from gateway: back hashtable with resource in ['result'], back hashtable with ur in ['result'] - use direct download
		/// - from file: hashtable with resource in ['result']
		/// - from direct download: hashtable with resource in ['result']
		/// </summary>
		/// <param name="file"></param>
		/// <param name="resType"></param>
		/// <param name="post"></param>
		/// <returns></returns>
        [Obsolete("Use WaitLoadResources")]
		IEnumerator WaitLoadResource(string file,ResourceSource resType,Hashtable post)
		{
			//Debug.Log("WaitLoadResource: " + file + " " + Enum.GetName(typeof(ResourceSource), resType));
			WWW loader;
			if (_getTable!=null && _getTable.Count > 0)
			{
				file += "?";
			    foreach (DictionaryEntry entry in _getTable)
			        file = file + (entry.Key + "=" + entry.Value + "&");
			    file = file.Remove(file.Length - 2, 1);
			}
			if (post != null && post.Count > 0)
			{
				var form = new WWWForm();
				foreach (DictionaryEntry entry in post)
					form.AddField(entry.Key.ToString(), entry.Value.ToString());
				if (Gateway.Key != String.Empty)
					form.AddField(Gateway.KeyName, Gateway.Key);
				loader = new WWW(file, form);
			}
			else
			{
				//Debug.Log(file);
				loader = new WWW(file);
			}
				
			yield return loader;
			/*while (!loader.isDone)
				yield return loader;*/
			if (loader.error != null)
			{
				ULog.Log(loader.error,ULogType.Error);
				Destroy(gameObject);
			}else
			{
				object val;
				if (_decode)
					val = new Serializer().Decode(loader.bytes);
				else
				{
					switch (resType)
					{
						case ResourceSource.Textures:
							val = loader.texture;
							break;
						case ResourceSource.Text:
							val = loader.text;
							break;
						case ResourceSource.AudioClips:
							val = loader.audioClip;
							break;
						case ResourceSource.Assets:
							val = loader.assetBundle;
							break;
						/*case ResourceSource.Movies:
							val = loader.movie;
							break;*/
						default:
							val = loader.bytes;
							break;
					}
				}
				OnGetResources(new Hashtable { { "result", val } });
			}
		}

		/*void OnGetResourceLink(object inData)
		{
			_decode = false;
			var table = (Hashtable) inData;
			StartCoroutine(WaitLoadLocalResource("http://" + table["url"].ToString().Replace("http://", "").Replace("https://", ""), (ResourceSource)table["type"], (Hashtable)table["post"]));
		}*/

		/*void OnGetResourceLink(object inData)
		{
			var table = (Hashtable) inData;
			//Debug.Log("OnGetResourceLink " + table["result"]);
			StartCoroutine(WaitLoadLocalResource("http://" + table["result"].ToString().Replace("http://", "").Replace("https://", ""), 
												_type, table,null));
		}*/

        void OnGetResources(object inData)
        {
            if (Gateway.Debug)
                ULog.Log("ResourceLoader.OnGetResource(key:"+_key+") " + inData);
            var table = (Hashtable)inData;
            if (Gateway.Debug)
                foreach (DictionaryEntry entry in table)
                    ULog.Log("OnGetResource Hashtable: " + entry.Key + " : " + entry.Value);

            if (table.ContainsKey("url"))
            {
                _decode = (bool)table["decode"];
                StartCoroutine(
                    WaitLoadResources(
                        Gateway.Host + Gateway.Path + Gateway.File.Replace(".php", ".ogg"),
                        _resourceType,
                        new Hashtable { { "r", _resourceType }, { "n", table["url"].ToString() } }));
            }
            else
            {
                var res = ResourceManager.AddResource(_key, table["result"]);
                if (_callback != null)
                    _callback(_key, res);
                Destroy(gameObject);
            }
        }

        /*[System.Obsolete("Use OnGetResources")]
		void OnGetResource(object inData)
		{
            if(Gateway.Debug)
                ULog.Log("OnGetResource "+inData);
			var table = (Hashtable)inData;
            if (Gateway.Debug)
                foreach (DictionaryEntry entry in table)
                    ULog.Log("OnGetResource Hashtable: " + entry.Key + " : " + entry.Value);

		    if(table.ContainsKey("url"))
			{
				_decode = (bool)table["decode"];
				StartCoroutine(
					WaitLoadResource(
						Gateway.Host+Gateway.Path+Gateway.File.Replace(".php",".ogg"),
						_type,
						new Hashtable { { "r", _type }, { "n", table["url"].ToString() } }));
			}
			else
			{
                var txtr = ResourceManager.AddResource(_key, table["result"]);
			    if (_callback != null)
			        _callback(_key, txtr);
			    Destroy(gameObject);
			}
		}*/

		/*internal void LoadResource(string resName, string extension, ResourceSource resType, bool decoded, ResourceManager.OnGetResource callback)
		{
			_callback = callback;
			_key = resName;
			StartCoroutine(WaitLoadResource(resName,extension, resType, decoded));
		}

		IEnumerator WaitLoadResource(string resName, string extension, ResourceSource resType, bool decoded)
		{
			var table = new Hashtable
				{
					{"name", resName},
					{"type", resType},
					{"ext", extension}
				};
			if (Gateway.DefaultGateway == GatewayType.File)
			{
				((FileObject)Gateway.GetSender()).Call(table,
					"file://" + 
					ResourceManager.BasePath + "/" + 
					ResourceManager.ResourcePaths[resType]+"/" +
					resName + "." + extension,
					ResourceManager.ResourceTypes[resType], 
					OnLoadRes, 
					decoded);
			}
			else
			{
				Gateway.GetSender().Call(
					ResourceManager.Instance, 
					"Load" + ResourceManager.ResourcePaths[resType],
					table,
					OnLoadRes);
			}
			yield return null;
		}

		void OnLoadRes(object inObject)
		{
			if(inObject is Hashtable)
			{
				var hash = (Hashtable) inObject;
				ResourceManager.AddResource(_key, hash["result"]);
				_callback(hash);
			}
			Destroy(gameObject);
		}*/
	}
}
