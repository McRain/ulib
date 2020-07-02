using System;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
    public class WwwObject : MonoBehaviour, IRemoteObject
    {
        private readonly List<UWebRequest> _requestPool = new List<UWebRequest>();
        private readonly List<UWebRequest> _activedRequest = new List<UWebRequest>();
        private readonly List<UWebRequest> _errorRequest = new List<UWebRequest>(); 

        public int RequestCointer;
        public int ActivedCointer;
        public int ErrorCounter;

        public delegate void RequestError(UWebRequest request);

        /// <summary>
        /// Evnt called on error in request
        /// </summary>
        public event RequestError OnRequestError;
        
        /// <summary>
        /// Return counter of actived request
        /// </summary>
        public int ActivedRequestsCount
        {
            get { return _activedRequest.Count; }
        }

        /// <summary>
        /// Call remote method.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="parameter"></param>
        public void Call(object target, string method, object parameter)
        {
            Call(target, method, parameter, null, null);
        }

        /// <summary>
        /// Call remote method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="parameter"></param>
        /// <param name="backTarget"></param>
        public void Call(object target, string method, object parameter, string backTarget)
        {
            Call(target, method, parameter, null, backTarget);
        }

        /// <summary>
        /// Call remote method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="parameter"></param>
        /// <param name="callBack"></param>
        /// <param name="write"></param>
        public void Call(object target, string method, object parameter, RemoteCallback callBack, bool write)
        {
            Call(target, method, parameter, callBack, null);
        }

        /// <summary>
        /// Call remote method
        /// </summary>
        /// <param name="target">An object that has the same class name with the server object, or name of a server class.</param>
        /// <param name="method">Name of target Method in Server class</param>
        /// <param name="parameter">sended parameters(object, dictionary, class ... etc)</param>
        /// <param name="callBack">Callback for recived server answer</param>
        public void Call(object target, string method, object parameter, RemoteCallback callBack)
        {
            Call(target, method, parameter, callBack, null);
        }

        public string Host { get; set; }

        public string Path { get; set; }

        public string File { get; set; }

        /// <summary>
        /// Call remote method
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="callback"></param>
        /// <param name="backTarget"></param>
        public UWebRequest Call(object target, string method, object parameters, RemoteCallback callback, string backTarget)
        {
            return Request(target, method, parameters, callback, backTarget);
        }

        private UWebRequest Request(object target, string method, object parameter, RemoteCallback callBack, string back)
        {
            if (_requestPool.Count > 0)
            {
                var request = _requestPool[0];
                _activedRequest.Add(request);
                ActivedCointer++;
                _requestPool.Remove(request);
                RequestCointer--;
                StartCoroutine(request.WaitTime(target is string ? target.ToString() : target.GetType().Name, method,
                                                    parameter, callBack, back));
                StartCoroutine(request.StartRequest(target is string ? target.ToString() : target.GetType().Name, method,
                                                    parameter, callBack, back));
                return request;
            }
            var newReqest = /*string.IsNullOrEmpty(Gateway.AppName)
                                ? */new UWebRequest(Host + Path + File, this)/*
                                : new UWebRequest(Gateway.Host + Gateway.Path + Gateway.File, this,
                                                  new Hashtable {{Gateway.AppParameter, Gateway.AppName}})*/;

            _activedRequest.Add(newReqest);
            ActivedCointer++;
            StartCoroutine(newReqest.WaitTime(target is string ? target.ToString() : target.GetType().Name, method,
                                                    parameter, callBack, back));
            StartCoroutine(newReqest.StartRequest(target is string ? target.ToString() : target.GetType().Name, method,
                                                  parameter, callBack, back));
            return newReqest;
        }

        internal void BackRequestToPool(UWebRequest request)
        {
            request.Clear();
            _requestPool.Add(request);
            RequestCointer++;
            _activedRequest.Remove(request);
            ActivedCointer--;
        }

        internal void PutRequestToError(UWebRequest request)
        {
            _errorRequest.Add(request);
            ErrorCounter++;
            _activedRequest.Remove(request);
            ActivedCointer--;
            if (OnRequestError != null)
                OnRequestError(request);
        }

        public void ResendRequest(UWebRequest request,object target,string method,object parameters,RemoteCallback callback,string back)
        {
            if(_requestPool.Contains(request))
                _requestPool.Remove(request);
            if(_activedRequest.Contains(request))
                _activedRequest.Remove(request);
            Request(target, method, parameters, callback, back);
        }

        /// <summary>
        /// Remove request from pool of the error requests
        /// </summary>
        /// <param name="request"></param>
        public void RemoveRequest(UWebRequest request)
        {
            if (_errorRequest.Contains(request))
                lock (_errorRequest)
                {
                    _errorRequest.Remove(request);
                    ErrorCounter--;
                }
        }

        public void Call(object target, string method, RemoteCallback callBack, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
