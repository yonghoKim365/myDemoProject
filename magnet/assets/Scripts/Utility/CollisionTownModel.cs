using UnityEngine;
using System.Collections;

public class CollisionTownModel : MonoBehaviour, IInputObject
{
    public enum FUNC_TYPE { NONE = 0, STAGE_OPEN, }
    /// <summary>
    /// 타운모델의 자신의 콜리더
    /// </summary>
    Collider myCol;
    /// <summary>
    /// 충돌체크해야할 대상(타운유닛이 고정으로 들어올 것이다)
    /// </summary>
    Collider TargetCollider;
    /// <summary>
    /// 현제 모델이 취해야할 펑션타입
    /// </summary>
    FUNC_TYPE FuncType = FUNC_TYPE.NONE;

    bool IsStay;

    /// <summary>
    /// 충돌시 일어나는 효과에 대한 정보를 넘겨준다.
    /// </summary>
    /// <param name="_target">대상충돌체</param>
    /// <param name="_funcType">충돌시 발생타입</param>
    public void CollisionModelInit(Collider _target, FUNC_TYPE _funcType = FUNC_TYPE.NONE)
    {
        TargetCollider = _target;
        FuncType = _funcType;

        myCol = GetComponentInChildren<Collider>();
        myCol.isTrigger = true;
    }

    void OnTriggerEnter(Collider _collider)
    {
        if (!IsStay)
            return;

        //충돌 처리를 해줘야하는 대상 외의 콜리더가 들어오면 빠져나간다.
        if (TargetCollider != _collider || !SceneManager.instance.GetState<TownState>().MyHero.CheckPotal() )
            return;

            OnTriggerEvent();

    }
    
    void OnTriggerStay(Collider _collider)
    {
        if (TargetCollider == _collider)
        {
            IsStay = true;
        }

        if (TargetCollider != _collider || !SceneManager.instance.GetState<TownState>().MyHero.CheckPotal())
            return;
            
        OnTriggerEvent();
    }

    void OnTriggerExit(Collider _collider)
    {
        IsStay = false;
    }
    
    public void OnTriggerEvent()
    {
        //충돌 처리를 해줘야하는 대상 외의 콜리더가 들어오면 빠져나간다.
        if (!IsStay)
            return;

        switch (FuncType)
        {
            case FUNC_TYPE.STAGE_OPEN:
                {
                    //싱글전투지역을 연다.
                    //QuestManager.instance.OpenChapter();
                }
                break;
        }
    }

    public void InputEvent(POINTER_INFO ptr)
    {
        if (!IsStay)
            return;
        
        if (ptr.evt == POINTER_INFO.INPUT_EVENT.CLICK)
        {
            OnTriggerEvent();
        }
    }

    /// <summary>
    /// 플레이어 캐릭터의 코스튬, 외형이 바뀌면 호출된다.
    /// 삭제했다가 재생성하므로 작업이 필요하기에 추가함.
    /// </summary>
    /// <param name="_target"></param>
    public void SetNewTargetCollider(Collider _target)
    {
        TargetCollider = _target;
    }
}
