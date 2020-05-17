using UnityEngine;
using System.Collections;

//동물을 위한 로밍 
public class PosMoveRoamer : MonoBehaviour {

    public Transform[] trans;
    public float fTime;

    LTBezierPath cr;

    void OnEnable() {
        cr = new LTBezierPath(new Vector3[] { trans[0].position, trans[2].position, trans[1].position, trans[3].position, trans[3].position, trans[5].position, trans[4].position, trans[6].position });

    }

    void Start()
    {
        LTDescr descr = LeanTween.move(gameObject, cr.pts, fTime).setOrientToPath(true).setRepeat(-1);
    }


    //private float iter;
    //void Update()
    //{
    //    // Or Update Manually
    //    //cr.place2d( sprite1.transform, iter );

    //    iter += Time.deltaTime * 0.07f;
    //    if (iter > 1.0f)
    //        iter = 0.0f;
    //}
}
