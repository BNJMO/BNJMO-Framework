using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNJMO
{
    public abstract class AbstractGameSave<T> : AbstractSingletonManager<AbstractGameSave<T>> where T : Enum
    {
        #region Public Events


        #endregion

        #region Public Methods

        public int GetSavedInt(T saveName, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(saveName.ToString(), defaultValue);
        }

        public void SaveInt(T saveName, int value)
        {
            PlayerPrefs.SetInt(saveName.ToString(), value);
        }

        public float GetSavedFloat(T saveName, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(saveName.ToString(), defaultValue);
        }

        public void SaveFloat(T saveName, float value)
        {
            PlayerPrefs.SetFloat(saveName.ToString(), value);
        }

        public string GetSavedString(T saveName, string defaultValue = "")
        {
            return PlayerPrefs.GetString(saveName.ToString(), defaultValue);
        }

        public void SaveString(T saveName, string value)
        {
            PlayerPrefs.SetString(saveName.ToString(), value);
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
