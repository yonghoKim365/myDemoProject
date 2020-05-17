using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class CharacterUtil
{
	public CharacterUtil ()
	{
	}

	public static void setTexture(SkinnedMeshRenderer smr, string resourceName)
	{
		if(smr.sharedMaterial != null)
		{
			if(smr.sharedMaterial.mainTexture != null)
			{
				if(smr.sharedMaterial.mainTexture.name != resourceName) smr.sharedMaterial.mainTexture = GameManager.resourceManager.getPartsTexture(resourceName);
			}
			else
			{
				smr.sharedMaterial.mainTexture = GameManager.resourceManager.getPartsTexture(resourceName);
			}
		}
	}


	public static void setPartsTexture(SkinnedMeshRenderer smr, HeroPartsData hpd)
	{
		string name;

		name = hpd.texture;

//		Debug.Log(name);
		
		if(smr.sharedMaterial != null)
		{
			if(smr.sharedMaterial.mainTexture != null)
			{
				if(smr.sharedMaterial.mainTexture.name != name) smr.sharedMaterial.mainTexture = GameManager.resourceManager.getPartsTexture(name);
			}
			else
			{
				smr.sharedMaterial.mainTexture = GameManager.resourceManager.getPartsTexture(name);
			}
		}
	}


	
	public static void setRare(int rareType, Monster mon)
	{
		// 이제 아웃라인 레어도를 안쓴다.
		return; // FUCKYOU

		/*
		if(PerformanceManager.isLowPc)
		{
			mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(true);

			if(mon.smrs.Length > mon.modelRendererIndex[ModelData.WITH_OUTLINE] && mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1)
			{
				mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(false);
			}

			if(mon.smrs.Length > mon.modelRendererIndex[ModelData.WITH_RARELINE] && mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1)
			{
				mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(false);
			}

			return;
		}
		*/

		rareType = RareType.D;

		if(rareType == RareType.D)
		{
			if(mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1)
			{
				mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(true);
				if(mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(false);
				if(mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(false);
			}
			else 
			{
				if(mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(true);
				if(mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(false);
				if(mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(false);
			}
		}
		else
		{
			if(mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1)
			{
				if(mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(true);
				if(mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(false);
				mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(true);
			}
			else if(mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1)
			{
				mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(true);
				if(mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(false);
				if(mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(false);
			}
			else
			{
				mon.smrs[mon.modelRendererIndex[ModelData.WITHOUT_OUTLINE]].gameObject.SetActive(true);
				if(mon.modelRendererIndex[ModelData.WITH_OUTLINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_OUTLINE]].gameObject.SetActive(false);
				if(mon.modelRendererIndex[ModelData.WITH_RARELINE] > -1) mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.SetActive(false);
			}


			if(mon.smrs.Length <= mon.modelRendererIndex[ModelData.WITH_RARELINE] || mon.modelRendererIndex[ModelData.WITH_RARELINE] < 0)
			{
#if UNITY_EDITOR
				UnityEngine.Debug.Log(" == 모델링에 외곽 라인이 없다!!!!  == ");
#endif
				return;
			}

			if(mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].gameObject.activeSelf)
			{
				switch(rareType)
				{
				case RareType.C:
					if( mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial == null)
					{
						mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial = getMaterial();
					}
					mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial.mainTexture = ResourceManager.instance.rareTexture;
					break;

				case RareType.B:
					if( mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial == null)
					{
						mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial = getMaterial();;
					}

					mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial.mainTexture = ResourceManager.instance.superRareTexture;
					break;					
				case RareType.A:
					if( mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial == null)
					{
						mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial = getMaterial();
					}

					mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial.mainTexture = ResourceManager.instance.legendTexture;
					break;					
				}
			}
		}
	}


	private static Stack<Material> _materialPool = new Stack<Material>();
	static Material getMaterial()
	{
		if(_materialPool.Count > 0) return _materialPool.Pop();
		else return new Material(Shader.Find("Unlit/Texture Color"));
	}

	public static void removeRareLineMaterial(Monster mon, bool isOriginal = false)
	{
		if(mon == null || mon.smrs == null || mon.smrs.Length <= mon.modelRendererIndex[ModelData.WITH_RARELINE] || mon.modelRendererIndex[ModelData.WITH_RARELINE] < 0)
		{
			return;
		}
		
		if(mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial != null)
		{
			mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial.mainTexture = null;
			mon.smrs[mon.modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial = null;
		}
	}



	public static void setRareNotMergedHeroMonster(Monster mon)
	{
		if(mon.smrs == null) return;
		foreach(SkinnedMeshRenderer smr in mon.smrs)
		{
			smr.gameObject.SetActive( smr.name.Contains(ModelData.OUTLINE_RARE_NAME) == false );
		}
	}



}

