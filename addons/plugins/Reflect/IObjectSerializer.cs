using System;

namespace ULIB
{
    public interface IObjectSerializer:ISerializePlugin
    {
        Type ObjectType { get; }

        byte ObjectCode { get; }

        /// <summary>
        /// Do not forget that you need to specify the object code (1 byte).This is ObjectCode.
        /// The code should be the first byte in the array.
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        byte[] ObjectEncode(object sourceObject,IUlibSerializer serializer);

        /// <summary>
        /// You get a byte array, without the object code.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="startPosition"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        object ObjectDecode(byte[] bytes, ref int startPosition, IUlibSerializer serializer);

    }
}
