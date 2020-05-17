using UnityEngine;
using System.Collections;

public class UICardFrame : MonoBehaviour 
{
	// 얘를 켜면 스킬 아이콘이 보이고 줄어드는 애니메이션이 동작된다.
	public GameObject goMakeSkillIconCover;

	public GameObject goNormalFrame, goRareFrame, goSuperRareFrame, goLegend4Frame, goLegend5Frame, goSSFrame;

	public GameObject goReinforceRibon;

	public GameObject goTranscendContainer;

	public UILabel lbName, lbLevel, lbMaxLevel, lbUpDownLevel, lbTranscendLevel, lbPlusTranscendLevel;

	public void showFrame(int rare, bool includeBg = true)
	{
		goMakeSkillIconCover.SetActive(false);
		goNormalFrame.SetActive(false); 
		goRareFrame.SetActive(false); 
		goSuperRareFrame.SetActive(false); 
		goLegend4Frame.SetActive(false); 
		goLegend5Frame.SetActive(false);
		goSSFrame.SetActive(false);
		if(goReinforceRibon != null) goReinforceRibon.SetActive(false);
		if(goTranscendContainer != null) goTranscendContainer.SetActive(false);

		if(includeBg) RuneStudioMain.instance.showCard512Studio(rare);
		
		switch(rare)
		{
		case RareType.D:
			goNormalFrame.SetActive(true); 
			break;
		case RareType.C:
			goRareFrame.SetActive(true); 
			break;
		case RareType.B:
			goSuperRareFrame.SetActive(true); 
			break;
		case RareType.A:
			goLegend4Frame.SetActive(true); 
			break;
		case RareType.S:
			goLegend5Frame.SetActive(true);
			break;
		case RareType.SS:
			goSSFrame.SetActive(true);
			break;
		}
	}


	public GameIDData data = null;

	public void setData(string id, GameIDData.Type type, bool include512Bg = false)
	{
		if(data == null) data = new GameIDData();
		data.parse(id, type);

		switch(data.type)
		{
		case GameIDData.Type.Equip:
			lbName.text = data.partsData.name;
			break;
		case GameIDData.Type.Skill:
			lbName.text = data.skillData.name;
			break;
		case GameIDData.Type.Unit:
			lbName.text = data.unitData.name;
			break;
		}

		setLevel(data.level);

		lbMaxLevel.text = data.maxLevel.ToString() ;

		showFrame(data.rare, include512Bg);
	}


	public static string getRareColorName(int rare, string name)
	{
		switch(rare)
		{
		case RareType.SS:
			return "[DF41F2]"+name +"[-]";
		case RareType.S:
			return "[FF7200]"+name +"[-]";
		case RareType.A:
			return "[35ACF7]"+name +"[-]";
		case RareType.B:
			return "[85EB2E]"+name +"[-]";
		case RareType.C:
			return "[FFCD33]"+name +"[-]";
		case RareType.D:
			return "[EABB89]"+name +"[-]";
		}

		return name;
	}



	public void setLevel(int currentLevel, int levelDiff = -1)
	{
		lbLevel.text = "l"+currentLevel+"/";

		Vector3 v = lbLevel.transform.localPosition;
		v.x += lbLevel.printedSize.x;
		v.y = lbMaxLevel.transform.localPosition.y;
		lbMaxLevel.transform.localPosition = v;

		if(lbUpDownLevel == null) return;

		if(levelDiff > 0)
		{
			lbUpDownLevel.enabled = true;
			lbUpDownLevel.text = "u"+levelDiff+"[-]";
		}
		else
		{
			lbUpDownLevel.enabled = false;
		}
	}


	public void setTranscendLevel(GameIDData gd)
	{
		if(goTranscendContainer != null)
		{
			if(gd.totalPLevel <= 0)
			{
				goTranscendContainer.SetActive(false);
			}
			else
			{
				goTranscendContainer.SetActive(true);
				lbTranscendLevel.text = "+[ffde25]"+gd.totalPLevel+"[-]";

				if(GameManager.me.uiManager.popupReforege.step == 2)
				{
					int plusLevel = gd.totalPLevel - GameManager.me.uiManager.popupReforege.currentLevel;

					if(plusLevel > 0)
					{
						lbPlusTranscendLevel.enabled = true;
						lbPlusTranscendLevel.text = "u"+plusLevel;
					}
					else
					{
						lbPlusTranscendLevel.enabled = false;
					}

					//lbPlusTranscendLevel.text = "";
				}
				else
				{
					lbPlusTranscendLevel.enabled = false;
				}
			}
		}
	}



	public void showDescriptionPanel(bool useDescriptionPanel, bool useSkillIconFrame)
	{
		if(useDescriptionPanel)
		{
			RuneStudioMain.instance.cam256.gameObject.SetActive(false);
			goMakeSkillIconCover.SetActive(false);
			transform.localPosition = new Vector3(0,0,0);
		}
		else
		{
			RuneStudioMain.instance.cam256.gameObject.SetActive(true);

			if(useSkillIconFrame)
			{
				goMakeSkillIconCover.SetActive(true);
				goMakeSkillIconCover.animation.Stop();
				goMakeSkillIconCover.transform.localScale = new Vector3(0.93454f, 1.666676f, 0.9402466f);
			}

			transform.localPosition = new Vector3(239,-47,0);
		}
	}


	public void showTranscendPanel()
	{
		goMakeSkillIconCover.SetActive(false);
		transform.localPosition = new Vector3(-122,-3,0);
	}


	public void hide()
	{
		transform.localPosition = new Vector3(-9999,-3,0);
	}


}
