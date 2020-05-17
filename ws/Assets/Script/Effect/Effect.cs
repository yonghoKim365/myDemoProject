using UnityEngine;
using System;

sealed public class Effect : MonoBehaviour
{
	public Effect ()
	{
	}

	public const string E_HIT_PARTICLE01 = "E_HIT_PARTICLE01";


	public ParticleEffect particleEffect;
	public GameObject effect;
	
	public Transform tf;
	
	private Quaternion _q;
	
	public Vector3 rotation
	{
		set
		{
			_q = tf.rotation;
			_q.eulerAngles = value;
			tf.rotation = _q;
			
			if(particleEffect != null)
			{
				particleEffect.tf.rotation = tf.rotation;
			}
			
		}
	}	


	public void resize(float size)
	{
//		_v = tf.transform.localScale;
//		_v.x = size;
//		_v.y = size;
//		_v.z = size;
//		tf.transform.localScale = _v;

		if(particleEffect != null)
		{
			particleEffect.resize(size);
		}

	}


	
	public void setData(GameObject go, ParticleEffect pe = null)
	{
		particleEffect = pe;
		effect = go;
	}
	
	private bool _enabled = false;
	
	public bool isEnabled
	{
		set
		{
			_enabled = value;
			gameObject.SetActive(value);
			
			if(value == false)
			{
				if(particleEffect != null)
				{
					GameManager.me.effectManager.setParticleEffect(particleEffect);
					particleEffect = null;
				}
				
				effect = null;
			}
		}
		get
		{
			return _enabled;
		}
	}
	
	
	
	
	
	public bool isCutScenePositionTween = false;
	public bool isCutSceneRotationTween = false;
	
	public Vector3 csStartPos = new Vector3();
	public Vector3 csTargetPos = new Vector3();
	public float csTargetDistance = 1.0f;
	
	public float csTweenSpeed = 100.0f;
	public float csTweenTime = 0.0f;
	private float _csProgressDeltaTime = 0.0f;
	private Quaternion _csStartRotation;
	private Quaternion _csTargetRotation;
	private Vector3 _v;
	
	public void cutSceneInit()
	{
		isCutScenePositionTween = false;
		isCutSceneRotationTween = false;
		csTweenSpeed = 0.0f;
		csTweenTime = 0.0f;
		_csProgressDeltaTime = 0.0f;
		csStartPos.x = 0;csStartPos.y = 0;csStartPos.z = 0;
		csTargetDistance = 0.0f;
	}
	
	public void setCSRotation(float motionTime, Vector3 rotationValue)
	{
		csTweenTime = motionTime;
		_csProgressDeltaTime = 0.0f;
		isCutSceneRotationTween = true;
		_csStartRotation = tf.rotation;
		_csTargetRotation = tf.rotation;
		_csTargetRotation.eulerAngles = rotationValue;
	}
	
	public void cutSceneUpdate()
	{
		if(isCutScenePositionTween)
		{
			_v = tf.position;
			
			tf.position = Vector3.MoveTowards(tf.position, csTargetPos, csTweenSpeed * CutSceneManager.cutSceneDeltaTime);
			
			//_ += tf.forward * csTweenSpeed * CutSceneManager.cutSceneDeltaTime;
			
			if(VectorUtil.Distance3D(csStartPos, tf.position) >= VectorUtil.Distance3D(csStartPos, csTargetPos))//csTargetDistance)
			{
				isCutScenePositionTween = false;
			}
		}
		else if(isCutSceneRotationTween)
		{
			_csProgressDeltaTime += CutSceneManager.cutSceneDeltaTime;
			
			if(_csProgressDeltaTime >= csTweenTime)
			{
				tf.rotation = _csTargetRotation;
				isCutSceneRotationTween = false;
			}
			else
			{
				tf.rotation = Quaternion.Slerp(_csStartRotation, _csTargetRotation, _csProgressDeltaTime/csTweenTime);	
			}
			
			if(particleEffect!=null)particleEffect.tf.rotation = tf.rotation;
		}
	}


	public void destroyAsset()
	{
		particleEffect = null;
		effect = null;
		tf = null;
	}

}

