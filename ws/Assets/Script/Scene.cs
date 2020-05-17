using System;

public class Scene
{
	public class STATE
	{
		public const int SPLASH = 100;
		public const int OPENING = 200;
		public const int TITLE = 300;
		public const int STORE = 400;
		public const int PLAY_INTRO = 500;
		public const int PLAY_READY = 600;
		public const int PLAY_WATING_ENEMY = 700;
		public const int PLAY_BATTLE = 800;
		public const int PLAY_WATING_BOSS = 900;
		public const int PLAY_BOSS = 1000;
		public const int PLAY_LAST_MONSTER_DIE = 1100;
		public const int PLAY_CLEAR_SUCCESS = 1200;
		public const int PLAY_CLEAR_FAILED = 1300;
		public const int PLAY_CLEAR_DRAW = 1400; 
		public const int CUT_SCENE_PLAY = 1500;
	}

	public Scene ()
	{
	}


	public enum IntroStep
	{
		IntroProgress, MissingPaymentCheck, WaitForTouch, NoticeCheck, AttendanceCheck, PlayGame
	}

}

