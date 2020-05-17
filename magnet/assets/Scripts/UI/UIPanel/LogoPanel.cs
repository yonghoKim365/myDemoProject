using UnityEngine;
using System.Collections;

public class LogoPanel : UIBasePanel {
    //public GameObject LogoGo;
    //public System.Action EndCallBack;
    //public UILabel CheckLoadText;
    //public UILabel LoadingStr;
    //public UISlider Slider;
    public UISprite WhiteSp;

    //public Transform Indicator;
    public UIPanel Guide;    //건전게임문구

    public AudioClip LogoSound;

    public UIPanel CIPanel;
    public UITexture CIMagnet;
    public UITexture CIQuwan;

    public float AlphaSpeed = 0.1f;
    public float AlphaDelay = 3f;

    //private bool IsLogoStay;

    public override void Init()
    {
        base.Init();
        CIPanel.alpha = 1;
        CIMagnet.alpha = 0;
        CIQuwan.alpha = 0;

        CIPanel.gameObject.SetActive(true);
        //LogoGo.SetActive(false);
        Guide.gameObject.SetActive(true); 
        transform.FindChild("Guide/Label").GetComponent<UILabel>().text = SystemDefine.LocalGuide;
    }

    public override void LateInit()
    {
        base.LateInit();

        StartCoroutine("CIAction");
    }

    IEnumerator CIAction()
    {
        yield return new WaitForSeconds(0.1f);

        //NGUITools.PlaySound(LogoSound);
        SoundManager.instance.PlayUIClip(LogoSound);
        float alpha = 0;
        
        //취완 마크 먼저 보여야한다.
        while (true)
        {
            alpha += (AlphaSpeed * Time.deltaTime);
            CIQuwan.alpha = alpha;
            if (1 <= alpha)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(0.4f);
        CIQuwan.gameObject.SetActive(false);
        
        alpha = 0f;
        //마그넷마크
        while (true)
        {
            alpha += (AlphaSpeed * Time.deltaTime);
            CIMagnet.alpha = alpha;
            if (1 <= alpha)
                break;

            yield return null;
        }
        yield return new WaitForSeconds(0.4f);

        while (true)
        {
            alpha -= (AlphaSpeed * Time.deltaTime);
            CIPanel.alpha = alpha;
            if (alpha <= 0)
                break;

            yield return null;
        }
        
        System.DateTime time = System.DateTime.Now.AddSeconds( 2.5f);
        //IsLogoStay = true;

        yield return NetworkClient.instance.InitializeNetworkClient();

        //float delay = 0f;
        //while (IsLogoStay)//LoginState 로딩되기까지 대기하는 곳
        //{
        //    yield return new WaitForSeconds(0.05f);
        //    //delay += Time.deltaTime + 0.05f;
        //}

        CIPanel.gameObject.SetActive(false);

        // 건전게임문구 추가
        alpha = 1;

		// use asset bundle
		//yield return StartCoroutine (LoadTextAssets ());
        
        float delayTime = (time-System.DateTime.Now).Seconds;

		SceneManager.instance.sw.Start ();

        SceneManager.instance.ActionEvent(_ACTION.GO_NEXT);
        yield return new WaitForSeconds(delayTime);//3초에서 로딩 하는 시간 제외하고 나머지

        while (true)
        {
            alpha -= (AlphaSpeed * Time.deltaTime);
            Guide.alpha = alpha;
            
            if (alpha <= 0)
                break;

            yield return null;

        }
        Guide.gameObject.SetActive(false);

  //      LogoGo.SetActive(true);

		//CheckLoadText.text = string.Format("加载 0%");

        //StartCoroutine(waitingPanelUpdate());
        yield return null;
    }

	IEnumerator LoadTextAssets(){

		//System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();

		//sw.Start ();

		yield return StartCoroutine (_LowDataMgr.instance.LoadAssetBundle ());

		//UnityEngine.Debug.Log (" LoadAsset, time1:" + sw.ElapsedMilliseconds / 1000f); // 1.1 sec in device
		
		_LowDataMgr.instance.DeserializeBigLowDatas ();

		//UnityEngine.Debug.Log (" LoadAsset, time2:" + sw.ElapsedMilliseconds / 1000f); // 8.1 sec in device

		//sw.Stop ();
	}

    /*
    IEnumerator waitingPanelUpdate()
    {
        yield return null;

        float startPos = -595;
        float barSize = Slider.foregroundWidget.localSize.x;

		Slider.value = 0;
		Slider.ForceUpdate ();

        _LowDataMgr.instance.LoadLowDataAllData((ratio, desc) =>
        {
            CheckLoadText.text = string.Format("加载 {0}%", (ratio * 100f).ToString("N0"));//100f
            Slider.value = ratio;
            
			if (Indicator != null){
            	Indicator.localPosition = new Vector3(ratio * barSize + startPos, Indicator.localPosition.y , Indicator.localPosition.z);
			}
            if (ratio == 1f)
            {
                //BenchMark.Mark("LOADING_END");
                //BenchMark.BenchTime("LOADING_START", "LOADING_END");

				if (CheckLoadText != null){
                	CheckLoadText.transform.parent.gameObject.SetActive(false);
				}
				if (LoadingStr != null){
                	LoadingStr.text = _LowDataMgr.instance.GetStringCommon(538);
				}

                EndCallBack();
            }
        });

    }
    */
    public override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
    }

    //public void EndLogoStay()
    //{
    //    IsLogoStay = false;
    //}
}
