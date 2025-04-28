using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class PlayerNetworkManager : AbstractSingletonManager<PlayerNetworkManager>
    {
        #region Public Events

        #endregion

        #region Public Methods

        #endregion

        #region Inspector Variables

        #endregion

        #region Private Variables

        private ENetworkID localNetworkID = ENetworkID.NONE;

        #endregion

        #region Life Cycle

        protected override void OnEnable()
        {
            base.OnEnable();

            // BEvents.NETWORK_NetworkStateUpdated.Event += On_NETWORK_NetworkStateUpdated;
            // BEvents.NETWORK_NewNetworkIDConnected.Event += On_NETWORK_NewNetworkIDConnected;
            // BEvents.NETWORK_NetworkIDDisconnected.Event += On_NETWORK_NetworkIDDisconnected;
            // BEvents.NETWORK_PlayerJoined.Event += On_NETWORK_PlayerJoined;
            // BEvents.NETWORK_PlayerLeft.Event += On_NETWORK_PlayerLeft;
            // BEvents.NETWORK_PlayerNameUpdated.Event += On_NETWORK_PlayerNameUpdated;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // BEvents.NETWORK_NetworkStateUpdated.Event -= On_NETWORK_NetworkStateUpdated;
            // BEvents.NETWORK_NewNetworkIDConnected.Event -= On_NETWORK_NewNetworkIDConnected;
            // BEvents.NETWORK_NetworkIDDisconnected.Event -= On_NETWORK_NetworkIDDisconnected;
            // BEvents.NETWORK_PlayerJoined.Event -= On_NETWORK_PlayerJoined;
            // BEvents.NETWORK_PlayerLeft.Event -= On_NETWORK_PlayerLeft;
            // BEvents.NETWORK_PlayerNameUpdated.Event -= On_NETWORK_PlayerNameUpdated;
        }

        #endregion

        #region Events Callbacks

        // private void On_NETWORK_NetworkStateUpdated(BEHandle<ENetworkState> handle)
        // {
        //     // Connected ?
        //     if (handle.Arg1 == ENetworkState.CLIENT
        //         || handle.Arg1 == ENetworkState.HOST)
        //     {
        //         localNetworkID = BEventManager.Inst.LocalNetworkID;
        //
        //         //EControllerID controllerID = BUtils.GetControllerIDFrom(localNetworkID);
        //
        //         //if (localNetworkID == ENetworkID.HOST)
        //         //{
        //         //    // Connect Controller
        //         //    InputManager.Instance.ConnectController(controllerID);
        //
        //         //    // Join Player
        //         //    localPlayerID = PlayerManager.Instance.OnNextFreePlayerJoinRequest(controllerID); // TODO : Move to OnButtonPressed
        //
        //         //    if (IS_NOT_NONE(localPlayerID))
        //         //    {
        //         //        partyMap.Add(controllerID, localPlayerID);
        //         //    }
        //         //}
        //     }
        //     else // Disconnected
        //     {
        //         // Disconnect all connected net controllers
        //         foreach (EControllerID controllerID in partyMap.Keys)
        //         {
        //             InputManager.Inst.DisconnectController(controllerID);
        //         }
        //
        //         // Reinitialize 
        //         localNetworkID = ENetworkID.NONE;
        //         //localPlayerID = EPlayerID.NONE;
        //         partyMap.Clear();
        //     }
        // }
        //
        // private void On_NETWORK_NewNetworkIDConnected(BEHandle<ENetworkID> handle)
        // {
        //     LogConsoleRed("On_NETWORK_NewNetworkIDConnected");
        //     if (BEventManager.Inst.LocalNetworkID == ENetworkID.HOST)
        //     {
        //         EControllerID newControllerID = BUtils.GetControllerIDFrom(handle.Arg1);
        //         if (IS_NOT_NONE(newControllerID)
        //             && InputManager.Inst.IsControllerConnected(newControllerID) == false)
        //         {
        //             StartCoroutine(JoinConnectedPlayerCoroutine(handle, newControllerID));
        //         }
        //     }
        // }
        //
        // private IEnumerator JoinConnectedPlayerCoroutine(BEHandle<ENetworkID> handle, EControllerID newControllerID)
        // {
        //     // Wait a delay
        //     yield return new WaitForSeconds(1.0f);
        //
        //     // Connect Controller
        //     InputManager.Inst.ConnectController(newControllerID, EControllerType.NetworkController);
        //
        //     // Join Player
        //     EPlayerID newPlayerID = PlayerManager.Inst.JoinPlayer(newControllerID); // TODO : Move to OnButtonPressed
        //     if (IS_NOT_NONE(newPlayerID))
        //     {
        //         // Spawn Player
        //         AbstractPawn newPawn = PlayerManager.Inst.SpawnPawn(newPlayerID);
        //         if (IS_NOT_NULL(newPawn))
        //         {
        //             newPawn.OwnerNetworkID = handle.Arg1;
        //         }
        //
        //         // Update map
        //         partyMap.Add(newControllerID, newPlayerID);
        //
        //         // Replicate all connected net controllers to all other clients (Refresh)
        //         // foreach (ENetworkID targetNetworkIDItr in BEventManager.Instance.GetConnectedNetworkIDs())
        //         // {
        //             // LogConsoleRed(targetNetworkIDItr.ToString());
        //             // if (targetNetworkIDItr != localNetworkID) // != Host
        //             // {
        //                 foreach (KeyValuePair<EControllerID, EPlayerID> pair in partyMap)
        //                 {
        //                     string playerName = PlayerManager.Inst.GetPlayerName(pair.Value);
        //                     BEvents.NETWORK_PlayerJoined.Invoke(
        //                         new BEHandle<EPlayerID, EControllerID, string>(pair.Value, pair.Key, playerName),
        //                         BEventReplicationType.TO_ALL);
        //                 }
        //             // }
        //         // }
        //     }
        // }
        //
        // private void On_NETWORK_NetworkIDDisconnected(BEHandle<ENetworkID> handle)
        // {
        //     if (BEventManager.Inst.LocalNetworkID == ENetworkID.HOST)
        //     {
        //         EControllerID controllerID = BUtils.GetControllerIDFrom(handle.Arg1);
        //         if (IS_NOT_NONE(controllerID)
        //             && IS_TRUE(InputManager.Inst.IsControllerConnected(controllerID))
        //             && IS_KEY_CONTAINED(partyMap, controllerID))
        //         {
        //             EPlayerID playerID = partyMap[controllerID];
        //             if (IS_NOT_NONE(playerID))
        //             {
        //                 partyMap.Remove(controllerID);
        //
        //                 // Disconnect Controller (-> Remove from party)
        //                 InputManager.Inst.DisconnectController(controllerID);
        //
        //                 BEvents.NETWORK_PlayerLeft.Invoke(
        //                     new BEHandle<EPlayerID, EControllerID>(playerID, controllerID),
        //                     BEventReplicationType.TO_ALL_OTHERS, true);
        //             }
        //         }
        //     }
        // }
        //
        // /* On Clients */
        // private void On_NETWORK_PlayerJoined(BEHandle<EPlayerID, EControllerID, string> handle)
        // {
        //     if (localNetworkID != handle.InvokingNetworkID
        //         && localNetworkID != ENetworkID.HOST
        //         && InputManager.Inst.IsControllerConnected(handle.Arg2) == false)
        //     {
        //         // Connect Controller
        //         InputManager.Inst.ConnectController(handle.Arg2, EControllerType.NetworkController);
        //
        //         // Update Party Map
        //         if (IS_KEY_NOT_CONTAINED(partyMap, handle.Arg2))
        //         {
        //             partyMap.Add(handle.Arg2, handle.Arg1);
        //         }
        //
        //         // Join Player
        //         IS_TRUE(PlayerManager.Inst.JoinPlayer(handle.Arg1, handle.Arg2, handle.Arg3));
        //
        //         // Spawn Player
        //         ENetworkID networkID = BUtils.GetNetworkIDFrom(handle.Arg2);
        //         AbstractPawn newPawn = PlayerManager.Inst.SpawnPawn(handle.Arg1);
        //         if (IS_NOT_NULL(newPawn)
        //             && IS_NOT_NONE(networkID))
        //         {
        //             newPawn.OwnerNetworkID = networkID;
        //         }
        //     }
        // }
        //
        // private void On_NETWORK_PlayerLeft(BEHandle<EPlayerID, EControllerID> handle)
        // {
        //     if (ARE_NOT_EQUAL(localNetworkID, handle.InvokingNetworkID)
        //         && ARE_NOT_EQUAL(localNetworkID, ENetworkID.HOST)
        //         && InputManager.Inst.IsControllerConnected(handle.Arg2) == true)
        //     {
        //         // Connect Controller
        //         InputManager.Inst.DisconnectController(handle.Arg2);
        //
        //         // Update Party Map
        //         if (IS_KEY_CONTAINED(partyMap, handle.Arg2))
        //         {
        //             partyMap.Remove(handle.Arg2);
        //         }
        //     }
        // }
        //
        // private void On_NETWORK_PlayerNameUpdated(BEHandle<EPlayerID, string> handle)
        // {
        //     PlayerManager.Inst.SetPlayerName(handle.Arg1, handle.Arg2);
        // }

        #endregion

        #region Others

        #endregion
    }
}