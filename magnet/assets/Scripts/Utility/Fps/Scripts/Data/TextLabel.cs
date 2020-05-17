/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter					       *
 * Script:		 TextLabel.cs                 		       *
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

namespace SmartFPSMeter.labels
{
    /// <summary>  
    /// "UnityEngine.TextAnchor" used as main anchor.
    /// 
    /// UpperLeft	    Text is anchored in upper left corner.                  Text will be left-aligned.
    /// UpperCenter     Text is anchored in upper side, centered horizontally.  Text will be center-aligned.
    /// UpperRight	    Text is anchored in upper right corner.                 Text will be right-aligned.
    /// 
    /// MiddleLeft	    Text is anchored in left side, centered vertically.     Text will be left-aligned.
    /// MiddleCenter	Text is centered both horizontally and vertically.      Text will be center-aligned.
    /// MiddleRight	    Text is anchored in right side, centered vertically.    Text will be right-aligned.
    /// 
    /// LowerLeft	    Text is anchored in lower left corner.                  Text will be left-aligned.
    /// LowerCenter	    Text is anchored in lower side, centered horizontally.  Text will be center-aligned.
    /// LowerRight	    Text is anchored in lower right corner.                 Text will be right-aligned.
    /// </summary>    

    [System.Serializable]
    public class TextLabel
    {
        [SerializeField]
        private GUIText myGUIText = null;

        [SerializeField]
        private GameObject myGO = null;

        private System.Text.StringBuilder myNewText = new System.Text.StringBuilder();

        private Vector3 position = Vector3.zero;
        private Vector2 offset = Vector2.zero;

        public float offsetX = 0f;
        public float offsetY = 0f;


        // LineSpacing
        internal float lineSpacing
        {
            get { return myGUIText.lineSpacing; }
            set
            {
                if( myGUIText.lineSpacing == value ) return;
                myGUIText.lineSpacing = value;
            }
        }


        // Label Setup
        internal void LabelSetup( TextAnchor anchor, GameObject parent )
        {
            myGO = GameObject.Find( anchor.ToString() );
            if( !myGO )
            {
                myGO = new GameObject( anchor.ToString(), typeof( GUIText ) );
                myGO.hideFlags = HideFlags.HideInHierarchy;
                myGO.transform.position = Vector3.zero;
                myGO.transform.parent = parent.transform;
            }
            myGUIText = myGO.guiText;
        }


        // Add Text
        internal void AddText( string newText )
        {
            if( myNewText.Length > 0 ) myNewText.Append( ADD_NEWLINE );
            myNewText.Append( newText );
        }

        // Update Text
        internal void UpdateText()
        {
            myGUIText.text = myNewText.ToString();
            myNewText.Length = 0;
        }

        // Clear Text
        internal void ClearText()
        {
            myNewText.Length = 0;
            myGUIText.text = string.Empty;                      
        }        


        // Set FontSize
        internal void SetLabelFont( Font labelFont )
        {
            myGUIText.font = labelFont;
        }

        // Set FontSize
        internal void SetFontSize( int newFontSize )
        {
            myGUIText.fontSize = newFontSize;
        }

        // SetLayer
        internal void SetLayer( int layerIndex )
        {
            myGO.layer = layerIndex;
        }


        // Set Anchor
        internal void SetAnchor( TextAnchor anchor, float width, float height )
        {
            float calcX = offsetX * width / 100f;
            float calcY = offsetY * height / 100f;

            switch( anchor )
            {
                case TextAnchor.LowerLeft:
                    position.x = 0f;
                    position.y = 0f;
                    offset.x = 5f + calcX;
                    offset.y = 5f + calcY;
                    myGUIText.alignment = TextAlignment.Left;
                    myGUIText.anchor = TextAnchor.LowerLeft;
                    break;

                case TextAnchor.LowerCenter:
                    position.x = 0.5f;
                    position.y = 0f;
                    offset.x = 0f + calcX;
                    offset.y = 5f + calcY;
                    myGUIText.alignment = TextAlignment.Center;
                    myGUIText.anchor = TextAnchor.LowerCenter;
                    break;

                case TextAnchor.LowerRight:
                    position.x = 1f;
                    position.y = 0f;
                    offset.x = -5f - calcX;
                    offset.y = 5f + calcY;
                    myGUIText.alignment = TextAlignment.Right;
                    myGUIText.anchor = TextAnchor.LowerRight;
                    break;

                case TextAnchor.MiddleLeft:
                    position.x = 0f;
                    position.y = 0.5f;
                    offset.x = 5f + calcX;
                    offset.y = -2.5f - calcY;
                    myGUIText.alignment = TextAlignment.Left;
                    myGUIText.anchor = TextAnchor.MiddleLeft;
                    break;

                case TextAnchor.MiddleCenter:
                    position.x = 0.5f;
                    position.y = 0.5f;
                    offset.x = 0f + calcX;
                    offset.y = -2.5f - calcY;
                    myGUIText.alignment = TextAlignment.Center;
                    myGUIText.anchor = TextAnchor.MiddleCenter;
                    break;

                case TextAnchor.MiddleRight:
                    position.x = 1f;
                    position.y = 0.5f;
                    offset.x = -5f - calcX;
                    offset.y = -2.5f - calcY;
                    myGUIText.alignment = TextAlignment.Right;
                    myGUIText.anchor = TextAnchor.MiddleRight;
                    break;      

                case TextAnchor.UpperLeft:
                    position.x = 0f;
                    position.y = 1f;
                    offset.x = 5f + calcX;
                    offset.y = -5f - calcY;
                    myGUIText.alignment = TextAlignment.Left;
                    myGUIText.anchor = TextAnchor.UpperLeft;
                    break;

                case TextAnchor.UpperCenter:
                    position.x = 0.5f;
                    position.y = 1f;
                    offset.x = 0f + calcX;
                    offset.y = -5f - calcY;
                    myGUIText.alignment = TextAlignment.Center;
                    myGUIText.anchor = TextAnchor.UpperCenter;
                    break;

                case TextAnchor.UpperRight:
                    position.x = 1f;
                    position.y = 1f;
                    offset.x = -5f - calcX;
                    offset.y = -5f - calcY;
                    myGUIText.alignment = TextAlignment.Right;
                    myGUIText.anchor = TextAnchor.UpperRight;
                    break;
            }

            myGO.transform.position = position;
            myGUIText.pixelOffset = offset;
        }


        internal const string ADD_NEWLINE = "\n";         

        internal static string ColorToString( Color32 сolor )
        {
            return string.Format( "<color=#{0}>",
                   сolor.r.ToString( "x2" ) + сolor.g.ToString( "x2" ) + сolor.b.ToString( "x2" ) + сolor.a.ToString( "x2" ) );
        }
        internal const string ADD_ENDLINE = "</color>";
    }
}