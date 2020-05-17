/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 FPSData.cs                   		       *
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
using SmartFPSMeter.labels;

namespace SmartFPSMeter.Data
{
    /// <summary>
    /// Shows frames per second & milliseconds data.
    /// </summary>
    [System.Serializable]
    public class FPSData : BaseData
    {
        [SerializeField]
        private float updateInterval = 0.45f;

        [SerializeField]
        private int warningValue = 35;

        [SerializeField]
        private int dangerValue = 25;

        [SerializeField]
        private bool showMS = true;

        [SerializeField]
        private bool showFPS = true;

        [SerializeField]
        private bool showMinMax = true;

        [SerializeField]
        private Color warningColor = new Color32( 238, 238, 0, 255 );
        private string currentWarningColor = string.Empty;

        [SerializeField]
        private Color dangerColor = new Color32( 238, 0, 0, 255 );
        private string currentDangerColor = string.Empty;

        // CountFPS
        private float lastFpsInterval = 0f;
        private float lastMinMaxInterval = 0f;
        private float frames = 0f;
        private float fps = 0f;
        private float minfps = 0f;
        private float maxfps = 0f;
        private float ms = 0f;
        private float minms = 0f;
        private float maxms = 0f;
        

        /// <summary>
        /// Current FPS Сondition is given by color ( Fine, Warning, Danger ).
        /// Сan be used as a hint to lower quality level.
        /// </summary>
        public Сonditions FPSСondition { get; private set; }

        /// <summary>
        /// Milliseconds string data.
        /// </summary>
        public string DataMS { get; private set; }
        private string currentMS = null;
        private string minmaxMS = null;

        /// <summary>
        /// Frames Per Second string data.
        /// </summary>
        public string DataFPS { get; private set; }
        private string currentFPS = null;
        private string minmaxFPS = null;

              
        /// <summary>
        /// Calculation fps & ms data value update interval.
        /// </summary>
        public float UpdateInterval
        {
            get { return updateInterval; }
            set
            {
                if( updateInterval == value ) return;
                updateInterval = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Sets warning value below which the color changes.
        /// If the value is above the color is changed to Fine.
        /// </summary>
        public int WarningValue
        {
            get { return warningValue; }
            set
            {
                if( warningValue == value ) return;
                warningValue = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Sets danger value below which the color changes.
        /// If the value is above the color is changed to Warning.
        /// </summary>
        public int DangerValue
        {
            get { return dangerValue; }
            set
            {
                if( dangerValue == value ) return;
                dangerValue = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see milliseconds data.
        /// </summary>
        public bool ShowMS
        {
            get { return showMS; }
            set
            {
                if( showMS == value ) return;
                showMS = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see frames per second data.
        /// </summary>
        public bool ShowFPS
        {
            get { return showFPS; }
            set
            {
                if( showFPS == value ) return;
                showFPS = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to minimum and maximum FPS readings since game start
        /// </summary>
        public bool ShowMinMax
        {
            get { return showMinMax; }
            set
            {
                if( showMinMax == value ) return;
                showMinMax = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        
        /// <summary>
        /// Text color for Warning fps value.
        /// </summary>
        public Color WarningColor
        {
            get { return warningColor; }
            set
            {
                if( warningColor == value ) return;
                warningColor = value;
                currentWarningColor = TextLabel.ColorToString( warningColor );
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Text color for Danger fps value.
        /// </summary>
        public Color DangerColor
        {
            get { return dangerColor; }
            set
            {
                if( dangerColor == value ) return;
                dangerColor = value;
                currentDangerColor = TextLabel.ColorToString( dangerColor );
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }


        // DataAwake
        internal override void DataAwake()
        {
            currentWarningColor = TextLabel.ColorToString( warningColor );
            currentDangerColor = TextLabel.ColorToString( dangerColor );
            base.DataAwake();
        }

        // CalculateFPSandMS
        internal void CalculateFPSandMS()
        {
            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if( timeNow > lastFpsInterval + updateInterval )
            {
                fps = frames / ( timeNow - lastFpsInterval );
                if( showMS ) ms = 1000f / fps;
                if( showMinMax )
                {
                    if( showFPS )
                    {
                        minfps = Mathf.Min( minfps, fps );
                        maxfps = Mathf.Max( maxfps, fps );
                    }

                    if( showMS )
                    {
                        minms = Mathf.Min( minms, ms );
                        maxms = Mathf.Max( maxms, ms );
                    }
                }
                frames = 0f;
                lastFpsInterval = timeNow;

                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }


            if( showMinMax )
            {
                if( minfps == 0f || maxfps == 0f || minms == 0f || maxms == 0f )
                {
                    minfps = fps;
                    maxfps = fps;
                    minms = ms;
                    maxms = ms;
                }

                if( timeNow > lastMinMaxInterval + updateInterval * 5.5f )
                {
                    minfps = fps;
                    maxfps = fps;
                    minms = ms;
                    maxms = ms;
                    lastMinMaxInterval = timeNow;
                }
            }
        }

        // GetFPSData
        internal string GetFPSData()
        {
            if( !outdated ) return dataInfo;

            bool needNewLine = false;

            dataText.Length = 0;

            if( fps > warningValue )
            {
                dataText.Append( currentColor );
                if( FPSСondition != Сonditions.Fine ) FPSСondition = Сonditions.Fine;
            }
            else if( fps < warningValue && fps > dangerValue )
            {
                dataText.Append( currentWarningColor );
                if( FPSСondition != Сonditions.Warning ) FPSСondition = Сonditions.Warning;
            }
            else if( fps < dangerValue )
            {
                dataText.Append( currentDangerColor );
                if( FPSСondition != Сonditions.Danger ) FPSСondition = Сonditions.Danger;
            }


            // ShowMS
            if( showMS )
            {
                currentMS = "MS: " + ms.ToString( "f1" );
                dataText.Append( currentMS );
                if( showMinMax )
                {
                    minmaxMS = " [" + minms.ToString( "f0" ) + ".." + maxms.ToString( "f0" ) + "]";                    
                    dataText.Append( minmaxMS );
                    DataMS = currentMS + minmaxMS;
                }
                else DataMS = currentMS;
                needNewLine = true;
            }
            else DataMS = string.Empty;

            // ShowFPS
            if( showFPS )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );

                currentFPS = "FPS: " + fps.ToString( "f1" );
                dataText.Append( currentFPS );
                if( showMinMax )
                {
                    minmaxFPS = " [" + minfps.ToString( "f0" ) + ".." + maxfps.ToString( "f0" ) + "]";
                    dataText.Append( minmaxFPS );
                    DataFPS = currentFPS + minmaxFPS;
                }
                else DataFPS = currentFPS;
            }
            else DataFPS = string.Empty;

            dataText.Append( TextLabel.ADD_ENDLINE );
            dataInfo = dataText.ToString();
            outdated = false;

            return dataInfo;
        }

        // ClearData
        internal override void ClearData()
        {
            base.ClearData();
            DataMS = string.Empty;
            DataFPS = string.Empty;
        }
    }
}