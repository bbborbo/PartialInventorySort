using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static PartialInventorySort.PartialInventorySortPlugin;

namespace PartialInventorySort.Modules
{
    public static class Tools
    {
        internal static bool isLoaded(string modguid)
        {
            foreach (KeyValuePair<string, PluginInfo> keyValuePair in Chainloader.PluginInfos)
            {
                string key = keyValuePair.Key;
                PluginInfo value = keyValuePair.Value;
                bool flag = key == modguid;
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public static class Assets
    {
        /// <summary>
        /// Loads an embedded asset bundle
        /// </summary>
        /// <param name="resourceBytes">The bytes returned by Properties.Resources.ASSETNAME</param>
        /// <returns>The loaded bundle</returns>
        internal static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();

        internal static AssetBundle LoadAssetBundle(string bundleName)
        {
            if (loadedBundles.ContainsKey(bundleName))
            {
                return loadedBundles[bundleName];
            }

            AssetBundle assetBundle = null;
            assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(PartialInventorySortPlugin.PInfo.Location), bundleName));

            loadedBundles[bundleName] = assetBundle;

            return assetBundle;
        }
    }
    public static class Bindings
    {
        internal static bool AprilFools;

        public static bool BindSection(string sectionName)
        {
            return CustomConfigFile.Bind<bool>("Bad Item Academy : Full Section Config",
                sectionName,
                true,
                "Vanilla is FALSE. Set to false if you wish to disable changes made to an entire item or group of items.").Value;
        }
        internal static ConfigFile CustomConfigFile { get; set; }
        public static void Init()
        {
            CustomConfigFile = new ConfigFile(Paths.ConfigPath + $"\\{modName}.cfg", true);
            CustomConfigFile.SaveOnConfigSet = false;
        }
        public static void Save()
        {
            CustomConfigFile.SaveOnConfigSet = true;
            CustomConfigFile.Save();
        }
    }
}
