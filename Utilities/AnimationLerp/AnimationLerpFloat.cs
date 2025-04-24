using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpFloat : AnimationLerp<float>
    {
        protected override float Lerp(float start, float end, float alpha)
        {
            return Mathf.LerpUnclamped(start, end, alpha);
        }
    }
}