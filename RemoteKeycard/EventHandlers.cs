using Smod2;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Collections.Generic;

namespace RemoteKeycard
{
    public class EventHandlers : IEventHandlerDoorAccess, IEventHandlerWaitingForPlayers
    {
        private Dictionary<int, Action<PlayerDoorAccessEvent>> ModeAction = new Dictionary<int, Action<PlayerDoorAccessEvent>>
        {
            { 1, LogicManager.LogicOneRemote        },
            { 2, LogicManager.LogicTwo              },
            { 3, LogicManager.LogicTree             },
            { 4, LogicManager.LogicFour             },
            { 5, LogicManager.LogicFive             },
            { 6, LogicManager.LogicSix              }
        };

        public void OnDoorAccess(PlayerDoorAccessEvent ev)
        {
            if (ev.Player.TeamRole.Team != Smod2.API.Team.SCP && !ev.Player.GetBypassMode())
            {
                if (ConfigManagers.Manager.RPCMode < 7 && ConfigManagers.Manager.RPCMode > 0)
                        ModeAction[ConfigManagers.Manager.RPCMode].Invoke(ev);
            }
        }

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            if (ConfigFile.GetBool("rpc_disable", false)) PluginManager.Manager.DisablePlugin(RemoteKeycard.plugin);
        }
    }
}