using UnityEngine;
using System.Collections;

public class UIChampionshipActionTooltip : MonoBehaviour {

	public UIButton btnRematch;
	public UIButton btnReplay;

	public GameObject go2ButtonContainer;
	public GameObject go1ButtonContainer;


	void Awake()
	{
		UIEventListener.Get(btnRematch.gameObject).onClick = onClickRematch;
		UIEventListener.Get(btnReplay.gameObject).onClick = onClickReplay;
	}

	void onClickRematch(GameObject go)
	{
		GameManager.me.uiManager.popupChampionshipAttack.show();
		GameManager.me.uiManager.popupChampionshipAttack.setData(_data, true, _slotIndex);
	}

	void onClickReplay(GameObject go)
	{
		Debug.Log(_data + " " + _slotIndex);
		switch(_slotIndex)
		{
		case 0: EpiServer.instance.sendGetReplay(isAttackGame,_data.userId, "R0"); break;
		case 1: EpiServer.instance.sendGetReplay(isAttackGame,_data.userId, "R1"); break;
		case 2: EpiServer.instance.sendGetReplay(isAttackGame,_data.userId, "R2"); break;
		}
		hide ();
	}


	Vector3 _v;
	P_Champion _data;
	int _slotIndex;
	bool isAttackGame = false;

	public void show(P_Champion data, Vector3 pos, bool isReplayOnly, int slotIndex, bool isAttack)
	{
		isAttackGame = isAttack;
		_data = data;
		_slotIndex = slotIndex;

		_v = transform.position;
		_v.x = pos.x;
		_v.y = pos.y;
		transform.position = _v;

		gameObject.SetActive(true);

		if(isReplayOnly)
		{
			go1ButtonContainer.SetActive(true);
			go2ButtonContainer.SetActive(false);

			btnRematch.gameObject.SetActive(false);

			_v  = btnReplay.transform.localPosition;
			_v.x = -1;
			btnReplay.transform.localPosition = _v;
		}
		else
		{
			go1ButtonContainer.SetActive(false);
			go2ButtonContainer.SetActive(true);

			btnRematch.gameObject.SetActive(true);

			_v  = btnRematch.transform.localPosition;
			_v.x = -48;
			btnRematch.transform.localPosition = _v;

			_v  = btnReplay.transform.localPosition;
			_v.x = 45;
			btnReplay.transform.localPosition = _v;


			if(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == "T25")
			{
				_v = GameManager.me.uiManager.uiMenu.camera.WorldToScreenPoint(go2ButtonContainer.transform.position);

				_v.x += 70.0f;
				_v.y -= 50.0f;

				GameManager.me.uiManager.uiTutorial.setArrowAndDim( _v.x, _v.y, false);
			}
		}
	}

	public void hide()
	{
		gameObject.SetActive(false);
	}

}
