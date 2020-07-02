public interface IUlibPlugin
{
    string PluginType { get; }

    void Activate();

    void Added();

    void Remove();
}