/*
 * ResourceManager.java
 * 占쌜쇽옙占쏙옙 占쏙옙짜: 2005. 5. 11.
 */
package component;

import org.havi.ui.HSound;

/**
 * @author zen
 */
public class ResourceManager
{
	private static ResourceManager instance = null;
	
	public HSound sound[] = new HSound[13];
	public int _gameType = 0;
	public int title_mode = 0;

	public static ResourceManager getInstance() {
		if (instance == null)
			instance = new ResourceManager();
		return instance;
	}

	public void setGameType(int t)
	{
//		System.out.println("rootCon.setGameType, type="+t);
		_gameType = t;
	}

	public int getGameType()
	{
		return _gameType;
	}

	public void setTitleMode(int mode)
	{
		title_mode = mode;
	}

	public int getTitleMode()
	{
		return title_mode;
	}

	public int load_gage_xy[][] =
	{
			{101, 2},
			{140, 8},
			{169, 31},
			{192, 65},
			{197, 102},
			{191, 141},
			{169, 172},
			{136, 192},
			{100, 198},
			{64, 192},
			{31, 171},
			{11, 141},
			{2, 102},
			{9, 66},
			{31, 33},
			{63, 12}
	};

	public int loadfade[][] =
	{
			{0, 192, 5},
			{19, 154, 4},
			{38, 116, 3},
			{57, 76, 2},
			{76, 38, 1},
			{76, 38, 0},
	};

	public int start_fade[][] =
	{
			{0, 192, 10},
			{19, 154, 8},
			{38, 116, 6},
			{57, 76, 4},
			{76, 38, 2},
			{76, 38, 0},
	};

	public int quit_fade[][] =
	{
			{76, 38, 0},
			{76, 38, 2},
			{57, 76, 4},
			{38, 116, 6},
			{19, 154, 8},
			{0, 192, 10},
	};

	public int titleAnyData[][] =
	{
			{531,-113,851,441},
			{557,8,765,397},
			{619,106,681,353},
			{662,189,595,309}
	};

	public int selectAniData[][] =
	{
			{799, 319, 0},
			{847, 223, 0},
			{911, 95,  0},
			{952, 13,  0},
			{911, 95,  1},
			{847, 223, 1},
			{799, 319, 1},
	};

	public int curAniData[][] =
	{
			{719, 1139},
			{722, 1136},
			{725, 1133},
			{728, 1130},
			{725, 1133},
			{722, 1136},
			{719, 1139},
	};

	public int cardData[] ={1080, 795, 628, 538};

	public int readyData[][] =
	{
			{740,567,100,22,40},
			{690,555,200,46,40},
			{628,541,325,74,60},
			{573,528,435,100,80},
			{503,512,575,132,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,521,500,114,100},
			{540,533,500,114,80},
			{540,753,500,114,60},
			{540,880,500,114,40},
			{540,1025,500,114,20},
			{540,1080,500,114,0},
	};

	public int startData[][] =
	{
			{530,9},
			{530,163},
			{530,346},
			{530,501},
			{530,473},
			{530,450},
			{530,445},
			{530,459},
			{530,480},
			{530,517},
			{530,517},
			{530,517},
			{530,517},
	};

	public int menuPos[][] =
	{
			{871, 418},
			{944, 488},
			{795, 553},
			{946, 620},
	};

	//TODO : destroy()
	public void dispose()
	{
		if(load_gage_xy != null) load_gage_xy = null;
		if(loadfade != null) loadfade = null;
		if(start_fade != null) start_fade = null;

		if(quit_fade != null) quit_fade = null;
		if(titleAnyData != null) titleAnyData = null;

		if(selectAniData != null) selectAniData = null;
		if(curAniData != null) curAniData = null;

		if(cardData != null) cardData = null;
 		if(readyData != null) readyData = null;

		if(startData != null) startData = null;
		if(menuPos != null) menuPos = null;

		if(sound != null)
		{
			for(int i = 0; i < sound.length; i++)
			{
				sound[i].stop();
				sound[i].dispose();
				sound[i] = null;
			}
			sound = null;
		}

		if(instance != null)
		{
			instance = null;
		}
	}
}
/*-> End Of File <------------------------------------------------------------*/

