using UnityEngine;
using System.Collections;

public class UIPlayTagSlot : MonoBehaviour 
{
	private Vector3 _v;

	public UISprite spLockImage, spPlayerIcon, spArrow;
	public UIButton btn;

	public UISprite spHp, spMp, spSp;

	public bool isPlayerSide = true;

	public int playerSlotIndex = 0;

	public bool isClicked = false;

	public Xfloat coolTime = 20.0f;
	
	public  bool isLocked = false;

	public enum State
	{
		Ready, CoolTime, Dead
	}

	public State state = State.Ready;


	void Start () 
	{
		UIEventListener.Get(btn.gameObject).onPress = onPress;//.onClick = onClick; //onPress;//Click;
	}




	public void init(bool playerSide, int index, bool isEnable, bool isVeryFirstInit)
	{
		isPlayerSide = playerSide;
		playerSlotIndex = index;

		isClicked = false;

		gameObject.SetActive(isEnable);

		transform.localPosition = Vector3.zero;

		if(playerSide)
		{
			transform.parent.localPosition = new Vector3(-682,32,0);
		}
		else
		{
			transform.parent.localPosition = new Vector3(314,32,0);
		}

		goFeverContainer.SetActive(true);

		spFeverBackground.enabled = false;

		spDead.enabled = (state == State.Dead);

		if(isVeryFirstInit || state != State.Dead)
		{
			resetCoolTime();
			isLocked = false;
			//spPlayerIcon.enabled = true;
		}

		if(isVeryFirstInit)
		{
			spDead.enabled = false;
		}
	}


	public UISprite spDead;
	public GameObject goFeverContainer;

	public void setDead()
	{
		//spPlayerIcon.enabled = false;
		spHp.fillAmount = 0;
		spMp.fillAmount = 0;
		spSp.fillAmount = 0;
		isLocked = true;
		isClicked = false;
		btn.isEnabled = false;
		spFeverBackground.enabled = false;
		spDead.enabled = true;
		goFeverContainer.SetActive(false);

		state = State.Dead;
	}


	public void onPress(GameObject go, bool isPress)
	{
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
#endif
		if(GameManager.me.isAutoPlay || GameManager.me.stageManager.isIntro) return;
		if(GameManager.me.recordMode != GameManager.RecordMode.record) return;

		if(GameManager.me.isPlaying == false) return;

		if(state == State.CoolTime) return;

		if(isPress)
		{
			if(isPlayerSide)
			{
				if(GameManager.me.player.hp > 0 && GameManager.me.player.state != Monster.DEAD)
				{
					GameManager.replayManager.changePlayer = true;
				}
			}
			/*
			else
			{
				
				if(GameManager.me.pvpPlayer.hp > 0 && GameManager.me.player.state != Monster.DEAD)
				{
					GameManager.replayManager.changePVPPlayer = true;
				}
				
			}
			*/
			
			GameManager.me.setMouseTouchOff();
		}
	}


	public void onPressByAi()
	{
		if(state == State.CoolTime) return;
		
		if(isPlayerSide)
		{
			if(GameManager.me.player.hp > 0 && GameManager.me.player.state != Monster.DEAD)
			{
				GameManager.replayManager.changePlayer = true;
			}
		}
		else
		{
			
			if(GameManager.me.pvpPlayer.hp > 0 && GameManager.me.player.state != Monster.DEAD)
			{
				GameManager.replayManager.changePVPPlayer = true;
			}
			
		}
		
		GameManager.me.setMouseTouchOff();
	}






	public void updateAllGauge(Player p)
	{
		updateHP(p.hpPer, p.hp, p.maxHp);
		updateMP(p.mp, p.maxMp);
		updateSP(p.sp, p.maxSp);
	}


	public void updateHP(float hpPer, float hp, float maxHp)
	{
		spHp.fillAmount = hpPer;

		if(hp <= 0)
		{
			btn.isEnabled = false;
			isLocked = true;
		}
	}
	
	public void updateMP(float mp, float maxMp)
	{
		spMp.fillAmount = mp/maxMp;
	}
	
	public void updateSP(float sp, float maxSp)
	{
		spSp.fillAmount = sp/maxSp;
	}	



//=======================================================================================	



	public void resetCoolTime()
	{
		if(GameManager.info.setupData.tagCoolTime <= 0.1f)
		{
			state = State.Ready;
			coolTime.Set( 0.0f );
			spLockImage.fillAmount = 0.0f;
			btn.isEnabled = true;
			spArrow.enabled = false;
			spFeverBackground.enabled = true;
		}
		else
		{
			state = State.CoolTime;
			coolTime = GameManager.info.setupData.tagCoolTime;
			spLockImage.fillAmount = 0.0f;
			btn.isEnabled = false;
			spArrow.enabled = false;
			spFeverBackground.enabled = false;
		}
	}	


	float _tempF;

	new public void update(bool fromAiSlot = false)
	{
		if(isLocked) return;

		if (isPlayerSide && playerSlotIndex == 0){
			if (state == State.CoolTime){
				Debug.Log(" stage :"+state);
			}
			else if (state == State.Ready){
				Debug.Log(" stage :"+state);
			}
			else if (state == State.Dead){
				Debug.Log(" stage :"+state);
			}
		}

		if(state == State.CoolTime)
		{
			coolTime.Set(coolTime - GameManager.globalDeltaTime);

			if(coolTime < 0.0f)
			{
				coolTime = 0.0f;
				state = State.Ready;
				btn.isEnabled = true;
				spLockImage.fillAmount = 1.0f;
				spArrow.enabled = false;

				if(isPlayerSide)
				{
					StartCoroutine( activeSkillMaxEffect() );
				}
			}
			else
			{
				_tempF = 1f - coolTime/GameManager.info.setupData.tagCoolTime;
				
				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0)
				{
				}
				else
					#endif
				{
					//if(MathUtil.abs(spLockImage.fillAmount,_tempF) > 0.02f || (_tempF <= 0.0f))
					{
						if(_tempF > 1)
						{
							_tempF = 1;
							spArrow.enabled = false;
						}
						else if(_tempF < 0)
						{
							_tempF = 0;
							spArrow.enabled = false;
						}
						else
						{
							if(spArrow.enabled == false)
							{
								spArrow.enabled = true;
							}
						}
						
						spLockImage.fillAmount = _tempF;
						
						spArrow.cachedTransform.localPosition = Util.getPositionByAngleAndDistanceWithoutTable((-_tempF + 0.25f)*360f, 45.0f);
						_v.x = 0; _v.y = 0; _v.z = -360.0f * _tempF + 90.0f;
						_q = spArrow.cachedTransform.localRotation;
						_q.eulerAngles = _v;
						spArrow.cachedTransform.localRotation = _q;
						
					}
				}
			}
		}

		Player p;

		if(isPlayerSide)
		{
			p = GameManager.me.battleManager.players[playerSlotIndex];
		}
		else
		{
			p = GameManager.me.battleManager.pvpPlayers[playerSlotIndex];
		}

		if(p == null) return;


		if(isClicked)
		{
			isClicked = false;

			if(state == State.Ready && GameManager.me.stageManager.playTime.Get() > 1.0f)
			{
				onClickProcess();
			}
		}
	}

	private Quaternion _q;

	public void testCode()
	{
		float tempAngle = -spLockImage.fillAmount + 0.25f;
		spArrow.cachedTransform.localPosition = Util.getPositionByAngleAndDistanceWithoutTable(tempAngle*360f, 45.0f);
		_v.x = 0; _v.y = 0; _v.z = -360.0f * spLockImage.fillAmount + 90.0f;
		_q = spArrow.cachedTransform.localRotation;
		_q.eulerAngles = _v;
		spArrow.cachedTransform.localRotation = _q;
	}


	void onClickProcess()
	{
		if(GameManager.me.currentScene == Scene.STATE.PLAY_BATTLE)// && GameManager.me.player.globalUnitCooltime <= 0)	
		{
			if(state == State.Ready)
			{
				GameManager.me.battleManager.startChangePlayer(isPlayerSide);
				btn.isEnabled = false;
				resetCoolTime();
			}
		}
	}

























	public UISprite spFeverBackground;


	static readonly string[] activeFeverIds = new string[]{"img_herochange_effect1","img_herochange_effect2","img_herochange_effect3","img_herochange_effect4","img_herochange_effect5","img_herochange_effect6","img_herochange_effect7","img_herochange_effect8"};
	
	IEnumerator activeSkillMaxEffect()
	{
		int index = 0;
		spFeverBackground.spriteName = activeFeverIds[index];
		spFeverBackground.enabled = true;

		while(true)
		{
			if(state != State.Ready)
			{
				spFeverBackground.enabled = false;
				break;
			}
			else if(coolTime.Get() > 0)
			{
				spFeverBackground.enabled = false;
				break;
			}

			yield return Util.ws008;
			++index;

			if(index > 12)
			{
				index = 0;
				spFeverBackground.spriteName = activeFeverIds[index];	
				spFeverBackground.enabled = true;
			}
			else if(index > 7)
			{
				if(index == 8)
				{
					spFeverBackground.enabled = false;
				}
			}
			else
			{
				spFeverBackground.spriteName = activeFeverIds[index];	
			}
		}
	}



	public void setVisible(bool isVisible)
	{
		gameObject.SetActive(isVisible);
	}


}
