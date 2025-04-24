namespace BNJMO
{
    public class AbstractPlayerStats
    {
        public EPlayerID PlayerID { get; set; }

        public AbstractPlayerStats()
        {
            
        }

        public AbstractPlayerStats(EPlayerID playerID)
        {
            PlayerID = playerID;
        }
    }
}