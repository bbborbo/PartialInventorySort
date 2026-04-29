using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static PartialInventorySort.Modules.Bindings;
using PartialInventorySort.Modules;
using System.Runtime.CompilerServices;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[module: UnverifiableCode]
#pragma warning disable 
namespace PartialInventorySort
{
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    [BepInPlugin(guid, modName, version)]
    public partial class PartialInventorySortPlugin : BaseUnityPlugin
    {
        public static PluginInfo PInfo;
        public const string guid = "com." + teamName + "." + modName;
        public const string teamName = "RiskOfBrainrot";
        public const string modName = "PartialInventorySortSystem";
        public const string version = "1.0.0";

        public static SortMode questItemSortMode = SortMode.VeryFirst;
        public static SortMode scrapItemSortMode = SortMode.BeforeUnsorted;
        public static SortMode lunarItemSortMode = SortMode.VeryFirst;

        void Awake()
        {
            AddHooks();
        }

        public static void DebugBreakpoint(string methodName, int breakpointNumber = -1)
        {
            string s = $"({modName}) {methodName} IL hook failed!";
            if (breakpointNumber >= 0)
                s += $" (breakpoint {breakpointNumber})";
            Debug.LogError(s);
        }
    }
}
