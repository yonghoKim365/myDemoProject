using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour, IManagerBase
{
	private IVector3 _v;
	
	//-----------------------------------------------------//
	private Stack<Bullet> _bulletPool = new Stack<Bullet>();	


	public Bullet defaultBullet;

	private int _chainUniqueNo = 0;
	private int _bulletUniqueNo = 0;
	public Bullet getBullet(bool isInit = false)
	{
		Bullet b;
		if(_bulletPool.Count > 0) b = _bulletPool.Pop();
		else
		{
			b = (Bullet)Instantiate(defaultBullet);//new Bullet(new GameObject());//(new Bullet((GameObject)Instantiate(GameManager.me.bullet)));
			b.gobj = b.gameObject;
			//b.bTransform = b.transform;
			//b.bTransform.parent = GameManager.me.stage;

			//b.shadow = getBulletShadow();
			
			if(isInit)
			{
				b.isEnabled = false;
				_bulletPool.Push(b);
			}
		}
		
		b.uniqueNo = _bulletUniqueNo;
		++_bulletUniqueNo;
		
		return b;
	}
		
	
	public void setBullet(Bullet bullet)
	{
		bullet.isDeleteObject = false;
		bullet.isEnabled = false;
		bullet.attackerInfo.clear();
		_bulletPool.Push(bullet);
	}

	public void destroyBulletAsset(Bullet bullet)
	{
		bullet.isDeleteObject = false;
		bullet.isEnabled = false;
		bullet.attackerInfo.clear();
		bullet.destroyAsset();
		Destroy(bullet.gameObject);
	}


	void Awake()
	{
	}

	
	public void init()
	{
	}
	
	
	public void clearStage(bool flag = true)
	{
		_bulletUniqueNo = 0;
		_chainUniqueNo = 0;

		delayBullets.Clear();

		int len = monsterBulletList.Count;
		for(int i = len -1; i >= 0; --i)
		{
			monsterBulletList[i].setDelete();
			destroyBulletAsset(monsterBulletList[i]);
		}		

		len = playerBulletList.Count;		
		for(int i = len -1; i >= 0; --i)
		{
			playerBulletList[i].setDelete();
			destroyBulletAsset(playerBulletList[i]);
		}

		while(_bulletPool.Count > 0)
		{
			Bullet b = _bulletPool.Pop();
			b.setDelete();
			destroyBulletAsset(b);
		}

		len = chainLightningList.Count;
		for(int i = len - 1; i >= 0; --i)
		{
			setChainLightning(chainLightningList[i]);
		}

		monsterBulletList.Clear();
		playerBulletList.Clear();
		chainLightningList.Clear();
	}	
	
	
	public List<ChainLightning> chainLightningList = new List<ChainLightning>();


	private Dictionary<string, Stack<ChainLightning>> _chainLightningPool = new Dictionary<string, Stack<ChainLightning>>();

	public ChainLightning getChainLightning(string chainId, string hitId)
	{
		ChainLightning cl;

		if(_chainLightningPool.ContainsKey(chainId) && _chainLightningPool[chainId].Count > 0)
		{
			cl = _chainLightningPool[chainId].Pop();
		}
		else
		{
			cl = new ChainLightning(chainId, hitId);
		}
		 
		cl.uniqueNo = _bulletUniqueNo;
		++_chainUniqueNo;
		chainLightningList.Add(cl);
		return cl;
	}


	public void setChainLightning(ChainLightning cl)
	{
		cl.isEnabled = false;

		if(_chainLightningPool.ContainsKey(cl.type) == false)
		{
			_chainLightningPool.Add(cl.type, new Stack<ChainLightning>());
		}

		_chainLightningPool[cl.type].Push(cl);
	}
	
	
	
	public List<Bullet> monsterBulletList = new List<Bullet>();		
	public List<Bullet> playerBulletList = new List<Bullet>();		


	
	
	private HitObject _tempHitObject;
	private Monster _tempMonster;
	private Bullet _tempBullet;
	private Bullet _tempBullet2;
	
	
	
	public void update()
	{
		for(int i = chainLightningList.Count -1; i >= 0; --i)
		{
//			if(GameManager.me.isPlaying == false) break;
			if(chainLightningList[i].isDeleteObject)
			{
				setChainLightning(chainLightningList[i]);
				chainLightningList.RemoveAt(i);
			}
			else
			{
				chainLightningList[i].update();
			}
		}

		updateDelayBullets();

		if(GameManager.me.characterManager.updatePlayerFirst)
		{
			updateBullets(playerBulletList, GameManager.me.characterManager.monsters);
			updateBullets(monsterBulletList, GameManager.me.characterManager.playerMonster);
		}
		else 
		{
			updateBullets(monsterBulletList, GameManager.me.characterManager.playerMonster);
			updateBullets(playerBulletList, GameManager.me.characterManager.monsters);
		}
	}	


	public List<Bullet> delayBullets = new List<Bullet>();

	void updateDelayBullets()
	{
		for(int i = delayBullets.Count - 1; i >= 0; --i)
		{
			delayBullets[i].delay -= GameManager.globalDeltaTime;

			if(delayBullets[i].delay < 0)
			{
				delayBullets[i].isEnabled = true;
				delayBullets[i].setReadyAttachedParticleEffect(false);
				delayBullets.RemoveAt(i);
			}
		}
	}


	
	void updateBullets(List<Bullet> bulletList, List<Monster> targetMonsterList)
	{

		int j = 0;
		bool isSkip = false;
		bool isHit = false;
		IFloat _tempF;

		for(int i = bulletList.Count -1 ; i >= 0; --i)
		{
			if(bulletList[i].isDeleteObject)
			{
				setBullet(bulletList[i]);
				bulletList.RemoveAt(i);
				continue;
			}

			_tempBullet = bulletList[i];

			_tempBullet.update();


			if(GameManager.me.currentScene != Scene.STATE.PLAY_BATTLE && UIPopupSkillPreview.isOpen == false)
			{
				continue;
			}


			_tempHitObject = _tempBullet.getHitObject();
			
			if(_tempBullet.isCollisionCheckAtEndOnly || _tempBullet.canCollide == false || _tempBullet.delay > 0)
			{
				
			}
			else
			{
				//======= 곡사포가 아닌것만 체크.
				///////////////////////////////////////////////////////////////////////////////////
				
				// 몬스터와 충돌 체크			
				int monsterCount = targetMonsterList.Count;
				isSkip = false;

				// 지속형 스킬일때 피격횟수를 매번 초기화하기위해.
				_tempBullet.checkTargetNum();


				for(j = 0 ; j < monsterCount; ++j)
				{
//					if(GameManager.me.isPlaying == false) break;

					_tempMonster = targetMonsterList[j];

					if(_tempMonster.isEnabled == false) continue;  // || _tempMonster.skipHitCheck

					isHit = false;

					if(_tempBullet.isCircleColliderType)
					{
						_tempF = _tempBullet.targetRange + _tempMonster.damageRange;
						if(_tempF >= VectorUtil.DistanceXZ(_tempBullet.bTransformPosition, _tempMonster.cTransformPosition))
						{
							isHit = true;
						}
					}
					else
					{
						if(_tempHitObject.intersectsBullet(_tempMonster.getHitObject()))
						{
							isHit = true;
						}
					}
					
					if(isHit)
					{	
						//Log.log("_tempBullet : " + _tempBullet + "_tempMonster : " + _tempMonster.resourceId);

						_tempBullet.prevHp = _tempBullet.hp.Get();

						if(_tempBullet.damageToCharacter != null) _tempBullet.damageToCharacter(_tempMonster);
						
						if(_tempMonster.isDeleteObject ) //|| (_tempMonster.hp <= 0 && _tempMonster.invincible == false))
						{
							//Log.log("==== _tempMonster.isDeleteObject !!");
							_tempBullet.bulletData.hitDeadActionStart(_tempMonster.cTransformPosition);
						}
						
						if(_tempBullet.hp.Get() <= 0 && _tempBullet.invincible == false)
						{
							//_tempBullet.setDelete();
							_tempBullet.destroy();
							//playerBulletList.RemoveAt(i);
							isSkip = true;
							break;
						}
						else
						{
							if(_tempBullet.prevHp > _tempBullet.hp.Get() && _tempBullet.damageCheckTypeNo == 5 )
							{
								int r = _tempBullet.retargetingRange.Get();
								
								if(r > 0)
								{
									_tempBullet.startRetargeting(_tempMonster);
								}
							}
						}
					}
				}


				if(isSkip) continue;
				// 곡사포가 아닌것들 여기까지 =====
			}
			// 화면 밖으로 벗어났는지 검사.
			if(_tempBullet.isOutSide()) // 화면 밖을 벗어났으면...
			{
				_tempBullet.setDelete();
			}
		}

		_tempBullet = null;
	}


	
}
