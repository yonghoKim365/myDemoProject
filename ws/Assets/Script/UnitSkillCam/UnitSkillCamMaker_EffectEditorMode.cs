using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public partial class UnitSkillCamMaker : MonoBehaviour 
{
	private MonsterData _md;

	public Monster prevCreatedUnit = null;
	public Monster prevCreatedEnemy = null;

	public BulletPatternData bulletPattern = new BulletPatternData();




	public void deleteHeroMonster()
	{
		if(prevCreatedUnit != null)
		{
			prevCreatedUnit.damageDead(false);
		}
		
		if(prevCreatedEnemy != null)
		{
			prevCreatedEnemy.damageDead(false);
		}
	}


	public void createNewMonster(GameObject go, float px, float ex)
	{
		if(go == null) return;

		deleteHeroMonster();

		_md = new MonsterData();

		_md.id = go.name.ToUpper();
		_md.name = go.name;
		_md.category = MonsterCategory.Category.UNIT;

		if(GameManager.info.monsterData.ContainsKey(_md.id) == false)
		{
			GameManager.info.monsterData.Add(_md.id, _md);
		}
		else
		{
			GameManager.info.monsterData[_md.id] = _md;
		}

		ModelData modelData;

		if(GameManager.info.modelData.ContainsKey(go.name))
		{
			modelData = GameManager.info.modelData[go.name];
		}
		else
		{
			modelData = new ModelData();
			GameManager.info.modelData.Add(go.name, modelData);
		}

		if(modelData.particleColors == null || modelData.particleColors.Length == 0)
		{
			modelData.particleColors = new Color[1];
			modelData.particleColors[0] = Color.white;
		}


		GameObject gob = (GameObject)Instantiate(go);

		BoxCollider bc = gob.AddComponent<BoxCollider>();
		
		bc.center = new Vector3(0,50,0);
		bc.extents = new Vector3(100,100,100);

		Monster m = gob.AddComponent<Monster>();
		_md.resource = go.name;
		m.resourceId = go.name;

		CharacterManager cm = GameManager.me.characterManager;

		if(cm.monsterResource.ContainsKey(m.resourceId) == false)
		{
			cm.monsterResource.Add(m.resourceId, m);
		}
		else
		{
			GameObject.Destroy(cm.monsterResource[m.resourceId]);

			cm.monsterResource[m.resourceId] =  m;
		}

		m.gameObject.SetActive(false);

		m.rendererInit(true);
		//m.gameObject.SetActive(false);

		UnitData un = GameManager.info.unitData["UN10100101"];

#if UNITY_EDITOR
		un.resource = _md.id;
#endif

		if(GameManager.info.aniData.ContainsKey(m.resourceId) == false)
		{
			GameManager.info.aniData.Add(m.resourceId, new Dictionary<string, AniData>());
		}

		prevCreatedUnit = GameManager.me.mapManager.addMonsterToStage(null, null, true, null, un.id , new Vector3(px, 0, 0  ));

		prevCreatedEnemy = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, un.id , new Vector3(ex, 0, 0  ));


		bulletPattern.id = prevCreatedUnit.monsterData.id;
		prevCreatedUnit.nowBulletPatternId = bulletPattern.id;
		prevCreatedEnemy.bulletPatternId = bulletPattern.id;

		bulletPattern.bulletIdLength = 2;
		bulletPattern.ids = new string[2];

		bulletPattern.ids[0] = "B0";
		bulletPattern.ids[1] = "EMPTY";

		GameManager.info.bulletPatternData[bulletPattern.id] = bulletPattern;

		GameManager.info.bulletData[bulletPattern.ids[0]] = new BulletData();

	}


	public static string CURRENT_ANI_NAME = null;

	public void changeAni(int type, bool isPlayerSide, AniData aniData, EffectEditorData.EffectEditBulletData bulletData, string customAniName = null)
	{
		if(prevCreatedUnit == null) return;

		CURRENT_ANI_NAME = customAniName;

		UnitData un = GameManager.info.unitData["UN10100101"];

		bulletPattern.ids[0] = "B0";

		BulletData bd = new BulletData();

		bd.destroy = false;
		bd.useDestroyEffect = false;
		bd.hp = 0;
		bd.hitActionReward = new Reward(null);
		bd.deadActionReward = new Reward(null);
		bd.destoryAction = new Reward(null);

		GameManager.info.bulletData["B0"] = bd;

		bd.attachEffectToShotPoint = bulletData.bulletEffect.attachedToParent;
		bd.id = "B0";



		if(type == 0) type = 1;

		if(bulletData.attackType >= 3)
		{
			type = bulletData.attackType;
		}

		AttackData ad = null;

		int[] opt = bulletData.atkTypeOptions(type).ToArray();
		ad = AttackData.getAttackData(type,opt[0],opt[1],opt[2],opt[3],opt[4],opt[5],opt[6]);

		if(ad != null)
		{
			un.attackType = ad;
			un.attackType.init( AttackData.AttackerType.Unit, AttackData.AttackType.Attack);
		}


		if(prevCreatedUnit != null)
		{
			prevCreatedUnit.unitData = un;
		}

		if(prevCreatedEnemy != null)
		{
			prevCreatedEnemy.unitData = un;
		}

		Monster mon = null;
		Dictionary<string, AniData> dic = null;

#if UNITY_EDITOR
		if(aniData.effect != null)
		{
			foreach(AniDataEffect ade in aniData.effect)
			{
				if(ade.goEffect != null)
				{
					EffectData ed = new EffectData();
					ed.id = ade.id;
					ed.resource = ade.goEffect.name;

					ed.type = EffectData.ResourceType.PARTICLE;

					GameManager.info.effectData[ed.id] = ed;

					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, ade.goEffect);
				}
			}
		}
#endif

		aniData.effectNum = (( aniData.effect == null)?0:aniData.effect.Length);


		if(bulletData.use)
		{
			#if UNITY_EDITOR
			for(int i = 0; i < 1; ++i)
			{
				if(bulletData.bulletEffect.effect != null)
				{
					EffectData ed = new EffectData();
					ed.id = "E_"+bulletData.bulletEffect.effect.name.ToString().ToUpper();
					ed.resource = bulletData.bulletEffect.effect.name.ToString();
					
					if(bulletData.bulletEffect.type == EffectEditorData.EffectEditBulletDetailData.Type.Object)
					{
						ed.type = EffectData.ResourceType.PREFAB;
					}
					else
					{
						ed.type = EffectData.ResourceType.PARTICLE;
					}
					
					GameManager.info.effectData[ed.id] = ed;



					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, bulletData.bulletEffect.effect);

					string effStr = "";

					switch(bulletData.bulletEffect.type)
					{
					case EffectEditorData.EffectEditBulletDetailData.Type.Indie:
						effStr = "IE:";
						break;
					case EffectEditorData.EffectEditBulletDetailData.Type.Object:
						effStr = "P:";
						break;
					case EffectEditorData.EffectEditBulletDetailData.Type.Particle:
						effStr = "E:";
						break;
					}

					effStr += ed.id + ",";

					effStr += (bulletData.bulletEffect.attachedToParent?"Y":"N") + ",";

					effStr += "";//(bulletData.bulletEffect.useOption?"Y":"N") + "";

					bd.setEffect( effStr );

				}
				else
				{
					bd.setEffect(string.Empty);
				}
			}


			for(int i = 0; i < 1; ++i)
			{
				if(bulletData.bulletEffect.goHitEffect != null)
				{
					EffectData ed = new EffectData();
					ed.id = "E_"+bulletData.bulletEffect.goHitEffect.name.ToString().ToUpper();
					ed.resource = bulletData.bulletEffect.goHitEffect.name.ToString();
					
					ed.type = EffectData.ResourceType.PARTICLE;

					GameManager.info.effectData[ed.id] = ed;
					
					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, bulletData.bulletEffect.goHitEffect);

					bd.hitEffect = ed.id;
					bulletPattern.ids[1] = ed.id;
				}
				else
				{
					bd.hitEffect = string.Empty;
				}
			}



			for(int i = 0; i < 1; ++i)
			{
				if(bulletData.bulletEffect.goGroundEffect != null)
				{
					EffectData ed = new EffectData();
					ed.id = "E_"+bulletData.bulletEffect.goGroundEffect.name.ToString().ToUpper();
					ed.resource = bulletData.bulletEffect.goGroundEffect.name.ToString();
					
					ed.type = EffectData.ResourceType.PARTICLE;
					
					GameManager.info.effectData[ed.id] = ed;
					
					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, bulletData.bulletEffect.goGroundEffect);
				}
			}

			for(int i = 0; i < 1; ++i)
			{
				if(bulletData.bulletEffect.goDestroyEffect != null)
				{
					EffectData ed = new EffectData();
					ed.id = "E_"+bulletData.bulletEffect.goDestroyEffect.name.ToString().ToUpper();
					ed.resource = bulletData.bulletEffect.goDestroyEffect.name.ToString();
					
					ed.type = EffectData.ResourceType.PARTICLE;
					
					GameManager.info.effectData[ed.id] = ed;
					
					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, bulletData.bulletEffect.goDestroyEffect);
					
					bd.useDestroyEffect = true;
					bd.destroyEffectId = ed.id;

					if(bulletData.bulletEffect.destroyOption == EffectEditorData.EffectEditBulletDetailData.DestroyEffType.BulletRotation)
					{
						bd.destroyEffectOption = BulletData.DestroyEffectOptionType.UseBulletRotation;
					}
					else
					{
						bd.destroyEffectOption = BulletData.DestroyEffectOptionType.Normal;
					}
				}
				else
				{
					bd.useDestroyEffect = false;
					bd.destroyEffectId = string.Empty;
				}
			}


			for(int i = 0; i < 1; ++i)
			{
				if(bulletData.bulletEffect.chainLighting != null)
				{
					EffectData ed = new EffectData();
					ed.id = "E_"+bulletData.bulletEffect.chainLighting.name.ToString().ToUpper();
					ed.resource = bulletData.bulletEffect.chainLighting.name.ToString();
					
					ed.type = EffectData.ResourceType.CHAIN;
					
					GameManager.info.effectData[ed.id] = ed;

					bulletPattern.ids[0] = ed.id;

					UnitSkillCamMaker.instance.usePrefabEffect = true;
					GameManager.resourceManager.setPrefabFromAssetBundle(ed, null, bulletData.bulletEffect.chainLighting.gameObject);
				}
			}

			#endif


			aniData.delayLength = bulletData.actionFrame.Count;
			aniData.delay = new float[aniData.delayLength];

			for(int i = 0; i < bulletData.actionFrame.Count; ++i)
			{
				aniData.delay[i] = ((float)bulletData.actionFrame[i])/30f;
			}

			aniData.shootingPointLength = bulletData.shotPoint.Count;
			aniData.shootingPositions = new int[aniData.shootingPointLength][];
			
			for(int i = 0; i < aniData.shootingPointLength; ++i)
			{
				aniData.shootingPositions[i] = new int[3];
				aniData.shootingPositions[i][0] = (int)( bulletData.shotPoint[i].x );
				aniData.shootingPositions[i][1] = (int)( bulletData.shotPoint[i].y );
				aniData.shootingPositions[i][2] = (int)( bulletData.shotPoint[i].z );
			}

			if(bulletData.targetTransform.Count > 0)
			{
				aniData.shootingPoint = bulletData.targetTransform.ToArray();
				aniData.shootingHandLength = aniData.shootingPoint.Length;
			}
			else
			{
				aniData.shootingPoint = new string[0];
				aniData.shootingHandLength = aniData.shootingPoint.Length;
			}
		}
		else
		{
			aniData.delayLength = 1;
			aniData.delay = new float[]{1};
			aniData.shootingPointLength = 0;
			aniData.shootingPoint = new string[0];
			aniData.shootingHandLength = aniData.shootingPoint.Length;
		}

//		if(bulletData.actionFrame.Count > 0)

		if(isPlayerSide && prevCreatedUnit != null)
		{
			if(prevCreatedUnit == null) return;
			mon = prevCreatedUnit;

			if(GameManager.info.aniData.TryGetValue(mon.resourceId, out dic) == false)
			{
				dic = new Dictionary<string, AniData>();
				GameManager.info.aniData.Add(mon.resourceId, dic);
			}
			
			if(dic.ContainsKey(aniData.ani)) dic[aniData.ani] = aniData;
			else dic.Add(aniData.ani,  aniData);

			GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(mon);

			if(GameManager.me.characterManager.monsters.Count > 0)
			{
				try
				{
					mon.setTarget(GameManager.me.characterManager.monsters[0]);
					mon.unitData.attackType.lookTargetAndAttack(mon, GameManager.me.characterManager.monsters[0], mon.unitData.attackType.isShortType);

				}
				catch
				{

				}

			}
			
		}
		else if(prevCreatedEnemy != null)
		{
			if(prevCreatedEnemy == null) return;
			mon = prevCreatedEnemy;

			if(GameManager.info.aniData.TryGetValue(mon.resourceId, out dic) == false)
			{
				dic = new Dictionary<string, AniData>();
				GameManager.info.aniData.Add(mon.resourceId, dic);
			}
			
			if(dic.ContainsKey(aniData.ani)) dic[aniData.ani] = aniData;
			else dic.Add(aniData.ani,  aniData);

			GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(mon);

			if(GameManager.me.characterManager.playerMonster.Count > 0)
			{
				try
				{
					mon.setTarget(GameManager.me.characterManager.playerMonster[0]);
					mon.unitData.attackType.lookTargetAndAttack(mon, GameManager.me.characterManager.playerMonster[0], mon.unitData.attackType.isShortType);

				}
				catch
				{

				}
			}
		}


	}
	
	
}
