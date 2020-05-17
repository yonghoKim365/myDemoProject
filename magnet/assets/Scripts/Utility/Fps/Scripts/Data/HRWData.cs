/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 HRWData.cs                    		       *
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
    /// Shows additional hardware information.
    /// </summary>
    [System.Serializable]
    public class HRWData : BaseData
    {
        [SerializeField]
        private bool showCPU = true;

        [SerializeField]
        private bool showRAM = true;

        [SerializeField]
        private bool showGPU = true;

        [SerializeField]
        private bool showGDV = true;
        

        /// <summary>
        /// CPU info string data.
        /// </summary>
        public string DataCPU { get; private set; }

        /// <summary>
        /// RAM info string data.
        /// </summary>
        public string DataRAM { get; private set; }

        /// <summary>
        /// GPU info string data.
        /// </summary>
        public string DataGPU { get; private set; }

        /// <summary>
        /// Graphics API info string data.
        /// </summary>
        public string DataGDV { get; private set; }

        
        /// <summary>
        /// Shows CPU info ( model name & cores count ).
        /// </summary>
        public bool ShowCPU
        {
            get { return showCPU; }
            set
            {
                if( showCPU == value ) return;
                showCPU = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Shows system memory size.
        /// </summary>
        public bool ShowRAM
        {
            get { return showRAM; }
            set
            {
                if( showRAM == value ) return;
                showRAM = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Shows GPU info ( model name, memory size, shader model, max texture size ).
        /// </summary>
        public bool ShowGPU
        {
            get { return showGPU; }
            set
            {
                if( showGPU == value ) return;
                showGPU = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Shows used render API on this GPU.
        /// </summary>
        public bool ShowGDV
        {
            get { return showGDV; }
            set
            {
                if( showGDV == value ) return;
                showGDV = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        
        // GetHardwareInfo
        internal string GetHardwareInfo()
        {
            if( !outdated ) return dataInfo;

            bool needNewLine = false;

            dataText.Length = 0;
            dataText.Append( currentColor );

            // ShowCPU
            if( showCPU )
            {
                DataCPU = "CPU: " + SystemInfo.processorType + " [cores: " + SystemInfo.processorCount.ToString() + "]";
                dataText.Append( DataCPU );
                needNewLine = true;
            }
            else DataCPU = string.Empty;

            // ShowRAM
            if( showRAM )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );
                DataRAM = "RAM: " + SystemInfo.systemMemorySize.ToString() + " mb";
                dataText.Append( DataRAM );
                needNewLine = true;
            }
            else DataRAM = string.Empty;

            // ShowGPU
            if( showGPU )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );

                DataGPU = "GPU: " + SystemInfo.graphicsDeviceName +
                          " [vram: " + SystemInfo.graphicsMemorySize.ToString() +
                          " mb, sm: " + SystemInfo.graphicsShaderLevel.ToString().Insert( 1, "." ) +
                          ", mts: " + SystemInfo.maxTextureSize.ToString() + " pix]";
                dataText.Append( DataGPU );
                needNewLine = true;
            }
            else DataGPU = string.Empty;

            // ShowGDV
            if( showGDV )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );
                DataGDV = "GDV: " + SystemInfo.graphicsDeviceVersion;
                dataText.Append( DataGDV );
            }
            else DataGDV = string.Empty;

            dataText.Append( TextLabel.ADD_ENDLINE );
            dataInfo = dataText.ToString();
            outdated = false;

            return dataInfo;
        }

        // ClearData
        internal override void ClearData()
        {
            base.ClearData();
            DataCPU = string.Empty;
            DataRAM = string.Empty;
            DataGPU = string.Empty;
            DataGDV = string.Empty;
        }
    }
}