using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WellFired;


public class TownCutSceneManager : MonoBehaviour {
	
	public USSequencer seqObject1;
	public USSequencer seqObject2;
	public Dictionary<int, Camera> cameraDic; // = new Dictionary<int, Camera> ();
	DialogPanel dialogPanel;

	public System.Action callback;
	public void playSeq( System.Action callback1,System.Action callback2){

		storeAliveCamera();

		seqObject1.gameObject.SetActive (true);
		seqObject2.gameObject.SetActive (true);

		//UIMgr.setTownPanelVisible (false);
		//UIMgr.setMapPanelVisible (false);
		//UIMgr.setChatPopupVisible(false);

		seqObject1.PlaybackFinished = (sequence) =>
		{
			//callback1();
		};

		seqObject2.PlaybackFinished = (sequence) =>
		{
			callback2();
			/*
			SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE);
			StartCoroutine(FakeIncreaseLoadingBar());

			closeDialogPanel();
			callback1();
			callback2(); // == OnLevelWasLoadedPart2
			restoreUnactivedCamera();
			destroySeqObjects();
			Destroy(this.gameObject);

			UIMgr.setTownPanelVisible (true);
			UIMgr.setMapPanelVisible (true);
			UIMgr.setChatPopupVisible(true);
			*/
		};
		seqObject1.Play ();

//		UIMgr.instance.UICamera.enabled = false;
//
//		seqObject1.PlaybackFinished = (sequence) =>
//		{
//			destroySeqObjects();
//			// revive cameras
//
//			// destroy
//			Destroy(this.gameObject);
//
//			UIMgr.instance.UICamera.enabled = true; //GameObject.Find ("UICamera").GetComponent<Camera> ().enabled = true;
//		};

//		seqObject1.Play ();
	}

	IEnumerator FakeIncreaseLoadingBar(){

		float v = 0;
		while (v < 1f) {
			SceneManager.instance.LoadingTipPanel.changeLoadingBar (v);
			v += 0.05f * Time.deltaTime;
			yield return null;
		}
	}

	public void InActiveObj(string[] objNames){

		for (int i=0; i<objNames.Length; i++) {
			if (seqObject1.transform.FindChild(objNames[i]) != null){
				// 오브젝트를 꺼도 sequence안에서 다시 살리니까 소용이 없다.
				//seqObject1.transform.FindChild(objNames[i]).gameObject.SetActive(false);

				Transform t = seqObject1.transform.FindChild(objNames[i]);
				SkinnedMeshRenderer[] meshs = seqObject1.transform.FindChild(objNames[i]).GetComponentsInChildren<SkinnedMeshRenderer>(true);
				foreach (SkinnedMeshRenderer r in meshs){
					r.enabled = false;
				}
			}
			if (seqObject2.transform.FindChild(objNames[i]) != null){
				SkinnedMeshRenderer[] meshs = seqObject2.transform.FindChild(objNames[i]).GetComponentsInChildren<SkinnedMeshRenderer>(true);
				foreach (SkinnedMeshRenderer r in meshs){
					r.enabled = false;
				}
			}
		}
	}

	void cb(){
		// callback.
	}

	public void destroySeqObjects(){
		Destroy (seqObject1.gameObject);
		Destroy (seqObject2.gameObject);

		seqObject1 = seqObject2 = null;
	}

//	public void OnDestroy()
//	{
//		destroySeqObjects ();
//	}

	public void storeAliveCamera(){
		int cnt = 0;
		cameraDic = new Dictionary<int, Camera> ();
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

}
