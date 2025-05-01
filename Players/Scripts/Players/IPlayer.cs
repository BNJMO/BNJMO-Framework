namespace BNJMO
{
    public interface IPlayer
    {
        EPlayerID PlayerID { get; }
        
        ESpectatorID SpectatorID { get; }
        
        EControllerID ControllerID { get; }
        
        ENetworkID NetworkID { get; }
        
        public bool IsLocalPlayer { get; }

        ETeamID TeamID { get; }

        string PlayerName { get; }
        
        EPlayerPartyState PartyState { get; }

        bool IsReady { get; }
        
        void DestroyPlayer();
    }
}
