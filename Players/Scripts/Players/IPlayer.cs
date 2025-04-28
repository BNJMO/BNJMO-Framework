namespace BNJMO
{
    public interface IPlayer
    {
        EPlayerID PlayerID { get; }
        
        EControllerID ControllerID { get; }
        
        ENetworkID NetworkID { get; }

        ETeamID TeamID { get; }

        string PlayerName { get; }
        
        EPlayerState PlayerState { get; }

        bool IsReady { get; }
        
        void DestroyPlayer();
    }
}
