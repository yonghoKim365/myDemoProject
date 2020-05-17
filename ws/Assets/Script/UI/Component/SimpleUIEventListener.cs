using UnityEngine;
using System.Collections;

public class SimpleUIEventListener : MonoBehaviour 
{
	public Camera targetCamera;

	public enum TouchEventType
	{
		None, MouseDown, MousePress, MouseUp
	}

	public TouchEventType type;

	public BoxCollider checkCollider;

	public delegate void Callback();
	public Callback downCallback = null;
	public Callback upCallback = null;
	public Callback pressCallback = null;


	void Update () 
	{
		bool pressed = Input.GetMouseButtonDown(0);
		bool unpressed = Input.GetMouseButtonUp(0);
		bool held = Input.GetMouseButton(0);

		if(checkCollider == null || type == TouchEventType.None) return;

		switch(type)
		{
		case TouchEventType.MouseDown:
			if(pressed && hitTest() && downCallback != null)
			{
				downCallback();
			}
			break;
		case TouchEventType.MousePress:
			if(held && hitTest() && pressCallback != null) 
			{
				pressCallback();
			}
			break;
		case TouchEventType.MouseUp:
			if(unpressed && hitTest() && upCallback != null) 
			{
				upCallback();
			}
			break;
		}
	}


	private RaycastHit uiCheckHitInfo;
	
	public bool hitTest()
	{
		if(Physics.Raycast(targetCamera.ScreenPointToRay(Input.mousePosition), out uiCheckHitInfo))
		{
			return (uiCheckHitInfo.collider != null && uiCheckHitInfo.collider == checkCollider);
		}

		return false;
	}

}
