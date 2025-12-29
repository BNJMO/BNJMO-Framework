using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpCameraFOV : AnimationLerp<float>
    {

        [SerializeField] 
        private Camera cameraRef;

        protected override void OnValidate()
        {
            base.OnValidate();
            
            
            SetComponentIfNull(ref cameraRef);
        }

        protected override float Lerp(float start, float end, float alpha)
        {
            float newFOV = Mathf.LerpUnclamped(start, end, alpha);
            cameraRef.fieldOfView = newFOV;
            return newFOV;
        }
    }
}