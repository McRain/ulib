using System.Collections.Generic;

public class ULanguageLibraryRu:ILanguageLibrary
{
    public string PluginType
    {
        get { return "ULanguage"; }
    }

    public void Activate()
    {
            
    }

    public void Added()
    {
            
    }

    public void Remove()
    {
            
    }

    public string Key
    {
        get { return "ru"; }
    }

    public string Label
    {
        get { return "Русский"; }
    }

    public string Section
    {
        get { return string.Empty; }
    }

    public Dictionary<string, string> Translation
    {
        get { return new Dictionary<string, string>();}
    }
}