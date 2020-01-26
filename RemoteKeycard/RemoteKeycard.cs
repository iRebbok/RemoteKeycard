using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;

namespace RemoteKeycard
{
    [PluginDetails(
        author = "iRebbok",
        name = "RemoteKeycard",
        id = "irebbok.remote.keycard",
        version = _version,
        SmodMajor = 3,
        SmodMinor = 5,
        SmodRevision = 0)]

    public class RemoteKeycard : Plugin
    {
        public const string _version = "1.3.3";

        internal static RemoteKeycard plugin;

        public override void OnDisable() =>
            ConfigManagers.Manager.ClearingData();

        public override void OnEnable()
        {
            plugin = this;
            ConfigManagers.Manager.ReloadConfig();
            this.Info($"{this.Details.name} ({this.Details.version}) successfully launched.");
        }

        public override void Register()
        {
            RegisterCommands();
            RegisterEvents(new EventHandlers());
        }

        private void RegisterCommands()
        {
            this.AddCommand("rk_disable", new DisableCommand());
            this.AddCommand("rk_reload", new ReloadCommand());
        }

        private void RegisterEvents(EventHandlers handler)
        {
            this.AddEventHandler(typeof(IEventHandlerDoorAccess), handler, Smod2.Events.Priority.High);
            this.AddEventHandler(typeof(IEventHandlerWaitingForPlayers), handler, Smod2.Events.Priority.Normal);
        }
    }
}
