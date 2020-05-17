using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Runtime.Serialization.Formatters.Binary;

public class LowDataSerialize : MonoBehaviour {

    static Character charLowData;
    static Item itemLowData;
    static Local stringLowData;
    static Resource resourceLowData;
    static Partner partnerLowData;
    static Mob monsterLowData;
    static NpcData npcLowData;
	static NonInteractiveNpcData nonInteractiveNpcLowData;
    static SkillTables skillLowData;
    static Newbie NewbieData;
    static DungeonTable dungeonData;
    static Etc etcData;
    static Level levelLowData;
    static Mail mailLowData;
    static Quest questLowData;
    static Enchant enchantLowData;
    static MissionTable missionLowData;
    static Map mapLowData;
    static PartnerLevel partnerLvLowData;
    static Shop shopLowData;
    static Price priceLowData;
    static Vip vipLowData;
    static Icon iconLowData;
    static Formula FormulaLowData;
    static Guild guildLowData;
    static GatchaReward gatchaRewardLowData;
    static Loading loadingLowData;
    static Title titleLowData;
    static Welfare welfareLowData;
    static Achievement achievementLowData;
    static ActiveReward activeLowData;
    static PVP pvpAutoRewardLowData;

    [MenuItem("MagnetGames/LowDataSerialize")]
    public static void DataSerialize()
    {
        DirectoryInfo dirinfo = new DirectoryInfo("Assets/Resources/SerializeData");
        foreach (var file in dirinfo.GetFiles())  // GetFiles("Assets/Resources/SerializeData", " *.bin", SearchOption.AllDirectories))
        {
            //일단 모든파일을 삭제하고 새로 생성
            file.Delete();
        }

        Debug.Log("StartDataSerialize");

        LoadLowDataALLForTool();
        SerializeLowData();
        ClearLowData();

        System.GC.Collect();

        Debug.Log("EndDataSerialize");

    }

    public static void ClearLowData()
    {
        charLowData = null;
        itemLowData = null;
        stringLowData = null;
        resourceLowData = null;
        partnerLowData = null;
        monsterLowData = null;
        npcLowData = null;
		nonInteractiveNpcLowData = null;
        NewbieData = null;
        skillLowData = null;
        dungeonData = null;
        etcData = null;
        levelLowData = null;
        mailLowData = null;
        questLowData = null;
        enchantLowData = null;
        missionLowData = null;
        mapLowData = null;
        partnerLvLowData = null;
        shopLowData = null;
        priceLowData = null;
        vipLowData = null;
        iconLowData = null;
        FormulaLowData = null;
        guildLowData = null;
        gatchaRewardLowData = null;
        loadingLowData = null;
        titleLowData = null;
        welfareLowData = null;
        achievementLowData = null;
        activeLowData = null;
        pvpAutoRewardLowData = null;
    }

    public static void SerializeLowData()
    {
        charLowData.SerializeData();
        itemLowData.SerializeData();
        stringLowData.SerializeData();
        resourceLowData.SerializeData();
        partnerLowData.SerializeData();
        monsterLowData.SerializeData();
        npcLowData.SerializeData();
		nonInteractiveNpcLowData.SerializeData ();
        NewbieData.SerializeData();
        skillLowData.SerializeData();
        dungeonData.SerializeData();
        etcData.SerializeData();
        levelLowData.SerializeData();
        mailLowData.SerializeData();
        questLowData.SerializeData();
        enchantLowData.SerializeData();
        missionLowData.SerializeData();
        mapLowData.SerializeData();
        partnerLvLowData.SerializeData();
        shopLowData.SerializeData();
        priceLowData.SerializeData();
        vipLowData.SerializeData();
        iconLowData.SerializeData();
        FormulaLowData.SerializeData();
        guildLowData.SerializeData();
        gatchaRewardLowData.SerializeData();
        loadingLowData.SerializeData();
        titleLowData.SerializeData();
        welfareLowData.SerializeData();
        achievementLowData.SerializeData();
        activeLowData.SerializeData();
        pvpAutoRewardLowData.SerializeData();
    }

    public static void LoadLowDataALLForTool()
    {
        charLowData = new Character();
        itemLowData = new Item();
        stringLowData = new Local();
        resourceLowData = new Resource();
        partnerLowData = new Partner();
        monsterLowData = new Mob();
        npcLowData = new NpcData();
		nonInteractiveNpcLowData = new NonInteractiveNpcData ();
        skillLowData = new SkillTables();
        NewbieData = new Newbie();
        dungeonData = new DungeonTable();
        etcData = new Etc();
        levelLowData = new Level();
        mailLowData = new Mail();
        questLowData = new Quest();
        enchantLowData = new Enchant();
        missionLowData = new MissionTable();
        mapLowData = new Map();
        partnerLvLowData = new PartnerLevel();
        shopLowData = new Shop();
        priceLowData = new Price();
        vipLowData = new Vip();
        iconLowData = new Icon();
        FormulaLowData = new Formula();
        guildLowData = new Guild();
        gatchaRewardLowData = new GatchaReward();
        loadingLowData = new Loading();
        titleLowData = new Title();
        welfareLowData = new Welfare();
        achievementLowData = new Achievement();
        activeLowData = new ActiveReward();
        pvpAutoRewardLowData = new PVP();

        charLowData.LoadLowData();
        itemLowData.LoadLowData();
        stringLowData.LoadLowData();
        resourceLowData.LoadLowData();
        partnerLowData.LoadLowData();
        monsterLowData.LoadLowData();
        npcLowData.LoadLowData();
		nonInteractiveNpcLowData.LoadLowData ();
        NewbieData.LoadLowData();
        skillLowData.LoadLowData();
        dungeonData.LoadLowData();
        etcData.LoadLowData();
        levelLowData.LoadLowData();
        mailLowData.LoadLowData();
        questLowData.LoadLowData();
        enchantLowData.LoadLowData();
        missionLowData.LoadLowData();
        mapLowData.LoadLowData();
        partnerLvLowData.LoadLowData();
        shopLowData.LoadLowData();
        priceLowData.LoadLowData();
        vipLowData.LoadLowData();
        iconLowData.LoadLowData();
        FormulaLowData.LoadLowData();
        guildLowData.LoadLowData();
        gatchaRewardLowData.LoadLowData();
        loadingLowData.LoadLowData();
        titleLowData.LoadLowData();
        welfareLowData.LoadLowData();
        achievementLowData.LoadLowData();
        activeLowData.LoadLowData();
        pvpAutoRewardLowData.LoadLowData();
    }

	public static void LoadLowDataALLForTableCheck()
	{
		charLowData = new Character();
		/*
		itemLowData = new Item();
		stringLowData = new Local();
		resourceLowData = new Resource();
		partnerLowData = new Partner();
		monsterLowData = new Mob();
		npcLowData = new NpcData();
		nonInteractiveNpcLowData = new NonInteractiveNpcData ();
		skillLowData = new SkillTables();
		NewbieData = new Newbie();
		dungeonData = new DungeonTable();
		etcData = new Etc();
		levelLowData = new Level();
		mailLowData = new Mail();
		questLowData = new Quest();
		enchantLowData = new Enchant();
		missionLowData = new MissionTable();
		mapLowData = new Map();
		partnerLvLowData = new PartnerLevel();
		shopLowData = new Shop();
		priceLowData = new Price();
		vipLowData = new Vip();
		iconLowData = new Icon();
		FormulaLowData = new Formula();
		guildLowData = new Guild();
		gatchaRewardLowData = new GatchaReward();
		loadingLowData = new Loading();
		titleLowData = new Title();
		welfareLowData = new Welfare();
		achievementLowData = new Achievement();
		activeLowData = new ActiveReward();
		pvpAutoRewardLowData = new PVP();
		*/
		charLowData.LoadLowDataForTableCheck();
		/*
		itemLowData.LoadLowData();
		stringLowData.LoadLowData();
		resourceLowData.LoadLowData();
		partnerLowData.LoadLowData();
		monsterLowData.LoadLowData();
		npcLowData.LoadLowData();
		nonInteractiveNpcLowData.LoadLowData ();
		NewbieData.LoadLowData();
		skillLowData.LoadLowData();
		dungeonData.LoadLowData();
		etcData.LoadLowData();
		levelLowData.LoadLowData();
		mailLowData.LoadLowData();
		questLowData.LoadLowData();
		enchantLowData.LoadLowData();
		missionLowData.LoadLowData();
		mapLowData.LoadLowData();
		partnerLvLowData.LoadLowData();
		shopLowData.LoadLowData();
		priceLowData.LoadLowData();
		vipLowData.LoadLowData();
		iconLowData.LoadLowData();
		FormulaLowData.LoadLowData();
		guildLowData.LoadLowData();
		gatchaRewardLowData.LoadLowData();
		loadingLowData.LoadLowData();
		titleLowData.LoadLowData();
		welfareLowData.LoadLowData();
		achievementLowData.LoadLowData();
		activeLowData.LoadLowData();
		pvpAutoRewardLowData.LoadLowData();
		*/
	}


}
