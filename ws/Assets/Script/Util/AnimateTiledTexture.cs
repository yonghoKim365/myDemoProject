using UnityEngine;
using System.Collections;

class AnimateTiledTexture : MonoBehaviour
{
	public int columns = 2;
	public int rows = 2;

	public float framesPerSecond = 10f;

	public bool isLoop = false;

	//the current frame to display
	public int index = 0;
	Vector2 size;

	public ParticleSystem particle;

	public bool useSharedMaterial = true;

	void OnEnable ()
	{
		index = 0;

		size = new Vector2(1f / columns, 1f / rows);

		if(useSharedMaterial)
		{
			renderer.sharedMaterial.SetTextureScale("_MainTex", size);
		}
		else
		{
			renderer.material.SetTextureScale("_MainTex", size);
		}


		StartCoroutine(updateTiling());
	}
	
	private IEnumerator updateTiling()
	{
		if(particle != null) particle.Play();

		while (true)
		{
			//move to the next index
			if (index >= rows * columns)
			{
				if(isLoop)
				{
					index = 0;
				}
				else
				{
					yield break;
					break;
				}
			}

			int xIndex = index % columns;
			int yIndex = index / columns;


			if(useSharedMaterial)
			{
				//split into x and y indexes
				renderer.sharedMaterial.SetTextureOffset("_MainTex", new Vector2 ((float)xIndex * size.x, 
				                                                                  1f - size.y - (float)yIndex * size.y));
			}
			else
			{
				//split into x and y indexes
				renderer.material.SetTextureOffset("_MainTex", new Vector2 ((float)xIndex * size.x, 
				                                                                  1f - size.y - (float)yIndex * size.y));
			}

			yield return new WaitForSeconds(1f / framesPerSecond);

			++index;
		}
		
	}
}