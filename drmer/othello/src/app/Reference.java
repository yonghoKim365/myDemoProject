package app;

import java.awt.Color;

import org.havi.ui.event.HRcCapabilities;
import org.havi.ui.event.HRcEvent;

public class Reference
{
	/**
	 * Temp variable to run program on Emulator
	 */
	public static final int PLAY_MODE = 0; 	//0 = box
											//1 = emul
	
	public static final int VER_PLAY = 0; 	//0 = ps3/LG/OPPO/pioneer
											//1 = sony
	
	public static int KEY_Red;
	public static int KEY_Green;
	public static int KEY_Yellow;
	public static int KEY_Blue;
	static{ assignColorKeys(); }

	/**
	 *  Status of MenuIcon
	 */
	public static final int MENU_STATUS_SELECTED = 1;
	public static final int MENU_STATUS_NOT_SELECTED = 0;
	public static final int MENU_STATUS_DIMMED = -1;

	public static final String SCENE_LOAD_CANVAS = "scene.LoadCanvas";
	public static final String SCENE_GAME_CANVAS = "scene.MainCanvas";

	/**
	 * iframe path
	 */
	public static final String IFRAME_TITLE_BG = "image/othello_title_bg.jpg";
	public static final String IFRAME_GAME_BG  = "image/othello_game_bg.jpg";

	/**
	 * "Y/N" Result
	 */
	public static final String YES = "Y";
	public static final String NO = "N";

	public static final String TRUE = "TRUE";
	public static final String FALSE = "FALSE";

	public static void assignColorKeys()
	{
		// VK_COLORED_KEY_0 부터 순서대로 해당 키에 매핑된 칼라에 해당하는 값을 알아낸다.
		int red;
		int green;
		int blue;
		int yellow;

		if(PLAY_MODE==1)
		{
			red = 0;
			green = 1;
			blue = 3;
			yellow = 2;
		}
		else
		{
			float[] hues = new float[4];

			for(int i=0; i<hues.length; i++)
				hues[i] = getKeyHue(i);
			red = getClosest(hues, Color.red);
			hues[red] = 1000f;
			green = getClosest(hues, Color.green);
			hues[green] = 1000f;
			blue = getClosest(hues, Color.blue);
			hues[blue] = 1000f;
			yellow = getClosest(hues, Color.yellow);
		}
		// 인자로 주어진 칼라와 같은 값이 무엇인지 알아낸다.
		// 키코드로 전환.
		KEY_Red = getRealKeyCode(red);
		KEY_Green = getRealKeyCode(green);
		KEY_Blue = getRealKeyCode(blue);
		KEY_Yellow = getRealKeyCode(yellow);
	}


	private static int getRealKeyCode(int key){
		switch (key)
		{
		case 0:
			key = HRcEvent.VK_COLORED_KEY_0;	break;
		case 1:
			key = HRcEvent.VK_COLORED_KEY_1;	break;
		case 2:
			key = HRcEvent.VK_COLORED_KEY_2;	break;
		case 3:
			key = HRcEvent.VK_COLORED_KEY_3;	break;
		}
		return key;
	}

	private static int getClosest(float[] hues, Color goal){
		float goalHue = getHue(goal);
		int result = -1;
		float resultDiff = 100f;
		for (int i=0; i<hues.length; i++){
			float diff = Math.abs(goalHue - hues[i]);
			if (diff > 0.5f && diff <= 1f){
				diff = 1f - diff;
			}
			if (resultDiff >= diff){
				result = i;
				resultDiff = diff;
			}
		}
		return result;
	}

	private static float getKeyHue(int key){
		key = key + HRcEvent.VK_COLORED_KEY_0;
		Color c = HRcCapabilities.getRepresentation(key).getColor();
		return getHue(c);
	}

	private static float getHue(Color c){
		float[] hsb = Color.RGBtoHSB(c.getRed(), c.getGreen(), c.getBlue(), null);
		return hsb[0] - ((float) Math.floor(hsb[0]));
	}
}
