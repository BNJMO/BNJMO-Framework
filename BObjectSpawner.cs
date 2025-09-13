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
    [MenuItem("GameObject/BNJMO/UI/BFrame", false, 0)]
    public static void CreateBFrame()
    {
        SpawnObject(BConsts.PATH_BFrame);
    }
    
    [MenuItem("GameObject/UI/BNJMO/BMenu", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BMenu", false, 0)]
    public static void CreateBMenu()
    {
        SpawnObject(BConsts.PATH_BMenu);
    }

    [MenuItem("GameObject/UI/BNJMO/BButton", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BButton", false, 0)]
    public static void CreateBButton()
    {
        SpawnObject(BConsts.PATH_BButton);
    }

    [MenuItem("GameObject/UI/BNJMO/BContainer", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BContainer", false, 0)]
    public static void CreateBContainer()
    {
        SpawnObject(BConsts.PATH_BContainer);
    }

    [MenuItem("GameObject/UI/BNJMO/BImage", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BImage", false, 0)]
    public static void CreateBImage()
    {
        SpawnObject(BConsts.PATH_BImage);
    }

    [MenuItem("GameObject/UI/BNJMO/BRawImage", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BRawImage", false, 0)]
    public static void CreateBRawImage()
    {
        SpawnObject(BConsts.PATH_BRawImage);
    }

    [MenuItem("GameObject/UI/BNJMO/BSpriteRenderer", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BSpriteRenderer", false, 0)]
    public static void CreateBSpriteRenderer()
    {
        SpawnObject(BConsts.PATH_BSpriteRenderer);
    }

    [MenuItem("GameObject/UI/BNJMO/B3DText", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/B3DText", false, 0)]
    public static void CreateB3DText()
    {
        SpawnObject(BConsts.PATH_B3DText);
    }

    [MenuItem("GameObject/UI/BNJMO/BText", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BText", false, 0)]
    public static void CreateBText()
    {
        SpawnObject(BConsts.PATH_BText);
    }
    
    [MenuItem("GameObject/UI/BNJMO/BInputField", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BInputField", false, 0)]
    public static void CreateBInputField()
    {
        SpawnObject(BConsts.PATH_InputField);
    }

    [MenuItem("GameObject/UI/BNJMO/BScrollView", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BScrollView", false, 0)]
    public static void CreateBScrollView()
    {
        SpawnObject(BConsts.PATH_BScrollView);
    }
    
    [MenuItem("GameObject/UI/BNJMO/BScrollViewNested", false, 0)]
    [MenuItem("GameObject/BNJMO/UI/BScrollViewNested", false, 0)]
    public static void CreateBScrollViewNested()
    {
        SpawnObject(BConsts.PATH_BScrollViewNested);
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
                spawnedObject.transform.localPosition = Vector3.one;
                spawnedObject.transform.localRotation = Quaternion.identity;
                spawnedObject.transform.localScale = Vector3.one;

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