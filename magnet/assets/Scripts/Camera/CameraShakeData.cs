using UnityEngine;
using System.Collections;

public class CameraShakeData : MonoBehaviour
{
    [SerializeField] Thinksquirrel.Utilities.CameraShake.ShakeType m_ShakeType = Thinksquirrel.Utilities.CameraShake.ShakeType.CameraMatrix;

    // numberOfShakes : 흔들리는 기능 횟수
    [SerializeField] int m_NumberOfShakes = 2;
    // shakeAmount : 흔들릴 방향별 강도
    [SerializeField] Vector3 m_ShakeAmount = Vector3.one;
    // rotationAmount : 흔들릴 방향별 강도
    [SerializeField] Vector3 m_RotationAmount = Vector3.one;
    // distance : 첫번째 흔들기의 초기 거리?
    [SerializeField] float m_Distance = 00.10f;
    // speed : 흔들기 속도
    [SerializeField] float m_Speed = 50.00f;
    // decay : 감쇠값 (0 ~ 1)
    [SerializeField] float m_Decay = 00.20f;
    // multiplyByTimeScale : Time.timeScale영향 받는지 유무. false면 1로 고정됨.
    [SerializeField] bool m_MultiplyByTimeScale = true;

    /// <summary>
    /// The type of shake to perform (camera matrix or local position).
    /// </summary>
    public Thinksquirrel.Utilities.CameraShake.ShakeType shakeType { get { return m_ShakeType; } set { m_ShakeType = value; } }
    /// <summary>
    /// The maximum number of shakes to perform.
    /// </summary>
    public int numberOfShakes { get { return m_NumberOfShakes; } set { m_NumberOfShakes = value; } }
    /// <summary>
    /// The amount to shake in each direction.
    /// </summary>
    public Vector3 shakeAmount { get { return m_ShakeAmount; } set { m_ShakeAmount = value; } }
    /// <summary>
    /// The amount to rotate in each direction.
    /// </summary>
    public Vector3 rotationAmount { get { return m_RotationAmount; } set { m_RotationAmount = value; } }
    /// <summary>
    /// The initial distance for the first shake.
    /// </summary>
    public float distance { get { return m_Distance; } set { m_Distance = value; } }
    /// <summary>
    /// The speed multiplier for the shake.
    /// </summary>
    public float speed { get { return m_Speed; } set { m_Speed = value; } }
    /// <summary>
    /// The decay speed (between 0 and 1). Higher values will stop shaking sooner.
    /// </summary>
    public float decay { get { return m_Decay; } set { m_Decay = value; } }
    /// <summary>
    /// If true, multiplies the final shake speed by the time scale.
    /// </summary>
    public bool multiplyByTimeScale { get { return m_MultiplyByTimeScale; } set { m_MultiplyByTimeScale = value; } }
}
