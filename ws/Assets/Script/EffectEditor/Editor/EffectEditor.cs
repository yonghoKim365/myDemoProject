using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;


public class EffectEditor : EditorWindow
{
	static public EffectEditor instance;
	
	public static EffectEditorData target
	{
		get { return NGUISettings.Get<EffectEditorData>("Effect Editor Data", null); }
		set { NGUISettings.Set("Effect Editor Data", value); }
	}

	public float effectResizeRatio = 1.0f;

	Vector2 mScroll = Vector2.zero;

//	Vector2 aniScroll = Vector2.zero;
//	Vector2 bulletScroll = Vector2.zero;

	
	EffectEditorData lastData;
	
	GameObject lastSelectedGameObject;
	GameObject currentSelectGameObject;
	
	string[] aniClips;

	private int _atkIndex = 1;

	private string _customAtkName = null;

	private int _currentSelectAnimation = 0;

	List<AniDataEffect> _aniFrameList = new List<AniDataEffect>();


	private float _dummySliderValue = 0.0f;

	GameObject dummy = null;

	private List<string> nameList = new List<string>();

	void OnGUI ()
	{
		if (lastData != target)
		{
			lastData = target;
			dataName = (target != null) ? target.name : "New Ani Data";
		}
		
		bool create = false;
		bool update = false;
		bool replace = false;
		bool sortAniFrame = false;
		bool refreshTime = false;

		string prefabPath = "";
		
		if (target != null && target.name == dataName)
		{
			prefabPath = AssetDatabase.GetAssetPath(target.gameObject.GetInstanceID());
		}

		if (string.IsNullOrEmpty(dataName)) dataName = "New Ani Data";
		if (string.IsNullOrEmpty(prefabPath)) prefabPath = NGUIEditorTools.GetSelectionFolder() + dataName + ".prefab";
		
		GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
		if (target == null && go != null) target = go.GetComponent<EffectEditorData>();



		NGUIEditorTools.SetLabelWidth(80f);

		GUILayout.BeginVertical();

			GUILayout.Space(6f);

			GUILayout.BeginHorizontal();
			
			if (go == null)
			{
				GUI.backgroundColor = Color.green;
				create = GUILayout.Button("생성", GUILayout.Width(76f));
			}
			else
			{
				GUI.backgroundColor = Color.red;
				create = GUILayout.Button("변경", GUILayout.Width(76f));
			}
			
			GUI.backgroundColor = Color.white;
			dataName = GUILayout.TextField(dataName);
			

			if(GUILayout.Button("클립보드", GUILayout.Width(76f)))
			{
				nameList.Add(dataName);
				dataName = ClipboardHelper.clipBoard;
			}

			if(GUILayout.Button("Undo", GUILayout.Width(76f)))
			{
				if(nameList.Count > 0)
				{
					dataName = nameList[nameList.Count-1];
				nameList.RemoveAt(nameList.Count-1);
				}
			}


			GUILayout.EndHorizontal();

		GUILayout.EndVertical();



		GUILayout.BeginVertical();

		if (create)
		{
			if (go == null || EditorUtility.DisplayDialog("주의", dataName + "가 이미 있으면 덮어씌워집니다.", "확인", "최소"))
			{
				replace = true;
				
				if (target == null || target.name != dataName)
				{
					// Create a new prefab for the atlas
					Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(prefabPath);
					
					// Create a new game object for the atlas
					go = new GameObject(dataName);
					EffectEditorData edd = go.AddComponent<EffectEditorData>();
					
					Debug.LogError(edd);
					
					// Update the prefab
					PrefabUtility.ReplacePrefab(go, prefab);
					DestroyImmediate(go);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
					
					// Select the atlas
					go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
					target = go.GetComponent<EffectEditorData>();
				}
			}
		}

		GUILayout.EndVertical();









		GUILayout.BeginVertical();
	
		EffectEditorSelector.Draw<EffectEditorData>("Select", target, OnSelectData, true);

		if (target != null && target.name == dataName)
		{

		}
		else
		{
			NGUIEditorTools.DrawSeparator();
			EditorGUILayout.HelpBox("작업하려면 새 이펙트 에디터 파일을 생성하시던가 선택하세요.", MessageType.Info);
		}

		GUILayout.EndVertical();










//		NGUIEditorTools.DrawHeader("Animation", true);
		{
			if(target != null)
			{

				GUILayout.BeginVertical();

				// 기본 캐릭터 모델링을 선택한다.
				GameObject modeling = EditorGUILayout.ObjectField(target.modeling, typeof(GameObject), false) as GameObject;

				EditorGUIUtility.labelWidth = 130;
				float scaleFactor = EditorGUILayout.FloatField("FBX ScaleFactor ", target.scaleFactor);

				if(Mathf.Approximately(scaleFactor, target.scaleFactor) == false)
				{
					target.scaleFactor = scaleFactor;
					update = true;
				}


				GUILayout.EndVertical();



				// 모델링이 새로 선택한 녀석이면 애니메이션이 있는지 확인한 후 있으면 세팅한다. 애니메이션이 없으면 캐릭터가 아니다.
				if(modeling != null && (lastSelectedGameObject == null || lastSelectedGameObject != modeling))
				{
					_currentSelectAnimation = 0;
					
					lastSelectedGameObject = modeling;
					
					currentSelectGameObject = modeling;
					
					update = true;
					
					Animation animation = currentSelectGameObject.GetComponent<Animation>();
					
					if ( animation != null )
					{
						List<string> anis = new List<string>();
						
						foreach ( AnimationState state in animation )
						{
							AnimationClip clip = state.clip;

							// 애니메이션에서는 공격 애니메이션만 뽑아낸다.
							if ( clip != null && clip.name.Contains("atk"))
							{
								anis.Add(clip.name);
							}
						}
						
						aniClips = anis.ToArray();
						
					}
					else
					{
						aniClips = null;
						modeling = null;
					}
				}
				
				if(aniClips == null || aniClips.Length == 0)
				{
					modeling = null;
				}

				// 애니메이션이 있으면 탭을 생성하고 애니메이션에 종속된 이펙트들을 설정할 수 있도록 만들어준다.
				if(modeling != null && aniClips != null)
				{
					float maxWidth = aniClips.Length * 100f;
					int a = GUILayout.Toolbar(_currentSelectAnimation, aniClips, GUILayout.MaxWidth(maxWidth));

					if(a != _currentSelectAnimation)
					{
						_currentSelectAnimation = a;
						_dummySliderValue = 0;
					}

					string aniName = aniClips[_currentSelectAnimation];

					if(aniName == "atk")
					{
						_atkIndex = 1;
						_customAtkName = null;
					}
					else
					{
						if(int.TryParse(aniName.Replace("atk",""), out _atkIndex))
						{
							_customAtkName = null; 
						}
						else
						{
							_customAtkName = aniName;
						}
					}

					EffectEditorData.EffectAniEditData ea =  EffectEditorData.getAniData(aniName, target.data);
					
					// 특정 동작이 없으면 동작을 추가시켜준다.
					if(ea == null)
					{
						EffectEditorData.EffectAniEditData newEffectAniEditData = new EffectEditorData.EffectAniEditData();
						newEffectAniEditData.aniName = aniName;
						target.data.Add(newEffectAniEditData);
						update = true;
						ea = newEffectAniEditData;
					}

					AniData ad = ea.aniData;
					ad.id = modeling.name;
					ad.ani = aniName;
					
					if(ad.effect == null) ad.effect = new AniDataEffect[0];


					mScroll = GUILayout.BeginScrollView(mScroll);


					if(dummy == null)
					{
						GUILayout.BeginVertical();

						// 한동작안에는 여러벌의 애니메이션이 등록될 수 있다.
						if(GUILayout.Button("더미 생성", GUILayout.Height(40f)))
						{
							createDummy(modeling);
						}


						GUILayout.EndVertical();
					}
					else
					{
						GUILayout.BeginVertical();

						if(GUILayout.Button("더미 삭제", GUILayout.Height(40f)))
						{
							deleteDummy();
						}
						GUILayout.EndVertical();

						GUILayout.BeginVertical();
						
						if(dummy != null && dummy.animation != null && dummy.animation.GetClip(aniName) != null)
						{
							float len = dummy.animation[aniName].length;
							
							float newTime = EditorGUILayout.Slider(_dummySliderValue, 0, len);
							
							if(Mathf.Approximately(newTime, _dummySliderValue) == false)
							{
								_dummySliderValue = newTime;
								goToDummyAnimation(aniName, _dummySliderValue);
							}

							EditorGUILayout.FloatField("프레임",Mathf.RoundToInt(newTime*30.0f));
						}

						GUILayout.EndVertical();
					}


					DrawSeperator(Color.black);
					GUILayout.Space(10f);





					GUILayout.BeginVertical();

					GUILayout.BeginHorizontal();

					if(GUILayout.Button("정렬", GUILayout.Height(40f), GUILayout.Width(50f)))
					{
						sortAniFrame = true;
					}

					if(GUILayout.Button("다시\n읽기", GUILayout.Height(40f), GUILayout.Width(80f)))
					{
						refreshTime = true;
						update = true;
					}


					// 한동작안에는 여러벌의 애니메이션이 등록될 수 있다.
					if(GUILayout.Button("애니메이션 이펙트 추가", GUILayout.Height(40f)))
					{
						AniDataEffect[] newAd = new AniDataEffect[ad.effect.Length+1];
						newAd[newAd.Length-1] = new AniDataEffect();
						newAd[newAd.Length-1].useThis = false;
						
						for(int i = 0; i < ad.effect.Length; ++i)
						{
							newAd[i] = ad.effect[i];								
						}
						ad.effect = newAd;
						
						update = true;
					}
					
					int deleteEffectIndex = -1;

					GUILayout.EndHorizontal();

					GUILayout.EndVertical();


//					aniScroll = GUILayout.BeginScrollView(aniScroll, GUILayout.MinHeight(150f), GUILayout.MaxHeight(250f));

					// 애니메이션 이펙트를 등록하는 부분이다.
					for(int j = 0; j < ad.effect.Length; ++j)
					{

						GUILayout.BeginVertical(GUILayout.Height(50f), GUILayout.Width(Screen.width));
						{
							DrawSeperator(Color.black);

							GUILayout.BeginHorizontal();

							if(ad.effect[j] == null) ad.effect[j] = new AniDataEffect();
							
							// 1. 프레임
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUIUtility.labelWidth = 40;
							
							int frameIndex = EditorGUILayout.IntField("프레임",ad.effect[j].frame, GUILayout.Width(80f));
							
							if(ad.effect[j].frame != frameIndex && frameIndex > 0)
							{
								ad.effect[j].frame = frameIndex;
								ad.effect[j].delay = ((float)ad.effect[j].frame)/30.0f;

								update = true;
							}

							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);



							// 1-1. Use
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							
							bool nUse = GUILayout.Toggle(ad.effect[j].useThis, "사용");
							
							if(ad.effect[j].useThis != nUse)
							{
								ad.effect[j].useThis = nUse;
								update = true;
							}
							
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);





							// 2. 타입
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUIUtility.labelWidth = 40;

							EditorGUILayout.LabelField("Type", GUILayout.Width(40f));
							AniDataEffect.PointType nP = (AniDataEffect.PointType)EditorGUILayout.EnumPopup(ad.effect[j].shotPoint, GUILayout.Width(120f));
							
							if(ad.effect[j].shotPoint != nP)
							{
								ad.effect[j].shotPoint = nP;
								update = true;
							}


							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);


							bool repalceEffectGameObject = false;

							// 3. 이펙트.
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUIUtility.labelWidth = 50;
							EditorGUILayout.LabelField("이펙트", GUILayout.Width(80f));
							GameObject nGo = (GameObject)EditorGUILayout.ObjectField(ad.effect[j].goEffect, typeof(GameObject), false, GUILayout.Width(200f));
							
							if(ad.effect[j].goEffect != nGo)
							{
								if(nGo != null && checkAnimationEffectParticle(nGo))
								{
									ad.effect[j].goEffect = nGo;
									ad.effect[j].id = "E_"+nGo.name.ToUpper();

									repalceEffectGameObject = true;
									update = true;
								}
								else if(nGo == null)
								{
									ad.effect[j].useThis = false;
									ad.effect[j].goEffect = nGo;
									repalceEffectGameObject = true;
									update = true;
								}
							}


							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);
							

							// 4. 위치 
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUIUtility.labelWidth = 50;

							Vector3 oriPos; oriPos.x = ad.effect[j].x; oriPos.y = ad.effect[j].y; oriPos.z = ad.effect[j].z;
							Vector3 newPos = EditorGUILayout.Vector3Field("위치", oriPos, GUILayout.Width(200f));
							
							if(Vector3.Equals(oriPos, newPos) == false)
							{
								ad.effect[j].x = newPos.x; ad.effect[j].y = newPos.y; ad.effect[j].z = newPos.z;
								update = true;
							}

							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);


							// 5. 회전 
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUIUtility.labelWidth = 50;

							Vector3 oriRot; oriRot.x = ad.effect[j].rx; oriRot.y = ad.effect[j].ry; oriRot.z = ad.effect[j].rz;
							Vector3 newRot = EditorGUILayout.Vector3Field("회전",oriRot, GUILayout.Width(200f));
							
							if(Vector3.Equals(oriRot, newRot) == false)
							{
								ad.effect[j].rx = newRot.x;
								ad.effect[j].ry = newRot.y;
								ad.effect[j].rz = newRot.z;

								if(Mathf.RoundToInt(newRot.x) != 0 ||
								   Mathf.RoundToInt(newRot.y) != 0 ||
								   Mathf.RoundToInt(newRot.z) != 0)
								{
									ad.effect[j].useCustomRotation = true;
								}
								else
								{
									ad.effect[j].useCustomRotation = false;
								}

								update = true;
							}
							
							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();
							GUILayout.Space(5f);


							if(ad.effect[j].goEffect != null)
							{
								if(repalceEffectGameObject || refreshTime)
								{
									Transform tfEffect = ad.effect[j].goEffect.transform;
									Transform tfChild;
									
									int cc = tfEffect.childCount;
									
									ad.effect[j].checkType = AniDataEffect.EffectType.WrongName;

									// 애니메이션에 붙이는 이펙트의 경우는 최소한 파티클이 있던가 애니메이션이 있어야한다. 
									for(int i = 0; i < cc; ++i)
									{
										tfChild = tfEffect.GetChild(i);
										if(tfChild.name == tfEffect.name)
										{
											// 파티클 이펙트가 있으면...
											if(tfChild.particleSystem != null)
											{
												ad.effect[j].checkType = AniDataEffect.EffectType.Particle;

												if(tfChild.particleSystem.loop == false)
												{
													ad.effect[j].timeLimit = tfChild.particleSystem.duration;
													ad.effect[j].timeLimitDefault = ad.effect[j].timeLimit;
												}
												else
												{
													ad.effect[j].timeLimit = -1;
													ad.effect[j].timeLimitDefault = ad.effect[j].timeLimit;
												}

												break;
											}
											else if(tfChild.animation != null && tfChild.animation.clip != null)
											{
												ad.effect[j].checkType = AniDataEffect.EffectType.MeshAnimation;

												if(tfChild.animation.clip.wrapMode != WrapMode.Loop)
												{
													ad.effect[j].timeLimit = tfChild.animation.clip.length;
													ad.effect[j].timeLimitDefault = ad.effect[j].timeLimit;
												}
												else
												{
													ad.effect[j].timeLimit = -1;
													ad.effect[j].timeLimitDefault = ad.effect[j].timeLimit;
												}
												break;	
											}

											break;
										}
									}

									update = true;
								}

								if(ad.effect[j].checkType == AniDataEffect.EffectType.WrongName)
								{
									// 이펙트 하위 이름이 다름.
									GUILayout.BeginVertical();
									GUILayout.FlexibleSpace();
									EditorGUIUtility.labelWidth = 150f;
									// 삭제시간.
									EditorGUILayout.LabelField("이펙트 이름이 달라요!!!");
									
									GUILayout.FlexibleSpace();
									GUILayout.EndVertical();
									GUILayout.Space(5f);
								}
								else if(ad.effect[j].checkType != AniDataEffect.EffectType.None)
								{
									// 6. 삭제 시간. 
									GUILayout.BeginVertical();
									GUILayout.FlexibleSpace();
									EditorGUIUtility.labelWidth = 80;

//									if(ad.effect[j].timeLimit > 0 && ad.effect[j].timeLimit  < 9999)
									{
										if(ad.effect[j].checkType == AniDataEffect.EffectType.Particle)
										{

											if((ad.effect[j].timeLimit > 0 && ad.effect[j].timeLimit  < 9999) == false)
											{
												EditorGUILayout.LabelField("LOOP");
											}

											float pTLimit = EditorGUILayout.FloatField("파티클 삭제: ",ad.effect[j].timeLimit, GUILayout.Width(120f));

											if(Mathf.Approximately( ad.effect[j].timeLimit, pTLimit) == false && pTLimit > 0)
											{
												ad.effect[j].timeLimit = pTLimit;
												update = true;
											}

											if(Mathf.Approximately( ad.effect[j].timeLimit, ad.effect[j].timeLimitDefault) == false)
											{
												if(GUILayout.Button("기본값"))
												{
													ad.effect[j].timeLimit = ad.effect[j].timeLimitDefault;
													update = true;
												}
											}

										}
										else if(ad.effect[j].checkType == AniDataEffect.EffectType.MeshAnimation)
										{
											float aTLimit = EditorGUILayout.FloatField("애니 삭제: ",ad.effect[j].timeLimit, GUILayout.Width(120f));

											if(Mathf.Approximately( ad.effect[j].timeLimit, aTLimit) == false && aTLimit > 0)
											{
												ad.effect[j].timeLimit = aTLimit;
												update = true;
											}

											if(Mathf.Approximately( ad.effect[j].timeLimit, ad.effect[j].timeLimitDefault) == false)
											{
												if(GUILayout.Button("기본값"))
												{
													ad.effect[j].timeLimit = ad.effect[j].timeLimitDefault;
													update = true;
												}
											}

										}
									}
//									else
//									{
//										EditorGUILayout.LabelField("LOOP");
//									}

									/*
									if(Mathf.Approximately(delTime, ad.effect[j].timeLimit) == false)
									{
										ad.effect[j].timeLimit = delTime;
										update = true;
									}
									*/
									
									GUILayout.FlexibleSpace();
									GUILayout.EndVertical();
									GUILayout.Space(5f);
								}
							}


							if(nP == AniDataEffect.PointType.AttachedTransform)
							{
								// 7. 본 :  있을 때만.
								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								EditorGUIUtility.labelWidth = 50;

								EditorGUILayout.LabelField("본 이름", GUILayout.Width(50));
								string np = (string)EditorGUILayout.TextField(ad.effect[j].parent, GUILayout.Width(100f));
								
								if(ad.effect[j].parent != np)
								{
									ad.effect[j].parent = np;
									update = true;
								}

								GUILayout.FlexibleSpace();
								GUILayout.EndVertical();
								GUILayout.Space(20f);
							}


							GUILayout.FlexibleSpace();



							// 9. 삭제 버튼..
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();

							if( GUILayout.Button("삭제", GUILayout.Width(80f)) && EditorUtility.DisplayDialog("진짜 삭제할거예요?", "복구 못합니다.","삭제", "취소"))
							{
								deleteEffectIndex = j;
								update = true;
							}

							GUILayout.FlexibleSpace();
							GUILayout.EndVertical();

							GUILayout.EndHorizontal();

							GUILayout.FlexibleSpace();
						}
						GUILayout.EndVertical();
					}

//					GUILayout.EndScrollView();

					if(deleteEffectIndex > -1) ad.effect = Util.removeElementFromArray<AniDataEffect>(ad.effect,deleteEffectIndex);
					
					if(sortAniFrame && ad.effect != null && ad.effect.Length > 1)
					{
						_aniFrameList.AddRange(ad.effect);
						_aniFrameList.Sort(AniDataEffect.sortByFrame);
						ad.effect = _aniFrameList.ToArray();
						_aniFrameList.Clear();
					}

					GUILayout.BeginVertical();
					//============= 애니메이션 이펙트 끝. ==============/
					DrawSeperator(Color.black);
					GUILayout.Space(10f);
					GUILayout.EndVertical();


					EffectEditorData.EffectEditBulletData bd = ea.bulletData;

					if(_atkIndex == 2 && bd.use) // 2번은 탄을 못날린다. 탄은 3번부터 날리는게 가능하다.
					{
						bd.use = false;
						update = true;
					}

					//============= 탄환 삽입 ==============/
					GUILayout.BeginVertical();

					// 한동작안에는 여러벌의 애니메이션이 등록될 수 있다.
					if(bd.use == false && GUILayout.Button("탄환 사용", GUILayout.Height(40f)))
					{
						bd.use = true;
						update = true;
					}
					else if(bd.use && GUILayout.Button("탄환 제거", GUILayout.Height(40f)))
					{
						bd.use = false;
						update = true;
					}
					GUILayout.EndVertical();


					GUILayout.BeginVertical(GUILayout.Height(60f),  GUILayout.Width(Screen.width));

					if(bd.setAtkOption == false)
					{
						bd.setAtkOption = true;

						Util.setListValue(bd.atkTypeOptions(0));
						Util.setListValue(bd.atkTypeOptions(1));
						Util.setListValue(bd.atkTypeOptions(2), 70,50,3,0);
						Util.setListValue(bd.atkTypeOptions(3), 1500,500,0,0);
						Util.setListValue(bd.atkTypeOptions(4), 1500,500,100,50,3);
						Util.setListValue(bd.atkTypeOptions(5), 1500,300,3,11,80,50,0);
						Util.setListValue(bd.atkTypeOptions(6), 700,100,50,2,0,0);
						Util.setListValue(bd.atkTypeOptions(7), 500,600,100,4,60,0);
						Util.setListValue(bd.atkTypeOptions(8), 200,100,3,1000);
						Util.setListValue(bd.atkTypeOptions(9), 5,11,400,400,300);
						Util.setListValue(bd.atkTypeOptions(10),5,2500,22,400,0,0);
						Util.setListValue(bd.atkTypeOptions(11),2500,5,2000,22,100,0,0);
						Util.setListValue(bd.atkTypeOptions(12),4,2000,11,300,400);
						Util.setListValue(bd.atkTypeOptions(13),1000,200,100,3);
						Util.setListValue(bd.atkTypeOptions(14),1000,100,5,5000);
						Util.setListValue(bd.atkTypeOptions(15),800,700,300,5,200,30);
						Util.setListValue(bd.atkTypeOptions(16),2000,500,1);
						Util.setListValue(bd.atkTypeOptions(17),500,300,50,100,60,500,505);
						Util.setListValue(bd.atkTypeOptions(18),2000, 800, 3);
						update = true;
					}




					// 탄을 사용할때는.
					if(bd.use)
					{
						GUILayout.BeginHorizontal();
						
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							EditorGUILayout.LabelField("공격타입", GUILayout.Width(80f));

							int atkType = (int)EditorGUILayout.Popup(bd.attackType, EffectEditorData.EffectEditBulletData.atkTypeList);
							
							// 원래는 애니메이션 타입에 맞춰서 설정할 수 있는 총알 모양을 강제하려고 했는데 의미가 없는것 같다...
							//if(_atkIndex > 1) atkType = _atkIndex;
							
							if(bd.attackType != atkType)
							{
								bd.attackType = atkType;
								update = true;
							}
							
							GUILayout.EndVertical();
							GUILayout.Space(5f);
						
						
						
						if(atkType == 15)
						{
								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								
								EditorGUILayout.LabelField("체인라이트닝", GUILayout.Width(80f));
								ElectricEffect cl = (ElectricEffect)EditorGUILayout.ObjectField(bd.bulletEffect.chainLighting, typeof(ElectricEffect), false, GUILayout.Width(200f));
								
								if(cl != bd.bulletEffect.chainLighting)
								{
									bd.bulletEffect.chainLighting = cl;
									update = true;
								}
								
								GUILayout.EndVertical();


								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								
								EditorGUILayout.LabelField("타격이펙트", GUILayout.Width(80f));
								GameObject clHit = (GameObject)EditorGUILayout.ObjectField(bd.bulletEffect.goHitEffect, typeof(GameObject), false, GUILayout.Width(200f));
								
								if(clHit != bd.bulletEffect.goHitEffect)
								{
									if(clHit != null)
									{
										if(checkAnimationEffectParticle(clHit))
										{
											bd.bulletEffect.goHitEffect = clHit;
											update = true;
										}
									}
									else
									{
										bd.bulletEffect.goHitEffect = clHit;
										update = true;
									}
								}
								GUILayout.EndVertical();
						}
						else if(atkType >= 3 )
						{
							if(_atkIndex > 1) atkType = _atkIndex;
							
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							
							// 탄환 옵션.
							EditorGUILayout.LabelField("탄환 옵션", GUILayout.Width(100f));
							EffectEditorData.EffectEditBulletDetailData.Type bOption = (EffectEditorData.EffectEditBulletDetailData.Type)EditorGUILayout.EnumPopup(bd.bulletEffect.type);
							
							if(bd.bulletEffect.type != bOption)
							{
								bd.bulletEffect.type = bOption;
							}
							
							GUILayout.EndVertical();
							
							
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							
							EditorGUILayout.LabelField("탄에 붙임", GUILayout.Width(100f));
							bool bap = EditorGUILayout.Toggle(bd.bulletEffect.attachedToParent);
							
							if(bd.bulletEffect.attachedToParent != bap)
							{
								bd.bulletEffect.attachedToParent = bap;
								update = true;
							}
							
							GUILayout.EndVertical();
							
							
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();


							EditorGUILayout.LabelField("추가옵션", GUILayout.Width(100f));
							bool bop = EditorGUILayout.Toggle(bd.bulletEffect.useOption);
							
							if(bd.bulletEffect.useOption != bop)
							{
								bd.bulletEffect.useOption = bop;
								update = true;
							}
							
							GUILayout.EndVertical();
							
							
							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();



							EditorGUILayout.LabelField("탄환이펙트", GUILayout.Width(80f));
							GameObject bullet = (GameObject)EditorGUILayout.ObjectField(bd.bulletEffect.effect, typeof(GameObject), false, GUILayout.Width(200f));
							
							if(bullet != bd.bulletEffect.effect)
							{
								bool canInput = true;
								
								if(bullet != null)
								{
									// 파티클 계열이면...
									if( bd.bulletEffect.type != EffectEditorData.EffectEditBulletDetailData.Type.Object)
									{
										if(checkAnimationEffectParticle(bullet, true, false) == false)
										{
											canInput = false;
										}
									}
								}
								
								if(canInput)
								{
									bd.bulletEffect.effect = bullet;
									update = true;
								}
							}
							
							GUILayout.EndVertical();

							GUILayout.BeginVertical();
							GUILayout.FlexibleSpace();
							
							EditorGUILayout.LabelField("타격이펙트", GUILayout.Width(80f));
							GameObject clHit = (GameObject)EditorGUILayout.ObjectField(bd.bulletEffect.goHitEffect, typeof(GameObject), false, GUILayout.Width(200f));
							
							if(clHit != bd.bulletEffect.goHitEffect)
							{
								if(clHit != null)
								{
									if(checkAnimationEffectParticle(clHit))
									{
										bd.bulletEffect.goHitEffect = clHit;
										update = true;
									}
								}
								else
								{
									bd.bulletEffect.goHitEffect = clHit;
									update = true;
								}
							}
							GUILayout.EndVertical();


							if(atkType == 7 || atkType == 17)
							{
								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								
								EditorGUILayout.LabelField("바닥이펙트", GUILayout.Width(80f));
								GameObject groundEff = (GameObject)EditorGUILayout.ObjectField(bd.bulletEffect.goGroundEffect, typeof(GameObject), false, GUILayout.Width(200f));
								
								if(groundEff != bd.bulletEffect.goGroundEffect)
								{
									if(groundEff != null)
									{
										if(checkAnimationEffectParticle(groundEff))
										{
											bd.bulletEffect.goGroundEffect = groundEff;
											update = true;
										}
									}
									else
									{
										bd.bulletEffect.goGroundEffect = groundEff;
										update = true;
									}
								}
								GUILayout.EndVertical();
							}
							else
							{
								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								
								EditorGUILayout.LabelField("폭파이펙트", GUILayout.Width(80f));
								GameObject destroyEff = (GameObject)EditorGUILayout.ObjectField(bd.bulletEffect.goDestroyEffect, typeof(GameObject), false, GUILayout.Width(200f));
								
								if(destroyEff != bd.bulletEffect.goDestroyEffect)
								{
									if(destroyEff != null)
									{
										if(checkAnimationEffectParticle(destroyEff))
										{
											bd.bulletEffect.goDestroyEffect = destroyEff;
											update = true;
										}
									}
									else
									{
										bd.bulletEffect.goDestroyEffect = destroyEff;
										update = true;
									}
								}
								GUILayout.EndVertical();


								GUILayout.BeginVertical();
								GUILayout.FlexibleSpace();
								EditorGUILayout.LabelField("폭파 옵션", GUILayout.Width(100f));
								EffectEditorData.EffectEditBulletDetailData.DestroyEffType dOption = (EffectEditorData.EffectEditBulletDetailData.DestroyEffType)EditorGUILayout.EnumPopup(bd.bulletEffect.destroyOption);
								
								if(bd.bulletEffect.destroyOption != dOption)
								{
									bd.bulletEffect.destroyOption = dOption;
									update = true;
								}
								
								GUILayout.EndVertical();
								GUILayout.Space(5f);

							}
						}
						
						GUILayout.EndHorizontal();


						GUILayout.BeginHorizontal();




						if( GUILayout.Button("기본값", GUILayout.Width(100.0f) ))
						{
							if(atkType == 0) Util.setListValue(bd.atkTypeOptions(0));
							else if(atkType == 1) Util.setListValue(bd.atkTypeOptions(1));
							else if(atkType == 2) Util.setListValue(bd.atkTypeOptions(2), 70,50,3,0);
							else if(atkType == 3) Util.setListValue(bd.atkTypeOptions(3), 1500,500,0,0);
							else if(atkType == 4) Util.setListValue(bd.atkTypeOptions(4), 1500,500,100,50,3);
							else if(atkType == 5) Util.setListValue(bd.atkTypeOptions(5), 1500,300,3,11,80,50,0);
							else if(atkType == 6) Util.setListValue(bd.atkTypeOptions(6), 700,100,50,2,0,0);
							else if(atkType == 7) Util.setListValue(bd.atkTypeOptions(7), 500,600,100,4,60,0);
							else if(atkType == 8) Util.setListValue(bd.atkTypeOptions(8), 200,100,3,1000);
							else if(atkType == 9) Util.setListValue(bd.atkTypeOptions(9), 5,11,400,400,300);
							else if(atkType == 10) Util.setListValue(bd.atkTypeOptions(10),5,2500,22,400,0,0);
							else if(atkType == 11) Util.setListValue(bd.atkTypeOptions(11),2500,5,2000,22,100,0,0);
							else if(atkType == 12) Util.setListValue(bd.atkTypeOptions(12),4,2000,11,300,400);
							else if(atkType == 13) Util.setListValue(bd.atkTypeOptions(13),1000,200,100,3);
							else if(atkType == 14) Util.setListValue(bd.atkTypeOptions(14),1000,100,5,5000);
							else if(atkType == 15) Util.setListValue(bd.atkTypeOptions(15),800,700,300,5,200,30);
							else if(atkType == 16) Util.setListValue(bd.atkTypeOptions(16),2000,500,1);
							else if(atkType == 17) Util.setListValue(bd.atkTypeOptions(17),500,300,50,100,60,500,505);
							else if(atkType == 18) Util.setListValue(bd.atkTypeOptions(18),2000, 800, 3);
							update = true;
						}


						bool valueUpdate = false;

						switch(atkType)
						{
						case 3: //TYPE 3. 직선발사 단일공격 // 최대비행거리	  비행속도	유도탄비행(1:ON)	타임리밋
							//bd.atkTypeOptions(3][0] 
							drawBulletValue("비행속도",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);

							drawBulletValue("유도탄(0:X, 1:O)",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);

							if(bd.atkTypeOptions(atkType)[2] == 1)
							{
								drawBulletValue("유도탄시간",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);
							}

							break;
						case 4: // 최대비행거리	비행속도	 데미지범위	최소데미지비율	최대피격유닛수

							drawBulletValue("비행속도",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);

							break;
						case 5: //최대비행거리	비행속도	최대피격유닛수	이펙트타입	이펙트Z/R	최소데미지비율

							drawBulletValue("비행속도",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);

							break;
						case 6: // 타임리밋	데미지범위	최소데미지비율	최대피격유닛수	최대타겟거리

							drawBulletValue("타임리밋",bd.atkTypeOptions(atkType),0, valueUpdate,out valueUpdate);

							break;
						case 7: // 타임리밋	데미지범위	최소데미지비율	최대피격유닛수	사선 각도	최대타겟거리

							drawBulletValue("시간제한",bd.atkTypeOptions(atkType),0, valueUpdate,out valueUpdate);
							drawBulletValue("사선각도",bd.atkTypeOptions(atkType),4, valueUpdate,out valueUpdate);

							break;
						case 8: //데미지범위	최소데미지비율	최대피격유닛수	최대타겟거리


							break;
						case 9: //최대피격유닛수	이펙트타입	이펙트Z/R	이펙트X	최대타겟거리

							break;
						case 10: // 최대피격유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X	최대타겟거리

							drawBulletValue("지속시간",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);

							break;
						case 11: //타임리밋	최대피격유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X	최대타겟거리

							drawBulletValue("타임리밋",bd.atkTypeOptions(atkType),0, valueUpdate,out valueUpdate);
							drawBulletValue("지속시간",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);

							break;
						case 12: //최대피격유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
							drawBulletValue("지속시간",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);
							break;
						case 13: //폭발대기시간	데미지범위	최소데미지비율	최대피격유닛수

							break;
						case 14: // 데미지범위	최소데미지비율	최대피격유닛수	자연폭발시간

							break;
						case 15: // 최대전체거리	최대연결거리A	최대연결거리B	최대연결유닛수	연결딜레이	최소데미지비율
							drawBulletValue("최대전체거리",bd.atkTypeOptions(atkType),0, valueUpdate,out valueUpdate);
							drawBulletValue("최대연결거리A",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);
							drawBulletValue("최대연결거리B",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);
							drawBulletValue("최대연결유닛수",bd.atkTypeOptions(atkType),3, valueUpdate,out valueUpdate);
							drawBulletValue("연결딜레이",bd.atkTypeOptions(atkType),4, valueUpdate,out valueUpdate);
							break;
						case 16: // 최대데미지거리	이펙트 지속시간
							drawBulletValue("지속시간",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);
							drawBulletValue("위치보정",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);
							break;
						case 17: // 타임리밋	데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격

							drawBulletValue("타임리밋",bd.atkTypeOptions(atkType),0, valueUpdate,out valueUpdate);
							drawBulletValue("사선각도",bd.atkTypeOptions(atkType),4, valueUpdate,out valueUpdate);
							drawBulletValue("낙하범위",bd.atkTypeOptions(atkType),5, valueUpdate,out valueUpdate);
							drawBulletValue("횟수/간격",bd.atkTypeOptions(atkType),6, valueUpdate,out valueUpdate);
							break;
						case 18: // 최대비행거리	비행속도	 탄환개수	
							drawBulletValue("비행속도",bd.atkTypeOptions(atkType),1, valueUpdate,out valueUpdate);
							drawBulletValue("탄환개수",bd.atkTypeOptions(atkType),2, valueUpdate,out valueUpdate);
							break;
						}

						if(valueUpdate)
						{
							update = true;
						}

						GUILayout.EndHorizontal();


						// 공격타입 3~18번.
						GUILayout.Space(10f);
						DrawSeperator(Color.gray);
						GUILayout.Space(10f);

						float boxNum = 2f;

						if(atkType == 15)
						{
							boxNum = 3f;
						}

						GUILayout.BeginHorizontal();

						GUILayout.BeginVertical(GUILayout.Width(Screen.width/boxNum - 10f));

						int deleteActionFrame = -1;
						int deleteShotPos = -1;
						int deleteBone = -1;

						GUILayout.BeginVertical();
						{

							GUILayout.BeginHorizontal();
							
							if(GUILayout.Button("정렬", GUILayout.Width(50f)))
							{
								bd.actionFrame.Sort();
							}

							if(GUILayout.Button("액션 프레임 추가"))
							{
								bd.actionFrame.Add(1);
								bd.actionFrame.Sort();
								update = true;
							}

							GUILayout.EndHorizontal();

							for(int i = 0; i < bd.actionFrame.Count; ++i)
							{
								
								{
									GUILayout.BeginHorizontal();
									{
										GUILayout.BeginVertical();
										{
											int ac = EditorGUILayout.IntField(bd.actionFrame[i]);
											
											if(ac != bd.actionFrame[i])
											{
												bd.actionFrame[i] = ac;
												update = true;
											}
											GUILayout.EndVertical();
										}
										
										GUILayout.BeginVertical();
										{
											if(GUILayout.Button("삭제"))
											{
												deleteActionFrame = i;
											}
										}
										GUILayout.EndVertical();
									}
									GUILayout.EndHorizontal();
								}
							}

						}
						GUILayout.EndVertical();


						if(deleteActionFrame >= 0)
						{
							bd.actionFrame.RemoveAt(deleteActionFrame);
							bd.actionFrame.Sort();
							update = true;
						}

						GUILayout.EndVertical();


						GUILayout.BeginVertical(GUILayout.Width(Screen.width/boxNum - 10f));
						
						if(GUILayout.Button("발사위치 추가"))
						{
							bd.shotPoint.Add(Vector3.zero);
							update = true;
						}
						
						for(int i = 0; i < bd.shotPoint.Count; ++i)
						{
							
							{
								GUILayout.BeginHorizontal();
								{
									GUILayout.BeginVertical();
									{
										Vector3 sv = EditorGUILayout.Vector3Field("",bd.shotPoint[i]);
										
										if(Vector3.Equals(sv, bd.shotPoint[i]) == false)
										{
											bd.shotPoint[i] = sv;
											update = true;
										}
										GUILayout.EndVertical();
									}
									
									GUILayout.BeginVertical();
									{
										if(GUILayout.Button("삭제"))
										{
											deleteShotPos = i;
										}
									}
									GUILayout.EndVertical();
								}
								GUILayout.EndHorizontal();
							}
						}
						
						if(deleteShotPos >= 0)
						{
							bd.shotPoint.RemoveAt(deleteShotPos);
							update = true;
						}
						
						GUILayout.EndVertical();




						if(atkType == 15)
						{
							GUILayout.BeginVertical(GUILayout.Width(Screen.width/boxNum - 10f));
							
							if(GUILayout.Button("붙일 본 추가"))
							{
								bd.targetTransform.Add(string.Empty);
								update = true;
							}
							
							for(int i = 0; i < bd.targetTransform.Count; ++i)
							{
								
								{
									GUILayout.BeginHorizontal();
									{
										GUILayout.BeginVertical();
										{
											string t = EditorGUILayout.TextField(bd.targetTransform[i]);
											
											if(t != bd.targetTransform[i])
											{
												bd.targetTransform[i] = t;
												update = true;
											}
											GUILayout.EndVertical();
										}
										
										GUILayout.BeginVertical();
										{
											if(GUILayout.Button("삭제"))
											{
												deleteBone = i;
											}
										}
										GUILayout.EndVertical();
									}
									GUILayout.EndHorizontal();
								}
							}
							
							if(deleteBone >= 0)
							{
								bd.targetTransform.RemoveAt(deleteBone);
								update = true;
							}
							
							GUILayout.EndVertical();
						}








						GUILayout.FlexibleSpace();

						GUILayout.EndHorizontal();

						// action frame 지정. // int 배열.
						// shot_pos 지정. // vector3 배열.
						// shooting_point 지정. // vector3 배열.
						
						// 실제 탄환모양.
					}

					GUILayout.FlexibleSpace();
					GUILayout.EndVertical();




					GUILayout.BeginVertical();
					//============= 애니메이션 이펙트 끝. ==============/
					DrawSeperator(Color.black);
					GUILayout.Space(100f);
					GUILayout.EndVertical();


					GUILayout.EndScrollView();

					int type = 1;
					int playAniType = 0;

					GUILayout.BeginHorizontal();

					//if(Application.isPlaying && GameManager.me != null)
					{

						EditorGUIUtility.labelWidth = 30f;
						EditorGUILayout.LabelField("아군위치");
						float px = EditorGUILayout.FloatField(target.unitX, GUILayout.MaxWidth(80f), GUILayout.MinWidth(80f));
						
						if(px != target.unitX)
						{
							target.unitX = px;
							update = true;
						}

						EditorGUIUtility.labelWidth = 30f;
						EditorGUILayout.LabelField("적군위치");
						float ex = EditorGUILayout.FloatField(target.enemyX, GUILayout.MaxWidth(80f), GUILayout.MinWidth(80f));
						
						if(ex != target.enemyX)
						{
							target.enemyX = ex;
							update = true;
						}

						if(GUILayout.Button("위치변경(Transform)"))
						{
							if(target != null && UnitSkillCamMaker.instance != null)
							{
								if(Application.isPlaying && GameManager.me != null) 
								{
									UnitSkillCamMaker.instance.repositionAll();
								}
							}
						}

						if(GUILayout.Button("캐릭터 생성"))
						{
							if(target != null && UnitSkillCamMaker.instance != null)
							{
								if(Application.isPlaying && GameManager.me != null) UnitSkillCamMaker.instance.createNewMonster(target.modeling, px, ex);
							}
						}

						if(GUILayout.Button("캐릭터 삭제"))
						{
							if(target != null && UnitSkillCamMaker.instance != null)
							{
								if(Application.isPlaying && GameManager.me != null) UnitSkillCamMaker.instance.deleteHeroMonster();;
							}
						}


						if(GUILayout.Button("모두 죽이기"))
						{
							if(target != null && UnitSkillCamMaker.instance != null)
							{
								if(Application.isPlaying && GameManager.me != null)
								{
									UnitSkillCamMaker.instance.deleteUnit();
									UnitSkillCamMaker.instance.killMonsterUnits(true);
								}
							}
						}

						if(GUILayout.Button("히어로 On/Off"))
						{
							GameManager.me.player.setVisible(!GameManager.me.player.isVisible);
						}

						GUILayout.FlexibleSpace();

						if(GUILayout.Button("아군 플레이"))
						{
							playAniType = 1;
						}

						if(GUILayout.Button("적군 플레이"))
						{
							playAniType = 2;
						}

					}

					GUILayout.EndHorizontal();

					if(Application.isPlaying && GameManager.me != null)
					{
						if(playAniType > 0)
						{
							if(UnitSkillCamMaker.instance != null)
							{
								UnitSkillCamMaker.instance.changeAni(_atkIndex, (playAniType == 1), ad, bd, _customAtkName);
							}
							
						}
					}



				}
				
				if(target != null && modeling != target.modeling)
				{
					target.modeling = modeling;
				}
			}


			GUILayout.FlexibleSpace();


			GUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("비율", GUILayout.Width(100f));

			effectResizeRatio = EditorGUILayout.FloatField(effectResizeRatio);

			if(GUILayout.Button("이펙트 사이즈 변경 (파티클만)"))
			{
				ParticleResizeUtil2.resize(Selection.activeGameObject, effectResizeRatio);
			}

			if(GUILayout.Button("이펙트 사이즈 변경 (Transfrom까지) "))
			{
				if(Selection.activeGameObject != null)
				{
					GameObject goNewEff = (GameObject)Instantiate(Selection.activeGameObject);
					if(goNewEff != null)
					{
						Util.resizeEffect(goNewEff, Selection.activeGameObject.name, effectResizeRatio);
					}
				}
			}

			if(GUILayout.Button("타이머 붙이기"))
			{
				AddAutoDestructionTimer();
			}

			GUILayout.EndHorizontal();

			GUILayout.Space(5f);

			DrawSeperator(Color.gray);

			GUILayout.Space(5f);

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			if(GUILayout.Button("중복입력에러 수정", GUILayout.Height(50f), GUILayout.Width(150f)))
			{
				lastSelectedGameObject = null;
			}

			if(GUILayout.Button("정보파일 입력", GUILayout.Height(50f), GUILayout.Width(150f)))
			{
				importData();
			}

			if(GUILayout.Button("정보파일 출력", GUILayout.Height(50f)))
			{
				exportData();
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();



			if(update)
			{
				if(target != null) target.SetDirty();

				if(sortAniFrame)
				{
					Repaint();
				}
			}
		}
	}


	void importData()
	{
		if(target == null && target.modeling == null)
		{
			return;
		}

		string prefabPath = AssetDatabase.GetAssetPath(target.gameObject.GetInstanceID());
		
		prefabPath = prefabPath.Substring(0,prefabPath.LastIndexOf("."))+".txt";

		if(File.Exists(prefabPath) == false)
		{
//			EditorUtility.DisplayDialog("",prefabPath + "에 정보파일이 없습니다.","확인");

			string p = AssetDatabase.GetAssetPath(target.gameObject.GetInstanceID());
			p = p.Substring(0,p.LastIndexOf("/"));

			prefabPath = EditorUtility.OpenFilePanel(
				"정보파일을 선택하세요.",
				p,
				"txt");

		}

		string temp = File.ReadAllText(prefabPath);

		string[] data = temp.Split("\r\n".ToCharArray());

		string[] d;

		string currentAni = string.Empty;

		List<AniDataEffect> adeList = new List<AniDataEffect>();

		for(int i = 0; i < data.Length; ++i)
		{
			string s = data[i];

			if(s.StartsWith("Name:"))
			{
				d = parseImportData(s);
				currentAni = string.Empty;
			}
			else if(s.StartsWith("Scale:"))
			{
				d = parseImportData(s);
				float.TryParse(d[1],out target.scaleFactor);
				currentAni = string.Empty;
			}
			else if(s.StartsWith("[ AniName:"))
			{
				d = parseImportData(s);
				currentAni = d[1];

				foreach(EffectEditorData.EffectAniEditData aniData in target.data)
				{
					if(aniData.aniName == currentAni)
					{
						foreach(AniDataEffect a in aniData.aniData.effect)
						{
							if(a.useThis == false)
							{
								adeList.Add(a);
							}
						}

						break;
					}
				}



			}
			else if(currentAni != string.Empty)
			{
				if(s.StartsWith("* effName"))
				{
					AniDataEffect ade = new AniDataEffect();

					d = parseImportData(s);
					ade.goEffect = (GameObject)AssetDatabase.LoadAssetAtPath(d[2], typeof(GameObject));

					ade.id = "E_"+ade.goEffect.name.ToUpper();

					d = parseImportData(data[i+2]);

					string[] logic = d[1].Split(',');

					ade.setLogicString(target.scaleFactor, logic);

					adeList.Add(ade);
					//target.data
				}
				else if(s.StartsWith("---- end effect info -"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni)
						{
							aniData.aniData.effect = adeList.ToArray();
							adeList.Clear();
						}
					}
				}
				else if(s.StartsWith("* 탄환"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2].Contains("Assets"))
							{
								d = parseImportData(data[i+2]);
								aniData.bulletData.bulletEffect.effect = (GameObject)AssetDatabase.LoadAssetAtPath(d[1], typeof(GameObject));
							}
							else if(data[i+2].Contains("빈 탄환"))
							{
								aniData.bulletData.bulletEffect.effect = null;
							}
							break;
						}
					}


				}
				else if(s.StartsWith("* 체인라이트닝"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2].Contains("Assets"))
							{
								d = parseImportData(data[i+2]);
								aniData.bulletData.bulletEffect.chainLighting = (ElectricEffect)AssetDatabase.LoadAssetAtPath(d[1], typeof(ElectricEffect));
							}
							break;
						}
					}
				}
				else if(s.StartsWith("* 타격이펙트"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2].Contains("Assets"))
							{
								d = parseImportData(data[i+2]);
								aniData.bulletData.bulletEffect.goHitEffect = (GameObject)AssetDatabase.LoadAssetAtPath(d[1], typeof(GameObject));

							}
							break;
						}
					}
				}
				else if(s.StartsWith("* 폭파이펙트 :"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2].Contains("Assets"))
							{
								d = parseImportData(data[i+2]);
								aniData.bulletData.bulletEffect.goDestroyEffect = (GameObject)AssetDatabase.LoadAssetAtPath(d[1], typeof(GameObject));
								
							}
							break;
						}
					}
				}
				else if(s.StartsWith("* 폭파이펙트 옵션"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2] == "BR")
							{
								aniData.bulletData.bulletEffect.destroyOption = EffectEditorData.EffectEditBulletDetailData.DestroyEffType.BulletRotation;
							}
							else 
							{
								aniData.bulletData.bulletEffect.destroyOption = EffectEditorData.EffectEditBulletDetailData.DestroyEffType.Normal;
							}
							break;
						}
					}
				}
				else if(s.StartsWith("* 이펙트 목록 추가"))
				{
					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni && aniData.bulletData.use)
						{
							if(data[i+2].Contains("Assets"))
							{
								d = parseImportData(data[i+2]);
								aniData.bulletData.bulletEffect.goGroundEffect = (GameObject)AssetDatabase.LoadAssetAtPath(d[1], typeof(GameObject));
								
							}
							break;
						}
					}
				}

				else if(s.StartsWith("* AF")) // ("* AF 액션프레임: ", str);
				{

					foreach(EffectEditorData.EffectAniEditData aniData in target.data)
					{
						if(aniData.aniName == currentAni)
						{
							aniData.bulletData.use = true;

							d = parseImportData(s);

							aniData.bulletData.actionFrame.Clear();

							int[] afs = Util.stringToIntArray(d[1],',');

							foreach(int f in afs)
							{
								aniData.bulletData.actionFrame.Add(f);
							}

							d = parseImportData(data[i+2]);
							//* 샷 위치:      * SP 샷 위치

							aniData.bulletData.shotPoint.Clear();

							if(d.Length >= 2 && d[1].Length > 3)
							{
								string[] vectors = d[1].Split('/');

								foreach(string str in vectors)
								{
									Vector3 vSp = Util.stringToVector3(str, ',');

									vSp.x *= target.scaleFactor * 0.01f;
									vSp.y *= target.scaleFactor * 0.01f;
									vSp.z *= target.scaleFactor * 0.01f;

									vSp.x = Mathf.RoundToInt(vSp.x);
									vSp.y = Mathf.RoundToInt(vSp.y);
									vSp.z = Mathf.RoundToInt(vSp.z);

									aniData.bulletData.shotPoint.Add( vSp );
								}

							}


							d = parseImportData(data[i+4]);
							//* 액션 위치: 없음    AP 액션 위치

							if(d[1].Contains("NONE") == false)
							{
								aniData.bulletData.targetTransform.Clear();

								foreach(string str in d[1].Split(','))
								{
									string tf = str.Trim();
									if(string.IsNullOrEmpty(tf) == false)
									{
										aniData.bulletData.targetTransform.Add(tf);
									}
								}

							}

							break;

						}
					}
				}

			}

		}

		EditorUtility.DisplayDialog("","입력 완료","확인");

		if(target != null) target.SetDirty();

	}

	string[] parseImportData(string inputLine)
	{
		string[] returnValue = inputLine.Split(':');
		for(int i = 0; i < returnValue.Length; ++i)
		{
			returnValue[i] = returnValue[i].Trim();
		}

		return returnValue;
	}



	StringBuilder _sb = new StringBuilder();
	void exportData()
	{
		_sb.Length = 0;

		if(target == null && target.modeling == null)
		{
			return;
		}

		add ("Name: "+target.modeling.name+"");
		add ("Scale: "+target.scaleFactor+"");
		addBreak(2);
		add ("======================================================");
		addBreak(2);

		float scale = target.scaleFactor * 0.01f;



		foreach( EffectEditorData.EffectAniEditData aniData in target.data )
		{
			if(target.modeling == null || target.modeling.animation == null || target.modeling.animation.GetClip(aniData.aniName) == null)
			{
				continue;
			}


			add("[ AniName: " + aniData.aniName);
			addBreak(1);
			add ("------- 이펙트 정보 ");
			foreach(AniDataEffect ade in aniData.aniData.effect)
			{
				if(ade.useThis == false) continue;
				if(ade.goEffect == null) continue;

				add ("* effName: ",ade.goEffect.name," : ",AssetDatabase.GetAssetPath(ade.goEffect));
				add ("* logic: "+ ade.getLogicString(scale));
				add ("------- ");
			}
			add ("---- end effect info -");
			addBreak(1);

			if(aniData.bulletData.use)
			{
				add ("------- 탄환 정보 ");
				
				string str = "";
				
				for(int i = 0; i < aniData.bulletData.actionFrame.Count ; ++i)
				{
					str += aniData.bulletData.actionFrame[i];
					
					if(i < aniData.bulletData.actionFrame.Count -1) str += ",";
				}
				
				add ("* AF 액션프레임: ", str);
				
				str = "";
				
				int len = aniData.bulletData.shotPoint.Count;
				for(int i = 0; i < len; ++i)
				{
					str += Mathf.RoundToInt(aniData.bulletData.shotPoint[i].x / scale).ToString()+",";
					str += Mathf.RoundToInt(aniData.bulletData.shotPoint[i].y / scale).ToString()+",";
					str += Mathf.RoundToInt(aniData.bulletData.shotPoint[i].z / scale).ToString();
					
					if(i < len -1 )
					{
						str += "/";
					}
				}
				
				add ("* SP 샷 위치: " + str);

				bool singleTransform = true;

				if(aniData.bulletData.targetTransform.Count > 1)
				{
					string fuck = aniData.bulletData.targetTransform[0];

					for(int i = 1; i < aniData.bulletData.targetTransform.Count; ++i)
					{
						if(fuck != aniData.bulletData.targetTransform[i])
						{
							singleTransform = false;
							break;
						}
					}
				}

				if(singleTransform)
				{
					if(aniData.bulletData.targetTransform.Count > 0)
					{
						add ("* AP 액션 위치: " + aniData.bulletData.targetTransform[0]);
					}
					else
					{
						add ("* 액션 위치: " + "없음(NONE)");
					}

				}
				else
				{
					add ("* AP 액션 위치: " + string.Join(",", aniData.bulletData.targetTransform.ToArray()));
				}

				addBreak(1);
				
				add("------");
				
				EffectEditorData.EffectEditBulletDetailData bd = aniData.bulletData.bulletEffect;
				
				str = "";
				
				add("* 탄환 : EFFECT");
				if(bd.effect != null)
				{
					add(bd.effect.name + " : " + AssetDatabase.GetAssetPath(bd.effect));
					
					switch(bd.type)
					{
					case EffectEditorData.EffectEditBulletDetailData.Type.Indie:
						str += "IE:";
						break;
					case EffectEditorData.EffectEditBulletDetailData.Type.Object:
						str += "P:";
						break;
					default:
						str += "E:";
						break;
					}
					
					str += "E_"+bd.effect.name.ToUpper();
					
					if(bd.attachedToParent)
					{
						str += ",Y";
					}
					else
					{
						str += ",N";
					}
					
					if(bd.useOption)
					{
						str += ",Y";
					}
					else
					{
						str += ",N";
					}
					
					add(str);
				}
				else
				{
					add ("빈 탄환입니다.");
					add ("메인탄환 이펙트에 아무것도 안넣으면 됩니다.");
				}
				
				
				if(bd.chainLighting != null)
				{
					add("* 체인라이트닝 ");
					
					add(bd.chainLighting.name + " : " + AssetDatabase.GetAssetPath(bd.chainLighting));
					
					add( "E_"+bd.chainLighting.name.ToUpper());
				}
				
				
				if(bd.goHitEffect != null)
				{
					add("* 타격이펙트 : HITEFFECT");
					
					add(bd.goHitEffect.name + " : " + AssetDatabase.GetAssetPath(bd.goHitEffect));
					
					add( "E_"+bd.goHitEffect.name.ToUpper());

				}


				if(bd.goDestroyEffect != null)
				{
					add("* 폭파이펙트 : USE_DESTROY_EFFECT");
					
					add(bd.goDestroyEffect.name + " : " + AssetDatabase.GetAssetPath(bd.goDestroyEffect));
					
					add( "E_"+bd.goDestroyEffect.name.ToUpper());

					if(bd.destroyOption == EffectEditorData.EffectEditBulletDetailData.DestroyEffType.BulletRotation)
					{
						add("* 폭파이펙트 옵션 : DESTROY_EFFECT_OPTION");
						add( "BR");
						add( "");
					}
				}


				if(bd.goGroundEffect != null)
				{
					add("* 이펙트 목록 추가 ");
					
					add(bd.goGroundEffect.name + " : " + AssetDatabase.GetAssetPath(bd.goGroundEffect));
					
					add( "E_"+bd.goGroundEffect.name.ToUpper());
				}


				addBreak(3);
			}
			add("+++++++++++++++++++++++++++++++++++++++++++++++++++");
			//aniData.bulletData
		}

		string result = _sb.ToString();

		string prefabPath = AssetDatabase.GetAssetPath(target.gameObject.GetInstanceID());

		prefabPath = prefabPath.Substring(0,prefabPath.LastIndexOf("."))+".txt";

		File.WriteAllText(prefabPath,result);

		EditorUtility.DisplayDialog("완료!","정보 파일 생성 완료!\n(Path: " + prefabPath + ")","확인");

		AssetDatabase.Refresh();

		//EffectEditorData.EffectAniEditData newEffectAniEditData = new EffectEditorData.EffectAniEditData();

	}

	void add(params object[] args)
	{
		for(int i = 0; i < args.Length; ++i)
		{
			_sb.Append(args[i].ToString());
		}

		addBreak(1);
	}

	void addBreak(int num)
	{
		for(int i = 0; i < num; ++i)
		{
			_sb.Append("\r\n");
		}
	}



	void drawBulletValue(string label, List<int> arr, int index, bool inputRefresh, out bool refresh)
	{
		int value = EditorGUILayout.IntField(label, arr[index]);

		if(value != arr[index])
		{
			arr[index] = value;
			refresh = true;
			return;
		}

		refresh = inputRefresh;
	}



	void OnSelectData (Object obj)
	{
		if(target != obj)
		{
			target = obj as EffectEditorData;

			if(target != null && UnitSkillCamMaker.instance != null)
			{
				if(Application.isPlaying && GameManager.me != null)
				{
					UnitSkillCamMaker.instance.deleteHeroMonster();
				}
			}

			deleteDummy();

			Repaint();
		}
	}

	
	void OnEnable () 
	{ 
		instance = this; 

//		Vector2 minSize = instance.minSize;
//		minSize.x = 1400;
//		instance.minSize = minSize;
	}
	
	void OnDisable () 
	{ 
		if(lastData != null)
		{
			lastData.SetDirty();
		}

		deleteDummy();


		instance = null; 
	}
	
	
	static public float GetFloat (string name, float defaultValue) { return EditorPrefs.GetFloat(name, defaultValue); }
	static public string GetString (string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }
	
	static string dataName
	{
		get { return  EditorPrefs.GetString("Editing Effect Data Name", null); }
		set { EditorPrefs.SetString("Editing Effect Data Name", value); }
	}
	
	
	static public void ShowProgress (float val)
	{
		EditorUtility.DisplayProgressBar("Updating", "Updating...", val);
	}


	void DrawSeperator(Color color)
	{
		EditorGUILayout.Space();

		Texture2D seperatorTexture = new Texture2D(1, 1);  //1 by 1 Pixel
		GUI.color = color;
		float y = GUILayoutUtility.GetLastRect().yMax;  //마지막 구성요소의 사각형
		GUI.DrawTexture(new Rect(5f, y, Screen.width-30f, 1.0f), seperatorTexture);  //인스펙터창의 가로사이즈 만큼 라인을 그린다.
		seperatorTexture.hideFlags = HideFlags.DontSave;  //매번 정보가 변경될 때 마다 인스펙터 창이 새로 랜더링 되므로 저장을 방지하여 메모리 누수를 방지한다.
		GUI.color = Color.white;  //기본 색상 화이트로 초기화 해준다. 
		
		EditorGUILayout.Space();
	}


	void DrawVerticleSeperator(Color color, float height, float xOffset = 0, float width = 1f)
	{
		GUI.color = color;
		Rect r = GUILayoutUtility.GetLastRect();
		r.x = r.x + r.width + 2 + xOffset;
		r.height = height;
		r.width = width;

		Texture2D seperatorTexture = new Texture2D(1, 1);  //1 by 1 Pixel
		GUI.DrawTexture(r, seperatorTexture);  //인스펙터창의 가로사이즈 만큼 라인을 그린다.
		seperatorTexture.hideFlags = HideFlags.DontSave;  //매번 정보가 변경될 때 마다 인스펙터 창이 새로 랜더링 되므로 저장을 방지하여 메모리 누수를 방지한다.
		GUI.color = Color.white;  //기본 색상 화이트로 초기화 해준다. 
	}




	public GameObject dummyUnit = null;
	
	public void createDummy(GameObject go)
	{
		if(dummy == null && Application.isPlaying)
		{
			dummy = (GameObject)Instantiate(go);
			dummy.transform.position = new Vector3(0,0,0);

			Quaternion q = new Quaternion();
			Vector3 v = new Vector3();

			v.y = 90;

			q.eulerAngles = v;

			dummy.transform.localRotation = q;

		}
	}
	
	
	public void deleteDummy()
	{
		if(dummy != null) GameObject.DestroyImmediate(dummy);
	}
	
	public void changeDummyAnimation(string atkName)
	{
		if(dummy != null)
		{
			dummy.animation.Play(atkName);
		}
	}
	
	public void goToDummyAnimation(string atkName, float time)
	{
		if(dummy != null)
		{
			dummy.animation[atkName].time = time;
			dummy.animation[atkName].speed = 0.0f;
			dummy.animation.Play(atkName);
		}
	}



	
	[MenuItem("Effect Editor/Open Effect Editor")]
	static public void OpenEffectEditor ()
	{
		EditorWindow.GetWindow<EffectEditor>(false, "Editor", true);
	}	


	static bool checkAnimationEffectParticle(GameObject go, bool checkHasParticleSystem = true, bool checkLoop = true)
	{
		Transform tfChild = null;

		if(go.particleSystem != null)
		{
			tfChild = go.transform;
		}
		else
		{
			tfChild = go.transform.FindChild(go.name);
		}

		if(tfChild == null)
		{
			EditorUtility.DisplayDialog("안내", "최상위와 그 다음 하위 게임오브젝트 이름 맞추세요.", "네");
			return false;
		}
		else
		{
			if(tfChild.particleSystem != null)
			{
				if(tfChild.particleSystem.loop && checkLoop)
				{
					if(EditorUtility.DisplayDialog("안내", "반복재생 파티클은 여기엔 쓸 수 없어요.\n그래도 쓸거예요?", "아니오", "네") == false)
					{
						return true;
					}
					return false;
				}
			}
			else if(tfChild.animation != null && checkHasParticleSystem)
			{
				if(tfChild.animation.clip.wrapMode == WrapMode.Loop && checkLoop)
				{
					EditorUtility.DisplayDialog("안내", "반복재생 애니메이션은 여기엔 쓸 수 없어요","네");
					return false;
				}
				else
				{
					EditorUtility.DisplayDialog("안내", "프리팹에 타이머를 붙여주세요.", "네");
					return false;
				}
			}
		}

		return true;
	}
	
	
	
	static public void AddAutoDestructionTimer()
	{
		foreach (Object o in Selection.GetFiltered(typeof (GameObject), SelectionMode.TopLevel | SelectionMode.Assets))
		{
			if(o is GameObject)
			{
				bool isChildrenType = false;

				GameObject go =  (GameObject)o;

				while(go.transform.parent != null)
				{
					go = go.transform.parent.gameObject;
				}

				Transform tfChild = null;

				if(tfChild.particleSystem != null)
				{
					tfChild = go.transform;
				}
				else
				{
					isChildrenType = true;
					tfChild = go.transform.FindChild(go.name);
				}


				if(tfChild == null)
				{
					EditorUtility.DisplayDialog("안내", "최상위와 그 다음 하위 게임오브젝트 이름 맞추세요.", "네");
				}
				else
				{
					if(tfChild.particleSystem != null)
					{
						if(tfChild.particleSystem.loop)
						{
							EditorUtility.DisplayDialog("안내", "파티클이 무한반복이네요? 수정해주세요.", "확인");
						}
						else
						{
							EditorUtility.DisplayDialog("안내", "이미 파티클 시스템이 붙어있네요.", "확인");
						}
					}
					else if(tfChild.animation != null)
					{
						if(tfChild.animation.clip.wrapMode == WrapMode.Loop)
						{
							EditorUtility.DisplayDialog("안내", "루프 애니메이션은 타이머를 못 걸겠죠?", "네");
						}
						else
						{
							if(isChildrenType)
							{
								ParticleSystem ps = tfChild.gameObject.AddComponent<ParticleSystem>();
								ps.renderer.sharedMaterial = null;
								ps.renderer.enabled = false;
								ps.Emit(0);
								ps.startSize = 1;
								ps.enableEmission = false;
								ps.maxParticles = 1;
								ps.loop = false;
								ps.playOnAwake = true;

								EditorUtility.DisplayDialog("완료", tfChild.animation.clip.length + "초\n는 직접 duration에 적어주세요.", "확인");

							}
						}
					}
				}
			}
		}
	}





	
	//					var reorderableList = new UnityEditorInternal.ReorderableList(fuck, typeof(int), true, true, true, true);
	//					reorderableList.DoList(rect);
	//or
	//					reorderableList.DoLayoutList();




	
	/*
				GUIStyle style = new GUIStyle();
				
				//style.normal.background = MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.1f));
				
				foreach (KeyValuePair<UISpriteData, int> iter in spriteList)	
					//------------------------------------//
					
					//foreach (KeyValuePair<string, int> iter in spriteList)
				{
					if(iter.Value == 2 && _renameMode) continue;
					
					++index;
					
					// ------ CUSTOM ------//
					
					GUILayout.BeginHorizontal();
					bool newSelected = GUILayout.Toggle(_listSelection[iter.Key.name], iter.Key.name, tk2dEditorSkin.SC_ListBoxItem, new GUILayoutOption[]{GUILayout.Width(250),GUILayout.Height(75)});

					NGUIEditorTools.SelectSprite(iter.Key.name);

					GUI.color = Color.green;
					GUILayout.Label("Add", GUILayout.Width(27f));
					GUI.color = Color.white;

					GUI.backgroundColor = Color.white;
					
					(GUILayout.Button("X", GUILayout.Width(22f))) mDelNames.Add(iter.Key.name);
					GUILayout.EndHorizontal();


*/
	



}


