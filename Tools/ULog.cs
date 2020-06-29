using System;
using UnityEngine;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public enum ULogType
	{
		/// <summary>
		/// 
		/// </summary>
		Log = 0,
		/// <summary>
		/// 
		/// </summary>
		Warning = 1,
		/// <summary>
		/// 
		/// </summary>
		Error = 2
	}

	/// <summary>
	/// 
	/// </summary>
	public static class ULog
	{
		/// <summary>
		/// 
		/// </summary>
		public static bool UseDebug;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgLog"></param>
		public static void Log(string msgLog)
		{
			Log(msgLog,ULogType.Log);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="msgLog"></param>
		/// <param name="logType"></param>
		public static void Log(string msgLog, ULogType logType)
		{
			if(UseDebug)
			{
                Debug.Log(msgLog);
				/*switch (logType)
				{
					case ULogType.Error:
						Debug.LogError(msgLog);
						break;
					case ULogType.Warning:
						Debug.LogWarning(msgLog);
						break;
					default:
						Debug.Log(msgLog);
						break;
				}*/
			}
			else
			{
				/*switch (logType)
				{
					case ULogType.Error:
						Console.WriteLine(msgLog);
						break;
					case ULogType.Warning:
						Console.WriteLine(msgLog);
						break;
					default:
						Console.WriteLine(msgLog);
						break;
				}*/
			}
		}
	}
}
