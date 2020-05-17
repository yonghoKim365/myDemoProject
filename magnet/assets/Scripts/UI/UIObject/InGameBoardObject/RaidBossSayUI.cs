using UnityEngine;
using System.Collections;

public class RaidBossSayUI : MonoBehaviour {

    public UILabel sayLabel;
    public UISprite BackSprite;

    public GameObject Owner;
    float Alpha = 255;
    float duration = 1f;

    void Awake()
    {
        fontcolor = sayLabel.color;
    }

    Color32 fontcolor;
    public void Show(GameObject _targetGO, string message, float _duration)
    {
        Owner = _targetGO;
        gameObject.SetActive(true);

        duration = Time.time + _duration;
        sayLabel.text = message;

        // 시작, 끝점 설정
        transform.localPosition = Vector3.zero;

        Alpha = 255;
        fontcolor.a = (byte)Alpha;
        sayLabel.color = fontcolor;
        BackSprite.color = new Color32(0, 0, 0, (byte)Alpha);
    }

    void Update()
    {
        if (null == Owner)
        {
            gameObject.SetActive(false);
            return;
        }

        if(duration < Time.time)
        {
            Alpha -= 350 * Time.deltaTime;
            fontcolor.a = (byte)Alpha;
            sayLabel.color = fontcolor;

            BackSprite.color = new Color32(0, 0, 0, (byte)Alpha);

            if (Alpha <= 0)
                this.gameObject.SetActive(false);
        }
    }


    void OnDisable()
    {
        Owner = null;
    }
}
