using UnityEngine;
using System.Collections;

public class GetTextUI : MonoBehaviour {

    public UILabel Text;
    public TweenScale tweenScale;
    public TweenAlpha tweenAlpha;

    GameObject owner;
    float factor = 0f;
    float randomX = 0;
    float screenWScale;// 화면비율에 따른 위치 보정용
    Vector3 offset = new Vector3(0, 3, 0);

    public void Show(GameObject target, string text)
    {
        screenWScale = (float)((float)1280 / Screen.width);

        owner = target;
        gameObject.SetActive(true);
        
        Text.text = text;
        
        factor = 0;
        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();
        tweenScale.ResetToBeginning();
        tweenScale.PlayForward();
        /*
        int RanX = Random.Range(0, 0);

        if (TownState.TownActive)
            randomX += (float)(RanX * 0.01f);
        else
            randomX += (float)(RanX * 0.02f);

        if (Random.Range(0, 2) != 0)
            randomX *= -1;
        */
        int RanX = Random.Range(0, 500);
        randomX += (float)(RanX * 0.05f);
        // 시작, 끝점 설정
        transform.localPosition = GetUIPos();
        
        //< 처음 시작위치 저장
        StartOwnelPos = owner.transform.position;
        StartOwnelPos.y += 1f;
        Update();
    }

    void Update()
    {
        if (null == owner)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 newPos = GetUIPos() + (Vector3.up * factor);
        newPos.x += randomX;
        factor += (10 + (factor * 2)) * Time.deltaTime;

        transform.localPosition = newPos;
        if (factor >= 120)
            gameObject.SetActive(false);
    }

    Vector3 StartOwnelPos;
    Vector3 GetUIPos()
    {
        return MathHelper.WorldToUIPosition(StartOwnelPos + offset) * screenWScale;
    }
}
