using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public static class UTools
	{
		/// <summary>
        /// Returns the string value hour from seconds 
		/// </summary>
		/// <param name="secound"></param>
		/// <returns></returns>
		public static string SecoundToHour(float secound)
		{
			return (Mathf.FloorToInt(secound / 3600)).ToString("##00.");
		}

		/// <summary>
        /// Returns the string value minutes from seconds 
		/// </summary>
		/// <param name="secound"></param>
		/// <returns></returns>
		public static string SecoundToMinute(float secound)
		{
			return (Mathf.FloorToInt(secound / 60)).ToString("##00.");//. + ":" + Math.Round((mintime % 60),2).ToString();sr.mintime.ToString("##:##00.00")
		}

		/// <summary>
        /// Returns the string value second from seconds (rounded) 
		/// </summary>
		/// <param name="secound"></param>
		/// <returns></returns>
		public static string SecoundToSecound(float secound)
		{
			return Math.Round((secound % 60), 0).ToString("##00.");
		}

		/// <summary>
        /// Returns the string value second from millisecond
		/// </summary>
		/// <param name="secound"></param>
		/// <returns></returns>
		public static string SecoundToSecoundMs(float secound)
		{
			return Math.Round((secound % 60), 2).ToString("##,##00.00");
		}

		/// <summary>
		/// Return MD5 hash
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string GetMD5(string val)
		{
			var x = new MD5CryptoServiceProvider();
			var bs = Encoding.UTF8.GetBytes(val);
			bs = x.ComputeHash(bs);
			var s = new StringBuilder();
			foreach (var b in bs)
				s.Append(b.ToString("x2").ToLower());
			var password = s.ToString();
			return password;
		}

		/// <summary>
		/// Return SHA1 hash
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public static string GetSHA1(string val)
		{
			var x = new SHA1CryptoServiceProvider();
			var bs = Encoding.UTF8.GetBytes(val);
			bs = x.ComputeHash(bs);
			var s = new StringBuilder();
			foreach (var b in bs)
				s.Append(b.ToString("x2").ToLower());
			return s.ToString();
		}
	}
}
