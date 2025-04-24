#define B_VUFORIA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
#endif

namespace BNJMO
{
    /// <summary>
    /// A stack on the MonoBehaviour that defines some useful utilities functions
    /// Copyright BNJMO
    /// </summary>
    // [SelectionBase] // Ensures this object is selected in the editor instead of one of its children (e.g. mesh)
    public abstract class BBehaviour : MonoBehaviour
    {
#region Life Cycle
        public virtual void Revalidate()
        {
            OnValidate();
        }

        /// <summary>
        /// Include this before your code:
        /// if (!CanValidate()) return;
        /// </summary>
        protected virtual void OnValidate()
        {
            
        }

        protected virtual void Awake()
        {
            InitializeComponents();
            InitializeObjecsInScene();

        }

        /// <summary>
        /// Use this to find components attached to the same gameobject and in order to assign references or bind them to callbacks (actually called in Awake)
        /// </summary>
        protected virtual void InitializeComponents()
        {

        }

        /// <summary>
        /// Use this to find objects in the scene in order to assign references or bind them to callbacks (actually called in Awake)
        /// </summary>
        protected virtual void InitializeObjecsInScene()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Start()
        {
            InitializeEventsCallbacks();

            StartCoroutine(LateStartCoroutine());
        }

        /// <summary>
        /// Use this method to listen to events and add their callbacks here (actually called in Start)
        /// </summary>
        protected virtual void InitializeEventsCallbacks()
        {

        }

        protected virtual void LateStart()
        {

        }

        private IEnumerator LateStartCoroutine()
        {
            yield return new WaitForEndOfFrame();

            LateStart();
        }

        protected virtual void Update()
        {
            UpdateDebugText();
        }

        protected virtual void UpdateDebugText()
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        //protected virtual void OnGUI()
        //{

        //}

        //protected virtual void OnDrawGizmos()
        //{

        //}
#endregion

#region Debug Log
        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        protected void LogConsole(string logText)
        {
            Debug.Log("<color=white>[" + name + "]</color> "
                /*"<color=black>[" + GetType() + "]</color>"*/+ " " + logText);
        }

        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        protected void LogConsole(string logText, string color)
        {
            Debug.Log("<color=white>[" + name + "]</color> <color=" + color + ">" + logText + "</color>");
        }
                
        /// <summary>
        /// Prints a log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        protected void LogConsoleRed(string logText)
        {
            Debug.Log("<color=white>[" + name + "]</color> <color=red>" + logText + "</color>");
        }

        /// <summary>
        /// Prints a warning log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        protected void LogConsoleWarning(string logText)
        {
            Debug.Log("<color=yellow>WARNING! </color>" + "<color=gray>[" + name + "]</color> " + logText);

        }

        /// <summary>
        /// Prints an error log text into the console if logging is enabled and the category of the text to log is not already added into the ignore list.
        /// </summary>
        /// <param name="logText"> Log text to print </param>
        /// <param name="category"> Category of the log text </param>
        protected void LogConsoleError(string logText)
        {
            Debug.Log("<color=red>ERROR! </color>" + "<color=gray>[" + name + "]</color> " + logText);
        }

        /// <summary>
        /// Prints a log text into a Debug Text component on the canvas in the scene, with a matching debug ID
        /// </summary>
        /// <param name="debugID"> Debug ID of the Debug Text component </param>
        /// <param name="logText"> Log text to print </param>
        protected void LogCanvas(string debugID, string logText)
        {
            if (DebugManager.IsInstanceSet)
            {
                DebugManager.Instance.DebugLogCanvas(debugID, logText);
            }
        }

        protected void LogNotification(string logText)
        {
            if (DebugManager.IsInstanceSet)
            {
                DebugManager.Instance.DebugLogNotification(logText);
            }
        }

        protected void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
        }

        #endregion

        #region Checkers
        /// <summary>
        /// Checks if the the given Unity object's reference is not null.
        /// Prints a warning in the console if null.
        /// </summary>
        /// <typeparam name="O"> Type of the object to check</typeparam>
        /// <param name="objectToCheck"> object to check </param>
        /// <returns> True if the given object has a valid reference, otherwise false</returns>
        protected bool IS_VALID<O>(O objectToCheck, bool reverseLogCondition = false) where O : UnityEngine.Object
        {
            if (objectToCheck)
            {
                if (reverseLogCondition)
                    LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is valid! ");

                return true;
            }

            if (!reverseLogCondition)
                LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is not valid! ");
            return false;
        }

        /// <summary>
        /// Checks if the the given Unity object's reference is not valid.
        /// Prints a warning in the console if not null.
        /// </summary>
        /// <typeparam name="O"> Type of the object to check</typeparam>
        /// <param name="objectToCheck"> object to check </param>
        /// <returns> True if the given object has a valid reference, otherwise false</returns>
        protected bool IS_NOT_VALID<O>(O objectToCheck, bool reverseLogCondition = false) where O : UnityEngine.Object
        {
            if (objectToCheck)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is valid!");

                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is not valid!");

            return true;
        }

        /// <summary>
        /// Checks if the the given object's reference is null.
        /// Prints a warning in the console if not null.
        /// </summary>
        /// <typeparam name="O"> Type of the object to check</typeparam>
        /// <param name="objectToCheck"> object to check </param>
        /// <returns> True if the given object has a valid reference, otherwise false</returns>
        protected bool IS_NULL<O>(O objectToCheck, bool reverseLogCondition = false)
        {
            if (objectToCheck != null)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> isn't null! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is null! ");

            return true;
        }

        /// <summary>
        /// Checks if the the given object's reference is not null.
        /// Prints a warning in the console if null.
        /// </summary>
        /// <typeparam name="O"> Type of the object to check</typeparam>
        /// <param name="objectToCheck"> object to check </param>
        /// <returns> True if the given object has a valid reference, otherwise false</returns>
        protected bool IS_NOT_NULL<O>(O objectToCheck, bool reverseLogCondition = false)
        {
            if (objectToCheck == null)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is null!");

                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("An object of type <color=cyan>" + typeof(O) + "</color> is not null!");

            return true;
        }

        /// <summary>
        /// Checks if the the given boolean is true
        /// Prints a warning in the console if not True.
        /// </summary>
        /// <param name="value"> boolean to check</param>
        /// <returns> True if the given boolean is true, otherwise false</returns>
        protected bool IS_TRUE(bool booleanToCheck)
        {
            if (booleanToCheck == false)
            {
                LogConsoleWarning("A boolean is false! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the the given boolean is true
        /// Prints a warning in the console if not True.
        /// </summary>
        /// <param name="value"> boolean to check</param>
        /// <returns> True if the given boolean is true, otherwise false</returns>
        protected bool IS_NOT_TRUE(bool booleanToCheck, bool reverseLogCondition = false)
        {
            if (booleanToCheck == true)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("A boolean is true! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("A boolean is not true! ");

            return true;
        }

        /// <summary>
        /// Checks if the given two variable are equal.
        /// Prints a warning in the console the variables are not equal.
        /// Note: To compare enums, please use "ARE_ENUMS_NOT_EQUAL"!
        /// </summary>
        /// <typeparam name="O"> Type of the varaibles </typeparam>
        /// <param name="variable1"> First variable </param>
        /// <param name="variable2"> Second variable </param>
        /// <returns></returns>
        protected bool ARE_EQUAL<O>(O variable1, O variable2, bool reverseLogCondition = false)
        {
            if (variable1 != null
                && variable1.Equals(variable2) == false)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("Two variables of type <color=cyan>" + typeof(O) + "</color> are not equal! " + variable1 + " - " + variable2);
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("Two variables of type <color=cyan>" + typeof(O) + "</color> are equal! " + variable1 + " - " + variable2);

            return true;
        }

        /// <summary>
        /// Checks if the given two variable are not equal.
        /// Prints a warning in the console the variables are equal.
        /// Note: To compare enums, please use "ARE_ENUMS_NOT_EQUAL"!
        /// </summary>
        /// <typeparam name="O"> Type of the varaibles </typeparam>
        /// <param name="variable1"> First variable </param>
        /// <param name="variable2"> Second variable </param>
        /// <returns></returns>
        protected bool ARE_NOT_EQUAL<O>(O variable1, O variable2, bool reverseLogCondition = false)
        {
            if (variable1 != null
                && variable1.Equals(variable2) == true)
            {

                if (!reverseLogCondition)
                    LogConsoleWarning("Two variables of type <color=cyan>" + typeof(O) + "</color> are equal! ");

                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("Two variables of type <color=cyan>" + typeof(O) + "</color> are not equal! ");

            return true;
        }

        /// <summary>
        /// Checks if the given enum has the value NONE. 
        /// Check is performed by a simple string conversion!
        /// Prints a warning in the console if not NONE.
        /// </summary>
        /// <typeparam name="E"> Type of the enum to check </typeparam>
        /// <param name="enumToCheck"> enum to check </param>
        /// <returns></returns>
        protected bool IS_NONE<E>(E enumToCheck, bool reverseLogCondition = false) where E : Enum
        {
            if (enumToCheck.ToString() != "NONE")
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> isn't NONE! ");
                
                return false;
            }
            
            if (reverseLogCondition)
                LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> is NONE! ");

            return true;
        }

        /// <summary>
        /// Checks if the given enum has NOT the value NONE. 
        /// Check is performed by a simple string conversion!
        /// Prints a warning in the console if NONE.
        /// </summary>
        /// <typeparam name="E"> Type of the enum to check </typeparam>
        /// <param name="enumToCheck"> enum to check </param>
        /// <returns></returns>
        protected bool IS_NOT_NONE<E>(E enumToCheck, bool reverseLogCondition = false) where E : Enum
        {
            if (enumToCheck.ToString() == "NONE")
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> is NONE! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("An enum of type <color=cyan>" + typeof(E) + "</color> is not NONE! ");

            return true;
        }

        /// <summary>
        /// Checks if the 2 given enums are equal.
        /// Prints a warning in the console if different.
        /// </summary>
        /// <typeparam name="E"> Type of the enum to check </typeparam>
        /// <param name="enumToCheck1"> first enum to check </param>
        /// <param name="enumToCheck2"> second enum to check with </param>
        /// <returns></returns>
        protected bool ARE_ENUMS_EQUAL<E>(E enumToCheck1, E enumToCheck2) where E : Enum
        {
            if (enumToCheck1.ToString().Equals(enumToCheck2.ToString()) == false)
            {
                LogConsoleWarning("Two enums of type <color=cyan>" + typeof(E) + "</color> are different! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the 2 given enums are not equal.
        /// Prints a warning in the console if not different.
        /// </summary>
        /// <typeparam name="E"> Type of the enum to check </typeparam>
        /// <param name="enumToCheck1"> first enum to check </param>
        /// <param name="enumToCheck2"> second enum to check with </param>
        /// <returns></returns>
        protected bool ARE_ENUMS_NOT_EQUAL<E>(E enumToCheck1, E enumToCheck2, bool reverseLogCondition = false) where E : Enum
        {
            if (enumToCheck1.ToString().Equals(enumToCheck2.ToString()) == true)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("Two enums of type <color=cyan>" + typeof(E) + "</color> are NOT different! ");

                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("Two enums of type <color=cyan>" + typeof(E) + "</color> are different! ");

            return true;
        }

        /// <summary>
        /// Checks if the given dictionary contains the given key.
        /// Prints a warning in the console if key is not found.
        /// </summary>
        /// <typeparam name="K"> type of the key </typeparam>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="dictionary"> dictionary to check into </param>
        /// <param name="key"> key to check inside the dictionary </param>
        /// <returns></returns>
        protected bool IS_KEY_CONTAINED<K, V>(Dictionary<K, V> dictionary, K key, bool reverseLogCondition = false)
        {
            if (dictionary.ContainsKey(key) == false)
            {

                if (!reverseLogCondition)
                    LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> doens't contain the key '" + key + "' ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> does contain the key '" + key + "' ! ");

            return true;
        }

        /// <summary>
        /// Checks if the given dictionary doesn't contains the given key.
        /// Prints a warning in the console if key is found.
        /// </summary>
        /// <typeparam name="K"> type of the key </typeparam>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="dictionary"> dictionary to check into </param>
        /// <param name="key"> key to check inside the dictionary </param>
        /// <returns></returns>
        protected bool IS_KEY_NOT_CONTAINED<K, V>(Dictionary<K, V> dictionary, K key, bool reverseLogCondition = false)
        {
            if (dictionary.ContainsKey(key) == true)
            {

                if (!reverseLogCondition)
                    LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> does already contain the key '" + key + "' ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("A dictionary with keys type <color=cyan>" + typeof(K) + "</color> does not contain the key '" + key + "' ! ");

            return true;
        }

        /// <summary>
        /// Checks if the given dictionary contains the given value.
        /// Prints a warning in the console if value is not found.
        /// </summary>
        /// <typeparam name="K"> type of the key </typeparam>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="dictionary"> dictionary to check into </param>
        /// <param name="value"> value to check inside the dictionary </param>
        /// <returns></returns>
        protected bool IS_VALUE_CONTAINED<K, V>(Dictionary<K, V> dictionary, V value)
        {
            if (dictionary.ContainsValue(value) == false)
            {
                LogConsoleWarning("A dictionary with values type <color=cyan>" + typeof(V) + "</color> doens't contain the value '" + value + "' ! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given dictionary doesn't contain the given value.
        /// Prints a warning in the console if value is found.
        /// </summary>
        /// <typeparam name="K"> type of the key </typeparam>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="dictionary"> dictionary to check into </param>
        /// <param name="value"> value to check inside the dictionary </param>
        /// <returns></returns>
        protected bool IS_VALUE_NOT_CONTAINED<K, V>(Dictionary<K, V> dictionary, V value)
        {
            if (dictionary.ContainsValue(value) == true)
            {
                LogConsoleWarning("A dictionary with values type <color=cyan>" + typeof(V) + "</color> does contain already the value '" + value + "' ! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given list contains the given value.
        /// Prints a warning in the console if value is not found.
        /// </summary>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="list"> list to check into </param>
        /// <param name="value"> value to check inside the list </param>
        /// <returns></returns>
        protected bool IS_VALUE_CONTAINED<V>(List<V> list, V value)
        {
            if (list.Contains(value) == false)
            {
                LogConsoleWarning("A list with values type <color=cyan>" + typeof(V) + "</color> doens't contain the value '" + value + "' ! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given list doesn't contain the given value.
        /// Prints a warning in the console if value is found.
        /// </summary>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="list"> list to check into </param>
        /// <param name="value"> value to check inside the list </param>
        /// <returns></returns>
        protected bool IS_VALUE_NOT_CONTAINED<V>(List<V> list, V value)
        {
            if (list.Contains(value) == true)
            {
                LogConsoleWarning("A list with values type <color=cyan>" + typeof(V) + "</color> does contain already the value '" + value + "' ! ");
                return false;
            }
            return true;
        }


        /// <summary>
        /// Checks if the given array contains the given value.
        /// Prints a warning in the console if value is not found.
        /// </summary>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="array"> array to check into </param>
        /// <param name="value"> value to check inside the list </param>
        /// <returns></returns>
        protected bool IS_VALUE_CONTAINED<V>(V[] array, V value)
        {
            if (array.Contains(value) == false)
            {
                LogConsoleWarning("A list with values type <color=cyan>" + typeof(V) + "</color> doens't contain the value '" + value + "' ! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given array doesn't contain the given value.
        /// Prints a warning in the console if value is found.
        /// </summary>
        /// <typeparam name="V"> type of the value </typeparam>
        /// <param name="array"> array to check into </param>
        /// <param name="value"> value to check inside the list </param>
        /// <returns></returns>
        protected bool IS_VALUE_NOT_CONTAINED<V>(V[] array, V value)
        {
            if (array.Contains(value) == true)
            {
                LogConsoleWarning("A list with valuess type <color=cyan>" + typeof(V) + "</color> does contain already the value '" + value + "' ! ");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given index is a valid one for the given array (i.e. the index is smaller than the lenght of the array)
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool IS_VALID_INDEX<V>(V[] array, int index, bool reverseLogCondition = false)
        {
            if (index >= array.Length)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("'" + index + "' is not a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("'" + index + "' is a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");

            return true;
        }

        /// <summary>
        /// Checks if the given index is a valid one for the given array (i.e. the index is smaller than the lenght of the array)
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool IS_VALID_INDEX<V>(List<V> list, int index, bool reverseLogCondition = false)
        {
            if (index >= list.Count)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("'" + index + "' is not a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("'" + index + "' is a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");

            return true;
        }

        /// <summary>
        /// Checks if the given index is a valid one for the given array (i.e. the index is smaller than the lenght of the array)
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool IS_NOT_VALID_INDEX<V>(V[] array, int index, bool reverseLogCondition = false)
        {
            if (index < array.Length)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("'" + index + "' is a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("'" + index + "' is not a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");

            return true;
        }

        /// <summary>
        /// Checks if the given index is a valid one for the given array (i.e. the index is smaller than the lenght of the array)
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected bool IS_NOT_VALID_INDEX<V>(List<V> list, int index, bool reverseLogCondition = false)
        {
            if (index < list.Count)
            {
                if (!reverseLogCondition)
                    LogConsoleWarning("'" + index + "' is a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");
                
                return false;
            }

            if (reverseLogCondition)
                LogConsoleWarning("'" + index + "' is not a valid index for the array of type type <color=cyan>" + typeof(V) + "</color> ! ");

            return true;
        }



        #endregion

        #region Object Type
        protected bool IsGameObjectInMainStage()
        {
            return GetObjectStage() == EObjectStage.MAIN_STAGE;
        }

        public EObjectStage GetObjectStage()
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(gameObject))
            {
                return EObjectStage.PRESISTENCE_STAGE;
            }

            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gameObject);
            if (currentStage == mainStage)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                {
                    var type = PrefabUtility.GetPrefabAssetType(gameObject);
                    var path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
                    //Debug.Log(string.Format("GameObject is part of a Prefab Instance in the MainStage and is of type: {0}. It comes from the prefab asset: {1}", type, path));
                    return EObjectStage.MAIN_STAGE;
                }
                else
                {
                    //Debug.Log("GameObject is a plain GameObject in the MainStage");
                    return EObjectStage.MAIN_STAGE;
                }
            }
            else
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                if (prefabStage != null)
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                    {
                        var type = PrefabUtility.GetPrefabAssetType(gameObject);
                        var nestedPrefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
                        //Debug.Log(string.Format("GameObject is in a PrefabStage. The GameObject is part of a nested Prefab Instance and is of type: {0}. The opened Prefab asset is: {1} and the nested Prefab asset is: {2}", type, prefabStage.prefabAssetPath, nestedPrefabPath));
                        return EObjectStage.PREFAB_STAGE;
                    }
                    else
                    {
                        var prefabAssetRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.prefabAssetPath);
                        var type = PrefabUtility.GetPrefabAssetType(prefabAssetRoot);
                        //Debug.Log(string.Format("GameObject is in a PrefabStage. The opened Prefab is of type: {0}. The GameObject comes from the prefab asset: {1}", type, prefabStage.prefabAssetPath));
                        return EObjectStage.PREFAB_STAGE;
                    }
                }
                else if (EditorSceneManager.IsPreviewSceneObject(gameObject))
                {
                    //Debug.Log("GameObject is not in the MainStage, nor in a PrefabStage. But it is in a PreviewScene so could be used for Preview rendering or other utilities.");
                    return EObjectStage.OTHER_STAGE;
                }
                else
                {
                    LogConsoleError("Unknown GameObject Info");
                }
            }
#endif
            return EObjectStage.OTHER_STAGE;
        }

        public void LogStageInformation()
        {
#if UNITY_EDITOR
            // First check if input GameObject is persistent before checking what stage the GameObject is in 
            if (EditorUtility.IsPersistent(gameObject))
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
                {
                    LogConsole("The GameObject is a temporary object created during import. OnValidate() is called two times with a temporary object during import: First time is when saving cloned objects to .prefab file. Second event is when reading .prefab file objects during import");
                }
                else
                {
                    LogConsole("GameObject is part of an imported Prefab Asset (from the Library folder)");
                }
                return;
            }

            // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
            var mainStage = StageUtility.GetMainStageHandle();
            var currentStage = StageUtility.GetStageHandle(gameObject);
            if (currentStage == mainStage)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                {
                    var type = PrefabUtility.GetPrefabAssetType(gameObject);
                    var path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
                    LogConsole(string.Format("GameObject is part of a Prefab Instance in the MainStage and is of type: {0}. It comes from the prefab asset: {1}", type, path));
                }
                else
                {
                    LogConsole("GameObject is a plain GameObject in the MainStage");
                }
            }
            else
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                if (prefabStage != null)
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                    {
                        var type = PrefabUtility.GetPrefabAssetType(gameObject);
                        var nestedPrefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
                        LogConsole(string.Format("GameObject is in a PrefabStage. The GameObject is part of a nested Prefab Instance and is of type: {0}. The opened Prefab asset is: {1} and the nested Prefab asset is: {2}", type, prefabStage.prefabAssetPath, nestedPrefabPath));
                    }
                    else
                    {
                        var prefabAssetRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.prefabAssetPath);
                        var type = PrefabUtility.GetPrefabAssetType(prefabAssetRoot);
                        LogConsole(string.Format("GameObject is in a PrefabStage. The opened Prefab is of type: {0}. The GameObject comes from the prefab asset: {1}", type, prefabStage.prefabAssetPath));
                    }
                }
                else if (EditorSceneManager.IsPreviewSceneObject(gameObject))
                {
                    LogConsole("GameObject is not in the MainStage, nor in a PrefabStage. But it is in a PreviewScene so could be used for Preview rendering or other utilities.");
                }
                else
                {
                    LogConsole("Unknown GameObject Info");
                }
            }
#endif
        }

        public void LogPrefabInformation()
        {
#if UNITY_EDITOR
            StringBuilder stringBuilder = new StringBuilder();

            // First check if input GameObject is persistent before checking what stage the GameObject is in 
            if (EditorUtility.IsPersistent(gameObject))
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
                {
                    stringBuilder.Append("The GameObject is a temporary object created during import. OnValidate() is called two times with a temporary object during import: First time is when saving cloned objects to .prefab file. Second event is when reading .prefab file objects during import");
                }
                else
                {
                    stringBuilder.Append("GameObject is part of an imported Prefab Asset (from the Library folder).\n");
                    stringBuilder.AppendLine("Prefab Asset: " + GetPrefabInfoString(gameObject));
                }

                Debug.Log(stringBuilder.ToString());
                return;
            }

            PrefabStage prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
            if (prefabStage != null)
            {
                GameObject openPrefabThatContentsIsPartOf = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.prefabAssetPath);
                stringBuilder.AppendFormat(
                    "The GameObject is part of the Prefab contents of the Prefab Asset:\n{0}\n\n",
                    GetPrefabInfoString(openPrefabThatContentsIsPartOf));
            }

            if (!PrefabUtility.IsPartOfPrefabInstance(gameObject))
            {
                stringBuilder.Append("The GameObject is a plain GameObject (not part of a Prefab instance).\n");
            }
            else
            {
                // This is the Prefab Asset that can be applied to via the Overrides dropdown.
                GameObject outermostPrefabAssetObject = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                // This is the Prefab Asset that determines the icon that is shown in the Hierarchy for the nearest root.
                GameObject nearestRootPrefabAssetObject = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject));
                // This is the Prefab Asset where the original version of the object comes from.
                GameObject originalPrefabAssetObject = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                stringBuilder.AppendFormat(
    @"Prefab Asset of the outermost Prefab instance the input GameObject is part of is:
{0}
Prefab Asset of the nearest Prefab instance root the input GameObject is part of is:
{1}
Prefab Asset of the innermost Prefab instance the input GameObject is part of is:
{2}
Complete nesting chain from outermost to original:
",
                GetPrefabInfoString(outermostPrefabAssetObject),
                GetPrefabInfoString(nearestRootPrefabAssetObject),
                GetPrefabInfoString(originalPrefabAssetObject));

                GameObject current = outermostPrefabAssetObject;
                while (current != null)
                {
                    stringBuilder.AppendLine(GetPrefabInfoString(current));
                    current = PrefabUtility.GetCorrespondingObjectFromSource(current);
                }
            }

            stringBuilder.AppendLine("");

            Debug.Log(stringBuilder.ToString());
#endif
        }

#if UNITY_EDITOR
        private string GetPrefabInfoString(GameObject prefabAssetGameObject)
        {

            string name = prefabAssetGameObject.transform.root.gameObject.name;
            string assetPath = AssetDatabase.GetAssetPath(prefabAssetGameObject);
            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(prefabAssetGameObject);
            return string.Format("<b>{0}</b> (type: {1}) at '{2}'", name, type, assetPath);
        }
#endif
#endregion

        protected bool CanValidate()
        {
            return GetObjectStage() == EObjectStage.MAIN_STAGE;
        }

        #region Coroutine
        /// <summary>
        /// Starts a couroutine and store it in the given enumerator. If the enumerator is already running a coroutine, then stop it and start a new one.
        /// Note : Add "ref" to the first enumerator argument and "()" at the end of the second coroutine argument.
        /// </summary>
        /// <param name="enumerator"> where the coroutine reference will be stored (define as a member in your class) </param>
        /// <param name="coroutine"> the coroutine name function to run + () with parameters if defined </param>
        protected void StartNewCoroutine(ref IEnumerator enumerator, IEnumerator coroutine)
        {
            if (gameObject.activeInHierarchy)
            {
                // Stop running coroutine
                if (enumerator != null)
                {
                    StopCoroutine(enumerator);
                }
                // Assign reference
                enumerator = coroutine;
                // Start new coroutine
                StartCoroutine(enumerator);
            }
        }

        /// <summary>
        /// Stops a couroutine if already running.
        /// </summary>
        /// <param name="enumerator"> where the coroutine reference will be stored (define as a member in your class) </param>
        /// <returns> True if the coroutine was running and got stopped, otherwise false </returns>
        protected bool StopCoroutineIfRunning(ref IEnumerator enumerator)
        {
            if (enumerator != null)
            {
                StopCoroutine(enumerator);
                enumerator = null;
                return true;
            }
            return false;
        }

        protected void Wait(float waitDuration, Action action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, action));
        }

        private IEnumerator WaitCoroutine(float waitDuration, Action action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke();
        }

        /* 1 Parameters */
        protected void Wait<A>(float waitDuration, A arg1, Action<A> action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, arg1, action));
        }

        private IEnumerator WaitCoroutine<A>(float waitDuration, A arg1, Action<A> action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke(arg1);
        }

        /* 2 Parameters */
        protected void Wait<A, B>(float waitDuration, A arg1, B arg2, Action<A, B> action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, arg1, arg2, action));
        }

        private IEnumerator WaitCoroutine<A, B>(float waitDuration, A arg1, B arg2, Action<A, B> action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke(arg1, arg2);
        }

        /* 3 Parameters */
        protected void Wait<A, B, C>(float waitDuration, A arg1, B arg2, C arg3, Action<A, B, C> action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, arg1, arg2, arg3, action));
        }

        protected IEnumerator WaitCoroutine<A, B, C>(float waitDuration, A arg1, B arg2, C arg3, Action<A, B, C> action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke(arg1, arg2, arg3);
        }

        /* 4 Parameters */
        protected void Wait<A, B, C, D>(float waitDuration, A arg1, B arg2, C arg3, D arg4, Action<A, B, C, D> action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, arg1, arg2, arg3, arg4, action));
        }

        protected IEnumerator WaitCoroutine<A, B, C, D>(float waitDuration, A arg1, B arg2, C arg3, D arg4, Action<A, B, C, D> action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke(arg1, arg2, arg3, arg4);
        }

        /* 5 Parameters */
        protected void Wait<A, B, C, D, E>(float waitDuration, A arg1, B arg2, C arg3, D arg4, E arg5, Action<A, B, C, D, E> action)
        {
            StartCoroutine(WaitCoroutine(waitDuration, arg1, arg2, arg3, arg4, arg5, action));
        }

        protected IEnumerator WaitCoroutine<A, B, C, D, E>(float waitDuration, A arg1, B arg2, C arg3, D arg4, E arg5, Action<A, B, C, D, E> action)
        {
            yield return new WaitForSeconds(waitDuration);

            action.Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        #endregion

        #region Get Component
        /// <summary>
        /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="logWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public T GetComponentWithCheck<T>(bool logWarningMessageIfNoneFound = true)
        { 
            T result = GetComponent<T>();
            if (logWarningMessageIfNoneFound && 
                result == null)
            {
                LogConsoleWarning("Component of type <color=cyan>" + typeof(T) + "</color> not found on '" + name + "'");
            }
            return result;
        }

        /// <summary>
        /// Looks for all components attached to the same gameobject of the given type and prompt an error message if none found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="logWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public T[] GetComponentsWithCheck<T>(bool logWarningMessageIfNoneFound = true)
        {
            T[] result = GetComponents<T>();
            if (logWarningMessageIfNoneFound 
                 && result.Length == 0)
            {
                LogConsoleWarning("Not any component of type <color=cyan>" + typeof(T) + "</color> not found on '" + name + "'");
            }
            return result;
        }

        /// <summary>
        /// Looks for component attached to the same gameobject of the given type and prompt an error message if not found.
        /// </summary>
        /// <typeparam name="T"> Type of component looking for (must be a Component) </typeparam>
        /// <param name="excludeSelf"></param>
        /// <param name="logWarningMessageIfNoneFound"> if true, promt a warning message in the log console to inform that not any component was found </param>
        /// <returns> Component found otherwise null </returns>
        public T GetComponentInHierarchy<T>(bool excludeSelf = true, bool logWarningMessageIfNoneFound = true)
        {
            T result = GetComponent<T>();
            if (result == null
                || excludeSelf)
            {
                result = GetComponentInChildren<T>();
                if (result == null)
                {
                    result = GetComponentInParents<T>(excludeSelf);
                }
            }

            if ((logWarningMessageIfNoneFound == true) && (result == null))
            {
                LogConsoleWarning("Component of type <color=cyan>" + typeof(T) + "</color> not found on '" + name + "'");
            }
            return result;
        }

        public T GetComponentInParents<T>(bool excludeSelf = true, bool logWarningMessageIfNoneFound = false)
        {
            T result = default(T);

            if (!excludeSelf)
            {
                result = GetComponent<T>();
                if (result != null)
                    return result;
            }
            
            Transform parent = transform.parent;
            while (parent != null && result == null)
            {
                result = parent.GetComponent<T>();
                parent = parent.parent;
            }

            if (logWarningMessageIfNoneFound 
                && result == null)
            {
                LogConsoleWarning("Component of type <color=cyan>" + typeof(T) + "</color> not found in the parents of '" + name + "'");
            }
            return result;
        }

        public T GetComponentInDirectParent<T>(bool logWarningMessageIfNoneFound = false)
        {
            T result = default(T);
            Transform parent = transform.parent;
            if (parent != null)
            {
                result = parent.GetComponent<T>();
            }

            if (logWarningMessageIfNoneFound 
                && result == null)
            {
                LogConsoleWarning("Component of type <color=cyan>" + typeof(T) + "</color> not found in the direct parent of '" + name + "'");
            }
            return result;
        }

        /// <summary>
        /// Retrieves the hierarchical uppermost transform starting from this gameobject.
        /// </summary>
        /// <returns></returns>
        public Transform GetUppermostParentTransform()
        {
            Transform uppermostParentTransform = transform;
            while (uppermostParentTransform.parent != null)
            {
                uppermostParentTransform = uppermostParentTransform.parent;
            }
            return uppermostParentTransform;
        }

        public void SetComponentIfNull<T>(ref T reference)
        {
            if (reference == null)
            {
                reference = GetComponent<T>();
            }
        }

        public void SetComponentInChildrenIfNull<T>(ref T reference)
        {
            if (reference == null)
            {
                reference = GetComponentInChildren<T>();
            }
        }

        public void SetComponentInParentIfNull<T>(ref T reference)
        {
            if (reference == null)
            {
                reference = GetComponentInParent<T>();
            }
        }

        public void SetComponentInHierarchyIfNull<T>(ref T reference)
        {
            if (reference == null)
            {
                reference = GetComponentInHierarchy<T>();
            }
        }

        public void SetReferenceFromSceneIfNull<T>(ref T reference) where T : UnityEngine.Object
        {
            if (reference == null)
            {
                reference = FindObjectOfType<T>();
            }
        }
        #endregion

        #region Event Invoke
        /// <summary>
        /// Invoke an event after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound(Action eventToInvoke)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <param name="arg1" > First generic parameter of the event </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound<A>(Action<A> eventToInvoke, A arg1)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke(arg1);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <param name="arg1" > First generic parameter of the event </param>
        /// <param name="arg2" > Second generic parameter of the event </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound<A, B>(Action<A, B> eventToInvoke, A arg1, B arg2)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke(arg1, arg2);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="C"> Third generic parameter type of the event </typeparam>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <param name="arg1" > First generic parameter of the event </param>
        /// <param name="arg2" > Second generic parameter of the event </param>
        /// <param name="arg3" > Third generic parameter of the event </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound<A, B, C>(Action<A, B, C> eventToInvoke, A arg1, B arg2, C arg3)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke(arg1, arg2, arg3);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="C"> Third generic parameter type of the event </typeparam>
        /// <typeparam name="D"> Fourth generic parameter type of the event </typeparam>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <param name="arg1" > First generic parameter of the event </param>
        /// <param name="arg2" > Second generic parameter of the event </param>
        /// <param name="arg3" > Third generic parameter of the event </param>
        /// <param name="arg4" > Fourth generic parameter of the event </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound<A, B, C, D>(Action<A, B, C, D> eventToInvoke, A arg1, B arg2, C arg3, D arg4)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke(arg1, arg2, arg3, arg4);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoke an event with the given parameters, after checking if it is bound (i.e. different from null)
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="C"> Third generic parameter type of the event </typeparam>
        /// <typeparam name="D"> Fourth generic parameter type of the event </typeparam>
        /// <typeparam name="E"> Fifth generic parameter type of the event </typeparam>
        /// <param name="eventToInvoke"> event to invoke </param>
        /// <param name="arg1" > First generic parameter of the event </param>
        /// <param name="arg2" > Second generic parameter of the event </param>
        /// <param name="arg3" > Third generic parameter of the event </param>
        /// <param name="arg4" > Fourth generic parameter of the event </param>
        /// <param name="arg5" > Fifth generic parameter of the event </param>
        /// <returns> true if invoked, otherwise false if not bound </returns>
        protected bool InvokeEventIfBound<A, B, C, D, E>(Action<A, B, C, D, E> eventToInvoke, A arg1, B arg2, C arg3, D arg4, E arg5)
        {
            if (eventToInvoke != null)
            {
                eventToInvoke.Invoke(arg1, arg2, arg3, arg4, arg5);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs(Action eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action);
                }
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs<A>(Action<A> eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action<A>);
                }
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs<A, B>(Action<A, B> eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action<A, B>);
                }
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Third generic parameter type of the event </typeparam>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs<A, B, C>(Action<A, B, C> eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action<A, B, C>);
                }
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Third generic parameter type of the event </typeparam>
        /// <typeparam name="D"> Fourth generic parameter type of the event </typeparam>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs<A, B, C, D>(Action<A, B, C, D> eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action<A, B, C, D>);
                }
            }
        }

        /// <summary>
        /// Clear all the callbacks registered to the given event
        /// </summary>
        /// <typeparam name="A"> First generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Second generic parameter type of the event </typeparam>
        /// <typeparam name="B"> Third generic parameter type of the event </typeparam>
        /// <typeparam name="D"> Fourth generic parameter type of the event </typeparam>
        /// <typeparam name="E"> Fifth generic parameter type of the event </typeparam>
        /// <param name="eventToClear"> Event to clear the callbacks from </param>
        protected void ClearEventCallbakcs<A, B, C, D, E>(Action<A, B, C, D, E> eventToClear)
        {
            if (eventToClear != null)
            {
                Delegate[] delegates = eventToClear.GetInvocationList();
                foreach (Delegate myDelegate in delegates)
                {
                    eventToClear -= (myDelegate as Action<A, B, C, D, E>);
                }
            }
        }
#endregion
    }
}