using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MaterialPropertyModifier : MonoBehaviour {

	public Color color = Color.white;

	public Material targetMaterial;

	public string targetShaderProperty = "_Color";

	public bool isShareMaterial = false;

	public bool useColor = true;

	public int renderQueue = 3000;

	public enum RenderQueueType
	{
		None, Back, Front, Custom
	}

	public RenderQueueType queueType = RenderQueueType.None;

	public Material renderQueueSourceMaterial;

	public int defaultRenderQueue = 3000;

	// Update is called once per frame
	void Awake ()
	{
		if(targetMaterial == null)
		{
			if(isShareMaterial == false)
			{
				if(renderer != null)
				{
#if UNITY_EDITOR
					if(Application.isPlaying)
					{
						targetMaterial = renderer.material;
					}
					else
					{
						targetMaterial = renderer.sharedMaterial;
					}

#else
					targetMaterial = renderer.material;
#endif

				}
			}
			else
			{
				if(renderer != null)
				{
					targetMaterial = renderer.sharedMaterial;
				}
			}
		}

		if(targetMaterial != null)
		{
			defaultRenderQueue = targetMaterial.renderQueue;

			switch(queueType)
			{
			case RenderQueueType.Back:
				if(renderQueueSourceMaterial != null)
				{
					targetMaterial.renderQueue = renderQueueSourceMaterial.renderQueue - 1;
				}
				break;
			case RenderQueueType.Front:
				if(renderQueueSourceMaterial != null)
				{
					targetMaterial.renderQueue = renderQueueSourceMaterial.renderQueue + 1;
				}
				break;
			case RenderQueueType.Custom:
				targetMaterial.renderQueue = renderQueue;
				break;
			}
		}
	}


	void Update () 
	{
		if(targetMaterial != null)
		{
			if(useColor)
			{
				targetMaterial.SetColor(targetShaderProperty, color);
			}

#if UNITY_EDITOR
			if(Application.isPlaying == false)
			{
				if(targetMaterial != null)
				{
					switch(queueType)
					{
					case RenderQueueType.Back:
						if(renderQueueSourceMaterial != null)
						{
							targetMaterial.renderQueue = renderQueueSourceMaterial.renderQueue - 1;
						}
						break;
					case RenderQueueType.Front:
						if(renderQueueSourceMaterial != null)
						{
							targetMaterial.renderQueue = renderQueueSourceMaterial.renderQueue + 1;
						}
						break;
					case RenderQueueType.Custom:
						targetMaterial.renderQueue = renderQueue;
						break;
					}
				}
			}
#endif
		}
	}


	void OnDestroy()
	{
		targetMaterial = null;
	}


}
