using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class RPCSender : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void OnCellPressedServerRpc(Vector2Int pressedCellPosition)
    {
        //     Cell pressedCell = null;
        //     switch (XOManager.Inst.CurrentGameMode.StateMachine.CurrentState)
        //     {
        //         /**/
        //         case EXOGameState.Overview:
        //             pressedCell = XOManager.Inst.BigBoard.GetCell(pressedCellPosition);
        //             break;
        //         case EXOGameState.InnerGrid:
        //             pressedCell = XOManager.Inst.CurrentGameMode.SelectedInnerBoard.GetCell(pressedCellPosition);
        //             break;
        //     }
        //
        //     XOManager.Inst.CurrentGameMode.OnCellPressed(pressedCell, null); //TODO: 
        //
        //     ulong hostId = Unity.Netcode.NetworkManager.Singleton.LocalClientId;
        //     var others = Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds
        //                      .Where(id => id != hostId).ToArray();
        //
        //     var param = new ClientRpcParams
        //     {
        //         Send = new ClientRpcSendParams { TargetClientIds = others }
        //     };
        //
        //     OnCellPressedClientRpc(pressedCellPosition, param);
        // }
        //
        // [ClientRpc]
        // public void OnCellPressedClientRpc(Vector2Int pressedCellPosition, ClientRpcParams clientRpcParams = default)
        // {
        //     Cell pressedCell = null;
        //     switch (XOManager.Inst.CurrentGameMode.StateMachine.CurrentState)
        //     {
        //         /**/
        //         case EXOGameState.Overview:
        //             pressedCell = XOManager.Inst.BigBoard.GetCell(pressedCellPosition);
        //             break;
        //         case EXOGameState.InnerGrid:
        //             pressedCell = XOManager.Inst.CurrentGameMode.SelectedInnerBoard.GetCell(pressedCellPosition);
        //             break;
        //     }
        //
        //     XOManager.Inst.CurrentGameMode.OnCellPressed(pressedCell, null); //TODO:
        // }
        //
        //
        // [ServerRpc(RequireOwnership = false)]
        // public void OnEndMultiplayerGameServerRpc(byte winnerPawnByte)
        // {
        //     EXOPawn winnerPawn = (EXOPawn)winnerPawnByte;
        //
        //     // XOManager.Inst.EndMultiplayerGame(winnerPawn);
        //
        //     ulong hostId = Unity.Netcode.NetworkManager.Singleton.LocalClientId;
        //     var others = Unity.Netcode.NetworkManager.Singleton.ConnectedClientsIds
        //                      .Where(id => id != hostId).ToArray();
        //
        //     var param = new ClientRpcParams
        //     {
        //         Send = new ClientRpcSendParams { TargetClientIds = others }
        //     };
        //
        //     OnEndMultiplayerGameClientRpc(winnerPawn, param);
        // }
        // [ClientRpc]
        // public void OnEndMultiplayerGameClientRpc(EXOPawn winnerPawn, ClientRpcParams clientRpcParams = default)
        // {
        //     // XOManager.Inst.EndMultiplayerGame(winnerPawn);
        // }
    }
}
