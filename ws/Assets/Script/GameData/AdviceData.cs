using System;
using System.Collections.Generic;

sealed public class AdviceData : BaseData
{
	public static List<GameIDData> tempCalcList = new List<GameIDData>();

	public AdviceCheckData[] checkData;

	public string text;

	public int score;

	public int button;

	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		Util.parseObject(l[k["SCORE"]], out score, true, 0);
		Util.parseObject(l[k["BUTTON"]], out button, true, 0);

		int len = 1;

		if(string.IsNullOrEmpty(l[k["VARIABLE2"]].ToString()) == false)
		{
			len = 2;
			if(string.IsNullOrEmpty(l[k["VARIABLE3"]].ToString()) == false)
			{
				len = 3;
				if(string.IsNullOrEmpty(l[k["VARIABLE4"]].ToString()) == false)
				{
					len = 4;
					if(string.IsNullOrEmpty(l[k["VARIABLE5"]].ToString()) == false)
					{
						len = 5;
					}
				}
			}
		}

		checkData = new AdviceCheckData[len];

		switch(len)
		{
		case 1:
			checkData[0] = AdviceCheckData.getData((string)l[k["VARIABLE1"]],(string)l[k["OPERATOR1"]],l[k["CONSTANT1"]]);
			break;
		case 2:
			checkData[0] = AdviceCheckData.getData((string)l[k["VARIABLE1"]],(string)l[k["OPERATOR1"]],l[k["CONSTANT1"]]);
			checkData[1] = AdviceCheckData.getData((string)l[k["VARIABLE2"]],(string)l[k["OPERATOR2"]],l[k["CONSTANT2"]]);
			break;
		case 3:
			checkData[0] = AdviceCheckData.getData((string)l[k["VARIABLE1"]],(string)l[k["OPERATOR1"]],l[k["CONSTANT1"]]);
			checkData[1] = AdviceCheckData.getData((string)l[k["VARIABLE2"]],(string)l[k["OPERATOR2"]],l[k["CONSTANT2"]]);
			checkData[2] = AdviceCheckData.getData((string)l[k["VARIABLE3"]],(string)l[k["OPERATOR3"]],l[k["CONSTANT3"]]);
			break;
		case 4:
			checkData[0] = AdviceCheckData.getData((string)l[k["VARIABLE1"]],(string)l[k["OPERATOR1"]],l[k["CONSTANT1"]]);
			checkData[1] = AdviceCheckData.getData((string)l[k["VARIABLE2"]],(string)l[k["OPERATOR2"]],l[k["CONSTANT2"]]);
			checkData[2] = AdviceCheckData.getData((string)l[k["VARIABLE3"]],(string)l[k["OPERATOR3"]],l[k["CONSTANT3"]]);
			checkData[3] = AdviceCheckData.getData((string)l[k["VARIABLE4"]],(string)l[k["OPERATOR4"]],l[k["CONSTANT4"]]);
			break;
		case 5:
			checkData[0] = AdviceCheckData.getData((string)l[k["VARIABLE1"]],(string)l[k["OPERATOR1"]],l[k["CONSTANT1"]]);
			checkData[1] = AdviceCheckData.getData((string)l[k["VARIABLE2"]],(string)l[k["OPERATOR2"]],l[k["CONSTANT2"]]);
			checkData[2] = AdviceCheckData.getData((string)l[k["VARIABLE3"]],(string)l[k["OPERATOR3"]],l[k["CONSTANT3"]]);
			checkData[3] = AdviceCheckData.getData((string)l[k["VARIABLE4"]],(string)l[k["OPERATOR4"]],l[k["CONSTANT4"]]);
			checkData[4] = AdviceCheckData.getData((string)l[k["VARIABLE5"]],(string)l[k["OPERATOR5"]],l[k["CONSTANT5"]]);
			break;
		}

		text = ((string)l[k["TEXT"]]).Replace("\\n","\n");
	}


	public bool check()
	{
		int len = checkData.Length;

		for(int i = 0; i < len; ++i)
		{
			if(checkData[i].check() == false)
			{
				return false;
			}
		}

		return true;
	}


	public static int sortByScoreFromLow(AdviceData x, AdviceData y)
	{
		return y.score.CompareTo(x.score);
	}


	static List<AdviceData> _advices = new List<AdviceData>();

	private static Callback.Default _adviceCallback = null;
	private static int _adviceButtonIndex = 0;

	public static void checkAdvice(Callback.Default callback)
	{
		string tip = null;

		_adviceButtonIndex = 0;

		AdviceData.getAdviceText(out tip, out _adviceButtonIndex);

		if(tip != null)
		{
			_adviceCallback = callback;

			// > 1번 : 히어로 페이지 / 2번 : 소환룬 페이지 / 3번 : 스킬룬 페이지 / 4번 : 미션 페이지 / 5번 : 친구 페이지 / 6번 : 상점									
			
			switch(_adviceButtonIndex)
			{
			case 1:

				if(TutorialManager.instance.clearCheck("T46") == false)
				{
					_adviceButtonIndex = 0;
				}

				break;
			case 2:

				if(TutorialManager.instance.clearCheck("T44") == false)
				{
					_adviceButtonIndex = 0;
				}

				break;
			case 3:

				if(TutorialManager.instance.clearCheck("T45") == false)
				{
					_adviceButtonIndex = 0;
				}

				break;
			case 4:

				if(TutorialManager.instance.clearCheck("T13") == false)
				{
					_adviceButtonIndex = 0;
				}

				break;
			case 5:

				if(TutorialManager.instance.clearCheck("T15") == false)
				{
					_adviceButtonIndex = 0;
				}

				break;
			}


			if(_adviceButtonIndex == 0)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Advice, tip, go, go, _adviceButtonIndex);
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.Advice, tip, go, directGo, _adviceButtonIndex);
			}
		}
		else
		{
			callback();
		}
	}

	// 그냥 가기.
	private static void go()
	{
		GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.None;

		_adviceCallback();
		_adviceCallback = null;
	}

	// 바로 가기.
	private static void directGo()
	{
		// > 1번 : 히어로 페이지 / 2번 : 소환룬 페이지 / 3번 : 스킬룬 페이지 / 4번 : 미션 페이지 / 5번 : 친구 페이지 / 6번 : 상점									

		switch(_adviceButtonIndex)
		{
		case 1:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Hero;
			break;
		case 2:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Summon;
			break;
		case 3:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Skill;
			break;
		case 4:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Mission;
			break;
		case 5:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Friend;
			break;
		case 6:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Shop;
			break;
		default:
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.None;
			break;
		}

		_adviceCallback();
		_adviceCallback = null;
	}



	public static void getAdviceText(out string tip, out int buttonIndex)
	{
		int len = GameManager.info.adviceData.Count;
		int nowNum = 0;

		for(int i = 0; i < len; ++i)
		{
			if(GameManager.info.adviceData[i].score <= 0) continue;

			if(nowNum > 0)
			{
				if(GameManager.info.adviceData[i].score < _advices[nowNum-1].score) break;
			}

			if(GameManager.info.adviceData[i].check())
			{
				_advices.Add(GameManager.info.adviceData[i]);
				++nowNum;
			}
		}

		if(_advices.Count > 0)
		{
			int selectIndex = UnityEngine.Random.Range(0,_advices.Count);
			tip = Util.stringSubstitute(_advices[selectIndex].text, GameDataManager.instance.name);
			buttonIndex = _advices[selectIndex].button;
			_advices.Clear();
		}
		else
		{
			tip = null;
			buttonIndex = 0;
		}
	}
}


public struct AdviceCheckData
{
//	GM	게임모드 (탐험:0/도전:1/챔피:2/친선:3)	
//	ACT	액트	
//	STAGE	스테이지	
//	ROUND	라운드	
//	ASR	액트*100+스테이지*10+라운드	
//	PLV	플레이어레벨	
//	HLV	히어로레벨	
//	PHLV	플레이어레벨 - 히어로레벨	
//	ITMPLVA	장착 히어로아이템 4종의 강화레벨 평균	
//	ITMQLVA	장착 히어로아이템 4종의 요구레벨 평균	
//	ITMRLVA	장착 히어로아이템 4종의 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	
//	EQUNCNT	소환룬 장착 개수 (0~5)	
//	EQSKCNT	스킬룬 장착 개수 (0~3)	
//	UNRLVA	소환룬 5종 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	
//	SKRLVA	스킬룬 3종 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	
//	UNBLVA	소환룬 5종 베이스레벨 평균	
//	SKBLVA	스킬룬 3종 베이스레벨 평균	
//	HERO	사용 히어로(0:레오/1:카일리/2:클로이/3:루크)	
//	RNITMLVA	소환/스킬룬 8종 레벨평균 - 장착 아이템 4종 요구레벨 평균	
//	CHLGM	도전모드 게임모드 (무한질주:0, 무한생존:1, 무한사냥:2)	
//	CHMGRD	챔피언십등급 (1:브론즈, 2:실버, 3:골드, 4:마스터, 5:플래티넘, 6:레전드)	


//	SPRATEA	소환룬 평균 SP소모비율		
//	MPRATEA	스킬룬 평균 MP소모비율		
	



	public enum Type
	{
		GM,
		ACT,
		STAGE,
		ROUND,
		ASR,
		HLV,
		ITMPLVA,
		ITMRLVA,
		EQUNCNT,
		EQSKCNT,
		UNRLVA,
		SKRLVA,
		UNBLVA,
		SKBLVA,
		HERO,
		CHLGM,
		CHMGRD,
		MISSION,
		UNPLVA,
		SKPLVA,
		ITMGRDA,
		UNGRDA,
		SKGRDA,

		SPRATEA,//	소환룬 평균 SP소모비율		
		MPRATEA,//	스킬룬 평균 MP소모비율		


		UNCP,	//소환룬을 합성할 수 있는지 (S : 50점 / A : 40점 … D : 10점)						
		SKCP,	//스킬룬을 합성할 수 있는지 (S : 50점 / A : 40점 … D : 10점)						
//		- 인벤 및 장착중인 룬들도 모두 체크하여 동일등급 20레벨이 있는지..						
//		- 상위 S등급부터 체크하여 상위 등급 점수로 처리						
//		예> C등급만 합성가능 : 20점 // B등급, D등급 합성가능 : 30점 // 합성가능 룬 없음 : 0점						
//		- 등급별 합성가능 여부만 체크 (예> C20레벨이 2개 있던 20개 있던 30점)						
		
		UNRF,//	장착중인 소환룬을 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)						
		SKRF,//	장착중인 스킬룬을 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)						
		EQRF//	장착중인 장비를 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)						
//		- 장착중인 15레벨 이하의 룬/장비가 존재할 때, 인벤에 동일한 등급의 1레벨 룬/장비가 있는지..						
//		- 상위 S등급부터 체크하여 상위 등급 점수로 처리						
//		예> 인벤에 B등급 10레벨, C등급 10레벨… 이 있고, 인벤에 B등급 1레벨, C등급 1레벨이 있다면, C등급 무시하고 30점)						
//		- 등급별 강화 여부만 체크 (재료 개수는 무관)						



	}

	public enum Operator
	{
		Equal, Less, More
	}

	public Operator op;
	public Type type;
	public int value;

	public static AdviceCheckData getData(string variable, string oprt, object constant)
	{
		AdviceCheckData data = new AdviceCheckData();

		switch(variable)
		{
		case 	"GM"	 :	data.type =  Type.GM ;break;
		case 	"ACT"	 :	data.type =  Type.ACT ;break;
		case 	"STAGE"	 :	data.type =  Type.STAGE ;break;
		case 	"ROUND"	 :	data.type =  Type.ROUND ;break;
		case 	"ASR"	 :	data.type =  Type.ASR ;break;
//		case 	"PLV"	 :	data.type =  Type.PLV ;break;
		case 	"HLV"	 :	data.type =  Type.HLV ;break;
//		case 	"PHLV"	 :	data.type =  Type.PHLV ;break;
		case 	"ITMPLVA" :	data.type =  Type.ITMPLVA ;break;
//		case 	"ITMQLVA" :	data.type =  Type.ITMQLVA ;break;
		case 	"ITMRLVA" :	data.type =  Type.ITMRLVA ;break;	
		case 	"EQUNCNT" :	data.type =  Type.EQUNCNT ;break;
		case 	"EQSKCNT" :	data.type =  Type.EQSKCNT ;break;
		case 	"UNRLVA"  :	data.type =  Type.UNRLVA ;break;
		case 	"SKRLVA"	 :	data.type =  Type.SKRLVA ;break;
		case 	"UNBLVA"	 :	data.type =  Type.UNBLVA ;break;
		case 	"SKBLVA"	 :	data.type =  Type.SKBLVA ;break;
		case 	"HERO"	 :	data.type =  Type.HERO ;break;
//		case 	"RNITMLVA":	data.type =  Type.RNITMLVA ;break;	
		case 	"CHLGM"	 :	data.type =  Type.CHLGM ;break;
		case 	"CHMGRD"	 :	data.type =  Type.CHMGRD ;break;
		case    "MISSION"    :  data.type =  Type.MISSION; break;


		case "UNPLVA": data.type = Type.UNPLVA; break;
		case "SKPLVA": data.type = Type.SKPLVA; break;
		case "ITMGRDA": data.type = Type.ITMGRDA; break;
		case "UNGRDA": data.type = Type.UNGRDA; break;
		case "SKGRDA": data.type = Type.SKGRDA; break;


		case "SPRATEA": data.type = Type.SPRATEA; break;
		case "MPRATEA": data.type = Type.MPRATEA; break;

		case "UNCP": data.type = Type.UNCP; break;
		case "SKCP": data.type = Type.SKCP; break;
		case "UNRF": data.type = Type.UNRF; break;
		case "SKRF": data.type = Type.SKRF; break;
		case "EQRF": data.type = Type.EQRF; break;



		}

		switch(oprt)
		{
		case ">=": 
			data.op = Operator.More; break;
		case "=": 
			data.op = Operator.Equal; break;
		case "<=": 
			data.op = Operator.Less; break;
		}

		Util.parseObject(constant, out data.value, true, 0);

		return data;
	}




	public bool check()
	{
		int intValue = 0;
		int num = 0;

		switch(type)
		{
		case Type.GM://	게임모드 (탐험:0/도전:1/챔피:2/친선:3)	

			switch(GameManager.me.stageManager.nowPlayingGameType)
			{
			case GameType.Mode.Epic:
				return calc (0);
				break;
//			case GameType.Mode.Challenge:
//				return calc (1);
//				break;
			case GameType.Mode.Championship:
				return calc (2);
				break;
			case GameType.Mode.Friendly:
				return calc (3);
				break;
			}
			break;
		case Type.ACT://	액트	
			return calc (GameDataManager.instance.maxAct);
			break;
		case Type.STAGE://	스테이지	
			return calc (GameDataManager.instance.maxStage);
			break;
		case Type.ROUND://	라운드	
			return calc (GameDataManager.instance.maxRound);
			break;
		case Type.ASR://	액트*100+스테이지*10+라운드	
			return calc (GameManager.me.stageManager.playAct*100 + GameManager.me.stageManager.playStage * 10 + GameManager.me.stageManager.playRound);
			break;
//		case Type.PLV://	플레이어레벨	
//			return calc (GameDataManager.instance.level);
//			break;
		case Type.HLV://	히어로레벨	
			return calc (GameDataManager.instance.level);
			break;
//		case Type.PHLV://	플레이어레벨 - 히어로레벨	
//			return calc (GameDataManager.instance.level - GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].level);
//			break;
		case Type.ITMPLVA://	장착 히어로아이템 4종의 강화레벨 평균	

			return calc (
				UnityEngine.Mathf.RoundToInt((float)(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsBody.itemInfo.reinforceLevel +
			         GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsHead.itemInfo.reinforceLevel +
			         GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsWeapon.itemInfo.reinforceLevel +
			         GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsVehicle.itemInfo.reinforceLevel) / 4.0f * 10.0f ) );
			break;

//		case Type.ITMQLVA://	장착 히어로아이템 4종의 요구레벨 평균	
//			return calc (
//				(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsBody.parts.reqLevel +
//			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsHead.parts.reqLevel +
//			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsWeapon.parts.reqLevel +
//			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsVehicle.parts.reqLevel)/4 );
//			break;
		case Type.ITMRLVA://	장착 히어로아이템 4종의 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	

			return calc (
				(rareScore(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsBody.rare) +
			 rareScore(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsHead.rare) +
			 rareScore(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsWeapon.rare) +
			 rareScore(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsVehicle.rare)) /4 );
			break;


		case Type.EQUNCNT://	소환룬 장착 개수 (0~5)

			int uLen = 0;
			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					++uLen;
				}
			}
			return calc (uLen);


		case Type.EQSKCNT://	스킬룬 장착 개수 (0~3)	

			int sLen = 0;
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					++sLen;
				}
			}
			return calc (sLen);


		case Type.UNRLVA://	소환룬 5종 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	

			intValue = 0;
			num = 0;

			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					intValue += rareScore(GameDataManager.instance.unitSlots[i].unitData.rare);
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);

			break;
		case Type.SKRLVA://	스킬룬 3종 레어도 평균 (노말:10, 레어:20, 슈레:30, 레전:40)	

			intValue = 0;
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					intValue += rareScore( GameDataManager.instance.skills[i].infoData.rare );
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue/num);

			break;
		case Type.UNBLVA://	소환룬 5종 베이스레벨 평균	

			intValue = 0;
			
			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					intValue += (GameDataManager.instance.unitSlots[i].unitData.baseLevel);
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue/num);

			break;
		case Type.SKBLVA://	스킬룬 3종 베이스레벨 평균	

			intValue = 0;
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					intValue += ( GameDataManager.instance.skills[i].infoData.skillData.baseLevel);
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);

			break;
		case Type.HERO://	사용 히어로(0:레오/1:카일리/2:클로이/3:루크)	

			switch(GameDataManager.instance.selectHeroId)
			{
			case Character.LEO:
				return calc (0);
			case Character.KILEY:
				return calc (1);
			case Character.CHLOE:
				return calc (2);
			case Character.LUKE:
				return calc (3);
			}

			break;

			/*
		case Type.RNITMLVA://	소환/스킬룬 8종 레벨평균 - 장착 아이템 4종 요구레벨 평균	

			intValue = 0;

			int c = 0;

			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					intValue += (GameDataManager.instance.unitSlots[i].unitData.level);
					++c;
				}
			}

			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					intValue += ( GameManager.info.heroSkillData[GameDataManager.instance.skills[i].id].level);
					++c;
				}
			}

			return calc (
				(intValue / c) - 
			(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsBody.parts.reqLevel +
			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsHead.parts.reqLevel +
			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsWeapon.parts.reqLevel +
			 GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsVehicle.parts.reqLevel)/4 );
			


			break;
*/

		case Type.CHLGM://	도전모드 게임모드 (무한질주:0, 무한생존:1, 무한사냥:2)	

			switch(GameManager.me.stageManager.nowRound.mode)
			{
			case RoundData.MODE.C_RUN: return calc (0);
			case RoundData.MODE.C_SURVIVAL: return calc (1);
			case RoundData.MODE.C_HUNT: return calc (2);
			}

			break;
		case Type.CHMGRD://	챔피언십등급 (1:브론즈, 2:실버, 3:골드, 4:마스터, 5:플래티넘, 6:레전드)	

			switch(GameDataManager.instance.champLeague)
			{
			case WSDefine.LEAGUE_BRONZE:
				return calc (1);
				break;
			case WSDefine.LEAGUE_SILVER:
				return calc (2);
				break;
			case WSDefine.LEAGUE_GOLD:
				return calc (3);
				break;
			case WSDefine.LEAGUE_MASTER:
				return calc (4);
				break;
			case WSDefine.LEAGUE_PLATINUM:
				return calc (5);
				break;
			case WSDefine.LEAGUE_LEGEND:
				return calc (6);
				break;
			}
			break;


		case Type.MISSION: //아직 보상을 받지 않은, 클리어된 미션 개수

			int rewardReadyMissionNum = 0;

			foreach(KeyValuePair<string, P_Mission> kv in GameDataManager.instance.missions)
			{
				if(kv.Value.state == WSDefine.MISSION_CLOSE) continue;

				if(kv.Value.state == WSDefine.MISSION_CLEAR)
				{
					++rewardReadyMissionNum;
				}
			}

			return calc (rewardReadyMissionNum);
			break;








		case Type.UNPLVA: //장착된 소환룬의 강화레벨 평균 (1~20 강화레벨의 평균값)

			intValue = 0;
			num = 0;
			
			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					intValue += GameDataManager.instance.unitSlots[i].unitInfo.reinforceLevel;
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);

			break;
		case Type.SKPLVA: //장착된 스킬룬의 강화레벨 평균 (1~20 강화레벨의 평균값)

			intValue = 0;
			num = 0;
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					intValue += GameDataManager.instance.skills[i].infoData.reinforceLevel;
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);

			break;
		case Type.SPRATEA: //장착된 소환룬 평균 SP소모비율

			intValue = 0;
			num = 0;
			
			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					float useSp = GameDataManager.instance.unitSlots[i].unitData.sp;
					useSp -= (useSp * GameManager.me.player.summonSpPercent(GameDataManager.instance.unitSlots[i].unitData));
					intValue += UnityEngine.Mathf.RoundToInt(useSp / GameManager.me.player.maxSp * 100.0f);
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);

			break;
		case Type.MPRATEA: //스킬룬 평균 MP소모비율

			intValue = 0;
			num = 0;
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					float useMp = GameDataManager.instance.skills[i].infoData.skillData.mp;
					useMp -= (useMp * GameManager.me.player.skillSpDiscount(GameDataManager.instance.skills[i].infoData.skillData));
					intValue += UnityEngine.Mathf.CeilToInt(useMp / GameManager.me.player.maxMp * 100.0f);
					++num;
				}
			}

			if(intValue == 0 || num == 0) return calc (0);
			return calc (intValue / num);
			break;

//			- 인벤 및 장착중인 룬들도 모두 체크하여 동일등급 20레벨이 있는지..	
//			- 상위 S등급부터 체크하여 상위 등급 점수로 처리	
//			예> C등급만 합성가능 : 20점 // B등급, D등급 합성가능 : 30점 // 합성가능 룬 없음 : 0점	
//			- 등급별 합성가능 여부만 체크 (예> C20레벨이 2개 있던 20개 있던 30점)	
		case Type.UNCP: //소환룬을 합성할 수 있는지 (S : 50점 / A : 40점 … D : 10점)

			num = GameDataManager.instance.unitInventoryList.Count;
			intValue = 0;

			int x = 0 ;int s = 0; int a = 0; int b = 0; int c = 0; int d = 0;

			for(int i = 0; i < num; ++i)
			{
				if(GameDataManager.instance.unitInventoryList[i].reinforceLevel >= 20)
				{
					switch(GameDataManager.instance.unitInventoryList[i].rare)
					{
//					case RareType.SS: ++x; break;
					case RareType.S: ++s; break;
					case RareType.A: ++a; break;
					case RareType.B: ++b; break;
					case RareType.C: ++c; break;
					case RareType.D: ++d; break;
					}
				}
			}

			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					if(GameDataManager.instance.unitSlots[i].unitInfo.reinforceLevel >= 20)
					{
						switch(GameDataManager.instance.unitSlots[i].unitInfo.rare)
						{
//						case RareType.SS: ++x; break;
						case RareType.S: ++s; break;
						case RareType.A: ++a; break;
						case RareType.B: ++b; break;
						case RareType.C: ++c; break;
						case RareType.D: ++d; break;
						}
					}
				}
			}

			if(x >= 2) intValue = 60;
			else if(s >= 2) intValue = 50;
			else if(a >= 2) intValue = 40;
			else if(b >= 2) intValue = 30;
			else if(c >= 2) intValue = 20;
			else if(d >= 2) intValue = 10;

			return calc(intValue);

			break;

		case Type.SKCP: //스킬룬을 합성할 수 있는지 (S : 50점 / A : 40점 … D : 10점)

			num = GameDataManager.instance.skillInventoryList.Count;
			intValue = 0;
			
			int sx = 0; int ss = 0; int sa = 0; int sb = 0; int sc = 0; int sd = 0;
			
			for(int i = 0; i < num; ++i)
			{
				if(GameDataManager.instance.skillInventoryList[i].reinforceLevel >= 20)
				{
					switch(GameDataManager.instance.skillInventoryList[i].rare)
					{
//					case RareType.SS: ++sx; break;
					case RareType.S: ++ss; break;
					case RareType.A: ++sa; break;
					case RareType.B: ++sb; break;
					case RareType.C: ++sc; break;
					case RareType.D: ++sd; break;
					}
				}
			}
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					if(GameDataManager.instance.skills[i].infoData.reinforceLevel >= 20)
					{
						switch(GameDataManager.instance.skills[i].infoData.rare)
						{
//						case RareType.SS: ++sx; break;
						case RareType.S: ++ss; break;
						case RareType.A: ++sa; break;
						case RareType.B: ++sb; break;
						case RareType.C: ++sc; break;
						case RareType.D: ++sd; break;
						}
					}
				}
			}

			if(sx >= 2) intValue = 60;
			else if(ss >= 2) intValue = 50;
			else if(sa >= 2) intValue = 40;
			else if(sb >= 2) intValue = 30;
			else if(sc >= 2) intValue = 20;
			else if(sd >= 2) intValue = 10;
			
			return calc(intValue);

			break;


//			- 장착중인 15레벨 이하의 룬/장비가 존재할 때, 인벤에 동일한 등급의 1레벨 룬/장비가 있는지..								
//			- 상위 S등급부터 체크하여 상위 등급 점수로 처리								
//			예> 인벤에 B등급 10레벨, C등급 10레벨… 이 있고, 인벤에 B등급 1레벨, C등급 1레벨이 있다면, C등급 무시하고 30점)								
//			- 등급별 강화 여부만 체크 (재료 개수는 무관)								


		case Type.UNRF: //장착중인 소환룬을 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)


			intValue = 0;
			num = 0;

			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.unitSlots[i].isOpen)
				{
					if(GameDataManager.instance.unitSlots[i].unitInfo.level <= 15)
					{
						foreach(GameIDData gd in GameDataManager.instance.unitInventoryList)
						{
							if(gd.rare == GameDataManager.instance.unitSlots[i].unitInfo.rare && gd.level == 1)
							{
								num = rareScore(gd.rare);
								if(num > intValue) intValue = num;
								break;
							}
						}
					}
				}
			}
			
			return calc (intValue );


			break;

		case Type.SKRF: //장착중인 스킬룬을 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)


			intValue = 0;
			num = 0;
			
			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.skills[i].isOpen)
				{
					if(GameDataManager.instance.skills[i].infoData.level <= 15)
					{
						foreach(GameIDData gd in GameDataManager.instance.skillInventoryList)
						{
							if(gd.rare == GameDataManager.instance.skills[i].infoData.rare && gd.level == 1)
							{
								num = rareScore(gd.rare);
								if(num > intValue) intValue = num;
								break;
							}
						}
					}
				}
			}

			return calc (intValue );

			break;

		case Type.EQRF: //장착중인 장비를 강화할 수 있는지  (S : 50점 / A : 40점 … D : 10점)

			intValue = 0;
			num = 0;

			GameIDData pd = GameManager.me.player.playerData.partsHead.itemInfo;

			if(pd.level <= 15)
			{
				foreach(GameIDData gd in GameDataManager.instance.partsInventoryList)
				{
					if(gd.rare == pd.rare && gd.level == 1)
					{
						num = rareScore(gd.rare);
						if(num > intValue) intValue = num;
						break;
					}
				}
			}


			pd = GameManager.me.player.playerData.partsWeapon.itemInfo;

			if(pd.level <= 15)
			{
				foreach(GameIDData gd in GameDataManager.instance.partsInventoryList)
				{
					if(gd.rare == pd.rare && gd.level == 1)
					{
						num = rareScore(gd.rare);
						if(num > intValue) intValue = num;
						break;
					}
				}
			}


			pd = GameManager.me.player.playerData.partsBody.itemInfo;

			if(pd.level <= 15)
			{
				foreach(GameIDData gd in GameDataManager.instance.partsInventoryList)
				{
					if(gd.rare == pd.rare && gd.level == 1)
					{
						num = rareScore(gd.rare);
						if(num > intValue) intValue = num;
						break;
					}
				}
			}

			pd = GameManager.me.player.playerData.partsVehicle.itemInfo;

			if(pd.level <= 15)
			{
				foreach(GameIDData gd in GameDataManager.instance.partsInventoryList)
				{
					if(gd.rare == pd.rare && gd.level == 1)
					{
						num = rareScore(gd.rare);
						if(num > intValue) intValue = num;
						break;
					}
				}
			}

			return calc (intValue );

			break;


		}




		return false;
	}

	int rareScore(int rare)
	{
		switch(rare)
		{
		case RareType.C:
			return 20;
		case RareType.B:
			return 30;
		case RareType.A:
			return 40;
		case RareType.S:
			return 50;
		case RareType.SS:
			return 60;
		default:
			return 10;
		}
	}


	bool calc(int inputValue)
	{
		switch(op)
		{
		case Operator.Equal:
			return (value == inputValue);
			break;
		case Operator.Less:
			return (value >= inputValue);
			break;
		case Operator.More:
			return (value <= inputValue);
			break;
		}

		return false;
	}
}




