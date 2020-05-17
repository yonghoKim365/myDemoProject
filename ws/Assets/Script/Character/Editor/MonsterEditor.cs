using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(Monster))]
public class MonsterEditor : Editor 
{
	Monster _mon;
	
	void OnEnable()
	{
		_mon = target as Monster;
	}

	int buff = 10;

	int spawnEffect = 1;

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginVertical();

		ModelData md = GameManager.info.modelData[_mon.resourceId];

		float scale = md.scale * 0.01f;

		float originalShadowSize = md.shadowSize/scale;
		float ss = EditorGUILayout.FloatField("ShadowSize:",originalShadowSize);

		if(Mathf.Approximately(ss,originalShadowSize) == false)
		{
			md.shadowSize = ss * scale;
			_mon.initShadowAndEffectSize();
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical();

		float originalSummonEffectSize = md.summonEffectSize/scale;
		float ses = EditorGUILayout.FloatField("SpawnSize:",originalSummonEffectSize);
		
		if(Mathf.Approximately(ses,originalSummonEffectSize) == false)
		{
			md.summonEffectSize = ses * scale;
			_mon.initShadowAndEffectSize();
		}

		spawnEffect = EditorGUILayout.IntField("소환",spawnEffect);

		if(GUILayout.Button("소환이펙트"))
		{
			if(spawnEffect >= RareType.D && spawnEffect <= RareType.SS)
			{
				GameManager.info.effectData[ UnitSlot.getSummonEffectByRare(spawnEffect) ].getEffect(-1000,_mon.cTransform.position, null, null, _mon.summonEffectSize);
			}
			else
			{
				GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_ENEMY].getEffect(-1000,_mon.cTransform.position, null, null, _mon.summonEffectSize);
			}
		}

		if(GUILayout.Button("소환이펙트"))
		{
			GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_RARE].getEffect(-1000,_mon.cTransform.position, null, null, _mon.summonEffectSize);
		}

		buff = EditorGUILayout.IntField("BuffType",buff);

		if(GUILayout.Button("버프 아이콘"))
		{
			if(GameManager.me.effectManager.loadAllEffects == false)
			{
				GameManager.me.effectManager.loadAllSkillEffect();
			}
			else
			{
				if(GameManager.info.skillEffectSetupData.ContainsKey(buff))
				{
					if( string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[buff].upIcon ) == false)
					{
						_mon.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[buff].upIcon);
					}
					else
					{
						_mon.characterEffect.addIconBuff(GameManager.info.skillEffectSetupData[buff].downIcon);
					}
				}
			}
		}


		if(GUILayout.Button("버프 이펙트"))
		{
			if(GameManager.me.effectManager.loadAllEffects == false)
			{
				GameManager.me.effectManager.loadAllSkillEffect();
			}
			else
			{
				if(GameManager.info.skillEffectSetupData.ContainsKey(buff))
				{
					EffectData ed = null;
					
					if( string.IsNullOrEmpty( GameManager.info.skillEffectSetupData[buff].effUp ) == false)
					{
						GameManager.info.effectData.TryGetValue(GameManager.info.skillEffectSetupData[buff].effUp, out ed);
					}
					else
					{
						GameManager.info.effectData.TryGetValue(GameManager.info.skillEffectSetupData[buff].effDown, out ed);
					}
					
					if(ed != null)
					{
						ed.getParticleEffectByCharacterSize(-1000, _mon, null, _mon.tf);
					}
				}
			}
		}


		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginVertical();

		float originalEffectSize = md.effectSize/scale;
		float es = EditorGUILayout.FloatField("HitSize:",originalEffectSize);
		
		if(Mathf.Approximately(es,originalEffectSize) == false)
		{
			md.effectSize = es * scale;
			_mon.initShadowAndEffectSize();
		}


		if(GUILayout.Button("피격이펙트"))
		{
			_mon.playDamageSoundAndEffect(-1000);
		}

		EditorGUILayout.EndVertical();


		DrawDefaultInspector();
	}
}
