using System;
using System.IO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.BZip2;

namespace ULIB
{
	public partial class Serializer
	{
		/*static int GetMaterialIndex(Material material)
		{
			if (!_materialPack.Contains(material))
				_materialPack.Add(material);
			return _materialPack.IndexOf(material);
		}*/

		/*static int GetTextureIndex(Texture txtr)
		{
			if (_texturePack == null)
				return -1;
			if (!_texturePack.Contains(txtr))
				_texturePack.Add(txtr);
			return _texturePack.IndexOf(txtr);
		}*/

		/*byte[] GetResource(object value)
		{
            if (!_objectPack.Contains(value))
			{
                _objectPack.Add(value);
				_bytesPack.Add(Encoding(value));
			}
			var result = new byte[]{252,0,0,0,0};
            Buffer.BlockCopy(EncodeInteger(_objectPack.IndexOf(value)), 0, result, 1, 4);
			return result;
		}*/

		/// <summary>
		/// Compress byte array 
		/// For decompress use Decode
		/// 
		/// </summary>
		/// <param name="data">array of byte</param>
		/// <returns></returns>
		public byte[] Compress(byte[] data)
		{
			MemoryStream mMsBZip2 = null;
			BZip2OutputStream mOsBZip2 = null;
			byte[] result;
			try
			{
				mMsBZip2 = new MemoryStream();
				mOsBZip2 = new BZip2OutputStream(mMsBZip2);
				mOsBZip2.Write(data, 0, data.Length);
				mOsBZip2.Close();

				var packLength = mMsBZip2.ToArray().Length;
				var unpackLength = data.Length;
				result = new byte[11 + packLength];
				Buffer.SetByte(result, 0, Version);
				Buffer.SetByte(result, 1, 0x0);//Size of next head (bytes)
				#region Head
				//reserved
				//Buffer.SetByte(result, 2, 0x0);example head
				#endregion
				Buffer.SetByte(result, 2, 0xfe);//Compress
				Buffer.BlockCopy(BitConverter.GetBytes(packLength), 0, result, 3, 4);
				Buffer.BlockCopy(BitConverter.GetBytes(unpackLength), 0, result, 7, 4);
				Buffer.BlockCopy(mMsBZip2.ToArray(), 0, result, 11, packLength);
				mMsBZip2.Close();
				mMsBZip2.Dispose();

			}
			finally
			{
			    if (mOsBZip2 != null)
			        mOsBZip2.Dispose();
			    if (mMsBZip2 != null)
			        mMsBZip2.Dispose();
			}
			return result;
		}

		private static TripleDESCryptoServiceProvider _tdes = new TripleDESCryptoServiceProvider();

		/// <summary>
		/// Encrypt bytes . For decrypt use Decode(byte[] inBytes).
		/// For store KEY and IV read CryptoKey and CryptoIv after encrypt and set it before Decode encrypted data.
		/// </summary>
		/// <param name="bytesInput">non encrypted bytes</param>
		/// <returns>encrypted bytes</returns>
		public byte[] Encrypt(byte[] bytesInput)
		{
			/*//СОЗДАЕМ TRIPLE DES 
			_tdes = new TripleDESCryptoServiceProvider();
			//COЗДАЕМ ENCRYPTOR
			using (var encryptor = _tdes.CreateEncryptor())
			{
				var result = new byte[bytesInput.Length];
				//БУФЕР
				var bytesBuffer = new byte[encryptor.OutputBlockSize];
				var index = 0;
				//ШИФРУЕМ
				while (bytesInput.Length - index > encryptor.InputBlockSize)
				{
					encryptor.TransformBlock(bytesInput, index, encryptor.InputBlockSize,
						bytesBuffer, 0);
					index += encryptor.InputBlockSize;
					//ЗАПИСЫВАЕМ В STRINGBUILDER
					//for (var count = 0; count < bytesBuffer.Length; count++)
					//	sbEncrypted.Append(bytesBuffer[count] + "A");//А-это сепаратор
						Buffer.BlockCopy(bytesBuffer,0,result,index,bytesBuffer.Length);
				}
				//ЗАПИСЫВАЕМ ТО, ЧТО ОСТАЛОСЬ
				bytesBuffer = encryptor.TransformFinalBlock(bytesInput, index, bytesInput.Length - index);
				//ЗАПИСЫВАЕМ В STRINGBUILDER
				//for (int count = 0; count < bytesBuffer.Length; count++)
					//sbEncrypted.Append(bytesBuffer[count] + "A");
					Buffer.BlockCopy(bytesBuffer, 0, result, index, bytesBuffer.Length);
				return result;
			}//DISPOSING*/
			//СОХРАНЯЕМ TDES (НУЖНО ИСПОЛЬЗОВАТЬ ОДИН И ТОТ ЖЕ TDES)
			//Session["tdes"] = _tdes;
			//ПОСЫЛАЕМ ЗАШИФРОВАННЫЙ ПАРАМЕТР
			//this.Response.Redirect("receive.aspx?pam=" + sbEncrypted.ToString());

			_tdes.GenerateKey();
			_tdes.GenerateIV();
			var tdes = new TripleDESCryptoServiceProvider { Key = _tdes.Key, IV = _tdes.IV, Padding = PaddingMode.Zeros };
			var encryptor = tdes.CreateEncryptor();
			var ms = new MemoryStream();
			var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
			cs.Write(bytesInput, 0, bytesInput.Length);
			cs.FlushFinalBlock();
			ms.Position = 0;
			var result = new byte[ms.Length+3];
			ms.Read(result, 3, (int)ms.Length);
			cs.Close();

			Buffer.SetByte(result, 0, Version);
			Buffer.SetByte(result, 1, 0x0);//Size of next head (bytes)
			#region Head
			//reserved
			//Buffer.SetByte(result, 2, 0x0);example head
			#endregion
			Buffer.SetByte(result, 2, 0xfd);//Compress

			return result;
		}

		/// <summary>
		/// Serialize object and convert to a string for save or send via RPC. For deserialize, use Decode(string val).
		/// Used default splitter '-'
		/// </summary>
		/// <param name="inObj">Yor any object</param>
		/// <returns>Ready string</returns>
		public string EncodeToString(object inObj)
		{
			return EncodeToString(inObj, "-");
		}

		/// <summary>
		/// Serialize object and convert to a string for save or send via RPC. For deserialize, use Decode(string val,string splitter).
		/// You can set any splitter
		/// </summary>
		/// <param name="inObj">Your any object</param>
		/// <param name="splitter">Ready string </param>
		/// <returns></returns>
		public string EncodeToString(object inObj,string splitter)
		{
			return splitter != "-" ? EncodeToString(Encode(inObj)).Replace("-", splitter) : EncodeToString(Encode(inObj));
		}

		/// <summary>
		/// Convert bytes to a string for save or send via RPC. For deserialize, use DecodeBytes(string val).
		/// </summary>
		/// <param name="inBytes">byte array</param>
		/// <returns>Ready string</returns>
		public string EncodeToString(byte[] inBytes)
		{
			return BitConverter.ToString(inBytes);
		}
	}
}
