/***********************************************************
 * 														   *
 * Asset:		 Smart FPS Meter          			       *
 * Script:		 MainMeterEditor.cs         		       *
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
using System;
using SmartFPSMeter.labels;

namespace SmartFPSMeter
{
    [CustomEditor( typeof( MainMeter ) )]
    public class MainMeterEditor : Editor
    {
        private MainMeter myTarget = null;
        private string[] stateNames = { "OFF", "ON" };

        private static int metersCount = 0;

        private const int textWidth = 115;
        private const int hintWidth = 15;

        // OnEnable
        void OnEnable()
        {
            myTarget = ( MainMeter )target;
            metersCount = FindObjectsOfType<MainMeter>().Length;
        }


        // OnInspectorGUI
        public override void OnInspectorGUI()
        {
            if( metersCount > 1 )
            {
                GUILayout.Space( 5 );
                EditorGUILayout.HelpBox( "ERROR: There are " + metersCount + " main meters in the scene. \nPlease ensure there is always exactly one main meter in the scene.", MessageType.Error );
                GUILayout.Space( 5 );
                EditorGUILayout.HelpBox( "ERROR: Smart FPS Meter is incorrectly placed in scene! \nPlease, use only \"GameObject->Create Other->Smart FPS Meter\" menu to correct place!", MessageType.Error );
                GUILayout.Space( 5 );
                myTarget.enabled = false;
                myTarget.MeterMode = MeterModes.Disable;
                return;
            }
            
            
            // BEGIN
            GUILayout.BeginVertical( "Box", GUILayout.Width( 380 ) );
            GUILayout.Space( 5 );
            //

            ShowParameters();
            if( GUI.changed ) EditorUtility.SetDirty( myTarget );

            // END
            GUILayout.Space( 5 );
            GUILayout.EndVertical();
            //
        }

        
        // Show Parameters
        private void ShowParameters()
        {
            GUILayout.BeginVertical( "Box" );
            GUILayout.BeginHorizontal();
            GUILayout.Space( 15 );
            myTarget.foldout = EditorGUILayout.Foldout( myTarget.foldout, "    Main Data", FoldOutStile() );        
            GUILayout.Space( 50 );
            myTarget.MeterMode = ( MeterModes )
                GUILayout.Toolbar( ( int )myTarget.MeterMode,
                    Enum.GetNames( typeof( MeterModes ) ),
                        GUILayout.Height( 17 ) );
            GUILayout.EndHorizontal();
            

            if( myTarget.foldout )
            {
                GUILayout.Space( 5 );

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Labels Layer", GUILayout.Width( textWidth ) );
                myTarget.LayerIndex = EditorGUILayout.LayerField( myTarget.LayerIndex );
                GUILayout.EndHorizontal();

                GUILayout.Space( 5 );

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Font", GUILayout.Width( textWidth ) );
                myTarget.LabelsFont = EditorGUILayout.ObjectField( myTarget.LabelsFont, typeof( Font ), false ) as Font;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Font Size", GUILayout.Width( textWidth ) );
                float fontSize = myTarget.FontSize;
                fontSize = EditorGUILayout.Slider( fontSize, 1f, 35f );
                myTarget.FontSize = ( int )fontSize;
                GUILayout.EndHorizontal();

                GUILayout.Space( 5 );

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Corner", GUILayout.Width( textWidth ) );
                myTarget.targetCorner = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.targetCorner );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Line Spacing", GUILayout.Width( textWidth ) );
                myTarget.LabelLineSpacing = EditorGUILayout.Slider( myTarget.LabelLineSpacing, 1f, 4f );
                GUILayout.EndHorizontal();

                GUILayout.Space( 2 );

                float minOffsetX = 0f;
                float minOffsetY = 0f;
                if( myTarget.targetCorner == TextAnchor.LowerCenter || myTarget.targetCorner == TextAnchor.UpperCenter ) minOffsetX = -35f;
                else if( myTarget.targetCorner == TextAnchor.MiddleCenter )
                {
                    minOffsetX = -35f;
                    minOffsetY = -35f;
                }
                else if( myTarget.targetCorner == TextAnchor.MiddleLeft || myTarget.targetCorner == TextAnchor.MiddleRight ) minOffsetY = -35f;

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Offset X", GUILayout.Width( textWidth ) );
                myTarget.LabelOffsetX = EditorGUILayout.Slider( myTarget.LabelOffsetX, minOffsetX, 35f );
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label( "Offset Y", GUILayout.Width( textWidth ) );
                myTarget.LabelOffsetY = EditorGUILayout.Slider( myTarget.LabelOffsetY, minOffsetY, 35f );
                GUILayout.EndHorizontal();
            }            

            GUILayout.Space( 2 );
            GUILayout.EndVertical();

            bool onOff = false;

            onOff = myTarget.fpsData.Enabled;
            CallNamedMethod( ref myTarget.fpsData.foldout, ref onOff, "    FPS Data", "FPSinspector" );
            myTarget.fpsData.Enabled = onOff;

            onOff = myTarget.memData.Enabled;
            CallNamedMethod( ref myTarget.memData.foldout, ref onOff, "    Memory Data", "MEMinspector" );
            myTarget.memData.Enabled = onOff;

            onOff = myTarget.hrwData.Enabled;
            CallNamedMethod( ref myTarget.hrwData.foldout, ref onOff, "    Hardware Info", "HRWinspector" );
            myTarget.hrwData.Enabled = onOff;

            onOff = myTarget.scrData.Enabled;
            CallNamedMethod( ref myTarget.scrData.foldout, ref onOff, "    Screen Data", "SCRinspector" );
            myTarget.scrData.Enabled = onOff;

            onOff = myTarget.levData.Enabled;
            CallNamedMethod( ref myTarget.levData.foldout, ref onOff, "    Level Info", "LEVinspector" );
            myTarget.levData.Enabled = onOff;
        }

        // Call NamedMethod ( Refletions no-no )
        private void CallNamedMethod( ref bool foldout, ref bool targetCornerBool, string dataName, string methodName )
        {
            GUILayout.BeginVertical( "Box" );
            GUILayout.BeginHorizontal();
            GUILayout.Space( 15 );
            foldout = EditorGUILayout.Foldout( foldout, dataName, FoldOutStile() );
            GUILayout.Space( textWidth );
            targetCornerBool = Convert.ToBoolean( GUILayout.Toolbar( Convert.ToInt32( targetCornerBool ), stateNames, GUILayout.Height( 17 ), GUILayout.Width( 170 ) ) );
            GUILayout.EndHorizontal();            
            if( foldout )
            {
                GUILayout.Space( 5 );
                switch( methodName )
                {
                    case "FPSinspector": FPSinspector(); break;
                    case "MEMinspector": MEMinspector(); break;
                    case "HRWinspector": HRWinspector(); break;
                    case "SCRinspector": SCRinspector(); break;
                    case "LEVinspector": LEVinspector(); break;
                }
            }

            GUILayout.Space( 2 );
            GUILayout.EndVertical();
        }


        // FPS inspector
        private void FPSinspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Anchor", GUILayout.Width( textWidth ) );
            myTarget.fpsData.Anchor = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.fpsData.Anchor );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Update Interval", GUILayout.Width( textWidth ) );
            myTarget.fpsData.UpdateInterval = EditorGUILayout.Slider( myTarget.fpsData.UpdateInterval, 0.1f, 5f );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show MS", GUILayout.Width( textWidth ) );
            myTarget.fpsData.ShowMS = EditorGUILayout.Toggle( myTarget.fpsData.ShowMS, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "milliseconds value." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show FPS", GUILayout.Width( textWidth ) );
            myTarget.fpsData.ShowFPS = EditorGUILayout.Toggle( myTarget.fpsData.ShowFPS, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "frames per second value." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show [min..max]", GUILayout.Width( textWidth ) );
            myTarget.fpsData.ShowMinMax = EditorGUILayout.Toggle( myTarget.fpsData.ShowMinMax, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "minimum and maximum readings." );
            GUILayout.EndHorizontal();

            float dangerValue = myTarget.fpsData.DangerValue;
            float warningValue = myTarget.fpsData.WarningValue;

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Coloration Range", GUILayout.Width( textWidth ) );
            EditorGUILayout.MinMaxSlider(                
                ref dangerValue,
                ref warningValue,
                1, 60 );
            GUILayout.EndHorizontal();

            myTarget.fpsData.DangerValue = ( int )dangerValue;
            myTarget.fpsData.WarningValue = ( int )warningValue;


            GUILayout.BeginHorizontal();
            GUILayout.Label( "Fine Color", GUILayout.Width( textWidth ) );
            myTarget.fpsData.MyColor = EditorGUILayout.ColorField( myTarget.fpsData.MyColor, GUILayout.Width( 55 ) );
            GUILayout.Label( "FPS: " + warningValue + "+" );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Warning Color", GUILayout.Width( textWidth ) );
            myTarget.fpsData.WarningColor = EditorGUILayout.ColorField( myTarget.fpsData.WarningColor, GUILayout.Width( 55 ) );
            GUILayout.Label( "FPS: " + dangerValue + " - " + warningValue );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Danger Color", GUILayout.Width( textWidth ) );
            myTarget.fpsData.DangerColor = EditorGUILayout.ColorField( myTarget.fpsData.DangerColor, GUILayout.Width( 55 ) );
            GUILayout.Label( "FPS: 0 - " + dangerValue );
            GUILayout.EndHorizontal();
        }


        // MEM inspector
        private void MEMinspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Anchor", GUILayout.Width( textWidth ) );
            myTarget.memData.Anchor = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.memData.Anchor );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Update Interval", GUILayout.Width( textWidth ) );
            myTarget.memData.UpdateInterval = EditorGUILayout.Slider( myTarget.memData.UpdateInterval, 0.1f, 5f );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Decimal", GUILayout.Width( textWidth ) );
            myTarget.memData.DecimalMEM = EditorGUILayout.Toggle( myTarget.memData.DecimalMEM, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "memory usage in decimal values." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show [mono]", GUILayout.Width( textWidth ) );
            myTarget.memData.ShowMEMmono = EditorGUILayout.Toggle( myTarget.memData.ShowMEMmono, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "allocated by managed mono objects." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show [alloc]", GUILayout.Width( textWidth ) );
            myTarget.memData.ShowMEMalloc = EditorGUILayout.Toggle( myTarget.memData.ShowMEMalloc, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "currently allocated by application." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show [reserv]", GUILayout.Width( textWidth ) );
            myTarget.memData.ShowMEMreserv = EditorGUILayout.Toggle( myTarget.memData.ShowMEMreserv, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "amount reserved for application." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Text Color", GUILayout.Width( textWidth ) );
            myTarget.memData.MyColor = EditorGUILayout.ColorField( myTarget.memData.MyColor, GUILayout.Width( 145 ) );
            GUILayout.EndHorizontal();
        }


        // HRW inspector
        private void HRWinspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Anchor", GUILayout.Width( textWidth ) );
            myTarget.hrwData.Anchor = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.hrwData.Anchor );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show CPU", GUILayout.Width( textWidth ) );
            myTarget.hrwData.ShowCPU = EditorGUILayout.Toggle( myTarget.hrwData.ShowCPU, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "model name and cores count." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show RAM", GUILayout.Width( textWidth ) );
            myTarget.hrwData.ShowRAM = EditorGUILayout.Toggle( myTarget.hrwData.ShowRAM, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "system memory size." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show GPU", GUILayout.Width( textWidth ) );
            myTarget.hrwData.ShowGPU = EditorGUILayout.Toggle( myTarget.hrwData.ShowGPU, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "model, vram, shader, texture size." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Show GDV", GUILayout.Width( textWidth ) );
            myTarget.hrwData.ShowGDV = EditorGUILayout.Toggle( myTarget.hrwData.ShowGDV, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "graphics device version." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Text Color", GUILayout.Width( textWidth ) );
            myTarget.hrwData.MyColor = EditorGUILayout.ColorField( myTarget.hrwData.MyColor, GUILayout.Width( 145 ) );
            GUILayout.EndHorizontal();
        }


        // SCR inspector
        private void SCRinspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Anchor", GUILayout.Width( textWidth ) );
            myTarget.scrData.Anchor = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.scrData.Anchor );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Quality Level", GUILayout.Width( textWidth ) );
            myTarget.scrData.ShowQL = EditorGUILayout.Toggle( myTarget.scrData.ShowQL, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "current quality level." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Screen Resolution", GUILayout.Width( textWidth ) );
            myTarget.scrData.ShowSR = EditorGUILayout.Toggle( myTarget.scrData.ShowSR, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "resolution, window size and dpi." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Text Color", GUILayout.Width( textWidth ) );
            myTarget.scrData.MyColor = EditorGUILayout.ColorField( myTarget.scrData.MyColor, GUILayout.Width( 145 ) );
            GUILayout.EndHorizontal();
        }


        // LEV inspector
        private void LEVinspector()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label( "Anchor", GUILayout.Width( textWidth ) );
            myTarget.levData.Anchor = ( TextAnchor )EditorGUILayout.EnumPopup( myTarget.levData.Anchor );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Level Name", GUILayout.Width( textWidth ) );
            myTarget.levData.ShowLN = EditorGUILayout.Toggle( myTarget.levData.ShowLN, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "loaded scene name." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Play Time", GUILayout.Width( textWidth ) );
            myTarget.levData.ShowPT = EditorGUILayout.Toggle( myTarget.levData.ShowPT, GUILayout.Width( hintWidth ) );
            GUILayout.Label( "total play time since game start." );
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label( "Text Color", GUILayout.Width( textWidth ) );
            myTarget.levData.MyColor = EditorGUILayout.ColorField( myTarget.levData.MyColor, GUILayout.Width( 145 ) );
            GUILayout.EndHorizontal();
        }



        // FoldOut Stile
        private GUIStyle FoldOutStile()
        {
            GUIStyle foldOutStyle = new GUIStyle( EditorStyles.foldout );
            foldOutStyle.fontStyle = FontStyle.Bold;
            foldOutStyle.fontSize = 13;
            foldOutStyle.stretchWidth = false;

            Color textColor = Color.red;
            foldOutStyle.onActive.textColor = textColor;
            foldOutStyle.onFocused.textColor = textColor;
            foldOutStyle.onHover.textColor = textColor;
            foldOutStyle.onNormal.textColor = textColor;
            foldOutStyle.active.textColor = textColor;
            foldOutStyle.focused.textColor = textColor;
            foldOutStyle.hover.textColor = textColor;
            foldOutStyle.normal.textColor = textColor;

            return foldOutStyle;
        }
    }
}