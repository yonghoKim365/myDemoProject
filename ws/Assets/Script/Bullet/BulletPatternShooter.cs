using System;
using UnityEngine;

public class BulletPatternShooter
{
	
	public BulletPatternShooter ()
	{
	}
	
//	private bool _isPlayerBullet = false;
//	
//	public void init(BulletPatternData bulletPatternData, Vector3 position, bool isPlayerBullet, Character cha = null)
//	{
//		_isPlayerBullet = isPlayerBullet;
//		
//		_startPosition = position;
//		_nowBulletPatternData = new BulletPatternData[1]{bulletPatternData};
//		_currentBulletPatternIndex = 0;
//		_nowBulletIndex = 0;
//		_shootDelay = 0.0f;
//		_prevBulletTime = 0;
//		_bulletConditionIndex = 0;	
//		_bulletTurnDelayLevel = 0;
//		isDelete = false;
//		_canShoot = true;	
//		_cha = cha;
//	}
//	
//	private Character _cha = null;
//	
//	private Vector3 _startPosition;
//	
//	protected bool _hasShootAni = false;
//	protected float _shootAniTime = 0.0f;
//	
//	protected BulletData _bulletData;
//	protected float _shootDelay = 1.0f;
//	protected float _prevBulletTime = 0.0f;
//	protected int _bulletAngle = 200;		
//	protected int _nowBulletIndex = 0;
//	protected string _nowBulletMotionType;
//	protected BulletPatternData _tempBulletPatternData;
//	protected int _currentBulletPatternIndex = 0;
//	protected BulletPatternData[] _nowBulletPatternData;
//	protected int _bulletTurnDelayLevel = 0;
//	protected int _bulletConditionIndex = 0;
//	
//	private bool _canShoot = true;
//	
//	public bool isDelete = false;
//	
//	private Vector3 _v;
//	
//	public void update()
//	{
//		if(_canShoot == false) return;
//		
//		if(_nowBulletPatternData == null || _nowBulletPatternData.Length == 0) return;
//		
//		_prevBulletTime += GameManager.globalDeltaTime;
//		
//		if(_prevBulletTime > _shootDelay || _shootDelay == 0.0f)
//		{
//			if(_shootDelay == 0.0f) _prevBulletTime = 0.0f;
//			else _prevBulletTime -= _shootDelay;
//			
//			// 총알 패턴을 가져온다. 이번에 쏠 녀석.
//			_tempBulletPatternData = _nowBulletPatternData[_currentBulletPatternIndex];
//			
//			// 이번턴에 쏘는 녀석이 이번턴 갯수보다 작다. 그럼 작업 시작.
//			if(_nowBulletIndex < _tempBulletPatternData.number)
//			{
//				_bulletAngle = _tempBulletPatternData.startAngle + _tempBulletPatternData.angleOffset * _nowBulletIndex;	
//				
//				if(_bulletAngle < 0) _bulletAngle += 360;
//				else if(_bulletAngle >= 360) _bulletAngle %= 360;
//				
//				// 이번턴의 총알 id들을 검사. id가 하나도 없으면 실제 총알은 발사하지 않는거다...
//				
//				bool justDummyTurn = false;
//				
//				
//				if(_tempBulletPatternData.bulletIdLength > 1)
//				{	
//					_bulletData = GameManager.info.bulletData[_tempBulletPatternData.ids[_nowBulletIndex]];
//				}
//				else if(_tempBulletPatternData.bulletIdLength == 0)
//				{
//					justDummyTurn = true;
//				}
//				else
//				{
//					_bulletData = GameManager.info.bulletData[_tempBulletPatternData.ids[0]];
//				}
//				
//				if(justDummyTurn == false)
//				{
//					_v.x = _startPosition.x;
//					_v.y = _startPosition.y;
//					_v.z = _startPosition.z;
//					
//					if(_tempBulletPatternData.useDisplayPosition)
//					{
//						_v.x = _tempBulletPatternData.displayX;
//						_v.y = _tempBulletPatternData.displayY;
//					}					
//					
//					_v.x += (_tempBulletPatternData.startXpos + _tempBulletPatternData.xPosOffset * _nowBulletIndex);
//					_v.y += (_tempBulletPatternData.startYpos + _tempBulletPatternData.yPosOffset * _nowBulletIndex);
//					
//					shootMissleDetail(_v);
//				}				
//				
//				_shootDelay = _tempBulletPatternData.delay;
//				
//				++_nowBulletIndex;
//			}			
//			
//			
//			if(_nowBulletIndex >= _tempBulletPatternData.number)
//			{
//				// 턴간 딜레이를 입력한다.
//				_shootDelay = _tempBulletPatternData.turnDelay[_bulletTurnDelayLevel];
//				
//				if(_shootDelay == -1)
//				{
//					_canShoot = false;
//					isDelete = true;
//				}
//				
//				_prevBulletTime = 0.0f;
//				
//				_nowBulletIndex = 0;
//				++_currentBulletPatternIndex;
//				
//				if(_currentBulletPatternIndex >= _nowBulletPatternData.Length)
//				{
//					_currentBulletPatternIndex = 0;
//				}
//			}
//			
//			if(_shootDelay == 0.0f)
//			{
//				update ();
//			}
//		}
//	}
//	
//	
//	private Bullet _tb;
//	protected void shootMissleDetail(Vector3 position)
//	{
//		/*
//		_tb = GameManager.me.bulletManager.getBullet();
//		_tb.init(_isPlayerBullet, _bulletData, _bulletData.speed, _bulletAngle, position, _cha);			
//		_tb.enabled = true;		
//		*/
//	}	
	
}

