using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class AnimationLerpTransform : AnimationLerp<Transform>
    {
        [Header("Transform")]
        [SerializeField] public bool StartFromCurrentTransform = false;
        [SerializeField] public bool LerpPosition = true;
        [SerializeField] public bool LerpRotation = true;
        [SerializeField] public bool LerpScale = true;
        [SerializeField] public Transform AnimatedTransform;

        private STransform startTransform;
        private STransform endTransform;

        public override void StartAnimation()
        {
            if (StartValue == null)
            {
                return;
            }

            if (EndValue == null)
            {
                return;
            }

            if (AnimatedTransform == null)
            {
                AnimatedTransform = transform;
            }

            if (StartFromCurrentTransform)
            {
                if (PlayInReverse == false)
                {
                    startTransform = BUtils.GetTransformStruct(transform);
                    endTransform = BUtils.GetTransformStruct(EndValue);
                }
                else
                {
                    startTransform = BUtils.GetTransformStruct(StartValue);
                    endTransform = BUtils.GetTransformStruct(transform);
                }
            }
            else
            {
                startTransform = BUtils.GetTransformStruct(StartValue);
                endTransform = BUtils.GetTransformStruct(EndValue);
            }

            base.StartAnimation();
        }

        public void StartAnimation(Transform animatedTransform)
        {
            if (IS_NOT_NULL(animatedTransform))
            {
                AnimatedTransform = animatedTransform;

                StartAnimation();
            }
        }

        protected override Transform Lerp(Transform start, Transform end, float alpha)
        {
            Vector3 position = startTransform.Position;
            if (LerpPosition == true)
            {
                position = Vector3.LerpUnclamped(startTransform.Position, endTransform.Position, alpha);
            }

            Quaternion rotation = startTransform.Rotation;
            if (LerpRotation == true)
            {
                rotation = Quaternion.LerpUnclamped(startTransform.Rotation, endTransform.Rotation, alpha);
            }

            Vector3 scale = startTransform.Scale;
            if (LerpScale == true)
            {
                scale = Vector3.LerpUnclamped(start.localScale, end.localScale, alpha);
            }

            if (IS_NOT_NULL(AnimatedTransform))
            { 
                AnimatedTransform.position = position;
                AnimatedTransform.rotation = rotation;
                AnimatedTransform.localScale = scale;
            }

            return AnimatedTransform;
        }

        protected override void On_AnimationEnded(AnimationLerp<Transform> animationLerp)
        {
            base.On_AnimationEnded(animationLerp);

            //AnimatedTransform = null;
        }
    }
}