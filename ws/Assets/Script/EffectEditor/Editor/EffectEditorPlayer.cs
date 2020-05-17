using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

[ExecuteInEditMode]
public class EffectEditorPlayer : EditorWindow
{

	[MenuItem("Effect Editor/Open Effect Player Editor")]
	static public void OpenEffectEditor ()
	{
		EditorWindow.GetWindow<EffectEditorPlayer>(false, "Editor", true);
	}	


	static public EffectEditorPlayer instance;
	
	public GameObject go;

	GameObject sample = null;


	void OnGUI ()
	{
		EditorGUILayout.BeginVertical();

		GameObject modeling = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;

		if(go != modeling)
		{
			go = modeling;
			isPlaying = false;

			if(sample != null) GameObject.DestroyImmediate(sample);
		}

		if(go != null)
		{
			if(GUILayout.Button("생성"))
			{
				if(sample == null)
				{
					sample = (GameObject)GameObject.Instantiate(go);
				}
			}

			if(GUILayout.Button("삭제"))
			{
				if(sample != null)
				{
					GameObject.DestroyImmediate(sample);
				}
			}


			if(GUILayout.Button("재생"))
			{
				if(sample != null) GameObject.DestroyImmediate(sample);

				sample = (GameObject)GameObject.Instantiate(go);

				if(sample != null)
				{

					anim = sample.GetComponent<Animator>();

					if(anim == null)
					{
						Animator[] ats = sample.GetComponentsInChildren<Animator>();
						if(ats != null && ats.Length > 0)
						{
							anim = ats[0];
						}
					}

					aniTarget = null;

					if(anim != null) 
					{
						aniTarget = anim.gameObject;

						UnityEditorInternal.AnimatorController act = anim.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
						UnityEditorInternal.StateMachine sm = act.GetLayer(0).stateMachine;
						
						for(int i = 0; i < sm.stateCount; i++) 
						{
							UnityEditorInternal.State state = sm.GetState(i);
							ac = state.GetMotion() as AnimationClip;
						}
					}



					if(sample.animation != null && sample.animation.clip != null)
					{
						ani = sample.animation;
					}
					else
					{
						ani = sample.GetComponentInChildren<Animation>();
					}

					if(ani != null && ani.clip != null)
					{
						ac = ani.clip;

						aniTarget = ani.gameObject;
					}

					if(ac != null)
					{
						aniLength = ac.length;
						wrapMode = ac.wrapMode;
					}

					Selection.activeGameObject = sample;

					if(sample.particleSystem != null)
					{
						sample.particleSystem.Play(true);
					}

					ParticleSystem[] pss = sample.GetComponentsInChildren<ParticleSystem>();

					if(pss != null && pss.Length > 0)
					{
						ps = pss[0];
					}
					else ps = null;

					Selection.activeGameObject = null;

					currentPlayTime = 0;

					isPlaying = true;
				}
			}
		}

		EditorGUILayout.EndVertical();
	}

	public WrapMode wrapMode;
	public float aniLength = 1;
	public float currentPlayTime = 0;

	public Animator anim;
	public Animation ani;

	public GameObject aniTarget;

	public AnimationClip ac;

	public ParticleSystem ps;

	void OnEnable () 
	{ 
		ac = null;
		anim = null;
		ani=  null;
		isPlaying = false;
		currentTime = Time.realtimeSinceStartup;
		instance = this; 
	}
	
	void OnDisable () 
	{ 
		if(sample != null)
		{
			GameObject.DestroyImmediate(sample);
		}

		aniTarget = null;
		ac = null;
		anim =  null;
		ani =  null;
		isPlaying = false;
		instance = null; 
	}


	float deltaTime = 0;
	float currentTime = Time.realtimeSinceStartup;

	public bool isPlaying = false;

	void Update()
	{
		if(isPlaying)
		{
			if(ps != null) Selection.activeGameObject = ps.gameObject;

			if(ac != null && aniTarget != null) aniTarget.SampleAnimation(ac,currentPlayTime);

			currentPlayTime += deltaTime;

			if(aniTarget == null || ac == null)
			{
				isPlaying = false;
			}
			else
			{
				if(currentPlayTime > ac.length)
				{
					if(ac.wrapMode == WrapMode.Loop)
					{
						currentPlayTime = 0;
					}
					else
					{
						isPlaying = false;
					}
				}
			}
		}

		deltaTime = Time.realtimeSinceStartup - currentTime;
		currentTime = Time.realtimeSinceStartup;
	}


}


