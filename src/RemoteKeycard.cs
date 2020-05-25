using Smod2;
using Smod2.Attributes;
using Smod2.Commands;
using Smod2.Config;
using Smod2.EventHandlers;
using Smod2.Events;
using System;

namespace RemoteKeycard
{
    [PluginDetails(
        author = "iRebbok",
        name = "RemoteKeycard",
        id = "net.irebbok.remotekeycard",
        version = _version,
        SmodMajor = 3,
        SmodMinor = 6,
        SmodRevision = 0)]
    public sealed class RemoteKeycard : Plugin
    {
        public const string _version = "1.4.1";

        private LogicHandler _logicHandler;

        public RemoteKeycard()
        {
            _logicHandler = new LogicHandler(this);
        }

        public override void OnDisable() { }

        public override void Register()
        {
            AddConfig(new ConfigSetting("rk_disable", false, false, "Activation control for the plugin"));
            AddConfig(new ConfigSetting("rk_cards", Array.Empty<string>(), false, "ItemTypes to use for filtering valid keycards"));
        }

        public override void OnEnable()
        {
            // Registration of events strictly after enabling of the plugin
            AddEventHandler(typeof(IEventHandlerDoorAccess), _logicHandler, Priority.High);
            AddEventHandler(typeof(IEventHandlerWaitingForPlayers), _logicHandler, Priority.Normal);
        }
    }
}
