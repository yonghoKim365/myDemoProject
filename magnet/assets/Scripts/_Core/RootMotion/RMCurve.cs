using UnityEngine;
using System.Collections;

[System.Serializable]
public class RMCurve : ScriptableObject 
{
    public string           Name;       // 매치된 애니메이션 이름
    public float            Length;     // 매치된 애니메이션 길이
    public AnimationCurve   XCurve;
    public AnimationCurve   YCurve;
    public AnimationCurve   ZCurve;

    public Vector3 GetOffset(float time)
    {
        return new Vector3( XCurve.Evaluate( time ),
            YCurve.Evaluate( time ),
            ZCurve.Evaluate( time ) );
    }

    public Vector3 CalcTotalDistance()
    {
        return new Vector3(
            XCurve.keys[XCurve.keys.Length - 1].value,
            YCurve.keys[YCurve.keys.Length - 1].value,
            ZCurve.keys[ZCurve.keys.Length - 1].value);
    }
}