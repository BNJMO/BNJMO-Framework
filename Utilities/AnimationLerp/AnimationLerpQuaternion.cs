using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpQuaternion : AnimationLerp<Quaternion>
    {
        protected override Quaternion Lerp(Quaternion start, Quaternion end, float alpha)
        {
            return Quaternion.LerpUnclamped(start, end, alpha);
        }
    }
}