using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class EffectManager : MonoBehaviour , IManagerBase
{
	public Dictionary<string, Dictionary<string, string>> bodyEffectInfo = new Dictionary<string, Dictionary<string, string>>();

	public Dictionary<string, Dictionary<string, Stack<ParticleResizeContainer> >> bodyEffectPool = new Dictionary<string, Dictionary<string, Stack<ParticleResizeContainer> >>();

	public Stack<Stack<ParticleResizeContainer>> bodyEffectListPool = new Stack<Stack<ParticleResizeContainer>>();

	public Stack<ParticleResizeContainer> resizerContainerPool = new Stack<ParticleResizeContainer>();

	public void destroyAllUnitBodyEffect()
	{
		foreach(KeyValuePair<string, Dictionary<string, Stack<ParticleResizeContainer>>> kv in bodyEffectPool)
		{
			if(kv.Value != null)
			{
				if(kv.Value != null)
				{
					foreach(KeyValuePair<string, Stack<ParticleResizeContainer>> kv2 in kv.Value)
					{
						while(kv2.Value != null && kv2.Value.Count > 0)
						{
							ParticleResizeContainer p = kv2.Value.Pop();
							p.destroy();
							resizerContainerPool.Push(p);
						}
					}
				}
			}
		}


		foreach(KeyValuePair<string, Dictionary<string, string>> kv in bodyEffectInfo)
		{
			if(kv.Value != null)
			{
				foreach(KeyValuePair<string, string> kv2 in kv.Value)
				{
					ResourceManager.instance.destroyPrefab( kv2.Value );
				}
			}
		}
	}


	public void removeBodyEffect(Monster mon)
	{
		if(mon != null && mon.bodyEffects != null)
		{
			while(mon.bodyEffects.Count > 0)
			{
				ParticleResizeContainer p = mon.bodyEffects.Pop();
				if(p != null && p.gameObject != null)
				{
					p.gameObject.SetActive(false);
					bodyEffectPool[mon.resourceId][p.gameObject.name].Push( p );
					p.gameObject.transform.parent = GameManager.me.assetPool;
				}
			}

			bodyEffectListPool.Push(mon.bodyEffects);
			mon.bodyEffects = null;
		}

	}

	public void checkUnitBodyEffect(Monster mon, bool isLobby)
	{
		StartCoroutine(checkUnitBodyEffectCT(mon, isLobby));
	}

	IEnumerator checkUnitBodyEffectCT(Monster mon, bool isLobby)
	{
		if(isLobby == false)
		{
			if(GameManager.me.effectManager.isCompleteLoadEffect == false)
			{
				if(GameManager.me.effectManager.didStartLoadEffect == false)
				{
					GameManager.me.effectManager.startLoadEffects(true);
				}
			}
		}

		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{ 
			yield return null; 
		};

		// unitmonster 중 s급 캐릭터의 transform 이름으로 effect를 찾는다.
		if(mon.unitData != null && ( (mon.unitData.rare >= RareType.SS && isLobby) || (mon.unitData.rare >= RareType.S && isLobby == false)) ) 
		{
			Transform[] trs;

			if(bodyEffectInfo.ContainsKey(mon.monsterData.resource) == false)
			{
				while(GameManager.me.effectManager.isCompleteLoadEffect == false) { yield return null; };

				bodyEffectInfo.Add(mon.monsterData.resource, new Dictionary<string, string>());
				bodyEffectPool.Add(mon.monsterData.resource, new Dictionary<string, Stack<ParticleResizeContainer>>());
				
				trs = mon.transform.GetComponentsInChildren<Transform>();
				string n;
				
				foreach(Transform tr in trs)
				{
					if(string.IsNullOrEmpty(tr.name) == false)
					{
						Util.stringBuilder.Length = 0;
						Util.stringBuilder.Append("E_");
						Util.stringBuilder.Append(mon.resourceId.ToUpper());
						Util.stringBuilder.Append("_EFF_");
						Util.stringBuilder.Append(tr.name.ToUpper());
						n = Util.stringBuilder.ToString();
						
						if(GameManager.info.effectData.ContainsKey(n))
						{
							bodyEffectInfo[mon.monsterData.resource].Add(tr.name, n);
							bodyEffectPool[mon.monsterData.resource].Add(n, new Stack<ParticleResizeContainer>());
						}
					}
				}
			}
			
			bool hasBodyEffect = false;
			
			foreach(KeyValuePair<string, string> kv in bodyEffectInfo[mon.monsterData.resource])
			{
				hasBodyEffect = true;
				GameManager.me.effectManager.addLoadEffectData(kv.Value);
			}

			startLoadEffects(true);

			while(GameManager.me.effectManager.isCompleteLoadEffect == false) { yield return null; };

			if(hasBodyEffect && mon.gameObject.activeSelf)
			{
				trs = mon.transform.GetComponentsInChildren<Transform>();
				string n;

				Stack<ParticleResizeContainer> effectList;

				if(bodyEffectListPool.Count > 0)
				{
					effectList = bodyEffectListPool.Pop();
				}
				else
				{
					effectList = new Stack<ParticleResizeContainer>();
				}

				foreach(Transform tr in trs)
				{
					if(bodyEffectInfo[mon.monsterData.resource].ContainsKey(tr.name))
					{
						ParticleResizeContainer p;

						if(bodyEffectPool[mon.monsterData.resource][bodyEffectInfo[mon.monsterData.resource][tr.name]].Count > 0)
						{
							p = bodyEffectPool[mon.monsterData.resource][bodyEffectInfo[mon.monsterData.resource][tr.name]].Pop();
							p.gameObject.SetActive(true);
						}
						else
						{
							p = new ParticleResizeContainer(GameManager.info.effectData[bodyEffectInfo[mon.monsterData.resource][tr.name]].getPrefabEffect());
							p.gameObject.name = bodyEffectInfo[mon.monsterData.resource][tr.name];
						}

						p.gameObject.transform.parent = tr;
						p.gameObject.transform.localPosition = Vector3.zero;
						p.gameObject.transform.localScale = Vector3.one;
						_q.eulerAngles = Vector3.zero;
						p.gameObject.transform.localRotation = _q;

						effectList.Push(p);

						if(isLobby)
						{
							p.resize( 1.0f/(110.0f/mon.uiSize2) * 0.6f);
						}
						else
						{
							p.resize(1.0f);
						}
					}
				}



				if(effectList.Count > 0)
				{
					mon.bodyEffects = effectList;
				}
			}
		}
	}

	Quaternion _q = new Quaternion();
	
}
