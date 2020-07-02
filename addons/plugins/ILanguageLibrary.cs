using System.Collections.Generic;

public interface ILanguageLibrary:IUlibPlugin
{
    string Key { get; }
    string Label { get; }
    string Section { get; }
    Dictionary<string,string> Translation { get; }
}