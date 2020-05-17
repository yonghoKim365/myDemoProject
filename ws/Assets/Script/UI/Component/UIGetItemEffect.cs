using UnityEngine;
using System.Collections;

public class UIGetItemEffect : MonoBehaviour 
{
	
	public UISprite starBody;
	
	bool _isStart = false;
	bool _isEnding = false;
	Vector3 _startPos,_endPos,_initPos;
	Vector3 _dPos = new Vector3();
	public float _r;
	float _pow;
	float _targetR;
	float _rFromCenter;
	public float _rSpeed; // 회전운동에서 해당 각도로 가려는 속도. 얘가 낮으면 물체는 목표로 가지않고 지 갈길을 간다...
	float _timeEnd;

	public Transform target;

	Transform tf;

	public Vector3 targetPos = new Vector3(-1,0,0);

	public float startPow = 200.0f;
	public float powOffset = 400.0f;

	void Awake()
	{
		tf = transform;
	}

	void Update()
	{
//		if(Input.GetMouseButtonUp(2))
//		{
//			StartEffect(tf.position, target.position);
//		}
		
		if(_isStart && !_isEnding)
		{
			_dPos.x = Mathf.Cos(_r*Mathf.Deg2Rad) * (_pow*Time.deltaTime);
			_dPos.y = Mathf.Sin(_r*Mathf.Deg2Rad) * (_pow*Time.deltaTime);
			
			tf.position += _dPos;

			_pow += powOffset*Time.deltaTime;

			_targetR = Mathf.Atan2(_endPos.y-tf.position.y,_endPos.x-tf.position.x) * Mathf.Rad2Deg;
			while(_targetR>180f) _targetR-=360f;
			while(_targetR<-179f) _targetR+=360f;
			
			_rFromCenter = _r - _targetR;
			while(_rFromCenter>180f) _rFromCenter-=360f;
			while(_rFromCenter<-179f) _rFromCenter+=360f;

			if(_rFromCenter>1f)
			{
				_r -= _rSpeed * Time.deltaTime;
			}
			else if(_rFromCenter<-1f)
			{
				_r += _rSpeed * Time.deltaTime;
			}

			if(Time.time >= _timeEnd || (Vector3.Distance(_endPos,tf.position)<10.0f || (tf.position.y - 20.0f > _endPos.y)) )
			{
				StopEffect();
				//UIMainGame.instance.PlayGetStarUIEffect();
			}
		}
	}
	
	Vector3 _scale = new Vector3();
	
	public void StartEffect(Vector3 startPos_p, string id)
	{
		if(tf == null) tf = transform;
		
		_isStart = true;
		_isEnding = false;
		
		_startPos = startPos_p;
		_startPos.z = tf.position.z;
		tf.position = _startPos;
		
		_endPos = target.position;
		_endPos.y -= 15.0f;
		_endPos.z = tf.position.z;
		
		_r = Random.Range(-180f,90f);
		_pow = startPow;

		_rSpeed = Random.Range(400f,600f);
		_timeEnd = Time.time+2f;

		starBody.spriteName = id;
		starBody.MakePixelPerfect();

		starBody.enabled = true;

		gameObject.SetActive(true);
	}
	
	void StopEffect()
	{
		_isEnding = true;
		StartCoroutine(StopProcess());
		tf.position = _endPos;
		starBody.enabled = false;
	}
	
	IEnumerator StopProcess()
	{
		yield return new WaitForSeconds(0.5f);
		_isStart=false;
		gameObject.SetActive(false);
		GameManager.me.effectManager.setUIGetItemEffect(this);
	}
	
}