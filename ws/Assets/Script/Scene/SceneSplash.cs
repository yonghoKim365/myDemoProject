using UnityEngine;
using System.Collections;

public class SceneSplash : MonoBehaviour {

	// Use this for initialization
	void Start () {

#if UNITY_ANDROID

		string model = SystemInfo.deviceModel;
		if(string.IsNullOrEmpty(model) == false)
		{
			model = model.ToLower();

			if(model.Contains("genymotion") || model.Contains("bluestacks"))
			{
				return;
			}
		}
#endif

		StartCoroutine(loadScene());
	}
			
	private IEnumerator loadScene()
	{
		yield return new WaitForSeconds(0.5f);
		Application.LoadLevelAdditiveAsync(1);//LoadLevel(1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
