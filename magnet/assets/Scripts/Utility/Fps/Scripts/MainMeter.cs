/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 MainMeter.cs               		       *
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
using System.Text;
using SmartFPSMeter.Data;
using SmartFPSMeter.labels;

namespace SmartFPSMeter
{
    /// <summary> 
    /// Enable      Allows to see data on screen and runs calculation.
    /// Background  Allows to read all enabled meters data keeping them hidden.
    /// Disable     Removes all meters, stop calculation and clear all data.
    /// </summary>
    public enum MeterModes : byte 
    {
        Enable, 
        Background, 
        Disable 
    }

    /// <summary>
    /// Main class of this asset is used to control all values.    
    /// </summary>    
    public class MainMeter : MonoBehaviour
    {
        [SerializeField]
        private MeterModes meterMode = MeterModes.Enable;

        [SerializeField]
        private int layerIndex = 0;

        [SerializeField]
        private Font labelsFont = null;

        [SerializeField]
        private int fontSize = 12;
        private int newFontSize = 0;

        /// <summary>
        /// 
        /// </summary>
        public TextAnchor targetCorner = TextAnchor.UpperLeft;

        /// <summary>
        /// Final string of FPS Data results ( data & color ).
        /// </summary>
        public string FPSresult { get; private set; }

        /// <summary>
        /// Final string of Memory Data results ( data & color ).
        /// </summary>
        public string MEMresult { get; private set; }

        /// <summary>
        /// Final string of Hardware Info results ( data & color ).
        /// </summary>
        public string HRWresult { get; private set; }

        /// <summary>
        /// Final string of Screen Data results ( data & color ).
        /// </summary>
        public string SCRresult { get; private set; }

        /// <summary>
        /// Final string of Level Info results ( data & color ).
        /// </summary>
        public string LEVresult { get; private set; }        

        /// <summary>
        /// Controlling all shows and calculation operations for Frames Per Second and Milliseconds meter.
        /// </summary>
        public FPSData fpsData = new FPSData();

        /// <summary>
        /// Mono, total Allocated & total Reserved memory meter.
        /// </summary>
        public MEMData memData = new MEMData();

        /// <summary>
        /// Additive hardware info.
        /// Shows CPU ( model name & cores count ).
        /// Shows RAM ( System memory size ).
        /// Shows GPU ( model name, memory size, shader model, max texture size ).
        /// Shows GDV ( used render API on this GPU ).
        /// </summary>
        public HRWData hrwData = new HRWData();

        /// <summary>
        ///  Shows screen resolution ( window size & dpi ).
        ///  Shows current quality level.
        /// </summary>
        public mfmSCRData scrData = new mfmSCRData();

        /// <summary>
        /// Shows current level ( scene ) name.
        /// Shows total play time since level load.
        /// </summary>
        public LEVData levData = new LEVData();

        [SerializeField]
        private TextLabel[] textLabels = null;
        private TextLabel tempLabel = null;

        [SerializeField]
        private int anchorsCount = 0;

        private bool outdated = false;

        // API Setup
        private static MainMeter instance = null;

        /// <summary>
        /// Allows to control MainMeter from code.
        /// </summary>
        public static MainMeter Instance
        {
            get
            {
                if( !instance )
                {
                    instance = FindObjectOfType<MainMeter>();
                    if( !instance ) Debug.LogError( "ERROR: Component [MainMeter] not found!" ); 
                }
                return instance;
            }
        }

#if UNITY_EDITOR
        [HideInInspector]
        public bool foldout = true;
#endif

        /// <summary>
        /// Sets the working mode.
        /// </summary>
        public MeterModes MeterMode
        {
            get { return meterMode; }
            set
            {
                if( meterMode == value ) return;
                meterMode = value;
                if( !Application.isPlaying ) return;
                SwitchModes();
            }
        }        

        /// <summary>
        /// Sets the Layer for Labels
        /// </summary>
        public int LayerIndex
        {
            get { return layerIndex; }
            set
            {
                if( layerIndex == value ) return;
                layerIndex = value;
                for( int cnt = 0; cnt < anchorsCount; cnt++ )
                    textLabels[ cnt ].SetLayer( layerIndex );
            }
        }        

        /// <summary>
        /// Font to render labels with.
        /// </summary>
        public Font LabelsFont
        {
            get { return labelsFont; }
            set
            {
                if( labelsFont == value ) return;                
                labelsFont = value;
                for( int cnt = 0; cnt < anchorsCount; cnt++ )
                    textLabels[ cnt ].SetLabelFont( labelsFont );
                SetAllText( true );
            }
        }

        /// <summary>
        /// The font size to use ( adapts to the screen resolution ).
        /// </summary>
        public int FontSize
        {
            get { return fontSize; }
            set
            {
                if( fontSize == value ) return;
                fontSize = value;
                if( !Application.isPlaying ) return;
                outdated = true;
                SetAllText( true );
            }
        }        
        
        /// <summary>
        /// Sets & return LineSpacing value for selected corner.
        /// </summary>
        public float LabelLineSpacing
        {
            get { return textLabels[ ( byte )targetCorner ].lineSpacing; }
            set
            {
                if( textLabels[ ( byte )targetCorner ].lineSpacing == value ) return;
                textLabels[ ( byte )targetCorner ].lineSpacing = value;
                SetAllText( true );
            }
        }
        
        /// <summary>
        /// Sets & return OffsetX value for selected corner.
        /// </summary>
        public float LabelOffsetX
        {
            get { return textLabels[ ( byte )targetCorner ].offsetX; }
            set
            {
                if( textLabels[ ( byte )targetCorner ].offsetX == value ) return;
                textLabels[ ( byte )targetCorner ].offsetX = value;
                SetAllText( true );
            }
        }

        /// <summary>
        /// Sets & return OffsetY value for selected corner.
        /// </summary>
        public float LabelOffsetY
        {
            get { return textLabels[ ( byte )targetCorner ].offsetY; }
            set
            {
                if( textLabels[ ( byte )targetCorner ].offsetY == value ) return;
                textLabels[ ( byte )targetCorner ].offsetY = value;
                SetAllText( true );
            }
        }
                
		GUIStyle largeFont;
        // Awake
        void Awake()
        {
			largeFont = new GUIStyle();
			largeFont.fontSize = 32;
            if( correctlyPlace )
            {
                instance = this;

                if( this.enabled ) SwitchModes();
                else MeterMode = MeterModes.Disable;

                outdated = true;
            }
        }

        // correctlyPlace
        private bool correctlyPlace
        {
            get
            {
                int metersNumber = FindObjectsOfType<MainMeter>().Length;
                if( metersNumber > 1 )
                {
                    this.enabled = false;
                    MeterMode = MeterModes.Disable;
                    Debug.LogError( "ERROR: Smart FPS Meter is incorrectly placed in scene! \nThere are " + metersNumber + " main meters in the scene." );

                    for( int cnt = 0; cnt < anchorsCount; cnt++ )
                    {
                        textLabels[ cnt ].SetFontSize( ( ( int )( 0.01f * scrData.screenWidth ) ) );
                        textLabels[ cnt ].AddText( "ERROR: Smart FPS Meter is incorrectly placed in scene! \nThere are " + metersNumber + " main meters in the scene." );
                        textLabels[ cnt ].UpdateText();
                    }
                    return false;
                }
                return true;
            }
        }

        // MeterSetup
        public void MeterSetup()
        {
            anchorsCount = System.Enum.GetNames( typeof( TextAnchor ) ).Length;
            textLabels = new TextLabel[ anchorsCount ];

            for( int cnt = 0; cnt < anchorsCount; cnt++ )
            {
                textLabels[ cnt ] = new TextLabel();
                textLabels[ cnt ].LabelSetup( ( TextAnchor )cnt, gameObject );
            }            

            outdated = true;


        }

        // Update
        void Update()
        {
            if( meterMode != MeterModes.Disable )
            {
				//Debug.Log("111111");
                if( fpsData.Enabled ) fpsData.CalculateFPSandMS();
                if( memData.Enabled ) memData.CalculateMEM();
                if( scrData.Enabled ) scrData.CalculateSCR();
                if( levData.Enabled ) levData.CalculatePlayTime();

                if( outdated )
                {
                    CalculateFontSize();
                    SetAllText( true );
                    outdated = false;
                }
            }
        }

        // OnEnable
        void OnEnable()
        {
            fpsData.DataAwake();
            memData.DataAwake();
            hrwData.DataAwake();
            scrData.DataAwake();
            levData.DataAwake();

            if( meterMode == MeterModes.Background ) return;

            MeterMode = MeterModes.Enable;
        }

        // OnDisable
        void OnDisable()
        {
            for( int cnt = 0; cnt < anchorsCount; cnt++ )
                textLabels[ cnt ].ClearText();            

            fpsData.ClearData();
            memData.ClearData();
            hrwData.ClearData();
            scrData.ClearData();
            levData.ClearData();
            ClearData();

            MeterMode = MeterModes.Disable;            
        }
        

        // SwitchModes
        private void SwitchModes()
        {
            switch( meterMode )
            {
                case MeterModes.Enable:
                    if( !this.enabled ) this.enabled = true;
                    break;

                case MeterModes.Background:
                    if( !this.enabled ) this.enabled = true;
                    for( int cnt = 0; cnt < anchorsCount; cnt++ )
                        textLabels[ cnt ].ClearText();                    
                    break;

                case MeterModes.Disable:
                    if( this.enabled ) this.enabled = false;
                    break;
            }
        } 


        // Calculate FontSize
        internal void CalculateFontSize()
        {
            if( !Application.isPlaying ) return;

            newFontSize = ( int )( fontSize / 1000f * scrData.screenWidth );

            for( int cnt = 0; cnt < anchorsCount; cnt++ )
                textLabels[ cnt ].SetFontSize( newFontSize );
        }
      
        
        // SetText
        internal void SetAllText( bool setalign )
        {
            if( !Application.isPlaying ) return;

            if( meterMode == MeterModes.Disable ) return;

            // levData.Enabled
            if( levData.Enabled )
            {
                LEVresult = levData.GetLevelInfo();

                if( MeterMode == MeterModes.Enable )
                {
                    tempLabel = textLabels[ ( int )levData.Anchor ];
                    tempLabel.AddText( LEVresult );
                    if( setalign ) tempLabel.SetAnchor( levData.Anchor, scrData.screenWidth, scrData.screenHeight );
                }
            }

            // fpsData.Enabled            
            if( fpsData.Enabled )
            {
                FPSresult = fpsData.GetFPSData();

                if( MeterMode == MeterModes.Enable )
                {

					/*
                    tempLabel = textLabels[ ( int )fpsData.Anchor ];
                    tempLabel.AddText( FPSresult );
                    if( setalign ) tempLabel.SetAnchor( fpsData.Anchor, scrData.screenWidth, scrData.screenHeight );
                    */
                }
                
            }            

            // memData.Enabled
            if( memData.Enabled )
            {
                MEMresult = memData.GetMemoryData();

                if( MeterMode == MeterModes.Enable )
                {
                    tempLabel = textLabels[ ( int )memData.Anchor ];
                    tempLabel.AddText( MEMresult );
                    if( setalign ) tempLabel.SetAnchor( memData.Anchor, scrData.screenWidth, scrData.screenHeight );
                }
            }

            // scrData.Enabled            
            if( scrData.Enabled )
            {
                SCRresult = scrData.GetScreenInfo();

                if( MeterMode == MeterModes.Enable )
                {
                    tempLabel = textLabels[ ( int )scrData.Anchor ];
                    tempLabel.AddText( SCRresult );
                    if( setalign ) tempLabel.SetAnchor( scrData.Anchor, scrData.screenWidth, scrData.screenHeight );
                }
            }

            // hrwData.Enabled            
            if( hrwData.Enabled )
            {
                HRWresult = hrwData.GetHardwareInfo();

                if( MeterMode == MeterModes.Enable )
                {
                    tempLabel = textLabels[ ( int )hrwData.Anchor ];
                    tempLabel.AddText( HRWresult );
                    if( setalign ) tempLabel.SetAnchor( hrwData.Anchor, scrData.screenWidth, scrData.screenHeight );
                }
            }

            // UpdateText
            for( int cnt = 0; cnt < anchorsCount; cnt++ )
            {
                textLabels[ cnt ].UpdateText();
            }
        }


        // ClearData
        internal void ClearData()
        {
            if( !Application.isPlaying ) return;

            FPSresult = string.Empty;
            MEMresult = string.Empty;
            HRWresult = string.Empty;
            SCRresult = string.Empty;
            LEVresult = string.Empty;
        }

		void OnGUI()
		{
			GUI.Label(new Rect(800,0,300,100),FPSresult , largeFont);
			GUI.Label(new Rect(800,100,300,100),MEMresult , largeFont);
		}
    }


}