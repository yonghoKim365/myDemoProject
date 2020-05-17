using UnityEngine;
using System.Collections;

public class UIWorldMapOpenStarEffect : MonoBehaviour {

	bool _isStart = false;
	bool _isEnding = false;
	Vector3 _startPos,_endPos,_initPos;
	Vector3 _dPos = new Vector3();
	float _r;
	float _pow;
	float _targetR;
	float _rFromCenter;
	float _rSpeed; // 회전운동에서 해당 각도로 가려는 속도. 얘가 낮으면 물체는 목표로 가지않고 지 갈길을 간다...
	float _timeEnd;
	
	Transform tf;
	
	public float startPow = 300.0f;
	public float powOffset = 600.0f;

	public float rValue1 = -180.0f;
	public float rValue2 = 90.0f;
	
	public float rSpeed1 = 200.0f;
	public float rSpeed2 = 400.0f;

	void Awake()
	{
		tf = transform;
	}


	void Update()
	{
		if(_isStart && !_isEnding)
		{
			if(Time.time >= _timeEnd || (Vector3.Distance(_endPos,tf.position)<80.0f))
			{
				_targetR = Mathf.Atan2(_endPos.y-tf.position.y,_endPos.x-tf.position.x) * Mathf.Rad2Deg;
				_dPos.x = Mathf.Cos(_targetR*Mathf.Deg2Rad) * (_pow*Time.deltaTime);
				_dPos.y = Mathf.Sin(_targetR*Mathf.Deg2Rad) * (_pow*Time.deltaTime);
				
				tf.position = Vector3.Lerp(tf.position, tf.position + _dPos, 0.5f);
			}
			else
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
			}

			Vector2 v1 =  _endPos;
			v1.x = _endPos.x;
			v1.y = _endPos.y;

			Vector2 v2 = tf.position;
			v2.x = tf.position.x;
			v2.y = tf.position.y;

			if((VectorUtil.Distance( v1, v2 ) <10.0f) )
			{
				StopEffect();
			}
		}
	}


	Vector3 _scale = new Vector3();


	public void StartEffect(Vector3 startPos_p, Vector3 endPos)
	{
		if(tf == null) tf = transform;
		
		_isStart = true;
		_isEnding = false;
		
		_startPos = startPos_p;
		_startPos.z -= 200.0f;
		tf.position = _startPos;
		
		_endPos = endPos;
		_endPos.z -= 200.0f;

		_r = Util.getFloatAngleBetweenXY(_startPos, _endPos) - (Random.Range(40,80) * ( (Random.Range(0,100)< 50)?-1:1 )) ;
		_pow = startPow;
		
		_rSpeed = Random.Range(rSpeed1,rSpeed2);
		_timeEnd = Time.time+2f;

//		Debug.LogError("_r : " + _r);

		gameObject.SetActive(true);
	}


	void StopEffect()
	{
		_isStart = false;
		_isEnding = true;
		StartCoroutine(StopProcess());
		tf.position = _endPos;
	}
	
	IEnumerator StopProcess()
	{
		yield return Util.ws05;
		_isStart=false;
		Destroy(this.gameObject);
	}














































}
