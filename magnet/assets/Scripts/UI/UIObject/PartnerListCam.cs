using UnityEngine;
using System.Collections;

public class PartnerListCam : MonoBehaviour {
    
    //public Transform Target;
    public UIDraggableCamera Draggable;
    public UIGrid Grid;

    public float MinX;
    public float MaxX;

    public bool IsStopDrag;

    private bool IsDrag;

    void Start()
    {
        MinX = -10;
        MaxX = (Grid.transform.childCount - 4) * Grid.cellWidth;
        MaxX -= 10;

        Vector2 startPos = Draggable.transform.localPosition;
        startPos.x = MinX;
        Draggable.transform.localPosition = startPos;
    }

    public void Refresh()
    {
        Vector2 startPos = Draggable.transform.localPosition;
        startPos.x = MinX;
        Draggable.transform.localPosition = startPos;

        if (!IsStopDrag)
        {
            int childCount = Grid.transform.childCount;
            int activeCount = 0;
            for (int i = 0; i < childCount; i++)
            {
                if (Grid.transform.GetChild(i).gameObject.activeSelf)
                    ++activeCount;
            }

            if ( 8 < activeCount)
            {
                MaxX = (activeCount - 4) * Grid.cellWidth;
                MaxX -= 10;
            }
        }
    }
    
    void OnPress(bool isPressed)
    {
        if (enabled && NGUITools.GetActive(gameObject) && Draggable != null)
        {
            Draggable.Press(isPressed);
        }

        if (IsDrag && !isPressed)
            IsDrag = false;
        else if(!isPressed)
        {
            if(Draggable != null)
            {
                Vector3 targetPos = Vector3.zero;
                if (Application.isEditor)
                {
                    targetPos = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {
                    if (Input.touchCount == 0)
                        return;

                    targetPos = UICamera.currentCamera.ScreenToWorldPoint(Input.touches[0].position);
                }

                targetPos.x += Draggable.transform.position.x;
                Vector3 hitPos = targetPos;
                int count = Grid.transform.childCount;
                for(int i=0; i < count; i++)
                {
                    Transform tf = Grid.transform.GetChild(i);
                    Vector3 min = tf.collider.bounds.min;//tf.position-(tf.collider.bounds.extents/2);//(*0.002777123f)GetComponent<BoxCollider>().size
                    Vector3 max = tf.collider.bounds.max;//tf.position+(tf.collider.bounds.extents/2);
                    
                    if(min.x <= hitPos.x && hitPos.x <= max.x)// && min.y <= hitPos.y && hitPos.y <= max.y)
                    {
                        UIEventTrigger tri = tf.GetComponent<UIEventTrigger>();
                        if (tri != null)
                        {
                            EventDelegate.Execute(tri.onClick);
                        }
                        break;
                    }
                }

            }
        }
    }
    
    void OnDrag(Vector2 delta)
    {
        IsDrag = true;

        if (IsStopDrag)
            return;

        if (enabled && NGUITools.GetActive(gameObject) && Draggable != null)
        {
            Draggable.Drag(delta);
        }
    }
    
    void OnScroll(float delta)
    {
        if (IsStopDrag)
            return;

        if (enabled && NGUITools.GetActive(gameObject) && Draggable != null)
        {
            Draggable.Scroll(delta);
        }
    }

    void LateUpdate()
    {
        if (IsStopDrag || Draggable == null)
            return;

        Vector3 pos = Draggable.transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, MinX, MaxX);
        Draggable.transform.localPosition = pos;
    }
}
