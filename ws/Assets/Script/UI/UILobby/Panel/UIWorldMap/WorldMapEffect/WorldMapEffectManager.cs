using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMapEffectManager : MonoBehaviour 
{
	[ExecuteInEditMode]
	public static WorldMapEffectManager instance;

	void Awake()
	{
		instance = this;
		saveDefaultTransformData();
		openNewActFsm.gameObject.SetActive(false);
	}

	void OnDestroy()
	{
		instance = null;
	}

	public UIWorldMapOpenStarEffect effect;

	public GameObject goStart;
	public GameObject goTarget;

	public int effectNum = 3;


	public void start()
	{
		if(goStart == null || goTarget == null) return;

		for(int i = 0; i < effectNum; ++i)
		{
			UIWorldMapOpenStarEffect eff = (UIWorldMapOpenStarEffect)Instantiate(effect);
			eff.transform.parent = effect.transform.parent;
			eff.StartEffect(goStart.transform.position, goTarget.transform.position);
		}

		SoundData.play("ui_stageclear");
	}



	public GameObject goNowButton;
	public GameObject goTargetButton;

	public GameObject[] currentPositionObjects;

	public void playStageAni()
	{
		GameDataManager.instance.maxAct = act;
		GameDataManager.instance.maxStage = stage;

		GameManager.me.uiManager.uiMenu.uiWorldMap.refresh();

		GameManager.me.uiManager.uiMenu.uiWorldMap.startNextStageAnimation();
	}


	public void play(bool isActOpen)
	{
		GameManager.me.uiManager.uiMenu.uiWorldMap.nowPlayingWalkAnimation = true;

		if(isActOpen && GameDataManager.instance.maxAct < GameManager.MAX_ACT + 1)
		{
			openNewActFsm.gameObject.SetActive(true);

			Vector3 lp = openNewActFsm.transform.localPosition;

			switch(GameDataManager.instance.maxAct)
			{
			case 2:
				lp.x = 240;
				break;
			case 3:
				lp.x = 735;
				break;
			case 4:
				lp.x = 1244;
				break;
			case 5:
				lp.x = 1857;
				break;
			case 6:
				lp.x = 2400;
				break;
			}

			openNewActFsm.transform.localPosition = lp;
		}
		else
		{
			openNewActFsm.gameObject.SetActive(false);
		}


		lockImageColor = Color.white;
		isActOpenEffect = isActOpen;
		_isPlayingActOpenEffect = false;
		clearFsm.enabled = true;
	}

	public bool isActOpenEffect = false;

	bool _isPlayingActOpenEffect = false;

	public void onCompleteClearEffect()
	{
#if UNITY_EDITOR
		Debug.Log("onCompleteClearEffect");
#endif

		if(isActOpenEffect)
		{
			_isPlayingActOpenEffect = true;
			openNewActFsm.enabled = true;
		}
		else
		{
			_isPlayingActOpenEffect = false;

			goStart = goNowButton;
			goTarget = goTargetButton;

			openFsm.enabled = true;

			start();
		}
	}


	public void startBreakActLockEffect()
	{
		SoundData.play("uicp_break");
	}



	public Color lockImageColor = new Color();

	public void onCompleteOpenNewActEffect()
	{

#if UNITY_EDITOR
		Debug.LogError("onCompleteOpenNewActEffect");
#endif

		if(spCurrentActLock != null)
		{
			spCurrentActLock.color = lockImageColor;
		}

		goStart = goNowButton;
		goTarget = goTargetButton;

		openFsm.enabled = true;
		_isPlayingActOpenEffect = false;

		start();
	}


	public void onCompleteOpenNewStageEffect()
	{
//		Debug.Log("onCompleteOpenNewStageEffect");
		_isPlayingActOpenEffect = false;

		openFsm.enabled = false;
		clearFsm.enabled = false;
		openNewActFsm.enabled = false;
		openNewActFsm.gameObject.SetActive(false);

		GameManager.me.uiManager.uiMenu.uiWorldMap.onCompleteFSMEffect();
	}

	public UISprite spActOpenAlphaTarget;

	public UISprite spCurrentActLock;



	void LateUpdate()
	{
		if(_isPlayingActOpenEffect)
		{
			if(spCurrentActLock != null)
			{
				spCurrentActLock.color = lockImageColor;
			}
		}
	}


	public PlayMakerFSM clearFsm;
	public PlayMakerFSM openFsm;
	public PlayMakerFSM openNewActFsm;


	public int act = 1;
	public int stage = 1;

	public void refreshPosition(int targetAct = -1, int targetStage = -1)
	{

		if( targetAct > GameManager.MAX_ACT)
		{
			goNowButton.gameObject.SetActive(false);
			goTargetButton.gameObject.SetActive(false);
			return;
		}


		UIWorldMap wm = GameManager.me.uiManager.uiMenu.uiWorldMap;

		UIWorldMapCheckPointButton[] cpb = null;
		UIWorldMapCheckPointButton[] nextCpb = null;

		if(targetAct > 0) act = targetAct;
		if(targetStage > 0) stage = targetStage;

		switch(act)
		{
		case 1: cpb = wm.mapRoad.cpAct1; nextCpb = wm.mapRoad.cpAct2; break;
		case 2: cpb = wm.mapRoad.cpAct2; nextCpb = wm.mapRoad.cpAct3; break;
		case 3: cpb = wm.mapRoad.cpAct3; nextCpb = wm.mapRoad.cpAct4; break;
		case 4: cpb = wm.mapRoad.cpAct4; nextCpb = wm.mapRoad.cpAct5; break;
		case 5: cpb = wm.mapRoad.cpAct5; nextCpb = wm.mapRoad.cpAct6; break;
		case 6: cpb = wm.mapRoad.cpAct6; break;
		}

		if(cpb == null) return;

		goNowButton.transform.position = cpb[stage - 1].transform.position;
		nextStageMaterial.SetTexture("_Diffuse",nextStageTextures[stage-1]);

		if(stage < 5)
		{
			goTargetButton.SetActive(true);
			goTargetButton.transform.position = cpb[stage].transform.position;
		}
		else 
		{
			if(act < GameManager.MAX_ACT)
			{
				goTargetButton.SetActive(true);
				if(nextCpb != null)
				{
					goTargetButton.transform.position = nextCpb[0].transform.position;
				}
			}
			else
			{
				goTargetButton.SetActive(false);
			}
		}


		foreach(GameObject go in currentPositionObjects)
		{
			go.SetActive(true);
		}

	
	}


	public Transform buttonEffectRoot;

	public Transform[] buttonEffectTfs;
	public ParticleSystem[] buttonEffectParticles;
	public Animation[] buttonEffectAnis;

	public PlayMakerFSM[] buttonEffectFSMs;

	Dictionary<Transform, ObjectDefaultInformation> _objectDefaultInfo = new Dictionary<Transform, ObjectDefaultInformation>();
	Dictionary<Transform, bool> _fsmActive = new Dictionary<Transform, bool>();
	Dictionary<AnimationState, float> _aniSpeed = new Dictionary<AnimationState, float>();


	public Material currentStageMaterial;
	public Material nextStageMaterial;
	public Material chainMaterial;

	public Texture[] nextStageTextures = new Texture[5];

	public void collectUI()
	{
		buttonEffectTfs = buttonEffectRoot.GetComponentsInChildren<Transform>(true);
		buttonEffectParticles = buttonEffectRoot.GetComponentsInChildren<ParticleSystem>(true);
		buttonEffectFSMs = buttonEffectRoot.GetComponentsInChildren<PlayMakerFSM>(true);
		buttonEffectAnis = buttonEffectRoot.GetComponentsInChildren<Animation>(true);
	}


	void saveDefaultTransformData()
	{
		if(buttonEffectTfs != null)
		{
			foreach(Transform tf in buttonEffectTfs)
			{
				ObjectDefaultInformation info = new ObjectDefaultInformation();
				info.isActive = tf.gameObject.activeSelf;
				info.localPosition = tf.localPosition;
				info.localRotation = tf.localRotation;
				info.localScale = tf.localScale;
				_objectDefaultInfo[tf] = info;
			}
		}

		if(buttonEffectFSMs != null)
		{
			foreach(PlayMakerFSM f in buttonEffectFSMs)
			{
				_fsmActive[f.transform] = f.enabled;
			}
		}

		if(buttonEffectAnis != null)
		{
			foreach(Animation f in buttonEffectAnis)
			{
				foreach(AnimationState s in f)
				{
					_aniSpeed[s] = s.speed;
				}
			}
		}

		currentStageMaterial.SetColor("_Color", new Color(0.4980392f,0.4980392f,0.4980392f,1) );
		chainMaterial.SetColor("_Color", new Color(0.4980392f,0.4980392f,0.4980392f,1) );

	}


	public void resetPlayMaker()
	{
		currentStageMaterial.SetColor("_Color", new Color(0.4980392f,0.4980392f,0.4980392f,1) );
		chainMaterial.SetColor("_Color", new Color(0.4980392f,0.4980392f,0.4980392f,1) );
		reset(buttonEffectTfs, buttonEffectParticles, buttonEffectFSMs, buttonEffectAnis);
	}

	void reset(Transform[] tfs, ParticleSystem[] ps, PlayMakerFSM[] fsms, Animation[] anis)
	{
		if(tfs != null)
		{
			ObjectDefaultInformation info;
			foreach(Transform tf in tfs)
			{
				info = _objectDefaultInfo[tf];
				tf.gameObject.SetActive(info.isActive);
				tf.localPosition = info.localPosition;
				tf.localRotation = info.localRotation;
				tf.localScale = info.localScale;
			}
		}
		
		if(ps != null)
		{
			foreach(ParticleSystem p in ps)
			{
				p.Stop();
				p.Clear();
				p.Play();
			}
		}
		
		if(fsms != null)
		{
			foreach(PlayMakerFSM f in fsms)
			{
				f.enabled = _fsmActive[f.transform];
			}
		}

		if(anis != null)
		{
			foreach(Animation f in anis)
			{
				foreach(AnimationState s in f)
				{
					s.speed = _aniSpeed[s];
				}
			}
		}
	}



}