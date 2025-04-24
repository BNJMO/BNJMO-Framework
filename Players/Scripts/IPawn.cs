using UnityEngine;

namespace BNJMO
{
    public interface IPawn
    {
        Vector3 Position { get; }

        Quaternion Rotation { get; }

        void DestroyPawn();
    }
}