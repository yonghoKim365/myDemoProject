using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPanelRefresher : MonoBehaviour 
{
	public UIWidget[] widgets;
	public List<UIWidget> widgetList = new List<UIWidget>();


	void OnEnable()
	{
		draw();
	}

	public void draw()
	{
		StartCoroutine(fuckyou());
	}


	WaitForSeconds wfs01 =new WaitForSeconds(0.01f);
	WaitForSeconds wfs1 =new WaitForSeconds(0.1f);

	IEnumerator fuckyou()
	{
		int c = 0;
		while(c < 3)
		{
			yield return wfs1;

			if(widgets != null)
			{
				for(int i = widgets.Length - 1; i >= 0; --i)
				{
					if(widgets[i] == null) continue;
					widgets[i].ForceUpdateVisibility();
				}
			}

			for(int i = widgetList.Count - 1; i >= 0; --i)
			{
				if(widgetList[i] == null) continue;
				widgetList[i].ForceUpdateVisibility();
			}

			++c;
		}
	}
}
