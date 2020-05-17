using UnityEngine;
using System.Collections;

public class LimitTimeUI : MonoBehaviour {

    public UILabel LimitTimeLabel;

    Unit owner;
    float height;

    // 화면비율에 따른 위치 보정용
    float screenWScale;

    public void Show(GameObject _target)
    {
        if (null == _target || null == _target.GetComponent<Unit>())
            return;

        screenWScale = (float)((float)1280 / Screen.width);

        owner = _target.GetComponent<Unit>();
        gameObject.SetActive(true);

        height = owner.Height;
    }

    void Update()
    {
        gameObject.SetActive(null != owner && owner.Usable);

        if (!gameObject.activeSelf)
            return;

        Vector3 newPos = owner.transform.position;
        newPos.y += height;

        transform.localPosition = MathHelper.WorldToUIPosition(newPos) * screenWScale;
    }

    public void SetTime(int value)
    {
        LimitTimeLabel.text = value.ToString();
    }
}
