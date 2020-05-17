using UnityEngine;
using System.Collections;

sealed public class BoxTransitionEffect : MonoBehaviour {
	
	public tk2dAnimatedSprite[] forwardSprs = new tk2dAnimatedSprite[8];
	public tk2dAnimatedSprite[] backwardSprs = new tk2dAnimatedSprite[8];
	
	// Use this for initialization
	void Start () {
		stopForward();
		stopBackward();
		
		foreach(tk2dAnimatedSprite spr in forwardSprs)
		{
			spr.animationCompleteDelegate = onCompleteForwardAni;
		}		
		
		foreach(tk2dAnimatedSprite spr in backwardSprs)
		{
			spr.animationCompleteDelegate = onCompleteBackwardAni;
		}			
		
	}
	
	private int _forwardAniCount;
	
	public void playForward()
	{
		_forwardAniCount = 0;
		
		foreach(tk2dAnimatedSprite spr in forwardSprs)
		{
			spr.gameObject.active = true;
			spr.Play();			
		}
	}
	
	
	private void onCompleteForwardAni(tk2dAnimatedSprite sprite, int clipId)
	{
		++_forwardAniCount;
		
		if(_forwardAniCount == 8) GameManager.me.onCompleteBoxTransitionForwardComplete();
	}	
	
	public IEnumerator delayBackward()
	{
		yield return new WaitForSeconds(0.5f);
		stopForward();
		playBackward();
	}
	
	
	private int _backwardAniCount;
	
	public void playBackward()
	{
		_backwardAniCount = 0;
		
		foreach(tk2dAnimatedSprite spr in backwardSprs)
		{
			spr.gameObject.active = true;
			spr.Play();			
		}
	}
	
	
	
	private void onCompleteBackwardAni(tk2dAnimatedSprite sprite, int clipId)
	{
		++_backwardAniCount;

		if(_backwardAniCount == 8) 
		{
			stopBackward();
			GameManager.me.onCompleteBoxTransitionBackwardComplete();
		}
	}		

	
	void stopForward()
	{
		foreach(tk2dAnimatedSprite spr in forwardSprs)
		{
			spr.StopAndResetFrame();
			spr.gameObject.active = false;
		}
	}
	
	void stopBackward()
	{
		foreach(tk2dAnimatedSprite spr in backwardSprs)
		{
			spr.StopAndResetFrame();
			spr.gameObject.active = false;
		}		
	}
	
}
