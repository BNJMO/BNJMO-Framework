using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BNJMO
{
    public class BObjectSpawner : BBehaviour
    {
#if UNITY_EDITOR

        [MenuItem("GameObject/UI/BNJMO/BFrame", false, 0)]
        public static void CreateBFrame()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BFrame);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }        
        
        [MenuItem("GameObject/UI/BNJMO/BMenu", false, 0)]
        public static void CreateBMenu()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BMenu);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BButton", false, 0)]
        public static void CreateBButton()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BButton);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BContainer", false, 0)]
        public static void CreateBContainer()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BContainer);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BImage", false, 0)]
        public static void CreateBImage()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BImage);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BRawImage", false, 0)]
        public static void CreateBRawImage()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BRawImage);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BSpriteRenderer", false, 0)]
        public static void CreateBSpriteRenderer()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BSpriteRenderer);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/B3DText", false, 0)]
        public static void CreateB3DText()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_B3DText);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BText", false, 0)]
        public static void CreateBText()
        {
            GameObject spawnedObject = SpawnObject(BConsts.PATH_BText);
            if (spawnedObject)
            {
                spawnedObject.transform.localPosition = Vector3.one;
            }
        }

        [MenuItem("GameObject/UI/BNJMO/BScrollView", false, 0)]
        public static void CreateBScrollView()
        {
            SpawnObject(BConsts.PATH_BScrollView);
        }

        private static GameObject SpawnObject(string resourcePath)
        {
            GameObject objectPrefab = Resources.Load<GameObject>(resourcePath);
            if (objectPrefab)
            {
                GameObject spawnedObject = Instantiate(objectPrefab);

                if (spawnedObject)
                {
                    // Remove (Clone) at the end
                    Debug.Log("spawnedObject : " + spawnedObject.name);
                    spawnedObject.name = spawnedObject.name.Replace("(Clone)", "");

                    // Set transform of the selected object as parent
                    GameObject selectedObject = (GameObject)Selection.activeObject;
                    if (selectedObject)
                    {
                        spawnedObject.transform.parent = selectedObject.transform;
                    }

                    // Set spawned object as selected
                    Selection.SetActiveObjectWithContext(spawnedObject, Selection.activeContext);
                    return spawnedObject;
                }
                else
                {
                    Debug.LogError("Couldn't spawn object!");
                }
            }
            else
            {
                Debug.LogError("The '" + resourcePath + "' prefab was not found in the Resources folder!");
            }
            return null;
        }
#endif

    }
}