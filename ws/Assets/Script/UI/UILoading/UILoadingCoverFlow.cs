using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILoadingCoverFlow : MonoBehaviour 
{

	void Start()
	{
	}

	
	void Drop () 
	{
		Debug.Log("Drop");
		//		Vector3 dist = mDragPosition - mDragStartPosition;
		//		if (dist.x>0f) SetSequence(true);
		//		else SetSequence(false);
		//		SetPosition(true);
	}
	
	void OnDrag (Vector2 delta) 
	{
		Debug.Log("OnDrag");
		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float dist = 0f;
		Vector3 currentPos = ray.GetPoint(dist);
		
		//		if (UICamera.currentTouchID == -1 || UICamera.currentTouchID == 0) {
		//			if (!mIsDragging) {
		//				mIsDragging = true;
		//				mDragPosition = currentPos;
		//			} else {
		//				Vector3 pos = mStartPosition - (mDragStartPosition - currentPos);
		//				Vector3 cpos = new Vector3(pos.x, mTrans.position.y, mTrans.position.z);
		//				mTrans.position = cpos;
		//			}
		//		}
	}
	
	void OnPress (bool isPressed) 
	{
		Debug.Log("OnPress");
		
	}


}

	
