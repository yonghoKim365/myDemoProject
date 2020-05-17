using UnityEngine;
using System.Collections;

public class CustomTweenRotation : UITweener {
	
	public enum _Direction
	{
		DIR_RIGHT,
		DIR_LEFT,
	}
	
	public _Direction Direction = _Direction.DIR_RIGHT;
	
	public Vector3 from;
	public Vector3 to;
	
	public float MinValue = 0;
	float SavedFactor = 0;
	
	Transform mTrans;
	
	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
	public Quaternion rotation { get { return cachedTransform.localRotation; } set { cachedTransform.localRotation = value; } }
	
	override protected void OnUpdate (float factor, bool isFinished)
	{
		float tmpFactor = 0;
		if(factor - SavedFactor >= MinValue)
		{
			tmpFactor = SavedFactor + MinValue;
			SavedFactor = SavedFactor + MinValue;
		}
		else if(SavedFactor > factor)
		{
			tmpFactor = 0;
			SavedFactor = 0;
		}
		else
		{
			tmpFactor = SavedFactor;
		}
		
		switch(Direction)
		{
		case _Direction.DIR_LEFT:
			cachedTransform.localEulerAngles = from * (1f - tmpFactor) + to * tmpFactor;
			break;
		case _Direction.DIR_RIGHT:
			cachedTransform.localEulerAngles = from * tmpFactor + to * (1f - tmpFactor);
			break;
		}
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenRotation Begin (GameObject go, float duration, Quaternion rot)
	{
		TweenRotation comp = UITweener.Begin<TweenRotation>(go, duration);
		comp.from = comp.value.eulerAngles;
		comp.to = rot.eulerAngles;
		return comp;
	}
}
