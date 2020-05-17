using UnityEngine;
using System.Collections;

public class CharacterShadow : MonoBehaviour {

	public tk2dSprite shadow;
	//private Transform _parent;

	Vector3 _v;

	public Transform tf;

	void Awake()
	{
	}

	public void setData(Transform parent, float size)
	{
		_v.x = 1; _v.y = 1; _v.z = 1;
		tf.localScale = _v;
		_v.x = size; _v.y = size; _v.z = size;
		shadow.scale = _v;

		tf.parent = parent;

		_v.x = 0.0f;
		_v.y = 1.0f;
		_v.z = 0.0f;
		tf.localPosition = _v;

		shadow.renderer.enabled = true;

		gameObject.SetActive(true);
	}



	public void setData(Transform parent, float sx, float sy)
	{
		_v.x = 1; _v.y = 1; _v.z = 1;
		tf.localScale = _v;
		_v.x = sx; _v.y = sy;
		shadow.scale = _v;
		
		tf.parent = parent;
		
		_v.x = 0.0f;
		_v.y = 1.0f;
		_v.z = 0.0f;
		tf.localPosition = _v;
		
		shadow.renderer.enabled = true;
		
		gameObject.SetActive(true);
	}




	void LateUpdate()
	{
		//if(CutSceneManager.nowOpenCutScene)
		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_MENU)
		{
			_v = tf.localPosition;
			_v.y = 1.0f;
			tf.localPosition = _v;
		}
		else
		{
			_v = tf.position;
			_v.y = 1.0f;
			tf.position = _v;
		}
	}

	public void destroy()
	{
		shadow = null;
		tf = null;
	}

	void OnDestroy()
	{
		destroy();
	}

}
