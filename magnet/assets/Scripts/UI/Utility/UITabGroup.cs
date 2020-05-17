using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
 * 스크립트 설명
 * TabButton 기능이다.
 * 하나의 버튼만 활성화 시킨다. 지금은 Delegate를 안하지만 추후에 필요하다면 추가해도 된다.
 * TabList에는 GameObject를 추가만 해주면 됨. 컬러 색상은 원하는 걸로 변경 가능.
 * TabList에 등록되어 있으면 UITabbase는 추가가 되어 있지 않아도 된다. Start()에서 알아서 없으면 추가함.
 * CoercionTab같은 경우에는 강제로 탭을 변경시키는 기능. 외부에서 접근할때 사용하면 된다.
 * 실행할 함수가 존제한다면 CallBack, CallBackArray 델리게이트를 이용해주자.
 */

public class UITabGroup : MonoBehaviour {
   
    public List<GameObject> TabList;
    public bool IsChangeImg;//버튼 이름에 사용하는 스프라이트 이름 똑같이 지정해줘야함.

    public string EnableIcon = "n_btn_d_07";//클릭된 객체의 스프라이트를 변경하고 싶다면 지정하면 된다.
    public string DisableIcon = "n_btn_d_12";//클릭되지 않은 객체의 스프라이트를 변경하고싶다면 지정하면 된다.
    public Color EnableColor = Color.clear;
    public Color DisableColor = Color.clear;
    public Color EnableEffColor = Color.clear;
    public Color DisableEffColor = Color.clear;

    //public List<GameObject> IgnoreObj;//무시하는 객체들이 있다면 등록.

    public delegate bool OnCallBack();
    public delegate void OnCallBackIndex(int idx);
    private OnCallBack ConCallback;//선행조건 콜백
    private OnCallBackIndex CallBackIdx;

    /// <summary>
    /// -1일 경우 초기화 하지 아니함.
    /// </summary>
    public int DefaultInitIndex = 0;
    public bool IsTemp;//콜백 함수 없이 제어하는거 없이. 그냥 버튼 동작만 사용할 경우.

    void Start()
    {
        if (EnableColor != Color.clear && EnableColor.a == 0)
            EnableColor.a = 1;

        if (DisableColor != Color.clear && DisableColor.a == 0)
            DisableColor.a = 1;

        if (EnableEffColor != Color.clear && EnableEffColor.a == 0)
            EnableEffColor.a = 1;

        if (DisableEffColor != Color.clear && DisableEffColor.a == 0)
            DisableEffColor.a = 1;

        if (!IsTemp)
            return;
        
        Initialize(null);
    }

    public void Initialize(OnCallBackIndex callBackIdx, OnCallBack conCallBack =null)
    {
        for(int i=0; i < TabList.Count; i++)
        {
            //스크립트가 추가가 되어 있지 아니하다면 추가하려고 검사한다.
            UITabbase tabBase = TabList[i].GetComponent<UITabbase>();
            if (tabBase == null)
            {
                tabBase = TabList[i].AddComponent<UITabbase>();
                tabBase.Init();
            }
            //누구한테 클릭되면 신호를 보내야 할지 정해준다.
            tabBase.TabGroup = this;
            
            UIButton uiBtn = TabList[i].GetComponent<UIButton>();
            if (uiBtn != null)
                uiBtn.duration = 0;
                    //uiBtn.tweenTarget = null;
        }

        ConCallback = conCallBack;
        CallBackIdx = callBackIdx;
        if (DefaultInitIndex < 0)
            return;

        OnClickChildBtn(TabList[DefaultInitIndex]);
    }

    /// <summary>
    /// 강제로 바꿔주고 싶다면
    /// </summary>
    /// <param name="array">바꿔줄 객체의 배열 번호</param>
    public void CoercionTab(int array)
    {
        if (TabList.Count <= array)
            return;

        GameObject targetGo = TabList[array];
        OnClickChildBtn(targetGo);
    }

    /// <summary>
    /// 강제로 바꿔주고 싶다면
    /// </summary>
    /// <param name="targetGo">바꿔줄 객체</param>
    public void CoercionTab(GameObject targetGo)
    {
        if (targetGo == null)
            return;

        OnClickChildBtn(targetGo);
    }

    /// <summary>
    /// UITabBase에서 클릭이 된 객체가 있다면 실행함
    /// 혹은 초기화할때 사용한다.
    /// </summary>
    /// <param name="targetGo">클릭이 된 객체</param>
    public void OnClickChildBtn(GameObject targetGo)
    {
        if (ConCallback != null && !ConCallback())
            return;

        for (int i = 0; i < TabList.Count; i++)
        {
            GameObject go = TabList[i];
            if (go != targetGo)
            {
                Changed(go, false, i);
                continue;
            }
            
            Changed(go, true, i);
        }
    }

    /// <summary>
    /// 라벨 컬러, UIButton의 상태를 변경함
    /// </summary>
    /// <param name="targetGo">바꿔질 객체</param>
    /// <param name="isEnable">어떤 상태로?</param>
    /// /// <param name="array">몇 번째 객체인지.</param>
    void Changed(GameObject targetGo, bool isEnable, int array)
    {
        targetGo.collider.enabled = !isEnable;
        DefaultChanged(targetGo, isEnable ? EnableIcon : DisableIcon,
            isEnable ? EnableColor : DisableColor,
            isEnable ? EnableEffColor : DisableEffColor );
        if (isEnable)
        {
            if (CallBackIdx != null)
                CallBackIdx(array);
        }
    }

    /// <summary>
    /// 콜라이더와 콜백함수를 제외한 것들 셋팅
    /// </summary>
    /// <param name="targetGo"></param>
    /// <param name="icon"></param>
    /// <param name="color"></param>
    void DefaultChanged(GameObject targetGo, string icon, Color col, Color effCol)
    {
        if(col != Color.clear)
        {
            UITabbase tabBase = targetGo.GetComponent<UITabbase>();
            tabBase.Label[0].color = col;
        }

        if(effCol != Color.clear)
        {
            UITabbase tabBase = targetGo.GetComponent<UITabbase>();
            tabBase.Label[0].effectColor = effCol;
        }

        if (!IsChangeImg)
        {
            UITabbase tabBase = targetGo.GetComponent<UITabbase>();
            //tabBase.Label.color = color;//일단 주석 추후에 사용하면 주석 풀고 사용할일없으면 삭제하자.
            //tabBase.Sprite.color = color;

            if (!string.IsNullOrEmpty(icon) && tabBase.Sprite != null)
                tabBase.ChangeSprite(icon);
        }
        else
        {
            Transform targetTf = targetGo.transform;
            Transform iconTf = targetTf.FindChild(icon);
            if (iconTf != null)
                targetTf.FindChild(icon).gameObject.SetActive(true);
            else
                Debug.LogError(string.Format("error not found object = {0}", icon) );

            if (icon.Contains(DisableIcon))
                targetTf.FindChild(EnableIcon).gameObject.SetActive(false);
            else
                targetTf.FindChild(DisableIcon).gameObject.SetActive(false);
        }
        
        UIButton uiBtn = targetGo.GetComponent<UIButton>();
        if (uiBtn != null)
        {
            if (!string.IsNullOrEmpty(icon))
                uiBtn.normalSprite = icon;
            
            uiBtn.duration = 0;
            //uiBtn.defaultColor = color;
            //  uiBtn.defaultColor = col;
        }
    }

    /// <summary>
    /// Initialize 실행 뒤에 해줘야함.
    /// </summary>
    /// <param name="tabTextArray">넣고자하는 텍스트들, *주의 할점 : TabList와 길이가 동일해야 함</param>
    public void SetTabText(string[] tabTextArray)
    {
        if (TabList.Count <= 0 || TabList[0].GetComponent<UITabbase>() == null)
            return;

        int loopCount = TabList.Count;
        for (int i=0; i < loopCount; i++)
        {
            UITabbase tabBase = TabList[i].GetComponent<UITabbase>();
            if (i < tabTextArray.Length)
                tabBase.ChangeLabel(tabTextArray[i]);
            else
                tabBase.gameObject.SetActive(false);
        }
    }
}
