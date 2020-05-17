using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable()]
public class GameInfoManager : MonoBehaviour 
{

	public string version = "0";

	public string clientFullVersion
	{
		get
		{
#if UNITY_ANDROID
			if(Application.isPlaying)
			{
				if(PlatformManager.instance != null && PlatformManager.instance.type == PlatformManager.Platform.Kakao)
				{
					return "K"+EpiServer.instance.clientVer + "." + version;
				}
			}

			return "A"+EpiServer.instance.clientVer + "." + version;
#else
			return "I"+EpiServer.instance.clientVer + "." + version;
#endif
		}
	}

	private static GameInfoManager _instance;
	
	public static GameInfoManager instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new GameObject("GameInfoManager").AddComponent<GameInfoManager>();
				_instance.init();
			}
			return _instance;
		}
	}
	
	void OnDestroy()
	{
//		Debug.Log("GameInfoManager OnDestroy!");

		if(versionData != null) versionData.Clear();
		versionData = null;

		if(effectFileNames != null) effectFileNames.Clear();
		effectFileNames = null;

		if(modelFileNames != null) modelFileNames.Clear();
		modelFileNames = null;

		if(mapFileNames != null) mapFileNames.Clear();
		mapFileNames = null;

		if(soundFileNames != null) soundFileNames.Clear();
		soundFileNames = null;

		if(textureFileNames != null) textureFileNames.Clear();
		textureFileNames = null;

		if(uiFileNames != null) uiFileNames.Clear();
		uiFileNames = null;

		if(etcFileNames != null) etcFileNames.Clear();
		etcFileNames = null;


		textData = null;
		uiTextData = null;
		zoneData = null;
		mapObjectData = null;

		baseUnitData = null;
		rareUnitData = null;
		baseMonsterUnitData = null;
		rareMonsterUnitData = null;	
		rareUnitDataByBaseId = null;	
		unitData = new Dictionary<string, UnitData>();
		cutSceneData = null;
		mapData = null;
		npcData = null;
		effectData = null;
		unitSkillData = null;
		heroBaseSkillData = null;
		heroSkillData = null;
		rareHeroSkillDataByBaseId = null;
		bulletPatternData = null;
		monsterData = null;
		heroMonsterData = null;
		heroMonsterAI = null;
		playerHeroData = null;
		rewardLogicData = null;
		bulletData = null;
		actData = null;
		stageData = null;
		roundData = null;		

		unitIconIndexData = null;
		skillIconIndexData = null;
		equipIconIndexData = null;

		hellSetupData = null;

		_instance = null;

		if(dataStreamDic != null) dataStreamDic.Clear();
		dataStreamDic = null;
	}
	
	
	public SetupData setupData;


	public Dictionary<string, byte[]> dataStreamDic = new Dictionary<string, byte[]>();

	public Dictionary<string, string> effectFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> modelFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> mapFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> soundFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> textureFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> uiFileNames = new Dictionary<string, string>();

	public Dictionary<string, string> etcFileNames = new Dictionary<string, string>();

	public Dictionary<string, TextData> textData;

	public Dictionary<string, TextData> uiTextData;

	public Dictionary<string, ZoneData> zoneData;
	public List<ZoneData> testDrawZones = new List<ZoneData>();
	public Dictionary<string, MonsterZoneData> monsterZoneData;
	public Dictionary<string, MapObjectData> mapObjectData;

	public Dictionary<string, ModelData> modelData;
	
	public Dictionary<int, SkillEffectSetupData> skillEffectSetupData;

	public Dictionary<string, LoadingScreenData> loadingScreenData;

	public List<LoadingTipData> tipData;

	public Dictionary<string, IconIndexData> unitIconIndexData;
	public Dictionary<string, IconIndexData> equipIconIndexData;
	public Dictionary<string, IconIndexData> skillIconIndexData;

	public Dictionary<string, UnitData> baseUnitData;
	public Dictionary<string, UnitData> rareUnitData;
	public Dictionary<string, UnitData> baseMonsterUnitData;
	public Dictionary<string, UnitData> rareMonsterUnitData;	
	public Dictionary<string, List<UnitData>> rareUnitDataByBaseId = new Dictionary<string, List<UnitData>>(StringComparer.Ordinal);	

	public Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);
	//public Dictionary<string, UnitData> monsterUnitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);

	public Dictionary<string, BaseHeroPartsData> heroBasePartsData;

	public Dictionary<string, HeroPartsData> heroPartsDic;
	public Dictionary<string, CutSceneData> cutSceneData = new Dictionary<string, CutSceneData>(StringComparer.Ordinal);

	public Dictionary<string, CutSceneData> unitSkillCamData;

	public Dictionary<int, MapData> mapData;
	public Dictionary<string, NPCData> npcData;
	public Dictionary<string, EffectData> effectData;

	public Dictionary<string, PlayerAiData> playerAiData;
	public Dictionary<string, AiGroupData> aiGroupData;

	public Dictionary<string, Dictionary<string, FaceAnimationInfo>> faceAniData;

	public Dictionary<string, Dictionary<string, AniData>> aniData;

	public Dictionary<string, SoundData> soundData = new Dictionary<string, SoundData>();

	public Dictionary<string, LobbyPositionData> lobbyPosition;

	public List<AdviceData> adviceData;

	public Dictionary<string, SkillIconData> skillIconData;

	public Dictionary<string, UnitSkillData> unitSkillData;
	public Dictionary<string, HeroSkillData> heroBaseSkillData;
	public Dictionary<string, HeroSkillData> heroSkillData;
	public Dictionary<string, List<HeroSkillData>> rareHeroSkillDataByBaseId = new Dictionary<string, List<HeroSkillData>>(StringComparer.Ordinal);
	public Dictionary<string, BulletPatternData> bulletPatternData;
	public Dictionary<string, MonsterData> monsterData;
	public Dictionary<string, HeroMonsterData> heroMonsterData;
	public Dictionary<string, MonsterHeroAI> heroMonsterAI;
	public Dictionary<string, PlayerHeroData[]> playerHeroData;
	public Dictionary<string, RewardLogicData> rewardLogicData;
	public Dictionary<string, BulletData> bulletData;
	public Dictionary<int, ActData> actData;
	public Dictionary<string, StageData> stageData;
	public Dictionary<string, RoundData> roundData;

	public Dictionary<string, Dictionary<int, TutorialData>> tutorialData = new Dictionary<string, Dictionary<int, TutorialData>>();

	public Dictionary<string, TutorialInfoData> tutorialInfoData;

	public Dictionary<string, TestModeData> testModeData;

	public Dictionary<string, TranscendData> transcendData;

	public Dictionary<string, P_Sigong> testSigong;


	public Dictionary<string, VersionData> versionData;

	public Dictionary<int, HellSetupData> hellSetupData;


	private void init()
	{
	}

}
