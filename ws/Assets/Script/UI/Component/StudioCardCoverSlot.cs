using UnityEngine;
using System.Collections;

public class StudioCardCoverSlot : MonoBehaviour {

	public UILabel lbName, lbLevel, lbMaxLevel;

	public GameObject[] bg;


	public int rare
	{
		set
		{
			for(int i = 0; i < 5; ++i)
			{
				bg[i].SetActive( i == value);
			}
		}
	}

	public void setData(GameIDData infoData)
	{
		rare = infoData.rare;

		lbLevel.text = "l"+infoData.level + "/";

		switch(infoData.type)
		{
		case GameIDData.Type.Equip:
			lbName.text = infoData.partsData.name;
			break;
		case GameIDData.Type.Unit:
			lbName.text = infoData.unitData.name;

			break;
		case GameIDData.Type.Skill:
			lbName.text = infoData.skillData.name;
			break;
		}

		lbMaxLevel.text = infoData.maxLevel.ToString();

		Vector3 v = lbLevel.transform.localPosition;
		v.x += lbLevel.printedSize.x;
		v.y = lbMaxLevel.transform.localPosition.y;
		lbMaxLevel.transform.localPosition = v;


	}


}
