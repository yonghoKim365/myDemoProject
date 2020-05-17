using UnityEngine;
using System.Collections;

public class Missing_Model : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(new Vector3(0f, 0f, 50f * Time.deltaTime));
	}
}
