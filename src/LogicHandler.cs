using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;
using System;

namespace RemoteKeycard
{
    internal sealed class LogicHandler
    {
        private RKConfig RKConfig => RemoteKeycard.instance.Config;
        private Item[] _cache;

        public void OnDoorAccess(InteractingDoorEventArgs ev)
        {
            if (!ev.Player.IsHuman)
                return;

            RemoteKeycard.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the door");
            RemoteKeycard.Debug($"Door permission: {ev.Door.RequiredPermissions.RequiredPermissions}");

            DoorLockMode lockMode = DoorLockUtils.GetMode((DoorLockReason)ev.Door.ActiveLocks);
            RemoteKeycard.Debug($"Door lock mode: {lockMode}");

            if (ev.IsAllowed
                || ((ev.Door is IDamageableDoor damageableDoor) && damageableDoor.IsDestroyed)
                || (ev.Door.NetworkTargetState && !lockMode.HasFlagFast(DoorLockMode.CanClose))
                || (!ev.Door.NetworkTargetState && !lockMode.HasFlagFast(DoorLockMode.CanOpen))
                || lockMode == DoorLockMode.FullLock)
            {
                RemoteKeycard.Debug("Door is locked, destroyed, or the player has access to open it");
                return;
            }

            RemoteKeycard.Debug("Further processing is allowed...");

            var playerIntentory = ev.Player.Inventory.items;
            ev.IsAllowed = Handle(playerIntentory, ev.Door.RequiredPermissions.RequiredPermissions.ToTruthyPermissions(), ev.Door.RequiredPermissions.RequireAll);
        }

        public void OnLockerAccess(InteractingLockerEventArgs ev)
        {
            if (!ev.Player.IsHuman || !RKConfig.HandleLockersAccess)
                return;

            RemoteKeycard.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the locker");
            RemoteKeycard.Debug($"Locker permissions: {ev.Chamber.accessToken}");

            if (ev.IsAllowed)
            {
                RemoteKeycard.Debug("Locker access is allowed");
                return;
            }

            RemoteKeycard.Debug("Further processing is allowed...");

            var permissions = Keycard.ToTruthyPermissions(ev.Chamber.accessToken);

            var playerIntentory = ev.Player.Inventory.items;
            ev.IsAllowed = Handle(playerIntentory, permissions);
        }

        public void OnGeneratorAccess(UnlockingGeneratorEventArgs ev)
        {
            if (!ev.Player.IsHuman || !RKConfig.HandleGeneratorsAccess)
                return;

            const Keycard.Permissions GENERATOR_ACCESS = Keycard.Permissions.ArmoryLevelTwo;

            RemoteKeycard.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the generator");
            RemoteKeycard.Debug($"Generator permissions: {GENERATOR_ACCESS}");

            if (ev.IsAllowed)
            {
                RemoteKeycard.Debug("Unlocking is allowed");
                return;
            }

            RemoteKeycard.Debug("Further processing is allowed...");

            var playerIntentory = ev.Player.Inventory.items;
            ev.IsAllowed = Handle(playerIntentory, GENERATOR_ACCESS);
        }

        public void OnOutsitePanelAccess(ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.Player.IsHuman || !RKConfig.HandleOutsidePanelAccess)
                return;

            const Keycard.Permissions PANEL_PERMISSION = Keycard.Permissions.ContainmentLevelThree;

            RemoteKeycard.Debug($"Player {ev.Player.Nickname} ({ev.Player.UserId}) is trying to access the outside panel");
            RemoteKeycard.Debug($"Outside panel permissions: {PANEL_PERMISSION}");

            if (ev.IsAllowed)
            {
                RemoteKeycard.Debug("Outside panel access is allowed");
                return;
            }

            RemoteKeycard.Debug("Further processing is allowed...");

            var pInv = ev.Player.Inventory.items;
            ev.IsAllowed = Handle(pInv, PANEL_PERMISSION);
        }

        private bool Handle(Inventory.SyncListItemInfo inv, Keycard.Permissions perms, bool requireAll = true)
        {
            if (perms == Keycard.Permissions.None)
                return true;

            foreach (var item in inv)
            {
                RemoteKeycard.Debug($"Processing an item in the player’s inventory: {item.id} ({(int)item.id})");

                if (RKConfig.Cards?.Length > 0 && !RKConfig.Cards.Contains(item.id))
                    continue;

                var gameItem = Array.Find(GetItems(), i => i.id == item.id);

                RemoteKeycard.Debug($"Game item is null: {gameItem == null}");
                // Relevant for items whose type was not found
                if (gameItem == null)
                    continue;

                var itemPerms = DoorPermissionUtils.TranslateObsoletePermissions(gameItem.permissions).ToTruthyPermissions();
                RemoteKeycard.Debug($"Game item processing: C {gameItem.itemCategory} ({(int)gameItem.itemCategory}) | T {item.id} ({(int)item.id}) | P {itemPerms}");

                if (itemPerms.HasFlagFast(perms, requireAll))
                {
                    RemoteKeycard.Debug($"Item has successfully passed permission validation: {gameItem.id} ({(int)gameItem.id})");
                    return true;
                }
            }

            return false;
        }

        public Item[] GetItems()
        {
            return _cache ?? (_cache = UnityEngine.Object.FindObjectOfType<Inventory>().availableItems);
        }
    }
}
