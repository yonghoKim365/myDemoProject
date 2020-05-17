using UnityEngine;
using System.Collections;

public class DontDestroyObject : MonoBehaviour
{
    public bool destroyComponent = true;

    void Awake()
    {
        DontDestroyOnLoad( gameObject );
    }

    void Start()
    {
        if (destroyComponent)
            Destroy( this );
    }
}
