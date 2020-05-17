using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class SoundManager : MonoBehaviour {
	
	private static SoundManager _instance;
	
	public static SoundManager instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new GameObject("SoundManager").AddComponent<SoundManager>();
				_instance.init();
				
			}
			return _instance;
		}
	}

	const int TOTAL_SOURCE = 10;

	public const int PLAYER_BGM_1 = 0;
	public const int PLAYER_BGM_2 = 1;
	
	public const int PLAYER_LOOP_EFFECT_1 = 2;
	public const int PLAYER_LOOP_EFFECT_2 = 3;
	
	public const int PLAYER_CHARGING_1 = 4;
	public const int PLAYER_CHARGING_2 = 5;
	
	public const int PLAYER_ONE_SHOT = 6;

	public const int PLAYER_DIALOG_VOICE = 7;

	public const int PLAYER_GROAN_VOICE = 8;

	public const int PLAYER_UI_SOUND = 9;



	private AudioSource[] _audioSources  = new AudioSource[TOTAL_SOURCE]; 

	private AudioClip _bgm1 = null;
	private AudioClip _bgm2 = null;
	
	private Dictionary<string, float> _effectSoundTimeChecker = new Dictionary<string, float>(StringComparer.Ordinal);


	private Dictionary<string, AudioClip> _bundleSfxDic = new Dictionary<string, AudioClip>();

	private Dictionary<string, AudioClip> _sfxDic = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> _bgmDic = new Dictionary<string, AudioClip>();
	private Dictionary<int, AudioClip> _tutorialDic = new Dictionary<int, AudioClip>();
	private Dictionary<string, AudioClip> _cutsceneDic = new Dictionary<string, AudioClip>();


	const float BGM_VOLUME = 0.6f;
	const float VOICE_VOLUME = 1.0f;
	const float SFX_VOLUME = 0.8f;

	AudioFader bgmFader = new AudioFader(BGM_VOLUME);
	AudioFader loopingEffectFader = new AudioFader(SFX_VOLUME);
	AudioFader chargingFader = new AudioFader(SFX_VOLUME);

	private void init()
	{
		for(int i = 0; i < TOTAL_SOURCE; ++i)
		{
			_audioSources[i] = (gameObject.AddComponent("AudioSource") as AudioSource); 
		}

		_audioSources[PLAYER_UI_SOUND].priority = 0;


		muteSFX(GameManager.me.isMuteSFX);
		muteBGM(GameManager.me.isMuteBgm);

		nowLoadingCutSceneAsset = false;
	}



	public void muteBGM(bool isMute)
	{
		_audioSources[PLAYER_BGM_1].volume = (isMute)?0.0f:BGM_VOLUME;
		_audioSources[PLAYER_BGM_2].volume = (isMute)?0.0f:BGM_VOLUME;

		if(isMute)
		{
			bgmFader.clear();
		}
	}
	
	
	public void muteSFX(bool isMute)
	{
		for(int i = 2; i < TOTAL_SOURCE; ++i)
		{
			_audioSources[i].volume = (isMute)?0.0f:SFX_VOLUME;
		}

		_audioSources[PLAYER_DIALOG_VOICE].volume = (isMute)?0.0f:VOICE_VOLUME;

		if(isMute)
		{
			chargingFader.clear();
			loopingEffectFader.clear();
		}
	}


	public void muteAll(bool isMute)
	{

		muteBGM(isMute);
		muteSFX(isMute);

	}

	public void stopAllSounds()
	{
		for(int i = 0; i < TOTAL_SOURCE; ++i)
		{
			_audioSources[i].Stop();
		}

		bgmFader.clear();
		chargingFader.clear();
		loopingEffectFader.clear();
		stopGroanVoice();
	}

	public void stopBG(bool clearAsset = false)
	{
		bgmFader.clear();

		_audioSources[PLAYER_BGM_1].Stop();
		_audioSources[PLAYER_BGM_2].Stop();
	}















	private AudioClip _mainThemeAc = null;
	private SoundData _nowPlayingBgData = null;
	public void playBG(SoundData sd, bool ignoreSameBgm = false)
	{
//		Debug.LogError("playbg");

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(ignoreSameBgm == false) stopBG();

		if(GameManager.me.isMuteBgm == false || sd.id == "bgm_maintheme")
		{
			if(sd.isAssetBundle)
			{
				if(_bgmDic.ContainsKey(sd.fileName) == false)
				{
					_nowPlayingBgData = sd;
					StartCoroutine(playBgCT(sd, ignoreSameBgm));
				}
				else
				{
					playBgmClip(_bgmDic[sd.fileName], ignoreSameBgm);
				}
			}
			else
			{
				if(_bgmDic.ContainsKey(sd.fileName) == false) _bgmDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);

				playBgmClip(_bgmDic[sd.fileName], ignoreSameBgm);
			}
		}
	}

	private string _lastLoadingUrl = "";
	private Dictionary<string, bool> _loadingUrls = new Dictionary<string, bool>();

	IEnumerator playBgCT(SoundData sd, bool ignoreSameBgm = false)
	{
		if(_loadingUrls.ContainsKey(sd.path) == false)
		{
			_loadingUrls.Add(sd.path, true);
		}
		else
		{
			if(_loadingUrls[sd.path] == true) yield break;
			else _loadingUrls[sd.path] = true;
		}
			
		using(WWW asset = new WWW(sd.path))
		{
			yield return asset;

//			Debug.LogError("sd path : " + sd.path);

			if(asset == null || asset.error != null || asset.isDone == false)
			{
#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + sd.path);
#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				if(sd.type == SoundData.Type.Music)
				{
					if(_bgmDic.ContainsKey(sd.fileName) == false) _bgmDic.Add( sd.fileName  , asset.assetBundle.mainAsset as AudioClip );

					if(sd == _nowPlayingBgData)
					{
						playBgmClip( _bgmDic[sd.fileName] , ignoreSameBgm);
					}

				}
				else
				{
					if(_sfxDic.ContainsKey(sd.fileName) == false && asset.assetBundle.mainAsset != null) _sfxDic.Add( sd.fileName , asset.assetBundle.mainAsset as AudioClip );

					if(sd == _nowPlayingBgData)
					{
						playBgmClip( _sfxDic[sd.fileName] , ignoreSameBgm);
					}
				}

				asset.assetBundle.Unload(false);
			}
			
			if(asset != null)
			{
				asset.Dispose();
			}
		}

		_loadingUrls[sd.path] = false;
	}	


	void playBgmClip(AudioClip ac, bool ignoreSameBgm = false)
	{
		if(ignoreSameBgm)
		{
			if(ac != null && _audioSources[PLAYER_BGM_1].clip != null && (_audioSources[PLAYER_BGM_1].clip == ac) && _audioSources[PLAYER_BGM_1].isPlaying)
			{
				return;
			}
		}

		_audioSources[PLAYER_BGM_1].clip = ac;
		_audioSources[PLAYER_BGM_1].volume = (GameManager.me.isMuteBgm)?0.0f:BGM_VOLUME;
		_audioSources[PLAYER_BGM_1].loop = true;
		_audioSources[PLAYER_BGM_1].Play();
	}





	void setClip(int playerIndex, AudioClip ac, bool isLoop)
	{
		_audioSources[playerIndex].clip = ac;
//		_audioSources[playerIndex].pan = 0.0f;
		_audioSources[playerIndex].volume = 0.0f;
		_audioSources[playerIndex].loop = isLoop;
		_audioSources[playerIndex].Play();
	}


	public enum SoundPlayType
	{
		Music, LoopEffect, Charging
	}


	AudioClip loadOrGetAudioClip(SoundData sd)
	{
		if(_bgmDic.ContainsKey(sd.fileName))
		{
			return _bgmDic[sd.fileName];
		}
		else if(_sfxDic.ContainsKey(sd.fileName))
		{
			return _sfxDic[sd.fileName];
		}
		else if(sd.isAssetBundle == false)
		{
			if(_bgmDic.ContainsKey(sd.fileName) == false) _bgmDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
			return _bgmDic[sd.fileName];
		}
#if UNITY_EDITOR
		Debug.LogError("fadein : " + sd.path + " 가 bgm에 없음.");
#endif
		return null;
	}


	AudioClip loadOrGetSFXAudioClip(SoundData sd)
	{
		if(sd.isAssetBundle == false && _sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);		
		}

		return _sfxDic[sd.fileName];
	}



	SoundData _sd;
	// 새로 재생할 사운드를 fade in.
	public void fadePlay(SoundPlayType soundType, string id, AudioFader.State fadeType, float fadeTime)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		AudioClip ac;
		AudioFader fader;

		int p1 = PLAYER_BGM_1;
		int p2 = PLAYER_BGM_2;

		if(soundType == SoundPlayType.Charging)
		{
			if(GameManager.me.isMuteSFX) return;
			p1 = PLAYER_CHARGING_1;
			p2 = PLAYER_CHARGING_2;
			fader = chargingFader;
		}
		else if(soundType == SoundPlayType.LoopEffect)
		{
			if(GameManager.me.isMuteSFX) return;
			p1 = PLAYER_LOOP_EFFECT_1;
			p2 = PLAYER_LOOP_EFFECT_2;
			fader = loopingEffectFader;
		}
		else
		{
			if(GameManager.me.isMuteBgm) return;
			fader = bgmFader;
		}

		switch(fadeType)
		{
		case AudioFader.State.FadeIn:
			if(GameManager.info.soundData.ContainsKey(id) == false) return;

			_sd = GameManager.info.soundData[id];

			if(soundType == SoundPlayType.Music)
			{
				ac = loadOrGetAudioClip(_sd);
				stopBG();
			}
			else
			{
				if(soundType == SoundPlayType.LoopEffect) stopLoopEffect();
				else stopLoopChargingEffect();

				ac = loadOrGetSFXAudioClip(_sd);
			}

			if(ac == null) return;
			setClip(p1, ac, true);

			fader.start(_audioSources[p1],_audioSources[p2], fadeType, fadeTime);
			break;

		case AudioFader.State.FadeOut:
			if(_audioSources[p1].isPlaying && _audioSources[p1].volume > 0)
			{
				fader.start(_audioSources[p1], null, fadeType, fadeTime);
			}
			else if(_audioSources[p2].isPlaying && _audioSources[p2].volume > 0)
			{
				fader.start(_audioSources[p2], null, fadeType, fadeTime);
			}
			break;

		case AudioFader.State.QueueFade:
		case AudioFader.State.CrossFade:

			if(GameManager.info.soundData.ContainsKey(id) == false) return;

			_sd = GameManager.info.soundData[id];

			if(soundType == SoundPlayType.Music)
			{
				ac = loadOrGetAudioClip(_sd);

			}
			else
			{
				ac = loadOrGetSFXAudioClip(_sd);
			}

			if(ac == null)
			{
				fadePlay(soundType, id, AudioFader.State.FadeOut, fadeTime);
				return;
			}

			if(_audioSources[p1].isPlaying && _audioSources[p1].volume > 0)
			{
				setClip(p2, ac, true);
				fader.start(_audioSources[p1], _audioSources[p2], fadeType, fadeTime);
			}
			else if(_audioSources[p2].isPlaying && _audioSources[p2].volume > 0)
			{
				setClip(p1, ac, true);
				fader.start(_audioSources[p2], _audioSources[p1], fadeType, fadeTime);
			}
			else
			{
				setClip(p1, ac, true);
				fader.start(_audioSources[p1], null, AudioFader.State.FadeIn, fadeTime);
			}
			break;
		}
	}




	void Update()
	{
		bgmFader.update();
		loopingEffectFader.update();
		chargingFader.update();
		updateGroanVoice();
	}



	public static bool nowLoadingCutSceneAsset = false;
	public void loadCutSceneSoundAsset(Callback.Default callback)
	{
		nowLoadingCutSceneAsset = true;
		StartCoroutine(loadCutSceneSoundAssetCT(callback));
	}
	
	IEnumerator loadCutSceneSoundAssetCT(Callback.Default callback)
	{
		string path = AssetBundleManager.getCutSceneSoundBundlePath(GameManager.me.stageManager.nowRound.id.ToUpper(), true);

#if UNITY_EDITOR
		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			string csi = CutSceneMaker.instance.nowCutSceneId.ToUpper();

			string[] c = csi.Split('_');

			path = AssetBundleManager.getCutSceneSoundBundlePath(c[0], true);
		}
#endif

		using(WWW asset = new WWW(path))
		{
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				UnityEngine.Object[] clips = asset.assetBundle.LoadAll( typeof( AudioClip ));
				
				foreach(UnityEngine.Object ac in clips)
				{
					if(_cutsceneDic.ContainsKey(ac.name) == false) _cutsceneDic.Add( ac.name , ac as AudioClip );
				}
				
				asset.assetBundle.Unload(false);
			}
			
			if(asset != null) asset.Dispose();

#if UNITY_EDITOR
			Debug.Log("CutScene Sound Load Complete!");
#endif
		}

		nowLoadingCutSceneAsset = false;
		if(callback != null) callback();
	}


	public void playCutSceneVoice(string id)
	{
		if(Time.timeScale >= 5.0f || GameManager.me.cutSceneManager.nowCutSceneSpeed > 1 || GameManager.me.cutSceneManager.isSkipMode) return;
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
#endif


		if(GameManager.me.isMuteSFX) return;

		if(_cutsceneDic.ContainsKey(id))
		{
			_audioSources[PLAYER_DIALOG_VOICE].clip = _cutsceneDic[id];
			_audioSources[PLAYER_DIALOG_VOICE].volume = VOICE_VOLUME;
			_audioSources[PLAYER_DIALOG_VOICE].loop = false;
			_audioSources[PLAYER_DIALOG_VOICE].Play();
		}
	}


	public void playPlayerVoice(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif


		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;
		if(GameManager.me.isMuteSFX || sd == null ) return;

		if(_audioSources[PLAYER_DIALOG_VOICE].isPlaying) return;

		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}

		_audioSources[PLAYER_DIALOG_VOICE].clip = _sfxDic[sd.fileName];
		_audioSources[PLAYER_DIALOG_VOICE].volume = SFX_VOLUME;
		_audioSources[PLAYER_DIALOG_VOICE].loop = false;
		_audioSources[PLAYER_DIALOG_VOICE].Play();
	}


	public void playPlayerDieVoice(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif
		
		
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;
		if(GameManager.me.isMuteSFX || sd == null ) return;
		
		if(_audioSources[PLAYER_DIALOG_VOICE].isPlaying) _audioSources[PLAYER_DIALOG_VOICE].Stop();
		
		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}
		_audioSources[PLAYER_DIALOG_VOICE].volume = SFX_VOLUME;
		_audioSources[PLAYER_DIALOG_VOICE].loop = false;
		_audioSources[PLAYER_DIALOG_VOICE].PlayOneShot( _sfxDic[sd.fileName]);
	}




	bool isPlayingGroanVoice = false;


	private SoundData[] _grnVoiceList = null;
	public void playGroanVoice(SoundData[] grnVoiceList)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif
		
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;
		
		if(GameManager.me.isMuteSFX  ) return;

		_grnVoiceList = grnVoiceList;

		if(_grnVoiceList == null) return;

		playNextGroanVoice();
	}

	void playNextGroanVoice()
	{
		isPlayingGroanVoice = true;

		SoundData sd = _grnVoiceList[UnityEngine.Random.Range(0,_grnVoiceList.Length)];

		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}
		
		_audioSources[PLAYER_GROAN_VOICE].clip = _sfxDic[sd.fileName];
//		_audioSources[PLAYER_GROAN_VOICE].pan = 0.0f;
		_audioSources[PLAYER_GROAN_VOICE].loop = false;
		_audioSources[PLAYER_GROAN_VOICE].volume = SFX_VOLUME;
		
		_audioSources[PLAYER_GROAN_VOICE].Play();
		
		_groanVoiceDelay = UnityEngine.Random.Range(1.5f,2.5f);
	}


	public void stopGroanVoice()
	{
		isPlayingGroanVoice = false;

		if(_audioSources[PLAYER_GROAN_VOICE].isPlaying) _audioSources[PLAYER_GROAN_VOICE].Stop();
		_audioSources[PLAYER_GROAN_VOICE].clip = null;
	}

	private float _groanVoiceDelay = 1000;
	void updateGroanVoice()
	{
		if(isPlayingGroanVoice)
		{
			if(_audioSources[PLAYER_GROAN_VOICE].isPlaying == false)
			{
				if(_groanVoiceDelay > 0)
				{
					_groanVoiceDelay -= Time.smoothDeltaTime;
					return;
				}

				playNextGroanVoice();
			}
		}
	}




	public void loadTutorialSoundAsset(Callback.Default callback)
	{
		_tutorialDic.Clear();
		StartCoroutine(loadTutorialSoundAssetCT(callback));
	}

	IEnumerator loadTutorialSoundAssetCT(Callback.Default callback)
	{

		string path = AssetBundleManager.getTutorialSoundBundlePath(TutorialManager.instance.nowTutorialId, true);

		using(WWW asset = new WWW(path))
		{
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				UnityEngine.Object[] clips = asset.assetBundle.LoadAll( typeof( AudioClip ));
				
				foreach(UnityEngine.Object ac in clips)
				{
					string sName = ac.name;
					int keyName = Convert.ToInt32(sName);
					if(_tutorialDic.ContainsKey(keyName) == false) _tutorialDic.Add( keyName , ac as AudioClip );
				}

				asset.assetBundle.Unload(false);
			}

			if(asset != null) asset.Dispose();

#if UNITY_EDITOR
			Debug.Log("Tutorial Sound Load Complete!");
#endif
		}

		callback();
	}



	public void playTutorialVoice(int subStep)
	{
		if(GameManager.me.isMuteSFX) return;
		if(_tutorialDic != null && _tutorialDic.ContainsKey(subStep))
		{
			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation) return;
			#endif

			stopTutorialVoice();

			_audioSources[PLAYER_DIALOG_VOICE].clip = _tutorialDic[subStep];
			_audioSources[PLAYER_DIALOG_VOICE].volume = BGM_VOLUME;
			_audioSources[PLAYER_DIALOG_VOICE].loop = false;
			_audioSources[PLAYER_DIALOG_VOICE].Play();
		}
	}

	public void stopTutorialVoice()
	{
		if(_audioSources[PLAYER_DIALOG_VOICE].isPlaying) _audioSources[PLAYER_DIALOG_VOICE].Stop();
		_audioSources[PLAYER_DIALOG_VOICE].clip = null;
	}


	public void clearTutorialVoiceAsset()
	{
		if(_audioSources[PLAYER_DIALOG_VOICE].isPlaying) _audioSources[PLAYER_DIALOG_VOICE].Stop();
		_audioSources[PLAYER_DIALOG_VOICE].clip = null;

		foreach(KeyValuePair<int, AudioClip> kv in _tutorialDic)
		{
			Resources.UnloadAsset(kv.Value);
		}

		_tutorialDic.Clear();
	}

	public void clearCutSceneVoiceAsset()
	{
		if(_audioSources[PLAYER_DIALOG_VOICE].isPlaying) _audioSources[PLAYER_DIALOG_VOICE].Stop();
		_audioSources[PLAYER_DIALOG_VOICE].clip = null;

		foreach(KeyValuePair<string, AudioClip> kv in _cutsceneDic)
		{
			Resources.UnloadAsset(kv.Value);
		}

		_cutsceneDic.Clear();
	}




	private AudioClip _tempAc = null;

	public void playEffect(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

		if(CutSceneManager.nowOpenCutScene)
		{
			if(Time.timeScale >= 5.0f || GameManager.me.cutSceneManager.nowCutSceneSpeed > 1 || GameManager.me.cutSceneManager.isSkipMode) return;
		}

		if(GameManager.me.isMuteSFX || sd == null ) return;

		float checkTime = 0;

		if(_effectSoundTimeChecker.TryGetValue(sd.fileName, out checkTime) == false)
		{
			_effectSoundTimeChecker.Add(sd.fileName, Time.realtimeSinceStartup);
		}
		else if(checkTime + 0.5f >= Time.realtimeSinceStartup)
		{
			return;
		}

		_effectSoundTimeChecker[sd.fileName] = Time.realtimeSinceStartup;

		if(_sfxDic.TryGetValue(sd.fileName, out _tempAc) == false )
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
			_audioSources[PLAYER_ONE_SHOT].PlayOneShot ( _sfxDic[sd.fileName] );
		}
		else
		{
			_audioSources[PLAYER_ONE_SHOT].PlayOneShot ( _tempAc );
		}

	}



	public void playUISound(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif
		
		if(GameManager.me.isMuteSFX || sd == null ) return;

//		_audioSources[PLAYER_ONE_SHOT].pan = 0.0f;
		
		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}

//		Debug.LogError("_audioSources[PLAYER_ONE_SHOT]. : " + _audioSources[PLAYER_ONE_SHOT].isPlaying);

		_audioSources[PLAYER_UI_SOUND].PlayOneShot ( _sfxDic[sd.fileName] );
	}



	
	public void playLoopEffect(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

		if(GameManager.me.isMuteSFX  ) return;

		stopLoopEffect(1);
		
		if(_audioSources[PLAYER_LOOP_EFFECT_1].isPlaying) return;

		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}

		_audioSources[PLAYER_LOOP_EFFECT_1].clip = _sfxDic[sd.fileName];
		_audioSources[PLAYER_LOOP_EFFECT_1].loop = true;
		_audioSources[PLAYER_LOOP_EFFECT_1].volume = SFX_VOLUME;
		_audioSources[PLAYER_LOOP_EFFECT_1].Play();
	}



	public void playLoopEffect2(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;
		
		if(GameManager.me.isMuteSFX  ) return;
		
		stopLoopEffect(2);
		
		if(_audioSources[PLAYER_LOOP_EFFECT_2].isPlaying) return;
		
		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}
		
		_audioSources[PLAYER_LOOP_EFFECT_2].clip = _sfxDic[sd.fileName];
		_audioSources[PLAYER_LOOP_EFFECT_2].loop = true;
		_audioSources[PLAYER_LOOP_EFFECT_2].volume = SFX_VOLUME;
		_audioSources[PLAYER_LOOP_EFFECT_2].Play();
	}



	
	public void stopLoopEffect(int stopAll = 0)
	{
		loopingEffectFader.clear();

		if(stopAll == 0)
		{
			_audioSources[PLAYER_LOOP_EFFECT_1].Stop();
			_audioSources[PLAYER_LOOP_EFFECT_2].Stop();

		}
		else if(stopAll == 1)
		{
			_audioSources[PLAYER_LOOP_EFFECT_1].Stop();

		}
		else if(stopAll == 2)
		{
			_audioSources[PLAYER_LOOP_EFFECT_2].Stop();

		}

	}


	public void playChargingEffect(SoundData sd)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
		#endif

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

		if(GameManager.me.isMuteSFX   ) return;

		stopLoopChargingEffect();

		if(_sfxDic.ContainsKey(sd.fileName) == false)
		{
			_sfxDic.Add(sd.fileName, Resources.Load(sd.path) as AudioClip);
		}
		
		_audioSources[PLAYER_CHARGING_1].clip = _sfxDic[sd.fileName];
//		_audioSources[PLAYER_CHARGING_1].pan = 0.0f;
		_audioSources[PLAYER_CHARGING_1].loop = true;
		_audioSources[PLAYER_CHARGING_1].volume = SFX_VOLUME;

		_audioSources[PLAYER_CHARGING_1].Play();
	}

	public void stopLoopChargingEffect()
	{
		chargingFader.clear();
		_audioSources[PLAYER_CHARGING_1].Stop();
		_audioSources[PLAYER_CHARGING_2].Stop();
	}


	public void clearSound()
	{
		stopAllSounds();

		foreach(AudioSource ac in _audioSources)
		{
			ac.clip = null;
		}

		List<string> deleteSfx = new List<string>();

		foreach(KeyValuePair<string, AudioClip> ac in _sfxDic)
		{
			if(_bundleSfxDic.ContainsKey(ac.Key) == false)
			{
				Resources.UnloadAsset(ac.Value);
				deleteSfx.Add(ac.Key);
			}
		}

		foreach(KeyValuePair<string, AudioClip> ac in _bgmDic)
		{
			Resources.UnloadAsset(ac.Value);
		}

		for(int i = deleteSfx.Count - 1; i >= 0; --i)
		{
			_sfxDic.Remove(deleteSfx[i]);
		}


		_bgmDic.Clear();

		clearTutorialVoiceAsset();
		clearCutSceneVoiceAsset();
	}


	void OnDestroy()
	{
		try
		{
			clearSound();
		}
		catch(Exception e)
		{
			#if UNITY_EDITOR
			Debug.LogError("SoundManager Destroy ERROR!!!! : " + e.Message);
#endif
		}

		_sfxDic = null;
		_bgmDic = null;
		_tutorialDic = null;
		_cutsceneDic = null;

		for(int i = 0; i < _audioSources.Length; ++i)
		{
			_audioSources[i] = null;
		}

		_instance = null;
	}
}


public class AudioFader
{
	public enum State
	{
		FadeIn, FadeOut, CrossFade, Idle, QueueFade
	}

	State _state = State.Idle;

	private AudioSource _a1;
	private AudioSource _a2;

	private float _fadeSpeed = 1.0f;

	public float maxVolume = 1.0f;

	public AudioFader(float v)
	{
		maxVolume = v;
	}

	public void start(AudioSource a1, AudioSource a2, State state, float fadeTime)
	{
		_a1 = a1;
		_a2 = a2;
		_state = state;
		_fadeSpeed = 1.0f / fadeTime;

		if(_state == State.CrossFade)
		{
			if(_a1.isPlaying == false && _a2.isPlaying == false)
			{
				_state = State.FadeIn;
			}
			else if(_a1.isPlaying == false && _a2.isPlaying)
			{
				AudioSource tempAs = _a1;
				_a1 = _a2;
				_a2 = _a1;
			}
		}
	}

	float _f;
	public void update()
	{
		if(_a1 == null) return;

		switch(_state)
		{
		case State.QueueFade:

			if(_a1.volume > 0.0f)
			{
				_f = _a1.volume - Time.deltaTime * _fadeSpeed;
				if(_f < 0)
				{
					_a1.volume = 0.0f;
					_a1.Stop();
					_a1 = _a2;
					_a1.volume = 0.0f;
					_a2 = null;
					_state = State.FadeIn;
				}
				else _a1.volume = _f;
			}


			break;

		case State.CrossFade:

			if(_a1.volume > 0.0f)
			{
				_f = _a1.volume - Time.deltaTime * _fadeSpeed;
				if(_f < 0)
				{
					_a1.volume = 0.0f;
					_a1.Stop();
				}
				else _a1.volume = _f;
			}


			if(_a2.volume < maxVolume)
			{
				_f = _a2.volume + Time.deltaTime * _fadeSpeed;
				if(_f > maxVolume)
				{
					_a2.volume = maxVolume;
				}
				else _a2.volume = _f;
			}

			if(_a1.volume <= 0.0f && _a2.volume >= maxVolume)
			{
				clear();
			}
			break;



		case State.FadeIn:

			if(_a1.volume < maxVolume)
			{
				_f = _a1.volume + Time.deltaTime * _fadeSpeed;
				if(_f > 1)
				{
					_a1.volume = maxVolume;
					clear();
				}
				else _a1.volume = _f;
			}
			break;

		case State.FadeOut:
			
			if(_a1.volume > 0.0f)
			{
				_f = _a1.volume - Time.deltaTime * _fadeSpeed;
				if(_f < 0)
				{
					_a1.volume = 0.0f;
					_a1.Stop();
					clear();
				}
				else _a1.volume = _f;
			}
			break;
		}
	}


	public void clear()
	{
		_state = State.Idle;
		_a1 = null;
		_a2 = null;
	}
}
