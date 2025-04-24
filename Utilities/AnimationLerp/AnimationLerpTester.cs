using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BNJMO
{
    public class AnimationLerpTester : BBehaviour
    {
        private AnimationLerpColor animationLerpColor;
        private AnimationLerpFloat animationLerpFloat;
        private AnimationLerpTransform animationLerpTrasmform;
        private AnimationLerpWrapper<float> wrapperScale = new AnimationLerpWrapper<float>();
        private MeshRenderer meshRenderer;


        protected override void Update()
        {
            base.Update();

        }

        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            animationLerpColor = GetComponentWithCheck<AnimationLerpColor>();
            animationLerpFloat = GetComponentWithCheck<AnimationLerpFloat>();
            animationLerpTrasmform = GetComponentWithCheck<AnimationLerpTransform>();

            meshRenderer = GetComponentWithCheck<MeshRenderer>();
            
        }



        public void StartAnimationColor()
        {
            animationLerpColor.StartAnimation(meshRenderer);
        }

        public void StartAnimationFloat()
        {
            wrapperScale = new AnimationLerpWrapper<float>();
            animationLerpFloat.StartAnimation(ref wrapperScale);
        }
           
        public void StartAnimationTransform()
        {
            animationLerpTrasmform.StartAnimation(transform);
        }


    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AnimationLerpTester))]
    public class AnimationLerpTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AnimationLerpTester myTarget = (AnimationLerpTester)target;

            if (GUILayout.Button("Play Animation Color"))
            {
                myTarget.StartAnimationColor();
            }

            if (GUILayout.Button("Play Animation Float"))
            {
                myTarget.StartAnimationFloat();
            }
                 
            if (GUILayout.Button("Play Animation Transform"))
            {
                myTarget.StartAnimationTransform();
            }


        }
    }
#endif
}


