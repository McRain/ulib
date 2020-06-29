using System;

namespace ULIB
{
    internal class ObjFilePlugin : IFilesPlugin
    {
        public string PluginType
        {
            get { return UPluginType.File; }
        }

        public void Activate()
        {
            //throw new NotImplementedException();
        }

        public void Added()
        {
            //throw new NotImplementedException();
        }

        public void Remove()
        {
            //throw new NotImplementedException();
        }

        public string FileType
        {
            get { return "obj"; }
        }

        public object Load(string fileName)
        {
            var str = FileManager.LoadText(fileName);

            return str;
        }

        public bool Save(string fileName, object value)
        {
            throw new NotImplementedException();
        }
    }
}
