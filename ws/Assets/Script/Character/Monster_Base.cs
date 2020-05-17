using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class Monster : MonoBehaviour//, ITweenableObject
{
	public enum SpecialType
	{
		NONE, SKIP_DESTORY_CHECK
	}

	public SpecialType specialType = SpecialType.NONE;

	public static int uniqueIndex = -1;

	public IFloat checkWalkTime = 0.0f;

	// 오브젝트 몬스터 중 양쪽 애들 다 길막기가 되는 녀석.
	public bool isBlockMonster = false;

	public string prevPlayingAniName = string.Empty;

//	public Xbool isInTargetZone = false; // 이 캐릭터가 타겟존에 있나?

	public Dictionary<string, AnimationState> animationStates = new Dictionary<string, AnimationState>(StringComparer.Ordinal);

	public Pet pet;
	
	protected AniData _tempAniData;	
	
	// 회피 확률.
	public Xint attackAvoidPercent = 0;
	
	protected int i = 0;
	public bool hasAni = true;

	public bool useRimShader = false;

	public int monsterUISlotIndex = -1;

	//public Dictionary<string, GameObject> parts = new Dictionary<string, GameObject>();

	public Dictionary<string, GameObject> heads;
	public Dictionary<string, GameObject> bodies;
	public Dictionary<string, GameObject> weapons;


	public bool hasShootingPos = false;
	public int[] shootingPos = null;

	public Transform shootingHand = null;

	public Dictionary<string, int[][]> shootingPositions = new Dictionary<string, int[][]>(StringComparer.Ordinal);
	public Dictionary<string, Transform[]> shootingTransforms = new Dictionary<string, Transform[]>(StringComparer.Ordinal);

	public Dictionary<string, Transform> effectParents;

	public string resourceId = null;

	public enum TYPE
	{
		PLAYER, HERO, UNIT, NPC, EFFECT, NONE
	}

	public enum ResourceType
	{
		PLAYER, MONSTER, PET, EFFECT
	}
	
	public MonsterStat stat = new MonsterStat();

	public bool isCutSceneOnlyCharacter = false;
	


	public float summonEffectSize = 1.0f;

	public float uiSize
	{
		get
		{
			ModelData md = GameManager.info.modelData[resourceId];
			return (md.width + md.height)/2.0f;
		}
	}

	public float uiSize2
	{
		get
		{
			return GameManager.info.modelData[resourceId].shotScale;
		}
	}


	// 캐릭터에 붙게될 파티클들은 이 사이즈에 따라 크기가 달라진다.
	public float effectSize = 1.0f;

	// 공격 거리 : 실제 모델의 공격 거리. 근접 공격 캐릭터에만 적용된다.
	// atkrange는 데이터 시트상의 공격거리다.
	public Xfloat hitRange = 0.0f;
	
	public float aniFadeTime = 0.2f;
	
	public int atkEffectType = 1;
	

	public IFloat fowardDirectionValue = 0.0f;

	public bool isPlayerSide = false; // 적군이냐 아군이냐
	public bool isPlayer = false; // monster냐 player냐 
	public bool isPlayersPlayer = false; // 플레이어가 움직이는 플레이어 캐릭터인가.
	public int playerTagIndex = -1;

	public Xbool nowPlayingSkillAni = false; // 공격 애니메이션 중인가?

	public Xbool isDefaultAttacking = false; // // 기본공격을 하고 있는지?

	public enum MoveType { NORMAL, SKILL, ATTACH_SKILL };
	public enum MoveState { Stop, Forward, Backward };

	public MoveType moveType = MoveType.NORMAL;
//	public MoveType moveType
//	{
//		set
//		{
//			_moveType = value;
//			if(isPlayerSide == false)
//			{
//				Debug.Log("MoveType: " + value);
//			}
//		}
//		get
//		{
//			return _moveType;
//		}
//	}


	public MoveState prevMoveState = MoveState.Stop;
	public MoveState moveState = MoveState.Stop;
//	public MoveState moveState
//	{
//		set
//		{
//			_moveState = value;
//			if(isPlayer) Log.log("MoveState: " + value);
//		}
//		get
//		{
//			return _moveState;
//		}
//	}

	public string bulletPatternId;
	public string nowBulletPatternId;
	
	protected IVector3 _v;
	protected IVector3 _v2;
	
	protected Xbool _isEnabled = false;
	
	protected IVector3 _boundExtens;
	protected IVector3 _boundCenter;

	protected IFloat _currentDamageTime = -1.0f;
	public bool isDamageFrame = false;	
	
//	protected Xfloat _invincibleTime = 0.0f;
	
	public EffectData deleteMotionEffect;
	
	public CharacterTooltip toolTip = null;
	
	public CharacterMinimapPointer miniMapPointer;
	
	public CharacterHpBar hpBar;	
	
	public Xfloat lineRight = 0.0f;
	public Xfloat lineLeft = 0.0f;

	public Monster target = null;
	public Monster skillTarget = null;


	// 얘들이 동작하면 해당 애니메이션을 무시한다.
	public bool ignoreIdleAni = false;

	public void setSkillTarget(Monster value)
	{
		if(skillTarget != null)
		{
			if(value != null && value == skillTarget) return;
			if(skillTarget.skillAttackers.Contains(this)) skillTarget.skillAttackers.Remove(this);
		}
		skillTarget = value;
		if(skillTarget != null) skillTarget.skillAttackers.Add(this);
	}

	public void setTarget(Monster value)
	{
		if(target != null)
		{
			if(value != null && value == target) return;
			if(target.attackers.Contains(this)) target.attackers.Remove(this);
		}
		target = value;			
		if(target != null)
		{
			target.attackers.Add(this);
			//effectTargetPosition = target.cTransform.position; 세라엘 밖에 안쓰는데 얘는 사치임...
		}
	}


	public int targetAngle = 0;
	
	public delegate bool SkillTargetMethod(Monster Attacker);
	public SkillTargetMethod skillTargetChecker; // 실제 스킬을 현재 쓸 수 있느냐를 판단하는 근거가 된다.
	public SkillTargetMethod skillMove; // 스킬을 쓸 수 없는 위치에 있을 때 히어로 몬스터를 이동시킨다.
	
	
	public List<Monster> attackers = new List<Monster>();
	public List<Monster> skillAttackers = new List<Monster>();

	// 나로 인해 데미지를 받은 녀석들.
	// 내가 유닛일때만 적용한다.
	public List<Monster> receiveDamageMonstersByMe = new List<Monster>();

	public int prevUniqueId = -999;

	public bool isSameMonster()
	{
		return stat.uniqueId == prevUniqueId && _isEnabled;
	}


	// 나를 때린 놈의 id.
	public int attackerUniqueId = -1;
	public Monster attacker = null;
	
	
	public IVector3 attackPosition = IVector3.zero;

	// 중심점에서 피격 당할 수 있는 반경.
	public IFloat damageRange = 0.0f;
	
	public IFloat distanceBetweenAttacker = 0;
	public IFloat distanceFromHitPoint = 0;
	
	public List<ChainLightning> chainLighting = new List<ChainLightning>();


//========================================================================================================	
	
	[HideInInspector] public int smrNumber = 0;
	[HideInInspector] public int mrNumber = 0;
	
	public PartsLoader partsLoader;
	
	public int[] modelRendererIndex = new int[]{-1,-1,-1};

	void Awake()
	{
		if(partsLoader == null) partsLoader = gameObject.AddComponent<PartsLoader>();
		ani = animation;
	}

	public void onCompleteLoadModel()
	{
		isReady.Set( true );
	}

	public TextureSpriteAnimation faceAniEye = null;
	public TextureSpriteAnimation faceAniMouth = null;

	public TextureSpriteAnimation faceAniEye2 = null;
	public TextureSpriteAnimation faceAniMouth2 = null;

	public void playFaceAni(string id)
	{
		if(GameManager.info.faceAniData[resourceId].ContainsKey(id) == false) return;

		FaceAnimationInfo fa = GameManager.info.faceAniData[resourceId][id];
		
		if(fa.type == "EYE")
		{
			if(faceAniEye != null) faceAniEye.setData( GameManager.info.faceAniData[resourceId][id] );
			if(faceAniEye2 != null) faceAniEye2.setData( GameManager.info.faceAniData[resourceId][id] );
		}
		else if(fa.type == "MOUTH")
		{
			if(faceAniMouth != null) faceAniMouth.setData( GameManager.info.faceAniData[resourceId][id] );
			if(faceAniMouth2 != null) faceAniMouth2.setData( GameManager.info.faceAniData[resourceId][id] );
		}
	}





	public void rendererInit(bool isOriginalResource = false)
	{
		smrs = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
		smrNumber = smrs.Length;
		
		mrs = transform.GetComponentsInChildren<MeshRenderer>();
		mrNumber = mrs.Length;		
		
		//if(animation.GetClip(DAMAGE) != null) animation[DAMAGE].layer = 10;
		if(normalCollider == null) normalCollider = gameObject.GetComponent<BoxCollider>();
		normalCollider.enabled = false;

		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].castShadows = false;
			mrs[i].receiveShadows = false;

			if(isOriginalResource == false)
			{
				if(mrs[i].material != null)
				{
				
				}
			}
		}

		modelRendererIndex[0] = -1;
		modelRendererIndex[1] = -1;
		modelRendererIndex[2] = -1;

		for(i = 0; i < smrNumber; ++i)
		{
			smrs[i].castShadows = false;
			smrs[i].receiveShadows = false;
			smrs[i].updateWhenOffscreen = false;

			if(smrs[i].name.Contains(ModelData.OUTLINE_RARE_NAME)) modelRendererIndex[ModelData.WITH_RARELINE] = i;
			else if(smrs[i].name.Contains(ModelData.OUTLINE_NAME)) modelRendererIndex[ModelData.WITH_OUTLINE] = i;
			else modelRendererIndex[ModelData.WITHOUT_OUTLINE] = i;
			
			smrs[i].updateWhenOffscreen = false;

			if(isOriginalResource == false)
			{
				if(smrs[i].material != null)
				{

				}
			}
		}

		if(PerformanceManager.isLowPc) setBoneQuality();

		if(normalCollider == null) normalCollider = gameObject.GetComponent<BoxCollider>();
		normalCollider.enabled = false;

		ani = animation;

		isReady.Set( true );

		if(isOriginalResource == false)
		{
			initAnimations();
			changeShader(false,true);
		}
	}





	public void removeRareLine()
	{
		for(int i = 0; i < smrNumber; ++i)
		{
			smrs[i].gameObject.SetActive( !smrs[i].name.Contains(ModelData.OUTLINE_RARE_NAME) );

			if(monsterData != null && monsterData.deleteParts != null)
			{
				for(int j = 0; j < monsterData.deleteParts.Length; ++j)
				{
					if(smrs[i].gameObject.name.Contains(monsterData.deleteParts[j]))
					{
						smrs[i].gameObject.SetActive(false);
					}
				}
			}
		}
	}



	void Start()
	{
		partsLoader.cha = this;		
	}

	public void removeTarget(bool removeSkillTarget = true)
	{
		if(target != null)
		{
			if(target.attackers.Contains(this)) target.attackers.Remove(this);		
		}
		target = null;
		
		if(removeSkillTarget)
		{
			setSkillTarget(null);
			
			skillMove = null;
			skillTargetChecker = null;
		}
	}	
	
	public void removeAttacker()
	{
		int attackerLength = attackers.Count;
		
		if(attackerLength > 0) // 나를 때리던 놈들이 있으면 걔들한테서 내가 죽었다고 그만 때리라고 알려준다.
		{
			for(i = 0; i < attackerLength; ++i)
			{
				if(attackers[i] != null)
				{
					attackers[i].target = null;
					attackers[i].targetAngle = -1;
				}
			}
			
			attackers.Clear();
		}	


		attackerLength = skillAttackers.Count;
		
		if(attackerLength > 0) // 나를 때리던 놈들이 있으면 걔들한테서 내가 죽었다고 그만 때리라고 알려준다.
		{
			for(i = 0; i < attackerLength; ++i)
			{
				if(skillAttackers[i] != null)
				{
					skillAttackers[i].skillTarget = null;
				}
			}
			
			skillAttackers.Clear();
		}	
	}

	
//	public float maxHp
//	{
//		set
//		{
//			maxHp.Set(value);
//		}
//		get
//		{
//			return maxHp.Get();
//		}
//	}
	
	protected Xfloat _hp = 100;
	public Xfloat maxHp = 100;
	public Xfloat hpPer = 0.0f;


	public virtual Xfloat hp
	{
		get
		{
			return _hp;
		}
		set
		{

			#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				if(value < 1) value = 1;
			}
			#endif

			if(value > maxHp) _hp.Set(maxHp);
			else if(value <= 0)
			{
				_hp.Set(0);
//				isInTargetZone = false; 
			}
			else
			{
				_hp.Set(value);
			}


			hpPer.Set( _hp/maxHp );
			
			setHpBar();
		}
	}
	
	
	public float hpWithoutDisplay
	{
		get
		{
			return _hp;
		}
		set
		{
			_hp = value;
			if(_hp > maxHp)
			{
				_hp.Set(maxHp.Get());
			}
			else if(_hp <= 0)
			{
				_hp.Set(0);
//				isInTargetZone = false; 
			}
		}
	}	
	
	
	public Xfloat maxMp = 100;
	protected Xfloat _mp = 0;
	public virtual Xfloat mp
	{
		get
		{
			return _mp;
		}
		set
		{
			_mp.Set( value );
			if(_mp > maxMp) _mp.Set( maxMp );
			else if(_mp < 0) _mp.Set( 0 );
		}
	}

	public Xfloat maxSp = 100;
	protected Xfloat _sp = 0;
	public virtual Xfloat sp
	{
		get
		{
			return _sp;
		}
		set
		{
			_sp.Set( value );
			if(_sp > maxSp) _sp.Set( maxSp );
			else if(_sp < 0) _sp = 0;
		}
	}

	public int targetUniqueId = -1;
	public IVector3 targetPosition = new IVector3();
	public IFloat targetHeight = 0.0f;
	
	public IFloat bodyYCenter = 0.0f;
	
	[HideInInspector]
	public BoxCollider normalCollider;
	
	public HitObject hitObject = new HitObject();

	[HideInInspector]
	public bool isDeleteObject = false;
	
	[HideInInspector]
	public bool isMonster = false;

	public Xbool invincible = false;
	
	public ParticleSystem particle = null;

	public ParticleEffect particleEffect = null;

	public ParticleEffect skillAttachedEffect = null;


	public CharacterShadow shadow;

	public MonsterCategory.Category category;
	public MonsterData monsterData;
	public UnitData unitData;
	public NPCData npcData;
	public StageMonsterData stageMonsterData;
	public GamePlayerData playerData = null;
	public bool isHero = false;
	
	public GameObject container;

	public Transform cTransform;
	public IVector3 cTransformPosition = new IVector3();
//	public Vector3 lineLeftPos = new Vector3();
//	public Vector3 lineRightPos = new Vector3();

	public Animation ani;

	public SkinnedMeshRenderer[] smrs;
	public MeshRenderer[] mrs;

	protected float _energyBarShowTime = 3.0f;

	public Vector3 rotation
	{
		set
		{
			_q = tf.rotation;
			_q.eulerAngles = value;
			tf.rotation = _q;
		}
	}
	
	public Transform tf;

//	// 고행상도. 컷씬과 로비에서 쓴다.
//	public Transform high;

//	// 저해상도. 인게임에서 쓴다.
//	public Transform low;

	public Transform combineRoot;

	public List<Bullet> attachedBullet = new List<Bullet>();
	
	public bool isVisible = true;
	public void setVisible(bool isShow, bool includePet = true)
	{
		isVisible = isShow;

		if(shadow != null)
		{
			shadow.renderer.enabled =  isShow ;
		}

		for(i = 0; i < smrNumber; ++i)
		{
			smrs[i].enabled = isShow;
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].enabled = isShow;
		}	


		if(pet != null && includePet)
		{
			pet.setVisible(isShow);
		}
	}



	public void setPartsVisible(string partsName, bool isShow)
	{
		for(i = 0; i < smrNumber; ++i)
		{
			if(smrs[i].name.Contains(partsName))
			{
				smrs[i].enabled = isShow;
			}
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			if(mrs[i].name.Contains(partsName))
			{
				mrs[i].enabled = isShow;
			}
		}	
	}



	public bool isVisibleForSkillCam = false;
	private static Color _skillCamColor = new Color();
	public void setVisibleForSkillCam(bool isShow, float alpha = 0.15f)
	{
		if(isShow)
		{
			if(alpha >= 0.9f) // 1.0으로 해도 되지만... 혹시 몰라서.. 어차피 알파값은 0.9를 쓸 일이 없다.
			{
				isVisibleForSkillCam = false;
				changeShader(false,true);
			}
			else
			{
				isVisibleForSkillCam = true;
				setShader(ResourceManager.instance.skillTransparent);
				_skillCamColor.r = 1; _skillCamColor.g = 1; _skillCamColor.b = 1;
				_skillCamColor.a = alpha;
				setColorWithoutRareLine(_skillCamColor);
			}
		}
		else
		{
			isVisibleForSkillCam = true;
			setShader(ResourceManager.instance.skillTransparent);
			_skillCamColor.r = 1; _skillCamColor.g = 1; _skillCamColor.b = 1;
			_skillCamColor.a = 0.0f;
			setColorWithoutRareLine(_skillCamColor);
		}
	}


	public AttackData attackData = null;

	protected void resetDefaultVals()
	{
		attackData = null;

		stat.reset();

		maxHp = 0.0f;

		waitEnemy = false;
		waitLine = 0.0f;
		hitRange = 0.0f;

		nowPlayingSkillAni.Set( false );
		isDefaultAttacking.Set( false );

		prevPlayingAniName = string.Empty;

		ignoreIdleAni = false;

		_damageMotionStep = 0;
	}


	public void baseInit()
	{
		specialType = SpecialType.NONE;

		stat.uniqueId = (++uniqueIndex);

		checkWalkTime = 0.0f;

		needToSetDead = false;
		isPlayersPlayer = false;

		if(isReady == false) return;
		
		normalCollider.enabled = false;

		isVisibleForSkillCam = false;

		changeShader(false, true);

		setColor(_normalColor);

		if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);

//		setBoneQuality();

		if(pet != null) tf = pet.transform;
		else tf = transform;

		//skipHitCheck = false;

		_currentDamageTime = -1.0f;
		isDamageFrame = false;
		
		_v = cTransform.localScale;
		
		_v.x = 1.0f;
		_v.y = 1.0f;
		_v.z = 1.0f;

//		if(VersionData.codePatchVer >= 1)
		{
			targetPosition = Vector3.zero;
		}

		if(monsterData != null)
		{
			_v.x = monsterData.scale;
			_v.y = monsterData.scale;
			_v.z = monsterData.scale;
		}
		
		cTransform.localScale = _v;	

		invincible = false;
//		_invincibleTime = 0.0f;
		
		_hitBulletTime.Clear();

		if(monsterData != null && monsterData.effectData != null)
		{
			foreach(AttachedEffectData aed in monsterData.effectData)
			{
				AttachedEffect ae = GameManager.me.effectManager.getAttachedEffect();
				_effects.Add(ae);
				_v = cTransformPosition;

				if(string.IsNullOrEmpty( aed.parentName ) == false && effectParents != null && effectParents.ContainsKey(aed.parentName))
				{
					ae.init(aed, effectParents[aed.parentName], _v);
				}
				else
				{
					ae.init(aed, transform, _v);
				}

				ae.isEnabled = true;
			}
		}

		_energyBarShowTime = -99.0f;
		
		_recoveryDelay.Set( 0.0f );
		_recoveryDelaySp.Set( 0.0f );
		_recoveryDelayMp.Set( 0.0f );
		_hpRecoveryDelay.Set( 0.0f );

		lineIndex = 0;

		playerTagIndex = -1;

		distanceBetweenAttacker = 0;
		distanceFromHitPoint = 0;
//		isInTargetZone = false;
		targetHeight = 0.0f;
		targetUniqueId = -1;
		bodyYCenter = 0.0f;
		isDeleteObject = false;


		_damage = 0;

		resetAnimations();
		
		if(toolTip != null) toolTip.visible = false;

		isFreeze.Set( false );

		_state = NORMAL;
		_state2 = NORMAL;
		
		hasShootingPos = false;

		attackAvoidPercent = 0;
		
		setHitObject();
		
		changeColorSet();

		setTarget(null);
		setSkillTarget(null);

		attackers.Clear();
		skillAttackers.Clear();

		targetAngle  = 0;

		if(isPlayerSide)
		{
			lineLeft = -99999.0f;
			lineRight = -99999.0f;
		}
		else
		{
			lineRight = 99999.0f;
			lineLeft = 99999.0f;	
		}

		isDeleteObject = false;

		if(monsterData != null && monsterData.defaultTexture != null)
		{
			for(int i = 0; i < smrNumber; ++i)
			{
				if(smrs[i].name.Contains("_line") == false)
				{
					CharacterUtil.setTexture(smrs[i],monsterData.defaultTexture);
				}
			}
		}
	}	

	protected void setUpdateWhenOffscreen(bool isUse)
	{
		if(isUse)
		{
			Vector3 smv = new Vector3(1200,5000,1200);

			for(int i = 0; i < smrNumber; ++i)
			{
				smrs[i].localBounds = new Bounds(Vector3.zero,smv);
				smrs[i].sharedMesh.bounds = new Bounds(Vector3.zero,smv);
				//smrs[i].updateWhenOffscreen = isUse;
			}

			ani.localBounds = new Bounds(Vector3.zero,smv);
			ani.cullingType = AnimationCullingType.BasedOnUserBounds;
		}
		else
		{
			for(int i = 0; i < smrNumber; ++i)
			{
				smrs[i].updateWhenOffscreen = isUse;
			}
		}
	}


	public float shadowSize
	{
		get
		{
			ModelData md = GameManager.info.modelData[resourceId];
			return md.shadowSize * monsterData.scale;
		}
	}

	public void initShadowAndEffectSize()
	{
		ModelData md = GameManager.info.modelData[resourceId];



		summonEffectSize = md.summonEffectSize * monsterData.scale;

		if(shadowSize <= 0.0f || monsterData.visible == false)
		{
			shadow.gameObject.SetActive( false );
		}
		else if(pet == null)
		{
//			if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY)
//			{
//				shadow.setData(cTransform, shadowSize);
//			}
//			else
			{
				/*
				switch(md.lobbySize)
				{
				case ModelData.LobbySize.Big:
					shadow.setData(cTransform, 2.8f);
					break;
				case ModelData.LobbySize.Medium:
					shadow.setData(cTransform, 1.8f);
					break;
				case ModelData.LobbySize.Small:
					shadow.setData(cTransform, 0.8f);
					break;
				default:
					shadow.setData(cTransform, shadowSize);
					break;
				}
				*/

				shadow.setData(cTransform, shadowSize);
			}
		}

		if(md.effectSize > 0)
		{
			effectSize = md.effectSize * monsterData.scale;
		}
		else
		{
			effectSize = md.sx / 85.0f * monsterData.scale;
		}

		if(effectSize > 2.2f) effectSize = 2.2f;
		else if(effectSize < 0.5f) effectSize = 0.5f;
	}

	private void initAnimations()
	{
		foreach(AnimationState st in ani)
		{
			animationStates[st.name] = st;
		}
		
		if(animationStates.ContainsKey(Monster.WALK) == false && animationStates.ContainsKey(Monster.NORMAL))
		{
			ani.AddClip(animationStates[Monster.NORMAL].clip, Monster.WALK);
		}
		
		if(animationStates.ContainsKey(Monster.NORMAL) == false && animationStates.ContainsKey(Monster.WALK))
		{
			ani.AddClip(animationStates[Monster.WALK].clip, Monster.NORMAL);
		}

		if(animationStates.ContainsKey(Monster.BWALK) == false && animationStates.ContainsKey(Monster.WALK))
		{
			ani.AddClip(animationStates[Monster.WALK].clip, Monster.BWALK);
		}

	}

	protected void resetAnimations()
	{
		foreach(AnimationState st in ani)
		{
			animationStates[st.name] = st;

			if(st.clip.name.Contains(SHOOT_HEADER))
			{
				st.clip.wrapMode = WrapMode.Once;
				st.speed = stat.originalAtkSpeedRate;

				if(pet != null)
				{
					if(pet.ani.GetClip(st.clip.name) != null)
					{
						pet.ani[st.clip.name].wrapMode = WrapMode.Once;	
						pet.ani[st.clip.name].speed = stat.originalAtkSpeedRate;
					}
				}
			}
			else
			{
				st.speed = 1.0f;
			}
		}

		initAni(WALK, WrapMode.Loop, stat.originalMoveSpeedRate);
		initAni(DEAD, WrapMode.ClampForever);
		initAni(NORMAL, WrapMode.Loop);

		initAni(BWALK, WrapMode.Loop);

		initAni(SKILL_START, WrapMode.ClampForever);
		initAni(SKILL_FORWARD, WrapMode.ClampForever);
		initAni(SKILL_LOOP, WrapMode.Loop);
		initAni(SKILL_NORMAL, WrapMode.ClampForever);
		initAni(SKILL_END, WrapMode.ClampForever);
	}
	
	
	public void changeAnimationSpeed(float aniSpeed = 1.0f)
	{
		foreach(AnimationState st in ani)
		{
			st.speed = aniSpeed;
		}		
		
		if(pet != null)
		{
			foreach(AnimationState st in pet.ani)
			{
				st.speed = aniSpeed;
			}				
		}
	}
	
	
	public void changeAnimationSpeed(string aniId, float aniSpeed = 1.0f)
	{
		if(float.IsNaN(aniSpeed)) aniSpeed = 1.0f;
		foreach(AnimationState st in ani)
		{
			if(st.clip.name == aniId)
			{
				st.speed = aniSpeed;
				break;
			}
		}		
		
		if(pet != null)
		{
			foreach(AnimationState st in pet.ani)
			{
				if(st.clip.name == aniId)
				{
					st.speed = aniSpeed;
					break;
				}
			}				
		}
	}	

	public void changeWalkAnimationSpeed(float aniSpeed = 1.0f)
	{
		aniSpeed = aniSpeed * stat.originalMoveSpeedRate;
		if(float.IsNaN(aniSpeed)) aniSpeed = 1.0f;
		foreach(AnimationState st in ani)
		{
			if(st.clip.name == Monster.WALK)
			{
				st.speed = aniSpeed;
				break;
			}
		}		
		
		if(pet != null)
		{
			foreach(AnimationState st in pet.ani)
			{
				if(st.clip.name == Monster.WALK)
				{
					st.speed = aniSpeed;
					break;
				}
			}				
		}
	}	





	public void changeAttackAnimationSpeed(float aniSpeed = 1.0f)
	{
		aniSpeed = aniSpeed * stat.originalAtkSpeedRate;
		if(float.IsNaN(aniSpeed)) aniSpeed = 0.1f;
		if(float.IsInfinity(aniSpeed)) aniSpeed = 1.0f;

		foreach(AnimationState st in ani)
		{
			if(st.clip.name.Contains(SHOOT_HEADER))
			{
				st.speed = aniSpeed;
			}
		}		
		
		if(pet != null)
		{
			foreach(AnimationState st in pet.ani)
			{
				st.speed = aniSpeed;
			}				
		}


		
		for(int i = _aniDelayMethods.Count - 1; i >= 0; --i)
		{
			if(_aniDelayMethods[i].type == MonsterDelayMethod.TYPE.onCompleteAnimation2 && _aniDelayMethods[i].currentState != Monster.DEAD)
			{
				_aniDelayMethods[i].changeWaitTime(aniSpeed);
				if(action != null && action.delay > 0)
				{
					action.delay = action.delay / aniSpeed ;
				}
				return;
			}
		}
	}	
	
	
		
	public void initAni(string clipId, WrapMode wrapMode, float speed = 1.0f)
	{
		if(animationStates.ContainsKey(clipId))
		{
			animationStates[clipId].clip.wrapMode = wrapMode;
			animationStates[clipId].speed = speed;

			if(ani[clipId] != null)
			{
				ani[clipId].wrapMode = wrapMode;
				ani[clipId].speed = speed;
			}
		}

		if(pet != null)
		{
			if(pet.ani.GetClip(clipId) != null)
			{
				pet.ani[clipId].wrapMode = wrapMode;			
				pet.ani[clipId].speed = speed;
			}
		}
	}
		
	
	
	public List<AttachedEffect> getAttachedEffects(string id)
	{
		List<AttachedEffect> effs = new List<AttachedEffect>();
		
		for(i = _effects.Count - 1; i >= 0; --i)
		{
			if(_effects[i].data.id == id)
			{
				effs.Add(_effects[i]);
			}
		}		
		
		return effs;
	}
	
	
	public AttachedEffect getAttachedEffect(string id)
	{
		for(i = _effects.Count - 1; i >= 0; --i)
		{
			if(_effects[i].data.id == id)
			{
				return _effects[i];
			}
		}
		
		return null;
	}
	
	
	public void addAttachedParticleEffect(string id)
	{
		AttachedEffect ae = GameManager.me.effectManager.getAttachedEffect();
		_effects.Add(ae);
		_v = cTransformPosition;
		ae.addParticle(id, tf, _v);
		ae.isEnabled = true;
	}
	
	public void addAttachedParticleEffectByCharacterSize(string id)
	{
		AttachedEffect ae = GameManager.me.effectManager.getAttachedEffect();
		_effects.Add(ae);
		_v = cTransformPosition;
		ae.addParticle(id, tf, _v, true, this);
		ae.isEnabled = true;		
	}
	
	
	protected Quaternion _q;

	
	public HitObject getHitObject()
	{
		_v = cTransformPosition;
		hitObject.setPosition(_v);
		
		return hitObject;
	}
	
	public HitObject getHitObject(Vector3 pos)
	{
		hitObject.setPosition(pos);
		
		return hitObject;
	}	
	
	
	public HitObject getHitObjectWithOffset(Vector3 offset)
	{
		_v = cTransformPosition;
		_v += offset;
		hitObject.setPosition(_v);
		
		return hitObject;
	}
	
	public HitObject getHitObjectWithOffset(Vector3 pos, Vector3 offset)
	{
		pos += offset;
		hitObject.setPosition(pos);
		
		return hitObject;
	}	

	
	public bool hitWithBullet(HitObject hitObject, Bullet bullet)
	{
		return hitObject.intersects(getHitObject());
	}

	
	
	public bool isEnabled
	{
		set
		{
			_isEnabled.Set( value );	

			if(pet != null) pet.container.SetActive(value);
			container.SetActive(value);

			if(value == false)
			{
				stat.uniqueId = -1;
				prevUniqueId = -1;
				changeShader(false);

				setColor(_normalColor);

				if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);

				clearEffect();
				clearAnimationMethod(true);
				monsterDeadCallback = null;

				if(chainLighting != null)
				{
					while(chainLighting.Count > 0)
					{
						chainLighting[0].removeCharacter(this);
						chainLighting.RemoveAt(0);
					}
				}

				if(shadow != null) shadow.gameObject.SetActive( false );
			}
		}
		get
		{
			return _isEnabled;	
		}
	}
		
	
	public void removeTooltip()
	{
		if(toolTip != null) GameManager.me.characterManager.setTooltip( toolTip );		
		toolTip = null;
	}

	
	public float getBulletRecordTime(Bullet b)
	{
		return _hitBulletTime[b.uniqueNo];
	}
	
	public bool canCheckThisBullet(Bullet b, bool recordTime = true)
	{
		if(_hitBulletTime.ContainsKey(b.uniqueNo))
		{
			if(GameManager.me.stageManager.playTime + (b.hitTimeOffset + 1.0f) - _hitBulletTime[b.uniqueNo] <= b.hitTimeOffset.Get())// || b.damageCheckTypeNo == 5)
			{
				return false;
			}
			
			if(recordTime) _hitBulletTime[b.uniqueNo] = GameManager.me.stageManager.playTime + (b.hitTimeOffset + 1.0f);
		}
		else
		{
			if(recordTime) _hitBulletTime.Add(b.uniqueNo, GameManager.me.stageManager.playTime + (b.hitTimeOffset + 1.0f));
		}
		
		return true;
	}
	

	public void resetBulletRecordTime(Bullet b, float resetTime = 0.0f)
	{
		_hitBulletTime[b.uniqueNo] = resetTime;
	}
	
	
	private Dictionary<int, Xfloat> _hitBulletTime = new Dictionary<int, Xfloat>();


	protected Xint _damage;
	public bool damage(Bullet b, Monster cha, float damagePer = 1.0f, float minimumDamagePer = 1.0f, bool useEffect = true, string effectId = "E_HIT_STRIKE", string soundId = null, float tempDiscountDamageValue = 1.0f)
	{
		//if(_isEnabled == false || _invincibleTime > 0.0f) return false;
		if(_isEnabled == false) return false;
		
		// 해당 총알과 맞은지 얼마나 됐나?
		if(canCheckThisBullet(b) == false)
		{
			return false;
		}
		
		if(b.hp > 0) --b.hp;

		b.bulletData.hitActionStart(cha.cTransformPosition, cha, b);
		
		if(b.attachedToCharacter || b.attachedDamageEffectToCharacter)
		{
			// 불의 숨결 같은 애들은 적의 몸에 타격 이펙트를 날리고.
//			Log.log(".attachedToCharacter || b.attachedDamageEffectToCharacter");
			return damage(b.attackerInfo.stat.monsterType, b.attackerInfo.shooter, b.attackerInfo.uniqueId, b.attackerInfo.isSkillType, b.totalDamageNum, b.bTransform, b.attackerInfo.stat.atkPhysic, b.attackerInfo.stat.atkMagic, damagePer, minimumDamagePer, useEffect, effectId, soundId, tempDiscountDamageValue);			
		}
		else
		{
			// 일반 총알은 총알과 적의 위치를 판단해서 이펙트를 날린다.
			_v = b.bTransformPosition;
			_v.z += (cha.cTransformPosition.z - _v.z) * 0.5f;
			_v.y = b.bTransformPosition.y;

			if(_v.y <= 0 && b.damageCheckTypeNo == 9)
			{
				_v.y = hitObject.height * 0.5f;
			}

			if(b.bTransformPosition.x < cha.cTransformPosition.x) // 총알이 좌측에서 터지면.
			{
				if( Xfloat.lessThan( _v.x , cha.lineLeft ) ) _v.x = cha.lineLeft;
			}
			else // 우측에서 터지면.
			{
				if( Xfloat.greaterThan( _v.x , cha.lineRight ) ) _v.x = cha.lineRight;
			}

			playDamageSoundAndEffect(b.attackerInfo.uniqueId,true,soundId,effectId,false,true,_v.x,_v.y,_v.z, true);

//			Log.log("damage");
			return damage(b.attackerInfo.stat.monsterType, b.attackerInfo.shooter, b.attackerInfo.uniqueId, b.attackerInfo.isSkillType, b.totalDamageNum, b.bTransform, b.attackerInfo.stat.atkPhysic, b.attackerInfo.stat.atkMagic, damagePer, minimumDamagePer, false, "E_HIT_STRIKE", null, tempDiscountDamageValue);
		}
	}
	
	

	
	
	public bool canAvoid()//bool useMissEffect = true)
	{
		if(attackAvoidPercent > 0)
		{
			if(attackAvoidPercent > GameManager.inGameRandom.Range(0,100))//;GameManager.getRandomNum())
			{
				//if(useMissEffect) showMissEffect();
				return true;
			}
		}
		
		return false;
	}
	
	

	
	
	protected void onCompleteDamageEffectTween()
	{
		isFreeze.Set( false );
	}
	

	public Xbool isFreeze = false;

	protected Color _normalColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	protected Color _damageColor = new Color(238.0f/255.0f, 91.0f/255.0f, 25.0f/255.0f);//new Color(0.8f, 0.5f, 0.5f, 1.0f);
	
	public enum ColorSet
	{
		NORMAL, BURN, DECAY
	}
	
	public void changeColorSet(ColorSet colorType = ColorSet.NORMAL)
	{
		switch(colorType)
		{
		case  ColorSet.NORMAL:

			if(GameManager.info.modelData.ContainsKey(resourceId) && GameManager.info.modelData[resourceId].useDefaultColor)
			{
				_normalColor = GameManager.info.modelData[resourceId].defaultColor;
			}
			else
			{
				_normalColor.r = 1.0f;
				_normalColor.g = 1.0f;
				_normalColor.b = 1.0f;

			}
//			_damageColor.r = 0.8f;
//			_damageColor.g = 0.5f;
//			_damageColor.b = 0.5f;
			
			break;
		case  ColorSet.BURN:
			
			_normalColor.r = 0.25f;
			_normalColor.g = 0.25f;
			_normalColor.b = 0.25f;
			
			//_damageColor.r = _normalColor.r * 0.8f;
			//_damageColor.g = _normalColor.r * 0.5f;
			//_damageColor.b = _normalColor.r * 0.5f;			
			
			break;
		case  ColorSet.DECAY:
			
			_normalColor.r = 0.25f;
			_normalColor.g = 0.25f;
			_normalColor.b = 0.25f;
			
			//_damageColor.r = _normalColor.r * 0.8f;
			//_damageColor.g = _normalColor.r * 0.5f;
			//_damageColor.b = _normalColor.r * 0.5f;			
			
			break;			
		}

		changeShader(false);

		setColor(_normalColor);

		if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);
	}


	
	// --------------------------------
	public Stack<ParticleResizeContainer> bodyEffects;


	private List<AttachedEffect> _effects = new List<AttachedEffect>();
	public CharacterEffect characterEffect = null;
	
	protected string _state;
	protected string _state2;
	
	public void setState(string value)
	{
		_state = value;
	}



	protected void setShootPosition(string aniId, int nowIndex = 0)
	{
		if(GameManager.info.aniData[resourceId][aniId].shootingPointLength > 0)
		{
			if(GameManager.info.aniData[resourceId][aniId].shootingHandLength > nowIndex)
			{
				shootingHand = shootingTransforms[aniId][nowIndex];
			}
			else if(GameManager.info.aniData[resourceId][aniId].shootingHandLength > 0)
			{
				shootingHand = shootingTransforms[aniId][0];
			}
			else
			{
				shootingHand = null;
			}

			if(GameManager.info.aniData[resourceId][aniId].shootingPointLength > nowIndex)
			{
				shootingPos = shootingPositions[aniId][nowIndex];
				hasShootingPos = true;
			}
			else
			{
				shootingPos = shootingPositions[aniId][0];
				hasShootingPos = true;
			}
		}
		else
		{
			shootingHand = null;
			shootingPos = null;
			hasShootingPos = false;
		}
	}



	protected Transform getShootHand(string aniId, int nowIndex = 0)
	{
		if(GameManager.info.aniData[resourceId][aniId].shootingPointLength > 0)
		{
			if(GameManager.info.aniData[resourceId][aniId].shootingHandLength > nowIndex)
			{
				return shootingTransforms[aniId][nowIndex];
			}
			else if(GameManager.info.aniData[resourceId][aniId].shootingHandLength > 0)
			{
				return shootingTransforms[aniId][0];
			}
		}

		return null;
	}




	public void onAttackAnimation(string currentState, int nowIndex, string aniDataId, int targetX, int targetY, int targetZ, int targetH)
	{
		//if(_state == currentState && _isEnabled)
		if(_isEnabled)
		{
			setShootPosition(aniDataId, nowIndex);
			onAttack(GameManager.info.aniData[resourceId][aniDataId].delayLength, targetX, targetY, targetZ, targetH);
		}
	}
	
	
	public void onCompleteAnimation(Callback callback = null)
	{
		if(callback != null) callback();
	}
	
	
	public void onCompleteAnimation(string currentState)
	{

		if(_state == DEAD || currentState == DEAD)
		{
			onCompleteDeadAni();
		}
		else if(currentState.Contains(SHOOT_HEADER))
		{
			onCompleteAttackAni();
		}
	}

	public void onCompleteSkillAnimation(string currentState)
	{
		//if(_state == currentState)
		{
			onCompleteAttackAni();
		}

	}	





/*
	protected IEnumerator onAttackAnimation(float waitTime, string currentState, int nowIndex, string aniDataId)
	{
		yield return new WaitForSeconds(waitTime);
		if(_state == currentState && hp > 0)
		{
			if(GameManager.info.aniData[resourceId][aniDataId].shootingPointLength > 0)
			{
				if(GameManager.info.aniData[resourceId][aniDataId].shootingPointLength > nowIndex)
				{
					shootingHand = shootingPositions[aniDataId][nowIndex];
				}
				else shootingHand = shootingPositions[aniDataId][0];
			}
			else
			{
				shootingHand = null;
			}
			
			onAttack(GameManager.info.aniData[resourceId][aniDataId].delayLength);
		}
	}

	
	public IEnumerator onCompleteAnimation(float waitTime, Callback callback = null)
	{
		yield return new WaitForSeconds(waitTime);
		
		if(callback != null)
		{
			callback();
		}
	}
	
	
	public IEnumerator onCompleteAnimation(float waitTime, string currentState)
	{
		yield return new WaitForSeconds(waitTime);

		if(_state == currentState)
		{
			if(_state.Contains(SHOOT_HEADER))
			{
				onCompleteAttackAni();
			}
		}

		if(_state == DEAD || _state == currentState)
		{
			onCompleteDeadAni();
		}
	}
	
	
	
	protected IEnumerator onCompleteSkillAnimation(float waitTime, string currentState)
	{
		yield return new WaitForSeconds(waitTime);
		
		if(_state == currentState)
		{
			onCompleteAttackAni();
		}
	}	
*/

	public void onCompleteDeadDeleteMotion()
	{
		_isEnabled.Set( false );
		isDeleteObject = true;		
	}


	
	
	
//===  VIRTUAL METHODS ====================================================================================	
	public float damageMotionValue = 10.0f;
	public float damageMotionStep2Value = 10.0f;
	protected const float _damageMotionValueRatio = 0.1f;
	protected const float _damageMotionStep2ValueRatio = 0.6f;

	public virtual void setHitObject()
	{
		_v = cTransform.localScale;

		float t = 0.0f;

		_boundCenter = normalCollider.center;
		_boundCenter.x *= _v.z;//_v.x;
		_boundCenter.y *= _v.y;
		_boundCenter.z *= _v.x;//_v.z;

		t = _boundCenter.z;
		_boundCenter.z = _boundCenter.x;
		_boundCenter.x = t;

		_boundExtens = normalCollider.size;
		_boundExtens.x *= 0.5f * _v.x;//0.5f * _v.x;
		_boundExtens.y *= 0.5f * _v.y;
		_boundExtens.z *= 0.5f * _v.z;//0.5f * _v.z;

		t = _boundExtens.z;
		_boundExtens.z = _boundExtens.x;
		_boundExtens.x = t;

//		if(_isFlip == false)
//		{
//			float tempF = _boundExtens.x;
//			_boundExtens.x = _boundExtens.z;
//			_boundExtens.z = tempF;
//			
//			tempF = _boundCenter.x;
//			_boundCenter.x = _boundCenter.z;
//			_boundCenter.z = tempF;			
//		}

//		Debug.LogError(isPlayerSide + "   isFlip : " + _isFlip + "   "  + _boundCenter + "    ex; " + _boundExtens);

		hitObject.init(_boundCenter, _boundExtens);		
		hitObject.setPosition(cTransformPosition);

//		Debug.LogError("pos : " + cTransformPosition);
//		Debug.LogError("hitObject  x: " + hitObject.x + "  width: " + hitObject.width + "  right : " + hitObject.right);

		bodyYCenter = hitObject.height * 0.55f;

		damageMotionValue = 8.0f;//hitObject.width * _damageMotionValueRatio;
		damageMotionStep2Value = damageMotionValue * _damageMotionStep2ValueRatio;
	}


	public void setHpBar()
	{
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1) return;
#endif

		if(hpBar != null)
		{
			hpBar.visible = true;
			hpBar.setData(hpPer);
			_energyBarShowTime = GameManager.info.setupData.hpBarShowTime;			
		}


		if(monsterUISlotIndex > -1)
		{
			UIPlay.getUnitSlot(monsterUISlotIndex).spHpBar.fillAmount = hpPer;
		}

	}


	public bool setDamageFrame()
	{
		if(!isDamageFrame && _currentDamageTime <= 0.0f)
		{
			isDamageFrame = true;
			
			//if(isMonster)
			{
				_currentDamageTime = MONSTER_DAMAGE_TIME;

				changeShader(true);

				setColor(_damageColor);

				if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(true);
			}
			
			return true;
		}
		
		return false;
	}	
	

	protected void setBoneQuality()
	{
		for(i = 0; i < smrNumber; ++i) 
		{ 
			if(PerformanceManager.isLowPc) smrs[i].quality = SkinQuality.Bone1;
			else smrs[i].quality = SkinQuality.Auto;
		}
	}


	public void setColor(Color c)
	{
		if(isVisibleForSkillCam) return;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
#endif
		for(i = 0; i < smrNumber; ++i) 
		{ 
			smrs[i].sharedMaterial.color = c ;
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].sharedMaterial.color = c;
		}

		if(pet != null) pet.setColor(c);

		if(miniMapPointer != null) miniMapPointer.pointer.color = c;
	}


	public Color getColor()
	{
		for(i = 0; i < smrNumber; ++i) 
		{ 
			return smrs[i].sharedMaterial.color;
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			return mrs[i].sharedMaterial.color;
		}

		return Color.white;
	}



	public void setColorWithoutRareLine(Color c)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif
		for(i = 0; i < smrNumber; ++i) 
		{ 
			smrs[i].sharedMaterial.color = c ;
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].sharedMaterial.color = c;
		}

		if(pet != null) pet.setColor(c);

		if(modelRendererIndex[ModelData.WITH_RARELINE] > -1)
		{
			c.a = 0;
			smrs[modelRendererIndex[ModelData.WITH_RARELINE]].sharedMaterial.color = c;
		}
		//if(miniMapPointer != null) miniMapPointer.pointer.color = c;
	}


	
	protected void updateColor()
	{
		if((_currentDamageTime < 0.0f && isDamageFrame))
		{

			changeShader(false);

			setColor(_normalColor);

			if(monsterUISlotIndex > -1)
			{
				UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);
			}

			isDamageFrame = false;
		}
	}	


	bool _useAdditiveShader = false;
	public void changeShader(bool isAdditive, bool must = false)
	{
		if(isVisibleForSkillCam) return;

		if(monsterData.canChangeShader == false) return;

		if(isAdditive)
		{
			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1) return;
			#endif

			if(PerformanceManager.isLowPc == false && useRimShader == false)
			{
				if(_useAdditiveShader == false || must) setShader(ResourceManager.instance.damageShader);
			}
		}
		else
		{
			if(_useAdditiveShader || must)
			{
				if(useRimShader)
				{
					setShader(ResourceManager.instance.rimShader);
				}
				else
				{
					setShader(ResourceManager.instance.normalShader);
				}
			}
		}

		_useAdditiveShader = isAdditive;

	}



	public void setOriginalShader()
	{
		for(i = 0; i < smrNumber; ++i) 
		{ 
			smrs[i].sharedMaterial.shader = Shader.Find( smrs[i].sharedMaterial.shader.name );
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].sharedMaterial.shader = Shader.Find( mrs[i].sharedMaterial.shader.name );
		}
	}



	public void setShader(Shader shader)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif

		for(i = 0; i < smrNumber; ++i) 
		{ 
			smrs[i].sharedMaterial.shader = shader;
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].sharedMaterial.shader = shader;
		}
		
		if(pet != null) pet.setShader(shader);
	}



	public void setLobbyShader(Shader shader)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif
		
		for(i = 0; i < smrNumber; ++i) 
		{ 
			smrs[i].sharedMaterial.shader = shader;
			smrs[i].sharedMaterial.SetTexture("_Ramp",ResourceManager.instance.lobbyShaderTexture);
			smrs[i].sharedMaterial.SetColor("_SColor",Color.white);
			smrs[i].sharedMaterial.SetColor("_LColor",Color.white);
		}
		
		for(i = 0; i < mrNumber; ++i)
		{
			mrs[i].sharedMaterial.shader = shader;
			mrs[i].sharedMaterial.SetTexture("_Ramp",ResourceManager.instance.lobbyShaderTexture);
			mrs[i].sharedMaterial.SetColor("_SColor",Color.white);
			mrs[i].sharedMaterial.SetColor("_LColor",Color.white);
		}
		
		if(pet != null) pet.setLobbyShader(shader);

//		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
//		_RimPower ("Rim Power", Float) = 1.4
//				
//		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
//		_LColor ("Highlight Color", Color) = (0.5,0.5,0.5,1)
	}

	
	//=== 여기까지 공통으로 쓰는 것 == //
	
	


	
	public void baseUpdate()
	{
		if(isReady == false) return;
		if(_isEnabled == false) return;

		updateColor();	
		
		updateRecoveryData();
		
//		_invincibleTime.Set( _invincibleTime - GameManager.globalDeltaTime );
		_currentDamageTime -= GameManager.globalDeltaTime;

		if(hpBar != null)
		{
			if(_energyBarShowTime > 0)
			{
				_energyBarShowTime-=GameManager.globalDeltaTime;
			}
			else if(_energyBarShowTime > -99)
			{
				hpBar.visible = false;
				_energyBarShowTime = -100.0f;
			}
		}
	}	
	
	
	public virtual void playAni(string id, Callback callback, float fadeTime = 0.2f)
	{
		aniFadeTime = fadeTime;	
		nowAniId = id;
		//fuckyouanimation.CrossFade(id, aniFadeTime);		

		getAniDelayMethod().onCompleteAnimation(ani[id].length, callback);
		//StartCoroutine(onCompleteAnimation(animation[id].length, callback));
	}	


	
	public virtual void onCompleteLoopSkillAni()
	{
	}		


	

	
	public delegate void Callback();
	
	
	
	
	public Xfloat originalAtkRange = 0;
	public Xfloat originalAtkSpeed = 0;
	public Xfloat originalAtkPhysic = 0;
	public Xfloat originalAtkMagic = 0;
	public Xfloat originalDefPhysic = 0;
	public Xfloat originalDefMagic = 0;
	public Xfloat originalSpeed = 0;
	public Xfloat originalMaxHp = 0;
	
	
	protected void saveCharacterOriginalValue()
	{
		originalAtkRange = stat.atkRange;
		originalAtkSpeed = stat.atkSpeed;
		originalAtkPhysic = stat.atkPhysic;
		originalAtkMagic = stat.atkMagic;
		originalDefPhysic = stat.defPhysic;
		originalDefMagic = stat.defMagic;
		originalSpeed = stat.speed;
		originalMaxHp = maxHp;
	}
	
	
	public void showMissEffect(Bullet checkMissBullet = null, Monster checkMissMonster = null)
	{
		if(checkMissBullet != null)
		{
			if(checkMissBullet.attackerInfo.stat.monsterType == Monster.TYPE.UNIT) return;
		}
		else if(checkMissMonster != null)
		{
			if(checkMissMonster.stat.monsterType == Monster.TYPE.UNIT) return;
		}


		WordEffect.showWordEffect(WordEffect.MISS, Color.red, cTransform, _boundExtens.y * 0.7f);
	}


	public virtual void setParent(Transform parent)
	{
		cTransform.parent = parent;
	}




	public void lookDirection(float direction)
	{
		_v = cTransform.position;
		_v.x += direction;
		tf.rotation = Util.getLookRotationQuaternion(_v - cTransform.position);	
	}

	void OnEnable()
	{
		if(ani != null)
		{
			if(ani.GetClip(NORMAL) != null) ani.Play(NORMAL);
			else if(ani.GetClip(WALK) != null) ani.Play(WALK);
		}

		if(particle != null) particle.Play();

		_doRenderSkipFrame = false;
	}	
	
	void OnDisable()
	{
		if(ani != null)
		{
			if(ani.GetClip(NORMAL) != null) ani.Play(NORMAL);
			else if(ani.GetClip(WALK) != null) ani.Play(WALK);
		}

		_doRenderSkipFrame = false;
	}	

	void OnDestroy()
	{
		smrs = null;
		mrs = null;
		destroy(false);
	}


	public virtual void destroy(bool isOriginal)
	{
		resetDefaultVals();

//		CharacterUtil.removeRareLineMaterial(this, isOriginal);

		if(heads != null) heads.Clear();
		heads = null;

		if(bodies != null) bodies.Clear();
		bodies = null;

		if(weapons != null) weapons.Clear();
		weapons = null;
		
		shootingPos = null;
		
		shootingHand = null;
		
		if(shootingPositions != null) shootingPositions.Clear();
		shootingPositions = null;

		if(shootingTransforms != null) shootingTransforms.Clear();
		shootingTransforms = null;

		if(effectParents != null) effectParents.Clear();
		effectParents = null;

		resourceId = null;

		_tempAniData = null;

		skillMove = null;

		skillTargetChecker = null;

		if(attackers != null) attackers.Clear();
		attackers  = null;

		if(skillAttackers != null) skillAttackers.Clear();
		skillAttackers  = null;

		target = null;
		skillTarget = null;

		deleteMotionEffect = null;

		if(receiveDamageMonstersByMe != null) receiveDamageMonstersByMe.Clear();
		receiveDamageMonstersByMe = null;

		attacker = null;

		if(animationStates != null) animationStates.Clear();
		animationStates = null;

		chainLighting  = null;

		if(partsLoader != null) partsLoader.cha = null;
		partsLoader = null;

		modelRendererIndex = null;

		normalCollider = null;

		hitObject = null;


		if(smrs != null)
		{
			for(int i = 0; i < smrNumber; ++i)
			{
				if(smrs[i].sharedMaterial != null) 
				{
					smrs[i].sharedMaterial.shader = null;

					if(isOriginal && smrs[i].sharedMaterial.mainTexture != null)
					{
						DestroyImmediate(smrs[i].sharedMaterial.mainTexture,true); // 주인공 파츠에는 적용하면 안된다.
					}

					smrs[i].sharedMaterial.mainTexture = null;
					DestroyImmediate(smrs[i].sharedMaterial,true);
					smrs[i].sharedMaterial = null;
				}
			}				
		}

		if(mrs != null)
		{
			for(int i = 0; i < mrNumber; ++i)
			{
				if(mrs[i].sharedMaterial != null) 
				{
					mrs[i].sharedMaterial.shader = null;
					
					if(isOriginal && mrs[i].sharedMaterial.mainTexture != null )
					{
						DestroyImmediate(mrs[i].sharedMaterial.mainTexture,true);
					}
					
					mrs[i].sharedMaterial.mainTexture = null;
					DestroyImmediate(mrs[i].sharedMaterial,true);
					mrs[i].sharedMaterial = null;
				}
			}				
		}


		smrNumber = 0;
		mrNumber = 0;
		smrs = null;
		mrs = null;
		aiSlots = null;
		ani = null;
		if(animationStates != null) animationStates.Clear();
		animationStates = null;
		if(bodies != null) bodies.Clear();
		bodies = null;
		bulletPatternId = null;
		nowBulletPatternId = null;
		chainLighting = null;
		normalCollider = null;
		npcData = null;
		unitData = null;
		heroMonsterData = null;
		monsterData = null;
		playerData = null;

		stageMonsterData = null;

		cTransform = null;

		container = null;

		tf = null;

		combineRoot = null;

		if(attachedBullet  != null) attachedBullet.Clear();
		attachedBullet = null;

		attackData  = null;

		if(_hitBulletTime  != null) _hitBulletTime.Clear();
		_hitBulletTime  = null;

		characterEffect = null;

		if(_effects  != null) _effects.Clear();
		_effects  = null;

		ani = null;

		
		particle = null;
		
		particleEffect = null;
		
		skillAttachedEffect = null;

		combineRoot = null;




		if(unitSlots != null)
		{
			for(int i = unitSlots.Length - 1; i >=0; --i)
			{
				if(unitSlots[i] != null) unitSlots[i].destroy();
				unitSlots[i] = null;
			}
		}

		if(skillSlots != null)
		{
			for(int i = skillSlots.Length - 1; i >=0; --i)
			{
				if(skillSlots[i] != null) skillSlots[i].destroy();
				skillSlots[i] = null;
			}
		}

		if(aiSlots != null)
		{
			for(int i = aiSlots.Length - 1; i >=0; --i)
			{
				if(aiSlots[i] != null) aiSlots[i].destroy();
				aiSlots[i] = null;
			}
		}

		shadow = null;
		
		cTransform = null;
		tf = null;
		target = null;
		toolTip = null;

		shootingHand = null;

		if(shootingPositions != null) shootingPositions.Clear();
		shootingPositions = null;

		if(shootingTransforms != null) shootingTransforms.Clear();
		shootingTransforms = null;

		if(effectParents != null) effectParents.Clear();
		effectParents = null;

		characterEffect = null;

		if(faceAniEye != null) faceAniEye.clear();
		faceAniEye = null;

		if(faceAniEye2 != null) faceAniEye2.clear();
		faceAniEye2 = null;

		if(faceAniMouth != null) faceAniMouth.clear();
		faceAniMouth = null;

		if(faceAniMouth2 != null) faceAniMouth2.clear();
		faceAniMouth2 = null;

		partsLoader = null;
	}
}


