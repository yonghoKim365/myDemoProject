using UnityEngine;
using System.Collections;

public enum QUALITY
{
    QUALITY_LOW = 2,
    QUALITY_MID = 1,
    QUALITY_HIGH = 0,
}

public class QualityManager : Immortal<QualityManager> {
    public QUALITY _quality;

    public QUALITY GetQuality() { return _quality; }

    public ModelQuality GetModelQuality()
    {
        if (_quality == QUALITY.QUALITY_HIGH)
            return ModelQuality.HIGH;
        else if (_quality == QUALITY.QUALITY_MID)
            return ModelQuality.LOW;
        else if (_quality == QUALITY.QUALITY_LOW)
            return ModelQuality.LOW;

        return ModelQuality.HIGH;
    }

    public void Initialize()
    {
        ////디폴트는 최고 퀄리티
        //int quality = PlayerPrefs.GetInt("Quality", (int)QUALITY.QUALITY_HIGH);
        //if(quality >= (int)QUALITY.QUALITY_HIGH && quality <= (int)QUALITY.QUALITY_LOW)
        //{

        //}
        //else
        //{
        //    quality = 0;
        //}

        //SetQuality((QUALITY)quality);

        //Shader.WarmupAllShaders();
        //화면재생속도 - 일단은 최대로 셋팅하고 해당옵션이 추가될경우 해당옵션에 맞게 수정 - 일단 사용안함

#if UNITY_EDITOR
        SetFrameRate(QUALITY.QUALITY_HIGH);
#else
        SetFrameRate(QUALITY.QUALITY_MID);
#endif
    }

    public void SetFrameRate(QUALITY quality)
    {
        switch (quality)
        {
            case QUALITY.QUALITY_LOW:
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 15;
                }
                break;

            case QUALITY.QUALITY_MID:
                {
                    QualitySettings.vSyncCount = 1;
                    Application.targetFrameRate = 30;
                }
                break;

            case QUALITY.QUALITY_HIGH:
                {
                    QualitySettings.vSyncCount = 1;
                    Application.targetFrameRate = -1;
                }
                break;
        }
    }

    public void SetQuality(QUALITY quality)
    {
        _quality = quality;
        switch (quality)
        {
            case QUALITY.QUALITY_LOW:
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.masterTextureLimit = 1;

                    //동그란 그림자 활성화
                }
                break;

            case QUALITY.QUALITY_MID:
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.masterTextureLimit = 1;

                    //실시간 그림자 켜기
                }
                break;

            case QUALITY.QUALITY_HIGH:
                {
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.masterTextureLimit = 0;

                    //실시간 그림자 켜기//내 캐릭터만 주변광 켜기
                }
                break;
        }
    }
}
