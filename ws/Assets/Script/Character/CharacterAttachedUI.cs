using UnityEngine;
using System.Collections;

public class CharacterAttachedUI : MonoBehaviour
{
	public Transform pointer;
	public Transform tf = null;

	public float yScreenLimit = 99999.0f;

	// 실제게임에서는 perspective 카메라를
	// preview일때는 preview 카메라를 쓴다.
	public static Camera gameViewCamera;

	void Awake()
	{
		_visible = false;
		if(tf == null) tf = transform;
	}
	
	public float xPos = 100.0f;
	public float yPos = 100.0f;
	
	public virtual void init(Transform pointer, float posX = 0.0f, float posY = 100.0f, bool isVisible = false)
	{
		this.pointer = pointer;
		visible = isVisible;
		
		xPos = posX;
		yPos = posY;
	}
	
	protected bool _visible = false;
	
	public virtual bool visible
	{
		get
		{
			return _visible;
		}
		set
		{
			_visible = value;

			if(value == true)
			{
				Update();
			}

			gameObject.SetActive(value);
		}
	}
	
	protected Vector2 _v2 = Vector2.one;
	
	
	protected bool _isEnabled = false;
	
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;	
			gameObject.SetActive(value);
			if(value == false)
			{
				_visible = false;
				pointer = null;
			}
		}
		get
		{
			return _isEnabled;	
		}
	}
	
	protected Vector3 _v;
	
	// Update is called once per frame
	void Update () 
	{
		if(pointer == null) return;	
		_v = pointer.position;
		_v.x += xPos;
		_v.y += yPos;
		_v = gameViewCamera.WorldToScreenPoint(_v);

		if(_v.y > yScreenLimit) _v.y = yScreenLimit;

		tf.position = GameManager.me.inGameGUICamera.ScreenToWorldPoint(_v);
	}
	
	public void setPosition()
	{
		if(pointer == null) return;	
		_v = pointer.position;
		_v.x += xPos;
		_v.y += yPos;		
		_v = gameViewCamera.WorldToScreenPoint(_v);

		// 521.
		if(_v.y > yScreenLimit) _v.y = yScreenLimit;

		tf.position = GameManager.me.inGameGUICamera.ScreenToWorldPoint(_v);		
	}

}
