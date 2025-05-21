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
        public ENetworkID OnNewClientListenerJoined(IClientListener newClientListener)
        {
            if (Authority != EAuthority.HOST
                || newClientListener == null)
                return ENetworkID.NONE;

            ENetworkID networkID;
            if (newClientListener.IsHost)
            {
                if (IS_KEY_CONTAINED(ConnectedClientListeners, ENetworkID.HOST_1, true))
                    return ENetworkID.NONE;
                
                networkID = ENetworkID.HOST_1;

            }
            else
            {
                networkID = GetNextFreeClientNetworkID();
                if (IS_NONE(networkID, true))
                    return ENetworkID.NONE;
            }

            newClientListener.NetworkID = networkID;
            ConnectedClientListeners.Add(networkID, newClientListener);
            if (newClientListener.IsLocalClient)
            {
                OnLocalPlayerControllerIDAssigned(newClientListener);
            }

            // Let all connected player listeners communicate their ENetworkIDs to everyone else in the party
            foreach (var clientListenerPairItr in ConnectedClientListeners)
            {
                ENetworkID networkIDItr = clientListenerPairItr.Key;
                IClientListener clientListenerItr = clientListenerPairItr.Value;
                clientListenerItr.RequestBroadcastAndSetNetworkID(networkIDItr);
            }
            return networkID;
        }

        public void OnClientListenerLeft(IClientListener clientListener)
        {
            if (Authority != EAuthority.HOST
                || clientListener == null)
                return;

            ENetworkID networkID = clientListener.NetworkID;
            if (ConnectedClientListeners.ContainsKey(networkID) == false)
                return;

            ConnectedClientListeners.Remove(networkID);
        }

        public void OnLocalPlayerControllerIDAssigned(IClientListener playerListener)
        {
            LocalClientListener = playerListener;
            BEvents.MULTIPLAYER_LaunchMultiplayerSucceeded.Invoke(new());
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
            
            if (LocalClientListener == null)
            {
                LogConsoleWarning("Trying to broadcast event but the local player listener is null!");
                return;
            }
            
            LocalClientListener.RequestBroadcastEvent(eventHandle, broadcastType, targetNetworkID);
        }
        
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables
        
        public abstract EAuthority Authority { get; protected set; }

        public abstract string LobbyCode { get; protected set; }
        
        public abstract ELobbyType LobbyType { get; protected set; }

        public abstract IClientListener LocalClientListener { get; protected set; }
        
        public ENetworkID LocalNetworkID
        {
            get
            {
                return LocalClientListener?.NetworkID ?? ENetworkID.LOCAL;
            }
        }

        public StateMachine<EMultiplayerState> StateMachine { get; } = new ();
        
        public Dictionary<ENetworkID, IClientListener> ConnectedClientListeners { get; } = new(); // TODO: Replicate to other clients. Currently only on host.
        
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
                if (ConnectedClientListeners.ContainsKey(networkIDitr) == false)
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
