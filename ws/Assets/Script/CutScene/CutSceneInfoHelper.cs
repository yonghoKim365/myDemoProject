using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CutSceneInfoHelper : MonoBehaviour {


	void Awake()
	{
	}

#if UNITY_EDITOR	
	// Update is called once per frame
	void Update () 
	{
		if(GameManager.me != null) copyCsInfoToClipboard();
	}
#endif


	public void copyCsInfoToClipboard()
	{
		#if UNITY_EDITOR
		
		if(Input.GetKey(KeyCode.Alpha0) || Input.GetMouseButtonUp(2))
		{
			string clipboard = "";
			
			Vector3 _v = GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition;
			clipboard =  Util.getPointNumber(_v.x,2) + "," + Util.getPointNumber(_v.y,2)  + "," + Util.getPointNumber(_v.z,2) ;
			clipboard += "	";
			clipboard += "	";
			clipboard += "	";
			float fov = GameManager.me.uiManager.uiPlay.gameCamera.fieldOfView;
			clipboard += fov.ToString();
			clipboard += "	";
			clipboard += "0";
			clipboard += "	";
			_v = GameManager.me.uiManager.uiPlay.gameCamera.transform.localRotation.eulerAngles;
			clipboard +=  Util.getPointNumber(_v.x,2) + "," + Util.getPointNumber(_v.y,2)  + "," + Util.getPointNumber(_v.z,2) ;
			
			ClipboardHelper.clipBoard = clipboard;
		}
		#endif
	}

}
