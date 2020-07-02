using UnityEngine;

namespace ULIB
{
    public interface IComponentSerializer : ISerializePlugin
    {
        string ClassName { get; }

        byte[] ComponentEncode(Component component,IUlibSerializer serializer);

        Component ComponentDecode(byte[] bytes, ref int startPosition,ref GameObject go,IUlibSerializer serializer);
    }
}
