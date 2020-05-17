using UnityEngine;
using System.Collections;

public class AnimatorResetter : MonoBehaviour {

	Animator anim;

	void Awake ()
	{
		anim = gameObject.GetComponent<Animator>();
	}


	void OnDisable()
	{
		Debug.LogError(anim.playbackTime);
		anim.playbackTime = 0.0f;
	}

}
