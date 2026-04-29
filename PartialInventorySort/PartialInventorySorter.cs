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

namespace PartialInventorySort
{
    public partial class PartialInventorySortPlugin
    {
        public static void AddHooks()
        {
            IL.RoR2.Inventory.SetItemAcquiredServer += ChangeItemAcquisitionOrder;
            //On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += SortInventoryDisplay;
        }
        public static void ChangeItemAcquisitionOrder(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            bool b = c.TryGotoNext(MoveType.After,
                x => x.MatchLdfld<Inventory>(nameof(Inventory.itemAcquisitionOrder)),
                x => x.MatchLdarg(1)
                );
            if (!b)
            {
                DebugBreakpoint(nameof(ChangeItemAcquisitionOrder));
                return;
            }
            c.Remove();
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Action<List<ItemIndex>, ItemIndex, Inventory>>((acquisitionOrder, itemIndexToAdd, inventory) =>
            {
                //do not sort any
                if (lunarItemSortMode == SortMode.Unsorted && questItemSortMode == SortMode.Unsorted && scrapItemSortMode == SortMode.Unsorted)
                {
                    acquisitionOrder.Add(itemIndexToAdd);
                    return;
                }

                List<ItemIndex> firstHalf = new List<ItemIndex>();
                List<ItemIndex> secondHalf = new List<ItemIndex>();

                ItemDef itemDefToAdd = ItemCatalog.GetItemDef(itemIndexToAdd);
                SortMode sortModeOfNewItem = GetSortMode(itemIndexToAdd);
                //bool sortNewByLunar = lunarItemSortMode != SortMode.Unsorted && (itemDefToAdd.deprecatedTier == ItemTier.Lunar);
                //bool sortNewByQuest = questItemSortMode != SortMode.Unsorted && (itemDefToAdd.ContainsTag(ItemTag.ObjectiveRelated));
                //bool sortNewByScrap = scrapItemSortMode != SortMode.Unsorted && (itemDefToAdd.ContainsTag(ItemTag.Scrap) || itemDefToAdd.ContainsTag(ItemTag.PriorityScrap));

                //sort all items in inventory into before or after our item
                foreach (ItemIndex index in acquisitionOrder)
                {
                    SortMode whereISortThisItem = GetSortMode(index);
                    //if this item has the same or an earlier sort mode as the new item, put it before the new item
                    //if this item has a later sort mode than the new item, put it after
                    if (whereISortThisItem > sortModeOfNewItem)
                        secondHalf.Add(index);
                    else
                        firstHalf.Add(index);
                }

                inventory.itemAcquisitionOrder = new List<ItemIndex>();
                inventory.itemAcquisitionOrder.AddRange(firstHalf);
                inventory.itemAcquisitionOrder.Add(itemIndexToAdd);
                inventory.itemAcquisitionOrder.AddRange(secondHalf);

                SortMode GetSortMode(ItemIndex indexToSort)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef(indexToSort);

                    bool sortByLunar = lunarItemSortMode != SortMode.Unsorted && (itemDef.deprecatedTier == ItemTier.Lunar);
                    if (sortByLunar)
                        return lunarItemSortMode;

                    bool sortByQuest = questItemSortMode != SortMode.Unsorted && (itemDef.ContainsTag(ItemTag.ObjectiveRelated));
                    if (sortByQuest)
                        return questItemSortMode;

                    bool sortByScrap = scrapItemSortMode != SortMode.Unsorted && (itemDef.ContainsTag(ItemTag.Scrap) || itemDef.ContainsTag(ItemTag.PriorityScrap));
                    if (sortByScrap)
                        return scrapItemSortMode;

                    return SortMode.Unsorted;
                }
            });
        }

        public static void SortInventoryDisplay(On.RoR2.UI.ItemInventoryDisplay.orig_UpdateDisplay orig, RoR2.UI.ItemInventoryDisplay self)
        {
            if (self == null || Run.instance == null || Run.instance.isRunning == false)
            {
                orig(self);
                return;
            }
        }
    }
}
