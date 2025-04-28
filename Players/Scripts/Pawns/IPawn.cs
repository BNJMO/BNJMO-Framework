using UnityEngine;

namespace BNJMO
{
    public interface IPawn
    {
        IPlayer Player { get; }
        
        Vector3 Position { get; set; }

        Quaternion Rotation { get; set; }

        void DestroyPawn();
    }
}