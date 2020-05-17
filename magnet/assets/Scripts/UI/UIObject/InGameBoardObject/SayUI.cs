using UnityEngine;
using System.Collections;

public class SayUI : MonoBehaviour
{
    public TweenAlpha   tweenAlpha;
    public UILabel      sayLabel;

    // 화면비율에 따른 위치 보정용
    float screenWScale;

    public GameObject  Owner;
    public Vector3     offset;
    float       factor = 0f;
    float       duration = 1f;

    bool CutSceneEvent = false;
    void Awake()
    {   
        screenWScale = (float)((float)1280 / Screen.width);
    }

    public void Show(GameObject _targetGO, string message, float _duration, bool _CutSceneEvent = false)
    {
        Owner = _targetGO;
        gameObject.SetActive( true );

        duration = _duration;
        tweenAlpha.delay = duration * 0.9f;
        tweenAlpha.duration = duration * 0.1f;
        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();

        sayLabel.text = message;

        Unit unit = _targetGO.GetComponent<Unit>();
        if (unit)
            offset = new Vector3( 0, unit.Height, 0 );
        else
            offset = Vector3.zero;

        CutSceneEvent = _CutSceneEvent;
        
        // 시작, 끝점 설정
        transform.localPosition = GetUIPos();
    }

    void Update()
    {
        if (null == Owner)
        {
            gameObject.SetActive( false );
            return;
        }

        transform.localPosition = GetUIPos();
        
        if (factor >= 1f)
        {
            gameObject.SetActive( false );
        }
        else
        { 
            factor += (1f / duration) * Time.deltaTime;
            factor = Mathf.Clamp01( factor );
        }
    }

    Vector3 GetUIPos()
    {
        if (CutSceneEvent)
            return CutSceneMgr.WorldToUIPosition(Owner.transform.position + offset) * screenWScale;

        return MathHelper.WorldToUIPosition( Owner.transform.position + offset ) * screenWScale;
    }

    void OnDisable()
    {
        Owner = null;
    }
}
