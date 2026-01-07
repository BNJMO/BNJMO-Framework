using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BNJMO
{
    public class PawnBase : BBehaviour, IPawn
    {
        #region Public Events


        #endregion

        #region Public Methods

        public void Init(SPawnInit pawnInit)
        {
            player = pawnInit.Player;
            Position = pawnInit.Position;
            Rotation = pawnInit.Rotation;
        }
        
        public void DestroyPawn()
        {
            BEvents.PAWNS_Destroyed.Invoke(new(player.PlayerID));
            Destroy(gameObject);
        }

        #endregion

        #region Inspector Variables

        [SerializeField] [ReadOnly] private PlayerBase player;

        #endregion

        #region Variables

        public IPlayer Player => player;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion

    }
}
