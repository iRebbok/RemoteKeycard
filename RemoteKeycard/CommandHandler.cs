using Smod2;
using Smod2.Commands;

namespace RemoteKeycard
{
    public class DisableCommand : ICommandHandler
    {
        public string GetCommandDescription() => "Disabling this plugin";
        public string GetUsage() => "rpc_disable";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            PluginManager.Manager.DisablePlugin(RemoteKeycard.plugin);
            return new string[] { "RemoteKeycard disable." };
        }
    }

    public class ReloadCommand : ICommandHandler
    { 
        public string GetCommandDescription() => "Reload configuration";
        public string GetUsage() => "rpc_reload";

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            ConfigManagers.Manager.ReloadConfig();
            return new string[] { "The configuration was successfully reloaded." };
        }
    }
}
