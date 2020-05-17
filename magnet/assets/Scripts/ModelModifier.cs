using UnityEngine;
using System.Collections;

public class ModelModifier : MonoBehaviour {

    /// <summary>
    /// srcMeshName의 게임오브젝트를 삭제하고 newPrefname내의 메쉬들을 추가함
    /// </summary>
    /// <param name="srcMeshName">삭제될 오브젝트의 이름</param>
    /// <param name="newPrefname">추가될 메쉬를 가진 오브젝트</param>
    public void ModelChange(string srcMeshName, string newPrefname)
    {
        GameObject newData = (GameObject)Instantiate(Resources.Load(string.Format("Character/Prefab/{0}", newPrefname)));
        GameObject srcMesh = transform.FindChild(srcMeshName).gameObject;

        //교체될 원본삭제
        DestroyImmediate(srcMesh.gameObject);

        SkinnedMeshRenderer[] BonedObjects = newData.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in BonedObjects)
            ProcessBonedObject(smr);

        //읽어온 새 데이터 삭제
        Destroy(newData);
    }

    /// <summary>
    /// 현재의 모델 하위에 newPrefname를 읽어서 메쉬들을 다 적용시켜준다.
    /// </summary>
    /// <param name="newPrefname">추가로 적용될 프리펩이름</param>
    public bool ModelApply(string newPrefname)
    {
        //Object prefeb = Resources.Load(newPrefname);
        GameObject prefeb = ResourceMgr.Load(newPrefname) as GameObject;

        if (prefeb == null)
        {
            Debug.LogWarning(string.Format("ModelApply-NotFoundPref: {0}", newPrefname));
            return false;
        }

        GameObject newData = (GameObject)Instantiate(prefeb);

        //if (newData == null)
        //{
        //    return false;
        //}

        SkinnedMeshRenderer[] BonedObjects = newData.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in BonedObjects)
            ProcessBonedObject(smr);

        //읽어온 새 데이터 삭제
        Destroy(newData);

        return true;
    }

    private void ProcessBonedObject(SkinnedMeshRenderer ThisRenderer)
    {
        // Create the SubObject
        GameObject _baseModel = new GameObject(ThisRenderer.gameObject.name);
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

