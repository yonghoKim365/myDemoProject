using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class CameraManager : Immortal<CameraManager>
{
    /*
     *  카메라 구성
     *      - Main : 주 카메라 설정
     *      - RtsCamera
     *      - CameraShake
     * 
     * + 씬에 맞는 카메라 셋팅
     * + 카메라의 더미 부모로 Transform 이동, 회전 조절하기. 카메라 자체는 흔들기용으로 사용.
     * 
     * 
     */

    // RtsCamera에 의해 조절될 Transform
    private Transform       proxyTrans;
    public RtsCamera        RtsCamera;
    public RtsCameraMouse   rtsCameraMouse;
    public RtsCameraKeys    rtsCameraKeys;

    // 실 카메라에 부착된 쉐이커
    public Thinksquirrel.Utilities.CameraShake Shaker, SkillEventShaker, cutSceneCamShaker;
    public Dictionary<byte, CameraShakeData>   shakeDataDic;

    public Camera mainCamera;

    //public XRayCamera XRayComponent;
    private GameObject TouchEff;
    private GameObject TownCamEff;

    protected override void Init()
    {
        base.Init();

        if (base.gameObject == null)
            return;

        proxyTrans = transform.FindChild( "TransformProxy" );
        if (null == proxyTrans)
        {
            Debug.LogError( "you must have TransformProxy Object in CameraManager." );
            return;
        }

        RtsCamera = proxyTrans.GetComponent<RtsCamera>();
        if (null == RtsCamera)
        { 
            RtsCamera = proxyTrans.gameObject.AddComponent<RtsCamera>();
            rtsCameraMouse = proxyTrans.gameObject.AddComponent<RtsCameraMouse>();
            rtsCameraKeys = proxyTrans.gameObject.AddComponent<RtsCameraKeys>();
        }
        else
        { 
            rtsCameraMouse = proxyTrans.GetComponent<RtsCameraMouse>();
            rtsCameraKeys = proxyTrans.GetComponent<RtsCameraKeys>();
        }

        mainCamera = proxyTrans.GetComponentInChildren<Camera>();
        Shaker = mainCamera.GetComponent<Thinksquirrel.Utilities.CameraShake>();
        if (null == Shaker)
            Shaker = mainCamera.gameObject.AddComponent<Thinksquirrel.Utilities.CameraShake>();

        if (transform.FindChild("ClickEffect") != null)
            Destroy(transform.FindChild("ClickEffect"));

        GameObject effParent = new GameObject("ClickEffect");
        effParent.transform.parent = transform;
        effParent.transform.localPosition = Vector3.zero;
        effParent.transform.localScale = Vector3.one;
        effParent.layer = LayerMask.NameToLayer("UILayer");

        TouchEff = UIHelper.CreateEffectInGame(effParent.transform, "Fx_UI_click_01", true);

        //ActiveCamEff(false);

        InitData();
    }

    protected void Start()
    {
        StartCoroutine( KeepInternalCameraTarget() );
    }

    void ClearSceneSetting()
    {
    }

    public void SceneInit()
    {
        ClearSceneSetting();

        RtsCamera.ResetToInitialValues( false, false );

        // 새로운 씬 입장시, 쉐이크 중이었던 것을 모두 꺼야, 간헐적 버그 방지
        Thinksquirrel.Utilities.CameraShake.CancelAllShakes();
    }

    //public void ActiveCamEff(bool isActive)
    //{
    //    if(isActive)
    //    {
    //        if (TownCamEff == null)
    //        {
    //            TownCamEff = UIHelper.CreateEffectInGame(mainCamera.transform, "Fx_UI_cam_leaf", false);
    //            TownCamEff.transform.localPosition = new Vector3(1.08f, 2.14f, 2.58f);
    //            TownCamEff.transform.localEulerAngles = new Vector3(30.45f, 178.8f, -32.45f);
    //        }

    //        ColorCorrectionCurves curves = mainCamera.gameObject.GetComponent<ColorCorrectionCurves>();
    //        if (curves != null)
    //            curves.enabled = true;

    //        FastBloom bloom = mainCamera.gameObject.GetComponent<FastBloom>();
    //        if (bloom != null)
    //            bloom.enabled = true;
    //    }
    //    else
    //    {
    //        if (TownCamEff != null)
    //            Destroy(TownCamEff);

    //        ColorCorrectionCurves curves = mainCamera.gameObject.GetComponent<ColorCorrectionCurves>();
    //        if (curves != null)
    //            curves.enabled = false;

    //        FastBloom bloom = mainCamera.gameObject.GetComponent<FastBloom>();
    //        if (bloom != null)
    //            bloom.enabled = false;
    //    }
    //}

    IEnumerator KeepInternalCameraTarget()
    {
        while (true)
        { 
            if (null != RtsCamera && null != RtsCamera.CameraTarget)
            { 
                DontDestroyOnLoad( RtsCamera.CameraTarget.gameObject );
                yield break;
            }

            yield return null;
        }
    }

    public void InitData()
    {
        shakeDataDic = new Dictionary<byte, CameraShakeData>();
        shakeDataDic.Add( 0, (ResourceMgr.Load("Camera/CameraShakeSet0") as GameObject).GetComponent<CameraShakeData>() );
        shakeDataDic.Add( 1, (ResourceMgr.Load("Camera/CameraShakeSet1") as GameObject).GetComponent<CameraShakeData>() );
        shakeDataDic.Add( 2, (ResourceMgr.Load("Camera/CameraShakeSet2") as GameObject).GetComponent<CameraShakeData>() );
        shakeDataDic.Add( 3, (ResourceMgr.Load("Camera/CameraShakeSet3") as GameObject).GetComponent<CameraShakeData>() ); 
    }

    public void SetSkillEventCamera(GameObject center)
    {
        SkillEventShaker = center.GetComponent<Thinksquirrel.Utilities.CameraShake>();
        if(SkillEventShaker == null)
            SkillEventShaker = center.AddComponent<Thinksquirrel.Utilities.CameraShake>();
    }

    public void EnableRPGCamera(bool enable = true)
    {
        RtsCamera.enabled = enable;
        if (null != rtsCameraMouse)
            rtsCameraMouse.enabled = false;
        if (null != rtsCameraKeys)
            rtsCameraKeys.enabled = false;
        
        // TODO : 수정되어야할 코드. 터치전용으로만 자동하는 카메라 클래스 제작필요
        //CameraMouseZoom mouseZoom = mainCamera.GetComponent<CameraMouseZoom>();
        //if (null != mouseZoom)
        //{ 
        //    mouseZoom.enabled = !enable;
        //}

        //if (enable)
        //{
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;
            mainCamera.transform.localScale = Vector3.one;
        //}
        //else
        //{
        //    mouseZoom.SetInitPos();
            
        //    proxyTrans.localPosition = Vector3.zero;
        //    proxyTrans.localRotation = Quaternion.identity;
        //    proxyTrans.localScale = Vector3.one;
        //}
    }

    /// <summary>
    /// 카메라가 대상을 바라보면서 따라다니도록 한다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="snap"></param>
    public void Follow(Transform target, bool snap = false)
    {
        if (null != RtsCamera)
            RtsCamera.Follow( target, snap );

        //Debug.Log( "Follow() : " + RtsCamera.FollowTarget, transform );
    }

    public void ShakeDefault()
    {
        //Debug.Log( "ShakeDefault()", transform );

        if (SkillEventMgr.ActiveEvent)
            SkillEventShaker.Shake();
        else
            Shaker.Shake();
    }

    public void ShakeClose()
    {
        if (SkillEventShaker == null)
            return;
        
        if (SkillEventMgr.ActiveEvent)
            SkillEventShaker.CancelShake();
        else
            Shaker.CancelShake();
    }

    public void Shake(Vector3 shakeAmount, byte shakeType = 1, System.Action callback = null, int numberOfShakes = 0)
    {
        if (SkillEventMgr.ActiveEvent && (SkillEventShaker == null || SkillEventShaker.gameObject == null || !SkillEventShaker.gameObject.activeSelf))
            return;

        if (!SkillEventMgr.ActiveEvent && (Shaker == null || Shaker.gameObject == null || !Shaker.gameObject.activeSelf))
            return;

        CameraShakeData data;
        if (shakeDataDic.TryGetValue( shakeType, out data ))
        {
            if (SkillEventMgr.ActiveEvent && SkillEventShaker.gameObject.activeSelf)
                SkillEventShaker.Shake(data.shakeType, numberOfShakes != 0 ? numberOfShakes : data.numberOfShakes, shakeAmount, data.rotationAmount, data.distance, data.speed, data.decay, 0, data.multiplyByTimeScale);
            else if (Camera.main != null && Camera.main.gameObject.activeSelf)
                Shaker.Shake(data.shakeType, numberOfShakes != 0 ? numberOfShakes : data.numberOfShakes, shakeAmount, data.rotationAmount, data.distance, data.speed, data.decay, 0, data.multiplyByTimeScale);

        }
        else
            ShakeDefault();
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0) )
        {
            if (UICamera.currentCamera != null)
            {
                Vector3 touchPos = Vector3.zero;

                if (0 < Input.touchCount)
                {
                    Touch touch = Input.GetTouch(0);
                    touchPos = UICamera.currentCamera.ScreenToWorldPoint(touch.position);
                }
                else
                    touchPos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
                
                TouchEff.SetActive(false);
                TouchEff.transform.position = touchPos;
                TouchEff.SetActive(true);
            }
        }
    }

	public void cutSceneCamShake(){

		if (cutSceneCamShaker != null) {
//			CameraShakeData data = new CameraShakeData();
//			data.shakeType = Thinksquirrel.Utilities.CameraShake.ShakeType.LocalPosition;
//			data.numberOfShakes = 2;
//			data.shakeAmount = new Vector3(1,1,1);
//			data.rotationAmount = Vector3.zero;
//			data.distance = 0.1f;
//			data.speed = 150;
//			data.decay = 0.2f;
//			data.multiplyByTimeScale = true;
			//public void Shake(ShakeType shakeType, int numberOfShakes, Vector3 shakeAmount, Vector3 rotationAmount, float distance, float speed, float decay, float uiShakeModifier, bool multiplyByTimeScale)
			cutSceneCamShaker.Shake(Thinksquirrel.Utilities.CameraShake.ShakeType.CameraMatrix, 2, new Vector3(1,1,1), Vector3.zero,
			                        0.1f, 150f, 0.2f, 0, true);
		}
	}

	public void SetGrayScale(bool b){
		mainCamera.GetComponent<GrayscaleEffect> ().enabled = b;
	}
}
