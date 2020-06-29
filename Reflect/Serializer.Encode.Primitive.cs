using System;
using System.Text;

namespace ULIB
{
	public partial class Serializer
	{
		private static readonly UTF8Encoding UEncoding = new UTF8Encoding();

        /// <summary>
        /// Return 1 byte (false - 1; true - 2)
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeBoolean(object inObject)
		{
			return (bool)inObject ? new byte[] { 2 } : new byte[] { 1 };
		}

        /// <summary>
        /// Return 4 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeInteger(object inObject)
		{
			return BitConverter.GetBytes((int)inObject);
		}

        /// <summary>
        /// Return 4 bytes
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeFloat(object inObject)
		{
			return BitConverter.GetBytes((float)inObject);
		}

        /// <summary>
        /// Return bytes with length of string data and strin in UTF8
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
		public static byte[] EncodeString(object inObject)
		{

			var encodedBytes = UEncoding.GetBytes((string)inObject);
			var stringLength = encodedBytes.Length;
			var result = new byte[4 + stringLength];
			Buffer.BlockCopy(BitConverter.GetBytes(stringLength), 0, result, 0, 4);
			Buffer.BlockCopy(encodedBytes, 0, result, 4, stringLength);
			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte[] EncodeDateTime(DateTime val)
        {
            return BitConverter.GetBytes(val.ToBinary());
        }

		/*byte[] EncodeEnum(Enum inObject)
		{
			var undertype = Enum.GetUnderlyingType(inObject.GetType());
			var enumClassBytes = EncodeString(undertype.Name);
			var enumCount = Enum.GetValues(undertype).Length;
			var result = new byte[enumClassBytes.Length + (enumCount * 4)];
			var i = enumClassBytes.Length;
			foreach (var byteI in from int ie in Enum.GetValues(undertype) select EncodeInteger(ie))
			{
				Buffer.BlockCopy(byteI, 0, result, i, 4);
				i += 4;
			}
			return result;
		}*/

	}
}
