﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace BNJMO
{
    /// <summary>
    /// Multiple channel (IDs) debug messages on every frame
    /// IDs:
    /// - AppScene
    /// - AppState
    /// - UIState
    /// - TestGameMode
    /// - AnimationLerp
    /// - NetControllerInputSource
    /// - ConnectedControllers
    /// - JoinedPlayers
    /// - MirrorPlayers
    /// </summary>
    public class DebugManager : AbstractSingletonManager<DebugManager>
    {
        private Dictionary<string, DebugText> debugTexts = new Dictionary<string, DebugText>();
        private Dictionary<string, bool> reportedDebugTexts = new Dictionary<string, bool>();
        private bool debugTextsInitialized = false;

        private NotifcationWindow notifcationWindow;

        protected override void OnNewSceneReinitialize(EAppScene newScene, EAppScene lastScene)
        {
            base.OnNewSceneReinitialize(newScene, lastScene);

            ReinitializeDebugTexts();

            notifcationWindow = FindObjectOfType<NotifcationWindow>();
        }

        private void ReinitializeDebugTexts()
        {
            debugTexts.Clear();
            reportedDebugTexts.Clear();

            DebugText[] foundDebugTexts = FindObjectsOfType<DebugText>();
            foreach (DebugText foundDebugText in foundDebugTexts)
            {
                if (debugTexts.ContainsKey(foundDebugText.DebugID) == false)
                {
                    debugTexts.Add(foundDebugText.DebugID, foundDebugText);
                }
                else
                {
                    LogConsoleWarning("A DebugText component with ID <color=gray>" + foundDebugText.DebugID + "</color> has already been found in this scene!");
                }
            }

            debugTextsInitialized = true;
        }

        /// <summary>
        /// Writes a text during this frame on the DebugText in this scene with the given ID. 
        /// Written text is erased on the next frame!
        /// </summary>
        public void DebugLogCanvas(string debugID, string logText)
        {
            if (debugTextsInitialized == true)
            {
                if (debugTexts.ContainsKey(debugID) == true)
                {
                    debugTexts[debugID].Log(logText);
                }
                else
                {
                    if (reportedDebugTexts.ContainsKey(debugID) == false)
                    {
                        reportedDebugTexts.Add(debugID, true);
                        if (BManager.Inst.Config.DebugButtonEvents)
                        {
                            LogConsoleWarning("Debug text with ID <color=gray>" + debugID + "</color> not found in this scene!");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a text during this frame on the DebugText in this scene with the default ID "DebugID". 
        /// Written text is erased on the next frame!
        /// </summary>
        public void DebugLogCanvas(string logText)
        {
            DebugLogCanvas("DebugID", logText);
        }

        public void DebugLogNotification(string logText)
        {
            if (notifcationWindow)
            {
                notifcationWindow.Notify(logText);
            }
            else
            {
                LogConsoleWarning("Trying to use the Notification Window but none found in the scene!");
            }
        }

    }
}