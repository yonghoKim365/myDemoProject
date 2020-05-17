using UnityEngine;
using System.Collections;

public class LobbyCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private int _index = 0;
	
	
	public GameObject body0;
	public GameObject body1;
	
	public GameObject head0;
	public GameObject head1;
	
	public void changeParts()
	{
		int fuck = _index % 4;
		
		switch(fuck)
		{
		case 0:
			body0.SetActive(true);
			body1.SetActive(false);
			break;
		case 1:
			head0.SetActive(true);
			head1.SetActive(false);
			break;
		case 2:
			body1.SetActive(true);
			body0.SetActive(false);
			
			break;
		case 3:
			head1.SetActive(true);
			head0.SetActive(false);			
			break;
		}
		
		
		
		
		++_index;
	}
	
}
