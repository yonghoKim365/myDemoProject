using UnityEngine;
using System.Collections;

public class UISoundSetting : MonoBehaviour
{

    public eUISfx Sound = eUISfx.UI_button_open_close;

    void OnPress(bool isPress)
    {
	   if (!isPress)
            return;
       
        SoundManager.instance.PlaySfxSound(Sound, false);
    }
    
}
