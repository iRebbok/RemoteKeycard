using Smod2;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using SMItemType = Smod2.API.ItemType;

namespace RemoteKeycard
{
    internal sealed class LogicHandler : IEventHandlerDoorAccess, IEventHandlerWaitingForPlayers
    {
        private readonly Plugin _plugin;
        // Allowed items for remote access
        private SMItemType[] _allowedTypes;

        public LogicHandler(Plugin plugin)
        {
            _plugin = plugin;
            _allowedTypes = null;
        }

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            if (ev.Allow != false || ev.Door.Destroyed != false || ev.Door.Locked != false)
                return;

            var playerIntentory = ev.Player.GetInventory();

            playerIntentory.RemoveAll(i => _allowedTypes != null && !_allowedTypes.Any(ai => i.ItemType == ai));

            foreach (var item in playerIntentory)
            {
                var gameItem = item.GetComponent() as Item;
                if (gameItem.permissions == null || gameItem.permissions.Length == 0)
                    continue;

                if (gameItem.permissions.Contains(ev.Door.Permission, StringComparison.InvariantCultureIgnoreCase))
                {
                    ev.Allow = true;
                    continue;
                }
            }

        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            if (_plugin.GetConfigBool("rk_disable"))
                PluginManager.Manager.DisablePlugin(_plugin);
            else
            {
                var arrayItems = _plugin.GetConfigList("rk_cards");
                if (arrayItems == null || arrayItems.Length == 0)
                    return;

                var allowedItems = new List<SMItemType>();
                SMItemType allowedItem = SMItemType.NULL;
                foreach (var item in arrayItems)
                {
                    if (Enum.TryParse<SMItemType>(item, true, out var enumedItem))
                        allowedItem = enumedItem;
                    else if (int.TryParse(item, out var numericItem) && Enum.IsDefined(typeof(SMItemType), numericItem))
                        allowedItem = (SMItemType)numericItem;

                    if (allowedItem == SMItemType.NULL)
                        continue;

                    allowedItems.Add(allowedItem);
                }
                _allowedTypes = allowedItems.ToArray();
            }
        }
    }
}
