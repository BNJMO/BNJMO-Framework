using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public class BAudioManager : AbstractSingletonManager<BAudioManager>
    {
        private List<SoundObject> aliveSoundObjects = new List<SoundObject>();
        private SoundObject soundObjectPrefab;

        protected override void Awake()
        {
            base.Awake();

            soundObjectPrefab = Resources.Load<SoundObject>(BConsts.PATH_SoundObject);
            IS_NOT_NULL(soundObjectPrefab);
        }

        public static SoundObject SpawnSoundObject(AudioClip audioClipToPlay, bool destroyWhenFinished = true)
        {
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

        //private SoundObject SpawnSoundObject(AudioClip audioClipToPlay)
        //{
        //    if ((audioClipToPlay != null)
        //        && (IS_NOT_NULL(soundObjectPrefab)))
        //    {
        //        SoundObject soundObject = Instantiate(soundObjectPrefab);
        //        soundObject.gameObject.name = "SO_" + audioClipToPlay.name;
        //        aliveSoundObjects.Add(soundObject);
        //        soundObject.SoundObjectWillGetDestroyed += On_SoundObject_SoundObjectWillGetDestroyed;
        //        return soundObject;
        //    }
        //    return null;
        //}

        private static SoundObject SpawnSoundObject(AudioClip audioClipToPlay)
        {
            SoundObject soundObjectPrefab = Resources.Load<SoundObject>(BConsts.PATH_SoundObject);

            if ((audioClipToPlay != null)
                && ((soundObjectPrefab)))
            {
                SoundObject soundObject = Instantiate(soundObjectPrefab);
                soundObject.gameObject.name = "SO_" + audioClipToPlay.name;
                return soundObject;
            }
            return null;
        }

        private void On_SoundObject_SoundObjectWillGetDestroyed(SoundObject soundObject)
        {
            if (IS_VALUE_CONTAINED(aliveSoundObjects, soundObject))
            {
                aliveSoundObjects.Remove(soundObject);
            }
        }
    }
}