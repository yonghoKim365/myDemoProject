using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using Sw;

public class CutSceneState : SceneStateBase
{

    public override void OnEnter(System.Action callback)
    {

        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () => 
        {
            base.OnEnter(callback);

            LoadLevelAsync("maintown_cutscene");

        });

        Time.timeScale = 1;
    }


    public override void OnExit(System.Action callback)
    {
        //캐릭터 정보도 다삭제

        UIHelper.SetMainCameraActive(true);
        base.OnExit( callback );

    }

    void OnLevelWasLoaded(int level)
    {
		if (Application.loadedLevelName != "maintown_cutscene")
            return;
	
		ResourceMgr.Clear ();
		
        NativeHelper.instance.DisableNavUI();
		
        //< 맵 로드
        IsMapLoad = true;

        SceneSetting();

        SetupMainCamera(true, GAME_MODE.NONE);

		MapEnvironmentSetting(Application.loadedLevelName); // move to here.

		if (!SceneManager.instance.IsYieldAction)
			SceneManager.instance.ShowLoadingTipPanel(false);

		StartCoroutine (LoadAndPlayCutScene ());

	}


	IEnumerator LoadAndPlayCutScene(){
		
		SceneManager.instance.ShowLoadingTipPanel(false);
		
		string[] CharObjNamesForInactive = new string[2];
		
		if (NetData.instance.GetUserInfo ()._userCharIndex == 11000) {
			CharObjNamesForInactive[0] = "pc_p_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 12000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 13000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_p_cutscene_01";
		}
	
		if (GameObject.Find ("TownCutSceneManager")) {
			TownCutSceneManager tcm = GameObject.Find ("TownCutSceneManager").GetComponent<TownCutSceneManager> (); 
			tcm.InActiveObj(CharObjNamesForInactive);
			tcm.playSeq( ()=>{}, ()=>{
				SceneManager.instance.ActionEvent(_ACTION.GO_TOWN);
			});
		}

		yield return null;
	}
}


