using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class PawnSpawnPositionBase : BBehaviour
    {
        public EPlayerID PayerID    { get { return playerID; } set { playerID = value; } }
        public Vector3 Position     { get { return transform.position; } set { transform.position = value; } }
        public Quaternion Rotation  { get { return transform.rotation; } set { transform.rotation = value; } }

        [SerializeField] private EPlayerID playerID;
        [SerializeField] private bool isHideMeshOnAwake;

        protected override void Awake()
        {
            base.Awake();

            if (isHideMeshOnAwake == true)
            {
                HideMesh();
            }
        }

        public virtual void HideMesh()
        {
            Renderer[] myMeshRenderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer myMeshRenderer in myMeshRenderers)
            {
                myMeshRenderer.enabled = false;
            }
        }
    }
}