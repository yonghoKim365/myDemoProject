using UnityEngine;
using System.Collections;

public class DamageUI : MonoBehaviour 
{
    //static Color DamagedEnemyColor = new Color(1f, 0.43f, 0f);

    public TweenScale   tweenScale;
    public TweenAlpha   tweenAlpha;
    public UISprite     criticalSpr;
    public UILabel      digitLbl;
    public UISprite DamInfoSprite;
    
    GameObject  owner;

    Vector3 offset = new Vector3(0, 3, 0);
    int defaultFontSize;
    float factor = 0f;
    //float duration = 1f;
    //float bounceAmount;

    // 화면비율에 따른 위치 보정용
    float screenWScale;

    Camera EventCamera;
    void Awake()
    {
        defaultFontSize = digitLbl.fontSize;

        screenWScale = (float)((float)1280 / Screen.width);
    }


    public void Show(GameObject _targetGO, GameObject attacker, int damage, bool isMyUnit, bool isCritical, bool isHeal = false, eDamResistType DamResist = eDamResistType.None )
    {
        owner = _targetGO;
        gameObject.SetActive( true );
        
        criticalSpr.enabled = isCritical;
        digitLbl.fontSize = (isCritical) ? (int)((float)defaultFontSize * 1.5f) : defaultFontSize;
        digitLbl.text = damage.ToString();

        DamInfoSprite.gameObject.SetActive(DamResist == eDamResistType.None && !isCritical /*&& _AttriButeType != eAttriButeType.NormalAttriBute*/);

        //< 대미지 감소상태라면 회색!
        if (DamResist != eDamResistType.None)
        {
            digitLbl.color = new Color(168 / 255f, 84 / 255f, 255 / 255f, 1f);
            digitLbl.text = damage.ToString();
        }
        else
        {
            if (isCritical)
            {
                digitLbl.color = Color.yellow;
            }
            else if (isHeal)
            {
                digitLbl.color = Color.green;
            }
            else
            {
                if (!isMyUnit)
                    digitLbl.color = Color.white;
                else
                    digitLbl.color = Color.red;

            }
            /*
            //< PVP는 예외처리
            if (G_GameInfo.GameMode == GAME_MODE.PVP)
            {
                if (isCritical)
                {
                    digitLbl.color = Color.yellow;
                }
                else if (isHeal)
                {
                    digitLbl.color = Color.green;
                }
                else
                {
                    if (owner.GetComponent<Unit>().TeamID == 0)
                        digitLbl.color = Color.red;
                    else
                        digitLbl.color = Color.white;

                    if (_AttriButeType == eAttriButeType.WinAttriBute)
                    {
                        DamInfoSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                        DamInfoSprite.color = WinAttriButeColor;
                    }
                    else if (_AttriButeType == eAttriButeType.LoseAttriBute)
                    {
                        DamInfoSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                        DamInfoSprite.color = LoseAttriButeColor;
                    }
                }
            }
            else
            {
                Unit unit = owner.GetComponent<Unit>();
                if (null != unit)
                {
                    switch (unit.UnitType)
                    {
                        case UnitType.Unit:
                        case UnitType.Npc:
                        case UnitType.Boss:
                        case UnitType.Prop:
                            {
                                if (isCritical)
                                {
                                    digitLbl.color = Color.yellow;
                                }
                                else if (isHeal)
                                {
                                    digitLbl.color = Color.green;
                                }
                                else
                                {
                                    if (attacker.GetComponent<Unit>().UnitType == UnitType.Unit)
                                        digitLbl.color = Color.white;
                                    else
                                        digitLbl.color = Color.red;

                                    if (_AttriButeType == eAttriButeType.WinAttriBute)
                                    {
                                        DamInfoSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                                        DamInfoSprite.color = WinAttriButeColor;
                                    }
                                    else if (_AttriButeType == eAttriButeType.LoseAttriBute)
                                    {
                                        DamInfoSprite.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                                        DamInfoSprite.color = LoseAttriButeColor;
                                    }
                                }

                            }
                            break;

                        default:
                            digitLbl.color = Color.white;
                            break;
                    }
                }
            }
            */
        }

        factor = 0;
        tweenAlpha.ResetToBeginning();
        tweenAlpha.PlayForward();
        tweenScale.ResetToBeginning();
        tweenScale.PlayForward();

        int RanX = Random.Range(0, 600);

        if (TownState.TownActive)
            randomX += (float)(RanX * 0.01f);
        else
            randomX += (float)(RanX * 0.02f);

        if (Random.Range(0, 2) != 0)
            randomX *= -1;

        // 시작, 끝점 설정
        transform.localPosition = GetUIPos();

        //< 처음 시작위치 저장
        StartOwnelPos = owner.transform.position;
        Update();
    }

    float randomX = 0;
    void Update()
    {
        if (null == owner)
        {
            gameObject.SetActive( false );
            return;
        }

        Vector3 newPos = GetUIPos() + (Vector3.up * factor);
        newPos.x += randomX;
        factor += (80 + (factor * 2)) * Time.deltaTime;
        
        transform.localPosition = newPos;
        if(factor >= 120)
            gameObject.SetActive(false);

        //Vector3 newPos = Vector3.Lerp( startPos, startPos + endPos, 1f - Mathf.Sin( 0.5f * Mathf.PI * (1f - factor) ) );
        //newPos.y += Bounce(factor) * bounceAmount;
        //transform.localPosition = newPos;
        
        //if (factor >= 1f)
        //{
        //    gameObject.SetActive( false );
        //}

        //factor += (1f / duration) * Time.deltaTime;
        //factor = Mathf.Clamp01( factor );
    }

    Vector3 StartOwnelPos;
    Vector3 GetUIPos()
    {
        //< 마을에서 하는거라면 다른 방법을 찾아야함
        //if (TownState.TownActive)
        //    return (CollectionInfoPanel._EventCamera.WorldToScreenPoint(StartOwnelPos + offset) - MathHelper.UI_Center_Pos) * screenWScale;

        return MathHelper.WorldToUIPosition(StartOwnelPos + offset) * screenWScale;
    }

    float Bounce(float t)
    {
        return Mathf.Sin(t * (Mathf.PI - 0.5f));
    }
}
