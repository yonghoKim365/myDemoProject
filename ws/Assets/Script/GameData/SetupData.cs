using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupData
{
	public SetupData ()
	{
	}

	public float playerHitCooltime = 0.0f;
//	public bool canUseSkillAndPetSlotAtStart = false;
	

	

	
	public float hpBarShowTime = 2.0f;
	
	public Xfloat recoveryDelay = 1.3f;

	public Xfloat waitPlayerRecoveryDelaySp = 2.6f;
	public Xfloat waitPlayerRecoveryDelayMp = 2.6f;
	public Xfloat waitPlayerRecoveryHpPer = 0.01f;

	
	public int defaultHp = 0;
	public int defaultSp = 0;
	public int defaultMp = 0;
	
	public bool useHpAsSp = false;
	
	public float characterAppearAndHitPercent = 100.0f;
	public int dropItemRangeX = 0;
	public int dropItemRangeY = 0;
	public int maxStage = 1;
	public Dictionary<int, string> monsterRangeLogic = new Dictionary<int, string>();
	public ScrollLevelData[] scrollLevelData = {new ScrollLevelData(),new ScrollLevelData(),new ScrollLevelData(),new ScrollLevelData(),new ScrollLevelData(),new ScrollLevelData()};
	public float scrollGaugeDecreaseTimeOffset = 1.0f;
	public float scrollGaugeDecreaseValue = 1.0f;
	public float scrollGaugeDecreaseWaitTime = 1.0f;
	
	
	public string[] defaultSkillInven;
	public string[] defaultUnitInven;
	public string[] defaultPartsInven;
	
	public float[] defaultPlayCamSpringValue;
	
	public CameraInfo[] cameraPreset;
	public int cameraPresetLength = 1;


	public string[] defaultLeo;
	public string[] defaultLeo2;

	public string[] defaultKiley;
	public string[] defaultKiley2;

	public string[] defaultChloe;
	public string[] defaultChloe2;

	public string[] defaultLoadEffects;

	public Xint[] tagKnuckBackValue = new Xint[2];

	public Xint tagStunTime = 2500;

	public Xfloat tagCoolTime = 20.0f;


	public string tagTest1Hero = "LEO";
	public string[] tagTest1Equips;
	public string[] tagTest1Unit;
	public string[] tagTest1Skill;

	public string tagTest2Hero = "CHLOE";
	public string[] tagTest2Equips;
	public string[] tagTest2Unit;
	public string[] tagTest2Skill;



}

public class ScrollLevelData
{
	public int addScrollvalue = 0;
	public int maxValue = 0;
}


public class CameraInfo
{
	public Vector3 position = new Vector3();
	public Vector3 rotation = new Vector3();
	public float fov;
}