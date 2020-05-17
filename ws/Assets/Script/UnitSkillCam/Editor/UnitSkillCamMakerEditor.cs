using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(UnitSkillCamMaker))]
public class UnitSkillCamMakerEditor : Editor 
{
	UnitSkillCamMaker _editor;
	SerializedObject _csm;

	void OnEnable()
	{
		_editor = target as UnitSkillCamMaker;
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

		bool useEffectEditor = false;

		bool changeSettingForEffectEditor = false;

		_editor.useEffectSkillCamEditor = EditorGUILayout.BeginToggleGroup("이펙트 에디터 사용",_editor.useEffectSkillCamEditor);
		_editor.useUnitSkillCamMaker = _editor.useEffectSkillCamEditor;
		
		if(_editor.useEffectSkillCamEditor)
		{
			_editor.resourceManager.useAssetDownload = false;
			_editor.debugManager.useDebug = true;
			_editor.disablePlayerAttack = true;
		}
		
		EditorGUILayout.EndToggleGroup();


		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		_editor.useUnitSkillCamEditor = EditorGUILayout.BeginToggleGroup("컷씬 에디터 사용",_editor.useUnitSkillCamEditor);

		if(_editor.useUnitSkillCamEditor)
		{
			_editor.useUnitSkillCamMaker = true;
		}

		EditorGUILayout.EndToggleGroup();

		EditorGUILayout.EndHorizontal();



		if(GUILayout.Button("로딩 최소화"))
		{
			_editor.debugManager.debugUnitId = new string[5];
			_editor.debugManager.debugRoundId = "TEST";
		}


		EditorGUILayout.Separator();

		if(_editor.useUnitSkillCamMaker)
		{
			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));

//			EditorGUILayout.BeginHorizontal();
//			_editor.source = EditorGUILayout.TextArea(_editor.source,GUILayout.Height(50));
//			EditorGUILayout.EndHorizontal();

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
				}
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.Box(GUIContent.none, MyGUIStyles.EditorLine , GUILayout.ExpandWidth(true), GUILayout.Height(2f));
			GUILayout.Space(10.0f);


			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("위치변경(transform)"))
			{
				UnityEditor.EditorApplication.isPaused = false;
				
				if(Application.isPlaying)
				{
					_editor.repositionAll();
				}
			}
			
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("위치변경(value)"))
			{
				UnityEditor.EditorApplication.isPaused = false;
				
				if(Application.isPlaying)
				{
					_editor.resetCharacterPosition();
				}
			}

			EditorGUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("카메라 초기화"))
			{
				UnityEditor.EditorApplication.isPaused = false;
				
				if(Application.isPlaying)
				{
					_editor.resetCamera();
				}
			}
			
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("적유닛 삭제"))
			{
				_editor.killMonsterUnits();
			}
			
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("유닛 제거"))
			{
				_editor.deleteUnit();
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20.0f);




			EditorGUILayout.BeginHorizontal();

			if(GUILayout.Button("기본공격"))
			{
				_editor.playAttack();
			}

			if(GUILayout.Button("스킬사용"))
			{
				_editor.playSkill();
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("히어로몬스터 생성"))
			{
				_editor.createHeroMonster(ClipboardHelper.clipBoard);
			}

			if(GUILayout.Button("히어로몬스터 삭제"))
			{
				_editor.killHeroMonster();
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("H 기본"))
			{
				_editor.playMonsterHeroAttack(true,0);
			}
			
			if(GUILayout.Button("H S1"))
			{
				_editor.playMonsterHeroAttack(false,0);
			}

			if(GUILayout.Button("H S2"))
			{
				_editor.playMonsterHeroAttack(false,1);
			}

			if(GUILayout.Button("H S3"))
			{
				_editor.playMonsterHeroAttack(false,2);
			}
			
			if(GUILayout.Button("H S4"))
			{
				_editor.playMonsterHeroAttack(false,3);
			}


			
			if(GUILayout.Button("H S5"))
			{
				_editor.playMonsterHeroAttack(false,4);
			}
			
			if(GUILayout.Button("H S6"))
			{
				_editor.playMonsterHeroAttack(false,5);
			}
			
			if(GUILayout.Button("H S7"))
			{
				_editor.playMonsterHeroAttack(false,6);
			}
			
			if(GUILayout.Button("H S8"))
			{
				_editor.playMonsterHeroAttack(false,7);
			}


			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();

			for(int i = 8; i <= 18; ++i)
			{
				if(GUILayout.Button("H S"+(i+1))) _editor.playMonsterHeroAttack(false,i);
			}

//			if(GUILayout.Button("H S9")) _editor.playMonsterHeroAttack(false,8);
//			if(GUILayout.Button("H S10")) _editor.playMonsterHeroAttack(false,9);
//			if(GUILayout.Button("H S11")) _editor.playMonsterHeroAttack(false,10);
//			if(GUILayout.Button("H S12")) _editor.playMonsterHeroAttack(false,11);
//			if(GUILayout.Button("H S13")) _editor.playMonsterHeroAttack(false,12);
//			if(GUILayout.Button("H S14")) _editor.playMonsterHeroAttack(false,13);
//			if(GUILayout.Button("H S15")) _editor.playMonsterHeroAttack(false,14);
//			if(GUILayout.Button("H S16")) _editor.playMonsterHeroAttack(false,15);

			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			for(int i = 1; i <= 10; ++i)
			{
				if(GUILayout.Button("A"+(i))) _editor.playMonsterHeroAni(i);
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			for(int i = 11; i <= 18; ++i)
			{
				if(GUILayout.Button("A"+(i))) _editor.playMonsterHeroAni(i);
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20.0f);


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("애니데이터 읽기(클립보드)"))
			{
				_editor.convertClipboardDataToAniData(ClipboardHelper.clipBoard);
			}

			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();

			if(GUILayout.Button("유닛 재설정"))
			{
				_editor.loadDebugUnit(ClipboardHelper.clipBoard);
			}

			if(GUILayout.Button("스킬 재설정"))
			{
				_editor.loadDebugSkill(ClipboardHelper.clipBoard);
			}

			if(GUILayout.Button("애니/총알 다시 읽기"))
			{
				_editor.resetAniData();
			}			
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("유닛 리소스 테스트"))
			{
				_editor.loadDebugAllUnit(ClipboardHelper.clipBoard);
			}

			if(GUILayout.Button("공격"))
			{
				_editor.debugAllAttack();
			}
			
			if(GUILayout.Button("스킬"))
			{
				_editor.debugAllSkill();
			}		

			if(GUILayout.Button("제거"))
			{
				_editor.debugAllRemove();
			}		

			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("유닛대칭생성"))
			{
				_editor.loadDebugAllUnit(ClipboardHelper.clipBoard, true);
			}
			
			if(GUILayout.Button("적 공격"))
			{
				_editor.debugAllAttack(false);
			}
			
			if(GUILayout.Button("적 스킬"))
			{
				_editor.debugAllSkill(false);
			}

			if(GUILayout.Button("적 삭제"))
			{
				_editor.killMonsterUnits(true);
			}
			
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("디버깅버튼활성"))
			{
				_editor.setActive(true);
			}
			
			if(GUILayout.Button("디버깅버튼비활성"))
			{
				_editor.setActive(false);
			}

			if(GUILayout.Button("다음맵"))
			{
				_editor.nextMap();
			}

			
			if(GUILayout.Button("사운드"))
			{
				_editor.soundOnOff();
			}


			EditorGUILayout.EndHorizontal();


			GUILayout.Space(50.0f);
			
			
			
			EditorGUILayout.BeginHorizontal();
			
			if(GUILayout.Button("다음스테이지"))
			{
				_editor.nextStage();
			}


			if(GUILayout.Button("모델링 에러체크"))
			{
				_editor.loadAllModel();
			}


			if(GUILayout.Button("모델링 이펙트 설정"))
			{
				_editor.loadModel(ClipboardHelper.clipBoard);
			}



			EditorGUILayout.EndHorizontal();
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}

		_csm.ApplyModifiedProperties();
		
	}

}




