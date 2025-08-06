using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace BNJMO
{
    public class NetcodeOnlineHandler : AbstractOnlineHandler
    {
        #region Public Events

        #endregion

        #region Public Methods
        
        /* Lobby and Matching making */
        public async override void CreatePrivateLobby()
        {
            BEvents.ONLINE_StartedLaunchingSession.Invoke(new (ELobbyType.Private));

            bool isSignedIn = await BAuthenticationManager.Inst.SignIn();
            if (isSignedIn == false)
            {
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.NoConnection);
                return;
            }
            
            CreateLobby(true);
        }

        public override async void JoinPrivateLobby(string lobbyCode)
        {
            if (ARE_EQUAL(lobbyCode, "", true)
                || ARE_ENUMS_NOT_EQUAL(StateMachine.CurrentState, EOnlineState.NotConnected, true)) 
                return;
            
            BEvents.ONLINE_StartedLaunchingSession.Invoke(new (ELobbyType.Private));
            
            bool isSignedIn = await BAuthenticationManager.Inst.SignIn();
            if (isSignedIn == false)
            {
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.NoConnection);
                return;
            }
            
            StartNewCoroutine(ref joiningOnlineSessionTimeoutEnumerator, JoiningMultiplayerTimeoutCoroutine());
            
            try
            {
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
                if (lobby == null)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinLobbyByCode);
                    return;
                }
                
                joinedLobby = lobby;
                StateMachine.UpdateState(EOnlineState.InLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinLobbyByCode);
            }
        }

        public override async void QuickMatch()
        {
            if (ARE_ENUMS_NOT_EQUAL(StateMachine.CurrentState, EOnlineState.NotConnected, true))
                return;
            
            BEvents.ONLINE_StartedLaunchingSession.Invoke(new (ELobbyType.QuickMatch));
            
            bool isSignedIn = await BAuthenticationManager.Inst.SignIn();
            if (isSignedIn == false)
            {
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.NoConnection);
                return;
            }
            
            StartNewCoroutine(ref joiningOnlineSessionTimeoutEnumerator, JoiningMultiplayerTimeoutCoroutine());
            
            try
            {
                // 1) Query for any lobby with exactly one slot open
                QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
                {
                    Count = 25,
                    Filters = new List<QueryFilter>
                    {
                        new (
                            QueryFilter.FieldOptions.AvailableSlots,
                            "0",
                            QueryFilter.OpOptions.GT
                        ),
                        new (
                            QueryFilter.FieldOptions.IsLocked,
                            "0",
                            QueryFilter.OpOptions.EQ
                        ),
                    }
                };
                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
                
                // 2) Keep only the public ones
                List<Lobby> publicLobbies = response.Results
                    .Where(lobby => lobby.IsPrivate == false)
                    .ToList();

                if (publicLobbies.Count > 0)
                {
                    // 3) Pick a random public lobby
                    int idx = UnityEngine.Random.Range(0, publicLobbies.Count);
                    Lobby lobby = publicLobbies[idx];

                    // 4) Join by ID
                    joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
                    if (joinedLobby == null)
                    {
                        OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinLobbyByQuickMatch);
                    }
                    
                    StateMachine.UpdateState(EOnlineState.InLobby);
                }
                else // no lobby available to join -> Create a new one
                {
                    CreateLobby();
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinLobbyByQuickMatch);
            }
        }

        public override async void SetLobbyLock(bool isLocked)
        {
            if (BAuthenticationManager.Inst.IsSignedIn == false)
            {
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.NoConnection);
                return;
            }
            
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    IsLocked = isLocked
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                LogConsoleError($"Failed to update lobby lock");
            }
        }
        
        public override async void ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason leaveReason)
        {
            ENetworkID oldLocalNetworkID = LocalNetworkID;
            if (joinedLobby != null)
            {
                try
                {
                    // Hand off lobby
                    // // TODO: Needs to be tested properly
                    if (bConfig.HandoffLobbyWhenHostDisconnects
                        && StateMachine.CurrentState == EOnlineState.InLobby
                        && Authority == EAuthority.HOST 
                        && joinedLobby.Players.Count > 1)
                    {
                        var players = joinedLobby.Players;
                        int myIndex = players.FindIndex(p => p.Id == AuthenticationService.Instance.PlayerId);
                        int nextIndex = (myIndex + 1) % players.Count;
                        string newHost = players[nextIndex].Id;
                    
                        joinedLobby = await LobbyService.Instance.UpdateLobbyAsync(
                            joinedLobby.Id,
                            new UpdateLobbyOptions { HostId = newHost }
                        );
                    }

                    if (Authority == EAuthority.HOST)
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                    }
                    else
                    {
                        await LobbyService.Instance.RemovePlayerAsync(
                            joinedLobby.Id,
                            AuthenticationService.Instance.PlayerId
                        );
                    }
                }
                catch (LobbyServiceException e)
                {
                    if (Authority == EAuthority.HOST)
                    {
                        LogConsoleError($"Failed to leave/cleanup lobby");
                        Debug.LogException(e);
                    }
                }
            }
      
            DisconnectFromRelay();
            joinedLobby = null;
            isStartingParty = false;
            LocalClientListener = null;
            ConnectedClientListeners.Clear();
            StopCoroutineIfRunning(ref joiningOnlineSessionTimeoutEnumerator);
            if (StateMachine.CurrentState != EOnlineState.NotConnected)
            {
                StateMachine.UpdateState(EOnlineState.NotConnected);
                BEvents.ONLINE_ShutdownSession.Invoke(new(leaveReason, oldLocalNetworkID));
            }
        }
     
        #endregion

        #region Inspector Variables
        

        #endregion

        #region Variables

        public override string LobbyCode
        {
            get
            {
                if (joinedLobby == null)
                    return "";
                
                return joinedLobby.LobbyCode;
            }
            protected set { }
        }

        public override ELobbyType LobbyType
        {
            get
            {
                if (joinedLobby == null)
                    return ELobbyType.None;
                
                return joinedLobby.IsPrivate ? ELobbyType.Private : ELobbyType.QuickMatch;
            }
            protected set { }
        }

        public override EAuthority Authority 
        {
            get
            {
                if (StateMachine.CurrentState is EOnlineState.NotConnected or EOnlineState.None)
                {
                    return EAuthority.LOCAL;
                }
                
                if (joinedLobby != null
                    && joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
                {
                    return EAuthority.HOST;
                }
                    
                return EAuthority.CLIENT;
            }
            protected set { }
        }

        public override IClientListener LocalClientListener { get; protected set; }

        public int RemainingAvailableSpotsInLobby { get; set; }
        
        private const string RELAY_CODE = "RELAY_CODE";
        
        private Lobby joinedLobby;
        private float heartBeatTimer;
        private float lobbyUpdateTimer;
        private bool isStartingParty;
        private BConfig bConfig => BManager.Inst.Config;
        private IEnumerator joiningOnlineSessionTimeoutEnumerator;

        #endregion

        #region Life Cycle

        protected override void Awake()
        {
            base.Awake();

            FetchOrSpawnNetworkManager();
        }

        protected override async void Start()
        {
            base.Start();
            
            StateMachine.DebugStateChange = true;
            StateMachine.UpdateState(EOnlineState.NotConnected);
            RemainingAvailableSpotsInLobby = BManager.Inst.Config.MaxNumberOfPlayersInParty;
        }
        
        protected override void Update()
        {
            base.Update();
            UpdateLobbyHeartbeat();
            UpdateLobby();
        }

        private IEnumerator JoiningMultiplayerTimeoutCoroutine()
        {
            yield return new WaitForSeconds(bConfig.JoinOnlineSessionTimeout);

            switch (StateMachine.CurrentState)
            {
                case EOnlineState.NotConnected:
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinLobbyTimeout);
                    break;
                
                case EOnlineState.InLobby when BManager.Inst.Config.AutomaticallyLaunchOnlineSession == true:
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinOnlineSessionTimeout);
                    break;
            }
        }
        
        #endregion

        #region Events Callbacks

        /* Relay */
        private void NetworkManager_OnClientConnected(ulong clientId)
        {
            // TODO: Completely remove if not used
        }
        
        private void NetworkManager_OnClientDisconnect(ulong clientID)
        {
            // TODO: Completely remove if not used
        }

        #endregion

        #region Others
        
        /* Update */
        private async void UpdateLobbyHeartbeat()   // TODO: Is this necessary? What does it even do?
        {
            if (StateMachine.CurrentState == EOnlineState.NotConnected
                || joinedLobby == null
                || Authority != EAuthority.HOST)
                return;
            
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0f)
            {
                float heartBeatTimerMax = 15;
                heartBeatTimer = heartBeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }

        private async void UpdateLobby()
        {
            if (StateMachine.CurrentState == EOnlineState.NotConnected
                || joinedLobby == null) 
                return;
            
            try
            {
                lobbyUpdateTimer -= Time.deltaTime;
                if (lobbyUpdateTimer > 0f) 
                    return;
                
                lobbyUpdateTimer = bConfig.LobbyUpdateTimeSeconds;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                // Auto start Online 
                if (StateMachine.CurrentState == EOnlineState.InLobby)
                {
                    if (Authority == EAuthority.HOST)
                    {
                        if (bConfig.AutomaticallyLaunchOnlineSession
                            && joinedLobby.Players.Count >= bConfig.NumberOfPlayersInLobbyToStartOnlineSession
                            && joinedLobby.Data[RELAY_CODE].Value == "0"
                            && isStartingParty == false)
                        {
                            StartOnlineSession();
                        }
                    }
                    else if (Authority == EAuthority.CLIENT)
                    {
                        if (joinedLobby.Data[RELAY_CODE].Value != "0")
                        {
                            JoinRelay(joinedLobby.Data[RELAY_CODE].Value);
                        }
                    }
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                {
                    ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason.HostLeft);
                }
            }
        }

        /* Lobby */
        private async void CreateLobby(bool isPrivate = false)
        {
            if (ARE_ENUMS_NOT_EQUAL(StateMachine.CurrentState, EOnlineState.NotConnected, true))
                return;
            
            StartNewCoroutine(ref joiningOnlineSessionTimeoutEnumerator, JoiningMultiplayerTimeoutCoroutine());
            
            try
            {
                int maxPlayers = RemainingAvailableSpotsInLobby;
                string lobbyName = GenerateRandomLobbyName();
                lobbyUpdateTimer = bConfig.LobbyUpdateTimeSeconds;

                var createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    IsLocked = false,
                    Data = new Dictionary<string, DataObject>
                    {
                        { RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, "0") },
                    }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                if (lobby == null)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.CreateLobby);
                    return;
                }
                
                joinedLobby = lobby;
                StateMachine.UpdateState(EOnlineState.InLobby);
                
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.CreateLobby);
            }
        }

        private async void StartOnlineSession()
        {
            if (StateMachine.CurrentState != EOnlineState.InLobby
                || Authority != EAuthority.HOST)
                return;

            try
            {
                if (joinedLobby == null)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.StartOnlineSession);
                }
                
                isStartingParty = true;
                
                string relayCode = await CreateRelay();
                if (string.IsNullOrEmpty(relayCode))
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.StartOnlineSession);
                    return;
                }

                if (joinedLobby == null) // In case the connection shut down when this method was waiting
                    return;
                
                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> 
                    {
                        {
                            RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode)
                        }
                    }
                });
                joinedLobby = lobby;
                StateMachine.UpdateState(EOnlineState.InOnlineSession);
                isStartingParty = false;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.StartOnlineSession);
            }
        }

        private string GenerateRandomLobbyName()
        {
            int id = UnityEngine.Random.Range(1000, 9999);
            return $"Lobby{id}";
        }

        /* Relay */
        private async Task<string> CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(joinedLobby.MaxPlayers);
                if (allocation == null)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.CreateRelay);
                    return null;
                }
                
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4, 
                    (ushort)allocation.RelayServer.Port, 
                    allocation.AllocationIdBytes, allocation.Key, 
                    allocation.ConnectionData);
                NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;
                NetworkManager.Singleton.StartHost();
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.CreateRelay);
                return null;
            }
        }

        private async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                if (joinAllocation == null)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinRelay);
                    return;
                }

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4, 
                    (ushort)joinAllocation.RelayServer.Port, 
                    joinAllocation.AllocationIdBytes, joinAllocation.Key, 
                    joinAllocation.ConnectionData, joinAllocation.HostConnectionData);
                bool startClientSuccess = NetworkManager.Singleton.StartClient();
                if (startClientSuccess == false)
                {
                    OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinRelay);
                    return;
                }
                
                NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
                StateMachine.UpdateState(EOnlineState.InOnlineSession);
            }
            catch (RelayServiceException e)
            {
                Debug.LogException(e);
                OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType.JoinRelay);
            }
        }
        
        private void DisconnectFromRelay()
        {
            if (StateMachine.CurrentState == EOnlineState.InOnlineSession
                && NetworkManager.Singleton.IsListening == false)
            {
                return;
            }

            NetworkManager.Singleton.Shutdown();
        }
        
        /* Failure */
        private void OnJoinMultiplayerFailure(EJoinOnlineSessionFailureType joinOnlineSessionFailureType)
        {
            LogConsoleError($"Couldn't join online session. Reason {joinOnlineSessionFailureType.ToString()}");
            StopCoroutineIfRunning(ref joiningOnlineSessionTimeoutEnumerator);
            BEvents.ONLINE_LaunchSessionFailed.Invoke(new(joinOnlineSessionFailureType));

            ShutdownLobbyAndMultiplayer(ELeaveOnlineSessionReason.JoinOnlineSessionFailure);
        }
        
        /* Network Manager */
        private void FetchOrSpawnNetworkManager()
        {
            // Look if existing in scene
            NetworkManager networkManager = FindAnyObjectByType<NetworkManager>();
            
            // Try to load prefab from BConfig
            if (networkManager == null)
            {
                NetworkManager networkManagerPrefab = BManager.Inst.Config.NetcodeNetworkManagerPrefab;
                
                // Try to load prefab from Resources
                if (networkManagerPrefab == null)
                {
                    networkManagerPrefab = Resources.Load<NetworkManager>(BConsts.PATH_NetcodeNetworkManager);
                }
                    
                if (IS_NULL(networkManagerPrefab, true))
                    return;

                networkManager = Instantiate(networkManagerPrefab);
                IS_NOT_NULL(networkManager);
            }
        }

        #endregion
    }
}
