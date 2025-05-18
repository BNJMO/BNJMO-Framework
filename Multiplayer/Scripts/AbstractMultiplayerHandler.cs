namespace BNJMO
{
    public abstract class AbstractMultiplayerHandler: BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        public abstract void CreatePrivateLobby();

        public abstract void JoinPrivateLobby(string lobbyCode);

        public abstract void QuickMatch();

        public abstract void SetLobbyLock(bool isLocked);

        public abstract void ShutdownLobbyAndMultiplayer(ELeaveMultiplayerReason leaveReason);
        
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables

        public abstract string LobbyCode { get; protected set; }
        
        public abstract ELobbyType LobbyType { get; protected set; }

        public abstract bool IsLocalPlayerHost { get; protected set; }
        
        public abstract ENetworkID LocalNetworkID { get; protected set; }
        
        public StateMachine<EMultiplayerState> StateMachine { get; protected set; } = new ();
        
        #endregion

        #region Life Cycle
        
        
        #endregion

        #region Events Callbacks

        #endregion
    }
}
