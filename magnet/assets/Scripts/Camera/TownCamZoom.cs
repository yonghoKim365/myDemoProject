using UnityEngine;
using System.Collections;

public class TownCamZoom : MonoBehaviour {

    public Vector3 MaxLimit = new Vector3(0, 12.66f, 20.11f);
    public Vector3 MinLimit = new Vector3(0, 3.76f, 8.31f);
    public Camera Cam;

    private float Valued;
    float StartDist;

    //public Vector2 moveFactor;

    private float speed = 1f;

    void Start()
    {
        Cam = CameraManager.instance.mainCamera;
        //Resolution rs = Screen.currentResolution;
        //moveFactor.x = rs.width / speed;
        //moveFactor.y = rs.height / speed;
    }

    public void ZoomUpdate(Joystick joy)
    {
        if (Cam == null)
            return;

        if (!joy.IsPress)
        {
#if UNITY_EDITOR
            Valued = Input.GetAxis("Mouse ScrollWheel");
            if (Valued != 0)
                MouseZoom();
#endif
#if UNITY_ANDROID || UNITY_IPHONE
            if (1 < Input.touchCount)
                TouchZoom();
#endif
        }

#if UNITY_ANDROID || UNITY_IPHONE
        if ( (joy.IsPress && 1 < Input.touchCount )|| ( !joy.IsPress && 1 == Input.touchCount) )
            DragRotate(joy);
#endif
    }
    
    void DragRotate(Joystick joy)
    {
        //if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (joy.IsPress)//조이스틱이 터치중인데 다른 터치가 존재한다면
            {
                for(int i=0; i < Input.touchCount; i++)//검사
                {
                    if (Input.GetTouch(i).fingerId == joy.CurrentTouch.fingerId)
                        continue;

                    touch = Input.GetTouch(i);
                    break;
                }
                
            }

            if (touch.phase == TouchPhase.Began)
            {
                //wasRotating = false;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                CameraManager.instance.RtsCamera.Rotation += (touch.deltaPosition.x * speed);
                CameraManager.instance.RtsCamera.Tilt -= (touch.deltaPosition.y * speed);
                //wasRotating = true;
            }
        }
    }

    void TouchZoom()
    {
        if(Input.touches[0].phase == TouchPhase.Began || Input.touches[1].phase == TouchPhase.Began)
        {
            StartDist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            //TouchType = 1;
            //FirstZoom = true;
        }
        else
        {
            /*
            if (FirstZoom)
            {
                FirstZoom = false;
                return;
            }
            */
            float dist = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

            float offset = dist - StartDist;
            if (Mathf.Abs(offset) > 100)
                offset *= 0.01f;
            StartDist = dist;
            Valued = offset * 0.1f;
        }

        CameraManager.instance.RtsCamera.Distance -= Valued;


        //Vector3 pos = Cam.transform.forward * (Valued);
        //pos = Cam.transform.localPosition - pos;

        //pos.x = Mathf.Clamp(pos.x, MinLimit.x, MaxLimit.x);
        //pos.y = Mathf.Clamp(pos.y, MinLimit.y, MaxLimit.y);
        //pos.z = Mathf.Clamp(pos.z, MinLimit.z, MaxLimit.z);

        //Cam.transform.localPosition = pos;
    }

    void MouseZoom()
    {
        CameraManager.instance.RtsCamera.Distance -= (Valued*10f);
        //Vector3 pos = Cam.transform.forward * (Valued*10f);
        //pos = Cam.transform.localPosition - pos;

        //pos.x = Mathf.Clamp(pos.x, MinLimit.x, MaxLimit.x);
        //pos.y = Mathf.Clamp(pos.y, MinLimit.y, MaxLimit.y);
        //pos.z = Mathf.Clamp(pos.z, MinLimit.z, MaxLimit.z);

        //Cam.transform.localPosition = pos;
    }
}
