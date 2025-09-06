using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BAudioManager : AbstractSingletonManager<BAudioManager>
    {
        #region Public Events


        #endregion

        #region Public Methods

        public static SoundObject SpawnSoundObject(SoundData soundData, bool destroyWhenFinished = true)
        {
            SoundObject soundObjectPrefab = Resources.Load<SoundObject>(BConsts.PATH_SoundObject);

            if (soundData == null
                || (soundData.Clip == null && soundData.RandomClips.Length == 0)
                || soundObjectPrefab == null)
                return null;
            
            SoundObject soundObject = Instantiate(soundObjectPrefab);
            string soundName = "Sound";
            if (soundData.Name != "")
            {
                soundName = soundData.Name;
            }
            else if (soundData.Clip != null)
            {
                soundName = soundData.Clip.name;
            }
            soundObject.gameObject.name = "SO_" + soundName;
            soundObject.DestroySoundWhenFinishedPlaying = destroyWhenFinished;
            soundObject.PlaySound(soundData);
            return soundObject;
        }
        
        public static SoundObject SpawnSoundObject(AudioClip audioClipToPlay, bool destroyWhenFinished = true)
        {
            SoundObject soundObjectPrefab = Resources.Load<SoundObject>(BConsts.PATH_SoundObject);

            if (audioClipToPlay != null
                && soundObjectPrefab)
            {
                SoundObject soundObject = Instantiate(soundObjectPrefab);
                soundObject.gameObject.name = "SO_" + audioClipToPlay.name;
                soundObject.DestroySoundWhenFinishedPlaying = destroyWhenFinished;
                soundObject.PlaySound(audioClipToPlay);
                return soundObject;
            }
            return null;
        }

        public static SoundObject SpawnSoundObject(AudioClip[] audioClipsToPlayFrom, bool destroyWhenFinished = true)
        {
            AudioClip audioClipToPlay = BUtils.GetRandomElement(audioClipsToPlayFrom);
            SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
            if (soundObject)
            {
                soundObject.DestroySoundWhenFinishedPlaying = destroyWhenFinished;
                soundObject.PlaySound(audioClipToPlay);
            }
            return soundObject;
        }

        public static SoundObject Spawn3DSoundObject(Vector3 position, AudioClip audioClipToPlay, bool destroyWhenFinished = true)
        {
            SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
            if (soundObject)
            {
                soundObject.DestroySoundWhenFinishedPlaying = destroyWhenFinished;
                soundObject.Play3DSound(position, audioClipToPlay);
            }
            return soundObject;
        }

        public static SoundObject Spawn3DSoundObject(Transform transform, AudioClip audioClipToPlay, bool destroyWhenFinished = true)
        {
            SoundObject soundObject = SpawnSoundObject(audioClipToPlay);
            if (soundObject)
            {
                soundObject.DestroySoundWhenFinishedPlaying = destroyWhenFinished;
                soundObject.Play3DSound(transform, audioClipToPlay);
            }
            return soundObject;
        }


        #endregion

        #region Inspector Variables


        #endregion

        #region Variables


        #endregion

        #region Life Cycle


        #endregion

        #region Events Callbacks


        #endregion

        #region Others


        #endregion
        
        


  

    }
}