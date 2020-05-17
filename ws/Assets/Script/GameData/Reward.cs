using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

sealed public class Reward
{
	private Dictionary<string,string>[] _rewardData;
	private RewardData[] _returnRewards;
	private int[] _totalPercent;
	
	public bool hasReward = false;
	
	public Reward(string data)
	{
		if(data == null || data.Length == 0)
		{
			hasReward = false;
			return;
		}
		
		// 1. |를 검사한다. 세트다...
		string[] setSplit = data.Split('|');
		
		int i = setSplit.Length;
		
		_rewardData = new Dictionary<string, string>[i];
		_returnRewards = new RewardData[i];
		_totalPercent = new int[i];
		
		for(int j = 0; j < i; ++j)
		{
			_returnRewards[j] = new RewardData();
		}
		
		i = 0;
		
		foreach(string partStr in setSplit)
		{
			_totalPercent[i] = 0;
			_rewardData[i] = Util.logicParser(partStr, i, _totalPercent);
			++i;
		}
		
		hasReward = (i > 0);
	}
	

	
	//2%BS:1@10,10%BP:1@10,5%M:[1~10],5%I:ITEM_001*5,10%EXP:[3~10]
	private string _key;
	private string _percent;
	private int _len;
	
	private int _randomNum;
	private int _totalPer;
	
	private float _dropItemDelay = 0.0f;
	
	private int _resultValue;
	
	
	public int startBulletHitAction(Vector3 effectPosition, Bullet bullet, Monster cha)
	{
		return start(effectPosition,false,false,bullet,cha,true);
	}
	
	
	private int _tempIndex = 0;
	public int start(Vector3 effectPosition, bool isUseDropItemDelay = false, bool isUseDropItemMotion = true, Bullet bullet = null, Monster cha = null, bool isHitAction = false)
	{
		checkReward();
		
		_dropItemDelay = 0.0f;
		_tempIndex = 0;
	
		_resultValue = 100;
		
		
		foreach(RewardData rd in _returnRewards)
		{
			switch(rd.type)
			{
			
				/*
			case RewardData.HP:
				
				GameManager.me.effectManager.getWordEffect().start("+"+System.Convert.ToInt32(rd.value),effectPosition);
				GameManager.me.player.hp += System.Convert.ToInt32(rd.value);
				//if(GameManager.me.player.hp > GameManager.me.player.maxHp) GameManager.me.player.hp = (int)GameManager.me.player.maxHp;
				
				break;
				
			case RewardData.SP:
				GameManager.me.player.sp += System.Convert.ToInt32(rd.value);
				break;
				
			case RewardData.NONE :
				break;
			
			case RewardData.EXP :
				Log.log("EXP 획득! : " + rd.valueNum);
				break;
			case RewardData.SCORE:
				
				GameManager.me.uiManager.uiPlay.addScore(rd.valueNum);
				break;
			case RewardData.REWARD_LOGIC:
				Log.log("rd.value: " + rd.value);
				GameManager.info.rewardLogicData[rd.value].reward.start(effectPosition, isUseDropItemDelay, isUseDropItemMotion);
				break;
			case RewardData.ATTACK:
				break;
			case RewardData.DEFENCE:
				break;
			case RewardData.ITEM :
				
				Log.log("!! ==  아이템 리워드를 보여주어라!! == !!");
				
				for(int i = 0; i < rd.valueNum; ++i)
				{
					if(isUseDropItemMotion)
					{
						MapObject mobj = GameManager.me.mapManager.addMapObjectToStage(rd.value, effectPosition);
						mobj.dropItemEffect.start(effectPosition, _dropItemDelay);
						//_dropItemDelay += 0.2f;
						++_tempIndex;
					}
					else
					{
						GameManager.me.mapManager.addMapObjectToStage(rd.value, effectPosition);						
					}
				}
				break;
				

			case RewardData.CREATE_MONSTER:
				
				Log.log("### CREATE_MONSTER ####");
				effectPosition.z = UnityEngine.Random.Range(-50,70);
				Monster m = GameManager.me.mapManager.addMonsterToStage(false, null, rd.value, effectPosition);
				if(bullet!= null && bullet.isPlayerBullet == false) m.isFlipX = true;
				break;
				*/
			
			case RewardData.CREATE_EFFECT:
				GameManager.info.effectData[rd.value].getEffect(-1000,effectPosition);
				break;

			case RewardData.CREATE_MAPOBJECT:
				GameManager.me.mapManager.addMapObjectToStage(rd.value, effectPosition);
				break;
				
			/*	
			case RewardData.BULLET_PATTERN:
				GameManager.me.bulletManager.setBulletShooter(GameManager.info.bulletPatternData[rd.value], effectPosition);
				break;
				
			case RewardData.BULLET_PATTERN_MONSTER:
				GameManager.me.bulletManager.setBulletShooter(GameManager.info.bulletPatternData[rd.value], effectPosition, false);
				break;
			*/
					
			/*	
			case RewardData.BULLET_TYPE:
				GameManager.me.player.setBulletType(rd.valueNum);
				break;

			
			case RewardData.BULLET_DAMAGE:
				_resultValue = rd.valueNum;
				break;
			
			
			// 좀 더 정리할 녀석....
			
			case RewardData.CHARACTER_SLOW:
				cha.changeCharacterSpeed(rd.valueNum,rd.duration);
				break;
				
			case RewardData.STUN:
				cha.characterEffect.setStun(rd.valueNum);
				break;

			case RewardData.ICE:
				cha.characterEffect.setIce(rd.valueNum);
				break;				
				
			case RewardData.BULLET_SKIP:
				++bullet.skipTime;
				break;
				
			case RewardData.NO_ATTACK:
				cha.setSience();
				break;
				
			//[To Do]	
				
			//[To Do]	
			case RewardData.POISON_DAMAGE:
				
				cha.characterEffect.setPoison(rd.valueNum , (int)rd.duration);
				// ???
				break;
			*/	
			case RewardData.QUAKE_EFFECT:
				
				GameManager.me.effectManager.quakeEffect(rd.duration, rd.valueNum);
				
				break;
						/*
				public const string BULLET_SKIP = "SKN"; // 50%SKN <= 30% 확률로 총알이 사라지지않고 관통됨.
				public const string BULLET_DAMAGE = "DMG"; // 50%DMG:100 <= 공격데미지 200%. <= 현재 데미지 x 2
				public const string STUN = "STN"; // 2%STN:10 // 10초간...2%확률로... 
				public const string NO_ATTACK = "NAK"; // 66%NAK:100 // 100초간 공격못함.
				public const string POISON_DAMAGE = "PSN"; // 30%PSN:[50~100]@3 <= 시작 데미지 50~100. 3초간 데미지입음. 시작데미지가 100이면. 1초후 100. 2초후 50. 3초후 25 입음.
				public const string CHARACTER_SLOW = "SLO"; // 30%SLO:50@3 <= 30% 확률로 3초간 적의 속도를 50%로..
				public const string BULLET_SPLASH = "SPL"; // 30%SPL:[1~100]@3 <= 30%확률로 스플래쉬 공격. 1~100 픽셀. 총 3마리까지.	
						*/	
				
			}
		}
		
		return _resultValue;
	}
	
	
	
	private RewardData[] checkReward()
	{
		int i = 0;
		
		_len = _rewardData.Length;
		
		
		for(i = 0; i < _len; ++i)
		{
			_randomNum = UnityEngine.Random.Range(0, _totalPercent[i]);
			_totalPer = 0;
			
			foreach(KeyValuePair<string, string> kv in _rewardData[i])
			{
				_returnRewards[i].type = RewardData.NONE;
				
				string[] keySplit = kv.Key.Split('%');
				if(keySplit.Length == 2)
				{
					_key = keySplit[1].Split('_')[0];
					_percent = keySplit[0];
					_totalPer += Convert.ToInt16( _percent );
					
					if(_totalPer >= _randomNum )
					{
						_returnRewards[i].setData(_key, kv.Value);
						break;
					}
				}
				else
				{
					_returnRewards[i].setData(keySplit[0], kv.Value);
				}
			}
		}
		
		return _returnRewards;
	}
}




public class RewardData
{
	public const string HP = "HP";
	public const string ITEM = "I";
	
	public const string BULLET_POWER = "BP";
	public const string BULLET_SPEED = "BS";
	public const string SCROLL_SPEED = "SS";
	
	public const string SCROLL_GAUGE = "SG";
	
	public const string NONE = "N";
	
	public const string EXP = "EXP";
	public const string SCORE = "SC";
	public const string REWARD_LOGIC = "RL";
	public const string ATTACK = "ATK";
	public const string DEFENCE = "DEF";
	
	
	public const string BULLET_TYPE = "BT"; 
	
	public const string BULLET_SKIP = "SKN"; // 50%SKN <= 30% 확률로 총알이 사라지지않고 관통됨.
	public const string BULLET_DAMAGE = "DMG"; // 50%DMG:100 <= 공격데미지 200%. <= 현재 데미지 x 2
	public const string STUN = "STN"; // 2%STN:10 // 10초간...2%확률로... 
	public const string NO_ATTACK = "NAK"; // 66%NAK:100 // 100초간 공격못함.
	public const string BULLET_PATTERN = "BPN"; // 30%BPN:SHOOTCROSS,50%BPN:WIZARD_1_1_1 <= 30%확률로 SHOOTCROSS, 50%확률로 WIZARD_1_1_ 발사.
	public const string BULLET_PATTERN_MONSTER = "BPNM"; // 30%BPN:SHOOTCROSS,50%BPN:WIZARD_1_1_1 <= 30%확률로 SHOOTCROSS, 50%확률로 WIZARD_1_1_ 발사.	
	public const string POISON_DAMAGE = "PSN"; // 30%PSN:[50~100]@3 <= 시작 데미지 50~100. 3초간 데미지입음. 시작데미지가 100이면. 1초후 100. 2초후 50. 3초후 25 입음.
	public const string CHARACTER_SLOW = "SLO"; // 30%SLO:50@3 <= 30% 확률로 3초간 적의 속도를 50%로..
	public const string BULLET_SPLASH = "SPL"; // 30%SPL:[1~100]@3 <= 30%확률로 스플래쉬 공격. 1~100 픽셀. 총 3마리까지.
	public const string CREATE_MAPOBJECT = "MO"; //MO:OBJECT_SPIDER_ATTACK_001 <= 100% 확률로 오브젝트 생성.

	public const string CREATE_EFFECT = "EFF";

	public const string ICE = "ICE";
	
	public const string CREATE_MONSTER = "MON";
	
	public const string QUAKE_EFFECT = "QKE";
	
	public const string SECRET_STAGE = "SM";
	
	public const string SP = "SP";
	
	
	public string type = "";
	
	public string value = "";
	
	public int valueNum;
	
	public float duration = 0.0f;
	
	
	
	public void setData(string key, string valueStr)
	{
		type = key;
		
		valueNum = 1;
		 
		if(valueStr.IndexOf("[") > -1) 
		{
			string[] valueTime = valueStr.Split('@');
			
			if(valueTime.Length == 2)
			{
				valueStr = valueTime[0];
				duration = (float)Convert.ToDouble(valueTime[1]);
			}
			
			string[] rangeValue = valueStr.Substring(1,valueStr.Length - 2).Split('~');
			valueNum = UnityEngine.Random.Range( System.Convert.ToInt32(rangeValue[0]), System.Convert.ToInt32(rangeValue[1]) + 1);
		}
		else if(valueStr.IndexOf("@") > -1)
		{
			string[] valueTime = valueStr.Split('@');
			valueNum = Convert.ToInt32(valueTime[0]);
			duration = (float)Convert.ToDouble(valueTime[1]);
		}
		else if(valueStr.IndexOf("*") > -1) 
		{
			string[] values = valueStr.Split('*');
			value = values[0];
			valueNum = Convert.ToInt32(values[1]);
		}		
		else
		{
			value = valueStr;
			switch(key)
			{
				
			case ITEM:
				break;
			case  NONE:
				break;
			case  REWARD_LOGIC:
				break;		
			case BULLET_SKIP:
				break;		
			case CREATE_MAPOBJECT:
				break;				
			case CREATE_MONSTER:
				break;
				
			default:
				
				try
				{
					valueNum = Convert.ToInt32(valueStr);
				}catch(Exception e)
				{
				}
				
				break;
				
				/*
			case HP: 
				valueNum = Convert.ToInt32(valueStr);
				break;
				
			case  BULLET_POWER:
				valueNum = Convert.ToInt32(valueStr);
				break;
			case  BULLET_SPEED:
				valueNum = Convert.ToInt32(valueStr);
				break;
			case  SCROLL_SPEED:
				valueNum = Convert.ToInt32(valueStr);
				break;
				

				
			case  EXP:
				valueNum = Convert.ToInt32(valueStr);
				break;
			case  SCORE:
				valueNum = Convert.ToInt32(valueStr);
				break;

			case  ATTACK:
				valueNum = Convert.ToInt32(valueStr);
				break;
			case  DEFENCE:
				valueNum = Convert.ToInt32(valueStr);
				break;
				

				
			case BULLET_DAMAGE:
				valueNum = Convert.ToInt32(valueStr);
				break;

				
			case STUN:
				valueNum = Convert.ToInt32(valueStr);
				break;
				
			case NO_ATTACK:
				valueNum = Convert.ToInt32(valueStr);
				break;
				
			case BULLET_PATTERN:
				valueNum = Convert.ToInt32(valueStr);
				break;
				
			case POISON_DAMAGE:
				valueNum = Convert.ToInt32(valueStr);
				break;		
				
				
			case CHARACTER_SLOW:
				valueNum = Convert.ToInt32(valueStr);
				break;			
				
			case CHARACTER_SLOW:
				valueNum = Convert.ToInt32(valueStr);
				break;						
				*/

				
			}
		}
	}
	
}
