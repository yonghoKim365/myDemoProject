using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameBoardPanel : UIBasePanel
{
    int[] _MaxPoolCount;

    public enum ShowingType
    {
        Damage = 1,
        Heal,
        //HpSlider,
        Say,
        Value,
        RaidBossSay,
        Status,
        Head,
        GetExp,//경험치 획득
        End
    }

    public GameObject DamagedUIPrefab;
    public GameObject StatusUIPrefab;
    //public GameObject HpSliderPrefab;
    public GameObject SayUIPrefab;
    public GameObject RaidBossSayUIPrefab;
    public GameObject LevelUiPrefab;
    public GameObject LimitTimePrefab;
    public GameObject NamePrefab;
    public GameObject GetExpTextPrefab;

    // Combo Widgets
    public Transform   ComboTrans;
    public TweenScale ComboTwScale;
    public TweenAlpha ComboTwAlpha;
    public UILabel ComboLabel;
    
    int damageIndex = 0;
    int statusIndex = 0;
    //int hpSliderIndex = 0;
    int sayIndex = 0;
    int raidsayIndex = 0;
    int valueIndex = 0;
    int headIndex = 0;
    int getExpIndex = 0;

    Dictionary<ShowingType, List<GameObject>> poolDic = new Dictionary<ShowingType, List<GameObject>>();

    GameObject LevelUiObj;
    public override void Init()
    {
        base.Init();

        _MaxPoolCount = new int[(int)ShowingType.End];

        if(!TownState.TownActive)
        {
            _MaxPoolCount[(int)ShowingType.Damage] = 20;
            _MaxPoolCount[(int)ShowingType.Status] = 3;
            //_MaxPoolCount[(int)ShowingType.HpSlider] = 10;
            _MaxPoolCount[(int)ShowingType.Say] = 4;
            _MaxPoolCount[(int)ShowingType.RaidBossSay] = 1;
            _MaxPoolCount[(int)ShowingType.Value] = 10;
            _MaxPoolCount[(int)ShowingType.Head] = 20;
            //_MaxPoolCount[(int)ShowingType.GetExp] = 10;
        }
        else//여기면 마을 아닌가?
        {
            //_MaxPoolCount[(int)ShowingType.Damage] = 10;
            //_MaxPoolCount[(int)ShowingType.Status] = 2;
            //_MaxPoolCount[(int)ShowingType.HpSlider] = 0;
            //_MaxPoolCount[(int)ShowingType.Say] = 1;
            //_MaxPoolCount[(int)ShowingType.RaidBossSay] = 0;
            //_MaxPoolCount[(int)ShowingType.Value] = 0;
            _MaxPoolCount[(int)ShowingType.Head] = 10;
        }

        InitPool(ShowingType.Damage, DamagedUIPrefab, _MaxPoolCount[(int)ShowingType.Damage]);
        InitPool(ShowingType.Status, StatusUIPrefab, _MaxPoolCount[(int)ShowingType.Status]);
        //InitPool(ShowingType.HpSlider, HpSliderPrefab, _MaxPoolCount[(int)ShowingType.HpSlider]);
        InitPool(ShowingType.Say, SayUIPrefab, _MaxPoolCount[(int)ShowingType.Say]);
        InitPool(ShowingType.RaidBossSay, RaidBossSayUIPrefab, _MaxPoolCount[(int)ShowingType.RaidBossSay]);
        InitPool(ShowingType.Value, LimitTimePrefab, _MaxPoolCount[(int)ShowingType.Value]);
        InitPool(ShowingType.Head, NamePrefab, _MaxPoolCount[(int)ShowingType.Head]);
        //InitPool(ShowingType.GetExp, GetExpTextPrefab, _MaxPoolCount[(int)ShowingType.GetExp]);

        // no pooling
        LevelUiObj = Instantiate(LevelUiPrefab) as GameObject;
        LevelUiObj.transform.AttachTo( transform, false );
        LevelUiObj.SetActive( false );

        ComboTrans.gameObject.SetActive(false);
    }

    public override void LateInit()
    {
        base.LateInit();
    }

    public void ShowCombo(int combo, float disappearTime = 1f)
    {
        if (0 == combo)
            return;

        if (!ComboTrans.gameObject.activeSelf)
            ComboTrans.gameObject.SetActive( true );

        ComboTwScale.ResetToBeginning();
        ComboTwScale.Play( true );

        // 콤보 글귀 없애기 위한 처리
        ComboTwAlpha.ResetToBeginning();
        ComboTwAlpha.delay = disappearTime - 0.2f;
        ComboTwAlpha.duration = 0.2f;
        ComboTwAlpha.Play( true );

        ComboLabel.text = combo.ToString();
    }

    public void ShowDamage(GameObject target, GameObject attacker, int damage, bool isMyUnit, bool isCritical, bool isHeal = false, eDamResistType DamResist = eDamResistType.None )
    {
        GameObject damageObject = poolDic[ShowingType.Damage][damageIndex++];
        damageObject.GetComponent<DamageUI>().Show(target, attacker, damage, isMyUnit, isCritical, isHeal, DamResist);
        CheckPool();
    }

    public void ShowBuff(GameObject target, GameObject attacker, string str)
    {
        GameObject _Object = poolDic[ShowingType.Status][statusIndex++];
        _Object.GetComponent<StatusUI>().Show(target, attacker, str);
        CheckPool();
    }
    /*
    public void ShowHpSlider(GameObject target)
    {
        if (TownState.TownActive)
            return;

        GameObject hpObj = poolDic[ShowingType.HpSlider][hpSliderIndex++];
        hpObj.GetComponent<HpSliderUI>().Show( target );
        CheckPool();
    }
    */
    public LimitTimeUI ShowValueUI(GameObject target)
    {
        GameObject valueObj = poolDic[ShowingType.Value][valueIndex++];
        valueObj.GetComponent<LimitTimeUI>().Show(target);
        CheckPool();

        return valueObj.GetComponent<LimitTimeUI>();
    }

    public void ShowHead(GameObject target, string name, uint prefix, uint suffix, bool isMy)
    {
        if (TownState.TownActive)
            return;

        List<GameObject> objList = poolDic[ShowingType.Head];
        bool isCreate = true;
        int loopCount = objList.Count;
        for(int i=0; i < loopCount; i++)
        {
            GameObject obj = objList[i];
            HeadObject head = obj.GetComponent<HeadObject>();
            if (head.Owner != null && head.Owner.cachedTransform == target.transform)//이미 등록되어 있는 유닛.
            {
                head.Show(target, name, prefix, suffix, isMy);
                return;
            }

            if (head.IsActivate)
                continue;

            isCreate = false;
            head.Show(target, name, prefix, suffix, isMy);
            break;
        }

        if (isCreate)
        {
            GameObject headObj = Instantiate(NamePrefab) as GameObject;
            headObj.transform.AttachTo(transform, true);
            headObj.SetActive(true);
            objList.Add(headObj);

            HeadObject head = headObj.GetComponent<HeadObject>();
            head.Show(target, name, prefix, suffix, isMy);
        }
    }

    //난투장에서 색깔변경
    public void ChangeNameColor()
    {
        List<long> AttackerList = (G_GameInfo.GameInfo as FreeFightGameInfo).KillAttackerRoleId;

        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
        {
            List<GameObject> objList = poolDic[ShowingType.Head];

            int loopCount = objList.Count;
            for (int i = 0; i < loopCount; i++)
            {
                GameObject obj = objList[i];
                HeadObject head = obj.GetComponent<HeadObject>();

                head.ChangeNameColor(false);

                for (int j = 0; j < AttackerList.Count; j++)
                {
                    if ( head.Owner as Pc && (ulong)AttackerList[j] == head.Owner.m_rUUID)
                    {
                        if( head !=null)
                        {
                            head.ChangeNameColor(true);
                            break;
                        }
                      
                    }

                }
            }
        }
    }


    /// <summary>
    /// 설정바꾸기
    /// </summary>
    public void ChangeHeadState(byte state)
    {
        if (state == 1 || state == 0)//나(파트너)
        {
            // 마을인가?
            if(TownState.TownActive)
            {
                SceneManager.instance.GetState<TownState>().MyHero.ShowHeadObj(true);
            }
            else
            {
                G_GameInfo.PlayerController.Leader.ShowHeadObj(true);
                if (G_GameInfo.PlayerController.Partners != null)
                {
                    for(int i=0;i< G_GameInfo.PlayerController.Partners.Count;i++)
                    {
                        G_GameInfo.PlayerController.Partners[i].ShowHeadObj(true);
                    }
                }


            }
        }

        List<Unit> unitList = G_GameInfo.GameInfo.characterMgr.allUnitList;
        for (int i = 0; i < unitList.Count; i++)
        {
            if (null == unitList[i] || !unitList[i].Usable)
                continue;
            //unitList[i].IsPartner
            //    || unitList[i].IsLeader ||
            unitList[i].ShowHeadObj(true);
        }


        //List<GameObject> objList = poolDic[ShowingType.Head];
        //for (int i = 0; i < objList.Count; i++)
        //{
        //    GameObject obj = objList[i];
        //    HeadObject head = obj.GetComponent<HeadObject>();
        //    if(head.Owner!=null)
        //    {
        //        head.ChangeHead(obj);   
        //    }
        //}
    }

    /// <summary> 경험치 획득 표기</summary>
    public void ShowGetExp(GameObject target, string getExpText)
    {
        GameObject getExpObject = poolDic[ShowingType.GetExp][getExpIndex++];
        getExpObject.GetComponent<GetTextUI>().Show(target, getExpText);

        CheckPool();
    }

    // 대상객체의 위치에 대사창 띄우기
    public GameObject ShowSay(GameObject target, string message, float duration, bool overlap = false, bool _CutSceneEvent = false)
    { 
        GameObject sayObj = poolDic[ShowingType.Say][sayIndex++];
        
        // simply 똑같은 객체에 대사창 여러개 안뜨도록 하기.
        if (!overlap)
        { 
            List<GameObject> uiObjs = poolDic[ShowingType.Say];
            for (int i = 0; i < uiObjs.Count; i++)
            {
                GameObject uiGO = uiObjs[i];
                if (target == uiGO.GetComponent<SayUI>().Owner)
                    uiGO.SetActive( false );
            }
        }

        sayObj.GetComponent<SayUI>().Show(target, message, duration, _CutSceneEvent);
        CheckPool();

        return sayObj;
    }

    // 화면 중앙에 띄우기
    public GameObject RaidBossShowSay(GameObject target, string message, float duration, bool overlap = false)
    {
        GameObject sayObj = poolDic[ShowingType.RaidBossSay][raidsayIndex++];
        sayObj.GetComponent<RaidBossSayUI>().Show(target, message, duration);
        CheckPool();

        return sayObj;
    }

    public void HideAll(ShowingType showType)
    {
        foreach (GameObject obj in poolDic[showType])
        {
            obj.SetActive( false );
        }
    }

    //< 해당 타입을 제외하고 모두 제어한다
    List<GameObject> ActiveList = new List<GameObject>();
    public void AllActive(ShowingType showType, bool type)
    {
        if(!type)
        {
            foreach (KeyValuePair<ShowingType, List<GameObject>> obj in poolDic)
            {
                if (obj.Key == showType)
                    continue;

                for (int i = 0; i < obj.Value.Count; i++)
                {
                    if (obj.Value[i].activeSelf)
                    {
                        ActiveList.Add(obj.Value[i]);
                        obj.Value[i].SetActive(type);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < ActiveList.Count; i++)
                ActiveList[i].SetActive(true);

            ActiveList.Clear();
        }
        
    }

    public void ShowLevelUp()
    {
        LevelUiObj.SetActive( true );

        UITweener[] tweens = LevelUiObj.GetComponents<UITweener>();
        foreach (UITweener tween in tweens)
        {
            tween.ResetToBeginning();
            tween.PlayForward();
        }

        TweenPosition tp = LevelUiObj.GetComponent<TweenPosition>();
        EventDelegate.Set( tp.onFinished, () => { LevelUiObj.SetActive( false ); } );
    }

    void InitPool(ShowingType showingType, GameObject prefabGO, int _MaxPoolCount)
    {
        if (!poolDic.ContainsKey( showingType ))
            poolDic.Add( showingType, new List<GameObject>() );

        for (int i = 0; i < _MaxPoolCount; ++i)
        {
            GameObject poolGO = GameObject.Instantiate( prefabGO ) as GameObject;
            poolGO.transform.AttachTo( transform, true );
            poolGO.SetActive( false );

            poolDic[showingType].Add( poolGO );
        }
    }

    void CheckPool()
    {
        //int maxCnt = MaxPoolCount - 1;

        if (_MaxPoolCount[(int)ShowingType.Damage] <= damageIndex)
            damageIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.Status] <= statusIndex)
            statusIndex = 0;
        //if (_MaxPoolCount[(int)ShowingType.HpSlider] <= hpSliderIndex)
            //hpSliderIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.Say] <= sayIndex)
            sayIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.RaidBossSay] <= raidsayIndex)
            raidsayIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.Value] <= valueIndex)
            valueIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.Head] <= headIndex)
            headIndex = 0;
        if (_MaxPoolCount[(int)ShowingType.GetExp] <= getExpIndex)
            getExpIndex = 0;
    }

}
