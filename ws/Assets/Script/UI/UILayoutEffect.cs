using UnityEngine;
using System.Collections;

public class UILayoutEffect : UIBase 
{

	public enum LayoutTransitionEffect
	{
		LetterBoxHide, LetterBoxShow, GameUI, GameUI_NO_TOUCH, FADE_IN, FADE_OUT, HIDE_ALL, SIDE_FADE_IN, SIDE_FADE_OUT
	}
	
	
	public override void show()
	{
		base.show();
	}
	
	public override void hide()
	{
		start(LayoutTransitionEffect.LetterBoxHide,0);
		start(LayoutTransitionEffect.FADE_IN,0);
		
		base.hide();
	}
	
	[HideInInspector]
	public bool isReady = false;
	
	public UISprite spLetterBoxTop;
	public UISprite spLetterBoxBottom;
	
	private Vector3 _v = new Vector3(1200.0f, 2.0f, 1.0f);
	private LayoutTransitionEffect effectType;
	
	public UISprite spFade;

	public Animation sideFade;

	public void start(LayoutTransitionEffect type, float motionTime = 0.0f, bool fadeWhite = false)
	{
		if(gameObject.activeSelf == false) show ();
		
		isReady = false;
		switch(type)
		{
		case LayoutTransitionEffect.LetterBoxHide:
			
			if(spLetterBoxTop.height > 0)
			{
				if(motionTime > 0.0f)
				{
					spLetterBoxTop.height = 120;
					spLetterBoxBottom.height = 120;
					spLetterBoxTop.enabled = true;
					spLetterBoxBottom.enabled = true;
					startLetterBox(2,motionTime);
					isVisibleAfterTransitionComplete = false;
				}
				else
				{
					isReady = true;
					spLetterBoxTop.width = 1200;
					spLetterBoxTop.height = 2;
					spLetterBoxBottom.width = 1200;
					spLetterBoxBottom.height = 2;
					spLetterBoxTop.enabled = isVisibleAfterTransitionComplete;
					spLetterBoxBottom.enabled = isVisibleAfterTransitionComplete;					
				}
			}
			break;
		case LayoutTransitionEffect.LetterBoxShow:

			if(motionTime > 0.0f)
			{
				GameManager.me.uiManager.uiPlay.hideMenu(motionTime * 0.5f);
				spLetterBoxTop.height = 0;
				spLetterBoxBottom.height = 0;
				spLetterBoxTop.enabled = true;
				spLetterBoxBottom.enabled = true;			
				startLetterBox(80,motionTime);
				isVisibleAfterTransitionComplete = true;
			}
			else
			{
				GameManager.me.uiManager.uiPlay.hideMenu(0.0f);
				isReady = true;
				spLetterBoxTop.height = 80;
				spLetterBoxBottom.height = 80;
				spLetterBoxTop.enabled = isVisibleAfterTransitionComplete;
				spLetterBoxBottom.enabled = isVisibleAfterTransitionComplete;					
			}
			break;			
		case LayoutTransitionEffect.GameUI:
			start(LayoutTransitionEffect.LetterBoxHide,motionTime * 0.5f);
			GameManager.me.uiManager.uiPlay.showMenu(motionTime);
			break;
		case LayoutTransitionEffect.GameUI_NO_TOUCH:
			
			start(LayoutTransitionEffect.LetterBoxHide,motionTime * 0.5f);
			GameManager.me.uiManager.uiPlay.showMenu(motionTime, true);
			break;		

		case LayoutTransitionEffect.FADE_IN:
			spFade.gameObject.SetActive(true);
			
			if(motionTime > 0)
			{
				if(fadeWhite) spFade.color = new Color(1.0f,1.0f,1.0f,1.0f);
				else spFade.color = new Color(0.0f,0.0f,0.0f,1.0f);

				TweenAlpha ta = TweenAlpha.Begin(spFade.gameObject,motionTime,0.0f);
				ta.eventReceiver = gameObject;
				ta.callWhenFinished = "onCompleteFadeIn";
			}
			else
			{
				if(fadeWhite) spFade.color = new Color(1.0f,1.0f,1.0f,0.0f);
				else spFade.color = new Color(0.0f,0.0f,0.0f,0.0f);
				spFade.gameObject.SetActive(false);
			}
			
			break;
		case LayoutTransitionEffect.FADE_OUT:
			spFade.gameObject.SetActive(true);
			
			if(motionTime > 0)
			{				
				if(fadeWhite) spFade.color = new Color(1.0f,1.0f,1.0f,0.0f);
				else spFade.color = new Color(0.0f,0.0f,0.0f,0.0f);
				TweenAlpha.Begin(spFade.gameObject,motionTime,1.0f);
			}
			else
			{
				if(fadeWhite) spFade.color = new Color(1.0f,1.0f,1.0f,1.0f);
				else spFade.color = new Color(0.0f,0.0f,0.0f,1.0f);
			}
			

			break;	

		case LayoutTransitionEffect.SIDE_FADE_IN:
			sideFade.gameObject.SetActive(true);

			sideFade.clip = sideFade.GetClip("nextstage_start");
			sideFade.Rewind();
			sideFade["nextstage_start"].time = 0;
			sideFade.Play("nextstage_start");
			break;

		case LayoutTransitionEffect.SIDE_FADE_OUT:
			sideFade.gameObject.SetActive(true);

			sideFade.clip = sideFade.GetClip("nextstage_end");
			sideFade.Rewind();
			sideFade["nextstage_end"].time = 0;
			sideFade.Play("nextstage_end");
			break;

		case LayoutTransitionEffect.HIDE_ALL:
			
			if(effectType == LayoutTransitionEffect.GameUI || effectType == LayoutTransitionEffect.GameUI_NO_TOUCH)
			{
				GameManager.me.uiManager.uiPlay.hideMenu(motionTime);
			}
			else if(effectType == LayoutTransitionEffect.LetterBoxShow)
			{
				start(LayoutTransitionEffect.LetterBoxHide,motionTime);
			}
			else
			{
				GameManager.me.uiManager.uiPlay.hideMenu(motionTime);
				start(LayoutTransitionEffect.LetterBoxHide,motionTime);
			}

			sideFade.gameObject.SetActive(false);

			break;
			
		}
		
		
		effectType = type;
	}
	
	void onCompleteFadeIn()
	{
		spFade.gameObject.SetActive(false);
	}
	
	private int _letterBoxCheckNum = 0;
	private bool isVisibleAfterTransitionComplete = false;
	
	void startLetterBox(int targetScale, float motionTime)
	{
		_letterBoxCheckNum = 0;
		
		TweenHeight tp = TweenHeight.Begin(spLetterBoxTop, motionTime, targetScale);
		tp.from = spLetterBoxTop.height;
		tp.to = targetScale;
		tp.eventReceiver = gameObject;
		tp.callWhenFinished = "onCompleteLatterBoxEffect";
		tp.method = UITweener.Method.EaseIn;
		
		TweenHeight tp2 = TweenHeight.Begin(spLetterBoxBottom, motionTime, targetScale);
		tp2.from = spLetterBoxBottom.height;
		tp2.to = targetScale;
		tp2.callWhenFinished = "onCompleteLatterBoxEffect";
		tp2.method = UITweener.Method.EaseIn;			
	}
	
	
	void onCompleteLatterBoxEffect()
	{
		++_letterBoxCheckNum;
		
		if(_letterBoxCheckNum == 2)
		{
			isReady = true;
			
			spLetterBoxTop.enabled = isVisibleAfterTransitionComplete;
			spLetterBoxBottom.enabled = isVisibleAfterTransitionComplete;			
			
		}
	}
	
}
