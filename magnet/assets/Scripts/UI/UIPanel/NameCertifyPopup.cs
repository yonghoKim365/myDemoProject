using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

public class NameCertifyPopup : UIBasePanel {

    public GameObject CheckPopup;//에러 팝업
    public GameObject SendCertify;//버튼

    public UIInput InputName;//유저 이름
    public UIInput InputNumber;//유저 주민

    public UILabel CertifyState;//인증 결과
    public UILabel CheckMsg;//에러 팝업
    public UILabel SendCertifyName;//인증 받기 or 다시 인증

    public bool IsSend;
    
    public override void Init()
    {
        base.Init();

        EventDelegate.Set(transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, Close);

        EventDelegate.Set(CheckPopup.transform.FindChild("Btn").GetComponent<UIEventTrigger>().onClick, delegate () {
            CheckPopup.SetActive(false);
        });

        InputName.enabled = false;
        InputNumber.enabled = false;
        SendCertify.SetActive(false);
        
        NetworkClient.instance.SendPMsgUserCertifyInfoC();
    }

    public void OnCertify(int certifyState, string name, string number)
    {
        if (certifyState != (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_SUCCESS)
        {
            int state = PlayerPrefs.GetInt(string.Format("CertifyState{0}", NetData.instance.UUID), (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_NULL);
            if (state == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_NULL)//미인증 상태
            {
                SetCertifyInfo(state, "", "");
            }
            else//뭔가 다른 상태였다는것
            {
                string byteUserName = PlayerPrefs.GetString(string.Format("CertifyUser{0}", NetData.instance.UUID), "");
                number = PlayerPrefs.GetString(string.Format("CertifyNumber{0}", NetData.instance.UUID), "");

                string userName = "";
                if (!string.IsNullOrEmpty(byteUserName) && byteUserName.Contains("|"))
                {
                    string[] split = byteUserName.Split('|');
                    byte[] bStrByte = new byte[split.Length];
                    for (int i = 0; i < split.Length; i++)
                    {
                        bStrByte[i] = byte.Parse(split[i]);
                    }

                    userName = Encoding.UTF8.GetString(bStrByte); // byte -> string
                }

                SetCertifyInfo(state, userName, number);
            }
        }
        else
        {
            SetCertifyInfo(certifyState, name, number);
        }
    }

    void SetCertifyInfo(int certifyState, string name, string number)
    {
        if (!string.IsNullOrEmpty(number))
        {
            string sub = "", newStr = "";
            if (4 < number.Length)
            {
                sub = number.Substring(2, number.Length - 4);
            }
            else
            {
                sub = number.Substring(0, number.Length);
            }

            for (int i = 0; i < sub.Length; i++)
            {
                newStr += "*";
            }

            number = number.Replace(sub, newStr);
        }

        if (!string.IsNullOrEmpty(name))
        {
            string sub = "", newStr = "";
            if (1 < name.Length)
                sub = name.Substring(0, name.Length - 1);
            else
                sub = name.Substring(0, name.Length);
            
            for (int i = 0; i < sub.Length; i++)
            {
                newStr += "*";
            }

            name = name.Replace(sub, newStr);
        }

        InputNumber.label.text = InputNumber.value = number;
        InputName.label.text = InputName.value = name;

        uint commonId = 0;
        if (certifyState == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_NULL)//미인증
            commonId = 930;
        else if (certifyState == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_REVIEW)//심사 중
            commonId = 931;
        else if (certifyState == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_FAIL)//인증 실패
            commonId = 933;
        else//인증 완료
            commonId = 932;
        
        CertifyState.text = _LowDataMgr.instance.GetStringCommon(commonId);

        if (certifyState == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_NULL || certifyState == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_FAIL)//미인증 이거나 인증 실패시 버튼 출력
        {
            InputName.enabled = true;
            InputNumber.enabled = true;

            SendCertify.SetActive(true);
            SendCertifyName.text = _LowDataMgr.instance.GetStringCommon((uint)(certifyState == 0 ? 943 : 942));
            EventDelegate.Set(SendCertify.GetComponent<UIEventTrigger>().onClick, OnClickCertify);
        }
    }

    public override void Close()
    {
        if (CheckPopup.activeSelf)
            CheckPopup.SetActive(false);
        else
        {
            base.Close();
            UIMgr.OpenTown();
        }
    }
    
    public void SetCheckPopup(uint commonId)
    {
        CheckMsg.text = _LowDataMgr.instance.GetStringCommon(commonId);
        CheckPopup.SetActive(true);
    }

    void OnClickCertify()
    {
        if (IsSend)
            return;

        int state = (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_FAIL;
        if (string.IsNullOrEmpty(InputNumber.value))//주민번호 입력 안함
        {
            SetCheckPopup(936);
        }
        else if (InputNumber.value.Length != 18)//주민 번호 개수 안맞음
        {
            SetCheckPopup(938);
        }
        else if (string.IsNullOrEmpty(InputName.value) )//이름 입력 안함
        {
            SetCheckPopup(937);
        }
        else if (InputName.value.Length < 2)//이름 입력 최소 개수 안맞음 2자 이상임
        {
            SetCheckPopup(939);
        }
        else if (!IsChinese(InputName.value) || Regex.IsMatch(InputName.value, @"\d"))
        {
            SetCheckPopup(939); // error. not chiness
        }
        else
            state = (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_REVIEW;
        
        string number = InputNumber.value, userName = InputName.value;
        
        if (state == (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_REVIEW )
        {
            IsSend = true;
            CertifyState.text = _LowDataMgr.instance.GetStringCommon(931);
            NetworkClient.instance.SendPMsgUserCertifySetC(userName, number);
        }
        else
        {
            CertifyState.text = _LowDataMgr.instance.GetStringCommon(933);//실패
        }

        string byteUserName = "";
        if (!string.IsNullOrEmpty(userName))
        {
            byte[] bStrByte = Encoding.UTF8.GetBytes(userName); // string -> byte
            byteUserName = bStrByte[0].ToString();
            for (int i = 1; i < bStrByte.Length; i++)
            {
                byteUserName = string.Format("{0}|{1}", byteUserName, bStrByte[i]);
            }

            string sub = "", newStr = "";
            if (1 < userName.Length)
                sub = userName.Substring(0, userName.Length - 1);
            else
                sub = userName.Substring(0, userName.Length);

            for (int i = 0; i < sub.Length; i++)
            {
                newStr += "*";
            }

            userName = userName.Replace(sub, newStr);
            InputName.label.text = InputName.value = userName;
        }
        
        PlayerPrefs.SetInt(string.Format("CertifyState{0}", NetData.instance.UUID), state);
        PlayerPrefs.SetString(string.Format("CertifyNumber{0}", NetData.instance.UUID), number);
        PlayerPrefs.SetString(string.Format("CertifyUser{0}", NetData.instance.UUID), byteUserName);

        if( !string.IsNullOrEmpty(number) )
        {
            string sub = "", newStr = "";
            if(4 < number.Length )
            {
                sub = number.Substring(2, number.Length - 4);
            }
            else
            {
                sub = number.Substring(0, number.Length);
            }

            for (int i = 0; i < sub.Length; i++)
            {
                newStr += "*";
            }

            number = number.Replace(sub, newStr);
            InputNumber.label.text = InputNumber.value = number;
        }
    }
    
    public void OnSendCertify(string name, string number)
    {
        CertifyState.text = _LowDataMgr.instance.GetStringCommon(932);//성공

        InputName.enabled = false;
        InputNumber.enabled = false;

        SendCertify.SetActive(false);

        PlayerPrefs.SetInt(string.Format("CertifyState{0}", NetData.instance.UUID), (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_SUCCESS);
    }

    public void OnError()
    {
        IsSend = false;
        CertifyState.text = _LowDataMgr.instance.GetStringCommon(933);//인증 실패
        SendCertifyName.text = _LowDataMgr.instance.GetStringCommon(942);

        PlayerPrefs.SetInt(string.Format("CertifyState{0}", NetData.instance.UUID), (int)Sw.USER_CERTIFY_STATUS.USER_CERTIFY_STATUS_FAIL);
    }

    public bool IsChinese(string text)
    {
        //return text.Any(c => (uint)c >= 0x4E00 && (uint)c <= 0x2FA1F);
        return text.Any(c =>
               (c >= 0x4E00 && c <= 0x9FFF) ||
               (c >= 0x3400 && c <= 0x4DBF) ||
               (c >= 0x3400 && c <= 0x4DBF) ||
               (c >= 0x20000 && c <= 0x2CEAF) ||
               (c >= 0x2E80 && c <= 0x31EF) ||
               (c >= 0xF900 && c <= 0xFAFF) ||
               (c >= 0xFE30 && c <= 0xFE4F) ||
               (c >= 0xF2800 && c <= 0x2FA1F)
        );
    }
    
}
