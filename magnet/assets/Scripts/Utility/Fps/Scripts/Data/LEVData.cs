/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 LEVData.cs                  		       *
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
using System.Diagnostics;
using SmartFPSMeter.labels;

namespace SmartFPSMeter.Data
{
    /// <summary>
    /// Shows play time & scene name.
    /// </summary>
    [System.Serializable]
    public class LEVData : BaseData
    {
        private bool showLN = true;
        private bool showPT = true;

        private Stopwatch stopWatch = new Stopwatch();

        private float lastInterval = 0f;


        /// <summary>
        /// Level name string data.
        /// </summary>
        public string DataLN { get; private set; }

        /// <summary>
        /// Play time info string data.
        /// </summary>
        public string DataPT { get; private set; }


        /// <summary>
        /// Shows loaded level ( scene ) name.
        /// </summary>
        public bool ShowLN
        {
            get { return showLN; }
            set 
            {
                if( showLN == value ) return;
                showLN = value; 
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see total play time since level load.
        /// </summary>
        public bool ShowPT
        {
            get { return showPT; }
            set 
            {
                if( showPT == value ) return;
                showPT = value; 
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }


        // DataAwake
        internal override void DataAwake()
        {
            stopWatch.Start();
            base.DataAwake();            
        }

        // CalculatePlayTime()
        internal void CalculatePlayTime()
        {
            if( stopWatch.Elapsed.Seconds != lastInterval )
            {
                lastInterval = stopWatch.Elapsed.Seconds;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }           
        }

        //Get Screen Info
        internal string GetLevelInfo()
        {
            if( !outdated ) return dataInfo;
                        
            bool needNewLine = false;

            dataText.Length = 0;
            dataText.Append( currentColor );

            if( showLN )
            {
                DataLN = "Level: " + Application.loadedLevelName;
                dataText.Append( DataLN );
                needNewLine = true;
            }
            else DataLN = string.Empty;

            if( showPT )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );
                DataPT = string.Format( "Time: {0:00}:{1:00}:{2:00}", stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds );
                dataText.Append( DataPT );
            }
            else DataPT = string.Empty;

            dataText.Append( TextLabel.ADD_ENDLINE );
            dataInfo = dataText.ToString();
            outdated = false;

            return dataInfo;
        }

        // ClearData
        internal override void ClearData()
        {
            stopWatch.Stop();
            base.ClearData();
            DataLN = string.Empty;
            DataPT = string.Empty;
        }
    }
}