using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WellFired;

public class CinemaSceneManager : MonoBehaviour {


	public static CinemaSceneManager instance;

	public USSequencer[] seqs;

	private Unit bossUnit;

	private int eventCnt;

	public Dictionary<int, Camera> cameraDic = new Dictionary<int, Camera> ();

	DialogPanel dialogPanel;

	private SingleGameInfo Stageinfo;

	public enum StartType {
		NOTHING = 0,
		BEFORE_GAME_START = 1,
		DURING_BATTLE = 2,
		AFTER_CLEAR = 3
	};
	
	public void setBossUnit(GameObject go){
		bossUnit = go.GetComponent<Unit> ();
	}




	void Update(){
		if (bossUnit != null) {
		
			if (bossUnit.CharInfo.Hp < bossUnit.CharInfo.MaxHp / 2) {

				QuestInfo questInfo = QuestManager.instance.GetLastestUnCompleteQuestInfo();

				if (questInfo != null && getCinemaSceneStartType(questInfo.unTaskId) == CinemaSceneManager.StartType.DURING_BATTLE){
					if (eventCnt == 0){
						eventCnt = 1;
						int startSeqIdx = getCinemaSceneStartingSeqIndex(questInfo.unTaskId);
						int endSeqIdx = getCinemaSceneEndingSeqIndex(questInfo.unTaskId);
						CutSceneMgr.StartCinemaScene(true, startSeqIdx,endSeqIdx, null);
					}
				}
#if UNITY_EDITOR
				else{

					if (SceneManager.instance.testData.bCutSceneTest){
						SingleGameInfo stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;

						CinemaSceneManager csm = GameObject.Find ("CinemaSceneManager").GetComponent<CinemaSceneManager> ();
						if (csm.getCinemaSceneStartTypeByStageID(stageinfo.StageId) == CinemaSceneManager.StartType.DURING_BATTLE){
							if (eventCnt == 0){
								eventCnt = 1;
								int startSeqIdx = getCinemaSceneStartingSeqIndex(questInfo.unTaskId);
								int endSeqIdx = getCinemaSceneEndingSeqIndex(questInfo.unTaskId);
								CutSceneMgr.StartCinemaScene(true, startSeqIdx,endSeqIdx, null);
							}
						}
					}

				}
#endif
			}
		}
	}

	public int getCinemaSceneStartingSeqIndex(uint questId){
		int seqIdx = 0;

#if UNITY_EDITOR
//		if (questId == 2) { /// 1-1
//			//return StartType.BEFORE_GAME_START;
//			seqIdx = 0;
//		}
//		if (questId == 6) { /// 1-2
//			//return StartType.AFTER_CLEAR;
//			seqIdx = 0;
//		}
#endif

		// 시작하는 시퀀스의 배열 인덱스.
		// 3-10은 3개의 시퀀스로 스테이지 시작시 0,1 클리어시 2를 플레이한다. 
		if (questId == 24) { /// 1-7
			seqIdx = 0;
		}
		if (questId == 98) {	// 3-10
			//return StartType.BEFORE_GAME_START;
			seqIdx = 0;
		}
		else if (questId == 99) {	// 3-10
			// return StartType.AFTER_CLEAR;
			seqIdx = 2;
		}
		else if (questId == 114) {	// 4-4
			seqIdx = 0;
		}
		else if (questId == 133) {	// 4-10
			// return StartType.AFTER_CLEAR;
			seqIdx = 0;
		}

		return seqIdx;
	}


	// end콜백을 받을 시퀀스의 인덱스.
	// 1개 이상의 시퀀스가 포함된 씬이 있어 시작인덱스와 종료인덱스가 다를수있어 종료인덱스도 지정해준다.
	public int getCinemaSceneEndingSeqIndex(uint questId){
		int seqIdx = seqs.Length - 1;

#if UNITY_EDITOR
//		if (questId == 2) { /// 1-1
//			//return StartType.BEFORE_GAME_START;
//			seqIdx = 0;
//		}
//		if (questId == 3) { /// 1-1
//			seqIdx = 0;
//		}
//		if (questId == 6) { /// 1-2
//			//return StartType.BEFORE_GAME_START;
//			seqIdx = 0;
//		}
#endif

		if (questId == 24) { /// 1-7
			seqIdx = 0;
		}
		if (questId == 98) {	// 3-10
			//return StartType.BEFORE_GAME_START;
			seqIdx = 1;
		}
		else if (questId == 99) {	// 3-10
			// return StartType.AFTER_CLEAR;
			seqIdx = 2;
		}
		else if (questId == 114) {	// 4-4
			seqIdx = 0;
		}
		else if (questId == 133) {	// 4-10
			// return StartType.AFTER_CLEAR;
			seqIdx = 0;
		}
		else if (questId == 164) {	// 5-10
			seqIdx = 1;
		}
		
		return seqIdx;
	}

	public StartType getCinemaSceneStartType(uint questId){

#if UNITY_EDITOR
//		if (questId == 2) { /// 1-1
//			return StartType.BEFORE_GAME_START;
//		}
//		if (questId == 6) { /// 1-2
//			return StartType.BEFORE_GAME_START;
//		}
#endif
//		if (questId == 13) { /// 104
//			return StartType.DURING_BATTLE;
//		}
//		if (questId == 17) { /// 104
//			return StartType.DURING_BATTLE;
//		}
//		// for test
//		if (questId == 3) { /// 1-1, for test
//			return StartType.AFTER_CLEAR;
//		}
//		if (questId == 7) { /// 1-2, for test
//			return StartType.AFTER_CLEAR;
//		}
		if (questId == 24) { /// 1-7
			return StartType.BEFORE_GAME_START;
		}
		else if (questId == 35) {	// 1-10
			return StartType.DURING_BATTLE;
		}
		else if (questId == 98) {	// 3-10
			return StartType.BEFORE_GAME_START;
		}
		else if (questId == 99) {	// 3-10
			return StartType.AFTER_CLEAR;
		}
		else if (questId == 114) {	// 4-4
			return StartType.AFTER_CLEAR;
		}
		else if (questId == 133) {	// 4-10
			return StartType.AFTER_CLEAR;
		}
		else if (questId == 164) {	// 5-10
			return StartType.AFTER_CLEAR;
		}

		return StartType.NOTHING;
	}

#if UNITY_EDITOR
	public StartType getCinemaSceneStartTypeByStageID(uint stageId){
		//return StartType.NOTHING;

		if (stageId == 107){ //24) { /// 1-7
			return StartType.BEFORE_GAME_START;
		}
		else if (stageId == 110){ //34) {	// 1-10
			return StartType.DURING_BATTLE;
		}
		else if (stageId == 310){ //97) {	// 3-10
			return StartType.BEFORE_GAME_START;
		}
		else if (stageId == 98) {	// 3-10
			return StartType.AFTER_CLEAR;
		}
		else if (stageId == 404){//113) {	// 4-4
			return StartType.AFTER_CLEAR;
		}
		else if (stageId == 410){ //132) {	// 4-10
			return StartType.AFTER_CLEAR;
		}
		else if (stageId == 510){ //163) {	// 5-10
			return StartType.AFTER_CLEAR;
		}
		
		return StartType.NOTHING;  
	}
#endif



	void Start(){
		if (instance == null) {
			instance = this;
			eventCnt = 0;

			Stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;
			setActiveSequences(false);
		}
	}

	public void setActiveSequences(bool b){
		for (int i=0; i<seqs.Length; i++) {
			seqs[i].gameObject.SetActive(b);
		}
	}

	public void storeAliveCamera(){
		int cnt = 0;
		foreach (Camera c in Camera.allCameras) {
			if (c.enabled){
 				cameraDic.Add(cnt, c);
				cnt++;
			}
		}
	}

	public void restoreUnactivedCamera(){
		for(int i=0;i<cameraDic.Count;i++){
			cameraDic[i].enabled = true;
		}
		cameraDic.Clear ();
	}

	public void setDialog(int textIdx){
		if (dialogPanel == null) {
			dialogPanel = UIMgr.Open ("UIPanel/DialogPanel").GetComponent<DialogPanel> ();
		}

		if (dialogPanel.gameObject.activeInHierarchy == false) {
			dialogPanel.gameObject.SetActive(true);
		}

		dialogPanel.setDialog (textIdx);
	}

	public void closeDialogPanel(){
		if (dialogPanel == null)
			return;

		dialogPanel.Close ();
	}

	public void setDialogVisible(bool b){
		if (dialogPanel == null)
			return;

		dialogPanel.gameObject.SetActive (b);
	}

	public void InActiveOtherCharsExceptMe(uint myCharIdx){
		string[] CharObjNamesForInactive = new string[2];
		
		if (myCharIdx == 11000) {
			CharObjNamesForInactive[0] = "pc_p_cos_skin_01";
			CharObjNamesForInactive[1] = "pc_d_cos_skin_01";
		}
		else if (myCharIdx == 12000){
			CharObjNamesForInactive[0] = "pc_f_cos_skin_01";
			CharObjNamesForInactive[1] = "pc_d_cos_skin_01";
		}
		else if (myCharIdx == 13000){
			CharObjNamesForInactive[0] = "pc_f_cos_skin_01";
			CharObjNamesForInactive[1] = "pc_p_cos_skin_01";
		}

		for (int i=0; i<CharObjNamesForInactive.Length; i++) {
			for (int j = 0; j < seqs.Length; j++) {
				if (seqs[j].transform.FindChild(CharObjNamesForInactive[i]) != null){
					// 오브젝트를 꺼도 sequence안에서 다시 살리니까 소용이 없다.
					//seqObject1.transform.FindChild(objNames[i]).gameObject.SetActive(false);
				
					SkinnedMeshRenderer[] meshs = seqs[j].transform.FindChild(CharObjNamesForInactive[i]).GetComponentsInChildren<SkinnedMeshRenderer>(true);
					foreach (SkinnedMeshRenderer r in meshs){
						r.enabled = false;
					}
				}
			}
		}
	}


}
