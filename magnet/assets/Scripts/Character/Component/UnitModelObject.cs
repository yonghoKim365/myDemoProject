using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 현 게임을 위한 유닛에 의해 생성된 모델 관리용 클래스
/// </summary>
public class UnitModelObject
{
/// <summary>
    /// 모델 객체가 셋팅되었는지 유무
    /// </summary>
    public bool         IsReady { private set; get; }

    public GameObject   Owner;
    public GameObject   Main { private set; get; }

    public bool IsNotShader = false;
    
    public Transform    Transform;

    public float Scale {
        set { 
            modelScale = value;
            Transform.localScale = Vector3.one * modelScale;
            // Smooth옵션 필요시 넣기.
        }
        get { return modelScale; }
    }

    public Vector3 ScaleVec3 {
        get { return Transform.localScale; }
    }

    public float OriginalScale { 
        private set { originalScale = value; }
        get { return originalScale; }
    }

    public System.Action<GameObject>    OnChangedModel;

    float modelScale;
    float originalScale;

    #region :: Cached Variables ::

    Dictionary<string, Transform>   cachedDic = new Dictionary<string, Transform>();
    Dictionary<string, Transform>   attachedDic = new Dictionary<string, Transform>();
    Renderer[]                      cachedRenderers;
    MeshRenderer[]                  cachedMeshRens;
    SkinnedMeshRenderer[]           cachedSkinnedMeshRens;

    #endregion

    public UnitModelObject()
    {
        Reset();
    }

    public UnitModelObject(GameObject owner, GameObject modelGO, float _originalScale = 1f)
    {
        Reset();

        Init( owner, modelGO, _originalScale);
    }

    public virtual void Init(GameObject owner, GameObject modelGO, float _originalScale = 1f)
    { 
        if (null == modelGO)
        { 
            Debug.LogError( "Null 객체로 생성자를 호출할 수 없습니다!" );
            return;
        }

        Owner = owner;
        Main = modelGO;
        Transform = Main.transform;

        Transform.parent = owner.transform;
        Transform.localPosition = Vector3.zero;
        Transform.localRotation = new Quaternion(0, 0, 0, 0);
        
        OriginalScale = _originalScale;
        Scale = OriginalScale;

        if (null != OnChangedModel)
            OnChangedModel( Main );

        IsReady = true;
    }

    public virtual void Reset()
    {
        IsReady = false;

        Main = null;
        Transform = null;

        modelScale = 0f;
        originalScale = 0f;

        cachedDic.Clear();
        attachedDic.Clear();
        cachedRenderers = null;
        cachedMeshRens = null;
        cachedSkinnedMeshRens = null;
    }

    public void DeleteModel()
    {
        if (null != Main)
            Object.Destroy( Main );

        Reset();
    }

    /// <summary>
    /// 처음 셋팅크기로 되돌린다.
    /// </summary>
    public void ApplyOriginalScale()
    {
        Scale = originalScale;
    }
    
    /// <summary>
    /// 해당 Transform을 찾아준다. (찾는다면 caching해둠)
    /// </summary>
    /// <param name="name">찾고자하는 Transform name</param>
    /// <param name="cachingName">다음에 찾고자 할 때, 사용할 이름(default = null)</param>
    public Transform FindAndCaching(string name, string cachingName = null)
    {
        Transform foundTrans = null;
        if (cachedDic.TryGetValue( name, out foundTrans ))
            return foundTrans;
        
        foundTrans = Transform.FindTransform( name );

        if (null != foundTrans)
        {
            cachedDic.Add( string.IsNullOrEmpty( cachingName ) ? name : cachingName, foundTrans );
        }

        return foundTrans;
    }

    /// <summary>
    /// 캐싱된 Transform들 중에 존재하는 객체를 찾아서 리턴해준다.
    /// </summary>
    /// <param name="tName">찾고자하는 transform name</param>
    public Transform GetCached(string tName)
    {
        Transform foundTrans = null;
        cachedDic.TryGetValue( tName, out foundTrans );
        return foundTrans;
    }

    public bool Attach(string targetName, Transform fromModel)
    {
        if (null == fromModel || string.IsNullOrEmpty(targetName))
        {
            Debug.LogWarning(this + " : Model이 없거나, 타겟 이름이 잘못되었습니다." );
            return false;
        }

        Transform targetTrans = FindAndCaching( targetName );

        if (null != targetTrans)
        { 
            fromModel.AttachTo( targetTrans, true );
            
            attachedDic.Add(fromModel.name, fromModel);
        }

        return true;
    }

    public bool Detach(string tName)
    {
        return attachedDic.Remove( tName );
    }

    public bool Detach(Transform trans)
    { 
        string removingName = null;
        foreach (Transform t in attachedDic.Values)
        {
            if (t == trans)
                removingName = t.name;
        }

        return Detach(removingName);
    }

    #region :: Renderers ::
    /*
     * <주의사항>
     * 함수들이 한번만 불리면 캐싱되는데, 자식들이 변경되고 난 뒤에,
     * 변경된 사항들이 적용된 렌더러들을 받고 싶을때 문제가 됨.
     *  
     */

    public Renderer[] GetRenders()
    {
        if (null == cachedRenderers)
        {
            Renderer[] rens = Main.GetComponentsInChildren<Renderer>(true);
            cachedRenderers = rens;
        }

        return cachedRenderers;
    }

    public MeshRenderer[] GetMeshRenders()
    {
        if (null == cachedMeshRens)
        {
            MeshRenderer[] rens = Main.GetComponentsInChildren<MeshRenderer>(true);
            cachedMeshRens = rens;
        }

        return cachedMeshRens;
    }

    public SkinnedMeshRenderer[] GetSkinnedMeshRenders()
    {
        if (null == cachedSkinnedMeshRens)
        {
            SkinnedMeshRenderer[] rens = Main.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            cachedSkinnedMeshRens = rens;
        }

        return cachedSkinnedMeshRens;
    }

    public void BakeMeshObject(float lifetime = 1.5f)
    {
        if (null == GetSkinnedMeshRenders())
            return;

        for(int i=0; i<cachedSkinnedMeshRens.Length; i++)
        {
            SkinnedMeshRenderer skinedMeshRenderer = cachedSkinnedMeshRens[i];

            Mesh mesh = new Mesh();
            skinedMeshRenderer.BakeMesh(mesh);

            GameObject newObj = new GameObject("Copyed Mesh", typeof(MeshRenderer), typeof(MeshFilter));

            newObj.transform.position = Owner.transform.position;
            newObj.transform.rotation = Owner.transform.rotation * skinedMeshRenderer.transform.localRotation;
            newObj.GetComponent<MeshFilter>().mesh = mesh;
            newObj.renderer.material = new Material(Shader.Find("Transparent/VertexLit with Z"));

            // 자동삭제를 위한 코드
            newObj.AddComponent<SmoothDestroy>().Init(newObj.renderer.material, new Color(1f, 1f, 1f, 0.5f), lifetime);
        }
    }

    #endregion

    #region :: Info ::

    public string PrintInfo()
    {
        string text = string.Format("Type : " + GetType() + "\n");

        text += string.Format( "MainModel : {0}\n", IsReady ? Main.ToString() : string.Empty );
        text += string.Format( "CachedList : {0}\n", IsReady ? cachedDic.Count : 0 );
        text += string.Format( "AttachedList : {0}\n", IsReady ? attachedDic.Count : 0 );

        return text;
    }

    #endregion
}
