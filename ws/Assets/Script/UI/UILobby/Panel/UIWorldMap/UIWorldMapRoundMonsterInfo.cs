using UnityEngine;
using System.Collections;

public class UIWorldMapRoundMonsterInfo : MonoBehaviour {

	public UISprite spBorder, spBackground, spHidden;
	public UISprite spIcon;
	public UILabel lbName;

	public Transform cachedTransform;

	public string checkId;
	public string iconId;
	public string name;
	public int rare;
	public int level;

	public int defaultIconDepth = 27;

	public enum Type
	{
		Normal, Hidden, Player
	}


	void Awake()
	{
		cachedTransform = transform;
	}

	void OnDestroy()
	{
		spBorder = null;
		spIcon = null;
		lbName = null;

		checkId = null;
		iconId = null;
		name = null;
	}

	public void resetPosition()
	{
		cachedTransform.localPosition = _originalPos;
		gameObject.SetActive(true);
	}

	Vector3 _originalPos;
	public void setPosition(Vector3 setPos)
	{
		_originalPos = setPos;
		cachedTransform.localPosition = new Vector3(0,1000,0);
		//resetPosition();
	}


	public void setData(string id, string monName, int type, int lv, Type iconType = Type.Normal)
	{
		checkId = id;
		iconId = id;
		name = monName;
		rare = type;
		level = lv;

		gameObject.SetActive(true);

		if(iconType == Type.Hidden)
		{
			spHidden.spriteName = "mark_question";
			spHidden.enabled = true;
			spIcon.enabled = false;
		}
		else if(iconType == Type.Player)
		{
			spHidden.spriteName = id;
			spHidden.enabled = true;
			spIcon.enabled = false;
		}
		else
		{
			spHidden.enabled = false;
			spIcon.enabled = true;

			MonsterData.setUnitIcon(iconId, spIcon, defaultIconDepth, false);
			
			spIcon.MakePixelPerfect();
			
			spIcon.width = 94;
			spIcon.height = 94;

		}

		lbName.text = name;

		switch(rare) // 1은 히어로 몬스터, 2는 히어로 MIDDLE_BOS // 3은 일반.
		{
		case 1:
			spBorder.spriteName = "img_monster_listbg1";
			lbName.color = new Color(0.9f,0.73f,0.08f);
			spBackground.color = new Color(79f/255f,45f/255f,23f/255f);
			spBackground.enabled = true;
			break;
		case 2:
			spBorder.spriteName = "img_monster_listbg2";
			lbName.color = new Color(0.855f,0.85f,0.84f);
			spBackground.color = new Color(53f/255f,54f/255f,68f/255f);
			spBackground.enabled = true;
			break;
		case 3:
			spBorder.spriteName = "img_monster_listbg3";
			lbName.color = new Color(0.95f,0.54f,0.38f);
			//spBackground.color = Color.white;
			spBackground.enabled = false;
			break;
		}
	}

}
