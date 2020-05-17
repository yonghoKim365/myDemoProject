using UnityEngine;
using System.Collections;

sealed public class WordEffect : CharacterAttachedUI {

	public const string MISS = "MISS";

	const string effect = "effect";

	public Animation animation;

	private tk2dTextMesh text;

	bool isClone = false;

	private Color _color;

	void Awake()
	{
		_visible = false;
		if(tf == null) tf = transform;
		if(text == null) text = gameObject.transform.Find(effect).GetComponent<tk2dTextMesh>();
	}

	public void destroyAsset()
	{
		text = null;
		animation = null;
	}


	public static void showWordEffect(string str, Color color, Transform pointer, float posX = 0.0f, float posY = 100.0f)
	{

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0.5f) return;
#endif

//		// FUCKYOU
//		return;
		GameManager.me.effectManager.getWordEffect().start(str, color, pointer, posX, posY);
	}

	Vector3 _pos = new Vector3();
	bool _isStart = false;
	public void start(string str, Color color, Transform pointer, float posX = 0.0f, float posY = 100.0f)
	{
		if(text == null) text = gameObject.transform.Find("effect").GetComponent<tk2dTextMesh>();
		
		text.maxChars = str.Length;
		text.text = str;
		text.color = color;
		_color = color;

		text.Commit();
		
		_v = text.GetMeshDimensionsForString(str);

		_pos = pointer.transform.position;

		_pos.x -= _v.x * 0.5f;
		_pos.x += posX;
		
		tf.position = GameManager.me.inGameGUICamera.ScreenToWorldPoint(GameManager.me.gameCamera.WorldToScreenPoint(_pos));;

		init (pointer, posX - _v.x * 0.5f, posY, true);

		_isStart = true;
		animation.Play();

		if(PerformanceManager.isLowPc) return;

		StartCoroutine(onCompleteEffectCT());
	}
	
	public void onCompleteEffect()
	{
		if(_isStart == false) return;
		_isStart = false;
		GameManager.me.effectManager.setWordEffect(this);
	}

	private static WaitForSeconds ws05 = new WaitForSeconds(0.05f);
	private static WaitForSeconds ws8 = new WaitForSeconds(0.8f);

	IEnumerator onCompleteEffectCT()
	{
		yield return ws8;

		while(true)
		{
			yield return ws05;
			if(_color.a > 0.1f)
			{
				_color.a -= 0.1f;
				text.color = _color;
			}
			else
			{
				break;
			}
		}

		yield return ws8;

		onCompleteEffect();
	}





	
}
