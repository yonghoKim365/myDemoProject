using UnityEngine;
using System.Collections;

public class MapTextureChangeObject : MonoBehaviour 
{
	Renderer _renderer;

//	public Texture2D originalTexture;
//	public Texture2D effectTexture;

	public Material original;
	public Material effect;

	public enum Type
	{
		Main, LightMap
	}

	public Type type = Type.LightMap;

	public void setOriginalTexture()
	{
		if(_renderer == null) _renderer = renderer;
		if(original != null) _renderer.sharedMaterial = original;

//		if(type == Type.Main)
//		{
//			_renderer.sharedMaterial.SetTexture("",originalTexture);
//		}
//		else
//		{
//			_renderer.sharedMaterial.SetTexture("",originalTexture);
//		}
	}

	public void setEffectTexture()
	{
		if(_renderer == null) _renderer = renderer;
		if(effect != null)  _renderer.sharedMaterial = effect;

//		if(type == Type.Main)
//		{
//			_renderer.sharedMaterial.SetTexture("",effectTexture);
//		}
//		else
//		{
//			_renderer.sharedMaterial.SetTexture("",effectTexture);
//		}
	}


	void Awake()
	{
		if(_renderer == null) _renderer = renderer;

		if(original != null && original.shader != null) original.shader = Shader.Find(original.shader.name);
		if(effect != null && effect.shader != null) effect.shader = Shader.Find(effect.shader.name);
	}

	void OnDestroy()
	{
		original = null;
		effect = null;
		_renderer = null;
	}

}
