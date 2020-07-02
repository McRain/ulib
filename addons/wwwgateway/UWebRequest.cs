using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ULIB
{
    public enum WebReqestStat
    {
        /// <summary>
        /// 
        /// </summary>
        Wait = 0,
        /// <summary>
        /// 
        /// </summary>
        Work = 1,
        Error = 2
    }

    public class UWebRequest
    {
        private WebReqestStat _status = WebReqestStat.Wait;
        private WWWForm _form;
        private readonly string _url;
        private readonly WwwObject _parent;
        private Dictionary<string, string> _headers;

        /// <summary>
        /// 
        /// </summary>
        public WWW Loader;

        public int ErrorCount;

        internal UWebRequest(string url, WwwObject parent)
        {
            _parent = parent;
            _url = url.StartsWith("http://") || url.StartsWith("https://") ? url : "http://" + url;
            _form = new WWWForm();

        }

        public void Clear()
        {
            Loader = null;
            _form = null;
            if(_headers!=null)
                _headers.Clear();
            ErrorCount = 0;
            _status = WebReqestStat.Wait;
        }

        /// <summary>
        /// 
        /// </summary>
        public WebReqestStat Status
        {
            get { return _status; }
        }

        private bool _loaded;

        public IEnumerator WaitTime(object target, string method, object parameter, RemoteCallback callback, string back)
        {
            yield return new WaitForSeconds(5);
            if (!_loaded)
                _parent.ResendRequest(this,target, method, parameter, callback, back);
        }

        public IEnumerator StartRequest(object target, string method, object parameter, RemoteCallback callback, string back)
        {
            _loaded = false;
            _status = WebReqestStat.Work;
            _form = new WWWForm();
            if (!string.IsNullOrEmpty(Gateway.AppName))
                _form.AddField(Gateway.AppParameter, Gateway.AppName);
            _form.AddField(Gateway.ClassKey, target is string ? target.ToString() : target.GetType().Name);
            _form.AddField(Gateway.MethodKey, method);
            _form.AddField(Gateway.AppParameter, Gateway.AppName);
            if (parameter != null)
                _form.AddBinaryData("data", /*_parent.Encode(parameter)*/new Serializer().Encode(parameter));

            /*if (!Application.isWebPlayer && !string.IsNullOrEmpty(Gateway.Key))
            {
                _headers = new Dictionary<string, string>();
                _form.headers["Cookie"] = Gateway.KeyName + "=" + Gateway.Key;
                Loader = new WWW(_url, _form.data, _headers);
            }
            else*/
                Loader = new WWW(_url, _form);
            yield return Loader;
            _loaded = true;
            if (Loader.error != null)
            {
                Debug.LogError("UWebRequest: loader ERROR "+Loader.error);
                if (callback != null)
                    callback(Loader.error);
                _status = WebReqestStat.Error;
                _parent.PutRequestToError(this);
                ErrorCount++;
            }
            else
            {
                var decode = true;
                if (Loader.responseHeaders != null)
                {
                    if (!Application.isWebPlayer && Loader.responseHeaders.ContainsKey("SET-COOKIE"))
                    {
                        var cookies = Loader.responseHeaders["SET-COOKIE"].Split(';');
                        foreach (
                            var cook in
                                cookies.Select(cooky => cooky.Split('='))
                                    .Where(cook => cook[0].Trim().Equals(Gateway.KeyName)))
                            Gateway.Key = cook[1].Trim();
                    }
                    if (Loader.responseHeaders.ContainsKey("NODECODE"))
                        decode = false;
                }
                object val;
                if (decode)
                    val = new Serializer().Decode(Loader.bytes) ?? Loader.text;
                else
                    val = Loader.text;
                if(val is UError)
                {
                    _status = WebReqestStat.Error;
                    _parent.PutRequestToError(this);
                    ErrorCount++;
                }
                else
                {
                    if (back != null)
                        SetBack(back, ref target, val);
                    if (callback != null)
                        callback(val);
                    _status = WebReqestStat.Wait;
                    _parent.BackRequestToPool(this);
                }
            }
        }

        private static void SetBack(string backTarget, ref object targetObject, object val)
        {
            if (backTarget == null) return;
            var tp = targetObject.GetType();
            var objFieldInfo = tp.GetField(backTarget);
            if (objFieldInfo != null)
                objFieldInfo.SetValue(targetObject, val);
            else
            {
                var objPropertyInfo = tp.GetProperty(backTarget);
                if (objPropertyInfo == null) return;
                try
                {
                    objPropertyInfo.SetValue(targetObject, val, null);
                }
                catch(Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }
    }
}
