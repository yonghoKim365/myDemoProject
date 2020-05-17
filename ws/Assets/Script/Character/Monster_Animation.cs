using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Monster : MonoBehaviour
{
	public string nowAniId = string.Empty;

	public virtual void onCompleteDeadAni()
	{
		if(deleteMotionEffect != null)
		{
			if(deleteMotionEffect.type == EffectData.ResourceType.CHARACTER)
			{
				//ff Log.log("delete effect!" + name);
				deleteMotionEffect.effectChracter.setPositionCtransform(cTransformPosition);
				deleteMotionEffect.effectChracter.tf.position = tf.position;
				deleteMotionEffect.effectChracter.tf.rotation = tf.rotation;
				
				deleteMotionEffect.effectChracter.isEnabled = true;
				deleteMotionEffect.effectChracter.playAni(DEAD, onCompleteEffectDeadAni);
				getAniDelayMethod().onCompleteAnimation(deleteMotionEffect.effectChracter.ani[DEAD].length, onCompleteEffectDeadAni);

				deleteMotionEffect.effectChracter.renderAniRightNow();
				cTransform.gameObject.SetActive(false);
				return;
			}
		}

		_isEnabled.Set( false );
		isDeleteObject = true;
		_isThisEffectCharacter = false;
		//StartCoroutine(startDeadEffect());
	}


	bool canPlayAni()
	{
		if(ignoreIdleAni && (nowAniId.StartsWith("wa") || nowAniId.StartsWith("nor")))
		{
			return false;
		}

		return true;
	}


	public virtual void renderAniRightNow()
	{
		if(!string.IsNullOrEmpty(nowAniId) && ani.GetClip(nowAniId) != null)
		{
			ani.clip = ani[nowAniId].clip;

			if(canPlayAni())
			{
				ani.Rewind();
				ani.Play(nowAniId);
			}

			prevPlayingAniName = nowAniId;
			nowAniId = string.Empty;
		}
	}


	public virtual void renderAni()
	{
		if(!string.IsNullOrEmpty(nowAniId) && ani.GetClip(nowAniId) != null)
		{
			if(!ani.IsPlaying(nowAniId) || prevPlayingAniName != nowAniId )
			{
				if(ani.clip != null && ani[ani.clip.name].time > 0.1f)
				{
					ani.clip = ani[nowAniId].clip;

					if(canPlayAni())
					{
						ani.Play(nowAniId);
					}
				}
				else
				{
					ani.clip = ani[nowAniId].clip;

					if(canPlayAni())
					{
						ani.CrossFade(nowAniId, 0.1f);
					}
				}

				prevPlayingAniName = nowAniId;
			}
			else if(_setAniCount > 0)
			{
				_setAniCount = 0;
				ani.clip = animation[nowAniId].clip;

				if(canPlayAni())
				{
					ani.CrossFade(nowAniId, 0.1f);
				}

				prevPlayingAniName = nowAniId;
			}
		}
		
		nowAniId = string.Empty;

	}


	public virtual void render()
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

		_needPositionRender = false;

	}


	Vector3 prevPos = Vector3.zero;
	protected bool _doRenderSkipFrame = false;

	public void setPlayAniRightNow(string id)
	{
		_state = id;
		nowAniId = id;
	}

	protected int _setAniCount = 0;
	public void playAni(string id, float fadeTime = 0.2f)
	{
		#if UNITY_EDITOR
		//ff Log.log(resourceId, " id : " + id + "   hasani: " + hasAni + "    ui:  " + GameManager.me.uiManager.currentUI);
		#endif

		int i = 0;

		if(hasAni == false)
		{
			getAniDelayMethod().onCompleteAnimation(0.5f, id);
			//StartCoroutine(onCompleteAnimation(0.5f, id));
		}
		else
		{
			//fuckyou if(pet != null) pet.animation.CrossFade(id, aniFadeTime);
			aniFadeTime = fadeTime;

			if(id.Contains(SHOOT_HEADER))
			{
				setAniData(id);

//				Debug.Log(resourceId + "   " + _tempAniData.ani);

				float speedValue = (1.0f / ani[_tempAniData.ani].speed);

				if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY)
				{
#if UNITY_EDITOR
					//ff Log.log(resourceId, "_tempAniData.delayLength : " + _tempAniData.delayLength);
#endif

					for(i = 0; i < _tempAniData.delayLength; ++i)
					{
#if UNITY_EDITOR
						//ff Log.log(resourceId, "getAniDelayMethod().onAttackAnimation : " , _tempAniData.delay[i], id, i, _tempAniData.ani);
#endif
						getAniDelayMethod().onAttackAnimation(_tempAniData.delay[i] * speedValue, id, i, _tempAniData.ani, this);
						//StartCoroutine(onAttackAnimation(_tempAniData.delay[i], id, i, _tempAniData.ani));	
					}

#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
					{
					}
					else
#endif
					{
						for(i = 0; i < _tempAniData.effectNum; ++i)
						{
							StartCoroutine(onAniEffect(_tempAniData.effect[i].delay * speedValue , _tempAniData.effect[i], _tempAniData.id, i, _tempAniData.ani));
						}

						for(i = 0; i < _tempAniData.camEffectNum; ++i)
						{
							StartCoroutine(onAniCamEffect(_tempAniData.camEffect[i].delay * speedValue, _tempAniData.camEffect[i], _tempAniData.ani));
						}

						for(i = 0; i < _tempAniData.soundNum; ++i)
						{
							StartCoroutine(onAniSound(_tempAniData.sound[i].delay * speedValue, _tempAniData.sound[i].id));
						}



					}

				}			

				++_setAniCount;
				nowAniId = _tempAniData.ani;

				if(GameManager.info.soundData.TryGetValue(resourceId, out SoundData.tempSoundData))
				{
					int atkNum = 0;

					if(Monster.ATK_INDEX.TryGetValue(_tempAniData.ani, out atkNum))
					{
						if(isPlayer)
						{
							SoundData.playPlayerAttackSound(SoundData.tempSoundData, playerData.characterId);
						}
						else
						{
							SoundData.playAttackSound(monsterData.resource,  atkNum);
						}
					}
				}

				getAniDelayMethod().onCompleteAnimation(ani[_tempAniData.ani].length * speedValue , id);
				//StartCoroutine(onCompleteAnimation(animation[_tempAniData.ani].length, id));
			}
			else if(id == DEAD)
			{
				_setAniCount = 0;

				//fuckyouanimation.CrossFade(id, aniFadeTime);
				if(ani.GetClip(id))
				{
					nowAniId = id;

					renderAni();

					getAniDelayMethod().onCompleteAnimation(ani[id].length * 1.1f, id);
				}
				else
				{
					getAniDelayMethod().onCompleteAnimation(0.1f, id);
				}



				//StartCoroutine(onCompleteAnimation(animation[id].length, id));
			}
			else
			{
				_setAniCount = 0;
				//if(animation.clip == animation[id].clip && animation.isPlaying && animation[id].clip.isLooping) return;
				//if(animation.clip == animation[id].clip && animation.isPlaying && animation[id].clip.isLooping) return;
				nowAniId = id;

			}
		}
	}	
	

	public delegate void DeadCallback(int uniqueId);
	public DeadCallback monsterDeadCallback;


	public Vector3 effectTargetPosition = new Vector3();

	IEnumerator onAniEffect(float effDelay, AniDataEffect ade, string aniDataId, int shotPointIndex, string aniId)
	{
		yield return new WaitForSeconds(effDelay);

#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useEffectSkillCamEditor)
		{
			if(ade.useThis == false) yield break;
		}
#endif

		if(_isEnabled)
		{
			switch(ade.shotPoint)
			{
			case AniDataEffect.PointType.ShotPoint:
				GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, Monster.setShootingHand(this), stat.uniqueId, this, null, null, ade.timeLimit);
				break;
			case AniDataEffect.PointType.CustomPoint:

				GameObject cpGo = GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, Monster.setShootingHandWithCustomValue(this,ade.x,ade.y,ade.z) , stat.uniqueId, this, null, null, ade.timeLimit);

				if(cpGo != null)
				{
					if(ade.useCustomRotation)
					{
						_v.x = ade.rx;
						_v.y = ade.ry;
						_v.z = ade.rz;
					}
					else
					{
						_v.x = 0.0f;
						_v.y = 0.0f;
						_v.z = 0.0f;
					}
					
					_q.eulerAngles = _v;
					cpGo.transform.localRotation = _q;
				}

				break;
			case AniDataEffect.PointType.CustomBullet:
				GameObject cbGo = GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, Monster.setShootingHandWithCustomValue(this,ade.x,ade.y,ade.z) , stat.uniqueId, this, null, null, ade.timeLimit);
				if(cbGo != null)
				{
					if(ade.useCustomRotation)
					{
						_v.x = ade.rx;
						_v.z = ade.rz;

						if(isPlayerSide)
						{
							_v.y = 90.0f + ade.ry;
						}
						else
						{
							_v.y = 270.0f + ade.ry;
						}
					}
					else
					{
						_v.x = 0.0f;
						_v.z = 0.0f;

						if(isPlayerSide)
						{
							_v.y = 90.0f;
						}
						else
						{
							_v.y = 270.0f;
						}
					}

					_q.eulerAngles = _v;
					cbGo.transform.localRotation = _q;
				}
				break;

			case AniDataEffect.PointType.Target:
				if(target != null)
				{
					GameObject tgo = GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, target.cTransformPosition , stat.uniqueId, this, null, null, ade.timeLimit);
					if(tgo != null)
					{
						_q = tgo.transform.localRotation;
						_v = _q.eulerAngles;
						_v.x = 0.0f;
						_v.y = 90.0f;
						_v.z = 0.0f;
						_q.eulerAngles = _v;
						tgo.transform.localRotation = _q;
					}
				}
				else if(effectTargetPosition.y > -500)
				{
					GameObject tgo2 = GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, effectTargetPosition , stat.uniqueId, this, null, null, ade.timeLimit);

					if(tgo2 != null)
					{
						_q = tgo2.transform.localRotation;
						_v = _q.eulerAngles;
						_v.x = 0.0f;
						_v.y = 90.0f;
						_v.z = 0.0f;
						_q.eulerAngles = _v;
						tgo2.transform.localRotation = _q;
					}
				}
				else
				{
					GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, Monster.setShootingHandWithCustomValue(this,ade.x,ade.y,ade.z) , stat.uniqueId, this, null, null, ade.timeLimit);
				}
				break;

			case AniDataEffect.PointType.AttachedShotPoint:
				GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, Monster.setShootingHandWithCustomValueWithoutMonsterPosition(this,ade.x,ade.y,ade.z) , stat.uniqueId, this, getShootHand(aniId,shotPointIndex), null, ade.timeLimit, true);
				break;


			case AniDataEffect.PointType.AttachedTransform:

				if(effectParents != null && ade.parent != null && effectParents.ContainsKey(ade.parent))
				{
					GameObject goAt = GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, 
					                                                                Monster.setShootingHandWithCustomValueWithoutMonsterPosition(this,ade.x,ade.y,ade.z) , 
					                                                                stat.uniqueId, this, effectParents[ade.parent], null, ade.timeLimit, true);


					if(goAt != null)
					{
						if(ade.useCustomRotation)
						{
							_v.x = ade.rx;
							_v.y = ade.ry;
							_v.z = ade.rz;
						}
						else
						{
							_v.x = 0.0f;
							_v.y = 0.0f;
							_v.z = 0.0f;
						}
						
						_q.eulerAngles = _v;
						goAt.transform.localRotation = _q;
					}



				}

				break;


			default:
				GameManager.info.effectData[ade.id].getEffect(stat.uniqueId, cTransform.position , stat.uniqueId , this, null, null, ade.timeLimit);
				break;
			}
		}
	}




	IEnumerator onAniSound(float soundDelay, string soundId)
	{
		yield return new WaitForSeconds(soundDelay);
		
		if(_isEnabled)
		{
			SoundData.play(soundId);
		}
	}



	IEnumerator onAniCamEffect(float effDelay, AniDataCamEffect ace, string aniDataId)
	{
		yield return new WaitForSeconds(effDelay);
		
		if(_isEnabled)
		{
			if(_state == aniDataId)
			{
				switch(ace.type)
				{
				case AniDataCamEffect.Type.Quake:
					GameManager.me.effectManager.quakeEffect(ace.optionValue,ace.optionValue2, EarthQuakeEffect.Type.Mad);
					break;
				}
			}
		}
	}


	
	
	public void onAttack (int totalDamageNum, int targetX, int targetY, int targetZ, int targetH)
	{
		action.onAttack(totalDamageNum, targetX, targetY, targetZ, targetH);
	}
	
	public void onCompleteAttackAni (bool isClearActionType = false)
	{
//		Log.log("onCompleteAttackAni",this); //ff
		isDefaultAttacking.Set( false );
		state = NORMAL;
		action.onCompleteAttackAni(isClearActionType);
		effectTargetPosition.y = -1000;
	}
	
	
	public virtual void onSkillShoot(AttackData.PlayerShoot shoot, int skillLevel, int applyReinforceLevel, string aniId)
	{
	}	
	
	public virtual void startSkillAniLoop2(AttackData.PlayerShoot shoot, int skillLevel, int applyReinforceLevel)
	{
	}

	public virtual void onCompleteSkillAni(string playingAniId)
	{
	}



	public void updateAnimationMethod()
	{
		for(int i = _aniDelayMethods.Count - 1; i >= 0; --i)
		{
			if(_aniDelayMethods[i].isEnabled == false)
			{
				_aniDelayMethodPool.Push(_aniDelayMethods[i]);
				_aniDelayMethods.RemoveAt(i);
			}
			else if(_aniDelayMethods[i].update())
			{
				_aniDelayMethods[i].start(this);
				_aniDelayMethods[i].isEnabled = false;
				_aniDelayMethodPool.Push(_aniDelayMethods[i]);
				_aniDelayMethods.RemoveAt(i);
			}
		}
	}
	
	public void clearAnimationMethod(bool isClear = false)
	{
		if(isClear)
		{
			for(int i = _aniDelayMethods.Count - 1; i >= 0; --i)
			{
				_aniDelayMethods[i].isEnabled = false;
				_aniDelayMethodPool.Push(_aniDelayMethods[i]);
			}
			_aniDelayMethods.Clear();
		}
		else
		{
			for(int i = _aniDelayMethods.Count - 1; i >= 0; --i)
			{
				_aniDelayMethods[i].isEnabled = false;
			}
		}
	}





	public void clearOnAttackAnimationMethod()
	{
		for(int i = _aniDelayMethods.Count - 1; i >= 0; --i)
		{
			if(_aniDelayMethods[i].type != MonsterDelayMethod.TYPE.onCompleteAnimation1 
			   || (_aniDelayMethods[i].type != MonsterDelayMethod.TYPE.onCompleteAnimation2 && _aniDelayMethods[i].currentState != Monster.DEAD) )
			{
				_aniDelayMethods[i].isEnabled = false;
//				_aniDelayMethodPool.Push(_aniDelayMethods[i]);
//				_aniDelayMethods.RemoveAt(i);
			}
		}
	}

	
	List<MonsterDelayMethod> _aniDelayMethods = new List<MonsterDelayMethod>();

	static Stack<MonsterDelayMethod> _aniDelayMethodPool = new Stack<MonsterDelayMethod>();
	
	protected MonsterDelayMethod getAniDelayMethod()
	{
		#if UNITY_EDITOR
//		Log.logError("getAniDelayMethod " + this);
		#endif

		MonsterDelayMethod dm;

		if(_aniDelayMethodPool.Count > 0)
		{
			dm = _aniDelayMethodPool.Pop();
		}
		else dm = new MonsterDelayMethod();

		_aniDelayMethods.Add(dm);

		return dm;
	}

}


public class MonsterDelayMethod
{
	public enum TYPE
	{
		onAttackAnimation, 
		onCompleteAnimation1,
		onCompleteAnimation2,
		onCompleteSkillAnimation,
		onCompleteSkillAni, 
		onSkillShoot,
		startSkillAniLoop2,
		none
	}
	
	public string state = Monster.NORMAL;
	
	public TYPE type = TYPE.none;
	public string currentState;

	IFloat _waitTime;
	int _nowIndex;

	int _applyReinforceLevel;
	int _skillLevel;
	string _id;

	Monster.Callback _callback;
	AttackData.PlayerShoot _shoot;

	bool _isEnabled = false;

	int _targetX = -1000;
	int _targetY = -1000;
	int _targetZ = -1000;
	int _targetH = -1000;


	public void onAttackAnimation(float waitTime, string state, int nowIndex, string id, Monster mon)
	{
		_isEnabled = true;
		type = TYPE.onAttackAnimation;
		_waitTime = waitTime;
		currentState = state;
		_nowIndex = nowIndex;
		_id = id;

		_targetX = mon.targetPosition.x.AsInt();
		_targetY = mon.targetPosition.y.AsInt();
		_targetZ = mon.targetPosition.z.AsInt();
		_targetH = mon.targetHeight.AsInt();

	}
	
	public void onCompleteAnimation(float waitTime, Monster.Callback callback = null)
	{
		_isEnabled = true;
		type = TYPE.onCompleteAnimation1;
		_waitTime = waitTime;
		_callback = callback;

	}
	
	public void onCompleteAnimation(float waitTime, string state)
	{
		_isEnabled = true;
		type = TYPE.onCompleteAnimation2;
		_waitTime = waitTime;
		currentState = state;
	}
	
	public void onCompleteSkillAnimation(float waitTime, string state)
	{
		_isEnabled = true;
		type = TYPE.onCompleteSkillAnimation;
		_waitTime = waitTime;
		currentState = state;
	}
	
	public void onCompleteSkillAni(float waitTime, string id)
	{
		_isEnabled = true;
		type = TYPE.onCompleteSkillAni;
		_waitTime = waitTime;
		_id = id;
	}


	public void onSkillShoot(float waitTime, AttackData.PlayerShoot shoot, int skillLevel, int applyReinforceLevel, string id, float fadeTime = 0.2f)
	{
		_isEnabled = true;
		type = TYPE.onSkillShoot;
		_waitTime = waitTime;
		_shoot = shoot;
		_skillLevel = skillLevel;

		_applyReinforceLevel = applyReinforceLevel;
		_id = id;

	}


	public void startSkillAniLoop2(float waitTime, AttackData.PlayerShoot shoot, int skillLevel, int applyReinforceLevel, float fadeTime = 0.2f)
	{
		_isEnabled = true;
		type = TYPE.startSkillAniLoop2;
		_shoot = shoot;
		_skillLevel = skillLevel;

		_applyReinforceLevel = applyReinforceLevel;
		_waitTime = waitTime;
		
	}



	public bool update()
	{
		_waitTime -= GameManager.globalDeltaTime;

		if(_waitTime <= 0) return true;

		return false;
	}
	
	public void start(Monster mon)
	{
#if UNITY_EDITOR
		//Log.log("ani method start! : " + type +  "     " + mon.resourceId); //ff 
#endif

		switch(type)
		{
		case TYPE.onAttackAnimation:
			mon.onAttackAnimation(currentState, _nowIndex, _id, _targetX, _targetY, _targetZ, _targetH);
			break;
			
		case TYPE.onCompleteAnimation1:
			mon.onCompleteAnimation(_callback);
			break;
			
		case TYPE.onCompleteAnimation2:
			mon.onCompleteAnimation(currentState);
			break;
			
		case TYPE.onCompleteSkillAnimation:
			mon.onCompleteSkillAnimation(currentState);
			break;
			
		case TYPE.onCompleteSkillAni:
			mon.onCompleteSkillAni(_id);
			break;
			
		case TYPE.onSkillShoot:
			mon.onSkillShoot(_shoot, _skillLevel, _applyReinforceLevel, _id);
			break;
			
		case TYPE.startSkillAniLoop2:
			mon.startSkillAniLoop2(_shoot, _skillLevel, _applyReinforceLevel);
			break;

		}
	}


	public void changeWaitTime(IFloat value)
	{
		if(_isEnabled && _waitTime > 0)
		{
			_waitTime = _waitTime / value;
		}
	}

	
	public bool isEnabled
	{
		set
		{
			if(value == false)
			{
				_isEnabled = false;
				_waitTime = 0.0f;
				currentState = null;
				_nowIndex = 0;
				_skillLevel = 1;
				_applyReinforceLevel = 0;

				_id = null;
			
				_callback = null;
				_shoot = null;
			}
		}
		get
		{
			return _isEnabled;
		}
	}
}