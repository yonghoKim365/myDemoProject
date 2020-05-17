using UnityEngine;
using System.Collections;

public class TextureAnimationLoopType : MonoBehaviour 
{
	public float xSpeed = 0.5f;
	public float ySpeed = 0.5f;

	private Vector2 _v;
	private float _time = 0.0f;

	public bool useLimit = false;
	public bool limtTypeIsMinus = true;
	public float limit = -0.6f;

	public string targetShaderProperty = "_MainTex";

	public bool isOnOffType = false;

	public enum ScrollType
	{
		Y_ONLY, X_ONLY, BOTH
	}

	public bool isShareMaterial = true;

	public ScrollType scrollType = ScrollType.Y_ONLY;

	private float _delay = 0.0f;

	private bool isOn = false;

	Material material;

	private bool _awakenMaterialIsShared = false;
	void Awake()
	{
		_awakenMaterialIsShared = isShareMaterial;

		if(renderer != null)
		{
			material = (isShareMaterial)?renderer.sharedMaterial:renderer.material;
		}
	}

	public void setMaterialType(bool isShare)
	{
		if(material != null)
		{
			if(isShare != _awakenMaterialIsShared)
			{
				_awakenMaterialIsShared = isShare;
				isShareMaterial = isShare;

				if(renderer != null)
				{
					material = (isShareMaterial)?renderer.sharedMaterial:renderer.material;
				}
			}
		}

		isShareMaterial = isShare;
	}


	void OnDestroy()
	{
		material = null;
	}


	void Update () 
	{ 
		if(isOnOffType)
		{
			_delay += Time.smoothDeltaTime;
			if(_delay > 0.2f)
			{
				if(material != null)
				{
					if(isOn)
					{
						_v.x = 0.0f;
					}
					else
					{
						_v.x = 0.5f;
					}

					isOn = !isOn;

					material.mainTextureOffset = _v;
					//if(isShareMaterial) cachedRenderer.sharedMaterial.mainTextureOffset = _v;
					//else cachedRenderer.material.mainTextureOffset = _v;
				}
			}
		}
		else
		{
			if(PerformanceManager.isLowPc) return;
			
			switch(scrollType)
			{
			case ScrollType.X_ONLY:
				_v.x += _time * xSpeed;
				_v.y = 0.0f;
				break;
			case ScrollType.Y_ONLY:
				_v.x = 0.0f;
				_v.y += _time * ySpeed;
				break;
			case ScrollType.BOTH:
				_v.x += _time * xSpeed;
				_v.y += _time * ySpeed;
				break;
			}
			
			if(useLimit)
			{
				if(limit < 0)
				{
					if(_v.y <= limit)
					{
						_v.y -= limit;
					}
				}
				else
				{
					if(scrollType == ScrollType.Y_ONLY)
					{
						while(_v.y >= limit)
						{
							_v.y -= limit;
						}
					}
				}
			}
			
			if(GameManager.globalDeltaTime >= 0)
			{
				_time = Time.smoothDeltaTime;
				if(material != null)
				{
					material.mainTextureOffset = _v;
//					if(isShareMaterial) 
//					{
//						if(cachedRenderer.sharedMaterial != null) cachedRenderer.sharedMaterial.mainTextureOffset = _v;
//						
//					}
//					else
//					{
//						if(cachedRenderer.material != null) cachedRenderer.material.mainTextureOffset = _v;
//					}
				}
				
			}
			#if UNITY_EDITOR
			else
			{
				_time = Time.smoothDeltaTime;
				
				if(material != null)
				{
					material.mainTextureOffset = _v;

//					if(isShareMaterial) 
//					{
//						if(cachedRenderer.sharedMaterial != null) cachedRenderer.sharedMaterial.mainTextureOffset = _v;
//						
//					}
//					else
//					{
//						if(cachedRenderer.material != null) cachedRenderer.material.mainTextureOffset = _v;
//					}
				}
			}
			#endif

		}


	}
}



