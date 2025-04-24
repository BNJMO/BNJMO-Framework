using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpColor : AnimationLerp<Color>
    {
        private MeshRenderer animationMeshRenderer;
        private string parameterName;

        public void StartAnimation(MeshRenderer meshRenderer, string name = "_Color")
        {
            animationMeshRenderer = meshRenderer;
            parameterName = name;

            StartAnimation();
        }

        protected override Color Lerp(Color start, Color end, float alpha)
        {
            return Color.LerpUnclamped(start, end, alpha);
        }

        protected override void On_AnimationUpdated(AnimationLerp<Color> animationLerp, Color value)
        {
            base.On_AnimationUpdated(animationLerp, value);

            if (animationMeshRenderer)
            {
                animationMeshRenderer.material.SetColor(parameterName, value);
            }
        }

        protected override void On_AnimationEnded(AnimationLerp<Color> animationLerp)
        {
            base.On_AnimationEnded(animationLerp);

            animationMeshRenderer = null;
        }
    }
}