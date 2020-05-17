using UnityEngine;
using System.Collections;

public class StatusUI : MonoBehaviour {

    public TweenScale tweenScale;
    public TweenAlpha tweenAlpha;
    public UILabel digitLbl;

    GameObject owner;

    Vector3 startPos;
    Vector3 offset = new Vector3(0, 3, 0);
    float duration = 1f;

    // 화면비율에 따른 위치 보정용
    float screenWScale;

	// Use this for initialization
	void Awake() 
    {
        screenWScale = (float)((float)1280 / Screen.width);
	}

    public void Show(GameObject _targetGO, GameObject attacker, string str)
    {
        owner = _targetGO;
        gameObject.SetActive(true);

        digitLbl.text = str;

        value = 0;
        duration = 1f;

        tweenAlpha.delay = duration * 0.7f;
        tweenAlpha.duration = duration * 0.3f;
        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();
        tweenScale.ResetToBeginning();
        tweenScale.PlayForward();

        // 시작, 끝점 설정
        transform.localPosition = GetUIPos();
    }

    float value = 0;
	void Update () 
    {
        if (null == owner)
        {
            gameObject.SetActive(false);
            return;
        }

        value += 120 * Time.deltaTime;
        transform.localPosition = GetUIPos() +(Vector3.up * value);

        if (value > 140)
            this.gameObject.SetActive(false);
	}

    Vector3 GetUIPos()
    {
        //< 마을에서 하는거라면 다른 방법을 찾아야함
        //if (TownState.TownActive)
        //    return (CollectionInfoPanel._EventCamera.WorldToScreenPoint(owner.transform.position + offset) - MathHelper.UI_Center_Pos) * screenWScale;

        return MathHelper.WorldToUIPosition(owner.transform.position + offset) * screenWScale;
    }

    float Bounce(float t)
    {
        return Mathf.Sin(t * (Mathf.PI - 0.5f));
    }
}
