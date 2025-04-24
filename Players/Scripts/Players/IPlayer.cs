namespace BNJMO
{
    public interface IPlayer
    {
        EPlayerID PlayerID { get; }

        ETeamID TeamID { get; }

        string PlayerName { get; }
        
        bool IsDead { get; }
    }
}
