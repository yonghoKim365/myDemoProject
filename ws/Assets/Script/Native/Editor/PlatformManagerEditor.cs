
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformManager))]
public class PlatformManagerEditor : Editor 
{
	const string KAKAO_MANIFEST_PATH = "/Platform/kakao/AndroidManifest.xml";
	const string GOOGLE_MANIFEST_PATH = "/Platform/google/AndroidManifest.xml";

	const string KAKAO_ICON_PATH = "/Platform/kakao/AppIcon_144_AOS.png";
	const string GOOGLE_ICON_PATH = "/Platform/google/AppIcon_144_AOS.png";
	const string ICON_PATH = "/Editor/images/appicon/AOS/AppIcon_144_AOS.png";

	const string KAKAO_DRAWABLE_ICON_PATH2 = "/Platform/kakao/drawable/small_icon.png";
	const string GOOGLE_DRAWABLE_ICON_PATH2 = "/Platform/google/drawable/small_icon.png";
	const string KAKAO_DRAWABLE_HDPI_ICON_PATH2 = "/Platform/kakao/drawable-hdpi/small_icon.png";
	const string GOOGLE_DRAWABLE_HDPI_ICON_PATH2 = "/Platform/google/drawable-hdpi/small_icon.png";
	const string KAKAO_DRAWABLE_XHDPI_ICON_PATH2 = "/Platform/kakao/drawable-xhdpi/small_icon.png";
	const string GOOGLE_DRAWABLE_XHDPI_ICON_PATH2 = "/Platform/google/drawable-xhdpi/small_icon.png";
	const string DRAWABLE_ICON_PATH = "/Plugins/Android/wemeApi/res/drawable/small_icon.png";
	const string DRAWABLE_HDPI_ICON_PATH = "/Plugins/Android/wemeApi/res/drawable-hdpi/small_icon.png";
	const string DRAWABLE_XHDPI_ICON_PATH = "/Plugins/Android/wemeApi/res/drawable-xhdpi/small_icon.png";




	const string MANIFEST_PATH = "/Plugins/Android/AndroidManifest.xml";

	const string KaKaoBundle = "com.kakaogame.windsoul";
	const string GoogleBundle = "com.linktomorrow.windsoul";

	const string GOOGLE = "google";
	const string KAKAO = "kakao";

	public override void OnInspectorGUI()
	{
		PlatformManager pm = (PlatformManager)target;
		
		EditorGUIUtility.LookLikeControls();

		PandoraManager pandora = (PandoraManager)EditorGUILayout.ObjectField(pm.pandora, typeof(PandoraManager));

		if(pandora != pm.pandora)
		{
			pm.pandora = pandora;
		}

		PlatformManager.Platform type = (PlatformManager.Platform)EditorGUILayout.EnumPopup("Android Market", pm.type);

		if( type != pm.type)
		{
			copyFiles(type);

			pm.type = type;

			switch(type)
			{
			case PlatformManager.Platform.Google:
				pm.pandora.androidMarket = GOOGLE;
				break;
			case PlatformManager.Platform.Kakao:
				pm.pandora.androidMarket = KAKAO;
				break;
			}

		}

		if (GUILayout.Button("파일복사"))
		{
			copyFiles(pm.type);
		}

		if ( GUI.changed )
		{
			EditorUtility.SetDirty(pm);
		}
	}

	void copyFiles(PlatformManager.Platform type)
	{
		try
		{

			string originalIconPath = "";
			string originalDrawableIconPath = "";
			string originalHDPIDrawableIconPath = "";
			string originalXHDPIDrawableIconPath = "";
			string originalMFPath = "";

			string targetIconPath = Application.dataPath + ICON_PATH;

			string targetDrawableIconPath = Application.dataPath + DRAWABLE_ICON_PATH;
			string targetDrawableHDPIIconPath = Application.dataPath + DRAWABLE_HDPI_ICON_PATH;
			string targetDrawableXHDPIIconPath = Application.dataPath + DRAWABLE_XHDPI_ICON_PATH;

			string targetMFPath = Application.dataPath + MANIFEST_PATH;

			switch(type)
			{
			case PlatformManager.Platform.Google:

				originalIconPath = Application.dataPath + GOOGLE_ICON_PATH;
				originalMFPath = Application.dataPath + GOOGLE_MANIFEST_PATH;
				PlayerSettings.bundleIdentifier = GoogleBundle;

				originalDrawableIconPath = Application.dataPath + GOOGLE_DRAWABLE_ICON_PATH2;
				originalHDPIDrawableIconPath = Application.dataPath + GOOGLE_DRAWABLE_HDPI_ICON_PATH2;
				originalXHDPIDrawableIconPath = Application.dataPath + GOOGLE_DRAWABLE_XHDPI_ICON_PATH2;

				break;
				
			case PlatformManager.Platform.Kakao:
				originalIconPath = Application.dataPath + KAKAO_ICON_PATH;
				originalMFPath = Application.dataPath + KAKAO_MANIFEST_PATH;
				PlayerSettings.bundleIdentifier = KaKaoBundle;

				originalDrawableIconPath = Application.dataPath + KAKAO_DRAWABLE_ICON_PATH2;
				originalHDPIDrawableIconPath = Application.dataPath + KAKAO_DRAWABLE_HDPI_ICON_PATH2;
				originalXHDPIDrawableIconPath = Application.dataPath + KAKAO_DRAWABLE_XHDPI_ICON_PATH2;

				break;
			}

			System.IO.File.Copy(originalIconPath, targetIconPath, true);
			System.IO.File.Copy(originalMFPath, targetMFPath, true);

			System.IO.File.Copy(originalDrawableIconPath, targetDrawableIconPath, true);
			System.IO.File.Copy(originalHDPIDrawableIconPath, targetDrawableHDPIIconPath, true);
			System.IO.File.Copy(originalXHDPIDrawableIconPath, targetDrawableXHDPIIconPath, true);

			long originalLibSize = 0;
			long targetLibSize = 0;
			
			long originalMFSize = 0;
			long targetMFSize = 0;

			originalLibSize = Util.getFileSize(originalIconPath);
			targetLibSize = Util.getFileSize(targetIconPath);

			originalMFSize = Util.getFileSize(originalMFPath);
			targetMFSize = Util.getFileSize(targetMFPath);

			if(originalLibSize != targetLibSize)
			{
				EditorUtility.DisplayDialog("","복사 오류","확인");
				return;
			}

			if(originalMFSize != targetMFSize)
			{
				EditorUtility.DisplayDialog("","복사 오류","확인");
				return;
			}

			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog("","입력 완료","확인");
		}
		catch
		{
			Debug.LogError("Error....");
		}
	}
	
	
}
