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
            RemoteKeycard.instance.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the door");
            RemoteKeycard.instance.Debug($"Door permission: {ev.Door.PermissionLevels}");

            if (ev.IsAllowed || ev.Door.Networkdestroyed || ev.Door.Networklocked)
            {
                RemoteKeycard.instance.Debug("Door is locked, destroyed, or the player has access to open it");
                return;
            }

            RemoteKeycard.instance.Debug("Further processing allowed...");

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

            RemoteKeycard.instance.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the locker");
            RemoteKeycard.instance.Debug("Locker permissions: (null)");

            if (ev.IsAllowed)
            {
                RemoteKeycard.instance.Debug("Locker access allowed");
                return;
            }

            RemoteKeycard.instance.Debug("Further processing allowed...");

            var playerIntentory = ev.Player.Inventory.items;

            var tempList = ListPool<string>.Shared.Rent();
            tempList.Add(ev.Chamber.accessToken);
            ev.IsAllowed = Handle(playerIntentory, tempList);
            ListPool<string>.Shared.Return(tempList);
        }

        public void OnGeneratorAccess(UnlockingGeneratorEventArgs ev)
        {
            if (!RKConfig.HandleGeneratorsAccess)
                return;

            const string GENERATOR_ACCESS = "ARMORY_LVL_2";

            RemoteKeycard.instance.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the generator");
            RemoteKeycard.instance.Debug($"Generator permissions: {GENERATOR_ACCESS}");

            if (ev.IsAllowed)
            {
                RemoteKeycard.instance.Debug("Unlocking allowed");
                return;
            }

            RemoteKeycard.instance.Debug("Further processing allowed...");

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
                RemoteKeycard.instance.Debug($"Processing an item in the player’s inventory: {item.id} ({(int)item.id})");

                if (RKConfig.Cards?.Length > 0 && !RKConfig.Cards.Contains(item.id))
                    continue;

                var gameItem = Array.Find(GetItems(), i => i.id == item.id);

                RemoteKeycard.instance.Debug($"Game item is null: {gameItem == null}");
                RemoteKeycard.instance.Debug($"Game item processing: C {gameItem.itemCategory} ({(int)gameItem.itemCategory}) | T {item.id} ({(int)item.id}) | P {string.Join(", ", gameItem.permissions)}");

                // Relevant for items whose type was not found
                if (gameItem == null)
                    continue;

                if (gameItem.permissions == null || gameItem.permissions.Length == 0)
                    continue;

                foreach (var itemPerm in gameItem.permissions)
                {
                    if (perms.Contains(itemPerm, StringComparison.Ordinal))
                    {
                        RemoteKeycard.instance.Debug($"Item has successfully passed permission validation: {gameItem.id} ({(int)gameItem.id})");
                        return true;
                    }
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
