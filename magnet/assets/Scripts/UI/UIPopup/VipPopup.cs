using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VipPopup : UIBasePanel
{
    public UILabel ExpLabel;    //현재렙업위한 경험치
    public UILabel[] vipLvLabel;//상단에 현재렙과 다음렙표시 
    public UILabel[] VipBenefitLabel;// 등급설명쪽 라벨 

    public UISprite ExpBar; //vip 렙업경험치바

    public UIButton[] Arrows; //뒤,앞 화살표
    public UIButton BtnPay; //결제버튼
    //public UIButton BtnClose;

    public UIScrollView Scroll;
    public UIGrid Grid;
    public GameObject SlotPrefab;

    public Transform RewardItemRoot;    // 아이템부모

    // 특권에서 type 에따라 변환되야할 스트링커먼 인덱스들(1->501 , 2-> 503..)
    private uint[] VipSpecialLowIndx = { 503, 504, 505, 506, 507, 508, 509, 510, 511, 512 };
    private List<Vip.VipDataInfo> VipInfo = new List<Vip.VipDataInfo>();
    private uint Viplv;

    public override void Init()
    {
        base.Init();
       // EventDelegate.Set(BtnClose.onClick, Close);

        for (int i = 0; i < 10; i++)
        {
            GameObject slot = Instantiate(SlotPrefab) as GameObject;
            slot.transform.parent = Grid.transform;
            slot.transform.localPosition = Vector3.zero;
            slot.transform.localScale = Vector3.one;
            slot.SetActive(false);
        }
        Destroy(SlotPrefab);

        NetworkClient.instance.SendPMsgVipQueryInfoC();

        EventDelegate.Set(Arrows[0].onClick, delegate () { OnclickInfoPopup(Viplv - 1); });
        EventDelegate.Set(Arrows[1].onClick, delegate () { OnclickInfoPopup(Viplv + 1); });

        EventDelegate.Set(BtnPay.onClick, 
            delegate ()  {
                //UIMgr.instance.AddPopup(18, null, null, null);
                uiMgr.AddPopup(141, 174, 117);
            });

    }

    void OnclickInfoPopup(uint lv)
    {
        Viplv = lv;
        List<Vip.VipDataInfo> VipInfo = _LowDataMgr.instance.GetLowDataVipData(lv);

        CurBenefit(VipInfo);


    }
    public void VipPopUpSetting(uint lv, uint exp, uint time, uint cnt)
    {
        Viplv = lv;
        vipLvLabel[0].text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), Viplv);
        vipLvLabel[1].text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), Viplv + 1);

        Arrows[0].transform.gameObject.SetActive(lv == 0 ? false : true);
        Arrows[1].transform.gameObject.SetActive(lv == 15 ? false : true);

        if (lv < 15)
        {
            Vip.VipLevelInfo lvInfo = _LowDataMgr.instance.GetLowDataVipLevel(lv + 2);
            ExpLabel.text = string.Format("{0}/{1}", exp, lvInfo.needexp);
            float expAmount = (float)exp / (float)lvInfo.needexp;
            ExpBar.fillAmount = expAmount;
        }

        //현재특권을 적어준다
        List<Vip.VipDataInfo> info = _LowDataMgr.instance.GetLowDataVipData(lv);

        VipInfo = info;

        CurBenefit(VipInfo);

    }

    // 현재 레벨의 보상을 보여준다
    public void CurBenefit(List<Vip.VipDataInfo> VipInfo)
    {

        //uint lv = VipInfo[0].VipGrade;
        //Arrows[0].transform.gameObject.SetActive(lv == 0 ? false : true);
        //Arrows[1].transform.gameObject.SetActive(lv == 15 ? false : true);

        Arrows[0].transform.gameObject.SetActive(Viplv == 0 ? false : true);
        Arrows[1].transform.gameObject.SetActive(Viplv == 15 ? false : true);


        VipBenefitLabel[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(501), Viplv);
        VipBenefitLabel[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(499), Viplv);

        List<Vip.VipDataInfo> itemData = new List<Vip.VipDataInfo>();
        // 주요 혜택아이콘을 보여준다 (type 3(소탕권), type 8(칭호), type 9(결석채우기쿠폰)
        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            GameObject go = Grid.transform.GetChild(i).gameObject;
            UILabel label = go.GetComponent<UILabel>();
            if (i >= VipInfo.Count)
            {
                go.SetActive(false);
                continue;
            }

            if (VipInfo[i].type == 3 || VipInfo[i].type == 8 || VipInfo[i].type == 9)
            {
                itemData.Add(VipInfo[i]);
            }

            //특권
            // 5 , 7 , 8 제외하고는 대입할값{0}이존재
            if (VipInfo[i].type != 5 || VipInfo[i].type != 6 || VipInfo[i].type != 7 || VipInfo[i].type != 8)
            // 수치도 같이 넣어줘요함
            {
                // 6번타입은 컨텐츠 보상 확률이 다르다
                if (VipInfo[i].type == 6)
                {
                    // 얘는 맵테이블에서 가지고와야함.
                    uint[] mapStr = { 532, 284, 12, 916, 917, 720, 823, 194, 283 };//스트링테이블아이디 (마을, 경험치던전, 난투장, 시나리오던전,보스레이드,콜로세움, 차관, 마탑, 골드던전)
                    uint strIdx = VipInfo[i].Typevalue - 1;
                    label.text = string.Format(_LowDataMgr.instance.GetStringCommon(VipSpecialLowIndx[VipInfo[i].type - 1]), _LowDataMgr.instance.GetStringCommon(mapStr[strIdx]));
                }
                else
                    // 아닌경우는 info의 밸류값
                    label.text = string.Format(_LowDataMgr.instance.GetStringCommon(VipSpecialLowIndx[VipInfo[i].type - 1]), VipInfo[i].Typevalue);

            }
            else
            {
                label.text = string.Format(_LowDataMgr.instance.GetStringCommon(VipSpecialLowIndx[VipInfo[i].type]));
            }

            go.SetActive(true);
        }

        Grid.Reposition();

        for (int i = 0; i < RewardItemRoot.childCount; i++)
        {

            GameObject go = RewardItemRoot.GetChild(i).gameObject;
            UISprite icon = go.transform.FindChild("icon").GetComponent<UISprite>();
            if (i >= itemData.Count)
            {
                icon.gameObject.SetActive(false);
            }
            else
            {
                switch (itemData[i].type)
                {
                    case 3: //소탕권
                        icon.spriteName = "Icon_10003";
                        break;
                    case 8: //칭호
                        icon.spriteName = "Icon_10010";
                        break;
                    case 9://출석채우기
                        icon.spriteName = "Icon_10011";
                        break;
                }
                icon.gameObject.SetActive(true);
            }
        }

    }

    public override void Close()
    {

        base.Close();
        UIMgr.OpenTown();
    }
}






   //void CheckChild(GameObject Allobject[i])
   // {
   //     List<Renderer> ChildRender = new List<Renderer>();
   //     //자식도 있다면 찾아서바꿔줘...
   //     if (AllObject[i].gameObject.transform.childCount >= 1)
   //     {
   //         for (int k = 0; k < AllObject[i].transform.childCount; k++)
   //         {
   //             if (AllObject[i].transform.GetChild(k).GetComponent<MeshRenderer>() == null)
   //             {
   //                 if (AllObject[i].transform.GetChild(k).GetComponent<SkinnedMeshRenderer>() != null)
   //                     ChildRender.Add(AllObject[i].transform.GetChild(k).GetComponent<SkinnedMeshRenderer>());
   //             }
   //             else
   //             {
   //                 ChildRender.Add(AllObject[i].transform.GetChild(k).GetComponent<MeshRenderer>());
   //             }
   //         }

   //         for (int j = 0; j < ChildRender.Count; j++)
   //         {
   //             Material[] mat = ChildRender[j].materials;
   //             int matCount = mat.Length;
   //             for (int k = 0; k < matCount; k++)
   //             {
   //                 if (mat[k] == null)
   //                     continue;
   //                 if (mat[k].shader.name == newShaderName)
   //                     continue;
   //                 // TransParenCutOut은 제외시킨다
   //                 if (mat[k].shader.name == TransCutOut)
   //                     continue;
   //                 // TransParenDiffuse은 제외시킨다
   //                 if (mat[k].shader.name == TransDiffuse)
   //                     continue;

   //                 Texture MainTex = mat[k].mainTexture;
   //                 mat[k].shader = Shader.Find(newShaderName);
   //                 mat[k].SetTexture(propertyName, MainTex);
   //                 // 컬러값 1,1,1,1
   //                 Color col = mat[k].color;
   //                 col.r = 1f;
   //                 col.g = 1f;
   //                 col.b = 1f;
   //                 mat[k].color = col;
   //             }

   //         }



   //     }







