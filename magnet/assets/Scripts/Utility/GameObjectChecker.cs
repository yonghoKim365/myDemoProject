using UnityEngine;
using System.Collections;

public class GameObjectChecker : MonoBehaviour {

	public GameObject[] objs;

	// Use this for initialization
	void Start () {
	
		for(int i=0;i<objs.Length;i++){
            if (objs[i] == null)
            {
                Debug.LogError("GameObjectChecker is objs null error! ArrNum=" + i);
                continue;
            }

			if (objs[i].activeSelf == false){
				objs[i].SetActive(true);
			}
		}

		Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
