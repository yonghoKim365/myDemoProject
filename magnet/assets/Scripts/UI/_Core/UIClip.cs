using UnityEngine;
using System.Collections;

public class UIClip : MonoBehaviour {

    void OnBecameVisible()
    {
        Debug.Log("화면 On");
    }

    void OnBecameInvisible()
    {
        Debug.Log("화면 Off");
    }
}
