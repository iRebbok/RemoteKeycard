using EXILED;

namespace RemoteKeycard
{
    public sealed class RemoteKeycard : Plugin
    {
        public const string _version = "1.4.1";

        private LogicHandler _logicHandler;

        public override string getName { get; } = "RemoteKeycard";

        public RemoteKeycard()
        {
            _logicHandler = new LogicHandler(this);
        }

        public override void OnReload() { }

        public override void OnDisable()
        {
            Events.WaitingForPlayersEvent -= _logicHandler.OnWaitingForPlayers;
            Events.DoorInteractEvent -= _logicHandler.OnDoorAccess;
        }

        public override void OnEnable()
        {
            Events.WaitingForPlayersEvent += _logicHandler.OnWaitingForPlayers;
            Events.DoorInteractEvent += _logicHandler.OnDoorAccess;
        }

    }
}
