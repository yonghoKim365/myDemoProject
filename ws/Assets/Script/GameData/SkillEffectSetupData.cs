using System;
using System.Collections.Generic;

sealed public class SkillEffectSetupData
{
	
	public int id;
	public string name;


	public int len;
	public bool useLevelCorrection = false;
	public bool isDurationType = false;
	public string effUp = null;
	public string effDown = null;

	public string effUpLoop = null;
	public string effDownLoop = null;

	public string soundUp = "";
	public string soundDown = "";


	public string upIcon = null;
	public string downIcon = null;

	public bool canApplyToNPC = false;
	public bool canApplyToHERO = false;

	public bool canApplyToMonsterHeroBuff = false;
	public bool canApplyToMonsterHeroDebuff = false;
	public bool canApplyToMonsterHeroHeal = false;
	public bool canApplyToMonsterHeroAttack = false;


	public string timeText = "";

	public string targetText = "";

	public string effectText = "";

	public string upText = "";
	public string downText = "";


	public SkillEffectSetupData ()
	{
	}
	
	public void setData (List<object> l, Dictionary<string, int> k)
	{
		name = l[k["NAME"]].ToString();

		Util.parseObject(l[k["ID"]], out id, true, 0);
		Util.parseObject(l[k["LEN"]], out len, true, 0);

		useLevelCorrection = (l[k["LV_FIX"]].ToString().Trim().Length > 0);
		isDurationType = (l[k["DURATION"]].ToString().Trim().Length > 0);

		if(l[k["EFF_UP"]] is string) effUp = (string)l[k["EFF_UP"]];
		if(l[k["EFF_DOWN"]] is string) effDown = (string)l[k["EFF_DOWN"]];	

		if(l[k["EFF_UPLOOP"]] is string) effUpLoop = (string)l[k["EFF_UPLOOP"]];	
		if(l[k["EFF_DOWNLOOP"]] is string) effDownLoop = (string)l[k["EFF_DOWNLOOP"]];	

		if(l[k["ICON_UP"]] is string) upIcon = (string)l[k["ICON_UP"]];	
		if(l[k["ICON_DOWN"]] is string) downIcon = (string)l[k["ICON_DOWN"]];	

		if(l[k["SOUND_UP"]] is string) soundUp = (string)l[k["SOUND_UP"]];	
		if(l[k["SOUND_DOWN"]] is string) soundDown = (string)l[k["SOUND_DOWN"]];	

		canApplyToNPC = (l[k["NPC"]].ToString() == "O");
		canApplyToHERO = (l[k["HERO"]].ToString() == "O");

		canApplyToMonsterHeroBuff = (l[k["M_HERO_BUFF"]].ToString() == "O");
		canApplyToMonsterHeroDebuff = (l[k["M_HERO_DEBUFF"]].ToString() == "O");
		canApplyToMonsterHeroHeal = (l[k["M_HERO_HEAL"]].ToString() == "O");
		canApplyToMonsterHeroAttack = (l[k["M_HERO_ATTACK"]].ToString() == "O");


		timeText = l[k["TIME_TEXT"]].ToString();
		
		targetText = l[k["TARGET_TEXT"]].ToString();
		
		effectText = l[k["EFFECT_TEXT"]].ToString();

		upText = l[k["UPTEXT"]].ToString();
		downText = l[k["DOWNTEXT"]].ToString();

	}


	public string getTimeText(string replaceStr)
	{
		return Util.stringSubstitute(timeText, replaceStr) + " ";
	}



	public string getTargetText(string replaceStr)
	{
		return Util.stringSubstitute(targetText, replaceStr);
	}



	public string getEffectText(string replaceStr, string replaceStr2 = "")
	{
		return Util.stringSubstitute(effectText, replaceStr, replaceStr2);
	}





	public void playSound(bool isUp)
	{
		if(isUp)
		{
			SoundData.play(soundUp);
		}
		else
		{
			SoundData.play(soundDown);
		}
	}

	
}

