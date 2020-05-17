using UnityEngine;
using System.Collections;
using System.Text;

/// <summary>
/// Resources.Load되는 객체들을 관리해주는 클래스
/// </summary>
public class ResourceMgr
{
	static public string UIRootPath = "UI/";
    static public string SoundPath = "Sound/";

    [SerializeField]
    static Cache<string, Object> cachedDic = new Cache<string, Object>();
	
    /// <summary>
    /// Prefab 전용 로더 (caching)
    /// </summary>
    public static Object Load(string path)
    {
        Object obj = cachedDic.Get(path);
        if (null != obj)
            return obj;

        obj = Resources.Load(path);
        cachedDic.Put(path, obj);

        return obj;
    }

	public static IEnumerator LoadAsync(string path, System.Action<Object> callback)
	{
		Object obj = cachedDic.Get(path);
		if (null != obj) {
			callback(obj);
			yield break;
		}

		ResourceRequest resReq = Resources.LoadAsync (path);
		while (!resReq.isDone) { 
			yield return null; 
		}
		
		obj = resReq.asset;
		cachedDic.Put(path, obj);

		callback(obj);		
	}

    /// <summary>
    /// UI 객체를 로드한다
    /// </summary>
    /// <param name="path">Root폴더 이름을 제외한 path</param>
    public static Object LoadUI(string path)
    {
        return Load(UIRootPath + path);
    }

    public static AudioClip LoadSound(string path)
    {
        return Load(SoundPath + path) as AudioClip;
    }

    /// <summary>
    /// 로드한 객체의 복사본을 생성해준다.
    /// </summary>
    public static ConverTYPE Instantiate<ConverTYPE>(string path) where ConverTYPE : Object
    {
        Object obj = Load(path);

        return obj != null ? GameObject.Instantiate(obj) as ConverTYPE : null;
    }

    /// <summary>
    /// 로드된 객체 생성후, 알맞은 컴포넌트를 찾아서 리턴해줌
    /// </summary>
    /// <typeparam name="ComponentTYPE">생성된 객체에서 찾고자하는 컴포넌트</typeparam>
    public static ComponentTYPE InstAndGetComponent<ComponentTYPE>(string path) where ComponentTYPE : Component
    {
        Object obj = Load(path);

        return obj != null ? (GameObject.Instantiate( obj ) as GameObject).GetComponent<ComponentTYPE>() : null;
    }

    /// <summary>
    /// UIRootPath를 제외한 경로만 넣어주면, 복사본 생성
    /// </summary>
    /// <param name="path">UIRootPath를 제외한 경로</param>
    /// <returns>생성된 GameObject</returns>
    public static GameObject InstantiateUI(string path)
    {
        Object obj = LoadUI(path);
		
        return obj != null ? GameObject.Instantiate(obj) as GameObject : null;
    }
	
    /// <summary>
    /// caching된 리스트와 사용되지 않는 리소스 정리
    /// </summary>
    public static void Clear()
    {
        cachedDic.Clear();
    }
}
