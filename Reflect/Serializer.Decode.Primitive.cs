using System;

namespace ULIB
{
	public partial class Serializer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static bool DecodeBoolean(byte[] inBytes, ref int startPos)
		{
			//Debug.Log("DecodeBoolean "+inBytes[startPos].ToString());
			var result = false;
			if (inBytes[startPos] == 2)
				result = true;
			startPos++;
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static int DecodeInteger(byte[] inBytes, ref int startPos)
		{
			startPos += 4;
			return BitConverter.ToInt32(inBytes, startPos - 4);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static float DecodeSingle(byte[] inBytes, ref int startPos)
		{
			startPos += 4;
			return BitConverter.ToSingle(inBytes, startPos - 4);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public static float DecodeFloat(byte[] inBytes, ref int startPos)
        {
            startPos += 4;
            return BitConverter.ToSingle(inBytes, startPos - 4);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inBytes"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		public static string DecodeString(byte[] inBytes, ref int startPos)
		{
			var strLength = DecodeInteger(inBytes, ref startPos);
			startPos += strLength;
            //Debug.Log("DecodeString position "+startPos);
			return System.Text.Encoding.UTF8.GetString(inBytes, startPos - strLength, strLength);
		}

        public static DateTime DecodeDiteTime(byte[] inBytes,ref int startPos)
        {
            startPos += 8;
            return DateTime.FromBinary(BitConverter.ToInt64(inBytes, startPos - 8));
        }
	}
}
