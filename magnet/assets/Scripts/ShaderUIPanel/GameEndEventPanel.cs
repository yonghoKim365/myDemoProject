using UnityEngine;
using System.Collections;

public class GameEndEventPanel : MonoBehaviour
{

    public MeshRenderer render;
    public Material mat;

    public Shader ShiftHdrshader;
    public Shader MotionBlurShader;

    TiltShiftHdr _TiltShiftHdr;
    MotionBlur _MotionBlur;
    void Awake()
    {
	   //< 시작하자 마자는 꺼준다.
	   render.enabled = false;

        //< UI카메라에 스크립트를 추가해준다
        //if (GameDefine.GamePerformance)
        //{
        //_TiltShiftHdr = UIMgr.instance.UICamera.gameObject.AddComponent<TiltShiftHdr>();
        //_TiltShiftHdr.mode = TiltShiftHdr.TiltShiftMode.IrisMode;
        //_TiltShiftHdr.blurArea = 8;
        //_TiltShiftHdr.maxBlurSize = 0;
        //_TiltShiftHdr.tiltShiftShader = ShiftHdrshader;
        //_TiltShiftHdr.enabled = false;

        //    _MotionBlur = Camera.main.gameObject.AddComponent<MotionBlur>();
        //    _MotionBlur.shader = MotionBlurShader;
        //    _MotionBlur.blurAmount = 0.8f;
        //    _MotionBlur.enabled = false;
        //}
    }

    RtsCamera rtsCamera;
    public void StartEvent(float speed, RtsCamera _rtsCamera)
    {
	   rtsCamera = _rtsCamera;
	   StartCoroutine(PanelUpdate(speed));
    }

    public void SetTargetCamera(Camera target)
    {
        if (target == null)
            return;

	   transform.AttachTo(target.transform);
	   transform.transform.Translate(Vector3.forward * 3, Space.Self);
	   transform.transform.localScale = new Vector3(4.5f, 2.5f, 1);
    }

    float NowValue, NowBlurSize;
    IEnumerator PanelUpdate(float speed)
    {
	   rtsCamera.Distance = 11;
	   rtsCamera.ZoomDampening = 6;

	   if (G_GameInfo.CharacterMgr.allTypeDic.ContainsKey(UnitType.Boss) && G_GameInfo.CharacterMgr.allTypeDic[UnitType.Boss].Count > 0 && G_GameInfo.CharacterMgr.allTypeDic[UnitType.Boss][0] != null)
		  rtsCamera.Follow(G_GameInfo.CharacterMgr.allTypeDic[UnitType.Boss][0].transform);

        //if (GameDefine.GamePerformance && _TiltShiftHdr != null && _MotionBlur != null)
        //{
            //_TiltShiftHdr.enabled = true;
            //_MotionBlur.enabled = true;

        //    while (true)
        //    {
        //        NowValue += speed * Time.deltaTime;
        //        NowBlurSize += (speed * 10) * Time.deltaTime;

        //        _TiltShiftHdr.maxBlurSize = NowBlurSize;

        //        if (NowValue >= 0.4f)
        //            break;

        //        yield return null;
        //    }

        //    render.enabled = false;
        //    speed *= 0.3f;
        //    while (true)
        //    {
        //        NowValue -= speed * Time.deltaTime;
        //        NowBlurSize -= (speed * 10) * Time.deltaTime;

        //        _TiltShiftHdr.maxBlurSize = NowBlurSize;
        //        if (NowValue <= 0)
        //            break;

        //        yield return null;
        //    }


        //    _TiltShiftHdr.enabled = false;
        //    Destroy(_TiltShiftHdr);
        //    //Destroy(_MotionBlur);
        //}

        //else
        {
            while (true)
		  {
			 NowValue += speed * Time.deltaTime;
			 if (NowValue >= 0.4f)
				break;

			 yield return null;
		  }

		  speed *= 0.3f;
		  while (true)
		  {
			 NowValue -= speed * Time.deltaTime;
			 if (NowValue <= 0)
				break;

			 yield return null;
		  }
	   }



	   Destroy(this.gameObject);
    }
}
