using System;
using System.Collections.Generic;

sealed public class SoundData
{
	public string id;

	public string link = "";

	public enum Type
	{
		Music, Effect, Bgm_Effect, Chracter, Player, UI
	}

	public bool isLoop = false;

	public string path = "";

	public Type type = Type.Effect;

	string dieId = null;
	string summonId = null;
	string atkId = null;
	string damageId = null;

	public int dieNum = 0;
	public int atkNum = 0;
	public int dmgNum = 0;
	public int grnNum = 0;

	public bool isAssetBundle = false;

	public string fileName = "";

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (l[k["ID"]]).ToString().Trim();

		link = (l[k["LINK"]]).ToString().Trim();

		isAssetBundle = (l[k["BUNDLESOUND"]]).ToString().Equals("Y");

		fileName = l[k["FILENAME"]].ToString().Trim();

//		UnityEngine.Debug.LogError( "id : " + id );

		switch((string)l[k["TYPE"]])
		{
		case "C":
			type = Type.Chracter;
			dieId = id+"_"+"die";
			summonId = id+"_"+"spn";
			atkId = id + "_" + "atk";
			damageId = id + "_" + "dmg";
			break;
		case "P":
			type = Type.Player;
			break;
		case "M":
			type = Type.Music;
			break;
		case "BE":
			type = Type.Bgm_Effect;
			break;
		case "UI":
			type = Type.UI;
			break;
		default:
			break;
		}

		if(isAssetBundle)
		{
#if UNITY_ANDROID
			path = ResourceManager.getLocalFilePath((string)l[k["PATH"]] + "A/" + fileName , AssetBundleManager.NORMAL_BUNDLE_EXTENSION_NAME);
#else
			path = ResourceManager.getLocalFilePath((string)l[k["PATH"]] + "I/" + fileName , AssetBundleManager.NORMAL_BUNDLE_EXTENSION_NAME);
#endif	
		}
		else
		{
			path = "sounds/" + (string)l[k["PATH"]] + "/" + fileName;
		}
	}




	private static SoundData _sd;
	public static void play(string id, bool ignoreSameSound = false)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info == null ||
		   GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		switch(_sd.type)
		{
		case SoundData.Type.Chracter:
			GameManager.soundManager.playEffect(_sd);
			break;
		case SoundData.Type.Effect:
			GameManager.soundManager.playEffect(_sd);
			break;
		case SoundData.Type.Bgm_Effect:
			GameManager.soundManager.playLoopEffect(_sd);
			break;
		case SoundData.Type.Music:
			GameManager.soundManager.playBG(_sd, ignoreSameSound);
			break;
		case SoundData.Type.UI:
			GameManager.soundManager.playUISound(_sd);
			break;
		}
	}

	public static void playVoice(string id)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info == null || 
		   GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		GameManager.soundManager.playPlayerVoice(_sd);
	}


	public static void playDieVoice(string id)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info == null || 
		   GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		
		GameManager.soundManager.playPlayerDieVoice(_sd);
	}



	public static void playLoopEffect2(string id)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info == null || 
		   GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		GameManager.soundManager.playLoopEffect2(_sd);
	}



	public static void playPlayerAttackSound(SoundData sd, Character.ID playerType = Character.ID.LEO, int playChance = 60)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		//기합 : 공격할 때, 약 60%확률로 플레이 여부 결정 후, 기합 보이스 5개중 한 개 랜덤선택 플레이
		if(UnityEngine.Random.Range(0,100) <= playChance)
		{
			string[] temp = null;

			switch(playerType)
			{
			case Character.ID.LEO:
				temp = Character.LEO_ATK;
				break;
			case Character.ID.KILEY:
				temp = Character.KILEY_ATK;
				break;
			case Character.ID.CHLOE:
				temp = Character.CHLOE_ATK;
				break;
			}

			if(temp != null)
			{
				if(sd.atkNum > 1)
				{
					int atkNum = UnityEngine.Random.Range(0,sd.atkNum);
					playVoice(temp[atkNum]);
				}
				else
				{
					playVoice( temp[0] );
				}
			}
		}
	}



	public static void playGroanSound(string id, Character.ID playerType = Character.ID.LEO)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		 if(_sd.type == Type.Player)
		{

			string[] temp = null;
			
			switch(playerType)
			{
			case Character.ID.LEO:
				temp = Character.LEO_GRN;
				break;
			case Character.ID.KILEY:
				temp = Character.KILEY_GRN;
				break;
			case Character.ID.CHLOE:
				temp = Character.CHLOE_GRN;
				break;
			}

			if(temp != null)
			{
				List<SoundData> grnSounds = new List<SoundData>();
				for(int i = 0; i < _sd.grnNum; ++i)
				{
					if(GameManager.info == null || 
					   GameManager.info.soundData.ContainsKey(temp[i]) == false) continue;

					grnSounds.Add(GameManager.info.soundData[temp[i]] );
				}

				if(grnSounds.Count > 0)
				{
					SoundManager.instance.playGroanVoice(grnSounds.ToArray());
				}
			}
		}
	}


	public static void playDieSound(string id, Character.ID playerType = Character.ID.LEO)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		if(string.IsNullOrEmpty(_sd.link) == false)
		{
			playDieSound(_sd.link);
			return;
		}

		if(_sd.type == Type.Chracter)
		{
			play(_sd.getCharacterDieSound());
		}
		else if(_sd.type == Type.Player)
		{

			string[] temp = null;
			
			switch(playerType)
			{
			case Character.ID.LEO:
				temp = Character.LEO_DIE;
				break;
			case Character.ID.KILEY:
				temp = Character.KILEY_DIE;
				break;
			case Character.ID.CHLOE:
				temp = Character.CHLOE_DIE;
				break;
			}
			
			if(temp != null)
			{
				if(_sd.dieNum > 1)
				{
					int dieNum = UnityEngine.Random.Range(0,_sd.dieNum);

					playDieVoice(temp[dieNum]);
				}
				else
				{
					playDieVoice( temp[0] );
				}
			}
		}
	}

	public static void playSummonSound(string id)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		if(string.IsNullOrEmpty(_sd.link) == false)
		{
			playSummonSound(_sd.link);
			return;
		}

		// 소환수들이 생성될때 그 캐릭터의 소환 사운드.
		if(_sd.type == Type.Chracter)
		{
			play(_sd.getUnitSummonSound());
		}
	}

	public static void playDamageSound(string id, Character.ID playerType = Character.ID.LEO)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		if(string.IsNullOrEmpty(_sd.link) == false)
		{
			playDamageSound(_sd.link, playerType);
			return;
		}

		if(_sd.type == Type.Chracter)
		{
			play(_sd.getDamageSound());
		}
		else if(_sd.type == Type.Player)
		{

			string[] temp = null;
			
			switch(playerType)
			{
			case Character.ID.LEO:
				temp = Character.LEO_DMG;
				break;
			case Character.ID.KILEY:
				temp = Character.KILEY_DMG;
				break;
			case Character.ID.CHLOE:
				temp = Character.CHLOE_DMG;
				break;
			}
			
			if(temp != null)
			{
				if(_sd.dmgNum > 1)
				{
					int dmgNum = UnityEngine.Random.Range(0,_sd.dmgNum);

					playVoice(temp[dmgNum]);
				}
				else
				{
					playVoice( temp[0] );
				}
			}


		}
	}



	public static void playAttackSound(string id, int type)
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif
		
		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		if(string.IsNullOrEmpty(_sd.link) == false)
		{
			playAttackSound(_sd.link, type);
			return;
		}

		play(_sd.getCharacterAtkSound(type));
	}




	public static void stop(string id)
	{
		if(GameManager.info.soundData.TryGetValue(id, out _sd) == false) return;

		switch(_sd.type)
		{
		case SoundData.Type.Music:
			GameManager.soundManager.stopBG();
			break;
		case SoundData.Type.Bgm_Effect:
			GameManager.soundManager.stopLoopEffect();
			break;
		}
	}



	public static void playHitSound(int typeNum, bool isUp)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1)
		{
			return;
		}
		#endif

		GameManager.info.skillEffectSetupData[typeNum].playSound(isUp);
	}

	public static string getHitSoundId(int typeNum, bool isUp)
	{
		if(isUp) return GameManager.info.skillEffectSetupData[typeNum].soundUp;
		else return GameManager.info.skillEffectSetupData[typeNum].soundDown;
	}


	public string getCharacterDieSound()
	{
		return dieId;
	}

	string _tempStr;
	public string getCharacterAtkSound(int type)
	{
		if(type == 0 || type == 1)
		{
			return atkId;
		}
		else
		{
			_tempStr = id + "_atk" + type;
			return _tempStr;
		}
	}


	public string getUnitSummonSound()
	{
		return summonId;
	}
	
	public string getDamageSound()
	{
		return damageId;
	}



	public static string nowPlayingBattleBgm = null;
	public static void playBattleBGM()
	{
		if(nowPlayingBattleBgm == null) return;
		SoundData.play(nowPlayingBattleBgm);
	}


	public static SoundData tempSoundData = null;

}