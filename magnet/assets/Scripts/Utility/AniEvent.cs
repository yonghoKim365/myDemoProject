using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GameObject에 존재하는 애니메이션의 AnimationEvent가 호출할 수 있는 함수들
/// </summary>
public class AniEvent : MonoBehaviour
{
    public Unit MyUnit;
    public GameObject MyObj;
	public GameObject FocusCam;

    public System.Action<int> callBack;

    public void Attack(float DamageRatio = 1.0f)
    {
        if (null == MyUnit)
            return;

        if (MyUnit.CurrentState == UnitState.Dying || MyUnit.CurrentState == UnitState.Dead || MyUnit.CharInfo.Hp <= 0)
            return;

        //Debug.Log( GetType() + ".Attack : " + DamageRatio );

        MyUnit.AttackEvent(DamageRatio);
    }

    public void AttackEnd()
    {
        if (null == MyUnit)
            return;
        //Debug.Log(GetType() + ".AttackEnd : ");
        
        MyUnit.PreEndAttackAnim = true;
    }

    public void DieEnd()
    {
        if (null == MyUnit)
            return;
        //Debug.Log(GetType() + ".DieEnd : ");
    }

    /// 이미 스킬애니메이션은 시작됐고, 실제 스킬 데이터가 적용되어야 하는 시점에 불리는 함수.
    public void SkillUse(int verifyId)
    {
        if (MyUnit == null)
            return;

        if (MyUnit.CurrentState == UnitState.Dying || MyUnit.CurrentState == UnitState.Dead || MyUnit.CharInfo.Hp <= 0)
            return;

        if (callBack != null)
            callBack(verifyId);


        if (!SceneManager.instance.IsRTNetwork || (SceneManager.instance.IsRTNetwork && (MyUnit.m_rUUID == NetData.instance._userInfo._charUUID)))
        {
            if (MyUnit.CurrentState == UnitState.Attack || MyUnit.CurrentState == UnitState.ManualAttack)
            {
                AbilityData ability = MyUnit.SkillCtlr.__SkillGroupInfo.GetAbility(MyUnit.UseSkillIdx, (uint)verifyId);

                if (ability != null)
                    SkillAbility.ActiveSkill(ability, MyUnit, true);
                else
                    Debug.Log("Notfound AbilityData : " + MyUnit.UseSkillIdx + ", " + verifyId);

                //MyUnit.StartSkill((uint)verifyId, true);
            }
            else if (MyUnit.CurrentState == UnitState.Skill)
            {
                AbilityData ability = MyUnit.SkillCtlr.__SkillGroupInfo.GetAbility(MyUnit.UseSkillIdx, (uint)verifyId);

                if (ability != null)
                    SkillAbility.ActiveSkill(ability, MyUnit, false);
                else
                    Debug.Log("Notfound AbilityData : " + MyUnit.UseSkillIdx + ", " + verifyId);

                //MyUnit.StartSkill((uint)verifyId, false);
            }
        }

		Sw.PMsgRoleAttackRecvS attackData = MyUnit.DamageDequeue();
		
		if(attackData != null)
		{
			//표시할 데이터가 있으면 일단 그냥 표시
			for (int i = 0; i < attackData.CInfo.Count; i++)
			{
				Unit target = null;
                Unit attacker = null;
				
				if (attackData.CInfo[i].UnTargetType == (int)Sw.ROLE_TYPE.ROLE_TYPE_USER)
				{
					target = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)attackData.CInfo[i].UllTargetId);
				}
				else if (attackData.CInfo[i].UnTargetType == (int)Sw.ROLE_TYPE.ROLE_TYPE_NPC)
				{
					target = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)attackData.CInfo[i].UllTargetId);
				}

                if (attackData.UnAttackerType == (int)Sw.ROLE_TYPE.ROLE_TYPE_USER)
                {
                    attacker = G_GameInfo.CharacterMgr.FindRoomUnit((ulong)attackData.UllAttackerId);
                }
                else if (attackData.UnAttackerType == (int)Sw.ROLE_TYPE.ROLE_TYPE_NPC)
                {
                    attacker = G_GameInfo.CharacterMgr.FindRoomNPC((ulong)attackData.UllAttackerId);
                }

                if(attacker != null)
                {
                    attacker.CharInfo.SuperArmor = (uint)attackData.UnSuperArmor;
                }

                if (target != null)
				{

					if (attackData.CInfo[i].UnDodge == 1)
					{
						//회피
						if (G_GameInfo.GameInfo.BoardPanel != null)
							G_GameInfo.GameInfo.BoardPanel.ShowBuff(target.gameObject, target.gameObject, _LowDataMgr.instance.GetStringCommon(21));
					}
					else if (attackData.CInfo[i].UnCritical == 1)
					{
						
						if (NetData.instance._userInfo._charUUID == (ulong)attackData.CInfo[i].UllTargetId)
						{
							//해당유닛이 내유닛이다.
							
							target.ShowDamagedFx(target, (int)attackData.CInfo[i].UnDamage, true, true, false, eDamResistType.None);
						}
						else
						{
							//아니다
							target.ShowDamagedFx(target, (int)attackData.CInfo[i].UnDamage, false, true, false, eDamResistType.None);
						}
                        target.StartFlickerFx();
                        target.CharInfo.Hp = attackData.CInfo[i].UnTargetHp;
                    }
					else
					{
						
						if (NetData.instance._userInfo._charUUID == (ulong)attackData.CInfo[i].UllTargetId)
						{
							//해당유닛이 내유닛이다.
							target.ShowDamagedFx(target, (int)attackData.CInfo[i].UnDamage, true, false, false, eDamResistType.None);
						}
						else
						{
							target.ShowDamagedFx(target, (int)attackData.CInfo[i].UnDamage, false, false, false, eDamResistType.None);
						}
                        target.StartFlickerFx();
                        target.CharInfo.Hp = attackData.CInfo[i].UnTargetHp;
                    }


                    target.CharInfo.SuperArmor = (uint)attackData.CInfo[i].UnTargetSuperArmor;
                }
			}
		}


        /*
        if (MyUnit.CurrentState == UnitState.Attack || MyUnit.CurrentState == UnitState.ManualAttack)
        {
            //< 콤보 예외처리
            int combo = MyUnit.CurCombo;
            if(MyUnit.CurrentState == UnitState.Attack || MyUnit.CurrentState == UnitState.ManualAttack)
            {
                if (MyUnit.CurCombo == 1)
                    combo = 1;
                else if (MyUnit.CurCombo == 2)
                    combo = 2;
                else if (MyUnit.CurCombo == 3)
                    combo = 3;
                else if (MyUnit.CurCombo == 0)
                    combo = 4;

                //Debug.Log(string.Format("combo count:{0}, MyUnit.CurCombo:{1}", combo, MyUnit.CurCombo));
            }

            //< 스킬 시전
            //MyUnit.StartSkill(0, (uint)combo, true);

            //MyUnit.StartSkill(0, (uint)combo);
            Debug.Log(string.Format("verifyId:{0}, MyUnit.UseSkillSlot:{1}", verifyId, MyUnit.UseSkillSlot));
            MyUnit.StartSkill((uint)verifyId, true);
        }
        else if (MyUnit.CurrentState == UnitState.Skill)
        {
            //< 스킬을 시전한다.
            //Debug.Log("UseSkillSlot " + MyUnit.UseSkillSlot + " , verifyId " + verifyId);
            MyUnit.StartSkill((uint)verifyId, false);
        }
        */
    }

    //public void ForGetList_SkillUse(int verifyId, UnitState CurrentState)
    //{
    //    if (MyUnit == null)
    //        return;

    //    //if (SceneManager.isRTNetworkMode != GAME_MODE.FREEFIGHT)
    //    if ( !SceneManager.instance.IsRTNetwork)
    //        return;

    //    if (MyUnit.Owner.m_LoginUUID != SceneManager.g_LoginUUID) //자기가 조정하는 캐릭터가 아니라면(컨트롤 이벤트를 발생시키는 놈이 아니라면)
    //        return;

    //    if (CurrentState == UnitState.Dying || CurrentState == UnitState.Dead || MyUnit.hp <= 0)
    //        return;

    //    if (callBack != null)
    //        callBack(verifyId);

    //    if (CurrentState == UnitState.Attack || CurrentState == UnitState.ManualAttack)
    //    {
    //        //Debug.Log(string.Format("UseSkillSlot:{0}, verifyId:{1}", MyUnit.UseSkillSlot, verifyId));
    //        MyUnit.StartSkill((uint)verifyId, true);
    //    }
    //    else if (CurrentState == UnitState.Skill)
    //    {
    //        //Debug.Log(string.Format("UseSkillSlot:{0}, verifyId:{1}", MyUnit.UseSkillSlot, verifyId));
    //        MyUnit.StartSkill((uint)verifyId, false);
    //    }

    //    /*
    //    if (CurrentState == UnitState.Attack || CurrentState == UnitState.ManualAttack)
    //    {
    //        //< 콤보 예외처리
    //        int combo = MyUnit.CurCombo;
    //        if (CurrentState == UnitState.Attack || CurrentState == UnitState.ManualAttack)
    //        {
    //            if (MyUnit.CurCombo == 1)
    //                combo = 1;
    //            else if (MyUnit.CurCombo == 2)
    //                combo = 2;
    //            else if (MyUnit.CurCombo == 3)
    //                combo = 3;
    //            else if (MyUnit.CurCombo == 0)
    //                combo = 4;
    //        }

    //        //< 스킬 시전
    //        MyUnit.StartSkill((uint)combo, true);
    //    }
    //    else if (CurrentState == UnitState.Skill)
    //    {
    //        //< 스킬을 시전한다.
    //        //Debug.Log("UseSkillSlot " + MyUnit.UseSkillSlot + " , verifyId " + verifyId);
    //        MyUnit.StartSkill((uint)verifyId, false);
    //    }
    //    */
    //}

    public void SkillUse_Child(int childGroupID)
    {
        if (MyUnit == null)
            return;

        //uint actionID = MyUnit.SkillCtlr.GetChildSkillAction( (uint)childGroupID );
        //if (0 != actionID)
        //    SkillAbility.DoAction( actionID, MyUnit );
    }

    public void SkillEnd()
    {
        if (null == MyUnit)
            return;

        //Debug.Log(GetType() + ".SkillEnd : ");

        // KRootMotion 중에 공격 상태로 바로 바뀌면 문제가되서, SkillState Exit()에서 체크해서 공격 바로하도록 함.

        //if (MyUnit.TargetID != GameDefine.unassignedID)
        //{
        //    MyUnit.AttackTarget( MyUnit.TargetID );
        //}
        //else
        //{
        //    MyUnit.ChangeState( UnitState.Idle );
        //}
    }

    public void PlaySound(int soundID)
    {
	   switch (soundID)
	   {
		  // 발자국 소리
		  case 1000:
			 //SoundHelper.TempPlayIngameSound(MyUnit.SpwanUnitId, SoundHelper.eSoundType.foot);
			 break;
	   }

        //if (MyUnit.FSM.Current() is SkillState)
        //    MyUnit.SkillSounds.Add(SoundHelper.PlaySfxSound((uint)soundID, 1.5f));
        //else
        //    SoundHelper.PlaySfxSound((uint)soundID, 1.5f);
    }

    public void CameraShake(float ShakePower)
    {
        //Debug.Log(GetType() + ".CameraShake : " + ShakePower);

        CameraManager.instance.Shake( Vector3.right * ShakePower, 1 );
    }

	public void CameraShakeDefault()
	{
		//Debug.Log(GetType() + ".CameraShake : " + ShakePower);
		
		CameraManager.instance.ShakeDefault();
	}

	public void cutSceneCamShake(){
		CameraManager.instance.cutSceneCamShake ();
	}

	public void CutSceneCamShakeAndSlow(float _slowDelay){
		CameraManager.instance.cutSceneCamShake ();
		startSlowEffect (_slowDelay);
	}

	public void CutSceneCamShakeSlowAndFinish(float _effectTime){
		CameraManager.instance.cutSceneCamShake ();
		startSlowEffect (_effectTime);
		TempCoroutine.instance.FrameDelay(_effectTime, () =>{
			Time.timeScale = GameDefine.DefaultTimeScale;

			if (SceneManager.instance.testData.bSingleSceneTestStart && SceneManager.instance.testData.bQuestTestStart){
				Time.timeScale = 5f;
			}
			CutSceneMgr.stopBossCutScene ();
		});

	}

	public void FinishCutScene(){
		CutSceneMgr.stopBossCutScene ();
	}

	public void CutSceneGlassCrash(float _delay){
		// effet not yet ready.
		Debug.Log (" cutscene glass crash effect");
	}

	public void CutSceneEffect(int idx){
		// effet not yet ready.
		Debug.Log (" cutscene effect, idx:" + idx);
	}

    public void SkillMove(string SpawnProjectile)
    {


        Time.timeScale = 0.1f;
        TempCoroutine.instance.FrameDelay(0.06f, () =>
        {
            Time.timeScale = GameDefine.DefaultTimeScale;
        });
    }


	// only for cutscene.
	public void startSlowEffect(float _delay){

		if (CutSceneMgr.isBossCutSceneProgress == false)
			return;

		Time.timeScale = 0.1f;

		if (GameObject.Find ("CutSceneCameraMover") != null){
			CutSceneSeqHelper cst = GameObject.Find ("CutSceneCameraMover").GetComponent<CutSceneSeqHelper> ();
			if (cst != null){
				Time.timeScale = cst.time_slowEffectSped;
			}
		}

		TempCoroutine.instance.FrameDelay(_delay, () => {
			Time.timeScale = GameDefine.DefaultTimeScale;
		});
	}
    
    /// 모든 액션의 시작 (발사체, 공격, 버프 등등)
    public void DoAction(int actionID)
    {
//        Debug.Log( GetType() + ".DoAction : " + actionID );

        //SkillAbility.DoAction( (uint)actionID, MyUnit );
    }

    //준비 종료시간체크 - 이시간이후로는 디스트럽안됨
    public void PrepareEnd()
    {

    }

    //모션 블랜딩가능한 시간시작
    public void MergeTime()
    {
		if (MyUnit == null)
			return;

        MyUnit.SkillBlend = true;

        if (NetData.instance._userInfo._charUUID == MyUnit.m_rUUID)
        {
            if (MyUnit.ReservedSkillIdx != 0 /*&& MyUnit.ReservedSkillTime + 1f >= Time.time*/)
            {
                MyUnit.UseSkill((int)MyUnit.ReservedSkillIdx);
            }
        }
    }

    public void StartMoveable(float speed)
    {
        //if(NetData.instance._userInfo._charUUID == MyUnit.m_rUUID)
        {
            MyUnit.MoveableSkill = true;

            if(MyUnit.FSM.Current() is SkillState)
                (MyUnit.FSM.Current() as SkillState).moveSpeedRatio = speed;
        }
    }

    public void EndMoveable()
    {
        //if(NetData.instance._userInfo._charUUID == MyUnit.m_rUUID)
        {
            MyUnit.MoveableSkill = false;
        }
    }

    public void SpawnFx(string effectName)
    {
        //Vector3 scale = MyUnit ? MyUnit.Model.ScaleVec3 : Vector3.one;
        //G_GameInfo.SpawnEffect( effectName, 1, MyUnit.transform, null, scale );

        if (MyUnit == null)
            return;

        // 파싱 체크
        //if (effectName.Contains("TerFx_"))
        //{
        //    effectName = effectName.Replace("TerFx_", "");

        //    //< 유닛의 속성을 얻는다
        //    if(MyUnit is Pc)
        //    {
        //        effectName += LowDataMgr.GetUnitData((MyUnit as Pc).syncData.LowID).property.ToString();
        //        MyUnit.SpawnEffect(effectName, 1, MyUnit.transform);
        //    }
        //    else if(MyUnit is Npc)
        //    {
        //        effectName += LowDataMgr.GetUnitData((MyUnit as Npc).enemyInfo.UnitId).property.ToString();
        //        MyUnit.SpawnEffect(effectName, 1, MyUnit.transform);
        //    }
        //}
        //else
        //    MyUnit.SpawnEffect(effectName, 1, MyUnit.transform);

        MyUnit.SpawnEffect(effectName, 1, MyUnit.transform);
    }

    public void FootEvent()
    {
        if (MyUnit == null)
            return;

        if (MyUnit.CurrentState != UnitState.Move)
            return;

        string effName = MyUnit.Animator.GetAnimationEffect(MyUnit.Animator.CurrentAnim);
        if( string.IsNullOrEmpty(effName) )
        {
            effName = "Fx_pc_moving_effect";
        }

        if(!effName.Equals("0"))
        {
            if (TownState.TownActive)
            {
                uint soundID = MyUnit.Animator.GetAnimationSound(MyUnit.Animator.CurrentAnim);

                if (soundID != 0)
                {
                    SoundManager.instance.PlaySfxUnitSound(soundID, MyUnit._audioSource, MyUnit.cachedTransform, true);
                }

                TownState.SpawnEffect(effName, MyUnit.cachedTransform.position, Quaternion.Euler(Vector3.zero));
            }
            else
            {
                uint soundID = MyUnit.Animator.GetAnimationSound(MyUnit.Animator.CurrentAnim);

                if (soundID != 0)
                {
                    SoundManager.instance.PlaySfxUnitSound(soundID, MyUnit._audioSource, MyUnit.cachedTransform);
                }

                G_GameInfo.SpawnEffect(effName, MyUnit.cachedTransform.position, Quaternion.Euler(Vector3.zero));
            }
        }
    }

	void GlassCrack(int imgnum)
	{
		FocusCam = GameObject.Find ("FocusingCamera(Clone)");
        if (FocusCam != null)
		    FocusCam.GetComponent<FocusingCamera>().SendMessage("EnableCrack",imgnum);
	}

	public void CamShake()
	{
		FocusCam = GameObject.Find ("FocusingCamera(Clone)");
        if (FocusCam != null)
            FocusCam.GetComponent<FocusingCamera>().Explosion();
	}
}