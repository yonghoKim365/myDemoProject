using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(CutSceneMaker))]
public class CutSceneMakerEditor : Editor 
{
	CutSceneMaker _editor;
	SerializedObject _csm;

	void OnEnable()
	{
		_editor = target as CutSceneMaker;
		_csm = new SerializedObject(_editor);

//		_nowCutScene = _csm.FindProperty("nowCutSceneId");
//		_nowPlayTime = _csm.FindProperty("csTime");

	}

	string nowCutSceneId = "";
	float nowCutScenePlayTime = 0;
	public override void OnInspectorGUI()
	{
		_csm.Update();

		base.DrawDefaultInspector();
		EditorGUILayout.BeginHorizontal();
		_editor.useCutSceneMaker = EditorGUILayout.BeginToggleGroup("컷씬 에디터 사용",_editor.useCutSceneMaker);

		if(_editor.useCutSceneMaker)
		{
			_editor.debugManager.useDebug = true;
			_editor.resourceManager.useAssetDownload = false;
		}


		EditorGUILayout.EndToggleGroup();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		if(_editor.useCutSceneMaker)
		{
			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("컷씬 소스");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			_editor.source = EditorGUILayout.TextArea(_editor.source,GUILayout.Height(50));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("클립보드 읽기"))
			{
				_editor.source = ClipboardHelper.clipBoard;

				if(Application.isPlaying)
				{
					UnityEditor.EditorApplication.isPaused = false;
					
					if(_editor.parseSource() == false)
					{
						EditorUtility.DisplayDialog("소스 오류","소스에 문제가 있습니다.\n클립보드에 올바른 데이터가 없습니다.","확인");
					}
					else
					{
						_editor.restart();
					}
				}
			}
			EditorGUILayout.EndHorizontal();


			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));

			EditorGUILayout.BeginHorizontal();


			int beforeSec = EditorGUILayout.IntField("설정시간", _editor.searchTimeOffset, GUILayout.MinWidth(300.0f));

			bool update = false;

			if(_editor.searchTimeOffset != beforeSec)
			{
				_editor.searchTimeOffset = beforeSec;
				update = true;
			}

			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button(beforeSec + "초 앞으로"))
			{
				UnityEditor.EditorApplication.isPaused = false;

				if(Application.isPlaying)
				{
					_editor.goToTime(CutSceneManager.cutScenePlayTime - beforeSec);
				}
			}

			if(GUILayout.Button(beforeSec + "초 뒤로"))
			{
				UnityEditor.EditorApplication.isPaused = false;

				if(Application.isPlaying)
				{
					_editor.goToTime(CutSceneManager.cutScenePlayTime + beforeSec);
				}
			}
			EditorGUILayout.EndHorizontal();






			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));
			
			EditorGUILayout.BeginHorizontal();

			int tsec = EditorGUILayout.IntField("타임머신", _editor.targetSearchTime, GUILayout.MinWidth(300.0f));

			if(tsec != _editor.targetSearchTime)
			{
				_editor.targetSearchTime = tsec;
				update = true;
			}

			if(update) UnityEditor.EditorUtility.SetDirty(_editor);


			EditorGUILayout.EndHorizontal();
			
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button(tsec + "초로 이동"))
			{
				UnityEditor.EditorApplication.isPaused = false;

				if(Application.isPlaying)
				{
					_editor.goToTime(tsec);
				}
			}
			EditorGUILayout.EndHorizontal();


			GUILayout.Space(10.0f);

			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("재생 재개"))
			{
				UnityEditor.EditorApplication.isPaused = false;

				if(Application.isPlaying)
				{
					Time.timeScale = 1.0f;
				}
			}
			EditorGUILayout.EndHorizontal();


			GUILayout.Space(10.0f);
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("씬뷰 카메라사용"))
			{
				if(Application.isPlaying)
				{
					controllSceneView();
				}
			}
			/*
			if(GUILayout.Button("선택 좌표복사"))
			{
				if(Application.isPlaying)
				{
					SelectedObjectPosition.Execute();
				}
			}
			*/

			if(GUILayout.Button("카메라좌표 복사"))
			{
				if(Application.isPlaying)
				{
					CutSceneCamInfoEditor.Execute();
				}
			}

			EditorGUILayout.EndHorizontal();


			GUILayout.Space(150.0f);


			EditorGUILayout.BeginHorizontal();

			if(GUILayout.Button("처음부터"))
			{
				UnityEditor.EditorApplication.isPaused = false;

				if(Application.isPlaying)
				{
					_editor.restart();
				}
			}

			EditorGUILayout.EndHorizontal();





//			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));
//			
//			EditorGUILayout.BeginHorizontal();
//			SerializedProperty prop2 = NGUIEditorTools.DrawProperty("맵디자인", serializedObject, "targetSearchTime", GUILayout.MinWidth(300.0f));
//			int tsec = prop1.intValue;
//			EditorGUILayout.EndHorizontal();


		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}

		_csm.ApplyModifiedProperties();
		
	}

	public static void controllSceneView()
	{
		SceneView sv = UnityEditor.SceneView.currentDrawingSceneView;
		
		if(Application.isPlaying)
		{

			GameManager.me.gameCamera.transform.localRotation = sv.camera.transform.localRotation;
			GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition = sv.camera.transform.localPosition;

			Debug.LogError("sv.size : " + sv.size);
			Debug.LogError("sv.camera : " + sv.camera.fieldOfView);

			if(sv.camera.orthographic == false)
			{
				GameManager.me.gameCamera.fieldOfView = sv.camera.fieldOfView;
			}
		}
	}


}



class CutSceneMakerEditorShortKey
{
	[MenuItem ("Custom/Quick Restart CutSceneMaker %1") ]
	static void Execute()
	{
		UnityEditor.EditorApplication.isPaused = false;

		if(Application.isPlaying)
		{
			CutSceneMaker.instance.restart();
		}
	}
}


class CutSceneMakerEditorShortKeyClipBoard
{
	[MenuItem ("Custom/Get Source from Clipboard CutSceneMaker %`") ]
	static void Execute()
	{
		if(Application.isPlaying)
		{
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				UnitSkillCamMaker.instance.source = ClipboardHelper.clipBoard;
			}
			else
			{
				CutSceneMaker.instance.source = ClipboardHelper.clipBoard;
			}


			UnityEditor.EditorApplication.isPaused = false;
			
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				if(UnitSkillCamMaker.instance.parseSource() == false)
				{
					EditorUtility.DisplayDialog("소스 오류","소스에 문제가 있습니다.\n클립보드에 올바른 데이터가 없습니다.","확인");
				}
			}
			else
			{
				if(CutSceneMaker.instance.parseSource() == false)
				{
					EditorUtility.DisplayDialog("소스 오류","소스에 문제가 있습니다.\n클립보드에 올바른 데이터가 없습니다.","확인");
				}
				else
				{
					CutSceneMaker.instance.restart();
				}
				
			}
		}
	}
}





public static class MyGUIStyles
{
	private static GUIStyle m_line = null;
	
	public static GUIStyle EditorLine
	{
		get 
		{
			if(m_line == null)
			{
				m_line = new GUIStyle("box");
				m_line.border.top = m_line.border.bottom = 1;
				m_line.margin.top = m_line.margin.bottom = 1;
				m_line.padding.top = m_line.padding.bottom = 1;
			}

			return m_line; 
		}
	}
}




