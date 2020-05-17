/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 MEMData.cs                  		       *
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
    /// Shows memory usage data.
    /// </summary>
    [System.Serializable]
    public class MEMData : BaseData
    {
        [SerializeField]
        private float updateInterval = 0.9f;

        [SerializeField]
        private bool decimalMEM = true;

        [SerializeField]
        private bool showMEMmono = true;

        [SerializeField]
        private bool showMEMalloc = true;

        [SerializeField]
        private bool showMEMreserv = true;

        // CountMEM
        private float lastMEMInterval = 0f;
        private float monoUsedSize = 0f;
        private float totalAllocatedMemory = 0f;
        private float totalReservedMemory = 0f;

        private string dcml = string.Empty;


        /// <summary>
        /// Mono memory usage string data.
        /// </summary>
        public string DataMEMmono { get; private set; }

        /// <summary>
        /// Total allocated memory usage string data.
        /// </summary>
        public string DataMEMalloc { get; private set; }

        /// <summary>
        /// Total reserved usage string data.
        /// </summary>
        public string DataMEMreserv { get; private set; }
        

        /// <summary>
        /// Calculation value update interval.
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
        /// Allows to output memory usage in decimal value.
        /// </summary>
        public bool DecimalMEM
        {
            get { return decimalMEM; }
            set 
            {
                if( decimalMEM == value ) return;
                decimalMEM = value;
                if( decimalMEM ) dcml = "f2";
                else dcml = "f0";
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see amount of memory, allocated by managed Mono objects.
        /// </summary>
        public bool ShowMEMmono
        {
            get { return showMEMmono; }
            set
            {
                if( showMEMmono == value ) return;
                showMEMmono = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see amount of memory, currently allocated by application.
        /// </summary>
        public bool ShowMEMalloc
        {
            get { return showMEMalloc; }
            set
            {
                if( showMEMalloc == value ) return;
                showMEMalloc = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        /// <summary>
        /// Allows to see memory amount reserved for application.
        /// </summary>
        public bool ShowMEMreserv
        {
            get { return showMEMreserv; }
            set
            {
                if( showMEMreserv == value ) return;
                showMEMreserv = value;
                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        
        // DataAwake
        internal override void DataAwake()
        {
            if( decimalMEM ) dcml = "f2";
            else dcml = "f0";
            base.DataAwake();
        }

        // CalculateMEM
        internal void CalculateMEM()
        {
            float timeNow = Time.realtimeSinceStartup;
            if( timeNow > lastMEMInterval + updateInterval )
            {
                if( showMEMmono ) monoUsedSize = System.GC.GetTotalMemory( false ) / 1048576f;          // Mono used size ... 
                if( showMEMalloc ) totalAllocatedMemory = Profiler.GetTotalAllocatedMemory() / 1048576f; // Get Total Allocated Memory ...
                if( showMEMreserv ) totalReservedMemory = Profiler.GetTotalReservedMemory() / 1048576f;   // Get Total Reserved Memory ...

                lastMEMInterval = timeNow;

                outdated = true;
                MainMeter.Instance.SetAllText( false );
            }
        }

        // GetMemoryData
        internal string GetMemoryData()
        {
            if( !outdated ) return dataInfo;            

            bool needNewLine = false;

            dataText.Length = 0;
            dataText.Append( currentColor );

            // mono
            if( showMEMmono )
            {
                DataMEMmono = "MEM [mono]: " + monoUsedSize.ToString( dcml ) + " mb";
                dataText.Append( DataMEMmono );
                needNewLine = true;
            }
            else DataMEMmono = string.Empty;

            // alloc
            if( showMEMalloc )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );
                DataMEMalloc = "MEM [alloc]: " + totalAllocatedMemory.ToString( dcml ) + " mb";
                dataText.Append( DataMEMalloc );
                needNewLine = true;
            }
            else DataMEMalloc = string.Empty;

            // total
            if( showMEMreserv )
            {
                if( needNewLine ) dataText.Append( TextLabel.ADD_NEWLINE );
                DataMEMreserv = "MEM [reserv]: " + totalReservedMemory.ToString( dcml ) + " mb";
                dataText.Append( DataMEMreserv );
            }
            else DataMEMreserv = string.Empty;

            dataText.Append( TextLabel.ADD_ENDLINE );
            dataInfo = dataText.ToString();
            outdated = false;

            return dataInfo;
        }

        // ClearData
        internal override void ClearData()
        {
            base.ClearData();
            DataMEMmono = string.Empty;
            DataMEMalloc = string.Empty;
            DataMEMreserv = string.Empty;
        }
    }
}