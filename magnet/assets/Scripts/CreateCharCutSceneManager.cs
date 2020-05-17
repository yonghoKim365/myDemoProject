using UnityEngine;
using System.Collections;
using WellFired;

public class CreateCharCutSceneManager : MonoBehaviour {


	public USSequencer[] charCutScenes;
	public USSequencer[] charCutScenesSub;

	private GameObject pcLight;
	private GameObject uiShadowLight;
	
	// Use this for initialization
	void Start () {
		for (int i=0; i<charCutScenes.Length; i++) {
			charCutScenes[i].gameObject.SetActive(false);
		}
		for (int i=0; i<charCutScenesSub.Length; i++) {
			charCutScenesSub[i].gameObject.SetActive(false);
		}

		uiShadowLight = GameObject.Find ("UI_ShadowLight");
		pcLight = GameObject.Find ("pc_light");

	}

	public void setActive(bool b){
		for (int i=0; i<charCutScenes.Length; i++) {
			charCutScenes[i].gameObject.SetActive(b);
		}
		for (int i=0; i<charCutScenesSub.Length; i++) {
			charCutScenesSub[i].gameObject.SetActive(b);
		}

		if (!b) {
			setUILight(true);
		}
	}

	public void setUILight(bool b){
		pcLight.SetActive (b);
		
		if (uiShadowLight == null){
			uiShadowLight = GameObject.Find ("UI_ShadowLight");
		}
		
		if (uiShadowLight!=null){
			uiShadowLight.SetActive (b);
		}
	}
	public bool isSeqPlaying(){
		for (int i=0; i<charCutScenes.Length; i++) {
			if (charCutScenes[i].IsPlaying)return true;
		}
		return false;
	}
	public void seqPlay(int i, System.Action callback){

		stopAll ();

		charCutScenes [i].gameObject.SetActive (true);
		charCutScenesSub [i].gameObject.SetActive (true);

		int a = 0;
		charCutScenes [i].PlaybackFinished = (sequence) =>
		{
			callback();
		};

		charCutScenes [i].Play ();

		pcLight.SetActive (false);
		uiShadowLight.SetActive (false);
	}

	public void stopAll(){
		for (int i=0; i<charCutScenes.Length; i++) {
			if (charCutScenes[i].gameObject.activeSelf){
				charCutScenes[i].Stop();
			}
		}
		for (int i=0; i<charCutScenesSub.Length; i++) {
			if (charCutScenesSub[i].gameObject.activeSelf){
				charCutScenesSub[i].Stop();
			}
		}

	}

	public void intoSequenceCallback(){
		int a = 0;

		UIBasePanel heroPanel = UIMgr.GetUIBasePanel("UIPanel/SelectHeroPanel");
		if (heroPanel != null)
			(heroPanel as SelectHeroPanel).OnClickJobSlotStep2 ();
	}
			

}
