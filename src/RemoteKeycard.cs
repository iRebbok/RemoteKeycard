using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;

using PlayerHandlers = Exiled.Events.Handlers.Player;

namespace RemoteKeycard
{
    public sealed class RKConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool HandleLockersAccess { get; set; } = true;
        public bool HandleGeneratorsAccess { get; set; } = true;

        // Just make an empty space as []
        public ItemType[] Cards { get; set; } = Array.Empty<ItemType>();
    }

    public sealed class RemoteKeycard : Plugin<RKConfig>
    {
        internal static RemoteKeycard instance;

        private readonly LogicHandler _logicHandler = new LogicHandler();

        public override string Author => "iRebbok";

        public RemoteKeycard()
        {
            instance = this;
        }

        public override void OnEnabled()
        {
            PlayerHandlers.InteractingDoor += _logicHandler.OnDoorAccess;
            PlayerHandlers.InteractingLocker += _logicHandler.OnLockerAccess;
            PlayerHandlers.UnlockingGenerator += _logicHandler.OnGeneratorAccess;
#if DEBUG
            Log.Debug($"Allowed items for processing: {(Config.Cards?.Length > 0 ? string.Join(", ", Config.Cards) : "(null)")}");
#endif
        }

        public override void OnDisabled()
        {
            PlayerHandlers.InteractingDoor -= _logicHandler.OnDoorAccess;
            PlayerHandlers.InteractingLocker -= _logicHandler.OnLockerAccess;
            PlayerHandlers.UnlockingGenerator += _logicHandler.OnGeneratorAccess;
        }
    }
}
