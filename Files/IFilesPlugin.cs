namespace ULIB
{
    public delegate object PluginFileLoader(string fileName);
    public delegate bool PluginFileSaver(string fileName,object data);

    public interface IFilesPlugin:IUlibPlugin
    {
        string FileType { get; }

        object Load(string fileName);

        bool Save(string fileName, object value);
    }
}
