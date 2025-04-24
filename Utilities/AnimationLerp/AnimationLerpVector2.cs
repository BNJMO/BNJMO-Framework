using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpVector2 : AnimationLerp<Vector2>
    {
        protected override Vector2 Lerp(Vector2 start, Vector2 end, float alpha)
        {
            return Vector2.LerpUnclamped(start, end, alpha);
        }
    }
}