using UnityEngine;
using System.Collections;

public class AppearBossPanel : UIBasePanel {

    public UILabel NameLabel;
    public TweenAlpha Alpha;
    public Animation Anim;
    
    //public float Duration = 0.5f;
    //public float Delay = 0f;
    
    private float RunTime;
    //private bool IsAlphaAction;
    
    public override void Init()
    {
        //Panel = GetComponent<UIPanel>();
        //Panel.alpha = 0;
        base.Init();

        Mob.MobInfo mLowData = null;
        int loopCount = G_GameInfo.CharacterMgr.allUnitList.Count;
        uint lowDataID = 0;
        for(int i=0; i < loopCount; i++)
        {
            Unit u = G_GameInfo.CharacterMgr.allUnitList[i];
            if (u.IsDie || u.UnitType != UnitType.Boss)
                continue;

            Npc n = u as Npc;
            lowDataID = n.NpcLowID;
            mLowData = _LowDataMgr.instance.GetMonsterInfo(lowDataID);
            break;
        }

        string name = null;
        string icon = null;

        if (mLowData == null)
        {
            name = "NotFound ID " + lowDataID;
            icon = "missing_icon";
        }
        else
        {
            name = _LowDataMgr.instance.GetStringUnit(mLowData.NameId);
            icon = mLowData.PortraitId;
        }
		NameLabel.text = name;
		G_GameInfo.GameInfo.HudPanel.SetBossInfo(name, icon);
        
		DoNameAniAlphaSound ();
    }

	public void DoNameAniAlphaSound(){

		//IsAlphaAction = true;
		SoundManager.instance.PlayBgmSound("BGM_Boss");

		GetComponent<UIPanel> ().alpha = 1.0f;

		float delay = 0.5f;

		if (GameObject.Find ("CutSceneCameraMover") != null){
			CutSceneSeqHelper csh = GameObject.Find ("CutSceneCameraMover").GetComponent<CutSceneSeqHelper> ();
			if (csh != null){
				delay = csh.name_maintain_time;
			}
		}

		Anim.Play("BossNameAnimation");
		float length = Anim.GetClip("BossNameAnimation").length;
		TempCoroutine.instance.FrameDelay(length+delay, delegate() {
			if (Alpha != null){
				Alpha.ResetToBeginning();
				Alpha.PlayForward();
			}
		} );
		SoundManager.instance.PlaySfxSound(eUISfx.UI_boss_revial, false);
	}


    /*
    void Update()
    {
        if (!IsAlphaAction)
            return;

        RunTime += Time.unscaledDeltaTime;
        if (RunTime < Delay)
            return;

        float rate = (RunTime-Delay)/Duration;
        rate = Mathf.Clamp01(rate);
        
        float value = Mathf.Lerp(0, 1, rate);

        Panel.alpha = value;

        if (rate == 1)
        {
            IsAlphaAction = false;
            Panel.alpha = 1;
        }
    }
    */
}
