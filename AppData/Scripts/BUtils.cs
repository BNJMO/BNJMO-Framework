using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BNJMO
{
    /// <summary>
    /// Contains utility functions.
    /// </summary>
    public static class BUtils
    {

        #region General
        /// <summary> Gets a random index for a given array </summary>
        public static int GetRandomIndex(int arrayLength)
        {
            return UnityEngine.Random.Range(0, arrayLength);
        }

        /// <summary> Gets a random element from the given array </summary>
        public static A GetRandomElement<A>(A[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary> Gets a random element from the given list </summary>
        public static A GetRandomElement<A>(List<A> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary> Gets a random rotation over Y axis. Can be used to get a random orientation for a gived character. </summary>
        public static Quaternion GetRndStandRotation()
        {
            return Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(0, 360), 0));
        }

        public static void TransformAxisToCamera(ref float axis_X, ref float axis_Z, Vector3 cameraForwardDirection, bool isRotation = false)
        {
            Vector2 coordinateForward = new Vector2(0.0f, 1.0f);
            Vector2 coordinateRight = new Vector2(1.0f, 0.0f);
            Vector2 cameraForward = new Vector2(cameraForwardDirection.normalized.x, cameraForwardDirection.normalized.z).normalized;
            Vector2 controllerAxis = new Vector2(axis_X, axis_Z).normalized;
            float dotWithRight = Vector2.Dot(coordinateRight, cameraForward);
            int sign;
            if (dotWithRight > 0.0f)
            {
                sign = -1;
            }
            else if (dotWithRight < 0.0f)
            {
                sign = 1;
            }
            else
            {
                sign = 0;
            }
            if (isRotation == true)
            {
                sign *= -1;
            }

            float angle = Mathf.Acos(Vector2.Dot(coordinateForward, cameraForward)) * sign;

            axis_Z = controllerAxis.y * Mathf.Cos(angle) + controllerAxis.x * Mathf.Sin(angle);
            axis_X = controllerAxis.x * Mathf.Cos(angle) - controllerAxis.y * Mathf.Sin(angle);
            controllerAxis = new Vector2(axis_X, axis_Z).normalized;

            axis_X = controllerAxis.x;
            axis_Z = controllerAxis.y;
        }

        public static Vector2 Get2DVector(Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        public static Vector3 Get3DVector(Vector2 vector2)
        {
            return new Vector3(vector2.x, 0.0f, vector2.y);
        }

        public static Vector3 Get3DPlanearVector(Vector3 vector3)
        {
            return new Vector3(vector3.x, 0.0f, vector3.z);
        }

        public static Vector3 GetRandomVector(float min, float max)
        {
            return new Vector3(
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max),
                UnityEngine.Random.Range(min, max)
                );
        }

        public static float GetPlanarDistance(Vector3 from, Vector3 to)
        {
            Vector3 fromPlanar = Get3DPlanearVector(from);
            Vector3 toPlanar = Get3DPlanearVector(to);
            return Vector3.Distance(fromPlanar, toPlanar);
        }

        public static STransform GetTransformStruct(Transform transform)
        {
            STransform result = new STransform();
            result.Position = transform.position;
            result.Rotation = transform.rotation;
            result.Scale = transform.localScale;
            return result;
        }

        public static bool IsEditorPlaying()
        {
#if UNITY_EDITOR
            return EditorApplication.isPlaying;
#endif
            return false;
        }

        public static string GetTimeAsString()
        {
            return DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2") + ":" + DateTime.Now.Second + ":" + DateTime.Now.Millisecond.ToString("D3");
        }

        public static int GetTimeAsInt()
        {
            return DateTime.Now.Hour * 10000000 + DateTime.Now.Minute * 100000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
        }

        public static bool GetRandomBool()
        {
            int rndmInt = UnityEngine.Random.Range(0, 2);
            return rndmInt == 0 ? false : true;
        }

        public static int GetRandomSign()
        {
            int rndmInt = UnityEngine.Random.Range(0, 2);
            return rndmInt == 0 ? -1 : 1;
        }

        /// <summary> Returns True if the string contains ONLY Arabic letters or whitespace.</summary>
        public static bool StringIsOnlyArabic(string str)
        {
            bool result = !string.IsNullOrEmpty(str)
                && Regex.IsMatch(str, @"^[\u0600-\u06FF\u0750-\u077F\u08A0-\u08FF\s]+$");
            return result;
        }

        #endregion

        #region Get Component
        /// <summary>
        /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public static T GetComponentWithCheck<T>(GameObject gameObject, bool promtWarningMessageIfNoneFound = true)
        {
            T result = gameObject.GetComponent<T>();
            if ((promtWarningMessageIfNoneFound == true) && (result == null))
            {
                Debug.Log("Component of type <color=cyan>" + typeof(T) + "</color> not found on '" + gameObject.name + "'");
            }
            return result;
        }

        /// <summary>
        /// Looks for all components attached to the same gameobject of the given type and prompt an error message if none found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public static T[] GetComponentsWithCheck<T>(GameObject gameObject, bool promtWarningMessageIfNoneFound = true)
        {
            T[] result = gameObject.GetComponents<T>();
            if ((promtWarningMessageIfNoneFound == true) && (result.Length == 0))
            {
                Debug.Log("Not any component of type <color=cyan>" + typeof(T) + "</color> not found on '" + gameObject.name + "'");
            }
            return result;
        }

        /// <summary>
        /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="promtWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public static T GetComponentInHierarchy<T>(GameObject gameObject, bool promtWarningMessageIfNoneFound = true)
        {
            T result = gameObject.GetComponent<T>();
            if (result == null)
            {
                result = gameObject.GetComponentInChildren<T>();
                if (result == null)
                {
                    result = gameObject.GetComponentInParent<T>();
                }
            }

            if ((promtWarningMessageIfNoneFound == true) && (result == null))
            {
                Debug.Log("Component of type <color=cyan>" + typeof(T) + "</color> not found on '" + gameObject.name + "'");
            }
            return result;
        }

        /// <summary>
        /// Retrieves the hierarchical uppermost transform starting from this gameobject.
        /// </summary>
        /// <returns></returns>
        public static Transform GetUppermostParentTransform(GameObject gameObject)
        {
            Transform uppermostParentTransform = gameObject.transform;
            while (uppermostParentTransform.parent != null)
            {
                uppermostParentTransform = uppermostParentTransform.parent;
            }
            return uppermostParentTransform;
        }
        #endregion

        #region Debug

        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        public static void LogConsole(string logText, GameObject caller = null)
        {
            Debug.Log("<color=gray>[" + caller != null ? caller.name : "" + "]</color> "
                /*"<color=black>[" + GetType() + "]</color>"*/+ " " + logText);
        }

        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        public static void LogConsole(string logText, string color, GameObject caller = null)
        {
            Debug.Log("<color=gray>[" + caller != null ? caller.name : "" + "]</color> <color=" + color + ">" + logText + "</color>");
        }

        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        public static void LogConsoleRed(string logText, GameObject caller = null)
        {
            Debug.Log("<color=gray>[" + caller != null ? caller.name : "" + "]</color> <color=red>" + logText + "</color>");
        }

        /// <summary>
        /// Prints a warning log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        public static void LogConsoleWarning(string logText, GameObject caller = null)
        {
            Debug.Log("<color=yellow>WARNING! </color>" + "<color=gray>[" + caller != null ? caller.name : "" + "]</color> " + logText);

        }

        /// <summary>
        /// Prints an error log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        public static void LogConsoleError(string logText, GameObject caller = null)
        {
            Debug.Log("<color=red>ERROR! </color>" + "<color=gray>[" + caller != null ? caller.name : "" + "]</color> " + logText);
        }

        public static void DrawDebugArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction.magnitude > 0.0f)
            {
                Debug.DrawRay(pos, direction);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength);
                Debug.DrawRay(pos + direction, left * arrowHeadLength);
            }
        }

        public static void DrawDebugArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            if (direction.magnitude > 0.0f)
            {
                Debug.DrawRay(pos, direction, color);

                Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
                Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
                Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
            }
        }

#endregion

        #region Sound
        /// <summary>
        /// Plays a random clip from a given clip soundbank on a given AudioSource component
        /// </summary>
        public static void PlayRandomSound(AudioSource source, AudioClip[] clips)
        {
            if (clips.Length != 0)
            {
                if (source != null)
                {
                    source.clip = clips[GetRandomIndex(clips.Length)];
                    source.Play();
                }
                else
                {
                    Debug.LogWarning("No AudioSource attached!");
                }

            }
            else
            {
                Debug.LogWarning("No audio clip attached!");
            }
        }

        public static void PlaySound(AudioSource source, AudioClip clip)
        {
            if (source != null)
            {
                source.clip = clip;
                source.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource attached!");
            }
        }

        public static void StopSound(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
            }
            else
            {
                Debug.LogWarning("No AudioSource attached!");
            }
        }


        /* Audio Listeners */
        public static AudioSource AddAudioListener(GameObject toGameObject)
        {
            AudioSource aS = toGameObject.AddComponent<AudioSource>();
            aS.playOnAwake = false;

            return aS;
        }

        public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D)
        {
            AudioSource aS = toGameObject.AddComponent<AudioSource>();
            aS.playOnAwake = false;

            if (is3D == true)
            {
                aS.spatialBlend = 1.0f;
            }
            else
            {
                aS.spatialBlend = 0.0f;
            }
            return aS;
        }

        public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume)
        {
            AudioSource aS = toGameObject.AddComponent<AudioSource>();
            aS.playOnAwake = false;

            if (is3D == true)
            {
                aS.spatialBlend = 1.0f;
            }
            else
            {
                aS.spatialBlend = 0.0f;
            }
            aS.volume = volume;
            return aS;
        }

        public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume, bool isLoop)
        {
            AudioSource aS = toGameObject.AddComponent<AudioSource>();
            aS.playOnAwake = false;

            if (is3D == true)
            {
                aS.spatialBlend = 1.0f;
            }
            else
            {
                aS.spatialBlend = 0.0f;
            }
            aS.volume = volume;
            aS.loop = isLoop;
            return aS;
        }

        public static AudioSource AddAudioListener(GameObject toGameObject, bool is3D, float volume, bool isLoop, AudioMixerGroup audioMixerGroup)
        {
            AudioSource aS = toGameObject.AddComponent<AudioSource>();
            aS.playOnAwake = false;
            aS.outputAudioMixerGroup = audioMixerGroup;
            if (is3D == true)
            {
                aS.spatialBlend = 1.0f;
            }
            else
            {
                aS.spatialBlend = 0.0f;
            }
            aS.volume = volume;
            aS.loop = isLoop;
            return aS;
        }
#endregion

        #region Networking
        
        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            throw new System.Exception("No network adapters with an IPv4 address in the system!");
        }

        public const string EMAIL_PATTERN = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
        public const string USERNAME_PATTERN = @"^[a-zA-Z0-9!#$%&*?^+_~.,=-]{1,18}$";
        public const string USERNAME_PLAYER_PATTERN = "^(player_[0-9]{1,14})$";
        //private const string PASSWORD_PATTERN = "^(?=.*[A-Z])(?=.*[!@#$&*])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,20}$";
        public const string PASSWORD_PATTERN = @"^[a-zA-Z0-9!#$%&*?^+_~.,=-]{5,20}$";
        public const string RANDOM_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static bool IsEmail(string email)
        {
            if (email != null)
            {
                return Regex.IsMatch(email, EMAIL_PATTERN);
            }
            else
            {
                return false;
            }
        }

        public static bool IsUsername(string username)
        {
            if (username != null)
            {
                if(!Regex.IsMatch(username, USERNAME_PLAYER_PATTERN))
                {
                    return Regex.IsMatch(username, USERNAME_PATTERN);
                }
            }
            return false;
        }

        public static bool IsPassword(string password)
        {
            if (password != null)
            {
                return Regex.IsMatch(password, PASSWORD_PATTERN);
            }
            return false;
        }

        public static string GenerateRandom(int length)
        {
            System.Random r = new System.Random();
            return new string(System.Linq.Enumerable.Repeat(RANDOM_CHARS, length).Select(s => s[r.Next(s.Length)]).ToArray());
        }

        public static string Sha256FromString(string toEncrypt)
        {
            var message = Encoding.UTF8.GetBytes(toEncrypt);
            SHA256Managed hashString = new SHA256Managed();

            string hex = "";
            var hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }

            return hex;
        }

        /// <summary>
        /// Converts the system time from System.DateTime into an integer
        /// </summary>
        /// <returns> 3 first digits : milliseconds, 4th and 5th : seconds, 6th and 7th : minutes, 8th and 9th : hours </returns>
        public static int GetSystemTime()
        {
            return DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 100000 + DateTime.Now.Hour * 10000000;
        }

        #endregion

        #region Players
        
        public static SPlayerReplicationArg CreatePlayerReplicationArgFromPlayer(PlayerBase fromPlayer)
        {
            SPlayerReplicationArg replicationArg = new()
            {
                NetworkID = fromPlayer.NetworkID,
                OwnerControllerID = fromPlayer.ControllerID,
                OwnerControllerType = fromPlayer.ControllerType,
                PlayerID = fromPlayer.PlayerID,
                SpectatorID = fromPlayer.SpectatorID,
                TeamID = fromPlayer.TeamID,
                PlayerName = fromPlayer.PlayerName,
                PlayerPictureBase64 = BUtils.EncodeSprite(fromPlayer.PlayerPicture),
            };
            return replicationArg;
        }

        #endregion

        #region Types Conversions
        /// <summary> Convert a PlayerID enum to an int </summary>
        public static int GetIntFrom(EPlayerID playerID)
        {
            int result = 0;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = 1;
                    break;
                case EPlayerID.PLAYER_2:
                    result = 2;
                    break;
                case EPlayerID.PLAYER_3:
                    result = 3;
                    break;
                case EPlayerID.PLAYER_4:
                    result = 4;
                    break;
            }
            return result;
        }

        /// <summary> Convert an int to a PlayerID enum </summary>
        public static EPlayerID GetPlayerIDFrom(int playerID)
        {
            EPlayerID result = EPlayerID.NONE;
            switch (playerID)
            {
                case 1:
                    result = EPlayerID.PLAYER_1;
                    break;
                case 2:
                    result = EPlayerID.PLAYER_2;
                    break;
                case 3:
                    result = EPlayerID.PLAYER_3;
                    break;
                case 4:
                    result = EPlayerID.PLAYER_4;
                    break;
            }
            return result;
        }

        /// <summary> Convert a ControllerID enum to an Char </summary>
        public static char GetCharFrom(EControllerID ControllerID)
        {
            char result = 'X';
            switch (ControllerID)
            {
                case EControllerID.DEVICE_1:
                    result = 'A';
                    break;
                case EControllerID.DEVICE_2:
                    result = 'B';
                    break;
                case EControllerID.DEVICE_3:
                    result = 'C';
                    break;
                case EControllerID.DEVICE_4:
                    result = 'D';
                    break;
            }
            return result;
        }

        /// <summary> Convert a Char to a ControllerID enum </summary>
        public static EControllerID GetControllerIDFrom(char ControllerID)
        {
            EControllerID result = EControllerID.NONE;
            switch (ControllerID)
            {
                case 'A':
                    result = EControllerID.DEVICE_1;
                    break;
                case 'B':
                    result = EControllerID.DEVICE_2;
                    break;
                case 'C':
                    result = EControllerID.DEVICE_3;
                    break;
                case 'D':
                    result = EControllerID.DEVICE_4;
                    break;
            }
            return result;
        }

        /// <summary> Convert a joystickType enum to a char</summary>
        public static char GetCharFrom(EInputAxis inputAxis)
        {
            char resut = 'X';
            switch (inputAxis)
            {
                case EInputAxis.MOVEMENT:
                    resut = 'L';
                    break;

                case EInputAxis.ROTATION:
                    resut = 'R';
                    break;
            }
            return resut;
        }

        /// <summary> Returns the same team ID as the player ID (e.g. Player 2 -> Team 2)</summary>
        public static ETeamID GetIdenticPlayerTeam(EPlayerID playerID)
        {
            ETeamID result = ETeamID.NONE;
            switch (playerID)
            {
                case EPlayerID.PLAYER_1:
                    result = ETeamID.TEAM_1;
                    break;

                case EPlayerID.PLAYER_2:
                    result = ETeamID.TEAM_2;
                    break;

                case EPlayerID.PLAYER_3:
                    result = ETeamID.TEAM_3;
                    break;

                case EPlayerID.PLAYER_4:
                    result = ETeamID.TEAM_4;
                    break;
            }
            return result;
        }

        /// <summary> Converts a controllerID (AI or Network) to a PlayerID </summary>
        public static EPlayerID GetPlayerIDFrom(EControllerID controllerID)
        {
            EPlayerID result = EPlayerID.NONE;
            switch (controllerID)
            {
                /* AI */
                case EControllerID.AI_1:
                    result = EPlayerID.PLAYER_1;
                    break;

                case EControllerID.AI_2:
                    result = EPlayerID.PLAYER_2;
                    break;

                case EControllerID.AI_3:
                    result = EPlayerID.PLAYER_3;
                    break;

                case EControllerID.AI_4:
                    result = EPlayerID.PLAYER_4;
                    break;
            }
            return result;
        }
        
        public static EControllerID GetAIControllerIDFrom(int index)
        {
            switch(index)
            {
                case 1:
                    return EControllerID.AI_1;
                        
                case 2:
                    return EControllerID.AI_2;
                                 
                case 3:
                    return EControllerID.AI_3;
                        
                case 4:
                    return EControllerID.AI_4;

                default:
                    return EControllerID.NONE;
            }
        }
        
        public static ENetworkID GetNetworkIDFrom(EControllerID controllerID)
        {
            switch (controllerID)
            {
                case EControllerID.REMOTE_1:
                    return ENetworkID.HOST_1;

                case EControllerID.REMOTE_2:
                    return ENetworkID.CLIENT_2;

                case EControllerID.REMOTE_3:
                    return ENetworkID.CLIENT_3;

                case EControllerID.REMOTE_4:
                    return ENetworkID.CLIENT_4;

                case EControllerID.REMOTE_5:
                    return ENetworkID.CLIENT_5;

                case EControllerID.REMOTE_6:
                    return ENetworkID.CLIENT_6;

                case EControllerID.REMOTE_7:
                    return ENetworkID.CLIENT_7;

                case EControllerID.REMOTE_8:
                    return ENetworkID.CLIENT_8;

                case EControllerID.REMOTE_9:
                    return ENetworkID.CLIENT_9;

                case EControllerID.REMOTE_10:
                    return ENetworkID.CLIENT_10;

                case EControllerID.REMOTE_11:
                    return ENetworkID.CLIENT_11;

                case EControllerID.REMOTE_12:
                    return ENetworkID.CLIENT_12;

                case EControllerID.REMOTE_13:
                    return ENetworkID.CLIENT_13;
                
                case EControllerID.REMOTE_14:
                    return ENetworkID.CLIENT_14;
                
                case EControllerID.REMOTE_15:
                    return ENetworkID.CLIENT_15;
                
                case EControllerID.REMOTE_16:
                    return ENetworkID.CLIENT_16;

                default:
                    return ENetworkID.NONE;
            }
        }

        /// <summary> Converts an array with 3 elements to a Vector3 </summary>
        public static Vector3 GetVectorFrom(float[] vector)
        {
            if (vector.Length != 3)
            {
                Debug.LogError("parameter vector should contain exactly 3 elements");
                return new Vector3();
            }

            return new Vector3(vector[0], vector[1], vector[2]);
        }
        #endregion

        #region Serialization
        
        public static string SerializeObject(System.Object objectToSerialize)
        {
            //Debug.Log("SerializeObject : <color=red>[" + BUtils.GetTimeAsString() + "] </color>");

            string serializedObject = "";

            switch (BManager.Inst.Config.bEHandleSerializationMethod)
            {
                case EBEHandleSerializationMethod.JSON_NEWTONSOFT:
                    serializedObject = JsonConvert.SerializeObject(objectToSerialize, new JsonSerializerSettings()
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        Formatting = Formatting.Indented,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                    break;

                case EBEHandleSerializationMethod.JSON_UNITY:
                    serializedObject = JsonUtility.ToJson(objectToSerialize);
                    break;
            }

            //Debug.Log("SerializeObject - End : <color=red>[" + BUtils.GetTimeAsString() + "] </color>");
            return serializedObject;
        }

        public static H DeserializeObject<H>(string serializedObject)
        {
            //Debug.Log("DeserializeObject : <color=red>[" + BUtils.GetTimeAsString() + "] </color>");

            H deserializedObject = default(H);

            switch (BManager.Inst.Config.bEHandleSerializationMethod)
            {
                case EBEHandleSerializationMethod.JSON_NEWTONSOFT:
                    deserializedObject = JsonConvert.DeserializeObject<H>(serializedObject);

                    break;

                case EBEHandleSerializationMethod.JSON_UNITY:
                    deserializedObject = JsonUtility.FromJson<H>(serializedObject);
                    break;
            }

            //Debug.Log("DeserializeObject - End : <color=red>[" + BUtils.GetTimeAsString() + "] </color>");
            return deserializedObject;
        }

        public static string EncodeTexture2D(Texture source, bool asPng = true, int jpgQuality = 85)
        {
            if (!source) return "";

            int w = source.width;
            int h = source.height;

            // Fast path: readable + uncompressed formats supported by EncodeTo...
            if (source is Texture2D t2d && IsReadable(t2d) && IsEncodeSupported(t2d.format))
            {
                byte[] direct = asPng ? t2d.EncodeToPNG() : t2d.EncodeToJPG(jpgQuality);
                return Convert.ToBase64String(direct);
            }

            // Convert to uncompressed, readable RGBA32 first
            Texture2D readable = ToReadableRGBA32(source, w, h);

            try
            {
                byte[] data = asPng ? readable.EncodeToPNG() : readable.EncodeToJPG(jpgQuality);
                return Convert.ToBase64String(data);
            }
            finally
            {
                UnityEngine.Object.Destroy(readable);
            }
        }

        private static Texture2D ToReadableRGBA32(Texture src, int w, int h)
        {
            // Try GPU conversion first (works for compressed, non-readable GPU textures)
            if (SystemInfo.copyTextureSupport != CopyTextureSupport.None)
            {
                try
                {
                    var dst = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
                    // Correct overloads:
                    // Graphics.ConvertTexture(src, dst);
                    // or Graphics.ConvertTexture(src, 0, dst, 0);
                    Graphics.ConvertTexture(src, dst);
                    return dst;
                }
                catch
                {
                    // fall through to RT path
                }
            }

            // Universal fallback: Blit to RT + ReadPixels
            RenderTexture rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
            var prev = RenderTexture.active;
            try
            {
                Graphics.Blit(src, rt);
                RenderTexture.active = rt;

                var readable = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
                readable.ReadPixels(new Rect(0, 0, w, h), 0, 0);
                readable.Apply(false, false);
                return readable;
            }
            finally
            {
                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);
            }
        }

        private static bool IsReadable(Texture2D tex)
        {
            try { var _ = tex.GetRawTextureData<byte>(); return true; }
            catch { return false; }
        }

        private static bool IsEncodeSupported(TextureFormat f) =>
            f == TextureFormat.RGBA32 ||
            f == TextureFormat.ARGB32 ||
            f == TextureFormat.BGRA32 ||
            f == TextureFormat.RGB24 ||
            f == TextureFormat.Alpha8 ||
            f == TextureFormat.R8;
        
        public static Texture2D DecodeTexture2D(string textureBase64)
        {
            if (textureBase64 == "")
                return null;
            
            byte[] imageBytes = Convert.FromBase64String(textureBase64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            return texture;
        }

        public static string EncodeSprite(Sprite sprite)
        {
            if (!sprite)
                return "";
            
            Texture2D texture2D = sprite.texture;
            return EncodeTexture2D(texture2D);
        }
        
        public static Sprite DecodeSprite(string textureBase64)
        {
            if (textureBase64 == "")
                return null;
            
            Texture2D texture2D = DecodeTexture2D(textureBase64);
            Sprite mySprite = Sprite.Create(
                texture2D,
                new Rect(0, 0, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f)
                );
            return mySprite;
        }

        #endregion
        
        #region Input
        
        public static bool IsControllerIDTouch(EControllerID controllerID)
        {
            return controllerID.ContainedIn(BConsts.TOUCH_CONTROLLERS);
        }     
        
        public static bool IsControllerIDAI(EControllerID controllerID)
        {
            return controllerID.ContainedIn(BConsts.AI_CONTROLLERS);
        } 
        
        public static bool IsControllerIDDevice(EControllerID controllerID)
        {
            return controllerID.ContainedIn(BConsts.DEVICE_CONTROLLERS);
        }   
        
        public static bool IsControllerIDRemote(EControllerID controllerID)
        {
            return controllerID.ContainedIn(BConsts.REMOTE_CONTROLLERS);
        }
        
        #endregion
    }
}