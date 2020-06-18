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
            var nickName = ev.Player?.nicknameSync?.MyNick ?? "Null";
            var userId = ev.Player?.characterClassManager?.UserId ?? "Null";
            Log.Debug($"Player {nickName} ({userId}) is trying to open the door");
#pragma warning disable CS0618 // Type or member is obsolete
            Log.Debug($"Door permission: {(string.IsNullOrEmpty(ev.Door.permissionLevel) ? "None" : ev.Door.permissionLevel)}");
#pragma warning restore CS0618 // Type or member is obsolete
#endif
            if (ev.Allow != false || ev.Door.destroyed != false || ev.Door.locked != false)
#if DEBUG
            {
                Log.Debug($"Door is locked or destroyed or the player {nickName} ({userId}) has access to open it");
                return;
            }
            else
                Log.Debug($"Further processing allowed...");
#else
                return;
#endif

            var playerIntentory = ev.Player?.inventory.items;

#if DEBUG
            Log.Debug($"Player inventory is null: {playerIntentory == null}");
#endif

            foreach (var item in playerIntentory)
            {
#if DEBUG
                Log.Debug($"Processing an item in the player’s inventory: {item.id} ({(int)item.id})");
#endif

                if (_allowedTypes != null && _allowedTypes.Any() && !_allowedTypes.Contains(item.id))
                    continue;

                var gameItem = GetItems().FirstOrDefault(i => i.id == item.id);

#if DEBUG
                Log.Debug($"Game item is null: {gameItem == null}");
                Log.Debug($"Game item processing: C {gameItem.itemCategory} ({(int)gameItem.itemCategory}) | T {item.id} ({(int)item.id}) | P {string.Join(", ", gameItem.permissions)}");
#endif

                // Relevant for items whose type was not found
                if (gameItem == null)
                    continue;

                if (gameItem.permissions == null || gameItem.permissions.Length == 0)
                    continue;

                foreach(var itemPerm in gameItem.permissions)
                    if (ev.Door.backwardsCompatPermissions.TryGetValue(itemPerm, out var flag) && ev.Door.PermissionLevels.HasPermission(flag))
                    {
#if DEBUG
                        Log.Debug($"Item has successfully passed permission validation: {gameItem.id} ({(int)gameItem.id})");
#endif
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
