using System;
using UnityEngine;

sealed public class Rectangle
{
	private float _x = 0.0f;
	private float _y = 0.0f;
	
	private float _width = 0.0f;
	private float _height = 0.0f;
	
	private float _right = 0.0f;
	private float _top = 0.0f;
	
	
	public float x
	{
		get
		{
			return _x;
		}
		set
		{
			_x = value;
			_right = _x + _width;
		}
	}
	
	
	public float y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = value;
			_top = _y + _height;
		}
	}	
	
	
	public float right
	{
		get
		{
			return _right;
		}
	}
	
	public float top
	{
		get
		{
			return _top;
		}
	}
	
	public float width
	{
		set
		{
			_width = value;
			_right = _x + value;
		}
		get
		{
			return _width;
		}
	}
	
	
	public float height
	{
		set
		{
			_height = value;
			_top = _y + value;
		}
		get
		{
			return _height;
		}
	}	
	
	
	public void copy(Rectangle r)
	{
		this.x = r.x;
		this.y = r.y;
		this.width = r.width;
		this.height = r.height;
	}
	
	
	public Rectangle (float x = 0.0f, float y = 0.0f, float width = 0.0f, float height = 0.0f)
	{
		this._x = x;
		this._y = y;
		this.width = width;
		this.height = height;
	}
	
	public bool contains(float posX, float posY)
	{
		if(posX >= this.x)
		{
			if(posX <= this._right)
			{
				if(posY >= this.y)
				{
					if(posY <= this._top)
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}	


	public bool containsTouchPoint(Vector2 touchPoint)
	{
		return contains ( Util.screenPositionWithCamViewRect(touchPoint) );
	}

	
	public bool contains(Vector2 pos)
	{
		if(pos.x >= this.x)
		{
			if(pos.x <= this._right)
			{
				if(pos.y >= this.y)
				{
					if(pos.y <= this._top)
					{
						return true;
					}
				}
			}
		}
		
		return false;
	}
	
	
	private bool _isXHit = false;
	private bool _isYHit = false;
	
	public bool intersects(Rectangle target)
	{
		_isXHit = false;
		_isYHit = false;
		
		if(target.x < this.x)
		{
			if(target._right >= this.x)
			{
				_isXHit = true;
			}
			else return false;
		}
		else if(this._right >= target.x)
		{
			_isXHit = true;
		}
		else return false;
		
		
		
		if(target.y < this.y)
		{
			if(target._top >= this.y)
			{
				_isYHit = true;
			}
			else return false;
		}
		else if(this._top >= target.y)
		{
			_isYHit = true;
		}
		else return false;
		
		return (_isXHit && _isYHit);
	}
}

