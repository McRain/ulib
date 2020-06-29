using System.Collections.Generic;
using System.Reflection;

namespace ULIB
{
    /// <summary>
    /// Implements methods for use ULIB-plugins or create your own plugin system.
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// The handler for activate custom plug-ins.
        /// </summary>
        /// <param name="iPlugin"></param>
        public delegate bool PluginHandler(IUlibPlugin iPlugin);

        /// <summary>
        /// The handler for deactivate custom plug-ins.
        /// </summary>
        /// <param name="iPlugin"></param>
        public delegate bool PluginRemover(IUlibPlugin iPlugin);

        private static readonly Dictionary<string, PluginHandler> Handlers = new Dictionary<string, PluginHandler>();
        private static readonly Dictionary<string, PluginRemover> Removers = new Dictionary<string, PluginRemover>();
        private static readonly List<IUlibPlugin> Plugins = new List<IUlibPlugin>();

        /// <summary>
        /// Add handler for register plugins.
        /// </summary>
        public static void RegisterPluginHandler(string pluginType, PluginHandler handler,PluginRemover remover)
        {
            if(!Handlers.ContainsKey(pluginType))
                Handlers.Add(pluginType,handler);
            if(!Removers.ContainsKey(pluginType))
                Removers.Add(pluginType, remover);
        }

        /// <summary>
        /// Remove handler for register plugins.
        /// </summary>
        /// <param name="pluginType"></param>
        public static void UnRegisterPluginHandler(string pluginType)
        {
            if (Handlers.ContainsKey(pluginType))
                Handlers.Remove(pluginType);
            if (!Removers.ContainsKey(pluginType))
                Removers.Remove(pluginType);
        }

        public static List<string> GetUlibPluginTypes()
        {
            var tp = typeof (UPluginType);
            var result = new List<string>();
            var finfos = tp.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var fieldInfo in finfos)
                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                    result.Add(fieldInfo.GetValue(null).ToString());
            return result;
        }

        /// <summary>
        /// Activated plugin and register.
        /// If you add plug-in type is not registered in ULIB - you need to add plug-in processor (RegisterPluginHandler) before adding the plug-in.
        /// Registered in ULIB type of plugin is:
        /// - "UPlugin"
        /// - "USerialize"
        /// - "ULanguage";
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <exception cref="PluginManagerException"></exception>
        public static void AddPlugin(IUlibPlugin plugin)
        {
            plugin.Activate();
            switch (plugin.PluginType)
            {
                case UPluginType.Plugin:
                    {
                        break;
                    }
                case UPluginType.Serialize:
                    {
                        Serializer.PluginHandler(plugin);
                        break;
                    }
                case UPluginType.Language:
                    {
                        LanguageManager.PluginHandler(plugin);
                        break;
                    }
                case UPluginType.Gateway:
                    {
                        Gateway.PluginHandler(plugin);
                        break;
                    }
                case UPluginType.File:
                    {
                        FileManager.PluginHandler(plugin);
                        break;
                    }
                case UPluginType.Quality:
                    {
                        QualityManager.PluginHandler(plugin);
                        break;
                    }
                default:
                    {
                        if (Handlers.ContainsKey(plugin.PluginType))
                            Handlers[plugin.PluginType](plugin);
                        else
                            throw new PluginManagerException(string.Format("Not find PluginHandler for type '{0}'. \nPlease use PluginManager.RegisterPluginHandler before. ", plugin.PluginType));
                        break;
                    }
            }

            if (!Plugins.Contains(plugin))
                Plugins.Add(plugin);
            plugin.Added();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <exception cref="PluginManagerException"></exception>
        public static void RemovePlugin(IUlibPlugin plugin)
        {
            if(Plugins.Contains(plugin))
            {
                plugin.Remove();
                switch (plugin.PluginType)
                {
                    case UPluginType.Plugin:
                        {
                            break;
                        }
                    case UPluginType.Serialize:
                        {
                            Serializer.PluginRemove(plugin);
                            break;
                        }
                    case UPluginType.Language:
                        {
                            LanguageManager.PluginRemove(plugin);
                            break;
                        }
                    case UPluginType.Gateway:
                        {
                            Gateway.PluginRemove(plugin);
                            break;
                        }
                    default:
                        {
                            if (Removers.ContainsKey(plugin.PluginType))
                                Removers[plugin.PluginType](plugin);
                            else
                                throw new PluginManagerException(string.Format("Not find Removers for type .{0}. \nPlease use PluginManager.RegisterPluginHandler before. ", plugin.PluginType));
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public static bool PluginExists(IUlibPlugin plugin)
        {
            if (Plugins.Contains(plugin))
                return true;
            foreach (var ulibPlugin in Plugins)
            {
                ULog.Log(ulibPlugin.PluginType);
            }
            return false;
        }
    }
}
