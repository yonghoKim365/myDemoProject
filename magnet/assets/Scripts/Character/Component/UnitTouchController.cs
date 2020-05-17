using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 인풋입력을 받아서 유닛을 이동시키고, 타겟팅해주는 컨트롤러
/// </summary>
/// <remarks>
/// InputManger에 의존성.
/// InGame중에 사용해야만 유효함.
/// </remarks>
public class UnitTouchController
{
    public Unit     Owner { private set; get; }

    bool            Enabled;
    //bool            ConcentratedMode = false;

    int             defaultLayer;
    int             terrainLayer;
    int             terrainOutsideLayer;

    InputManager    InputManager;
    
    //GameObject      ClickEffect;
    //GameObject      DragEffect;
    Transform       TargetingEffect;

    public UnitTouchController()
    {
        defaultLayer = LayerMask.NameToLayer( "Default" );
        terrainLayer = LayerMask.NameToLayer( "Terrain" );
        terrainOutsideLayer = LayerMask.NameToLayer( "Terrain_Outside" );

        InputManager = InputManager.instance;

        //ClickEffect = ResourceMgr.Instantiate<GameObject>("Effect/_Common/Pointer_01") as GameObject;
        //ClickEffect.SetActive( false );

        //DragEffect = ResourceMgr.Instantiate<GameObject>("Effect/_Common/Drag_01") as GameObject;
        //DragEffect.SetActive( false );
    }

    public void Enable(bool enabled)
    {
        Enabled = enabled;

        HideDragEffect();
    }

    /// <summary>
    /// 모두공격, 단일공격 모드 설정
    /// </summary>
    public void ConcentratedFireMode(bool mode)
    {
        //ConcentratedMode = mode;
    }
    
    /// <summary>
    /// 선택된 유닛을 인풋입력에 따라 조정가능하도록 설정한다.
    /// </summary>
    /// <param name="target"></param>
    public UnitTouchController StartControl(Unit owner)
    {
        if (null == owner)
        {
            Debug.LogWarning( "존재하지 않는 유닛입니다." );
            return null;
        }

        Reset();

        InputManager.AddNonUIHitDelegate( OnNonUIInput );

        Enabled = true;
        
        return this;
    }

    public void EndControl()
    {
        Reset();
        
        Enabled = false;
    }

    public void Reset()
    {
        // 인풋 관련 연결 해제
        InputManager.RemoveNonUIHitDelegate( OnNonUIInput );
    }

    /// <summary>
    /// UI를 제외한 인풋에 관련된 데이터를 받는 함수
    /// </summary>
    void OnNonUIInput(POINTER_INFO ptr)
    {
        if (!Owner.Usable || !Enabled)
            return;

        // OnGUI 사용할때, OnGUI Component가 클릭되고나서도, 현재 이동 인풋같은게 안먹히기 위해 넣은 코드 (지워도됨)
        if (GUIUtility.hotControl != 0)
            return;

        int hitLayer = ptr.hitInfo.transform.gameObject.layer;

        if (ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
        {
            // 1차로 타겟직접 클릭인지 검사
            if (hitLayer == defaultLayer)
            {
            }
            else
            {
                //Unit target = null;
                //if (hitLayer == terrainLayer || hitLayer == terrainOutsideLayer)
                //{
                //    target = FindAndAttackTarget( ptr.hitInfo.point );
                //}

                //// 지형 클릭 처리
                //if (null == target && (hitLayer == terrainLayer || hitLayer == terrainOutsideLayer))
                //{
                //    Owner.Aggro.ClearData();
                //    Owner.SetTarget( GameDefine.unassignedID );
                //    bool success = Owner.MovePosition( ptr.hitInfo.point );

                //    // 이동할 지점에 이펙트 표시
                //    if (success)
                //        ShowClickMoveEffect( Owner.DestPosition );
                //}
            }
        }
        // Touch Pressing 상태에서 이동하기
        else if (ptr.mouseOrTouch.pressStarted && ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG)
        {
            if (hitLayer == terrainLayer || hitLayer == terrainOutsideLayer)
            {
                //Unit foundTarget = FindAndAttackTarget( ptr.hitInfo.point );

                //// 새로운 타겟은 찾지 못했다면, 지형 클릭으로 간주해서 이미 존재하는 타겟도 초기화
                //if (null == foundTarget && Owner.TargetID != GameDefine.unassignedID)
                //{
                //    Owner.Aggro.ClearData();
                //    Owner.SetTarget( GameDefine.unassignedID );
                //}

                //if (null == foundTarget && Owner.MovePosition( ptr.hitInfo.point ))
                //    ShowDragEffect( Owner.DestPosition );
                //else
                //    HideDragEffect();
            }
        }
        else if (ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
        {
            //HideDragEffect();
        }
    }

    /// <summary>
    /// 정해진 위치에서 가장가까운 적을 찾아서 타겟으로 설정하도록 한다.
    /// </summary>
    /// <returns></returns>
    Unit FindAndAttackTarget(Vector3 point)
    {
        Unit foundTarget = G_GameInfo.CharacterMgr.NearestTarget( Owner.TeamID, Owner.CharInfo.AttackType, point, GameDefine.ClickThresholds, true );

        //if (null != foundTarget)
        //{
        //    Owner.Aggro.ClearData();
        //    Owner.Aggro.AddAggro( foundTarget, GameDefine.FixedAggroPoint );

        //    // TODO : 수동 공격이 빠져서 이렇게함.
        //    int targetID = foundTarget.GetInstanceID();
        //    Owner.AttackTarget( targetID );

        //    // 모두공격 옵션이 켜져있다면 타겟팅 모두 바꾸기.
        //    if (ConcentratedMode)
        //    {
        //        List<Unit> allUnits = Owner.Owner.Units;
        //        for (int i = 0; i < allUnits.Count; i++)
        //        {
        //            if (Owner == allUnits[i])
        //                continue;

        //            allUnits[i].AttackTarget( targetID );
        //        }
        //    }
            
        //    //Owner.SetTarget( foundTarget.GetInstanceID() );
        //}

        return foundTarget;
    }

    public void ShowClickMoveEffect(Vector3 showPos)
    { 
        //ClickEffect.animation.Stop();
        //ClickEffect.transform.position = showPos;
        //ActiveAnimation.Play( ClickEffect.animation, null, AnimationOrTween.Direction.Forward, AnimationOrTween.EnableCondition.EnableThenPlay, AnimationOrTween.DisableCondition.DisableAfterForward);
    }

    public void ShowDragEffect(Vector3 pos)
    {
        //if (!DragEffect.activeSelf)
        //    DragEffect.SetActive( true );
        //DragEffect.transform.position = pos;
    }

    public void HideDragEffect()
    {
        //if (DragEffect.activeSelf)
        //    DragEffect.SetActive( false );
    }

    public void ShowTargetFx(int changedInstId)
    {
        // 타겟이 변경 되었거나 내 캐릭터의 타겟 정보만 처리한다
        if (Owner.TargetID == changedInstId)
            return;

        // despawn Fx
        if (null != TargetingEffect)
        {
            SelfDespawn.ClearPartileSystem( TargetingEffect );
            G_GameInfo.EffectPool.Despawn( TargetingEffect, G_GameInfo.EffectPool.group );
            TargetingEffect = null;
        }

        Unit Target = null;
        if (!G_GameInfo.CharacterMgr.InvalidTarget( changedInstId, ref Target ))
        {
            TargetingEffect = G_GameInfo.SpawnEffect( "Fx_Targeting", 1f, Target.transform, Target.transform, Owner.Model.ScaleVec3 );
            if (TargetingEffect.gameObject.layer != 0)
                NGUITools.SetChildLayer( TargetingEffect, 0 );
        }
    }
}
