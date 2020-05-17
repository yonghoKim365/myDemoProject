using UnityEngine;
using System.Collections;

public class ModelSimulationPanel : UIBasePanel {

	// Use this for initialization
    public GameObject ModelGO;
    public ModelSimulation _ModelSimulation;
	void Awake () 
    {
        UIEventListener.Get(this.gameObject).onDrag = (go, value) =>
        {
            if (ModelGO == null)
                return;

            //< 모델 이동
            if(Input.GetKey(KeyCode.LeftAlt))
            {
                ModelGO.transform.localPosition -= Vector3.left * (value.x * 0.02f);
                ModelGO.transform.localPosition += Vector3.up * (value.y * 0.02f);

                _ModelSimulation.center = ModelGO.transform.localPosition;
            }
            else
            {
                //< 회전
                ModelGO.transform.Rotate(-(Vector3.up * value.x));
            }
        };
	}
}
