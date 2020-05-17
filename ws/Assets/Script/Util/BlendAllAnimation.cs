using UnityEngine;
using System.Collections;

public class BlendAllAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void playAni()
	{
		int i = 0;
		
		foreach(AnimationState st in animation)
		{
			if(i == 0)
			{
				animation.Play(st.name);
			}
			else
			{
				animation.Blend(st.name);
			}
			
			++i;
		}
	}

	void OnEnable()
	{
		playAni();
	}
}
