using UnityEngine;
using System.Collections;

public class FlareLayerSetter : MonoBehaviour 
{
	private Behaviour _layer;

	void Awake()
	{

		Component c = gameObject.GetComponent ("FlareLayer");

		if(c != null)
		{
			_layer = (Behaviour)c;
		}

		if (_layer != null)
		{
			_layer.enabled = false;
		}
	}

	public bool isEnabled
	{
		set
		{
			if(_layer != null)
			{
				_layer.enabled = value;
			}
		}
	}

}
