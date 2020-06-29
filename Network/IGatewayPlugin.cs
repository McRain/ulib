using UnityEngine;

namespace ULIB
{
    public interface IGatewayPlugin:IUlibPlugin
    {
        string Key { get;}
        IRemoteObject Gateway { get; }
        GameObject gameObject { get; }
    }
}
