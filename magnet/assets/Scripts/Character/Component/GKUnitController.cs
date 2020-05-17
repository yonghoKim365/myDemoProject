using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 필요 기능
/// 지형 터치 & 타겟 터치
/// EasyTouch이용한 이동 및 공격 기능
/// </remarks>
[DisallowMultipleComponent()]
public class GKUnitController : MonoBehaviour
{
    public Unit             Owner { private set; get; }
    public Quaternion       RotationFromJoyStick = Quaternion.identity;
    public Quaternion       RotationFromInterJoyStick = Quaternion.identity;

    //int             defaultLayer;
    //int             terrainLayer;
    //int             terrainOutsideLayer;

    Camera          BaseCamera;
    Transform       OwnerTrans;
    Joystick        JoyController;



    //public Vector3 TargetPos;//타일 방식에 사용되는 것. 가야할 목표지점을 저장해 놓는다.
    //int BeforePosX, BeforePosY;

    TownState _townState = null;

    void Awake()
    {
        Owner = GetComponent<Unit>();
        OwnerTrans = Owner.transform;

        //defaultLayer = LayerMask.NameToLayer( "Default" );
        //terrainLayer = LayerMask.NameToLayer( "Terrain" );
        //terrainOutsideLayer = LayerMask.NameToLayer( "Terrain_Outside" );

        _townState = SceneManager.instance.CurrentStateBase() as TownState;


    }

    void OnDestroy()
    {
        ActivateJoystick( false );
    }

    public void Stop()
    {
        StopJoystick();
    }

    public void ShowTargetFx(int changedInstId)
    {
        //// 타겟이 변경 되었거나 내 캐릭터의 타겟 정보만 처리한다
        //if (Owner.TargetID == changedInstId)
        //    return;

        //// despawn Fx
        //if (null != TargetingEffect)
        //{
        //    SelfDespawn.ClearPartileSystem( TargetingEffect );
        //    G_GameInfo.EffectPool.Despawn( TargetingEffect, G_GameInfo.EffectPool.group );
        //    TargetingEffect = null;
        //}

        //Unit Target = null;
        //if (!G_GameInfo.CharacterMgr.InvalidTarget( changedInstId, ref Target ))
        //{
        //    TargetingEffect = G_GameInfo.SpawnEffect( "Fx_Targeting", 1f, Target.transform, Target.transform, Owner.Model.ScaleVec3 );
        //    if (TargetingEffect.gameObject.layer != 0)
        //        NGUITools.SetChildLayer( TargetingEffect, 0 );
        //}
    }


    #region :: Joystick Control (조이스틱 이동 관련) ::

    /// <summary>
    /// 유닛을 조이스틱으로 컨트롤 되도록 한다.
    /// </summary>
    public void StartJoystick(Joystick joy, Camera _baseCamera = null)
    {
        if (null == _baseCamera)
        {
            if (null == BaseCamera)
                BaseCamera = Camera.main;
        }
        else
            BaseCamera = _baseCamera;

        JoyController = joy;
        ActivateJoystick( true );
    }

    public void StopJoystick()
    { 
        ActivateJoystick( false );
    }

    /// <summary>
    /// 조이스틱 활성화 유무
    /// </summary>
    public void ActivateJoystick(bool enable)
    {
        if (JoyController == null)
            return;

        // 조이스틱 보류됨.
        if (enable)
        {
            // 중복적용 방지하기 위한 코드
            /*
            EasyJoystick.On_JoystickMove -= On_JoystickMove;
            EasyJoystick.On_JoystickMove += On_JoystickMove;
            EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
            EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
            EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
            EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
            */
            JoyController.SetCallback(On_JoystickMoveStart, On_JoystickMove, On_JoystickMoveEnd);
        }
        else
        {
            /*
            EasyJoystick.On_JoystickMove -= On_JoystickMove;
            EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
            EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
            */
            
            JoyController.SetCallback(null, null, null);
        }
    }

	/// <summary>
    /// [Callback] 조이스틱으로 지정한 방향으로 이동하기.
    /// </summary>
    void On_JoystickMove(Joystick joy) //MovingJoystick move)
    {
        if (!Owner.NotChangeStateCehck())
		{
            if (Owner.StopState)
                On_JoystickMoveEnd(null);
            return;
        }

        if (Owner.FSM.Current() is SkillState || Owner.FSM.Current() is ManualAttackState)
        {
            if(Owner.FSM.Current() is SkillState && Owner.MoveableSkill == true)
            {
                //이동가능한 스킬
                RotationFromInterJoyStick = RotationFromJoystick(joy);
                return;
            }
            else
            {
                if (Owner.SkillBlend)
                {
                    //움직일수 있는 상태일경우 상태를 풀어주자
                    Owner.ChangeState(UnitState.Idle);
                }
                else
                {
                    On_JoystickMoveEnd(null);
                }

                if (Owner.FSM.Current() is SkillState)
                    return;
            }
        }

        if (Owner.ActiveAttack && !G_GameInfo._GameInfo.AutoMode)
        {
            //OwnerTrans.rotation = RotationFromJoystick(joy);
            RotationFromInterJoyStick = RotationFromJoystick(joy);
        }
        else
        {
            if (joy.IsMovingJoy())
            {
                OwnerTrans.rotation = RotationFromJoystick(joy);
                float SpeedRatio = 1f;
                Owner.StopState = Owner.MovePosition(OwnerTrans.position + (OwnerTrans.forward * 0.5f), SpeedRatio);
            }
        }
    }


    void On_JoystickMoveStart(Joystick joy)//MovingJoystick move)
    {
        Owner.StopState = true;
        RotationFromInterJoyStick = Quaternion.identity;

        if ( TownState.TownActive )
            SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
    }

    void On_JoystickMoveEnd(Joystick joy)//MovingJoystick move)
    {
        if (Owner.StopState != false)
        {
            Owner.StopState = false;
            RotationFromJoyStick = Quaternion.identity;
            RotationFromInterJoyStick = Quaternion.identity;
        }

        /*
        if (Owner.CurrentState == UnitState.Move)
        {
            Owner.ChangeState(UnitState.Idle);

            if(SceneManager.instance.IsRTNetwork)
                iFunClient.instance.ReqMoveEnd(Owner.m_rUUID, Owner.transform.position, Owner.transform.eulerAngles.y);
        }
        */
    }

    Quaternion RotationFromJoystick(Joystick joy)//MovingJoystick move)
    {
        float angle = joy.GetAngle();//move.Axis2Angle( true );
        //RotationFromJoyStick = Quaternion.Euler( 0, BaseCamera.transform.rotation.eulerAngles.y + angle, 0 );
        RotationFromJoyStick = Quaternion.Euler(0, (angle + BaseCamera.transform.rotation.eulerAngles.y), 0);
        return RotationFromJoyStick;
    }

    #endregion

}
