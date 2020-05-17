using UnityEngine;
using System.Collections.Generic;

/* 스크립트 설명
 * Tab Button에 추가가 되어 있어야 할 스크립트이다. 자기가 클릭이 되면 TabGroup에 알려서 알아서 셋팅함.
 * 구지 이 스크립트가 추가가 되어 있지 않아도 됨. UITabGroup에서 없으면 알아서 추가한다. 내부 변수 또한 알아서 추가하지만 별도로 지정을 하고 싶다면 해도 된다.
 * 다만 알아서 추가할때 UITabGroup.TabList에 등록되어 있는 객체에 다가 이 스크립트를 추가하니 예외의 상황이 있다면 따로 추가를 해줘야 함.
*/
public class UITabbase : MonoBehaviour {
    [HideInInspector]
    public UITabGroup TabGroup;//UITabGroup에서 추가한다.

    public List<UILabel> Label;//Awake에서 찾아서 추가한다.
    public List<UISprite> Sprite;//Awake에서 찾아서 추가한다.

    public void Init()
    {
        if (Label == null)
        {
            //Label = GetComponent<UILabel>();//자기 자신이 라벨인지
            //if (Label == null)//아니면 자식에서 찾는다.
            //    Label = GetFindUILabel(transform);
            Label = UIHelper.FindComponents<UILabel>(transform);
        }

        if (Sprite == null)
        {
            //Sprite = GetComponent<UISprite>();//자기 자신이 스프라이트인지.
            //if (Sprite == null)//아니면 자식에서 찾는다.
            //    Sprite = GetFindUISprite(transform);
            Sprite = UIHelper.FindComponents<UISprite>(transform);
        }
    }
    
    /// <summary>
    /// 이벤트 함수 자기가 클릭되면 어미인UITabGroup으로 알린다
    /// </summary>
    //void OnPress(bool isPress)
    void OnClick()
    {
        //if (!isPress)
            //return;
        
        TabGroup.OnClickChildBtn(gameObject);
    }

    public void ChangeSprite(string spName)
    {
        for(int i=0; i < Sprite.Count; i++)
        {
            Sprite[i].spriteName = spName;
        }
    }
    
    public void ChangeLabel(string str)
    {
        for(int i=0; i < Label.Count; i++)
        {
            Label[i].text = str;
        }
    }
}
