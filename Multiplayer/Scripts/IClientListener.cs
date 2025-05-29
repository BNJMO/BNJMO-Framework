namespace BNJMO
{
    public interface IClientListener
    {
        public void RequestBroadcastAndSetNetworkID(ENetworkID newNetworkID);
        
        public void RequestBroadcastEvent(AbstractBEventHandle eventHandle, BEventBroadcastType broadcastType,
            ENetworkID targetNetworkID);
        
        public ENetworkID NetworkID { get; set; }
        
        public bool IsHost { get; set; } 
        
        public bool IsLocalClient { get; set; }
        
    }
}