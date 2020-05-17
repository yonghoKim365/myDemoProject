using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class SoundManager : MonoBehaviour {
	
	public bool isCompleteLoadSound = false;
	public Queue<string> _soundLoaderId = new Queue<string>();
	public Queue<string> _soundLoaderFileName = new Queue<string>();

	public void addLoadSoundData(string soundDataId)
	{
		if(GameManager.info.soundData.ContainsKey(soundDataId) == false)
		{
			#if UNITY_EDITOR
			Debug.LogError(soundDataId + " 가 info에 없음 " );
#endif
			return;
		}

		_sd = GameManager.info.soundData[soundDataId];

//		Debug.LogError("ready add: " + soundDataId);

		if(_sd.isAssetBundle && _soundLoaderFileName.Contains(_sd.fileName) == false)
		{
			if(_sd.type == SoundData.Type.Music)
			{
				if(_bgmDic.ContainsKey(_sd.fileName) == false)
				{
					isCompleteLoadSound = false;
					_soundLoaderId.Enqueue(_sd.id);
					_soundLoaderFileName.Enqueue(_sd.fileName);

//					Debug.LogError("add: " + soundDataId);
				}
			}
			else
			{
				if(_sfxDic.ContainsKey(_sd.fileName) == false)
				{
					isCompleteLoadSound = false;
					_soundLoaderId.Enqueue(_sd.id);
					_soundLoaderFileName.Enqueue(_sd.fileName);

//					Debug.LogError("add: " + soundDataId);
				}
			}
		}
	}
	
	private int _totalSoundNum = 0;
	int _leftSoundNum = 0;
	
	public void startLoadSounds(bool veryFirst = false)
	{
		_leftSoundNum =  _soundLoaderId.Count;
		
		#if UNITY_EDITOR
		Debug.Log("startLoadSounds : " + _leftSoundNum);
		#endif
		if(veryFirst) _totalSoundNum = _leftSoundNum;
		
		if(_leftSoundNum> 0)
		{
			//			progress = 1.0f - (float)_leftSoundNum / (float)_totalSoundNum;
			_soundLoaderFileName.Dequeue();
			StartCoroutine(loadSoundAsset( GameManager.info.soundData[ _soundLoaderId.Dequeue() ] ));
		}
		else
		{
			//			progress = 1;
			isCompleteLoadSound = true;
		}
	}
	
	
	IEnumerator loadSoundAsset(SoundData sd)
	{
		using(WWW asset = new WWW(sd.path))
		{
			yield return asset;

//			Debug.LogError("load: " + sd.fileName);

			if(asset == null || asset.error != null || asset.isDone == false)
			{
				#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + sd.path);
#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
//				Debug.LogError("save: " + sd.fileName);

				if(sd.type == SoundData.Type.Music)
				{
					if(_bgmDic.ContainsKey(sd.fileName) == false && asset.assetBundle.mainAsset != null) _bgmDic.Add(sd.fileName , asset.assetBundle.mainAsset as AudioClip );
				}
				else
				{
					if(_sfxDic.ContainsKey(sd.fileName) == false && asset.assetBundle.mainAsset != null) _sfxDic.Add(sd.fileName  , asset.assetBundle.mainAsset as AudioClip );
				}

				asset.assetBundle.Unload(false);
			}
			
			if(asset != null) asset.Dispose();
		}

		startLoadSounds();
	}	
	
	
	private Dictionary<string, AssetBundle> loadedBundle = new Dictionary<string, AssetBundle>();
	public void saveMapAssetBundle(string assetName, AssetBundle asset)
	{
		if(loadedBundle.ContainsKey(assetName))
		{
			if(loadedBundle[assetName] != null && loadedBundle[assetName] == asset) return;
			loadedBundle[assetName] = asset;
		}
		else
		{
			loadedBundle.Add( assetName , asset );
		}
	}
	
	public void unloadAllMapAssetBundle()
	{
		if(loadedBundle == null) return;
		foreach(KeyValuePair<string, AssetBundle> kv in loadedBundle)
		{
			if(kv.Value != null)
			{
				kv.Value.Unload(true);
				Destroy(kv.Value);
			}
		}
		
		loadedBundle.Clear();
	}


	public void setSfxSoundFromAssetBundle(AssetBundle ab)
	{
		if(ab == null) return;

		UnityEngine.Object[] obs = ab.LoadAll(typeof(AudioClip));

		foreach(UnityEngine.Object o in obs)
		{
			_bundleSfxDic.Add(o.name, o as AudioClip);

			_sfxDic.Add(o.name, o as AudioClip);
		}
	}


}
