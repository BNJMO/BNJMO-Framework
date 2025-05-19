namespace BNJMO
{
    public interface IMultiplayerPlayerListener
    {
        public void RequestBroadcastAndSetNetworkID(ENetworkID newNetworkID);
        
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID);
        
        public ENetworkID NetworkID { get; set; }
        
        public bool IsHost { get; set; } 
        
        public bool IsLocalPlayer { get; set; }
        
    }
}