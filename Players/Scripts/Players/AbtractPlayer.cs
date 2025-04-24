using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractPawn : BBehaviour, IPlayer, IPawn
    {
        public virtual EPlayerID PlayerID { get { return playerID; } set { playerID = value; } }

        public virtual ENetworkID OwnerNetworkID { get { return ownerNetworkID; } set { ownerNetworkID = value; } }

        public ETeamID TeamID { get; set; }

        public string PlayerName { get; } = "Player"; // TODO: Initialize when adding player

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public bool IsDead { get; set; }

        public void DestroyPawn()
        {
            Destroy(gameObject);
        }

        [SerializeField]
        private EPlayerID playerID = EPlayerID.NONE;

        [SerializeField]
        [ReadOnly]
        private ENetworkID ownerNetworkID = ENetworkID.NONE;
    }
}