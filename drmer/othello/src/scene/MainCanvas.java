package scene;

import java.awt.Graphics;
import java.awt.event.KeyEvent;
import java.io.IOException;

import javax.tv.util.TVTimer;
import javax.tv.util.TVTimerSpec;
import javax.tv.util.TVTimerWentOffEvent;
import javax.tv.util.TVTimerWentOffListener;

import org.havi.ui.HSound;

import app.Reference;

import com.dreamer.x2let.app.control.ScreenManager;
import com.dreamer.x2let.ui.core_game.Animator;
import com.dreamer.x2let.ui.core_game.BasicComponent;
import com.dreamer.x2let.ui.core_game.BasicContainerSceneImpl;
import com.dreamer.x2let.ui.core_game.LogicInterface;
import com.dreamer.x2let.util.ImageUtil;
import com.dreamer.x2let.util.ResourceUtil;
import component.Board;
import component.CGameConstants;
import component.ResourceManager;
import component.VModule;

import drawer.MappingDrawer;
import dreamer.biddle.client.application.AppManager;
import dreamer.biddle.client.remote.BluRemoteServer;

public class MainCanvas extends BasicContainerSceneImpl implements LogicInterface, CGameConstants, TVTimerWentOffListener
{
	private static final long serialVersionUID = 1L;

	private TVTimerSpec timerSpec;

	private ResourceManager rm = null;
	private VModule vm;

	BasicComponent sceneComponent;

	boolean sceneInitialized = false;

	private final int TITLE_MODE		= 0;
	private final int GAME_MODE			= 1;

	public int play_mode = TITLE_MODE;

	/////////////////////////////////// title state/////////////////////////////////////////
	private final int TC_LOAD		= 0;
	private final int TC_SELECT		= 100;
	private final int TC_QUIT		= 200;
	private final int TC_OUT		= 300;
	private final int TC_HELP		= 400;
	private final int TC_WAIT 		= 500;

	private int  selectCnt = 0;
	private int  selectAni = 0;
	private int  curAni = 0;
	private int  cardY = 0;
	private boolean selectbool = true;
	private int title_mode = TC_LOAD;
	private int old_title_mode = title_mode;
	private int _cursor_idx;
	private int _cursor_y[];
	private int popupCursor;
	private boolean okBool = false;
	private int okCnt = 0;
	private boolean selBool = true;

	private int aniCnt=0;

	public static final int MENU_MAX = 3;

	/////////////////////////////////// game state/////////////////////////////////////////
	public final int GS_INTRO		= 0;
	public final int GS_MYSTONE		= 100;
	public final int GS_READY		= 200;
	public final int GS_PLAY		= 300;
	public final int GS_HELP		= 400;
	public final int GS_GAMEOVER	= 500;
	public final int GS_MENU		= 600;
	public final int GS_TITLE		= 700;
	private final int BLUAD_BANNER 			= 800;
	private final int BLUAD_DETAIL 			= 900;

	public int game_mode; // game state, PLAY, HELP, QUIT,
	public int game_oldMode = game_mode;

	// play step
	private final static int PS_1 		= 0;
	private final static int PS_2 		= 10;
	private final static int PS_COM		= 20;
	private final static int PS_GAMEOVER 	= 30;

	private Board board;
	private int nowStone;
	private static final int INIT_CURSOR_X = 3; // 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙 커占쏙옙 占쏙옙치(X)
	private static final int INIT_CURSOR_Y = 3; // 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙 커占쏙옙 占쏙옙치(Y)
	private int cursorX; // 占쏙옙占쏙옙占실울옙占쏙옙占쏙옙 커占쏙옙 占쏙옙치(X)
	private int cursorY; // 占쏙옙占쏙옙占실울옙占쏙옙占쏙옙 커占쏙옙 占쏙옙치(Y)

	private int others_cursorX; // 占쏙옙占쏙옙占실울옙占쏙옙占쏙옙 커占쏙옙 占쏙옙치(X)
	private int others_cursorY; // 占쏙옙占쏙옙占실울옙占쏙옙占쏙옙 커占쏙옙 占쏙옙치(Y)

	//	private static final int STONE_LEFT_TOP_X = 574;
	private static final int STONE_LEFT_TOP_X = 454;
	private static final int STONE_LEFT_TOP_Y = 239;
	private int sx;
	private int sy;

	public int rsCnt; // ready,start mode counter
	public int playMode;
	public int helpCnt;
	public boolean cursorBool;	// true == 占쏙옙치占쏙옙 占쏙옙占쏙옙占싹몌옙 커占쏙옙占쏙옙占쏙옙占�占쏙옙占�占쌕뀐옙占�
	public boolean bEnableKey; 	// true == 키 占쌉뤄옙 占쏙옙占쏙옙.
	public boolean overbool; 	// true == 占쏙옙占싱삼옙 占쏙옙 占쌘몌옙占쏙옙 占쏙옙占쏙옙.
	public boolean bGotoGameover; // gameover占쏙옙 占쏙옙占쏙옙 占싼댐옙.
	public boolean menubool;
	public boolean gamebool;
	public boolean resultbool;

	public int active_cnt; // active stone ani counter
	public int ani_temp;
	public int result_temp;
	public int delay_cnt; // black turn 占쏙옙 white turn 占쏙옙占쏙옙 delay
	public int over_cnt;
	public int menu_temp;

	public int popup_idx = -1;
	public int myStoneColor;
	private int stone;
	public int popup_cursor;

	public int gCnt;
	private int overCnt;

	public int turn_panel_cnt;
	private boolean blk_turn_bool = false;
	private boolean wht_turn_bool = false;

	private int fadeState[] = new int [10];
	private int tempCnt = 9;
	private int changeCnt = 0;

	private boolean loadBool=false;

	boolean b_sndBGM=false;

	
	public BluRemoteServer brs;
	
	MappingDrawer drawer;

	public void initScene()
	{
		rm = ResourceManager.getInstance();
		vm = new VModule();

		_cursor_y = new int[MENU_MAX];
		_cursor_y[0] = 444+20;
		_cursor_y[1] = 496+20;
		_cursor_y[2] = 548+20;

		for(int i=0; i<3; i++)
		{
			ImageUtil.getImage("image/title/mode_card"+ i +".png");
			ImageUtil.getImage("image/title/game_help"+ i +".png");
		}
	}

	public void resetScene()
	{
	}

	public void showScene()
	{
		setVisible(true);
		
		brs = BluRemoteServer.getInstance();
		brs.attachToBluRemoteServer(this);
		
		if (!sceneInitialized)
		{
			drawer = new MappingDrawer();
//			drawer.setImageObserver(this);
			drawer.setScreenMapping(1080, 720);
			
			sceneComponent = new SceneComponent();
			sceneComponent.setBounds(0, 0, 1280, 720);
			add(sceneComponent);

//			introComponent = new IntroComponent();
//			introComponent.setBounds(0, 0, 720, 480);
////			add(introComponent);
//
//			gameComponent = new GameComponent();
//			gameComponent.setBounds(0, 0, 720, 480);
////			add(gameComponent);

			sceneInitialized = true;
		}

		titleInit();
		gameInit();
		if(!loadBool) startTimer();

		Animator.getInstance().setLogicInterface(this);
		Animator.getInstance().start();
	}

	public void hideScene()
	{
		if(play_mode==TITLE_MODE)
			stopBGM(SND_INTRO);
		else
			stopBGM(SND_BGM);
		
		Animator.getInstance().pause();
	}

	public void destroyScene()
	{
		if(play_mode==TITLE_MODE)
			stopBGM(SND_INTRO);
		else
			stopBGM(SND_BGM);
		
		if(drawer != null) drawer = null;
		if(vm != null) vm=null;
		
		if (board != null){
			board.dispose();
			board = null;
		}
		
		Animator.getInstance().pause();
	}

	public void startTimer()
	{
		if(timerSpec == null)
		{
			timerSpec = new TVTimerSpec();
			timerSpec.setAbsolute(true);
			timerSpec.setDelayTime(50);
			timerSpec.setRepeat(false);
			timerSpec.setRegular(true);
			timerSpec.addTVTimerWentOffListener(this);
		}
		try
		{
			TVTimer.getTimer().scheduleTimerSpec(timerSpec);
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	}
	public void stopTimer()
	{
		if(timerSpec != null)
		{
			TVTimer.getTimer().deschedule(timerSpec);
			timerSpec.removeTVTimerWentOffListener(this);
			timerSpec = null;
		}
	}
	public void timerWentOff(TVTimerWentOffEvent e)
	{
		try
		{
			for(int i = 0; i < rm.sound.length; i++)
			{
				rm.sound[i] = new HSound();
			}
			byte [] snd0 = ResourceUtil.getInstance().getByteArray(("sound/bgm.bdmv"));
			rm.sound[0].set(snd0);
			byte [] snd1 = ResourceUtil.getInstance().getByteArray(("sound/draw.bdmv"));
			rm.sound[1].set(snd1);
			byte [] snd2 = ResourceUtil.getInstance().getByteArray(("sound/ready.bdmv"));
			rm.sound[2].set(snd2);
			byte [] snd3 = ResourceUtil.getInstance().getByteArray(("sound/stone_a.bdmv"));
			rm.sound[3].set(snd3);
			byte [] snd4 = ResourceUtil.getInstance().getByteArray(("sound/win.bdmv"));
			rm.sound[4].set(snd4);
			byte [] snd5 = ResourceUtil.getInstance().getByteArray(("sound/wrong.bdmv"));
			rm.sound[5].set(snd5);
			byte [] snd6 = ResourceUtil.getInstance().getByteArray(("sound/key_move.bdmv"));
			rm.sound[6].set(snd6);
			byte [] snd7 = ResourceUtil.getInstance().getByteArray(("sound/key_select.bdmv"));
			rm.sound[7].set(snd7);
			byte [] snd8 = ResourceUtil.getInstance().getByteArray(("sound/lose.bdmv"));
			rm.sound[8].set(snd8);
			byte [] snd9 = ResourceUtil.getInstance().getByteArray(("sound/menu.bdmv"));
			rm.sound[9].set(snd9);
			byte [] snd10 = ResourceUtil.getInstance().getByteArray(("sound/warning.bdmv"));
			rm.sound[10].set(snd10);
			byte [] snd11 = ResourceUtil.getInstance().getByteArray(("sound/start.bdmv"));
			rm.sound[11].set(snd11);
			byte [] snd12 = ResourceUtil.getInstance().getByteArray(("sound/intro.bdmv"));
			rm.sound[12].set(snd12);

			//			ImageUtil.getImage("image/popup/pop_up.png");
			//			ImageUtil.getImage("image/popup/emblem_black.png");
			//			ImageUtil.getImage("image/popup/emblem_white.png");
			//			ImageUtil.getImage("image/popup/emblem_draw.png");
			//
			//			ImageUtil.getImage("image/popup/message_draw.png");
			//			ImageUtil.getImage("image/popup/message_win.png");
			//
			//			ImageUtil.getImage("image/popup/my_stone_message.png");
			//			ImageUtil.getImage("image/popup/message_lose.png");
			//
			//			ImageUtil.getImage("image/turn_black.png");
			//			ImageUtil.getImage("image/turn_panel.png");
			//			ImageUtil.getImage("image/turn_white.png");
			//
			//			ImageUtil.getImage("image/chip.png");
			//			ImageUtil.getImage("image/ready.png");
			//			ImageUtil.getImage("image/start.png");
		}
		catch (IOException ie)
		{
			ie.printStackTrace();
		}
		stopTimer();
		loadBool = true;
	}

	public void titleInit()
	{
		title_mode = rm.getTitleMode();

		selectCnt = 0;
		selectAni = 0;
		curAni = 0;
		cardY = 0;
		selectbool = true;
		okBool = false;
		okCnt = 0;

		_cursor_idx = 0;
		popupCursor = 0;

		tempCnt = 9;
		changeCnt=0;
		for(int i=0; i<10; i++){
			fadeState[i] = 0;
		}

		selBool = true;
	}

	public void gameInit()
	{
		game_mode = GS_INTRO;
		board = new Board();

		cursorX = INIT_CURSOR_X;
		cursorY = INIT_CURSOR_Y;

		rsCnt = 0;
		playMode = 0;
		helpCnt = 0;
		cursorBool=false;
		bEnableKey = true;
		overbool = false;
		bGotoGameover = false;
		menubool = false;
		gamebool = false;
		resultbool = true;

		active_cnt=0;
		ani_temp=2;
		result_temp = -1;
		delay_cnt = -1;
		over_cnt = 0;
		menu_temp = 0;

		overCnt = 0;

		turn_panel_cnt = 2;
		blk_turn_bool = false;
		wht_turn_bool = false;

		tempCnt = 9;
		changeCnt=0;
		for(int i=0; i<10; i++){
			fadeState[i] = 0;
		}
		b_sndBGM=true;
	}


	//TODO : logic
	//TODO : logic
	//TODO : logic
	//TODO : logic
	long startTime = 0;    	
	long endTime = 0;
	public void logic() throws InterruptedException 
	{
		int defaultSleep = 100;

		endTime = System.currentTimeMillis() - startTime;

		if(endTime <= defaultSleep)
		{
			if(endTime <= 0) endTime = 0;
			int sleepTime = (int)(defaultSleep - endTime);
			Thread.sleep(sleepTime);
		}
		else
		{
			Thread.sleep(100);
		}
		startTime = System.currentTimeMillis();

		switch(play_mode)
		{
		case TITLE_MODE:
			titleLogic();
			break;
		case GAME_MODE:
			if(game_mode != BLUAD_BANNER && game_mode != BLUAD_DETAIL)
			{
				gameLogic();
			}
			break;
		}
		sceneComponent.draw();
	}


	private void titleLogic()
	{
		switch(title_mode)
		{
		case TC_LOAD:
			//			for(int i=9; i>=0; i--)
			//			{
			//				if(tempCnt<=i)
			//				{
			//					fadeState[i]++;
			//					if(fadeState[i]>5)
			//						fadeState[i] = 5;
			//				}
			//			}
			//			tempCnt--;
			//			if(tempCnt<0) tempCnt=0;
			//
			//			changeCnt+=1;
			//			if(changeCnt>=16)
			//			{
			////				playBGM();
			//				title_mode = TC_SELECT;
			//				changeCnt=0;
			//				for(int i=0; i<10; i++)
			//					fadeState[i] = 0;
			//			}
			aniCnt++;
			if(aniCnt>5)aniCnt=0;
			break;
		case TC_SELECT:
			select();
			break;
		case TC_OUT:
			for(int i=0; i<=9; i++)
			{
				if(tempCnt>=i)
				{
					fadeState[i]++;
					if(fadeState[i]>5)
						fadeState[i] = 5;
				}
			}
			tempCnt++;
			if(tempCnt>9) tempCnt=9;

			changeCnt+=1;
			if(changeCnt>=16)
			{
				gotoGame();
			}
			break;
		case TC_WAIT:
			if(loadBool)
			{
				title_mode = TC_SELECT;
			}
		}
	}

	private void gameLogic()
	{
		gCnt++;
		switch(game_mode)
		{
		case GS_INTRO:
			intro();
			break;
		case GS_MYSTONE:
			rsCnt++;
			if(rsCnt>15)
			{
				game_mode = GS_READY;
				rsCnt = 0;
			}
			break;
		case GS_READY:
			if (rsCnt==2)
				playSound(SND_READY);
			else if (rsCnt==15)
				playSound(SND_START);
			rsCnt++;
			if(rsCnt>25)
			{
				board.initBoard(false);
				setGameState(GS_PLAY);
				rsCnt = 0;
				turn_panel_cnt = 0;
			}
			break;
		case GS_PLAY:
			if(blk_turn_bool)
			{
				turn_panel_cnt--;
				if (turn_panel_cnt<0)
				{
					turn_panel_cnt=0;
					blk_turn_bool = false;
				}
			}
			if(wht_turn_bool)
			{
				turn_panel_cnt++;
				if (turn_panel_cnt>4)
				{
					turn_panel_cnt=4;
					wht_turn_bool = false;
				}
			}
			procPlay();
			break;
		case GS_GAMEOVER:
			overCnt++;
			if(overCnt>50)
			{
				gameInit();
				overCnt=0;
				setGameState(GS_READY);
			}
			break;
		case GS_TITLE:
			for(int i=0; i<=9; i++)
			{
				if(tempCnt>=i)
				{
					fadeState[i]++;
					if(fadeState[i]>5)
						fadeState[i] = 5;
				}
			}
			tempCnt++;
			if(tempCnt>9) tempCnt=9;

			changeCnt+=1;
			if(changeCnt>=16)
			{
				gotoTitle();
			}
			break;
		}
	}

	public void select()
	{
		if(selBool)
		{
			playBGM(SND_INTRO);
			selBool = false;
		}
		if(!okBool)
		{
			selectCnt++;
			if(selectCnt>3)
			{
				selectCnt = 3;
				cardY++;
				if(cardY>3)
				{
					cardY = 3;
					selectbool = false;
				}
				if(_cursor_idx==1)
				{
					selectAni++;
					curAni++;
					if(selectAni>6)
					{
						selectAni=6;
						curAni=0;
					}
				}
				else if(_cursor_idx==0)
				{
					selectAni--;
					curAni++;
					if(selectAni<0)
					{
						selectAni=0;
						curAni=0;
					}
				}
			}
		}
		else
		{
			okCnt++;
			if(okCnt>8)
			{
				okCnt=8;
				cardY--;
				if(cardY<0)
				{
					cardY = 0;
					title_mode = TC_OUT;
					tempCnt = 0;
					stopBGM(SND_INTRO);
				}
			}
		}
	}

	public void intro()
	{
		for(int i=9; i>=0; i--)
		{
			if(tempCnt<=i)
			{
				fadeState[i]++;
				if(fadeState[i]>5)
					fadeState[i] = 5;
			}
		}
		tempCnt--;
		if(tempCnt<0) tempCnt=0;

		changeCnt+=1;
		if(changeCnt>=16)
		{
			game_mode = GS_MYSTONE;
			if (rm.getGameType() == GAME_TYPE_2P)
				game_mode = GS_READY;

			changeCnt=0;
			for(int i=0; i<10; i++)
				fadeState[i] = 0;
		}
	}

	public void setGameState(int _newState)
	{
		if (game_mode == GS_PLAY){
			stopBGM(SND_INTRO);
		}

		game_mode = _newState;
		if(game_mode == GS_PLAY)
		{
			if(b_sndBGM)
			{
				playBGM(SND_BGM);
				b_sndBGM=false;
			}
			getNewCursorPos(Board.BLACK_STONE);
		}
	}

	public int rand(int max){
		return (int)(Math.random()*10%max);
	}

	private void drawMystone(Graphics g)
	{
		drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_up.png"), 512, 245, this);
		drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_black.png"), 607, 372, this);
		drawer.drawImage(g, ImageUtil.getImage("image/popup/my_stone_message.png"), 604, 504, this);
	}


	private void drawReady(Graphics g)
	{
		int readyCnt = rsCnt;
		if(rsCnt>20) readyCnt = 20;
		vm.setAlpha(g, rm.readyData[readyCnt][4]);
		drawer.drawImage(g, ImageUtil.getImage("image/ready.png"), rm.readyData[readyCnt][0], rm.readyData[readyCnt][1],
				rm.readyData[readyCnt][2], rm.readyData[readyCnt][3], this);
		vm.offAlpha(g);

		if(rsCnt>12)
			drawer.drawImage(g, ImageUtil.getImage("image/start.png"), rm.startData[rsCnt-13][0], rm.startData[rsCnt-13][1], this);
	}


	boolean whiteBool = false;
	boolean blackBool = false;
	boolean activeBool = false;
	public void procPlay()
	{
		if (board.isActiveStone()!=0)
		{
			if(whiteBool)
			{
				active_cnt++;
				if(active_cnt > 4)
				{
					active_cnt = 4;
					activeBool = true;
					whiteBool = false;
				}
			}
			else if(blackBool)
			{
				active_cnt--;
				if(active_cnt < 0)
				{
					active_cnt = 0;
					activeBool = true;
					blackBool = false;
				}
			}
			if(activeBool)
			{
				delay_cnt = 0;
				activeBool = false;
				ani_temp = 2;
				board.changeActiveStone(nowStone);

				if (!board.check_DOOL_JA_RI(Board.WHITE_STONE))
				{
					if(rm.getGameType()==GAME_TYPE_1P) setPlayStep(PS_COM);
					else if(rm.getGameType()==GAME_TYPE_2P) setPlayStep(PS_2);
					gamebool=true;
				}
				if (!board.check_DOOL_JA_RI(Board.BLACK_STONE))
				{
					setPlayStep(PS_1);
					gamebool=true;
				}
				if(!board.check_DOOL_JA_RI(Board.WHITE_STONE) && !board.check_DOOL_JA_RI(Board.BLACK_STONE)){
					overbool=true;
				}
			}
		}

		if(delay_cnt >= 0 || gamebool)
		{
			delay_cnt++;
			if(delay_cnt >= 8)
			{
				delay_cnt = -1;
				gamebool = false;

				//2008-07-18 10:06占쏙옙占쏙옙
				if(game_mode == GS_PLAY)
				{
					if (rm.getGameType()==GAME_TYPE_1P)
					{
						if(getPlayStep()==PS_1)
						{
							//turntemp = TURN_WHITE;
							setPlayStep(PS_COM);
							putStones();

						}
						else if(getPlayStep()==PS_COM)
						{
							//turntemp = TURN_BLACK;
							setPlayStep(PS_1);
							bEnableKey=true;
							getNewCursorPos(Board.BLACK_STONE);
							SetCoinTurnStart();
						}
					}
					else if (rm.getGameType()==GAME_TYPE_2P)
					{
						bEnableKey=true;
						if(getPlayStep()==PS_1)
						{
							//turntemp = TURN_WHITE;
							setPlayStep(PS_2);
							getNewCursorPos(Board.WHITE_STONE);
							SetCoinTurnStart();
						}
						else if(getPlayStep()==PS_2)
						{
							//turntemp = TURN_BLACK;
							setPlayStep(PS_1);
							getNewCursorPos(Board.BLACK_STONE);
							SetCoinTurnStart();
						}
					}
				}
			}
		}

		if (game_mode == GS_PLAY && bGotoGameover==false)
		{
			int cntP1 = 0;
			int cntP2 = 0;

			for (int i = 0; i < Board.HEIGHT; i++)
			{
				for (int j = 0; j < Board.WIDTH; j++)
				{
					if (board.getStone(j, i) == Board.BLACK_STONE)cntP1++;
					else if (board.getStone(j, i) == Board.WHITE_STONE)cntP2++;
				}
			}

			if(cntP1+cntP2==64 || cntP1==0 || cntP2==0 || overbool)
			{
				bGotoGameover=true;
				overbool = false;
			}

			if (cntP1 > cntP2) result_temp = 1;
			else if (cntP1 < cntP2) result_temp = 2;
			else if(cntP1 == cntP2) result_temp = 3;
		}

		if(bGotoGameover)
		{
			over_cnt++;
			if(over_cnt>10)
			{
				over_cnt=0;
				bGotoGameover = false;
				stopBGM(SND_BGM);

				setPlayStep(PS_GAMEOVER);
				setGameState(GS_GAMEOVER);

				if (rm.getGameType()==GAME_TYPE_2P) playSound(SND_WIN);

				if (result_temp==3){ //draw
					playSound(SND_DRAW);
				}
				else if (result_temp==1){ //black win
					if (rm.getGameType()==GAME_TYPE_1P) playSound(SND_WIN);
				}
				else if (result_temp==2){ //white win
					if (rm.getGameType()==GAME_TYPE_1P) playSound(SND_LOSE);
				}
			}
		}
	}

	public int getPlayStep() {
		return playMode;
	}

	public void setPlayStep(int m)
	{
		playMode = m;
	}

	public void drawImageAlpha(Graphics g, String img, int x, int y, int alphaVal)
	{
		vm.setAlpha(g, alphaVal);
		drawer.drawImage(g,ImageUtil.getImage(img), x, y, this);
		vm.offAlpha(g);
	}

	public void drawOtherCursor(Graphics g)
	{
		if (popup_idx >=0)return;
		if (delay_cnt != -1)return;
		if (getPlayStep() == PS_GAMEOVER || bGotoGameover)return;
		if (getPlayStep() == PS_1)return;
		if (getPlayStep() == PS_2)return;
		if (others_cursorX < 0 || others_cursorY < 0)return;

		int cx = STONE_LEFT_TOP_X + (others_cursorX * 84) - 12;
		int cy = STONE_LEFT_TOP_Y + (others_cursorY * 84) - 12;

		drawer.drawImage(g, ImageUtil.getImage("image/cursor/cursor.png"), cx, cy, cx+108, cy+108,
				216, 0, 108+216, 108, this);
	}

	public void drawCursor(Graphics g)
	{
		if (delay_cnt != -1)return;
		if (getPlayStep() == PS_GAMEOVER || bGotoGameover)return;
		if (rm.getGameType() == GAME_TYPE_1P && getPlayStep() != PS_1)return;

		// cursorBool == false

		int cursorPic = 0;
		if (cursorBool)cursorPic = 1;

		sx = STONE_LEFT_TOP_X + (cursorX * 84) - 12;
		sy = STONE_LEFT_TOP_Y + (cursorY * 84) - 12;
		//		drawer.drawImage(g,"image/cursor/cursor" + cursorPic + ".png", sx, sy, this);
		drawer.drawImage(g, ImageUtil.getImage("image/cursor/cursor.png"), sx, sy, sx+108, sy+108,
				(cursorPic*108), 0, 108+(cursorPic*108), 108, this);
		cursorBool=false;
	}

	public void getNewCursorPos(int _stoneColor)
	{
		int totalGuidePoint=0;

		//if (_guideMark){
		for (int i = 0; i < Board.HEIGHT; i++){
			for (int j = 0; j < Board.WIDTH; j++){
				if (board.getStone(j, i) == Board.EMPTY){
					if (board.putStone(j, i, _stoneColor, false)){
						totalGuidePoint++;
					}
				}
			}
		}
		if (totalGuidePoint==0)return;

		int posx[] = new int[totalGuidePoint];
		int posy[] = new int[totalGuidePoint];

		int readCnt=0;
		for (int i = 0; i < Board.HEIGHT; i++){
			for (int j = 0; j < Board.WIDTH; j++){
				if (board.getStone(j, i) == Board.EMPTY){
					if (board.putStone(j, i, _stoneColor, false)){
						posx[readCnt] = j;
						posy[readCnt] = i;
						readCnt++;
					}
				}
			}
		}

		int rand = (int)(Math.random()*10%totalGuidePoint);
		cursorX = posx[rand];
		cursorY = posy[rand];
	}

	public void drawBoard(Graphics g)
	{
		boolean _guideMark = false;
		int _nowStone = Board.BLACK_STONE;

		if (board.isActiveStone() == 0 && bEnableKey)
		{
			if (getPlayStep() == PS_1 || getPlayStep() == PS_2)
			{
				_guideMark = true;
				if (getPlayStep() == PS_2)_nowStone = Board.WHITE_STONE;
			}
		}

		for (int i = 0; i < Board.HEIGHT; i++)
		{
			for (int j = 0; j < Board.WIDTH; j++)
			{
				stone = board.getStone(j, i);
				sx = STONE_LEFT_TOP_X + (j * 84);
				sy = STONE_LEFT_TOP_Y + (i * 84);

				if (stone == Board.WHITE_STONE)
				{
					drawer.drawImage(g, ImageUtil.getImage("image/chip.png"), sx, sy, sx+84, sy+84, 0, 0, 84, 84, this);
				}
				else if (stone == Board.BLACK_STONE)
				{
					drawer.drawImage(g, ImageUtil.getImage("image/chip.png"), sx, sy, sx+84, sy+84, (4*84), 0, 84+(4*84), 84, this);
				}
				else
				{
					if (_guideMark)
					{
						if (board.putStone(j, i, _nowStone, false))
						{
							int tmp = gCnt%20;
							if (tmp > 9)tmp = 20-tmp;

							tmp = tmp * 10 + 1;
							drawImageAlpha(g, "image/cursor/guidemark.png", sx, sy, tmp);
						}
					}
				}
			}
		}

		if (board.isActiveStone()!=0)
		{
			for (int i = 0; i < Board.HEIGHT; i++)
			{
				for (int j = 0; j < Board.WIDTH; j++)
				{
					if (board.getStone(j, i) == Board.ACTIVE_STONE)
					{
						sx = STONE_LEFT_TOP_X + (j * 84);
						sy = STONE_LEFT_TOP_Y + (i * 84);

						if (nowStone == Board.BLACK_STONE)
							whiteBool = true;
						else
							blackBool = true;
						drawer.drawImage(g, ImageUtil.getImage("image/chip.png"), sx, sy, sx+84, sy+84,
								(active_cnt*84), 0, 84+(active_cnt*84), 84, this);
					}
				}
			}
		}
	}

	public void drawStoneAni(Graphics g, int sx, int sy, int _stoneColor, int aniCnt)
	{
		if (_stoneColor == Board.BLACK_STONE)
		{
			if(aniCnt<2)//==1)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/whiteBig.png"), sx, sy, sx+95, sy+109,
						(2*95), 0, 95+(2*95), 109, this);
			else if(aniCnt==2)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/blackBig.png"), sx, sy, sx+95, sy+109,
						(1*95), 0, 95+(1*95), 109, this);
			else if(aniCnt==3)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/blackBig.png"), sx, sy, sx+95, sy+109,
						0, 0, 95, 109, this);
		}
		else
		{
			if(aniCnt<2)//==1)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/blackBig.png"), sx, sy, sx+95, sy+109,
						(2*95), 0, 95+(2*95), 109, this);
			else if(aniCnt==2)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/whiteBig.png"), sx, sy, sx+95, sy+109,
						(1*95), 0, 95+(1*95), 109, this);
			else if(aniCnt==3)
				drawer.drawImage(g, ImageUtil.getImage("image/stone/whiteBig.png"), sx, sy, sx+95, sy+109,
						0, 0, 95, 109, this);
		}
	}

	public void putStones()
	{
		bEnableKey=false;
		others_cursorX = cursorX;
		others_cursorY = cursorY;
		if (getPlayStep() == PS_1)
		{
			if (board.check_DOOL_JA_RI(Board.BLACK_STONE))
			{
				if (board.putStone(cursorX, cursorY, Board.BLACK_STONE,	true))
				{
					playSound(SND_STONE_A);
					nowStone = Board.BLACK_STONE;
					cursorBool = true;
					wht_turn_bool = true;
				}
				else
				{
					playSound(SND_WRONG);
					bEnableKey = true;
				}
			}
		}
		else if (getPlayStep() == PS_2)
		{
			if (board.check_DOOL_JA_RI(Board.WHITE_STONE))
			{
				if (board.putStone(cursorX, cursorY, Board.WHITE_STONE,	true))
				{
					playSound(SND_STONE_A);
					nowStone = Board.WHITE_STONE;
					cursorBool = true;
					blk_turn_bool = true;
				}
				else
				{
					playSound(SND_WRONG);
					bEnableKey=true;
				}
			}
		}
		else if ( getPlayStep() == PS_COM)
		{
			if (board.check_DOOL_JA_RI(Board.WHITE_STONE))
			{
				if(board.putComputer())
				{
					playSound(SND_STONE_A);
					nowStone = Board.WHITE_STONE;
					blk_turn_bool = true;
				}
			}
		}
	}


	//TODO : SceneComponent
	//TODO : SceneComponent
	class SceneComponent extends BasicComponent {
		protected void draw(Graphics g) {
			switch(play_mode)
			{
			case TITLE_MODE:
				switch(title_mode)
				{
				case TC_LOAD:
					if(aniCnt < 3) drawer.drawImage(g, ImageUtil.getImage("image/PRESS_ENTER_KEY.png"), 725, 933, null);
					break;
				case TC_SELECT:
					if(selectbool)
					{
						drawer.drawImage(g, ImageUtil.getImage("image/title/mode_card"+ rm.getGameType()+ ".png"), 799, rm.cardData[cardY], this);
						if(okCnt>0 && okCnt<8)
						{
							if(okCnt%2==0) drawer.drawImage(g, ImageUtil.getImage("image/title/mode_card2.png"), 799, rm.cardData[cardY], this);
						}
					}
					else
					{
						if(selectCnt>2)
						{
							drawer.drawImage(g, ImageUtil.getImage("image/title/mode_card" + rm.selectAniData[selectAni][2] + ".png"), rm.selectAniData[selectAni][0],
									538, rm.selectAniData[selectAni][1], 431, this);

							drawer.drawImage(g, ImageUtil.getImage("image/title/mode_arr.png"), rm.curAniData[curAni][0], 715, rm.curAniData[curAni][0]+59, 715+83,
									0, 0, 59, 83, this);
							drawer.drawImage(g, ImageUtil.getImage("image/title/mode_arr.png"), rm.curAniData[curAni][1]+59, 715, rm.curAniData[curAni][1], 715+83,
									0, 0, 59, 83, this);
						}
					}
					break;
				case TC_QUIT:
					drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_up.png"), 682, 245, this);
					drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_exit_on.png"), 775, 507, this);
					drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_question.png"), 1103, 507, this);
					if (popupCursor == 0)
					{
						drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_yes_on.png"), 831, 613, this);
						drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_no_off.png"), 999, 613, this);
					}
					else if (popupCursor == 1)
					{
						drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_yes_off.png"), 831, 613, this);
						drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_no_on.png"), 999, 613, this);
					}
					break;
				case TC_OUT:
					for(int i=0; i<10; i++)
						drawQuitFade(g, i);
					break;
				case TC_HELP: //160
					drawer.drawImage(g,ImageUtil.getImage("image/help/help_back.png"), 266+160, 196, this);
					drawer.drawImage(g, ImageUtil.getImage("image/help/help_back.png"), 801+160+535, 196, 801+160, 196+687,
							0, 0, 535, 687, this);
					drawer.drawImage(g,ImageUtil.getImage("image/help/help_title.png"), 533+160, 240, this);
					drawer.drawImage(g,ImageUtil.getImage("image/help/help_p" + helpCnt + ".png"), 351+160, 359, this);
					drawer.drawImage(g, ImageUtil.getImage("image/help/help_page.png"), 1076+160, 789, 1076+160+143, 789+42,
							0, helpCnt*42, 143, 42+(helpCnt*42), this);
					drawer.drawImage(g,ImageUtil.getImage("image/help/pop_ok.png"), 763+160, 787, this);
					break;
				case TC_WAIT:
					drawLoading(g);
					break;
				}
				break;
			case GAME_MODE:
				//				drawRedKey(g);
				switch(game_mode)
				{
				case GS_INTRO:
					drawTopTurnPanel(g);
					for(int i=0; i<10; i++)
						drawStartFade(g, i);
					break;
				case GS_MYSTONE:
					drawTopTurnPanel(g);
					drawMystone(g);
					break;
				case GS_READY:
					drawTopTurnPanel(g);
					drawReady(g);
					break;
				case GS_PLAY:
					drawCursor(g);
					drawOtherCursor(g);
					drawBoard(g);
					drawScoreBoard(g);
					drawTopTurnPanel(g);
					if(turn_panel_cnt<=1)
						drawer.drawImage(g, ImageUtil.getImage("image/turn_black.png"), 130, 364, this);
					else if(turn_panel_cnt>2)
						drawer.drawImage(g, ImageUtil.getImage("image/turn_white.png"), 1220, 364, this);
					break;
				case GS_HELP:
					drawer.drawImage(g, ImageUtil.getImage("image/turn_panel.png"), 574, 109, 574+431, 109+102,
							0, (2*102), 431, 102+(2*102), this);
					drawHelp(g);
					break;
				case GS_GAMEOVER:
					drawBoard(g);
					drawTopTurnPanel(g);
					drawGameover(g);
					break;
				case GS_MENU:
					drawer.drawImage(g, ImageUtil.getImage("image/turn_panel.png"), 574, 109, 574+431, 109+102,
							0, (2*102), 431, 102+(2*102), this);
					drawGameQuit(g);
					break;
				case GS_TITLE:
					for(int i=0; i<10; i++)
						drawQuitFade(g, i);
					break;
				}
			}
		}
	}



	public final int load_gage_xy[] = { 39,1, 50, 3, 58, 11, 64, 24, 66, 39,64,50, 58,58, 49,64, 38, 66, 25, 64, 12,58, 4, 50, 1, 39, 3, 25, 12, 12,24, 4};
	public boolean _load_gage_indicater[] = new boolean[16];
	public int _load_gage_cnt;
	public void drawLoading(Graphics g)
	{//, int x, int y){

		int x = (int)((1920/2)*0.6666) - (84/2);//254/2;
		int y = (int)((1080/2)*0.6666) - (110/2);//334/2;


		g.drawImage(ImageUtil.getImage("image/loading/load_bg.png"), x,y, null);

		for(int i=0;i<_load_gage_indicater.length;i++)
		{
			if (_load_gage_indicater[i])
			{
				g.drawImage(ImageUtil.getImage("image/loading/load_bar.png"),
						x+load_gage_xy[i*2],y+load_gage_xy[i*2+1],x+load_gage_xy[i*2]+17,y+load_gage_xy[i*2+1]+17,
						i*17,0,(i+1)*17,17, null);
			}
		}
		_load_gage_cnt++;
		if (_load_gage_cnt==16)_load_gage_cnt=0;

		_load_gage_indicater[_load_gage_cnt] = !_load_gage_indicater[_load_gage_cnt];
	}


	private void drawStartFade(Graphics g, int i)
	{
		vm.setAlpha(g, (rm.start_fade[fadeState[i]][2]*10));
//		drawer.drawImage(g, ImageUtil.getImage("image/fade.png"), (i*192)+rm.start_fade[fadeState[i]][0], 0, rm.start_fade[fadeState[i]][1], 720, null);
		g.drawImage(ImageUtil.getImage("image/fade.png"), (i*128)+rm.start_fade[fadeState[i]][0], 0, rm.start_fade[fadeState[i]][1], 720, null);
		vm.offAlpha(g);
	}
	private void drawQuitFade(Graphics g, int i)
	{
		vm.setAlpha(g, (rm.quit_fade[fadeState[i]][2]*10));
//		drawer.drawImage(g, ImageUtil.getImage("image/fade.png"), (i*192)+rm.quit_fade[fadeState[i]][0], 0, rm.quit_fade[fadeState[i]][1], 720, null);
		g.drawImage(ImageUtil.getImage("image/fade.png"), (i*128)+rm.quit_fade[fadeState[i]][0], 0, rm.quit_fade[fadeState[i]][1], 720, null);
		vm.offAlpha(g);
	}

	public void drawHelp(Graphics g)
	{
		drawer.drawImage(g,ImageUtil.getImage("image/help/help_back.png"), 266, 196, this);
		drawer.drawImage(g, ImageUtil.getImage("image/help/help_back.png"), 801+535, 196, 801, 196+687,
				0, 0, 535, 687, this);
		drawer.drawImage(g,ImageUtil.getImage("image/help/help_title.png"), 533, 240, this);
		drawer.drawImage(g,ImageUtil.getImage("image/help/help_p" + helpCnt + ".png"), 351, 359, this);
		drawer.drawImage(g, ImageUtil.getImage("image/help/help_page.png"), 1076, 789, 1076+143, 789+42,
				0, helpCnt*42, 143, 42+(helpCnt*42), this);
		drawer.drawImage(g,ImageUtil.getImage("image/help/pop_ok.png"), 763, 787, this);
	}

	public void SetCoinTurnStart()
	{
		bSmallCoinTurn = true;
		nCoinTurnCnt = 0;
	}

	//public int score_y_offset;
	public boolean 	bSmallCoinTurn;
	public int	nCoinTurnCnt=0;
	public void drawScoreBoard(Graphics g)
	{
		// mode indicator
		int modeX = 707;
		int modeY = 940;
		int pic = rm.getGameType();
		drawer.drawImage(g, ImageUtil.getImage("image/player_mode.png"), modeX, modeY, modeX+165, modeY+19,
				(pic*165), 0, 165+(pic*165), 19, this);

		//		int stonePic = 0;

		if (bSmallCoinTurn)
		{
			//			if (nCoinTurnCnt==7)stonePic=2;
			//			if (nCoinTurnCnt==8)stonePic=1;
			//			if (nCoinTurnCnt==9)stonePic=0;
			//			if (nCoinTurnCnt==10)stonePic=2;
			//			if (nCoinTurnCnt==11)stonePic=1;
			nCoinTurnCnt++;
			if (nCoinTurnCnt == 12)
			{
				bSmallCoinTurn = false;
				nCoinTurnCnt = 0;
			}
		}

		//		if(turn_panel_cnt<=1)
		//		drawer.drawImage(g, ImageUtil.getImage("image/turn_black.png"), 130, 364, this);
		//		else if(turn_panel_cnt>2)
		//		drawer.drawImage(g, ImageUtil.getImage("image/turn_white.png"), 1220, 364, this);
		//		sdfsdfsdf
		//		if (nCoinTurnCnt != 1 && nCoinTurnCnt != 4 && nCoinTurnCnt != 7){
		//		if (getPlayStep() == PS_1){
		//		drawer.drawImage(g, ImageUtil.getImage("image/turn_black.png"), 130, 364, this);
		//		}
		//		else if (getPlayStep() == PS_2 || getPlayStep() == PS_COM){
		//		drawer.drawImage(g, ImageUtil.getImage("image/turn_white.png"), 1220, 364, this);
		//		}
		//		}

		// draw score number
		int cntBlack = 0;
		int cntWhite = 0;
		int s;

		for (int i = 0; i < Board.HEIGHT; i++)
		{
			for (int j = 0; j < Board.WIDTH; j++)
			{
				s = board.getStone(j, i);
				if (s == Board.BLACK_STONE) cntBlack++;
				else if (s == Board.WHITE_STONE) cntWhite++;
				else if (s == Board.ACTIVE_STONE)
				{
					if (nowStone == Board.BLACK_STONE) cntBlack++;
					else cntWhite++;
				}
			}
		}
		int num_w = 72;
		int num_h = 78;
		int tum = 72-15;

		//		PS_1
		String str = Integer.toString(cntBlack);
		if(str.length()==1) str = "0"+str;
		for(int i = str.length() ; i > 0 ; i--)
		{
			int digit = Integer.parseInt(str.substring(i - 1, i));
			digit*=num_w;

			drawer.drawImage(g, ImageUtil.getImage("image/num.png"),
					SCORE_BLACKX+(i-str.length())*tum, SCORE_Y,
					SCORE_BLACKX+(i-str.length())*tum+num_w, SCORE_Y+num_h,
					0+digit, 0, num_w+digit, num_h, null);
		}
		//		PS_2
		String str1 = Integer.toString(cntWhite);
		if(str1.length()==1) str1 = "0"+str1;
		for(int i = str1.length() ; i > 0 ; i--)
		{
			int digit = Integer.parseInt(str1.substring(i - 1, i));
			digit*=num_w;

			drawer.drawImage(g, ImageUtil.getImage("image/num.png"),
					SCORE_WHITEX+(i-str1.length())*tum, SCORE_Y,
					SCORE_WHITEX+(i-str1.length())*tum+num_w, SCORE_Y+num_h,
					0+digit, 0, num_w+digit, num_h, null);
		}
	}

	public void drawTopTurnPanel(Graphics g)
	{
		drawer.drawImage(g, ImageUtil.getImage("image/turn_panel.png"), 574, 109, 574+431, 109+102,
				0, (turn_panel_cnt*102), 431, 102+(turn_panel_cnt*102), this);
	}

	public void drawGameover(Graphics g)
	{
		drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_up.png"), 512, 245, this);
		if(rm.getGameType()==GAME_TYPE_1P)
		{
			if (result_temp==1)	// black win
			{
				if(resultbool) resultbool=false;
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_black.png"), 607, 372, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/sel_Black.png"), 595, 421, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_win.png"), 671, 491, this);
			}
			else if(result_temp==2) // white win
			{
				if(resultbool) resultbool=false;
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_black.png"), 607, 372, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/sel_Black.png"), 595, 421, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_lose.png"), 623, 491, this);
			}
			else if(result_temp==3) // draw
			{
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_draw.png"), 592, 405, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_draw.png"), 594, 539, this);
			}
		}
		else if (rm.getGameType()==GAME_TYPE_2P)
		{
			if (result_temp==1)	// black win
			{
				if(resultbool) resultbool=false;
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_black.png"), 607, 372, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/sel_Black.png"), 595, 421, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_win.png"), 671, 491, this);
			}
			else if(result_temp==2) // white win
			{
				if(resultbool) resultbool=false;
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_white.png"), 607, 372, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/sel_White.png"), 595, 421, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_win.png"), 671, 491, this);
			}
			else if(result_temp==3) // draw
			{
				drawer.drawImage(g,ImageUtil.getImage("image/popup/emblem_draw.png"), 592, 405, this);
				drawer.drawImage(g,ImageUtil.getImage("image/popup/message_draw.png"), 594, 539, this);
			}
		}

		//		draw score number
		int cntBlack = 0;
		int cntWhite = 0;
		int s;

		for (int i = 0; i < Board.HEIGHT; i++)
		{
			for (int j = 0; j < Board.WIDTH; j++)
			{
				s = board.getStone(j, i);
				if (s == Board.BLACK_STONE) cntBlack++;
				else if (s == Board.WHITE_STONE) cntWhite++;
				else if (s == Board.ACTIVE_STONE)
				{
					if (nowStone == Board.BLACK_STONE) cntBlack++;
					else cntWhite++;
				}
			}
		}

		int overX = 657;
		int overX1 = 839;
		int overY = 623;

		drawer.drawImage(g, ImageUtil.getImage("image/chip.png"), overX, overY, overX+84, overY+84, (4*84), 0, 84+(4*84), 84, this);
		drawer.drawImage(g, ImageUtil.getImage("image/chip.png"), overX1, overY, overX1+84, overY+84, 0, 0, 84, 84, this);

		int num_w = 72;
		int num_h = 78;
		int tum = 72-15;
		//		PS_1
		String str = Integer.toString(cntBlack);
		if(str.length()==1) str = "0"+str;
		for(int i = str.length() ; i > 0 ; i--)
		{
			int digit = Integer.parseInt(str.substring(i - 1, i));
			digit*=num_w;

			drawer.drawImage(g, ImageUtil.getImage("image/num.png"),
					761-72 +(i-str.length())*tum, 670,
					761-72 +(i-str.length())*tum+num_w, 670+num_h,
					0+digit, 0, num_w+digit, num_h, null);
		}

		drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_colon.png"), 779, 686, this);
		//		PS_2
		String str1 = Integer.toString(cntWhite);
		if(str1.length()==1) str1 = "0"+str1;
		for(int i = str1.length() ; i > 0 ; i--)
		{
			int digit = Integer.parseInt(str1.substring(i - 1, i));
			digit*=num_w;

			drawer.drawImage(g, ImageUtil.getImage("image/num.png"),
					945-72 +(i-str1.length())*tum, 670,
					945-72 +(i-str1.length())*tum+num_w, 670+num_h,
					0+digit, 0, num_w+digit, num_h, null);
		}
	}

	public void drawGameQuit(Graphics g)
	{
		drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_up.png"), 512, 245, this);

		//		if(menu_temp != 0) drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_arr.png"), 768, 417, 768+45, 417+41, 0, 0, 45, 41, this);
		//		if(menu_temp != 3)drawer.drawImage(g, ImageUtil.getImage("image/popup/pop_arr.png"), 768, 739, 768+45, 739+41, 0, 41, 45, 0, this);

		int menuX = 625;

		drawer.drawImage(g,ImageUtil.getImage("image/popup/pop_resume_on.png"), menuX, 471, this);
		drawer.drawImage(g,ImageUtil.getImage("image/popup/pop_main_on.png"), menuX, 537, this);
		drawer.drawImage(g,ImageUtil.getImage("image/popup/pop_help_on.png"), menuX, 607, this);
		drawer.drawImage(g,ImageUtil.getImage("image/popup/pop_exit_on.png"), menuX, 674, this);

		drawer.drawImage(g,ImageUtil.getImage("image/popup/pop_cursor.png"), rm.menuPos[menu_temp][0], rm.menuPos[menu_temp][1], this);
	}

	public void playBGM(int fileName){
		try{
			rm.sound[fileName].loop();
		}catch(Exception e){
			System.out.println(" error in playBgm,e="+e);
		}
	}

	public void stopBGM(int filename){
		try{
			rm.sound[filename].stop();
		}catch(Exception e){
			System.out.println(" error in StopBgm,e="+e);
		}
	}

	public void playSound(int index){
		try{
			rm.sound[index].play();
		}
		catch(Exception e){
			System.out.println(" error in playSound, snd idx="+index+",e="+e);
		}

	}

	public int _gKey;
	public void keyPressed(KeyEvent e)
	{
		// TODO Auto-generated method stub
		int keyCode = e.getKeyCode();

		switch(play_mode)
		{
		case TITLE_MODE:
			titleKeyPress(keyCode);
			break;
		case GAME_MODE:
			gameKeyPress(keyCode);
			break;
		}
	}

	public void titleKeyPress(int keyCode)
	{
		if(title_mode == TC_OUT) return;
		switch(title_mode)
		{
		case TC_LOAD:
			if (keyCode == KeyEvent.VK_ENTER)
			{
				if(loadBool)
				{
					title_mode = TC_SELECT;
				}
				else
				{
					title_mode = TC_WAIT;
				}
			}
			else if (keyCode == Reference.KEY_Blue)
			{
				old_title_mode = title_mode;
				title_mode = TC_QUIT;
			}
			else if (keyCode == Reference.KEY_Yellow)
			{
				old_title_mode = title_mode;
				title_mode = TC_HELP;
			}
			break;
		case TC_SELECT:
			if (keyCode == KeyEvent.VK_ENTER)
			{
				playSound(SND_KEY_SELECT);
				if (_cursor_idx==0) rm.setGameType(GAME_TYPE_1P);
				else if (_cursor_idx==1) rm.setGameType(GAME_TYPE_2P);
				okBool = true;
				selectbool = true;
			}
			else if (keyCode == KeyEvent.VK_LEFT || keyCode == KeyEvent.VK_RIGHT)
			{
				if(_cursor_idx==0){
					_cursor_idx=1;
					selectAni=0;
					curAni=0;
				}
				else{
					_cursor_idx=0;
					selectAni=6;
					curAni=0;
				}
				playSound(SND_KEY_MOVE);
			}
			else if (keyCode == Reference.KEY_Blue)
			{
				old_title_mode = title_mode;
				title_mode = TC_QUIT;
			}
			else if (keyCode == Reference.KEY_Yellow)
			{
				old_title_mode = title_mode;
				title_mode = TC_HELP;
			}
			break;
		case TC_QUIT:
			switch(keyCode)
			{
			case KeyEvent.VK_LEFT:
				popupCursor = 0;
				break;
			case KeyEvent.VK_RIGHT:
				popupCursor = 1;
				//				popupCursor = (popupCursor == 0 ? 1 : 0);
				break;
			case KeyEvent.VK_ENTER:
				if (popupCursor == 0){
					exit();
				}
				else
				{
					title_mode = old_title_mode;
					popupCursor = 0;
				}
				break;
			}
			break;
		case TC_HELP:
			switch(keyCode)
			{
			case KeyEvent.VK_LEFT:
				helpCnt--;
				if(helpCnt<0)helpCnt=0;
				break;
			case KeyEvent.VK_RIGHT:
				helpCnt++;
				if(helpCnt>1)helpCnt=1;
				break;
			case KeyEvent.VK_ENTER:
				helpCnt=0;
				title_mode = old_title_mode;
				break;
			}
			break;
		}
	}

	public void gameKeyPress(int keyCode)
	{
		_gKey = keyCode;

		if (popup_idx >= 0)
			return;

		switch(game_mode)
		{
		case GS_PLAY:
			if(bEnableKey == false)
			{
//				DEBUGF(" bEnableKey == false ");
				return;
			}

			if (keyCode == Reference.KEY_Blue)
			{
				setGameState(GS_MENU);
				playSound(SND_WARNING);
			}

			if (keyCode == KeyEvent.VK_LEFT){
				cursorX = (cursorX > 0) ? cursorX - 1 : Board.WIDTH - 1;
			}
			else if (keyCode == KeyEvent.VK_RIGHT){
				cursorX = (cursorX < Board.WIDTH - 1) ? cursorX + 1 : 0;
			}
			else if (keyCode == KeyEvent.VK_UP){
				cursorY = (cursorY > 0) ? cursorY - 1 : Board.HEIGHT - 1;
			}
			else if (keyCode == KeyEvent.VK_DOWN){
				cursorY = (cursorY < Board.HEIGHT - 1) ? cursorY + 1 : 0;
			}
			else if (keyCode == KeyEvent.VK_ENTER){
				putStones();
			}
			break;
		case GS_HELP:
			switch(keyCode)
			{
			case KeyEvent.VK_LEFT:
				helpCnt--;
				if(helpCnt<0)helpCnt=0;
				break;
			case KeyEvent.VK_RIGHT:
				helpCnt++;
				if(helpCnt>1)helpCnt=1;
				break;
			case KeyEvent.VK_ENTER:
				helpCnt=0;
				setGameState(GS_PLAY);
				break;
			}
			break;
		case GS_MENU:
			switch(keyCode)
			{
			case KeyEvent.VK_UP:
				menu_temp--;
				if(menu_temp<0) menu_temp = 0;
				playSound(SND_KEY_MOVE);
				break;
			case KeyEvent.VK_DOWN:
				menu_temp++;
				if(menu_temp>3) menu_temp = 3;
				playSound(SND_KEY_MOVE);
				break;
			case KeyEvent.VK_ENTER:
				playSound(SND_MENU);
				if(menu_temp==0)
				{
					setGameState(GS_PLAY);
					menu_temp=0;
				}
				else if(menu_temp==1)
				{
					game_mode = GS_TITLE;
					tempCnt = 0;
				}
				else if(menu_temp == 2)
				{
					setGameState(GS_HELP);
				}
				else
				{
					exit();
				}
				menu_temp=0;
				break;
			}
			break;
		}
	}

	public void keyReleased(KeyEvent e) {
		// TODO Auto-generated method stub
	}
	public void keyTyped(KeyEvent e) {
		// TODO Auto-generated method stub
	}

	public void displayIframe(ScreenManager iFrameChanger)
	{
		iFrameChanger.display(Reference.IFRAME_TITLE_BG);
	}
	public void goNextScene(String scenename)
	{
		forwardScene(scenename);
	}
	public void iframeChange(String name)
	{
		ScreenManager.getInstance().display(name);
	}


	public void gotoGame()
	{
		iframeChange("image/othello_game_bg.jpg");
		play_mode = GAME_MODE;
		gameInit();
	}

	public void gotoTitle()
	{
		rm.setGameType(0);
		stopBGM(SND_BGM);
		iframeChange("image/othello_title_bg.jpg");
		play_mode = TITLE_MODE;
		rm.setTitleMode(100);
		titleInit();
	}


	private void exit()
	{
		try
		{
			System.out.println(">>> Yahoo exit() start...");
			AppManager.getInstance().destroyApp();
		}
		catch (Exception e)
		{
			System.out.println(">>> Error Yahoo exit() : " + e.getMessage());
		}
	}


	public void DEBUGF(Object obj){
		//rm.DEBUGF(obj);
		//send_memory("[othellox2let]"+obj.toString());
		System.out.println(obj);
	}
	
	
//	private void setClip(Graphics g, int x, int y, int width, int height)
//    {
//    	g.setClip((int)(x*0.6666),(int)(y*0.6666),(int)(width*0.6666),(int)(height*0.6666));
//    }
//    
//    private void clearRect(Graphics g, int x, int y, int width, int height)
//    {
//    	g.clearRect((int)(x*0.6666), (int)(y*0.6666), (int)(width*0.6666), (int)(height*0.6666));
//    }
//    
//    private void translate(Graphics g, int x, int y)
//    {
//    	g.translate((int)(x*0.6666), (int)(y*0.6666));
//    }
//    
//    private void drawImage(Graphics g, Image img, int x, int y, ImageObserver observer)
//    {
//    	g.drawImage(img,(int)(x*0.6666),(int)(y*0.6666),observer);
//    }
//    
//    private void drawImage(Graphics g, Image img, int x, int y, int w, int h, ImageObserver observer)
//    {
//    	g.drawImage(img,(int)(x*0.6666),(int)(y*0.6666), (int)(w*0.6666),(int)(h*0.6666), observer);
//    }
//    
//    private void drawImage(Graphics g, Image img, int dx1, int dy1, int dx2, int dy2, int sx1, int sy1, int sx2, int sy2, ImageObserver observer)
//    {
//    	g.drawImage(img,(int)(dx1*0.6666), (int)(dy1*0.6666), (int)(dx2*0.6666), (int)(dy2*0.6666), (int)(sx1*0.6666), (int)(sy1*0.6666), (int)(sx2*0.6666), (int)(sy2*0.6666) ,observer);
//    }
//
//    private void drawString(Graphics g, String str, int x, int y)
//    {
//    	g.drawString(str, (int)(x*0.6666), (int)(y*0.6666));
//    }
//    
//    private void fillRect(Graphics g, int x, int y, int width, int height)
//    {
//    	g.fillRect((int)(x*0.6666), (int)(y*0.6666), (int)(width*0.6666), (int)(height*0.6666));
//    }
	
	
	
}
