using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMissionClearNotice : MonoBehaviour 
{

	public UIMissionClearNoticePanel panel;
	public UIMissionClearNoticePanel panel2;

	public static UIMissionClearNotice instance = null;

	public 

	void Awake ()
	{
		instance = this;
		gameObject.SetActive(false);
		camera.enabled = true;
	}

	void OnDestroy()
	{
		instance = null;
		panel = null;
		panel2 = null;
	}

	public Stack<UIMissionClearNoticePanel> pool = new Stack<UIMissionClearNoticePanel>();
	public Stack<UIMissionClearNoticePanel> pool2 = new Stack<UIMissionClearNoticePanel>();

	Queue<P_MissionNotification> _queue = new Queue<P_MissionNotification>();

	public void start(Dictionary<string, P_MissionNotification> data)
	{
		isPlaying = false;

		if(data == null) return;

		_queue.Clear();

		foreach(KeyValuePair<string, P_MissionNotification> kv in data)
		{
			if(kv.Value.state == WSDefine.MISSION_CLEAR )
			{
				if(kv.Value.desc == null)
				{
					_queue.Enqueue(kv.Value);
				}
			}
		}

		foreach(KeyValuePair<string, P_MissionNotification> kv in data)
		{
			if(kv.Value.state == WSDefine.MISSION_CLEAR )
			{
				if(kv.Value.desc != null)
				{
					_queue.Enqueue(kv.Value);
				}
			}
		}

		if(_queue.Count > 0)
		{
			isPlaying = true;
			gameObject.SetActive(true);
			StartCoroutine(playMissionNoticePanel());
		}
	}

	//test
	public void start(Dictionary<string, P_Mission> data)
	{
		isPlaying = false;

		if(data == null) return;
		
		_queue.Clear();

		int i = 0;

		foreach(KeyValuePair<string, P_Mission> kv in data)
		{
			if(instance.gameObject.activeSelf == false) instance.gameObject.SetActive(true);

			P_MissionNotification n = new P_MissionNotification();
			n.id = kv.Value.id;
			n.title = kv.Value.title;
			n.desc = kv.Value.desc;
			n.rewards = kv.Value.rewards;
			n.category = kv.Value.category;

			++i;
			_queue.Enqueue(n);
		}

		isPlaying = true;

		StartCoroutine(playMissionNoticePanel());
	}



	public static void setPanelToPool(UIMissionClearNoticePanel p)
	{
		if(instance != null)
		{
			if(p.isScarlet)
			{
				instance.pool2.Push(p);
			}
			else
			{
				instance.pool.Push(p);
			}

		}
	}


	public float delay = 1.4f;
	public float delay2 = 2.5f;

	public bool isPlaying = false;

	IEnumerator playMissionNoticePanel()
	{
		int count = _queue.Count;
		int index = 0;

		isPlaying = true;

		bool normalNotice = true;

		while(_queue.Count > 0)
		{
			if(normalNotice)
			{
				yield return new WaitForSeconds(delay);
			}
			else
			{
				yield return new WaitForSeconds(delay2);
			}

			P_MissionNotification d = _queue.Dequeue();

			UIMissionClearNoticePanel p;

			normalNotice = (d.desc == null);

			if(normalNotice)
			{
				if(pool.Count > 0)
				{
					p = pool.Pop();
				}
				else
				{
					p = ((UIMissionClearNoticePanel)Instantiate(panel));
					p.transform.parent = panel.transform.parent;
				}
			}
			else
			{
				if(pool2.Count > 0)
				{
					p = pool2.Pop();
				}
				else
				{
					p = ((UIMissionClearNoticePanel)Instantiate(panel2));
					p.transform.parent = panel2.transform.parent;
					p.transform.localPosition = new Vector3(-4,-67,-163);
				}
			}


			p.start(d, index, _queue.Count == 0);

			p.animation.Play();

			++index;
		}

		isPlaying = false;
	}

}
