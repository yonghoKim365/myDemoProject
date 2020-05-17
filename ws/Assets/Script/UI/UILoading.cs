using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UILoading : UIBase {

	public static bool nowLoading = false;

	const int MAX_NUM = 5;

	public Monster sample = null;
	float[] defaultScale = new float[MAX_NUM];

	public Camera characterCamera;

	Vector3 _v;
	Quaternion _q;
	public bool ready = false;

	public UILabel lbTip, lbCharacterName, lbRole, lbAttackType, lbSkill; 
	public Transform sampleContainer;

	public UISprite spProgress;

	int totalCount = 0;

	private float _startTime = -1000.0f;

	public const float MIN_TIME = 3.0f;

	public UISprite spGrade;

	public override void show ()
	{
#if UNITY_EDITOR
		Debug.LogError("=== show loading");
#endif

		GameManager.me.uiManager.menuCamera3.gameObject.SetActive(false);

		_startTime = RealTime.time;

		GameManager.me.playCamRenderImage.enabled = false;

		ready = false;

		base.show ();

		removeSample();

		init ();

		UIPlay.nowSkillEffectCamStatus = UIPlay.SKILL_EFFECT_CAM_STATUS.None;

		GameManager.soundManager.clearSound();

		lbTip.text = LoadingTipData.getTip();
	}


	public bool checkCloseTime()
	{
		return (RealTime.time - _startTime >= MIN_TIME);
	}


	public override void hide()
	{
		_startTime = -1000;
		spProgress.fillAmount = 0.0f;
		ready = false;
		removeSample();

		lbTip.text = "";
		lbRole.text = "";
		lbSkill.text = "";
		lbCharacterName.text = "";
		lbAttackType.text = "";

		base.hide();
	}

	public void removeSample()
	{
		if(sample != null)
		{
			if( sample.ani.GetClip(Monster.ATK) == null)
			{
				sample.ani[Monster.NORMAL].time = 0.0f;
				sample.ani[Monster.NORMAL].speed = 1.0f;
			}
			else
			{
				sample.ani[Monster.ATK].time = 0.0f;
				sample.ani[Monster.ATK].speed = 1.0f;
			}

			GameManager.me.characterManager.cleanMonster(sample);
			sample = null;
		}

//		if(mTrans != null)
//		{
//			iTween it = mTrans.GetComponent<iTween>();
//			if(it != null) it.enabled = false;
//		}
	}

	void init()
	{
		lbCharacterName.text = "";
		spProgress.fillAmount = 0;
	}

	public bool isWaitingForLoading = false;
	public bool isCloseLoadingSecene = false;

	public void showLoadingTipMonster()
	{
		if(gameObject.activeSelf)
		{
			ready = false;
			isWaitingForLoading = true;
			isCloseLoadingSecene = false;
			setPreloadingMonster();
		}
		else isWaitingForLoading = false;
	}

	void setPreloadingMonster()
	{
		RoundData rd = GameManager.me.stageManager.nowRound;

		if(rd.id.Contains("TEST")) return;


		int totalWeight = 0;

		foreach(KeyValuePair<string, LoadingScreenData> kv in GameManager.info.loadingScreenData)
		{
			totalWeight += kv.Value.weight;
		}

		int selectPoint = UnityEngine.Random.Range(0,totalWeight+1);
		int currentPoint = 0;

		foreach(KeyValuePair<string, LoadingScreenData> kv in GameManager.info.loadingScreenData)
		{
			currentPoint += kv.Value.weight;

			if(currentPoint >= selectPoint)
			{
				StartCoroutine(getMonster( kv.Value ));
				return;
			}
		}


		foreach(KeyValuePair<string, LoadingScreenData> kv in GameManager.info.loadingScreenData)
		{
			StartCoroutine(getMonster( kv.Value ));
			return;
		}
	}



	IEnumerator getMonster( LoadingScreenData ld  )
	{
		totalCount = 0;

		if(gameObject.activeInHierarchy)
		{
			GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData[ ld.unitId ].resource]);

			GameDataManager.instance.startModelLoad(); 
			
			while(GameDataManager.instance.isCompleteLoadModel == false)
			{ 
				yield return null; 
			}
		}

		if(gameObject.activeInHierarchy)
		{
			sample = GameManager.me.characterManager.getMonster(false,true,GameManager.info.unitData[ ld.unitId ].resource,false);

			sample.unitData = GameManager.info.unitData[ld.unitId];

			if(sample.unitData.rare == RareType.SS)
			{
				spGrade.spriteName = "img_runegrade_ss";
			}
			else
			{
				spGrade.spriteName = "img_runegrade_s";
			}

			sample.heroMonsterData  = null;

			sample.setParent( sampleContainer );

			sample.setColor(Color.white);
			//_v.x = 0.0f + cellWidth*((i+1)-1); 
			_v.x = 0;
			_q = sample.tf.localRotation;
			_v = _q.eulerAngles;
			_v.x = 6.5f; _v.y = 200.0f; _v.z = 0.0f;
			_q.eulerAngles = _v;

			sample.tf.localRotation = _q;

			lbCharacterName.text = ld.name;
			lbRole.text = ld.type;
			lbAttackType.text = ld.attackType;
			lbSkill.text = ld.skill;
			resultCode = ld.code;

			parseCode();

			resize ();

			++totalCount;
		}

		if(gameObject.activeInHierarchy == false)
		{
			removeSample();
		}
		else
		{
			ready = true;
		}

		isWaitingForLoading = false;
		GameDataManager.progress = 0;
	}

	bool mIsDragging = false;

	Transform mTrans;
	Vector3 mPosition, mLocalPosition;
	Vector3 mDragStartPosition;
	Vector3 mDragPosition;
	Vector3 mStartPosition;

	public float cellWidth = 370.0f;
	public int seq = 3;

	void Awake()
	{
		mTrans = sampleContainer;
		mPosition = mTrans.position;
		mLocalPosition = mTrans.localPosition;
	}


	/*
	void SetSequence(bool isRight)
	{
		Vector3 dist = mLocalPosition - mTrans.localPosition;
		float distX = Mathf.Round(dist.x/cellWidth);
		seq = (int)distX;
	}


	Hashtable hash = new Hashtable();
	void SetPosition(bool isMotion)
	{
		Vector3 pos = mLocalPosition;
		pos -= new Vector3(seq * cellWidth, 0f, 0f);
		if (isMotion) 
		{
			hash.Clear();
			hash.Add("position", pos);
			hash.Add("islocal", true);
			hash.Add("time", 0.3f);
			hash.Add("easetype", iTween.EaseType.linear);
			iTween.MoveTo(mTrans.gameObject, hash);
		} 
		else 
		{
			mTrans.localPosition = pos;
		}

		int nowIndex = 0;
		if(seq+1 < 0)
		{
			if((seq+1) % totalCount == 0 ) nowIndex = 0;
			else nowIndex = totalCount + ((seq+1) % totalCount);
		}
		else nowIndex = ((seq+1) % totalCount);

		if(sample[nowIndex] == null) return;

		if(sample[nowIndex].unitData != null)
		{
			lbTitle.text = sample[nowIndex].unitData.name;
		}
		else if(sample[nowIndex].heroMonsterData != null)
		{
			lbTitle.text = sample[nowIndex].heroMonsterData.name;
		}
	}


	void Drop () 
	{
		if(sample == null || sample == null || ready == false) return;

		Vector3 dist = mDragPosition - mDragStartPosition;
		if (dist.x>0f) SetSequence(true);
		else SetSequence(false);
		SetPosition(true);
	}
	
	void OnDrag (Vector2 delta) 
	{
		if(sample == null || sample == null || ready == false) return;

		Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
		float dist = 0f;
		Vector3 currentPos = ray.GetPoint(dist);
		
		if (UICamera.currentTouchID == -1 || UICamera.currentTouchID == 0) {
			if (!mIsDragging) 
			{
				mIsDragging = true;
				mDragPosition = currentPos;
			} 
			else 
			{
				Vector3 pos = mStartPosition - (mDragStartPosition - currentPos);
				Vector3 cpos = new Vector3(pos.x, mTrans.position.y, mTrans.position.z);
				mTrans.position = cpos;
			}
		}
	}
	
	void OnPress (bool isPressed) 
	{
		if(sample == null || sample == null || ready == false) return;

		mIsDragging = false;
		Collider col = collider;
		if (col != null) 
		{
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition);
			float dist = 0f;
			mDragStartPosition = ray.GetPoint(dist);
			mStartPosition = mTrans.position;
			col.enabled = !isPressed;
		}
		if (!isPressed) Drop();
	}

*/

	Color _c = new Color();
	void LateUpdate()
	{
		if(_startTime > -1000)
		{
			float progress = 0.0f;
			float timeProgress = (RealTime.time - _startTime) / MIN_TIME  ;

			if(GameManager.me.recordMode == GameManager.RecordMode.continueGame)
			{
				progress = isWaitingForLoading?0.0f:0.5f + (GameDataManager.progress * 0.5f) * 0.7f + (2.0f - GameManager.me.replayPrepareDelay) * 0.3f;
			}
			else
			{

				if(isCloseLoadingSecene == false)
				{
					if(isWaitingForLoading)
					{
						progress = ((float)totalCount / 1.0f )*0.3f;
					}
					else
					{
						progress = 0.3f + (GameDataManager.progress * 0.5f);
					}
				}
				else
				{
					progress = ((float)totalCount /1.0f);
				}
			}

			if(progress > 0.9f && timeProgress > 0.75f)
			{
				spProgress.fillAmount = progress;
			}
			else
			{
				spProgress.fillAmount = Mathf.Min( progress , timeProgress);
			}
		}
		return;

		/*
		if(sample == null || sample == null || ready == false) return;

		int currentSeq = (int)Mathf.Round(mLocalPosition.x - mTrans.localPosition.x/cellWidth);
		
		int firstIndex = 0;
		
		if(currentSeq < 0)
		{
			if(currentSeq % totalCount == 0 ) firstIndex = 0;
			else firstIndex = totalCount + (currentSeq % totalCount);
		}
		else firstIndex = (currentSeq % totalCount);

		float dist = 0;
		float color = 0;

		for(int i = 0; i < 3; ++i)
		{
			_v = sample[firstIndex].cTransform.localPosition;
			_v.x = (currentSeq-1) * cellWidth + i * cellWidth;
			sample[firstIndex].cTransform.localPosition = _v;
			
			dist = MathUtil.abs(6971.0f,sample[firstIndex].cTransform.position.x);
			dist /= cellWidth;
			if(dist > 1.0f) dist = 1.0f;

			_c.r = 1.0f - dist * 0.6f;
			_c.g = _c.r; _c.b = _c.r;

			dist = 1.0f - dist * 0.4f;

			sample[firstIndex].setColor(_c);

			_v.x = (defaultScale[firstIndex]) * dist;
			_v.y = _v.x; _v.z = _v.x;
			sample[firstIndex].cTransform.localScale = _v;
			++firstIndex;
			if(firstIndex >= totalCount) firstIndex = 0;
		}
		*/
	}


	public int editMode_nowIndex = 0;
	public int editMode_totalMonster = 0;
	public string editMode_monster = "";
	public string editModeResource = "";

	List<LoadingScreenData> _editDatas;

	public void initEditMode()
	{
		_editDatas = new List<LoadingScreenData>();

		editMode_nowIndex = 0;

		List<string> selectUnits = new List<string>();

		foreach(KeyValuePair<string, LoadingScreenData> kv in GameManager.info.loadingScreenData)
		{
			_editDatas.Add(kv.Value);
		}

		editMode_totalMonster = _editDatas.Count;
	}


	public void editModeNextMonster()
	{
		removeSample();

		StartCoroutine(editModeGetMonster());
	}


	IEnumerator editModeGetMonster()
	{
		int i = editMode_nowIndex;

		if(GameManager.info.unitData[_editDatas[i].unitId].rare == RareType.SS)
		{
			spGrade.spriteName = "img_runegrade_ss";
		}
		else
		{
			spGrade.spriteName = "img_runegrade_s";
		}

		editModeResource = GameManager.info.monsterData[GameManager.info.unitData[_editDatas[i].unitId].resource].resource;

		GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[GameManager.info.unitData[_editDatas[i].unitId].resource]);
				
		GameDataManager.instance.startModelLoad(); 
			
		while(GameDataManager.instance.isCompleteLoadModel == false)
		{ 
			yield return null; 
		}

		sample = GameManager.me.characterManager.getMonster(false,true,GameManager.info.unitData[_editDatas[i].unitId].resource,false);
		sample.unitData = GameManager.info.unitData[_editDatas[i].unitId];
		sample.setParent( sampleContainer );
		sample.setColor(Color.white);

		editMode_monster = sample.unitData.baseUnitId;

		_v.x = 0.0f;
		
		ModelData md = GameManager.info.modelData[sample.resourceId];
		
		_v.y = GameManager.info.modelData[sample.resourceId].shotYPos; 
		_v.z = 0.0f;
		sample.cTransform.localPosition = _v;
		
		_q = sample.tf.localRotation;
		_v = _q.eulerAngles;
		_v.x = 6.5f; _v.y = 200.0f; _v.z = 0.0f;
		_q.eulerAngles = _v;
		
		sample.tf.localRotation = _q;

		LoadingScreenData ld = _editDatas[i];
		lbCharacterName.text = ld.name;
		lbRole.text = ld.type;
		lbAttackType.text = ld.attackType;
		lbSkill.text = ld.skill;
		resultCode = ld.code;
		parseCode();
		resize ();
				
		sample.container.SetActive(true);
		
		if( sample.ani.GetClip(Monster.LOADING) == null)
		{
			sample.ani[Monster.NORMAL].time = sample.ani[Monster.NORMAL].length / 2.0f;
			sample.ani[Monster.NORMAL].speed = 0.0f;
			sample.ani.Play(Monster.NORMAL);
		}
		else
		{
			sample.ani.Play(Monster.LOADING);
		}

		++editMode_nowIndex;
		if(editMode_nowIndex >= _editDatas.Count) editMode_nowIndex = 0;
	}

	public float editMode_shotScale = 1;
	public Vector3 editModeRotation = new Vector3();
	public Vector3 editModePostion = new Vector3();


	public string resultCode = "";

	public void saveCode()
	{
		ModelData md = GameManager.info.modelData[sample.resourceId];

		editMode_shotScale = (sample.cTransform.localScale / ((float)md.scale / 100.0f)).x;//* 0.01f)).x;

		editModePostion = sample.cTransform.localPosition /  ((float)md.scale / 100.0f);//* 0.01f);

		editModeRotation = sample.cTransform.localRotation.eulerAngles;

		List<string> f = new List<string>();

		f.Add(string.Format("{0:0.0}",editMode_shotScale));

		f.Add(string.Format("{0:0.0}",editModePostion.x));
		f.Add(string.Format("{0:0.0}",editModePostion.y));
		f.Add(string.Format("{0:0.0}",editModePostion.z));

		f.Add(string.Format("{0:0.0}",editModeRotation.x));
		f.Add(string.Format("{0:0.0}",editModeRotation.y));
		f.Add(string.Format("{0:0.0}",editModeRotation.z));


		f.Add(string.Format("{0:0.0}",characterCamera.fieldOfView));
		f.Add(string.Format("{0:0.0}",characterCamera.transform.localPosition.x));
		f.Add(string.Format("{0:0.0}",characterCamera.transform.localPosition.y));
		f.Add(string.Format("{0:0.0}",characterCamera.transform.localPosition.z));


		resultCode = string.Join(",",f.ToArray());

	}

	public void parseCode()
	{
		float[] f = Util.stringToFloatArray(resultCode,',');

		if(f.Length > 6)
		{
			editMode_shotScale = f[0];
			
			editModePostion.x = f[1];
			editModePostion.y = f[2];
			editModePostion.z = f[3];
			
			editModeRotation.x = f[4];
			editModeRotation.y = f[5];
			editModeRotation.z = f[6];
		}

		if(f.Length > 7)
		{
			characterCamera.fieldOfView = f[7];
			
			_v.x = f[8];
			_v.y = f[9];
			_v.z = f[10];
			
			characterCamera.transform.localPosition = _v;
		}
	}



	public void resize()
	{
		try
		{
			ModelData md = GameManager.info.modelData[sample.resourceId];
			
			defaultScale[0] = (editMode_shotScale * ((float)md.scale * 0.01f));
			_v.x = defaultScale[0]; _v.y = _v.x; _v.z = _v.x;
			sample.cTransform.localScale = _v;

			_v = sample.cTransform.localPosition;
			_v = editModePostion * ((float)md.scale * 0.01f);
			sample.cTransform.localPosition = _v;

			_q.eulerAngles = editModeRotation;
			sample.cTransform.localRotation = _q;

			sample.isEnabled = true;

			if( sample.ani.GetClip(Monster.LOADING) == null)
			{
				sample.ani[Monster.NORMAL].time = sample.ani[Monster.NORMAL].length / 2.0f;
				sample.ani[Monster.NORMAL].speed = 0.0f;
				sample.ani.Play(Monster.NORMAL);
			}
			else
			{
				sample.ani[Monster.LOADING].time = sample.ani[Monster.LOADING].length / 2.0f;
				sample.ani[Monster.LOADING].speed = 0.0f;
				sample.ani.Play(Monster.LOADING);
			}
		}
		catch(System.Exception e)
		{

		}
	}



	void OnDisable()
	{
		nowLoading = false;
	}

	void OnEnable()
	{
		nowLoading = true;
	}


}

