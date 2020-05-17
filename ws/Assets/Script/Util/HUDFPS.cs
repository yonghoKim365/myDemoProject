using UnityEngine;
using System.Collections;

public class HUDFPS : MonoBehaviour 
{

// Attach this to a GUIText to make a frames/second indicator.
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// correct overall FPS even if the interval renders something like
// 5.5 frames.
 
public  float updateInterval = 0.5F;
 
private float accum   = 0; // FPS accumulated over the interval
private int   frames  = 0; // Frames drawn over the interval
private float timeleft; // Left time for current interval
private string fpsStr = "";
void Start()
{
    if( !guiText && false )
    {
        Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
        enabled = false;
        return;
    }
    timeleft = updateInterval;  
}
 
void Update()
{
    timeleft -= Time.deltaTime;
    accum += Time.timeScale/Time.deltaTime;
    ++frames;
    
    // Interval ended - update GUI text and start new interval
    if( timeleft <= 0.0 )
    {
        // display two fractional digits (f2 format)
    float fps = accum/frames;
    string format = System.String.Format("{0:F2} FPS",fps);
    //guiText.text = format;
	fpsStr = format;
    timeleft = updateInterval;
        accum = 0.0F;
        frames = 0;

#if UNITY_EDITOR
			if(Input.GetMouseButton(0))
			{
				Vector2 v = Util.screenPositionWithCamViewRect(Input.mousePosition);
				fpsStr += "\n" + "x :" + (int)v.x + "  y: " + (int)v.y;
			}
#endif

    }
}
	void OnGUI()
	{
		GUI.color = Color.red;
		GUI.skin.box.fontSize = 17;
		//GUI.Box(new Rect(20,400,150,30),fpsStr);
		GUI.Label(new Rect(20,0,150,60), fpsStr);
	}
}