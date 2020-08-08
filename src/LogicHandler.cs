using Exiled.API.Features;
using Exiled.Events.EventArgs;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;

namespace RemoteKeycard
{
    internal sealed class LogicHandler
    {
        private RKConfig RKConfig => RemoteKeycard.instance.Config;
        private Item[] _cache;

        public void OnDoorAccess(InteractingDoorEventArgs ev)
        {
#if DEBUG
            Log.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the door");
#pragma warning disable CS0618 // Type or member is obsolete
            Log.Debug($"Door permission: {(string.IsNullOrEmpty(ev.Door.permissionLevel) ? "None" : ev.Door.permissionLevel)}");
#pragma warning restore CS0618 // Type or member is obsolete
#endif
            if (ev.IsAllowed || ev.Door.destroyed || ev.Door.locked)
#if DEBUG
            {
                Log.Debug($"Door is locked or destroyed or the player {ev.Player.Nickname} ({ev.Player.UserId}) has access to open it");
                return;
            }
            else
                Log.Debug("Further processing allowed...");
#else
                return;
#endif

            var playerIntentory = ev.Player.Inventory.items;

            var tempList = ListPool<string>.Shared.Rent();
            tempList.AddRange(GetDoorPermissions(ev.Door));
            ev.IsAllowed = Handle(playerIntentory, tempList);
            ListPool<string>.Shared.Return(tempList);
        }

        public void OnLockerAccess(InteractingLockerEventArgs ev)
        {
            if (!RKConfig.HandleLockersAccess)
                return;

#if DEBUG
            Log.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the locker");
            Log.Debug("Locker permissions: (null)");
#endif

            // I'm waiting for it
        }

        public void OnGeneratorAccess(UnlockingGeneratorEventArgs ev)
        {
            if (!RKConfig.HandleGeneratorsAccess)
                return;

            const string GENERATOR_ACCESS = "ARMORY_LVL_2";

#if DEBUG
            Log.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the generator");
            Log.Debug($"Generator permissions: {GENERATOR_ACCESS}");
#endif

            if (ev.IsAllowed)
#if DEBUG
            {
                Log.Debug("Unlocking allowed");
                return;
            }
            else
                Log.Debug("Further processing allowed...");
#else
                return;
#endif

            var playerIntentory = ev.Player.Inventory.items;

            var tempList = ListPool<string>.Shared.Rent();
            tempList.Add(GENERATOR_ACCESS);
            ev.IsAllowed = Handle(playerIntentory, tempList);
            ListPool<string>.Shared.Return(tempList);
        }

        private bool Handle(Inventory.SyncListItemInfo inv, List<string> perms)
        {
            foreach (var item in inv)
            {
#if DEBUG
                Log.Debug($"Processing an item in the player’s inventory: {item.id} ({(int)item.id})");
#endif

                if (RKConfig.Cards?.Length > 0 && !RKConfig.Cards.Contains(item.id))
                    continue;

                var gameItem = Array.Find(GetItems(), i => i.id == item.id);

#if DEBUG
                Log.Debug($"Game item is null: {gameItem == null}");
                Log.Debug($"Game item processing: C {gameItem.itemCategory} ({(int)gameItem.itemCategory}) | T {item.id} ({(int)item.id}) | P {string.Join(", ", gameItem.permissions)}");
#endif

                // Relevant for items whose type was not found
                if (gameItem == null)
                    continue;

                if (gameItem.permissions == null || gameItem.permissions.Length == 0)
                    continue;

                foreach (var itemPerm in gameItem.permissions)
                    if (perms.Contains(itemPerm, StringComparison.Ordinal))
                    {
#if DEBUG
                        Log.Debug($"Item has successfully passed permission validation: {gameItem.id} ({(int)gameItem.id})");
#endif
                        return true;
                    }
            }

            return false;
        }

        public Item[] GetItems()
        {
#pragma warning disable IDE0074 // Use compound assignment
            return _cache ?? (_cache = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems);
#pragma warning restore IDE0074 // Use compound assignment
        }

        private IEnumerable<string> GetDoorPermissions(Door door)
        {
            foreach (var pair in Door.backwardsCompatPermissions)
            {
                if ((door.PermissionLevels & pair.Value) != 0)
                    yield return pair.Key;
            }
        }
    }
}
