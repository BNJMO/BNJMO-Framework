using UnityEngine;

namespace BNJMO
{
    public class BackUIAction : AbstractUIAction
    {
        public bool IsStaysInSameAppState { get { return isStaysInSameAppState; } }

        [SerializeField] private bool isStaysInSameAppState = true;
    }
}
