using System;
using UnityEngine;

sealed public class HitObject
{
	private IFloat _x = 0.0f;
	private IFloat _y = 0.0f;
	private IFloat _z = 0.0f;
	
	private IFloat _width = 0.0f;
	private IFloat _height = 0.0f;
	private IFloat _depth = 0.0f;
	
	public IFloat right = 0.0f;
	public IFloat top = 0.0f;
	public IFloat distance = 0.0f;

	public IFloat x
	{
		get
		{
			return _x;
		}
		set
		{
			_x = value;
			right = _x + _width;
		}
	}
	
	public IFloat y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = value;
			top = _y + _height;
		}
	}	
	
	
	public IFloat z
	{
		get
		{
			return _z;
		}
		set
		{
			_z = value;
			distance = _z + _depth;
		}
	}	

	public IFloat width
	{
		set
		{
			_width = value;
			right = _x + value;
			//diagonalWidthDepth = Mathf.Sqrt(_width*_width + _depth*_depth);
		}
		get
		{
			return _width;
		}
	}
	
	
	public IFloat height
	{
		set
		{
			_height = value;
			top = _y + value;
		}
		get
		{
			return _height;
		}
	}	
	
	
	public IFloat depth
	{
		set
		{
			_depth = value;
			distance = _z + value;
			//diagonalWidthDepth = Mathf.Sqrt(_width*_width + _depth*_depth);
		}
		get
		{
			return _depth;
		}
	}		
	
	
	
	public void copy(HitObject h)
	{
		this.x = h.x;
		this.y = h.y;
		this.z = h.z;
		this.width = h.width;
		this.height = h.height;
		this.depth = h.depth;
	}
	
	
	public HitObject (float x = 0.0f, float y = 0.0f, float z = 1.0f, float width = 0.0f, float height = 0.0f, float depth = 1.0f)
	{
		this._x = x;
		this._y = y;
		this._z = z;
		this.width = width;
		this.height = height;
		this.depth = depth;
	}
	
	
	
	//public float diagonalWidthDepth = 1.0f;

	private IVector3 _boundCenter;
	private IVector3 _boundExtens;
	
	public HitObject( UnityEngine.Bounds bounds)
	{
		_boundCenter = bounds.center;
		_boundExtens = bounds.extents;
		
		width = _boundExtens.x * 2.0f;
		height = _boundExtens.y * 2.0f;
		depth = _boundExtens.z * 2.0f;
		
		x = _boundCenter.x - _boundExtens.x;
		y = _boundCenter.y - _boundExtens.y;
		z = _boundCenter.z - _boundExtens.z;

		lineLeft = _boundCenter.x - _boundExtens.x;
		lineRight = _boundCenter.x + _boundExtens.x;
	}
	
	
	public void init(IVector3 boundCenter, IVector3 boundExtens)
	{
		boundCenter.x = MathUtil.Round(boundCenter.x * 100.0f) / 100.0f;
		boundCenter.y = MathUtil.Round(boundCenter.y * 100.0f) / 100.0f;
		boundCenter.z = MathUtil.Round(boundCenter.z * 100.0f) / 100.0f;
		
		boundExtens.x = MathUtil.Round(boundExtens.x * 100.0f) / 100.0f;
		boundExtens.y = MathUtil.Round(boundExtens.y * 100.0f) / 100.0f;
		boundExtens.z = MathUtil.Round(boundExtens.z * 100.0f) / 100.0f;

		width = boundExtens.x * 2.0f;
		height = boundExtens.y * 2.0f;
		depth = boundExtens.z * 2.0f;
		
		_boundCenter = boundCenter;
		_boundExtens = boundExtens;
		
		x = boundCenter.x - boundExtens.x;
		y = boundCenter.y - boundExtens.y;
		z = boundCenter.z - boundExtens.z;	

		lineLeft = _boundCenter.x - _boundExtens.x;
		lineRight = _boundCenter.x + _boundExtens.x;

		//Log.log("init center: " + boundCenter + "    extends: " + boundExtens);
	}
	
	
	
	private bool _isXHit = false;
	private bool _isYHit = false;
	private bool _isZHit = false;


	public bool intersects2(float targetX, float targetZ, float targetRight, float targetDist)
	{
		_isXHit = false;
		_isZHit = false;
		
		if(targetX < _x)
		{
			if(targetRight >= _x)
			{
				_isXHit = true;
			}
			else return false;
		}
		else if(right >= targetRight)
		{
			_isXHit = true;
		}
		else return false;
		
		if(targetZ < _z)
		{
			if(targetDist >= _z)
			{
				_isZHit = true;
			}
			else return false;
		}
		else if(distance >= targetZ)
		{
			_isZHit = true;
		}
		else return false;
		
		return (_isXHit && _isZHit);
	}


	// slow...
	public bool intersects(IVector3 pos, IVector3 targetPos, HitObject target)
	{
		setPosition(pos);
		target.setPosition(pos);
		return intersects(target);
	}
	
	
	public bool intersects(HitObject target)
	{
//		Log.log("this: ", "x:", this.x, "y:",this.y, "z:",this.z, "right:",this.right, "top:", this.top, "dist:",this.distance);
//		Log.log("target: ","x:",target.x, "y:",target.y, "z:",target.z, "right:",target.right, "top:", target.top, "dist:",target.distance);
		
		_isXHit = false;
		_isYHit = false;
		_isZHit = false;
		
		if(target.x < _x)
		{
			if(target.right >= _x)
			{
				_isXHit = true;
			}
			else return false;
		}
		else if(this.right >= target.x)
		{
			_isXHit = true;
		}
		else return false;
		
		
		
		if(target.y < _y)
		{
			if(target.top >= _y)
			{
				_isYHit = true;
			}
			else return false;
		}
		else if(this.top >= target.y)
		{
			_isYHit = true;
		}
		else return false;

		
		if(target.z < _z)
		{
			if(target.distance >= _z)
			{
				_isZHit = true;
			}
			else return false;
		}
		else if(distance >= target.z)
		{
			_isZHit = true;
		}
		else return false;		
		
		
		return (_isXHit && _isYHit && _isZHit);
	}






	public bool intersectsBullet(HitObject target)
	{

		_isXHit = false;
		_isYHit = false;
		_isZHit = false;

		if(target.top >= 0 && this.top >= 0)
		{
			_isYHit = true;
		}
		else return false;

		if(target.x < _x)
		{
			if(target.right >= _x)
			{
				_isXHit = true;
			}
			else return false;
		}
		else if(this.right >= target.x)
		{
			_isXHit = true;
		}
		else return false;


		if(target.z < _z)
		{
			if(target.distance >= _z)
			{
				_isZHit = true;
			}
			else return false;
		}
		else if(distance >= target.z)
		{
			_isZHit = true;
		}
		else return false;		
		
		
		return (_isXHit && _isYHit && _isZHit);
	}



	
	public void setPosition(IVector3 pos)
	{
		x = pos.x + _boundCenter.x - _boundExtens.x;
		y = pos.y + _boundCenter.y - _boundExtens.y;
		z = pos.z + _boundCenter.z - _boundExtens.z;
	}

	public float lineLeft = 0.0f;
	public float lineRight = 0.0f;
	
}

