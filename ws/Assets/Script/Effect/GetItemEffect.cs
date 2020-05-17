using UnityEngine;
using System.Collections;

sealed public class GetItemEffect : MonoBehaviour {
	
	public Animation animation;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public bool isEnabled
	{
		set
		{
			gameObject.SetActive(value);
		}
	}
	
	
	public void start(Vector3 pos, bool isLocalPosition = true)
	{
		pos.z = -50.0f;
		
		if(isLocalPosition) transform.localPosition = pos;
		else transform.position = pos;
		
		animation.Play();
	}
	
	public void onCompleteEffect()
	{
		GameManager.me.effectManager.setGetItemEffect(this);
	}



	
}

