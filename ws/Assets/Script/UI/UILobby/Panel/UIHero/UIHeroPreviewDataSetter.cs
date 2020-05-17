using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIHeroPreviewDataSetter : MonoBehaviour {


	public string code;

	public Monster sampleHero
	{
		get
		{
			try
			{
				if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.sampleHero != null)
				{
					return GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.sampleHero;
				}
			}
			catch(System.Exception e)
			{

			}

			return null;
		}
	}

	Vector3 _v; Quaternion _q;

	public void getPreviewPositionValue()
	{
		if(sampleHero == null) return;
		
		Vector3 s = sampleHero.cTransform.localScale;
		Vector3 p = sampleHero.cTransform.localPosition;
		Vector3 r = sampleHero.cTransform.localRotation.eulerAngles;
		
		List<string> f = new List<string>();
		
		f.Add(string.Format("{0:0.0}",s.x));
		
		f.Add(string.Format("{0:0.0}",p.x));
		f.Add(string.Format("{0:0.0}",p.y));
		f.Add(string.Format("{0:0.0}",p.z));
		
		f.Add(string.Format("{0:0.0}",r.x));
		f.Add(string.Format("{0:0.0}",r.y));
		f.Add(string.Format("{0:0.0}",r.z));
		
		code = string.Join(",",f.ToArray());
	}
	
	
	public void setPreviewPositionValue()
	{
		if(sampleHero == null) return;
		if(string.IsNullOrEmpty(code) == false)
		{
			float[] c = Util.stringToFloatArray(code,',');
			
			if(c.Length == 7)
			{
				_v = sampleHero.cTransform.localScale;
				_v.x = c[0]; _v.y = c[0]; _v.z = c[0];
				sampleHero.cTransform.localScale = _v;
				
				_v.x = c[1];
				_v.y = c[2];
				_v.z = c[3];
				
				sampleHero.cTransform.localPosition = _v;
				
				_v.x = c[4];
				_v.y = c[5];
				_v.z = c[6];
				
				_q.eulerAngles = _v;
				
				sampleHero.cTransform.localRotation = _q;
				
			}
		}
	}
}
