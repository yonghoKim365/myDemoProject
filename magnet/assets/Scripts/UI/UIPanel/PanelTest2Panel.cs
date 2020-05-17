using UnityEngine;
using System.Collections;

public class PanelTest2Panel : MonoBehaviour {


	bool bShow;

	public UIButton btnClose;
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<UiOpenEvent>().setBasePanel(GetComponent<UIPanel>());
		EventDelegate.Set (btnClose.onClick, () => {
			setShow(false);
			UIMgr.OpenTown();
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setShow(bool _bShow){
		bShow = _bShow;
		if (bShow) {
			transform.localPosition = Vector3.zero;
			gameObject.GetComponent<UiOpenEvent>().SetEvent(true);
		} else {
			gameObject.GetComponent<UiOpenEvent>().SetEvent(false, ()=>{
				transform.localPosition = Vector3.one * 10000;
			});
		}
	}
}
