using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

[ExecuteInEditMode]
public class GetEffectReference : EditorWindow
{
	
	[MenuItem("Effect Editor/Open Texture Reference Checker")]
	static public void OpenEffectEditor ()
	{
		EditorWindow.GetWindow<GetEffectReference>(false, "Reference Checker", true);
	}	

	static public GetEffectReference instance;

	public string path = Application.dataPath;
	private string _newPath = null;

	private Dictionary<object, string> _selectList = new Dictionary<object, string>();

	Vector3 scrollPos;

	void OnGUI ()
	{
		EditorGUILayout.BeginVertical();

		path = EditorGUILayout.TextField("Target Path", path); 

		if(GUILayout.Button("대상 폴더 선택", GUILayout.Height(50)))
		{
			_newPath = EditorUtility.OpenFolderPanel("",Application.dataPath,"");

			if(_newPath != null && _newPath != path)
			{
				path = _newPath;

				path = "Assets"+path.Replace(Application.dataPath,"");

				_newPath = null;
				PlayerPrefs.SetString("EFFCHECK_FOLDER", path);
			}
		}
		EditorGUILayout.EndVertical();


		EditorGUILayout.BeginVertical();

		if(GUILayout.Button("수집시작!", GUILayout.Height(50)))
		{
			UnityEngine.Object o = Selection.activeObject;

			if(o != null && o as Texture2D)
			{
				SelectUsesOfAsset(path);
			}
		}

		EditorGUILayout.EndVertical();



		if(_selectList.Count > 0)
		{
			EditorGUILayout.BeginVertical();
			
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300f));

			foreach(KeyValuePair<object, string> kv in _selectList)
			{
				{
					EditorGUILayout.BeginHorizontal(GUILayout.Height(30));
					
					EditorGUILayout.LabelField(kv.Value);
					
					if(GUILayout.Button("선택"))
					{
						Selection.activeObject = (UnityEngine.Object)kv.Key;
					}
					
					EditorGUILayout.EndHorizontal();

				}
			}
			
			GUILayout.EndScrollView();
			
			EditorGUILayout.EndVertical();
		}
	}

	
	void OnEnable () 
	{ 
		path = PlayerPrefs.GetString("EFFCHECK_FOLDER", Application.dataPath);
		instance = this; 
	}
	
	void OnDisable () 
	{ 
		instance = null; 
	}


	public void SelectUsesOfAsset( string includePath ) 
	{
		Object cur = Selection.activeObject;
		
		string[] paths = AssetDatabase.GetAllAssetPaths();
		
		List<string> p = new List<string>();

		_selectList.Clear();

		for(int i = 0; i < paths.Length; ++i)
		{
			if(paths[i].Contains(includePath) == false ||
				paths[i].ToLower().EndsWith(".psd") || 
			   paths[i].ToLower().EndsWith(".tga") || 
			   paths[i].ToLower().EndsWith(".png"))
			{
				continue;
			}
			
			p.Add(paths[i]);
		}
		
		paths = p.ToArray();
		
		List<Object> results = new List<Object>();
		
		foreach(string pp in paths)
		{
			Object checkObj = AssetDatabase.LoadMainAssetAtPath(pp);
			
			if(checkDependencies(checkObj, cur))
			{
				results.Add(checkObj);
			}
		}
		
		if(results.Count > 0)
		{
			foreach(Object o in results)
			{
				_selectList.Add(o, AssetDatabase.GetAssetPath(o));
			}
			
			results.Add(cur);
			
			Selection.objects = results.ToArray();
			
		}
		else
		{
			Debug.LogError("이 파일에 연결된 어셋이 없습니다.");
		}
	}
	
	
	
	static bool checkDependencies(Object checkObj, Object cur)
	{
		Object[] objs = EditorUtility.CollectDependencies(new Object[]{checkObj});
		
		if(objs == null || objs.Length < 2)
		{
			return false;
		}
		
		foreach(Object so in objs)
		{
			if(so == cur)
			{
				return true;
			}
		}
		
		return false;
	}

	
	
}


