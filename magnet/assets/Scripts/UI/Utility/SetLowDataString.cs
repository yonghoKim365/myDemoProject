using UnityEngine;
using System.Collections;

public class SetLowDataString : MonoBehaviour {
    public enum StringType
    {
        Common=0, Item, Unit,
    }

    public int Key;//uint로 설정하면 프리팹에서 설정이 되지 않는다. 
    public int CharArr = -1;//0 ~ 해당 번째만 입력
    public string Insert;//{0}에 추가 될 변수 
    public bool IsHorizontal = true;//텍스트 가로 세로
    public StringType Type = StringType.Common;//어떤 함수를 사용할지 정하는 값

    private UILabel ThisLabel;//자신의 라벨

    void Awake()
    {
        if (ThisLabel == null)
            ThisLabel = this.GetComponent<UILabel>();

    }

    void Start () {
        SetString();
    }
	
    //원활한 작업을 위한 함수. 외부에서 작업을 하고 싶을때 사용.
    public void SetString(int key, StringType type)
    {
        Key = key;
        Type = type;
        //이부분은 아마 Awake 보다 빨리 실행시키면 들어올것이다.
        if (ThisLabel == null)//혹시 모를 예외처리.
            ThisLabel = this.GetComponent<UILabel>();

        SetString();
    }

    void SetString()
    {
        string loc = null;
        switch(Type)
        {
            case StringType.Common :
                loc = _LowDataMgr.instance.GetStringCommon((uint)Key);
                break;
            case StringType.Item :
                loc = _LowDataMgr.instance.GetStringItem((uint)Key);
                break;
            case StringType.Unit :
                loc = _LowDataMgr.instance.GetStringUnit((uint)Key);
                break;
        }

        if (string.IsNullOrEmpty(loc))
            return;

        if (loc.Contains("{0}"))
        {
            loc = string.Format(loc, Insert);
            //return;
        }

        if(0 <= CharArr)
        {
            char[] cArr = loc.ToCharArray();
            if (CharArr < cArr.Length)
                loc = cArr[CharArr].ToString();
            else
            {
                loc = "";
                Debug.LogError("SetLowDataSting is CharArr IndesxOutOfRange error " + CharArr + "/" + cArr.Length);
            }
        }

        if(!IsHorizontal)
        {
            char[] arr = loc.ToCharArray();
            loc = arr[0].ToString();
            int length = arr.Length;
            for(int i=1; i < length; i++)
            {
                if(arr[i] == ' ' )
                    loc += string.Format("");
                else
                    loc = string.Format("{0}\n{1}", loc, arr[i] );
            }
        }

        if (ThisLabel == null)
            Debug.LogError(string.Format("is UILabel null error this={0}, parent={1}, key={2}", gameObject.name, transform.parent != null ? transform.parent.name : "null", Key) );
        else
        {
            if (loc == null)
                ThisLabel.text = Key.ToString();
            else
                ThisLabel.text = loc;
        }
    }
    
}
