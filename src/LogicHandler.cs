using EXILED;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace RemoteKeycard
{
    internal sealed class LogicHandler
    {
        private readonly Plugin _plugin;
        // Allowed items for remote access
        private List<ItemType> _allowedTypes;
        // If the previous hash is the same,
        // then do not parse the list of allowed items again
        private int _allowedTypesHash;
        private Item[] _cache;

        public LogicHandler(Plugin plugin)
        {
            _plugin = plugin;
            _allowedTypes = null;
        }

        public void OnDoorAccess(ref DoorInteractionEvent ev)
        {
#if DEBUG
            Log.Info($"OnDoorAccess event is null: {ev == null}");
            Log.Info($"OnDoorAccess door is null: {ev?.Door == null}");
            Log.Info($"OnDoorAccess player is null: {ev?.Player == null}");
#endif
            if (ev.Allow != false || ev.Door.destroyed != false || ev.Door.locked != false)
                return;

            var playerIntentory = ev.Player.inventory.items;

#if DEBUG
            Log.Info($"OnDoorAccess player inventory is null: {playerIntentory == null}");
#endif

            foreach (var item in playerIntentory)
            {
                if (_allowedTypes != null && !_allowedTypes.Any() && !_allowedTypes.Contains(item.id))
                    continue;

                var gameItem = GetItems().FirstOrDefault(i => i.id == item.id);

                // Relevant for items whose type was not found
                if (gameItem == null)
                    continue;

#if DEBUG
                Log.Info($"OnDoorAccess game item is null: {gameItem == null}");
#endif

                if (gameItem.permissions == null || gameItem.permissions.Length == 0)
                    continue;

                foreach(var itemPerm in gameItem.permissions)
                    if (ev.Door.backwardsCompatPermissions.TryGetValue(itemPerm, out var flag) && ev.Door.PermissionLevels.HasPermission(flag))
                    {
                        ev.Allow = true;
                        continue;
                    }
            }

        }

        public void OnWaitingForPlayers()
        {
            if (Plugin.Config.GetBool("rk_disable"))
                _plugin.OnDisable();
            else
            {
                var arrayItems = Plugin.Config.GetString("rk_cards").Split(',');
                if (arrayItems == null || arrayItems.Length == 0 || _allowedTypesHash == arrayItems.GetHashCode())
                    return;

                _allowedTypesHash = arrayItems.GetHashCode();
                var allowedItems = new List<ItemType>();
                ItemType allowedItem = ItemType.None;
                foreach (var item in arrayItems)
                {
                    if (Enum.TryParse<ItemType>(item, true, out var enumedItem))
                        allowedItem = enumedItem;
                    else if (int.TryParse(item, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericItem) && Enum.IsDefined(typeof(ItemType), numericItem))
                        allowedItem = (ItemType)numericItem;

                    if (allowedItem == ItemType.None)
                        continue;

                    allowedItems.Add(allowedItem);
                }
                _allowedTypes = allowedItems;
            }
        }

        public Item[] GetItems()
        {
            if (_cache == null)
                _cache = GameObject.FindObjectOfType<Inventory>().availableItems;
            return _cache;
        }
    }
}
