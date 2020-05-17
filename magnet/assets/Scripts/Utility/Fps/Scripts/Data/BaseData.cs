/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 BaseData.cs                 		       *
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
    /// Сonditions for meters, yet using only fps meter
    /// </summary>
    public enum Сonditions : byte
    {
        Fine,
        Warning,
        Danger
    }

    /// <summary>
    /// Bace class for all meters.
    /// </summary>
    public class BaseData
    {
        [SerializeField]
        protected bool enabled = true;

        [SerializeField]
        protected TextAnchor anchor = TextAnchor.UpperRight;

        [SerializeField]
        protected Color myColor;
        protected string currentColor = string.Empty;

        protected System.Text.StringBuilder dataText = new System.Text.StringBuilder();
        protected string dataInfo = string.Empty;
        protected bool outdated = false;


#if UNITY_EDITOR
        [HideInInspector]
        public bool foldout = true;
#endif
        

        /// <summary>
        /// Enables or disables meter with immediate label refresh.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if( enabled == value ) return;
                enabled = value;

                if( enabled )
                {
                    DataAwake();
                }
                else
                {
                    MainMeter.Instance.ClearData();
                    ClearData();
                }
            }
        }

        /// <summary>
        /// Sets the memory data label position.
        /// </summary>
        public TextAnchor Anchor
        {
            get { return anchor; }
            set
            {
                if( anchor == value ) return;
                anchor = value;
                outdated = true;
                MainMeter.Instance.SetAllText( true );
            }
        }

        /// <summary>
        /// Text color for this meter.
        /// </summary>
        public Color MyColor
        {
            get { return myColor; }
            set
            {
                if( myColor == value ) return;
                myColor = value;
                currentColor = TextLabel.ColorToString( myColor );
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }


        // DataAwake
        internal virtual void DataAwake()
        {
            currentColor = TextLabel.ColorToString( myColor );
            dataInfo = string.Empty;
            outdated = true;
            MainMeter.Instance.SetAllText( true );
        }

        // ClearData
        internal virtual void ClearData()
        {
            dataText.Length = 0;
        }
    }
}