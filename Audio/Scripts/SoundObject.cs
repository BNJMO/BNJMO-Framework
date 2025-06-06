﻿using System.Collections;
using UnityEngine;
using System;

namespace BNJMO
{
    public class SoundObject : BBehaviour
    {
        public event Action<SoundObject> SoundFinishedPlayed;
        public event Action<SoundObject> SoundObjectWillGetDestroyed;

        public AudioSource AudioSource { get { return myAudioSource; } }
        public AudioClip AudioClip { get { return AudioClip; } }
        public bool DestroySoundWhenFinishedPlaying { get { return destroySoundWhenFinishedPlaying; } set { destroySoundWhenFinishedPlaying = value; } }

        [Header("Sound Object")]
        [SerializeField]
        private AudioSource myAudioSource;

        [SerializeField] 
        private AudioClip audioClip;

        [SerializeField]
        private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField]
        private bool destroySoundWhenFinishedPlaying = false;

        private IEnumerator onSoundFinishedPlayinEnumerator;
        private IEnumerator fadeInEnumerator;
        private IEnumerator fadeOutEnumerator;

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            SetComponentIfNull(ref myAudioSource);

            if (!audioClip && myAudioSource)
            {
                audioClip = myAudioSource.clip;
            }
        }

        /// <summary>
        /// Play 2D sound
        /// </summary>
        /// <param name="audioClipToPlay"></param>
        /// <param name="destroyWhenFinished"></param>
        /// <param name="isLoop"></param>
        public void PlaySound(AudioClip audioClipToPlay = null)
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;

            // Assign audio clip
            if (audioClipToPlay != null)
            {
                myAudioSource.clip = audioClipToPlay;
            }
            else
            {
                myAudioSource.clip = audioClip;
            }

            if (IS_NOT_NULL(myAudioSource.clip))
            {
                // Play sound
                myAudioSource.Play();

                // Callback for when sound finished playing
                StartNewCoroutine(ref onSoundFinishedPlayinEnumerator, OnSoundFinishedPlayingCoroutine(myAudioSource.clip.length, DestroySoundWhenFinishedPlaying));
            }
        }

        /// <summary>
        /// Play 3D sound at given position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="audioClipToPlay"></param>
        /// <param name="destroyWhenFinished"></param>
        /// <param name="isLoop"></param>
        public void Play3DSound(Vector3 position, AudioClip audioClipToPlay = null)
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;

            myAudioSource.spatialBlend = 1.0f;
            transform.position = position;

            PlaySound(audioClipToPlay);
        }

        /// <summary>
        /// Play 3D sound and attach it to given parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="audioClipToPlay"></param>
        /// <param name="destroyWhenFinished"></param>
        /// <param name="isLoop"></param>
        public void Play3DSound(Transform parent, AudioClip audioClipToPlay = null)
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;

            myAudioSource.spatialBlend = 1.0f;
            transform.parent = parent;

            PlaySound(audioClipToPlay);
        }

        public void StopSound()
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;

            myAudioSource.Stop();
        }

        [Sirenix.OdinInspector.Button("Fade Volume")]
        public void FadeVolume(float toVolume = 1.0f, float duration = 1.0f, bool stopSoundAtEndOfFade = false)
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;

            StopCoroutineIfRunning(ref fadeInEnumerator);
            StopCoroutineIfRunning(ref fadeOutEnumerator);

            if (myAudioSource.volume > toVolume)
            {
                StartNewCoroutine(ref fadeOutEnumerator, FadeOutCoroutine(toVolume, duration, stopSoundAtEndOfFade));
            }
            else
            {
                StartNewCoroutine(ref fadeInEnumerator, FadeInCoroutine(toVolume, duration, stopSoundAtEndOfFade));
            }

        }

        private IEnumerator OnSoundFinishedPlayingCoroutine(float delay, bool destroyWhenFinished)
        {
            yield return new WaitForSeconds(delay);

            InvokeEventIfBound(SoundFinishedPlayed, this);

            if (destroyWhenFinished)
            {
                InvokeEventIfBound(SoundObjectWillGetDestroyed, this);
                Destroy(gameObject);
            }
        }

        private IEnumerator FadeInCoroutine(float toVolume, float duration, bool stopSoundAtEndOfFade)
        {
            float startVolume = myAudioSource.volume;
            float startTime = Time.time;
            float alpha = 0.0f;
            while (alpha < 1.0f)
            {
                alpha = (Time.time - startTime) / duration;
                myAudioSource.volume = fadeCurve.Evaluate(alpha / (toVolume - startVolume));

                yield return new WaitForEndOfFrame();
            }

            myAudioSource.volume = toVolume;

            if (stopSoundAtEndOfFade)
            {
                myAudioSource.Stop();
            }
        }

        private IEnumerator FadeOutCoroutine(float toVolume, float duration, bool stopSoundAtEndOfFade)
        {
            float startVolume = myAudioSource.volume;
            float startTime = Time.time;
            float alpha = 0.0f;
            while (alpha < 1.0f)
            {
                alpha = (Time.time - startTime) / duration;
                myAudioSource.volume = fadeCurve.Evaluate((1.0f - alpha) / (toVolume - startVolume));

                yield return new WaitForEndOfFrame();
            }

            myAudioSource.volume = toVolume;

            if (stopSoundAtEndOfFade)
            {
                myAudioSource.Stop();
            }
        }

    }
}
