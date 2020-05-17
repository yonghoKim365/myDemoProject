using UnityEngine;
using System.Collections;

public class LampEffect : MonoBehaviour {

	public GameObject goOn;
	public GameObject goOff;

	public void turnOn(bool isOn)
	{
		goOn.SetActive(isOn);
		goOff.SetActive(!isOn);
	}

}
