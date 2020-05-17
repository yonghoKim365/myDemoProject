using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// UIScrollView에 표시되어야할 리스트들의 기반 클래스
/// 
/// 이 클래스를 상속받아서 데이터 초기화 및 리스트 셋팅을 하도록 한다.
/// </summary>
/// <typeparam name="PARENT">부모 패널</typeparam>
public abstract class ListBase<PARENT> : MonoBehaviour 
    where PARENT : UIBasePanel
{
    public    PARENT  parentPanel;
    protected UIScrollView        scrollView;
    protected List<GameObject>    contentsList;
    protected int defaultSlotCount = 0;
    protected int curSlotCount = 0;
    
    void Awake()
    {
        Init();
    }
    
    protected virtual void Init()
    {
        parentPanel = NGUITools.FindInParents<PARENT>( gameObject );

        contentsList = new List<GameObject>();
        scrollView = NGUITools.FindInParents<UIScrollView>( gameObject );

       // if (null == scrollView)
       //     Debug.LogError("can't find UIScrollView. you must have UIScrollView!", gameObject);
    }

    /// <summary>
    /// 실제 리스트를 채워주는 작업을 하는 함수
    /// </summary>
    /// <typeparam name="TYPE">여러 탭의 리스트를 표현해야할 때 사용할 타입</typeparam>
    /// <param name="type">보통 카테고리용으로 사용하면됨</param>
    /// <remarks>
    /// ---- 기본 순서 ----
    /// 1. Contents 클리어   
    /// 2. ListContents 생성
    /// 3. 위치 정렬
    /// </remarks>
    public abstract void ShowList(int type, params object[] args);

    //< 해당 패널에서 무언가에 특별한 행동을 취해야할때 사용
    public virtual void EventCheck()
    {

    }

    //< 데이터만 업데이트될시에 호출
    public virtual void DataUpdate() { }

    /// <summary>
    /// 리스트 정렬 및 ScrollView 위치 초기화
    /// </summary>
    public virtual void ResetPosition()
    {
        SendMessage( "Reposition", SendMessageOptions.DontRequireReceiver );
        if (null != scrollView)
        {
            scrollView.ResetPosition();
        }
    }

    public virtual void MakeList()
    {

    }

    /// <summary>
    /// 리스트에 해당 Contents를 추가한다.
    /// </summary>
    /// <param name="prefabGO">리스트에 복제되어 추가될 프리팹</param>
    /// <returns>리스트에 추가된 Contents객체</returns>
    public virtual GameObject AddContents(GameObject prefabGO, string name = null)
    {
        if (prefabGO == null)
        {
            if (GameDefine.TestMode)
                Debug.Log( "prefabGO is null" );
            return null;

        }

        GameObject contentsGO = Instantiate( prefabGO ) as GameObject;
        if (name != null)
            contentsGO.name = name;

        contentsGO.transform.AttachTo( transform );

        contentsList.Add( contentsGO );

        return contentsGO;
    }

    public virtual GameObject AddContents(GameObject prefabGO, Transform parent, string name = null)
    {
        if (prefabGO == null)
        {
            if (GameDefine.TestMode)
                Debug.Log("prefabGO is null");
            return null;

        }

        GameObject contentsGO = Instantiate(prefabGO) as GameObject;
        if (name != null)
            contentsGO.name = name;

        contentsGO.transform.AttachTo(parent);

        contentsList.Add(contentsGO);

        return contentsGO;
    }

    public virtual GameObject SetContents( int Num, string name = null )
    {
        if(name != null)
            contentsList[Num].name = name;

        return contentsList[Num];
    }

    public GameObject GetContents(int no)
    {
        return contentsList[Mathf.Clamp( no, 0, contentsList.Count )];
    }

    public List<GameObject> GetContentsAll()
    {
        return contentsList;
    }

    /// <summary>
    /// 리스트에 존재하는 모든 컨텐츠를 파괴한다.
    /// </summary>
    public virtual void ClearContents()
    {
        for (int i = 0; i < contentsList.Count; i++)
        {
            if (null != contentsList[i])
            {
                DestroyImmediate(contentsList[i]);
            }
        }

        contentsList.Clear();
    }
}
