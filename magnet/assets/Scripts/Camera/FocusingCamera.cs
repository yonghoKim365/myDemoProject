using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thinksquirrel.Utilities;

[RequireComponent(typeof(Camera))]
public class FocusingCamera : MonoBehaviour 
{
    [System.Serializable]
    public class FocusingGOData
    {
        public LayerMask    SavedLayer;
        public GameObject   Object;
    }

    // 메인 카메라
    public Camera TargetCamera;
    
    // 포커싱 전용 레이어
    public LayerMask    FocusLayer;
    public int          FocusLayerValue;

    // 카메라용 반투명 가리개
    public GameObject CoverPrefab, SkillCoverPrefab;
    private GameObject coverObj, SkillcoverObj;
    private MeshRenderer coverMeshRender, SkillcoverMeshRender;
    public Material CoverMaterial { get { return coverMeshRender.materials[0]; } }
    public Material SkillCoverMaterial { get { return SkillcoverMeshRender.materials[0]; } }

    // Inspector에서 확인용으로 public으로 설정함.
    public List<FocusingGOData> TargetGOList = new List<FocusingGOData>();

    public Camera Mycamera;
    readonly int DepthStep = 1;

	private ScreenOverlay someJS;
	private float val = 1.0f;
	bool m_MultiShake;
	private GameObject StartCam;
	public Texture2D[] CameraFXs;


    void Awake()
    {
        if (null != CoverPrefab)
            coverObj = Instantiate( CoverPrefab ) as GameObject;

        if (SkillCoverPrefab != null)
            SkillcoverObj = Instantiate(SkillCoverPrefab) as GameObject;

        if (null != coverObj)
        { 
            coverObj.transform.AttachTo( transform, false );
            coverObj.transform.localPosition = new Vector3(0,0,1);
            coverObj.transform.localRotation = Quaternion.identity;

            coverMeshRender = coverObj.GetComponent<MeshRenderer>();
        }

        if (SkillcoverObj != null)
        {
            SkillcoverObj.transform.AttachTo(transform, false);
            SkillcoverObj.transform.localPosition = new Vector3(0, 0, 1);
            SkillcoverObj.transform.localRotation = Quaternion.identity;

            SkillcoverMeshRender = SkillcoverObj.GetComponent<MeshRenderer>();
            SkillcoverObj.SetActive(false);
        }

        FocusLayerValue = (int)( Mathf.Log( FocusLayer.value ) / Mathf.Log( 2 ) );

        Mycamera = this.GetComponent<Camera>();
		someJS = this.GetComponent<ScreenOverlay> ();
    }

    public void SetSkillEvent(bool type)
    {
        coverObj.SetActive(!type);
        SkillcoverObj.SetActive(type);
    }

    public void ChangeParent(Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        coverObj.transform.parent = this.transform;
        coverObj.transform.localPosition = new Vector3(0,0,1);
        coverObj.transform.localRotation = Quaternion.identity;
    }
    
    public void SetTargetCamera(Camera target)
    {
        if (null == target)
            return;

        camera.CopyFrom( target );
        camera.depth = target.depth + DepthStep;
        camera.cullingMask = FocusLayer;
        camera.clearFlags = CameraClearFlags.Depth;
        // 메인카메라에서 포커싱 관련 Layer는 제외시키기
        //target.cullingMask &= ~FocusLayer.value;

        transform.AttachTo( target.transform );

        TargetCamera = target;

        gameObject.SetActive( false );
    }

    bool StartEffectCheck = false;
    public void StartEffect(float shadeStrength = 0.38f, bool CutSceneEvent = false)
    {
        if (StartEffectCheck)
            return;

        StartEffectCheck = true;

        gameObject.SetActive( true );

        if (CutSceneEvent)
        {
			StartCoroutine(MotionBlur(0.7f));
            StopCoroutine("EndEffectUpdate");
            StopCoroutine("StartEffectUpdate");
            StartCoroutine("StartEffectUpdate");
        }
        else
        {
            alpha = 255;
            CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
        }
    }

    float alpha = 0;
    IEnumerator StartEffectUpdate()
    {
        while(true)
        {
            alpha += 500 * Time.deltaTime;
            if (alpha > 180)
            {
                alpha = 180;
                CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
                break;
            }

            CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
            yield return null;
        }

        yield return null;
    }

    public void EndEffect(bool CutSceneEvent = false)
    {
        if (!StartEffectCheck)
            return;

        StartEffectCheck = false;

        RestoreObjects();

        if (CutSceneEvent)
        {
            StopCoroutine("StartEffectUpdate");
            StopCoroutine("EndEffectUpdate");
            StartCoroutine("EndEffectUpdate");
        }
        else
        {
            alpha = 0;
            CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
            gameObject.SetActive(false);
        }
    }

    //< 다른거 필요없고 무조건 강제적으로 꺼야할때처리
    public void EndUpdate()
    {
        StartEffectCheck = false;
        RestoreObjects();

        StopCoroutine("StartEffectUpdate");
        StopCoroutine("EndEffectUpdate");
        StartCoroutine("EndEffectUpdate");
    }

    IEnumerator EndEffectUpdate()
    {
        while (true)
        {
            alpha -= 400 * Time.deltaTime;
            if (alpha < 0)
            {
                alpha = 0;
                CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
                break;
            }

            CoverMaterial.color = new Color32(255, 255, 255, (byte)alpha);
            yield return null;
        }

        gameObject.SetActive(false);
        yield return null;
    }
    /// <summary>
    /// 포커싱할 객체를 등록한다.
    /// </summary>
    public void AddObject(params GameObject[] focusObjs)
    {
        if (null == focusObjs)
            return;

        for (int i = 0; i < focusObjs.Length; i++)
        {
            GameObject go = focusObjs[i];
            TargetGOList.Add( new FocusingGOData() { SavedLayer = go.layer, Object = go } );

            // 포커싱용 레이어로 수정하기.
            NGUITools.SetChildLayer( go.transform, FocusLayerValue );
        }
    }

    /// <summary>
    /// 포커싱에 사용했던 객체를 원래대로 되돌리도록 한다.
    /// </summary>
    void RestoreObjects()
    {
        foreach (FocusingGOData data in TargetGOList)
        {
            if (null != data.Object)
            {
                // 포커싱용 레이어로 수정하기.
                NGUITools.SetChildLayer( data.Object.transform, data.SavedLayer );
            }
        }

        TargetGOList.Clear();
    }

	IEnumerator MotionBlur(float val)
	{
		this.GetComponent<MotionBlur> ().enabled = true;
		yield return new WaitForSeconds (val);
		this.GetComponent<MotionBlur> ().blurAmount = 0.0f;
		this.GetComponent<MotionBlur> ().enabled = false;
	}
	
	public void EnableCrack(int num)
	{
		if (num < CameraFXs.Length) {
			someJS.texture = CameraFXs [num];
			someJS.enabled = true;
			StartCoroutine (FadeOutCrack (0.5f));
		}
	}

	public void Explosion()
	{
		m_MultiShake = true;

		var rot = new Vector3(2, .5f, 5);
		this.GetComponent<CameraShake>().Shake(this.GetComponent<CameraShake>().shakeType, 2, Vector3.one, rot, 0.25f, 50.0f, 0.30f, 1.0f, true, () => m_MultiShake = false);
		//this.GetComponentInParent<CameraShake>().Shake(this.GetComponentInParent<CameraShake>().shakeType, 5, Vector3.one, rot, 0.25f, 50.0f, 0.20f, 1.0f, true, () => m_MultiShake = false);
		StartCam = GameObject.Find ("StartCamera");
		StartCam.GetComponent<CameraShake>().Shake(StartCam.GetComponent<CameraShake>().shakeType, 2, Vector3.one, rot, 0.25f, 50.0f, 0.30f, 1.0f, true, () => m_MultiShake = false);
	}

	IEnumerator FadeOutCrack(float num)
	{
		yield return new WaitForSeconds(num);
		while(someJS.intensity>0.01f){
		yield return new WaitForSeconds(0.01f);
		someJS.intensity-=0.05f;
		Debug.Log ("ScreenOverlay Intensity:"+someJS.intensity);
		}
		someJS.enabled = false;

	}
	
}
