using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PVPGradeSlotManager : MonoBehaviour 
{
	public Transform tfMySlot;
	public Transform tfEnemySlot;

	Quaternion _q = new Quaternion();

	List<string> _ids = new List<string>();


	public bool isCompleteLoadingGradeSymbol = true;

	public void init(int myGrade, int enemyGrade)
	{
		gameObject.SetActive(true);

		isCompleteLoadingGradeSymbol = false;

		GameManager.me.effectManager.addLoadEffectData("E_CLASS_"+myGrade);
		GameManager.me.effectManager.addLoadEffectData("E_CLASS_"+enemyGrade);

		StartCoroutine(loadLogo(myGrade, enemyGrade));
	}


	List<GameObject> _logos = new List<GameObject>();


	IEnumerator loadLogo(int myGrade, int enemyGrade)
	{
		GameManager.me.effectManager.startLoadEffects(true);

		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}

		GameObject goMyLogo = GameManager.info.effectData["E_CLASS_"+myGrade].getPrefabEffect();
		GameObject goEnemyLogo = GameManager.info.effectData["E_CLASS_"+enemyGrade].getPrefabEffect();

		_logos.Add(goMyLogo);
		_logos.Add(goEnemyLogo);

		if(goMyLogo != null)
		{
			goMyLogo.transform.parent = tfMySlot;
			goMyLogo.transform.localScale = Vector3.one;
			goMyLogo.transform.localPosition = Vector3.zero;
			_q.eulerAngles = Vector3.zero;
			goMyLogo.transform.localRotation = _q;
		}
		
		if(goEnemyLogo != null)
		{
			goEnemyLogo.transform.parent = tfEnemySlot;
			
			goEnemyLogo.transform.localScale = Vector3.one;
			goEnemyLogo.transform.localPosition = Vector3.zero;
			_q.eulerAngles = Vector3.zero;
			goEnemyLogo.transform.localRotation = _q;
		}

		isCompleteLoadingGradeSymbol = true;
	}




	public void destroy()
	{
		for(int i = _logos.Count - 1; i >= 0; --i)
		{
			if(_logos[i] != null)
			{
				GameObject.DestroyImmediate(_logos[i], true);
			}
		}

		_logos.Clear();

		gameObject.SetActive(false);
	}


}
