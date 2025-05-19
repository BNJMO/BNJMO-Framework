using System.Collections.Generic;

namespace BNJMO
{
    public abstract class AbstractMultiplayerHandler : BBehaviour
    {
        #region Public Events


        #endregion

        #region Public Methods

        /* Match making */
        public abstract void CreatePrivateLobby();

        public abstract void JoinPrivateLobby(string lobbyCode);

        public abstract void QuickMatch();

        public abstract void SetLobbyLock(bool isLocked);

        public abstract void ShutdownLobbyAndMultiplayer(ELeaveMultiplayerReason leaveReason);
        
        /* Player Listener */
        public ENetworkID OnNewPlayerListenerJoined(IMultiplayerPlayerListener newPlayerListener)
        {
            if (Authority != EAuthority.HOST
                || newPlayerListener == null)
                return ENetworkID.NONE;

            ENetworkID networkID;
            if (newPlayerListener.IsHost)
            {
                if (IS_KEY_CONTAINED(ConnectedPlayerListeners, ENetworkID.HOST_1, true))
                    return ENetworkID.NONE;
                
                networkID = ENetworkID.HOST_1;

            }
            else
            {
                networkID = GetNextFreeClientNetworkID();
                if (IS_NONE(networkID, true))
                    return ENetworkID.NONE;
            }
            
            ConnectedPlayerListeners.Add(networkID, newPlayerListener);
            if (newPlayerListener.IsLocalPlayer)
            {
                LocalPlayerListener = newPlayerListener;
            }

            // Let all connected player listeners communicate their ENetworkIDs to everyone else in the party
            foreach (var playerListenerPairItr in ConnectedPlayerListeners)
            {
                ENetworkID networkIDItr = playerListenerPairItr.Key;
                IMultiplayerPlayerListener playerListenerItr = playerListenerPairItr.Value;
                playerListenerItr.RequestBroadcastAndSetNetworkID(networkIDItr);
            }
            return networkID;
        }

        public void OnPlayerListenerLeft(IMultiplayerPlayerListener playerListener)
        {
            if (Authority != EAuthority.HOST
                || playerListener == null)
                return;

            ENetworkID networkID = playerListener.NetworkID;
            if (ConnectedPlayerListeners.ContainsKey(networkID) == false)
                return;

            ConnectedPlayerListeners.Remove(networkID);
        }

        public void OnLocalPlayerControllerIDAssigned(IMultiplayerPlayerListener playerListener)
        {
            if (playerListener == null)
                return;

            LocalPlayerListener = playerListener;
        }
        
        /* Event Replication */
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID)
        {
            if (StateMachine.CurrentState != EMultiplayerState.InParty)
            {
                LogConsoleWarning("Trying to broadcast event but the Multiplayer State is not In Party");
                return;
            }
            
            if (LocalPlayerListener == null)
            {
                LogConsoleWarning("Trying to broadcast event but the local player listener is null!");
                return;
            }
            
            LocalPlayerListener.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
        }
        
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables
        
        public abstract EAuthority Authority { get; protected set; }

        public abstract string LobbyCode { get; protected set; }
        
        public abstract ELobbyType LobbyType { get; protected set; }

        public abstract IMultiplayerPlayerListener LocalPlayerListener { get; protected set; }
        
        public ENetworkID LocalNetworkID
        {
            get
            {
                return LocalPlayerListener?.NetworkID ?? ENetworkID.NONE;
            }
        }

        public StateMachine<EMultiplayerState> StateMachine { get; } = new ();
        
        public Dictionary<ENetworkID, IMultiplayerPlayerListener> ConnectedPlayerListeners { get; } = new();
        
        #endregion

        #region Life Cycle
        
        
        #endregion

        #region Events Callbacks

        #endregion
        
        #region Others
        
        /* NetworkID */
        private ENetworkID GetNextFreeClientNetworkID()
        {
            ENetworkID networkID = ENetworkID.NONE;
            foreach (ENetworkID networkIDitr in BConsts.NETWORK_CLIENTS)
            {
                if (ConnectedPlayerListeners.ContainsKey(networkIDitr) == false)
                {
                    networkID = networkIDitr;
                    break;
                }
            }
            return networkID;
        }
        
        #endregion
    }
}
