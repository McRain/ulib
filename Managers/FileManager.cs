using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ULIB
{
	/// <summary>
	/// Provides methods for loading, reading, searching files and saving any object to files.
	/// </summary>
	public sealed class FileManager
	{
		private static readonly char[] IniSplitters = {'='};
		/// <summary>
		/// Symbols to identify sections: SectionChars[0] - symbol for start section , SectionChars[1] - for end section
		/// </summary>
		private static readonly char[] SectionSymbols = {'[',']'};
		/// <summary>
		/// Symbols to identify comments: default ';' and '#'
		/// </summary>
		private static readonly Regex CommentsSymbols = new Regex(@"^[;#]");

		private static readonly char[] IniTrim = {' ','"'};

        private static readonly Dictionary<string, PluginFileLoader> Loaders = new Dictionary<string, PluginFileLoader>();

        private static readonly Dictionary<string, PluginFileSaver> Savers = new Dictionary<string, PluginFileSaver>();

        #region Plugins

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iPlugin"></param>
        /// <returns></returns>
        internal static bool PluginHandler(IUlibPlugin iPlugin)
        {
            //ULog.Log("PluginHandler");
            if (iPlugin != null)
            {
                var ioface = (IFilesPlugin)iPlugin;
                Loaders.Add(ioface.FileType,ioface.Load);
                Savers.Add(ioface.FileType, ioface.Save);

                /*var gatewayKey = ioface.Key;
                var gatewayObject = ioface.Gateway;
                if (gatewayObject != null)
                {
                    gatewayObject.Host = Host;
                    gatewayObject.Path = Path;
                    gatewayObject.File = File;
                    //ULog.Log("PluginHandler " + gatewayKey);
                    if (!_gateways.ContainsKey(gatewayKey))
                        _gateways.Add(gatewayKey, gatewayObject);
                    else
                        throw new GatewayException(string.Format("A duplicate key : {0}.", gatewayKey));
                }
                else
                    throw new GatewayException(string.Format("Gateway is null in plugin."));*/
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        internal static bool PluginRemove(IUlibPlugin plugin)
        {
            var tp = plugin.GetType();
            var ioface = tp.GetInterface("IFilesPlugin");
            if (ioface != null)
            {
                /*var gatewayKey = tp.GetProperty("Key");
                if (_gateways.ContainsKey(gatewayKey.ToString()))
                    _gateways.Remove(gatewayKey.ToString());*/
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool PluginExists(object key)
        {
            return Loaders.ContainsKey(key.ToString());
        }

        #endregion

        #region PluginExecute

        public static object Load(string fileName,string fileType)
        {
            return Loaders.ContainsKey(fileType) ? Loaders[fileType](fileName) : null;
        }

        public static void Save(string fileName,string fileType, object data)
        {
            if(Savers.ContainsKey(fileType) )
                Savers[fileType](fileName, data);
        }

	    #endregion

        #region Text File

        /// <summary>
		/// Save data to text file
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="data">String data</param>
		public static void SaveText(string fileName, string data)
		{
            /*if(Gateway.Debug)
			    ULog.Log("Text " + data);
			var file = new FileInfo(fileName);
			var dirInfo = file.Directory;
			if (dirInfo != null && !dirInfo.Exists)
				dirInfo.Create();
			var sb = new StringBuilder();
			sb.Append(data);
			var stream = new StringWriter(sb);
			stream.Write(data);
			stream.Close();*/
            /*var writer = File.CreateText(fileName);
            writer.WriteLine(data);
            writer.Close();*/
            SaveBytes(fileName, new UTF8Encoding().GetBytes(data),true);
		}

		/// <summary>
		/// Return string data readed from text file or string.empty if file not exist (or on other problem)
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns></returns>
		public static string LoadText(string fileName)
		{
		    var fileBytes = LoadBytes(fileName);
		    return fileBytes.Length>0 ? Encoding.UTF8.GetString(fileBytes, 0, fileBytes.Length) : string.Empty;
		}

        /// <summary>
        /// Return string array readed from text file or empty array if file not exist (or on other problem)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] LoadTexts(string fileName)
        {
            var str = LoadText(fileName);
            return string.IsNullOrEmpty(str) ? new string[0] : str.Split('\n');
        }

        #endregion

        #region Object

        /// <summary>
		/// Encode any object and save to file
		/// </summary>
		/// <param name="fileName">File name</param>
        /// <param name="obj">any object (string,int,mesh,List,Dictionary,MyClass, etc.</param>
		/// <returns></returns>
		public static int SaveObject(string fileName,object obj)
		{
			return SaveBytes(fileName, new Serializer().Encode(obj), true);
		}

		/// <summary>
        /// Encode any object and save to file
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="obj">any object (string,int,mesh,List,Dictionary,MyClass, etc.</param>
		/// <param name="rewrite">If true: file will be overwrite; If false: file will be not saved</param>
		/// <returns>Return saved bytes count or 0 if not saved</returns>
		public static int SaveObject(string fileName, object obj,bool rewrite)
		{
			return SaveBytes(fileName, new Serializer().Encode(obj), rewrite);
		}

		/// <summary>
		/// Encode object and save to file. Can be compressed.
		/// </summary>
		/// <param name="fileName">File name</param>
        /// <param name="obj">any object (string,int,mesh,List,Dictionary,MyClass, etc.</param>
		/// <param name="rewrite">If true: file will be overwrite; If false: file will be not saved</param>
		/// <param name="compressed"></param>
		/// <returns>Return saved bytes count or 0 if not saved</returns>
		public static int SaveObject(string fileName, object obj, bool rewrite, bool compressed)
		{
			var ser = new Serializer();
			return SaveBytes(fileName, compressed ? ser.Compress(ser.Encode(obj)) : ser.Encode(obj), rewrite);
		}

		/// <summary>
		/// Loaded data from file and decode to object
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns></returns>
		public static object LoadObject(string fileName)
		{
		    var objectBytes = LoadBytes(fileName);
            return objectBytes.Length > 0 ? new Serializer().Decode(objectBytes) : null;
		    /*var file = new FileInfo(fileName);
			if (file.Exists)
			{
				var fs = new FileStream(fileName, FileMode.Open);
				var bytes = new byte[fs.Length];
				fs.Read(bytes, 0, (int)fs.Length);
				fs.Close();
				return new Serializer().Decode(bytes);
			}
			return null;*/
		}

        #endregion

        #region bytes

        /// <summary>
		/// Save bytes to file. If path not exists - will be created.
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="bytes">Array of byte</param>
		/// <param name="rewrite">If true: file will be overwrite; If false: file will be not saved (return 0)</param>
		/// <returns>Return saved bytes count or 0 if not saved</returns>
		public static int SaveBytes(string fileName, byte[] bytes,bool rewrite)
        {
            if (!rewrite && File.Exists(fileName))
                return 0;
            var writer = File.Create(fileName);
            writer.Write(bytes,0,bytes.Length);
            writer.Close();
            return bytes.Length;
            /*var file = new FileInfo(fileName);
			var dirInfo = file.Directory;
			if (dirInfo != null && !dirInfo.Exists)
				dirInfo.Create();

			//if (!file.Directory.Exists)
			//	Directory.CreateDirectory(file.Directory.FullName);
			if (!file.Exists || rewrite)
			{
				var fs = new FileStream(file.FullName, FileMode.Create);
				fs.Write(bytes, 0, bytes.Length);
				fs.Close();
				return bytes.Length;
			}
			return 0;*/
        }

		
		/// <summary>
		/// Return bytes loaded from file
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <returns></returns>
		public static byte[] LoadBytes(string fileName)
		{
		    var result = new byte[0];
            if (File.Exists(fileName))
            {
                var reader = File.OpenRead(fileName);
                result = new byte[reader.Length];
                reader.Read(result, 0, result.Length);
                reader.Close();
            }else if(Gateway.Debug)
		        ULog.Log("(179)FileManager error. File not exists: "+fileName,ULogType.Warning);
            return result;

		    /*var file = new FileInfo(fileName);
			if (file.Exists)
			{
				var fs = new FileStream(fileName, FileMode.Open);
				var bytes = new byte[fs.Length];
				fs.Read(bytes, 0, bytes.Length);
				fs.Close();
				return bytes;
			}
			return null;*/
		}

	    #endregion

        #region Ini

        /// <summary>
		/// Save data to ini-file.
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="dict">key/value pair data</param>
		/// <param name="merge">if true - the data will be combined, old value will bee rewrite if some key </param>
		public static void SaveIni(string fileName, Dictionary<string, object> dict,bool merge)
		{

			var file = new FileInfo(fileName);
			var dirInfo = file.Directory;
			if (dirInfo != null && !dirInfo.Exists)
				dirInfo.Create();

			var iniData = LoadIni(fileName, null);
			if(!merge)
				iniData.Clear();
			foreach (var keyValue in dict)
			{
				if (!iniData.ContainsKey(keyValue.Key))
					iniData.Add(keyValue.Key,keyValue.Value.ToString());
				else
					iniData[keyValue.Key] = keyValue.Value.ToString();
			}
			using (var streamWriter = new StreamWriter(fileName))
				foreach (var iniVal in iniData)
					streamWriter.WriteLine("{0}={1}", iniVal.Key, iniVal.Value);

		}
		
		/// <summary>
		/// Save sectioned data to ini-file
		/// </summary>
		/// <param name="fileName">File name</param>
		/// <param name="dict">key/value pair data</param>
		/// <param name="merge">if true - the data will be combined, old value will bee rewrite if some key </param>
        public static void SaveIni(string fileName, Dictionary<string, Dictionary<string, object>> dict, bool merge)
		{
		    var parser = new FileIniDataParser();
		    var data = new IniData();
            if(!merge && File.Exists(fileName))
            {
                File.Delete(fileName);
            }else
                data = parser.LoadFile(fileName);
            foreach (var section in dict)
            {
                var sectionName = section.Key;
                if (!data.Sections.ContainsSection(sectionName))
                    data.Sections.AddSection(sectionName);
                var sectionData = section.Value;
                var sec = data.Sections[sectionName];

                foreach (var keyVal in sectionData)
                {
                    if (!sec.ContainsKey(keyVal.Key))
                        sec.AddKey(keyVal.Key);
                    sec[keyVal.Key] = ValueToString(keyVal.Value);
                }
            }
		    parser.SaveFile(fileName,data);
		    /*var file = new FileInfo(fileName);
			var dirInfo = file.Directory;
			if (dirInfo != null && !dirInfo.Exists)
				dirInfo.Create();

			var parser = new FileIniDataParser();
			var data = new IniData();
			if(!merge)
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
			}else
				data = parser.LoadFile(fileName);
			foreach (var section in dict)
			{
				var sectionName = section.Key;
				if (!data.Sections.ContainsSection(sectionName))
					data.Sections.AddSection(sectionName);
				var sectionData = section.Value;
				var sec = data.Sections[sectionName];

				foreach (var keyVal in sectionData)
				{
					if (!sec.ContainsKey(keyVal.Key))
						sec.AddKey(keyVal.Key);
					sec[keyVal.Key] = keyVal.Value.ToString();
				}
					
			}
			parser.SaveFile(fileName, data);*/
		}

        private static string ValueToString(object val)
        {
            switch (val.GetType().FullName)
            {
                case "System.Single[]":
                    var f = string.Empty;
                    foreach (var v in (float[]) val)
                        f += v + ",";
                    return f.Remove(f.Length - 1);
                default:
                    return val.ToString();
            }
        } 

		/// <summary>
		/// Return the key / value pair, loaded from the ini-file.
		/// </summary>
		/// <param name="iniFile">File name</param>
		/// <param name="onlySection">If not null - loads only the specified section data.
		/// If null - load all key/value from file.</param>
		/// <returns></returns>
		public static Dictionary<string, string> LoadIni(string iniFile, string onlySection)
		{
		    return ParseIni(LoadText(iniFile), onlySection);
		    /*var targetFile = new FileInfo(iniFile);
			if (targetFile.Exists)
				using (var streamReader = new StreamReader(targetFile.FullName))
					result = ParseIni(streamReader.ReadToEnd(), onlySection);
			else if (Serializer.debug)
				ULog.Log(iniFile + " not exists.", ULogType.Warning);*/
			//return result;
		}

	    /// <summary>
		/// Return sectioned key/value pair loaded from the ini-file.
		/// </summary>
		/// <param name="iniFile">File name</param>
		/// <returns></returns>
		public static Dictionary<string, Dictionary<string, object>> LoadIni(string iniFile)
		{
			var result = new Dictionary<string, Dictionary<string, object>>();
			var parser = new FileIniDataParser();
			var data = parser.LoadFile(iniFile);
			foreach (var section in data.Sections)
			{
				result.Add(section.SectionName, new Dictionary<string, object>());
				foreach (var key in section.Keys)
					result[section.SectionName].Add(key.KeyName, key.Value);
			}
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iniData"></param>
		/// <param name="onlySection"></param>
		/// <returns></returns>
		public static Dictionary<string, string> ParseIni(string iniData, string onlySection)
		{
			var result = new Dictionary<string, string>();
			using (var streamReader = new StringReader(iniData))
			{

				string line;
				var targetSection = false;
				while ((line = streamReader.ReadLine()) != null)
				{
					if (!CommentsSymbols.IsMatch(line.TrimStart(), 0) &&
					    line.Trim().Length > 2) //skip comment and empty(3 char minimum: a=b
					{
						if (!string.IsNullOrEmpty(onlySection))
						{
							if (line.Trim().StartsWith(SectionSymbols[0].ToString()))
							{
								var section = line.Trim().Replace(SectionSymbols[0].ToString(), "").Replace(SectionSymbols[1].ToString(), "");
								targetSection = section.Equals(onlySection);
							}
							else if (targetSection)
							{
								var sarr = line.Split(IniSplitters);
								result.Add(sarr[0].Trim(), sarr[1].Trim(IniTrim));
							}
						}
						else if (!line.Trim().StartsWith(SectionSymbols[0].ToString()))
						{
							var sarr = line.Split(IniSplitters);
							if (!result.ContainsKey(sarr[0].Trim()))
								result.Add(sarr[0].Trim(), sarr[1].Trim(IniTrim));
						}
					}
				}
			}

			return result;
		}

        #endregion

        #region List

        /// <summary>
		/// Return all folders and files from path 
		/// </summary>
		/// <param name="dir">Folder for search</param>
		/// <param name="recursively">If true - include files and folders from subfolders</param>
		/// <param name="pattern">Search by part of name</param>
		/// <param name="extension">Search file by extension</param>
		/// <returns></returns>
		public static List<string> GetAll(string dir, bool recursively, string pattern, string extension)
		{
			var list = new List<string>();
			FindAll(dir, ref list, true, true, recursively,pattern,extension);
			return list;
		}

		/// <summary>
		/// Return all files from path
		/// </summary>
		/// <param name="dir">Folder for search</param>
		/// <param name="recursively">If true - include files from subfolders</param>
		/// <param name="pattern">Search by part of name </param>
		/// <param name="extension">Search file by extension</param>
		/// <returns></returns>
		public static List<string> GetFiles(string dir, bool recursively, string pattern,string extension)
		{
			//Debug.Log("GetFiles " + dir+" : "+extension);

			var list = new List<string>();
			FindAll(dir, ref list, true, false, recursively, pattern, extension);
			return list;
		}

		/// <summary>
		/// Return all folders from path
		/// </summary>
		/// <param name="dir">Folder for search</param>
		/// <param name="recursively">If true - include files from subfolders</param>
		/// <param name="pattern">Pattern for search by part of name</param>
		/// <returns></returns>
		public static List<string> GetFolders(string dir, bool recursively, string pattern)
		{
			var list = new List<string>();
			FindAll(dir, ref list, false, true, recursively, pattern,"");
			return list;
		}

	    private static void FindAll(string dir,ref List<string>  list, bool files,bool folders,bool recursive, string pattern,string extension)
		{
			var dirInfo = new DirectoryInfo(dir);
			var fsiArray = !pattern.Equals("") ? dirInfo.GetFileSystemInfos(pattern) : dirInfo.GetFileSystemInfos();
			foreach (var fsi in fsiArray)
			{
				if (fsi is DirectoryInfo)
				{
					if (folders)
						list.Add(fsi.FullName);
					if(recursive)
						FindAll(fsi.FullName, ref list, files, folders, true, pattern, extension);
				}
				else if (files && fsi.Extension.Contains(extension))
					list.Add(fsi.FullName);
			}
        }

        #endregion
    

    
    }
}
