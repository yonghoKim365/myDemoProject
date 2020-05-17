using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class NGUIUILabelFontChanger : MonoBehaviour 
{
	public UnityEngine.Object fromFont;

	public UnityEngine.Object toFont;

	public GameObject root;

	public void change(UILabel[] lb)
	{
		UIFont fromUIFont = null;
		UIFont toUIFont = null;

		GameObject go = toFont as GameObject;
		if(go != null)
		{
			toUIFont = go.GetComponent<UIFont>();
		}


		GameObject g = fromFont as GameObject;
		if(g != null)
		{
			fromUIFont = g.GetComponent<UIFont>();
		}


		for(int i = lb.Length -1; i >= 0; --i)
		{
			if( (fromUIFont != null && lb[i].ambigiousFont == fromUIFont) ||  (fromFont != null && lb[i].ambigiousFont == fromFont))
			{
				if(toUIFont != null)
				{
					lb[i].ambigiousFont = toUIFont;
				}
				else
				{
					lb[i].ambigiousFont = toFont;
				}

				Debug.Log("change font: " + lb[i].name);
			}
		}
	}
}
