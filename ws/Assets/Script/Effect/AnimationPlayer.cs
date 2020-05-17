using UnityEngine;
using System.Collections;

public class AnimationPlayer : MonoBehaviour 
{
	public Animation ani;

	public string logic = string.Empty;
	private int _aniIndex = 0;
	private AnimationPlayerData[] _data;

	private int _playIndex = 0;

	void Awake()
	{
		if(ani == null) ani = animation;
		if(_data == null) setData();
	}

	void OnEnable()
	{
		if(ani == null) ani = animation;
		if(_data == null) setData();

		play();
	}

	void OnDisable()
	{
		if(ani != null)
		{
			ani.Stop();
		}
	}

	public void reset()
	{
		ani.Stop();
		setData();
		play();
	}


	void play()
	{
		++_playIndex;
		StartCoroutine(playCT(_playIndex));
	}


	IEnumerator playCT(int currentPlayIndex)
	{
		int index = 0;

		while(index < _data.Length && _playIndex == currentPlayIndex)
		{
			AnimationPlayerData apd = _data[index];

			if(index == 0)
			{
				ani.Play(apd.aniName);
			}
			else
			{
				ani.CrossFade(apd.aniName, 0.02f);
			}

			if(apd.delay > 0)
			{
				yield return new WaitForSeconds(apd.delay);
			}

			++index;
		}
	}

	void OnDestroy()
	{
		ani = null;
	}

	void setData()
	{
		string[] tmp = logic.Split('/');

		_data = new AnimationPlayerData[tmp.Length];

		for(int i = 0; i < tmp.Length; ++i)
		{
			string[] tmp2 = tmp[i].Split(':');

			AnimationPlayerData apd = new AnimationPlayerData();

			apd.aniName = tmp2[0];

			if(tmp2.Length == 2)
			{
				float.TryParse(tmp2[1], out apd.delay);
			}
			else
			{
				apd.delay = ani[apd.aniName].length;
			}

			_data[i] = apd;
		}
	}
}


public struct AnimationPlayerData
{
	public string aniName;
	public float delay;

}
