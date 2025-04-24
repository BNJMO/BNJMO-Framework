//  
// Copyright (c) BNJMO
//

using System.Collections;
using UnityEngine;
using System;
using UnityEditor;
using Sirenix.OdinInspector;

namespace BNJMO
{
    public enum EUpdateMethod
    {
        UPDATE = 1,
        COROUTINE = 2,
    }

    public abstract class AnimationLerp<A> : BBehaviour
    {
        public event Action<AnimationLerp<A>> AnimationStarted;
        public event Action<AnimationLerp<A>, A> AnimationUpdated;
        public event Action<AnimationLerp<A>> AnimationRlooped;
        public event Action<AnimationLerp<A>> AnimationEnded;
        public event Action<AnimationLerp<A>> AnimationStopped;

        public string AnimationName { get { return animationName; } set { animationName = value; } }
        [Header("Animation Lerp")]
        [SerializeField] private string animationName = "AnimLerp_X";
        [SerializeField] public float PlayDuration = 3.0f;
        [SerializeField] public float StartDelay = 0.0f;
        [SerializeField] public bool PlayInReverse = false;
        [SerializeField] public A StartValue;
        [SerializeField] public A EndValue;
        [SerializeField] public bool IsLoop;
        [SerializeField] public EUpdateMethod UpdateMethod = EUpdateMethod.UPDATE;
        [SerializeField] public AnimationCurve Curve     { get { return curve; } }
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        [Header("Debug")]
        [SerializeField] bool logAnimationEvents = false;
        [Button("Start Animation")] private void Button_StartAnimation() { OnTestAnimationButton(); }

        public A CurrentValue           { get; private set; }
        public float CurrentPercentage  { get; private set; }
        public float CurrentAlpha       { get; private set; }
        
        [SerializeField] [ReadOnly] public bool IsPlaying;

        private AnimationLerpWrapper<A> valueWrapper = new AnimationLerpWrapper<A>();
        private IEnumerator CurrentAnimationEnumerator;
        float startTime;



        protected override void Awake()
        {
            base.Awake();

            AnimationStarted += On_AnimationStarted;
            AnimationUpdated += On_AnimationUpdated;
            AnimationRlooped += On_AnimationRlooped;
            AnimationEnded += On_AnimationEnded;
            AnimationStopped += On_AnimationStopped;
        }

        protected override void Update()
        {
            base.Update();

            if ((UpdateMethod == EUpdateMethod.UPDATE)
                && (IsPlaying == true))
            {
                if (CurrentPercentage < 1.0f)
                {
                    float percentage = (Time.time - startTime) / PlayDuration;
                    ProgressLerpAnimation(percentage);

                    InvokeEventIfBound(AnimationUpdated, this, CurrentValue);
                }
                else
                {
                    CurrentPercentage = 1.0f;

                    // Loop?
                    if (IsLoop == true)
                    {
                        InvokeEventIfBound(AnimationRlooped, this);
                        CurrentPercentage = 0.0f;
                    }
                    else
                    {
                        IsPlaying = false;
                        valueWrapper = new AnimationLerpWrapper<A>();
                        InvokeEventIfBound(AnimationEnded, this);
                    }
                }
            }

            // Debug text
            if (logAnimationEvents == true)
            {
                LogCanvas("AnimationLerp", AnimationName + " - Is Running : " + IsPlaying + "\n"
                    + "CurrentPercentage : " + CurrentPercentage + "\n"
                    + "CurrentAlpha : " + CurrentAlpha);
            }
        }

        public virtual void StartAnimation(ref AnimationLerpWrapper<A> animationLerpWrapper)
        {
            valueWrapper = animationLerpWrapper;

            StartAnimation();
        }

        public virtual void StartAnimation(A startValue, A endValue, float playTime = 0.0f, bool isLoop = false, bool playInReverse = false, float startDelay = 0.0f)
        {
            StartValue = startValue;
            EndValue = endValue;
            if (playTime > 0.0f)
            {
                PlayDuration = playTime;
            }
            if (startDelay > 0.0f)
            {
                StartDelay = startDelay;
            }
            IsLoop = isLoop;
            PlayInReverse = playInReverse;

            StartAnimation();
        }

        public virtual void StartAnimation()
        {
            if (PlayDuration > 0.0f)
            {
                switch (UpdateMethod)
                {
                    case EUpdateMethod.UPDATE:
                        StartAnimationUpdate();
                        break;

                    case EUpdateMethod.COROUTINE:
                        StartNewCoroutine(ref CurrentAnimationEnumerator, CurrentAnimationCoroutine(true));
                        break;
                }
            }
            else
            {
                LogConsoleError("Trying to start an AnimationLerp that has playTime set to 0!");
            }
        }

        public virtual void StopAnimation(bool setEndValue = false)
        {
            if (CurrentAnimationEnumerator != null)
            {
                StopCoroutine(CurrentAnimationEnumerator);
            }

            IsPlaying = false;

            InvokeEventIfBound(AnimationStopped, this);

            if (setEndValue == true)
            {
                ProgressLerpAnimation(1.0f);
            }
        }

        protected IEnumerator CurrentAnimationCoroutine(bool isFirstRun)
        {
            // Trigger start event and wait for delay
            if (isFirstRun == true)
            {
                InvokeEventIfBound(AnimationStarted, this);

                if (StartDelay > 0.0f)
                {
                    yield return new WaitForSeconds(StartDelay);
                }
            }

            // Animation body
            CurrentPercentage = 0.0f;
            CurrentValue = StartValue;
            startTime = Time.time;
            IsPlaying = true;

            while (CurrentPercentage < 1.0f)
            {
                float percentage = (Time.time - startTime) / PlayDuration;
                ProgressLerpAnimation(percentage);

                InvokeEventIfBound(AnimationUpdated, this, CurrentValue);

                yield return new WaitForEndOfFrame();
            }
            CurrentPercentage = 1.0f;

            // Loop?
            if (IsLoop == true)
            {
                InvokeEventIfBound(AnimationRlooped, this);
                StartNewCoroutine(ref CurrentAnimationEnumerator, CurrentAnimationCoroutine(true));
            }
            else
            {
                IsPlaying = false;
                valueWrapper = new AnimationLerpWrapper<A>();
                InvokeEventIfBound(AnimationEnded, this);
            }
        }

        protected void StartAnimationUpdate()
        {
            // Trigger start event
           
            InvokeEventIfBound(AnimationStarted, this);

            // TODO: Delay
            //if (StartDelay > 0.0f)
            //{
            //    yield return new WaitForSeconds(StartDelay);
            //}
            

            // Animation body
            CurrentPercentage = 0.0f;
            CurrentValue = StartValue;
            startTime = Time.time;
            IsPlaying = true;
        }



        private void ProgressLerpAnimation(float percentage)
        {
            CurrentPercentage = percentage;

            if (PlayInReverse == false)
            {
                CurrentAlpha = curve.Evaluate(CurrentPercentage);
            }
            else
            {
                CurrentAlpha = curve.Evaluate(1.0f - CurrentPercentage);
            }

            CurrentValue = Lerp(StartValue, EndValue, CurrentAlpha);

            valueWrapper.Value = CurrentValue;
        }

        protected abstract A Lerp(A start, A end, float alpha);

        private void OnTestAnimationButton()
        {
            if (BUtils.IsEditorPlaying() == true)
            {
                StartAnimation();
            }
        }
        #region Events Callbacks
        protected virtual void On_AnimationStarted(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " started. Play time : " + PlayDuration);
            }
        }

        protected virtual void On_AnimationUpdated(AnimationLerp<A> animationLerp, A value)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " progressed : " + CurrentPercentage);
            }
        }

        private void On_AnimationRlooped(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " relooped");
            }
        }

        protected virtual void On_AnimationEnded(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " ended");
            }
        }

        private void On_AnimationStopped(AnimationLerp<A> animationLerp)
        {
            if (logAnimationEvents == true)
            {
                LogConsole(AnimationName + " stopped");
            }
        }
        #endregion
    }
}
