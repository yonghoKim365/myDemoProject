using UnityEngine;
using System.Collections;

public class StudioCardBgSlot : MonoBehaviour 
{
	public Monster sample;
	public Player sampleHero;


	public Transform tfStage;

	public UISprite spSkillIcon, spSkillRare;

	public MeshRenderer renderer;

	public int rare
	{
//		set
//		{
//			for(int i = 0; i < 4; ++i)
//			{
//				bg[i].SetActive( i == value);
//			}
//		}

		set
		{
			if(value > RareType.SS) value = RareType.SS;
			renderer.sharedMaterial = RuneStudioMain.instance.smallCardBgMaterial[value];
		}

	}

	void clearCharacter()
	{
		if(sample != null)
		{
			GameManager.me.characterManager.cleanMonster(sample);
			sample = null;
		}

		if(sampleHero != null)
		{
			GameManager.me.characterManager.cleanMonster(sampleHero);
			sampleHero = null;
		}
	}

	public void setData(GameIDData info)
	{
		switch(info.type)
		{
		case GameIDData.Type.Equip:
			setParts(info);
			break;
		case GameIDData.Type.Unit:
			setUnit(info);
			break;
		case GameIDData.Type.Skill:
			setHeroSkillData(info);
			break;
		}
	}



	void setHeroSkillData(GameIDData info)
	{
		clearCharacter();

		spSkillIcon.gameObject.SetActive(true);
		spSkillRare.gameObject.SetActive(true);

		Icon.setSkillIcon(info.getSkillIcon(), spSkillIcon);

		rare = info.rare;

		switch(info.rare)
		{
		case RareType.D:
			spSkillRare.spriteName = "img_cardskill_d";
			break;
		case RareType.C:
			spSkillRare.spriteName = "img_cardskill_c";
			break;
		case RareType.B:
			spSkillRare.spriteName = "img_cardskill_b";
			break;
		case RareType.A:
			spSkillRare.spriteName = "img_cardskill_a";
			break;
		case RareType.S:
			spSkillRare.spriteName = "img_cardskill_s";
			break;
		case RareType.SS:
			spSkillRare.spriteName = "img_cardskill_ss";
			break;
		}
	}


	void setUnit(GameIDData info)
	{
		clearCharacter();

		spSkillIcon.gameObject.SetActive(false);
		spSkillRare.gameObject.SetActive(false);

		rare = info.rare;

		getMonster(sample, info.unitData, tfStage, info.rare, false);
	}


	void setParts(GameIDData info)
	{
		clearCharacter();
		
		spSkillIcon.gameObject.SetActive(false);
		spSkillRare.gameObject.SetActive(false);
		
		rare = info.rare;
		
		getPlayer(info);
	}


	public void hide()
	{
		clearCharacter();
	}


	Vector3 _v;
	Quaternion _q;

	public float customSizeRatio = 1.0f;

	void getMonster(Monster mon, UnitData md, Transform parent, int rare, bool isPlayAni = true)
	{
		mon = GameManager.me.characterManager.getMonster(false,true,md.resource,false);
		
		CharacterUtil.setRare(rare, mon);

		mon.setParent( parent );

		mon.cTransform.localScale *= 110.0f / mon.uiSize2 * 0.3f * customSizeRatio; //90.0f / sample.getHitObject().height;
		
		mon.container.SetActive(true);

		_v = mon.cTransform.localPosition;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		mon.cTransform.localPosition = _v;
		
		_q = parent.transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		parent.transform.localRotation = _q;
		
		_q = mon.cTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		mon.cTransform.localRotation = _q;
		
		
		_q = mon.tf.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		mon.tf.localRotation = _q;

		_v = mon.cTransform.localPosition;
		_v.y = GameManager.info.modelData[mon.resourceId].shotYPos;
		mon.cTransform.localPosition = _v;

		_v = mon.cTransform.position;
		mon.shadow.tf.position = _v;



		
		if(isPlayAni && mon.ani.GetClip(Monster.NORMAL) != null)
		{
			mon.ani.Play(Monster.NORMAL);
		}
		else
		{
			if(GameManager.info.modelData[mon.resourceId].poseTime <= 0)
			{
				mon.ani[Monster.NORMAL].time = GameManager.info.modelData[mon.resourceId].poseTime;
				mon.ani[Monster.NORMAL].speed = 0.0f;
				mon.ani.Play(Monster.NORMAL);

			}
			else if( mon.ani.GetClip(Monster.ATK) == null)
			{
				mon.ani[Monster.NORMAL].time = mon.ani[Monster.NORMAL].length / 2.0f;
				mon.ani[Monster.NORMAL].speed = 0.0f;
				mon.ani.Play(Monster.NORMAL);
			}
			else
			{
				mon.ani[Monster.ATK].time = GameManager.info.modelData[mon.resourceId].poseTime;
				mon.ani[Monster.ATK].speed = 0.0f;
				mon.ani.Play(Monster.ATK);
			}
		}

		sample = mon;
	}





	private GamePlayerData _gpd = new GamePlayerData(Character.LEO);
	void getPlayer(GameIDData data)
	{
		if(sampleHero == null)
		{
			sampleHero = (Player)GameManager.me.characterManager.getMonster(true,true,data.partsData.character,false);
		}
		
		if(GameDataManager.instance.heroes.ContainsKey(data.partsData.character))
		{
			GameDataManager.instance.heroes[data.partsData.character].copyTo(_gpd);
		}
		else
		{
			GameDataManager.instance.defaultHeroData[data.partsData.character].copyTo(_gpd);
		}


		if(data.partsData.type == HeroParts.WEAPON)
		{
			_gpd.partsWeapon = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 20.0f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-70,-575);
			_q.eulerAngles = new Vector3(0,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
		}
		else if(data.partsData.type == HeroParts.HEAD)
		{
			_gpd.partsHead = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 16.0f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-27,-576);
			_q.eulerAngles = new Vector3(3,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
			
		}
		else if(data.partsData.type == HeroParts.BODY)
		{
			_gpd.partsBody = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 21.8f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-181.8379f,-115.0146f,-476.4911f);
			_q.eulerAngles = new Vector3(-9.700012f,2.8f,1.5f);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 100.0f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
			//-175  -15  572
		}
		else if(data.partsData.type == HeroParts.VEHICLE)
		{
			_gpd.partsVehicle = new HeroPartsItem(data.partsData.character,data.serverId);
			//			popupCharacterCamera.fieldOfView = 25.8f;
			//			popupCharacterCamera.transform.localPosition = new Vector3(-176,-20,-573);
			_q.eulerAngles = new Vector3(6.31f,1.78f,0);
			//			popupCharacterCamera.transform.localRotation = _q;
			//			popupCharacterCamera.nearClipPlane = 273.4f;
			//			popupCharacterCamera.farClipPlane = 583.61f;
		}
		
		
		sampleHero.init(_gpd,true,false);
		
		sampleHero.container.SetActive(true);
		
		if(data.partsData.type == HeroParts.WEAPON)
		{
			int len = sampleHero.smrs.Length;
			for(int i = 0; i < len; ++i)
			{
				sampleHero.smrs[i].enabled = (sampleHero.smrs[i].name == data.partsData.resource || sampleHero.smrs[i].name == (data.partsData.resource + "_arrow"));
			}
		}
		else if(data.partsData.type == HeroParts.VEHICLE)
		{
			sampleHero.pet = (Pet)GameManager.me.characterManager.getMonster(true,true,data.partsData.resource.ToUpper(),false);
			sampleHero.pet.init(sampleHero);
			sampleHero.setVisible(false,false);
			sampleHero.pet.isEnabled = true;
			sampleHero.ani.Stop();
		}
		else if(data.partsData.type == HeroParts.BODY || data.partsData.type == HeroParts.HEAD)
		{
			int len = sampleHero.smrs.Length;
			for(int i = 0; i < len; ++i)
			{
				sampleHero.smrs[i].enabled = (sampleHero.smrs[i].name.Contains("weapon") == false);
			}
		}		
		
		
		
		sampleHero.setParent( tfStage );
		_v = sampleHero.cTransform.localPosition;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		sampleHero.cTransform.localPosition = _v;
		
		_q = tfStage.transform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		tfStage.transform.localRotation = _q;
		
		_q = sampleHero.cTransform.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		sampleHero.cTransform.localRotation = _q;
		
		
		_v.x = 0; _v.y =0; _v.z = 0;
		sampleHero.tf.localPosition = _v;
		
		_q = sampleHero.tf.localRotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		sampleHero.tf.localRotation = _q;


		if(data.partsData.type == HeroParts.WEAPON)
		{
			sampleHero.animation.Play("weapon");
			
			if(data.partsData.setPreviewPosition(sampleHero) == false)
			{
				if(data.partsData.character == "LEO")
				{
					_v = sampleHero.cTransform.localPosition;
					_v.y = -13f;
					sampleHero.cTransform.localPosition = _v;
				}
				
				sampleHero.cTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
			}
		}
		else if(data.partsData.type == HeroParts.HEAD)
		{
			_v = sampleHero.cTransform.localPosition;
			_v.x = 0.0f;
			_v.y = -61f;
			_v.z = 0.0f;
			sampleHero.cTransform.localPosition = _v;

			sampleHero.animation.Play("idle");
			sampleHero.cTransform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
		}
		else
		{
			sampleHero.animation.Play("idle");
			sampleHero.cTransform.localScale = Vector3.one;
		}


		_v = sampleHero.cTransform.position;
		sampleHero.shadow.transform.position = _v;



	}








}
