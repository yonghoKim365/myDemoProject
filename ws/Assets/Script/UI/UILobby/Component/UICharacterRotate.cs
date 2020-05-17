using UnityEngine;
using System.Collections;
using System.Text;

public class UICharacterRotate : MonoBehaviour 
{
	public Transform container;

	private float _draggingDistance = 0.0f;
	private bool _isTouchDragging = false;
	
	private float _dragginDistance2 = 0.0f;
	private bool _isTouchDragging2 = false;
	private bool _checkTouchStart = false;	
	
	private float touchDistance;	
	private Vector2 prevMousePosition = Vector2.zero;	
	
	private bool _isMouseDown = false;
	private bool _isMouseDownStart = false;

	public bool canRotate = true;

	public const int STATE_NORMAL = 0;
	public const int STATE_PRESS = 1;
	
	public int state = 0;

	void Update()
	{
		if(state == STATE_PRESS)
		{
			checkDrag();
		}
	}


	private float _rotateSpeed = 30.0f;

	void checkDrag () {
		
		Vector2 mousePos = Vector2.zero;
		
		bool isMouseUp = false;
		bool isDrag = false;
		
		#if UNITY_EDITOR
		
		if(_isMouseDown == false) _isMouseDown = Input.GetMouseButtonDown(0);
		isMouseUp = Input.GetMouseButtonUp(0);
		if(isMouseUp) _isMouseDown = false;
		mousePos = Input.mousePosition;
		#elif UNITY_ANDROID || UNITY_IPHONE
		if(Input.touchCount >= 1)
		{
			Touch t = Input.GetTouch(0);
			_isMouseDown = (t.phase == TouchPhase.Began);
			isMouseUp = (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled);
			mousePos = t.position;
			isDrag = (t.phase == TouchPhase.Moved);
		}
		#endif		
		
		// 터치가 시작한 순간.
		if(Input.GetMouseButtonDown(0))
		{
#if UNITY_EDITOR
			Debug.Log("터치가 시작.");
#endif
			prevMousePosition = mousePos;
			_draggingDistance = 0.0f;
			_isTouchDragging = false;
			_isMouseDown = true;
			_isMouseDownStart = true;
		}
		
		// 화면 이동. 마우스가 눌린 상태.
		if(Input.GetMouseButton(0) && _isMouseDownStart)
		{
			if(canRotate) container.Rotate(0,((prevMousePosition.x)-Input.mousePosition.x)*_rotateSpeed*Time.smoothDeltaTime, 0);
			
			float deltaX = prevMousePosition.x - mousePos.x;
			float deltaY = prevMousePosition.y - mousePos.y;
			_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
			if(_draggingDistance > 20.0f) _isTouchDragging = true;
			
			prevMousePosition = mousePos;
		}
	}	
}