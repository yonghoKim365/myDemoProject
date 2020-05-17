using UnityEngine;
using System.Collections;

public class MeshMerger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Merge();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Merge()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.mesh.Clear();
	                             
	MeshFilter [] meshFilters = GetComponentsInChildren<MeshFilter>( true );
	transform.GetComponent<MeshRenderer>().material = meshFilters[0].renderer.sharedMaterial;     
	              
	CombineInstance[] combine = new CombineInstance[meshFilters.Length-1];
	              
	int i = 0;
	int ci = 0;
	while ( i < meshFilters.Length )
	{
		if( meshFilter != meshFilters[i] )
		{
			combine[ci].mesh = meshFilters[i].sharedMesh;
			combine[ci].transform = meshFilters[i].transform.localToWorldMatrix;
			++ci;
		}
			meshFilters[i].gameObject.SetActive( false );//.renderer.enabled = false;
			i++;
		}          
		meshFilter.mesh.CombineMeshes( combine );
		transform.gameObject.SetActive( true );
	}	
	
}
