using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BNJMO
{
    public class SoundObject : BBehaviour
    {
        #region Public Events

        public event Action<SoundObject> SoundFinishedPlayed;
        public event Action<SoundObject> SoundObjectWillGetDestroyed;
        
        #endregion

        #region Public Methods

        public void InitSoundData(SoundData soundData)
        {
            if (IS_NOT_VALID(myAudioSource, true)
                || IS_NULL(soundData, true))
                return;
            
            AudioClip = null;
            if (soundData.RandomClips.Length > 0)
            {
                AudioClip = BUtils.GetRandomElement(soundData.RandomClips);
            }
            else
            {
                AudioClip = soundData.Clip;
            }
            
            if (IS_NULL(AudioClip, true))
            {
                LogConsoleWarning($"No valid clip found sound data {soundData.Name}.");
            }
            myAudioSource.clip = AudioClip;
            myAudioSource.outputAudioMixerGroup = soundData.AudioMixerGroup;
            myAudioSource.volume = Random.Range(soundData.MinVolume, soundData.MaxVolume);
            myAudioSource.pitch = Random.Range(soundData.MinPitch, soundData.MaxPitch);
            myAudioSource.spatialBlend = soundData.SpatialBlend;
            myAudioSource.transform.position = soundData.AtTransform ? soundData.AtTransform.position : soundData.AtPosition;
        }

        public void PlaySound(SoundData soundData)
        {
            if (IS_NOT_VALID(myAudioSource, true)
                || IS_NULL(soundData, true))
                return;

            InitSoundData(soundData);

            if (IS_NOT_VALID(myAudioSource.clip, true))
                return;

            Wait(soundData.Delay, () =>
            {
                // Play sound
                myAudioSource.Play();

                // Callback for when sound finished playing
                StartNewCoroutine(ref onSoundFinishedPlayinEnumerator,
                    OnSoundFinishedPlayingCoroutine(myAudioSource.clip.length, DestroySoundWhenFinishedPlaying));
            });
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
                myAudioSource.clip = AudioClip;
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

        public void PlaySound()
        {
            if (IS_NOT_VALID(myAudioSource, true))
                return;
            
            myAudioSource.Play();
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
            if (IS_NOT_VALID(myAudioSource, true)
                || duration <= 0.0f)
                return;

            StopCoroutineIfRunning(ref controlVolumeFromCurveEnumerator);
            StartNewCoroutine(ref fadeEnumerator, FadeCoroutine(toVolume, duration, stopSoundAtEndOfFade));
        }
        
        public void ControlVolumeFromCurve(AnimationCurve volumeCurve, float duration = 1.0f, bool stopSoundAtEndOfFade = false)
        {
            if (IS_NOT_VALID(myAudioSource, true)
                || duration <= 0.0f)
                return;

            StopCoroutineIfRunning(ref fadeEnumerator);
            StartNewCoroutine(ref controlVolumeFromCurveEnumerator, 
                ControlVolumeFromCurveCoroutine(volumeCurve, duration, stopSoundAtEndOfFade));
        }

        #endregion

        #region Inspector Variables
        
        [Header("Sound Object")]
        [SerializeField]
        private AudioSource myAudioSource;
        
        [SerializeField]
        private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField]
        private bool destroySoundWhenFinishedPlaying = false;

        #endregion

        #region Variables

        public AudioSource AudioSource => myAudioSource;
        public AudioClip AudioClip { get; private set; }
        public bool DestroySoundWhenFinishedPlaying { get { return destroySoundWhenFinishedPlaying; } set { destroySoundWhenFinishedPlaying = value; } }

        private IEnumerator onSoundFinishedPlayinEnumerator;
        private IEnumerator fadeEnumerator;
        private IEnumerator controlVolumeFromCurveEnumerator;

        #endregion

        #region Life Cycle

        protected override void OnValidate()
        {
            if (!CanValidate()) return;
            base.OnValidate();

            SetComponentIfNull(ref myAudioSource);
            if (!myAudioSource)
            {
                myAudioSource = gameObject.AddComponent<AudioSource>();
            }

            if (!AudioClip)
            {
                AudioClip = myAudioSource.clip;
            }
        }

        #endregion

        #region Events Callbacks


        #endregion

        #region Others

        
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

        private IEnumerator FadeCoroutine(float toVolume, float duration, bool stopSoundAtEndOfFade)
        {
            float startVolume = myAudioSource.volume;
            float startTime = Time.time;
            float alpha = 0.0f;
            while (alpha < 1.0f)
            {
                alpha = (Time.time - startTime) / duration;
                myAudioSource.volume = Mathf.Lerp(startVolume, toVolume, fadeCurve.Evaluate(alpha));

                yield return new WaitForEndOfFrame();
            }

            myAudioSource.volume = toVolume;

            if (stopSoundAtEndOfFade)
            {
                myAudioSource.Stop();
            }
        }

        private IEnumerator ControlVolumeFromCurveCoroutine(AnimationCurve volumeCurve, float duration, bool stopSoundAtEndOfFade)
        {
            var startTime = Time.time;
            var alpha = 0.0f;
            var startVolume = myAudioSource.volume;
            while (alpha < 1.0f)
            {
                alpha = (Time.time - startTime) / duration;
                myAudioSource.volume = volumeCurve.Evaluate(alpha) * startVolume;
                yield return new WaitForEndOfFrame();
            }
            myAudioSource.volume = volumeCurve.Evaluate(1.0f) * startVolume;

            if (stopSoundAtEndOfFade)
            {
                myAudioSource.Stop();
            }
        }

        #endregion
    }
}
