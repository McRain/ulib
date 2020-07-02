namespace ULIB
{
    public delegate void RemoteCallback(object data);

    public interface IRemoteObject
    {
        string Host { get; set; }
        string File { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="parameter"></param>
        /// <param name="callBack"> </param>
        void Call(object target, string method, object parameter, RemoteCallback callBack);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <param name="callBack"></param>
        /// <param name="parameters"> </param>
        void Call(object target, string method, RemoteCallback callBack,params object[] parameters);
    }
}
