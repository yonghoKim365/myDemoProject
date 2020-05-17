using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public partial class Player : Monster
{

	public const string LEO_ATTACK_EFFECT = "E_LEO_BLADE";


	sealed public override void render()
	{
		renderAni();

		if(GameManager.renderSkipFrame == false) _doRenderSkipFrame = _needPositionRender;
		
		if(_needPositionRender)
		{
			//prevPos = cTransform.position;
			//cTransform.position = VectorUtil.lerp(cTransform.position, cTransformPosition, GameManager.renderDeltaTime);
			cTransform.position = cTransformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			_needPositionRender = false;
			
//			_v = cTransform.position;
//			_v.x = cTransform.position.x;
//			_v.y = 0.5f;
//			_v.z = cTransform.position.z;
//			shadow.transform.position = _v;
		}
		else if(GameManager.renderSkipFrame && _doRenderSkipFrame)
		{
			//cTransform.position = VectorUtil.lerp(prevPos, cTransformPosition, GameManager.renderDeltaTime);
			cTransform.position = cTransformPosition * GameManager.renderRatio + prevTransformPosition * (1.0f -  GameManager.renderRatio);
			
//			_v = cTransform.position;
//			_v.x = cTransform.position.x;
//			_v.y = 0.5f;
//			_v.z = cTransform.position.z;
//			shadow.transform.position = _v;

		}


		updateChargingEffect();

		_needPositionRender = false;
		
	}

	public override void renderAni()
	{
		if(!string.IsNullOrEmpty(nowAniId) && ani.GetClip(nowAniId) != null)
		{
			if( (isDefaultAttacking && nowAniId.Contains(SHOOT_HEADER) == false) || 
			   (nowPlayingSkillAni && prevPlayingAniName.Contains(SKILL_HEAD) && nowAniId.Contains(SKILL_HEAD) == false))
			{
				if(pet != null && (nowAniId == WALK || nowAniId == NORMAL || nowAniId == BWALK))
				{
					if(!pet.ani.IsPlaying(nowAniId) || pet.prevPlayingAniName != nowAniId)
					{
						pet.ani.clip = pet.ani[nowAniId].clip;
						pet.ani.CrossFade(nowAniId, 0.2f);
						pet.prevPlayingAniName = nowAniId;
					}
				}
				else if(nowAniId == DEAD )
				{
					ani.clip = ani[nowAniId].clip;
					ani.CrossFade(nowAniId, 0.2f);
					prevPlayingAniName = nowAniId;
					
					if(pet != null)
					{
						pet.ani.CrossFade(nowAniId, 0.2f);
						pet.prevPlayingAniName = nowAniId;
					}
				}
			}
			else
			{
				if(!ani.IsPlaying(nowAniId) || prevPlayingAniName != nowAniId)
				{
					ani.clip = ani[nowAniId].clip;
					
					if(prevPlayingAniName.Contains(SKILL_HEAD) && pet != null && pet.ani.IsPlaying(nowAniId))
					{
						ani[nowAniId].time = pet.ani[nowAniId].time;
					}
					
					if(nowAniId == ATK)
					{
						switch(playerData.characterId)
						{
						case Character.ID.LEO:
							StartCoroutine(delayEffect(0.4f,LEO_ATTACK_EFFECT, 90.0f));
							break;
						}
					}
					
					
					ani.CrossFade(nowAniId, 0.2f);
					prevPlayingAniName = nowAniId;
					
					if(pet != null && nowAniId.Contains(SKILL_HEAD) == false) pet.ani.CrossFade(nowAniId, 0.2f);
				}
				else if(_setAniCount > 0)
				{
					_setAniCount = 0;
					ani.clip = animation[nowAniId].clip;
					ani.CrossFade(nowAniId, 0.2f);
					prevPlayingAniName = nowAniId;
					
					if(pet != null && nowAniId.Contains(SKILL_HEAD) == false)
					{
						pet.ani.CrossFade(nowAniId, 0.2f);
					}
				}
			}
		}
		
		nowAniId = string.Empty;
	}



	public override void renderAniRightNow()
	{
		if(!string.IsNullOrEmpty(nowAniId) && ani.GetClip(nowAniId) != null)
		{
//			if(!ani.IsPlaying(nowAniId))
			{
				ani.clip = ani[nowAniId].clip;
				
				if(nowAniId == ATK)
				{
					switch(playerData.characterId)
					{
					case Character.ID.LEO:
						StartCoroutine(delayEffect(0.4f,LEO_ATTACK_EFFECT, 90.0f));
						break;
					}
				}
				
				ani.CrossFade(nowAniId, 0.2f);
				prevPlayingAniName = nowAniId;
				
				if(pet != null && nowAniId.Contains(SKILL_HEAD) == false) pet.ani.CrossFade(nowAniId, 0.2f);
			}
		}
		
		nowAniId = string.Empty;
	}




	IEnumerator delayEffect(float waitTime, string id, float yOffset)
	{
		yield return new WaitForSeconds(waitTime);
	
		if(ani.clip.name == ATK)
		{
			if(isPlayerSide)
			{
				_v.x = 0; _v.y = 0; _v.z = 0;
			}
			else
			{
				_v.x = 0; _v.y = 180; _v.z = 0;
			}
			
			_q.eulerAngles = _v;

			_v = cTransform.position;
			_v.y = yOffset;

			GameManager.info.effectData[id].getEffect(-1000,_v,null,cTransform).transform.localRotation = _q;;
		}
	}




	// 죽음 애니메이션이 끝났을 때.
	sealed public override void onCompleteDeadAni()
	{
//		Debug.LogError("===  onCompleteDeadAni!!!  ====");
		//_state = DEAD_MOTION;

		changeShader(false);

		setColor(_normalColor);

		clearEffect();

		_isEnabled.Set( false );	

		container.gameObject.SetActive (true);
		if(pet != null) pet.cTransform.gameObject.SetActive(true);

		if(isPlayerSide) isDeleteObject = true;	

		if(npcData == null && playerTagIndex > -1)
		{
			GameManager.me.battleManager.startRelay(isPlayerSide, playerTagIndex);
		}
	}


	//----------------- 스킬 관련 -------------------//
	
	// loop 중인 애니중에 원상복귀 시키는...
	sealed public override void onCompleteLoopSkillAni()
	{
		if(_state2 == SKILL_LOOP)
		{
			nowAniId = Monster.SKILL_END;
			//fuckyouanimation.CrossFade(Monster.SKILL_END);	
			//StartCoroutine(onCompleteSkillAni(animation[Monster.SKILL_END].length, SKILL_END));
			getAniDelayMethod().onCompleteSkillAni(ani[Monster.SKILL_END].length, SKILL_END);
		}
		else
		{
			getAniDelayMethod().onCompleteSkillAni(0.01f, SKILL_END);
		}
	}	

	public void startSkillAniLinear(AttackData.PlayerShoot shoot, int heroSkillLevel, int applyReinforceLevel, float fadeTime = 0.2f)
	{
		_state2 = SKILL_FORWARD;
		aniFadeTime = fadeTime;
		nowPlayingSkillAni.Set( true );
		_tempAniData = GameManager.info.aniData[resourceId][SKILL_FORWARD];
		nowAniId = _tempAniData.ani;
		renderAniRightNow();
		//fuckyouanimation.CrossFade(_tempAniData.ani, aniFadeTime);
		//StartCoroutine(onSkillShoot(_tempAniData.delay[0], shoot, rareLevel, _tempAniData.ani));	
		//StartCoroutine(onCompleteSkillAni(animation[_tempAniData.ani].length, _tempAniData.ani));
		getAniDelayMethod().onSkillShoot(_tempAniData.delay[0], shoot, heroSkillLevel, applyReinforceLevel, _tempAniData.ani);
		getAniDelayMethod().onCompleteSkillAni(ani[_tempAniData.ani].length, _tempAniData.ani);

	}
	
	public void startSkillAniNormal(AttackData.PlayerShoot shoot, int heroSkillLevel, int applyReinforceLevel, float fadeTime = 0.2f)
	{
		_state2 = SKILL_NORMAL;
		aniFadeTime = fadeTime;
		nowPlayingSkillAni.Set( true );
		_tempAniData = GameManager.info.aniData[resourceId][SKILL_NORMAL];		
		nowAniId = SKILL_NORMAL;
		renderAniRightNow();
		//fuckyouanimation.CrossFade(SKILL_NORMAL, aniFadeTime);

		getAniDelayMethod().onSkillShoot(_tempAniData.delay[0], shoot, heroSkillLevel, applyReinforceLevel, _tempAniData.ani);
		getAniDelayMethod().onCompleteSkillAni(ani[_tempAniData.ani].length, _tempAniData.ani);
	}
	
	public void startSkillAniLoop(AttackData.PlayerShoot shoot, int heroSkillLevel, int applyReinforceLevel, float fadeTime = 0.2f)
	{
		_state2 = SKILL_START;
		aniFadeTime = fadeTime;
		nowPlayingSkillAni.Set( true );
		nowAniId = Monster.SKILL_START;
		renderAniRightNow();
		//fuckyouanimation.CrossFade(SKILL_START, aniFadeTime);

		getAniDelayMethod().startSkillAniLoop2(ani[SKILL_START].length, shoot, heroSkillLevel, applyReinforceLevel);
	}



	// 전방 및 일반 스킬을 쓴후 정상으로 돌아가는...
	sealed public override void onCompleteSkillAni(string playingAniId)
	{
		nowPlayingSkillAni.Set( false );
		state = Monster.NORMAL;
		moveType = MoveType.NORMAL;
		skillMoveIsNormal = true;
	}
	


	sealed public override void onSkillShoot(AttackData.PlayerShoot shoot, int heroSkillLevel, int applyReinforceLevel, string aniId)
	{
		setShootPosition(aniId);

		if(_hp > 0)
		{
			shoot(this, heroSkillLevel, applyReinforceLevel);
		}
	}	
	
	
	sealed public override void startSkillAniLoop2(AttackData.PlayerShoot shoot, int heroSkillLevel, int applyReinforceLevel)
	{
		if(_state2 == SKILL_START)
		{
			_state2 = SKILL_LOOP;
			nowAniId = SKILL_LOOP;
			//fuckyouanimation.CrossFade(SKILL_LOOP, aniFadeTime);
			setShootPosition(nowAniId);
			shoot(this, heroSkillLevel, applyReinforceLevel);
		}
		else nowPlayingSkillAni.Set( false );
	}

}



