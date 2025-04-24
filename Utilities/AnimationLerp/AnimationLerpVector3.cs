using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpVector3 : AnimationLerp<Vector3>
    {
        protected override Vector3 Lerp(Vector3 start, Vector3 end, float alpha)
        {
            return Vector3.LerpUnclamped(start, end, alpha);
        }
    }
}