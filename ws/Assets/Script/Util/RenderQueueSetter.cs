using UnityEngine;
using System.Collections;

public class RenderQueueSetter : MonoBehaviour
{
	Renderer[] _renderers;
	public int renderQueue = 3000;
	private int _renderQueue = 0;
	
	public bool useSharedMaterialType = false;
	
	void Awake ()
	{
		_renderers = GetComponentsInChildren<Renderer>();

		updateRenderQueue();
	}


	void Update()
	{
		if(_renderQueue != renderQueue)
		{
			updateRenderQueue();
		}
	}

	void updateRenderQueue()
	{
		if(_renderers == null) return;

		if(useSharedMaterialType)
		{
			foreach( Renderer r in _renderers )
			{
				r.sharedMaterial.renderQueue = renderQueue;
			}
		}
		else
		{
			foreach( Renderer r in _renderers )
			{
				r.material.renderQueue = renderQueue;
			}
		}

		_renderQueue = renderQueue;
	}

	
}