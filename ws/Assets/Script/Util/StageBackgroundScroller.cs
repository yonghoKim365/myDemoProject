using System;
using System.Collections;
using UnityEngine;

public class StageBackgroundScroller
{
	public StageBackgroundScroller ()
	{
	}

	private tk2dSprite[] _sprites;
	private Transform[] _transforms;
	
	private float _scrollSpeed = 0.0f;
	private float _speedOffset = 1.0f;
	
	private float _width = 0.0f;
	private float _height = 0.0f;
	
	private int[] _indexNum;
	
	public bool isScroll = true;
	
	private int _scrollType = 0;
	
	public const int SCROLL_VERTICAL = 0;
	public const int SCROLL_HORIZONTAL = 1;
	
	public float scrollValue = 0.0f;
	
	private int _needNum = 0;
	
	/*
	public void changeBackground(BackgroundData bgData)
	{
		for(int i = 0; i < _needNum; ++i)
		{
			tk2dSpriteCollectionData scd = GameManager.resourceManager.getSpriteCollection(bgData.id);
			_sprites[i].SwitchCollectionAndSprite(scd, scd.GetSpriteIdByName(bgData.id));
		}
		
		_width = _sprites[0].GetBounds().extents.x*2.0f;
		_height = _sprites[0].GetBounds().extents.y*2.0f;		
		
		resetPosition();
		
		//_hasOverLayer = (bgData.baseEffect != BackgroundData.NONE);
		
		if(_hasOverLayer)
		{
			foreach(tk2dSprite spr in _overSpr)
			{
				tk2dSpriteCollectionData scd = GameManager.resourceManager.getSpriteCollection(bgData.baseEffect);
				spr.SwitchCollectionAndSprite(scd, scd.GetSpriteIdByName(bgData.baseEffect));
				spr.renderer.enabled = true;
			}
		}
		else
		{
			foreach(tk2dSprite spr in _overSpr)
			{
				spr.renderer.enabled = false;
			}			
		}
	}
	*/
	
	
	public void changeBackground(string id)
	{
		for(int i = 0; i < _needNum; ++i)
		{
			tk2dSpriteCollectionData scd = GameManager.resourceManager.getSpriteCollection(id);
			_sprites[i].SetSprite(scd, scd.GetSpriteIdByName(id));
		}
		
		_width = _sprites[0].GetBounds().extents.x*2.0f;
		_height = _sprites[0].GetBounds().extents.y*2.0f;		
		
		resetPosition();
		
		_hasOverLayer = false;	
	}
	
	private bool _isEnabled = false;
	
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;
			
			if(_sprites != null)
			{
				foreach(tk2dSprite spr in _sprites)
				{
					spr.gameObject.SetActive(value);//spr.renderer.enabled = value;
				}
				
				
				if(_hasOverLayer)
				{
					foreach(tk2dSprite spr in _overSpr)
					{
						spr.gameObject.SetActive(value);//spr.renderer.enabled = value;
					}
				}
			}
		}
		get
		{
			return _isEnabled;
		}
	}
	
	
	
	private bool _hasOverLayer =  false;
	
	public void create(int scrollType, float scrollSpeed, Transform parent, string spriteName, float depth, bool useOverLayer = false)
	{
		_scrollSpeed = scrollSpeed;
		
		// 첫번째 이미지를 가져온다...
		GameObject gobj = new GameObject("background");	
		gobj.transform.parent = parent;//
		
		tk2dSprite spr = gobj.AddComponent<tk2dSprite>();
		tk2dSpriteCollectionData scd = GameManager.resourceManager.getSpriteCollection(spriteName);
		spr.SetSprite(scd, scd.FirstValidDefinitionIndex);
		spr.renderer.material = scd.FirstValidDefinition.material;
		spr.Build();

		_scrollType = scrollType;
		
		_width = spr.GetBounds().extents.x*2.0f;
		_height = spr.GetBounds().extents.y*2.0f;
		
		_needNum = Mathf.RoundToInt((GameManager.me.tk2dGameCamera.targetResolution.y ) / _height) + 2;
		
		_indexNum = new int[_needNum];
		
		_sprites = new tk2dSprite[_needNum];	
		_transforms = new Transform[_needNum];
		
		_vectors = new Vector3[_needNum];
		
		_tempNum = new int[_needNum];
		
		int i = 0;
		
		for(i = 0; i < _needNum; ++i)
		{
			_vectors[i] = Vector3.zero;
			
			if(i == 0)
			{
				_sprites[i] = spr;
			}
			else
			{
				GameObject tempGobj = new GameObject("background");	
				tempGobj.transform.parent = parent;//
				
				_sprites[i] = tempGobj.AddComponent<tk2dSprite>();
				tk2dSpriteCollectionData tScd = GameManager.resourceManager.getSpriteCollection(spriteName);
				_sprites[i].SetSprite(tScd, tScd.GetSpriteIdByName(spriteName));
				_sprites[i].renderer.material = tScd.FirstValidDefinition.material;
				_sprites[i].Build();
			}
			
			_indexNum[i] = i;
			_transforms[i] = _sprites[i].transform;			
		}		
		
		_vectors[0] = _transforms[0].position;
		_vectors[0].x = GameManager.me.tk2dGameCamera.targetResolution.x/2.0f;
		_vectors[0].y = GameManager.me.tk2dGameCamera.targetResolution.y/2.0f;
		_vectors[0].z = depth;
		
		for(i = 0; i < _needNum; ++i)
		{
			_transforms[i].position = _vectors[0];
		}
		
		_hasOverLayer = useOverLayer;
		if(useOverLayer) createOverLayer(spriteName);
		//createOverLayer(spriteName);
		
		resetPosition();
	}
	
	private Vector3 _v;
	
	private tk2dSprite[] _overSpr;
	private float _overColorIndex = 0.0f;
	private float _overColorIndexDirection = 1.0f;
	
	private void createOverLayer(string spriteName)
	{
		_overSpr = new tk2dSprite[_needNum];
		
		for(int i = 0; i < _needNum; ++i)
		{
			GameObject tempGobj = new GameObject("overBG");	
			tk2dSprite spr = tempGobj.AddComponent<tk2dSprite>();
			tk2dSpriteCollectionData tScd = GameManager.resourceManager.getSpriteCollection(spriteName);
			spr.SetSprite(tScd, tScd.GetSpriteIdByName(spriteName));
			spr.renderer.material = tScd.FirstValidDefinition.material;
			spr.Build();		
			
			tempGobj.transform.parent = _transforms[i];
			
			_v = tempGobj.transform.localPosition;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.z = -0.1f;
			tempGobj.transform.localPosition = _v;
			
			_overSpr[i] = spr;
			
			spr.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		}
	}
	
	private float _overColorEffectTimer = 0.0f;
	private float _skipColorTimer = 0;
	
	private Color _tempColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	private void setOverColor()
	{
		_skipColorTimer += GameManager.globalDeltaTime;
		if(_skipColorTimer < 0.1f) return;
		_skipColorTimer = 0.0f;
		
		_overColorEffectTimer += GameManager.globalDeltaTime;
		if(_overColorEffectTimer <= 0.0f) return;
		
		_overColorIndex += 0.1f*_overColorIndexDirection;
		
		if(_overColorIndex <= 0.0f)
		{
			_overColorIndex = 0.0f;
			_overColorIndexDirection *= -1.0f;
			_overColorEffectTimer = -1.0f;
		}
		else if(_overColorIndex >= 1.0f)
		{
			_overColorIndex = 1.0f;
			
			_overColorIndexDirection *= -1.0f;
			
			_overColorEffectTimer = -0.5f;
		}
		
		_tempColor.a = _overColorIndex;
		//Color c = new Color(1.0f, 1.0f, 1.0f, _overColorIndex);
		
		for(i = 0; i < _needNum; ++i)
		{
			_overSpr[i].color = _tempColor;
		}
	}
	

	
	private Vector3 _v1;

	private Vector3[] _vectors;
	
	private int[] _tempNum;
	
	
	public float scrollSpeed
	{
		set
		{
			_scrollSpeed = value;
		}
		get
		{
			return _scrollSpeed;
		}
	}
	
	private Transform _tempTransform;
	
	private int i;
	private int j;
	public void update()
	{
		scrollValue = 0.0f;

		if(_hasOverLayer) setOverColor();
		
		if(!isScroll) return;
		
		for(i = 0; i < _needNum; ++i)
		{
			_vectors[i] = _transforms[_indexNum[i]].position;
		}
		
		scrollValue = scrollSpeed * GameManager.globalDeltaTime;
		
		if(_scrollType == SCROLL_VERTICAL)
		{
			_vectors[0].y -= scrollValue;
			
			for(j = 1; j < _needNum; ++j)
			{
				_vectors[j].y = _height + _vectors[j-1].y - _speedOffset;
			}
			
			for(j = 0; j < _needNum; ++j)
			{
				_transforms[j].position = _vectors[j];
			}			
			
			
			if(_vectors[0].y < -_height / 2.0f) // 제일 하단 녀석이 화면을 벗어났으면...
			{
				// 제일 하단 녀석을 제일 위로 올려야한다...
				_tempTransform = _transforms[0];		
				
				for(j = 0; j < _needNum; ++j)
				{
					if(j == _needNum-1)
					{
						_transforms[j] = _tempTransform;
					}
					else
					{
						_transforms[j] = _transforms[j+1];	
					}
				}
			}
		}
		else // 좌우 이동...
		{
			_vectors[0].x -= scrollValue;
			
			for(j = 1; j < _needNum; ++j)
			{
				_vectors[j].x = _width + _vectors[j-1].x - _speedOffset;
			}
			
			for(j = 0; j < _needNum; ++j)
			{
				_transforms[j].position = _vectors[j];
			}			
			
			
			if(_vectors[0].x < -_width / 2.0f) // 제일 하단 녀석이 화면을 벗어났으면...
			{
				// 제일 하단 녀석을 제일 위로 올려야한다...
				_tempTransform = _transforms[0];		
				
				for(j = 0; j < _needNum; ++j)
				{
					if(j == _needNum-1)
					{
						_transforms[j] = _tempTransform;
					}
					else
					{
						_transforms[j] = _transforms[j+1];	
					}
				}
			}			
		}
	}
	
	
	private void resetPosition()
	{
		int i, j;
		
		if(_scrollType == SCROLL_VERTICAL)
		{
			for(i = 0; i < _needNum; ++i)
			{
				_vectors[i]	 = _transforms[_indexNum[i]].position;
			}
			
			_vectors[0].y = 0.0f;
			
			for(i = 1; i < _needNum; ++i)
			{
				_vectors[i].y = _height + _vectors[i-1].y - _speedOffset;
			}
			
			for(j = 0; j < _needNum; ++j)
			{
				_transforms[_indexNum[j]].position = _vectors[j];
			}
			
		}
		else // 좌우 이동....
		{
			for(i = 0; i < _needNum; ++i)
			{
				_vectors[i]	 = _transforms[_indexNum[i]].position;
			}
			
			_vectors[0].x = 0.0f;
			
			for(i = 1; i < _needNum; ++i)
			{
				_vectors[i].x = _width + _vectors[i-1].x - _speedOffset;
			}
			
			for(j = 0; j < _needNum; ++j)
			{
				_transforms[_indexNum[j]].position = _vectors[j];
			}			
				
		}		
	}
	
}

