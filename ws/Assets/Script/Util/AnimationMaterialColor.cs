using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AnimationMaterialColor : MonoBehaviour {

	public Color color = new Color();
	public Material targetMaterial;



	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(targetMaterial != null)
		{
			targetMaterial.color = color;
		}
	}
}
