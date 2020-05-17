/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 PrefabCreator.cs              		       *
 * 														   *
 * Copyright(c): Victor Klepikov						   *
 * Support: 	 http://bit.ly/vk-Support				   *
 * 														   *
 * mySite:       http://vkdemos.ucoz.org				   *
 * myAssets:     http://bit.ly/VictorKlepikovUnityAssets   *
 * myTwitter:	 http://twitter.com/VictorKlepikov		   *
 * myFacebook:	 http://www.facebook.com/vikle4 		   *
 * 														   *
 ***********************************************************/



using UnityEngine;
using UnityEditor;

namespace SmartFPSMeter.Data
{
    public class PrefabCreator : Editor
    {
        private const string mainGOName = "_SmartFpsMeter";
        private const string menuAbbrev = "GameObject/Create Other";

        private static GameObject meterGO = null;


        // CreateJoysManager 
        [MenuItem( menuAbbrev + "/Smart FPS Meter" )]
        private static void CreateMeter()
        {
            meterGO = new GameObject( mainGOName, typeof( MainMeter ) );
            meterGO.transform.position = Vector3.zero;

            MainMeter tmpMeter = meterGO.GetComponent<MainMeter>();

            tmpMeter.fpsData.Anchor = TextAnchor.UpperRight;
            tmpMeter.memData.Anchor = TextAnchor.LowerRight;
            tmpMeter.scrData.Anchor = TextAnchor.UpperLeft;
            tmpMeter.hrwData.Anchor = TextAnchor.LowerLeft;
            tmpMeter.levData.Anchor = TextAnchor.UpperCenter;

            tmpMeter.fpsData.MyColor = new Color32( 0, 238, 0, 255 );
            tmpMeter.memData.MyColor = new Color32( 0, 238, 238, 255 );
            tmpMeter.scrData.MyColor = new Color32( 238, 238, 0, 255 );
            tmpMeter.hrwData.MyColor = new Color32( 238, 100, 0, 255 );
            tmpMeter.levData.MyColor = new Color32( 180, 180, 180, 255 );

            tmpMeter.MeterSetup();            
        }

        [MenuItem( menuAbbrev + "/Smart FPS Meter", true )]
        private static bool ValidateCreateMeter()
        {
            return !FindObjectOfType<MainMeter>();
        }
    }
}