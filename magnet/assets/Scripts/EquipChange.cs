using UnityEngine;
using System.Collections;

public class EquipChange : MonoBehaviour {
    public void ModelChange(string srcMeshName, string newPrefname, string newMeshName)
    {
        GameObject newData = (GameObject)Instantiate(Resources.Load(string.Format("Character/Prefab/{0}", newPrefname)));
        GameObject newMesh = newData.transform.FindChild(newMeshName).gameObject;
        GameObject srcMesh = transform.FindChild(srcMeshName).gameObject;

        //원본삭제
        DestroyImmediate(srcMesh.gameObject);

        SkinnedMeshRenderer[] BonedObjects = newMesh.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in BonedObjects)
            ProcessBonedObject(smr);

        DestroyImmediate(newData);
    }

    private void ProcessBonedObject(SkinnedMeshRenderer ThisRenderer)
    {
        // Create the SubObject
        GameObject _baseModel = new GameObject(ThisRenderer.gameObject.name);
        //_baseModel.name = _srcMeshName;
        _baseModel.transform.parent = transform;

        // Add the renderer
        SkinnedMeshRenderer NewRenderer = _baseModel.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;

        // Assemble Bone Structure
        Transform[] MyBones = new Transform[ThisRenderer.bones.Length];

        // As clips are using bones by their names, we find them that way.
        for (int i = 0; i < ThisRenderer.bones.Length; i++)
            MyBones[i] = FindChildByName(ThisRenderer.bones[i].name, transform);

        // Assemble Renderer
        NewRenderer.bones = MyBones;
        NewRenderer.sharedMesh = ThisRenderer.sharedMesh;
        NewRenderer.materials = ThisRenderer.materials;
    }

    private Transform FindChildByName(string ThisName, Transform ThisGObj)
    {
        Transform ReturnObj;

        if (ThisGObj.name == ThisName)
            return ThisGObj.transform;

        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);
            if (ReturnObj)

                return ReturnObj;
        }

        return null;
    }

    
}

