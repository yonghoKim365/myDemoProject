using UnityEngine;
using System.Collections;

public class TimeProgressCalc : MonoBehaviour {

	WaitForSeconds ws005 = new WaitForSeconds(0.05f);

	int _workIndex = 0;

	public void start(float duration, Callback.Progress callback)
	{
		++_workIndex;

		StartCoroutine(valueUpdater(_workIndex, duration, callback));
	}


	IEnumerator valueUpdater(int workIndex, float duration,  Callback.Progress callback)
	{
		float time = 0.0f;
		bool isComplete = false;
		
		while(workIndex == _workIndex && isComplete == false)
		{
			time += 0.05f;
			float step = time / duration;
			step = Easing.EaseOut( time / duration, EasingType.Cubic);
			
			if(time >= duration) 
			{
				step = 1.0f;
				isComplete = true;
			}

			if(callback != null) callback(step, isComplete);

			yield return ws005;
		}
	}
}
