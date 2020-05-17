using UnityEngine;
using System.Collections;

sealed public class NumberEffect : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	}
	
	
	public void start(int num)
	{
		tk2dTextMesh font = gameObject.GetComponent<tk2dTextMesh>();
		
		if(num > 0)
		{
			font.text = "+" + num;
			font.color = Color.blue;
		}
		else
		{
			font.text = ""+num;
			font.color = Color.red;
		}
		
		this.name = "plus"+font.text;
		
		font.Commit();
		
		Hashtable ht = new Hashtable();
		
		ht.Add("y",60.0f);
		ht.Add("time", 1);
		ht.Add("looptype",iTween.LoopType.none);
		ht.Add("oncomplete","onCompleteMotion");
		ht.Add("easetype",iTween.EaseType.easeOutQuad);
		
		iTween.MoveBy(gameObject, ht);
	}

	private const string MISS = "MISS!!!";

	private string _lastString = "";
	Hashtable ht = new Hashtable();

	public void startMiss()
	{
		if(_lastString != MISS)
		{
			_lastString = MISS;
			tk2dTextMesh font = gameObject.GetComponent<tk2dTextMesh>();

			font.text = _lastString;
			font.color = Color.cyan;

			font.Commit();

			ht.Add("y",60.0f);
			ht.Add("time", 1);
			ht.Add("looptype",iTween.LoopType.none);
			ht.Add("oncomplete","onCompleteMotion");
			ht.Add("easetype",iTween.EaseType.easeOutQuad);
		}

		iTween.MoveBy(gameObject, ht);
	}

	void onCompleteMotion()
	{
		Destroy(gameObject);
	}
}
