using UnityEngine;
using System.Collections;

public class UIPopupPaused : UIPopupBase {
	
	public UIButton btnResume,btnRestart;

	public UILabel lbMsg;

	public override void show ()
	{
		if(gameObject.activeSelf) return;
		GameManager.me.isPaused = true;
		if(GameManager.replayManager != null) GameManager.replayManager.tempSave();
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		base.show ();
	}

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnResume.gameObject).onClick = onSelectResume;
		UIEventListener.Get(btnRestart.gameObject).onClick = onCheckRestart;
	}



	void onSelectResume(GameObject go)
	{
		//PerformanceManager.GetInstance().CheckRoot();

#if UNITY_EDITOR
		GameManager.setTimeScale = 1.0f;
#endif

		GameManager.me.isPaused = false;

		if(GameManager.me.isFastPlay && GameManager.me.isAutoPlay)
		{
			GameManager.setTimeScale = UIPlay.FAST_PLAY_SPEED;
		}

		//SoundManager.GetInstance().bgm_play.Resume();
		hide ();
	}



	void onCheckRestart(GameObject go)
	{
		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Hell)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("MSG_PAUSE_QUIT_HELL") ,onSelectRestart);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Championship || GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Friendly)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("MSG_PAUSE_QUIT_PVP") ,onSelectRestart);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong )
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("MSG_PAUSE_QUIT_HELL") ,onSelectRestart);
		}
		else
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("MSG_PAUSE_QUIT_GAME")  ,onSelectRestart);
		}
	}


	void onSelectRestart()
	{

		GameManager.me.isPaused = true;
		GameManager.setTimeScale = 1.0f;

#if UNITY_EDITOR

		if(DebugManager.instance.useDebug )
		{
			GameManager.me.returnToSelectScene();
		}
		else
#endif
		{
			GameManager.me.stageManager.isSurrenderGame = true;

			HellModeManager.instance.hellClearType = Result.Type.GiveUp;

			GameManager.me.stageManager.nowPlayingGameResult = Result.Type.Fail;

			GameManager.me.playGameOver = true;

			GameManager.me.isPlaying = false;

//			GameManager.me.playCamRenderImage.saturationAmount = 1.0f;
//			GameManager.me.playCamRenderImage.enabled = true;

			GameManager.me.onCompleteRound(WSDefine.GAME_GIVEUP);
		}
		//GameManager.setTimeScale = 1;
		//Main.GetInstance().isRunLock = true;
		//Main.GetInstance().SetEndGame("shop");
		hide ();
	}

	void OnDestroy()
	{
		if(btnResume != null) UIEventListener.Get(btnResume.gameObject).onClick = null;
		if(btnRestart != null) UIEventListener.Get(btnRestart.gameObject).onClick = null;
		btnResume = null;
		btnRestart= null;
	}

}
